//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    667b88b7-6d70-4442-bc1d-2ef7d520a62a
//        CLR Version:              4.0.30319.18444
//        Name:                     UMPPageHead
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPPageHead
//
//        created by Charley at 2014/8/22 16:20:07
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls.Models;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 页面头部
    /// </summary>
    public class UMPPageHead : UMPUserControl
    {
        #region Constructors

        static UMPPageHead()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UMPPageHead),
                new FrameworkPropertyMetadata(typeof(UMPPageHead)));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
                new CommandBinding(ApplicationCommands.Open,
                    ShowSettingCommand_Executed,
                    ShowSettingCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
                new CommandBinding(OpenCloseLeftPanelCommand,
                    OpenCloseLeftPanelCommand_Executed,
                    OpenCloseLeftPanelCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
                new CommandBinding(LogoutCommand,
                    LogoutCommand_Executed,
                    LogoutCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
                new CommandBinding(ChangePasswordCommand,
                    ChangePasswordCommand_Executed,
                    ChangePasswordCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
               new CommandBinding(NavigateHomeCommand,
                   NavigateHomeCommand_Executed,
                   NavigateHomeCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
              new CommandBinding(SetDefaultPageCommand,
                  SetDefaultPageCommand_Executed,
                  SetDefaultPageCommand_CanExecute));

            CommandManager.RegisterClassCommandBinding(typeof(UMPPageHead),
             new CommandBinding(OpenIMCommand,
                 OpenIMCommand_Executed,
                 OpenIMCommand_CanExecute));
        }

        public UMPPageHead()
        {
            ListThemes = new ObservableCollection<ThemeInfoItem>();
            ListLanugages = new ObservableCollection<LangInfoItem>();

            Loaded += UMPPageHead_Loaded;
        }

        void UMPPageHead_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #endregion


        #region PageHeadTypeProperty

        public static readonly DependencyProperty PageHeadTypeProperty =
            DependencyProperty.Register("PageHeadType", typeof(PageHeadType), typeof(UMPPageHead), new PropertyMetadata(PageHeadType.Default, OnPageHeadTypeChanged));

        public PageHeadType PageHeadType
        {
            get { return (PageHeadType)GetValue(PageHeadTypeProperty); }
            set { SetValue(PageHeadTypeProperty, value); }
        }

        private static void OnPageHeadTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                pageHead.OnPageHeadTypeChanged((PageHeadType)e.OldValue, (PageHeadType)e.NewValue);
            }
        }

        public void OnPageHeadTypeChanged(PageHeadType oldValue, PageHeadType newValue)
        {
            InitInfo();
        }

        #endregion


        #region SessionInfo

        public static readonly DependencyProperty SessionInfoProperty =
            DependencyProperty.Register("SessionInfo", typeof(SessionInfo), typeof(UMPPageHead), new PropertyMetadata(default(SessionInfo)));

        public SessionInfo SessionInfo
        {
            get { return (SessionInfo)GetValue(SessionInfoProperty); }
            set { SetValue(SessionInfoProperty, value); }
        }

        #endregion


        #region LogoProperty

        public static readonly DependencyProperty LogoProperty =
            DependencyProperty.Register("Logo", typeof(ImageSource), typeof(UMPPageHead), new PropertyMetadata(default(ImageSource)));

        public ImageSource Logo
        {
            get { return (ImageSource)GetValue(LogoProperty); }
            set { SetValue(LogoProperty, value); }
        }

        #endregion


        #region AppNamePropety

        public static readonly DependencyProperty AppNameProperty =
            DependencyProperty.Register("AppName", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }

        #endregion


        #region UserNameProperty

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        #endregion


        #region RoleName

        public static readonly DependencyProperty RoleNameProperty =
            DependencyProperty.Register("RoleName", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string RoleName
        {
            get { return (string)GetValue(RoleNameProperty); }
            set { SetValue(RoleNameProperty, value); }
        }

        #endregion


        #region HeadIconProperty

        public static readonly DependencyProperty HeadIconProperty =
            DependencyProperty.Register("HeadIcon", typeof(ImageSource), typeof(UMPPageHead), new PropertyMetadata(default(ImageSource)));

        public ImageSource HeadIcon
        {
            get { return (ImageSource)GetValue(HeadIconProperty); }
            set { SetValue(HeadIconProperty, value); }
        }

        #endregion


        #region MsgCount

        public static readonly DependencyProperty MsgCountProperty =
            DependencyProperty.Register("MsgCount", typeof (string), typeof (UMPPageHead), new PropertyMetadata(default(string)));

        public string MsgCount
        {
            get { return (string) GetValue(MsgCountProperty); }
            set { SetValue(MsgCountProperty, value); }
        }

        #endregion


        #region IsMsgCountVisible

        public static readonly DependencyProperty IsMsgCountVisibleProperty =
            DependencyProperty.Register("IsMsgCountVisible", typeof (bool), typeof (UMPPageHead), new PropertyMetadata(default(bool)));

        public bool IsMsgCountVisible
        {
            get { return (bool) GetValue(IsMsgCountVisibleProperty); }
            set { SetValue(IsMsgCountVisibleProperty, value); }
        }

        #endregion


        #region PopupSetting

        private const string PART_PopupSetting = "PART_PopupSetting";

        private Popup mPopupSetting;

        private static void ShowSettingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = (UMPPageHead)sender;
            if (pageHead != null)
            {
                pageHead.ShowSetting();
            }
        }

        private static void ShowSettingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            UMPPageHead pageHead = (UMPPageHead)sender;
            if (pageHead != null)
            {
                e.CanExecute = pageHead.IsSettingEnable;
            }
        }

        public void ShowSetting()
        {
            if (mPopupSetting != null)
            {
                mPopupSetting.IsOpen = true;
            }
        }

        #endregion


        #region Members

        private bool mIsSettingEnable = true;

        public bool IsSettingEnable
        {
            get { return mIsSettingEnable; }
            set { mIsSettingEnable = value; }
        }

        private bool mIsSetDefaultPageEnable = true;

        public bool IsSetDefaultPageEnable
        {
            get { return mIsSetDefaultPageEnable; }
            set { mIsSetDefaultPageEnable = value; }
        }

        #endregion


        #region Themes

        public static readonly DependencyProperty ListThemesProperty =
            DependencyProperty.Register("ListThemes", typeof(ObservableCollection<ThemeInfoItem>), typeof(UMPPageHead), new PropertyMetadata(default(ObservableCollection<ThemeInfoItem>)));

        public ObservableCollection<ThemeInfoItem> ListThemes
        {
            get { return (ObservableCollection<ThemeInfoItem>)GetValue(ListThemesProperty); }
            set { SetValue(ListThemesProperty, value); }
        }

        private void InitThemes()
        {
            ListThemes.Clear();

            if (SessionInfo != null && SessionInfo.SupportThemes != null)
            {
                for (int i = 0; i < SessionInfo.SupportThemes.Count; i++)
                {
                    ThemeInfo themeInfo = SessionInfo.SupportThemes[i];

                    ThemeInfoItem item = new ThemeInfoItem();
                    item.Name = themeInfo.Name;
                    item.Display = themeInfo.Display;
                    item.ThumbImage = GetThumbImage(item);
                    ListThemes.Add(item);
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

        private const string PART_ListBoxThemes = "PART_ListBoxThemes";
        private ListBox mListBoxThemes;

        void mListBoxThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems == null || e.RemovedItems.Count <= 0) { return; }
            var item = mListBoxThemes.SelectedItem as ThemeInfoItem;
            if (item != null)
            {
                ThemeInfo themeInfo = SessionInfo.SupportThemes.FirstOrDefault(t => t.Name == item.Name);
                if (themeInfo != null)
                {
                    ThemeInfo = themeInfo;
                    SessionInfo.ThemeInfo = themeInfo;
                    SessionInfo.ThemeName = themeInfo.Name;
                    PageHeadEventArgs args = new PageHeadEventArgs();
                    args.Code = 100;
                    args.Data = themeInfo;
                    OnPageHeadEvent(args);
                }
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
                        , "CommonControls/UMPPageHead.xaml");
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


        #region Languages

        public static readonly DependencyProperty ListLanugagesProperty =
            DependencyProperty.Register("ListLanugages", typeof(ObservableCollection<LangInfoItem>), typeof(UMPPageHead), new PropertyMetadata(default(ObservableCollection<LangInfoItem>)));

        public ObservableCollection<LangInfoItem> ListLanugages
        {
            get { return (ObservableCollection<LangInfoItem>)GetValue(ListLanugagesProperty); }
            set { SetValue(ListLanugagesProperty, value); }
        }

        private void InitLangTypes()
        {
            ListLanugages.Clear();
            if (SessionInfo != null && SessionInfo.SupportLangTypes != null)
            {
                for (int i = 0; i < SessionInfo.SupportLangTypes.Count; i++)
                {
                    LangTypeInfo langType = SessionInfo.SupportLangTypes[i];
                    LangInfoItem langInfoItem = new LangInfoItem();
                    langInfoItem.Code = langType.LangID;
                    langInfoItem.Display = langType.Display;
                    ListLanugages.Add(langInfoItem);
                }
            }
        }

        private const string PART_ListBoxLangs = "PART_ListBoxLangs";
        private ListBox mListBoxLangs;

        void mListBoxLangs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems == null || e.RemovedItems.Count <= 0) { return; }
            var langItem = mListBoxLangs.SelectedItem as LangInfoItem;
            if (langItem != null)
            {
                LangTypeInfo langTypeInfo = SessionInfo.SupportLangTypes.FirstOrDefault(l => l.LangID == langItem.Code);
                if (langTypeInfo != null)
                {
                    LangTypeInfo = langTypeInfo;
                    SessionInfo.LangTypeInfo = langTypeInfo;
                    SessionInfo.LangTypeID = langTypeInfo.LangID;
                    ChangeLanguage();
                    PageHeadEventArgs args = new PageHeadEventArgs();
                    args.Code = 110;
                    args.Data = langTypeInfo;
                    OnPageHeadEvent(args);
                }
            }
        }

        private List<LanguageInfo> mListLanguageInfos = new List<LanguageInfo>();

        private void InitLanguageInfos()
        {
            try
            {
                mListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = SessionInfo;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("11"));
                webRequest.ListData.Add(string.Format("0"));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                if (SessionInfo == null) { return; }
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(SessionInfo)
                    , WebHelper.CreateEndpointAddress(SessionInfo.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    mListLanguageInfos.Add(langInfo);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        public string GetLanguageInfo(string name, string display)
        {
            LanguageInfo lang =
                mListLanguageInfos.FirstOrDefault(l => l.LangID == SessionInfo.LangTypeInfo.LangID && l.Name == name);
            if (lang == null)
            {
                return display;
            }
            return lang.Display;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            //ToolTipHome = GetLanguageInfo("Tip_Home", "Home");
            ToolTipHome = GetLanguageInfo("Tip_Logout", "Logout");
            ToolTipSetting = GetLanguageInfo("Tip_Setting", "Setting");
            ToolTipChangPwd = GetLanguageInfo("Tip_ChangePwd", "Change Password");
            ToolTipCloseLeft = GetLanguageInfo("Tip_CloseLeft", "Open or close left panel");
            ToolTipLogout = GetLanguageInfo("Tip_Logout", "Logout");
            ToolTipExit = GetLanguageInfo("Tip_Exist", "Exist");
            //ToolTipSetDefaultPage = GetLanguageInfo("Tip_SetDefaultPage", "Set default page");
            LabelTheme = GetLanguageInfo("Label_Theme", "Theme");
            LabelLanguage = GetLanguageInfo("Label_Language", "Language");

            for (int i = 0; i < ListThemes.Count; i++)
            {
                ListThemes[i].Display = GetLanguageInfo(string.Format("Theme_{0}", ListThemes[i].Name),
                    ListThemes[i].Name);
            }
        }

        #endregion


        #region Init

        private void Init()
        {
            InitLanguageInfos();
            InitThemes();
            if (ThemeInfo != null)
            {
                ThemeInfoItem item = ListThemes.FirstOrDefault(t => t.Name == ThemeInfo.Name);
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
            InitLangTypes();
            if (LangTypeInfo != null)
            {
                LangInfoItem lang = ListLanugages.FirstOrDefault(l => l.Code == LangTypeInfo.LangID);
                if (lang != null)
                {
                    lang.IsSelected = true;
                }
            }
        }

        public void InitInfo()
        {
            if (SessionInfo != null)
            {
                ThemeInfo = SessionInfo.ThemeInfo;
                LangTypeInfo = SessionInfo.LangTypeInfo;
                AppServerInfo = SessionInfo.AppServerInfo;
                UserName = SessionInfo.UserInfo.UserName;
                RoleName = SessionInfo.RoleInfo.Name;
                GetHeadIcon(SessionInfo.UserInfo);
                GetLogoIcon();
                Init();
                ChangeLanguage();
                ChangeTheme();
            }
        }

        private void GetHeadIcon(UserInfo userInfo)
        {
            try
            {
                string path = string.Format("{0}://{1}:{2}/HeadIcons/0.bmp"
                              , AppServerInfo.Protocol
                              , AppServerInfo.Address
                              , AppServerInfo.Port);
                if (userInfo != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = SessionInfo;
                    webRequest.Code = (int)RequestCode.WSGetResourceProperty;
                    webRequest.ListData.Add("001");
                    webRequest.ListData.Add("102HeadIcon");
                    webRequest.ListData.Add("1");
                    webRequest.ListData.Add(userInfo.UserID.ToString());
                    Service11012Client client = new Service11012Client(
                        WebHelper.CreateBasicHttpBinding(SessionInfo),
                        WebHelper.CreateEndpointAddress(
                            SessionInfo.AppServerInfo,
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
                HeadIcon = image;
            }
            catch { }
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
                Logo = image;
            }
            catch
            {

            }
        }

        private void ShowMessage(string msg)
        {
            ThreadPool.QueueUserWorkItem(
                a => MessageBox.Show(string.Format("{0}", msg), "UMP", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        #endregion


        #region Templates

        private const string PART_ButtonOpenIM = "PART_ButtonOpenIM";

        private Button mButtonOpenIM;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mPopupSetting = GetTemplateChild(PART_PopupSetting) as Popup;
            if (mPopupSetting != null)
            {

            }
            mListBoxThemes = GetTemplateChild(PART_ListBoxThemes) as ListBox;
            if (mListBoxThemes != null)
            {
                mListBoxThemes.SelectionChanged += mListBoxThemes_SelectionChanged;
            }
            mListBoxLangs = GetTemplateChild(PART_ListBoxLangs) as ListBox;
            if (mListBoxLangs != null)
            {
                mListBoxLangs.SelectionChanged += mListBoxLangs_SelectionChanged;
            }

            mButtonOpenIM = GetTemplateChild(PART_ButtonOpenIM) as Button;
            if (mButtonOpenIM != null)
            {
                mButtonOpenIM.DataContext = this;
            }
        }

        #endregion


        #region PageHeadEvent

        public event EventHandler<PageHeadEventArgs> PageHeadEvent;

        protected virtual void OnPageHeadEvent(PageHeadEventArgs e)
        {
            if (PageHeadEvent != null)
            {
                PageHeadEvent(this, e);
            }
        }

        #endregion


        #region ToolTips and Label

        public static readonly DependencyProperty ToolTipHomeProperty =
            DependencyProperty.Register("ToolTipHome", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipHome
        {
            get { return (string)GetValue(ToolTipHomeProperty); }
            set { SetValue(ToolTipHomeProperty, value); }
        }

        public static readonly DependencyProperty ToolTipSettingProperty =
            DependencyProperty.Register("ToolTipSetting", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipSetting
        {
            get { return (string)GetValue(ToolTipSettingProperty); }
            set { SetValue(ToolTipSettingProperty, value); }
        }

        public static readonly DependencyProperty ToolTipChangPwdProperty =
            DependencyProperty.Register("ToolTipChangPwd", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipChangPwd
        {
            get { return (string)GetValue(ToolTipChangPwdProperty); }
            set { SetValue(ToolTipChangPwdProperty, value); }
        }

        public static readonly DependencyProperty ToolTipCloseLeftProperty =
            DependencyProperty.Register("ToolTipCloseLeft", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipCloseLeft
        {
            get { return (string)GetValue(ToolTipCloseLeftProperty); }
            set { SetValue(ToolTipCloseLeftProperty, value); }
        }

        public static readonly DependencyProperty ToolTipLogoutProperty =
            DependencyProperty.Register("ToolTipLogout", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipLogout
        {
            get { return (string)GetValue(ToolTipLogoutProperty); }
            set { SetValue(ToolTipLogoutProperty, value); }
        }

        public static readonly DependencyProperty ToolTipExitProperty =
            DependencyProperty.Register("ToolTipExit", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string ToolTipExit
        {
            get { return (string)GetValue(ToolTipExitProperty); }
            set { SetValue(ToolTipExitProperty, value); }
        }

        //public static readonly DependencyProperty ToolTipSetDefaultPageProperty =
        //   DependencyProperty.Register("ToolTipSetDefaultPage", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        //public string ToolTipSetDefaultPage
        //{
        //    get { return (string)GetValue(ToolTipSetDefaultPageProperty); }
        //    set { SetValue(ToolTipSetDefaultPageProperty, value); }
        //}

        public static readonly DependencyProperty LabelThemeProperty =
            DependencyProperty.Register("LabelTheme", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string LabelTheme
        {
            get { return (string)GetValue(LabelThemeProperty); }
            set { SetValue(LabelThemeProperty, value); }
        }

        public static readonly DependencyProperty LabelLanguageProperty =
            DependencyProperty.Register("LabelLanguage", typeof(string), typeof(UMPPageHead), new PropertyMetadata(default(string)));

        public string LabelLanguage
        {
            get { return (string)GetValue(LabelLanguageProperty); }
            set { SetValue(LabelLanguageProperty, value); }
        }

        #endregion


        #region OpenCloseLeftPanelCommand

        private static RoutedUICommand mOpenCloseLeftPanelCommand = new RoutedUICommand();

        public static RoutedUICommand OpenCloseLeftPanelCommand
        {
            get { return mOpenCloseLeftPanelCommand; }
        }

        private static void OpenCloseLeftPanelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = 121;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void OpenCloseLeftPanelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        #region LogoutCommand

        private static RoutedUICommand mLogoutCommand = new RoutedUICommand();

        public static RoutedUICommand LogoutCommand
        {
            get { return mLogoutCommand; }
        }

        private static void LogoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = 201;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void LogoutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        #region SetDefaultPageCommand

        private static RoutedUICommand mSetDefaultPageCommand = new RoutedUICommand();

        public static RoutedUICommand SetDefaultPageCommand
        {
            get { return mSetDefaultPageCommand; }
        }

        private static void SetDefaultPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = PageHeadEventArgs.EVT_DEFAULT_PAGE;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void SetDefaultPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            UMPPageHead pageHead = (UMPPageHead)sender;
            if (pageHead != null)
            {
                e.CanExecute = pageHead.IsSetDefaultPageEnable;
            }
        }

        #endregion


        #region ChangePasswordCommand

        private static RoutedUICommand mChangePasswordCommand = new RoutedUICommand();

        public static RoutedUICommand ChangePasswordCommand
        {
            get { return mChangePasswordCommand; }
        }

        private static void ChangePasswordCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = 120;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void ChangePasswordCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        #region NavigateHomeCommand

        private static RoutedUICommand mNavigateHomeCommand = new RoutedUICommand();

        public static RoutedUICommand NavigateHomeCommand
        {
            get { return mNavigateHomeCommand; }
        }

        private static void NavigateHomeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = 202;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void NavigateHomeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        #region OpenIMCommand

        private static RoutedUICommand mOpenIMCommand = new RoutedUICommand();

        public static RoutedUICommand OpenIMCommand
        {
            get { return mOpenIMCommand; }
        }

        private static void OpenIMCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UMPPageHead pageHead = sender as UMPPageHead;
            if (pageHead != null)
            {
                PageHeadEventArgs args = new PageHeadEventArgs();
                args.Code = PageHeadEventArgs.EVT_OPENIM;
                pageHead.OnPageHeadEvent(args);
            }
        }

        private static void OpenIMCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion
    }
}
