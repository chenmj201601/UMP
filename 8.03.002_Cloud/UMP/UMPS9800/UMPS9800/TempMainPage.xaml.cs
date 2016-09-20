//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8adafdf0-e5bb-44f0-8a7c-f687b67f6453
//        CLR Version:              4.0.30319.18063
//        Name:                     TempMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS9800
//        File Name:                TempMainPage
//
//        created by Charley at 25/8/2015 14:52:25
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using UMPS9800.Wcf98001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common98001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS9800
{
    /// <summary>
    /// TempMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class TempMainPage
    {

        #region Members

        private List<ModuleInfo> mListModuleInfos; 
        private BackgroundWorker mWorker;

        #endregion


        public TempMainPage()
        {
            InitializeComponent();

            mListModuleInfos = new List<ModuleInfo>();

            FrameTemp.LoadCompleted += FrameTemp_LoadCompleted;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "TempMainPage";
                StylePath = "UMPS9800/TempMainPage.xaml";

                base.Init();

                if (MyWaiter != null)
                {
                    MyWaiter.Visibility = Visibility.Visible;
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    LoadModuleList();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MyWaiter != null)
                    {
                        MyWaiter.Visibility = Visibility.Hidden;
                    }

                    InitTempFrame();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadModuleList()
        {
            try
            {
                mListModuleInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int) S9800Codes.GetModuleList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service98001Client client = new Service98001Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service98001"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    var strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ModuleInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ModuleInfo info = optReturn.Data as ModuleInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tModuleInfo is null"));
                        return;
                    }
                    mListModuleInfos.Add(info);
                }

                App.WriteLog("PageLoad", string.Format("LoadModuleList end."));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitTempFrame()
        {
            try
            {
                if (PageHead != null)
                {
                    PageHead.IsSettingEnable = false;
                }
                var loadingReturn = App.LoadingMessageReturn;
                if (loadingReturn == null) { return; }
                string subModule = loadingReturn.Data;
                var moduleInfo = mListModuleInfos.FirstOrDefault(m => m.Module.ToString() == subModule);
                if (moduleInfo == null)
                {
                    App.WriteLog("PageLoad", string.Format("ModuleInfo not exist.\t{0}", subModule));
                    return;
                }
                string url = moduleInfo.Url;
                if (PageHead != null)
                {
                    PageHead.AppName = moduleInfo.AppName;
                }
                FrameTemp.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));

                App.WriteLog("PageLoad", string.Format("Frame url setted.\t{0}", url));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            try
            {
                switch (e.Code)
                {
                    //切换主题
                    case 100:
                        ThemeInfo themeInfo = e.Data as ThemeInfo;
                        if (themeInfo != null)
                        {
                            ThemeInfo = themeInfo;
                            App.Session.ThemeInfo = themeInfo;
                            App.Session.ThemeName = themeInfo.Name;
                            ChangeTheme();
                            SendThemeChangeMessage();
                        }
                        break;
                    //切换语言
                    case 110:
                        LangTypeInfo langType = e.Data as LangTypeInfo;
                        if (langType != null)
                        {
                            LangTypeInfo = langType;
                            App.Session.LangTypeInfo = langType;
                            App.Session.LangTypeID = langType.LangID;
                            MyWaiter.Visibility = Visibility.Visible;
                            mWorker = new BackgroundWorker();
                            mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                            mWorker.RunWorkerCompleted += (s, re) =>
                            {
                                mWorker.Dispose();
                                MyWaiter.Visibility = Visibility.Hidden;
                                ChangeLanguage();
                                SendLanguageChangeMessage();
                            };
                            mWorker.RunWorkerAsync();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void FrameTemp_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                SendLoadedMessage();

                App.WriteLog("SendLoaded", string.Format("SendLoaded end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void ChangeFrameUrl(string subModule)
        {
            try
            {
                App.WriteLog("ChangeFrame", string.Format("Changing frame...\t{0}", subModule));
                var moduleInfo = mListModuleInfos.FirstOrDefault(m => m.Module.ToString() == subModule);
                if (moduleInfo == null)
                {
                    App.WriteLog("ChangeFrame", string.Format("ModuleInfo not exist.\t{0}", subModule));
                    return;
                }
                string url = moduleInfo.Url;
                if (PageHead != null)
                {
                    PageHead.AppName = moduleInfo.AppName;
                }
                FrameTemp.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));

                App.WriteLog("ChangeFrame", string.Format("Frame url setted.\t{0}", url));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region NetPipeMessage

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    ChangeLanguage();
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                        case (int)RequestCode.SCOperation:
                            string subModule = webRequest.Data;
                            ChangeFrameUrl(subModule);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        #endregion
    }
}
