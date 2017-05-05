//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d6c6b8f0-29d6-48bd-b788-23bfcac4891f
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateWindow
//
//        created by Charley at 2016/8/1 18:27:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using Microsoft.Win32;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow
    {

        #region Members

        private bool mIsInited;
        private UpdateInfo mUpdateInfo;
        private InstallState mInstallState;
        private InstallInfo mInstallInfo;
        private DatabaseInfo mDataBaseInfo;
        private int mCurrentStep;   //当前步骤，0：BugInfoList；1：LicenseAgree；2：Options；3：ProgressView；4：UpdateResultView

        private List<ServiceInfo> mListAllServiceInfos;
        private List<ServiceInfo> mListInstalledServices;
        private List<InstallProduct> mListAllProductInfos;
        private List<InstallProduct> mListInstalledProducts;

        #endregion


        public UpdateWindow()
        {
            InitializeComponent();

            mListAllServiceInfos = new List<ServiceInfo>();
            mListInstalledServices = new List<ServiceInfo>();
            mListAllProductInfos = new List<InstallProduct>();
            mListInstalledProducts = new List<InstallProduct>();

            Loaded += UpdateWindow_Loaded;
            Closing += UpdateWindow_Closing;
            MouseLeftButtonDown += (s, e) => DragMove();

            BtnAppMenu.Click += BtnAppMenu_Click;
            BtnAppMinimize.Click += BtnAppMinimize_Click;
            BtnAppMaximize.Click += BtnAppMaximize_Click;
            BtnAppRestore.Click += BtnAppRestore_Click;
            BtnAppClose.Click += BtnAppClose_Click;

            //BtnPrevious.Click += BtnPrevious_Click;
            //BtnNext.Click += BtnNext_Click;
            //BtnClose.Click += BtnClose_Click;
        }

        void UpdateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void UpdateWindow_Closing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show(App.GetLanguageInfo("N002", string.Format("Confirm close window?")), App.AppName, MessageBoxButton.YesNo,
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
                WindowState = WindowState.Normal;
                BorderMain.Padding = new Thickness(0);
                BorderCheckPanel.Visibility = Visibility.Visible;
                BorderMainPanel.Visibility = Visibility.Collapsed;
                BtnAppRestore.Visibility = Visibility.Collapsed;
                BtnAppMaximize.Visibility = Visibility.Visible;

                SetCheckBusy(true, App.GetLanguageInfo("N001", string.Format("Checking environment on this machine, please wait for a moment...")));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    App.LoadAllLanguages();
                    InitAllProductInfos();
                    InitAllServiceInfos();
                    InitInstallState();
                    InitDatabaseInfo();
                    InitInstallInfo();
                    InitInstallInfoDB();
                    InitProductsInstalled();
                    InitInstalledServices();
                    ReadUpdateInfo();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetCheckBusy(false, string.Empty);

                    BorderCheckPanel.Visibility = Visibility.Collapsed;
                    BorderMainPanel.Visibility = Visibility.Visible;

                    InitAppMenu();
                    ShowBasicInformation();

                    mCurrentStep = 0;
                    ShowStepInformation();

                    UCRightPanelView rightView = new UCRightPanelView();
                    rightView.UpdateInfo = mUpdateInfo;
                    rightView.DatabaseInfo = mDataBaseInfo;
                    rightView.InstallState = mInstallState;
                    rightView.ListInstalledProducts = mListInstalledProducts;
                    BorderRightPanel.Child = rightView;

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAllServiceInfos()
        {
            try
            {
                mListAllServiceInfos.Clear();


                #region UMP Services

                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService00";
                serviceInfo.ServiceName = "UMP Service 00";
                serviceInfo.DisplayName = "UMP Service 00";
                serviceInfo.FileName = "UMPService00.exe";
                serviceInfo.ProcessName = "UMPService00";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService01";
                serviceInfo.ServiceName = "UMP Service 01";
                serviceInfo.DisplayName = "UMP Service 01";
                serviceInfo.FileName = "UMPService01.exe";
                serviceInfo.ProcessName = "UMPService01";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService02";
                serviceInfo.ServiceName = "UMP Service 02";
                serviceInfo.DisplayName = "UMP Service 02";
                serviceInfo.FileName = "UMPService02.exe";
                serviceInfo.ProcessName = "UMPService02";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService03";
                serviceInfo.ServiceName = "UMP Service 03";
                serviceInfo.DisplayName = "UMP Service 03";
                serviceInfo.FileName = "UMPService03.exe";
                serviceInfo.ProcessName = "UMPService03";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService04";
                serviceInfo.ServiceName = "UMP Service 04";
                serviceInfo.DisplayName = "UMP Service 04";
                serviceInfo.FileName = "UMPService04.exe";
                serviceInfo.ProcessName = "UMPService04";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService05";
                serviceInfo.ServiceName = "UMP Service 05";
                serviceInfo.DisplayName = "UMP Service 05";
                serviceInfo.FileName = "UMPService05.exe";
                serviceInfo.ProcessName = "UMPService05";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService06";
                serviceInfo.ServiceName = "UMP Service 06";
                serviceInfo.DisplayName = "UMP Service 06";
                serviceInfo.FileName = "UMPService06.exe";
                serviceInfo.ProcessName = "UMPService06";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService07";
                serviceInfo.ServiceName = "UMP Service 07";
                serviceInfo.DisplayName = "UMP Service 07";
                serviceInfo.FileName = "UMPService07.exe";
                serviceInfo.ProcessName = "UMPService07";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService08";
                serviceInfo.ServiceName = "UMP Service 08";
                serviceInfo.DisplayName = "UMP Service 08";
                serviceInfo.FileName = "UMPService08.exe";
                serviceInfo.ProcessName = "UMPService08";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService09";
                serviceInfo.ServiceName = "UMP Service 09";
                serviceInfo.DisplayName = "UMP Service 09";
                serviceInfo.FileName = "UMPService09.exe";
                serviceInfo.ProcessName = "UMPService09";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService10";
                serviceInfo.ServiceName = "UMP Service 10";
                serviceInfo.DisplayName = "UMP Service 10";
                serviceInfo.FileName = "UMPService10.exe";
                serviceInfo.ProcessName = "UMPService10";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService13";
                serviceInfo.ServiceName = "UMP Service 13";
                serviceInfo.DisplayName = "UMP Service 13";
                serviceInfo.FileName = "UMPService13.exe";
                serviceInfo.ProcessName = "UMPService13";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_UMP;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region Voice

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPCRYPTION";
                serviceInfo.ServiceName = "UMPCRYPTION";
                serviceInfo.DisplayName = "UMP Cryption";
                serviceInfo.FileName = "UMPCryption.exe";
                serviceInfo.ProcessName = "UMPCryption";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPDBbridgeServer";
                serviceInfo.ServiceName = "UMPDBbridgeServer";
                serviceInfo.DisplayName = "UMP DBBridge Server";
                serviceInfo.FileName = "DBbridgeServer.exe";
                serviceInfo.ProcessName = "DBbridgeServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPFileService";
                serviceInfo.ServiceName = "UMPFS";
                serviceInfo.DisplayName = "UMP FileService";
                serviceInfo.FileName = "UMPFileService.exe";
                serviceInfo.ProcessName = "UMPFileService";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPKeyGenerator";
                serviceInfo.ServiceName = "UMPKEYGEN";
                serviceInfo.DisplayName = "UMP KeyGenerator";
                serviceInfo.FileName = "KeyGenServer.exe";
                serviceInfo.ProcessName = "KeyGenServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPSftp";
                serviceInfo.ServiceName = "UMPSFTP";
                serviceInfo.DisplayName = "UMP SFTP";
                serviceInfo.FileName = "UMPSftp.exe";
                serviceInfo.ProcessName = "UMPSftp";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPTellServer";
                serviceInfo.ServiceName = "UMPTellServer";
                serviceInfo.DisplayName = "UMP Tell Server";
                serviceInfo.FileName = "TellServer.exe";
                serviceInfo.ProcessName = "TellServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPVoiceServer";
                serviceInfo.ServiceName = "UMPVoiceServer";
                serviceInfo.DisplayName = "UMP Voice Server";
                serviceInfo.FileName = "VoiceServer.exe";
                serviceInfo.ProcessName = "VoiceServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPFDCServer";
                serviceInfo.ServiceName = "UMPFDCServer";
                serviceInfo.DisplayName = "UMP FDC Server";
                serviceInfo.FileName = "UMPFDCServer.exe";
                serviceInfo.ProcessName = "UMPFDCServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region Alarm

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPAlarmMonitor";
                serviceInfo.ServiceName = "UMPAlarmMonitor";
                serviceInfo.DisplayName = "UMP Alarm Monitor";
                serviceInfo.FileName = "AlarmMonitor.exe";
                serviceInfo.ProcessName = "AlarmMonitor";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_ALARMSERVER;
                mListAllServiceInfos.Add(serviceInfo);

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPAlarmServer";
                serviceInfo.ServiceName = "UMPAlarmServer";
                serviceInfo.DisplayName = "UMP Alarm Server";
                serviceInfo.FileName = "AlarmServer.exe";
                serviceInfo.ProcessName = "AlarmServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_ALARMSERVER;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region CMServer

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPCMServer";
                serviceInfo.ServiceName = "UMPCMServer";
                serviceInfo.DisplayName = "UMP NetPacket Retransmit Server";
                serviceInfo.FileName = "CMServer.exe";
                serviceInfo.ProcessName = "CMServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_CMSERVER;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region CTI

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPCTIDBBridge";
                serviceInfo.ServiceName = "UMPCTIDBB";
                serviceInfo.DisplayName = "UMP CTI DB Bridge";
                serviceInfo.FileName = "CTIDBB.exe";
                serviceInfo.ProcessName = "CTIDBB";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_CTIHUB;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPCTIHub";
                serviceInfo.ServiceName = "UMPCTIHUB";
                serviceInfo.DisplayName = "UMP CTI Hub Server";
                serviceInfo.FileName = "CTIHubServer.exe";
                serviceInfo.ProcessName = "CTIHubServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_CTIHUB;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region DEC

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPDEC";
                serviceInfo.ServiceName = "UMPDEC";
                serviceInfo.DisplayName = "UMP DEC Server";
                serviceInfo.FileName = "decserver.exe";
                serviceInfo.ProcessName = "decserver";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_DEC;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region Screen

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPScreen";
                serviceInfo.ServiceName = "UMPScreen";
                serviceInfo.DisplayName = "UMP Screen Server";
                serviceInfo.FileName = "ScreenServer.exe";
                serviceInfo.ProcessName = "ScreenServer";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_SCREENSERVER;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region SpeechAnalysis

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPSpeechAnalysis";
                serviceInfo.ServiceName = "UMPSpeechAnalysis";
                serviceInfo.DisplayName = "UMP Speech Analysis";
                serviceInfo.FileName = "SpeechAnalysis..exe";
                serviceInfo.ProcessName = "SpeechAnalysis.";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_SPEECHANALYSIS;
                mListAllServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPUploadRecord";
                serviceInfo.ServiceName = "UMPUploadRecord";
                serviceInfo.DisplayName = "UMP Upload Record";
                serviceInfo.FileName = "UploadRecord..exe";
                serviceInfo.ProcessName = "UploadRecord.";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_SPEECHANALYSIS;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region CaptureServer

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPCaptureServer";
                serviceInfo.ServiceName = "UMPCaptureServer";
                serviceInfo.DisplayName = "UMP Capture Server";
                serviceInfo.FileName = "VCLogIPRaw.exe";
                serviceInfo.ProcessName = "VCLogIPRaw";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_CAPTURESERVER;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion


                #region RecoverServer

                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPRecoverServer";
                serviceInfo.ServiceName = "UMPRecoverServer";
                serviceInfo.DisplayName = "UMP Recover Server";
                serviceInfo.FileName = "VCLogIPRec.exe";
                serviceInfo.ProcessName = "VCLogIPRec";
                serviceInfo.Package = UpdateConsts.PACKAGE_NAME_RECOVERSERVER;
                mListAllServiceInfos.Add(serviceInfo);

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAllProductInfos()
        {
            try
            {
                mListAllProductInfos.Clear();
                InstallProduct product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_UMP;
                product.Package = UpdateConsts.PACKAGE_NAME_UMP;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_UMP;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_UMP;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_ALARMSERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_ALARMSERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_ALARMSERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_ALARMSERVER;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_CMSERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_CMSERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_CMSERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_CMSERVER;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_CTIHUB;
                product.Package = UpdateConsts.PACKAGE_NAME_CTIHUB;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_CTIHUB;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_CTIHUB;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_DEC;
                product.Package = UpdateConsts.PACKAGE_NAME_DEC;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_DEC;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_DEC;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_LICENSESERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_LICENSESERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_LICENSESERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_LICENSESERVER;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_SCREENSERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_SCREENSERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_SCREENSERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_SCREENSERVER;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_SOFTRECORD;
                product.Package = UpdateConsts.PACKAGE_NAME_SOFTRECORD;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_SOFTRECORD;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_SOFTRECORD;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_SPEECHANALYSIS;
                product.Package = UpdateConsts.PACKAGE_NAME_SPEECHANALYSIS;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_SPEECHANALYSIS;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_SPEECHANALYSIS;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_IXPATCH;
                product.Package = UpdateConsts.PACKAGE_NAME_IXPATCH;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_IXPATCH;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_IXPATCH;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_VOICE;
                product.Package = UpdateConsts.PACKAGE_NAME_VOICE;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_VOICE;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_VOICE;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_CAPTURESERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_CAPTURESERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_CAPTURESERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_CAPTURESERVER;
                mListAllProductInfos.Add(product);
                product = new InstallProduct();
                product.ProductName = UpdateConsts.PACKAGE_NAME_RECOVERSERVER;
                product.Package = UpdateConsts.PACKAGE_NAME_RECOVERSERVER;
                product.DisplayName = UpdateConsts.PACKAGE_NAME_RECOVERSERVER;
                product.ProductGuid = UpdateConsts.PACKAGE_GUID_RECOVERSERVER;
                mListAllProductInfos.Add(product);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInstallInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "VoiceCyber\\UMP");
                string strFile = Path.Combine(path, InstallInfo.FILE_NAME);
                if (!File.Exists(strFile))
                {
                    App.WriteLog("InitInstallInfo", string.Format("File not exist.\t{0}", strFile));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<InstallInfo>(strFile);
                if (!optReturn.Result)
                {
                    App.WriteLog("InitInstallInfo", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                var installInfo = optReturn.Data as InstallInfo;
                if (installInfo == null)
                {
                    App.WriteLog("InitInstallInfo", string.Format("File not exist.\t{0}", strFile));
                    return;
                }
                mInstallInfo = installInfo;
                App.WriteLog("InitInstallInfo", string.Format("End.\t{0}", mInstallInfo.Version));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInstallInfoDB()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitProductsInstalled()
        {
            try
            {
                mListInstalledProducts.Clear();
                if (mInstallState == null) { return; }
                InstallProduct product;


                #region 从注册表获取产品安装信息

                for (int i = 0; i < mListAllProductInfos.Count; i++)
                {
                    product = mListAllProductInfos[i];
                    string strGUID = product.ProductGuid;
                    RegistryKey key = Registry.LocalMachine;
                    string strSubKey;
                    //如果是64位系统
                    if (Environment.Is64BitOperatingSystem)
                    {
                        strSubKey =
                            string.Format("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}",
                                strGUID);
                    }
                    else
                    {
                        strSubKey = string.Format("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}",
                            strGUID);
                    }
                    RegistryKey keyAppInfo = key.OpenSubKey(strSubKey);
                    if (keyAppInfo == null)
                    {
                        continue;
                    }
                    var installPath = keyAppInfo.GetValue("InstallLocation");
                    if (installPath != null)
                    {
                        product.InstallPath = installPath.ToString();
                    }
                    var version = keyAppInfo.GetValue("DisplayVersion");
                    if (version != null)
                    {
                        product.Version = FormatVersion(version.ToString());
                    }
                    mListInstalledProducts.Add(product);

                    App.WriteLog("InitProducts",
                        string.Format("Installed product:{0}\tPath:{1}\tVersion:{2}", product.ProductName,
                            product.InstallPath, product.Version));
                }

                App.WriteLog("InitProducts", string.Format("Installed products count:{0}", mListInstalledProducts.Count));

                #endregion


                #region 如果存在安装信息，从安装信息中获取产品最新版本信息

                if (mInstallInfo != null)
                {
                    for (int i = 0; i < mListInstalledProducts.Count; i++)
                    {
                        product = mListInstalledProducts[i];
                        var temp =
                            mInstallInfo.ListProducts.FirstOrDefault(p => p.ProductGuid == product.ProductGuid);
                        if (temp != null)
                        {
                            var a = CompareVersion(product.Version, temp.Version);
                            if (a > 0)
                            {
                                product.Version = FormatVersion(temp.Version);     //最新版本（补丁版本）

                                App.WriteLog("InitProducts",
                                    string.Format("Confirm version by InstallInfo {0}:{1}", product.ProductName,
                                        product.Version));
                            }
                        }
                    }
                }

                #endregion


                #region 如果数据库创建了，从数据库中获取产品的最新版本信息

                if (mInstallState.IsDatabaseCreated
                   && mDataBaseInfo != null)
                {
                    int dbType = mDataBaseInfo.TypeID;
                    string strConn = mDataBaseInfo.GetConnectionString();
                    OperationReturn optReturn = null;
                    string strSql;
                    if (dbType == 2)
                    {
                        strSql = string.Format("SELECT * FROM T_00_301 ORDER BY C002, C006 DESC, C003 DESC, C001");
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                    }
                    if (dbType == 3)
                    {
                        strSql = string.Format("SELECT * FROM T_00_301 ORDER BY C002, C006 DESC, C003 DESC, C001");
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                    }
                    if (optReturn == null)
                    {
                        App.WriteLog("InitProducts", string.Format("Database type invalid."));
                    }
                    else
                    {
                        DataSet objDataSet = optReturn.Data as DataSet;
                        if (objDataSet == null
                            || objDataSet.Tables.Count <= 0)
                        {
                            App.WriteLog("InitProducts", string.Format("DataSet is null or table not exist."));
                        }
                        else
                        {
                            for (int i = 0; i < mListInstalledProducts.Count; i++)
                            {
                                product = mListInstalledProducts[i];
                                DataRow dr =
                                    objDataSet.Tables[0].Select(string.Format("C002 = '{0}'", product.Package))
                                        .FirstOrDefault();
                                if (dr != null)
                                {
                                    string strVer = dr["C006"].ToString();
                                    var a = CompareVersion(product.Version, strVer);
                                    if (a > 0)
                                    {
                                        product.Version = FormatVersion(strVer);

                                        App.WriteLog("InitProducts",
                                            string.Format("Confirm version by database {0}:{1}", product.ProductName,
                                                product.Version));
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion


                #region 判断UMP是否安装

                mInstallState.IsUMPInstalled = false;
                product = mListInstalledProducts.FirstOrDefault(p => p.ProductGuid == UpdateConsts.PACKAGE_GUID_UMP);
                if (product != null)
                {
                    mInstallState.IsUMPInstalled = true;
                    mInstallState.UMPInstallPath = product.InstallPath;
                    App.WriteLog("InitProducts",
                        string.Format("UMP has installed on this machine.\tPath:{0};Version:{1}", product.InstallPath,
                            product.Version));
                }

                #endregion


                #region 确定InstallState的CurrerntVersion，取所有产品中最新的版本作为InstallState的CurrerntVersion

                string strVersion = string.Empty;
                for (int i = 0; i < mListInstalledProducts.Count; i++)
                {
                    product = mListInstalledProducts[i];
                    if (string.IsNullOrEmpty(strVersion))
                    {
                        strVersion = product.Version;
                    }
                    var a = CompareVersion(strVersion, product.Version);
                    if (a > 0)
                    {
                        strVersion = product.Version;
                    }
                }
                mInstallState.CurrentVersion = strVersion;

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInstalledServices()
        {
            try
            {
                mListInstalledServices.Clear();
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; i < mListAllServiceInfos.Count; i++)
                {
                    var service = mListAllServiceInfos[i];
                    string serviceName = service.ServiceName;
                    var temp = services.FirstOrDefault(s => s.ServiceName == serviceName);
                    if (temp == null) { continue; }
                    service.Status = (int)temp.Status;
                    mListInstalledServices.Add(service);

                    App.WriteLog("InitServices",
                        string.Format("Install service:{0}\tStatus{1}", service.ServiceName,
                            (ServiceControllerStatus)service.Status));
                }

                App.WriteLog("InitServices", string.Format("Installed service count:{0}", mListInstalledServices.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitDatabaseInfo()
        {
            try
            {
                if (mInstallState == null) { return; }
                mInstallState.IsDatabaseCreated = false;
                mDataBaseInfo = null;
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP.Server\\Args01.UMP.xml");
                if (!File.Exists(path))
                {
                    App.WriteLog("InitDatabase", string.Format("Args01.UMP.xml not exist.\t{0}", path));
                    return;
                }
                DatabaseInfo dbInfo = new DatabaseInfo();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("DatabaseParameters");
                if (node == null)
                {
                    App.WriteLog("InitDatabase", string.Format("DatabaseParameters node not exist"));
                    return;
                }
                string strValue;
                int intValue;
                bool isSetted = false;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var isEnableAttr = temp.Attributes["P03"];
                        if (isEnableAttr != null)
                        {
                            strValue = isEnableAttr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            if (strValue != "1") { continue; }
                        }
                        var attr = temp.Attributes["P02"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.TypeID = intValue;
                            }
                        }
                        attr = temp.Attributes["P04"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            dbInfo.Host = strValue;
                        }
                        attr = temp.Attributes["P05"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.Port = intValue;
                            }
                        }
                        attr = temp.Attributes["P06"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            dbInfo.DBName = strValue;
                        }
                        attr = temp.Attributes["P07"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = App.DecryptStringM004(strValue);
                            dbInfo.LoginName = strValue;
                        }
                        attr = temp.Attributes["P08"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            dbInfo.Password = strValue;
                            strValue = App.DecryptStringM004(strValue);
                            dbInfo.RealPassword = strValue;
                        }
                        isSetted = true;
                        break;
                    }
                }
                mInstallState.IsDatabaseCreated = isSetted;
                if (!isSetted)
                {
                    App.WriteLog("InitDatabase", string.Format("Database not created."));
                    return;
                }
                mDataBaseInfo = dbInfo;
                mInstallState.DBType = dbInfo.TypeID;
                mInstallState.DBConnectionString = dbInfo.GetConnectionString();

                App.WriteLog("InitDatabase", string.Format("Read database info end.\t{0}", mDataBaseInfo));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ReadUpdateInfo()
        {
            try
            {
                mUpdateInfo = null;
                var rsUpdateInfo =
                    App.GetResourceStream(new Uri("/UMPUpdater;component/UpdateInfo.xml", UriKind.RelativeOrAbsolute));
                if (rsUpdateInfo == null)
                {
                    ShowException(string.Format("UpdateInfo.xml not exist."));
                    return;
                }
                var stream = rsUpdateInfo.Stream;
                if (stream == null)
                {
                    ShowException(string.Format("UpdateInfo.xml not exist."));
                    return;
                }
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string strContent = reader.ReadToEnd();
                OperationReturn optReturn = XMLHelper.DeserializeObject<UpdateInfo>(strContent);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                UpdateInfo updateInfo = optReturn.Data as UpdateInfo;
                if (updateInfo == null)
                {
                    ShowException(string.Format("Fail.\t UpdateInfo is null"));
                    return;
                }
                mUpdateInfo = updateInfo;
                if (mInstallState != null)
                {
                    mInstallState.UpdateVersion = mUpdateInfo.Version;
                }

                App.WriteLog("ReadUpdateInfo", string.Format("Read UpdateInfo end.\t{0}", mUpdateInfo.Name));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInstallState()
        {
            try
            {
                InstallState installState = new InstallState();
                installState.IsUMPInstalled = false;
                installState.IsDatabaseCreated = false;
                installState.IsLogined = false;

                installState.IsBackupUMP = false;
                installState.IsUpdateLang = true;
                installState.UMPBackupPath = string.Empty;
                installState.IsCompressBackup = true;
                installState.LangUpdateMode = 1;
                installState.IsSaveUpdateData = false;

                mInstallState = installState;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAppMenu()
        {
            try
            {
                ContextMenu menu = new ContextMenu();
                MenuItem item = new MenuItem();
                item.Click += AppMenuItem_Click;
                item.Header = "English ( U.S )";
                item.Tag = 1033;
                if (item.Tag.ToString() == App.LangID.ToString())
                {
                    item.IsChecked = true;
                }
                menu.Items.Add(item);
                item = new MenuItem();
                item.Click += AppMenuItem_Click;
                item.Header = "简体中文";
                item.Tag = 2052;
                if (item.Tag.ToString() == App.LangID.ToString())
                {
                    item.IsChecked = true;
                }
                menu.Items.Add(item);
                item = new MenuItem();
                item.Click += AppMenuItem_Click;
                item.Header = "繁体中文";
                item.Tag = 1028;
                if (item.Tag.ToString() == App.LangID.ToString())
                {
                    item.IsChecked = true;
                }
                menu.Items.Add(item);
                item = new MenuItem();
                item.Click += AppMenuItem_Click;
                item.Header = "日本语";
                item.Tag = 1041;
                if (item.Tag.ToString() == App.LangID.ToString())
                {
                    item.IsChecked = true;
                }
                menu.Items.Add(item);

                BtnAppMenu.ContextMenu = menu;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void ShowBasicInformation()
        {
            try
            {
                if (mInstallState == null) { return; }

                TxtTitle.Text = string.Format("UMP Updater {0}", mInstallState.UpdateVersion);
                TxtStatusCompany.Text = string.Format("{0}", ConstValue.VCT_COMPANY_LONGNAME);
                TxtStatusVersion.Text = string.Format("{0}", mInstallState.UpdateVersion);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowStepInformation()
        {
            try
            {
                switch (mCurrentStep)
                {
                    case 0:
                        UCBugInfoLister ucBugLister = new UCBugInfoLister();
                        ucBugLister.PageParent = this;
                        ucBugLister.UpdateInfo = mUpdateInfo;
                        ucBugLister.InstallState = mInstallState;
                        BorderLeftPanel.Child = ucBugLister;
                        break;
                    case 1:
                        UCLicenseAgree ucLicenseAgree = new UCLicenseAgree();
                        ucLicenseAgree.PageParent = this;
                        ucLicenseAgree.UpdateInfo = mUpdateInfo;
                        ucLicenseAgree.InstallState = mInstallState;
                        BorderLeftPanel.Child = ucLicenseAgree;
                        break;
                    case 2:
                        UCUpdateOptions ucUpdateOptions = new UCUpdateOptions();
                        ucUpdateOptions.PageParent = this;
                        ucUpdateOptions.UpdateInfo = mUpdateInfo;
                        ucUpdateOptions.InstallState = mInstallState;
                        ucUpdateOptions.ListProducts = mListInstalledProducts;
                        BorderLeftPanel.Child = ucUpdateOptions;
                        break;
                    case 3:
                        UCUpdateProgress ucUpdateProgress = new UCUpdateProgress();
                        ucUpdateProgress.PageParent = this;
                        ucUpdateProgress.UpdateInfo = mUpdateInfo;
                        ucUpdateProgress.InstallState = mInstallState;
                        ucUpdateProgress.ListProducts = mListInstalledProducts;
                        ucUpdateProgress.ListAllServices = mListAllServiceInfos;
                        ucUpdateProgress.ListInstalledServices = mListInstalledServices;
                        ucUpdateProgress.DatabaseInfo = mDataBaseInfo;
                        BorderLeftPanel.Child = ucUpdateProgress;
                        break;
                    case 4:
                        UCUpdateResult ucUpdateResult = new UCUpdateResult();
                        ucUpdateResult.PageParent = this;
                        ucUpdateResult.UpdateInfo = mUpdateInfo;
                        ucUpdateResult.InstallState = mInstallState;
                        BorderLeftPanel.Child = ucUpdateResult;
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void RefreshRightView()
        {
            try
            {
                UCRightPanelView rightView = new UCRightPanelView();
                rightView.UpdateInfo = mUpdateInfo;
                rightView.DatabaseInfo = mDataBaseInfo;
                rightView.InstallState = mInstallState;
                rightView.ListInstalledProducts = mListInstalledProducts;
                BorderRightPanel.Child = rightView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private double CompareVersion(string ver1, string ver2)
        {
            //比较两个版本号的大小，返回正表示ver2比ver1新版本
            try
            {
                string str;
                if (ver1.Contains('.'))
                {
                    str = ver1.Replace('P', '.');
                    str = str.Replace('p', '.');
                    string[] list = str.Split(new[] { '.' });
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    bool hasPack = false;
                    if (list.Length > 0)
                    {
                        a = int.Parse(list[0].Trim());
                    }
                    if (list.Length > 1)
                    {
                        b = int.Parse(list[1].Trim());
                    }
                    if (list.Length > 2)
                    {
                        c = int.Parse(list[2].Trim());
                    }
                    if (list.Length > 3)
                    {
                        d = int.Parse(list[3].Trim());
                        hasPack = true;
                    }
                    if (list.Length > 4)
                    {
                        e = int.Parse(list[4].Trim());
                    }
                    if (!hasPack)
                    {
                        str = string.Format("{0}{1}{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                    }
                    else
                    {
                        str = string.Format("{0}{1}{2}.{3}{4}", a.ToString("0"), b.ToString("00"), c.ToString("000"),
                            d.ToString("00"), e.ToString("000"));
                    }
                }
                else
                {
                    str = ver1.Replace('P', '.');
                    str = str.Replace('p', '.');
                }
                double doubleVer1 = double.Parse(str);

                if (ver2.Contains('.'))
                {
                    str = ver2.Replace('P', '.');
                    str = str.Replace('p', '.');
                    string[] list = str.Split(new[] { '.' });
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    bool hasPack = false;
                    if (list.Length > 0)
                    {
                        a = int.Parse(list[0].Trim());
                    }
                    if (list.Length > 1)
                    {
                        b = int.Parse(list[1].Trim());
                    }
                    if (list.Length > 2)
                    {
                        c = int.Parse(list[2].Trim());
                    }
                    if (list.Length > 3)
                    {
                        d = int.Parse(list[3].Trim());
                        hasPack = true;
                    }
                    if (list.Length > 4)
                    {
                        e = int.Parse(list[4].Trim());
                    }
                    if (!hasPack)
                    {
                        str = string.Format("{0}{1}{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                    }
                    else
                    {
                        str = string.Format("{0}{1}{2}.{3}{4}", a.ToString("0"), b.ToString("00"), c.ToString("000"),
                            d.ToString("00"), e.ToString("000"));
                    }
                }
                else
                {
                    str = ver2.Replace('P', '.');
                    str = str.Replace('p', '.');
                }
                double doubleVer2 = double.Parse(str);
                return doubleVer2 - doubleVer1;
            }
            catch
            {
                return -1;
            }
        }

        private string FormatVersion(string version)
        {
            //格式化版本号，将803002P01000形式转换成点分形式，如8.03.002 P01.000
            string strReturn = version;
            try
            {
                if (!strReturn.Contains('.'))
                {
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    bool hasPack = false;
                    if (strReturn.Length >= 1)
                    {
                        a = int.Parse(strReturn.Substring(0, 1));
                    }
                    if (strReturn.Length >= 3)
                    {
                        b = int.Parse(strReturn.Substring(1, 2));
                    }
                    if (strReturn.Length >= 6)
                    {
                        c = int.Parse(strReturn.Substring(3, 3));
                    }
                    if (strReturn.Length >= 7)
                    {
                        hasPack = true;
                    }
                    if (strReturn.Length >= 9)
                    {
                        d = int.Parse(strReturn.Substring(7, 2));
                    }
                    if (strReturn.Length >= 12)
                    {
                        e = int.Parse(strReturn.Substring(9, 3));
                    }
                    if (!hasPack)
                    {
                        strReturn = string.Format("{0}.{1}.{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                    }
                    else
                    {
                        strReturn = string.Format("{0}.{1}.{2} P{3}.{4}", a.ToString("0"), b.ToString("00"),
                            c.ToString("000"), d.ToString("00"), e.ToString("000"));
                    }
                }
            }
            catch { }
            return strReturn;
        }

        #endregion


        #region AppButton Handlers

        void BtnAppClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void BtnAppRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            BorderMain.Padding = new Thickness(0);
            BtnAppRestore.Visibility = Visibility.Collapsed;
            BtnAppMaximize.Visibility = Visibility.Visible;
        }

        void BtnAppMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            BorderMain.Padding = new Thickness(8);
            BtnAppRestore.Visibility = Visibility.Visible;
            BtnAppMaximize.Visibility = Visibility.Collapsed;
        }

        void BtnAppMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void BtnAppMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                if (btn == null) { return; }
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.Placement = PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;


                #region For Test

                //int langID = 2052;
                //LangLister lister = new LangLister();
                //lister.LangID = langID;
                //lister.LangName = "zh-cn";
                //LanguageInfo lang = new LanguageInfo();
                //lang.LangID = langID;
                //lang.Name = "10001";
                //lang.Display = "123123";
                //lister.ListLangInfos.Add(lang);

                //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.XML", langID));
                //OperationReturn optReturn = XMLHelper.SerializeFile(lister, path);
                //if (!optReturn.Result)
                //{
                //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //ShowInformation(string.Format("End.\t{0}", path));

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void AppMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = sender as MenuItem;
                if (item == null) { return; }
                string strLangID = item.Tag.ToString();
                int langID;
                if (!int.TryParse(strLangID, out langID)) { return; }
                App.LangID = langID;
                App.LoadAllLanguages();
                ChangeLanguage();
                InitAppMenu();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Button EventHandlers

        public void ToPrevious()
        {
            if (mCurrentStep > 0)
            {
                mCurrentStep--;
                ShowStepInformation();
            }
        }

        public void ToNext()
        {
            if (mCurrentStep < 4)
            {
                mCurrentStep++;
                ShowStepInformation();
            }
        }

        public void ToClose()
        {
            Close();
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
                WorkWaiter.Visibility = isWorking ? Visibility.Visible : Visibility.Collapsed;
                TxtStatusContent.Text = msg;
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

        #endregion


        #region ChangeLanguage

        private void ChangeLanguage()
        {
            try
            {
                var childView = BorderLeftPanel.Child as IChildView;
                if (childView != null)
                {
                    childView.ChangeLanguage();
                }
                childView = BorderRightPanel.Child as IChildView;
                if (childView != null)
                {
                    childView.ChangeLanguage();
                }
            }
            catch { }
        }

        #endregion

    }
}
