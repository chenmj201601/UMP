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
using Common2400;
using PFShareClassesC;
using UMPS2400.Entries;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// UC_EncryptionPolicyTypeU.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptionPolicyTypeU
    {
        #region 变量定义
        //父窗口
        public UC_AddPolicy IPageParent = null;
        #endregion

        public UC_EncryptionPolicyTypeU()
        {
            InitializeComponent();
            this.Loaded += UC_EncryptionPolicyTypeU_Loaded;
            chkShowPasswordI.Checked += chkShowPasswordI_Checked;
            chkShowPasswordI.Unchecked += chkShowPasswordI_Unchecked;
            chkChangeKey.Checked += chkChangeKey_Checked;
            chkChangeKey.Unchecked += chkChangeKey_Unchecked;
            chkShowPasswordC.Checked += chkShowPasswordC_Checked;
            chkShowPasswordC.Unchecked += chkShowPasswordC_Unchecked;
            radEnableImmediately.Checked += radEnableImmediately_Checked;
            radEnableDate.Checked += radEnableDate_Checked;
            StartDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(ButtonControl_SelectedDateChanged);
        }

        void UC_EncryptionPolicyTypeU_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            InitControls();
        }

        #region Overried
        public override void ChangeLanguage()
        {
            InitLanguage();
        }
        #endregion

        #region Init
        private void InitLanguage()
        {
            LabelEncryptionKey.Content = CurrentApp.GetLanguageInfo("2402L019", "Encryption key");
            chkShowPasswordI.Content = CurrentApp.GetLanguageInfo("2402L020", "Display key");
            chkChangeKey.Content = CurrentApp.GetLanguageInfo("2402L021", "New key");
            chkShowPasswordC.Content = CurrentApp.GetLanguageInfo("2402L022", "Display key");
            radEnableDate.Content = CurrentApp.GetLanguageInfo("2402L023", "Come into effect");
            radEnableImmediately.Content = CurrentApp.GetLanguageInfo("2402L024", "Immediate effect");
        }

        private void InitControls()
        {
            if (IPageParent.iAddOrModify == (int)OperationType.Add)
            {
                TextPasswordHidden.IsEnabled = true;
                TextPasswordShow.IsEnabled = true;
                chkShowPasswordI.IsEnabled = true;

                TextChangePwdHidden.IsEnabled = false;
                TextChangePwdShow.IsEnabled = false;
                chkChangeKey.IsEnabled = false; chkChangeKey.IsChecked = false;
                chkShowPasswordC.IsEnabled = false;

                StartDate.IsEnabled = false;

                radEnableDate.IsEnabled = false; radEnableDate.IsChecked = false;
                radEnableImmediately.IsEnabled = false; radEnableImmediately.IsChecked = false;
            }
            else if (IPageParent.iAddOrModify == (int)OperationType.Modify)
            {
                chkChangeKey.IsEnabled = true;
                TextChangePwdHidden.IsEnabled = false;
                chkShowPasswordC.IsEnabled = true;
                StartDateTime.Text = S2400App.GolbalCurrentEncryptionDBTime.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00";
                if (IPageParent.mainPage.HasKeyGenServer)
                {
                    radEnableImmediately.Visibility = Visibility.Visible;
                }
                else
                {
                    radEnableImmediately.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 加密密钥旁边的“显示密钥”的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkShowPasswordI_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                TextPasswordShow.Visibility = Visibility.Visible;
                TextPasswordHidden.Visibility = Visibility.Hidden;
                TextPasswordShow.Text = TextPasswordHidden.Password;
            }
            catch { }
        }

        /// <summary>
        /// 加密密钥旁边的“显示密钥”的取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkShowPasswordI_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                TextPasswordShow.Visibility = Visibility.Hidden;
                TextPasswordHidden.Visibility = Visibility.Visible;
                TextPasswordHidden.Password = TextPasswordShow.Text;
            }
            catch { }
        }

        /// <summary>
        /// “新密钥”的取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkChangeKey_Unchecked(object sender, RoutedEventArgs e)
        {
            TextChangePwdShow.IsEnabled = false; chkShowPasswordC.IsEnabled = false; TextChangePwdHidden.IsEnabled = false;
            radEnableDate.IsEnabled = false; radEnableImmediately.IsEnabled = false;
            StartDateTime.IsEnabled = false;
        }

        /// <summary>
        /// “新密钥”的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkChangeKey_Checked(object sender, RoutedEventArgs e)
        {
            TextChangePwdShow.IsEnabled = true; chkShowPasswordC.IsEnabled = true; TextChangePwdHidden.IsEnabled = true;
            radEnableDate.IsEnabled = true; radEnableImmediately.IsEnabled = true;
            radEnableDate.IsChecked = true; StartDateTime.IsEnabled = true;
        }

        /// <summary>
        /// "新密钥"旁的“显示密钥”的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkShowPasswordC_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                TextChangePwdShow.Visibility = Visibility.Visible;
                TextChangePwdHidden.Visibility = Visibility.Hidden;
                TextChangePwdShow.Text = TextChangePwdHidden.Password;
            }
            catch { }
        }

        /// <summary>
        /// "新密钥"旁的“显示密钥”的取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkShowPasswordC_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                TextChangePwdShow.Visibility = Visibility.Hidden;
                TextChangePwdHidden.Visibility = Visibility.Visible;
                TextChangePwdHidden.Password = TextChangePwdShow.Text;
            }
            catch { }
        }

        /// <summary>
        /// “立即生效”的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radEnableImmediately_Checked(object sender, RoutedEventArgs e)
        {
            StartDateTime.IsEnabled = false;
        }

        /// <summary>
        /// “生效于”的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radEnableDate_Checked(object sender, RoutedEventArgs e)
        {
            StartDateTime.IsEnabled = true;
        }

        /// <summary>
        /// “生效于”日期控件的选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrDateTime = string.Empty;
            string LStrSenderName = string.Empty;

            try
            {
                Microsoft.Windows.Controls.DatePicker LDateTimeSender = sender as Microsoft.Windows.Controls.DatePicker;
                LStrSenderName = LDateTimeSender.Name;
                if (LStrSenderName == "StartDate" && StartDateTime.IsEnabled == true)
                {
                    LStrDateTime = StartDateTime.Text.Substring(11);
                    StartDateTime.Text = LDateTimeSender.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + LStrDateTime;
                }
                if (LStrSenderName == "StartDate" && StartDateTime.IsEnabled == false)
                {
                    LStrDateTime = StartDateTime.Text.Substring(0, 10) + " 00:00:00";
                    LDateTimeSender.SelectedDate = Convert.ToDateTime(LStrDateTime);
                }
            }
            catch { }
        }
        #endregion

        #region 父窗口调用的函数
        /// <summary>
        /// 将界面上的值写入EncryptionPolicy对象
        /// </summary>
        /// <returns></returns>
        public UMPEncryptionPolicy GetPolicyInTypeU(UMPEncryptionPolicy policy,PolicyUpdateEntry policyUpdate)
        {
            string strPwd = string.Empty;
            if (IPageParent.iAddOrModify == (int)OperationType.Add)
            {
                if (chkShowPasswordI.IsChecked == true)
                {
                    strPwd = TextPasswordShow.Text;
                }
                else
                {
                    strPwd = TextPasswordHidden.Password;
                }
                
            }
            else if (IPageParent.iAddOrModify == (int)OperationType.Modify)
            {
                if (chkChangeKey.IsChecked == true)
                {
                    if (chkShowPasswordC.IsChecked == true) { strPwd = TextChangePwdShow.Text; } else { strPwd = TextChangePwdHidden.Password; }
                    policyUpdate.IsUpdatePwd = true;
                }
                else
                {
                    strPwd = S2400EncryptOperation.DecryptWithM002( IPageParent.policyModifying.TypeuEncryptKey);
                }
                if (radEnableDate.IsChecked == true)
                {
                    policy.TypeUStartTime = DateTime.Parse(StartDateTime.Text).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    policyUpdate.EffectTime = policy.TypeUStartTime;
                }
                else if (radEnableImmediately.IsChecked == true)
                {
                    policy.IsImmediately = 1;
                    IPageParent.mainPage.GetAppServerCurrentTime();
                    policyUpdate.EffectTime = S2400App.GolbalCurrentEncryptionDBTime.ToString("yy-MM-dd HH:mm:ss");
                }
            }
            policy.TypeuEncryptKey = S2400EncryptOperation.EncryptWithM002(strPwd);
            return policy;
        }

        /// <summary>
        /// 检查数据完整性
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool CheckData(ref string strError)
        {
            bool boResult = false;
            if (IPageParent.iAddOrModify == (int)OperationType.Add)
            {
                string LStrInputEncryptionKey = string.Empty;
                 if (chkShowPasswordI.IsChecked == true) { LStrInputEncryptionKey = TextPasswordShow.Text; } else { LStrInputEncryptionKey = TextPasswordHidden.Password; }
                 if (LStrInputEncryptionKey.Length < 6 || LStrInputEncryptionKey.Length > 64)
                {
                    boResult = false;
                    strError = "002";
                    return boResult;
                }
                boResult = true;
                return boResult;
            }
            else if (IPageParent.iAddOrModify == (int)OperationType.Modify)
            {
                //在修改自定义输入类型的策略时 需要检查新密钥和开始时间是否合法
                string LStrInputEncryptionKey = string.Empty;
                if (chkChangeKey.IsChecked == true)
                {
                    if (chkShowPasswordC.IsChecked == true) { LStrInputEncryptionKey = TextChangePwdShow.Text; } else { LStrInputEncryptionKey = TextChangePwdHidden.Password; }
                    if (LStrInputEncryptionKey.Length < 6 || LStrInputEncryptionKey.Length > 64)
                    {
                        boResult = false;
                        strError = "012";
                        return boResult;
                    }
                }
                boResult = true;
                return boResult;
            }
            return boResult;
        }
        #endregion
    }
}
