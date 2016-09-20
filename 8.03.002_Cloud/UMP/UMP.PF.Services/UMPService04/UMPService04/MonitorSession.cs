//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    aff4e96f-a656-498e-a733-e7bb9184365f
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorSession
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                MonitorSession
//
//        created by Charley at 2015/6/23 13:15:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;

/*
 * ======================================================================
 * 
 * MonitorSession 工作逻辑
 * 
 * ***启动***
 * 1、根据TcpClient，创建网络流（SslStream）
 * 2、安全验证，对于服务端只需绑定安全证书，不用检查客户端证书
 * 3、接收消息（RequestMessage）
 * 4、处理接收到的消息，并返回回复消息（ReturnMessage）给客户端
 * 5、处理通知消息，并把通知消息发送给客户端
 * 
 * ***握手过程（暂时没有加入LogOn和Auth的登录验证功能）***
 * 1、客户端连接成功（安全验证通过）后，向服务器发送Hello消息
 * 2、服务器收到Hello消息后回复Welcome消息，其中附带服务器信息及分配的SessionID
 * 3、客户端收到Welcome消息后需要记下SessionID，后续的请求消息中都要带上这个SessionID
 * 握手成功后就可以发送请求消息（RequestMessage，并指定Command）给服务器了
 * 
 * 注：
 * 1、为了维持连接，每次接收或发送消息时都会更新LastActiveTime为当前最新时间
 * 2、为了维持连接，客户端需要定时向服务器发送心跳信息（Heartbeat）
 * 3、超过空闲时间，服务器会主动关闭连接
 * 
 * ======================================================================
 */

namespace UMPService04
{
    public partial class MonitorSession
    {

        #region Members

        public X509Certificate2 Certificate { get; set; }
        public string RootDir { get; set; }

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
        private DateTime mLastActiveTime;
        private string mRemoteEndpoint;

        #endregion


        #region Public Properties

        #region SessionID

        public string SessionID
        {
            get { return mSessionID; }
        }

        #endregion


        #region RemoteEndpoint

        public string RemoteEndpoint
        {
            get { return mRemoteEndpoint; }
        }

        #endregion


        #region LastActiveTime

        public DateTime LastActiveTime
        {
            get { return mLastActiveTime; }
        }

        #endregion


        #region Connected

        public bool Connected
        {
            get { return mIsConnected; }
        }

        #endregion

        #endregion


        /// <summary>
        /// 创建一个会话实例
        /// </summary>
        /// <param name="tcpClient"></param>
        public MonitorSession(TcpClient tcpClient)
        {
            mMsgEncoding = MessageEncoding.None;
            mReceiveBuffer = new byte[mMaxBufferSize];
            mBufferedData = new byte[mMaxBufferSize];
            mBufferedSize = 0;
            mLastActiveTime = DateTime.Now;
            mSessionID = Guid.NewGuid().ToString();
            mTcpClient = tcpClient;
            mRemoteEndpoint = tcpClient.Client.RemoteEndPoint.ToString();

            mListMonitorObjects = new List<MonitorObject>();
            mMonitorType = MonitorType.None;
        }


        #region Start and Stop
        /// <summary>
        /// 启动此会话
        /// </summary>
        public void Start()
        {
            try
            {
                if (Certificate == null)
                {
                    OnDebug(LogMode.Error, string.Format("Server certificate is null"));
                    Stop();
                    return;
                }
                mStream = new SslStream(mTcpClient.GetStream(), false, null);
                mStream.AuthenticateAsServer(Certificate, false, SslProtocols.Default, false);
                mIsConnected = true;
                OnDebug(LogMode.Info, string.Format("SSL connected.\t{0}", mSessionID));
                mStream.BeginRead(mReceiveBuffer, 0, mHeadSize, ReceiveMessageWorker, mStream);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SessionStart fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 停止此会话
        /// </summary>
        public void Stop()
        {
            try
            {
                mIsConnected = false;

                //关闭实时监听
                if (mNMonCore != null)
                {
                    mNMonCore.StopMon();
                    mNMonCore = null;
                }
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
        /// 向客户端发送消息
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
            }
            return optReturn;
        }
        /// <summary>
        /// 向客户端发送回复消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息对象</param>
        public OperationReturn SendMessage(MessageHead head, ReturnMessage message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                message.SessionID = mSessionID;
                head.Type = MessageType.Response;
                switch (head.Encoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
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
        /// 向客户端发送回复消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public OperationReturn SendMessage(int command, ReturnMessage message)
        {
            return SendMessage(GetMessageHead(), message);
        }
        /// <summary>
        /// 向客户端发送回复消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendMessage(ReturnMessage message)
        {
            return SendMessage(message.Command, message);
        }
        /// <summary>
        /// 向客户端发送通知消息
        /// </summary>
        /// <param name="head"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendMessage(MessageHead head, NotifyMessage message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                message.SessionID = mSessionID;
                head.Type = MessageType.Notify;
                switch (head.Encoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
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
        /// 向客户端发送通知消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendMessage(int command, NotifyMessage message)
        {
            return SendMessage(GetMessageHead(), message);
        }
        /// <summary>
        /// 向客户端发送通知消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendMessage(NotifyMessage message)
        {
            return SendMessage(message.Command, message);
        }
        /// <summary>
        /// 向客户端发送错误消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationReturn SendErrorMessage(ReturnMessage message)
        {
            return SendMessage((int)RequestCode.NCError, message);
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
                    OnDebug(LogMode.Error, string.Format("Receive 0 length message. Client disconnected."));
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
                    string strMessage = Encoding.UTF8.GetString(temp);
                    DoClientMessage(msgHead, strMessage);
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

        private void DoClientMessage(MessageHead head, string strMessage)
        {
            try
            {
                MessageEncoding encoding = head.Encoding;
                RequestMessage request;
                OperationReturn optReturn;
                int intValue;
                switch (encoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
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
                    case MessageEncoding.UTF8XML:
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
                switch (request.Command)
                {
                    case (int)RequestCode.NCHello:
                        SendWelcomeMessage();
                        break;
                    case (int)RequestCode.NCHearbeat:
                        //此处无需做处理
                        OnDebug(LogMode.Info, string.Format("HeartbeatMessage"));
                        break;
                }
                DoClientCommand(request);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoClientMessage fail.\t{0}", ex.Message));
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

        private void SendWelcomeMessage()
        {
            try
            {
                //Welcome消息返回的信息
                //ListData1：Welcome
                //ListData2：SessionID
                //ListData3：Encoding
                //ListData4：DateTime（now,UTC）,可用于计算时间偏差
                ReturnMessage retMsg = new ReturnMessage();
                retMsg.Result = true;
                retMsg.Code = 0;
                retMsg.SessionID = mSessionID;
                retMsg.Command = (int)RequestCode.NCWelcome;
                retMsg.ListData.Add(string.Format("Welcome UMPService04!"));
                retMsg.ListData.Add(string.Format("SessionID:{0}", mSessionID));
                retMsg.ListData.Add(string.Format("Encoding:{0}", mMsgEncoding));
                retMsg.ListData.Add(string.Format("{0}", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                SendMessage((int)RequestCode.NCWelcome, retMsg);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SendWelcomeMessage fail.\t{0}", ex.Message));
            }
        }

        private string ParseCommand(int command)
        {
            if (command > 10000)
            {
                return ((RequestCode)command).ToString();
            }
            return ((Service04Command)command).ToString();
        }

        #endregion


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, mRemoteEndpoint, msg);
        }

        #endregion
    }
}
