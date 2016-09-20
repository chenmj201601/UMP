using System;
using System.Collections.Generic;
using System.Data;
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

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCDatabaseBasicInformation : UserControl,S0001Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private List<string> IListStrServerInformation = new List<string>();

        public UCDatabaseBasicInformation()
        {
            InitializeComponent();
            this.Loaded += UCDatabaseBasicInformation_Loaded;
        }

        private void UCDatabaseBasicInformation_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();

            ButtonDataPath.Click += ButtonDataLogPathClicked;
            ButtonLogPath.Click += ButtonDataLogPathClicked;
        }

        private void ButtonDataLogPathClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;

                if (IOperationEvent == null) { return; }
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "C001";
                LEventArgs.ObjectSource0 = LButtonClicked.Tag.ToString();
                LEventArgs.ObjectSource1 = string.Empty;
                IOperationEvent(this, LEventArgs);
            }
            catch { }
        }

        private void ShowElementLanguage()
        {
            TabItemDatabaseGenaral.Header = " " + App.GetDisplayCharater("M02017") + " ";
            LabelDatabaseName.Content = App.GetDisplayCharater("M02018");
            CheckBoxWithReplace.Content = App.GetDisplayCharater("M02034");
            LabelCollations.Content = App.GetDisplayCharater("M02019");
            LabelRecoveryMode.Content = App.GetDisplayCharater("M02020");
            ComboBoxItemRecoveryMode1.Content = App.GetDisplayCharater("M02021");
            ComboBoxItemRecoveryMode2.Content = App.GetDisplayCharater("M02022");
            ComboBoxItemRecoveryMode3.Content = App.GetDisplayCharater("M02023");

            TabItemDataFileOptions.Header = " " + App.GetDisplayCharater("M02024") + " ";
            LabelDataPath.Content = App.GetDisplayCharater("M02025");
            GroupBoxDataSize.Header = " " + App.GetDisplayCharater("M02026") + " ";
            LabelDataInitSize.Content = App.GetDisplayCharater("M02027");
            LabelDataMaximumSize.Content = App.GetDisplayCharater("M02028");
            GroupBoxDataGrowth.Header = " " + App.GetDisplayCharater("M02029") + " ";
            RadioButtonDataPrecent.Content = App.GetDisplayCharater("M02030");
            RadioButtonDataMegabytes.Content = App.GetDisplayCharater("M02031");

            TabItemLogFileOptions.Header = " " + App.GetDisplayCharater("M02032") + " ";
            LabelLogPath.Content = App.GetDisplayCharater("M02033");
            GroupBoxLogSize.Header = " " + App.GetDisplayCharater("M02026") + " ";
            LabelLogInitSize.Content = App.GetDisplayCharater("M02027");
            LabelLogMaximumSize.Content = App.GetDisplayCharater("M02028");
            GroupBoxLogGrowth.Header = " " + App.GetDisplayCharater("M02029") + " ";
            RadioButtonLogPrecent.Content = App.GetDisplayCharater("M02030");
            RadioButtonLogMegabytes.Content = App.GetDisplayCharater("M02031");
        }

        public void ShowDatabaseInformation(OperationParameters AOperationParameters)
        {
            string LStrCollationName = string.Empty;
            string LStrCollationDescription = string.Empty;

            try
            {
                DataTable LDataTableCollations = ((DataSet)AOperationParameters.ObjectSource2).Tables[0];
                IListStrServerInformation = AOperationParameters.ObjectSource1 as List<string>;

                ComboBoxItem LComboBoxItemDefaultCollation = new ComboBoxItem();
                ComboBoxCollations.Items.Clear();
                foreach (DataRow LDataRowSingleCollations in LDataTableCollations.Rows)
                {
                    ComboBoxItem LComboBoxItemSingleCollationItem = new ComboBoxItem();
                    LComboBoxItemSingleCollationItem.Style = (Style)App.Current.Resources["ComboBoxItemFontStyle"];
                    LStrCollationName = LDataRowSingleCollations[1].ToString();
                    LStrCollationDescription = LDataRowSingleCollations[5].ToString();
                    LComboBoxItemSingleCollationItem.Content = LStrCollationName;
                    LComboBoxItemSingleCollationItem.DataContext = LStrCollationName;
                    LComboBoxItemSingleCollationItem.ToolTip = LStrCollationDescription;
                    ComboBoxCollations.Items.Add(LComboBoxItemSingleCollationItem);
                    if (LStrCollationName == IListStrServerInformation[1]) { LComboBoxItemDefaultCollation = LComboBoxItemSingleCollationItem; }
                }
                ComboBoxCollations.SelectedItem = LComboBoxItemDefaultCollation;

                TextBoxDataPath.Text = IListStrServerInformation[2];
                TextBoxLogPath.Text = IListStrServerInformation[3];
            }
            catch { }
        }

        public void ShowSelectedLocateFolder(string AStrServerFolder, string AStrFolderType)
        {
            if (AStrFolderType == "D")
            {
                TextBoxDataPath.Text = AStrServerFolder;
            }
            else
            {
                TextBoxLogPath.Text = AStrServerFolder;
            }
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();

            try
            {
                //0：数据库名称
                LListStrReturn.Add(TextBoxDatabaseName.Text.Trim());

                //1：覆盖数据库
                if (CheckBoxWithReplace.IsChecked == true) { LListStrReturn.Add("1"); } else { LListStrReturn.Add("0"); }

                //2：排序规则
                ComboBoxItem LComboBoxItemCollation = ComboBoxCollations.SelectedItem as ComboBoxItem;
                if (LComboBoxItemCollation != null) { LListStrReturn.Add(LComboBoxItemCollation.DataContext.ToString()); } else { LListStrReturn.Add(""); }

                //3：恢复模式
                ComboBoxItem LComboBoxItemRecoveryMode = ComboBoxRecoveryMode.SelectedItem as ComboBoxItem;
                if (LComboBoxItemCollation != null) { LListStrReturn.Add(LComboBoxItemRecoveryMode.DataContext.ToString()); } else { LListStrReturn.Add(""); }

                //4：数据文件保存路径
                LListStrReturn.Add(TextBoxDataPath.Text.Trim());

                //5：数据文件初始大小
                LListStrReturn.Add(TextBoxDataInitSize.Text.Trim());

                //6：数据文件最大大小
                LListStrReturn.Add(TextBoxDataMaximumSize.Text.Trim());

                //7：数据文件增长方式
                //8：数据文件增长大小
                if (RadioButtonDataPrecent.IsChecked == true)
                {
                    LListStrReturn.Add("P");
                    LListStrReturn.Add(TextBoxDataPrecent.Text.Trim());
                }
                else
                {
                    LListStrReturn.Add("M");
                    LListStrReturn.Add(TextBoxDataMegabytes.Text.Trim());
                }

                //9：日志文件保存路径
                LListStrReturn.Add(TextBoxLogPath.Text.Trim());

                //10：日志文件初始大小
                LListStrReturn.Add(TextBoxLogInitSize.Text.Trim());

                //11：日志文件最大大小
                LListStrReturn.Add(TextBoxLogMaximumSize.Text.Trim());

                //12：日志文件增长方式
                //13：日志文件增长大小
                if (RadioButtonLogPrecent.IsChecked == true)
                {
                    LListStrReturn.Add("P");
                    LListStrReturn.Add(TextBoxLogPrecent.Text.Trim());
                }
                else
                {
                    LListStrReturn.Add("M");
                    LListStrReturn.Add(TextBoxLogMegabytes.Text.Trim());
                }

                //数据库名称不允许为空
                if (string.IsNullOrEmpty(LListStrReturn[0])) { AStrCallReturn = "ER0011"; LListStrReturn.Clear(); return LListStrReturn; }
                //数据文件的路径不允许为空
                if (string.IsNullOrEmpty(LListStrReturn[4])) { AStrCallReturn = "ER0012"; LListStrReturn.Clear(); return LListStrReturn; }


                int LIntInitSize = 0, LIntMaxSize = 0, LIntPrecent = 0, LIntMegabytes = 0;

                //数据文件初始大小错误， 允许的范围为 5 ～ 2048
                if (!int.TryParse(LListStrReturn[5], out LIntInitSize)) { AStrCallReturn = "ER0013"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntInitSize < 5 || LIntInitSize > 2048) { AStrCallReturn = "ER0013"; LListStrReturn.Clear(); return LListStrReturn; }
                //数据文件最大值错误， 允许的范围为 5 ～ 102400， 如果为 0 则不限制最大值
                if (!int.TryParse(LListStrReturn[6], out LIntMaxSize)) { AStrCallReturn = "ER0014"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntMaxSize < 0 || LIntMaxSize > 102400) { AStrCallReturn = "ER0014"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntMaxSize != 0 && LIntMaxSize < 5) { AStrCallReturn = "ER0014"; LListStrReturn.Clear(); return LListStrReturn; }
                //数据文件“最大值”不允许小于“初始大小”
                if (LIntMaxSize != 0 && LIntMaxSize < LIntInitSize) { AStrCallReturn = "ER0015"; LListStrReturn.Clear(); return LListStrReturn; }

                if (LListStrReturn[7] == "P")
                {
                    //按百分比增长错误， 允许的范围为 1 ～ 100
                    if (!int.TryParse(LListStrReturn[8], out LIntPrecent)) { AStrCallReturn = "ER0016"; LListStrReturn.Clear(); return LListStrReturn; }
                    if (LIntPrecent <= 0 || LIntPrecent > 100) { AStrCallReturn = "ER0016"; LListStrReturn.Clear(); return LListStrReturn; }
                }
                else
                {
                    //按 MB 增长错误， 允许的范围为 32 ～ 1024
                    if (!int.TryParse(LListStrReturn[8], out LIntMegabytes)) { AStrCallReturn = "ER0017"; LListStrReturn.Clear(); return LListStrReturn; }
                    if (LIntMegabytes < 32 || LIntMegabytes > 1024) { AStrCallReturn = "ER0017"; LListStrReturn.Clear(); return LListStrReturn; }
                }

                //日志文件的路径不允许为空
                if (string.IsNullOrEmpty(LListStrReturn[9])) { AStrCallReturn = "ER0018"; LListStrReturn.Clear(); return LListStrReturn; }

                //日志文件初始大小错误， 允许的范围为 2 ～ 1024
                if (!int.TryParse(LListStrReturn[10], out LIntInitSize)) { AStrCallReturn = "ER0019"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntInitSize < 2 || LIntInitSize > 1024) { AStrCallReturn = "ER0019"; LListStrReturn.Clear(); return LListStrReturn; }
                //日志文件最大大小错误， 允许的范围为 2 ～ 10240， 如果为 0 则不限制最大值
                if (!int.TryParse(LListStrReturn[11], out LIntMaxSize)) { AStrCallReturn = "ER0020"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntMaxSize < 0 || LIntMaxSize > 10240) { AStrCallReturn = "ER0020"; LListStrReturn.Clear(); return LListStrReturn; }
                if (LIntMaxSize != 0 && LIntMaxSize < 2) { AStrCallReturn = "ER0020"; LListStrReturn.Clear(); return LListStrReturn; }
                //日志文件“最大值”不允许小于“初始大小”
                if (LIntMaxSize != 0 && LIntMaxSize < LIntInitSize) { AStrCallReturn = "ER0021"; LListStrReturn.Clear(); return LListStrReturn; }

                if (LListStrReturn[12] == "P")
                {
                    //日志文件按百分比增长错误， 允许的范围为 1 ～ 100
                    if (!int.TryParse(LListStrReturn[13], out LIntPrecent)) { AStrCallReturn = "ER0022"; LListStrReturn.Clear(); return LListStrReturn; }
                    if (LIntPrecent <= 0 || LIntPrecent > 100) { AStrCallReturn = "ER0022"; LListStrReturn.Clear(); return LListStrReturn; }
                }
                else
                {
                    //日志文件按 MB 增长错误， 允许的范围为 16 ～ 512
                    if (!int.TryParse(LListStrReturn[13], out LIntMegabytes)) { AStrCallReturn = "ER0023"; LListStrReturn.Clear(); return LListStrReturn; }
                    if (LIntMegabytes < 16 || LIntMegabytes > 512) { AStrCallReturn = "ER0023"; LListStrReturn.Clear(); return LListStrReturn; }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GetSettedData()");
                LListStrReturn.Clear(); }

            return LListStrReturn;
        }
        
    }
}
