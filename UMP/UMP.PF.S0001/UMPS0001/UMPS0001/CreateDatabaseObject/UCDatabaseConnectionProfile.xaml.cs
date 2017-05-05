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
    public partial class UCDatabaseConnectionProfile : UserControl
    {
        public Page000001A IPageParent = null;

        public UCDatabaseConnectionProfile()
        {
            InitializeComponent();
            this.Loaded += UCDatabaseConnectionProfile_Loaded;
        }

        private void UCDatabaseConnectionProfile_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemDatabaseConnectionProfile.Header = " " + App.GetDisplayCharater("M02080") + " ";
            LabelPDatabaseType.Content = App.GetDisplayCharater("M02073");
            LabelPDBServerName.Content = App.GetDisplayCharater("M02074");
            LabelPServerPort.Content = App.GetDisplayCharater("M02075");
            if (IPageParent.IStrDatabaseType == "3")
            {
                LabelPDBOrServiceName.Content = App.GetDisplayCharater("M02077");
            }
            else
            {
                LabelPDBOrServiceName.Content = App.GetDisplayCharater("M02076");
            }
            LabelPLoginName.Content = App.GetDisplayCharater("M02078");
            LabelPLoginPassword.Content = App.GetDisplayCharater("M02079");
        }

        public void InitCreatedDatabaseInformation(List<string> AListStrDatabaseProfile)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrLoginPassword = string.Empty;
            int LIntPasswordLength = 0;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                if (IPageParent.IStrDatabaseType == "2")
                {
                    TextBoxPDatabaseType.Text = App.GetDisplayCharater("M02007");
                }
                if (IPageParent.IStrDatabaseType == "3")
                {
                    TextBoxPDatabaseType.Text = App.GetDisplayCharater("M02008");
                }

                TextBoxPServerName.Text = AListStrDatabaseProfile[0];
                TextBoxPServerPort.Text = AListStrDatabaseProfile[1];
                TextBoxPDBOrServiceName.Text = AListStrDatabaseProfile[4];
                TextBoxPLoginName.Text = AListStrDatabaseProfile[2];
                LStrLoginPassword = EncryptionAndDecryption.EncryptDecryptString(AListStrDatabaseProfile[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                TextBoxPLoginPassword.DataContext = LStrLoginPassword;
                LIntPasswordLength = LStrLoginPassword.Length;
                LStrLoginPassword = string.Empty;
                for (int LIntLoopPwdLength = 0; LIntLoopPwdLength < LIntPasswordLength; LIntLoopPwdLength++) { LStrLoginPassword += "*"; }
                TextBoxPLoginPassword.Text = LStrLoginPassword;
            }
            catch { }
        }
    }
}
