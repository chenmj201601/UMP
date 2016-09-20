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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using UMPS2400.MainUserControls;

namespace UMPS2400
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class EncryptMainPage
    {
        #region 变量定义
        private BackgroundWorker mWorker;
        #endregion

        public EncryptMainPage()
        {
            InitializeComponent();
        }

        protected override void Init()
        {
            PageName = "ResourceMainPage";
            StylePath = "UMPS2400/EncryptMainPageResources.xaml";
            base.Init();
            CurrentApp.WriteLog("Init EncryptMainPage , CurrentLoadingModule = " + S2400App.CurrentLoadingModule);

            CurrentApp.SendLoadedMessage();

            Load_Page();
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Encryption management");
        }

        public void Load_Page()
        {
            switch (S2400App.CurrentLoadingModule)
            {
                case S2400App.S2400Module.EncryptionPolicy:
                    CurrentApp.WriteLog("EncryptMainPage -   Init() EncryptionPolicy");
                    grdMain.Children.Clear();
                    UC_EncryptionPolicyManagement ucEncryptionPolicy = new UC_EncryptionPolicyManagement();
                    ucEncryptionPolicy.CurrentApp = CurrentApp;
                    ucEncryptionPolicy.parentWin = this;
                    // PageHead.AppName = CurrentApp.GetLanguageInfo("2402H006", "UMP encryption policy management");
                    grdMain.Children.Add(ucEncryptionPolicy);
                    CurrentApp.WriteLog("EncryptMainPage -   Init() EncryptionPolicy end");
                    break;
                case S2400App.S2400Module.KeyGenServer:
                    CurrentApp.WriteLog("EncryptMainPage -   Init() KeyGenServer");
                    grdMain.Children.Clear();
                    UC_EncryptServersManagement ucEncryptServersManagement = new UC_EncryptServersManagement();
                    ucEncryptServersManagement.CurrentApp = CurrentApp;
                    ucEncryptServersManagement.parentWin = this;
                  //  PageHead.AppName = CurrentApp.GetLanguageInfo("2401H001", "UMP encryption service management");
                    grdMain.Children.Add(ucEncryptServersManagement);
                    CurrentApp.WriteLog("EncryptMainPage -   Init() KeyGenServer end");
                    break;
                case S2400App.S2400Module.KeyRemindSetting:
                    grdMain.Children.Clear();
                    UC_EncryptMailSetting ucRemindSetting = new UC_EncryptMailSetting();
                    ucRemindSetting.CurrentApp = CurrentApp;
                    ucRemindSetting.parentWin = this;
                    //PageHead.AppName = CurrentApp.GetLanguageInfo("2404H002", "UMP key update notification settings");
                    grdMain.Children.Add(ucRemindSetting);
                    break;
                case S2400App.S2400Module.ServerPolicyBinding:
                    grdMain.Children.Clear();
                    UC_EncryptionPolicyBindding ucBinding = new UC_EncryptionPolicyBindding();
                    ucBinding.CurrentApp = CurrentApp;
                    ucBinding.parentWin = this;
                    //PageHead.AppName = CurrentApp.GetLanguageInfo("2403H001", "UMP加密策略綁定");
                    grdMain.Children.Add(ucBinding);
                    break;
                default:
                    CurrentApp.WriteLog("EncryptMainPage -   Init() default");
                    grdMain.Children.Clear();
                    ucEncryptServersManagement = new UC_EncryptServersManagement();
                    ucEncryptServersManagement.CurrentApp = CurrentApp;
                    ucEncryptServersManagement.parentWin = this;
                   // PageHead.AppName = CurrentApp.GetLanguageInfo("2401H001", "UMP encryption service management");
                    grdMain.Children.Add(ucEncryptServersManagement);
                    break;
            }
        }

        public void ShowStausMessage(string strMsg, bool bIsShow)
        {
            SetBusy(bIsShow, strMsg);
        }

        private void ChangeUsercontrolThemes()
        {
            if (grdMain.Children.Count <= 0)
                return;
            switch (S2400App.CurrentLoadingModule)
            {
                case S2400App.S2400Module.EncryptionPolicy:
                    UC_EncryptionPolicyManagement ucEncryptionPolicy = grdMain.Children[0] as UC_EncryptionPolicyManagement;
                    ucEncryptionPolicy.CurrentApp = CurrentApp;
                    ucEncryptionPolicy.PopupPanel.ChangeTheme();
                    ucEncryptionPolicy.ChangeTheme();
                    break;
                case S2400App.S2400Module.KeyGenServer:
                    UC_EncryptServersManagement ucEncryptServersManagement = new UC_EncryptServersManagement();
                    ucEncryptServersManagement.CurrentApp = CurrentApp;
                    ucEncryptServersManagement.PopupPanel.ChangeTheme();
                    ucEncryptServersManagement.ChangeTheme();
                    break;
                case S2400App.S2400Module.KeyRemindSetting:
                    UC_EncryptMailSetting ucRemindSetting = new UC_EncryptMailSetting();
                    ucRemindSetting.CurrentApp = CurrentApp;
                    ucRemindSetting.ChangeTheme();
                    break;
                case S2400App.S2400Module.ServerPolicyBinding:
                    UC_EncryptionPolicyBindding ucBinding = new UC_EncryptionPolicyBindding();
                    ucBinding.CurrentApp = CurrentApp;
                    ucBinding.ChangeTheme();
                    break;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Encryption management");
            if (grdMain.Children.Count > 0)
            {
                if (grdMain.Children[0] == null)
                {
                    return;
                }
                switch (S2400App.CurrentLoadingModule)
                {
                    case S2400App.S2400Module.EncryptionPolicy:
                        UC_EncryptionPolicyManagement ucEncryptionPolicy = grdMain.Children[0] as UC_EncryptionPolicyManagement;
                        if (ucEncryptionPolicy == null)
                        {
                            return;
                        }
                        ucEncryptionPolicy.CurrentApp = CurrentApp;
                        ucEncryptionPolicy.ChangeLanguage();
                        //PageHead.AppName = CurrentApp.GetLanguageInfo("2401H001", "UMP encryption service management");
                        CurrentApp.WriteLog("UC_EncryptionPolicyManagement change language");
                        break;
                    case S2400App.S2400Module.KeyGenServer:
                      //  UC_EncryptServersManagement ucEncryptServersManagement = grdMain.Children[1] as UC_EncryptServersManagement;
                        UC_EncryptServersManagement ucEncryptServersManagement = grdMain.Children[0] as UC_EncryptServersManagement;
                        if (ucEncryptServersManagement == null)
                        {
                            return;
                        }
                        ucEncryptServersManagement.CurrentApp = CurrentApp;
                        ucEncryptServersManagement.ChangeLanguage();
                       // PageHead.AppName = CurrentApp.GetLanguageInfo("2402H006", "UMP encryption service management");
                        CurrentApp.WriteLog("UC_EncryptServersManagement change language");
                        break;
                    case S2400App.S2400Module.KeyRemindSetting:
                      //  UC_EncryptMailSetting ucRemindSetting = grdMain.Children[2] as UC_EncryptMailSetting;
                        UC_EncryptMailSetting ucRemindSetting = grdMain.Children[0] as UC_EncryptMailSetting;
                        if (ucRemindSetting == null)
                        {
                            return;
                        }
                        ucRemindSetting.CurrentApp = CurrentApp;
                        ucRemindSetting.ChangeLanguage();
                       // PageHead.AppName = CurrentApp.GetLanguageInfo("2404H002", "UMP key alert");
                        CurrentApp.WriteLog("UC_EncryptMailSetting change language");
                        break;
                    case S2400App.S2400Module.ServerPolicyBinding:
                      //  UC_EncryptionPolicyBindding ucEncryptionPolicyBindding = grdMain.Children[3] as UC_EncryptionPolicyBindding;
                        UC_EncryptionPolicyBindding ucEncryptionPolicyBindding = grdMain.Children[0] as UC_EncryptionPolicyBindding;
                        if (ucEncryptionPolicyBindding == null)
                        {
                            return;
                        }
                        ucEncryptionPolicyBindding.CurrentApp = CurrentApp;
                        ucEncryptionPolicyBindding.ChangeLanguage();
                      //  PageHead.AppName = CurrentApp.GetLanguageInfo("2403001", "UMP policy binding");
                        CurrentApp.WriteLog("UC_EncryptionPolicyBindding change language");
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
                    //ShowException("1" + ex.Message);
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
    }
}
