using Common2400;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
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
using UMPS2400.Entries;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// UC_EncryptionPolicyTypeC.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptionPolicyTypeC
    {
        #region 变量定义
        public UC_AddPolicy IPageParent = null;
        #endregion

        public UC_EncryptionPolicyTypeC()
        {
            InitializeComponent();
            this.Loaded += UC_EncryptionPolicyTypeC_Loaded;
            ComboOccurs.SelectionChanged += ComboOccurs_SelectionChanged;
            chkUpdateKeyImmediately.Checked += chkUpdateKeyImmediately_Checked;
            chkUpdateKeyImmediately.Unchecked += chkUpdateKeyImmediately_Unchecked;
            TextExecutionInterval.TextChanged += TextExecutionInterval_TextChanged;
        }

        void TextExecutionInterval_TextChanged(object sender, TextChangedEventArgs e)
        {
            string LStrExcInterval= TextExecutionInterval.Text.Trim();
            int LIntNumbersDigital = 0;
            if (!int.TryParse(LStrExcInterval, out LIntNumbersDigital)) { LIntNumbersDigital = 1; }
            if (LIntNumbersDigital <= 1) { LIntNumbersDigital = 1; TextExecutionInterval.Text = "1"; }
        }

        void UC_EncryptionPolicyTypeC_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            InitControl();
            
        }

        #region Overried
        public override void ChangeLanguage()
        {
            InitLanguage();
        }
        #endregion

        #region Init
        public void InitLanguage()
        {
            LabelOccurs.Content = CurrentApp.GetLanguageInfo("2402015", "Execution cycle");
            LabelOccursDailyMonthlyTip.Content = string.Empty;
            LabelFirstDay.Content = CurrentApp.GetLanguageInfo("2402L017", "The first day");
            cmbSunday.Content = CurrentApp.GetLanguageInfo("2402012", "Sunday");
            cmbMonday.Content = CurrentApp.GetLanguageInfo("2402013", "Monday");
            LabelFirstDayOfWeek.Content = CurrentApp.GetLanguageInfo("2402L018", "The first day of the week");
            cmbOccursD.Content = CurrentApp.GetLanguageInfo("2402ComboTagD", "Day");
            cmbOccursM.Content = CurrentApp.GetLanguageInfo("2402ComboTagM", "Month");
            cmbOccursW.Content = CurrentApp.GetLanguageInfo("2402ComboTagW", "Week");
            cmbOccursU.Content = CurrentApp.GetLanguageInfo("2402ComboTagU", "Custom");
            chkUpdateKeyImmediately.Content = CurrentApp.GetLanguageInfo("2402014", "Immediate update key");
            chkUpdateCycle.Content = CurrentApp.GetLanguageInfo("2402015", "Update Period");
        }

        public void InitControl()
        {
            if (IPageParent.iAddOrModify == (int)OperationType.Add)
            {
                ComboOccurs.SelectedIndex = 2;
            }
            else
            {
                chkUpdateKeyImmediately.IsEnabled = true;
                //执行周期
                switch (IPageParent.policyModifying.PolicyOccursFrequency)
                {
                    case "D":
                        ComboOccurs.SelectedIndex = 0;
                        chkUpdateCycle.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "W":
                        ComboOccurs.SelectedIndex = 1;
                        if (IPageParent.policyModifying.BeginDayofCycle == "0")
                        {
                            ComboFirstDayOfWeek.SelectedIndex = 0;
                        }
                        else
                        {
                            ComboFirstDayOfWeek.SelectedIndex = 1;
                        }
                        ComboFirstDayOfWeek.IsEnabled = false;
                        chkUpdateCycle.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "M":
                        ComboOccurs.SelectedIndex = 2;
                        chkUpdateCycle.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "U":
                        ComboOccurs.SelectedIndex = 3;
                        TextExecutionInterval.Text = IPageParent.policyModifying.BeginDayofCycle;
                       // chkUpdateKeyImmediately.IsEnabled = true;
                        chkUpdateCycle.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
                ComboOccurs.IsEnabled = false;
                if (IPageParent.mainPage.HasKeyGenServer)
                {
                    chkUpdateKeyImmediately.Visibility = Visibility.Visible;
                }
                else
                {
                    chkUpdateKeyImmediately.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// "执行周期"的选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboOccurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strTag = (ComboOccurs.SelectedItem as ComboBoxItem).Tag.ToString();
            if (strTag == "D" || strTag == "M")
            {
                OccursDailyMonthlyPanel.Visibility = Visibility.Visible;
                OccursWeeklyPanel.Visibility = Visibility.Collapsed;
                OccursCustomPanel.Visibility = Visibility.Collapsed;
                string strOccurs = CurrentApp.GetLanguageInfo("2402ComboTag" + strTag, string.Empty);
                LabelOccursDailyMonthlyTip.Content = string.Format(CurrentApp.GetLanguageInfo("2402L016", "Every {0} generate a new key"), strOccurs);
            }
            else if (strTag == "W")
            {
                OccursDailyMonthlyPanel.Visibility = Visibility.Collapsed;
                OccursWeeklyPanel.Visibility = Visibility.Visible;
                OccursCustomPanel.Visibility = Visibility.Collapsed;
            }
            else if (strTag == "U")
            {
                OccursDailyMonthlyPanel.Visibility = Visibility.Collapsed;
                OccursWeeklyPanel.Visibility = Visibility.Collapsed;
                OccursCustomPanel.Visibility = Visibility.Visible;
                chkUpdateCycle.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// "立即更新密钥"的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkUpdateKeyImmediately_Checked(object sender, RoutedEventArgs e)
        {
            if (ComboOccurs.SelectedIndex == 3) { chkUpdateCycle.IsEnabled = true; }
        }

        /// <summary>
        /// "立即更新密钥"的取消选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkUpdateKeyImmediately_Unchecked(object sender, RoutedEventArgs e)
        {
            chkUpdateCycle.IsEnabled = false; chkUpdateCycle.IsChecked = false;
        }
        #endregion

        #region 父窗口调用的函数
        public bool CheckData(ref string strError)
        {
            bool boResult = false;
            //if (IPageParent.iAddOrModify == (int)OperationType.Add)
            //{
                ComboBoxItem item = ComboOccurs.SelectedItem as ComboBoxItem;
                if (item.Tag.ToString() == "U")
                {
                    if (string.IsNullOrEmpty(TextExecutionInterval.Text) || TextExecutionInterval.Text.Trim() == "0")
                    {
                        strError = "003";
                        return boResult;
                    }
                }
                return true;
            //}
            //else if (IPageParent.iAddOrModify == (int)OperationType.Modify)
            //{

            //}
        }

        /// <summary>
        /// 将界面上的值写入EncryptionPolicy对象
        /// </summary>
        /// <returns></returns>
        public UMPEncryptionPolicy GetPolicyInTypeC(UMPEncryptionPolicy policy,PolicyUpdateEntry policyUpdate)
        {
            ComboBoxItem item = ComboOccurs.SelectedItem as ComboBoxItem;
            policy.PolicyOccursFrequency = item.Tag.ToString();
            if (item.Tag.ToString() == "U")
            {
                //如果是用户自定义天数
                policy.BeginDayofCycle = TextExecutionInterval.Text;
            }
            else if (item.Tag.ToString() == "W")
            {
                //如果是周
                ComboBoxItem firstDayItem = ComboFirstDayOfWeek.SelectedItem as ComboBoxItem;
                policy.BeginDayofCycle = firstDayItem.Tag.ToString();
            }

            if (IPageParent.iAddOrModify == (int)OperationType.Modify)
            {
                if (chkUpdateKeyImmediately.IsChecked == true)
                {
                    IPageParent.mainPage.GetAppServerCurrentTime();
                    policyUpdate.EffectTime = S2400App.GolbalCurrentEncryptionDBTime.ToString("yyyy-MM-dd HH:mm:ss");
                    policyUpdate.IsUpdatePwd = true;
                    policy.IsImmediately = 1;
                    if (chkUpdateCycle.IsChecked == true)
                    {
                        policy.ResetCycle = "1";
                        policyUpdate.IsResetCycle = true;
                    }
                }
            }
            return policy;
        }
        #endregion

    }
}
