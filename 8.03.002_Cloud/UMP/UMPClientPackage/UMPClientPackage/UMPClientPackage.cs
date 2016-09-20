using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Xml;

namespace UMPClientPackage
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPClientPackage : ServiceBase
    {
        public static string IStrEventLogSource = "UMP Client Service";
        public static string IStrApplicationName = "UMP";
        public static string IStrBaseDirectory = string.Empty;

        /// <summary>
        /// 服务是否可以继续工作
        /// </summary>
        public bool IBoolCanContinue = true;

        private string IStrPFCertificateFile = string.Empty;
        private string IStrPFCertificateHashString = "C3BBF9EA2C0DA7FEAA17043A0A6010A522ABAB87";
        private string IStrServerCertificateFile = string.Empty;
        private string IStrServerCertificateHashString = string.Empty;

        private Thread IThreadCheckPFCertificate;
        private Thread IThreadCheckServerCertificate;
        private bool IBoolInCheckPFCertificate;
        private bool IBoolCanAbortCheckPFCertificate = true;
        private bool IBoolInCheckServerCertificate;
        private bool IBoolCanAbortCheckServerCertificate = true;

        public UMPClientPackage()
        {
            InitializeComponent();
        }

        #region 服务启动
        protected override void OnStart(string[] args)
        {
            Thread LThreadStart = new Thread(StartThisService);
            LThreadStart.Start();
        }

        private void StartThisService()
        {
            try
            {
                if (!EventLog.SourceExists(IStrEventLogSource)) { EventLog.CreateEventSource(IStrEventLogSource, IStrApplicationName); }
                UMPServiceLog.Source = IStrEventLogSource;
                UMPServiceLog.Log = IStrApplicationName;
                UMPServiceLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);
                WriteEntryLog("Service Started At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);

                IStrBaseDirectory = GetCurrentBaseDirectory();
                WriteEntryLog("AppDomain.CurrentDomain.BaseDirectory :\n" + IStrBaseDirectory, EventLogEntryType.Information);

                //IStrPFCertificateFile = System.IO.Path.Combine(IStrBaseDirectory, "UMP.PF.Certificate.pfx");
                //WriteEntryLog("Certificate File Name :\n" + IStrPFCertificateFile, EventLogEntryType.Information);

                IThreadCheckPFCertificate = new Thread(CheckPFCertificateIsExist);
                IThreadCheckPFCertificate.Start();

                IThreadCheckServerCertificate = new Thread(CheckServiceCertificateIsExist);
                IThreadCheckServerCertificate.Start();
            }
            catch (Exception ex)
            {
                WriteEntryLog("Start Service (Client Package) Exception:\n" + ex, EventLogEntryType.Error);
            }
        }
        #endregion

        #region 服务停止
        protected override void OnStop()
        {
            try
            {
                WriteEntryLog("Stop Service (Client Package) Start", EventLogEntryType.Information);

                IBoolCanContinue = false;
                Thread.Sleep(300);

                WriteEntryLog("Thread Check Certificate Aborting", EventLogEntryType.Information);
                while (IBoolInCheckPFCertificate) { Thread.Sleep(100); }
                while (!IBoolCanAbortCheckPFCertificate) { Thread.Sleep(50); }
                while (IBoolInCheckServerCertificate) { Thread.Sleep(100); }
                while (!IBoolCanAbortCheckServerCertificate) { Thread.Sleep(50); }
                try
                {
                    IThreadCheckPFCertificate.Abort();
                    WriteEntryLog("Thread Check PF Certificate Aborted", EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    WriteEntryLog("Abort Thread Check PF Certificate Failed. Exception :\n" + ex, EventLogEntryType.Warning);
                }
                try
                {
                    IThreadCheckServerCertificate.Abort();
                    WriteEntryLog("Thread Check Server Certificate Aborted", EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    WriteEntryLog("Abort Thread Check Server Certificate Failed. Exception :\n" + ex, EventLogEntryType.Warning);
                }

                WriteEntryLog("Stop Service (Client Package) Finished", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEntryLog("Stop Service (Client Package) Exception:\n" + ex, EventLogEntryType.Error);
            }
        }

        #endregion

        #region 检测 UMP.PF.Certificate.pfx 是否存在,如果不存在则安装
        private void CheckPFCertificateIsExist()
        {
            bool LBoolInTrustedPublisher = true;
            bool LBoolInRoot = true;
            string LStrCallTrustedReturn = string.Empty;
            string LStrCallRoot = string.Empty;

            bool LBoolInstall2TrustedPublisher = true;
            bool LBoolInstall2Root = true;


            try
            {
                while (IBoolCanContinue)
                {
                    IBoolInCheckPFCertificate = true;
                    IBoolCanAbortCheckPFCertificate = false;


                    #region 获取证书文件路径及HashString

                    string strPath =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                    string strCertFile = Path.Combine(strPath, "UMP.PF.Certificate.pfx");
                    if (!File.Exists(strCertFile))
                    {
                        WriteEntryLog(string.Format("Certificate file not exist.\t{0}", strCertFile),
                            EventLogEntryType.Error);
                        Thread.Sleep(10 * 1000);
                        continue;
                    }
                    IStrPFCertificateFile = strCertFile;
                    X509Certificate2 LX509Certificate = new X509Certificate2(IStrPFCertificateFile, "VoiceCyber,123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    string strHashString = LX509Certificate.GetCertHashString();
                    IStrPFCertificateHashString = strHashString;

                    #endregion


                    #region 检测是否存在

                    LBoolInTrustedPublisher = CertificateIsExist(IStrPFCertificateHashString, StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                    LBoolInRoot = CertificateIsExist(IStrPFCertificateHashString, StoreName.Root, StoreLocation.LocalMachine, ref LStrCallRoot);

                    #endregion


                    #region 如果不存在，则安装

                    if (!LBoolInTrustedPublisher)
                    {
                        if (string.IsNullOrEmpty(LStrCallTrustedReturn))
                        {
                            LBoolInstall2TrustedPublisher = InstallCertificate(IStrPFCertificateFile, "VoiceCyber,123", StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                            if (!LBoolInstall2TrustedPublisher)
                            {
                                WriteEntryLog("Failed To Install A Security Certificate. (TrustedPublisher)", EventLogEntryType.Error);
                            }
                            else
                            {
                                WriteEntryLog("Install A Security Certificate. (Root)", EventLogEntryType.Warning);
                            }
                        }
                        else
                        {
                            WriteEntryLog("Error Occurred In The Process Of Detecting The Security Certificate. (TrustedPublisher)", EventLogEntryType.Error);
                        }
                    }

                    if (!LBoolInRoot)
                    {
                        if (string.IsNullOrEmpty(LStrCallRoot))
                        {
                            LBoolInstall2Root = InstallCertificate(IStrPFCertificateFile, "VoiceCyber,123", StoreName.Root, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                            if (!LBoolInstall2Root)
                            {
                                WriteEntryLog("Failed To Install A Security Certificate. (Root)", EventLogEntryType.Error);
                            }
                            else
                            {
                                WriteEntryLog("Install A Security Certificate. (Root)", EventLogEntryType.Warning);
                            }
                        }
                        else
                        {
                            WriteEntryLog("Error Occurred In The Process Of Detecting The Security Certificate. (Root)", EventLogEntryType.Error);
                        }
                    }

                    #endregion


                    Thread.Sleep(10 * 1000);
                }

                IBoolInCheckPFCertificate = false;
                IBoolCanAbortCheckPFCertificate = true;
            }
            catch (Exception ex)
            {
                WriteEntryLog("CheckPFCertificateIsExist()\n" + ex, EventLogEntryType.Error);
            }
        }

        private void CheckServiceCertificateIsExist()
        {
            bool LBoolInTrustedPublisher = true;
            bool LBoolInRoot = true;
            string LStrCallTrustedReturn = string.Empty;
            string LStrCallRootReturn = string.Empty;

            bool LBoolInstall2TrustedPublisher = true;
            bool LBoolInstall2Root = true;


            try
            {
                while (IBoolCanContinue)
                {
                    IBoolInCheckServerCertificate = true;
                    IBoolCanAbortCheckServerCertificate = false;


                    #region 获取证书文件路径及HashString

                    string strPath =
                          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                    string strSettedPath = Path.Combine(strPath, "UMP.Setted.xml");
                    if (!File.Exists(strSettedPath))
                    {
                        WriteEntryLog(string.Format("UMP.Setted.xml file not exist.\t{0}", strSettedPath),
                            EventLogEntryType.Error);
                        Thread.Sleep(10 * 1000);
                        continue;
                    }
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(strSettedPath);
                        var nodeUMPServer = doc.SelectSingleNode("UserSetted/UMPServerSetted") as XmlElement;
                        if (nodeUMPServer == null)
                        {
                            WriteEntryLog(string.Format("UMPServerSetted node not exist."), EventLogEntryType.Error);
                            Thread.Sleep(10 * 1000);
                            continue;
                        }
                        string strHost = nodeUMPServer.GetAttribute("UMPServerHost");
                        string strCertPath = Path.Combine(strPath, string.Format("UMP.S.{0}.pfx", strHost));
                        if (!File.Exists(strCertPath))
                        {
                            WriteEntryLog(string.Format("Certificate file not exist.\t{0}", strCertPath), EventLogEntryType.Error);
                            Thread.Sleep(10 * 1000);
                            continue;
                        }
                        IStrServerCertificateFile = strCertPath;
                    }
                    catch (Exception ex)
                    {
                        WriteEntryLog(string.Format("Get server host fail.\t{0}", ex.Message), EventLogEntryType.Error);
                        Thread.Sleep(10 * 1000);
                        continue;
                    }
                    X509Certificate2 LX509Certificate = new X509Certificate2(IStrServerCertificateFile, "VoiceCyber,123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    string strHashString = LX509Certificate.GetCertHashString();
                    IStrServerCertificateHashString = strHashString;

                    #endregion


                    #region 检测是否存在
                    LBoolInTrustedPublisher = CertificateIsExist(IStrServerCertificateHashString, StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                    LBoolInRoot = CertificateIsExist(IStrServerCertificateHashString, StoreName.Root, StoreLocation.LocalMachine, ref LStrCallRootReturn);
                    #endregion


                    #region 如果不存在，则安装
                    if (!LBoolInTrustedPublisher)
                    {
                        if (string.IsNullOrEmpty(LStrCallTrustedReturn))
                        {
                            LBoolInstall2TrustedPublisher = InstallCertificate(IStrServerCertificateFile, "VoiceCyber,123", StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                            if (!LBoolInstall2TrustedPublisher)
                            {
                                WriteEntryLog("Failed To Install A Security Certificate. (TrustedPublisher)", EventLogEntryType.Error);
                            }
                            else
                            {
                                WriteEntryLog("Install A Security Certificate. (Root)", EventLogEntryType.Warning);
                            }
                        }
                        else
                        {
                            WriteEntryLog("Error Occurred In The Process Of Detecting The Security Certificate. (TrustedPublisher)", EventLogEntryType.Error);
                        }
                    }

                    if (!LBoolInRoot)
                    {
                        if (string.IsNullOrEmpty(LStrCallRootReturn))
                        {
                            LBoolInstall2Root = InstallCertificate(IStrServerCertificateFile, "VoiceCyber,123", StoreName.Root, StoreLocation.LocalMachine, ref LStrCallTrustedReturn);
                            if (!LBoolInstall2Root)
                            {
                                WriteEntryLog("Failed To Install A Security Certificate. (Root)", EventLogEntryType.Error);
                            }
                            else
                            {
                                WriteEntryLog("Install A Security Certificate. (Root)", EventLogEntryType.Warning);
                            }
                        }
                        else
                        {
                            WriteEntryLog("Error Occurred In The Process Of Detecting The Security Certificate. (Root)", EventLogEntryType.Error);
                        }
                    }
                    #endregion


                    Thread.Sleep(10 * 1000);
                }
                IBoolInCheckServerCertificate = false;
                IBoolCanAbortCheckServerCertificate = true;
            }
            catch (Exception ex)
            {
                WriteEntryLog("CheckServerCertificateIsExist()\n" + ex, EventLogEntryType.Error);
            }
        }

        #endregion

        #region 根据 HashString 判断证书是否在 指定的存储区域、存储位置 中存在
        /// <summary>
        /// 根据 HashString 判断证书是否在 指定的存储区域、存储位置 中存在
        /// </summary>
        /// <param name="AStrHashString">HashString</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回获取时的错误信息</param>
        /// <returns>True / False</returns>
        private bool CertificateIsExist(string AStrHashString, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = false;

            try
            {
                AStrReturn = string.Empty;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                {
                    var certHashString = LX509CertificateSingle.GetCertHashString();
                    if (certHashString != null && certHashString.Trim() == AStrHashString) { LBoolReturn = true; break; }
                }
                LX509Store.Close();
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "CertificateIsExist()\n" + ex;
            }

            return LBoolReturn;
        }
        #endregion

        #region 安装证书
        /// <summary>
        /// 安装证书
        /// </summary>
        /// <param name="AStrCertificateFile">证书文件完整路径和名称</param>
        /// <param name="AStrImportPassword">安装证书的密码</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回True时，返回该证书的哈希值（十六进制）；当返回False时，返回安装时的错误信息</param>
        /// <returns></returns>
        private bool InstallCertificate(string AStrCertificateFile, string AStrImportPassword, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;

                X509Certificate2 LX509Certificate = new X509Certificate2(AStrCertificateFile, AStrImportPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                AStrReturn = LX509Certificate.GetCertHashString();

                X509Store LX509Store = null;
                LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.MaxAllowed);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
                LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "InstallCertificate()\n" + ex.Message;
            }

            return LBoolReturn;
        }
        #endregion

        #region 获取当前服务安装的路径
        /// <summary>
        /// 获取当前服务安装的路径
        /// </summary>
        /// <returns></returns>
        public string GetCurrentBaseDirectory()
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch
            {
                LStrReturn = string.Empty;
            }

            return LStrReturn;
        }
        #endregion

        #region 将日志写入 Windows 事情
        /// <summary>
        /// 将日志写入 Windows 事情
        /// </summary>
        /// <param name="AStrWriteBody">日志内容</param>
        /// <param name="AEntryType">日志类型</param>
        private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        {
            try
            {
                UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);
            }
            catch { }
        }
        #endregion
    }
}
