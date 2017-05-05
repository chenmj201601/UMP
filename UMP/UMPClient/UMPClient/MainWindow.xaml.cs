using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using Microsoft.Win32;
using UMPClient.Models;
using UMPClient.Wcf00000;
using VoiceCyber.Common;

namespace UMPClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private const string CERT_PASSWORD = "VoiceCyber,123";
        private const string CERT_HASH = "2D508A175B6836ADB6E220BA63E00F2F0881E75F";

        private bool mIsInited;
        private UMPSettingInfo mSettingInfo;
        private UMPServerInfo mCurrentServer;
        private ObservableCollection<ComboItem> mListUMPServerItems;
        private bool mIsWorking;
        private bool mIsFail;
        private string mErrorMsg;
        private bool mIsInstalled;
        private string mAppShortName;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListUMPServerItems = new ObservableCollection<ComboItem>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            MouseLeftButtonDown += (s, e) => DragMove();

            BtnAppMenu.Click += BtnAppMenu_Click;
            BtnAppMinimize.Click += BtnAppMinimize_Click;
            BtnAppClose.Click += BtnAppClose_Click;

            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            ComboServers.SelectionChanged += ComboServers_SelectionChanged;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mIsInstalled) { return; }
            var result = MessageBox.Show(App.GetLanguageInfo("N007", string.Format("Confirm close window?")), App.AppName, MessageBoxButton.YesNo,
              MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ComboServers.ItemsSource = mListUMPServerItems;

                WindowState = WindowState.Normal;
                BorderMain.Padding = new Thickness(0);
                BorderCheckPanel.Visibility = Visibility.Visible;
                BorderMainPanel.Visibility = Visibility.Collapsed;

                SetCheckBusy(true, string.Format("Checking environment on this machine, please wait for a moment..."));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUMPSettingInfos();
                    App.LoadAllLanguages();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetCheckBusy(false, string.Empty);

                    BorderCheckPanel.Visibility = Visibility.Collapsed;
                    BorderMainPanel.Visibility = Visibility.Visible;

                    InitAppMenuButtons();

                    LoadUMPServerItems();
                    var item = mListUMPServerItems.FirstOrDefault(ser => ser.IntValue == 1);
                    if (item == null)
                    {
                        item = mListUMPServerItems.FirstOrDefault();
                    }
                    ComboServers.SelectedItem = item;

                    InitSettings();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUMPSettingInfos()
        {
            try
            {
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                string strFile = Path.Combine(strPath, UMPSettingInfo.FILE_NAME);
                if (!File.Exists(strFile))
                {
                    App.WriteLog("LoadSetting", string.Format("UMPSetting file not exist.\t{0}", strFile));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<UMPSettingInfo>(strFile);
                if (!optReturn.Result)
                {
                    App.WriteLog("LoadSetting", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                UMPSettingInfo setting = optReturn.Data as UMPSettingInfo;
                if (setting == null)
                {
                    App.WriteLog("LoadSetting", string.Format("UMPSetting is null."));
                    return;
                }
                mSettingInfo = setting;
                App.LangID = mSettingInfo.LangID;
                App.WriteLog("LoadSetting", string.Format("End"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUMPServerItems()
        {
            try
            {
                mListUMPServerItems.Clear();
                if (mSettingInfo == null) { return; }
                for (int i = 0; i < mSettingInfo.ListServerInfos.Count; i++)
                {
                    var info = mSettingInfo.ListServerInfos[i];
                    ComboItem item = new ComboItem();
                    item.Data = info;
                    item.Name = info.Address;
                    item.Display = info.Address;
                    item.IntValue = info.IsDefault ? 1 : 0;
                    mListUMPServerItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAppMenuButtons()
        {
            try
            {
                ContextMenu menu = new ContextMenu();
                menu.Opacity = 0.8;

                for (int i = 0; i < App.ListLangTypeInfos.Count; i++)
                {
                    var langTypeInfo = App.ListLangTypeInfos[i];
                    int langID = langTypeInfo.LangID;
                    string strLangName = langTypeInfo.LangName;
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += LangMenuItem_Click;
                    menuItem.Header = App.GetLanguageInfo(string.Format("Lang{0}", langID), strLangName);
                    menuItem.Tag = langID;
                    menuItem.IsChecked = langID == App.LangID;
                    menu.Items.Add(menuItem);
                }

                BtnAppMenu.ContextMenu = menu;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSettings()
        {
            try
            {
                if (mSettingInfo != null)
                {
                    CbLaunchUMP.IsChecked = mSettingInfo.LaunchUMP == 1;
                    CbLaunchAgent.IsChecked = mSettingInfo.LaunchAgent == 1;
                }
                var server = ComboServers.SelectedItem as ComboItem;
                if (server == null)
                {
                    TxtServerPort.Text = "8081";
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void InstallComponents()
        {
            try
            {

                #region 验证

                if (mSettingInfo == null)
                {
                    mSettingInfo = new UMPSettingInfo();
                }
                mSettingInfo.LangID = App.LangID;
                mSettingInfo.StyleName = "Style01";
                int intPort;
                string strPort = TxtServerPort.Text;
                if (!int.TryParse(strPort, out intPort))
                {
                    ShowException(App.GetLanguageInfo("N002", string.Format("Server port invalid!")));
                    return;
                }
                string strServerAddress = ComboServers.Text;
                if (string.IsNullOrEmpty(strServerAddress))
                {
                    ShowException(App.GetLanguageInfo("N001", string.Format("Server address is empty!")));
                    return;
                }
                UMPServerInfo currentServer = mSettingInfo.ListServerInfos.FirstOrDefault(s => s.Address == strServerAddress);
                if (currentServer == null)
                {
                    currentServer = new UMPServerInfo();
                    currentServer.Address = strServerAddress;
                    mSettingInfo.ListServerInfos.Add(currentServer);
                    ComboItem item = new ComboItem();
                    item.Data = currentServer;
                    item.Name = currentServer.Address;
                    item.Display = currentServer.Address;
                    item.IntValue = currentServer.Port;
                    mListUMPServerItems.Add(item);
                }
                currentServer.Port = intPort;
                currentServer.IsDefault = true;
                mCurrentServer = currentServer;

                mSettingInfo.LaunchUMP = CbLaunchUMP.IsChecked == true ? 1 : 0;
                mSettingInfo.LaunchAgent = CbLaunchAgent.IsChecked == true ? 1 : 0;

                App.WriteLog("InstallComponent",
                    string.Format("Begin install component.\tServer:{0};Port:{1}", strServerAddress, intPort));

                #endregion


                mIsWorking = true;
                mIsFail = false;
                mErrorMsg = string.Empty;
                SetBusy(true, string.Format("Begin install components..."));
                SetButtonEnable(mIsWorking);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    GetAppShortName();
                    Thread.Sleep(500);
                    DownloadUMPServer01();
                    Thread.Sleep(500);
                    DownloadAppLogo();
                    Thread.Sleep(500);
                    DownloadCertificates();
                    Thread.Sleep(500);
                    DownloadReportViewer();
                    Thread.Sleep(500);

                    InstallCertificates();
                    Thread.Sleep(500);
                    InstallCLRTypeComponent();
                    Thread.Sleep(500);
                    InstallReportViewerComponent();
                    Thread.Sleep(500);
                    SetTrustWebSite();
                    Thread.Sleep(500);
                    SetAuthRootCertUpdatePolicy();
                    Thread.Sleep(500);
                    CreateShortcuts();
                    Thread.Sleep(500);
                    SaveSettingInfos();
                    Thread.Sleep(500);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);
                    mIsWorking = false;
                    SetButtonEnable(mIsWorking);

                    if (mIsFail)
                    {
                        App.WriteLog("InstallComponent", string.Format("Install fail.\t{0}", mErrorMsg));
                        ShowException(App.GetLanguageInfo("N005", "Install fail!"));
                        return;
                    }
                    mIsInstalled = true;
                    App.WriteLog("InstallComponent", string.Format("Install successful"));
                    ShowInformation(App.GetLanguageInfo("N006", string.Format("Install components successful!")));
                    if (mSettingInfo != null)
                    {
                        if (mIsInstalled)
                        {
                            if (mSettingInfo.LaunchUMP == 1
                                || mSettingInfo.LaunchAgent == 1)
                            {
                                if (mSettingInfo.LaunchUMP == 1)
                                {
                                    LaunchUMP();
                                }
                                if (mSettingInfo.LaunchAgent == 1)
                                {
                                    LaunchAgent();
                                }
                                Close();
                            }
                        }
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetAppShortName()
        {
            try
            {
                if (mIsFail) { return; }

                SetBusy(true, App.GetLanguageInfo("S001", string.Format("Getting application short name...")));
                List<string> listArgs = new List<string>();
                Service00000Client client = new Service00000Client(CreateBinding(),
                    CreateEndpointAddress(mCurrentServer, "Service00000"));
                OperationDataArgs optArgs = client.OperationMethodA(1, listArgs);
                client.Close();
                if (!optArgs.BoolReturn)
                {
                    mIsFail = true;
                    mErrorMsg = string.Format("{0}", optArgs.StringReturn);
                    return;
                }
                mAppShortName = optArgs.StringReturn;
                App.WriteLog("GetAppShortName", string.Format("AppShortName:{0}", mAppShortName));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void DownloadAppLogo()
        {
            try
            {
                if (mIsFail) { return; }

                SetBusy(true, App.GetLanguageInfo("S002", string.Format("Downloading application logo...")));
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, "UMPAppLogo.ico");
                DownloadConfig config = new DownloadConfig();
                config.Method = 1;
                config.Host = mCurrentServer.Address;
                config.Port = mCurrentServer.Port;
                config.RequestPath = string.Format("Logo\\logo4.ico");
                config.SavePath = strFile;
                OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    mIsFail = true;
                    mErrorMsg = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Exception);
                    return;
                }
                App.WriteLog("DownloadAppLogo", string.Format("End.\t{0}", strFile));

                //复制到系统目录
                string sys = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "UMPAppLogo.ico");
                File.Copy(strFile, sys, true);
                App.WriteLog("DownloadAppLogo", string.Format("Copy to system directory end.\t{0}", sys));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void DownloadCertificates()
        {
            try
            {
                if (mIsFail) { return; }

                string strName = "UMP.PF.Certificate";
                SetBusy(true, string.Format(App.GetLanguageInfo("S006", string.Format("Downloading certificate...\t{0}", strName)), strName));
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, string.Format("{0}.pfx", strName));
                string strRequest = string.Format("Components\\Certificates\\{0}.pfx", strName);
                DownloadConfig config = new DownloadConfig();
                config.Method = 1;
                config.Host = mCurrentServer.Address;
                config.Port = mCurrentServer.Port;
                config.RequestPath = strRequest;
                config.SavePath = strFile;
                OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    //mIsFail = true;
                    //mErrorMsg = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Exception);
                    //return;
                    App.WriteLog("DownloadUMPCert", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
                App.WriteLog("DownloadUMPCert", string.Format("End.\t{0}", strFile));

                strName = string.Format("UMP.S.{0}", mCurrentServer.Address);
                SetBusy(true, string.Format(App.GetLanguageInfo("S006", string.Format("Downloading certificate...\t{0}", strName)), strName));
                strFile = Path.Combine(strPath, string.Format("{0}.pfx", strName));
                strRequest = string.Format("Components\\Certificates\\{0}.pfx", strName);
                config = new DownloadConfig();
                config.Method = 1;
                config.Host = mCurrentServer.Address;
                config.Port = mCurrentServer.Port;
                config.RequestPath = strRequest;
                config.SavePath = strFile;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    //mIsFail = true;
                    //mErrorMsg = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Exception);
                    App.WriteLog("DownloadServerCert", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
                App.WriteLog("DownloadServerCert", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void DownloadReportViewer()
        {
            try
            {
                if (mIsFail) { return; }

                List<string> listItems = new List<string>();
                listItems.Add("ReportViewer0000");
                listItems.Add("ReportViewer0001");
                listItems.Add("ReportViewer1033");
                listItems.Add("ReportViewer1028");
                listItems.Add("ReportViewer1041");
                listItems.Add("ReportViewer2052");

                for (int i = 0; i < listItems.Count; i++)
                {
                    string strName = listItems[i];
                    SetBusy(true, string.Format(App.GetLanguageInfo("S008", string.Format("Downloading ReportViewer...\t{0}", strName)), strName));
                    string strPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }
                    string strFile = Path.Combine(strPath, string.Format("{0}.msi", strName));
                    string strRequest = string.Format("Components\\ReportViewer\\{0}.msi", strName);
                    DownloadConfig config = new DownloadConfig();
                    config.Method = 1;
                    config.Host = mCurrentServer.Address;
                    config.Port = mCurrentServer.Port;
                    config.RequestPath = strRequest;
                    config.SavePath = strFile;
                    OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                    if (!optReturn.Result)
                    {
                        mIsFail = true;
                        mErrorMsg = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Exception);
                        return;
                    }
                    App.WriteLog("DownloadReport", string.Format("End.\t{0}", strFile));
                }
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void DownloadUMPServer01()
        {
            try
            {
                if (mIsFail) { return; }

                string strName = "UMP.Server.01";
                SetBusy(true, App.GetLanguageInfo("S004", string.Format("Downloading UMP.Server.01 ...\t{0}", strName)));
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, string.Format("{0}.xml", strName));
                string strRequest = string.Format("GlobalSettings\\{0}", string.Format("{0}.xml", strName));
                DownloadConfig config = new DownloadConfig();
                config.Method = 1;
                config.Host = mCurrentServer.Address;
                config.Port = mCurrentServer.Port;
                config.RequestPath = strRequest;
                config.SavePath = strFile;
                OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    mIsFail = true;
                    mErrorMsg = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Exception);
                    return;
                }
                App.WriteLog("DownloadUMPServer01", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallCertificates()
        {
            try
            {
                if (mIsFail) { return; }

                InstallUMPCertificate();
                InstallServerCertificate();
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallUMPCertificate()
        {
            try
            {
                if (mIsFail) { return; }

                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                string strCertName = "UMP.PF.Certificate";
                SetBusy(true, string.Format(App.GetLanguageInfo("S051", string.Format("Installing UMP Certificate ...\t{0}", strCertName)), strCertName));
                string strFile = Path.Combine(strPath, string.Format("{0}.pfx", strCertName));
                if (!File.Exists(strFile))
                {
                    App.WriteLog("InstallUMPCert", string.Format("File not exist.\t{0}", strFile));
                    return;
                }
                string strHash = CERT_HASH;
                byte[] certData = File.ReadAllBytes(strFile);
                if (!IsCertificateExist(strHash, StoreName.AuthRoot, StoreLocation.LocalMachine))
                {
                    InstallCertificateToStore(StoreName.AuthRoot, StoreLocation.LocalMachine, certData,
                        CERT_PASSWORD);
                }
                App.WriteLog("InstallUMPCert", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                App.WriteLog("InstallUMPCert", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void InstallServerCertificate()
        {
            try
            {
                if (mIsFail) { return; }

                string strPath = Path.Combine(
                   Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                string strCertName = string.Format("UMP.S.{0}", mCurrentServer.Address);
                SetBusy(true, string.Format(App.GetLanguageInfo("S051", string.Format("Installing Server Certificate ...\t{0}", strCertName)), strCertName));
                string strXml = Path.Combine(strPath, "UMP.Server.01.xml");
                if (!File.Exists(strXml))
                {
                    App.WriteLog("InstallServerCert", string.Format("File not exist.\t{0}", strXml));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(strXml);
                var protocolNode = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (protocolNode == null)
                {
                    App.WriteLog("InstallServerCert", string.Format("ProtocolNode not exist."));
                    return;
                }
                for (int i = 0; i < protocolNode.ChildNodes.Count; i++)
                {
                    var ele = protocolNode.ChildNodes[i] as XmlElement;
                    if (ele == null) { continue; }
                    var strProtocol = ele.GetAttribute("Protocol");
                    if (strProtocol.ToLower().Equals("https"))
                    {
                        var strHash = ele.GetAttribute("OtherArgs");
                        if (!string.IsNullOrEmpty(strHash))
                        {
                            App.WriteLog("InstallServerCert",
                                string.Format("Server certificate hash is {0}", strHash));
                            string strFile = Path.Combine(strPath, string.Format("{0}.pfx", strCertName));
                            if (!File.Exists(strFile))
                            {
                                App.WriteLog("InstallServerCert", string.Format("File not exist.\t{0}", strFile));
                                return;
                            }
                            byte[] certData = File.ReadAllBytes(strFile);
                            if (!IsCertificateExist(strHash, StoreName.AuthRoot, StoreLocation.LocalMachine))
                            {
                                InstallCertificateToStore(StoreName.AuthRoot, StoreLocation.LocalMachine, certData,
                                    CERT_PASSWORD);
                            }
                            App.WriteLog("InstallServerCert", string.Format("End.\t{0}", strFile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("InstallServerCert", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private bool IsCertificateExist(string certHash, StoreName storeName, StoreLocation storeLocation)
        {
            bool isExist = false;
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            for (int i = 0; i < store.Certificates.Count; i++)
            {
                var cert = store.Certificates[i];
                var certHashString = cert.GetCertHashString();
                if (certHashString != null && certHashString.Equals(certHash))
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        private void InstallCertificateToStore(StoreName storeName, StoreLocation storeLocation, byte[] certData, string password)
        {
            X509Certificate2 cert = new X509Certificate2(certData, password);
            X509Store store = new X509Store(storeName, storeLocation);
            cert.FriendlyName = mAppShortName;
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();
        }

        private void InstallCLRTypeComponent()
        {
            try
            {
                if (mIsFail) { return; }

                string strName;
                if (Environment.Is64BitOperatingSystem)
                {
                    strName = "ReportViewer0000";
                }
                else
                {
                    strName = "ReportViewer0001";
                }
                SetBusy(true, App.GetLanguageInfo("S052", string.Format("Installing CLR Type...\t{0}", strName)));
                string strFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\UMPClient\\{0}.msi", strName));
                if (!File.Exists(strFile))
                {
                    App.WriteLog("InstallCLRType", string.Format("File not exist.\t{0}", strFile));
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = strFile;
                process.StartInfo.Arguments = "/qn";
                process.Start();
                process.WaitForExit();
                if (process.HasExited == false)
                {
                    process.Kill();
                }
                process.Dispose();

                App.WriteLog("InstallCLRType", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void InstallReportViewerComponent()
        {
            try
            {
                if (mIsFail) { return; }

                List<string> listNames = new List<string>();
                listNames.Add("ReportViewer1033");
                listNames.Add("ReportViewer1028");
                listNames.Add("ReportViewer1041");
                listNames.Add("ReportViewer2052");

                for (int i = 0; i < listNames.Count; i++)
                {
                    string strName = listNames[i];
                    SetBusy(true, App.GetLanguageInfo("S052", string.Format("Installing ReportViewer...\t{0}", strName)));
                    string strFile = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        string.Format("UMP\\UMPClient\\{0}.msi", strName));
                    if (!File.Exists(strFile))
                    {
                        App.WriteLog("InstallCLRType", string.Format("File not exist.\t{0}", strFile));
                        return;
                    }
                    Process process = new Process();
                    process.StartInfo.FileName = strFile;
                    process.StartInfo.Arguments = "/qn";
                    process.Start();
                    process.WaitForExit();
                    if (process.HasExited == false)
                    {
                        process.Kill();
                    }
                    process.Dispose();
                    App.WriteLog("InstallReport", string.Format("End.\t{0}", strFile));
                }
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void SetTrustWebSite()
        {
            try
            {
                if (mIsFail) { return; }

                string strHost = mCurrentServer.Address;
                SetBusy(true, string.Format(App.GetLanguageInfo("S053", string.Format("Setting TrustWebSite...\t{0}", strHost)), strHost));
                RegistryKey rootKey = Registry.Users;
                string[] listUsers = rootKey.GetSubKeyNames();
                for (int i = 0; i < listUsers.Length; i++)
                {
                    RegistryKey userKey = rootKey.OpenSubKey(listUsers[i]);
                    if (userKey == null) { continue; }
                    RegistryKey volatileKey = userKey.OpenSubKey("Volatile Environment");
                    if (volatileKey == null) { continue; }
                    var attrUserName = volatileKey.GetValue("USERNAME");
                    if (attrUserName == null) { continue; }
                    string strUserName = attrUserName.ToString();
                    if (!strUserName.ToLower().Equals(Environment.UserName.ToLower())) { continue; }
                    bool isIPAddress = HostIsIPAddress();
                    string strSourceType = isIPAddress ? "IPAddress" : "MachineName";
                    string strFile =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "UMP\\UMPClient\\UMP.Server.01.xml");
                    if (!File.Exists(strFile))
                    {
                        App.WriteLog("SetTrustWebSite", string.Format("File not exist.\t{0}", strFile));
                        return;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strFile);
                    var zonesNode = doc.SelectSingleNode("UMPSetted/TrustZones");
                    if (zonesNode == null)
                    {
                        App.WriteLog("SetTrustWebSite", string.Format("TrustZones node not exist."));
                        return;
                    }
                    var machineKey = Registry.LocalMachine;
                    var versionKey = machineKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                    if (versionKey == null)
                    {
                        App.WriteLog("SetTrustWebSite", string.Format("WindowsVersion key not exist."));
                        return;
                    }
                    var attrVersion = versionKey.GetValue("CurrentVersion");
                    if (attrVersion == null)
                    {
                        App.WriteLog("SetTrustWebSite", string.Format("CurrentVersion attrabute not exist."));
                        return;
                    }
                    string strVersion = attrVersion.ToString();
                    double intVersion;
                    if (!double.TryParse(strVersion, out intVersion))
                    {
                        App.WriteLog("SetTrustWebSite", string.Format("WindowsVersion invalid.\t{0}", strVersion));
                        return;
                    }
                    string strInstallType = "NONE";
                    if (intVersion > 6.0)
                    {
                        var attrType = versionKey.GetValue("InstallationType");
                        if (attrType == null)
                        {
                            App.WriteLog("SetTrustWebSite", string.Format("InstallationType attrabute not exist."));
                            return;
                        }
                        strInstallType = attrType.ToString().ToUpper();
                    }
                    App.WriteLog("SetTrustWebSite",
                        string.Format("WinVersion:{0};InstallType:{1};SourceType:{2}", intVersion, strInstallType,
                            strSourceType));
                    for (int j = 0; j < zonesNode.ChildNodes.Count; j++)
                    {
                        var zoneNode = zonesNode.ChildNodes[j] as XmlElement;
                        if (zoneNode == null) { continue; }
                        if (zoneNode.GetAttribute("WinVersion").Equals(intVersion.ToString())
                            && zoneNode.GetAttribute("InstallType").Equals(strInstallType)
                            && zoneNode.GetAttribute("SourceType").Equals(strSourceType))
                        {
                            string strRootPath = zoneNode.GetAttribute("RootRegistryKey");
                            string strSubPath = zoneNode.GetAttribute("SubRegistryKey");
                            RegistryKey currentUserKey = Registry.CurrentUser;
                            RegistryKey zoneMapKey =
                                currentUserKey.OpenSubKey(strRootPath, true);
                            if (zoneMapKey == null)
                            {
                                App.WriteLog("SetTrustWebSite", string.Format("ZoneMap key not exist."));
                                return;
                            }
                            RegistryKey mapKey = zoneMapKey.CreateSubKey(strSubPath);
                            if (mapKey == null)
                            {
                                App.WriteLog("SetTrustWebSite", string.Format("Map key not exist."));
                                return;
                            }
                            if (isIPAddress)
                            {
                                string[] listSubMapKeys = mapKey.GetSubKeyNames();
                                bool isExist = false;
                                for (int k = 0; k < listSubMapKeys.Length; k++)
                                {
                                    string strSubMap = listSubMapKeys[k];
                                    var subMapKey = mapKey.OpenSubKey(strSubMap, true);
                                    if (subMapKey != null)
                                    {
                                        var attrRange = subMapKey.GetValue(":Range");
                                        if (attrRange == null)
                                        {
                                            continue;
                                        }
                                        string strRange = attrRange.ToString();
                                        if (strRange.Equals(strHost))
                                        {
                                            isExist = true;
                                            subMapKey.SetValue("http", 2, RegistryValueKind.DWord);
                                            subMapKey.SetValue("https", 2, RegistryValueKind.DWord);

                                            App.WriteLog("SetTrustWebSite",
                                                string.Format("TrustWebsite setted.\t{0}\t{1}", strSubMap, strHost));
                                        }
                                    }
                                }
                                if (!isExist)
                                {
                                    int k = 1;
                                    string strSubMap;
                                    do
                                    {
                                        strSubMap = string.Format("Range{0}", k);
                                        if (!listSubMapKeys.Contains(strSubMap))
                                        {
                                            break;
                                        }
                                        k++;
                                    } while (k < int.MaxValue);
                                    var subMapKey = mapKey.CreateSubKey(strSubMap);
                                    if (subMapKey != null)
                                    {
                                        subMapKey.SetValue(":Range", strHost);
                                        subMapKey.SetValue("http", 2, RegistryValueKind.DWord);
                                        subMapKey.SetValue("https", 2, RegistryValueKind.DWord);

                                        App.WriteLog("SetTrustWebSite",
                                            string.Format("TrustWebsite setted.\t{0}\t{1}", strSubMap, strHost));
                                    }
                                }
                            }
                            else
                            {
                                string[] listSubMapKeys = mapKey.GetSubKeyNames();
                                if (listSubMapKeys.Contains(strHost))
                                {
                                    string strSubMap = strHost;
                                    var subMapKey = mapKey.OpenSubKey(strSubMap, true);
                                    if (subMapKey != null)
                                    {
                                        subMapKey.SetValue("http", 2, RegistryValueKind.DWord);
                                        subMapKey.SetValue("https", 2, RegistryValueKind.DWord);

                                        App.WriteLog("SetTrustWebSite",
                                          string.Format("TrustWebsite setted.\t{0}\t{1}", strSubMap, strHost));
                                    }
                                }
                                else
                                {
                                    string strSubMap = strHost;
                                    var subMapKey = mapKey.CreateSubKey(strSubMap);
                                    if (subMapKey != null)
                                    {
                                        subMapKey.SetValue("http", 2, RegistryValueKind.DWord);
                                        subMapKey.SetValue("https", 2, RegistryValueKind.DWord);

                                        App.WriteLog("SetTrustWebSite",
                                          string.Format("TrustWebsite setted.\t{0}\t{1}", strSubMap, strHost));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void SetAuthRootCertUpdatePolicy()
        {
            //通过修改注册表，关闭跟证书自动更新功能
            try
            {
                if (mIsFail) { return; }

                //SetBusy(true, string.Format("Setting AuthRoot Certificate update policy..."));
                string path = @"SOFTWARE\Policies\Microsoft\SystemCertificates\AuthRoot";
                RegistryKey rootKey = Registry.LocalMachine;
                RegistryKey authRootKey = rootKey.CreateSubKey(path);
                if (authRootKey == null) { return; }
                authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);

                if (Environment.Is64BitOperatingSystem)
                {
                    path = @"SOFTWARE\Wow6432Node\Policies\Microsoft\SystemCertificates\AuthRoot";
                    rootKey = Registry.LocalMachine;
                    authRootKey = rootKey.CreateSubKey(path);
                    if (authRootKey == null) { return; }
                    authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);
                }

                App.WriteLog("SetAuthRootPolicy", string.Format("Set AuthRoot Certificate update policy end."));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void CreateShortcuts()
        {
            try
            {
                if (mIsFail) { return; }

                if (mSettingInfo.LaunchUMP <= 0) { return;}       //如果没有勾选运行UMP，则无需创建UMP的快捷方式
                SetBusy(true, App.GetLanguageInfo("S055", string.Format("Creating shortcut...")));
                string strTempDir =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "UMP\\UMPClient");
                if (!Directory.Exists(strTempDir))
                {
                    Directory.CreateDirectory(strTempDir);
                }
                string strXml = Path.Combine(strTempDir, "UMP.Server.01.xml");
                if (!File.Exists(strXml))
                {
                    App.WriteLog("CreateShortcut", string.Format("File not exist.\t{0}", strXml));
                    return;
                }
                string strProtocol = string.Empty;
                string strHost = mCurrentServer.Address;
                int intPort = mCurrentServer.Port;
                XmlDocument doc = new XmlDocument();
                doc.Load(strXml);
                var protocolNodes = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (protocolNodes == null)
                {
                    App.WriteLog("CreateShortcut", string.Format("ProtocolNodes not exist."));
                    return;
                }
                for (int i = 0; i < protocolNodes.ChildNodes.Count; i++)
                {
                    var node = protocolNodes.ChildNodes[i] as XmlElement;
                    if (node != null)
                    {
                        string strActived = node.GetAttribute("Activated");
                        if (strActived.Equals("1"))
                        {
                            string str = node.GetAttribute("Protocol").ToLower();
                            if (str == "http"
                                || str == "https")
                            {
                                strProtocol = str;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(strProtocol))
                {
                    App.WriteLog("CreateShortcut", string.Format("Protocol invalid."));
                    return;
                }
                if (strProtocol.Equals("https"))
                {
                    intPort++;
                }
                App.WriteLog("CreateShortcut", string.Format("Protocol is {0}", strProtocol));

                string linkName = string.Format("{0}.lnk", mAppShortName);
                string linkDir = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                string linkPath = Path.Combine(linkDir, linkName);
                string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Internet Explorer");
                string targetPath = Path.Combine(targetDir, "IEXPLORE.EXE");
                string targetIcon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "UMPAppLogo.ico");
                string args = string.Format("{0}://{1}:{2}/UMPMain.xbap", strProtocol, strHost, intPort);
                string vbs = string.Empty;
                vbs += string.Format("Set WshShell = WScript.CreateObject(\"WScript.Shell\")\r\n");
                vbs += string.Format("set oShellLink = WshShell.CreateShortcut(\"{0}\")\r\n", linkPath);
                vbs += string.Format("Dim fso\r\n");
                vbs += string.Format("Set fso=CreateObject(\"Scripting.FileSystemObject\")\r\n");
                vbs += string.Format("oShellLink.TargetPath = \"{0}\"\r\n", targetPath);
                vbs += string.Format("oShellLink.Arguments = \"{0}\"\r\n", args);
                vbs += string.Format("oShellLink.WorkingDirectory = \"{0}\"\r\n", targetDir);
                vbs += string.Format("oShellLink.WindowStyle = 1\r\n");
                vbs += string.Format("oShellLink.IconLocation = \"{0},0\"\r\n", targetIcon);
                vbs += string.Format("oShellLink.Save");
                string vbsFile = Path.Combine(strTempDir, string.Format("Temp0.vbs"));
                File.WriteAllText(vbsFile, vbs);
                Process.Start(vbsFile);
                App.WriteLog("CreateShortcut", string.Format("StartMenu Shortcut created"));

                linkDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                linkPath = Path.Combine(linkDir, linkName);
                vbs = string.Empty;
                vbs += string.Format("Set WshShell = WScript.CreateObject(\"WScript.Shell\")\r\n");
                vbs += string.Format("set oShellLink = WshShell.CreateShortcut(\"{0}\")\r\n", linkPath);
                vbs += string.Format("Dim fso\r\n");
                vbs += string.Format("Set fso=CreateObject(\"Scripting.FileSystemObject\")\r\n");
                vbs += string.Format("oShellLink.TargetPath = \"{0}\"\r\n", targetPath);
                vbs += string.Format("oShellLink.Arguments = \"{0}\"\r\n", args);
                vbs += string.Format("oShellLink.WorkingDirectory = \"{0}\"\r\n", targetDir);
                vbs += string.Format("oShellLink.WindowStyle = 1\r\n");
                vbs += string.Format("oShellLink.IconLocation = \"{0},0\"\r\n", targetIcon);
                vbs += string.Format("oShellLink.Save");
                vbsFile = Path.Combine(strTempDir, string.Format("Temp1.vbs"));
                File.WriteAllText(vbsFile, vbs);
                Process.Start(vbsFile);
                App.WriteLog("CreateShortcut", string.Format("Desktop Shortcut created"));
            }
            catch (Exception ex)
            {
                mIsFail = true;
                mErrorMsg = ex.Message;
            }
        }

        private void SaveSettingInfos()
        {
            try
            {
                if (mIsFail) { return;}

                if (mSettingInfo == null) { return; }
                SetBusy(true, App.GetLanguageInfo("S052", string.Format("Saving config information...")));
                string strPath = Path.Combine(
                   Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, UMPSettingInfo.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mSettingInfo, strFile);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                App.WriteLog("SaveSetting", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LaunchUMP()
        {
            try
            {
                string strLink = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    string.Format("{0}.lnk", mAppShortName));
                if (File.Exists(strLink))
                {
                    Process.Start(strLink);
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("LaunchUMP", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LaunchAgent()
        {
            try
            {
                if (mCurrentServer == null) { return;}

                string strLink = string.Format("http://{0}:{1}/UMPS3104.application", mCurrentServer.Address,
                    mCurrentServer.Port);
                Process.Start(strLink);
            }
            catch (Exception ex)
            {
                App.WriteLog("LaunchAgent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region AppButton Handlers

        void BtnAppClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void BtnAppMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void BtnAppMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button clickButton = sender as Button;
                if (clickButton == null) { return; }
                clickButton.ContextMenu.PlacementTarget = clickButton;
                clickButton.ContextMenu.Placement = PlacementMode.Bottom;
                clickButton.ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            InstallComponents();
        }

        void ComboServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ComboServers.SelectedItem as ComboItem;
            if (item == null) { return; }
            var info = item.Data as UMPServerInfo;
            if (info == null) { return; }
            TxtServerPort.Text = info.Port.ToString();
        }

        void LangMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as MenuItem;
                if (btn == null) { return; }
                if (btn.Tag == null) { return; }
                string strLangID = btn.Tag.ToString();
                int intLangID;
                if (!int.TryParse(strLangID, out intLangID)) { return; }
                App.LangID = intLangID;
                if (mSettingInfo != null)
                {
                    mSettingInfo.LangID = intLangID;
                }
                App.LoadAllLanguages();
                InitAppMenuButtons();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Basics

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetBusy(bool isWorking, string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ImageStatus.Visibility = isWorking ? Visibility.Visible : Visibility.Collapsed;
                TxtStatusContent.Text = msg;
                TxtStatusContent.ToolTip = msg;
            }));
        }

        private void SetCheckBusy(bool isWorking, string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MyWaiter.Visibility = isWorking ? Visibility.Visible : Visibility.Collapsed;
                TxtStatus.Text = msg;
            }));
        }

        private BasicHttpBinding CreateBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            return binding;
        }

        private EndpointAddress CreateEndpointAddress(UMPServerInfo serverInfo, string serviceName)
        {
            string uri = string.Format("http://{0}:{1}/Wcf2Client/{2}.svc", serverInfo.Address, serverInfo.Port,
                serviceName);
            return new EndpointAddress(new Uri(uri, UriKind.Absolute));
        }

        private bool HostIsIPAddress()
        {
            try
            {
                IPAddress address;
                return IPAddress.TryParse(mCurrentServer.Address, out address);
            }
            catch
            {
                return false;
            }
        }

        private void SetButtonEnable(bool isWorking)
        {
            BtnConfirm.IsEnabled = !isWorking;
            BtnClose.IsEnabled = !isWorking;
            BtnAppClose.IsEnabled = !isWorking;
        }

        #endregion


        #region ChangeLanguage

        private void ChangeLanguage()
        {
            try
            {
                TabServerInfo.Header = App.GetLanguageInfo("T003", "Server Information");
                TxtServerInfo.Text = App.GetLanguageInfo("T005", "Please input UMP server information");
                LbServerHost.Text = App.GetLanguageInfo("T006", "Server Address");
                LbServerPort.Text = App.GetLanguageInfo("T007", "Server Port");

                CbLaunchUMP.Content = App.GetLanguageInfo("T008", "Launch UMP after installed");
                CbLaunchAgent.Content = App.GetLanguageInfo("T009", "Launch UMP Agent Client");

                BtnConfirm.Content = App.GetLanguageInfo("B001", "Confirm");
                BtnClose.Content = App.GetLanguageInfo("B002", "Close");
            }
            catch { }
        }

        #endregion

    }
}
