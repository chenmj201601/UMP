using PFShareClassesC;
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
    public partial class UCConnect2Oracle : UserControl
    {
        public UCConnect2Oracle()
        {
            InitializeComponent();
            this.Loaded += UCConnect2Oracle_Loaded;
        }

        private void UCConnect2Oracle_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
            TextBoxServerOName.Focus();
        }

        private void ShowElementLanguage()
        {
            TabItemConnect2Oracle.Header = " " + App.GetDisplayCharater("M02014") + " ";
            LabelServerOName.Content = App.GetDisplayCharater("M02010");
            LabelServerOPort.Content = App.GetDisplayCharater("M02011");
            LabelServiceName.Content = App.GetDisplayCharater("M02015");
            LabelLoginOName.Content = App.GetDisplayCharater("M02012");
            LabelLoginOPassword.Content = App.GetDisplayCharater("M02013");
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrDBServer = string.Empty;
            string LStrDBPort = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPwd = string.Empty;
            string LStrServiceName = string.Empty;
            int LIntDBPort = 0;

            string LStrVerificationCode004 = string.Empty;

            try
            {
                AStrCallReturn = string.Empty;
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrDBServer = TextBoxServerOName.Text.Trim();
                LStrDBPort = TextBoxServerOPort.Text.Trim();
                LStrLoginID = TextBoxLoginOName.Text.Trim();
                LStrLoginPwd = PasswordBoxLoginOPassword.Password;
                LStrServiceName = TextBoxServiceName.Text.Trim();

                if (string.IsNullOrEmpty(LStrDBServer)) { TextBoxServerOName.Focus(); AStrCallReturn = "ER0001"; return LListStrReturn; }

                if (string.IsNullOrEmpty(LStrDBPort)) { TextBoxServerOPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }
                if (!int.TryParse(LStrDBPort, out LIntDBPort)) { TextBoxServerOPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }
                if (LIntDBPort <= 0 || LIntDBPort > 65535) { TextBoxServerOPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }

                if (string.IsNullOrEmpty(LStrLoginID)) { TextBoxLoginOName.Focus(); AStrCallReturn = "ER0003"; return LListStrReturn; }

                if (string.IsNullOrEmpty(LStrServiceName)) { TextBoxServiceName.Focus(); AStrCallReturn = "ER0004"; return LListStrReturn; }

                //LStrDBServer = EncryptionAndDecryption.EncryptDecryptString(LStrDBServer, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //LStrDBPort = EncryptionAndDecryption.EncryptDecryptString(LStrDBPort, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //LStrLoginID = EncryptionAndDecryption.EncryptDecryptString(LStrLoginID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrLoginPwd = EncryptionAndDecryption.EncryptDecryptString(LStrLoginPwd, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //LStrServiceName = EncryptionAndDecryption.EncryptDecryptString(LStrServiceName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LListStrReturn.Add(LStrDBServer); LListStrReturn.Add(LStrDBPort); LListStrReturn.Add(LStrLoginID); LListStrReturn.Add(LStrLoginPwd); LListStrReturn.Add(LStrServiceName);
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }
    }
}
