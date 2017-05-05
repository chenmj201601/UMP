//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c48e8061-db48-4d0a-86dc-54d2de793b45
//        CLR Version:              4.0.30319.42000
//        Name:                     TempMainView
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS9800
//        File Name:                TempMainView
//
//        created by Charley at 2016/2/23 13:48:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;
using UMPS9800.Wcf98001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common98001;
using VoiceCyber.UMP.Communications;

namespace UMPS9800
{
    /// <summary>
    /// TempMainView.xaml 的交互逻辑
    /// </summary>
    public partial class TempMainView
    {

        #region Members

        private List<ModuleInfo> mListModuleInfos;
        private BackgroundWorker mWorker;

        #endregion


        public TempMainView()
        {
            InitializeComponent();

            mListModuleInfos = new List<ModuleInfo>();

            FrameTemp.LoadCompleted += FrameTemp_LoadCompleted;
            FrameTemp.Navigating += (s, e) => FrameTemp.SuppressScriptErrors(true);
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "TempMainPage";
                StylePath = "UMPS9800/TempMainPage.xaml";

                base.Init();

                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    LoadModuleList();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    InitTempFrame();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadModuleList()
        {
            try
            {
                mListModuleInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S9800Codes.GetModuleList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service98001Client client = new Service98001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service98001"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    var strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ModuleInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ModuleInfo info = optReturn.Data as ModuleInfo;
                    if (info == null)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\tModuleInfo is null"));
                        return;
                    }
                    mListModuleInfos.Add(info);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("LoadModuleList end."));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitTempFrame()
        {
            try
            {
                string strArgs = CurrentApp.StartArgs;
                if (string.IsNullOrEmpty(strArgs))
                {
                    //CurrentApp.ShowExceptionMessage(string.Format("StartArgs invalid"));
                    //return;
                    strArgs = "9801";
                }
                var moduleInfo = mListModuleInfos.FirstOrDefault(m => m.Module.ToString() == strArgs);
                if (moduleInfo == null)
                {
                    CurrentApp.WriteLog("PageLoad", string.Format("ModuleInfo not exist.\t{0}", strArgs));
                    return;
                }
                string url = moduleInfo.Url;
                FrameTemp.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));

                CurrentApp.WriteLog("PageLoad", string.Format("Frame url setted.\t{0}", url));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void FrameTemp_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                CurrentApp.SendLoadedMessage();

                CurrentApp.WriteLog("SendLoaded", string.Format("SendLoaded end"));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);

            try
            {
                int code = webRequest.Code;
               
                switch (code)
                {
                    case (int)RequestCode.ACTaskReNavigateApp:
                        string strAppID = string.Empty;
                        string strArgs = string.Empty;
                        if (webRequest.ListData.Count > 0)
                        {
                            strAppID = webRequest.ListData[0];
                        }
                        if (webRequest.ListData.Count > 1)
                        {
                            strArgs = webRequest.ListData[1];
                        }
                        if (strAppID == CurrentApp.ModuleID.ToString())
                        {
                            if (!string.IsNullOrEmpty(strArgs)
                                && strArgs != CurrentApp.StartArgs)
                            {
                                CurrentApp.StartArgs = strArgs;
                                ChangeFrameUrl(strArgs);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("AppEvent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations

        private void ChangeFrameUrl(string subModule)
        {
            try
            {
                CurrentApp.WriteLog("ChangeFrame", string.Format("Changing frame...\t{0}", subModule));
                var moduleInfo = mListModuleInfos.FirstOrDefault(m => m.Module.ToString() == subModule);
                if (moduleInfo == null)
                {
                    CurrentApp.WriteLog("ChangeFrame", string.Format("ModuleInfo not exist.\t{0}", subModule));
                    return;
                }
                string url = moduleInfo.Url;
                FrameTemp.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));

                CurrentApp.WriteLog("ChangeFrame", string.Format("Frame url setted.\t{0}", url));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion
    }

    /// <summary>
    /// 屏蔽浏览器弹出的脚本报错信息
    /// </summary>
    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
