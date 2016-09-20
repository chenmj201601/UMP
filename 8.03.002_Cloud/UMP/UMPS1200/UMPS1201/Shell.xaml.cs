//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    db9719be-30b9-48ab-a7ae-b3ee6146473a
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                Shell
//
//        created by Charley at 2016/1/22 10:39:28
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using UMPS1201.Wcf11012;
using UMPS1201.Wcf12001;
using UMPS1600;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS1201
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell
    {

        #region Members

        private IModuleManager mModuleManager;
        private IEventAggregator mEventAggregator;
        private IAppControlService mAppController;
        private List<UserParamInfo> mListUserParameters;
        private List<BasicAppInfo> mListModuleInfos;
        private List<ThirdPartyAppInfo> mListThirdPartyApps;
        //private int mIMMsgCount;
        private IMMainPage mIMMainPage;

        #endregion


        public Shell(IModuleManager moduleManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
        {
            mListUserParameters = new List<UserParamInfo>();
            mListModuleInfos = new List<BasicAppInfo>();
            mListThirdPartyApps = new List<ThirdPartyAppInfo>();

            mModuleManager = moduleManager;
            mAppController = appController;
            mEventAggregator = eventAggregator;
            if (mEventAggregator != null)
            {
                mEventAggregator.GetEvent<AppCommunicationEvent>()
                    .Subscribe(AppCommunicationEvent_Published, ThreadOption.BackgroundThread);
            }

            InitializeComponent();

            Loaded += Shell_Loaded;
        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
        }


        #region Init and Load

        public void Init()
        {
            try
            {
                GridMain.MouseMove += BorderMain_MouseMove;
                GridMain.MouseDown += BorderMain_MouseDown;
                GridMain.KeyDown += BorderMain_KeyDown;

                PageName = "UMPMain";
                StylePath = "UMPS1201/Shell.xaml";
                AppServerInfo = App.Session.AppServerInfo;
                LangTypeInfo = App.Session.LangTypeInfo;
                ThemeInfo = App.Session.ThemeInfo;

                ChangeTheme();

                GridLoginPage.Visibility = Visibility.Visible;
                GridMainPage.Visibility = Visibility.Collapsed;

                if (!App.IsDBSetted)
                {
                    ShowException(App.GetLanguageInfoXml("InfoM10001", "Database not exist."));
                    return;
                }
                if (!App.IsProtocolValid)
                {
                    ShowException(App.GetLanguageInfoXml("InfoM10002", "Protocol invalid."));
                    return;
                }

                MyWaiter.Visibility = Visibility.Visible;
                //TxtLoginStatus.Text = string.Format("Loading components, please wait for a moment ...");
                TxtLoginStatus.Text = string.Empty;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadThirdPartyAppInfos();
                    //LoadCoreApps();
                    LoadLoginApp();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    try
                    {
                        LoadSingleApp(S1200Consts.MODULENAME_LOGIN);
                        //LoadSingleApp(S1200Consts.MODULENAME_PAGEHEAD);
                        //LoadSingleApp(S1200Consts.MODULENAME_STATUSBAR);
                        //LoadSingleApp(S1200Consts.MODULENAME_TASKPAGE);

                        App.WriteLog("Init", string.Format("Load login application end."));

                        MyWaiter.Visibility = Visibility.Collapsed;
                        TxtLoginStatus.Text = string.Empty;

                        ChangeTheme();
                        ChangeLanguage();

                        //NavigateApp(S1200Consts.MODULEID_DASHBOARD, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadLoginApp()
        {
            try
            {
                if (mAppController == null) { return; }
                if (App.AppConfigs == null
                    || App.AppConfigs.ListApps == null)
                {
                    App.WriteLog("LoadLoginApp", string.Format("AppConfigs is null"));
                    return;
                }
                List<AppConfigInfo> listConfigs = new List<AppConfigInfo>();
                AppConfigInfo info = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_LOGIN);
                if (info != null)
                {
                    info.Session = App.Session;
                    info.PanelName = S1200Consts.REGIONNAME_LOGIN;
                    listConfigs.Add(info);
                    var temp =
                        mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_LOGIN);
                    if (temp == null)
                    {
                        mAppController.ListAppConfigs.Add(info);
                    }
                }
                for (int i = 0; i < listConfigs.Count; i++)
                {
                    DownloadAppFiles(listConfigs[i]);
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("LoadLoginApp", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LoadCoreApps()
        {
            try
            {
                if (mAppController == null) { return; }
                if (App.AppConfigs == null
                    || App.AppConfigs.ListApps == null)
                {
                    App.WriteLog("LoadCoreApp", string.Format("AppConfigs is null"));
                    return;
                }
                List<AppConfigInfo> listConfigs = new List<AppConfigInfo>();
                AppConfigInfo info;
                //info = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_LOGIN);
                //if (info != null)
                //{
                //    info.Session = App.Session;
                //    info.PanelName = S1200Consts.REGIONNAME_LOGIN;
                //    listConfigs.Add(info);
                //    var temp =
                //        mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_LOGIN);
                //    if (temp == null)
                //    {
                //        mAppController.ListAppConfigs.Add(info);
                //    }
                //}
                info = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_PAGEHEAD);
                if (info != null)
                {
                    info.Session = App.Session;
                    info.PanelName = S1200Consts.REGIONNAME_PAGEHEAD;
                    listConfigs.Add(info);
                    var temp =
                       mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_PAGEHEAD);
                    if (temp == null)
                    {
                        mAppController.ListAppConfigs.Add(info);
                    }
                }
                info = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_STATUSBAR);
                if (info != null)
                {
                    info.Session = App.Session;
                    info.PanelName = S1200Consts.REGIONNAME_STATUSBAR;
                    listConfigs.Add(info);
                    var temp =
                       mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_STATUSBAR);
                    if (temp == null)
                    {
                        mAppController.ListAppConfigs.Add(info);
                    }
                }
                //info = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_TASKPAGE);
                //if (info != null)
                //{
                //    info.Session = App.Session;
                //    info.PanelName = S1200Consts.REGIONNAME_TASKPAGE;
                //    listConfigs.Add(info);
                //    var temp =
                //       mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == S1200Consts.MODULEID_TASKPAGE);
                //    if (temp == null)
                //    {
                //        mAppController.ListAppConfigs.Add(info);
                //    }
                //}
                for (int i = 0; i < listConfigs.Count; i++)
                {
                    DownloadAppFiles(listConfigs[i]);
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("LoadCoreApp", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LoadUserParameters()
        {
            try
            {
                mListUserParameters.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(string.Format("{0}", ConstValue.UP_DEFAULT_PAGE));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.WriteLog("LoadUserParam", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.WriteLog("LoadUserParam", string.Format("Fail.\t{0}", "ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<UserParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("LoadUserParam", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info == null)
                    {
                        App.WriteLog("LoadUserParam", string.Format("Fail.\t{0}", "UserParamInfo is null"));
                        return;
                    }
                    mListUserParameters.Add(info);
                }

                App.WriteLog("LoadUserParam", string.Format("Load end.\t{0}", mListUserParameters.Count));
            }
            catch (Exception ex)
            {
                App.WriteLog("LoadUserParam", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LoadModuleInfos()
        {
            try
            {
                mListModuleInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.GetAppBasicInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicAppInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicAppInfo info = optReturn.Data as BasicAppInfo;
                    if (info == null) { continue; }
                    mListModuleInfos.Add(info);
                }

                App.WriteLog("LoadModules", string.Format("Load end.\t{0}", mListModuleInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadThirdPartyAppInfos()
        {
            try
            {
                mListThirdPartyApps.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.GetThirdPartyAppList;
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ThirdPartyAppInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ThirdPartyAppInfo info = optReturn.Data as ThirdPartyAppInfo;
                    if (info == null) { continue; }
                    mListThirdPartyApps.Add(info);
                }

                App.WriteLog("LoadThirdParty", string.Format("Load end.\t{0}", mListModuleInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadLogicalPartTableInfos()
        {
            try
            {
                var session = App.Session;
                if (session == null) { return;}
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.GetLogicalPartTableList;
                webRequest.ListData.Add(session.UserID.ToString());
                webRequest.ListData.Add(session.RentInfo.Token);
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                int count = 0;
                if (session.ListPartitionTables == null)
                {
                    session.ListPartitionTables = new List<PartitionTableInfo>();
                }
                session.ListPartitionTables.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<PartitionTableInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    PartitionTableInfo info = optReturn.Data as PartitionTableInfo;
                    if (info == null) { continue; }
                    session.ListPartitionTables.Add(info);
                    count++;
                }

                App.WriteLog("LoadLogicalPartTables", string.Format("Load end.\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region NavigateApp

        private void NavigateMainPage()
        {
            try
            {
                GridLoginPage.Visibility = Visibility.Collapsed;
                GridMainPage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoLoginSystem(WebRequest webRequest)
        {
            try
            {
                var session = webRequest.Session;
                App.Session.SetSessionInfo(session);
                App.WriteLog("LoginSystem", string.Format("User logined"));
                App.WriteLog("LoginSystem", string.Format("{0}", App.Session.LogInfo()));

                MyWaiter.Visibility = Visibility.Visible;
                TxtLoginStatus.Text = string.Empty;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadCoreApps();
                    LoadLogicalPartTableInfos();
                    LoadUserParameters();
                    LoadModuleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;
                    TxtLoginStatus.Text = string.Empty;

                    NavigateMainPage();

                    LoadSingleApp(S1200Consts.MODULENAME_PAGEHEAD);
                    LoadSingleApp(S1200Consts.MODULENAME_STATUSBAR);

                    //CreateIMPanel();
                    //NavigateDefaultApp();

                    BackgroundWorker worker2 = new BackgroundWorker();
                    worker2.DoWork += (s2, de2) => Thread.Sleep(500);   //  这里异步延时0.5s，以便先加载PageHead
                    worker2.RunWorkerCompleted += (s2, re2) =>
                    {
                        CreateIMPanel();
                        NavigateDefaultApp();
                    };
                    worker2.RunWorkerAsync();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void NavigateLoginPage()
        {
            try
            {
                GridLoginPage.Visibility = Visibility.Visible;
                GridMainPage.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoLogoutSystem(WebRequest webRequest)
        {
            try
            {
                if (mIMMainPage != null)
                {
                    try
                    {
                        mIMMainPage.LogOff();
                    }
                    catch { }
                    mIMMainPage = null;
                }
                var panels = DockingMain.Layout.Descendents().ToList();
                for (int i = 0; i < panels.Count; i++)
                {
                    var panel = panels[i];

                    var layoutContent = panel as LayoutAnchorable;
                    if (layoutContent != null)
                    {
                        //if (layoutContent.ContentId == S1200Consts.CONTENTID_TASKPAGE) { continue; }
                        layoutContent.Hide();
                    }
                }
                NavigateLoginPage();
                if (webRequest.ListData != null
                    && webRequest.ListData.Count > 0)
                {
                    string strReason = webRequest.ListData[0];
                    if (strReason == "LR03")
                    {
                        TxtLoginStatus.Text = string.Format(App.GetLanguageInfo("S0000065", "Login timeout"));
                    }
                    if (strReason == "LR04")
                    {
                        TxtLoginStatus.Text = string.Format(App.GetLanguageInfo("S0000067", "Log on other terminal"),
                            App.Session.UserInfo.Account);
                    }
                    if (strReason == "LR05")
                    {
                        string strIdleTimeout = string.Empty;
                        if (webRequest.ListData.Count > 1)
                        {
                            strIdleTimeout = webRequest.ListData[1];
                        }
                        TxtLoginStatus.Text = string.Format(App.GetLanguageInfo("S0000066", "Idle timeout"), strIdleTimeout);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateAppDocument(AppConfigInfo appInfo)
        {
            try
            {
                if (mAppController == null) { return; }
                var app = appInfo;
                if (app == null) { return; }
                int moduleID = app.ModuleID;
                app.Session = App.Session;
                app.PanelName = string.Format("Panel{0}", app.ModuleID);
                app.AppTitle = GetAppTitle(app);
                var temp = mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == moduleID);
                if (temp == null)
                {
                    App.WriteLog("CreateAppDocument",
                        string.Format("App not in AppController, begin create.\t{0}\t{1}", app.ModuleID, app.StartArgs));

                    //模块未加载，创建并加载模块
                    mAppController.ListAppConfigs.Add(app);

                    AppContainer container = new AppContainer();
                    container.DataContext = app;
                    container.IsInited = false;
                    container.SetValue(AppContainer.AppTitleProperty, app.AppTitle);
                    container.SetValue(AppContainer.IconProperty, GetAppIcon(app));
                    container.SetValue(NameProperty, string.Format("Region{0}", app.ModuleID));
                    container.SetValue(RegionManager.RegionNameProperty, string.Format("Panel{0}", app.ModuleID));
                    container.SetResourceReference(StyleProperty, "AppContainerStyle");
                    app.Container = container;

                    LayoutAnchorable document = new LayoutAnchorable();
                    document.Title = app.AppTitle;
                    document.ContentId = app.PanelName;
                    document.Content = container;
                    BitmapImage bitMap = new BitmapImage();
                    bitMap.BeginInit();
                    bitMap.UriSource = new Uri(GetAppIcon(app), UriKind.RelativeOrAbsolute);
                    bitMap.EndInit();
                    document.IconSource = bitMap;
                    PanelDocuments.Children.Add(document);

                    ShowAppDocument(app);

                    //先下载模块的文件
                    ShowStatusMessage(true, string.Format("Loading application..."));
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (s, de) => DownloadAppFiles(app);
                    worker.RunWorkerCompleted += (s, re) =>
                    {
                        worker.Dispose();
                        ShowStatusMessage(false, string.Empty);

                        LoadSingleApp(app.ModuleName);
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    temp.StartArgs = app.StartArgs;

                    App.WriteLog("CreateAppDocument",
                       string.Format("App alreay in AppController, begin show it.\t{0}\t{1}", temp.ModuleID, temp.StartArgs));

                    var panels = DockingMain.Layout.Descendents().ToList();
                    for (int i = 0; i < panels.Count; i++)
                    {
                        var panel = panels[i];

                        var layoutContent = panel as LayoutAnchorable;
                        if (layoutContent != null
                            && layoutContent.ContentId == app.PanelName)
                        {
                            layoutContent.Title = app.AppTitle;
                        }
                    }

                    ShowAppDocument(temp);

                    App.WriteLog("CreateAppDocument",
                     string.Format("App alreay in AppController, begin renavigate it.\t{0}\t{1}", temp.ModuleID, temp.StartArgs));

                    SendReNavigateAppMessage(temp.ModuleID, temp.StartArgs);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowAppDocument(IAppConfigInfo appInfo)
        {
            try
            {
                var panel =
                   DockingMain.Layout.Descendents()
                       .OfType<LayoutAnchorable>()
                       .FirstOrDefault(p => p.ContentId == appInfo.PanelName);
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) =>
                    {

                    };
                    panel.Closing += (s, e) =>
                    {
                        var temp = appInfo;
                        CloseApp(temp);
                        e.Cancel = true;
                        var tempPanel = s as LayoutAnchorable;
                        if (tempPanel != null)
                        {
                            tempPanel.Hide();
                        }
                    };

                    panel.Show();
                    panel.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void NavigateApp(WebRequest webRequest)
        {
            try
            {
                if (webRequest.ListData == null
                    || webRequest.ListData.Count <= 0) { return; }
                string strAppID = webRequest.ListData[0];
                int intAppID;
                if (!int.TryParse(strAppID, out intAppID)) { return; }
                if (intAppID == S1200Consts.MODULEID_ASM)
                {
                    //特殊模块的特殊处理
                    //ASM打开一个新的浏览器
                    if (webRequest.ListData.Count > 1)
                    {
                        string strArgs = webRequest.ListData[1];
                        OpenThirdPartyBrower(intAppID, strArgs);
                    }
                    return;
                }
                //intAppID = 1206;
                var app = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == intAppID);
                if (app == null)
                {
                    ShowException(string.Format("Application not configed.\t{0}", intAppID));
                    return;
                }
                if (webRequest.ListData.Count > 1)
                {
                    string strArgs = webRequest.ListData[1];
                    app.StartArgs = strArgs;
                }
                if (webRequest.ListData.Count > 2)
                {
                    string strIcon = webRequest.ListData[2];
                    app.Icon = strIcon;
                }

                App.WriteLog("NavigateApp", string.Format("Navigate app.\t{0}\t{1}", app.ModuleID, app.StartArgs));

                CreateAppDocument(app);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SendReNavigateAppMessage(int appID, string startArgs)
        {
            try
            {
                WebRequest request = new WebRequest();
                request.Session = App.Session;
                request.Code = (int)RequestCode.ACTaskReNavigateApp;
                request.ListData.Add(appID.ToString());
                request.ListData.Add(startArgs);
                PublishEvent(request);
            }
            catch (Exception ex)
            {
                App.WriteLog("ReNavigateApp", string.Format("Send renavigate app message fail.\t{0}", ex.Message));
            }
        }

        private void SaveModuleUsageHistory(AppConfigInfo appInfo)
        {
            try
            {
                if (appInfo.ModuleID == S1200Consts.MODULEID_LOGIN
                    || appInfo.ModuleID == S1200Consts.MODULEID_PAGEHEAD
                    || appInfo.ModuleID == S1200Consts.MODULEID_STATUSBAR
                    || appInfo.ModuleID == S1200Consts.MODULEID_TASKPAGE
                    || appInfo.ModuleID == S1200Consts.MODULEID_DASHBOARD)
                {
                    return;
                }
                ModuleUsageInfo usageInfo = new ModuleUsageInfo();
                usageInfo.AppID = appInfo.ModuleID;
                usageInfo.ModuleID = usageInfo.AppID;
                if (!string.IsNullOrEmpty(appInfo.StartArgs))
                {
                    //某些模块，启动参数是ModuleID，此处不够严格，暂定如此
                    int moduleID;
                    if (int.TryParse(appInfo.StartArgs, out moduleID)
                        && moduleID > 1000
                        && moduleID < 10000)
                    {
                        usageInfo.ModuleID = moduleID;
                    }
                }
                usageInfo.UserID = App.Session.UserID;
                usageInfo.BeginTime = DateTime.Now.ToUniversalTime();
                usageInfo.SessionID = appInfo.SessionID;
                usageInfo.StartArgs = appInfo.StartArgs;
                usageInfo.HostName = Environment.MachineName;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(usageInfo);
                if (!optReturn.Result)
                {
                    App.WriteLog("SaveModuleUsage", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.SetAppUsageInfo;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service12001Client client = new Service12001Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.WriteLog("SaveModuleUsage", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                App.WriteLog("SaveModuleUsage", string.Format("Save end.\t{0}", appInfo.ModuleID));
            }
            catch (Exception ex)
            {
                App.WriteLog("SaveModuleUsage", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void NavigateDefaultApp()
        {
            try
            {
                var param =
                    mListUserParameters.FirstOrDefault(
                        p => p.ParamID == ConstValue.UP_DEFAULT_PAGE && p.UserID == App.Session.UserID);
                int appID = S1200Consts.MODULEID_DASHBOARD;
                int moduleID = appID;
                string strArgs = string.Empty;
                string strIcon = @"Images\S0000\S0000019.png";
                if (param != null)
                {
                    string strValue = param.ParamValue;
                    if (strValue.Length > 20)
                    {
                        string strModuleID = strValue.Substring(20);
                        int temp;
                        if (int.TryParse(strModuleID, out temp))
                        {
                            moduleID = temp;
                        }
                    }
                    var moduleInfo = mListModuleInfos.FirstOrDefault(m => m.ModuleID == moduleID);
                    if (moduleInfo != null)
                    {
                        appID = moduleInfo.AppID;
                        strArgs = moduleInfo.Args;
                        strIcon = moduleInfo.Icon;
                    }
                }
                AppConfigInfo app = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == appID);
                if (app == null)
                {
                    ShowException(string.Format("Application not configed.\t{0}", appID));
                    return;
                }
                app.StartArgs = strArgs;
                app.Icon = strIcon;

                App.WriteLog("NavigateApp", string.Format("Begin navigate app.\t{0}\t{1}", app.ModuleID, app.StartArgs));

                CreateAppDocument(app);
            }
            catch (Exception ex)
            {
                App.WriteLog("NavigateApp", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private void OpenThirdPartyBrower(int moduleID, string strArgs)
        {
            try
            {
                App.WriteLog("OpenThirdParty", string.Format("Module:{0};Args:{1}", moduleID, strArgs));
                switch (moduleID)
                {
                    case S1200Consts.MODULEID_ASM:
                        var thirdApp = mListThirdPartyApps.FirstOrDefault(a => a.Name == S1200Consts.THIRDPARTY_APP_NAME_ASM);
                        if (thirdApp == null) { return; }
                        OperationReturn optReturn = GetUserOrgInfo();
                        if (!optReturn.Result)
                        {
                            App.WriteLog("OpenThirdParty",
                                string.Format("Get UserOrgInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        OrgInfo orgInfo = optReturn.Data as OrgInfo;
                        if (orgInfo == null)
                        {
                            App.WriteLog("OpenThirdParty", string.Format("OrgInfo is null"));
                            return;
                        }
                        strArgs = string.Format(strArgs,
                            App.EncryptString(App.Session.UserInfo.Account),
                            App.EncryptString(App.Session.UserID.ToString()),
                            App.EncryptString(App.Session.LangTypeID.ToString()),
                            App.EncryptString(App.Session.RoleID.ToString()),
                            App.EncryptString(orgInfo.OrgID.ToString()),
                            App.EncryptString(App.Session.UserInfo.Password));
                        string strUrl = string.Format("{0}{1}:{2}{3}",
                             thirdApp.Protocol,
                             thirdApp.HostAddress == "127.0.0.1" ? App.Session.AppServerInfo.Address : thirdApp.HostAddress,
                             thirdApp.HostPort,
                             strArgs);
                        App.WriteLog("OpenThirdParty", string.Format("Browser url:{0}", strUrl));
                        Process process = new Process();
                        process.StartInfo.FileName = "iexplore.exe";
                        process.StartInfo.Verb = "open";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        process.StartInfo.Arguments = strUrl;
                        process.Start();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn GetUserOrgInfo()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add("-1");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListData is null");
                    return optReturn;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("OrgInfo not exist");
                    return optReturn;
                }
                string strInfo = webReturn.ListData[0];
                optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ResourceObject obj = optReturn.Data as ResourceObject;
                if (obj == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ResourceObject is null");
                    return optReturn;
                }
                OrgInfo orgInfo = new OrgInfo();
                orgInfo.OrgID = obj.ObjID;
                orgInfo.Name = obj.Name;
                orgInfo.Description = obj.FullName;
                optReturn.Data = orgInfo;
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

        private void ShowStatusMessage(bool isWorking, string msg)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.ACStatusSetStatus;
                webRequest.ListData.Add(isWorking ? "1" : "0");
                webRequest.ListData.Add(msg);

                if (mEventAggregator != null)
                {
                    mEventAggregator.GetEvent<CompositePresentationEvent<WebRequest>>().Publish(webRequest);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string ParseAppEventCode(int code)
        {
            string strReturn = code.ToString();
            try
            {
                strReturn = ((RequestCode)code).ToString();
            }
            catch { }
            return strReturn;
        }

        private void SetAppLoaded(WebRequest webRequest)
        {
            try
            {
                var session = webRequest.Session;
                if (session == null) { return; }
                string strSessionID = session.SessionID;
                int intModuleID = session.ModuleID;
                if (string.IsNullOrEmpty(strSessionID)
                    || intModuleID < 0) { return; }
                var app = mAppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == intModuleID);
                if (app == null) { return; }
                var appInfo = app as AppConfigInfo;
                if (appInfo != null)
                {
                    SaveModuleUsageHistory(appInfo);
                }
                if (app.Container != null)
                {
                    var container = app.Container as AppContainer;
                    if (container != null)
                    {
                        container.IsInited = true;

                        App.WriteLog("ModuleLoad", string.Format("Module Loaded.\t{0}\t{1}", app.AppName, strSessionID));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeTheme(WebRequest webRequest)
        {
            try
            {
                var themeName = webRequest.Data;
                if (!string.IsNullOrEmpty(themeName))
                {
                    var session = App.Session;
                    if (session == null) { return; }
                    var themeInfo = session.SupportThemes.FirstOrDefault(t => t.Name == themeName);
                    if (themeInfo == null) { return; }
                    ThemeInfo = themeInfo;
                    session.ThemeInfo = themeInfo;
                    session.ThemeName = themeInfo.Name;
                    ChangeTheme();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeLanguage(WebRequest webRequest)
        {
            try
            {
                var strLangID = webRequest.Data;
                int langID;
                if (!int.TryParse(strLangID, out langID)) { return; }
                var session = App.Session;
                if (session == null) { return; }
                var langTypeInfo = session.SupportLangTypes.FirstOrDefault(t => t.LangID == langID);
                if (langTypeInfo == null) { return; }
                LangTypeInfo = langTypeInfo;
                session.LangTypeInfo = langTypeInfo;
                session.LangTypeID = langTypeInfo.LangID;
                App.InitAllLanguages();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void PublishEvent(WebRequest webRequest)
        {
            try
            {
                if (mEventAggregator != null)
                {
                    mEventAggregator.GetEvent<AppCommunicationEvent>().Publish(webRequest);
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("PublishEvent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private string GetAppIcon(AppConfigInfo app)
        {
            string strReturn = string.Empty;
            try
            {
                string strIcon = app.Icon;
                strReturn = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                    App.Session.AppServerInfo.Protocol,
                    App.Session.AppServerInfo.Address,
                    App.Session.AppServerInfo.Port,
                    App.Session.ThemeName,
                    strIcon);
            }
            catch (Exception ex)
            {
                App.WriteLog("GetAppIcon", string.Format("Fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        private string GetAppTitle(AppConfigInfo app)
        {
            string strReturn = string.Empty;
            try
            {
                strReturn = app.AppTitle;
                //某些模块，启动参数是ModuleID，此处不够严格，暂定如此
                int moduleID;
                if (int.TryParse(app.StartArgs, out moduleID)
                    && moduleID > 1000
                    && moduleID < 10000)
                {
                    strReturn = App.GetLanguageInfo(string.Format("FO{0}", moduleID), strReturn);
                }
                else
                {
                    strReturn = App.GetLanguageInfo(string.Format("FO{0}", app.ModuleID), strReturn);
                }
                return strReturn;
            }
            catch
            {
                return strReturn;
            }
        }

        #endregion


        #region Operation

        public void Close()
        {
            try
            {
                if (mIMMainPage != null)
                {
                    try
                    {
                        mIMMainPage.LogOff();
                    }
                    catch { }
                    mIMMainPage = null;
                }
                for (int i = 0; i < mAppController.ListAppConfigs.Count; i++)
                {
                    var appConfig = mAppController.ListAppConfigs[i];
                    CloseApp(appConfig);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CloseApp(IAppConfigInfo appInfo)
        {
            try
            {
                var app = appInfo.UMPApp as UMPApp;
                if (app != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)RequestCode.CSModuleClose;
                    app.DoAppCommand(webRequest);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoSetDefaultPage()
        {
            try
            {
                var document = PanelDocuments.SelectedContent as LayoutAnchorable;
                if (document == null) { return; }
                var app = mAppController.ListAppConfigs.FirstOrDefault(a => a.PanelName == document.ContentId);
                if (app == null) { return; }
                int moduleID = app.ModuleID;
                if (!string.IsNullOrEmpty(app.StartArgs))
                {
                    //某些模块，启动参数是ModuleID，此处不够严格，暂定如此
                    int temp;
                    if (int.TryParse(app.StartArgs, out temp)
                        && temp > 1000
                        && temp < 10000)
                    {
                        moduleID = temp;
                    }
                }

                string strUserID = App.Session.UserID.ToString();
                string strModule = moduleID.ToString();
                string strParamValue = string.Format("{0}{1}{2}", strUserID, ConstValue.SPLITER_CHAR_3, strModule);
                UserParamInfo param = new UserParamInfo();
                param.UserID = App.Session.UserID;
                param.ParamID = ConstValue.UP_DEFAULT_PAGE;
                param.GroupID = ConstValue.UP_GROUP_PAGE;
                param.SortID = 0;
                param.DataType = DBDataType.NVarchar;
                param.ParamValue = strParamValue;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(param);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSSaveUserParamList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session)
                    , WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                MessageBox.Show(App.GetLanguageInfo("COMN006", "Set default page successful!"), App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoNavigateHome()
        {
            try
            {
                int intAppID = S1200Consts.MODULEID_DASHBOARD;
                var app = App.AppConfigs.ListApps.FirstOrDefault(a => a.ModuleID == intAppID);
                if (app == null)
                {
                    ShowException(string.Format("Application not configed.\t{0}", intAppID));
                    return;
                }
                app.StartArgs = string.Empty;
                app.Icon = @"Images\S0000\S0000019.png";
                CreateAppDocument(app);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangePassword()
        {
            try
            {
                UCChangePassword uc = new UCChangePassword();
                uc.PageParent = this;
                uc.LoginReturnCode = "S01A01";
                PopupPanel.Title = App.GetLanguageInfo("S0000030", "Change password");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeRole()
        {
            try
            {
                UCChangeRole uc = new UCChangeRole();
                uc.PageParent = this;
                PopupPanel.Title = App.GetLanguageInfo("S0000045", "Select a role");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnChangePasswordResult(string strResult)
        {
            try
            {
                PopupPanel.IsOpen = false;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnChangeRoleResult(string strResult)
        {
            try
            {
                PopupPanel.IsOpen = false;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoGlobalSettingChanged(WebRequest webRequest)
        {
            try
            {
                if (webRequest.ListData == null
                    || webRequest.ListData.Count <= 1) { return; }
                string strName = webRequest.ListData[0];
                switch (strName)
                {
                    case ConstValue.GS_KEY_PARAM_ROLE:
                        //切换角色，关闭所有页面，打开默认页
                        var panels = DockingMain.Layout.Descendents().ToList();
                        for (int i = 0; i < panels.Count; i++)
                        {
                            var panel = panels[i];

                            var layoutContent = panel as LayoutAnchorable;
                            if (layoutContent != null)
                            {
                                layoutContent.Hide();
                            }
                        }
                        //如果是登录时发送的切换角色消息，不需要导航到默认页，否则需要切换到默认页
                        if (webRequest.ListData.Count > 3)
                        {
                            string strType = webRequest.ListData[3];
                            if (strType == "1")
                            {
                                break;
                            }
                        }
                        NavigateDefaultApp();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region IM Window

        private void CreateIMPanel()
        {
            try
            {
                var session = App.Session;
                if (session == null) { return; }

                //IM权限panding
                var module = mListModuleInfos.FirstOrDefault(m => m.ModuleID == S1200Consts.MODULEID_IM);
                if (module == null) { return; }

                mIMMainPage = new IMMainPage(session);
                mIMMainPage.StatusChangeEvent += IMMainPage_StatusChangeEvent;
                mIMMainPage.Width = 780;
                mIMMainPage.Height = 500;
                mIMMainPage.Login();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenIMPanel()
        {
            try
            {
                var session = App.Session;
                if (session == null) { return; }
                if (mIMMainPage == null)
                {
                    CreateIMPanel();
                }

                if (IMPanel == null) { return; }
                if (mIMMainPage == null) { return; }
                IMPanel.Title = string.Format("{0}", session.UserInfo.Account);
                IMPanel.Width = 800;
                IMPanel.Height = 550;
                IMPanel.IsModal = false;
                IMPanel.Content = mIMMainPage;
                IMPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetIMMsgState(int type, string strCount)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.ACPageHeadSetIMState;
                webRequest.ListData.Add(type.ToString());
                webRequest.ListData.Add(strCount);
                PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void IMMainPage_StatusChangeEvent(int msgType, string strMsg)
        {
            App.WriteLog("IMMessage", string.Format("{0}\t{1}", msgType, strMsg));
            switch (msgType)
            {
                case IM_STATUS_LOGIN:
                    SetIMMsgState(IM_STATUS_LOGIN, strMsg);
                    break;
                case IM_STATUS_LOGOFF:
                    SetIMMsgState(IM_STATUS_LOGOFF, strMsg);
                    break;
                case IM_STATUS_NEWMSG:
                    SetIMMsgState(IM_STATUS_NEWMSG, strMsg);
                    break;
            }
        }

        //IM Chart 事件代码
        private const int IM_STATUS_LOGIN = 1;
        private const int IM_STATUS_LOGOFF = 2;
        private const int IM_STATUS_NEWMSG = 3;

        private void DoOpenIMPanel(WebRequest webRequest)
        {
            try
            {
                OpenIMPanel();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region 定时Online消息

        private DateTime mLastActiveTime;

        private void SetOnline()
        {
            try
            {
                DateTime now = DateTime.Now;
                var diff = now - mLastActiveTime;

                if (diff.TotalMilliseconds > 5000)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)RequestCode.ACLoginOnline;
                    PublishEvent(webRequest);
                    mLastActiveTime = now;
                }
            }
            catch (Exception ex)
            {

            }
        }

        void BorderMain_KeyDown(object sender, KeyEventArgs e)
        {
            SetOnline();
        }

        void BorderMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetOnline();
        }

        void BorderMain_MouseMove(object sender, MouseEventArgs e)
        {
            SetOnline();
        }

        #endregion


        #region Load Application

        private void DownloadAppFiles(AppConfigInfo appInfo)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.GetAppFileList;
                webRequest.ListData.Add(appInfo.ModuleID.ToString());
                Service12001Client client = new Service12001Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    App.WriteLog("DownloadApp", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.WriteLog("DownloadApp", string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                string strDirName = webReturn.Message;
                appInfo.Version = strDirName;
                string strAppDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    string.Format("Apps\\UMPS{0}", appInfo.ModuleID));
                string strAppInfoFile = Path.Combine(strAppDir, AppConfigInfo.FILE_NAME);
                if (File.Exists(strAppInfoFile))
                {
                    optReturn = XMLHelper.DeserializeFile<AppConfigInfo>(strAppInfoFile);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("DownloadApp",
                            string.Format("Load AppInfoFile fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    var existAppInfo = optReturn.Data as AppConfigInfo;
                    if (existAppInfo == null)
                    {
                        App.WriteLog("DownloadApp",
                          string.Format("Load AppInfoFile fail.\t AppConfigInfo is null"));
                        return;
                    }
                    if (existAppInfo.Version == strDirName)
                    {
                        App.WriteLog("DownloadApp",
                         string.Format("AppFiles already exist."));
                        return;
                    }
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    var str = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<AppFileInfo>(str);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("DownloadApp",
                          string.Format("Load AppInfoFile fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AppFileInfo appFile = optReturn.Data as AppFileInfo;
                    if (appFile == null)
                    {
                        App.WriteLog("DownloadApp",
                            string.Format("AppFileInfo is null"));
                        return;
                    }
                    DownloadAppFile(appInfo.ModuleID, strDirName, appFile);
                }
                if (!Directory.Exists(strAppDir))
                {
                    Directory.CreateDirectory(strAppDir);
                }
                optReturn = XMLHelper.SerializeFile(appInfo, strAppInfoFile);
                if (!optReturn.Result)
                {
                    App.WriteLog("DownloadApp",
                      string.Format("Save AppInfoFile fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }

                App.WriteLog("DownloadApp",
                    string.Format("Download AppFiles end.\t{0}\t{1}", appInfo.ModuleID, appInfo.StartArgs));
            }
            catch (Exception ex)
            {
                App.WriteLog("DownloadApp", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DownloadAppFile(int moduleID, string dirName, AppFileInfo appFile)
        {
            try
            {
                string strPath = string.Format("Application Files\\{0}\\{1}", dirName, appFile.Name);
                string saveDir = string.Format("Apps\\UMPS{0}", moduleID);
                saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveDir);
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                string saveName = Path.Combine(saveDir, appFile.Name);
                DownloadConfig config = new DownloadConfig();
                config.Method = App.Session.AppServerInfo.SupportHttps ? 2 : 1;
                config.Host = App.Session.AppServerInfo.Address;
                config.Port = App.Session.AppServerInfo.Port;
                config.IsReplace = true;
                config.RequestPath = strPath;
                config.SavePath = saveName;
                OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    App.WriteLog("DownloadApp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                if (!File.Exists(saveName))
                {
                    App.WriteLog("DownloadApp", string.Format("Fail.\tFile not exist.\t{0}", saveName));
                    return;
                }
                FileInfo fileInfo = new FileInfo(saveName);
                fileInfo.LastWriteTime = appFile.ModifyTime;

                //App.WriteLog("DownloadApp", string.Format("Download file end.\t{0} \t{1}", strPath, saveName));
            }
            catch (Exception ex)
            {
                App.WriteLog("DownloadApp", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LoadSingleApp(string appName)
        {
            try
            {
                mModuleManager.LoadModule(appName);

                App.WriteLog("LoadApp", string.Format("Load single app end.\t{0}", appName));
            }
            catch (Exception ex)
            {
                var e = ex as ModuleTypeLoadingException;
                if (e != null)
                {
                    var temp = e.GetRootException();

                    App.WriteLog("Test", temp.ToString());
                }
                App.WriteLog("LoadApp", string.Format("Load single app fail.\t{0}\t{1}", appName, ex.Message));
            }
        }

        #endregion


        #region Event Handlers

        private void AppCommunicationEvent_Published(WebRequest webRequest)
        {
            //App.WriteLog("AppEvent", string.Format("Code:{0}", ParseAppEventCode(webRequest.Code)));

            int code = webRequest.Code;
            switch (code)
            {
                case (int)RequestCode.ACLoginLoginSystem:
                    Dispatcher.Invoke(new Action(() => DoLoginSystem(webRequest)));
                    break;
                case (int)RequestCode.ACLoginLogout:
                    Dispatcher.Invoke(new Action(() => DoLogoutSystem(webRequest)));
                    break;
                case (int)RequestCode.ACTaskNavigateApp:
                    Dispatcher.Invoke(new Action(() => NavigateApp(webRequest)));
                    break;
                case (int)RequestCode.CSModuleLoaded:
                    Dispatcher.Invoke(new Action(() => SetAppLoaded(webRequest)));
                    break;
                case (int)RequestCode.CSThemeChange:
                    Dispatcher.Invoke(new Action(() => DoChangeTheme(webRequest)));
                    break;
                case (int)RequestCode.CSLanguageChange:
                    Dispatcher.Invoke(new Action(() => DoChangeLanguage(webRequest)));
                    break;
                case (int)RequestCode.ACPageHeadDefaultPage:
                    Dispatcher.Invoke(new Action(DoSetDefaultPage));
                    break;
                case (int)RequestCode.CSHome:
                    Dispatcher.Invoke(new Action(DoNavigateHome));
                    break;
                case (int)RequestCode.ACPageHeadOpenIMPanel:
                    Dispatcher.Invoke(new Action(() => DoOpenIMPanel(webRequest)));
                    break;
                case (int)RequestCode.CSChangePassword:
                    Dispatcher.Invoke(new Action(DoChangePassword));
                    break;
                case (int)RequestCode.CSRoleChange:
                    Dispatcher.Invoke(new Action(DoChangeRole));
                    break;
                case (int)RequestCode.SCGlobalSettingChanged:
                    Dispatcher.Invoke(new Action(() => DoGlobalSettingChanged(webRequest)));
                    break;
            }
        }

        #endregion


        #region ChangeTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name,
                        StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
                        "Default",
                        StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源文件
            try
            {
                string uri = string.Format("/Themes/Default/UMPS1201/ShellAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);

                uri = string.Format("/Themes/Default/UMPS1201/ShellStatic.xaml");
                resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("2" + ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                App.AppTitle = "UMP Main";

                for (int i = 0; i < mAppController.ListAppConfigs.Count; i++)
                {
                    var app = mAppController.ListAppConfigs[i];
                    var temp = app as AppConfigInfo;
                    if (temp != null)
                    {
                        app.AppTitle = GetAppTitle(temp);
                    }

                    var panel =
                        DockingMain.Layout.Descendents()
                            .OfType<LayoutAnchorable>()
                            .FirstOrDefault(p => p.ContentId == app.PanelName);
                    if (panel != null)
                    {
                        panel.Title = app.AppTitle;
                    }
                }
            }
            catch (Exception ex) { }
        }

        #endregion

    }
}
