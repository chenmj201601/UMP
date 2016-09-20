//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6dd47b73-bab5-4824-adc7-27ba0b2271c7
//        CLR Version:              4.0.30319.18444
//        Name:                     MediaClient
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03Demo
//        File Name:                MediaClient
//
//        created by Charley at 2015/3/26 17:19:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace UMPService03Demo
{
    public class MediaClient
    {
        #region Members

        private TcpClient mTcpClient;
        private SslStream mStream;
        private int mHeadSize = ConstValue.NET_MESSAGE_HEAD_SIZE;
        private int mMaxBufferSize = ConstValue.NET_BUFFER_MAX_SIZE;
        private byte[] mBuffer;
        private MessageHead mMessageHead;
        private MessageEncoding mMsgEncoding;
        private string mSessionID;

        #endregion

        #region SessionID

        public string SessionID
        {
            get { return mSessionID; }
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


        /// <summary>
        /// Create a MediaClient
        /// </summary>
        public MediaClient()
        {
            mMsgEncoding = MessageEncoding.None;
            mBuffer = new byte[mMaxBufferSize];
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
                    OnConnected(string.Format("Server connected"));
                    mStream.BeginRead(mBuffer, 0, mHeadSize, ReceiveHeadWorker, mStream);
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
        /// 向客户端发送消息
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
                            if (i < request.ListData.Count)
                            {
                                strListData += string.Format("{0}{1}", msg, ConstValue.SPLITER_CHAR_2);
                            }
                            else
                            {
                                strListData += string.Format("{0}", msg);
                            }
                            strMessage += string.Format("{0}", strListData);
                            SendMessage(head, strMessage);
                        }
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
        /// 向服务器发送消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public OperationReturn SendMessage(int command, RequestMessage message)
        {
            return SendMessage(GetMessageHead(command), message);
        }

        #endregion


        #region 接收消息

        private void ReceiveHeadWorker(IAsyncResult result)
        {
            try
            {
                SslStream stream = result.AsyncState as SslStream;
                if (stream == null)
                {
                    OnDebug(LogMode.Error, string.Format("NetworkStream is null"));
                    return;
                }
                int length = stream.EndRead(result);
                if (length != mHeadSize)
                {
                    OnDebug(LogMode.Error, string.Format("MessageHead length invalid"));
                    return;
                }
                //接收消息头
                MessageHead msgHead = (MessageHead)Converter.Bytes2Struct(mBuffer, typeof(MessageHead));
                int size = msgHead.Size;
                int command = msgHead.Command;
                //MessageEncoding encoding = msgHead.Encoding;
                //if (encoding != mMsgEncoding)
                //{
                //    OnDebug(LogMode.Info, string.Format("MessageEncoding changed.\t{0}\t{1}", mMsgEncoding, encoding));
                //    msgHead.Encoding = encoding;
                //    mMsgEncoding = encoding;
                //}
                mMessageHead = msgHead;
                OnDebug(LogMode.Info, string.Format("Message size:{0}\tCommand:{1}", size, command));
                stream.BeginRead(mBuffer, 0, size, ReceiveMessageWorker, stream);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ReceiveHead fail.\t{0}", ex.Message));
            }
        }

        private void ReceiveMessageWorker(IAsyncResult result)
        {
            try
            {
                SslStream stream = result.AsyncState as SslStream;
                if (stream == null)
                {
                    OnDebug(LogMode.Error, string.Format("NetworkStream is null"));
                    return;
                }
                int length = stream.EndRead(result);
                if (length != mMessageHead.Size)
                {
                    OnDebug(LogMode.Error, string.Format("Message size invalid\t{0}\t{1}", mMessageHead.Size, length));
                    return;
                }
                //接收消息内容
                string strMsg = Encoding.UTF8.GetString(mBuffer, 0, length);
                int command = mMessageHead.Command;
                OnDebug(LogMode.Info, string.Format("Receive command:{0}\tData:{1}", command, strMsg));
                DoServerMessage(mMessageHead, strMsg);
                stream.BeginRead(mBuffer, 0, mHeadSize, ReceiveHeadWorker, stream);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ReceiveMessage fail.\t{0}", ex.Message));
            }
        }

        private void DoServerMessage(MessageHead head, string strMessage)
        {
            try
            {
                int command = head.Command;
                MessageEncoding msgEncoding = head.Encoding;
                ReturnMessage retMessage;
                OperationReturn optReturn;
                int intValue;
                switch (msgEncoding)
                {
                    case MessageEncoding.None:
                    case MessageEncoding.UTF8String:
                        retMessage = new ReturnMessage();
                        string[] list = strMessage.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                        if (list.Length > 0)
                        {
                            retMessage.Result = list[0] == "1";
                        }
                        if (list.Length > 1)
                        {
                            if (!int.TryParse(list[1], out intValue))
                            {
                                OnDebug(LogMode.Error, string.Format("ReturnMessage code invalid.\t{0}", list[1]));
                                return;
                            }
                            retMessage.Code = intValue;
                        }
                        if (list.Length > 2)
                        {
                            retMessage.SessionID = list[2];
                        }
                        if (list.Length > 3)
                        {
                            retMessage.Message = list[3];
                        }
                        if (list.Length > 4)
                        {
                            retMessage.Data = list[4];
                        }
                        if (list.Length > 5)
                        {
                            string strListData = list[5];
                            string[] listData = strListData.Split(new[] { ConstValue.SPLITER_CHAR_2 },
                                StringSplitOptions.None);
                            for (int i = 0; i < listData.Length; i++)
                            {
                                retMessage.ListData.Add(listData[i]);
                            }
                        }
                        break;
                    case MessageEncoding.UTF8XML:
                        optReturn = XMLHelper.DeserializeObject<ReturnMessage>(strMessage);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("Deserialize RequestMessage fail.\t{0}\t{1}", optReturn.Code,
                                    optReturn.Message));
                            return;
                        }
                        retMessage = optReturn.Data as ReturnMessage;
                        if (retMessage == null)
                        {
                            OnDebug(LogMode.Error, string.Format("RequestMessage is null"));
                            return;
                        }
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Encoding invalid.\t{0}", mMsgEncoding));
                        return;
                }
                OnMessageReceived(command, retMessage);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoClientMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        public MessageHead GetMessageHead(int command)
        {
            MessageHead head = new MessageHead();
            head.Flag = "US";
            head.Encoding = mMsgEncoding;
            head.Encryption = EncryptionMode.None;
            head.State = MessageState.LastPacket;
            head.Command = command;
            return head;
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


        #region MessageReceived

        public event Action<int, ReturnMessage> MessageReceived;

        private void OnMessageReceived(int command, ReturnMessage retMessage)
        {
            //收到欢迎消息取得会话ID
            if (command == (int)Service03Command.Welcome)
            {
                mSessionID = retMessage.SessionID;
            }
            if (MessageReceived != null)
            {
                MessageReceived(command, retMessage);
            }
        }

        #endregion
    }
}
