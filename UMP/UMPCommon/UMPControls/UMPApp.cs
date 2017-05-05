//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8cd98d7b-5f3a-4720-a5f7-0c9f7cd3fe94
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPApp
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPApp
//
//        created by Charley at 2015/5/4 13:51:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 封装UMP的Application，实现如下功能
    /// 1、SessionInfo初始化与加载
    /// 2、应用程序在线监视
    /// 3、NetPipe创建，发送与接收消息
    /// 3、写运行日志
    /// 4、写操作日志
    /// 5、其他基本操作（抛出消息框，获取显示语言等）
    /// 
    /// UMP的应用程序可继承此类，
    /// 首先在可重写的保护方法SetAppInfo中设置AppName和ModuleID（必须）
    /// 在可重写的保护方法Init中追加其他需要在应用程序启动后执行的操作（即Init代替了原OnStartup）
    /// 注意与base.Init()的先后顺序，一般写在base.Init()之后
    /// 尽量不要重写OnStartup
    /// </summary>
    public class UMPApp : IModule
    {

        public UMPApp(bool runAsModule)
        {
            ListLanguageInfos = new List<LanguageInfo>();
            MonitorHelper = new LocalMonitorHelper();
            RunAsModule = runAsModule;
            Current = this;
        }

        public UMPApp(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : this(true)
        {
            RegionManager = regionManager;
            EventAggregator = eventAggregator;
            AppController = appController;
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<AppCommunicationEvent>()
                    .Subscribe(AppCommunicationEvent_Published, ThreadOption.BackgroundThread);
            }
        }


        #region Members

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string AppName = string.Empty;
        /// <summary>
        /// 应用程序标题
        /// </summary>
        public string AppTitle = string.Empty;
        /// <summary>
        /// 应用程序所属的模块编码（4位小模块编号）
        /// </summary>
        public int ModuleID = 0;
        /// <summary>
        /// 应用终端类型
        /// </summary>
        public int AppType = (int)Common.AppType.Unkown;
        /// <summary>
        /// 临时目录位置，临时目录通常位于公共ApplicationData下的UMP目录
        /// 可放运行日志，记住的信息，布局等临时文件
        /// </summary>
        public string TempPath;
        /// <summary>
        /// 每个应用程序启动后都会生成一个SessionInfo对象，其中包含一些全局信息
        /// </summary>
        public SessionInfo Session;
        /// <summary>
        /// 负责处理与主模块通许
        /// </summary>
        public NetPipeHelper NetPipeHelper;
        /// <summary>
        /// 负责处理监视对象的管理
        /// </summary>
        public LocalMonitorHelper MonitorHelper;
        /// <summary>
        /// 显示语言信息
        /// </summary>
        public List<LanguageInfo> ListLanguageInfos;
        /// <summary>
        /// 发送LoadingMessage消息后的返回值
        /// 通常此返回值包含App需要在页面加载完成后自动执行的操作
        /// 用于主模块控制子模块自动执行某操作
        /// </summary>
        public string StartArgs;

        //Compsite 相关
        public IRegionManager RegionManager;
        public IEventAggregator EventAggregator;
        public IAppControlService AppController;
        /// <summary>
        /// 是否以模块运行，默认以独立的应用程序运行
        /// </summary>
        public bool RunAsModule = false;
        /// <summary>
        /// 当前应用程序
        /// </summary>
        public UMPApp Current;

        /// <summary>
        /// 当前加载的视图
        /// </summary>
        public UMPUserControl CurrentView;

        private LogOperator mLogOperator;

        #endregion


        #region Operation

        public virtual void Startup()
        {
            //一般派生的子类不要重写此方法
            SetAppInfo();
            Init();
            //加载视图
            SetView();
            InitView();
            WriteLog("AppLoad", string.Format("Application loaded.\t{0}", AppName));
        }

        public virtual void Exit()
        {
            RemoveView();
            if (NetPipeHelper != null)
            {
                NetPipeHelper.Stop();
                NetPipeHelper = null;
            }
            WriteLog("AppExit", string.Format("App ended."));
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
        }

        public virtual OperationReturn DoAppCommand(WebRequest webRequest)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                int code = webRequest.Code;
                switch (code)
                {
                    case (int)RequestCode.CSModuleClose:
                        Exit();
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private void DealRenavigateApp(WebRequest webRequest)
        {
            try
            {
                if (webRequest.ListData == null || webRequest.ListData.Count < 1) { return; }
                string strAppID = webRequest.ListData[0];
                if (!strAppID.Equals(ModuleID.ToString())) { return; }
                Exit();
                Init();
                //加载视图
                SetView();
                InitView();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Init and Load
        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            SetAppInfo();
            Init();
            //加载视图
            SetView();
            InitView();
            WriteLog("AppLoad", string.Format("Application loaded.\t{0}", AppName));
        }
        /// <summary>
        /// 设置AppName及ModuleID
        /// </summary>
        protected virtual void SetAppInfo()
        {
            AppName = "Default";
            AppTitle = "Default";
            ModuleID = 0;
            AppType = (int)Common.AppType.Unkown;
        }
        /// <summary>
        /// 应用程序启动时调用此方法，处理在启动时需要执行的操作
        /// 代替OnStartup
        /// </summary>
        protected virtual void Init()
        {
            try
            {
                TempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}", AppName));
                CreateLogOperator();
                WriteLog("AppLoad", string.Format("App starting..."));
                InitSessionInfo();
                LoadSessionInfo();
                CreateNetPipeService();
                SendLoadingMessage();
                if (RunAsModule)
                {
                    var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
                    if (app != null)
                    {
                        StartArgs = app.StartArgs;
                        WriteLog("AppLoad", string.Format("Version:{0};StartArgs:{1}", app.Version, StartArgs));

                        if (app.Session != null)
                        {
                            Session.SetSessionInfo(app.Session);
                            app.UMPApp = Current;
                            app.Update(Session);

                            WriteLog("AppLoad", string.Format("SessionInfo updated.\t{0}", Session));
                        }
                    }
                }
                if (Session != null)
                {
                    MonitorHelper.IsRememberObject = Session.IsMonitor;
                    WriteLog("AppLoad", string.Format("{0}", Session));
                    WriteLog("AppLoad", string.Format("{0}", Session.LogInfo()));
                }

                InitLanguageInfos();

            }
            catch (Exception ex)
            {
                WriteLog("AppLoad", string.Format("Init fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 设置视图CurrentView
        /// </summary>
        protected virtual void SetView()
        {
            //CurrentView=view;
        }
        /// <summary>
        /// 初始化视图
        /// </summary>
        protected virtual void InitView()
        {
            try
            {
                var view = CurrentView;
                view.CurrentApp = Current;
                if (RunAsModule
                    && CurrentView != null)
                {
                    var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
                    if (app != null)
                    {
                        if (RegionManager.Regions[app.PanelName].Views != null)
                        {
                            foreach (var temp in RegionManager.Regions[app.PanelName].Views)
                            {
                                try
                                {
                                    RegionManager.Regions[app.PanelName].Remove(temp);
                                }
                                catch { }
                            }
                        }
                        if (!string.IsNullOrEmpty(view.PageName))
                        {
                            RegionManager.Regions[app.PanelName].Add(view, view.PageName);

                            WriteLog("InitView", string.Format("Init View.\t{0}", view.PageName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("InitView", string.Format("Init fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 用于切换页面，加载CurrentView指定的View
        /// </summary>
        public void InitCurrentView()
        {
            InitView();
        }
        /// <summary>
        /// 初始化SessionInfo，如果应用程序单机运行（不在主模块中运行），此处可设置SessionInfo的默认值
        /// </summary>
        protected virtual void InitSessionInfo()
        {
            Session = SessionInfo.CreateSessionInfo(AppName, ModuleID, AppType);
            Session.InstallPath = @"C:\UMPRelease";

            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            Session.RentInfo = rentInfo;
            Session.RentID = ConstValue.RENT_DEFAULT;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            userInfo.Password = "voicecyber";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;
            Session.RoleID = ConstValue.ROLE_SYSTEMADMIN;

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            Session.ThemeInfo = themeInfo;
            Session.ThemeName = "Default";

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            Session.SupportThemes.Add(themeInfo);

            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            Session.SupportLangTypes.Add(langType);
            Session.LangTypeInfo = langType;
            Session.LangTypeID = langType.LangID;

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "zh-cn";
            langType.Display = "繁体中文";
            Session.SupportLangTypes.Add(langType);
        }
        /// <summary>
        /// 从xml文件加载SessionInfo
        /// </summary>
        protected virtual void LoadSessionInfo()
        {
            try
            {
                string path = Path.Combine(TempPath, "umpsession.xml");
                if (!File.Exists(path))
                {
                    WriteLog("AppLoad", string.Format("umpsession.xml not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<SessionInfo>(path);
                if (!optReturn.Result)
                {
                    WriteLog("AppLoad",
                        string.Format("Load SessionInfo from xml file fail.\t{0}\t{1}", optReturn.Code,
                            optReturn.Message));
                    return;
                }
                SessionInfo session = optReturn.Data as SessionInfo;
                if (session == null)
                {
                    WriteLog("AppLoad",
                       string.Format("Load SessionInfo from xml file fail.\tSessionInfo is null"));
                    return;
                }
                Session.SetSessionInfo(session);
                Session.IsMonitor = session.IsMonitor;
                WriteLog("AppLoad",
                      string.Format("Load SessionInfo from xml file end."));
            }
            catch (Exception ex)
            {
                WriteLog("AppLoad", string.Format("Load SessionInfo from xml file fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 加载语言信息
        /// </summary>
        protected virtual void InitLanguageInfos()
        {
            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfos.Add(langInfo);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }
        /// <summary>
        /// 重新加载语言信息
        /// </summary>
        public void InitAllLanguageInfos()
        {
            InitLanguageInfos();
        }

        #endregion


        #region Basic

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(TempPath, "Logs");
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
                mLogOperator.WriteLog(LogMode.Info, AppName, msg);
            }
        }
        /// <summary>
        /// 弹出异常消息框
        /// </summary>
        /// <param name="msg">消息内容</param>
        public void ShowExceptionMessage(string msg)
        {
            //MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, AppTitle, MessageBoxButton.OK, MessageBoxImage.Error));
        }
        /// <summary>
        /// 弹出提示消息框
        /// </summary>
        /// <param name="msg">消息内容</param>
        public void ShowInfoMessage(string msg)
        {
            //MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Information);
            ThreadPool.QueueUserWorkItem(
                a => MessageBox.Show(msg, AppTitle, MessageBoxButton.OK, MessageBoxImage.Information));
        }


        #endregion


        #region View Operation

        /// <summary>
        /// 模块退出时需要移除视图
        /// </summary>
        protected virtual void RemoveView()
        {
            try
            {
                var view = CurrentView;
                if (view == null) { return; }
                if (AppController != null)
                {
                    var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
                    if (app != null)
                    {
                        if (RegionManager.Regions[app.PanelName].Views != null)
                        {
                            foreach (var temp in RegionManager.Regions[app.PanelName].Views)
                            {
                                try
                                {
                                    RegionManager.Regions[app.PanelName].Remove(temp);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("RemoveView", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Language

        /// <summary>
        /// 获取显示语言
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="display">显示语言</param>
        /// <returns></returns>
        public string GetLanguageInfo(string name, string display)
        {
            try
            {
                LanguageInfo lang =
                    ListLanguageInfos.FirstOrDefault(l => l.LangID == Session.LangTypeInfo.LangID && l.Name == name);
                if (lang == null)
                {
                    return display;
                }
                return lang.Display;
            }
            catch (Exception ex)
            {
                WriteLog("GetLang", string.Format("GetLang fail.\t{0}", ex.Message));
                return display;
            }
        }
        /// <summary>
        /// 获取提示显示语言
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="display">显示语言</param>
        /// <returns></returns>
        public string GetMessageLanguageInfo(string name, string display)
        {
            name = string.Format("{0}N{1}", ModuleID, name);
            return GetLanguageInfo(name, display);
        }

        #endregion


        #region NetPipe

        /// <summary>
        /// 接收到NetPipe消息时触发此事件
        /// </summary>
        public event Action<WebRequest> NetPipeEvent;

        private void OnNetPipeEvent(WebRequest webRequest)
        {
            if (NetPipeEvent != null)
            {
                NetPipeEvent(webRequest);
            }

            var current = Current;
            if (current != null)
            {
                current.OnDealNetPipeEvent(webRequest);
            }
        }

        protected virtual void OnDealNetPipeEvent(WebRequest webRequest)
        {
            try
            {
                AppCommunicationEvent_Published(webRequest);
            }
            catch (Exception ex)
            {
                WriteLog("NetPipe", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void CreateNetPipeService()
        {
            NetPipeHelper = new NetPipeHelper(false, Session.SessionID);
            NetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                NetPipeHelper.Start();
                WriteLog("NetPipe", string.Format("NetPipe service created.\t{0}", Session.SessionID));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                var code = arg.Code;
                var strData = arg.Data;
                WriteLog("NetPipe", string.Format("RecieveMessage\t{0}\t{1}", code, strData));
                switch (code)
                {
                    //主模块修改当前登录用户的密码时，要同步修改子模块的用户密码
                    case (int)RequestCode.SCChangePassword:
                        string strNewPassword = strData;
                        if (Session != null
                            && Session.UserInfo != null)
                        {
                            Session.UserInfo.Password = strNewPassword;
                        }
                        break;
                    //在线监视消息
                    case (int)RequestCode.CSMonitor:
                        webReturn = DealMonitorMessage(arg);
                        break;
                    //激活进程消息
                    case (int)RequestCode.CSActiveProcess:
                        webReturn = DealActiveProcessMessage(arg);
                        break;
                }
                ThreadPool.QueueUserWorkItem(a => OnNetPipeEvent(arg));
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }
        /// <summary>
        /// 发送NetPipe消息（向主模块发送）
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <returns></returns>
        public WebReturn SendNetPipeMessage(WebRequest request)
        {
            if (NetPipeHelper == null) { CreateNetPipeService(); }
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                if (NetPipeHelper == null)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_OBJECT_NULL;
                    webReturn.Message = string.Format("NetPipe Service is null");
                    return webReturn;
                }
                //WriteLog("NetPipe", string.Format("SendMessage\tCode:{0}\t{1}", request.Code, request.Data));

                //var temp = NetPipeHelper.SendMessage(request, string.Empty);
                //return temp;

                //以模块运行方式下，不能发送NetPipe消息
                if (!RunAsModule)
                {
                    var temp = NetPipeHelper.SendMessage(request, string.Empty);
                    return temp;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        #endregion


        #region AppCommunication Event

        /// <summary>
        /// AppCommunicationEvent
        /// </summary>
        public event Action<WebRequest> AppEvent;

        private void AppCommunicationEvent_Published(WebRequest webRequest)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => OnAppEvent(webRequest)));

            if (AppEvent != null)
            {
                AppEvent(webRequest);
            }
        }
        /// <summary>
        /// 处理AppCommunicationEvent
        /// </summary>
        /// <param name="webRequest"></param>
        protected virtual void OnAppEvent(WebRequest webRequest)
        {
            try
            {
                int code = webRequest.Code;
                switch (code)
                {
                    case (int)RequestCode.ACTaskReNavigateApp:
                        DealRenavigateApp(webRequest);
                        break;
                    case (int)RequestCode.ACLoginLoginSystem:
                        Exit();
                        Init();
                        //加载视图
                        SetView();
                        InitView();
                        break;
                    case (int)RequestCode.ACLoginLogout:
                        Exit();
                        break;
                    case (int)RequestCode.SCGlobalSettingChanged:
                        OnGlobalSettingChanged(webRequest);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        protected virtual void OnGlobalSettingChanged(WebRequest webRequest)
        {
            try
            {
                if (webRequest.ListData == null) { return; }
                if (webRequest.ListData.Count < 1) { return; }
                string strName = webRequest.ListData[0];
                switch (strName)
                {
                    case ConstValue.GS_KEY_PARAM_PASSWORD:
                        if (webRequest.ListData.Count > 1)
                        {
                            Session.UserInfo.Password = webRequest.ListData[1];
                        }
                        break;
                    case ConstValue.GS_KEY_PARAM_ROLE:
                        long longValue;
                        if (webRequest.ListData.Count > 1
                            && long.TryParse(webRequest.ListData[1], out longValue))
                        {
                            Session.RoleInfo.ID = longValue;
                            Session.RoleID = longValue;
                        }
                        if (webRequest.ListData.Count > 2)
                        {
                            Session.RoleInfo.Name = webRequest.ListData[2];
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog("GlobalSettingChanged", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region  Module Message

        public void SendModuleCloseMessage()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)RequestCode.CSModuleClose;
                PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                WriteLog("AppMessage", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public void SendLoadingMessage()
        {
            try
            {
                //以模块方式运行，无需发送LoadingMessage
                if (!RunAsModule)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = Session;
                    webRequest.Code = (int)RequestCode.CSModuleLoading;
                    PublishEvent(webRequest);
                }
            }
            catch (Exception ex)
            {
                WriteLog("AppMessage", string.Format("Send LoadingMessage fail.\t{0}", ex.Message));
            }
        }

        public void SendLoadedMessage()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)RequestCode.CSModuleLoaded;
                PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                WriteLog("AppMessage", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Monitor 相关

        private WebReturn DealMonitorMessage(WebRequest request)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                //ListData
                //0     Open/Close ( 0 表示关闭，之后不再向列表中添加监视对象；1 表示开启）
                //1     Command (指令，0 表示获取监控列表中的对象）
                if (request.ListData == null || request.ListData.Count < 2)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("ListData is null or count invalid");
                    return webReturn;
                }
                bool isRemember = request.ListData[0] == "1";
                MonitorHelper.IsRememberObject = isRemember;
                int command;
                if (!int.TryParse(request.ListData[1], out command))
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("Command invalid");
                    return webReturn;
                }
                if (command == 0)
                {
                    return MonitorHelper.DealMonitorMessage(request);
                }
                //OperationReturn optReturn;
                switch (command)
                {
                    case ConstValue.MONITOR_COMMAND_GETSESSIONINFO:
                        webReturn.Data = Session.ToString();
                        break;
                    default:
                        List<string> listArgs = new List<string>();
                        for (int i = 2; i < request.ListData.Count; i++)
                        {
                            listArgs.Add(request.ListData[i]);
                        }
                        var app = Current;
                        if (app != null)
                        {
                            webReturn = app.OnMonitorMessage(command, listArgs);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = 1;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        protected virtual WebReturn OnMonitorMessage(int command, List<string> listParams)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {

            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        #endregion


        #region 进程激活相关

        private WebReturn DealActiveProcessMessage(WebRequest request)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                string strCmd = request.Data;
                int intCmd;
                if (!int.TryParse(strCmd, out intCmd))
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("Command invalid.\t{0}", strCmd);
                    return webReturn;
                }
                var current = Current;
                if (current == null)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_OBJECT_NULL;
                    webReturn.Message = string.Format("UMPApp is null");
                    return webReturn;
                }
                return current.OnActiveProcessMessage(intCmd, request.ListData);
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        protected virtual WebReturn OnActiveProcessMessage(int command, List<string> listParams)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {

            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        #endregion


        #region OperationLog
        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="optID">操作编码</param>
        /// <param name="result">操作结果</param>
        /// <param name="contentID">日志消息的语言编码</param>
        /// <param name="listParams">参数内容</param>
        public void WriteOperationLog(string optID, string result, string contentID, List<string> listParams)
        {
            try
            {
                //Result
                //R0        操作失败
                //R1        操作成功
                //R2        失败（异常）
                //R3        关闭（取消）
                //R4        其他
                string strParams = string.Empty;
                for (int i = 0; i < listParams.Count; i++)
                {
                    strParams += string.Format("{0}{1}{1}{1}", listParams[i], ConstValue.SPLITER_CHAR_2);
                }
                strParams = strParams.Substring(0, strParams.Length - 3);

                WebRequest request = new WebRequest();
                request.Session = Session;
                request.Code = (int)RequestCode.WSWriteOperationLog;
                request.ListData.Add(ModuleID.ToString());
                request.ListData.Add(optID);
                request.ListData.Add(Environment.MachineName);
                request.ListData.Add(string.Empty);
                request.ListData.Add(string.Empty);
                request.ListData.Add(result);
                request.ListData.Add(contentID);
                request.ListData.Add(strParams);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(request);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("OperationLog",
                        string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                WriteLog("OperationLog", string.Format("Write operation log fail.\t{0}", ex.Message));
            }
        }
        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="optID">操作编码</param>
        /// <param name="result">操作结果</param>
        /// <param name="msg">日志内容</param>
        public void WriteOperationLog(string optID, string result, string msg)
        {
            try
            {
                //Result
                //R0        操作失败
                //R1        操作成功
                //R2        失败（异常）
                //R3        关闭（取消）
                //R4        其他

                WebRequest request = new WebRequest();
                request.Session = Session;
                request.Code = (int)RequestCode.WSWriteOperationLog;
                request.ListData.Add(ModuleID.ToString());
                request.ListData.Add(optID);
                request.ListData.Add(Environment.MachineName);
                request.ListData.Add(string.Empty);
                request.ListData.Add(string.Empty);
                request.ListData.Add(result);
                request.ListData.Add(string.Format("LOG{0}000", ModuleID.ToString("0000")));
                request.ListData.Add(msg);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(request);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("OperationLog",
                        string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                WriteLog("OperationLog", string.Format("Write operation log fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region App Communication

        public void PublishEvent(WebRequest webRequest)
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<AppCommunicationEvent>().Publish(webRequest);
            }
        }

        #endregion

    }
}
