using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using NativeWebSocket;
using System.Linq.Expressions;
using Google.Protobuf;
using System.Linq;

namespace Squick {

    public enum RpcProtocolType
    {
        None,
        TcpSquickRPC,
        WebSocketSquickRPC,
        WebSocketSecuritySquickRPC,
    }

    public enum NetState
    {
        Connecting,
        Connected,
        Disconnected
    }

	public enum NetEventType
    {
        None,
        Connected,
        Disconnected,
        ConnectionRefused,
        DataReceived
    }

    public class SocketPacket
    {
        public SocketPacket()
        {
            sb = new StringRingBuffer(ConstDefine.PACKET_BUFF_SIZE);
        }

        private StringRingBuffer sb;
        internal StringRingBuffer Sb { get => sb; set => sb = value; }

        public void Reset()
        {
            sb.Clear();
        }

        public void FromBytes(byte[] by, int bytesCount)
        {
            sb.Clear();
            sb.Push(by, bytesCount);
        }
    }

    public class NetEventParams
    {
        public void Reset()
        {
             client = null;
             clientID = 0;
             socket = null;
             eventType = NetEventType.None;
             message = "";
             packet = null;
            protocolType = RpcProtocolType.None;
        }

        public NetClient client = null;
        public int clientID = 0;
        public TcpClient socket = null;
        public NetEventType eventType = NetEventType.None;
        public string message = "";
        public SocketPacket packet = null;
        public RpcProtocolType protocolType = RpcProtocolType.None;
        public byte[] websocketPacket;

    }
	
    public class NetClient
    {
        private string m_hostname;
        private int m_port;
        public static void PrintBytes(string info, byte[] bytesData, int len)
        {
            string data = "";
            int id = bytesData[0] * 15 + bytesData[1];
            for (int i = 0; i < len; i++)
            {
                data += bytesData[0 + i].ToString("X02") + ","; // .toString("x2"); 
            }
            //string prev; // = "NetDebug: server " + m_hostname + ":" + m_port + " ";
            Debug.Log(info + "  [" + data.ToString() + " ]" + " length: " + len);
        }
		public NetClient(NetListener xNetListener)
        {
			mxNetListener = xNetListener;
            Init();
        }

        void Init()
        {

            mxState = NetState.Disconnected;
            mxEvents = new Queue<NetEventType>();
            mxMessages = new Queue<string>();
            mxPackets = new Queue<SocketPacket>();
            mxPacketPool = new Queue<SocketPacket>();
            mxWebsocketPackets = new Queue<byte[]>();
        }

        private NetState mxState;
        private NetworkStream mxStream;
        private StreamWriter mxWriter;
        private StreamReader mxReader;
        private Thread mxReadThread;
        private RpcProtocolType protocolType;
        private TcpClient mxClient;
        private WebSocket mxWebsocket;
        private Queue<NetEventType> mxEvents;
        private Queue<string> mxMessages;
        private Queue<SocketPacket> mxPackets;
        private Queue<SocketPacket> mxPacketPool;
        private Queue<byte[]> mxWebsocketPackets;

        private NetListener mxNetListener;

        private byte[] tempReadBytes = new byte[ConstDefine.PACKET_BUFF_SIZE];

        private NetEventParams eventParams = new NetEventParams();

        public bool IsConnected()
        {
            return mxState == NetState.Connected;
        }

        public NetState GetState()
        {
            return mxState;
        }

        public NetListener GetNetListener()
        {
            return mxNetListener;
        }

        public void Execute()
        {

#if !UNITY_WEBGL || UNITY_EDITOR
            if(mxWebsocket != null)
            {
                mxWebsocket.DispatchMessageQueue();
            }
#endif

            while (mxEvents.Count > 0)
            {
                
                lock (mxEvents)
                {
                    if (mxEvents.Count > 0)
                    {
                        
                        NetEventType eventType = mxEvents.Dequeue();
                        Debug.Log("asdfjkasdfjasdf   " + eventType.ToString());
                        eventParams.Reset();
                        eventParams.eventType = eventType;
                        eventParams.client = this;
                        eventParams.socket = mxClient;
                        eventParams.protocolType = protocolType;

                        if (eventType == NetEventType.Connected)
                        {
                            mxNetListener.OnClientConnect(eventParams);
                        }
                        else if (eventType == NetEventType.Disconnected)
                        {
                            mxNetListener.OnClientDisconnect(eventParams);

                            
                            //mxReader.Close();
                            //mxWriter.Close();
                            //mxClient.Close();

                        }
                        else if (eventType == NetEventType.DataReceived)
                        {
                            if (protocolType == RpcProtocolType.TcpSquickRPC)
                            {
                                lock (mxPackets)
                                {
                                    if (mxPackets.Count > 0)
                                    {
                                        eventParams.packet = mxPackets.Dequeue();
                                        mxNetListener.OnDataReceived(eventParams);
                                        mxPacketPool.Enqueue(eventParams.packet);
                                    }
                                }
                            } else if (protocolType == RpcProtocolType.WebSocketSquickRPC)
                            {
                                
                                lock (mxWebsocketPackets)
                                {
                                    if (mxWebsocketPackets.Count > 0)
                                    {
                                        eventParams.websocketPacket = mxWebsocketPackets.Dequeue();
                                        mxNetListener.OnDataReceived(eventParams);
                                    }
                                }
                            }

                        }
                        else if (eventType == NetEventType.ConnectionRefused)
                        {

                        }
                    }
                }
            }
        }

        private void OnWebsocketOpen()
        {
            Debug.Log("On websocket open");
            mxState = NetState.Connected;
            mxEvents.Enqueue(NetEventType.Connected);
        }

        private void OnWebsocketError(string e)
        {
            Debug.Log("On websocket error: " + e.ToString());
        }

        private void OnWebsocketClose(WebSocketCloseCode e)
        {
            Debug.Log("On websocket close");
        }

        private void OnWebsocketMessage(byte[] data)
        {
            Debug.Log("On websocket message");
            lock (mxWebsocketPackets)
            {
                mxWebsocketPackets.Enqueue(data);
            }
            lock (mxEvents)
            {
                mxEvents.Enqueue(NetEventType.DataReceived);
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            Debug.Log("Tcp connected!");
            try
            {
                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.EndConnect(ar);

                SetTcpClient(tcpClient);

            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NetEventType.ConnectionRefused);
                }
            }
        }


        private SocketPacket GetPacketFromPool()
        {
            if (mxPacketPool.Count <= 0)
            {
                mxPacketPool.Enqueue(new SocketPacket());
            }

            SocketPacket packet = mxPacketPool.Dequeue();
            packet.Reset();

            return packet;
        }
        private void ReadData()
        {
            bool endOfStream = false;

            while (!endOfStream)
            {
               int bytesRead = 0;
               try
               {
                    Array.Clear(tempReadBytes, 0, ConstDefine.PACKET_BUFF_SIZE);
                    bytesRead = mxStream.Read(tempReadBytes, 0, ConstDefine.PACKET_BUFF_SIZE);

                }
               catch (Exception e)
               {
                   e.ToString();
               }

               if (bytesRead == 0)
               {
                   endOfStream = true;
               }
               else // 大于0
               {
                   lock (mxEvents)
                   {
                       mxEvents.Enqueue(NetEventType.DataReceived);
                   }
                   lock (mxPackets)
                   {
                        SocketPacket packet = GetPacketFromPool();
                        packet.FromBytes(tempReadBytes, bytesRead);
                        mxPackets.Enqueue(packet);
                   }
               }
            }

            mxState = NetState.Disconnected;
            mxClient.Close();
            lock (mxEvents)
            {
                mxEvents.Enqueue(NetEventType.Disconnected);
            }

        }

        // Public
        public void Connect(string hostname, int port, RpcProtocolType rpcProtocolType)
        {
            Debug.Log("connecting: " + hostname + ":" + port + " rpc type: " + rpcProtocolType);
            if (mxState == NetState.Connected)
            {
                return;
            }
            
            mxState = NetState.Connecting;

            mxMessages.Clear();
            mxEvents.Clear();
            m_hostname = hostname;
            m_port = port;
            protocolType = rpcProtocolType;
            switch (protocolType)
            {
                case RpcProtocolType.TcpSquickRPC:
                    {
                        mxClient = new TcpClient();
                        mxClient.NoDelay = true;
                        mxClient.BeginConnect(hostname,
                                             port,
                                             new AsyncCallback(ConnectCallback),
                                             mxClient);
                    }
                    break;
                case RpcProtocolType.WebSocketSquickRPC:
                    {
                        string webSocketURL = "ws://" + hostname + ":" + port;
                        Debug.Log("Websocket connect: " + webSocketURL);
                        mxWebsocket = new WebSocket(webSocketURL);
                    }
                    break;
                case RpcProtocolType.WebSocketSecuritySquickRPC:
                    {
                        port = 8888; // for test
                        string webSocketURL = "wss://" + hostname + ":" + port;
                        mxWebsocket = new WebSocket(webSocketURL);
                    }
                    break;
                
                default:
                    Debug.LogError("Not surrport this protocol type: " + protocolType);
                    break;
            }

            if (mxWebsocket != null)
            {
                mxWebsocket.OnOpen += OnWebsocketOpen;
                mxWebsocket.OnError += OnWebsocketError;
                mxWebsocket.OnClose += OnWebsocketClose;
                mxWebsocket.OnMessage += OnWebsocketMessage;
                WebSocketConnect();
            }

        }
        public async void WebSocketConnect()
        {
            await mxWebsocket.Connect();
        }

        public async void Disconnect()
        {
            mxState = NetState.Disconnected;

            try { if (mxReader != null) mxReader.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxWriter != null) mxWriter.Close(); }
            catch (Exception e) { e.ToString(); }
            switch(protocolType)
            {
                case RpcProtocolType.TcpSquickRPC:
                    {
                        try { if (mxClient != null) mxClient.Close(); }
                        catch (Exception e) { e.ToString(); }
                    }
                    break;
                case RpcProtocolType.WebSocketSquickRPC:
                    {
                        try { if (mxWebsocket != null)  await mxWebsocket.Close(); }
                        catch (Exception e) { e.ToString(); }
                    }
                    break;
                case RpcProtocolType.WebSocketSecuritySquickRPC:
                    {
                        try { if (mxWebsocket != null) await mxWebsocket.Close(); }
                        catch (Exception e) { e.ToString(); }
                    }
                    break;

            }
            
        }

        public void SendBytes(byte[] bytes, int length)
        {
            PrintBytes( "NetClient.cs:375, Send Bytes: ", bytes, length);
            if (!IsConnected())
                return;

            switch (protocolType)
            {
                case RpcProtocolType.TcpSquickRPC:
                    {
                        try
                        {
                            mxStream.Write(bytes, 0, length);
                            mxStream.Flush();
                        }
                        catch (Exception e)
                        {
                            lock (mxEvents)
                            {
                                mxEvents.Enqueue(NetEventType.Disconnected);
                                Disconnect();
                            }
                        }
                    }
                    break;
                case RpcProtocolType.WebSocketSquickRPC:
                    {
                        try
                        {
                            if (mxWebsocket != null)
                            {
                                byte[] webSocketSend = new byte[length];
                                Array.Copy(bytes, 0, webSocketSend, 0, length);
                                mxWebsocket.Send(webSocketSend);
                            }
                        }
                        catch (Exception e)
                        {
                            lock (mxEvents)
                            {
                                mxEvents.Enqueue(NetEventType.Disconnected);
                                Disconnect();
                            }
                        }
                    }
                    break;
                case RpcProtocolType.WebSocketSecuritySquickRPC:
                    {
                        try
                        {
                            if (mxWebsocket != null)
                            {
                                byte[] webSocketSend = new byte[length];
                                Array.Copy(bytes, 0, webSocketSend, 0, length);
                                mxWebsocket.Send(webSocketSend);
                            }
                        }
                        catch (Exception e)
                        {
                            lock (mxEvents)
                            {
                                mxEvents.Enqueue(NetEventType.Disconnected);
                                Disconnect();
                            }
                        }
                            
                    }
                    break;
            }
            
        }

        private void SetTcpClient(TcpClient tcpClient)
        {
            mxClient = tcpClient;

            if (mxClient.Connected)
            {
                mxStream = mxClient.GetStream();
                mxReader = new StreamReader(mxStream);
                mxWriter = new StreamWriter(mxStream);

                mxState = NetState.Connected;

                mxEvents.Enqueue(NetEventType.Connected);

                mxReadThread = new Thread(ReadData);
                mxReadThread.IsBackground = true;
                mxReadThread.Start();
            }
            else
            {
                mxState = NetState.Disconnected;
            }
        }
    }
}