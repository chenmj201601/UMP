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
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_ServerConfig;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_LanguageInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UC_LanguageInfo : UserControl
    {
        public List<LanguageInfo> lstLanguageInfos;
        public UC_LanManager LanManagerWindow;

        //保存某一个messageid对应的所有语种的语言包
        public UC_LanguageInfo(List<LanguageInfo> lstLans, UC_LanManager _main)
        {
            InitializeComponent();
            lstLanguageInfos = lstLans;
            LanManagerWindow = _main;
            this.Loaded += UC_LanguageInfo_Loaded;
        }

        void UC_LanguageInfo_Loaded(object sender, RoutedEventArgs e)
        {
            InitControl();
        }

        /// <summary>
        /// 初始化界面元素
        /// </summary>
        public void InitControl()
        {
            spDisplayContent.Children.Clear();
            spTipContent.Children.Clear();
            lblMessageID.Content = string.Empty;
            lblModuleID.Content = string.Empty;
            lblChildModuleID.Content = string.Empty;
            lblInPageFrame.Content = string.Empty;

            if (lstLanguageInfos.Count <= 0)
                return;

            lblMessageID.Content = lstLanguageInfos[0].MessageID;
            lblModuleID.Content = lstLanguageInfos[0].ModuleID;
            lblChildModuleID.Content = lstLanguageInfos[0].ChildModuleID;
            lblInPageFrame.Content = lstLanguageInfos[0].InFrame;

            StackPanel spDisplay;
            Label lblLanguageCode;
            TextBlock txtDisplay;
            for (int i = 0; i < lstLanguageInfos.Count; i++)
            {
                lblLanguageCode = new Label();
               lblLanguageCode.Content = ServerConfigOperationInServer.GetLanguageItemInDBByMessageID(App.GCurrentUmpServer
                    , App.GCurrentDBServer, lstLanguageInfos[i].LanguageCode, lstLanguageInfos[i].LanguageCode);
                lblLanguageCode.Margin = new Thickness(10);
                txtDisplay = new TextBlock();
                txtDisplay.Tag = lstLanguageInfos[i].LanguageCode;
                txtDisplay.Background = Brushes.Transparent;
                txtDisplay.TextWrapping = TextWrapping.Wrap;
                txtDisplay.Text = lstLanguageInfos[i].DisplayMessage;
                txtDisplay.Margin = new Thickness(30, 0, 10, 0);
                spDisplay = new StackPanel();
                spDisplay.Children.Add(lblLanguageCode);
                spDisplay.Children.Add(txtDisplay);
                spDisplayContent.Children.Add(spDisplay);
            }

            StackPanel spTip;
            TextBlock txtTip;
            for (int i = 0; i < lstLanguageInfos.Count; i++)
            {
                lblLanguageCode = new Label();
                lblLanguageCode.Content  = ServerConfigOperationInServer.GetLanguageItemInDBByMessageID(App.GCurrentUmpServer
                   , App.GCurrentDBServer, lstLanguageInfos[i].LanguageCode, lstLanguageInfos[i].LanguageCode);
                lblLanguageCode.Margin = new Thickness(10);
                txtTip = new TextBlock();
                txtTip.Tag = lstLanguageInfos[i].LanguageCode;
                txtTip.Background = Brushes.Transparent;
                txtTip.TextWrapping = TextWrapping.Wrap;
                txtTip.Text = lstLanguageInfos[i].TipMessage;
                txtTip.Margin = new Thickness(30, 0, 10, 0);
                spTip = new StackPanel();
                spTip.Children.Add(lblLanguageCode);
                spTip.Children.Add(txtTip);
                spTipContent.Children.Add(spTip);
            }
        }
    }
}
