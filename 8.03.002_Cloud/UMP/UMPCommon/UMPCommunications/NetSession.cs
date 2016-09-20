//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e6f188c-0213-4ad9-93ca-b641b0a9fa4c
//        CLR Version:              4.0.30319.18063
//        Name:                     NetSession
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NetSession
//
//        created by Charley at 2015/11/27 11:58:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 表示一个网络会话
    /// </summary>
    public class NetSession : INetSession
    {

        #region Members

        public X509Certificate2 Certificate { get; set; }
        public IEncryptable EncryptObject { get; set; }

        private string mName;
        private string mSessionID;
        private bool mIsSSL;
        private int mMsgEncoding;
        private int mEncryption;
        private DateTime mLastActiveTime;
        private bool mIsConnected;
        private string mRemoteEndpoint;

        private Stream mStream;
        private TcpClient mTcpClient;
        private int mHeadSize = ConstValue.NET_MESSAGE_HEAD_SIZE;
        private int mMaxBufferSize = ConstValue.NET_BUFFER_MAX_SIZE;
        private byte[] mReceiveBuffer;
        private byte[] mBufferedData;
        private int mBufferedSize;
        private object mWriteLocker = new object();

        #endregion


        #region Public Properties

        #region Name
        /// <summary>
        /// 服务别名
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        #endregion


        #region SessionID
        /// <summary>
        /// 获取当前会话的唯一编号
        /// </summary>
        public string SessionID
        {
            get { return mSessionID; }
        }

        #endregion


        #region IsSSL
        /// <summary>
        /// 获取或设置是否启用SSL安全连接
        /// </summary>
        public bool IsSSL
        {
            get { return mIsSSL; }
            set { mIsSSL = value; }
        }
        #endregion


        #region MessageEncoding
        /// <summary>
        /// 获取或获取设置消息编码类型
        /// </summary>
        public int MsgEncoding
        {
            get { return mMsgEncoding; }
            set { mMsgEncoding = value; }
        }
        #endregion


        #region EncryptionMode
        /// <summary>
        /// 获取或设置消息加密模式
        /// </summary>
        public int Encryption
        {
            get { return mEncryption; }
            set { mEncryption = value; }
        }
        #endregion


        #region RemoteEndpoint
        /// <summary>
        /// 获取远端终结点地址
        /// </summary>
        public string RemoteEndpoint
        {
            get { return mRemoteEndpoint; }
        }
        #endregion


        #region LastActiveTime
        /// <summary>
        /// 获取最后一次发送或接收数据库的时间
        /// </summary>
        public DateTime LastActiveTime
        {
            get { return mLastActiveTime; }
        }
        #endregion


        #region IsConnected
        /// <summary>
        /// 获取是否连接状态
        /// </summary>
        public bool IsConnected
        {
            get { return mIsConnected; }
        }
        #endregion

        #endregion


        /// <summary>
        /// 创建NetSession
        /// </summary>
        public NetSession()
        {
            mIsSSL = false;
            mIsConnected = false;
            mMsgEncoding = (int)MessageEncoding.None;
            mEncryption = (int)EncryptionMode.None;
            mBufferedSize = 0;
            mReceiveBuffer = new byte[mMaxBufferSize];
            mBufferedData = new byte[mMaxBufferSize];
            mLastActiveTime = DateTime.Now;
            mSessionID = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 指定TcpClient创建NetSession
        /// </summary>
        /// <param name="tcpClient"></param>
        public NetSession(TcpClient tcpClient)
            : this()
        {
            mTcpClient = tcpClient;
            mRemoteEndpoint = mTcpClient.Client.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// 启动此会话
        /// </summary>
        public virtual void Start()
        {
            try
            {
                if (mIsSSL)
                {
                    if (Certificate == null)
                    {
                        OnDebug(LogMode.Error, string.Format("Server certificate is null"));
                        Stop();
                        return;
                    }
                    SslStream sslStream = new SslStream(mTcpClient.GetStream(), false, null);
                    sslStream.AuthenticateAsServer(Certificate, false, SslProtocols.Default, false);
                    mStream = sslStream;
                }
                else
                {
                    NetworkStream netStream = mTcpClient.GetStream();
                    mStream = netStream;
                }
                mIsConnected = true;
                OnDebug(LogMode.Info, string.Format("Client connected.\t{0}", mSessionID));
                mStream.BeginRead(mReceiveBuffer, 0, mHeadSize, ReceiveMessageWorker, mStream);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SessionStart fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 停止会话
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                mIsConnected = false;
                if (mStream != null)
                {
                    mStream.Close();
                }
                if (mTcpClient != null)
                {
                    mTcpClient.Close();
                }
                OnDebug(LogMode.Info, string.Format("Session stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Session stop fail.\t{0}", ex.Message));
            }
        }


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
                    mLastActiveTime = DateTime.Now;
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
        /// 向客户端发送消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public OperationReturn SendMessage(MessageHead head, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return SendMessage(head, data);
        }
        /// <summary>
        /// 向客户端发送回复消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">回复消息</param>
        public virtual OperationReturn SendMessage(MessageHead head, ReturnMessage message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                message.SessionID = mSessionID;
                head.Type = (int)MessageType.Response;
                head.Command = message.Command;
                switch (head.Encoding)
                {
                    case (int)MessageEncoding.None:
                    case (int)MessageEncoding.UTF8String:
                        var ret = message;
                        string strMessage = string.Empty;
                        //Result
                        strMessage += string.Format("{0}{1}", ret.Result ? "1" : "0", ConstValue.SPLITER_CHAR);
                        //Code
                        strMessage += string.Format("{0}{1}", ret.Code, ConstValue.SPLITER_CHAR);
                        //SessionID
                        strMessage += string.Format("{0}{1}", ret.SessionID, ConstValue.SPLITER_CHAR);
                        //Command
                        strMessage += string.Format("{0}{1}", ret.Command, ConstValue.SPLITER_CHAR);
                        //Message
                        strMessage += string.Format("{0}{1}", ret.Message, ConstValue.SPLITER_CHAR);
                        //Data
                        strMessage += string.Format("{0}{1}", ret.Data, ConstValue.SPLITER_CHAR);
                        //ListData
                        string strListData = string.Empty;
                        for (int i = 0; i < ret.ListData.Count; i++)
                        {
                            string msg = ret.ListData[i];
                            if (i < ret.ListData.Count - 1)
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
                        string strLog = string.Format("MessageEncoding type not support.\t{0}", head.Encoding);
                        OnDebug(LogMode.Error, strLog);
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                        optReturn.Message = strLog;
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
        /// 向客户端发送回复消息
        /// </summary>
        /// <param name="message">回复消息</param>
        /// <returns></returns>
        public virtual OperationReturn SendMessage(ReturnMessage message)
        {
            return SendMessage(GetMessageHead(), message);
        }
        /// <summary>
        /// 向客户端发送通知消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">通知消息</param>
        /// <returns></returns>
        public virtual OperationReturn SendMessage(MessageHead head, NotifyMessage message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                message.SessionID = mSessionID;
                head.Type = (int)MessageType.Notify;
                head.Command = message.Command;
                switch (head.Encoding)
                {
                    case (int)MessageEncoding.None:
                    case (int)MessageEncoding.UTF8String:
                        var ret = message;
                        string strMessage = string.Empty;
                        //SessionID
                        strMessage += string.Format("{0}{1}", ret.SessionID, ConstValue.SPLITER_CHAR);
                        //Command
                        strMessage += string.Format("{0}{1}", ret.Command, ConstValue.SPLITER_CHAR);
                        //Data
                        strMessage += string.Format("{0}{1}", ret.Data, ConstValue.SPLITER_CHAR);
                        //ListData
                        string strListData = string.Empty;
                        for (int i = 0; i < ret.ListData.Count; i++)
                        {
                            string msg = ret.ListData[i];
                            if (i < ret.ListData.Count - 1)
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
                        string strLog = string.Format("MessageEncoding type not support.\t{0}", head.Encoding);
                        OnDebug(LogMode.Error, strLog);
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                        optReturn.Message = strLog;
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
        /// 向客户端发送通知消息
        /// </summary>
        /// <param name="message">通知消息</param>
        /// <returns></returns>
        public virtual OperationReturn SendMessage(NotifyMessage message)
        {
            return SendMessage(GetMessageHead(), message);
        }

        #endregion


        #region 接收消息
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="result"></param>
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
                    ClientDisconnected();
                    return;
                }
            }
            catch (ObjectDisposedException) { return; }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EndReceive fail.\t{0}", ex.Message));
                ClientDisconnected();
                return;
            }
            try
            {
                mLastActiveTime = DateTime.Now;
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
                    DoClientMessage(msgHead, temp);
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
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("BeginReceive fail.\t{0}", ex.Message));
                ClientDisconnected();
            }
        }
        /// <summary>
        /// 处理客户端消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="data">消息数据</param>
        protected virtual void DoClientMessage(MessageHead head, byte[] data)
        {
            try
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
                if (head.Type == (int) MessageType.Request)
                {
                    string strMsg = Encoding.UTF8.GetString(temp);
                    DoClientMessage(head, strMsg);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoClientMessage fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 处理客户端消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="strMessage">消息内容</param>
        protected virtual void DoClientMessage(MessageHead head, string strMessage)
        {
            try
            {
                int encoding = head.Encoding;
                RequestMessage request;
                OperationReturn optReturn;
                int intValue;
                if (head.Type == (int) MessageType.Request)
                {
                    switch (encoding)
                    {
                        case (int)MessageEncoding.None:
                        case (int)MessageEncoding.UTF8String:
                            request = new RequestMessage();
                            string[] list = strMessage.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                            if (list.Length > 0)
                            {
                                if (!int.TryParse(list[0], out intValue))
                                {
                                    OnDebug(LogMode.Error, string.Format("RequestMessage command invalid.\t{0}", list[0]));
                                    return;
                                }
                                request.Command = intValue;
                            }
                            if (list.Length > 1)
                            {
                                request.SessionID = list[1];
                            }
                            if (list.Length > 2)
                            {
                                request.Data = list[2];
                            }
                            if (list.Length > 3)
                            {
                                string strListData = list[3];
                                string[] listData = strListData.Split(new[] { ConstValue.SPLITER_CHAR_2 },
                                    StringSplitOptions.None);
                                for (int i = 0; i < listData.Length; i++)
                                {
                                    request.ListData.Add(listData[i]);
                                }
                            }
                            break;
                        case (int)MessageEncoding.UTF8XML:
                            optReturn = XMLHelper.DeserializeObject<RequestMessage>(strMessage);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("Deserialize RequestMessage fail.\t{0}\t{1}", optReturn.Code,
                                        optReturn.Message));
                                return;
                            }
                            request = optReturn.Data as RequestMessage;
                            if (request == null)
                            {
                                OnDebug(LogMode.Error, string.Format("RequestMessage is null"));
                                return;
                            }
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Encoding invalid.\t{0}", mMsgEncoding));
                            return;
                    }
                    OnDebug(LogMode.Info, string.Format("Receive command:{0}", ParseCommand(request.Command)));

                    DoClientCommand(request);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoClientMessage fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 处理客户端命令
        /// </summary>
        /// <param name="request">客户端请求消息</param>
        protected virtual void DoClientCommand(RequestMessage request)
        {
            switch (request.Command)
            {
                case (int)RequestCode.NCHello:
                    SendWelcomeMessage();
                    break;
                case (int)RequestCode.NCHearbeat:
                    //此处无需做处理
                    OnDebug(LogMode.Debug, string.Format("HeartbeatMessage"));
                    break;
                case (int)RequestCode.NCLogon:
                    DealLogonMessage(request);
                    break;
            }
        }

        #endregion


        #region Others
        /// <summary>
        /// 根据当前会话信息创建消息头
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
        /// 解析命令码
        /// </summary>
        /// <param name="command">命令码</param>
        /// <returns></returns>
        protected virtual string ParseCommand(int command)
        {
            return ((RequestCode)command).ToString();
        }
        /// <summary>
        /// 连接断开，执行垃圾清理操作
        /// </summary>
        public virtual void ClientDisconnected()
        {
            mIsConnected = false;
        }
        /// <summary>
        /// 发送Welcome消息
        /// </summary>
        protected virtual void SendWelcomeMessage()
        {
            try
            {

                /*
                 * Welcome消息的内容定义
                 * 
                 * ListData：
                 * 0：       Message（欢迎消息）
                 * 1：       SessionID
                 * 2：       AuthCode（32个字节）
                 * 3：       Encoding
                 * 4：       EncryptionMode
                 * 5：       当前时间（UTC），可用于计算时间差
                 */

                ReturnMessage retMsg = new ReturnMessage();
                retMsg.Result = true;
                retMsg.Code = 0;
                retMsg.SessionID = mSessionID;
                retMsg.Command = (int)RequestCode.NCWelcome;
                retMsg.ListData.Add(string.Format("Welcome {0}!", mName));
                retMsg.ListData.Add(string.Format("{0}", mSessionID));
                retMsg.ListData.Add(GenerateAuthCode());
                retMsg.ListData.Add(string.Format("{0}", mMsgEncoding));
                retMsg.ListData.Add(string.Format("{0}", mEncryption));
                retMsg.ListData.Add(string.Format("{0}", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                SendMessage(retMsg);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendWelcomeMessage fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 处理认证消息
        /// </summary>
        /// <param name="request"></param>
        protected virtual void DealLogonMessage(RequestMessage request)
        {
            try
            {
                if (request.ListData.Count < 2)
                {
                    OnDebug(LogMode.Error, string.Format("Logon message invalid."));
                    return;
                }

                /*
                 * Auth消息的内容定义 
                 * 
                 *  ListData：（无）
                 *  
                 */

                string strVeriCode = request.ListData[1];
                string strAuthCode = GenerateAuthCode();
                string strTemp = Utils.GetVerificationCode(strAuthCode);
                ReturnMessage retMessage = new ReturnMessage();
                retMessage.SessionID = mSessionID;
                retMessage.Command = (int)RequestCode.NCAuth;
                if (strVeriCode == strTemp)
                {
                    retMessage.Result = true;

                    OnDebug(LogMode.Info, string.Format("Authenticated."));
                }
                else
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_AUTH_FAIL;
                    retMessage.Message = string.Format("VerificationCode error.");

                    OnDebug(LogMode.Error, string.Format("Authenticate fail.\tVerificationCode error."));
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealLogonMessage fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateAuthCode()
        {
            string strSession = mSessionID;
            if (EncryptObject == null)
            {
                OnDebug(LogMode.Error, string.Format("No EncryptObject, can not generate AuthCode."));
                return string.Empty;
            }
            return EncryptObject.EncryptString(strSession, (int)EncryptionMode.SHA256V00Hex);
        }

        #endregion


        #region Debug
        /// <summary>
        /// 调试消息
        /// </summary>
        public event Action<LogMode, string, string> Debug;

        protected void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        protected void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, mRemoteEndpoint, msg);
        }

        #endregion

    }
}
