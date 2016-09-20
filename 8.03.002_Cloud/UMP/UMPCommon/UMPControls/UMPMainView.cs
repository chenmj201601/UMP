//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    35b493e6-8bc2-420a-bfaf-cf18eddc3da1
//        CLR Version:              4.0.30319.42000
//        Name:                     UMPMainView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPMainView
//
//        created by Charley at 2016/3/17 17:33:03
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;


namespace VoiceCyber.UMP.Controls
{
    public class UMPMainView : UMPUserControl, IRegionMemberLifetime
    {

        #region Members

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        protected bool IsInited = false;

        #endregion


        static UMPMainView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UMPMainView),
                new FrameworkPropertyMetadata(typeof(UMPMainView)));
        }

        public UMPMainView()
        {
            Loaded += UMPMainView_Loaded;
            Unloaded += UMPMainView_Unloaded;
        }

        void UMPMainView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInited)
            {
                if (CurrentApp != null)
                {
                    CurrentApp.AppEvent += AppCommunicationEvent_Published;
                }

                Init();
                IsInited = true;
            }
        }

        void UMPMainView_Unloaded(object sender, RoutedEventArgs e)
        {
            //Close();
        }


        #region Init and Load

        protected virtual void Init()
        {
            try
            {
                if (CurrentApp == null
                    || CurrentApp.Session == null) { return; }

                ThemeInfo = CurrentApp.Session.ThemeInfo;
                LangTypeInfo = CurrentApp.Session.LangTypeInfo;
                AppServerInfo = CurrentApp.Session.AppServerInfo;
                mLastActiveTime = DateTime.Now;

                //ChangeTheme();
                //ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Close

        public virtual void Close()
        {

        }

        #endregion


        #region AppCommunicationEvent

        private void AppCommunicationEvent_Published(WebRequest webRequest)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => OnAppEvent(webRequest)));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        protected virtual void OnAppEvent(WebRequest webRequest)
        {
            try
            {
                var code = webRequest.Code;
                var strData = webRequest.Data;
                switch (code)
                {
                    case (int)RequestCode.CSLanguageChange:
                        int langID;
                        if (int.TryParse(strData, out langID))
                        {
                            DoChangeLanguage(langID);
                        }
                        break;
                    case (int)RequestCode.CSThemeChange:
                        DoChangeTheme(strData);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        public void SetBusy(bool isWorking, string msg)
        {
            try
            {
                WebRequest request = new WebRequest();
                request.Session = CurrentApp.Session;
                request.Code = (int)RequestCode.ACStatusSetStatus;
                request.ListData.Add(isWorking ? "1" : "0");
                request.ListData.Add(msg);
                CurrentApp.PublishEvent(request);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeTheme(string strThemeName)
        {
            try
            {
                ThemeInfo themeInfo = CurrentApp.Session.SupportThemes.FirstOrDefault(t => t.Name == strThemeName);
                if (themeInfo != null)
                {
                    ThemeInfo = themeInfo;
                    CurrentApp.Session.ThemeInfo = themeInfo;
                    CurrentApp.Session.ThemeName = themeInfo.Name;
                    ChangeTheme();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeLanguage(int langID)
        {
            try
            {
                LangTypeInfo langType =
                    CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID == langID);
                if (langType == null) { return; }
                LangTypeInfo = langType;
                CurrentApp.Session.LangTypeInfo = langType;
                CurrentApp.Session.LangTypeID = langType.LangID;
                SetBusy(true, string.Empty);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
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
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.ACLoginOnline;
                    CurrentApp.PublishEvent(webRequest);
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


        #region Template

        private const string PART_Panel = "PART_Panel";
        private Border mBorderPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {
                mBorderPanel.MouseMove += BorderMain_MouseMove;
                mBorderPanel.MouseDown += BorderMain_MouseDown;
                mBorderPanel.KeyDown += BorderMain_KeyDown;
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
                        ThemeInfo.Name
                        , "CommonControls/UMPCommon.xaml");
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
                    string uri = string.Format("/UMPControls;component/Themes/{0}/{1}",
                        "Default"
                        , "CommonControls/UMPCommon.xaml");
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPControls;component/Themes/{0}/{1}",
                        "Default"
                        , "CommonControls/UMPMainView.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }
        }

        #endregion


        #region KeepAlive

        public bool KeepAlive
        {
            get { return false; }
        }

        #endregion

    }
}
