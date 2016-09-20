using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUninstall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private List<ShotcutInfo> mListShotcutInfos;
        private List<ServiceInfo> mListServiceInfos;
        private InstallProduct mUMPProduct;
        private BackgroundWorker mWorker;
        private LogOperator mLogOperator;
        private bool mIsOptFail;
        private bool mIsContinue;
        private string mErrorMsg;
        private bool mIsInstalled;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListShotcutInfos = new List<ShotcutInfo>();
            mListServiceInfos = new List<ServiceInfo>();

            MouseLeftButtonDown += (s, me) => DragMove();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnMiniButton.Click += (s, me) => WindowState = WindowState.Minimized;
            BtnCloseButton.Click += (s, me) => Close();
            BtnUninstall.Click += BtnUninstall_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLogOperator();
            InitServiceInfos();
            InitShotcutInfos();
            Init();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mIsContinue)
            {
                string strMsg = string.Format("Confirm break uninstallation?");
                var result = MessageBox.Show(strMsg, SetupConsts.APP_NAME, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                WriteLog("AppClose", string.Format("Uninstallation break."));
                mIsContinue = false;
                Thread.Sleep(200);
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                LoadInstallInfo();
                if (mIsInstalled
                    && mUMPProduct != null)
                {
                    TxtVersion.Text = mUMPProduct.Version;
                }
                else
                {
                    TxtVersion.Text = string.Format("Not installed");
                }
                PanelPreUninstall.Visibility = Visibility.Visible;
                PanelUninstallProcess.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadInstallInfo()
        {
            try
            {
                bool is64BitOS = Environment.Is64BitOperatingSystem;
                string path;
                if (is64BitOS)
                {
                    path = string.Format(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0}",
                        UpdateConsts.PACKAGE_GUID_UMP);
                }
                else
                {
                    path = string.Format(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0}",
                        UpdateConsts.PACKAGE_GUID_UMP);
                }
                RegistryKey rootKey = Registry.LocalMachine;
                RegistryKey umpKey = rootKey.OpenSubKey(path);
                if (umpKey != null)
                {
                    mIsInstalled = true;
                    mUMPProduct = new InstallProduct();
                    mUMPProduct.Package = UpdateConsts.PACKAGE_NAME_UMP;
                    mUMPProduct.ProductGuid = UpdateConsts.PACKAGE_GUID_UMP;
                    InstallComponent installComponent = new InstallComponent();
                    installComponent.ModuleID = 0;
                    installComponent.ModuleName = UpdateConsts.COMPONENT_NAME_UMP;
                    mUMPProduct.ListComponents.Add(installComponent);

                    string strVersion = umpKey.GetValue("DisplayVersion").ToString();
                    string strInstallPath = umpKey.GetValue("InstallLocation").ToString();
                    mUMPProduct.Version = strVersion;
                    mUMPProduct.InstallPath = strInstallPath;

                    WriteLog("LoadInstallInfo",
                        string.Format("Version:{0};InstallDirectory:{1}", mUMPProduct.Version,
                            mUMPProduct.InstallPath));
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitServiceInfos()
        {
            try
            {
                mListServiceInfos.Clear();
                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService00";
                serviceInfo.ServiceName = "UMP Service 00";
                serviceInfo.DisplayName = "UMP Service 00";
                serviceInfo.FileName = "UMPService00.exe";
                serviceInfo.ProcessName = "UMPService00";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService01";
                serviceInfo.ServiceName = "UMP Service 01";
                serviceInfo.DisplayName = "UMP Service 01";
                serviceInfo.FileName = "UMPService01.exe";
                serviceInfo.ProcessName = "UMPService01";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService02";
                serviceInfo.ServiceName = "UMP Service 02";
                serviceInfo.DisplayName = "UMP Service 02";
                serviceInfo.FileName = "UMPService02.exe";
                serviceInfo.ProcessName = "UMPService02";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService03";
                serviceInfo.ServiceName = "UMP Service 03";
                serviceInfo.DisplayName = "UMP Service 03";
                serviceInfo.FileName = "UMPService03.exe";
                serviceInfo.ProcessName = "UMPService03";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService04";
                serviceInfo.ServiceName = "UMP Service 04";
                serviceInfo.DisplayName = "UMP Service 04";
                serviceInfo.FileName = "UMPService04.exe";
                serviceInfo.ProcessName = "UMPService04";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService05";
                serviceInfo.ServiceName = "UMP Service 05";
                serviceInfo.DisplayName = "UMP Service 05";
                serviceInfo.FileName = "UMPService05.exe";
                serviceInfo.ProcessName = "UMPService05";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService06";
                serviceInfo.ServiceName = "UMP Service 06";
                serviceInfo.DisplayName = "UMP Service 06";
                serviceInfo.FileName = "UMPService06.exe";
                serviceInfo.ProcessName = "UMPService06";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService07";
                serviceInfo.ServiceName = "UMP Service 07";
                serviceInfo.DisplayName = "UMP Service 07";
                serviceInfo.FileName = "UMPService07.exe";
                serviceInfo.ProcessName = "UMPService07";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService08";
                serviceInfo.ServiceName = "UMP Service 08";
                serviceInfo.DisplayName = "UMP Service 08";
                serviceInfo.FileName = "UMPService08.exe";
                serviceInfo.ProcessName = "UMPService08";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService09";
                serviceInfo.ServiceName = "UMP Service 09";
                serviceInfo.DisplayName = "UMP Service 09";
                serviceInfo.FileName = "UMPService09.exe";
                serviceInfo.ProcessName = "UMPService09";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService10";
                serviceInfo.ServiceName = "UMP Service 10";
                serviceInfo.DisplayName = "UMP Service 10";
                serviceInfo.FileName = "UMPService10.exe";
                serviceInfo.ProcessName = "UMPService10";
                mListServiceInfos.Add(serviceInfo);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format("InitServiceInfos fail.\t{0}", ex.Message));
            }
        }

        private void InitShotcutInfos()
        {
            try
            {
                mListShotcutInfos.Clear();
                ShotcutInfo info = new ShotcutInfo();
                info.Name = "ConfigCenter";
                info.LinkName = "MAMT.lnk";
                info.LinkPath = "MAMT.lnk";
                info.TargetPath = "ManagementMaintenance\\UMP.MAMT.exe";
                info.Args = string.Empty;
                info.Description = "UMP MAMT";
                mListShotcutInfos.Add(info);
                info = new ShotcutInfo();
                info.Name = "Uninstall";
                info.LinkName = "Uninstall.lnk";
                info.LinkPath = "Uninstall.lnk";
                info.TargetPath = "UMPUninstall.exe";
                info.Args = string.Empty;
                info.Description = "Uninstall UMP";
                mListShotcutInfos.Add(info);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void UninstallUMP()
        {
            try
            {
                if (!mIsInstalled)
                {
                    ShowErrorMessage(string.Format("UMP not installed!"));
                    return;
                }
                if (mUMPProduct == null) { return; }
                string strMsg = string.Format("Confirm uninstall UMP?");
                var result = MessageBox.Show(strMsg, SetupConsts.APP_NAME, MessageBoxButton.YesNo,
                     MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                PanelPreUninstall.Visibility = Visibility.Collapsed;
                PanelUninstallProcess.Visibility = Visibility.Visible;
                BtnUninstall.IsEnabled = false;
                SetInstallProgress(0.0);
                WriteLog("Uninstall", string.Format("Begin uninstall"));
                mIsOptFail = false;
                mIsContinue = true;
                mErrorMsg = string.Empty;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    StopWinServices();
                    Thread.Sleep(1000);
                    RemoveWinServices();
                    Thread.Sleep(1000);
                    RemoveShortcuts();
                    Thread.Sleep(1000);
                    RemoveRegistry();
                    Thread.Sleep(1000);
                    RemoveWebSite();
                    Thread.Sleep(1000);
                    RemoveFiles();
                    Thread.Sleep(1000);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetInstallMsg(0, string.Empty);

                    mIsContinue = false;
                    if (mIsOptFail)
                    {
                        WriteLog("Unistall", string.Format("Uninstall fail.\t{0}", mErrorMsg));
                        ShowErrorMessage(string.Format("Uninstall fail.\t{0}", mErrorMsg));
                        PanelPreUninstall.Visibility = Visibility.Visible;
                        PanelUninstallProcess.Visibility = Visibility.Collapsed;
                        BtnUninstall.IsEnabled = true;
                        return;
                    }
                    WriteLog("Unistall", string.Format("Uninstall successful"));
                    ShowInfoMessage(string.Format("Uninstall successful"));
                    RemoveInstallDirectory();
                    Close();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void StopWinServices()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                ServiceController[] services = ServiceController.GetServices();
                int count = mListServiceInfos.Count;
                for (int i = 0; i < mListServiceInfos.Count; i++)
                {
                    var info = mListServiceInfos[i];
                    string strName = info.Name;
                    string path = mUMPProduct.InstallPath;
                    path = Path.Combine(path, "WinServices");
                    string file = Path.Combine(path, info.FileName);
                    if (!File.Exists(file))
                    {
                        WriteLog("StopService", string.Format("File not exist.\t{0}", file));
                        continue;
                    }
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == info.ServiceName);
                    if (controller == null)
                    {
                        WriteLog("StopService", string.Format("WinService not exist.\t{0}", strName));
                        continue;
                    }
                    try
                    {
                        string strMsg = string.Format("{0}", info.Name);
                        SetInstallMsg(SetupConsts.STA_STOPSERVICE, string.Format("{0}", strMsg));
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                        WriteLog("StopService", string.Format("Service stopped.\t{0}", strName));
                    }
                    catch (Exception ex)
                    {
                        WriteLog("StopService", string.Format("Stop service fail.\t{0}", ex.Message));
                    }
                    //等待5s，检查进程是否仍在运行，如果任在运行，杀死进程
                    Thread.Sleep(5 * 1000);
                    KillServiceProcess(info.ServiceName);
                    double percentage = (i + 1) / (count * 1.0);
                    percentage = percentage * SetupConsts.PERCENTAGE_STOPSERVICE;
                    percentage = percentage + SetupConsts.PERCENTAGE_BASE_STOPSERVICE;
                    SetInstallProgress(percentage);
                }
                WriteLog("StopService", string.Format("All service stopped.\t{0}", count));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_STOPSERVICE + SetupConsts.PERCENTAGE_STOPSERVICE);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void KillServiceProcess(string serviceName)
        {
            try
            {
                var serviceInfo = mListServiceInfos.FirstOrDefault(s => s.ServiceName == serviceName);
                if (serviceInfo == null)
                {
                    WriteLog("KillServiceProcess", string.Format("ServiceInfo not exist.\t{0}", serviceName));
                    return;
                }
                string processName = serviceInfo.ProcessName;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    if (!process.ProcessName.Equals(processName)) { continue; }
                    process.Kill();
                    WriteLog("KillServiceProcess", string.Format("Kill process end.\t{0}", processName));
                }
            }
            catch (Exception ex)
            {
                WriteLog("KillServiceProcess", string.Format("KillServiceProcess fail.\t{0}", ex.Message));
            }
        }

        private void RemoveWinServices()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                ServiceController[] services = ServiceController.GetServices();
                int count = mListServiceInfos.Count;
                for (int i = 0; i < mListServiceInfos.Count; i++)
                {
                    var info = mListServiceInfos[i];
                    string strName = info.Name;
                    string path = mUMPProduct.InstallPath;
                    path = Path.Combine(path, "WinServices");
                    string file = Path.Combine(path, info.FileName);
                    string log = Path.Combine(path, string.Format("{0}.InstallLog", strName));
                    if (!File.Exists(file))
                    {
                        WriteLog("RemoveService", string.Format("File not exist.\t{0}", file));
                        continue;
                    }
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == info.ServiceName);
                    if (controller != null)
                    {
                        string strMsg = string.Format("{0}", info.Name);
                        SetInstallMsg(SetupConsts.STA_REMOVESERVICE, strMsg);
                        string[] options = { string.Format("/LogFile={0}", log) };
                        try
                        {
                            TransactedInstaller transacted = new TransactedInstaller();
                            AssemblyInstaller installer = new AssemblyInstaller(file, options);
                            transacted.Installers.Add(installer);
                            transacted.Uninstall(null);
                        }
                        catch (Exception ex)
                        {
                            WriteLog("RemoveService", string.Format("Remove service fail.\t{0}\t{1}", strName, ex.Message));
                            continue;
                        }
                        WriteLog("RemoveService", string.Format("Remove service end.\t{0}", strName));
                        double percentage = (i + 1) / (count * 1.0);
                        percentage = percentage * SetupConsts.PERCENTAGE_REMOVESERVICE;
                        percentage = percentage + SetupConsts.PERCENTAGE_BASE_REMOVESERVICE;
                        SetInstallProgress(percentage);
                    }
                }
                WriteLog("RemoveService", string.Format("All service Removed.\t{0}", count));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_REMOVESERVICE + SetupConsts.PERCENTAGE_REMOVESERVICE);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void RemoveShortcuts()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string linkdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
                    ConstValue.VCT_COMPANY_LONGNAME, ConstValue.UMP_PRODUCTER_SHORTNAME);
                if (Directory.Exists(linkdir))
                {
                    string strMsg = string.Format("{0}", linkdir);
                    SetInstallMsg(SetupConsts.STA_REMOVESHOTCUT, strMsg);
                    Directory.Delete(linkdir, true);
                    WriteLog("Shortcut", string.Format("RemoveShortcut end.\t{0}", linkdir));
                    SetInstallProgress(SetupConsts.PERCENTAGE_BASE_REMOVESHOTCUT + SetupConsts.PERCENTAGE_REMOVESHOTCUT);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void RemoveRegistry()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                bool is64BitOS = Environment.Is64BitOperatingSystem;
                string path;
                if (is64BitOS)
                {
                    path = string.Format(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0}", mUMPProduct.ProductGuid);
                }
                else
                {
                    path = string.Format(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0}", mUMPProduct.ProductGuid);
                }
                string strMsg = string.Format("{0}", path);
                SetInstallMsg(SetupConsts.STA_REMOVEREGISTRY, strMsg);
                RegistryKey rootKey = Registry.LocalMachine;
                rootKey.DeleteSubKey(path, false);
                WriteLog("RemoveReg", string.Format("Remove registry end.\t{0}", path));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_REMOVEREGISTRY + SetupConsts.PERCENTAGE_REMOVEREGISTRY);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void RemoveWebSite()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string appPoolName = SetupConsts.IIS_APPPOOL_NAME_UMP;
                string webSiteName = SetupConsts.IIS_SITE_NAME_UMP;
                string strMsg = string.Format("{0}", webSiteName);
                SetInstallMsg(SetupConsts.STA_REMOVEWEBSITE, strMsg);

                //如果应用程序池已经存在，则删除
                DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                foreach (DirectoryEntry appPool in appPools.Children)
                {
                    if (appPool.Name.Equals(appPoolName))
                    {
                        appPool.DeleteTree();
                        WriteLog("WebSite", string.Format("AppPool {0} removed.", appPoolName));
                    }
                }

                //如果站点已经存在，则删除
                DirectoryEntry rootEntry = new DirectoryEntry("IIS://localhost/W3SVC");
                foreach (DirectoryEntry site in rootEntry.Children)
                {
                    if (site.SchemaClassName.Equals("IIsWebServer"))
                    {
                        var alias = site.Properties["ServerComment"].Value;
                        if (alias != null
                            && alias.ToString().Equals(webSiteName))
                        {
                            rootEntry.Children.Remove(site);
                            WriteLog("WebSite", string.Format("Site {0} Removed.", webSiteName));
                        }
                    }
                }

                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_REMOVEWEBSITE + SetupConsts.PERCENTAGE_REMOVEWEBSITE);

                WriteLog("WebSite", string.Format("Remove UMP WebSite end."));
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void RemoveFiles()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string path = mUMPProduct.InstallPath;
                if (Directory.Exists(path))
                {
                    List<FileInfo> listFiles = new List<FileInfo>();
                    GetAllFiles(path, ref listFiles);
                    int count = listFiles.Count;
                    for (int i = 0; i < count; i++)
                    {
                        FileInfo temp = listFiles[i];
                        string name = temp.FullName;
                        try
                        {
                            string strMsg = string.Format("{0}", name);
                            SetInstallMsg(SetupConsts.STA_REMOVEFILES, strMsg);
                            File.Delete(name);
                        }
                        catch (Exception ex)
                        {
                            WriteLog("RemoveFiles", string.Format("Remove file fail.\t{0}\t{1}", name, ex.Message));
                            continue;
                        }
                        //WriteLog("RemoveFiles", string.Format("Remove File end.\t{0}", name));
                        double percentage = (i + 1) / (count * 1.0);
                        percentage = percentage * SetupConsts.PERCENTAGE_REMOVEFILES;
                        percentage = percentage + SetupConsts.PERCENTAGE_BASE_REMOVEFILES;
                        SetInstallProgress(percentage);
                    }
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch (Exception ex)
                    {
                        WriteLog("RemoveFiles", string.Format("Remove file fail.\t{0}\t{1}", path, ex.Message));
                    }
                }
                WriteLog("RemoveFiles", string.Format("Remove files end.\t{0}", path));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_REMOVEFILES + SetupConsts.PERCENTAGE_REMOVEFILES);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void GetAllFiles(string path, ref List<FileInfo> listFiles)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] listDirs = dir.GetDirectories();
            for (int i = 0; i < listDirs.Length; i++)
            {
                GetAllFiles(listDirs[i].FullName, ref listFiles);
            }
            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                listFiles.Add(files[i]);
            }
        }

        private void RemoveInstallDirectory()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (mUMPProduct == null) { return; }

                string tempPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        ConstValue.UMP_PRODUCTER_SHORTNAME, SetupConsts.APP_NAME);
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                string tempFile = Path.Combine(tempPath, "temp.bat");
                string dir = mUMPProduct.InstallPath;
                string parentDir = dir.Substring(0, dir.LastIndexOf("\\"));
                string volume = dir.Substring(0, 2);     //盘符
                string[] listCommands = new string[4];
                listCommands[0] = string.Format("{0}", volume);
                listCommands[1] = string.Format("cd \"{0}\"", parentDir);
                listCommands[2] = string.Format("ping /n 3 127.1>nul");     //暂停3s
                listCommands[3] = string.Format("rd /s /q \"{0}\"", dir);
                File.WriteAllLines(tempFile, listCommands);
                Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        #endregion


        #region Others

        private void SetInstallMsg(int state, string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                string strMsg = string.Empty;
                switch (state)
                {
                    case SetupConsts.STA_STOPSERVICE:
                        strMsg += SetupConsts.STA_NAME_STOPSERVICE;
                        break;
                    case SetupConsts.STA_REMOVESERVICE:
                        strMsg += SetupConsts.STA_NAME_REMOVESERVICE;
                        break;
                    case SetupConsts.STA_REMOVESHOTCUT:
                        strMsg += SetupConsts.STA_NAME_REMOVESHOTCUT;
                        break;
                    case SetupConsts.STA_REMOVEREGISTRY:
                        strMsg += SetupConsts.STA_NAME_REMOVEREGISTRY;
                        break;
                    case SetupConsts.STA_REMOVEFILES:
                        strMsg += SetupConsts.STA_NAME_REMOVEFILES;
                        break;
                }
                TxtUninstallMsg.ToolTip = msg;
                if (msg.Length > 55)
                {
                    string strPre = msg.Substring(0, 3);
                    string strLast = msg.Substring(msg.Length - 50);
                    msg = string.Format("{0}...{1}", strPre, strLast);
                }
                TxtUninstallMsg.Text = string.Format("{0} {1}", strMsg, msg);
            }));
        }

        private void SetInstallProgress(double value)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressUninstall.Value = value;
                TxtProgress.Text = string.Format("{0} %", value.ToString("0.00"));
            }));
        }

        private void ShowErrorMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, SetupConsts.APP_NAME, MessageBoxButton.OK, MessageBoxImage.Error)));
        }

        private void ShowInfoMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, SetupConsts.APP_NAME, MessageBoxButton.OK, MessageBoxImage.Information)));
        }

        #endregion


        #region EventHandlers

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mIsContinue)
                {
                    mIsContinue = false;
                    mIsOptFail = true;
                    WriteLog("CancelInstall", string.Format("Uninstallation canceled"));
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {
            UninstallUMP();
        }

        #endregion


        #region LogOperator

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("{0}\\{1}\\Logs", ConstValue.UMP_PRODUCTER_SHORTNAME, SetupConsts.APP_NAME));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("AppInfo\r\n");
                strInfo += string.Format("\tLogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog("AppLoad", strInfo);
            }
            catch { }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="msg">消息内容</param>
        public void WriteLog(string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="msg">消息类别</param>
        public void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, SetupConsts.APP_NAME, msg);
            }
        }

        #endregion

    }
}
