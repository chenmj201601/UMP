using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
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
using PFShareClassesS;
using UMP.MAMT.BasicModule;
using UMP.MAMT.WCFService00000;
using VoiceCyber.UMP.Communications;

namespace UMP.MAMT.CreateDatabaseObject
{
    public partial class UCBasicRentUserData : UserControl
    {
        public SystemMainWindow IPageParent = null;

        private BackgroundWorker IBackgroundWorkerObtainNeededData = null;

        public DataTable IDataTableMAMTSupportL = null;

        public UCBasicRentUserData()
        {
            InitializeComponent();
            this.Loaded += UCBasicRentUserData_Loaded;
        }

        private void UCBasicRentUserData_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
            TextBoxRentOrgToken.IsReadOnly = true;
            PasswordBoxAdminPassword.Focus();
        }

        private void ShowElementLanguage()
        {
            TabItemBasicRentUserData.Header = " " + App.GetDisplayCharater("M02054") + " ";
            LabelRentOrgName.Content = App.GetDisplayCharater("M02055");
            LabelRentOrgToken.Content = App.GetDisplayCharater("M02056");
            LabelAdminPassword.Content = App.GetDisplayCharater("M02057");
            LabelAdminConfirmPwd.Content = App.GetDisplayCharater("M02058");
            LabelDefaultLanguage.Content = App.GetDisplayCharater("M02059");
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrRentName = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrAdminPassword = string.Empty;
            string LStrConfirmPwd = string.Empty;
            string LStrDefaultLanguage = string.Empty;
            int LIntRentToken = 0;

            try
            {
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrRentName = TextBoxRentOrgName.Text.Trim();
                if (string.IsNullOrEmpty(LStrRentName))
                {
                    AStrCallReturn = "ER0026"; return LListStrReturn;
                }
                LStrRentToken = TextBoxRentOrgToken.Text.Trim();
                if (string.IsNullOrEmpty(LStrRentToken))
                {
                    AStrCallReturn = "ER0027"; return LListStrReturn;
                }
                if (!int.TryParse(LStrRentToken, out LIntRentToken))
                {
                    AStrCallReturn = "ER0027"; return LListStrReturn;
                }
                if (LIntRentToken < 0 || LIntRentToken > 99999)
                {
                    AStrCallReturn = "ER0027"; return LListStrReturn;
                }
                LStrRentToken = LIntRentToken.ToString("00000");

                LStrAdminPassword = PasswordBoxAdminPassword.Password;
                if (string.IsNullOrEmpty(LStrAdminPassword))
                {
                    AStrCallReturn = "ER0028"; return LListStrReturn;
                }
                LStrConfirmPwd = PasswordBoxAdminConfirmPwd.Password;

                if (LStrAdminPassword != LStrConfirmPwd)
                {
                    AStrCallReturn = "ER0025"; return LListStrReturn;
                }

                LStrAdminPassword = EncryptionAndDecryption.EncryptDecryptString(LStrAdminPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                ComboBoxItem LComboBoxItemLanguage = ComboBoxDefaultLanguage.SelectedItem as ComboBoxItem;
                if (LComboBoxItemLanguage == null)
                {
                    AStrCallReturn = "ER0029"; return LListStrReturn;
                }
                LStrDefaultLanguage = LComboBoxItemLanguage.DataContext.ToString();

                //0-租户名称
                LListStrReturn.Add(LStrRentName);
                //1-租户Token
                LListStrReturn.Add(LStrRentToken);
                //2-超级系统管理员密码
                LListStrReturn.Add(LStrAdminPassword);
                //3-系统默认界面显示语言
                LListStrReturn.Add(LStrDefaultLanguage);
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        public void ObtainNeededData()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M02086"));
                IBackgroundWorkerObtainNeededData = new BackgroundWorker();
                IBackgroundWorkerObtainNeededData.RunWorkerCompleted += IBackgroundWorkerObtainNeededData_RunWorkerCompleted;
                IBackgroundWorkerObtainNeededData.DoWork += IBackgroundWorkerObtainNeededData_DoWork;
                IBackgroundWorkerObtainNeededData.RunWorkerAsync();
            }
            catch
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M02086"));
                if (IBackgroundWorkerObtainNeededData != null)
                {
                    IBackgroundWorkerObtainNeededData.Dispose();
                    IBackgroundWorkerObtainNeededData = null;
                }
            }
            
        }

        private void IBackgroundWorkerObtainNeededData_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrWcfArgs = new List<string>();

            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            IDataTableMAMTSupportL = new DataTable();
            LListStrWcfArgs.Add(App.GClassSessionInfo.LangTypeInfo.LangID.ToString());
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
            OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
            LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
            LWCFOperationReturn = LService00000Client.OperationMethodA(31, LListStrWcfArgs);
            if (LWCFOperationReturn.BoolReturn)
            {
                IDataTableMAMTSupportL = LWCFOperationReturn.DataSetReturn.Tables[0];
            }
            else { IDataTableMAMTSupportL = null; }
            LService00000Client.Close();
            LService00000Client = null;
        }

        private void IBackgroundWorkerObtainNeededData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrLangID = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);

                if (e.Error != null)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02060") + "\n\n" + e.Error.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (IDataTableMAMTSupportL == null)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02060"),"", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ComboBoxDefaultLanguage.Items.Clear();
                ComboBoxItem LComboBoxItemLanguageSelected = null;
                foreach (DataRow LDataRowSingleLanguage in IDataTableMAMTSupportL.Rows)
                {
                    ComboBoxItem LComboBoxItemSingleLanguage = new ComboBoxItem();
                    LComboBoxItemSingleLanguage.Style = (Style)App.Current.Resources["ComboBoxItemFontStyle"];
                    LStrLangID = LDataRowSingleLanguage["C004"].ToString();
                    LComboBoxItemSingleLanguage.Content = "(" + LStrLangID + ")" + LDataRowSingleLanguage["C002"].ToString();
                    LComboBoxItemSingleLanguage.DataContext = LStrLangID;
                    ComboBoxDefaultLanguage.Items.Add(LComboBoxItemSingleLanguage);
                    if (LStrLangID == App.GStrLoginUserCurrentLanguageID) { LComboBoxItemLanguageSelected = LComboBoxItemSingleLanguage;}
                }
                if (LComboBoxItemLanguageSelected != null) { ComboBoxDefaultLanguage.SelectedItem = LComboBoxItemLanguageSelected; }
            }
            catch { }
            finally
            {
                IBackgroundWorkerObtainNeededData.Dispose();
                IBackgroundWorkerObtainNeededData = null;
            }
        }
    }
}
