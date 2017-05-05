//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    78be8f51-b198-4124-a76e-96a12fbba770
//        CLR Version:              4.0.30319.18063
//        Name:                     ls_FTP_Server
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSftp
//        File Name:                ls_FTP_Server
//
//        created by Charley at 2015/9/10 17:51:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using LumiSoft.Net;
using LumiSoft.Net.FTP.Server;
using UMPCommon;

namespace UMPSftp
{
    /// <summary>
    /// Virtual ftp server.
    /// </summary>
    internal class ls_FTP_Server
    {
        private FTP_Server m_pServer = null;
        private string m_FtpRoot = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ftpRootDir">Ftp root dir for this server.</param>
        /// <param name="ip">IP address on which to listen.</param>
        /// <param name="port">Port on which to listen.</param>
        public ls_FTP_Server(string ftpRootDir, string ip, int port)
        {
            m_FtpRoot = ftpRootDir;

            m_pServer = new LumiSoft.Net.FTP.Server.FTP_Server();

            this.m_pServer.CommandIdleTimeOut = 5000;
            this.m_pServer.LogCommands = false;
            this.m_pServer.MaxBadCommands = 30;
            this.m_pServer.SessionIdleTimeOut = 8000;
            this.m_pServer.Port = port;
            this.m_pServer.IpAddress = ip;
            this.m_pServer.CreateDir += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnCreateDir);
            this.m_pServer.StoreFile += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnStoreFile);
            this.m_pServer.DeleteDir += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnDeleteDir);
            this.m_pServer.SysError += new LumiSoft.Net.ErrorEventHandler(this.OnSysError);
            this.m_pServer.DirExists += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnDirExists);
            this.m_pServer.FileExists += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnFileExists);
            this.m_pServer.RenameDirFile += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnRenameDirFile);
            this.m_pServer.DeleteFile += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnDeleteFile);
            this.m_pServer.ValidateIPAddress += new LumiSoft.Net.ValidateIPHandler(this.OnValidateIP);
            this.m_pServer.GetFile += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnGetFile);
            this.m_pServer.GetDirInfo += new LumiSoft.Net.FTP.Server.FileSysEntryEventHandler(this.OnGetDirInfo);
            this.m_pServer.AuthUser += new LumiSoft.Net.FTP.Server.AuthUserEventHandler(this.OnAuthUser);
            this.m_pServer.SessionLog += m_pServer_SessionLog;
            m_pServer.Enabled = true;
            m_pServer.LogCommands = true;
        }

        void m_pServer_SessionLog(object sender, Log_EventArgs e)
        {
            LogHelper.InfoLog(e.LogText);
        }


        #region Events handling

        #region FTP server events

        #region method OnAuthUser

        public void OnValidateIP(object sender, ValidateIP_EventArgs e)
        {
            e.Validated = true;
        }

        #endregion

        #region method OnAuthUser

        public void OnAuthUser(object sender, AuthUser_EventArgs e)
        {
            LogHelper.InfoLog(string.Format("{0} clinet count:{1}", IPAddress.Parse(((IPEndPoint)e.Session.RemoteEndPoint).Address.ToString()), m_pServer.ClientCount));
            string[] user = e.UserName.Split('|');
            if (user.Length < 2)
            {
                LogHelper.InfoLog(string.Format("User's format error,user:{0};", e.UserName));
                e.Validated = false;
                return;
            }
            OtherDirFileInfo ofi = new OtherDirFileInfo();
            ofi.UserName = user[0];
            ofi.TenantID = user[1];
            ofi.Password = e.PasswData;
            Parameters par = Parameters.Instance();
            string result = Common.SendSSLMsg(par.SftpParam.AuthenticateServerHost, par.SftpParam.AuthenticateServerPort, CreateAuthMsg(ofi));
            string[] strings;
            if (ReturnAuthAnalyze(result, out strings, true))
            {
                LogHelper.InfoLog(string.Format("login successed,user:{0};", e.UserName));
                e.Validated = true;
            }
            else
            {
                LogHelper.InfoLog(string.Format("Authentication Failed,user:{0},pwd:***;", e.UserName));
                LogHelper.DebugLog(string.Format("Authentication server info:{0},{1},pwd:{2};", par.SftpParam.AuthenticateServerHost,
                    par.SftpParam.AuthenticateServerPort, e.PasswData));
                e.Validated = false;
            }
        }

        #endregion


        #region method OnGetDirInfo

        public void OnGetDirInfo(object sender, FileSysEntry_EventArgs e)
        {
            try
            {
                DataTable dt = e.DirInfo.Tables["DirInfo"];

                string physicalPath = GetPhysicalPath(e.Name);

                // Add directories
                if (Directory.Exists(physicalPath))
                {
                    string[] dirs = Directory.GetDirectories(physicalPath);
                    foreach (string d in dirs)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Name"] = new DirectoryInfo(d).Name;
                        dr["Date"] = Directory.GetCreationTime(d);
                        //	dr["Size"] = "";
                        dr["IsDirectory"] = true;

                        dt.Rows.Add(dr);
                    }

                    // Add virtual folders
                    if (File.Exists(physicalPath + "__Config_ftp.xml"))
                    {
                        try
                        {
                            DataSet ds = new DataSet();
                            ds.ReadXml(physicalPath + "__Config_ftp.xml");

                            foreach (DataRow dr in ds.Tables["virtualFolder"].Rows)
                            {
                                // ToDo: if virtual folder is same name as physical folder

                                // Add vitual folder to list only if it exists
                                string vDirPhysicalPath = dr["path"].ToString();
                                if (Directory.Exists(vDirPhysicalPath))
                                {
                                    DataRow drX = dt.NewRow();
                                    drX["Name"] = dr["name"].ToString();
                                    drX["Date"] = Directory.GetCreationTime(vDirPhysicalPath);
                                    //	drX["Size"] = "";
                                    drX["IsDirectory"] = true;

                                    dt.Rows.Add(drX);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    // Add files
                    string[] files = Directory.GetFiles(physicalPath);
                    foreach (string f in files)
                    {
                        // Hide config file
                        if (Path.GetFileName(f).ToLower() != "__config_ftp.xml")
                        {
                            DataRow dr = dt.NewRow();
                            dr["Name"] = Path.GetFileName(f);
                            dr["Date"] = File.GetCreationTime(f);
                            dr["Size"] = new FileInfo(f).Length;
                            dr["IsDirectory"] = false;

                            dt.Rows.Add(dr);
                        }
                    }

                }
            }
            catch
            {
                e.Validated = false;
            }
        }

        #endregion

        #region method OnDirExists

        public void OnDirExists(object sender, FileSysEntry_EventArgs e)
        {
            if (!Directory.Exists(GetPhysicalPath(e.Name)))
            {
                e.Validated = false;
            }
        }

        #endregion

        #region method OnCreateDir

        public void OnCreateDir(object sender, FileSysEntry_EventArgs e)
        {
            if (Directory.Exists(GetPhysicalPath(e.Name)))
            {
                e.Validated = false;
            }
            else
            {
                Directory.CreateDirectory(GetPhysicalPath(e.Name));
            }
        }

        #endregion

        #region method OnDeleteDir

        public void OnDeleteDir(object sender, FileSysEntry_EventArgs e)
        {
            if (!IsVirtualDir(e.Name) && Directory.Exists(GetPhysicalPath(e.Name)))
            {
                Directory.Delete(GetPhysicalPath(e.Name));
            }
            else
            {
                e.Validated = false;
            }
        }

        #endregion

        #region method OnRenameDirFile

        public void OnRenameDirFile(object sender, FileSysEntry_EventArgs e)
        {
            // Remove last /
            string to = e.NewName.Substring(0, e.NewName.Length - 1);
            string from = e.Name.Substring(0, e.Name.Length - 1);

            if (IsVirtualDir(to) || IsVirtualDir(from) || Directory.Exists(GetPhysicalPath(to)) || File.Exists(GetPhysicalPath(to)))
            {
                e.Validated = false;
            }
            else
            {
                if (Directory.Exists(GetPhysicalPath(from)))
                {
                    Directory.Move(GetPhysicalPath(from), GetPhysicalPath(to));
                }
                else if (File.Exists(GetPhysicalPath(from)))
                {
                    File.Move(GetPhysicalPath(from), GetPhysicalPath(to));
                }
            }
        }

        #endregion


        #region method OnFileExists

        public void OnFileExists(object sender, FileSysEntry_EventArgs e)
        {
            try
            {
                if (File.Exists(GetPhysicalPath(e.Name)))
                {
                    e.Validated = true;
                }
            }
            catch
            {
                e.Validated = false;
            }
        }

        #endregion

        #region method OnGetFile

        public void OnGetFile(object sender, FileSysEntry_EventArgs e)
        {
            try
            {
                //if(File.Exists(GetPhysicalPath(e.Name))){
                //	e.FileStream = File.OpenRead(GetPhysicalPath(e.Name));
                OtherDirFileInfo ofi = new OtherDirFileInfo();

                string[] user = e.Session.UserName.Split('|');
                if (user.Length < 2)
                {
                    LogHelper.InfoLog(string.Format("User's format error,user:{0};", e.Session.UserName));
                    return;
                }
                ofi.UserName = user[0];
                ofi.TenantID = user[1];
                ofi.Password = e.Session.Password;

                string[] request_paths = e.Name.Split('-');
                if (request_paths.Length != 3)
                {
                    //LogHelper.InfoLog("Request file format error!");
                    return;
                }

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
                            if (File.Exists(soucre_file_path))
                            {
                                e.FileStream = File.OpenRead(soucre_file_path);
                            }
                            else
                            {
                                LogHelper.ErrorLog(string.Format("file not found,filename:{0}", soucre_file_path));
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.ErrorLog(ex.Message);
                        }
                    }
                    else
                    {
                        LogHelper.ErrorLog(string.Format("AuthAnalyze failed,count:{0}", strings.Length));
                    }
                }
                else
                {
                    LogHelper.ErrorLog(string.Format("Authenticate failed,reference type:{0},reference:{1},partition:{2}"
                        , refe_type, request_paths[1], request_paths[2]));
                }
            }
            catch
            {
                e.Validated = false;
            }
        }

        #endregion

        #region method OnStoreFile

        public void OnStoreFile(object sender, FileSysEntry_EventArgs e)
        {
            try
            {
                LogHelper.InfoLog(string.Format("OnStoreFile, e.Name = {0}", e.Name));

                if (!File.Exists(GetPhysicalPath(e.Name)))
                {
                    e.FileStream = File.Create(GetPhysicalPath(e.Name));
                }
                else
                {
                    try
                    {
                        File.Delete(GetPhysicalPath(e.Name));
                    }
                    catch (System.Exception ex)
                    {
                        LogHelper.ErrorLog(ex);
                    }

                    e.FileStream = File.Create(GetPhysicalPath(e.Name));
                }
            }
            catch(Exception ex)
            {
                LogHelper.ErrorLog(ex);
                e.Validated = false;
            }

        }

        #endregion

        #region method OnDeleteFile

        public void OnDeleteFile(object sender, FileSysEntry_EventArgs e)
        {
            if (File.Exists(GetPhysicalPath(e.Name)))
            {
                File.Delete(GetPhysicalPath(e.Name));
            }
            else
            {
                e.Validated = false;
            }
        }

        #endregion


        #region method OnSysError

        public void OnSysError(object sender, Error_EventArgs e)
        {
            LogHelper.ErrorLog(e.Exception);
        }

        #endregion

        #endregion

        #endregion


        #region method GetPhysicalPath

        /// <summary>
        /// Gets physical path from absolute path (replaces virtual folders with real path).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetPhysicalPath(string path)
        {
            string pPath = m_FtpRoot + "\\";

            bool endsWithSep = false;

            if (path.StartsWith("/"))
            {
                path = path.Substring(1, path.Length - 1);
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
                endsWithSep = true;
            }

            string[] pathParts = path.Split('/', '\\');
            foreach (string part in pathParts)
            {
                if (part.Length > 0)
                {
                    // This is physical directory
                    if (Directory.Exists(pPath + part))
                    {
                        pPath += part + "/";
                    }
                    else
                    {
                        // See if virtual folder
                        if (File.Exists(pPath + "__Config_ftp.xml"))
                        {
                            try
                            {
                                DataSet ds = new DataSet();
                                ds.ReadXml(pPath + "__Config_ftp.xml");

                                ds.Tables["virtualFolder"].DefaultView.RowFilter = "name='" + part + "'";
                                if (ds.Tables["virtualFolder"].DefaultView.Count > 0)
                                {
                                    pPath = ds.Tables["virtualFolder"].DefaultView[0]["path"].ToString();

                                    if (!pPath.EndsWith("\\"))
                                    {
                                        pPath += "/";
                                    }
                                }
                                else
                                {
                                    pPath += part + "/";
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            pPath += part + "/";
                        }
                    }
                }
            }

            if (!endsWithSep && pPath.EndsWith("/"))
            {
                pPath = pPath.Substring(0, pPath.Length - 1);
            }

            return pPath;
        }

        #endregion

        #region method IsVirtualDir

        /// <summary>
        /// Gets if specified dir is virtual directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsVirtualDir(string dir)
        {
            if (dir.StartsWith("/"))
            {
                dir = dir.Substring(1, dir.Length - 1);
            }
            if (dir.EndsWith("/"))
            {
                dir = dir.Substring(0, dir.Length - 1);
            }

            string pPath = "";

            //--- Move dir up and get physical path
            string[] pathParts = dir.Split('/', '\\');
            if (pathParts.Length > 1)
            {
                //		pPath = "";
                for (int i = 0; i < (pathParts.Length - 1); i++)
                {
                    pPath += pathParts[i] + "/";
                }

                if (pPath.Length == 0)
                {
                    pPath = "/";
                }
            }
            //	else{
            //		pPath = "";
            //	}

            pPath = GetPhysicalPath(pPath);
            //---------------------------------------------

            if (!Directory.Exists(pPath + pathParts[pathParts.Length - 1]))
            {
                // See if virtual folder
                if (File.Exists(pPath + "__Config_ftp.xml"))
                {
                    try
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(pPath + "__Config_ftp.xml");

                        ds.Tables["virtualFolder"].DefaultView.RowFilter = "name='" + pathParts[pathParts.Length - 1] + "'";
                        if (ds.Tables["virtualFolder"].DefaultView.Count > 0)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        #endregion


        #region method EndServer

        /// <summary>
        /// Stops server and does clean up.
        /// </summary>
        public void EndServer()
        {
            if (this.m_pServer != null)
            {
                this.m_pServer.Enabled = false;
                this.m_pServer = null;
            }
        }

        #endregion

        string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
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
    }
}
