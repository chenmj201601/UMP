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

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCCreateLoginAccount : UserControl
    {
        public UCCreateLoginAccount()
        {
            InitializeComponent();
            this.Loaded += UCCreateLoginAccount_Loaded;
        }

        private void UCCreateLoginAccount_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemCreateLogin.Header = " " + App.GetDisplayCharater("M02036") + " ";
            LabelLoginID.Content = App.GetDisplayCharater("M02037");
            LabelAccountPassword.Content = App.GetDisplayCharater("M02038");
            LabelLoginConfirmPassword.Content = App.GetDisplayCharater("M02039");
            CheckBoxCheckPolicyOn.Content = App.GetDisplayCharater("M02040");
            CheckBoxSysAdmin.Content = App.GetDisplayCharater("M02041");
            CheckBoxServerAdmin.Content = App.GetDisplayCharater("M02042");
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPassword = string.Empty;
            string LStrLoginConfirmPassword = string.Empty;

            try
            {
                LStrLoginID = TextBoxLoginID.Text.Trim();
                if (string.IsNullOrEmpty(LStrLoginID)) { TextBoxLoginID.Focus(); AStrCallReturn = "ER0024"; LListStrReturn.Clear(); return LListStrReturn; }
                //0-登录名
                LListStrReturn.Add(LStrLoginID);
                LStrLoginPassword = PasswordBoxAccountPassword.Password;
                LStrLoginConfirmPassword = PasswordBoxLoginConfirmPassword.Password;
                if (LStrLoginPassword != LStrLoginConfirmPassword)
                {
                    PasswordBoxAccountPassword.Focus(); AStrCallReturn = "ER0025"; LListStrReturn.Clear(); return LListStrReturn;
                }
                //1-登录密码
                LListStrReturn.Add(LStrLoginPassword);
                //2-强制实施密码策略
                if (CheckBoxCheckPolicyOn.IsChecked == true) { LListStrReturn.Add("1"); } else { LListStrReturn.Add("0"); }
                //3-DefaultDatabase
                LListStrReturn.Add("");
                //4-SysAdmin
                if (CheckBoxSysAdmin.IsChecked == true) { LListStrReturn.Add("1"); } else { LListStrReturn.Add("0"); }
                //5-ServerAdmin
                if (CheckBoxServerAdmin.IsChecked == true) { LListStrReturn.Add("1"); } else { LListStrReturn.Add("0"); }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

    }
}
