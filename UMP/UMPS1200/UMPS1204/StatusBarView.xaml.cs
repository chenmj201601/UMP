//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f702fabc-d706-46e0-a0b5-909a7b8bdee5
//        CLR Version:              4.0.30319.42000
//        Name:                     StatusBarView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1204
//        File Name:                StatusBarView
//
//        created by Charley at 2016/1/22 12:13:11
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1204
{
    /// <summary>
    /// StatusBarView.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBarView
    {

        #region Memebers



        #endregion


        public StatusBarView()
        {
            InitializeComponent();
        }


        protected override void Init()
        {
            try
            {
                PageName = "StatusBarView";
                StylePath = "UMPS1204/StatusBarView.xaml";

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                SetDisplayVersion();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetDisplayVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                int a = version.Major;
                int b = version.Minor;
                int c = version.Build;
                string str = string.Format("{0}.{1}.{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                TxtVersion.Text = str;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetStatus(WebRequest webRequest)
        {
            try
            {
                if (webRequest.ListData == null
                    || webRequest.ListData.Count < 2)
                {
                    return;
                }
                bool isWorking = webRequest.ListData[0] == "1";
                string strMsg = webRequest.ListData[1];
                //MyWaiter.Visibility = isWorking ? Visibility.Visible : Visibility.Hidden;
                if (isWorking)
                {
                    CustomWaiter waiter = new CustomWaiter();
                    waiter.FontSize = 5;
                    waiter.ViewColor = Brushes.DarkRed;
                    waiter.VerticalAlignment = VerticalAlignment.Center;
                    waiter.Width = 500;
                    waiter.Height = 5;
                    //waiter.SetResourceReference(StylePathProperty, "MyWaiterStyle");
                    BorderWaiter.Child = waiter;
                }
                else
                {
                    BorderWaiter.Child = null;
                }
                TxtMsg.Text = strMsg;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);

            try
            {
                int code = webRequest.Code;
                //CurrentApp.WriteLog("AppEvent", string.Format("{0}", ParseAppEventCode(code)));
                switch (code)
                {
                    case (int)RequestCode.ACLoginLoginSystem:
                        Dispatcher.Invoke(new Action(() => DoLoginSystem(webRequest)));
                        break;
                    case (int)RequestCode.ACStatusSetStatus:
                        SetStatus(webRequest);
                        break;
                }
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
                CurrentApp.Session.SetSessionInfo(session);
                CurrentApp.WriteLog("LoginSystem", string.Format("{0}", CurrentApp.Session.LogInfo()));

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

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
                    string uri = string.Format("/UMPS1204;component/Themes/{0}/{1}",
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

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = string.Format("UMP");
            }
            catch { }
        }
    }
}
