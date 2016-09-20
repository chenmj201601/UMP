using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace UMPS3106.Models
{
    /// <summary>
    /// 连接Service03 的帮助类
    /// </summary>
    public class Service03Helper
    {

        #region Members

        private MediaClient mMediaClient;
        private string mHostAddress;
        private int mHostPort;
        private bool mIsConnected;
        private bool mIsWaiting;
        private int mConnectTimeout = 10;   //连接超时时间，单位s
        private int mReceiveTimeout = 30;   //接收消息超时时间，单位s
        private int mCommand;
        private ReturnMessage mReturnMessage;
        private Func<OperationReturn> mConnectDeleage;

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
        /// 获取当前是否连接上Service03服务
        /// </summary>
        public bool Connected
        {
            get { return mIsConnected; }
        }

        #endregion


        /// <summary>
        /// 创建一个Service03Helper
        /// </summary>
        public Service03Helper()
        {
            mIsConnected = false;
        }


        #region Operations

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
                    mMediaClient = new MediaClient();
                    mMediaClient.MessageEncoding = MessageEncoding.UTF8XML;
                    mMediaClient.Debug += OnDebug;
                    mMediaClient.Connected += msg => mIsConnected = true;
                    mMediaClient.ReturnMessageReceived += MediaClient_MessageReceived;
                    count = 0;
                    mConnectDeleage = () => mMediaClient.Connect(mHostAddress, mHostPort);
                    optReturn = mConnectDeleage.Invoke();
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

        #endregion


        #region Close

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


        #region EventHandler

        void MediaClient_MessageReceived(ReturnMessage retMessage)
        {
            if (mCommand == retMessage.Command)
            {
                mReturnMessage = retMessage;
                mIsWaiting = false;
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
    }
}
