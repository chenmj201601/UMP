//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    26a45481-c735-45f6-8092-12573af104d8
//        CLR Version:              4.0.30319.42000
//        Name:                     PageHeadView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1203
//        File Name:                PageHeadView
//
//        created by Charley at 2016/1/22 12:06:33
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS1203
{
    /// <summary>
    /// PageHeadView.xaml 的交互逻辑
    /// </summary>
    public partial class PageHeadView
    {

        #region Memebers

        private ObservableCollection<ThemeInfoItem> mListThemeInfoItems;
        private ObservableCollection<LangInfoItem> mListLangInfoItems; 

        #endregion


        public PageHeadView()
        {
            InitializeComponent();

            mListLangInfoItems = new ObservableCollection<LangInfoItem>();
            mListThemeInfoItems = new ObservableCollection<ThemeInfoItem>();

            BtnLogout.Click += BtnLogout_Click;
            BtnOpenSetting.Click += BtnOpenSetting_Click;
            ListBoxLanguages.SelectionChanged += ListBoxLanguages_SelectionChanged;
            ListBoxThemes.SelectionChanged += ListBoxThemes_SelectionChanged;
        }

        protected override void Init()
        {
            try
            {
                ListBoxLanguages.ItemsSource = mListLangInfoItems;
                ListBoxThemes.ItemsSource = mListThemeInfoItems;

                PageName = "PageHeadView";
                StylePath = "UMPS1203/PageHeadView.xaml";

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                InitThemeInfoItems();
                InitLangInfoItems();
                InitUserInfo();

                ChangeTheme();
                ChangeLanguage();
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
                if(CurrentApp==null){return;}
                var session = CurrentApp.Session;
                if(session==null){return;}
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
                    item.Code = lang.LangID;
                    item.Display = lang.Display;
                    item.IsSelected = false;
                    mListLangInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnOpenSetting_Click(object sender, RoutedEventArgs e)
        {
            PopupSettings.IsOpen = true;
        }

        void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebRequest webRequest=new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) RequestCode.ACPageHeadLogout;
                CurrentApp.PublishEvent(webRequest);
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
                if (item == null) { return;}
                if (CurrentApp == null || CurrentApp.Session == null) { return;}
                var session = CurrentApp.Session;
                ThemeInfo themeInfo = session.SupportThemes.FirstOrDefault(t => t.Name == item.Name);
                if (themeInfo != null)
                {
                    ThemeInfo = themeInfo;
                    session.ThemeInfo = themeInfo;
                    session.ThemeName = themeInfo.Name;
                    WebRequest webRequest=new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int) RequestCode.CSThemeChange;
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
       
    }
}
