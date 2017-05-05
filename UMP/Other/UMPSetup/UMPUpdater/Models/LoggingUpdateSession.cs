//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    b52b6c25-550c-440d-8c9d-a09880eab11d
//        CLR Version:              4.0.30319.42000
//        Name:                     LoggingUpdateSession
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                LoggingUpdateSession
//
//        Created by Charley at 2016/9/8 10:38:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Updates;


namespace UMPUpdater.Models
{
    public class LoggingUpdateSession : NetSession
    {

        #region Members

        public List<LoggingServerInfo> ListLoggingServers;
        public InstallInfo InstallInfo;

        private string mToken;
        private LoggingServerInfo mLoggingServer;

        #endregion


        public LoggingUpdateSession(TcpClient tcpClient)
            : base(tcpClient)
        {

        }


        #region 处理请求消息

        private void DoDownloadUMPData(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.SessionID = SessionID;
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.Command = request.Command;
            try
            {
                var listArgs = request.ListData;
                if (listArgs == null
                    || listArgs.Count < 1)
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_PARAM_INVALID;
                    retMessage.Message = string.Format("Param count invalid");
                    SendMessage(retMessage);
                    LogError(retMessage);
                    return;
                }
                string strToken = listArgs[0];
                if (ListLoggingServers == null
                    || InstallInfo == null) { return; }
                var loggingServer = ListLoggingServers.FirstOrDefault(l => l.Token == strToken);
                if (loggingServer == null)
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_PARAM_INVALID;
                    retMessage.Message = string.Format("Token invalid.\t{0}", strToken);
                    SendMessage(retMessage);
                    LogError(retMessage);
                    return;
                }
                mToken = strToken;
                mLoggingServer = loggingServer;
                string strInstallTime = DateTime.Parse(InstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                string strFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\UMPUpdater\\Temp\\UMPData_{0}.zip", strInstallTime));
                if (!File.Exists(strFile))
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_FILE_NOT_EXIST;
                    retMessage.Message = string.Format("UMPData.zip not exist.\t{0}", strFile);
                    SendMessage(retMessage);
                    LogError(retMessage);
                    return;
                }
                FileInfo fileInfo = new FileInfo(strFile);
                long totalsize = fileInfo.Length;
                string fileName = fileInfo.Name;
                retMessage.ListData.Add(totalsize.ToString());
                retMessage.ListData.Add(fileName);
                SendMessage(retMessage);
                LogInfo(retMessage.Command, string.Format("Get UMPData.zip info end.\t{0}\t{1}", totalsize, fileName));

                ThreadPool.QueueUserWorkItem(a => DoTransferUMPData());
            }
            catch (Exception ex)
            {
                retMessage.Result = false;
                retMessage.Code = Defines.RET_FAIL;
                retMessage.Message = ex.Message;
                SendMessage(retMessage);
                LogError(retMessage);
            }
        }

        private void DoUpdateProgress(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.SessionID = SessionID;
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.Command = request.Command;
            try
            {
                var listArgs = request.ListData;
                if (listArgs == null
                    || listArgs.Count < 3)
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_PARAM_INVALID;
                    retMessage.Message = string.Format("Param count invalid");
                    SendMessage(retMessage);
                    LogError(retMessage);
                    return;
                }
                if (mLoggingServer == null) { return; }
                string strToken = listArgs[0];
                string strProgress = listArgs[1];
                string strFlag = listArgs[2];
                double progress;
                int intFlag;
                if (!double.TryParse(strProgress, out progress)
                    || !int.TryParse(strFlag, out intFlag))
                {
                    retMessage.Result = false;
                    retMessage.Code = Defines.RET_PARAM_INVALID;
                    retMessage.Message = string.Format("Param invalid");
                    SendMessage(retMessage);
                    LogError(retMessage);
                    return;
                }
                SendMessage(retMessage);

                mLoggingServer.Progress = progress;
                mLoggingServer.UpdateFlag = intFlag;
                OnProgress(progress);

                string str = string.Format("{0:0.00} %", progress);
                LogInfo(retMessage.Command, string.Format("LoggingServer {0} Progress:{1};UpdateFlag:{2}", mLoggingServer.HostAddress, str, intFlag));
            }
            catch (Exception ex)
            {
                retMessage.Result = false;
                retMessage.Code = Defines.RET_FAIL;
                retMessage.Message = ex.Message;
                SendMessage(retMessage);
                LogError(retMessage);
            }
        }

        #endregion


        #region 处理通知消息

        private void DoTransferUMPData()
        {
            try
            {
                if (InstallInfo == null) { return; }
                string strInstallTime = DateTime.Parse(InstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                string strFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\UMPUpdater\\Temp\\UMPData_{0}.zip", strInstallTime));
                if (!File.Exists(strFile))
                {
                    LogError((int)LoggingUpdateCommand.TransferUMPData, string.Format("UMPData.zip not exist.\t{0}", strFile));
                    return;
                }
                FileInfo fileInfo = new FileInfo(strFile);
                long totalSize = fileInfo.Length;
                LogInfo((int)LoggingUpdateCommand.TransferUMPData,
                  string.Format("Begin transfer UMPData.zip...\t{0}\t{1}", totalSize, strFile));
                FileStream fs = File.OpenRead(strFile);
                int bufferSize = 102400;
                byte[] buffer = new byte[bufferSize];
                int intSize = 0;
                long transferIndex = 0;
                long transferedSize = 0;
                do
                {
                    if (IsConnected)
                    {
                        intSize = fs.Read(buffer, 0, bufferSize);
                        if (intSize > 0)
                        {
                            //transferIndex++;
                            //transferedSize += intSize;
                            //notMessage = new NotifyMessage();
                            //notMessage.SessionID = SessionID;
                            //notMessage.Command = (int)LoggingUpdateCommand.TransferUMPData;
                            //notMessage.ListData.Add(mToken);
                            //notMessage.ListData.Add(transferIndex.ToString());
                            //notMessage.ListData.Add(intSize < bufferSize ? "1" : "0");
                            //notMessage.ListData.Add(intSize.ToString());
                            //byte[] data = new byte[intSize];
                            //Array.Copy(buffer, 0, data, 0, intSize);

                            ////string strData = Converter.Byte2Hex(data);

                            //StringBuilder sb = new StringBuilder();     //使用stringbuilder可以提高效率
                            //for (int i = 0; i < data.Length; i++)
                            //{
                            //    sb.Append(data[i].ToString("X2"));
                            //}
                            //string strData = sb.ToString();

                            //notMessage.ListData.Add(strData);
                            //SendMessage(notMessage);


                            //不使用NotifyMessage包装类，因为要传输的数据是二进制的字节流数据
                            transferIndex++;
                            transferedSize += intSize;
                            var msgHead = GetMessageHead();
                            msgHead.Type = (int)MessageType.RawData;
                            msgHead.Command = (int)LoggingUpdateCommand.TransferUMPData;
                            msgHead.State = intSize < bufferSize ? 3 : 0;     //State bit1:是否最后一个数据包；bit2：是否数据结束（自定义）
                            byte[] data = new byte[intSize];
                            Array.Copy(buffer, 0, data, 0, intSize);
                            SendMessage(msgHead, data);

                            //var p = transferedSize / (totalSize * 1.0);
                            //OnDebug(LogMode.Info,
                            //    string.Format(
                            //        "Transfer UMPData.zip.\tTransferIndex:{0}\t;Size:{1}\tTransferedSize:{2}\tPecentage:{3:0.00}%",
                            //        transferIndex, intSize, transferedSize, p * 100));
                        }
                    }
                    Thread.Sleep(1);
                } while (intSize > 0);
                fs.Close();
                fs.Dispose();
                LogInfo((int)LoggingUpdateCommand.TransferUMPData,
                    string.Format("Transfer UMPData.zip end.\t{0}\t{1}", totalSize, strFile));
            }
            catch (Exception ex)
            {
                LogError((int)LoggingUpdateCommand.TransferUMPData, string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Client Commands

        protected override void DoClientCommand(RequestMessage request)
        {
            base.DoClientCommand(request);

            OnDebug(LogMode.Info, string.Format("Request message.\t{0}", request.Command));

            switch (request.Command)
            {
                case (int)LoggingUpdateCommand.DownloadUMPData:
                    DoDownloadUMPData(request);
                    break;
                case (int)LoggingUpdateCommand.UpdateProgress:
                    DoUpdateProgress(request);
                    break;
            }
        }

        #endregion


        #region Others

        private void LogError(ReturnMessage retMessage)
        {
            OnDebug(LogMode.Error, string.Format("{0}\t{1}\t{2}", ParseCommand(retMessage.Command), retMessage.Code, retMessage.Message));
        }

        private void LogError(int command, string msg)
        {
            OnDebug(LogMode.Error, string.Format("{0}\t{1}", ParseCommand(command), msg));
        }

        private void LogInfo(int command, string msg)
        {
            OnDebug(LogMode.Info, string.Format("{0}\t{1}", ParseCommand(command), msg));
        }

        protected override string ParseCommand(int command)
        {
            if (command > 1000)
            {
                return ((LoggingUpdateCommand)command).ToString();
            }
            return base.ParseCommand(command);
        }

        #endregion


        #region Progress

        public event Action<LoggingServerInfo, double> ProgressEvent;

        private void OnProgress(double progress)
        {
            if (mLoggingServer == null) { return; }
            if (ProgressEvent != null)
            {
                ProgressEvent(mLoggingServer, progress);
            }
        }

        #endregion

    }

    /// <summary>
    /// 通讯指令
    /// </summary>
    public enum LoggingUpdateCommand
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 
        /// LoggingUpdater请求下载UMPData.zip
        /// 
        /// 参数：
        /// 0：  Token（验证码）
        /// 
        /// 响应：
        /// 0：  TotalSize
        /// 1：  FileName
        /// </summary>
        DownloadUMPData = 1001,
        /// <summary>
        /// 
        /// LoggingUpdater向UMPUpdater报告更新进度
        /// 
        /// 参数：
        /// 0：  Token（验证码）
        /// 1：  Progress（当前进度值，百分比）
        /// 2：  Flag（1表示完成，2表示出错）
        /// 
        /// 响应：
        /// 无
        /// </summary>
        UpdateProgress = 1002,

        /// <summary>
        /// 
        /// UMPUpdater向LoggingUpdater传送UMPData.zip
        /// 
        /// 注意，这里传输的数据是二进制的字节流数据
        /// MessageHead中指定了数据大小，Command 及 State
        /// 
        /// </summary>
        TransferUMPData = 2001,
    }
}
