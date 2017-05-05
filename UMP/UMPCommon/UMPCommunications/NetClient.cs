//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d8d45c78-c322-43ef-9048-da26d5efabda
//        CLR Version:              4.0.30319.18063
//        Name:                     NetClient
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NetClient
//
//        created by Charley at 2015/9/21 13:37:39
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

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// Socket客户端的基本实现
    /// </summary>
    public class NetClient : INetClient
    {

        #region Members

        public IEncryptable EncryptObject { get; set; }

        private TcpClient mTcpClient;
        private Stream mStream;
        private int mHeadSize = ConstValue.NET_MESSAGE_HEAD_SIZE;
        private int mMaxBufferSize = ConstValue.NET_BUFFER_MAX_SIZE;
        private byte[] mReceiveBuffer;
        private byte[] mBufferedData;
        private string mHost;
        private int mPort;
        private bool mIsSSL;
        private int mBufferedSize;
        private int mMsgEncoding;
        private int mEncryption;
        private string mSessionID;
        private bool mIsConnected;
        private Thread mThreadHeartbeat;
        private int mHeartbeatInterval = 30;       //心跳间隔时间,单位s
        private object mWriteLocker = new object();

        #endregion


        #region Public Properties

        #region SessionID

        public string SessionID
        {
            get { return mSessionID; }
        }

        #endregion


        #region IsConnected

        /// <summary>
        /// 是否连接上服务器
        /// </summary>
        public bool IsConnected
        {
            get { return mIsConnected; }
        }

        #endregion


        #region MessageEncoding
        /// <summary>
        /// Get or Set MessageEncoding
        /// </summary>
        public int MsgEncoding
        {
            get { return mMsgEncoding; }
            set { mMsgEncoding = value; }
        }

        #endregion


        #region EncryptionMode
        /// <summary>
        /// 加密方式
        /// </summary>
        public int Encryption
        {
            get { return mEncryption; }
            set { mEncryption = value; }
        }

        #endregion


        #region Host

        public string Host
        {
            get { return mHost; }
            set { mHost = value; }
        }

        #endregion


        #region Port

        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        #endregion


        #region IsSSL

        public bool IsSSL
        {
            get { return mIsSSL; }
            set { mIsSSL = value; }
        }

        #endregion

        #endregion


        public NetClient()
        {
            mMsgEncoding = (int)MessageEncoding.None;
            mEncryption = (int)EncryptionMode.None;
            mReceiveBuffer = new byte[mMaxBufferSize];
            mBufferedData = new byte[mMaxBufferSize];
            mBufferedSize = 0;
            mIsConnected = false;
            mHost = string.Empty;
            mPort = 0;
            mIsSSL = false;
        }


        #region Connect and Stop

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <returns></returns>
        public OperationReturn Connect()
        {
            return Connect(mHost, mPort);
        }
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="address">服务器地址</param>
        /// <param name="port">端口</param>
        public OperationReturn Connect(string address, int port)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                mTcpClient = new TcpClient(AddressFamily.InterNetwork);
                mTcpClient.Connect(address, port);
                if (mTcpClient.Connected)
                {
                    if (mIsSSL)
                    {
                        SslStream sslStream = new SslStream(mTcpClient.GetStream(), false, (s, cert, chain, err) => true);
                        sslStream.AuthenticateAsClient(address);
                        mStream = sslStream;
                    }
                    else
                    {
                        mStream = mTcpClient.GetStream();
                    }
                    mIsConnected = true;
                    OnConnectionEvent(Defines.EVT_NET_CONNECTED, string.Format("Server connected"));
                    OnDebug(LogMode.Info,
                        string.Format("Server connected.\tLocal:{0}\tRemote:{1}", mTcpClient.Client.LocalEndPoint,
                            mTcpClient.Client.RemoteEndPoint));

                    //开始接收数据
                    mStream.BeginRead(mReceiveBuffer, 0, mHeadSize, ReceiveMessageWorker, mStream);

                    //发送Hello消息
                    SendHelloMessage();

                    //启动发送心跳的线程
                    CreateHeartbeatThread();
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Connect fail.\t{0}", ex.Message));
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
        /// <summary>
        /// 停止客户端
        /// </summary>
        public void Stop()
        {
            try
            {
                mIsConnected = false;

                //停止心跳线程
                StopHeartbeatThread();

                //关闭连接
                try
                {
                    if (mStream != null)
                    {
                        mStream.Close();
                    }
                }
                catch { }
                try
                {
                    if (mTcpClient != null)
                    {
                        mTcpClient.Close();
                    }

                }
                catch { }

                OnDebug(LogMode.Info, string.Format("Session stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Session stop fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 发送消息

        /// <summary>
        /// 发送字节流数据
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="data">消息数据</param>
        /// <returns></returns>
        public OperationReturn SendMessage(MessageHead head, byte[] data)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mTcpClient.Connected)
                {
                    byte[] temp = data;
                    if (mEncryption > 0)
                    {
                        //数据加密
                        if (EncryptObject == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("EncryptObject is null");
                            return optReturn;
                        }
                        temp = EncryptObject.EncryptBytes(data, mEncryption);
                    }
                    int size = temp.Length;
                    head.Size = size;
                    byte[] bufferHead = Converter.Struct2Bytes(head);
                    lock (mWriteLocker)
                    {
                        mStream.Write(bufferHead, 0, bufferHead.Length);
                        mStream.Write(temp, 0, temp.Length);
                        mStream.Flush();
                    }
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_CONNECTED;
                    optReturn.Message = string.Format("TcpClient not connected");
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendMessage fail.\t{0}", ex.Message));
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息内容</param>
        public OperationReturn SendMessage(MessageHead head, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return SendMessage(head, data);
        }
        /// <summary>
        /// 向服务器发送请求消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息对象</param>
        public OperationReturn SendMessage(MessageHead head, RequestMessage message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                message.SessionID = mSessionID;
                head.Type = (int)MessageType.Request;
                head.Command = message.Command;
                switch (head.Encoding)
                {
                    case (int)MessageEncoding.None:
                    case (int)MessageEncoding.UTF8String:
                        var request = message;
                        string strMessage = string.Empty;
                        //Command
                        strMessage += string.Format("{0}{1}", request.Command, ConstValue.SPLITER_CHAR);
                        //SessionID
                        strMessage += string.Format("{0}{1}", request.SessionID, ConstValue.SPLITER_CHAR);
                        //Data
                        strMessage += string.Format("{0}{1}", request.Data, ConstValue.SPLITER_CHAR);
                        //ListData
                        string strListData = string.Empty;
                        for (int i = 0; i < request.ListData.Count; i++)
                        {
                            string msg = request.ListData[i];
                            if (i < request.ListData.Count - 1)
                            {
                                strListData += string.Format("{0}{1}", msg, ConstValue.SPLITER_CHAR_2);
                            }
                            else
                            {
                                strListData += string.Format("{0}", msg);
                            }
                        }
                        strMessage += string.Format("{0}", strListData);
                        SendMessage(head, strMessage);
                        break;
                    case (int)MessageEncoding.UTF8XML:
                        optReturn = XMLHelper.SeriallizeObject(message);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("Seralize message fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return optReturn;
                        }
                        SendMessage(head, optReturn.Data.ToString());
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("MessageEncoding type not support.\t{0}", head.Encoding));
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendMessage fail.\t{0}", ex.Message));
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
        /// <summary>
        /// 向服务器发送请求消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public OperationReturn SendMessage(int command, RequestMessage message)
        {
            return SendMessage(GetMessageHead(command), message);
        }
        /// <summary>
        /// 向服务器发送请求消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendMessage(RequestMessage message)
        {
            return SendMessage(message.Command, message);
        }

        #endregion


        #region 接收消息

        private void ReceiveMessageWorker(IAsyncResult result)
        {
            var stream = result.AsyncState as Stream;
            if (stream == null) { return; }
            int intSize;
            try
            {
                intSize = stream.EndRead(result);
                if (intSize <= 0)
                {
                    OnDebug(LogMode.Error, string.Format("Receive 0 length message. Server disconnected."));
                    mIsConnected = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(LogMode.Error, string.Format("EndReceive fail.\t{0}", ex.Message));
                }
                mIsConnected = false;
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

                    //得到消息头
                    var msgHead = (MessageHead)Converter.Bytes2Struct(temp, typeof(MessageHead));
                    int dataSize = msgHead.Size;
                    OnDebug(LogMode.Debug, string.Format("MessageHead\tDataSize:{0}", dataSize));

                    //循环，直到接收完DataSize大小的数据
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
                        return;
                    }

                    //取出数据
                    temp = new byte[dataSize];
                    Array.Copy(mReceiveBuffer, 0, temp, 0, temp.Length);
                    DoServerMessage(msgHead, temp);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ReceiveMessage fail.\t{0}", ex.Message));
            }
            try
            {
                stream.BeginRead(mReceiveBuffer, 0, mHeadSize - mBufferedSize, ReceiveMessageWorker, stream);
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(LogMode.Error, string.Format("BeginReceive fail.\t{0}", ex.Message));
                }
                mIsConnected = false;
            }
        }

        private void DoServerMessage(MessageHead head, byte[] data)
        {
            byte[] temp = data;
            if (head.Encryption > 0)
            {
                //解密数据
                if (EncryptObject == null)
                {
                    OnDebug(LogMode.Error, string.Format("EncryptObject is null."));
                    return;
                }
                temp = EncryptObject.DecryptBytes(data, head.Encryption);
            }
            OnMessageReceived(head, temp);
            if (head.Type == (int)MessageType.Response
                || head.Type == (int)MessageType.Notify)
            {
                string strMsg = Encoding.UTF8.GetString(temp);
                DoServerMessage(head, strMsg);
            }
        }

        private void DoServerMessage(MessageHead head, string strMessage)
        {
            try
            {
                int msgEncoding = head.Encoding;
                ReturnMessage retMessage = null;
                NotifyMessage norMessage = null;
                OperationReturn optReturn;
                int intValue;
                string[] list;
                switch (msgEncoding)
                {
                    case (int)MessageEncoding.None:
                    case (int)MessageEncoding.UTF8String:
                        switch (head.Type)
                        {
                            case (int)MessageType.Response:
                                retMessage = new ReturnMessage();
                                list = strMessage.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                                if (list.Length > 0)
                                {
                                    retMessage.Result = list[0] == "1";
                                }
                                if (list.Length > 1)
                                {
                                    if (int.TryParse(list[1], out intValue))
                                    {
                                        retMessage.Code = intValue;
                                    }
                                }
                                if (list.Length > 2)
                                {
                                    retMessage.SessionID = list[2];
                                }
                                if (list.Length > 3)
                                {
                                    if (int.TryParse(list[3], out intValue))
                                    {
                                        retMessage.Command = intValue;
                                    }
                                }
                                if (list.Length > 4)
                                {
                                    retMessage.Message = list[4];
                                }
                                if (list.Length > 5)
                                {
                                    retMessage.Data = list[5];
                                }
                                if (list.Length > 6)
                                {
                                    string strListData = list[6];
                                    string[] listData = strListData.Split(new[] { ConstValue.SPLITER_CHAR_2 },
                                        StringSplitOptions.None);
                                    for (int i = 0; i < listData.Length; i++)
                                    {
                                        retMessage.ListData.Add(listData[i]);
                                    }
                                }
                                break;
                            case (int)MessageType.Notify:
                                norMessage = new NotifyMessage();
                                list = strMessage.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                                if (list.Length > 0)
                                {
                                    norMessage.SessionID = list[0];
                                }
                                if (list.Length > 1)
                                {
                                    if (int.TryParse(list[1], out intValue))
                                    {
                                        norMessage.Command = intValue;
                                    }
                                }
                                if (list.Length > 2)
                                {
                                    norMessage.Data = list[2];
                                }
                                if (list.Length > 3)
                                {
                                    string strListData = list[3];
                                    string[] listData = strListData.Split(new[] { ConstValue.SPLITER_CHAR_2 },
                                        StringSplitOptions.None);
                                    for (int i = 0; i < listData.Length; i++)
                                    {
                                        norMessage.ListData.Add(listData[i]);
                                    }
                                }
                                break;
                        }
                        break;
                    case (int)MessageEncoding.UTF8XML:
                        switch (head.Type)
                        {
                            case (int)MessageType.Response:
                                optReturn = XMLHelper.DeserializeObject<ReturnMessage>(strMessage);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("Deserialize ReturnMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                retMessage = optReturn.Data as ReturnMessage;
                                if (retMessage == null)
                                {
                                    OnDebug(LogMode.Error, string.Format("ReturnMessage is null"));
                                    return;
                                }
                                break;
                            case (int)MessageType.Notify:
                                optReturn = XMLHelper.DeserializeObject<NotifyMessage>(strMessage);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("Deserialize NotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                norMessage = optReturn.Data as NotifyMessage;
                                if (norMessage == null)
                                {
                                    OnDebug(LogMode.Error, string.Format("NotifyMessage is null"));
                                    return;
                                }
                                break;
                        }
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Encoding invalid.\t{0}", mMsgEncoding));
                        return;
                }
                switch (head.Type)
                {
                    case (int)MessageType.Response:
                        if (retMessage != null)
                        {
                            OnReturnMessageReceived(retMessage);
                        }
                        break;
                    case (int)MessageType.Notify:
                        if (norMessage != null)
                        {
                            OnNotifyMessageReceived(norMessage);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoServerMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        /// <summary>
        /// 生成消息头
        /// </summary>
        /// <returns></returns>
        public MessageHead GetMessageHead()
        {
            MessageHead head = new MessageHead();
            head.Flag = "US";
            head.Encoding = mMsgEncoding;
            head.Encryption = mEncryption;
            head.State = (int)MessageState.LastPacket;
            return head;
        }

        /// <summary>
        /// 生成消息头
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns></returns>
        public MessageHead GetMessageHead(int command)
        {
            var head = GetMessageHead();
            head.Command = command;
            return head;
        }

        private void SendHelloMessage()
        {
            try
            {

                /*
                 * Hello消息的内容定义
                 * 
                 * ListData：
                 * 0：       当前时间,UTC
                 * 1：       Encoding
                 * 2：       EncryptionMode
                 * 
                 */

                RequestMessage request = new RequestMessage();
                request.Command = (int)RequestCode.NCHello;
                request.ListData.Add(string.Format("{0}", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                request.ListData.Add(string.Format("{0}", mMsgEncoding));
                request.ListData.Add(string.Format("{0}", mEncryption));
                SendMessage((int)RequestCode.NCHello, request);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendHelloMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 心跳包

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
                OnDebug(LogMode.Info, string.Format("Hearbeat thread started.\t{0}", mThreadHeartbeat.ManagedThreadId));
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
                }

                OnDebug(LogMode.Info, string.Format("Heartbeat thead stopped."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopHeartbeatThread fail.\t{0}", ex.Message));
            }
        }

        private void HeartbeatWorker()
        {
            try
            {
                while (true)
                {
                    //发送心跳消息
                    RequestMessage request = new RequestMessage();
                    request.SessionID = mSessionID;
                    request.Command = (int)RequestCode.NCHearbeat;
                    SendMessage((int)RequestCode.NCHearbeat, request);
                    Thread.Sleep(1000 * mHeartbeatInterval);
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("HeartbeatWorker fail.\t{0}", ex.Message));
                }
            }
        }

        #endregion


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "NetClient", msg);
        }

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        #endregion


        #region ConnectionEvent

        public event EventHandler<ConnectionEventArgs> ConnectionEvent;

        private void OnConnectionEvent(int code, string msg)
        {
            if (ConnectionEvent != null)
            {
                ConnectionEventArgs args = new ConnectionEventArgs();
                args.Code = code;
                args.Name = mSessionID;
                args.Message = msg;
                ConnectionEvent(this, args);
            }
        }

        #endregion


        #region MessageReceived

        /// <summary>
        /// 收到消息数据
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private void OnMessageReceived(MessageHead head, byte[] data)
        {
            if (MessageReceived != null)
            {
                MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                args.Name = mSessionID;
                args.Head = head;
                args.Data = data;
                MessageReceived(this, args);
            }
        }

        #endregion


        #region ReturnMessageReceived

        /// <summary>
        /// 收到回复消息
        /// </summary>
        public event EventHandler<ReturnMessageReceivedEventArgs> ReturnMessageReceived;

        private void OnReturnMessageReceived(ReturnMessage retMessage)
        {
            //收到欢迎消息取得会话ID
            if (retMessage.Command == (int)RequestCode.NCWelcome)
            {
                mSessionID = retMessage.SessionID;
            }
            if (ReturnMessageReceived != null)
            {
                ReturnMessageReceivedEventArgs args = new ReturnMessageReceivedEventArgs();
                args.Name = mSessionID;
                args.ReturnMessage = retMessage;
                ReturnMessageReceived(this, args);
            }
        }

        #endregion


        #region NotifyMessageReceived

        /// <summary>
        /// 收到通知消息
        /// </summary>
        public event EventHandler<NotifyMessageReceivedEventArgs> NotifyMessageReceived;

        private void OnNotifyMessageReceived(NotifyMessage notifyMessage)
        {
            if (NotifyMessageReceived != null)
            {
                NotifyMessageReceivedEventArgs args = new NotifyMessageReceivedEventArgs();
                args.Name = mSessionID;
                args.NotifyMessage = notifyMessage;
                NotifyMessageReceived(this, args);
            }
        }

        #endregion

    }
}
