//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    32ce4045-32bc-4059-9329-2dcac74f0197
//        CLR Version:              4.0.30319.18444
//        Name:                     IndexPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPMain
//        File Name:                IndexPage
//
//        created by Charley at 2014/8/25 14:45:31
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Linq;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPMain
{
    /// <summary>
    /// IndexPage.xaml 的交互逻辑
    /// </summary>
    public partial class IndexPage
    {
        private NetPipeHelper mNetPipeHelper;
        //private SubWindowHelper mSubWindowHelper;

        public IndexPage()
        {
            InitializeComponent();

            Loaded += IndexPage_Loaded;
        }

        void IndexPage_Loaded(object sender, RoutedEventArgs e)
        {
            BtnTest.Click += BtnTest_Click;
            BtnStart.Click += BtnStart_Click;
            BtnStop.Click += BtnStop_Click;
            BtnTest2.Click += BtnTest2_Click;

            //mSubWindowHelper = new SubWindowHelper();
            //mSubWindowHelper.Init(GridContainer, SubWindow);
        }

        void BtnTest2_Click(object sender, RoutedEventArgs e)
        {
            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == "Style02");
            if (themeInfo != null)
            {
                App.Session.ThemeInfo = themeInfo;
                App.Session.ThemeName = themeInfo.Name;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.SCThemeChange;
                SendNetPipeMessage(webRequest);
            }
            //LangTypeInfo langTypeInfo = App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID == 2052);
            //if (langTypeInfo != null)
            //{
            //    App.Session.LangTypeInfo = langTypeInfo;
            //    App.Session.LangTypeID = langTypeInfo.LangID;
            //    WebRequest webRequest = new WebRequest();
            //    webRequest.Session = App.Session;
            //    webRequest.Code = (int)RequestCode.SCLanguageChange;
            //    SendNetPipeMessage(webRequest);
            //}
        }

        void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (mNetPipeHelper != null)
            {
                mNetPipeHelper.Stop();
                OnShowMessage(string.Format("Service stopped."));
            }
        }

        void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            CreateNetPipeService();
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            NavigateContent();
            TxtState.Text = "Loading...";
            //SubWindow.Visibility = Visibility.Visible;
        }

        private void CreateNetPipeService()
        {
            mNetPipeHelper = new NetPipeHelper(true, string.Empty);
            mNetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                mNetPipeHelper.Start();
                OnShowMessage(string.Format("Service started."));
            }
            catch (Exception ex)
            {
                OnShowMessage(string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                var clientSession = arg.Session;
                if (clientSession != null)
                {
                    var clientName = clientSession.AppName;
                    var clientID = clientSession.SessionID;
                    OnShowMessage(string.Format("Request:{0}({1}) \t{2}({3})",
                            (RequestCode)arg.Code,
                            arg.Code,
                            clientName,
                            clientID));

                    switch (arg.Code)
                    {
                        case (int)RequestCode.CSModuleLoading:
                            webReturn.Session = clientSession;
                            webReturn.Data = "CreateScoreSheet";
                            break;
                        case (int)RequestCode.CSModuleLoaded:
                            SubWindow.Visibility = Visibility.Visible;
                            TxtState.Text = "Loaded";
                            break;
                    }
                }
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

        private void NavigateContent()
        {
            //BrowserSubModule.Loaded += (s, ee) => BrowserSubModule.Navigate(new Uri(string.Format("http://192.168.6.75:8081/UMPS1101.xbap"), UriKind.Absolute));
            //BrowserSubModule.Navigate(new Uri(string.Format("http://192.168.6.75:8081/UMPS1101.xbap"), UriKind.Absolute));

            BrowserSubModule.Loaded += (s, ee) => BrowserSubModule.Navigate(new Uri(string.Format("http://192.168.6.75:8081/UMPS3101.xbap"), UriKind.Absolute));
            BrowserSubModule.Navigate(new Uri(string.Format("http://192.168.6.75:8081/UMPS3101.xbap"), UriKind.Absolute));
        }

        private void SendNetPipeMessage(WebRequest webRequest)
        {
            if (mNetPipeHelper == null) { CreateNetPipeService(); }
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                if (mNetPipeHelper == null)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_OBJECT_NULL;
                    webReturn.Message = string.Format("NetPipe Service is null");
                    return;
                }
                var temp = mNetPipeHelper.SendMessage(webRequest, "UMPS1101");
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
        }

        private void OnShowMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }
}
