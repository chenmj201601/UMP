//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22357183-d7c6-40fe-9c86-e899771ba4b1
//        CLR Version:              4.0.30319.18063
//        Name:                     DecConnector
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                DecConnector
//
//        created by Charley at 2015/6/15 14:57:11
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.SDKs.DEC
{
    /// <summary>
    /// 连接DEC服务器
    /// 1、实现与DEC服务器握手并建立，维护连接
    /// 2、订阅消息（使用MessageString类型，可绑定MessageReceiveEvent事件获取订阅到的消息）
    /// 3、发布消息（调用PublishMessage方法）
    /// 注意，如果要启用断线重连功能，应该调用BeginConnect方法而不是Connect方法
    /// </summary>
    public class DecConnector : IEncryptable
    {

        #region Members

        private string mHost;
        private int mPort;
        private byte[] mReceiveBuffer;
        private int mBufferedSize;
        private byte[] mBufferedData;
        private TcpClient mTcpClient;
        private Stream mNetStream;
        private bool mIsConnected;
        private int mHearbeatDuration;
        private Thread mThreadHeartbeat;
        private Thread mThreadReconnect;
        private int mDistHeadSize;
        private int mRequestID;
        private ulong mEndpointID;


        #endregion


        /// <summary>
        /// 创建一个连接器
        /// </summary>
        public DecConnector()
        {
            mHost = string.Empty;
            mPort = 3072;
            mReceiveBuffer = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
            mBufferedData = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
            mBufferedSize = 0;
            mIsConnected = false;
            mDistHeadSize = Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1));
            mIsSubscribeAfterAuthed = false;
            mHearbeatDuration = 30;
            mRequestID = 0;
            mEndpointID = 0;
            mModuleType = 0;
            mModuleNumber = 0;
            mDataEncrypt = 0;
            mDataFormat = 0;
            mReconnectTime = 30;
            mIsSSL = false;
            mAppName = string.Format("DEC4Net Demo");
            mMaskMsg = new MessageString();
            mMessageMsg = new MessageString();
            GetAppVersion();
        }


        #region Public Properties


        #region Host Address
        /// <summary>
        /// 获取或设置DEC服务器地址
        /// </summary>
        public string Host
        {
            get { return mHost; }
            set { mHost = value; }
        }

        #endregion


        #region Host Port
        /// <summary>
        /// 获取或设置DEC服务器端口
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


        #region 订阅掩码

        private MessageString mMaskMsg;
        /// <summary>
        /// 获取或设置订阅掩码
        /// </summary>
        public MessageString MaskMsg
        {
            get { return mMaskMsg; }
            set { mMaskMsg = value; }
        }

        #endregion


        #region 订阅消息

        private MessageString mMessageMsg;
        /// <summary>
        /// 获取或设置订阅消息码
        /// </summary>
        public MessageString MessageMsg
        {
            get { return mMessageMsg; }
            set { mMessageMsg = value; }
        }

        #endregion


        #region EndpointID
        /// <summary>
        /// 获取本连接的ID（每次建立连接，服务器会分配一个唯一的ID用来标识本次连接）
        /// </summary>
        public ulong EndpointID
        {
            get { return mEndpointID; }
        }

        #endregion


        #region ModuleType

        private ushort mModuleType;
        /// <summary>
        /// 设置模块类型
        /// </summary>
        public ushort ModuleType
        {
            set { mModuleType = value; }
        }

        #endregion


        #region ModuleNumber

        private ushort mModuleNumber;
        /// <summary>
        /// 设置模块编号
        /// </summary>
        public ushort ModuleNumber
        {
            set { mModuleNumber = value; }
        }

        #endregion


        #region DataEncrypt

        private int mDataEncrypt;
        /// <summary>
        /// 设置加密方式
        /// </summary>
        public int DataEncrypt
        {
            set { mDataEncrypt = value; }
        }

        #endregion


        #region DataFormat

        private int mDataFormat;
        /// <summary>
        /// 设置数据格式
        /// </summary>
        public int DataFormat
        {
            set { mDataFormat = value; }
        }

        #endregion


        #region AppName

        private string mAppName;
        /// <summary>
        /// 设置应用名（连接时使用此名称作为描述）
        /// </summary>
        public string AppName
        {
            get { return mAppName; }
            set { mAppName = value; }
        }

        #endregion


        #region AppVersion

        private string mAppVersion;
        /// <summary>
        /// 设置版本信息，默认为本SDK库的版本
        /// </summary>
        public string AppVersion
        {
            set { mAppVersion = value; }
        }

        #endregion


        #region IsSSL

        private bool mIsSSL;
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool IsSSL
        {
            set { mIsSSL = value; }
        }

        #endregion


        #region 重连间隔时间

        private int mReconnectTime;
        /// <summary>
        /// 重连时间间隔,单位：s
        /// </summary>
        public int ReconnectTime
        {
            set { mReconnectTime = value; }
        }

        #endregion


        #region 是否认证成功后订阅消息

        private bool mIsSubscribeAfterAuthed;
        /// <summary>
        /// 是否认证成功后自动订阅消息
        /// </summary>
        public bool IsSubscribeAfterAuthed
        {
            set { mIsSubscribeAfterAuthed = value; }
        }

        #endregion

        #endregion


        #region Public Functions

        /// <summary>
        /// 连接到服务器
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
                mTcpClient = new TcpClient();
                mTcpClient.Connect(mHost, mPort);
                if (mTcpClient.Connected)
                {
                    if (mIsSSL)
                    {
                        SslStream sslStream = new SslStream(mTcpClient.GetStream(), false, (s, cert, chain, error) => true);
                        sslStream.AuthenticateAsClient(mHost);
                        mNetStream = sslStream;
                    }
                    else
                    {
                        NetworkStream networkStream = mTcpClient.GetStream();
                        mNetStream = networkStream;
                    }

                    mIsConnected = true;
                    OnDebug(string.Format("Server connected.\tRemote:{0}\tLocal:{1}", mTcpClient.Client.RemoteEndPoint,
                       mTcpClient.Client.LocalEndPoint));

                    mNetStream.BeginRead(mReceiveBuffer, 0, mDistHeadSize, ReceiveMessageWorker,
                        mNetStream);

                    //发送Hello消息包
                    SendHelloPackage();
                }
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("Connect fail.\t{0}", ex.Message));
            }
        }

        public void BeginConnect()
        {
            CreateReconnectThread();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                StopReconnectThread();
                StopHeartbeatThread();
                mIsConnected = false;
                if (mNetStream != null)
                {
                    try
                    {
                        mNetStream.Close();
                        mNetStream.Dispose();
                    }
                    catch { }
                    mNetStream = null;
                }
                if (mTcpClient != null)
                {
                    try
                    {
                        mTcpClient.Close();
                    }
                    catch { }
                    mTcpClient = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("Close fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        public void AddSubscribe()
        {
            AddSubscribe(mMaskMsg, mMessageMsg);
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="message"></param>
        public void AddSubscribe(MessageString mask, MessageString message)
        {
            mMaskMsg = mask;
            mMessageMsg = message;
            SendAddSubscribePackage();
        }
        /// <summary>
        /// 退订所有消息
        /// </summary>
        public void ClearSubscribe()
        {
            SendClearSubscribePackage();
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="message"></param>
        public void PublishMessage(string msg, MessageString message)
        {
            SendPublishMessage(msg, message);
        }

        #endregion


        #region ReceiveMessage

        private void ReceiveMessageWorker(IAsyncResult result)
        {
            //先接收识别头
            var stream = result.AsyncState as Stream;
            if (stream == null) { return; }
            int intSize;
            try
            {
                intSize = stream.EndRead(result);
                if (intSize <= 0)
                {
                    OnDebug(string.Format("Recieve 0 length message."));
                    mIsConnected = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(string.Format("EndRecieve fail.\t{0}", ex.Message));
                }
                mIsConnected = false;
                return;
            }
            try
            {
                //OnDebug(string.Format("Receive:{0}Byte", intSize));
                Array.Copy(mReceiveBuffer, 0, mBufferedData, mBufferedSize, intSize);
                mBufferedSize += intSize;
                if (mBufferedSize >= mDistHeadSize)
                {
                    //实际上只有可能等于，因为每次接收都是指定大小的

                    //取出数据
                    byte[] temp = new byte[mDistHeadSize];
                    Array.Copy(mBufferedData, 0, temp, 0, mDistHeadSize);
                    //重置缓存区
                    mBufferedSize = 0;
                    mBufferedData.Initialize();

                    //得到识别头，根据FollowSize大小接收基本头，扩展头和数据区
                    var distHead = (_TAG_NETPACK_DISTINGUISHHEAD_VER1)Converter.Bytes2Struct(temp, typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1));
                    int followSize = distHead._followsize;
                    //OnDebug(string.Format("DistHead\tFollowSize:{0}", followSize));

                    //循环，直到接收完FollowSize大小
                    int receiveSize = 0;
                    while (receiveSize < followSize)
                    {
                        intSize = stream.Read(mReceiveBuffer, receiveSize, followSize - receiveSize);
                        receiveSize += intSize;
                        Thread.Sleep(1);
                    }
                    if (receiveSize != followSize)
                    {
                        OnDebug(string.Format("FollowSize invalid"));
                        mIsConnected = false;
                        return;
                    }
                    MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                    args.DistHead = DistHead.FromHead(distHead);
                    int baseHeadSize = distHead._basesize;
                    if (baseHeadSize > 0)
                    {
                        //OnDebug(string.Format("BaseHead:{0}\tSize:{1}", distHead._basehead, baseHeadSize));
                        temp = new byte[baseHeadSize];
                        Array.Copy(mReceiveBuffer, 0, temp, 0, temp.Length);
                        args.BaseHead = temp;
                        switch (distHead._basehead)
                        {
                            case DecDefines.NETPACK_BASETYPE_CONNECT_WELCOME:
                                var welcomeHead = (_TAG_NETPACK_BASEHEAD_CONNECT_WELCOME)Converter.Bytes2Struct(temp,
                                    typeof(_TAG_NETPACK_BASEHEAD_CONNECT_WELCOME));
                                OnDebug(string.Format("Message:{0}\t{1}\t({2})", "Welcome",
                                    Helpers.ConvertByteArrayToString(welcomeHead._server_appname),
                                    welcomeHead._client_endpointid));

                                mEndpointID = welcomeHead._client_endpointid;
                                if (mEndpointID <= 0)
                                {
                                    OnDebug(string.Format("EndpointID invalid"));
                                }

                                //收到欢迎消息，发送登录消息（延时不能超过3s，否则服务端会断开本次连接）
                                SendLogonPackage(welcomeHead);

                                break;
                            case DecDefines.NETPACK_BASETYPE_CONNECT_AUTHEN:
                                var authHead =
                                    (_TAG_NETPACK_BASEHEAD_CONNECT_AUTHEN)
                                        Converter.Bytes2Struct(temp, typeof(_TAG_NETPACK_BASEHEAD_CONNECT_AUTHEN));
                                OnDebug(string.Format("Message:{0}\t{1}", "Auth", authHead._errorcode));
                                if (authHead._errorcode != 0)
                                {
                                    OnDebug(string.Format("登录验证失败\t{0}", authHead._errorcode));
                                }
                                else
                                {
                                    //登录成功，触发ServerConnected事件
                                    OnServerConnectedEvent(true, string.Format("Server connected.\t{0}", mEndpointID));

                                    //登录成功，启动发送心跳包的线程
                                    CreateHeartbeatThread();

                                    //订阅消息，登录成功后就可以订阅消息了
                                    if (mIsSubscribeAfterAuthed)
                                    {
                                        SendAddSubscribePackage();
                                    }
                                }
                                break;
                            case DecDefines.NETPACK_BASETYPE_CONNECT_ERROR:
                                //握手过程中出现错误
                                var errorHead =
                                    (_TAG_NETPACK_BASEHEAD_ERROR)
                                        Converter.Bytes2Struct(temp, typeof(_TAG_NETPACK_BASEHEAD_ERROR));
                                OnDebug(string.Format("Message:{0}\t{1}", "Error", errorHead._error_code));
                                break;
                            case DecDefines.NETPACK_BASETYPE_RES_ADDSUBSCRIBE:
                                var addSubscribeHead =
                                    (_TAG_NETPACK_BASEHEAD_RES_ADD_SUBSCRIBE)
                                        Converter.Bytes2Struct(temp, typeof(_TAG_NETPACK_BASEHEAD_RES_ADD_SUBSCRIBE));
                                OnDebug(string.Format("Message:{0}\t{1}", "RESAddSubscribe", addSubscribeHead._errorcode));
                                if (addSubscribeHead._errorcode != 0)
                                {
                                    OnDebug(string.Format("订阅消息失败\t{0}", addSubscribeHead._errorcode));
                                }
                                break;
                            case DecDefines.NETPACK_BASETYPE_APPLICATION_VER1:
                                var appVer1 =
                                    (NETPACK_BASEHEAD_APPLICATION_VER1)
                                        Converter.Bytes2Struct(temp, typeof(NETPACK_BASEHEAD_APPLICATION_VER1));
                                //收到应用层消息
                                OnDebug(string.Format("Message:{0}\t{1}", "App_Ver1", appVer1._datasize));
                                var appHead = AppHead.FromAppVer1(appVer1);
                                args.AppHead = appHead;
                                break;
                        }
                    }
                    int extHeadSize = distHead._extsize;
                    if (extHeadSize > 0)
                    {
                        //OnDebug(string.Format("ExtHead:{0}\tSize:{1}", distHead._exthead, extHeadSize));
                        temp = new byte[extHeadSize];
                        Array.Copy(mReceiveBuffer, baseHeadSize, temp, 0, temp.Length);
                        args.ExtHead = temp;
                    }
                    int dataSize = distHead._datasize;
                    if (dataSize > 0)
                    {
                        //OnDebug(string.Format("DataSize:{0}", dataSize));
                        temp = new byte[dataSize];
                        Array.Copy(mReceiveBuffer, baseHeadSize + extHeadSize, temp, 0, temp.Length);
                        args.Data = temp;
                    }

                    OnMessageReceivedEvent(args);
                }
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("DealMessage fail.\t{0}", ex.Message));
            }
            try
            {
                stream.BeginRead(mReceiveBuffer, 0, mDistHeadSize - mBufferedSize, ReceiveMessageWorker, stream);
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(string.Format("BeginReceive fail.\t{0}", ex.Message));
                }
                mIsConnected = false;
            }
        }

        #endregion


        #region SendMessage

        internal void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            SendMessage(data);
        }

        internal void SendMessage(_TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head, byte[] data)
        {
            byte[] byteHead = Converter.Struct2Bytes(ver1Head);
            byte[] byteSend = new byte[byteHead.Length + data.Length];
            Array.Copy(byteHead, 0, byteSend, 0, byteHead.Length);
            Array.Copy(data, 0, byteSend, byteHead.Length, data.Length);
            SendMessage(byteSend);
        }

        internal void SendMessage(_TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head)
        {
            byte[] byteHead = Converter.Struct2Bytes(ver1Head);
            byte[] byteSend = new byte[byteHead.Length];
            Array.Copy(byteHead, 0, byteSend, 0, byteHead.Length);
            SendMessage(byteSend);
        }

        internal void SendMessage(byte[] data)
        {
            try
            {
                if (!mIsConnected)
                {
                    OnDebug(string.Format("Server not connected"));
                    return;
                }
                mNetStream.Write(data, 0, data.Length);
                mNetStream.Flush();
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Send Package

        #region Send Hello Package

        private void SendHelloPackage()
        {
            try
            {
                _TAG_NETPACK_BASEHEAD_CONNECT_HELLO helloPackage = new _TAG_NETPACK_BASEHEAD_CONNECT_HELLO();
                helloPackage._client_appname = Helpers.ConvertStringToByteArray(mAppName, DecDefines.LEN_APPNAME);
                helloPackage._client_moduleid = mModuleType;
                helloPackage._client_number = mModuleNumber;
                helloPackage._client_version = Helpers.ConvertStringToByteArray(mAppVersion, DecDefines.LEN_VERSION);
                helloPackage._client_starttime = Helpers.CreateTimestamp(DateTime.Now.ToUniversalTime());
                helloPackage._client_connecttime = Helpers.CreateTimestamp(DateTime.Now.ToUniversalTime());
                helloPackage._client_protocol = new ulong[4];
                helloPackage._client_protocol[0] = Helpers.MakeNetPackProtocol(1, 0);

                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                    DecDefines.NETPACK_PACKTYPE_CONNECT,
                    DecDefines.NETPACK_BASETYPE_CONNECT_HELLO,
                    DecDefines.NETPACK_EXTTYPE_NOTHING,
                    0);
                byte[] messageHead = Converter.Struct2Bytes(helloPackage);
                SendMessage(ver1Head, messageHead);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Send Logon Package

        private void SendLogonPackage(_TAG_NETPACK_BASEHEAD_CONNECT_WELCOME welcome)
        {
            try
            {
                byte[] session = welcome._authenticatecode;
                byte[] code = Helpers.GetValidicationCode(session); //计算认证码
                _TAG_NETPACK_BASEHEAD_CONNECT_LOGON logon = new _TAG_NETPACK_BASEHEAD_CONNECT_LOGON();
                logon._heartbeat = mHearbeatDuration;
                logon._authenticatecode = code;

                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                   DecDefines.NETPACK_PACKTYPE_CONNECT,
                   DecDefines.NETPACK_BASETYPE_CONNECT_LOGON,
                   DecDefines.NETPACK_EXTTYPE_NOTHING,
                   0);

                byte[] messageHead = Converter.Struct2Bytes(logon);
                SendMessage(ver1Head, messageHead);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SendHeartbeat Package

        private void SendHeartbeatPackage()
        {
            try
            {
                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                   DecDefines.NETPACK_PACKTYPE_HEARTBEAT,
                   DecDefines.NETPACK_BASETYPE_NOTHING,
                   DecDefines.NETPACK_EXTTYPE_NOTHING,
                   0);
                SendMessage(ver1Head);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SendAddSubscribe Package

        private void SendAddSubscribePackage()
        {
            try
            {
                if (mMaskMsg == null || mMessageMsg == null)
                {
                    OnDebug(string.Format("MaskMsg or MessageMsg is null"));
                    return;
                }

                _TAG_NETPACK_MESSAGE mask = mMaskMsg.GetMessage();
                _TAG_NETPACK_MESSAGE message = mMessageMsg.GetMessage();

                _TAG_NETPACK_BASEHEAD_REQ_ADD_SUBSCRIBE addSubscribe = new _TAG_NETPACK_BASEHEAD_REQ_ADD_SUBSCRIBE();
                addSubscribe._requestid = GetRequestID();
                addSubscribe._mask = mask;
                addSubscribe._message = message;

                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                   DecDefines.NETPACK_PACKTYPE_REQUEST,
                   DecDefines.NETPACK_BASETYPE_REQ_ADDSUBSCRIBE,
                   DecDefines.NETPACK_EXTTYPE_NOTHING,
                   0);
                byte[] messageHead = Converter.Struct2Bytes(addSubscribe);
                SendMessage(ver1Head, messageHead);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SendClearSubscribe Package

        private void SendClearSubscribePackage()
        {
            try
            {
                _TAG_NETPACK_BASEHEAD_REQ_CLEAR_SUBSCRIBE clearSubscribe = new _TAG_NETPACK_BASEHEAD_REQ_CLEAR_SUBSCRIBE();
                clearSubscribe._requestid = mRequestID++;

                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                   DecDefines.NETPACK_PACKTYPE_REQUEST,
                   DecDefines.NETPACK_BASETYPE_REQ_CLEARSUBSCRIBE,
                   DecDefines.NETPACK_EXTTYPE_NOTHING,
                   0);
                byte[] messageHead = Converter.Struct2Bytes(clearSubscribe);
                SendMessage(ver1Head, messageHead);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SendPublishMessage Package

        private void SendPublishMessage(string msg, MessageString message)
        {
            try
            {
                byte[] byteMsg = Encoding.UTF8.GetBytes(msg);

                NETPACK_BASEHEAD_APPLICATION_VER1 appVer1 = new NETPACK_BASEHEAD_APPLICATION_VER1();
                appVer1._channel = 0;
                appVer1._encrypt = (byte)mDataEncrypt;
                appVer1._compress = 0;
                appVer1._format = (byte)mDataFormat;

                appVer1._datasize = (ushort)byteMsg.Length;
                appVer1._validsize = Helpers.GetEncryptStoreSize((byte)mDataEncrypt, (ushort)byteMsg.Length);

                _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head = CreateVer1Head(
                    DecDefines.NETPACK_PACKTYPE_APPLICATION,
                    DecDefines.NETPACK_BASETYPE_APPLICATION_VER1,
                    DecDefines.NETPACK_EXTTYPE_NOTHING,
                    appVer1._validsize);

                _TAG_NETPACK_MESSAGE messageMessage = Helpers.CreateMessage(message.SourceModule, message.SourceNumber,
                    message.TargetModule, message.TargetNumber, message.Number, message.SmallType, message.MiddleType,
                    message.LargeType);
                ver1Head._message = messageMessage;

                byte[] byteBaseHead = Converter.Struct2Bytes(appVer1);
                byte[] byteSend = new byte[byteBaseHead.Length + byteMsg.Length];
                Array.Copy(byteBaseHead, 0, byteSend, 0, byteBaseHead.Length);
                Array.Copy(byteMsg, 0, byteSend, byteBaseHead.Length, byteMsg.Length);
                SendMessage(ver1Head, byteSend);
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("SendMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion

        #endregion


        #region 识别头

        private _TAG_NETPACK_DISTINGUISHHEAD_VER1 CreateVer1Head(byte packType,
            ushort baseHeadType,
            ushort extHeadType,
            ushort validSize)
        {
            _TAG_NETPACK_DISTINGUISHHEAD distHead = Helpers.CreateDistinguishHead();
            ushort distHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD));

            ushort baseHeadSize = 0;
            switch (baseHeadType)
            {
                case DecDefines.NETPACK_BASETYPE_NOTHING:
                    baseHeadSize = 0;
                    break;
                case DecDefines.NETPACK_BASETYPE_APPLICATION_VER1:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(NETPACK_BASEHEAD_APPLICATION_VER1));
                    break;
                case DecDefines.NETPACK_BASETYPE_CONNECT_ERROR:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_ERROR));
                    break;
                case DecDefines.NETPACK_BASETYPE_CONNECT_HELLO:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_HELLO));
                    break;
                case DecDefines.NETPACK_BASETYPE_CONNECT_WELCOME:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_WELCOME));
                    break;
                case DecDefines.NETPACK_BASETYPE_CONNECT_LOGON:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_LOGON));
                    break;
                case DecDefines.NETPACK_BASETYPE_CONNECT_AUTHEN:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_AUTHEN));
                    break;
                case DecDefines.NETPACK_BASETYPE_REQ_ADDSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_REQ_ADD_SUBSCRIBE));
                    break;
                case DecDefines.NETPACK_BASETYPE_RES_ADDSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_RES_ADD_SUBSCRIBE));
                    break;
                case DecDefines.NETPACK_BASETYPE_REQ_DELSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_REQ_DEL_SUBSCRIBE));
                    break;
                case DecDefines.NETPACK_BASETYPE_RES_DELSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_RES_DEL_SUBSCRIBE));
                    break;
                case DecDefines.NETPACK_BASETYPE_REQ_CLEARSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_REQ_CLEAR_SUBSCRIBE));
                    break;
                case DecDefines.NETPACK_BASETYPE_RES_CLEARSUBSCRIBE:
                    baseHeadSize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_RES_CLEAR_SUBSCRIBE));
                    break;
            }
            ushort extHeadSize = 0;
            //扩展包头类型
            switch (extHeadType)
            {
                case DecDefines.NETPACK_EXTTYPE_NOTHING:
                    extHeadSize = 0;
                    break;
            }
            int headSize = distHeadSize + baseHeadSize + extHeadSize;
            _TAG_NETPACK_ENCRYPT_CONTEXT encryptContext = new _TAG_NETPACK_ENCRYPT_CONTEXT();
            encryptContext._encrypt = DecDefines.NETPACK_ENCRYPT_NOTHING;
            ushort dataSize = Helpers.GetEncryptStoreSize(encryptContext._encrypt, validSize);
            short packSize = (short)(headSize + dataSize);

            _TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1head = new _TAG_NETPACK_DISTINGUISHHEAD_VER1();
            ver1head._dist = distHead;
            ver1head._packtype = packType;
            ver1head._followsize = (ushort)(packSize - distHeadSize);
            ver1head._source = mEndpointID;
            ver1head._target = 0xffffffffffffffff;
            ver1head._timestamp = (ulong)DateTime.Now.Ticks;
            ver1head._basehead = baseHeadType;
            ver1head._basesize = baseHeadSize;
            ver1head._exthead = extHeadType;
            ver1head._extsize = extHeadSize;
            ver1head._datasize = dataSize;
            ver1head._state = 0;
            ver1head._moduleid = mModuleType;
            ver1head._number = mModuleNumber;
            ver1head._timestamp = (ulong)DateTime.Now.Ticks;
            return ver1head;
        }

        #endregion


        #region Debug

        public event Action<string> Debug;

        private void OnDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }

        #endregion


        #region ServerConnectedEvent

        public event Action<bool, string> ServerConnectedEvent;

        private void OnServerConnectedEvent(bool isConnected, string msg)
        {
            if (ServerConnectedEvent != null)
            {
                ServerConnectedEvent(isConnected, msg);
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


        #region Hearbeat

        private void CreateHeartbeatThread()
        {
            try
            {
                if (mThreadHeartbeat != null)
                {
                    mThreadHeartbeat.Abort();
                    mThreadHeartbeat = null;
                }
            }
            catch { }
            try
            {
                mThreadHeartbeat = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            SendHeartbeatPackage();
                            Thread.Sleep(500 * mHearbeatDuration);
                        }
                    }
                    catch (Exception ex)
                    {
                        var abort = ex as ThreadAbortException;
                        if (abort == null)
                        {
                            OnDebug(string.Format("HeartbeatWorker fail.\t{0}", ex.Message));
                        }
                    }
                });
                mThreadHeartbeat.Start();
                OnDebug(string.Format("HeartbeatThread created.\t{0}", mThreadHeartbeat.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("CreateHeartbeatThread fail.\t{0}", ex.Message));
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
                }
            }
            catch (Exception ex)
            {
                //OnDebug(string.Format("StopHeartbeatThread fail.\t{0}", ex.Message));
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
                mThreadReconnect = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            if (!mIsConnected)
                            {
                                Connect();
                            }
                            Thread.Sleep(1000 * mReconnectTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        var abort = ex as ThreadAbortException;
                        if (abort == null)
                        {
                            OnDebug(string.Format("Connect fail.\t{0}", ex.Message));
                        }
                    }
                });
                mThreadReconnect.Start();

                OnDebug(string.Format("Reconnect thread started.\t{0}", mThreadReconnect.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("CreateReconnectThread fail.\t{0}", ex.Message));
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
                }

                OnDebug(string.Format("Reconnect thread stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("StopReconnectThread fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Encrypt and Decrypt

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return source;
        }

        public string DecryptString(string source, int mode)
        {
            return source;
        }

        public string DecryptString(string source)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return source;
        }

        public string EncryptString(string source, int mode)
        {
            return source;
        }

        public string EncryptString(string source)
        {
            return source;
        }

        #endregion


        #region Others

        private int GetRequestID()
        {
            if (mRequestID < 102400)
            {
                mRequestID++;
            }
            else
            {
                mRequestID = 1;
            }
            return mRequestID;
        }

        private void GetAppVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                mAppVersion = string.Format("{0:0}.{1:00}.{2:000}.{3:000}",
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision);
            }
            catch { }
        }

        #endregion

    }
}
