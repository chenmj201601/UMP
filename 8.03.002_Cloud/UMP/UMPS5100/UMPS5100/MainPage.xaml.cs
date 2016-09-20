using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Common;
using UMPS5100.MainUserControls;

namespace UMPS5100
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage
    {
        private BackgroundWorker mWorker;

        public MainPage()
        {
            InitializeComponent();
        }

        public void Load_Page()
        {
            switch (S5100App.CurrentLoadingModule)
            {
                case S5100App.S5100Module.BookmarkLevel:
                    CurrentApp.WriteLog("BookmarkLevel -   Init() BookmarkLevel");
                    grdMain.Children.Clear();
                    UC_BookmarkLevel ucBookmarkLevel = new UC_BookmarkLevel();
                    ucBookmarkLevel.CurrentApp = CurrentApp;
                    //PageHead.AppName = CurrentApp.GetLanguageInfo("5101002", "Bookmark levels management");
                    grdMain.Children.Add(ucBookmarkLevel);
                    break;
                case S5100App.S5100Module.KeyWord:
                    CurrentApp.WriteLog("KeyWord -   Init() KeyWord");
                    grdMain.Children.Clear();
                    UC_KeyWorld ucKeyWorld = new UC_KeyWorld();
                    ucKeyWorld.CurrentApp = CurrentApp;
                    ucKeyWorld.parentWin = this;
                    // PageHead.AppName = CurrentApp.GetLanguageInfo("5102003", "Keyword management");
                    grdMain.Children.Add(ucKeyWorld);
                    break;
                //case CurrentApp.S2400Module.KeyRemindSetting:
                //    grdMain.Children.Clear();
                //    UC_EncryptMailSetting ucRemindSetting = new UC_EncryptMailSetting();
                //    ucRemindSetting.parentWin = this;
                //    PageHead.AppName = CurrentApp.GetLanguageInfo("2404H002", "UMP key update notification settings");
                //    grdMain.Children.Add(ucRemindSetting);
                //    break;
                //case CurrentApp.S2400Module.ServerPolicyBinding:
                //    grdMain.Children.Clear();
                //    UC_EncryptionPolicyBindding ucBinding = new UC_EncryptionPolicyBindding();
                //    ucBinding.parentWin = this;
                //    PageHead.AppName = CurrentApp.GetLanguageInfo("2403H001", "UMP加密策略綁定");
                //    grdMain.Children.Add(ucBinding);
                //    break;
                default:
                    CurrentApp.WriteLog("BookmarkLevel -   Init() BookmarkLevel");
                    grdMain.Children.Clear();
                    ucBookmarkLevel = new UC_BookmarkLevel();
                    ucBookmarkLevel.CurrentApp = CurrentApp;
                    //PageHead.AppName = CurrentApp.GetLanguageInfo("5101002", "Bookmark levels management");
                    grdMain.Children.Add(ucBookmarkLevel);
                    break;
            }
        }

        #region Overried
        protected override void Init()
        {
            PageName = "SpeechAnalysisMainPage";
            StylePath = "UMPS5100/SpeechAnalysisPageResources.xaml";
            base.Init();
            CurrentApp.WriteLog("Init EncryptMainPage , CurrentLoadingModule = " + S5100App.CurrentLoadingModule);

            CurrentApp.SendLoadedMessage();
            Load_Page();
            S5100App.mainPage = this;
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Speech Analysis");
        }

        //protected override void set()
        //{
        //    base.SetDefaultPage();
        //    SetDefaultPage((int)CurrentApp.CurrentLoadingModule);
        //}

       
        private void ChangeUsercontrolThemes()
        {
            if (grdMain.Children.Count <= 0)
                return;
            switch (S5100App.CurrentLoadingModule)
            {
                case S5100App.S5100Module.BookmarkLevel:
                    UC_BookmarkLevel uc_bookmarkLevel = new UC_BookmarkLevel();
                    uc_bookmarkLevel.ChangeTheme();
                    break;
                case S5100App.S5100Module.KeyWord:
                    //UC_EncryptServersManagement ucEncryptServersManagement = new UC_EncryptServersManagement();
                    //ucEncryptServersManagement.ChangeTheme();
                    break;
                case S5100App.S5100Module.SpeechToTtext:
                    //UC_EncryptMailSetting ucRemindSetting = new UC_EncryptMailSetting();
                    //ucRemindSetting.ChangeTheme();
                    break;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Speech Analysis");
            if (grdMain.Children.Count > 0)
            {
                switch (S5100App.CurrentLoadingModule)
                {
                    case S5100App.S5100Module.BookmarkLevel:
                        UC_BookmarkLevel ucBookmarkLevel = grdMain.Children[0] as UC_BookmarkLevel;
                        if (ucBookmarkLevel == null)
                        {
                            return;
                        }
                        ucBookmarkLevel.ChangeLanguage();
                        //PageHead.AppName = CurrentApp.GetLanguageInfo("5101002", "Bookmark levels management");
                        break;
                    case S5100App.S5100Module.KeyWord:
                        UC_KeyWorld ucKeyWorld = grdMain.Children[0] as UC_KeyWorld;
                        if (ucKeyWorld == null)
                        {
                            return;
                        }
                        ucKeyWorld.ChangeLanguage();
                        //PageHead.AppName = CurrentApp.GetLanguageInfo("2402H006", "UMP encryption service management");
                        //CurrentApp.WriteLog("UC_EncryptServersManagement change language");
                        break;
                    case S5100App.S5100Module.SpeechToTtext:
                        //UC_EncryptMailSetting ucRemindSetting = grdMain.Children[0] as UC_EncryptMailSetting;
                        //ucRemindSetting.ChangeLanguage();
                        //PageHead.AppName = CurrentApp.GetLanguageInfo("2404H002", "UMP key alert");
                        //CurrentApp.WriteLog("UC_EncryptMailSetting change language");
                        break;
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
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
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

    }
}
