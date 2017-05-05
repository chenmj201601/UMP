//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    efd715df-22c3-44a9-b9d1-97a71c3d40c3
//        CLR Version:              4.0.30319.42000
//        Name:                     DecHelper
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                DecPublishDemo
//        File Name:                DecHelper
//
//        Created by Charley at 2016/10/20 17:14:56
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.SDKs.DEC;


namespace DecPublishDemo
{
    public class DecHelper
    {

        #region Members

        private DecConnector mDecConnector;
        private string mHostAddress;
        private int mHostPort;
        private bool mIsConnected;
        private int mConnectTimeout = 10;   //连接超时时间，单位s
        private int mReceiveTimeout = 60;   //接收消息超时时间，单位s
        private ManualResetEvent mResetEvent = new ManualResetEvent(false);

        #endregion


        #region HostAddress

        /// <summary>
        /// 指定服务器地址
        /// </summary>
        public string HostAddress
        {
            set { mHostAddress = value; }
        }

        #endregion


        #region HostPort

        /// <summary>
        /// 指定端口
        /// </summary>
        public int HostPort
        {
            set { mHostPort = value; }
        }

        #endregion


        #region Connected

        /// <summary>
        /// 获取当前是否连接上服务器
        /// </summary>
        public bool Connected
        {
            get { return mIsConnected; }
        }

        #endregion


        public DecHelper()
        {
            mIsConnected = false;
        }


        #region Operations

        public OperationReturn PublishMessage(string content, MessageString message)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            bool bResult;
            try
            {
                if (mDecConnector == null
                    || !mDecConnector.IsConnected)
                {
                    mIsConnected = false;
                    mDecConnector = new DecConnector();
                    mDecConnector.Debug += msg => OnDebug(LogMode.Debug, "DECConnector", msg);
                    mDecConnector.MessageReceivedEvent += DecConnector_MessageReceivedEvent;
                    mDecConnector.ServerConnectedEvent += DecConnector_ServerConnectedEvent;
                    mDecConnector.AppName = string.Format("Wcf21061");
                    mDecConnector.Host = mHostAddress;
                    mDecConnector.Port = mHostPort;
                    mDecConnector.ModuleNumber = 0;
                    mDecConnector.ModuleType = 2106;

                    mDecConnector.DataEncrypt = 0;
                    mDecConnector.DataFormat = DecDefines.NETPACK_BASEHEAD_VER1_FORMAT_XML;

                    mResetEvent.Reset();
                    mDecConnector.Connect();
                    bResult = mResetEvent.WaitOne(mConnectTimeout * 1000);
                    if (!bResult)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_TIMEOUT;
                        optReturn.Message = string.Format("Connect timeout");
                        return optReturn;
                    }
                }
                if (!mIsConnected)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_CONNECTED;
                    optReturn.Message = string.Format("Not connect to server");
                    return optReturn;
                }

                //mResetEvent.Reset();
                //mDecConnector.PublishMessage(content, message);
                //bResult = mResetEvent.WaitOne(mReceiveTimeout * 1000);
                //if (!bResult)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_TIMEOUT;
                //    optReturn.Message = string.Format("Recieve timeout");
                //    return optReturn;
                //}

                mDecConnector.PublishMessage(content, message);
                Thread.Sleep(2000);     //等待2s
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private void DecConnector_ServerConnectedEvent(bool isConnected, string msg)
        {
            OnDebug(LogMode.Info, string.Format("DecConnector_ConnectEvent\t{0}", msg));
            if (isConnected)
            {
                mIsConnected = true;
                mResetEvent.Set();
            }
        }

        private void DecConnector_MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {

        }

        public void Close()
        {
            if (mDecConnector != null)
            {
                mDecConnector.Close();
                mDecConnector = null;
            }
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
            OnDebug(mode, "DecHelper", msg);
        }

        #endregion


        #region Defines


        #region 消息类型

        public const string MSG_VOC_HEARTBEAT = "8002:0001:0001:0003";
        public const string MSG_VOC_RECORDSTARTED = "8002:0001:0002:0003";
        public const string MSG_VOC_RECORDSTOPPED = "8002:0001:0002:0004";
        public const string MSG_VOC_CALLINFO = "8002:0001:0002:0009";
        public const string MSG_VOC_AGENTLOGON = "8002:0001:0002:000A";
        public const string MSG_VOC_AGENTLOGOFF = "8002:0001:0002:000B";
        public const string MSG_VOC_CHANNELCONNECTED = "8002:0001:0002:000E";
        public const string MSG_VOC_CHANNELDISCONNECTED = "8002:0001:0002:000F";

        public const string MSG_SCR_HEARTBEAT = "8003:0001:0001:0003";
        public const string MSG_SCR_RECORDSTARTED = "8003:0001:0002:0001";
        public const string MSG_SCR_RECORDSTOPPED = "8003:0001:0002:0002";
        public const string MSG_SCR_AGENTLOGON = "8003:0001:0002:0004";
        public const string MSG_SCR_AGENTLOGOFF = "8003:0001:0002:0005";

        #endregion


        #region 节点

        public const string NODE_MESSAGE = "Message";
        public const string NODE_DEVICEINFOMATION = "DeviceInformation";
        public const string NODE_DEVICEINFOMATION2 = "DeviceInfomation";        //由于822版本的录音系统把单词Information拼写错了，写成了infomation，这里做一个兼容处理，是Service04仍然支持822的录音系统
        public const string NODE_RECORDINFO = "recordinfo";
        public const string NODE_RECORDORIGINALDATA = "recordoriginaldata";
        public const string NODE_CALLERID = "callerid";
        public const string NODE_CALLEDID = "calledid";
        public const string NODE_DIRECTIONFLAG = "directionflag";
        public const string NODE_CALLINFOAGENTID = "agentid";
        public const string NODE_REALEXTENSION = "realextension";

        public const string NODE_FILEPARAM = "FileParam";
        public const string NODE_RECOVERCHANNELS = "RecoverChannels";
        public const string NODE_RECOVERCHANNEL = "RecoverChannel";

        #endregion


        #region 属性

        public const string ATTR_MESSAGEID = "MessageID";
        public const string ATTR_CURRENTTIME = "CurrentTime";
        public const string ATTR_VOICEID = "VoiceID";
        public const string ATTR_SCRSVRID = "ScrSvrID";
        public const string ATTR_CHANNELID = "ChannelID";
        public const string ATTR_DEVICEID = "DeviceID";
        public const string ATTR_DEVICETYPE = "DeviceType";
        public const string ATTR_AGENTID = "AgentID";
        public const string ATTR_RECORDREFERENCE = "RecordReference";
        public const string ATTR_RECORDTIME = "RecordTime";
        public const string ATTR_RECORDLENGTH = "RecordLength";

        public const string ATTR_VALUE = "value";

        public const string ATTR_FILEPATH = "FilePath";
        public const string ATTR_TIMEFROM = "TimeFrom";
        public const string ATTR_TIMETO = "TimeTo";
        public const string ATTR_USEINDTIME = "UseIndTime";
        public const string ATTR_ORIGINALVOICENUMBER = "OriginalVoiceNumber";
        public const string ATTR_ORIGINALCHANNELID = "OriginalChannelID";

        #endregion


        #endregion

    }
}
