using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace UMPS3601.Models
{
    /// <summary>
    /// 连接Service03 的帮助类
    /// </summary>
    public class Service03Helper
    {

        #region Members

        private NetClient mNetClient;
        private string mHostAddress;
        private int mHostPort;
        private bool mIsConnected;
        private int mConnectTimeout = 10;   //连接超时时间，单位s
        private int mReceiveTimeout = 30;   //接收消息超时时间，单位s
        private int mCommand;
        private ReturnMessage mReturnMessage;
        private Func<OperationReturn> mConnectDeleage;
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
            bool bResult;
            try
            {
                if (mNetClient == null || !mNetClient.IsConnected)
                {
                    mIsConnected = false;
                    mNetClient = new NetClient();
                    mNetClient.MsgEncoding = (int)MessageEncoding.None;
                    mNetClient.Debug += OnDebug;
                    mNetClient.ConnectionEvent += (s, e) =>
                    {
                        if (e.Code == Defines.EVT_NET_CONNECTED)
                        {
                            mIsConnected = true;
                            mResetEvent.Set();
                        }
                    };
                    mNetClient.ReturnMessageReceived += MediaClient_ReturnMessageReceived;
                    mNetClient.Host = mHostAddress;
                    mNetClient.Port = mHostPort;
                    mNetClient.IsSSL = true;
                    mResetEvent.Reset();
                    mNetClient.Connect();
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
                mCommand = request.Command;
                mResetEvent.Reset();
                mNetClient.SendMessage(request);
                bResult = mResetEvent.WaitOne(mReceiveTimeout * 1000);
                if (!bResult)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_TIMEOUT;
                    optReturn.Message = string.Format("Receive timeout");
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
            if (mNetClient != null)
            {
                mNetClient.Stop();
            }
        }

        #endregion


        #region EventHandler

        void MediaClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            var ret = e.ReturnMessage;
            if (ret != null)
            {
                if (ret.Command == mCommand)
                {
                    mReturnMessage = e.ReturnMessage;
                    mResetEvent.Set();
                }
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
            OnDebug(mode, "Service03Helper", msg);
        }

        #endregion
    }
}
