//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3729b451-b066-4592-90a2-3513cda3b19d
//        CLR Version:              4.0.30319.18444
//        Name:                     Service03Helper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03Demo
//        File Name:                Service03Helper
//
//        created by Charley at 2015/3/30 10:31:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;

namespace UMPService03Demo
{
    public class Service03Helper
    {

        #region Members

        private NetClient mMediaClient;
        private string mHostAddress;
        private int mHostPort;
        private bool mIsConnected;
        private bool mIsWaiting;
        private int mConnectTimeout = 10;   //连接超时时间，单位s
        private int mReceiveTimeout = 30;   //接收消息超时时间，单位s
        private int mCommand;
        private ReturnMessage mReturnMessage;
        private Func<OperationReturn> mConnectDelegate;

        #endregion


        #region Public Properties

        /// <summary>
        /// 指定服务器地址
        /// </summary>
        public string HostAddress
        {
            set { mHostAddress = value; }
        }
        /// <summary>
        /// 指定端口
        /// </summary>
        public int HostPort
        {
            set { mHostPort = value; }
        }

        #endregion
       

        /// <summary>
        /// 创建一个Service03Helper
        /// </summary>
        public Service03Helper()
        {
            mIsConnected = false;
        }


        #region Public Functions
        /// <summary>
        /// 请求消息
        /// </summary>
        /// <param name="request">参数</param>
        /// <returns></returns>
        public OperationReturn DoRequest(RequestMessage request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                int count;
                if (mMediaClient == null || !mMediaClient.IsConnected)
                {
                    mIsConnected = false;
                    mMediaClient = new NetClient();
                    mMediaClient.MsgEncoding = (int)MessageEncoding.UTF8XML;
                    mMediaClient.Debug += mMediaClient_Debug;
                    mMediaClient.ConnectionEvent += MediaClient_ConnectionEvent;
                    mMediaClient.ReturnMessageReceived += MediaClient_ReturnMessageReceived;
                    mMediaClient.IsSSL = true;
                    count = 0;
                    mConnectDelegate = () => mMediaClient.Connect(mHostAddress, mHostPort);
                    optReturn = mConnectDelegate.Invoke();
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    while (!mIsConnected && count < mConnectTimeout * 10)
                    {
                        count++;
                        Thread.Sleep(100);
                    }
                    if (count >= mConnectTimeout * 10)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_TIMEOUT;
                        optReturn.Message = string.Format("Connect to server timeout.\t{0}:{1}", mHostAddress, mHostPort);
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
                mIsWaiting = true;
                count = 0;
                mCommand = request.Command;
                optReturn = mMediaClient.SendMessage(mCommand, request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                while (mIsWaiting && count < mReceiveTimeout * 10)
                {
                    count++;
                    Thread.Sleep(100);
                }
                if (count >= mReceiveTimeout * 10)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_TIMEOUT;
                    optReturn.Message = string.Format("Receive message timeout");
                    return optReturn;
                }
                optReturn.Data = mReturnMessage;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
       
        /// <summary>
        /// 关闭，回收资源
        /// </summary>
        public void Close()
        {
            if (mMediaClient != null)
            {
                mMediaClient.Stop();
            }
        }

        #endregion


        #region Others

        void MediaClient_ReturnMessageReceived(object sender,ReturnMessageReceivedEventArgs e)
        {
            //if (mCommand == e.ReturnMessage.Command)
            //{
            //    mReturnMessage = e.ReturnMessage;
            //    mIsWaiting = false;
            //}
            mReturnMessage = e.ReturnMessage;
            mIsWaiting = false;
        }

        void MediaClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            if (e.Code == Defines.EVT_NET_CONNECTED)
            {
                mIsConnected = true;
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

        void mMediaClient_Debug(LogMode mode, string cat, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, string.Format("{0}\t{1}", cat, msg));
            }
        }

        #endregion
    }
}
