using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3106.Models
{
    /// <summary>
    /// 录音记录操作类
    /// 下载、解密、转换、获取密钥等一些列操作
    /// </summary>
    public class RecordOperator
    {

        #region Members

        private const string PATH_FORMAT = @"\vox\*YYYY*MM\*DD\*VID\*CID\";

        public SessionInfo Session;
        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;

        private RecordInfo mRecordInfo;

        #endregion


        #region RecordInfo

        public RecordInfo RecordInfo
        {
            get { return mRecordInfo; }
            set { mRecordInfo = value; }
        }

        #endregion


        public UMPApp CurrentApp;


        public RecordOperator()
        {

        }

        public RecordOperator(RecordInfo recordInfo)
            : this()
        {
            mRecordInfo = recordInfo;
        }


        #region Operations

        public OperationReturn DownloadFileToAppServer()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                string strServerAddress = mRecordInfo.VoiceIP;


                #region 获取下载服务器列表

                List<DownloadServerItem> listServerItems = new List<DownloadServerItem>();
                if (ListDownloadParams != null)
                {
                    var downloadParams = ListDownloadParams.Where(d => d.VoiceAddress == strServerAddress || d.VoiceAddress == string.Empty).OrderBy(d => d.ID).ToList();
                    for (int i = 0; i < downloadParams.Count; i++)
                    {
                        var downloadParam = downloadParams[i];
                        if (!downloadParam.IsEnabled) { continue; }
                        var item = new DownloadServerItem();
                        item.Type = 1;
                        item.Info = downloadParam;
                        listServerItems.Add(item);
                    }
                }
                if (ListSftpServers != null)
                {
                    var sftps =
                        ListSftpServers.Where(s => s.HostAddress == strServerAddress).OrderBy(s => s.ObjID).ToList();
                    for (int i = 0; i < sftps.Count; i++)
                    {
                        var sftp = sftps[i];
                        var item = new DownloadServerItem();
                        item.Type = 0;
                        item.Info = sftp;
                        listServerItems.Add(item);
                    }
                }
                if (listServerItems.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_DOWNLOADSERVER_NOT_EXIST;
                    optReturn.Message = string.Format("Download server not exist.\t{0}", strServerAddress);
                    return optReturn;
                }
                if (Service03Helper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Service03Helper is null");
                    return optReturn;
                }

                #endregion


                #region 下载文件

                RequestMessage request;
                ReturnMessage retMessage;
                bool isDownloaded = false;

                //遍历可用的下载服务器尝试下载文件
                for (int i = 0; i < listServerItems.Count; i++)
                {
                    var item = listServerItems[i];
                    var type = item.Type;
                    request = new RequestMessage();
                    if (type == 0)
                    {
                        //Sftp方式
                        var sftpServer = item.Info as SftpServerInfo;
                        if (sftpServer == null) { continue; }
                        string strPartInfo = string.Empty;
                        PartitionTableInfo partInfo =
                            Session.ListPartitionTables.FirstOrDefault(
                                p =>
                                    p.TableName == ConstValue.TABLE_NAME_RECORD && p.PartType == TablePartType.DatetimeRange);
                        if (partInfo != null)
                        {
                            DateTime startTime = mRecordInfo.StartRecordTime;
                            strPartInfo = string.Format("{0}{1}", startTime.ToString("yy"), startTime.ToString("MM"));
                        }
                        OnDebug("DownloadAppServer",
                            string.Format("Downloading by sftp.\t{0}:{1}\t{2}", sftpServer.HostAddress,
                                sftpServer.HostPort, mRecordInfo.SerialID));
                        request.Command = (int)Service03Command.DownloadRecordFile;
                        request.ListData.Add(sftpServer.HostAddress);
                        request.ListData.Add(sftpServer.HostPort.ToString());
                        request.ListData.Add(string.Format("{0}|{1}", Session.UserID, Session.RentInfo.Token));
                        request.ListData.Add(Session.UserInfo.Password);
                        request.ListData.Add(mRecordInfo.RowID.ToString());
                        request.ListData.Add(mRecordInfo.SerialID.ToString());
                        request.ListData.Add(strPartInfo);
                        //MediaType为0，认为是.wav文件
                        int mediaType = mRecordInfo.MediaType;
                        if (mediaType == 0)
                        {
                            mediaType = 1;
                        }
                        request.ListData.Add(mediaType.ToString());
                    }
                    else if (type == 1)
                    {
                        //NAS方式
                        var downloadParam = item.Info as DownloadParamInfo;
                        if (downloadParam == null) { continue; }
                        optReturn = GetRecordPath(downloadParam);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        string path = optReturn.Data.ToString();
                        if (string.IsNullOrEmpty(path))
                        {
                            OnDebug("DownloadAppServer", string.Format("Record path is empty"));
                            continue;
                        }
                        if (!string.IsNullOrEmpty(downloadParam.RootDir))
                        {
                            path = string.Format("/{0}{1}", downloadParam.RootDir, path);
                        }
                        path = string.Format("{0}{1}.{2}", path, mRecordInfo.RecordReference,
                            mRecordInfo.MediaType == 2 ? "vls" : "wav");
                        OnDebug("DownloadAppServer",
                           string.Format("Downloading by nas.\t{0}:{1}\t{2}\t{3}", downloadParam.Address,
                               downloadParam.Port, path, mRecordInfo.SerialID));
                        string serialID = mRecordInfo.SerialID.ToString();
                        request.Command = (int)Service03Command.DownloadRecordFileNas;
                        request.ListData.Add(downloadParam.Address);
                        request.ListData.Add(downloadParam.Port.ToString());
                        request.ListData.Add(downloadParam.UserName);
                        request.ListData.Add(downloadParam.Password);
                        request.ListData.Add(path);
                        request.ListData.Add(serialID);
                        int mediaType = mRecordInfo.MediaType;
                        if (mediaType == 2)
                        {
                            mediaType = 2;
                        }
                        else { mediaType = 1; }
                        request.ListData.Add(mediaType.ToString());
                    }
                    else
                    {
                        OnDebug("DownloadAppServer",
                            string.Format("Method invalid.\t{0}", type));
                        continue;
                    }
                    optReturn = Service03Helper.DoRequest(request);
                    if (!optReturn.Result)
                    {
                        OnDebug("DownloadAppServer",
                            string.Format("Download by {0} fail.\t{1}\t{2}", type, optReturn.Code,
                                optReturn.Message));
                        continue;
                    }
                    retMessage = optReturn.Data as ReturnMessage;
                    if (retMessage == null)
                    {
                        OnDebug("DownloadAppServer", string.Format("RetMessage is null"));
                        continue;
                    }
                    if (!retMessage.Result)
                    {
                        OnDebug("DownloadAppServer",
                            string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                        continue;
                    }
                    OnDebug("DownloadAppServer", string.Format("Download by {0} end.\t{1}", type, mRecordInfo.SerialID));
                    isDownloaded = true;
                    optReturn.Data = retMessage.Data;
                    break;
                }
                if (!isDownloaded)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_DOWNLOAD_APPSERVER_FAIL;
                    optReturn.Message =
                        string.Format("Download file to AppServer fail. Reference log for detail information");
                    return optReturn;
                }

                #endregion

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

        public OperationReturn OriginalDecryptRecord(string fileName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn.Data = fileName;
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                var encryptFlag = mRecordInfo.EncryptFlag;
                if (encryptFlag != "0")
                {
                    //加密标识不为“0” 的无需做原始解密
                    return optReturn;
                }
                var mediaType = mRecordInfo.MediaType;
                if (mediaType == 2)
                {
                    //录屏文件无需做原始解密
                    return optReturn;
                }
                if (Service03Helper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Service03Helper is null");
                    return optReturn;
                }
                RequestMessage request;
                ReturnMessage retMessage;
                request = new RequestMessage();
                request.Command = (int)Service03Command.OriginalDecryptFile;
                request.ListData.Add(fileName);
                optReturn = Service03Helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RetMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                fileName = retMessage.Data;
                optReturn.Data = fileName;
                OnDebug("OrignalDecrypt", string.Format("OrignalDecrypt end.\t{0}", fileName));
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

        public OperationReturn DecryptRecord(string fileName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn.Data = fileName;
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                var encryptFlag = mRecordInfo.EncryptFlag;
                if (encryptFlag == "0") { return optReturn; }     //未加密的无需解密操作
                if (encryptFlag != "2")
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("EncryptFlag invalid.\t{0}", encryptFlag);
                    return optReturn;
                }
                if (ListEncryptInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListEncryptInfo is null");
                    return optReturn;
                }
                var encryptInfo = ListEncryptInfo.FirstOrDefault(s => s.ServerAddress == mRecordInfo.VoiceIP);
                if (encryptInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("EncryptInfo is null");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(encryptInfo.RealPassword))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("RealPassword is empty");
                    return optReturn;
                }
                return DecryptRecord(fileName, encryptInfo.RealPassword);
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

        public OperationReturn DecryptRecord(string fileName, string password)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn.Data = fileName;
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                var encryptFlag = mRecordInfo.EncryptFlag;
                if (encryptFlag == "0") { return optReturn; }     //未加密的无需解密操作
                if (encryptFlag != "2")
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("EncryptFlag invalid.\t{0}", encryptFlag);
                    return optReturn;
                }
                if (Service03Helper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Service03Helper is null");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(password))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("RealPassword is empty");
                    return optReturn;
                }
                RequestMessage request;
                ReturnMessage retMessage;
                request = new RequestMessage();
                request.Command = (int)Service03Command.DecryptRecordFile;
                request.ListData.Add(fileName);
                request.ListData.Add(password);
                optReturn = Service03Helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RetMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                fileName = retMessage.Data;
                optReturn.Data = fileName;
                OnDebug("DecryptRecord", string.Format("DecryptRecord end.\t{0}", fileName));
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

        public OperationReturn EncryptRecord(string fileName, string strPassword)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn.Data = fileName;
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                if (mRecordInfo.MediaType != 1)
                {
                    //只能对录音文件进行加密
                    return optReturn;
                }
                if (Service03Helper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Service03Helper is null");
                    return optReturn;
                }
                RequestMessage request;
                ReturnMessage retMessage;
                request = new RequestMessage();
                request.Command = (int)Service03Command.EncryptRecordFile;
                request.ListData.Add(fileName);
                request.ListData.Add(strPassword);
                optReturn = Service03Helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RetMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                fileName = retMessage.Data;
                optReturn.Data = fileName;
                OnDebug("EncryptRecord", string.Format("EncryptRecord end.\t{0}", fileName));
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

        public OperationReturn ConvertWaveFormat(string fileName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn.Data = fileName;
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                var format = mRecordInfo.WaveFormat;
                RequestMessage request;
                ReturnMessage retMessage;
                switch (format)
                {
                    case VOICEFORMAT_G729_A:
                        request = new RequestMessage();
                        request.Command = (int)Service03Command.ConvertWaveFormat;
                        request.ListData.Add(fileName);
                        request.ListData.Add("1");
                        optReturn = Service03Helper.DoRequest(request);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        retMessage = optReturn.Data as ReturnMessage;
                        if (retMessage == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("RetMessage is null");
                            return optReturn;
                        }
                        if (!retMessage.Result)
                        {
                            optReturn.Result = false;
                            optReturn.Code = retMessage.Code;
                            optReturn.Message = retMessage.Message;
                            return optReturn;
                        }
                        fileName = retMessage.Data;
                        optReturn.Data = fileName;
                        OnDebug("ConvertFormat", string.Format("ConvertWaveFormat end.\t{0}", fileName));
                        break;
                }
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

        public OperationReturn GetRealPassword(RecordEncryptInfo encryptInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                if (encryptInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("EncryptInfo is null");
                    return optReturn;
                }
                //新密钥解老密钥
                List<string> listArgs = new List<string>();
                listArgs.Add("1");                                                                          //对象类型：1
                listArgs.Add(encryptInfo.ServerAddress);                                                    //加密对象：录音服务器的IP地址或机器名
                listArgs.Add(mRecordInfo.SerialID.ToString());                                              //录音流水号：C002
                listArgs.Add(mRecordInfo.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss"));                  //录音开始时间
                listArgs.Add(encryptInfo.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));                          //密钥截至时间
                listArgs.Add(encryptInfo.Password);                                                         //新密钥
                listArgs.Add(string.Empty);                                                                 //无
                Service06ServerInfo server = new Service06ServerInfo();
                server.Host = Session.AppServerInfo.Address;
                server.Port = Session.AppServerInfo.SupportHttps
                    ? Session.AppServerInfo.Port - 7
                    : Session.AppServerInfo.Port - 6;
                OnDebug("GetRealPass", string.Format("Getting real password.\t{0}", encryptInfo.ServerAddress));
                optReturn = Service06Helper.DoOperation(server, Service06Command.GET_PASS, listArgs);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strReturn = optReturn.Data.ToString();
                string[] listReturn = strReturn.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                if (listReturn.Length <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Return value length invalid");
                    return optReturn;
                }
                string strError = listReturn[0];
                strError = ((S3106App)CurrentApp).DecryptString(strError);
                if (strError.StartsWith("ERROR"))
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_GET_REAL_PASSWORD_FAIL;
                    optReturn.Message = string.Format("Get real password fail.\t{0}", strError);
                    return optReturn;
                }
                string strKey1B = string.Empty;
                if (listReturn.Length > 2)
                {
                    strKey1B = listReturn[2];
                }
                strKey1B = ((S3106App)CurrentApp).DecryptString(strKey1B);
                encryptInfo.RealPassword = strKey1B;
                OnDebug("GetRealPass",
                    string.Format("Get real password end.\t{0}", ((S3106App)CurrentApp).EncryptString(strKey1B)));
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

        public OperationReturn DownloadFileToLocal(string fileName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = string.Empty;
                optReturn.Data = path;
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_SESSION;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("FileName is empty");
                    return optReturn;
                }
                //录屏文件的播放要借助MediaUtils，如果本地没有，需要从服务器上下载
                optReturn = Utils.DownloadMediaUtils(Session);
                //optReturn = DownloadMediaUtils();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ConstValue.TEMP_PATH_MEDIADATA);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, string.Format("{0}_{1}", Session.SessionID, fileName));
                optReturn.Data = path;
                //如果已经存在，无需重复下载
                if (!File.Exists(path))
                {
                    DownloadConfig config = new DownloadConfig();
                    config.Method = Session.AppServerInfo.SupportHttps ? 2 : 1;
                    config.Host = Session.AppServerInfo.Address;
                    config.Port = Session.AppServerInfo.Port;
                    config.IsAnonymous = true;
                    config.RequestPath = string.Format("{0}/{1}", ConstValue.TEMP_DIR_MEDIADATA,
                        fileName);
                    config.SavePath = path;
                    optReturn = DownloadHelper.DownloadFile(config);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    optReturn.Data = path;
                }
                OnDebug("DownloadLocal", string.Format("Download end.\t{0}", path));
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


        #region Others

        private OperationReturn GetRecordPath(DownloadParamInfo downloadParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mRecordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = RET_NO_RECORDINFO;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                string fmt = PATH_FORMAT;
                if (downloadParam != null
                    && !string.IsNullOrEmpty(downloadParam.VocPathFormat))
                {
                    if ((mRecordInfo.MediaType == 1
                        || mRecordInfo.MediaType == 0)
                        && !string.IsNullOrEmpty(downloadParam.VocPathFormat))
                    {
                        fmt = downloadParam.VocPathFormat;
                    }
                    if (mRecordInfo.MediaType == 2
                        && !string.IsNullOrEmpty(downloadParam.ScrPathFormat))
                    {
                        fmt = downloadParam.ScrPathFormat;
                    }
                }
                optReturn.Message = fmt;
                int iYear, iMonth, iDay, iVoiceID, iChannelID;
                DateTime dtStartRecordTime;

                dtStartRecordTime = mRecordInfo.StartRecordTime;
                iYear = dtStartRecordTime.Year;
                iMonth = dtStartRecordTime.Month;
                iDay = dtStartRecordTime.Day;
                iVoiceID = mRecordInfo.VoiceID;
                iChannelID = mRecordInfo.ChannelID;

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

        #endregion


        #region Debug

        public event Action<string, string> Debug;

        private void OnDebug(string category, string msg)
        {
            if (Debug != null)
            {
                Debug(category, msg);
            }
        }

        #endregion


        #region ReturnCode

        /*
         * 返回值代码
         * 
         * 注：三位的代码是一般错误代码，参考VoiceCyber.Common.Defines中的定义
         * 
         * 这里列出的是自定义的错误代码，仅限RecordOperator，为四位代码
         * 
         */

        public const int RET_NO_SESSION = 1001;                     //没有SessionInfo
        public const int RET_NO_RECORDINFO = 1002;                  //没有指定记录
        public const int RET_DOWNLOADSERVER_NOT_EXIST = 1003;       //下载服务器不存在（下载参数列表和SftpServer列表中均未找到）
        public const int RET_DOWNLOAD_APPSERVER_FAIL = 1004;        //将文件下载到AppServer失败
        public const int RET_GET_REAL_PASSWORD_FAIL = 1005;         //获取实际密钥失败

        #endregion


        #region VoiceFormat

        public const string VOICEFORMAT_G729_A = "G729a";

        #endregion

    }
}
