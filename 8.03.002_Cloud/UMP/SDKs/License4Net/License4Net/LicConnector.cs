//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a912b108-32c1-4dae-a669-b3553da6ceba
//        CLR Version:              4.0.30319.18063
//        Name:                     LicConnector
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                LicConnector
//
//        created by Charley at 2015/7/27 10:44:25
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// License服务的连接器，建立和维护与License服务器的连接
    /// </summary>
    public class LicConnector
    {

        #region Members

        private string mClient;
        private string mHost;
        private int mPort;
        private bool mIsConnected;
        private bool mIsLogined;
        private TcpClient mTcpClient;
        private Stream mNetStream;
        private int mHeadSize;
        private int mBufferedSize;
        private byte[] mReceiveBuffer;
        private byte[] mBufferedData;
        private IEncryptable mEncryptObject;
        private bool mIsMsgEncrypt;
        private int mHeartbeatNum;
        private Thread mThreadHeartbeat;
        private int mReconnectNum;
        private Thread mThreadReconnect;
        private int mModuleNumber;
        private int mModuleTypeID;
        private string mProtocolVersion;

        #endregion


        /// <summary>
        /// 创建一个License连接器
        /// </summary>
        public LicConnector()
        {
            mClient = "LicConnector";
            mHost = string.Empty;
            mPort = 3071;
            mIsConnected = false;
            mIsLogined = false;
            mIsMsgEncrypt = false;
            mHeartbeatNum = 30;     //默认心跳时间30s
            mReconnectNum = 10;     //默认重连时间60s
            mModuleNumber = 0;
            mModuleTypeID = 0;
            mProtocolVersion = "2.00";

            mReceiveBuffer = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
            mBufferedData = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
            mHeadSize = LicDefines.NET_PACKET_HEADER_SIZE;
            mBufferedSize = 0;
        }


        #region Public Members


        #region Client

        /// <summary>
        /// 获取或设置用户自定义的名称
        /// </summary>
        public string Client
        {
            get { return mClient; }
            set { mClient = value; }
        }

        #endregion


        #region Host Address
        /// <summary>
        /// 获取或设置服务器地址
        /// </summary>
        public string Host
        {
            get { return mHost; }
            set { mHost = value; }
        }

        #endregion


        #region Host Port
        /// <summary>
        /// 获取或设置服务器端口
        /// </summary>
        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        #endregion


        #region IsConnected
        /// <summary>
        /// 获取当前连接状况
        /// </summary>
        public bool IsConnected
        {
            get { return mIsConnected; }
        }

        #endregion


        #region IsLogined

        public bool IsLogined
        {
            get { return mIsLogined; }
        }

        #endregion


        #region EncryptObject

        public IEncryptable EncryptObject
        {
            set { mEncryptObject = value; }
        }

        #endregion


        #region IsMsgEncrypt

        public bool IsMsgEncrypt
        {
            get { return mIsMsgEncrypt; }
        }

        #endregion


        #region ModuleNumber

        public int ModuleNumber
        {
            get { return mModuleNumber; }
            set { mModuleNumber = value; }
        }

        #endregion


        #region ModuleTypeID

        public int ModuleTypeID
        {
            get { return mModuleTypeID; }
            set { mModuleTypeID = value; }
        }

        #endregion


        #region ProtocolVersion

        public string ProtocolVersion
        {
            get { return mProtocolVersion; }
            set { mProtocolVersion = value; }
        }

        #endregion


        #endregion


        #region Public Functions

        /// <summary>
        /// 连接上服务器
        /// </summary>
        public void Connect()
        {
            try
            {
                if (mIsConnected)
                {
                    //关闭，重新连接
                    Close();
                }
                mIsConnected = false;
                mIsLogined = false;
                mTcpClient = new TcpClient();
                mTcpClient.Connect(mHost, mPort);
                if (mTcpClient.Connected)
                {
                    SslStream sslStream = new SslStream(mTcpClient.GetStream(), false, (s, cert, chain, err) => true);
                    //sslStream.AuthenticateAsClient(mHost, null, SslProtocols.Tls11, false);
                    sslStream.AuthenticateAsClient(mHost);
                    mNetStream = sslStream;

                    mIsConnected = true;
                    OnDebug(LogMode.Info,
                        string.Format("Server connected.\tRemote:{0}\tLocal:{1}\tProtocol:{2}",
                            mTcpClient.Client.RemoteEndPoint,
                            mTcpClient.Client.LocalEndPoint, sslStream.SslProtocol));

                    mNetStream.BeginRead(mReceiveBuffer, 0, mHeadSize, ReceiveMessageWorker,
                      mNetStream);

                    //触发ServerConnectedEvent事件
                    OnServerConnectionEvent(Defines.EVT_NET_CONNECTED, string.Format("Server connected"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Connect fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 开始连接服务，并在一段时间后进行重连
        /// </summary>
        public void BeginConnect()
        {
            CreateReconnectThread();
        }
        /// <summary>
        /// 关闭连接器，释放资源
        /// </summary>
        public void Close()
        {
            try
            {
                mIsLogined = false;
                mIsConnected = false;
                StopHeartbeatThread();
                StopReconnectThread();
                if (mNetStream != null)
                {
                    try
                    {
                        mNetStream.Close();
                        mNetStream.Dispose();
                        mNetStream = null;
                    }
                    catch { }
                }
                if (mTcpClient != null)
                {
                    try
                    {
                        mTcpClient.Close();
                        mTcpClient = null;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Close connector fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SendMessage

        public void SendMessage(string msg)
        {
            NetPacketHeader header = new NetPacketHeader();
            header.Flag = new[] { (byte)'L', (byte)'M' };
            header.Format = LicDefines.DH_FORMAT_JSON;
            header.State = (ushort)(mIsMsgEncrypt ? 4 : 0);
            SendMessage(header, msg);
        }

        public void SendMessage(NetPacketHeader header, string msg)
        {
            OnDebug(LogMode.Debug, string.Format("Send:\t{0}", msg));
            SendMessage(header, Encoding.ASCII.GetBytes(msg));
        }

        public void SendMessage(NetPacketHeader header, byte[] data)
        {
            try
            {
                bool isEncrypt = (header.State & 4) > 0;
                if (isEncrypt
                    && mEncryptObject != null)
                {
                    data = mEncryptObject.EncryptBytes(data, (int)EncryptionMode.AES256V13Hex);
                }
                header.Size = (uint)data.Length;
                if (!mIsConnected
                    || mNetStream == null)
                {
                    OnDebug(LogMode.Error, string.Format("Server not connected or stream is null"));
                    return;
                }
                byte[] byteHeader = Converter.Struct2Bytes(header);
                mNetStream.Write(byteHeader, 0, byteHeader.Length);
                mNetStream.Write(data, 0, data.Length);
                mNetStream.Flush();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region ReceivedMessage

        private void ReceiveMessageWorker(IAsyncResult result)
        {
            //先接收消息头
            var stream = result.AsyncState as Stream;
            if (stream == null) { return; }
            int intSize;
            try
            {
                intSize = stream.EndRead(result);
                if (intSize <= 0)
                {
                    OnDebug(LogMode.Error, string.Format("Recieve 0 length message."));
                    mIsConnected = false;
                    mIsLogined = false;
                    OnServerConnectionEvent(Defines.EVT_NET_DISCONNECTED, string.Format("Recieve 0 length message"));
                    return;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EndReceive fail.\t{0}", ex.Message));
                mIsConnected = false;
                mIsLogined = false;
                OnServerConnectionEvent(Defines.EVT_NET_DISCONNECTED, string.Format("EndReceive fail"));
                return;
            }
            try
            {
                //OnDebug(LogMode.Debug, string.Format("Receive:{0}Byte", intSize));
                Array.Copy(mReceiveBuffer, 0, mBufferedData, mBufferedSize, intSize);
                mBufferedSize += intSize;
                if (mBufferedSize >= mHeadSize)
                {
                    //实际上只有可能等于，因为每次接收都是指定大小的

                    //取出数据
                    byte[] temp = new byte[mHeadSize];
                    Array.Copy(mBufferedData, 0, temp, 0, mHeadSize);
                    //重置缓存区
                    mBufferedSize = 0;
                    mBufferedData.Initialize();

                    //得到消息头，根据Size大小接收数据区部分
                    var msgHead = (NetPacketHeader)Converter.Bytes2Struct(temp, typeof(NetPacketHeader));
                    int dataSize = (int)msgHead.Size;
                    //OnDebug(LogMode.Debug, string.Format("MsgHead\tDataSize:{0}", dataSize));

                    //循环，直到接收完DataSize大小
                    int receiveSize = 0;
                    while (receiveSize < dataSize)
                    {
                        intSize = stream.Read(mReceiveBuffer, receiveSize, dataSize - receiveSize);
                        receiveSize += intSize;
                        Thread.Sleep(1);
                    }
                    if (receiveSize != dataSize)
                    {
                        OnDebug(LogMode.Error, string.Format("DataSize invalid"));
                        mIsConnected = false;
                        mIsLogined = false;
                        return;
                    }

                    //取出数据区数据
                    temp = new byte[dataSize];
                    Array.Copy(mReceiveBuffer, 0, temp, 0, temp.Length);

                    MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                    args.Client = mClient;
                    args.Header = msgHead;
                    args.DataSize = dataSize;
                    if ((msgHead.State & 4) > 0)
                    {
                        //加密的，先解密
                        if (mEncryptObject != null)
                        {
                            temp = mEncryptObject.DecryptBytes(temp, (int) EncryptionMode.AES256V13Hex);
                        }
                    }
                    args.Data = temp;
                    string strValue = Encoding.ASCII.GetString(temp);
                    strValue = strValue.TrimEnd('\0','\r', '\n');
                    OnDebug(LogMode.Debug, string.Format("Recv:\t{0}", strValue));
                    args.StringData = strValue;
                    DealMessage(msgHead, strValue);
                    OnMessageReceivedEvent(args);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealMessage fail.\t{0}", ex.Message));
            }
            try
            {
                stream.BeginRead(mReceiveBuffer, 0, mHeadSize - mBufferedSize, ReceiveMessageWorker, stream);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("BeginReceive fail.\t{0}", ex.Message));
                mIsConnected = false;
                mIsLogined = false;
                OnServerConnectionEvent(Defines.EVT_NET_DISCONNECTED, string.Format("BeginReceive fail"));
            }
        }

        #endregion


        #region DealMessage

        private void DealMessage(NetPacketHeader header, string strMsg)
        {
            try
            {
                JsonObject json = new JsonObject(strMsg);
                int classid = (int)json[LicDefines.KEYWORD_MSG_COMMON_CLASSID].Number;
                int messageid = (int)json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID].Number;
                JsonObject jsonReturn;
                string strSession;
                string strVerify;
                switch (classid)
                {
                    case LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE:
                        switch (messageid)
                        {
                            case LicDefines.LICENSE_MSG_AUTH_WELCOME:
                                bool isEncrypt = (header.State & 4) > 0;
                                mIsMsgEncrypt = isEncrypt;
                                strSession =
                                    json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_SESSION]
                                        .Value;
                                OnDebug(LogMode.Info, string.Format("Session code:\t{0}", strSession));
                                strVerify = LicUtils.GetVerificationCode(strSession);
                                OnDebug(LogMode.Info, string.Format("Verification code:\t{0}", strVerify));
                                jsonReturn = new JsonObject();
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_CLASSID] =
                                    new JsonProperty(string.Format("{0}", LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_CLASSDESC] =
                                    new JsonProperty(string.Format("\"{0}\"",
                                        LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE)));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID] =
                                    new JsonProperty(string.Format("{0}", LicDefines.LICENSE_MSG_AUTH_LOGON));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_MESSAGEDESC] =
                                    new JsonProperty(string.Format("\"{0}\"",
                                        LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE,
                                            LicDefines.LICENSE_MSG_AUTH_LOGON)));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_CURRENTTIME] =
                                    new JsonProperty(string.Format("\"{0}\"",
                                        DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA] = new JsonProperty(new JsonObject());
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_CONNTION_HEARTBEAT
                                    ] = new JsonProperty(string.Format("{0}", mHeartbeatNum));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_MODULENAME
                                    ] = new JsonProperty(string.Format("\"{0}\"", mClient));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_MODULENUMBER
                                   ] = new JsonProperty(string.Format("{0}", mModuleNumber));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_MODULETYPEID
                                  ] = new JsonProperty(string.Format("{0}", mModuleTypeID));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_AUTH_PROTOCOL
                                  ] = new JsonProperty(string.Format("\"{0}\"", mProtocolVersion));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_SESSION
                                  ] = new JsonProperty(string.Format("\"{0}\"", strSession));
                                jsonReturn[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_AUTH_VERIFICATION
                                    ] = new JsonProperty(string.Format("\"{0}\"", strVerify));
                                SendMessage(jsonReturn.ToString());
                                break;
                            case LicDefines.LICENSE_MSG_AUTH_SUCCESS:
                                strVerify =
                                    json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_AUTH_VERIFICATION]
                                        .Value;
                                mIsLogined = true;
                                //登录成功
                                OnDebug(LogMode.Info, string.Format("Logon successful.\t{0}", strVerify));
                                //触发登录成功事件
                                OnServerConnectionEvent(Defines.EVT_NET_AUTHED, strVerify);
                                //启动心跳线程
                                CreateHeartbeatThread();
                                break;
                            case LicDefines.LICENSE_MSG_AUTH_FAILED:
                            case LicDefines.LICENSE_MSG_AUTH_DENY_CLIENT_TYPE:
                            case LicDefines.LICENSE_MSG_AUTH_PROTOCOL_CLIENT:
                            case LicDefines.LICENSE_MSG_AUTH_PROTOCOL_SERVER:
                            case LicDefines.LICENSE_MSG_AUTH_WRONGFUL_SERVER:
                            case LicDefines.LICENSE_MSG_AUTH_TIME_NOT_SYNC:
                                //登录失败
                                OnDebug(LogMode.Info, string.Format("Logon failed.\t{0}", messageid));
                                mIsLogined = false;
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Send Package



        #endregion


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, mClient, msg);
        }

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        #endregion


        #region ServerConnectionEvent

        public event Action<int, string, string> ServerConnectionEvent;

        private void OnServerConnectionEvent(int code, string msg)
        {
            if (ServerConnectionEvent != null)
            {
                ServerConnectionEvent(code, mClient, msg);
            }
        }

        #endregion


        #region MessageReceivedEvent

        public event EventHandler<MessageReceivedEventArgs> MessageReceivedEvent;

        private void OnMessageReceivedEvent(MessageReceivedEventArgs args)
        {
            if (MessageReceivedEvent != null)
            {
                MessageReceivedEvent(this, args);
            }
        }

        #endregion


        #region Heartbeat

        private void CreateHeartbeatThread()
        {
            if (mThreadHeartbeat != null)
            {
                try
                {
                    mThreadHeartbeat.Abort();
                    mThreadHeartbeat = null;
                }
                catch { }
            }
            try
            {
                mThreadHeartbeat = new Thread(HeartbeatWorker);
                mThreadHeartbeat.Start();
                OnDebug(LogMode.Info, string.Format("Heartbeat thread started.\t{0}", mThreadHeartbeat.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateHeartbeatThread fail.\t{0}", ex.Message));
            }
        }

        private void StopHeartbeatThread()
        {
            try
            {
                if (mThreadHeartbeat != null)
                {
                    mThreadHeartbeat.Abort();
                    mThreadHeartbeat = null;
                    OnDebug(LogMode.Info, string.Format("Hearbeat thread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopHeartbeatThread fail.\t{0}", ex.Message));
            }
        }

        private void HeartbeatWorker()
        {
            while (true)
            {
                try
                {
                    if (mIsConnected)
                    {
                        JsonObject json = new JsonObject();
                        json[LicDefines.KEYWORD_MSG_COMMON_CLASSID] =
                            new JsonProperty(string.Format("{0}", LicDefines.LICENSE_MSG_CLASS_CONNECTION));
                        json[LicDefines.KEYWORD_MSG_COMMON_CLASSDESC] =
                            new JsonProperty(string.Format("\"{0}\"",
                                LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_CONNECTION)));
                        json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID] =
                            new JsonProperty(string.Format("{0}", LicDefines.LICENSE_MSG_CONNECTION_HEARTBEAT));
                        json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEDESC] =
                            new JsonProperty(string.Format("\"{0}\"",
                                LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_CONNECTION,
                                    LicDefines.LICENSE_MSG_CONNECTION_HEARTBEAT)));
                        json[LicDefines.KEYWORD_MSG_COMMON_CURRENTTIME] =
                                  new JsonProperty(string.Format("\"{0}\"",
                                      DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                        SendMessage(json.ToString());

                        OnDebug(LogMode.Info, string.Format("Send heartbeat end"));
                    }
                }
                catch (Exception ex)
                {
                    OnDebug(LogMode.Error, string.Format("Send heartbeat fail.\t{0}", ex.Message));
                }

                Thread.Sleep(mHeartbeatNum / 2 * 1000);
            }
        }

        #endregion


        #region Reconnect

        private void CreateReconnectThread()
        {
            if (mThreadReconnect != null)
            {
                try
                {
                    mThreadReconnect.Abort();
                    mThreadReconnect = null;
                }
                catch { }
            }
            try
            {
                mThreadReconnect = new Thread(ReconnectWorker);
                mThreadReconnect.Start();
                OnDebug(LogMode.Info, string.Format("Reconnect thread started.\t{0}", mThreadReconnect.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateReconnectThread fail.\t{0}", ex.Message));
            }
        }

        private void StopReconnectThread()
        {
            try
            {
                if (mThreadReconnect != null)
                {
                    mThreadReconnect.Abort();
                    mThreadReconnect = null;
                    OnDebug(LogMode.Info, string.Format("Reconnect thread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopReconnectThread fail.\t{0}", ex.Message));
            }
        }

        private void ReconnectWorker()
        {
            while (true)
            {
                try
                {
                    if (!mIsConnected)
                    {
                        Connect();
                    }
                }
                catch (Exception ex)
                {
                    OnDebug(LogMode.Error, string.Format("ReconnectWorker fail.\t{0}", ex.Message));
                }

                Thread.Sleep(mReconnectNum * 1000);
            }
        }

        #endregion

    }
}
