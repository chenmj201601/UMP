//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    b42a2910-27ce-4cd3-9ecf-44be0e8657e7
//        CLR Version:              4.0.30319.42000
//        Name:                     MediaServer
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService12
//        File Name:                MediaServer
//
//        Created by Charley at 2016/9/14 17:41:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService12;
using VoiceCyber.UMP.Encryptions;


namespace UMPService12
{
    public class MediaServer
    {

        #region Memebers

        private string mAppName = "UMPService12";
        private LogOperator mLogOperator;
        private ConfigInfo mConfigInfo;
        private string mRootDir;
        private string mMediaDataDir;
        private string mMediaUtilDir;
        private int mPort;

        private HttpListener mListener;
        private Thread mThreadMediaDataRecycle;
        private int mMediaDataRecycleInterval = 30;         //回删MediaData的频率，30m
        private int mMediaDataSaveDays = 2;                 //回删天数，2d

        #endregion


        public MediaServer()
        {
            mPort = 8081 - 12;
        }


        public void Start()
        {
            try
            {
                if (Program.IsConsole)
                {
                    CreateFileLog();
                }
                OnDebug(LogMode.Info, string.Format("MediaServer starting..."));
                Init();
                StartListener();
                CreateMediaDataRecycleThread();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaServer start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                StopMediaDataRecycleThread();
                StopListener();
                OnDebug(LogMode.Info, string.Format("MediaServer stopped"));
                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                    mLogOperator = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaServer stop fail.\t{0}", ex.Message));
            }
        }


        #region Init

        private void Init()
        {
            try
            {
                //加载配置选项
                LoadConfigInfo();
                ApplyConfigInfo();

                //获取主目录
                string path = AppDomain.CurrentDomain.BaseDirectory;
                OnDebug(LogMode.Info, string.Format("Root directory:{0}", path));
                mRootDir = path;
                mMediaDataDir = Path.Combine(mRootDir, ConstValue.TEMP_DIR_MEDIADATA);
                if (!Directory.Exists(mMediaDataDir))
                {
                    Directory.CreateDirectory(mMediaDataDir);
                }
                OnDebug(LogMode.Info, string.Format("MediaData path:{0}", mMediaDataDir));
                mMediaUtilDir = Path.Combine(mRootDir, ConstValue.TEMP_DIR_MEDIAUTILS);
                if (!Directory.Exists(mMediaUtilDir))
                {
                    Directory.CreateDirectory(mMediaUtilDir);
                }
                OnDebug(LogMode.Info, string.Format("MediaUtil path:{0}", mMediaUtilDir));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaServer init fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 侦听连接

        private void StartListener()
        {
            if (mListener != null)
            {
                try
                {
                    mListener.Stop();
                }
                catch { }
                mListener = null;
            }
            try
            {
                mListener = new HttpListener();
                string strUrl = string.Format("http://*:{0}/", mPort);
                string[] prefixs = { strUrl };
                for (int i = 0; i < prefixs.Length; i++)
                {
                    mListener.Prefixes.Add(prefixs[i]);
                }
                mListener.Start();
                OnDebug(LogMode.Info, string.Format("Listener started. \t{0}", strUrl));

                mListener.BeginGetContext(AcceptClient, mListener);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StartListener fail.\t{0}", ex.Message));
            }
        }

        private void StopListener()
        {
            try
            {
                if (mListener != null)
                {
                    mListener.Stop();
                }
                mListener = null;
                OnDebug(LogMode.Info, string.Format("MediaListener stopped."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopListener fail.\t{0}", ex.Message));
            }
        }

        private void AcceptClient(IAsyncResult result)
        {
            try
            {
                var listener = result.AsyncState as HttpListener;
                if (listener == null)
                {
                    OnDebug(LogMode.Error, string.Format("HttpListener is null"));
                    return;
                }

                HttpListenerContext context = listener.EndGetContext(result);
                OnDebug(LogMode.Info, string.Format("New client connected."));
                DealRequest(context);

                listener.BeginGetContext(AcceptClient, listener);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("AcceptClient fail.\t{0}", ex.Message));
            }
        }

        private void DealRequest(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                string strName = string.Empty;
                var remote = request.RemoteEndPoint;
                if (remote != null)
                {
                    strName = remote.ToString();
                }

                #region 以下打印请求相关信息

                OnDebug(LogMode.Info, strName, string.Format("{0} {1}", request.HttpMethod, request.RawUrl));
                if (request.AcceptTypes != null)
                    OnDebug(LogMode.Info, strName, string.Format("Accept: {0}", string.Join(",", request.AcceptTypes)));
                if (request.UserLanguages != null)
                    OnDebug(LogMode.Info, strName, string.Format("Accept-Language: {0}", string.Join(",", request.UserLanguages)));
                OnDebug(LogMode.Info, strName, string.Format("User-Agent: {0}", request.UserAgent));
                OnDebug(LogMode.Info, strName, string.Format("Accept-Encoding: {0}", request.Headers["Accept-Encoding"]));
                OnDebug(LogMode.Info, strName, string.Format("Connection: {0}", request.KeepAlive ? "Keep-Alive" : "close"));
                OnDebug(LogMode.Info, strName, string.Format("Host: {0}", request.UserHostName));
                OnDebug(LogMode.Info, strName, string.Format("Pragma: {0}", request.Headers["Pragma"]));
                OnDebug(LogMode.Info, strName, string.Format("Local: {0}", request.LocalEndPoint));
                OnDebug(LogMode.Info, strName, string.Format("Remote: {0}", request.RemoteEndPoint));

                #endregion


                var requestStream = request.InputStream;
                var reader = new StreamReader(requestStream, Encoding.UTF8);
                string strContent = reader.ReadToEnd();
                var url = request.Url;
                string path = url.AbsolutePath;
                string query = url.Query;
                path = path.ToLower();
                path = path.TrimStart('/');
                var response = context.Response;
                RequestParamInfo requestInfo = new RequestParamInfo();
                requestInfo.Name = strName;
                requestInfo.Action = path;
                requestInfo.Args = query.ToLower().TrimStart(new[] { '?' });
                requestInfo.Data = strContent;
                requestInfo.Context = context;
                requestInfo.Request = request;
                requestInfo.Response = response;
                switch (path)
                {
                    case Service12Consts.HTTP_ACTION_NAME_PLAY:
                        DealActionPlay(requestInfo);
                        break;
                    default:
                        ResponseError(requestInfo);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealRequest fail.\t{0}", ex.Message));
            }
        }

        private void ResponseError(RequestParamInfo requestInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = false;
            optReturn.Code = Defines.RET_FAIL;
            optReturn.Message = string.Format("Unkown error.");
            ResponseError(requestInfo, optReturn);
        }

        private void ResponseError(RequestParamInfo requestInfo, OperationReturn optReturn)
        {
            try
            {
                JsonObject jsonResponse = new JsonObject();
                jsonResponse["Result"] = new JsonProperty(optReturn.Result);
                jsonResponse["Code"] = new JsonProperty(optReturn.Code);
                jsonResponse["Message"] = new JsonProperty(string.Format("\"{0}\"", optReturn.Message));

                var response = requestInfo.Response;
                if (response == null) { return; }
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Content-type", "application/x-www-form-urlencoded");
                var responseStream = response.OutputStream;
                string strResponse = jsonResponse.ToString();
                var writer = new StreamWriter(responseStream, Encoding.UTF8);
                writer.WriteLine(strResponse);
                writer.Flush();
                writer.Close();
                responseStream.Flush();
                responseStream.Close();

                string strName = requestInfo.Name;
                if (!string.IsNullOrEmpty(strName))
                {
                    OnDebug(LogMode.Error, strName, string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ResponseError fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations

        private void DealActionPlay(RequestParamInfo requestInfo)
        {
            try
            {
                OperationReturn optReturn = new OperationReturn();
                optReturn.Result = true;
                optReturn.Code = 0;
                string strName = requestInfo.Name;
                if (string.IsNullOrEmpty(strName)) { return; }
                string strArgs = requestInfo.Args;
                strArgs = DecryptString004(strArgs);
                //OnDebug(LogMode.Info,strName, string.Format("Args:{0}", strArgs));
                var args = HttpUtility.ParseQueryString(strArgs);
                if (args.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    ResponseError(requestInfo, optReturn);
                    return;
                }


                #region 获取Token,录音流水号

                string strToken = args[Service12Consts.HTTP_PARAM_NAME_TOKEN];
                if (string.IsNullOrEmpty(strToken))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Token empty.");
                    ResponseError(requestInfo, optReturn);
                    return;
                }
                string strRecordRef = args[Service12Consts.HTTP_PARAM_NAME_RECORDREFERENCE];
                if (string.IsNullOrEmpty(strRecordRef))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RecordReference empty.");
                    ResponseError(requestInfo, optReturn);
                    return;
                }

                #endregion


                string strFileName = string.Format("{0}_{1}", strToken, strRecordRef);
                string strExt = "wav";
                string strTemp;
                string strSource;
                string strTarget;
                int intValue;


                #region 将文件拷贝到MediaData中

                strTarget = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strFileName, strExt));
                if (!File.Exists(strTarget))
                {
                    //获取存储路径
                    string strSavePath = args[Service12Consts.HTTP_PARAM_NAME_SAVEPATH];
                    if (string.IsNullOrEmpty(strSavePath))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("SavePath empty.");
                        ResponseError(requestInfo, optReturn);
                        return;
                    }
                    strSource = strSavePath;
                    File.Copy(strSource, strTarget, true);
                    OnDebug(LogMode.Info, strName, string.Format("Copy file end.\t{0}\t{1}", strSource, strTarget));
                }

                #endregion


                #region 原始解密

                string strOriginalDecrypt = args[Service12Consts.HTTP_PARAM_NAME_ORIGINALDECRYPT];
                if (!string.IsNullOrEmpty(strOriginalDecrypt)
                    && strOriginalDecrypt == "1")
                {
                    string strFileConvert = Path.Combine(mMediaUtilDir, ConstValue.TEMP_FILE_FILECONVERT);
                    if (!File.Exists(strFileConvert))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                        optReturn.Message = string.Format("FileConvert not exist.\t{0}", strFileConvert);
                        ResponseError(requestInfo, optReturn);
                        return;
                    }
                    strTemp = string.Format("{0}_O", strFileName);
                    strTarget = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strTemp, strExt));
                    if (!File.Exists(strTarget))
                    {
                        strSource = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strFileName, strExt));
                        if (!File.Exists(strSource))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                            optReturn.Message = string.Format("SourceFile not exist.\t{0}", strSource);
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        string strCmd = string.Format("\"{0}\" \"{1}\" /r:all /d", strSource, strTarget);
                        optReturn = ExecuteExe(strFileConvert, strCmd);
                        if (!optReturn.Result)
                        {
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        strFileName = strTemp;
                        OnDebug(LogMode.Info, strName, string.Format("OriginalDecrypt file end.\t{0}\t{1}", strSource, strTarget));
                    }
                }

                #endregion


                #region 解密文件

                string strDecryptFile = args[Service12Consts.HTTP_PARAM_NAME_DECRYPTFILE];
                if (!string.IsNullOrEmpty(strDecryptFile)
                    && strDecryptFile == "1")
                {
                    string strFileConvert = Path.Combine(mMediaUtilDir, ConstValue.TEMP_FILE_FILECONVERT);
                    if (!File.Exists(strFileConvert))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                        optReturn.Message = string.Format("FileConvert not exist.\t{0}", strFileConvert);
                        ResponseError(requestInfo, optReturn);
                        return;
                    }
                    strTemp = string.Format("{0}_D", strFileName);
                    strTarget = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strTemp, strExt));
                    if (!File.Exists(strTarget))
                    {
                        string strDecryptPassword = args[Service12Consts.HTTP_PARAM_NAME_DECRYPTPASSWORD];
                        if (string.IsNullOrEmpty(strDecryptPassword))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DecryptPassword empty.");
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        strDecryptPassword = DecryptString004(strDecryptPassword);
                        strSource = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strFileName, strExt));
                        if (!File.Exists(strSource))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                            optReturn.Message = string.Format("SourceFile not exist.\t{0}", strSource);
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        string strCmd = string.Format("\"{0}\" \"{1}\" /r:all /d /p:\"{2}\"", strSource, strTarget,
                            strDecryptPassword);
                        optReturn = ExecuteExe(strFileConvert, strCmd);
                        if (!optReturn.Result)
                        {
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        strFileName = strTemp;
                        OnDebug(LogMode.Info, strName, string.Format("Decrypt file end.\t{0}\t{1}", strSource, strTarget));
                    }
                }

                #endregion


                #region 转换格式

                string strConvertWavFormat = args[Service12Consts.HTTP_PARAM_NAME_CONVERTWAVFORMAT];
                if (!string.IsNullOrEmpty(strConvertWavFormat))
                {
                    int wavFormat;
                    int wavUtil = 1;
                    int wavExt = 1;
                    if (!int.TryParse(strConvertWavFormat, out intValue))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("ConvertWavFormat invalid.");
                        ResponseError(requestInfo, optReturn);
                        return;
                    }
                    wavFormat = intValue;
                    if (wavFormat > 0)
                    {
                        string strConvertWavUtil = args[Service12Consts.HTTP_PARAM_NAME_CONVERTWAVUTIL];
                        if (!string.IsNullOrEmpty(strConvertWavUtil)
                            && int.TryParse(strConvertWavUtil, out intValue))
                        {
                            wavUtil = intValue;
                        }
                        string strConvertWavExt = args[Service12Consts.HTTP_PARAM_NAME_CONVERTWAVEXTENSION];
                        if (!string.IsNullOrEmpty(strConvertWavExt)
                            && int.TryParse(strConvertWavExt, out intValue))
                        {
                            wavExt = intValue;
                        }
                        strSource = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strFileName, strExt));
                        if (!File.Exists(strSource))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                            optReturn.Message = string.Format("SourceFile not exist.\t{0}", strSource);
                            ResponseError(requestInfo, optReturn);
                            return;
                        }
                        strExt = wavExt == 2 ? "mp3" : "wav";
                        strTemp = string.Format("{0}_F", strFileName);
                        strTarget = Path.Combine(mMediaDataDir, string.Format("{0}.{1}", strTemp, strExt));
                        if (!File.Exists(strTarget))
                        {
                            string strUtil;
                            string strCmd;
                            if (wavUtil == 2)
                            {
                                strUtil = Path.Combine(mMediaUtilDir, "lame.exe");
                                strCmd = string.Format("-V 2 \"{0}\" \"{1}\"", strSource, strTarget);
                            }
                            else if (wavUtil == 3)
                            {
                                strUtil = Path.Combine(mMediaUtilDir, "ffmpeg.exe");
                                strCmd = string.Format("-y -i \"{0}\" -ar 8000 -ab 12.2k -ac 1 \"{1}\"", strSource, strTarget);
                            }
                            else
                            {
                                strUtil = Path.Combine(mMediaUtilDir, "FileConvert.exe");
                                strCmd = string.Format("\"{0}\" \"{1}\" /r:all /f:{2}", strSource, strTarget, wavFormat);
                            }
                            if (!File.Exists(strUtil))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                                optReturn.Message = string.Format("WavUtil not exist.");
                                ResponseError(requestInfo, optReturn);
                                return;
                            }
                            optReturn = ExecuteExe(strUtil, strCmd);
                            if (!optReturn.Result)
                            {
                                ResponseError(requestInfo, optReturn);
                                return;
                            }
                            strFileName = strTemp;
                            OnDebug(LogMode.Info, strName, string.Format("ConvertWavFormat end.\t{0}\t{1}", strSource, strTarget));
                        }
                    }
                }

                #endregion


                #region 返回文件流

                string strFilePath = Path.Combine(mMediaDataDir, string.Format("{0}.wav", strFileName));
                if (!File.Exists(strFilePath))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Media file not exist.\t{0}", strFilePath);
                    ResponseError(requestInfo, optReturn);
                    return;
                }
                var response = requestInfo.Response;
                if (response == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Response is null.");
                    ResponseError(requestInfo, optReturn);
                    return;
                }
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Content-type", "application/octet-stream");
                var responseStream = response.OutputStream;
                if (responseStream == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ResponseStream is null.");
                    ResponseError(requestInfo, optReturn);
                    return;
                }
                FileStream fs = File.OpenRead(strFilePath);
                int bufferSize = 10240;
                byte[] buffer = new byte[bufferSize];
                int intSize;
                do
                {
                    intSize = fs.Read(buffer, 0, bufferSize);
                    if (intSize > 0)
                    {
                        responseStream.Write(buffer, 0, intSize);
                    }
                } while (intSize > 0);
                fs.Close();
                responseStream.Flush();
                responseStream.Close();
                OnDebug(LogMode.Info, strName, string.Format("Send file end\t{0}", strFilePath));

                #endregion

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealActionPlay fail.\t{0}", ex.Message));
                ResponseError(requestInfo);
            }
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
                        optReturn.Code = Service12Consts.DECRYPT_PASSWORD_ERROR;
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

        #endregion


        #region MediaData 回删

        private void CreateMediaDataRecycleThread()
        {
            if (mThreadMediaDataRecycle != null)
            {
                try
                {
                    mThreadMediaDataRecycle.Abort();
                }
                catch { }
                mThreadMediaDataRecycle = null;
            }
            try
            {
                mThreadMediaDataRecycle = new Thread(MediaDataRecycleWorker);
                mThreadMediaDataRecycle.Start();
                OnDebug(LogMode.Info,
                    string.Format("MediaDataRecycle thread created.\t{0}", mThreadMediaDataRecycle.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateMediaDataRecycleThread fail.\t{0}", ex.Message));
            }
        }

        private void StopMediaDataRecycleThread()
        {
            if (mThreadMediaDataRecycle != null)
            {
                try
                {
                    mThreadMediaDataRecycle.Abort();
                }
                catch { }
                mThreadMediaDataRecycle = null;
                OnDebug(LogMode.Info, string.Format("MediaDataRecycle thread stopped."));
            }
        }

        private void MediaDataRecycleWorker()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mMediaDataRecycleInterval * 60 * 1000);

                    try
                    {
                        string strPath = mMediaDataDir;
                        if (Directory.Exists(strPath))
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(strPath);
                            var files = dirInfo.GetFiles().OrderBy(f => f.CreationTime).ToList();
                            DateTime now = DateTime.Now;
                            for (int i = 0; i < files.Count; i++)
                            {
                                var file = files[i];
                                if ((now - file.CreationTime).TotalDays > mMediaDataSaveDays)
                                {
                                    file.Delete();
                                }
                            }
                        }
                    }
                    catch (ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("MediaDataRecycleWorker fail.\t{0}", ex.Message));
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaDataRecycleWorker fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region ConfigInfo

        private void LoadConfigInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_DIR_UMP);
                path = Path.Combine(path, mAppName);
                path = Path.Combine(path, ConstValue.TEMP_FILE_CONFIGINFO);
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("ConfigInfo file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<ConfigInfo>(path);
                if (!optReturn.Result)
                {
                    OnDebug(LogMode.Error,
                        string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ConfigInfo configInfo = optReturn.Data as ConfigInfo;
                if (configInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("LoadConfigInfo fail.\tConfigInfo is null"));
                    return;
                }
                mConfigInfo = configInfo;
                OnDebug(LogMode.Info, string.Format("LoadConfigInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadConfigInfo fail.\t{0}", ex.Message));
            }
        }

        private void ApplyConfigInfo()
        {
            try
            {
                if (mConfigInfo == null
                    || mConfigInfo.ListSettings == null) { return; }

                SetLogMode();

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ApplyConfigInfo fail.\t{0}", ex.Message));
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

                if (Program.IsConsole)
                {
                    WriteLog(mode, category, msg);
                }
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "MediaServer", msg);
        }

        #endregion


        #region LogOperator

        private void CreateFileLog()
        {
            try
            {
                string path = GetLogPath();
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                if (Program.IsDebug)
                {
                    //调试模式下记录所有日志信息
                    mLogOperator.LogMode = LogMode.All;
                }
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("LogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n", Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog(LogMode.Info, string.Format("{0}", strInfo));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("CreateFileLog fail.\t{0}", ex.Message));
            }
        }

        private void WriteLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        private void WriteLog(string category, string msg)
        {
            WriteLog(LogMode.Info, category, msg);
        }

        private void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, "MediaServer", msg);
        }

        private void WriteLog(string msg)
        {
            WriteLog(LogMode.Info, msg);
        }

        private string GetLogPath()
        {
            string strReturn = string.Empty;
            try
            {
                //从LocalMachine文件中读取日志路径
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"VoiceCyber\UMP\config\localmachine.ini");
                if (File.Exists(path))
                {
                    string[] arrInfos = File.ReadAllLines(path, Encoding.Default);
                    for (int i = 0; i < arrInfos.Length; i++)
                    {
                        string strInfo = arrInfos[i];
                        if (strInfo.StartsWith("LogPath="))
                        {
                            string str = strInfo.Substring(8);
                            if (!string.IsNullOrEmpty(str))
                            {
                                strReturn = str;
                                break;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(strReturn)
                    || !Directory.Exists(strReturn))
                {
                    //如果读取失败，或者目录不存在，使用默认目录
                    strReturn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        string.Format("UMP\\{0}\\Logs", mAppName));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, mAppName);
                }
                //创建日志文件夹
                if (!Directory.Exists(strReturn))
                {
                    Directory.CreateDirectory(strReturn);
                }
            }
            catch { }
            return strReturn;
        }

        private void SetLogMode()
        {
            try
            {
                if (mConfigInfo == null) { return; }
                var setting = mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_LOG_MODE);
                if (setting == null) { return; }
                string strValue = setting.Value;
                int intValue;
                if (int.TryParse(strValue, out intValue)
                    && intValue > 0)
                {
                    if (mLogOperator != null)
                    {
                        mLogOperator.LogMode = (LogMode)intValue;
                        OnDebug(LogMode.Info, string.Format("LogMode changed.\t{0}", (LogMode)intValue));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("SetLogMode fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Encryption

        private string DecryptString004(string source)
        {
            try
            {
                return ServerAESEncryption.DecryptString(source, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return source;
            }
        }

        #endregion

    }
}
