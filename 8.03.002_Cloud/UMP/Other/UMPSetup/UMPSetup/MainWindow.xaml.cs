using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using UMPSetup.Models;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPSetup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private ObservableCollection<ComponentItem> mListComponentItems;
        private List<ServiceInfo> mListServiceInfos;
        private List<ShotcutInfo> mListShotcutInfos;
        private InstallProduct mUMPProduct;
        private InstallInfo mInstallInfo;
        private BackgroundWorker mWorker;
        private LogOperator mLogOperator;
        private bool mIsOptFail;
        private bool mIsContinue;
        private string mErrorMsg;
        private string mInstallSource;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListComponentItems = new ObservableCollection<ComponentItem>();
            mListServiceInfos = new List<ServiceInfo>();
            mListShotcutInfos = new List<ShotcutInfo>();

            MouseLeftButtonDown += (s, me) => DragMove();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnMiniButton.Click += (s, me) => WindowState = WindowState.Minimized;
            BtnCloseButton.Click += (s, me) => Close();
            BtnBrowseDir.Click += BtnBrowseDir_Click;
            CbSelectAll.Click += CbSelectAll_Click;
            BtnInstall.Click += BtnInstall_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxComponents.ItemsSource = mListComponentItems;
            CreateLogOperator();
            InitComponentItems();
            InitServiceInfos();
            InitShotcutInfos();
            InitInstallInfo();
            Init();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mIsContinue)
            {
                string strMsg = string.Format("Confirm break installation?");
                var result = MessageBox.Show(strMsg, SetupConsts.APP_NAME, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                WriteLog("AppClose", string.Format("Installation break."));
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
                if (mUMPProduct != null)
                {
                    TxtInstallDir.Text = mUMPProduct.InstallPath;
                    TxtVersion.Text = mUMPProduct.Version;
                }
                else
                {
                    TxtInstallDir.Text = string.Empty;
                    TxtVersion.Text = string.Empty;
                }
                CbSelectAll.IsChecked = true;
                PanelPreInstall.Visibility = Visibility.Visible;
                PanelInstallProcess.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitInstallInfo()
        {
            try
            {
                mUMPProduct = new InstallProduct();
                mUMPProduct.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mUMPProduct.Version = "8.03.002";
                mUMPProduct.ProductGuid = UpdateConsts.PACKAGE_GUID_UMP;
                mUMPProduct.ProductName = ConstValue.UMP_PRODUCTER_SHORTNAME;
                mUMPProduct.DisplayName = ConstValue.UMP_PRODUCTER_SHORTNAME;
                mUMPProduct.InstallPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VoiceCyber\\UMP");

                string strPath = AppDomain.CurrentDomain.BaseDirectory;
                mInstallSource = Path.Combine(strPath, "UMPSetup.exe");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitComponentItems()
        {
            mListComponentItems.Clear();
            InstallComponent installComponent = new InstallComponent();
            installComponent.ModuleID = 0;
            installComponent.ModuleName = UpdateConsts.COMPONENT_NAME_UMP;
            ComponentItem item = new ComponentItem();
            item.Name = "Main Feature";
            item.IsChecked = true;
            item.IsEnabled = false;
            item.Info = installComponent;
            mListComponentItems.Add(item);
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

        private void InstallUMP()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtInstallDir.Text))
                {
                    ShowErrorMessage(string.Format("Install directory is empty"));
                    return;
                }
                if (mUMPProduct == null) { return; }
                var result = MessageBox.Show(string.Format("Confirm install UMP on this machine?"), SetupConsts.APP_NAME, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                string dir = TxtInstallDir.Text;
                string strMsg;
                if (Directory.Exists(dir))
                {
                    strMsg = string.Format("Install direcotry already exist. Do you want to replace exist files and install anyway?");
                    result = MessageBox.Show(strMsg, SetupConsts.APP_NAME, MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                mUMPProduct.InstallPath = dir;
                InstallComponent installComponent =
                    mUMPProduct.ListComponents.FirstOrDefault(c => c.ModuleName == UpdateConsts.COMPONENT_NAME_UMP);
                if (installComponent == null)
                {
                    installComponent = new InstallComponent();
                    mUMPProduct.ListComponents.Add(installComponent);
                    installComponent.ModuleID = 0;
                    installComponent.ModuleName = UpdateConsts.COMPONENT_NAME_UMP;
                }

                InstallInfo installInfo = new InstallInfo();
                installInfo.SessionID = Guid.NewGuid().ToString();
                installInfo.Type = (int)PackType.Product;
                installInfo.InstallTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.BeginTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.Version = mUMPProduct.Version;
                installInfo.MachineName = Environment.MachineName;
                installInfo.OSVersion = Environment.OSVersion.ToString();
                installInfo.OSAccount = Environment.UserName;
                installInfo.ListProducts.Add(mUMPProduct);
                mInstallInfo = installInfo;

                var resource = App.GetResourceStream(new Uri("Resources/UMPData.zip", UriKind.Relative));
                if (resource == null)
                {
                    ShowErrorMessage(string.Format("UMPData not exist"));
                    return;
                }
                PanelPreInstall.Visibility = Visibility.Collapsed;
                PanelInstallProcess.Visibility = Visibility.Visible;
                BtnInstall.IsEnabled = false;
                SetInstallProgress(0.0);
                WriteLog("Install", string.Format("Begin install.\t{0}", dir));
                mIsOptFail = false;
                mIsContinue = true;
                mErrorMsg = string.Empty;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    InstallDrivers();
                    Thread.Sleep(1000);
                    ExtractFiles(resource);
                    Thread.Sleep(1000);
                    InstallWebSite();
                    Thread.Sleep(1000);
                    WriteRegistryInfo();
                    Thread.Sleep(1000);
                    InstallWinServices();
                    Thread.Sleep(1000);
                    StartWinServices();
                    Thread.Sleep(1000);
                    CreateShortCuts();
                    Thread.Sleep(1000);
                    MoveUMPServerConfig();
                    Thread.Sleep(1000);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetInstallMsg(0, string.Empty);

                    mIsContinue = false;
                    if (mIsOptFail)
                    {
                        WriteLog("Install", string.Format("Install fail.\t{0}", mErrorMsg));
                        ShowErrorMessage(string.Format("Install fail.\t{0}", mErrorMsg));
                        PanelPreInstall.Visibility = Visibility.Visible;
                        PanelInstallProcess.Visibility = Visibility.Collapsed;
                        BtnInstall.IsEnabled = true;
                        return;
                    }
                    mInstallInfo.EndTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    WriteLog("Install", string.Format("Install successful"));
                    ShowInfoMessage(string.Format("Install successful"));
                    SaveInstallInfo();
                    LaunchMAMT();
                    Close();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InstallDrivers()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = currentDir;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "Resources");

                string driver = Path.Combine(path, string.Format("{0} x86.msi", SetupConsts.DRIVER_FILE_SCT));
                if (Environment.Is64BitOperatingSystem)
                {
                    driver = Path.Combine(path, string.Format("{0} x64.msi", SetupConsts.DRIVER_FILE_SCT));
                }
                InstallSingleDriver(SetupConsts.DRIVER_FILE_SCT, driver, true);

                driver = Path.Combine(path, string.Format("{0} x86.msi", SetupConsts.DRIVER_FILE_SMO));
                if (Environment.Is64BitOperatingSystem)
                {
                    driver = Path.Combine(path, string.Format("{0} x64.msi", SetupConsts.DRIVER_FILE_SMO));
                }
                InstallSingleDriver(SetupConsts.DRIVER_FILE_SMO, driver, true);

                driver = Path.Combine(path, string.Format("{0} x86.msi", SetupConsts.DRIVER_FILE_SNC));
                if (Environment.Is64BitOperatingSystem)
                {
                    driver = Path.Combine(path, string.Format("{0} x64.msi", SetupConsts.DRIVER_FILE_SNC));
                }
                InstallSingleDriver(SetupConsts.DRIVER_FILE_SNC, driver, true);

                InstallODACDriver(path);

                WriteLog("InstallDrivers", string.Format("All drivers install end."));
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallSingleDriver(string name, string driver, bool isBackground)
        {
            if (mIsOptFail) { return; }
            if (!mIsContinue) { return; }
            if (mUMPProduct == null) { return; }

            if (!File.Exists(driver))
            {
                WriteLog("InstallDrivers", string.Format("File not exist.\t{0}\t{1}", name, driver));
                return;
            }
            string strMsg = string.Format("{0}", name);
            SetInstallMsg(SetupConsts.STA_INSTALLDRIVER, strMsg);
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = driver;
                if (isBackground)
                {
                    process.StartInfo.Arguments = "/qn";
                }
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                bool result = process.WaitForExit(60 * 1000);
                if (!result)
                {
                    int code = process.ExitCode;
                    WriteLog("InstallDrivers", string.Format("Install fail.\t{0}\t{1}", name, code));
                    return;
                }
                WriteLog("InstallDrivers", string.Format("Install end.\t{0}", name));
            }
            catch (Exception ex)
            {
                WriteLog("InstallDrivers", string.Format("Install fail.\t{0}\t{1}", name, ex.Message));
            }
        }

        private void InstallODACDriver(string odacDir)
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string path = Path.Combine(odacDir, string.Format("ODAC{0}", "x86"));
                if (Environment.Is64BitOperatingSystem)
                {
                    path = Path.Combine(odacDir, string.Format("ODAC{0}", "x64"));
                }
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string volume = dir.Substring(0, 2);
                string installPath = string.Format("{0}\\UMPODAC", volume);
                string oracleName = "ODAC";
                string tempDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.UMP_PRODUCTER_SHORTNAME, SetupConsts.APP_NAME);
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                string tempFile = Path.Combine(tempDir, "temp.bat");
                string[] commands = new string[5];
                commands[0] = string.Format("cd \"{0}\"", path);
                commands[1] = string.Format("install.bat odp.net4 {0} {1} true", installPath, oracleName);
                commands[2] = string.Format(volume);
                commands[3] = string.Format("cd \"{0}\"", installPath);
                commands[4] = string.Format("configure.bat odp.net4 {0} true", oracleName);
                File.WriteAllLines(tempFile, commands);

                string strMsg = string.Format("{0}", SetupConsts.DRIVER_FILE_ODAC);
                SetInstallMsg(SetupConsts.STA_INSTALLDRIVER, strMsg);

                Process process = new Process();
                process.StartInfo.FileName = tempFile;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                bool result = process.WaitForExit(60 * 1000);
                if (!result)
                {
                    int code = process.ExitCode;
                    WriteLog("InstallDrivers", string.Format("Install ODAC fail.\t{0}", code));
                    return;
                }
                process.Dispose();
                WriteLog("InstallDrivers", string.Format("Install ODAC end.\t{0}\t{1}", oracleName, installPath));
            }
            catch (Exception ex)
            {
                WriteLog("InstallDrivers", string.Format("Install ODAC driver fail.\t{0}", ex.Message));
            }
        }

        private void ExtractFiles(StreamResourceInfo resource)
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                var stream = resource.Stream as UnmanagedMemoryStream;
                if (stream == null) { return; }
                string targetDir = mUMPProduct.InstallPath;
                if (!Directory.Exists(targetDir)) { return; }
                using (var zipStream = new ZipInputStream(stream))
                {
                    long totalSize = stream.Length;
                    long zipSize = 0;
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        string dirName = targetDir;
                        string pathToZip = theEntry.Name;
                        if (!string.IsNullOrEmpty(pathToZip))
                        {
                            dirName = Path.GetDirectoryName(Path.Combine(dirName, pathToZip));
                        }
                        DateTime datetime = theEntry.DateTime;
                        if (string.IsNullOrEmpty(dirName)) { continue; }
                        string fileName = Path.GetFileName(pathToZip);
                        Directory.CreateDirectory(dirName);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string filePath = Path.Combine(dirName, fileName);
                            string strMsg = filePath;
                            SetInstallMsg(SetupConsts.STA_EXTRACT, strMsg);
                            //WriteLog("Extract", string.Format("Extracting... {0}", filePath));
                            using (FileStream streamWriter = File.Create(filePath))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipStream.Read(data, 0, data.Length);
                                    zipSize = zipStream.Position;
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }

                            //还原文件的修改时间
                            FileInfo fileInfo = new FileInfo(filePath);
                            fileInfo.LastWriteTime = datetime;

                            //WriteLog("Extract", string.Format("Extracted {0}", filePath));
                        }
                        double percentage = zipSize / (totalSize * 1.0);
                        percentage = percentage * SetupConsts.PERCENTAGE_EXTRACT;
                        percentage = percentage + SetupConsts.PERCENTAGE_BASE_EXTRACT;
                        SetInstallProgress(percentage);
                    }
                    SetInstallProgress(SetupConsts.PERCENTAGE_BASE_EXTRACT + SetupConsts.PERCENTAGE_EXTRACT);
                    WriteLog("Extract", string.Format("Extract file end.\t{0}", totalSize));
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallWebSite()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string appPoolName = SetupConsts.IIS_APPPOOL_NAME_UMP;
                string webSiteName = SetupConsts.IIS_SITE_NAME_UMP;
                string wcfServiceAppName = SetupConsts.IIS_APP_PATH_WCFSERVICE;
                string wcf2ClientAppName = SetupConsts.IIS_APP_PATH_WCF2CLIENT;
                string wcf1600AppName = SetupConsts.IIS_APP_PATH_WCF1600;
                int intPort = 8081;

                string siteDirectory = mUMPProduct.InstallPath;

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

                //创建应用程序池
                DirectoryEntry umpAppPool = appPools.Children.Add(appPoolName, "IIsApplicationPool");
                umpAppPool.CommitChanges();
                WriteLog("WebSite", string.Format("AppPool {0} created.", appPoolName));

                //配置应用程序池
                ServerManager sm = new ServerManager();
                sm.ApplicationPools[appPoolName].ManagedRuntimeVersion = "v4.0";
                sm.ApplicationPools[appPoolName].AutoStart = true;
                sm.ApplicationPools[appPoolName].ManagedPipelineMode = ManagedPipelineMode.Classic;
                sm.CommitChanges();
                WriteLog("WebSite", string.Format("Config AppPool {0} end.", appPoolName));

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

                //创建站点
                int siteID = GetSiteID();
                rootEntry = new DirectoryEntry("IIS://localhost/W3SVC");
                DirectoryEntry umpSite = rootEntry.Children.Add(siteID.ToString(), "IIsWebServer");
                umpSite.Properties["ServerComment"].Value = webSiteName;
                umpSite.Properties["AppPoolId"].Value = appPoolName;
                umpSite.Properties["ServerBindings"].Value = string.Format("*:{0}:", intPort);
                umpSite.CommitChanges();
                WriteLog("WebSite", string.Format("WebSite {0} created.", webSiteName));

                //创建虚拟目录
                DirectoryEntry rootVirtualDir = umpSite.Children.Add("ROOT", "IIsWebVirtualDir");
                rootVirtualDir.Properties["Path"].Value = siteDirectory;
                rootVirtualDir.CommitChanges();
                WriteLog("WebSite", string.Format("VirtalDir Root created.\t{0}", siteDirectory));

                //创建子应用程序
                sm = new ServerManager();
                var umpSiteTemp = sm.Sites[webSiteName];
                string appDir = Path.Combine(siteDirectory, wcfServiceAppName);
                umpSiteTemp.Applications.Add(string.Format("/{0}", wcfServiceAppName), appDir);
                appDir = Path.Combine(siteDirectory, wcf2ClientAppName);
                umpSiteTemp.Applications.Add(string.Format("/{0}", wcf2ClientAppName), appDir);
                appDir = Path.Combine(siteDirectory, wcf1600AppName);
                umpSiteTemp.Applications.Add(string.Format("/{0}", wcf1600AppName), appDir);
                sm.CommitChanges();
                WriteLog("WebSite", string.Format("Application created.\t{0}", webSiteName));

                //启用AspNet4.0 Web扩展
                string webExtName = "ASP.NET v4.0.30319";
                rootEntry = new DirectoryEntry("IIS://localhost/W3SVC");
                rootEntry.Invoke("EnableWebServiceExtension", webExtName);
                rootEntry.CommitChanges();
                WriteLog("WebSite", string.Format("EnableWebServiceExtension end.\t{0}", webExtName));

            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void WriteRegistryInfo()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                SetInstallMsg(SetupConsts.STA_WRITEREG, string.Format("Write registry information"));
                RegistryKey rootKey = Registry.LocalMachine;
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
                WriteLog("WriteReg", string.Format("Begin write registry info into \"{0}\"", path));
                RegistryKey umpKey = rootKey.CreateSubKey(path);
                if (umpKey != null)
                {
                    umpKey.SetValue("Name", mUMPProduct.ProductName);
                    umpKey.SetValue("DisplayName", mUMPProduct.DisplayName);
                    umpKey.SetValue("ProductGuid", mUMPProduct.ProductGuid);
                    umpKey.SetValue("Publisher", ConstValue.VCT_COMPANY_LONGNAME);
                    umpKey.SetValue("DisplayVersion", mUMPProduct.Version);
                    umpKey.SetValue("InstallLocation", mUMPProduct.InstallPath);
                    umpKey.SetValue("InstallDate", mInstallInfo.InstallTime);
                    umpKey.SetValue("UninstallString", Path.Combine(mUMPProduct.InstallPath, "UMPUninstall.exe"));
                    umpKey.SetValue("InstallSource", mInstallSource);

                    WriteLog("WriteReg", string.Format("Write registry end"));
                    SetInstallProgress(SetupConsts.PERCENTAGE_BASE_WRITEREG + SetupConsts.PERCENTAGE_WRITEREG);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallWinServices()
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
                        WriteLog("InstallService", string.Format("File not exist.\t{0}", file));
                        continue;
                    }
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == info.ServiceName);
                    if (controller == null)
                    {
                        string strMsg = string.Format("{0}", info.Name);
                        SetInstallMsg(SetupConsts.STA_INSTALLSERVICE, strMsg);
                        string[] options = { };
                        try
                        {
                            TransactedInstaller transacted = new TransactedInstaller();
                            AssemblyInstaller installer = new AssemblyInstaller(file, options);
                            transacted.Installers.Add(installer);
                            transacted.Install(new Hashtable());
                        }
                        catch (Exception ex)
                        {
                            WriteLog("InstallService", string.Format("Install service fail.\t{0}\t{1}", strName, ex.Message));
                            continue;
                        }
                        WriteLog("InstallService", string.Format("Install service end.\t{0}", strName));
                        double percentage = (i + 1) / (count * 1.0);
                        percentage = percentage * SetupConsts.PERCENTAGE_INSTALLSERVICE;
                        percentage = percentage + SetupConsts.PERCENTAGE_BASE_INSTALLSERVICE;
                        SetInstallProgress(percentage);
                    }
                }
                WriteLog("InstallService", string.Format("All service installed.\t{0}", count));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_INSTALLSERVICE + SetupConsts.PERCENTAGE_INSTALLSERVICE);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void StartWinServices()
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
                        WriteLog("StartService", string.Format("File not exist.\t{0}", file));
                        continue;
                    }
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == info.ServiceName);
                    if (controller == null)
                    {
                        WriteLog("StartService", string.Format("WinService not exist.\t{0}", strName));
                        continue;
                    }
                    try
                    {
                        string strMsg = string.Format("{0}", info.Name);
                        SetInstallMsg(SetupConsts.STA_STARTSERVICE, strMsg);
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 60));
                    }
                    catch (Exception ex)
                    {
                        WriteLog("StartService", string.Format("Start service fail.\t{0}\t{1}", strName, ex.Message));
                        continue;
                    }
                    WriteLog("StartService", string.Format("Start service end.\t{0}", strName));
                    double percentage = (i + 1) / (count * 1.0);
                    percentage = percentage * SetupConsts.PERCENTAGE_STARTSERVICE;
                    percentage = percentage + SetupConsts.PERCENTAGE_BASE_STARTSERVICE;
                    SetInstallProgress(percentage);
                }
                WriteLog("StartService", string.Format("All service Started.\t{0}", count));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_STARTSERVICE + SetupConsts.PERCENTAGE_STARTSERVICE);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void CreateShortCuts()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                int count = mListShotcutInfos.Count;
                for (int i = 0; i < count; i++)
                {
                    var info = mListShotcutInfos[i];
                    string linkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
                       ConstValue.VCT_COMPANY_LONGNAME, ConstValue.UMP_PRODUCTER_SHORTNAME);
                    Directory.CreateDirectory(linkDir);
                    string linkPath = Path.Combine(linkDir, info.LinkPath);
                    string targetDir = mUMPProduct.InstallPath;
                    string targetPath = Path.Combine(targetDir, info.TargetPath);
                    targetDir = targetPath.Substring(0, targetPath.LastIndexOf("\\"));

                    string vbs = string.Empty;
                    vbs += string.Format("Set WshShell = WScript.CreateObject(\"WScript.Shell\")\r\n");
                    vbs += string.Format("set oShellLink = WshShell.CreateShortcut(\"{0}\")\r\n", linkPath);
                    vbs += string.Format("Dim fso\r\n");
                    vbs += string.Format("Set fso=CreateObject(\"Scripting.FileSystemObject\")\r\n");
                    vbs += string.Format("oShellLink.TargetPath = \"{0}\"\r\n", targetPath);
                    vbs += string.Format("oShellLink.WorkingDirectory = \"{0}\"\r\n", targetDir);
                    vbs += string.Format("oShellLink.WindowStyle = 1\r\n");
                    vbs += string.Format("oShellLink.IconLocation = \"{0},0\"\r\n", targetPath);
                    vbs += string.Format("oShellLink.Description = \"{0}\"\r\n", info.Description);
                    vbs += string.Format("oShellLink.Save");

                    string tempDir =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            ConstValue.UMP_PRODUCTER_SHORTNAME, SetupConsts.APP_NAME);
                    try
                    {
                        string strMsg = string.Format("{0}", info.Name);
                        SetInstallMsg(SetupConsts.STA_INSTALLSHOTCUT, strMsg);
                        if (!Directory.Exists(tempDir))
                        {
                            Directory.CreateDirectory(tempDir);
                        }
                        string vbsFile = Path.Combine(tempDir, string.Format("Temp{0}.vbs", i));
                        File.WriteAllText(vbsFile, vbs, Encoding.Unicode);
                        Process.Start(vbsFile);
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Shortcut", string.Format("CreateShortcut fail.\t{0}\t{1}", info.Name, ex.Message));
                        continue;
                    }
                    WriteLog("Shortcut", string.Format("CreateShortcut end.\t{0}", info.Name));
                    double percentage = (i + 1) / (count * 1.0);
                    percentage = percentage * SetupConsts.PERCENTAGE_CREATESHOT;
                    percentage = percentage + SetupConsts.PERCENTAGE_BASE_CREATESHOT;
                    SetInstallProgress(percentage);
                }
                WriteLog("Shortcut", string.Format("All Shortcut created.\t{0}", count));
                SetInstallProgress(SetupConsts.PERCENTAGE_BASE_CREATESHOT + SetupConsts.PERCENTAGE_CREATESHOT);
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void MoveUMPServerConfig()
        {
            try
            {
                if (mIsOptFail) { return; }
                if (!mIsContinue) { return; }
                if (mUMPProduct == null) { return; }

                string installDir = mUMPProduct.InstallPath;
                string dir = Path.Combine(installDir, "UMP.Server");
                if (!Directory.Exists(dir))
                {
                    WriteLog("MoveConfigFile", string.Format("Directory not exist.\t{0}", dir));
                    return;
                }
                string dest = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                dest = Path.Combine(dest, "UMP.Server");
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                FileInfo[] files = dirInfo.GetFiles();
                int count = files.Length;
                for (int i = 0; i < count; i++)
                {
                    FileInfo file = files[i];
                    File.Copy(file.FullName, Path.Combine(dest, file.Name), true);
                }

                WriteLog("MoveConfigFile", string.Format("MoveConfigFile end.\t{0}", count));
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void LaunchMAMT()
        {
            try
            {
                if (mUMPProduct == null) { return; }
                string path = mUMPProduct.InstallPath;
                path = Path.Combine(path, "ManagementMaintenance");
                string dir = path;
                path = Path.Combine(path, "UMP.MAMT.exe");
                if (!File.Exists(path))
                {
                    WriteLog("LaunchMAMT", string.Format("File not exist."));
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = dir;
                process.Start();
            }
            catch (Exception ex)
            {
                WriteLog("LaunchMAMT", string.Format("Launch MAMT fail.\t{0}", ex.Message));
            }
        }

        private void SaveInstallInfo()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "VoiceCyber\\UMP");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, InstallInfo.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mInstallInfo, path);
                if (!optReturn.Result)
                {
                    WriteLog("SaveInstallInfo", string.Format("Save InstallInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WriteLog("SaveInstallInfo", string.Format("Save InstallInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                WriteLog("SaveInstallInfo", string.Format("Save InstallInfo fail.\t{0}", ex.Message));
            }
        }

        private int GetSiteID()
        {
            int result = 1;
            DirectoryEntry serviceEntry = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry entry in serviceEntry.Children)
            {
                if (entry.SchemaClassName == "IIsWebServer")
                {
                    int siteID;
                    try
                    {
                        siteID = int.Parse(entry.Name);
                    }
                    catch { continue; }
                    if (siteID == result)
                    {
                        result++;
                    }
                }
            }
            return result;

        }

        #endregion


        #region EventHandlers

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mIsContinue)
                {
                    var result = MessageBox.Show(string.Format("Confirm cancel install?"), SetupConsts.APP_NAME,
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (result != MessageBoxResult.Yes) { return; }

                    mIsContinue = false;
                    mIsOptFail = true;
                    WriteLog("CancelInstall", string.Format("Installation canceled"));
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

        void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            InstallUMP();
        }

        void CbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbSelectAll.IsChecked == true)
                {
                    for (int i = 0; i < mListComponentItems.Count; i++)
                    {
                        mListComponentItems[i].IsChecked = true;
                    }
                }
                if (CbSelectAll.IsChecked == false)
                {
                    for (int i = 0; i < mListComponentItems.Count; i++)
                    {
                        var item = mListComponentItems[i];
                        if (item.IsEnabled)
                        {
                            item.IsChecked = false;
                        }
                    }
                }
                if (CbSelectAll.IsChecked == null)
                {
                    CbSelectAll.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnBrowseDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (!string.IsNullOrEmpty(TxtInstallDir.Text))
                {
                    dialog.SelectedPath = TxtInstallDir.Text;
                }
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TxtInstallDir.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
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
                    case SetupConsts.STA_EXTRACT:
                        strMsg += SetupConsts.STA_NAME_EXTRACT;
                        break;
                    case SetupConsts.STA_WRITEREG:
                        strMsg += SetupConsts.STA_NAME_WRITEREG;
                        break;
                    case SetupConsts.STA_INSTALLSERVICE:
                        strMsg += SetupConsts.STA_NAME_INSTALLSERVICE;
                        break;
                    case SetupConsts.STA_STARTSERVICE:
                        strMsg += SetupConsts.STA_NAME_STARTSERVICE;
                        break;
                    case SetupConsts.STA_INSTALLSHOTCUT:
                        strMsg += SetupConsts.STA_NAME_INSTALLSHOTCUT;
                        break;
                    case SetupConsts.STA_INSTALLDRIVER:
                        strMsg += SetupConsts.STA_NAME_INSTALLDRIVER;
                        break;
                    case SetupConsts.STA_INSTALLWEBSITE:
                        strMsg += SetupConsts.STA_NAME_INSTALLWEBSITE;
                        break;
                }
                TxtInstallMsg.ToolTip = msg;
                if (msg.Length > 55)
                {
                    string strPre = msg.Substring(0, 3);
                    string strLast = msg.Substring(msg.Length - 50);
                    msg = string.Format("{0}...{1}", strPre, strLast);
                }
                TxtInstallMsg.Text = string.Format("{0} {1}", strMsg, msg);
            }));
        }

        private void SetInstallProgress(double value)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressInstall.Value = value;
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
