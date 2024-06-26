using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices.ComTypes;
using Google.Protobuf;

namespace Squick
{
	public class ConstDefine
	{
		public static int PACKET_BUFF_SIZE = 65535;
		public static int PACKET_HEAD_SIZE = 6;
		public static int MAX_PACKET_LEN = 1024 * 1024 * 20;
	};

	public class MsgHead
	{
		public MsgHead()
		{
			unMsgID = 0;
			unDataLen = 0;
		}
		public UInt16 unMsgID;
		public UInt32 unDataLen;

		public void Reset()
		{
			unMsgID = 0;
			unDataLen = 0;
			Array.Clear(byHead, 0, ConstDefine.PACKET_HEAD_SIZE);
		}

		private byte[] byHead = new byte[ConstDefine.PACKET_HEAD_SIZE];

		public byte[] GetHeadBytes()
		{
			return byHead;
		}

		public byte[] EnCode()
		{
			byte[] tempByMsgID = BitConverter.GetBytes(unMsgID);
			byte[] tempByDataLen = BitConverter.GetBytes(unDataLen);

			bool isLittle = BitConverter.IsLittleEndian;
			if (isLittle)
			{
				Array.Reverse(tempByMsgID);
				Array.Reverse(tempByDataLen);
			}

			Array.Copy(tempByMsgID, 0, byHead, 0, sizeof(UInt16));
			Array.Copy(tempByDataLen, 0, byHead, sizeof(UInt16), sizeof(UInt32));

			return byHead;
		}

		private byte[] byMsgID = new byte[sizeof(UInt16)];
		private byte[] byDataLen = new byte[sizeof(UInt32)];
		public bool DeCode()
		{
			Array.Clear(byMsgID, 0, sizeof(UInt16));
			Array.Clear(byDataLen, 0, sizeof(UInt16));

			Array.Copy(byHead, 0, byMsgID, 0, sizeof(UInt16));
			Array.Copy(byHead, sizeof(UInt16), byDataLen, 0, sizeof(UInt32));

			bool isLittle = BitConverter.IsLittleEndian;
			if (isLittle)
			{
				Array.Reverse(byMsgID);
				Array.Reverse(byDataLen);
			}

			unMsgID = BitConverter.ToUInt16(byMsgID,0);
			unDataLen = BitConverter.ToUInt32(byDataLen,0);

			return true;
		}
	};

    public class NetListener
    {      
		private StringRingBuffer mPacket = new StringRingBuffer(ConstDefine.MAX_PACKET_LEN, true);

		public delegate void EventDelegation(NetEventType eventType);
		private EventDelegation mHandlerDelegation;
        
		public delegate void MsgDelegation(int id, MemoryStream stream);
		private Dictionary<int, MsgDelegation> mhtMsgDelegation = new Dictionary<int, MsgDelegation>();

		private MsgHead head = new MsgHead();
		private MemoryStream dataReceivedBodyStream = new MemoryStream();

		private StringRingBuffer packet = new StringRingBuffer(ConstDefine.MAX_PACKET_LEN);

		public void OnClientConnect(NetEventParams eventParams)
		{
			mPacket.Clear();

			//LogModule.Instance.Log(LogModule.LOG_LEVEL.DEBUG, "Client connected");
			if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NetEventType.Connected);
            }
        }

		public void OnClientDisconnect(NetEventParams eventParams)
        {
            if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NetEventType.Disconnected);
            }
        }

        public void OnClientConnectionRefused(NetEventParams eventParams)
        {
            if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NetEventType.ConnectionRefused);
            }
        }

		public void OnDataReceived(NetEventParams eventParams)
		{
			if(eventParams.protocolType == RpcProtocolType.TcpSquickRPC)
			{
                mPacket.Push(eventParams.packet.Sb, eventParams.packet.Sb.Size());
                eventParams.packet.Sb.Clear();
            }
            else if(eventParams.protocolType == RpcProtocolType.WebSocketSquickRPC || eventParams.protocolType == RpcProtocolType.WebSocketSecuritySquickRPC)
			{
                mPacket.Push(eventParams.websocketPacket, eventParams.websocketPacket.Length);
                NetClient.PrintBytes("OnDataReceived :231, Recv Bytes: ", eventParams.websocketPacket, eventParams.websocketPacket.Length);
            }
			

            OnDataReceived();
		}

		void OnDataReceived()
		{
			if (mPacket.Size() >= ConstDefine.PACKET_HEAD_SIZE)
			{
				head.Reset();

				if (mPacket.Pop(head.GetHeadBytes(), ConstDefine.PACKET_HEAD_SIZE, true))
                {
					if (head.DeCode())
					{
                        // 当总包大小恰好为得到的数据包大小
                        if (head.unDataLen == mPacket.Size() - ConstDefine.PACKET_HEAD_SIZE)
						{
                            packet.Clear();
                            packet.Push(mPacket, (int)head.unDataLen);

							if (false == OnDataReceived(packet))
							{
								OnClientDisconnect(new NetEventParams());
							}
						}
                        // 处理粘包
                        else if (mPacket.Size() > head.unDataLen - ConstDefine.PACKET_HEAD_SIZE)
						{
                            packet.Clear();
                            packet.Push(mPacket, (int)head.unDataLen);

							if (false == OnDataReceived(packet))
							{
								OnClientDisconnect(new NetEventParams());
							}else
							{
								// 如果正常处理，尝试再次去Pop包
                                OnDataReceived();
                            }
						}else
						{
							// 处理分包，积累大小到一定程度通过以上两个条件进行处理
						}
					}
				}
			}
		}
		
		bool OnDataReceived(StringRingBuffer sb)
		{
			head.Reset();
			if (sb.Pop(head.GetHeadBytes(), ConstDefine.PACKET_HEAD_SIZE))
            {
				head.DeCode();
                if (head.unDataLen == sb.Size() + ConstDefine.PACKET_HEAD_SIZE)
				{
					Int32 nBodyLen = (Int32)sb.Size();
					if (nBodyLen >= 0)
					{
						dataReceivedBodyStream.SetLength(0);
						dataReceivedBodyStream.Position = 0;
						sb.ToMemoryStream(dataReceivedBodyStream);
                        OnMessageEvent(head, dataReceivedBodyStream);
						return true;
					}
					else
					{
						// space packet, thats impossible
					}
                
                }
			}
			return false;
		}

		private void OnMessageEvent(MsgHead head, MemoryStream ms)
        {
            if (mhtMsgDelegation.ContainsKey(head.unMsgID))
            {
                MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[head.unMsgID];
				ms.Position = 0;
                myDelegationHandler(head.unMsgID, ms);
            }
            else
            {
				Debug.LogWarning("ReciveMsg:" + head.unMsgID + "  and no handler!!!!");
            }
        }

		public bool RegisteredNetEventHandler(EventDelegation eventHandler)
        {
            mHandlerDelegation += eventHandler;
            return true;
        }

		public bool RegisteredDelegation(int eMsg, MsgDelegation msgDelegate)
		{
			if(!mhtMsgDelegation.ContainsKey(eMsg))
			{
				MsgDelegation myDelegationHandler = new MsgDelegation(msgDelegate);
				mhtMsgDelegation.Add(eMsg, myDelegationHandler);
			}
			else
			{
				MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[eMsg];
				myDelegationHandler += new MsgDelegation(msgDelegate);
                mhtMsgDelegation[eMsg] = myDelegationHandler;
            }

			return true;
		}

        public void RemoveDelegation(int eMsg)
		{
			mhtMsgDelegation.Remove(eMsg);
		}

	}
}
