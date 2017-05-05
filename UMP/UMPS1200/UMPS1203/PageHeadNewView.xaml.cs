//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    41246012-26e7-45fb-a839-7f4588cf78e6
//        CLR Version:              4.0.30319.42000
//        Name:                     PageHeadNewView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1203
//        File Name:                PageHeadNewView
//
//        created by Charley at 2016/3/22 15:10:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UMPS1203.Models;
using UMPS1203.Wcf11012;
using UMPS1203.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;

namespace UMPS1203
{
    /// <summary>
    /// PageHeadNewView.xaml 的交互逻辑
    /// </summary>
    public partial class PageHeadNewView
    {

        #region Memebers

        private List<BasicAppInfo> mListModuleInfos;
        private ObservableCollection<ThemeInfoItem> mListThemeInfoItems;
        private ObservableCollection<LangInfoItem> mListLangInfoItems;
        private ObservableCollection<ModuleGroupItem> mListModuleGroups;
        private List<BasicModuleItem> mListModuleItems;

        #endregion


        public PageHeadNewView()
        {
            InitializeComponent();

            mListModuleInfos = new List<BasicAppInfo>();
            mListLangInfoItems = new ObservableCollection<LangInfoItem>();
            mListThemeInfoItems = new ObservableCollection<ThemeInfoItem>();
            mListModuleGroups = new ObservableCollection<ModuleGroupItem>();
            mListModuleItems = new List<BasicModuleItem>();

            //BtnLogout.Click += BtnLogout_Click;
            BtnOpenSetting.Click += BtnOpenSetting_Click;
            BtnNavigateHome.Click += BtnNavigateHome_Click;
            ListBoxLanguages.SelectionChanged += ListBoxLanguages_SelectionChanged;
            ListBoxThemes.SelectionChanged += ListBoxThemes_SelectionChanged;
            BtnOpenIM.Click += BtnOpenIM_Click;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                ListBoxLanguages.ItemsSource = mListLangInfoItems;
                ListBoxThemes.ItemsSource = mListThemeInfoItems;
                ListBoxModuleGroups.ItemsSource = mListModuleGroups;

                PageName = "PageHeadNewView";
                StylePath = "UMPS1203/PageHeadNewView.xaml";

                CommandBindings.Add(new CommandBinding(AppItemCommand, AppItemCommand_Executed,
                    (s, ce) => ce.CanExecute = true));
                CommandBindings.Add(new CommandBinding(PageHeadCommand, PageHeadCommand_Executed,
                   (s, ce) => ce.CanExecute = true));

                base.Init();

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    if (CurrentApp != null)
                    {
                        CurrentApp.SendLoadedMessage();
                    }
                    LoadModuleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitThemeInfoItems();
                    InitLangInfoItems();
                    InitUserInfo();
                    CreateModuleItems();

                    //ChangeTheme();
                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitUserInfo()
        {
            try
            {
                if (CurrentApp == null) { return; }
                var session = CurrentApp.Session;
                if (session == null) { return; }
                TxtUserAccount.Text = session.UserInfo.Account;
                TxtRoleName.Text = session.RoleInfo.Name;
                var theme = mListThemeInfoItems.FirstOrDefault(t => t.Name == session.ThemeName);
                if (theme != null)
                {
                    ListBoxThemes.SelectedItem = theme;
                }
                var lang = mListLangInfoItems.FirstOrDefault(t => t.Code == session.LangTypeID);
                if (lang != null)
                {
                    ListBoxLanguages.SelectedItem = lang;
                }
                GetLogoIcon();
                GetHeadIcon();
                SetIMState(false, string.Empty);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitThemeInfoItems()
        {
            try
            {
                mListThemeInfoItems.Clear();
                if (CurrentApp == null) { return; }
                var session = CurrentApp.Session;
                if (session == null) { return; }
                for (int i = 0; i < session.SupportThemes.Count; i++)
                {
                    var theme = session.SupportThemes[i];
                    var item = new ThemeInfoItem();
                    item.Name = theme.Name;
                    item.Display = theme.Display;
                    item.Description = theme.Display;
                    item.ThumbImage = GetThumbImage(item);
                    item.IsSelected = false;
                    mListThemeInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitLangInfoItems()
        {
            try
            {
                mListLangInfoItems.Clear();
                if (CurrentApp == null) { return; }
                var session = CurrentApp.Session;
                if (session == null) { return; }
                for (int i = 0; i < session.SupportLangTypes.Count; i++)
                {
                    var lang = session.SupportLangTypes[i];
                    var item = new LangInfoItem();
                    item.Info = lang;
                    item.Code = lang.LangID;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("COML{0}", lang.LangID), lang.Display);
                    item.IsSelected = false;
                    mListLangInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadModuleInfos()
        {
            try
            {
                mListModuleInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetAppBasicInfoList;
                webRequest.ListData.Add(CurrentApp.Session.RoleID.ToString());
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

                CurrentApp.WriteLog("LoadModules", string.Format("Load end.\t{0}", mListModuleInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create Operations

        private void CreateModuleItems()
        {
            try
            {
                mListModuleGroups.Clear();
                mListModuleItems.Clear();
                string strIcon;
                var groups = mListModuleInfos.GroupBy(m => m.Category);
                foreach (var group in groups)
                {
                    string strGroupName = group.Key;
                    string strGroupDisplay = CurrentApp.GetLanguageInfo(string.Format("{0}Content", strGroupName), strGroupName);
                    strIcon = string.Format("{0}://{1}:{2}/Themes/{3}/Images/S0002/{4}.png",
                        CurrentApp.Session.AppServerInfo.Protocol,
                        CurrentApp.Session.AppServerInfo.Address,
                        CurrentApp.Session.AppServerInfo.Port,
                        CurrentApp.Session.ThemeName,
                        strGroupName);
                    if (strGroupName == "G1601")
                    {
                        //特殊模块，特殊处理
                        continue;
                    }
                    ModuleGroupItem groupItem = new ModuleGroupItem();
                    groupItem.CurrentApp = CurrentApp;
                    groupItem.Name = strGroupName;
                    groupItem.Title = strGroupDisplay;
                    groupItem.Icon = strIcon;
                    foreach (var app in group)
                    {
                        strIcon = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                           CurrentApp.Session.AppServerInfo.Protocol,
                           CurrentApp.Session.AppServerInfo.Address,
                           CurrentApp.Session.AppServerInfo.Port,
                           CurrentApp.Session.ThemeName,
                           app.Icon);
                        BasicModuleItem moduleItem = new BasicModuleItem();
                        moduleItem.CurrentApp = CurrentApp;
                        moduleItem.ModuleID = app.ModuleID;
                        moduleItem.Name = app.Title;
                        moduleItem.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", app.ModuleID), app.Title);
                        moduleItem.Icon = strIcon;
                        moduleItem.Tip = moduleItem.Title;
                        moduleItem.Category = CurrentApp.GetLanguageInfo(string.Format("{0}Content", strGroupName), strGroupName);
                        moduleItem.Info = app;
                        groupItem.ListApps.Add(moduleItem);
                        mListModuleItems.Add(moduleItem);
                    }
                    mListModuleGroups.Add(groupItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnOpenSetting_Click(object sender, RoutedEventArgs e)
        {
            PopupSettings.IsOpen = true;
        }

        void BtnNavigateHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.CSHome;
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            DoLogoutSystem();
        }

        void BtnOpenIM_Click(object sender, RoutedEventArgs e)
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

        void ListBoxThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems == null || e.RemovedItems.Count <= 0) { return; }
                var item = ListBoxThemes.SelectedItem as ThemeInfoItem;
                if (item == null) { return; }
                if (CurrentApp == null || CurrentApp.Session == null) { return; }
                var session = CurrentApp.Session;
                ThemeInfo themeInfo = session.SupportThemes.FirstOrDefault(t => t.Name == item.Name);
                if (themeInfo != null)
                {
                    ThemeInfo = themeInfo;
                    session.ThemeInfo = themeInfo;
                    session.ThemeName = themeInfo.Name;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.CSThemeChange;
                    webRequest.Data = themeInfo.Name;
                    CurrentApp.PublishEvent(webRequest);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListBoxLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems == null || e.RemovedItems.Count <= 0) { return; }
                var item = ListBoxLanguages.SelectedItem as LangInfoItem;
                if (item == null) { return; }
                if (CurrentApp == null || CurrentApp.Session == null) { return; }
                var session = CurrentApp.Session;
                LangTypeInfo langType = session.SupportLangTypes.FirstOrDefault(t => t.LangID == item.Code);
                if (langType != null)
                {
                    LangTypeInfo = langType;
                    session.LangTypeInfo = langType;
                    session.LangTypeID = langType.LangID;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.CSLanguageChange;
                    webRequest.Data = langType.LangID.ToString();
                    CurrentApp.PublishEvent(webRequest);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void DoLogoutSystem()
        {
            try
            {
                var result = MessageBox.Show(CurrentApp.GetLanguageInfo("S0000054", "Confirm logout?"), CurrentApp.AppTitle,
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.ACPageHeadLogout;
                    CurrentApp.PublishEvent(webRequest);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoCloseLeftPanel()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACPageHeadLeftPanel;
                CurrentApp.PublishEvent(webRequest);
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
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACPageHeadDefaultPage;
                CurrentApp.PublishEvent(webRequest);
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
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.CSChangePassword;
                CurrentApp.PublishEvent(webRequest);
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
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.CSRoleChange;
                CurrentApp.PublishEvent(webRequest);
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
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACPageHeadOpenIMPanel;
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Reload()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    if (CurrentApp != null)
                    {
                        CurrentApp.SendLoadedMessage();
                    }
                    LoadModuleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitThemeInfoItems();
                    InitLangInfoItems();
                    InitUserInfo();
                    CreateModuleItems();

                    //ChangeTheme();
                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetIMState(bool isShow, string strCount)
        {
            try
            {
                BorderMsgCount.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
                TxtMsgCount.Text = strCount;
                TxtMsgCount.ToolTip = strCount;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoSetIMState(WebRequest webRequest)
        {
            try
            {
                string strType = string.Empty;
                bool isShow = false;
                string strCount = string.Empty;
                string strTip = string.Empty;
                if (webRequest.ListData != null)
                {
                    if (webRequest.ListData.Count > 0)
                    {
                        strType = webRequest.ListData[0];
                    }
                    if (strType == "1")
                    {
                        BorderLoginState.Visibility = Visibility.Visible;
                    }
                    if (strType == "2")
                    {
                        BorderLoginState.Visibility = Visibility.Collapsed;
                    }
                    if (strType == "3")
                    {
                        if (webRequest.ListData.Count > 1)
                        {
                            strCount = webRequest.ListData[1];
                        }
                        int intValue;
                        if (int.TryParse(strCount, out intValue)
                            && intValue > 0)
                        {
                            isShow = intValue > 0;
                            strTip = strCount;
                            if (intValue > 100)
                            {
                                strCount = "99+";
                            }
                        }
                    }
                }
                BorderMsgCount.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
                TxtMsgCount.Text = strCount;
                TxtMsgCount.ToolTip = strTip;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                    string uri = string.Format("/UMPS1203;component/Themes/{0}/{1}",
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

        #endregion


        #region Others

        private string GetThumbImage(ThemeInfoItem themeInfoItem)
        {
            string strReturn = string.Empty;
            try
            {
                string path = string.Format("{0}://{1}:{2}/Themes/{3}/bg.jpg"
                    , AppServerInfo.Protocol
                    , AppServerInfo.Address
                    , AppServerInfo.Port
                    , themeInfoItem.Name);
                strReturn = path;
            }
            catch
            {

            }
            return strReturn;
        }

        private void GetLogoIcon()
        {
            try
            {
                string path = string.Format("{0}://{1}:{2}/Logo/logo.png"
                    , AppServerInfo.Protocol
                    , AppServerInfo.Address
                    , AppServerInfo.Port);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path);
                image.EndInit();
                ImageLogo.Source = image;
            }
            catch
            {

            }
        }

        private void GetHeadIcon()
        {
            try
            {
                string path = string.Format("{0}://{1}:{2}/HeadIcons/0.bmp"
                              , AppServerInfo.Protocol
                              , AppServerInfo.Address
                              , AppServerInfo.Port);
                if (CurrentApp != null
                    && CurrentApp.Session != null)
                {
                    var session = CurrentApp.Session;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetResourceProperty;
                    webRequest.ListData.Add("001");
                    webRequest.ListData.Add("102HeadIcon");
                    webRequest.ListData.Add("1");
                    webRequest.ListData.Add(session.UserID.ToString());
                    Service11012Client client = new Service11012Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service11012"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (webReturn.Result
                        && webReturn.ListData != null
                        && webReturn.ListData.Count > 0)
                    {
                        string strInfo = webReturn.ListData[0];
                        string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length >= 2)
                        {
                            string strIcon = arrInfo[1];
                            path = string.Format("{0}://{1}:{2}/HeadIcons/{3}.bmp"
                               , AppServerInfo.Protocol
                               , AppServerInfo.Address
                               , AppServerInfo.Port
                               , strIcon);
                        }
                    }
                }
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path);
                image.EndInit();
                ImageHead.Source = image;
            }
            catch { }
        }

        #endregion


        #region OnAppEvent

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);

            try
            {
                int code = webRequest.Code;
                switch (code)
                {
                    case (int)RequestCode.ACPageHeadSetIMState:
                        Dispatcher.Invoke(new Action(() => DoSetIMState(webRequest)));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region AppItemCommand

        private static RoutedUICommand mAppItemCommand = new RoutedUICommand();

        public static RoutedUICommand AppItemCommand
        {
            get { return mAppItemCommand; }
        }

        private void AppItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var item = e.Parameter as BasicModuleItem;
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


        #region PageHeadCommand

        private static RoutedUICommand mPageHeadCommand = new RoutedUICommand();

        public static RoutedUICommand PageHeadCommand
        {
            get { return mPageHeadCommand; }
        }

        private void PageHeadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Parameter == null) { return; }
                var strName = e.Parameter.ToString();
                switch (strName)
                {
                    case S1200Consts.PH_CMD_LOGOUT:
                        DoLogoutSystem();
                        break;
                    case S1200Consts.PH_CMD_LEFTPANEL:
                        DoCloseLeftPanel();
                        break;
                    case S1200Consts.PH_CMD_DEFAULTPAGE:
                        DoSetDefaultPage();
                        break;
                    case S1200Consts.PH_CMD_CHANGEPASSWORD:
                        DoChangePassword();
                        break;
                    case S1200Consts.PH_CMD_CHANGEROLE:
                        DoChangeRole();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = "UMP";

                TxtRoleName.Text = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", CurrentApp.Session.RoleID),
                    CurrentApp.Session.RoleInfo.Name);
                BtnNavigateHome.ToolTip = CurrentApp.GetLanguageInfo("Tip_Home", "Home");
                BtnOpenSetting.ToolTip = CurrentApp.GetLanguageInfo("Tip_Setting", "Setting");
                TxtThemeList.Text = CurrentApp.GetLanguageInfo("Label_Theme", "Themes");
                TxtLanguageList.Text = CurrentApp.GetLanguageInfo("Label_Language", "Languages");
                BtnLeftPanel.ToolTip = CurrentApp.GetLanguageInfo("Tip_CloseLeft", "LeftPanel");
                BtnDefaultPage.ToolTip = CurrentApp.GetLanguageInfo("Tip_SetDefaultPage", "Default Page");
                BtnChangePassword.ToolTip = CurrentApp.GetLanguageInfo("Tip_ChangePwd", "Change Password");
                BtnChangeRole.ToolTip = CurrentApp.GetLanguageInfo("Tip_ChangeRole", "Change Role");
                BtnLogout.ToolTip = CurrentApp.GetLanguageInfo("Tip_Logout", "Logout");

                for (int i = 0; i < mListModuleItems.Count; i++)
                {
                    var item = mListModuleItems[i];
                    item.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", item.ModuleID), item.Name);
                }
                for (int i = 0; i < mListModuleGroups.Count; i++)
                {
                    var item = mListModuleGroups[i];
                    item.Title = CurrentApp.GetLanguageInfo(string.Format("{0}Content", item.Name), item.Name);
                }

                for (int i = 0; i < mListLangInfoItems.Count; i++)
                {
                    var item = mListLangInfoItems[i];
                    var info = item.Info;
                    if (info != null)
                    {
                        item.Display = CurrentApp.GetLanguageInfo(string.Format("COML{0}", info.LangID), info.Display);
                    }
                }

                var lang = mListLangInfoItems.FirstOrDefault(t => t.Code == CurrentApp.Session.LangTypeID);
                if (lang != null)
                {
                    ListBoxLanguages.SelectedItem = lang;
                }
            }
            catch { }
        }

        #endregion

    }
}
