//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7b974582-1c84-45eb-ba41-b40601237a34
//        CLR Version:              4.0.30319.42000
//        Name:                     TaskPageView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205
//        File Name:                TaskPageView
//
//        created by Charley at 2016/1/24 17:43:13
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UMPS1205.Models;
using UMPS1205.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;

namespace UMPS1205
{
    /// <summary>
    /// TaskPageView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskPageView
    {

        #region Memebers

        private ObservableCollection<AppInfoItem> mListAppInfoItems;
        private List<BasicAppInfo> mListBasicAppInfos;
        private ObservableCollection<AppGroupItem> mListAppGroups;

        #endregion


        public TaskPageView()
        {
            InitializeComponent();

            mListAppInfoItems = new ObservableCollection<AppInfoItem>();
            mListBasicAppInfos = new List<BasicAppInfo>();
            mListAppGroups = new ObservableCollection<AppGroupItem>();

            //BtnDemo.Click += BtnDemo_Click;
        }

        protected override void Init()
        {
            try
            {
                PageName = "TaskPageView";
                StylePath = "UMPS1205/TaskPageView.xaml";

                //ListBoxAppList.ItemsSource = mListAppInfoItems;
                ListBoxGroups.ItemsSource = mListAppGroups;

                CommandBindings.Add(new CommandBinding(OpenAppCommand, OpenAppCommand_Executed,
                    (s, e) => { e.CanExecute = true; }));

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                SetBusy(true, string.Format("Loading basic informations..."));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadBasicAppInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    InitAppInfoItems();
                };
                worker.RunWorkerAsync();

                //ChangeTheme();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadBasicAppInfos()
        {
            try
            {
                mListBasicAppInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetAppBasicInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
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
                    if (info == null)
                    {
                        ShowException(string.Format("BasicAppInfo is null"));
                        return;
                    }
                    mListBasicAppInfos.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAppInfoItems()
        {
            try
            {
                //mListAppInfoItems.Clear();
                //for (int i = 0; i < mListBasicAppInfos.Count; i++)
                //{
                //    var info = mListBasicAppInfos[i];

                //    AppInfoItem item = new AppInfoItem();
                //    item.ModuleID = info.ModuleID;
                //    item.Title = info.Title;
                //    item.Category = info.Category;
                //    item.Icon = GetAppIcon(info);
                //    item.Info = info;
                //    mListAppInfoItems.Add(item);
                //}

                mListAppInfoItems.Clear();
                mListAppGroups.Clear();
                var groups = mListBasicAppInfos.GroupBy(g => g.Category);
                foreach (var group in groups)
                {
                    AppGroupItem groupItem=new AppGroupItem();
                    groupItem.Name = group.Key;
                    groupItem.Title = CurrentApp.GetLanguageInfo(string.Format("{0}Content", group.Key), group.Key);
                    foreach (var info in group)
                    {
                        AppInfoItem infoItem=new AppInfoItem();
                        infoItem.ModuleID = info.ModuleID;
                        infoItem.Category = info.Category;
                        infoItem.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", info.ModuleID), info.Title);
                        infoItem.Icon = GetAppIcon(info);
                        infoItem.Info = info;
                        groupItem.ListApps.Add(infoItem);
                        mListAppInfoItems.Add(infoItem);
                    }
                    mListAppGroups.Add(groupItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region EventHandlers

        void BtnDemo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACTaskNavigateApp;
                webRequest.ListData.Add("1299");
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private string GetAppIcon(BasicAppInfo info)
        {
            string strReturn = string.Empty;
            try
            {
                var session = CurrentApp.Session;
                if (session == null)
                {
                    return strReturn;
                }
                var appServerInfo = CurrentApp.Session.AppServerInfo;
                if (appServerInfo == null)
                {
                    return strReturn;
                }
                string url = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                    appServerInfo.Protocol,
                    appServerInfo.Address,
                    appServerInfo.Port,
                    session.ThemeName,
                    info.Icon);
                strReturn = url;
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("GetAppIcon", string.Format("Fail.\t{0}", ex.Message));
            }
            return strReturn;
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
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1205;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListAppGroups.Count; i++)
                {
                    var group = mListAppGroups[i];
                    group.Title = CurrentApp.GetLanguageInfo(string.Format("{0}Content", group.Name), group.Name);
                }
                for (int i = 0; i < mListAppInfoItems.Count; i++)
                {
                    var item = mListAppInfoItems[i];
                    item.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", item.ModuleID),
                        string.Format("UMPS{0}", item.ModuleID));
                }
            }
            catch (Exception ex) { }
        }

        #endregion


        #region OpenAppCommand

        private static RoutedUICommand mOpenAppCommand = new RoutedUICommand();

        public static RoutedUICommand OpenAppCommand
        {
            get { return mOpenAppCommand; }
        }

        private void OpenAppCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var item = e.Parameter as AppInfoItem;
                if (item == null) { return; }
                var info = item.Info;
                if (info == null) { return; }
                int appID = info.AppID;
                string strArgs = info.Args;
                string strIcon = info.Icon;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACTaskNavigateApp;
                webRequest.ListData.Add(appID.ToString());
                webRequest.ListData.Add(strArgs);
                webRequest.ListData.Add(strIcon);
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

    }
}
