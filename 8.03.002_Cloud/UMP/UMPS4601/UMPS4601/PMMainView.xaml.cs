using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace UMPS4601
{
    /// <summary>
    /// PMMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class PMMainView
    {
        #region
        private BackgroundWorker mWorker;
        #endregion

        public PMMainView()
        {
            InitializeComponent();
        }

        protected override void Init()
        {
            PageName = "PMMainView";
            StylePath = "UMPS4601/PMMainPage.xaml";
            base.Init();
            //App.WriteLog("Init EncryptMainPage , CurrentLoadingModule = " + App.CurrentLoadingModule);

            //SendLoadedMessage();
            Load_Page();
            SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
            //App.PMMainPage = this;

            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                SetBusy(false, string.Empty);
                ChangeLanguage();
            };
            mWorker.RunWorkerAsync();
        }

        public void Load_Page()
        {
            //测试用的  可以注释 
            if (CurrentApp.StartArgs == null)
            {
                CurrentApp.StartArgs = "4602";
            }

            switch (CurrentApp.StartArgs)
            {
                case "4601":
                    //App.WriteLog("EncryptMainPage -   Init() EncryptionPolicy");
                    MainGrid.Children.Clear();
                    UC_PMParameterBindingPage ucPMParameterBindingPage = new UC_PMParameterBindingPage();
                    ucPMParameterBindingPage.CurrentApp = CurrentApp;
                    ucPMParameterBindingPage.ParentPage = this;
                    //PageHead.AppName = App.GetLanguageInfo("4601", "UMP PM Parameter Binding");
                    MainGrid.Children.Add(ucPMParameterBindingPage);
                    break;
                case "4602":
                    MainGrid.Children.Clear();
                    ProductManagementShow PmShow = new ProductManagementShow();
                    PmShow.CurrentApp = CurrentApp;
                    PmShow.ParentPage = this;
                    //PageHead.AppName = App.GetLanguageInfo("4602", "UMP PM Analysis");
                    MainGrid.Children.Add(PmShow);
                    break;
                case "4603":
                    MainGrid.Children.Clear();
                    UC_KPIInfoShowPage ucKPIInfoShowPage = new UC_KPIInfoShowPage();
                    ucKPIInfoShowPage.CurrentApp = CurrentApp;
                    ucKPIInfoShowPage.ParentPage = this;
                    //PageHead.AppName = App.GetLanguageInfo("4603", "UMP KPI Detail");
                    MainGrid.Children.Add(ucKPIInfoShowPage);
                    break;
            }
        }

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                if (CurrentApp.StartArgs == null)
                {
                    CurrentApp.StartArgs = "4602";
                }
                if (MainGrid.Children.Count > 0)
                {
                    switch (CurrentApp.StartArgs)
                    {
                        case "4601":
                            UC_PMParameterBindingPage ucPMParameterBindingPage = MainGrid.Children[0] as UC_PMParameterBindingPage;
                            ucPMParameterBindingPage.CurrentApp = CurrentApp;
                            ucPMParameterBindingPage.ChangeLanguage();
                            //PageHead.AppName = App.GetLanguageInfo("4601", "UMP PM Parameter Binding");
                            CurrentApp.WriteLog("UC_EncryptionPolicyManagement change language");
                            break;
                        case "4602":
                            ProductManagementShow productManagementShow = MainGrid.Children[0] as ProductManagementShow;
                            productManagementShow.CurrentApp = CurrentApp;
                            productManagementShow.ChangeLanguage();
                            //PageHead.AppName = App.GetLanguageInfo("4602", "UMP PM Analysis");
                            CurrentApp.WriteLog("UC_EncryptionPolicyManagement change language");
                            break;
                        case "4603":
                            UC_KPIInfoShowPage ucKPIInfoShowPage = MainGrid.Children[0] as UC_KPIInfoShowPage;
                            ucKPIInfoShowPage.CurrentApp = CurrentApp;
                            ucKPIInfoShowPage.ChangeLanguage();
                            //PageHead.AppName = App.GetLanguageInfo("4603", "UMP PM KPI SHOW");
                            CurrentApp.WriteLog("UC_EncryptionPolicyManagement change language");
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("ChangeLang", string.Format("ChangeLang fail.\t{0}", ex.Message));
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
                    //App.ShowExceptionMessage("1" + ex.Message);
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
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }
        }

    }
}
