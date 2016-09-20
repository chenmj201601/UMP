//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7598d6c8-6241-4ebc-9a34-4054bc3598c6
//        CLR Version:              4.0.30319.18063
//        Name:                     MainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPDemo
//        File Name:                MainPage
//
//        created by Charley at 2014/8/20 20:49:20
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPDemo
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage
    {
        private string mClientID = "UMPS11099";
        private NetPipeHelper mNetPipeHelper;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            BtnTest.Click += BtnTest_Click;
            BtnBlue.Click += BtnBlue_Click;
            BtnGreen.Click += BtnGreen_Click;

            CreateNetPipeService();

            Init();
            PageHead.ThemeInfo = ThemeInfo;
            PageHead.AppServerInfo = AppServerInfo;
            ChangeTheme();
        }

        void PageHead_ThemeInfoChanged(ThemeInfo obj)
        {
            ThemeInfo = obj;
            ChangeTheme();
        }

        void BtnGreen_Click(object sender, RoutedEventArgs e)
        {
            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "MetroLight";
            themeInfo.Color = "Green";
            ThemeInfo = themeInfo;
            ChangeTheme();
        }

        void BtnBlue_Click(object sender, RoutedEventArgs e)
        {
            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Blue";
            ThemeInfo = themeInfo;
            ChangeTheme();
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            PopupPanel.IsOpen = true;

            //WebRequest webRequest = new WebRequest();
            ////webRequest.Session = mClientID;
            //SessionInfo session=new SessionInfo();
            //session.SessionID = Guid.NewGuid().ToString();
            //session.AppName = mClientID;
            //UserInfo user=new UserInfo();
            //user.UserName = "charley";
            //session.UserInfo = user;
            //webRequest.Session = session;

            //webRequest.Code = 100;
            //webRequest.Data = string.Format("Hello! this is Client:{0}", mClientID);

            //OnShowMessage(webRequest.Data);
            //if (mNetPipeHelper != null)
            //{
            //    WebReturn webReturn = mNetPipeHelper.SendMessage(webRequest, mClientID);
            //    if (webReturn.Result)
            //    {
            //        OnShowMessage(string.Format("{0}", webReturn.Data));
            //    }
            //    else
            //    {
            //        OnShowMessage(string.Format("Receive fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
            //    }
            //}
        }

        private void Init()
        {
            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            ThemeInfo = themeInfo;

            AppServerInfo webInfo = new AppServerInfo();
            webInfo.Address = "192.168.6.75";
            webInfo.Port = 8081;
            webInfo.Protocol = "http";
            AppServerInfo = webInfo;
        }

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                                      AppServerInfo.Protocol,
                                      AppServerInfo.Address,
                                      AppServerInfo.Port,
                                      ThemeInfo.Name
                                      , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch { }
            }
            if (!bPage)
            {
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
                                      ThemeInfo.Name
                                      , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch { }
            }
        }

        private void CreateNetPipeService()
        {
            if (mNetPipeHelper != null)
            {
                mNetPipeHelper.Stop();
            }
            mNetPipeHelper = new NetPipeHelper(false, mClientID);
            mNetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            var result = mNetPipeHelper.Start();
            if (result)
            {
                OnShowMessage(string.Format("NetPipe service created."));
            }
            else
            {
                OnShowMessage(string.Format("Create NetPipe service fail."));
            }
        }

        WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            if (arg != null)
            {

            }
            return null;
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
