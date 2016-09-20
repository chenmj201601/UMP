//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b5688abe-c283-457b-8339-cb9e3f8d12bc
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorClient
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                MonitorClient
//
//        created by Charley at 2015/6/23 17:21:29
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
using VoiceCyber.UMP.Communications;

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 客户端，实现与服务器建立与维护连接，发送接收消息
    /// </summary>
    public class MonitorClient
    {

        #region Members

        private TcpClient mTcpClient;
        private SslStream mStream;
        private int mHeadSize = ConstValue.NET_MESSAGE_HEAD_SIZE;
        private int mMaxBufferSize = ConstValue.NET_BUFFER_MAX_SIZE;
        private byte[] mReceiveBuffer;
        private byte[] mBufferedData;
        private int mBufferedSize;
        private MessageEncoding mMsgEncoding;
        private string mSessionID;
        private bool mIsConnected;
        private Thread mThreadHeartbeat;
        private int mHeartbeatInterval = 30;       //心跳间隔时间,单位s

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


        #region Get/Set MessageEncoding
        /// <summary>
        /// Get or Set MessageEncoding
        /// </summary>
        public MessageEncoding MessageEncoding
        {
            get { return mMsgEncoding; }
            set { mMsgEncoding = value; }
        }

        #endregion

        #endregion


        /// <summary>
        /// 创建一个客户端
        /// </summary>
        public MonitorClient()
        {
            mMsgEncoding = MessageEncoding.None;
            mReceiveBuffer = new byte[mMaxBufferSize];
            mBufferedData = new byte[mMaxBufferSize];
            mBufferedSize = 0;
            mIsConnected = false;
        }


        #region Connect and Stop
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
                    mStream = new SslStream(mTcpClient.GetStream(), false, (s, cert, chain, err) => true);
                    mStream.AuthenticateAsClient(address);
                    mIsConnected = true;
                    OnConnected(string.Format("Server connected"));
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
        /// 向服务器发送消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息内容</param>
        public OperationReturn SendMessage(MessageHead head, string message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mTcpClient.Connected)
                {
                    byte[] bufferData = Encoding.UTF8.GetBytes(message);
                    int size = bufferData.Length;
                    head.Size = size;
                    byte[] bufferHead = Converter.Struct2Bytes(head);
                    mStream.Write(bufferHead, 0, bufferHead.Length);
                    mStream.Write(bufferData, 0, bufferData.Length);
                    mStream.Flush();
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
            }
            return optReturn;
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
                head.Type = MessageType.Request;
                switch (head.Encoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
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
                    case MessageEncoding.UTF8XML:
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
            return SendMessage(GetMessageHead(), message);
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
                OnDebug(LogMode.Error, string.Format("EndReceive fail.\t{0}", ex.Message));
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
                    //OnDebug(LogMode.Info, string.Format("MessageHead\tDataSize:{0}", dataSize));

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
                    string strMessage = Encoding.UTF8.GetString(temp);
                    DoServerMessage(msgHead, strMessage);
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
                OnDebug(LogMode.Error, string.Format("BeginReceive fail.\t{0}", ex.Message));
                mIsConnected = false;
            }
        }

        private void DoServerMessage(MessageHead head, string strMessage)
        {
            try
            {
                MessageEncoding msgEncoding = head.Encoding;
                ReturnMessage retMessage = null;
                NotifyMessage norMessage = null;
                OperationReturn optReturn;
                int intValue;
                string[] list;
                switch (msgEncoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
                        switch (head.Type)
                        {
                            case MessageType.Response:
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
                            case MessageType.Notify:
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
                    case MessageEncoding.UTF8XML:
                        switch (head.Type)
                        {
                            case MessageType.Response:
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
                            case MessageType.Notify:
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
                string msg;
                switch (head.Type)
                {
                    case MessageType.Response:
                        if (retMessage != null)
                        {
                            switch (retMessage.Command)
                            {
                                case (int)RequestCode.NCWelcome:
                                    msg = retMessage.LogInfo();
                                    OnDebug(LogMode.Info, string.Format("WelcomeMessage received.\t{0}", msg));
                                    break;
                            }
                            OnReturnMessageReceived(retMessage);
                        }
                        break;
                    case MessageType.Notify:
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

        public MessageHead GetMessageHead()
        {
            MessageHead head = new MessageHead();
            head.Flag = "US";
            head.Encoding = mMsgEncoding;
            head.Encryption = EncryptionMode.None;
            head.State = MessageState.LastPacket;
            return head;
        }

        private void SendHelloMessage()
        {
            try
            {
                RequestMessage request = new RequestMessage();
                request.Command = (int)RequestCode.NCHello;
                request.ListData.Add(string.Format("LocalTime:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
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
                OnDebug(LogMode.Error, string.Format("HeartbeatWorker fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Debug

        public event Action<LogMode, string> Debug;

        private void OnDebug(LogMode mode, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, msg);
            }
        }

        #endregion


        #region Connected

        public event Action<string> Connected;

        private void OnConnected(string msg)
        {
            if (Connected != null)
            {
                Connected(msg);
            }
        }

        #endregion


        #region ReturnMessageReceived

        /// <summary>
        /// 收到回复消息
        /// </summary>
        public event Action<ReturnMessage> ReturnMessageReceived;

        private void OnReturnMessageReceived(ReturnMessage retMessage)
        {
            //收到欢迎消息取得会话ID
            if (retMessage.Command == (int)RequestCode.NCWelcome)
            {
                mSessionID = retMessage.SessionID;
            }
            if (ReturnMessageReceived != null)
            {
                ReturnMessageReceived(retMessage);
            }
        }

        #endregion


        #region NotifyMessageReceived

        /// <summary>
        /// 收到通知消息
        /// </summary>
        public event Action<NotifyMessage> NotifyMessageReceived;

        private void OnNotifyMessageReceived(NotifyMessage notifyMessage)
        {
            if (NotifyMessageReceived != null)
            {
                NotifyMessageReceived(notifyMessage);
            }
        }

        #endregion

    }
}
