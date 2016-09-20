using PFShareClassesC;
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
using UMPS0001.WCFService00001;
using VoiceCyber.UMP.Communications;

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCConfirmConnectionProfile : UserControl
    {
        public Page000001A IPageParent = null;

        private bool IBoolHaveError = false;
        private BackgroundWorker IBackgroundWorkerObtainDatabasesLogins = null;
        private OperationDataArgs IWCFOperationDataArgs = null;
        private OperationParameters IParameterStep22 = null;
        private OperationParameters IParameterStep23 = null;

        public UCConfirmConnectionProfile()
        {
            InitializeComponent();
            this.Loaded += UCConfirmConnectionProfile_Loaded;
        }

        private void UCConfirmConnectionProfile_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemConnectionProfile.Header = " " + App.GetDisplayCharater("M02044") + " ";
            LabelDBServerName.Content = App.GetDisplayCharater("M02045");
            LabelServerPort.Content = App.GetDisplayCharater("M02046");
            LabelDatabases.Content = App.GetDisplayCharater("M02047");
            LabelLoginName.Content = App.GetDisplayCharater("M02048");
            LabelLoginPassword.Content = App.GetDisplayCharater("M02049");
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrVerificationCode004 = string.Empty;

            string LStrServerName = string.Empty;
            string LStrServerPort = string.Empty;
            string LStrDatabase = string.Empty;
            string LStrLoginAccount = string.Empty;
            string LStrLongPwd = string.Empty;

            try
            {
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrServerName = TextBoxServerName.Text;
                LStrServerPort = TextBoxServerPort.Text;

                ComboBoxItem LComboBoxItemDatabase = ComboBoxDatabases.SelectedItem as ComboBoxItem;
                if (LComboBoxItemDatabase == null)
                {
                    AStrCallReturn = "M02050";
                    return LListStrReturn;
                }
                LStrDatabase = LComboBoxItemDatabase.DataContext.ToString();

                LStrLoginAccount = ComboBoxLoginName.Text;
                if (string.IsNullOrEmpty(LStrLoginAccount))
                {
                    AStrCallReturn = "M02051";
                    return LListStrReturn;
                }

                LStrLongPwd = PasswordBoxLoginPassword.Password;
                LStrLongPwd = EncryptionAndDecryption.EncryptDecryptString(LStrLongPwd, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //0-数据库服务器
                LListStrReturn.Add(LStrServerName);
                //1-端口
                LListStrReturn.Add(LStrServerPort);
                //2-LogingID
                LListStrReturn.Add(LStrLoginAccount);
                //3-Login Password
                LListStrReturn.Add(LStrLongPwd);
                //4-数据库名
                LListStrReturn.Add(LStrDatabase);
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        public void ShowConfirmInformation(List<string> AListStrDatabaseServerProfile, OperationParameters AOperationParameters22, OperationParameters AOperationParameters23)
        {
            string LStrVerificationCode104 = string.Empty;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                GridConfirmInformation.IsEnabled = false;
                IBoolHaveError = false;
                IParameterStep22 = AOperationParameters22;
                IParameterStep23 = AOperationParameters23;

                TextBoxServerName.Text = EncryptionAndDecryption.EncryptDecryptString(AListStrDatabaseServerProfile[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                TextBoxServerPort.Text = EncryptionAndDecryption.EncryptDecryptString(AListStrDatabaseServerProfile[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                IPageParent.ShowWaitProgressBar(true);
                IBackgroundWorkerObtainDatabasesLogins = new BackgroundWorker();
                IBackgroundWorkerObtainDatabasesLogins.RunWorkerCompleted += IBackgroundWorkerObtainDatabasesLogins_RunWorkerCompleted;
                IBackgroundWorkerObtainDatabasesLogins.DoWork += IBackgroundWorkerObtainDatabasesLogins_DoWork;
                IBackgroundWorkerObtainDatabasesLogins.RunWorkerAsync(AListStrDatabaseServerProfile);
            }
            catch
            {
                IPageParent.ShowWaitProgressBar(false);
                GridConfirmInformation.IsEnabled = true;
                IBoolHaveError = true;
                if (IBackgroundWorkerObtainDatabasesLogins != null)
                {
                    IBackgroundWorkerObtainDatabasesLogins.Dispose();
                    IBackgroundWorkerObtainDatabasesLogins = null;
                }
            }
        }

        private void IBackgroundWorkerObtainDatabasesLogins_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrDatabaseServerProfile = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(204, LListStrDatabaseServerProfile);
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerObtainDatabasesLogins_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IPageParent.ShowWaitProgressBar(false);
                GridConfirmInformation.IsEnabled = true;

                if (e.Error != null)
                {
                    IBoolHaveError = true;
                    MessageBox.Show(App.GetDisplayCharater("M02052") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(IWCFOperationDataArgs.StringReturn))
                {
                    IBoolHaveError = true;
                    MessageBox.Show(App.GetDisplayCharater("M02052") + "\n\n" + IWCFOperationDataArgs.StringReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ShowDatabasesInComboBox();
            }
            catch { }
            finally
            {
                IBackgroundWorkerObtainDatabasesLogins.Dispose();
                IBackgroundWorkerObtainDatabasesLogins = null;
            }
        }

        private void ShowDatabasesInComboBox()
        {
            string LStrCreatedDatabaseName = string.Empty;
            

            try
            {
                ComboBoxDatabases.Items.Clear();
                ComboBoxItem LComboBoxItemDatabaseSelected = null;

                if (IParameterStep22.ObjectSource0 != null)
                {
                    List<string> LListStrCreateDBOptions = IParameterStep22.ObjectSource0 as List<string>;
                    LStrCreatedDatabaseName = LListStrCreateDBOptions[0];
                }

                foreach (string LStrSingleDatabase in IWCFOperationDataArgs.ListStringReturn)
                {
                    ComboBoxItem LComboBoxItemSingleDatabase = new ComboBoxItem();
                    LComboBoxItemSingleDatabase.Style = (Style)App.Current.Resources["ComboBoxItemFontStyle"];
                    LComboBoxItemSingleDatabase.Content = LStrSingleDatabase;
                    LComboBoxItemSingleDatabase.DataContext = LStrSingleDatabase;
                    ComboBoxDatabases.Items.Add(LComboBoxItemSingleDatabase);
                    if (LStrSingleDatabase == LStrCreatedDatabaseName) { LComboBoxItemDatabaseSelected = LComboBoxItemSingleDatabase; }
                }
                ComboBoxDatabases.SelectionChanged += ComboBoxDatabasesSelectionChanged;
                if (LComboBoxItemDatabaseSelected != null) { ComboBoxDatabases.SelectedItem = LComboBoxItemDatabaseSelected; }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "ShowDatabasesInComboBox"); }
        }

        private void ComboBoxDatabasesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem LComboBoxItemLoginIDSelected = null;
            string LStrSingleLogin = string.Empty;

            string LStrCreatedLoginAccount = string.Empty;
            string LStrCreatedLoginPassword = string.Empty;

            try
            {
                if (IParameterStep23.ObjectSource0 != null)
                {
                    List<string> LListStrCreateLogin = IParameterStep23.ObjectSource0 as List<string>;
                    LStrCreatedLoginAccount = LListStrCreateLogin[0];
                    LStrCreatedLoginPassword = LListStrCreateLogin[1];
                }

                ComboBoxItem LComboBoxItemSelectedDatabase = ComboBoxDatabases.SelectedItem as ComboBoxItem;
                if (LComboBoxItemSelectedDatabase == null) { return; }
                string LStrSelectedDatabase = LComboBoxItemSelectedDatabase.DataContext.ToString();
                DataTable LDataTableDBLogins = IWCFOperationDataArgs.DataSetReturn.Tables[LStrSelectedDatabase];
                ComboBoxLoginName.Items.Clear();
                foreach (DataRow LDataRowLogin in LDataTableDBLogins.Rows)
                {
                    ComboBoxItem LComboBoxItemSingleLogin = new ComboBoxItem();
                    LComboBoxItemSingleLogin.Style = (Style)App.Current.Resources["ComboBoxItemFontStyle"];
                    LStrSingleLogin = LDataRowLogin[0].ToString();
                    LComboBoxItemSingleLogin.Content = LStrSingleLogin;
                    LComboBoxItemSingleLogin.Tag = LStrSingleLogin;
                    ComboBoxLoginName.Items.Add(LComboBoxItemSingleLogin);
                    if (LStrSingleLogin == LStrCreatedLoginAccount) { LComboBoxItemLoginIDSelected = LComboBoxItemSingleLogin; }
                }
                if (LComboBoxItemLoginIDSelected != null)
                {
                    ComboBoxLoginName.SelectedItem = LComboBoxItemLoginIDSelected;
                    PasswordBoxLoginPassword.Password = LStrCreatedLoginPassword;
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "ComboBoxDatabasesSelectionChanged"); }
        }
    }
}
