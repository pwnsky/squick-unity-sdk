using System;
using System.IO;
using Google.Protobuf;
using UnityEngine;
using Rpc;

namespace Squick
{
	public partial class NetModule : IModule
    {
		private NetListener mNetListener;
		private NetClient mNetClient;
        ISEventModule mEvent;
        //sender
		private MemoryStream mxBody = new MemoryStream();
		private MsgHead mxHead = new MsgHead();
        private byte[] sendBytes = new byte[ConstDefine.PACKET_BUFF_SIZE];

        private string accountID;
        private string playerID;
        private string key;
        private long lastHeartBeatTime = 0;
        private bool isProxyAuthed = false;
        private int heartBeatIndex = 0;

        public enum ProxyEvent
        {
            CONNECTED,
            CONNECTE_TIMEOUT,
            DISCONNECTED,
            AUTH_SUCCESS,
            AUTH_FAILED,
        };

        public NetModule(IPluginManager pluginManager)
        {
            mNetListener = new NetListener();
            mPluginManager = pluginManager;
        }
        
        public override void Awake()
        {
            mEvent = mPluginManager.FindModule<ISEventModule>();
            // 监听网络事件
            AddNetEventCallBack(NetEventDelegation);
        }

		public override void Start()
		{

        }

        public override void Update(long time)
        {
            
			if (null != mNetClient)
			{
				mNetClient.Execute();
			}

            if(time - lastHeartBeatTime > 5000 && isProxyAuthed)
            {
                lastHeartBeatTime = time;
                OnReqHeartBeat();
            }
        }

        public override void BeforeDestroy()
        {
			if (null != mNetClient)
            {
                mNetClient.Disconnect();
            }
        }

        public override void Destroy()
        {
			mNetClient = null;
		}

		public override void AfterStart()
		{
            mEvent.BindEvent("net", (int)NetEventType.Connected, OnConnected);
            AddReceiveCallBack((int)ProxyRPC.AckConnectProxy, OnAckConnectProxy);
            AddReceiveCallBack((int)ProxyRPC.AckHeartbeat, OnAckHeatBeat);
        }

        public bool Connect(string ip, int port, string key, string accountID, RpcProtocolType rpcProtocolType)
        {
            this.key = key;
            this.accountID = accountID;
            Debug.Log(Time.realtimeSinceStartup.ToString() + " StartConnect " + ip + " " + port.ToString());
            mNetClient = new NetClient(mNetListener);
            mNetClient.Connect(ip, port, rpcProtocolType);
            return true;
        }

        public void OnConnected()
        {
            // 通过秘钥连接
            ReqConnectProxy req = new ReqConnectProxy();
            req.Key = ByteString.CopyFromUtf8(key);
            req.AccountId = ByteString.CopyFromUtf8(accountID);
            SendMsg((int)Rpc.ProxyRPC.ReqConnectProxy, req.ToByteString());
            mEvent.DoEvent("proxy", (int)ProxyEvent.CONNECTED);
            lastHeartBeatTime = DateTime.Now.Ticks / 10000;
        }

        void OnAckConnectProxy(int id, MemoryStream ms)
        {
            AckConnectProxy ack = AckConnectProxy.Parser.ParseFrom(ms);
            if (ack.Code == 0)
            {
                isProxyAuthed = true;
                mEvent.DoEvent("proxy", (int)ProxyEvent.AUTH_SUCCESS);
            }
            else
            {
                isProxyAuthed = false;
                mEvent.DoEvent("proxy", (int)ProxyEvent.AUTH_FAILED);
            }
        }

        //发送心跳
        public void OnReqHeartBeat()
        {
            Debug.Log("HeartBeat");
            ReqHeartBeat req = new ReqHeartBeat();
            req.Index = heartBeatIndex;
            heartBeatIndex++;
            SendMsg((int)Rpc.ProxyRPC.ReqHeartbeat, req.ToByteString());
        }
        public void OnAckHeatBeat(int id, MemoryStream ms)
        {
            AckHeartBeat ack = AckHeartBeat.Parser.ParseFrom(ms);
            Debug.Log("Ack: HeartBeat " + ack.Index);
        }

        public void DisconnectFromServer()
        {
            mNetClient.Disconnect();
        }

        public NetState GetState()
        {
            return mNetClient.GetState();
        }

		public void AddReceiveCallBack(int eMsg, Squick.NetListener.MsgDelegation netHandler)
        {
            mNetListener.RegisteredDelegation(eMsg, netHandler);
        }
  
		public void AddNetEventCallBack(Squick.NetListener.EventDelegation netHandler)
        {
			mNetListener.RegisteredNetEventHandler(netHandler);
        }

        public void SendMsg(int unMsgID, ByteString data)
        {
            if (mNetClient != null)
            {
                mxBody.SetLength(0);
                data.WriteTo(mxBody);

                mxHead.unMsgID = (UInt16)unMsgID;
                mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.PACKET_HEAD_SIZE;

                byte[] bodyByte = mxBody.ToArray();
                byte[] headByte = mxHead.EnCode();

                Array.Clear(sendBytes, 0, ConstDefine.PACKET_BUFF_SIZE);
                headByte.CopyTo(sendBytes, 0);
                bodyByte.CopyTo(sendBytes, headByte.Length);
                Debug.Log(" MsgID: " + unMsgID);
                mNetClient.SendBytes(sendBytes, bodyByte.Length + headByte.Length);
            }
        }


        private void NetEventDelegation(NetEventType eventType)
        {
            Debug.Log(Time.realtimeSinceStartup.ToString() + " Event " + eventType.ToString());
            mEvent.DoEvent("net", (int)eventType);
        }

        static public Rpc.Vector2 NFToPB(SVector2 value)
        {
            Rpc.Vector2 vector = new Rpc.Vector2();
            vector.X = value.X();
            vector.Y = value.Y();
            return vector;
        }
        static public SVector2 PBToNF(Rpc.Vector2 xVector)
        {
            SVector2 xData = new SVector2(xVector.X, xVector.Y);

            return xData;
        }

        static public Rpc.Vector3 NFToPB(SVector3 value)
        {
            Rpc.Vector3 vector = new Rpc.Vector3();
            vector.X = value.X();
            vector.Y = value.Y();
            vector.Z = value.Z();
            return vector;
        }

        static public Squick.SVector3 PBToNF(Rpc.Vector3 xVector)
        {
            SVector3 xData = new SVector3(xVector.X, xVector.Y, xVector.Z);
            return xData;
        }
    }
}