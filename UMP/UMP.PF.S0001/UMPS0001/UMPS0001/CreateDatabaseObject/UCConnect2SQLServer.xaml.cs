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
    public partial class UCConnect2SQLServer : UserControl
    {
        public UCConnect2SQLServer()
        {
            InitializeComponent();
            this.Loaded += UCConnect2SQLServer_Loaded;
        }

        private void UCConnect2SQLServer_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
            TextBoxServerName.Focus();
        }

        private void ShowElementLanguage()
        {
            TabItemConnect2SQLServer.Header = " " + App.GetDisplayCharater("M02009") + " ";
            LabelDBServerName.Content = App.GetDisplayCharater("M02010");
            LabelServerPort.Content = App.GetDisplayCharater("M02011");
            LabelLoginName.Content = App.GetDisplayCharater("M02012");
            LabelLoginPassword.Content = App.GetDisplayCharater("M02013");
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrDBServer = string.Empty;
            string LStrDBPort = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPwd = string.Empty;
            int LIntDBPort = 0;

            string LStrVerificationCode004 = string.Empty;

            try
            {
                AStrCallReturn = string.Empty;
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrDBServer = TextBoxServerName.Text.Trim();
                LStrDBPort = TextBoxServerPort.Text.Trim();
                LStrLoginID = TextBoxLoginName.Text.Trim();
                LStrLoginPwd = PasswordBoxLoginPassword.Password;

                if (string.IsNullOrEmpty(LStrDBServer)) { TextBoxServerName.Focus(); AStrCallReturn = "ER0001"; return LListStrReturn; }

                if (string.IsNullOrEmpty(LStrDBPort)) { TextBoxServerPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }
                if (!int.TryParse(LStrDBPort, out LIntDBPort)) { TextBoxServerPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }
                if (LIntDBPort <= 0 || LIntDBPort > 65535) { TextBoxServerPort.Focus(); AStrCallReturn = "ER0002"; return LListStrReturn; }

                if (string.IsNullOrEmpty(LStrLoginID)) { TextBoxLoginName.Focus(); AStrCallReturn = "ER0003"; return LListStrReturn; }

                LStrDBServer = EncryptionAndDecryption.EncryptDecryptString(LStrDBServer, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrDBPort = EncryptionAndDecryption.EncryptDecryptString(LStrDBPort, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrLoginID = EncryptionAndDecryption.EncryptDecryptString(LStrLoginID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrLoginPwd = EncryptionAndDecryption.EncryptDecryptString(LStrLoginPwd, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LListStrReturn.Add(LStrDBServer); LListStrReturn.Add(LStrDBPort); LListStrReturn.Add(LStrLoginID); LListStrReturn.Add(LStrLoginPwd);
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }
    }
}
