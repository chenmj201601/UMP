//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    ba1e0941-7273-40d1-ae71-17a5d3356808
//        CLR Version:              4.0.30319.42000
//        Name:                     UpdateEngine
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                LoggingUpdater
//        File Name:                UpdateEngine
//
//        Created by Charley at 2016/9/8 17:40:49
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;


namespace LoggingUpdater
{

    public class UpdateEngine
    {

        #region Members

        private int mUpdateMode;        //升级模式：1：指定升级文件升级；2：连接UMPUpdater在线自动升级
        private NetClient mUpdateClient;
        private DateTime mLastActiveTime;
        private string mUpdateFile;
        private string mUpdateHost;
        private int mUpdatePort;
        private string mUpdateToken;
        private Stream mUpdateFileStream;
        private long mUpdateFileTotalSize;
        private long mTransferedSize;
        private double mUpdateProgress;
        private int mUpdateFlag;
        private Thread mThreadNotifyProgress;
        private int mNotifyProgressInterval = 3;        //通知频率，3s

        #endregion


        #region LastActiveTime

        public DateTime LastActiveTime
        {
            get { return mLastActiveTime; }
        }

        #endregion


        public UpdateEngine()
        {
            mUpdateMode = 1;
            mLastActiveTime = DateTime.Now;
            mUpdateProgress = 0.0;
            mUpdateFlag = 0;
        }


        public void Start(string[] args)
        {
            try
            {
                //Args 说明
                //0     升级模式1                  升级模式2
                //
                //1     升级文件路径               主机地址   
                //2                                升级端口 
                //3                                验证码（Token）
                mUpdateMode = 1;
                if (args == null
                    || args.Length < 1)
                {
                    OnDebug(LogMode.Error, "EngineStart", string.Format("Args count invalid"));
                    Stop();
                    return;
                }
                string strMode;
                string strUpdateFile = string.Empty;
                string strUpdateHost = string.Empty;
                string strUpdatePort;
                string strUpdateToken = string.Empty;
                int intUpdateMode;
                int intUpdatePort = 0;
                strMode = args[0];
                if (!int.TryParse(strMode, out intUpdateMode))
                {
                    OnDebug(LogMode.Error, "EngineStart", string.Format("Args UpdateMode invalid."));
                    Stop();
                    return;
                }
                OnDebug(LogMode.Info, string.Format("UpdateMode:{0}", intUpdateMode));
                if (intUpdateMode == 1)
                {
                    if (args.Length < 2)
                    {
                        OnDebug(LogMode.Error, "EngineStart", string.Format("Args count invalid"));
                        Stop();
                        return;
                    }
                    strUpdateFile = args[1];
                    OnDebug(LogMode.Info, string.Format("Args\tUpdateFile:{0}", strUpdateFile));
                }
                else if (intUpdateMode == 2)
                {
                    if (args.Length < 4)
                    {
                        OnDebug(LogMode.Error, "EngineStart", string.Format("Args count invalid"));
                        Stop();
                        return;
                    }
                    strUpdateHost = args[1];
                    strUpdatePort = args[2];
                    strUpdateToken = args[3];
                    if (!int.TryParse(strUpdatePort, out intUpdatePort))
                    {
                        OnDebug(LogMode.Error, "EngineStart", string.Format("Args Port invalid."));
                        Stop();
                        return;
                    }
                    OnDebug(LogMode.Info, string.Format("Args\tHost:{0};Port:{1};Token:{2}", strUpdateHost, intUpdatePort, strUpdateToken));
                }
                else
                {
                    OnDebug(LogMode.Error, "EngineStart", string.Format("Args UpdateMode not support"));
                    Stop();
                    return;
                }
                mUpdateMode = intUpdateMode;
                mUpdateFile = strUpdateFile;
                mUpdateHost = strUpdateHost;
                mUpdatePort = intUpdatePort;
                mUpdateToken = strUpdateToken;
                mLastActiveTime = DateTime.Now;
                if (mUpdateMode == 2)
                {
                    CreateUpdateClient();
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, "EngineStart", string.Format("Start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            StopNotifyProgressThread();
            if (mUpdateClient != null)
            {
                mUpdateClient.Stop();
            }
            mUpdateClient = null;
            if (mUpdateFileStream != null)
            {
                try
                {
                    mUpdateFileStream.Close();
                }
                catch { }
            }
            mUpdateFileStream = null;
            Program.IsWorking = false;
        }


        #region 创建UpdateClient连接到UMPUpdater

        private void CreateUpdateClient()
        {
            try
            {
                if (mUpdateClient != null)
                {
                    mUpdateClient.Stop();
                    mUpdateClient = null;
                }
                mUpdateClient = new NetClient();
                mUpdateClient.Debug += (mode, cat, msg) => OnDebug(mode, "UpdateClient", string.Format("{0}\t{1}", cat, msg));
                mUpdateClient.ConnectionEvent += mUpdateClient_ConnectionEvent;
                mUpdateClient.MessageReceived += mUpdateClient_MessageReceived;
                mUpdateClient.ReturnMessageReceived += mUpdateClient_ReturnMessageReceived;
                mUpdateClient.Host = mUpdateHost;
                mUpdateClient.Port = mUpdatePort;
                ThreadPool.QueueUserWorkItem(a => mUpdateClient.Connect());

                OnDebug(LogMode.Info, "UpdateClient", string.Format("CreateUpdateClient End"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, "UpdateClient", string.Format("CreateUpdateClient Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations

        private void DownloadUMPData()
        {
            try
            {
                if (mUpdateClient == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mUpdateClient.SessionID;
                request.Command = (int)LoggingUpdateCommand.DownloadUMPData;
                request.ListData.Add(mUpdateToken);
                mUpdateClient.SendMessage(request);
                OnDebug(LogMode.Info, string.Format("DownloadUMPData request sended.\t{0}", mUpdateToken));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DownloadUMPData fail.\t{0}", ex.Message));
            }
        }

        private void DealDownloadUMPData(ReturnMessage retMessage)
        {
            try
            {
                var listArgs = retMessage.ListData;
                if (listArgs == null
                    || listArgs.Count < 2)
                {
                    OnDebug(LogMode.Error, string.Format("DealDownloadUMPData\tFail.\tParam count invalid."));
                    return;
                }
                string strTotalSize = listArgs[0];
                string strFileName = listArgs[1];
                OnDebug(LogMode.Info,
                  string.Format("DealDownloadUMPData\tTotalSize:{0}\tFileName:{1}", strTotalSize, strFileName));
                long size;
                if (!long.TryParse(strTotalSize, out size))
                {
                    OnDebug(LogMode.Error, string.Format("Totalsize invalid.\t{0}", strTotalSize));
                    return;
                }
                mUpdateFileTotalSize = size;
                mTransferedSize = 0;
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "UMP\\LoggingUpdater\\Temp");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, strFileName);
                if (File.Exists(strFile))
                {
                    File.Delete(strFile);
                }
                mUpdateFile = strFile;
                mUpdateFileStream = new FileStream(strFile, FileMode.CreateNew, FileAccess.Write);
                OnDebug(LogMode.Info, string.Format("Begin write data to UpdateFile.\t{0}\t{1}", mUpdateFile, mUpdateFileTotalSize));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealDownloadUMPData fail.\t{0}", ex.Message));
            }
        }

        private void DealTransferUMPData(MessageHead msgHead, byte[] data)
        {
            try
            {
                mLastActiveTime = DateTime.Now;
                int intFlag = msgHead.State;
                int intSize = msgHead.Size;
                if (mUpdateFileStream == null)
                {
                    OnDebug(LogMode.Error, string.Format("DealTransferUMPData\tFail.\tUpdateFileStream is null."));
                    return;
                }
                if (intSize > 0)
                {
                    mTransferedSize += intSize;

                    mUpdateFileStream.Write(data, 0, intSize);
                    mUpdateFileStream.Flush();


                    #region 计算进度

                    var p = mTransferedSize / (mUpdateFileTotalSize * 1.0);
                    //var a = (p * LoggingUpdateConsts.PRO_DOWNLOADUPDATEFILE) +
                    //        LoggingUpdateConsts.PRO_BASE_DOWNLOADUPDATEFILE;
                    var a = p * 100;
                    mUpdateProgress = a;

                    #endregion

                }
                if ((intFlag & 2) > 0)
                {
                    mUpdateFileStream.Close();
                    OnDebug(LogMode.Info, string.Format("End write data to UpdateFile."));

                    //完成更新
                    mUpdateFlag = 1;
                    mUpdateProgress = 100;
                    OnNotifyProgress();
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealTransferUMPData fail.\t{0}", ex.Message));
            }
        }

        private void DealNotifyProgress(ReturnMessage retMessage)
        {
            try
            {
                //如果工作已经完成，就可以终止程序得来
                if (mUpdateProgress >= 100)
                {
                    Program.IsWorking = false;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealNotifyProgress fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region UpdateClientEvent

        void mUpdateClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var msgHead = e.Head;
            if (msgHead.Type == (int)MessageType.RawData)
            {
                int command = msgHead.Command;
                switch (command)
                {
                    case (int)LoggingUpdateCommand.TransferUMPData:
                        DealTransferUMPData(msgHead, e.Data);
                        break;
                }
            }
        }

        void mUpdateClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            //收到消息，刷新LastActiveTime
            mLastActiveTime = DateTime.Now;
            var retMessage = e.ReturnMessage;
            if (retMessage == null) { return; }
            OnDebug(LogMode.Info, "UpdateClient",
                string.Format("ReturnMessage\tName:{0};\tCommand:{1};\tResult:{2};\tCode:{3};\tMessage:{4}", e.Name,
                    retMessage.Command, retMessage.Result,
                    retMessage.Code, retMessage.Message));
            if (!retMessage.Result)
            {
                OnDebug(LogMode.Error,
                    string.Format("ReturnMessage\tFail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                return;
            }
            switch (retMessage.Command)
            {
                case (int)LoggingUpdateCommand.DownloadUMPData:
                    DealDownloadUMPData(retMessage);
                    break;
                case (int)LoggingUpdateCommand.UpdateProgress:
                    DealNotifyProgress(retMessage);
                    break;
            }
        }

        void mUpdateClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            OnDebug(LogMode.Info, "UpdateClient",
                string.Format("ConnectionEvent\tName:{0};\tCode:{1};\t{2}", e.Name, e.Code, e.Message));

            if (e.Code == Defines.EVT_NET_CONNECTED)
            {
                DownloadUMPData();

                CreateNotifyProgressThread();
            }
        }

        #endregion


        #region NotifyProgress Thread

        private void CreateNotifyProgressThread()
        {
            if (mThreadNotifyProgress != null)
            {
                try
                {
                    mThreadNotifyProgress.Abort();
                }
                catch { }
                mThreadNotifyProgress = null;
            }
            try
            {
                mThreadNotifyProgress = new Thread(NotifyProgressWorker);
                mThreadNotifyProgress.Start();
                OnDebug(LogMode.Info,
                    string.Format("Thread NotifyProgress created.\t{0}", mThreadNotifyProgress.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateNotifyProgressThread fail.\t{0}", ex.Message));
            }
        }

        private void StopNotifyProgressThread()
        {

            if (mThreadNotifyProgress != null)
            {
                try
                {
                    mThreadNotifyProgress.Abort();
                }
                catch { }
                mThreadNotifyProgress = null;
            }
        }

        private void NotifyProgressWorker()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mNotifyProgressInterval * 1000);

                    OnNotifyProgress();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("NotifyProgressWorker fail.\t{0}", ex.Message));
            }
        }

        private void OnNotifyProgress()
        {
            try
            {
                if (mUpdateClient == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mUpdateClient.SessionID;
                request.Command = (int)LoggingUpdateCommand.UpdateProgress;
                request.ListData.Add(mUpdateToken);
                request.ListData.Add(mUpdateProgress.ToString());
                request.ListData.Add(mUpdateFlag.ToString());
                mUpdateClient.SendMessage(request);
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("OnNotifyProgress fail.\t{0}", ex.Message));
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
            OnDebug(mode, "UpdateEngine", msg);
        }

        #endregion

    }
}
