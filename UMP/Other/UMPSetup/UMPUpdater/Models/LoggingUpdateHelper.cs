//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    d150ab62-aa3e-4eb7-a0c6-56022b06693b
//        CLR Version:              4.0.30319.42000
//        Name:                     LoggingUpdateHelper
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                LoggingUpdateHelper
//
//        Created by Charley at 2016/9/7 13:29:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using VoiceCyber.UMP.Updates;


namespace UMPUpdater.Models
{
    public class LoggingUpdateHelper : INetServer
    {

        #region Members

        public const int SERVICE00_PORT = 8009;
        public const int CONNECT_TIMEOUT = 10;      //连接超时时间，10s
        public const int RECEIVE_TIMEOUT = 30;      //接收消息超时时间，30s

        public const string SERVICE00_COMMAND_U001 = "U001";

        public const string FILE_NAME_LOGGINGUPDATER = "LoggingUpdater.exe";


        public List<LoggingServerInfo> ListLoggingServers;
        public AppServerInfo AppServerInfo;
        public InstallInfo InstallInfo;
        public InstallProduct UMPProductInfo;

        private ManualResetEvent mResetEvent = new ManualResetEvent(false);
        private int mUpdatePort;
        private TcpListener mTcpListener;
        private string mToken;      //一个GUID串，每次执行DoUpdate的时候生成，用于标识与LoggingUpdater的一次连接

        private IList<INetSession> mListSessions;

        #endregion


        #region ListSessions

        public IList<INetSession> ListSessions
        {
            get { return mListSessions; }
        }

        #endregion


        public LoggingUpdateHelper()
        {
            mListSessions = new List<INetSession>();

            mUpdatePort = 8010;
        }


        #region Operations

        public void Start()
        {
            //Do noting
        }

        public void Stop()
        {
            Close();
        }

        public void DoUpdate()
        {
            try
            {
                if (ListLoggingServers == null) { return; }
                if (AppServerInfo == null) { return; }
                if (UMPProductInfo == null) { return; }

                mToken = Guid.NewGuid().ToString();
                OnDebug(LogMode.Info, string.Format("Token:{0}", mToken));
                bool result;


                #region 将LoggingUpdate.exe复制到http临时目录

                OnAppendMessageEvent(true, string.Format("Copying LoggingUpdater..."));
                string strPath = UMPProductInfo.InstallPath;
                string strSource = Path.Combine(strPath, FILE_NAME_LOGGINGUPDATER);
                if (!File.Exists(strSource))
                {
                    OnDebug(LogMode.Error, string.Format("LoggingUpdater file not exist.\t{0}", strSource));
                    return;
                }
                string strTargetPath = Path.Combine(strPath, ConstValue.TEMP_DIR_MEDIADATA);
                if (!Directory.Exists(strTargetPath))
                {
                    Directory.CreateDirectory(strTargetPath);
                }
                string strTarget = Path.Combine(strTargetPath, FILE_NAME_LOGGINGUPDATER);
                try
                {
                    File.Copy(strSource, strTarget, true);
                }
                catch (Exception ex)
                {
                    OnDebug(LogMode.Error, string.Format("Copy loggingupdater fail.\t{0}", ex.Message));
                    return;
                }
                OnAppendMessageEvent(true, string.Format("Copy LoggingUpdater end."));
                OnDebug(LogMode.Info, string.Format("Copy loggingupdater end.\t{0}", strTarget));

                #endregion


                #region 创建TcpListener侦听LoggingUpdater的连接

                OnAppendMessageEvent(true, string.Format("Starting UpdateServer..."));
                if (mTcpListener != null)
                {
                    try
                    {
                        mTcpListener.Stop();
                    }
                    catch { }
                    mTcpListener = null;
                }
                mTcpListener = new TcpListener(IPAddress.Any, mUpdatePort);
                try
                {
                    mTcpListener.Start();
                    OnDebug(LogMode.Info, string.Format("TcpListener started.\t{0}", mTcpListener.LocalEndpoint));
                    mTcpListener.BeginAcceptTcpClient(AcceptLoggingClient, mTcpListener);
                }
                catch (Exception ex)
                {
                    OnDebug(LogMode.Error, string.Format("TcpListener start fail.\t{0}", ex.Message));
                    return;
                }
                OnAppendMessageEvent(true, string.Format("UpdaterServer started."));

                #endregion


                for (int i = 0; i < ListLoggingServers.Count; i++)
                {
                    var loggingServer = ListLoggingServers[i];
                    string strAddress = loggingServer.HostAddress;
                    loggingServer.Progress = 0.0;
                    loggingServer.UpdateFlag = 0;


                    #region 生成各个LoggingServer的Token

                    string strToken = GenerateToken(loggingServer);
                    loggingServer.Token = strToken;
                    OnDebug(LogMode.Info, string.Format("LoggingServer {0} Token:{1}", strAddress, strToken));
                    string strMessage = string.Format("{0}", EncryptStringM004(SERVICE00_COMMAND_U001));

                    #endregion


                    #region 连接到Service00

                    OnAppendMessageEvent(true, string.Format("Connecting {0}...", strAddress));
                    TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                    SslStream stream;
                    try
                    {
                        OnDebug(LogMode.Info, string.Format("Connecting {0}...", strAddress));
                        tcpClient.Connect(strAddress, SERVICE00_PORT);
                        stream = new SslStream(tcpClient.GetStream(), false, (sender, cert, chain, errs) => true, null);
                        stream.AuthenticateAsClient(strAddress);
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("Connect to {0} fail. {1}", strAddress, ex.Message));
                        loggingServer.Progress = 100;
                        loggingServer.UpdateFlag = 2;       //失败
                        continue;
                    }
                    OnAppendMessageEvent(true, string.Format("LoggingServer {0} connected", strAddress));
                    OnDebug(LogMode.Info,
                        string.Format("Service00 {0} connected.\t{1}\t{2}", strAddress,
                            tcpClient.Client.LocalEndPoint, tcpClient.Client.RemoteEndPoint));
                    loggingServer.TcpClient = tcpClient;
                    loggingServer.Stream = stream;

                    #endregion


                    #region 发送并接收U001消息

                    OnAppendMessageEvent(true, string.Format("Sending update message to {0}...", strAddress));
                    List<string> args = new List<string>();
                    args.Add(string.Format("{0}", AppServerInfo.Address));      //Update Host
                    args.Add(string.Format("{0}", AppServerInfo.Port));         //Http Port
                    args.Add(string.Format("{0}", mUpdatePort));                //Update Port
                    args.Add(string.Format("{0}", FILE_NAME_LOGGINGUPDATER));   //FileName
                    args.Add(string.Format("{0}", strToken));   //Token
                    for (int j = 0; j < args.Count; j++)
                    {
                        args[j] = EncryptStringM004(args[j]);
                    }
                    for (int j = 0; j < args.Count; j++)
                    {
                        strMessage += string.Format("{0}{1}", ConstValue.SPLITER_CHAR, args[j]);
                    }
                    strMessage += "\r\n";
                    loggingServer.BufferedSize = 0;
                    loggingServer.RecieveMessage = string.Empty;
                    //异步接收消息
                    stream.BeginRead(loggingServer.Buffer, 0, ConstValue.NET_BUFFER_MAX_SIZE, Service00ReceiveCallback,
                        loggingServer);
                    mResetEvent.Reset();
                    //发送消息
                    try
                    {
                        byte[] data = Encoding.UTF8.GetBytes(strMessage);
                        stream.Write(data);
                        stream.Flush();
                        OnDebug(LogMode.Info, string.Format("Send message to {0} end.", strAddress));
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("Send message to {0} fail. {1}", strAddress, ex.Message));
                        loggingServer.Progress = 100;
                        loggingServer.UpdateFlag = 2;       //失败
                        continue;
                    }
                    result = mResetEvent.WaitOne(RECEIVE_TIMEOUT * 1000);
                    if (!result)
                    {
                        OnDebug(LogMode.Error, string.Format("Recieve message from {0} fail. Timeout!", strAddress));
                        loggingServer.Progress = 100;
                        loggingServer.UpdateFlag = 2;       //失败
                        continue;
                    }
                    //处理返回消息
                    string strMsg = loggingServer.RecieveMessage;
                    strMsg = DecryptStringM004(strMsg);
                    if (strMsg.StartsWith("Error"))
                    {
                        OnDebug(LogMode.Error, string.Format("Receive error message from {0}. {1}", strAddress, strMsg));
                        loggingServer.Progress = 100;
                        loggingServer.UpdateFlag = 2;       //失败
                        continue;
                    }
                    OnAppendMessageEvent(true, string.Format("LoggingServer {0} responsed", strAddress));

                    #endregion


                    OnDebug(LogMode.Info, string.Format("Receive message from {0} end. {1}", strAddress, strMsg));

                }

                if (!CheckComplete())
                {
                    OnAppendMessageEvent(true, string.Format("Updating LoggingServer..."));
                    mResetEvent.Reset();
                    result = mResetEvent.WaitOne();
                    if (!result)
                    {
                        OnDebug(LogMode.Error, string.Format("Wait for loggingServer update complete timeout !"));
                        return;
                    }
                }

                OnAppendMessageEvent(true, string.Format("LoggingServer update complete"));
                OnDebug(LogMode.Info, string.Format("LoggingServer update complete"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private bool CheckComplete()
        {
            bool isComplete = true;
            if (ListLoggingServers == null)
            {
                return true;
            }
            for (int i = 0; i < ListLoggingServers.Count; i++)
            {
                var server = ListLoggingServers[i];
                isComplete = isComplete && (server.UpdateFlag == 1 || server.UpdateFlag == 2);
            }
            return isComplete;
        }

        public void Close()
        {
            try
            {
                for (int i = mListSessions.Count - 1; i >= 0; i--)
                {
                    var session = mListSessions[i];
                    session.Stop();
                    mListSessions.Remove(session);
                }
                if (mTcpListener != null)
                {
                    try
                    {
                        mTcpListener.Stop();
                    }
                    catch { }
                    mTcpListener = null;
                }
                if (ListLoggingServers != null)
                {
                    for (int i = 0; i < ListLoggingServers.Count; i++)
                    {
                        var loggingServer = ListLoggingServers[i];
                        var stream = loggingServer.Stream;
                        if (stream != null)
                        {
                            try
                            {
                                stream.Close();
                            }
                            catch { }
                            stream.Dispose();
                        }
                        var tcpClient = loggingServer.TcpClient;
                        if (tcpClient != null)
                        {
                            try
                            {
                                tcpClient.Close();
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Close fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Callback

        private void AcceptLoggingClient(IAsyncResult result)
        {
            TcpListener listener = result.AsyncState as TcpListener;
            if (listener == null) { return; }
            TcpClient client;
            try
            {
                client = listener.EndAcceptTcpClient(result);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EndAcceptClient fail.\t{0}", ex.Message));
                return;
            }
            try
            {
                OnDebug(LogMode.Info, string.Format("New client connected.\t{0}", client.Client.RemoteEndPoint));
                LoggingUpdateSession session = new LoggingUpdateSession(client);
                session.Debug += OnDebug;
                session.ProgressEvent += (s, p) => DealLoggingUpdateProgress();
                session.ListLoggingServers = ListLoggingServers;
                session.InstallInfo = InstallInfo;
                mListSessions.Add(session);
                session.Start();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("AcceptClient fail.\t{0}", ex.Message));
            }
            try
            {
                listener.BeginAcceptTcpClient(AcceptLoggingClient, listener);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("BeginAcceptClient fail.\t{0}", ex.Message));
            }
        }

        private void Service00ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var loggingServer = result.AsyncState as LoggingServerInfo;
                if (loggingServer == null) { return; }
                string strAddress = loggingServer.HostAddress;
                int bufferedSize = loggingServer.BufferedSize;
                string strMessage = loggingServer.RecieveMessage;
                var stream = loggingServer.Stream;
                if (stream == null) { return; }
                bool bEnd = false;
                int intSize = stream.EndRead(result);
                if (intSize <= 0)
                {
                    OnDebug(LogMode.Error, string.Format("Service00 recieve callback fail.\tReceive 0 length message"));
                    return;
                }
                string strMsg = Encoding.UTF8.GetString(loggingServer.Buffer, bufferedSize, intSize);
                strMessage += strMsg;
                if (strMessage.EndsWith(string.Format("{0}End{0}\r\n", ConstValue.SPLITER_CHAR)))
                {
                    strMessage = strMessage.Substring(0, strMessage.Length - 7).TrimEnd('\r', '\n');
                    bEnd = true;
                }
                bufferedSize += intSize;
                loggingServer.BufferedSize = bufferedSize;
                loggingServer.RecieveMessage = strMessage;
                if (bEnd)
                {
                    OnDebug(LogMode.Info,
                        string.Format("LoggingServer {0} receive message {1}", strAddress, loggingServer.RecieveMessage));
                    mResetEvent.Set();
                }
                else
                {
                    stream.BeginRead(loggingServer.Buffer, bufferedSize, ConstValue.NET_BUFFER_MAX_SIZE - bufferedSize,
                        Service00ReceiveCallback, loggingServer);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Service00 recieve callback fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region ProgressEvent

        private void DealLoggingUpdateProgress()
        {
            try
            {
                if (ListLoggingServers == null) { return; }
                int count = ListLoggingServers.Count;
                if (count <= 0) { return; }
                double p = 0.0;
                bool isComplete = true;
                for (int i = 0; i < ListLoggingServers.Count; i++)
                {
                    var server = ListLoggingServers[i];
                    p += server.Progress / (count * 1.0);
                    isComplete = isComplete && (server.UpdateFlag == 1 || server.UpdateFlag == 2);
                }
                OnProgressEvent(p);
                if (isComplete)
                {
                    mResetEvent.Set();
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealLoggingUpdateProgress fail.\t{0}", ex.Message));
            }
        }

        public event Action<double> ProgressEvent;

        private void OnProgressEvent(double progress)
        {
            if (ProgressEvent != null)
            {
                ProgressEvent(progress);
            }
        }

        #endregion


        #region AppendMessageEvent

        public event Action<bool, string> AppendMessageEvent;

        private void OnAppendMessageEvent(bool isCurrentOpt, string msg)
        {
            if (AppendMessageEvent != null)
            {
                AppendMessageEvent(isCurrentOpt, msg);
            }
        }

        #endregion


        #region Others

        private string GenerateToken(LoggingServerInfo info)
        {
            string strCode = string.Format("{0}{1}", mToken, info.HostAddress);
            return ServerHashEncryption.EncryptString(strCode, EncryptionMode.SHA256V00Hex);
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
            OnDebug(mode, "LogUpHelper", msg);
        }

        #endregion


        #region Encryption

        public static string EncryptStringM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptStringM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string EncryptStringM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptStringM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        #endregion

    }
}
