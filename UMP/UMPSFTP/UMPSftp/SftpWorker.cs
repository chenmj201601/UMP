using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using nsoftware.IPWorksSSH;
using System.Collections;
using PFShareClassesC;
using UMPCommon;

namespace UMPSftp
{
    /// <summary>
    /// 上传下载为非SFTP根目录的文件信息类
    /// </summary>
    class OtherDirFileInfo
    {
        public OtherDirFileInfo() { clean(); }
        public void clean()
        {
            m_filePath = string.Empty;
            m_is_download = true;
        }
        //public string m_original_path;
        public string m_filePath;
        public bool m_is_download;
        public string UserName;
        public string Password;
        public string TenantID;
    }

    class SftpWorker
    {
        Hashtable m_filepath = new Hashtable();
        Sftpserver _sftpserver;
        #region sftp event
        void _sftpserver_OnResolvePath(object sender, SftpserverResolvePathEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnResolvePath ");
        }
        void _sftpserver_OnDirList(object sender, SftpserverDirListEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnDirList ");
        }
        void _sftpserver_OnError(object sender, SftpserverErrorEventArgs e)
        {
            LogHelper.ErrorLog("Error - " + e.Description + " ErrorCode:(" + e.ErrorCode.ToString() + ")");
            _sftpserver.Disconnect(e.ConnectionId);
        }
        void _sftpserver_OnSSHStatus(object sender, SftpserverSSHStatusEventArgs e)
        {
            LogHelper.DebugLog(e.Message);
        }
        void _sftpserver_OnFileRead(object sender, SftpserverFileReadEventArgs e)
        {
//             FileStream fs = File.Open(@"D:\sftp\1.wav", FileMode.Open, FileAccess.ReadWrite);
//             byte[] bytes = new byte[e.Length];
//             fs.Read(bytes, (int)e.FileOffset, e.Length);            
//             _sftpserver.Connections[e.ConnectionId].FileDataB = bytes;
//           Console.WriteLine(" _sftpserver_OnFileRead ");
        }
        void _sftpserver_OnSetAttributes(object sender, SftpserverSetAttributesEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnSetAttributes ");
        }
        void _sftpserver_OnFileWrite(object sender, SftpserverFileWriteEventArgs e)
        {            
            Console.WriteLine(" _sftpserver_OnFileWrite ");
        }
        void _sftpserver_OnFileRename(object sender, SftpserverFileRenameEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnFileRename ");
        }
        void _sftpserver_OnFileRemove(object sender, SftpserverFileRemoveEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnFileRemove ");
        }
        void _sftpserver_OnDirRemove(object sender, SftpserverDirRemoveEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnDirRemove ");
        }
        void _sftpserver_OnDirCreate(object sender, SftpserverDirCreateEventArgs e)
        {
            Console.WriteLine(" _sftpserver_OnDirCreate ");
        }
        void _sftpserver_OnConnectionRequest(object sender, SftpserverConnectionRequestEventArgs e)
        {
            LogHelper.InfoLog(e.Address + ":" + e.Port.ToString() + " is attempting to connect.");
        }
        void _sftpserver_OnDisconnected(object sender, SftpserverDisconnectedEventArgs e)
        {
            LogHelper.InfoLog("Now Disconnected - " + e.Description + " (" + e.StatusCode.ToString() + ")");
            m_filepath.Remove(e.ConnectionId);
        }
        void _sftpserver_OnConnected(object sender, SftpserverConnectedEventArgs e)
        {
            LogHelper.InfoLog("Now Connected - " + e.Description + " (" + e.StatusCode.ToString() + ")");
        }
        #endregion

        void _sftpserver_OnSSHUserAuthRequest(object sender, SftpserverSSHUserAuthRequestEventArgs e)
        {

            LogHelper.InfoLog(string.Format("AuthMethod is {0};{1};***;", e.AuthMethod, e.User));

            Parameters par = Parameters.Instance();
            LogHelper.InfoLog(string.Format("IP:{0};Port:{1};", par.SftpParam.AuthenticateServerHost, par.SftpParam.AuthenticateServerPort));

            if (e.AuthMethod == "none")
            {
                LogHelper.InfoLog("AuthMethod is none");
                return;
            }
            string[] user = e.User.Split('|');
            if (user.Length < 2)
            {
                LogHelper.InfoLog(string.Format("User's format error,user:{0};", e.User));
                return;
            }
            OtherDirFileInfo ofi = new OtherDirFileInfo();
            ofi.UserName = user[0];
            ofi.TenantID = user[1];
            ofi.Password = e.AuthParam;
           
            string result = Common.SendSSLMsg(par.SftpParam.AuthenticateServerHost, par.SftpParam.AuthenticateServerPort, CreateAuthMsg(ofi));
            string[] strings;
            if (ReturnAuthAnalyze(result, out strings, true))
            {
                m_filepath.Add(e.ConnectionId, ofi);
                e.Accept = true;
                LogHelper.InfoLog(string.Format("login successed,user:{0};", e.User));
            }
            else
            {
                LogHelper.InfoLog(string.Format("Authentication Failed,user:{0},pwd:***;", e.User));
                LogHelper.DebugLog(string.Format("Authentication server info:{0},{1},pwd:{2};", par.SftpParam.AuthenticateServerHost,
                    par.SftpParam.AuthenticateServerPort,e.AuthParam));
            }
            return;
        }

        void _sftpserver_OnFileClose(object sender, SftpserverFileCloseEventArgs e)
        {
            LogHelper.InfoLog(e.User + " transferred " + e.Path);

            OtherDirFileInfo ofi = m_filepath[e.ConnectionId] as OtherDirFileInfo;
            if (ofi != null)
            {
                if (ofi.m_is_download && !string.IsNullOrEmpty(ofi.m_filePath))
                {
                    try
                    {
                        LogHelper.InfoLog(string.Format("file download success,delete file:{0}", ofi.m_filePath));
                        File.Delete(ofi.m_filePath);
                    }
                    catch (System.Exception ex)
                    {
                        LogHelper.ErrorLog(ex);
                    }
                }
                else
                {
                    Parameters pars = Parameters.Instance();
                    string scr_path = "";
                    if (ofi.m_filePath.IndexOf(".vls") != -1)
                        scr_path = pars.SftpParam.ScrDriver + e.Path;
                    else
                        scr_path = pars.SftpParam.VoxDriver + e.Path;
                    scr_path = scr_path.Replace("/", @"\");
                    try
                    {
                        //如果没有原路径代表可能此文件之前为获取文件大小
                        if (string.IsNullOrEmpty(ofi.m_filePath))
                        {
                            File.Delete(pars.SftpParam.RootDir + e.Path);
                            LogHelper.InfoLog(string.Format("file download success,delete file:{0}sp", ofi.m_filePath));
                        }
                        else
                        {
                            Common.CopyFileEx(ofi.m_filePath, scr_path, true);
                            File.Delete(ofi.m_filePath);
                            LogHelper.InfoLog(string.Format("file upload success,copye file:{0},delete file:{1};", scr_path, ofi.m_filePath));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogHelper.ErrorLog(ex);
                    }
                }
                ofi.clean();
            }
        }

        void _sftpserver_OnFileOpen(object sender, SftpserverFileOpenEventArgs e)
        {
            string operation = "";

            if ((e.Flags & 1) != 0) //Read
            {
                operation = "downloading";
                //如果fileopen状态为没有发现文件则认为这是下载并且文件也确实没有，需要把otherdirfileinfo的状态清空
                if (e.StatusCode == 2)
                {
                    if (m_filepath.Contains(e.ConnectionId))
                    {
                        OtherDirFileInfo odfi = m_filepath[e.ConnectionId] as OtherDirFileInfo;
                        if (!odfi.m_is_download)
                        {
                            //还需要删除多余目录
                            odfi.clean();
                        }
                    }
                }
            }
            if ((e.Flags & 2) != 0) //Write
            {
                operation = "uploading";
            }
            LogHelper.InfoLog(e.User + " started " + operation + " " + e.Path + " " + e.Flags);            
        }

        //根据此开发包特性需要在真正下载之前就要完成准备工作，因此在此事件中完成
        void _sftpserver_OnGetAttributes(object sender, SftpserverGetAttributesEventArgs e)
        {
            LogHelper.InfoLog(string.Format("_sftpserver_OnGetAttributes GetAtt:{0}", e.Path));
            OtherDirFileInfo ofi;
            if (m_filepath.ContainsKey(e.ConnectionId))
            {
                ofi = m_filepath[e.ConnectionId] as OtherDirFileInfo;
                if (!string.IsNullOrEmpty(ofi.m_filePath))
                {
                    return;
                }
            }
            else
            {
                LogHelper.InfoLog(string.Format("Not found OtherDirFileInfo,connectionid:{0}", e.ConnectionId));
                return;
            }

            //发现带有.后缀名的一概认为上传
            if (e.Path.IndexOf(".") > 1)
            {
                try
                {
                    Parameters pars = Parameters.Instance();
                    string dest = pars.SftpParam.RootDir + e.Path;
                    ofi.m_filePath = dest.Replace("/", "\\");//ftp下绝对路径
                    if (!Directory.Exists(ofi.m_filePath.Substring(0, ofi.m_filePath.LastIndexOf("\\"))))
                    {
                        Directory.CreateDirectory(ofi.m_filePath.Substring(0, ofi.m_filePath.LastIndexOf("\\")));
                    }
                    ofi.m_is_download = false;
                }
                catch (System.Exception ex)
                {
                    LogHelper.ErrorLog(ex);
                }
                return;
            }
            //以下为下载规则逻辑，不符合逻辑一概认为请求错误
            //流水号类型-流水号-分区表信息
            //exp:c001-1-1503
            string[] request_paths = e.Path.Split('-');
            if (request_paths.Length != 3)
            {
                //LogHelper.InfoLog("Request file format error!");
                return;
            }
            //如果没发现文件则认为请求的文件为非sftp根目录文件
            if (e.StatusCode == 2)
            {
                string refe_type = request_paths[0].Substring(request_paths[0].IndexOf('/') + 1);
                //按文件名请求绝对路径
                Parameters pars = Parameters.Instance();
                string result = Common.SendSSLMsg(pars.SftpParam.AuthenticateServerHost, pars.SftpParam.AuthenticateServerPort, CreateAuthMsg(ofi, refe_type, request_paths[1], request_paths[2]));
                string[] strings;
                if (ReturnAuthAnalyze(result, out strings, false))
                {
                    if (strings.Length > 2)
                    {
                        string soucre_file_path = strings[1];//源文件绝对路径
                        try
                        {
                            string dest = pars.SftpParam.RootDir + e.Path;
                            if (File.Exists(soucre_file_path))
                            {
                                ofi.m_filePath = dest.Replace("/", @"\");//ftp下绝对路径
                                ofi.m_is_download = true;
                                Common.CopyFileEx(soucre_file_path, ofi.m_filePath, true);
                                using (FileStream fs = File.Open(dest, FileMode.Open, FileAccess.Read))
                                {
                                    e.FileSize = fs.Length;
                                    e.FileType = 1;
                                    e.StatusCode = 0;
                                }
                            }
                            else
                            {
                                LogHelper.ErrorLog(string.Format("file not found,filename:{0}",soucre_file_path));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        LogHelper.ErrorLog(string.Format("AuthAnalyze failed,count:{0}",strings.Length));
                    }
                    m_filepath[e.ConnectionId] = ofi;
                }
                else
                {
                    LogHelper.ErrorLog(string.Format("Authenticate failed,reference type:{0},reference:{1},partition:{2}"
                        ,refe_type,request_paths[1],request_paths[2]));
                }
            }
        }

        public bool StartSFtpServer()
        {
            if (_sftpserver != null)
            {
                _sftpserver.Dispose();
                _sftpserver = null;
            }
            try
            {
                _sftpserver = new Sftpserver();
                _sftpserver.Config("Charset=UTF8");
                _sftpserver.OnConnected += new Sftpserver.OnConnectedHandler(_sftpserver_OnConnected);
                _sftpserver.OnConnectionRequest += new Sftpserver.OnConnectionRequestHandler(_sftpserver_OnConnectionRequest);
                _sftpserver.OnDirList += new Sftpserver.OnDirListHandler(_sftpserver_OnDirList);
                _sftpserver.OnDisconnected += new Sftpserver.OnDisconnectedHandler(_sftpserver_OnDisconnected);
                _sftpserver.OnError += new Sftpserver.OnErrorHandler(_sftpserver_OnError);
                _sftpserver.OnFileClose += new Sftpserver.OnFileCloseHandler(_sftpserver_OnFileClose);
                _sftpserver.OnFileOpen += new Sftpserver.OnFileOpenHandler(_sftpserver_OnFileOpen);
                _sftpserver.OnDirCreate += new Sftpserver.OnDirCreateHandler(_sftpserver_OnDirCreate);
                _sftpserver.OnDirRemove += new Sftpserver.OnDirRemoveHandler(_sftpserver_OnDirRemove);
                _sftpserver.OnSetAttributes += new Sftpserver.OnSetAttributesHandler(_sftpserver_OnSetAttributes);
                _sftpserver.OnFileRead += new Sftpserver.OnFileReadHandler(_sftpserver_OnFileRead);
                _sftpserver.OnFileRemove += new Sftpserver.OnFileRemoveHandler(_sftpserver_OnFileRemove);
                _sftpserver.OnFileRename += new Sftpserver.OnFileRenameHandler(_sftpserver_OnFileRename);
                _sftpserver.OnFileWrite += new Sftpserver.OnFileWriteHandler(_sftpserver_OnFileWrite);
                _sftpserver.OnGetAttributes += new Sftpserver.OnGetAttributesHandler(_sftpserver_OnGetAttributes);
                _sftpserver.OnResolvePath += new Sftpserver.OnResolvePathHandler(_sftpserver_OnResolvePath);
                _sftpserver.OnSSHStatus += new Sftpserver.OnSSHStatusHandler(_sftpserver_OnSSHStatus);
                _sftpserver.OnSSHUserAuthRequest += new Sftpserver.OnSSHUserAuthRequestHandler(_sftpserver_OnSSHUserAuthRequest);

                Parameters pars = Parameters.Instance();
                _sftpserver.SSHCert = new Certificate(CertStoreTypes.cstPFXFile
                    , pars.KeyFilePath
                    , pars.Password
                    , pars.Subject);

                _sftpserver.LocalPort = pars.SftpParam.FTP_Port;
                _sftpserver.RootDirectory = pars.SftpParam.RootDir;
                _sftpserver.Listening = true;
                LogHelper.InfoLog(string.Format("start success. port:{0},RootDir:{1}", pars.SftpParam.FTP_Port,
                    pars.SftpParam.RootDir));
                return true;
            }
            catch (System.Exception ex)
            {
                LogHelper.ErrorLog(ex);
                return false;
            }
        }

        public void StopSFtpServer()
        {
            try
            {
                if (_sftpserver != null)
                {
                    if (_sftpserver.Listening)
                    {
                        _sftpserver.Listening = false;
                        _sftpserver.Shutdown();
                    }
                    _sftpserver.Dispose();
                    _sftpserver = null;
                }

            }
            catch (System.Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
        }

        string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        string CreateAuthMsg(OtherDirFileInfo ofi, string refe_type = "", string refe_value = "", string partition = "")
        {
            if (string.IsNullOrEmpty(ofi.UserName) || string.IsNullOrEmpty(ofi.TenantID) || string.IsNullOrEmpty(ofi.Password))
                return string.Empty;

            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            LStrSendMessage = EncryDecryHelper.EncryptionCommuString("M01A21");                            //0
            LStrSendMessage += AscCodeToChr(27) + (ofi.TenantID);    //1
            LStrSendMessage += AscCodeToChr(27) + (ofi.UserName);    //2
            LStrSendMessage += AscCodeToChr(27) + (ofi.Password);    //3
            LStrSendMessage += AscCodeToChr(27) + (refe_value);      //4
            LStrSendMessage += AscCodeToChr(27) + (refe_type);       //5
            LStrSendMessage += AscCodeToChr(27) + (partition);       //6
            LogHelper.DebugLog(LStrSendMessage);
            return LStrSendMessage;
        }

        bool ReturnAuthAnalyze(string retString, out string[] strings, bool nodata)
        {
            //retString = Common.DecryptionCommuString(retString);
            strings = retString.Split((char)27);
            strings[0] = EncryDecryHelper.DecryptionCommuString(strings[0]);
            if (strings[0] == "S01A00" ||
                (nodata
                && (strings[0] == "E01A61" //获取录音信息失败
                || strings[0] == "E01A62" //没有查询到对应录音信息
                //|| strings[0] == "E01A63" //文件已删除
                )))
            {
                return true;
            }

            LogHelper.ErrorLog(string.Format("Error code:{0}", strings[0]));
            return false;
        }
    }
}
