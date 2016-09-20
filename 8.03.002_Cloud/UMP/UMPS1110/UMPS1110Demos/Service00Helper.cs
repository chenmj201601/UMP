//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    775b250f-380b-448e-9094-9f3dc1b38c33
//        CLR Version:              4.0.30319.18444
//        Name:                     Service00Helper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110Demos
//        File Name:                Service00Helper
//
//        created by Charley at 2015/3/7 18:00:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPS1110Demos
{
    public class Service00Helper
    {
        private Func<OperationReturn> mConnectDelegate;

        #region HostAddress

        private string mHostAddress;
        /// <summary>
        /// 服务地址
        /// </summary>
        public string HostAddress
        {
            get { return mHostAddress; }
            set
            {
                if (mHostAddress != value)
                {
                    mIsConnected = false;
                }
                mHostAddress = value;
            }
        }

        #endregion


        #region HostPort

        private int mHostPort;
        /// <summary>
        /// 服务端口
        /// </summary>
        public int HostPort
        {
            get { return mHostPort; }
            set
            {
                if (mHostPort != value)
                {
                    mIsConnected = false;
                }
                mHostPort = value;
            }
        }

        #endregion


        #region IsConnected

        private bool mIsConnected;
        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        public bool IsConnected
        {
            get { return mIsConnected; }
            //set { mIsConnected = value; }
        }

        #endregion


        #region DebugEvent
        /// <summary>
        /// 调试信息，可供异常判断
        /// </summary>
        public event Action<string> Debug;

        private void OnDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }

        #endregion


        #region Members

        private const int MAX_BUFFER_SIZE = 1024;
        private const int RECIEVE_TIMEOUT = 1800;    //接收消息的超时时间，单位100ms
        private const int CONNECT_TIMEOUT = 300;     //连接超时时间，单位100ms
        private const int CONNECT_HOLD_TIMEOUT = 300;       //连接维持的最长空闲时间，超过此时间内没有任何请求接收消息则自动关闭连接,单位1s
        private const int CHECK_INTERVAL = 10;      //检查连接的时间间隔，单位s

        private int mBufferSize;
        private byte[] mBufferData;
        private TcpClient mTcpClient;
        private SslStream mSslStream;
        private Thread mThreadReceive;
        private Thread mThreadCheck;
        private DateTime mLastActiveTime;
        private string mReturnMessage;
        private int mRecieveTimeout;
        private int mConnectTimeout;
        private int mConnectHoldTimeout;
        private bool mIsWaiting;

        #endregion


        /// <summary>
        /// 创建一个连接UMPService00服务的帮助器
        /// </summary>
        public Service00Helper()
        {
            mBufferData = new byte[MAX_BUFFER_SIZE];
            mRecieveTimeout = RECIEVE_TIMEOUT;
            mConnectTimeout = CONNECT_TIMEOUT;
            mConnectHoldTimeout = CONNECT_HOLD_TIMEOUT;
            mLastActiveTime = DateTime.Now;
            mHostAddress = "127.0.0.1";
            mHostPort = 8009;
            mIsConnected = false;
            mIsWaiting = false;

            mConnectDelegate = Connect;
        }


        #region Basic

        /// <summary>
        /// 启动帮助器
        /// </summary>
        /// <returns></returns>
        public OperationReturn Start()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                CreateRecieveThread();
                CreateCheckThread();
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
        /// 停止帮助器，在不用时调用以关闭Socket连接并清理资源
        /// </summary>
        /// <returns></returns>
        public OperationReturn Stop()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mThreadCheck != null)
                {
                    try
                    {
                        mThreadCheck.Abort();
                        mThreadCheck = null;
                    }
                    catch { }
                }
                if (mThreadReceive != null)
                {
                    try
                    {
                        mThreadReceive.Abort();
                        mThreadReceive = null;
                    }
                    catch { }
                }
                Close();
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


        #region Connect Operation

        private OperationReturn Connect()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mIsConnected)
                {
                    optReturn.Message = string.Format("Already connected.\t{0}\t{1}", mHostAddress, mHostPort);
                    return optReturn;
                }
                mTcpClient = new TcpClient(mHostAddress, mHostPort);
                if (mTcpClient.Connected)
                {
                    mSslStream = new SslStream(mTcpClient.GetStream(), false, (sender, cert, chain, errs) => true, null);
                    mSslStream.AuthenticateAsClient(mHostAddress);
                    mIsConnected = true;
                    mLastActiveTime = DateTime.Now;
                    OnDebug(string.Format("Server connected.\t{0}", mTcpClient.Client.RemoteEndPoint));
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_CONNECTED;
                    optReturn.Message = string.Format("TcpClient Connect fail.");
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private void Close()
        {
            mIsConnected = false;
            if (mSslStream != null)
            {
                try
                {
                    mSslStream.Close();
                    mSslStream.Dispose();
                    mSslStream = null;
                }
                catch { }
            }
            if (mTcpClient != null)
            {
                try
                {
                    mTcpClient.Close();
                    mTcpClient = null;
                    OnDebug(string.Format("Connection closed"));
                }
                catch { }
            }
        }

        private OperationReturn SendMessage(string msg)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                msg = string.Format("{0}\r\n", msg);
                byte[] data = Encoding.UTF8.GetBytes(msg);
                mSslStream.Write(data);
                mSslStream.Flush();
                mLastActiveTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format("SendMessage fail.\t{0}", ex.Message);
            }
            return optReturn;
        }

        #endregion


        #region ReceiveThread

        private void CreateRecieveThread()
        {
            try
            {
                if (mThreadReceive != null)
                {
                    try
                    {
                        mThreadReceive.Abort();
                        mThreadReceive = null;
                    }
                    catch { }
                }
                mThreadReceive = new Thread(ReceiveMessageWorker);
                mThreadReceive.Start();
                OnDebug(string.Format("Receive thread created\t{0}", mThreadReceive.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("CreateRecieveThread fail.\t{0}", ex.Message));
            }
        }

        private void ReceiveMessageWorker()
        {
            try
            {
                string strMessage = string.Empty;
                while (true)
                {
                    try
                    {
                        if (!mIsConnected
                            || mTcpClient == null
                            || !mTcpClient.Connected
                            || mSslStream == null)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        mBufferSize = mSslStream.Read(mBufferData, 0, MAX_BUFFER_SIZE);
                        if (mBufferSize == 0)
                        {
                            OnDebug(string.Format("Server disconnected"));
                            mIsConnected = false;
                            Thread.Sleep(1000);
                            continue;
                        }
                        mLastActiveTime = DateTime.Now;
                        string singleMessage = Encoding.UTF8.GetString(mBufferData, 0, mBufferSize);
                        //OnDebug(string.Format("SingleMessage:{0}", singleMessage));
                        strMessage += singleMessage;
                        //OnDebug(string.Format("TotalLength:{0}", strMessage.Length));
                        if (strMessage.EndsWith(string.Format("{0}End{0}\r\n", ConstValue.SPLITER_CHAR)))
                        {
                            strMessage = strMessage.Substring(0, strMessage.Length - 7);
                            if (strMessage.EndsWith("\r\n"))
                            {
                                strMessage = strMessage.Substring(0, strMessage.Length - 2);
                            }
                            mReturnMessage = strMessage;
                            strMessage = string.Empty;
                            OnDebug(string.Format("ReceiveMessage:{0}", mReturnMessage));
                            mIsWaiting = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnDebug(string.Format("ReceiveMessage fail.\t{0}", ex.Message));
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("ReceiveMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region CheckThread

        private void CreateCheckThread()
        {
            try
            {
                if (mThreadCheck != null)
                {
                    mThreadCheck.Abort();
                    mThreadCheck = null;
                }
                mThreadCheck = new Thread(CheckConnection);
                mThreadCheck.Start();
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("CreateCheckThread fail.\t{0}", ex.Message));
            }
        }

        private void CheckConnection()
        {
            try
            {
                while (true)
                {
                    DateTime now = DateTime.Now;
                    if ((now - mLastActiveTime).TotalSeconds > mConnectHoldTimeout)
                    {
                        Close();
                    }
                    OnDebug(string.Format("Check Connection"));
                    Thread.Sleep(1000 * CHECK_INTERVAL);
                }
            }
            catch (Exception ex)
            {
                OnDebug(string.Format("CheckConnection fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="listParams">参数组</param>
        /// <returns></returns>
        public OperationReturn DoOperation(string command, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                int intTimeout = 0;
                if (!mIsConnected)
                {
                    optReturn = mConnectDelegate.Invoke();
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    while (!mIsConnected && intTimeout < mConnectTimeout)
                    {
                        Thread.Sleep(100);
                        intTimeout++;
                    }
                    if (!mIsConnected)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_TIMEOUT;
                        optReturn.Message = string.Format("Connect timeout");
                        return optReturn;
                    }
                }
                string strMessage = string.Format("{0}", EncryptString(command));
                if (listParams != null && listParams.Count > 0)
                {
                    for (int i = 0; i < listParams.Count; i++)
                    {
                        strMessage += string.Format("{0}{1}", ConstValue.SPLITER_CHAR, EncryptString(listParams[i]));
                    }
                }
                OnDebug(string.Format("SendMessage:{0}", strMessage));
                optReturn = SendMessage(strMessage);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                mIsWaiting = true;
                intTimeout = 0;
                while (mIsWaiting && intTimeout < mRecieveTimeout)
                {
                    Thread.Sleep(100);
                    intTimeout++;
                }
                if (mIsWaiting)
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


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion

    }

    /// <summary>
    /// 请求指令
    /// </summary>
    public class RequestCommand
    {
        public const string GET_HOST_NAME = "G001";
        public const string GET_DISK_INFO = "G002";
        public const string GET_NETWORK_CARD = "G003";
        public const string GET_SUBDIRECTORY = "G004";
        public const string GET_CTI_SERVICENAME = "G006";
        public const string GET_SUBFILE = "G007";

        public const string SET_RESOURCE_CHANGED = "R001";
    }
}
