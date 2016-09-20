//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3b16c11e-09e5-4554-a6e9-bf450f536e5a
//        CLR Version:              4.0.30319.18444
//        Name:                     MediaOperation
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                MediaOperation
//
//        created by Charley at 2015/3/28 17:08:34
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using VoiceCyber.WebSockets;

/*
 * ======================================================================
 * 
 * MediaOperation 工作逻辑
 * 
 * 说明：
 * MediaOperation 实际上仍然是MediaSession，在这里主要是处理各种消息
 * 1、请求消息
 * 2、通知消息
 * 
 * ***媒体下载***
 * 1、DownloadRecordFile
 * 2、DownloadRecordFileNas
 * 3、DownloadRecord
 * 
 * ***媒体文件加解密***
 * 1、DecryptRecordFile
 * 2、OriginalDecryptFile
 * 3、EncryptRecordFile
 * 
 * ***音频编码转化***
 * 1、ConvertWaveFormat
 * 
 * ***Isa播放控制***
 * 1、IsaStart
 * 2、IsaStop
 * 3、IsaBehavior
 * 
 * ======================================================================
 * 
 */

namespace UMPService03
{
    /// <summary>
    /// 相关操作
    /// </summary>
    public class MediaOperation : NetSession
    {

        #region Members

        public string RootDir { get; set; }
        public string MediaDataDir { get; set; }
        public List<ResourceConfigInfo> ListResourceInfos;

        private WebSocket mIsaWebSocket;
        private IsaServerInfo mCurrentIsaServer;

        #endregion


        public MediaOperation(TcpClient tcpClient)
            : base(tcpClient)
        {

        }


        #region Operations

        #region DownloadOperations

        private void DownloadRecordFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     Sftp 服务器地址
                //1     Sftp 端口
                //2     登录用户名
                //3     登录密码
                //4     RecordNo（C001）
                //5     RecordReference（C002)
                //6     PartitionTable（yyMM)
                //7     MediaType(1：Voice(.wav)；2：Screen(.vls)）,默认为1
                if (listParams == null || listParams.Count < 7)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strAddress = listParams[0];
                string strPort = listParams[1];
                string strUser = listParams[2];
                string strPassword = listParams[3];
                string recordNo = listParams[4];
                string recordRef = listParams[5];
                string partition = listParams[6];
                string strMediaType = "1";
                if (listParams.Count > 7)
                {
                    strMediaType = listParams[7];
                    if (strMediaType != "1" && strMediaType != "2")
                    {
                        SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                            string.Format("MediaType invalid"));
                        return;
                    }
                }
                string strLog =
                  string.Format(
                      "Args\tHost:{0};Port:{1};User:{2};Password:***;RecordNo:{3};RecordRef:{4};Partition:{5};MediaType:{6}",
                      strAddress,
                      strPort,
                      strUser,
                      recordNo,
                      recordRef,
                      partition,
                      strMediaType);
                LogInfo(command, strLog);
                int port;
                if (!int.TryParse(strPort, out port))
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                          string.Format("Param port invalid"));
                    return;
                }
                string fileName = string.Format("{0}_{1}", recordRef, SessionID);
                fileName = GetBase64String(fileName);
                fileName = string.Format("{0}.{1}", fileName, strMediaType == "2" ? "vls" : "wav");
                string path = Path.Combine(MediaDataDir, fileName);
                //文件已经存在，无需重复下载
                if (File.Exists(path))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = fileName;
                    SendMessage(retMessage);
                    LogInfo(command, fileName);
                    return;
                }
                SftpHelperConfig config = new SftpHelperConfig();
                config.ServerHost = strAddress;
                config.ServerPort = port;
                config.AuthType = 1;
                config.LoginUser = strUser;
                config.LoginPassword = strPassword;
                config.RemoteFile = string.Format("C001-{0}-{1}", recordNo, partition);
                config.LocalFile = path;
                OperationReturn optReturn = SftpHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = fileName;
                SendMessage(retMessage);
                LogInfo(command, fileName);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DownloadRecordFileNas(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                // 下载参数
                // 0：服务器地址
                // 1：端口
                // 2：登录名
                // 3：登录密码
                // 4：源文件路径
                // 5: Recordref C002
                // 6: MediaType(1：Voice(.wav)；2：Screen(.vls)）,默认为1
                if (listParams == null || listParams.Count < 6)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strAddress = listParams[0];
                string strPort = listParams[1];
                string strUser = listParams[2];
                string strPassword = listParams[3];
                string sourcePath = listParams[4];
                string recordRef = listParams[5];
                string strMediaType = "1";
                if (listParams.Count > 6)
                {
                    strMediaType = listParams[6];
                    if (strMediaType != "1" && strMediaType != "2")
                    {
                        SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                           string.Format("MediaType invalid"));
                        return;
                    }
                }
                string strLog =
                   string.Format(
                       "Args\tHost:{0};Port:{1};User:{2};Password:***;SourcePath:{3};RecordRef:{4};MediaType:{5}",
                       strAddress,
                       strPort,
                       strUser,
                       sourcePath,
                       recordRef,
                       strMediaType);
                LogInfo(command, strLog);
                int port;
                if (!int.TryParse(strPort, out port))
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                          string.Format("Param port invalid"));
                    return;
                }
                string fileName = string.Format("{0}_{1}", recordRef, SessionID);
                fileName = GetBase64String(fileName);
                fileName = string.Format("{0}.{1}", fileName, strMediaType == "2" ? "vls" : "wav");
                string path = Path.Combine(MediaDataDir, fileName);
                //文件已经存在，无需重复下载
                if (File.Exists(path))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = fileName;
                    SendMessage(retMessage);
                    LogInfo(command, fileName);
                    return;
                }
                string[] downloadParams = new string[7];
                downloadParams[0] = strAddress;
                downloadParams[1] = port.ToString();
                downloadParams[2] = strUser;
                downloadParams[3] = strPassword;
                downloadParams[4] = sourcePath;
                downloadParams[5] = path;
                downloadParams[6] = "1";
                OperationReturn optReturn = DownloadHelper.DownloadFileFTP(downloadParams);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = fileName;
                SendMessage(retMessage);
                LogInfo(command, fileName);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DownloadRecord(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     UserID
                //1     Password(M004)
                //2     RecordInfo(Json格式）
                //3     DownloadPreference（下载优先级）
                //          0：自动（通常优先NAS）
                //          1：优先NAS
                //          2：优先本地
                //          3：仅NAS
                //          4：仅本地
                if (listParams == null || listParams.Count < 4)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strUserID = listParams[0];
                string strPassword = listParams[1];
                string strRecordInfo = listParams[2];
                string strPrefer = listParams[3];
                string strLog = string.Format("Args\tUserID:{0};Password:***;RecordInfo:{1};Prefer:{2}",
                    strUserID,
                    strRecordInfo,
                    strPrefer);
                LogInfo(command, strLog);

                string strPartition = string.Empty;


                #region 检查参数有效性

                int intPrefer;
                if (!int.TryParse(strPrefer, out intPrefer))
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                        string.Format("Prefer param invalid.\t{0}", strPrefer));
                }
                try
                {
                    strPassword = DecryptString(strPassword);
                }
                catch (Exception ex)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                        string.Format("Password param invalid.\t{0}", ex.Message));
                }
                long rowid;
                string strSerialID;
                string strServerIP;
                UMPRecordInfo recordInfo;
                OperationReturn optReturn;
                try
                {
                    JsonObject jsonRecord = new JsonObject(strRecordInfo);
                    optReturn = GetRecordInfoFromJsonObject(jsonRecord);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(retMessage, optReturn);
                        return;
                    }
                    recordInfo = optReturn.Data as UMPRecordInfo;
                    if (recordInfo == null)
                    {
                        SendErrorMessage(retMessage, Defines.RET_OBJECT_NULL,
                            string.Format("RecordInfo is null"));
                        return;
                    }
                    rowid = recordInfo.RowID;
                    strSerialID = recordInfo.SerialID;
                    strServerIP = recordInfo.ServerIP;
                    if (string.IsNullOrEmpty(strServerIP))
                    {
                        SendErrorMessage(retMessage, Defines.RET_STRING_EMPTY,
                            string.Format("ServerIP is empty"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                        string.Format("RecordInfo param invalid.\t{0}", ex.Message));
                    return;
                }

                #endregion


                #region 获取所有可用下载服务器列表

                //获取所有下载服务器
                List<DownloadServerItem> listServers = new List<DownloadServerItem>();
                switch (intPrefer)
                {
                    case 0:
                    case 1:
                        if (ListResourceInfos != null)
                        {
                            var nass =
                                ListResourceInfos.Where(r => r.ObjType == DownloadParamInfo.RESOURCE_DOWNLOADPARAM)
                                    .OrderBy(r => r.ObjID)
                                    .ToList();
                            for (int i = 0; i < nass.Count; i++)
                            {
                                var nas = nass[i] as DownloadParamInfo;
                                if (nas == null) { continue; }
                                //过滤调不属于本服务器的下载参数
                                if (nas.ServerIP == strServerIP
                                    || nas.ServerIP == string.Empty)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 1;
                                    item.Info = nas;
                                    listServers.Add(item);
                                }
                            }
                            var sftps =
                                ListResourceInfos.Where(r => r.ObjType == SftpServerInfo.RESOURCE_SFTPSERVER)
                                    .OrderBy(r => r.ObjID)
                                    .ToList();
                            for (int i = 0; i < sftps.Count; i++)
                            {
                                var sftp = sftps[i] as SftpServerInfo;
                                if (sftp == null) { continue; }
                                if (sftp.HostAddress == strServerIP)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 0;
                                    item.Info = sftp;
                                    listServers.Add(item);
                                }
                            }
                        }
                        break;
                    case 2:
                        if (ListResourceInfos != null)
                        {
                            var sftps =
                              ListResourceInfos.Where(r => r.ObjType == SftpServerInfo.RESOURCE_SFTPSERVER)
                                  .OrderBy(r => r.ObjID)
                                  .ToList();
                            for (int i = 0; i < sftps.Count; i++)
                            {
                                var sftp = sftps[i] as SftpServerInfo;
                                if (sftp == null) { continue; }
                                if (sftp.HostAddress == strServerIP)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 0;
                                    item.Info = sftp;
                                    listServers.Add(item);
                                }
                            }
                            var nass =
                                ListResourceInfos.Where(r => r.ObjType == DownloadParamInfo.RESOURCE_DOWNLOADPARAM)
                                    .OrderBy(r => r.ObjID)
                                    .ToList();
                            for (int i = 0; i < nass.Count; i++)
                            {
                                var nas = nass[i] as DownloadParamInfo;
                                if (nas == null) { continue; }
                                if (nas.ServerIP == strServerIP)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 1;
                                    item.Info = nas;
                                    listServers.Add(item);
                                }
                            }
                        }
                        break;
                    case 3:
                        if (ListResourceInfos != null)
                        {
                            var nass =
                                ListResourceInfos.Where(r => r.ObjType == DownloadParamInfo.RESOURCE_DOWNLOADPARAM)
                                    .OrderBy(r => r.ObjID)
                                    .ToList();
                            for (int i = 0; i < nass.Count; i++)
                            {
                                var nas = nass[i] as DownloadParamInfo;
                                if (nas == null) { continue; }
                                if (nas.ServerIP == strServerIP)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 1;
                                    item.Info = nas;
                                    listServers.Add(item);
                                }
                            }
                        }
                        break;
                    case 4:
                        if (ListResourceInfos != null)
                        {
                            var sftps =
                              ListResourceInfos.Where(r => r.ObjType == SftpServerInfo.RESOURCE_SFTPSERVER)
                                  .OrderBy(r => r.ObjID)
                                  .ToList();
                            for (int i = 0; i < sftps.Count; i++)
                            {
                                var sftp = sftps[i] as SftpServerInfo;
                                if (sftp == null) { continue; }
                                if (sftp.HostAddress == strServerIP)
                                {
                                    var item = new DownloadServerItem();
                                    item.Type = 0;
                                    item.Info = sftp;
                                    listServers.Add(item);
                                }
                            }
                        }
                        break;
                    default:
                        SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                            string.Format("Prefer param invalid.\t{0}", intPrefer));
                        return;
                }

                if (listServers.Count <= 0)
                {
                    SendErrorMessage(retMessage, Defines.RET_NOT_EXIST,
                        string.Format("Download server not exist"));
                    return;
                }

                #endregion


                #region 遍历可用的下载服务器依次下载

                bool isDownloaded = false;
                //string fileName = string.Format("{0}_{1}.{2}",
                //            strSerialID,
                //            mSessionID,
                //            mediaType == 2 ? "vls" : "wav");
                string fileName = string.Format("{0}_{1}", strSerialID, SessionID);
                fileName = GetBase64String(fileName);
                fileName = string.Format("{0}.{1}", fileName, strSerialID == "2" ? "vls" : "wav");
                string savePath = Path.Combine(MediaDataDir, fileName);
                //文件已经存在，无需重复下载
                if (File.Exists(savePath))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = fileName;
                    SendMessage(retMessage);
                    LogInfo(command, fileName);
                    return;
                }
                for (int i = 0; i < listServers.Count; i++)
                {
                    var downloadServer = listServers[i];
                    if (downloadServer.Type == 0)
                    {
                        //Sftp方式
                        var sftp = downloadServer.Info as SftpServerInfo;
                        if (sftp == null)
                        {
                            OnDebug(LogMode.Error, string.Format("SftpServer is null"));
                            continue;
                        }
                        LogInfo(command,
                            string.Format("Downloading by Sftp.\t{0};{1};{2}", sftp.HostAddress, sftp.HostPort,
                                strSerialID));
                        SftpHelperConfig config = new SftpHelperConfig();
                        config.ServerHost = sftp.HostAddress;
                        config.ServerPort = sftp.HostPort;
                        config.AuthType = 1;
                        config.LoginUser = string.Format("{0}|{1}", strUserID, "00000");
                        config.LoginPassword = strPassword;
                        config.RemoteFile = string.Format("C001-{0}-{1}", rowid, strPartition);
                        config.LocalFile = savePath;
                        optReturn = SftpHelper.DownloadFile(config);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("Download by sftp fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        isDownloaded = true;
                    }
                    if (downloadServer.Type == 1)
                    {
                        //Nas方式
                        var nas = downloadServer.Info as DownloadParamInfo;
                        if (nas == null)
                        {
                            OnDebug(LogMode.Error, string.Format("DownloadParam is null"));
                            continue;
                        }
                        LogInfo(command,
                           string.Format("Downloading by Nas.\t{0};{1};{2}", nas.Address, nas.Port,
                               strSerialID));
                        optReturn = GetRecordPath(nas, recordInfo);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("GetRecordPath fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        string requestPath = optReturn.Data.ToString();
                        if (!string.IsNullOrEmpty(nas.RootDir))
                        {
                            requestPath = string.Format("\\{0}{1}", nas.RootDir, requestPath);
                        }
                        requestPath = Path.Combine(requestPath, string.Format("{0}.wav", recordInfo.RecordReference));
                        DownloadConfig config = new DownloadConfig();
                        config.Method = 11;
                        config.Host = nas.Address;
                        config.Port = nas.Port;
                        config.IsAnonymous = false;
                        config.LoginName = nas.AuthName;
                        config.Password = nas.AuthPassword;
                        config.RequestPath = requestPath;
                        config.SavePath = savePath;
                        config.IsReplace = true;
                        optReturn = DownloadHelper.DownloadFile(config);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error, string.Format("Download by nas fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        isDownloaded = true;
                    }
                    if (isDownloaded) { break; }
                }

                #endregion


                if (!isDownloaded)
                {
                    SendErrorMessage(retMessage, Defines.RET_FAIL, string.Format("Download fail"));
                    return;
                }
                retMessage.Data = fileName;
                SendMessage(retMessage);
                LogInfo(command, fileName);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #region EncryptDecryptOperations

        private void DecryptRecordFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                // 解密文件
                // 0：源文件名
                // 1：密码（M004加密）
                if (listParams == null || listParams.Count < 2)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strSourceFile = listParams[0];
                string strPassword = listParams[1];
                LogInfo(command,
                    string.Format("Source:{0}\tPass:***\t", strSourceFile));
                //LogInfo(Service03Command.DecryptRecordFile,
                //    string.Format("Source:{0}\tPass:{1}\t", strSourceFile, strPassword));
                strPassword = DecryptString(strPassword);
                string strExePath = Path.Combine(RootDir, string.Format("{0}\\FileConvert.exe", ConstValue.TEMP_DIR_MEDIAUTILS));
                if (!File.Exists(strExePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("FileConvert not exist.\t{0}", strExePath));
                    return;
                }
                string strSourcePath = Path.Combine(MediaDataDir, strSourceFile);
                if (!File.Exists(strSourcePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("Source file not exist.\t{0}", strSourcePath));
                    return;
                }
                string strTargetFile = string.Format("D_{0}", strSourceFile);
                string strTargetPath = Path.Combine(MediaDataDir, strTargetFile);

                ////2015/09/17 注释，考虑到有可能第二次输入错误的密码
                //if (File.Exists(strTargetPath))
                //{
                //    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                //    retMessage.Data = strTargetFile;
                //    SendMessage(retMessage.Command, retMessage);
                //    LogInfo(Service03Command.DecryptRecordFile, strTargetFile);
                //    return;
                //}

                string strArgs = string.Format("\"{0}\" \"{1}\" /r:all /d /p:\"{2}\"", strSourcePath, strTargetPath,
                    strPassword);
                OperationReturn optReturn = ExecuteExe(strExePath, strArgs);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = strTargetFile;
                SendMessage(retMessage);
                LogInfo(command, strTargetFile);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void OriginalDecryptFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                // 对文件原始解密
                // 0：源文件名
                if (listParams == null || listParams.Count < 1)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strSourceFile = listParams[0];
                LogInfo(command,
                    string.Format("Source:{0}\t", strSourceFile));
                string strExePath = Path.Combine(RootDir, string.Format("{0}\\FileConvert.exe", ConstValue.TEMP_DIR_MEDIAUTILS));
                if (!File.Exists(strExePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("FileConvert not exist.\t{0}", strExePath));
                    return;
                }
                string strSourcePath = Path.Combine(MediaDataDir, strSourceFile);
                if (!File.Exists(strSourcePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("Source file not exist.\t{0}", strSourcePath));
                    return;
                }
                string strTargetFile = string.Format("O_{0}", strSourceFile);
                string strTargetPath = Path.Combine(MediaDataDir, strTargetFile);
                if (File.Exists(strTargetPath))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = strTargetFile;
                    SendMessage(retMessage);
                    LogInfo(command, strTargetFile);
                    return;
                }
                string strArgs = string.Format("\"{0}\" \"{1}\" /r:all /d", strSourcePath, strTargetPath);
                OperationReturn optReturn = ExecuteExe(strExePath, strArgs);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = strTargetFile;
                SendMessage(retMessage);
                LogInfo(command, strTargetFile);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void EncryptRecordFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                // 对文件加密
                // 0：源文件名
                // 1: 加密密钥
                if (listParams == null || listParams.Count < 2)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strSourceFile = listParams[0];
                string strPassword = listParams[1];
                LogInfo(command,
                    string.Format("Source:{0}\t", strSourceFile));
                string strExePath = Path.Combine(RootDir, string.Format("{0}\\{1}", ConstValue.TEMP_DIR_MEDIAUTILS, ConstValue.TEMP_FILE_FILECONVERT));
                if (!File.Exists(strExePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("FileConvert not exist.\t{0}", strExePath));
                    return;
                }
                string strSourcePath = Path.Combine(MediaDataDir, strSourceFile);
                if (!File.Exists(strSourcePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("Source file not exist.\t{0}", strSourcePath));
                    return;
                }
                string strTargetFile = string.Format("E_{0}", strSourceFile);
                string strTargetPath = Path.Combine(MediaDataDir, strTargetFile);
                if (File.Exists(strTargetPath))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = strTargetFile;
                    SendMessage(retMessage);
                    LogInfo(command, strTargetFile);
                    return;
                }
                string strArgs = string.Format("\"{0}\" \"{1}\" /r:all /e /n:\"{2}\"", strSourcePath, strTargetPath,
                    strPassword);
                OperationReturn optReturn = ExecuteExe(strExePath, strArgs);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = strTargetFile;
                SendMessage(retMessage);
                LogInfo(command, strTargetFile);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #region WaveFormatOperations

        private void ConvertWaveFormat(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     原文件名称
                //1     要转换的格式代码（默认转换为PCM，即：1）
                //2     转换工具代码（默认使用FileConvert转换，即：1）
                //3     生成的文件的扩展名（wav,mp3等，默认：wav）
                //
                //注：1、格式代码，1：PCM；85：MP3
                //2、转换工具，1：FileConvert；2：Lame；3：ffmpeg
                if (listParams == null || listParams.Count < 1)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strSourceFile = listParams[0];
                LogInfo(command, string.Format("Source:{0}", strSourceFile));
                string strFormat = "1";
                string strUtil = "1";
                string strExt = string.Empty;
                if (listParams.Count > 1)
                {
                    strFormat = listParams[1];
                    LogInfo(command, string.Format("ConvertFormat:{0}", strFormat));
                }
                if (listParams.Count > 2)
                {
                    strUtil = listParams[2];
                    LogInfo(command, string.Format("ConvertUtil:{0}", strUtil));
                }
                if (listParams.Count > 3)
                {
                    strExt = listParams[3];
                    LogInfo(command, string.Format("Extentsion:{0}", strExt));
                }
                string strExePath;
                if (strUtil == "2")
                {
                    strExePath = Path.Combine(RootDir, string.Format("{0}\\lame.exe", ConstValue.TEMP_DIR_MEDIAUTILS));
                }
                else if (strUtil == "3")
                {
                    strExePath = Path.Combine(RootDir, string.Format("{0}\\ffmpeg.exe", ConstValue.TEMP_DIR_MEDIAUTILS));
                }
                else
                {
                    strExePath = Path.Combine(RootDir, string.Format("{0}\\FileConvert.exe", ConstValue.TEMP_DIR_MEDIAUTILS));
                }
                if (!File.Exists(strExePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("FileConvert not exist.\t{0}", strExePath));
                    return;
                }
                string strSourcePath = Path.Combine(MediaDataDir, strSourceFile);
                if (!File.Exists(strSourcePath))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("Source file not exist.\t{0}", strSourcePath));
                    return;
                }
                string strTargetFile = string.Format("F_{0}", strSourceFile);
                if (!string.IsNullOrEmpty(strExt))
                {
                    strTargetFile = string.Format("{0}.{1}", strTargetFile, strExt);
                }
                string strTargetPath = Path.Combine(MediaDataDir, strTargetFile);
                if (File.Exists(strTargetPath))
                {
                    retMessage.Code = Defines.RET_FILE_ALREADY_EXIST;
                    retMessage.Data = strTargetFile;
                    SendMessage(retMessage);
                    LogInfo(command, strTargetFile);
                    return;
                }
                string strArgs;
                if (strUtil == "2")
                {
                    strArgs = string.Format("-V 2 \"{0}\" \"{1}\"", strSourcePath, strTargetPath);
                }
                else if (strUtil == "3")
                {
                    strArgs = string.Format("-y -i \"{0}\" -ar 8000 -ab 12.2k -ac 1 \"{1}\"", strSourcePath, strTargetPath);
                }
                else
                {
                    strArgs = string.Format("\"{0}\" \"{1}\" /r:all /f:{2}", strSourcePath, strTargetPath, strFormat);
                }
                OperationReturn optReturn = ExecuteExe(strExePath, strArgs);
                if (!optReturn.Result)
                {
                    SendErrorMessage(retMessage, optReturn);
                    return;
                }
                retMessage.Data = strTargetFile;
                SendMessage(retMessage);
                LogInfo(command, strTargetFile);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #region ISA Control

        private void IsaStart(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     Address
                if (listParams == null || listParams.Count < 1)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                       string.Format("ListData is null or count invalid"));
                    return;
                }
                string strServerAddress = listParams[0];
                var isaServers = ListResourceInfos.Where(r => r.ObjType == IsaServerInfo.RESOURCE_ISASERVER).ToList();
                IsaServerInfo isaServer = null;
                for (int i = 0; i < isaServers.Count; i++)
                {
                    var temp = isaServers[i] as IsaServerInfo;
                    if (temp != null
                        && temp.HostAddress == strServerAddress)
                    {
                        isaServer = temp;
                    }
                }
                //判断ISA 服务器是否存在
                if (isaServer == null)
                {
                    SendErrorMessage(retMessage, Defines.RET_NOT_EXIST,
                        string.Format("IsaServer not exist.\t{0}", strServerAddress));
                    return;
                }
                mCurrentIsaServer = isaServer;
                if (mIsaWebSocket != null)
                {
                    try
                    {
                        mIsaWebSocket.Close();
                        mIsaWebSocket = null;
                    }
                    catch { }
                }
                mIsaWebSocket =
                    new WebSocket(string.Format("ws://{0}:{1}/cplay", isaServer.HostAddress, isaServer.HostPort));
                mIsaWebSocket.OnError +=
                    (s, ee) => LogError(command, string.Format("IsaWebSocketError\t{0}", ee.Message));
                mIsaWebSocket.OnOpen +=
                    (s, oe) => LogInfo(command, string.Format("IsaWebSocketOpen\tOpened"));
                mIsaWebSocket.OnClose +=
                    (s, ce) =>
                        LogInfo(command, string.Format("IsaWebSocketClose\tClosed:{0}", ce.Reason));
                mIsaWebSocket.OnMessage += IsaWebSocket_OnMessage;
                mIsaWebSocket.Connect();
                retMessage.Data = mCurrentIsaServer.LogInfo();
                SendMessage(retMessage);
                LogInfo(command, string.Format("IsaWebSocket connected.\t{0}", isaServer.HostAddress));
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void IsaStop(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                if (mIsaWebSocket != null)
                {
                    try
                    {
                        mIsaWebSocket.Close();
                        mIsaWebSocket = null;
                    }
                    catch { }
                }
                mCurrentIsaServer = null;
                SendMessage(retMessage);
                LogInfo(command, string.Format("IsaStop end."));
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void IsaBehavior(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     Action
                //1     RefID
                //2     Position
                //3     Speed
                if (listParams == null || listParams.Count < 4)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                      string.Format("ListData is null or count invalid"));
                    return;
                }
                string strAction = listParams[0];
                string strRefID = listParams[1];
                string strPosition = listParams[2];
                string strSpeed = listParams[3];
                if (mCurrentIsaServer == null)
                {
                    SendErrorMessage(retMessage, Defines.RET_OBJECT_NULL,
                        string.Format("IsaServer is null"));
                    return;
                }
                if (mIsaWebSocket == null)
                {
                    SendErrorMessage(retMessage, Defines.RET_OBJECT_NULL,
                        string.Format("IsaWebSocket is null"));
                    return;
                }
                string strAccessToken = mCurrentIsaServer.AccessToken;
                //string strAccessToken = "a5520c7335736ed958b0d6f18d55f2e0";
                string strRequestContent;
                JsonObject jsonRequest = new JsonObject();
                jsonRequest["access_token"] = new JsonProperty(string.Format("\"{0}\"", strAccessToken));
                jsonRequest["action"] = new JsonProperty(string.Format("\"{0}\"", strAction));
                jsonRequest["position"] = new JsonProperty(string.Format("{0}", strPosition));
                jsonRequest["sid"] = new JsonProperty(string.Format("\"{0}\"", strRefID));
                jsonRequest["speed"] = new JsonProperty(string.Format("{0}", strSpeed));
                strRequestContent = jsonRequest.ToString();
                LogDebug(command, string.Format("Send:{0}", strRequestContent));
                mIsaWebSocket.Send(strRequestContent);
                retMessage.Data = strRefID;
                SendMessage(retMessage);
                LogInfo(command,
                    string.Format("IsaBehavior sended.\t{0}\t{1}", strAction, strRefID));
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void IsaWebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            int command = (int)Service03Command.IsaNotifyPosition;
            try
            {
                OperationReturn optReturn;
                var msg = e.Data;
                JsonObject json = new JsonObject(msg);
                LogDebug(command, string.Format("Recv:{0}", json));
                if (json["action"] != null)
                {
                    string strAction = json["action"].Value;
                    if (strAction != "display"
                        && strAction != "mouse")
                    {
                        LogError(command, string.Format("ErrorMsg:{0}", strAction));
                        return;
                    }
                }
                if (json["frambuffer"] != null
                   && json["frambuffer"]["pixels"] != null
                   && json["frambuffer"]["position"] != null)
                {
                    int pos = (int)json["frambuffer"]["position"].Number;
                    int count = json["frambuffer"]["pixels"].Count;
                    string strImagFile = string.Empty;
                    if (count > 0)
                    {
                        //如果存在多个，取最后一张图片
                        if (json["frambuffer"]["pixels"][count - 1]["pixelImg"] != null)
                        {
                            string strImagPath = json["frambuffer"]["pixels"][count - 1]["pixelImg"].Value;
                            //DownloadImage(strPath);
                            optReturn = IsaDownloadImage(strImagPath);
                            if (!optReturn.Result)
                            {
                                LogError(command,
                                    string.Format("Download image fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            strImagFile = optReturn.Data.ToString();
                        }
                    }
                    if (pos == 0)
                    {
                        //Position为 0 表示播放结束了
                        LogInfo(command, string.Format("Playback stopped"));
                    }
                    NotifyMessage notMessage = new NotifyMessage();
                    notMessage.SessionID = SessionID;
                    notMessage.Command = (int)Service03Command.IsaNotifyPosition;
                    notMessage.ListData.Add(pos.ToString());
                    notMessage.ListData.Add(strImagFile);
                    SendMessage(notMessage);
                    LogDebug(command, string.Format("IsaNotifyPosition\t{0}", pos));
                }
            }
            catch (Exception ex)
            {
                LogError(command, string.Format("DealMessage fail.\t{0}", ex.Message));
            }
        }

        private OperationReturn IsaDownloadImage(string path)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("Image path empty");
                    return optReturn;
                }
                if (mCurrentIsaServer == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("IsaServer is null");
                    return optReturn;
                }
                string file = path.Substring(path.LastIndexOf("/") + 1);
                file = string.Format("{0}_{1}", SessionID, file);
                string savePath = MediaDataDir;
                if (!Directory.Exists(savePath))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("MediaDataDir not exist.\t{0}", savePath);
                    return optReturn;
                }
                savePath = Path.Combine(savePath, file);
                DownloadConfig config = new DownloadConfig();
                config.Method = 1;
                config.Host = mCurrentIsaServer.HostAddress;
                config.Port = 80;
                config.IsAnonymous = true;
                config.IsReplace = true;
                config.RequestPath = path;
                config.SavePath = savePath;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Data = file;
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

        #endregion


        #region 文件上传下载

        private void DownloadFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {

            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        private void UploadFile(RequestMessage request)
        {
            int command = request.Command;
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.Code = 0;
            retMessage.SessionID = SessionID;
            retMessage.Command = command;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     源文件名
                //1     设备ID
                //2     目标文件（相对根目录）
                if (listParams == null || listParams.Count < 3)
                {
                    SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                        string.Format("Request param is null or count invalid"));
                    return;
                }
                string strSourceFile = listParams[0];
                string strDeviceID = listParams[1];
                string strTargetFile = listParams[2];
                string strLog = string.Format("Args\tSourceFile:{0};DeviceID:{1};TargetFile:{2};",
                   strSourceFile,
                   strDeviceID,
                   strTargetFile);
                LogInfo(command, strLog);
                string path = MediaDataDir;
                path = Path.Combine(path, strSourceFile);
                if (!File.Exists(path))
                {
                    SendErrorMessage(retMessage, Defines.RET_FILE_NOT_EXIST,
                        string.Format("SourceFile not exist"));
                    return;
                }
                if (ListResourceInfos == null)
                {
                    SendErrorMessage(retMessage, Defines.RET_NOT_EXIST,
                        string.Format("ListResourceInfos not exist"));
                    return;
                }
                StorageDeviceInfo device =
                    ListResourceInfos.FirstOrDefault(d => d.ObjID.ToString() == strDeviceID) as StorageDeviceInfo;
                if (device == null)
                {
                    device = ListResourceInfos.FirstOrDefault(d => d.ID.ToString() == strDeviceID) as StorageDeviceInfo;
                    if (device == null)
                    {
                        SendErrorMessage(retMessage, Defines.RET_NOT_EXIST,
                            string.Format("StorageDevice not exist"));
                        return;
                    }
                }
                LogInfo(command, string.Format("DeviceInfo\t{0}", device.LogInfo()));
                OperationReturn optReturn;
                switch (device.DeviceType)
                {
                    //case 0:

                    //    break;
                    case 1:
                        string strCommand = string.Format("net use {0} /User:{1} {2} /PERSISTENT:YES", device.RootDir, device.AuthName, device.AuthPassword);
                        string strBat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempBat.bat");
                        if (File.Exists(strBat))
                        {
                            File.Delete(strBat);
                        }
                        string[] listCommands = new string[1];
                        listCommands[0] = strCommand;
                        File.WriteAllLines(strBat, listCommands);
                        optReturn = ExecuteBat(strBat);
                        if (!optReturn.Result)
                        {
                            SendErrorMessage(retMessage, optReturn);
                            return;
                        }
                        string target = string.Format("{0}\\{1}", device.RootDir, strTargetFile);
                        File.Copy(path, target, true);
                        retMessage.ListData.Add(device.LogInfo());
                        retMessage.ListData.Add(target);
                        break;
                    //case 2:

                    //    break;
                    default:
                        SendErrorMessage(retMessage, Defines.RET_PARAM_INVALID,
                            string.Format("DeviceType invalid.\t{0}", device.DeviceType));
                        return;
                }
                SendMessage(retMessage);
                LogInfo(command, strTargetFile);
            }
            catch (Exception ex)
            {
                SendErrorMessage(retMessage, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #endregion


        #region Client Commands

        protected override void DoClientCommand(RequestMessage request)
        {
            base.DoClientCommand(request);

            switch (request.Command)
            {
                case (int)Service03Command.DownloadRecordFile:
                    DownloadRecordFile(request);
                    break;
                case (int)Service03Command.DownloadRecordFileNas:
                    DownloadRecordFileNas(request);
                    break;
                case (int)Service03Command.DownloadRecord:
                    DownloadRecord(request);
                    break;
                case (int)Service03Command.ConvertWaveFormat:
                    ConvertWaveFormat(request);
                    break;
                case (int)Service03Command.DecryptRecordFile:
                    DecryptRecordFile(request);
                    break;
                case (int)Service03Command.OriginalDecryptFile:
                    OriginalDecryptFile(request);
                    break;
                case (int)Service03Command.EncryptRecordFile:
                    EncryptRecordFile(request);
                    break;
                case (int)Service03Command.IsaStart:
                    IsaStart(request);
                    break;
                case (int)Service03Command.IsaStop:
                    IsaStop(request);
                    break;
                case (int)Service03Command.IsaBehavior:
                    IsaBehavior(request);
                    break;
                case (int)Service03Command.UploadFile:
                    UploadFile(request);
                    break;
            }
        }

        #endregion


        #region Others

        protected override string ParseCommand(int command)
        {
            string str = base.ParseCommand(command);
            if (command >= 1000 && command < 10000)
            {
                str = ((Service03Command)command).ToString();
            }
            return str;
        }

        private void SendErrorMessage(ReturnMessage retMessage, int errCode, string msg)
        {
            retMessage.Result = false;
            retMessage.Code = errCode;
            retMessage.Message = msg;
            SendMessage(retMessage);

            OnDebug(LogMode.Error, string.Format("Command:{0}\t{1}\t{2}", ParseCommand(retMessage.Command), errCode, msg));
        }

        private void SendErrorMessage(ReturnMessage retMessage, OperationReturn optReturn)
        {
            SendErrorMessage(retMessage, optReturn.Code, optReturn.Message);
        }

        private void LogDebug(int command, string msg)
        {
            OnDebug(LogMode.Debug, string.Format("{0}\t{1}", command, msg));
        }

        private void LogInfo(int command, string msg)
        {
            OnDebug(LogMode.Info, string.Format("{0}\t{1}", command, msg));
        }

        private void LogError(int command, string msg)
        {
            OnDebug(LogMode.Error, string.Format("{0}\t{1}", command, msg));
        }

        private OperationReturn ExecuteExe(string exePath, string arguments)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!File.Exists(exePath))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Exe file not exist.\t{0}", exePath);
                    return optReturn;
                }
                Process processExe = new Process();
                processExe.StartInfo.FileName = exePath;
                processExe.StartInfo.Arguments = arguments;
                processExe.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processExe.Start();
                processExe.WaitForExit();
                if (processExe.HasExited == false)
                {
                    processExe.Kill();
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Exe exit with error");
                    return optReturn;
                }
                int intRet = processExe.ExitCode;
                if (intRet != 0)
                {
                    if (intRet == 86)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Service03Consts.DECRYPT_PASSWORD_ERROR;
                        optReturn.Message = string.Format("Decrypt password error");
                        return optReturn;
                    }
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Exe return error code:{0}", intRet);
                    return optReturn;
                }
                processExe.Dispose();
                optReturn.Data = intRet;
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

        private OperationReturn GetRecordPath(DownloadParamInfo downloadParam, UMPRecordInfo recordInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (recordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                string fmt = @"\*YYYY*MM\*DD\*VID\*CID\";
                if (recordInfo.MediaType == 1 || recordInfo.MediaType == 0)
                {
                    fmt = @"\vox\*YYYY*MM\*DD\*VID\*CID\";
                }
                if (recordInfo.MediaType == 2)
                {
                    fmt = @"\scr\*YYYY*MM\*DD\*VID\*CID\";
                }
                if (downloadParam != null)
                {
                    if ((recordInfo.MediaType == 1
                        || recordInfo.MediaType == 0)
                        && !string.IsNullOrEmpty(downloadParam.VocPathFormat))
                    {
                        fmt = downloadParam.VocPathFormat;
                    }
                    if (recordInfo.MediaType == 2
                        && !string.IsNullOrEmpty(downloadParam.ScrPathFormat))
                    {
                        fmt = downloadParam.ScrPathFormat;
                    }
                }
                optReturn.Message = fmt;
                int iYear, iMonth, iDay, iVoiceID, iChannelID;
                DateTime dtStartRecordTime;

                dtStartRecordTime = recordInfo.StartRecordTime;
                iYear = dtStartRecordTime.Year;
                iMonth = dtStartRecordTime.Month;
                iDay = dtStartRecordTime.Day;
                iVoiceID = recordInfo.ServerID;
                iChannelID = recordInfo.ChannelID;

                Match reVID = Regex.Match(fmt, @"(\*)(\d*)(VID)");
                Match reCID = Regex.Match(fmt, @"(\*)(\d*)(CID)");
                if (reVID.Success)
                {
                    if (reVID.Groups[2].Value == string.Empty)
                    {
                        fmt = Regex.Replace(fmt, @"(\*)(\d*)(VID)", iVoiceID.ToString("D04"));
                    }
                    else
                    {
                        fmt = Regex.Replace(fmt, @"(\*)(\d*)(VID)", iVoiceID.ToString("D" + reVID.Groups[2]));
                    }
                }
                if (reCID.Success)
                {
                    if (reCID.Groups[2].Value == string.Empty)
                    {
                        fmt = Regex.Replace(fmt, @"(\*)(\d*)(CID)", iChannelID.ToString("D05"));
                    }
                    else
                    {
                        fmt = Regex.Replace(fmt, @"(\*)(\d*)(CID)", iChannelID.ToString("D" + reCID.Groups[2]));
                    }
                }

                fmt = Regex.Replace(fmt, @"(\*YYYY)", iYear.ToString("D04"));
                fmt = Regex.Replace(fmt, @"(\*YY)", (iYear % 100).ToString("D02"));
                fmt = Regex.Replace(fmt, @"(\*MM)", iMonth.ToString("D02"));
                fmt = Regex.Replace(fmt, @"(\*DD)", iDay.ToString("D02"));

                optReturn.Data = fmt;
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

        private OperationReturn GetRecordInfoFromJsonObject(JsonObject json)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                UMPRecordInfo recordInfo = new UMPRecordInfo();
                recordInfo.RowID = Convert.ToInt64(json[UMPRecordInfo.PRO_ROWID].Number);
                recordInfo.SerialID = json[UMPRecordInfo.PRO_SERIALID].Value;
                recordInfo.RecordReference = json[UMPRecordInfo.PRO_RECORDREFERENCE].Value;
                recordInfo.StartRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STARTRECORDTIME].Value);
                recordInfo.StopRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STOPRECORDTIME].Value);
                recordInfo.Extension = json[UMPRecordInfo.PRO_EXTENSION].Value;
                recordInfo.Agent = json[UMPRecordInfo.PRO_AGENT].Value;
                recordInfo.MediaType = Convert.ToInt32(json[UMPRecordInfo.PRO_MEDIATYPE].Number);
                recordInfo.EncryptFlag = json[UMPRecordInfo.PRO_ENCRYPTFLAG].Value;
                recordInfo.ServerID = Convert.ToInt32(json[UMPRecordInfo.PRO_SERVERID].Number);
                recordInfo.ServerIP = json[UMPRecordInfo.PRO_SERVERIP].Value;
                recordInfo.ChannelID = Convert.ToInt32(json[UMPRecordInfo.PRO_CHANNELID].Number);
                recordInfo.StringInfo = json.ToString();

                optReturn.Data = recordInfo;
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

        private OperationReturn GetJsonObjectFromRecordInfo(UMPRecordInfo recordInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                JsonObject json = new JsonObject();
                json[UMPRecordInfo.PRO_ROWID] = new JsonProperty(string.Format("{0}", recordInfo.RowID));
                json[UMPRecordInfo.PRO_SERIALID] = new JsonProperty(string.Format("\"{0}\"", recordInfo.SerialID));
                json[UMPRecordInfo.PRO_RECORDREFERENCE] = new JsonProperty(string.Format("\"{0}\"", recordInfo.RecordReference));
                json[UMPRecordInfo.PRO_STARTRECORDTIME] = new JsonProperty(string.Format("\"{0}\"", recordInfo.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));
                json[UMPRecordInfo.PRO_STOPRECORDTIME] = new JsonProperty(string.Format("\"{0}\"", recordInfo.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));
                json[UMPRecordInfo.PRO_EXTENSION] = new JsonProperty(string.Format("\"{0}\"", recordInfo.Extension));
                json[UMPRecordInfo.PRO_AGENT] = new JsonProperty(string.Format("\"{0}\"", recordInfo.Agent));
                json[UMPRecordInfo.PRO_SERVERID] = new JsonProperty(string.Format("{0}", recordInfo.ServerID));
                json[UMPRecordInfo.PRO_SERVERIP] = new JsonProperty(string.Format("\"{0}\"", recordInfo.ServerIP));
                json[UMPRecordInfo.PRO_CHANNELID] = new JsonProperty(string.Format("{0}", recordInfo.ChannelID));
                json[UMPRecordInfo.PRO_MEDIATYPE] = new JsonProperty(string.Format("{0}", recordInfo.MediaType));
                json[UMPRecordInfo.PRO_ENCRYPTFLAG] = new JsonProperty(string.Format("\"{0}\"", recordInfo.EncryptFlag));

                optReturn.Data = json;
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

        private OperationReturn ConnectFileShareState(string path, string userName, string password)
        {
            //设置共享目录访问权限，（模拟登录，应该是）
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string strCommand = string.Format("net use {0} /User:{1} {2} /PERSISTENT:YES", path, userName, password);
                process.StandardInput.WriteLine(strCommand);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                if (process.HasExited == false)
                {
                    process.Kill();
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Cmd exit with error");
                    return optReturn;
                }
                int intRet = process.ExitCode;
                //if (intRet != 0)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_FAIL;
                //    optReturn.Message = string.Format("Cmd return error code:{0}", intRet);
                //    return optReturn;
                //}
                process.Dispose();
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

        private OperationReturn ExecuteBat(string path)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Bat file not exist.\t{0}", path);
                    return optReturn;
                }
                Process processExe = new Process();
                processExe.StartInfo.FileName = path;
                processExe.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processExe.Start();
                processExe.WaitForExit();
                if (processExe.HasExited == false)
                {
                    processExe.Kill();
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Bat exit with error");
                    return optReturn;
                }
                int intRet = processExe.ExitCode;
                if (intRet != 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Bat return error code:{0}", intRet);
                    return optReturn;
                }
                processExe.Dispose();
                optReturn.Data = intRet;
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

        private string GetBase64String(string strSource)
        {
            byte[] byteSource = Encoding.ASCII.GetBytes(strSource);
            return Convert.ToBase64String(byteSource);
        }

        #endregion


        #region Encryption and Decryption

        public string EncryptString(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string DecryptString(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string DecryptFromDB(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        #endregion

    }
}
