using System;
using System.Collections.Generic;
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

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCWizardDownloadConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardDownloadConfig : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public int DownloadNum;
        public UCWizardDownloadParameters NextPage;

        public UCWizardDownloadConfig()
        {
            InitializeComponent();

            DownloadNum = 0;
            this.Loaded += UCWizardDownloadConfig_Loaded;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButnNext.Click += ButnNext_Click;
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            DownloadNum++;
            //下一页，进入正式配置界面
            if (NextPage == null)
            {
                UCWizardDownloadParameters ucwizard = new UCWizardDownloadParameters();
                ucwizard.MainPage = MainPage;
                ucwizard.PrePage = this;
                ucwizard.CurrentApp = CurrentApp;
                ucwizard.WizardHelper = this.WizardHelper;
                ucwizard.DownLoadNumber = DownloadNum;
                MainPage.PopupPanel.Title = "Config Wizard";
                MainPage.PopupPanel.Content = ucwizard;
                MainPage.PopupPanel.IsOpen = true;
            }
            else
            {
                NextPage.DownLoadNumber = DownloadNum;
                NextPage.LoopLoad();
                MainPage.PopupPanel.Content = NextPage;
            }
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //下一页，进入回删归档参数配置界面
            CurrentApp.ShowInfoMessage("Over : )");
            MainPage.PopupPanel.IsOpen = false;
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            DownloadNum--;
            if (DownloadNum == -1)
            {
                MainPage.PopupPanel.Content = PrePage;
            }
            else
            {
                NextPage.DownLoadNumber = DownloadNum;
                NextPage.LoopLoad();
                MainPage.PopupPanel.Content = NextPage;
            }
        }

        private void UCWizardDownloadConfig_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ15001", "Config Wizard");
            this.LabIsConfig.Content = string.Format(CurrentApp.GetLanguageInfo("1110WIZ15002", "已配置{0}个下载参数，是否需要配置下一个下载参数？"), DownloadNum.ToString());

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0006", "确定");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0007", "取消");
        }

    }
}
