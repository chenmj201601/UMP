using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using PFShareClassesS;
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.DatabaseSetting
{
    public partial class DatabaseProfieEdit : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        private DataTable IDataTableDatabaseProfile = null;
        private List<string> IListStrOldProfile = new List<string>();

        public DatabaseProfieEdit(DataTable ADataTableDatabaseProfile)
        {
            InitializeComponent();

            IDataTableDatabaseProfile = ADataTableDatabaseProfile;

            this.Loaded += DatabaseProfieEdit_Loaded;
            this.Closing += DatabaseProfieEdit_Closing;
            this.MouseLeftButtonDown += DatabaseProfieEdit_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseEdit.Click += WindowsButtonClicked;
            ButtonDatabaseProfile.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        private void DatabaseProfieEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void DatabaseProfieEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInDoing) { e.Cancel = true; return; }
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }

        private void GSystemMainWindow_IOperationEvent(object sender, MamtOperationEventArgs e)
        {
            if (e.StrElementTag == "CSID")
            {
                App.DrawWindowsBackGround(this);
            }

            if (e.StrElementTag == "CLID")
            {
                DisplayElementCharacters(true);
            }

            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
        }

        private void DatabaseProfieEdit_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageDatabaseProfile.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            

            DisplayElementCharacters(false);

            TextBoxServerName.Focus();

            RadioButtonDBType2.Checked += RadioButtonDBTypeChecked;
            RadioButtonDBType3.Checked += RadioButtonDBTypeChecked;

            ShowSettedDatabaseProfile();
        }

        private void RadioButtonDBTypeChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton LRadioButtonDBType = sender as RadioButton;
                if (LRadioButtonDBType.Name == "RadioButtonDBType2")
                {
                    LabelServiceName.Content = App.GetDisplayCharater("M01061");
                    IDataTableDatabaseProfile.Rows[0]["DBType"] = "2";
                }
                else
                {
                    LabelServiceName.Content = App.GetDisplayCharater("M01062");
                    IDataTableDatabaseProfile.Rows[0]["DBType"] = "3";
                }
                
            }
            catch { }
        }

        private void ShowSettedDatabaseProfile()
        {
            if (IDataTableDatabaseProfile.Rows[0]["DBType"].ToString() == "2")
            {
                RadioButtonDBType2.IsChecked = true;
            }
            else
            {
                RadioButtonDBType3.IsChecked = true;
            }

            TextBoxServerName.Text = IDataTableDatabaseProfile.Rows[0]["ServerHost"].ToString();
            TextBoxServerPort.Text = IDataTableDatabaseProfile.Rows[0]["ServerPort"].ToString();
            TextBoxLoginAccount.Text = IDataTableDatabaseProfile.Rows[0]["LoginID"].ToString();
            PasswordBoxLoginPassword.Password = IDataTableDatabaseProfile.Rows[0]["LoginPwd"].ToString();
            TextBoxServiceName.Text = IDataTableDatabaseProfile.Rows[0]["NameService"].ToString();

            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["DBType"].ToString());
            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["ServerHost"].ToString());
            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["ServerPort"].ToString());
            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["LoginID"].ToString());
            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["LoginPwd"].ToString());
            IListStrOldProfile.Add(IDataTableDatabaseProfile.Rows[0]["NameService"].ToString());
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelDatabaseProfile.Content = App.GetDisplayCharater("M01055");
            TabItemDatabaseProfile.Header = " " + App.GetDisplayCharater("M01065") + " ";
            LabelDatabaseType.Content = App.GetDisplayCharater("M01056");
            LabelServerName.Content = App.GetDisplayCharater("M01057");
            LabelServerPort.Content = App.GetDisplayCharater("M01058");
            LabelLoginAccount.Content = App.GetDisplayCharater("M01059");
            LabelLoginPassword.Content = App.GetDisplayCharater("M01060");
            if (IDataTableDatabaseProfile.Rows[0]["DBType"].ToString() == "2")
            {
                LabelServiceName.Content = App.GetDisplayCharater("M01061");
            }
            else
            {
                LabelServiceName.Content = App.GetDisplayCharater("M01062");
            }
            ButtonDatabaseProfile.Content = App.GetDisplayCharater("M01032");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01033");
        }

        private void WindowsButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;
                string LStrClickedName = LButtonClicked.Name;

                switch (LStrClickedName)
                {
                    case "ButtonApplicationMenu":
                        //目标   
                        LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
                        //位置   
                        LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        //显示菜单   
                        LButtonClicked.ContextMenu.IsOpen = true;
                        break;
                    case "ButtonCloseEdit":
                        CloseThisWindow();
                        break;
                    case "ButtonDatabaseProfile":
                        BeginResetDatabaseProfile();
                        break;
                    case "ButtonCloseWindow":
                        CloseThisWindow();
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        private void CloseThisWindow()
        {
            if (!IBoolInDoing) { this.Close(); }
        }

        private void MainPanel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var uie = e.OriginalSource as UIElement;
                if (e.Key == Key.Enter)
                {
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
            catch { }
        }

        #region 设置数据库连接参数，并通知Service 01 重新获取数据库连接参数
        private BackgroundWorker IBackgroundWorkerApplyChanged = null;
        private List<string> IListStrNewProfile = new List<string>();

        private bool IBoolApplyReturn = false;
        private string IStrApplyReturn = string.Empty;

        private void BeginResetDatabaseProfile()
        {
            string LStrCallReturn = string.Empty;

            try
            {
                if (IBoolInDoing) { return; }

                if (ConnectProfileIsChanged(ref LStrCallReturn)) { return; }
                if (!string.IsNullOrEmpty(LStrCallReturn))
                {
                    MessageBox.Show(App.GetDisplayCharater("E02" + LStrCallReturn), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IBoolInDoing = true;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01067"));
                IBackgroundWorkerApplyChanged = new BackgroundWorker();
                IBackgroundWorkerApplyChanged.RunWorkerCompleted += IBackgroundWorkerApplyChanged_RunWorkerCompleted;
                IBackgroundWorkerApplyChanged.DoWork += IBackgroundWorkerApplyChanged_DoWork;
                IBackgroundWorkerApplyChanged.RunWorkerAsync(IListStrNewProfile);
            }
            catch (Exception ex)
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerApplyChanged != null)
                {
                    IBackgroundWorkerApplyChanged.Dispose();
                    IBackgroundWorkerApplyChanged = null;
                }
                App.ShowExceptionMessage("BeginResetDatabaseProfile()\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerApplyChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrParameters = e.Argument as List<string>;
            IBoolApplyReturn = true;
            IStrApplyReturn = string.Empty;
            if (App.GBoolRunAtServer)
            {
                IBoolApplyReturn = AtServerConnect2SpecifiedDatabase(LListStrParameters);
            }
        }

        private void IBackgroundWorkerApplyChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBoolInDoing = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IBackgroundWorkerApplyChanged.Dispose();
            IBackgroundWorkerApplyChanged = null;
            if (!IBoolApplyReturn)
            {
                if (IStrApplyReturn == "0.00.000")
                {
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M01068"), IListStrNewProfile[1], IListStrNewProfile[5]), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(App.GetDisplayCharater("M01069") + "\n" + IStrApplyReturn, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }

            if (MessageBox.Show(string.Format(App.GetDisplayCharater("M01070"), IListStrNewProfile[1], IListStrNewProfile[5]), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes) { return; }

            if (App.GBoolRunAtServer)
            {
                IStrApplyReturn = string.Empty;
                IBoolApplyReturn = WriteDatabaseProfile2Xml();
            }

            if (!IBoolApplyReturn)
            {
                MessageBox.Show(App.GetDisplayCharater("M01071") + "\n" + IStrApplyReturn, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BeginRestartService();
        }

        private bool ConnectProfileIsChanged(ref string AStrErrorReturn)
        {
            bool LBoolReturn = false;
            string LStrDBType = string.Empty;
            string LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginID = string.Empty, LStrLoginPwd = string.Empty;
            string LStrServiceName = string.Empty;
            UInt16 LUintServerPort = 0;

            try
            {
                AStrErrorReturn = string.Empty;
                IListStrNewProfile.Clear();
                if (RadioButtonDBType2.IsChecked == true) { LStrDBType = "2"; } else { LStrDBType = "3"; }
                LStrServerName = TextBoxServerName.Text.Trim();
                LStrServerPort = TextBoxServerPort.Text.Trim();
                LStrLoginID = TextBoxLoginAccount.Text.Trim();
                LStrLoginPwd = PasswordBoxLoginPassword.Password;
                LStrServiceName = TextBoxServiceName.Text.Trim();

                if (string.IsNullOrEmpty(LStrServerName)) { AStrErrorReturn = "001"; return false; }
                if (string.IsNullOrEmpty(LStrServerPort)) { AStrErrorReturn = "002"; return false; }
                if (!UInt16.TryParse(LStrServerPort, out LUintServerPort)) { AStrErrorReturn = "002"; return false; }
                if (LUintServerPort <= 1024 || LUintServerPort > 65535) { AStrErrorReturn = "002"; return false; }
                if (string.IsNullOrEmpty(LStrLoginID)) { AStrErrorReturn = "003"; return false; }
                if (string.IsNullOrEmpty(LStrServiceName))
                {
                    if (LStrDBType == "2") { AStrErrorReturn = "004"; return false; }
                    if (LStrDBType == "3") { AStrErrorReturn = "005"; return false; }
                }

                IListStrNewProfile.Add(LStrDBType);         //0
                IListStrNewProfile.Add(LStrServerName);     //1
                IListStrNewProfile.Add(LStrServerPort);     //2
                IListStrNewProfile.Add(LStrLoginID);        //3
                IListStrNewProfile.Add(LStrLoginPwd);       //4
                IListStrNewProfile.Add(LStrServiceName);    //5

                LBoolReturn = true;
                for (int LIntLoop = 0; LIntLoop < 6; LIntLoop++)
                {
                    if (IListStrNewProfile[LIntLoop] != IListStrOldProfile[LIntLoop]) { LBoolReturn = false; break; }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = true;
                AStrErrorReturn = string.Empty;
            }

            return LBoolReturn;
        }

        private bool AtServerConnect2SpecifiedDatabase(List<string> AlistDatabaseProfile)
        {
            bool LBooReturn = true;
            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;
            string LStrSelectC000 = App.GStrSpliterChar + App.GStrSpliterChar + App.GStrSpliterChar + App.GStrSpliterChar + "1";
            DataSet LDataSetSelectReturn = new DataSet();
            int LIntSelectedReturn = 0;

            LStrDynamicSQL = "SELECT C002 FROM T_00_000 WHERE C000 = '" + LStrSelectC000 + "'";
            IStrApplyReturn = "0.00.000";

            #region 连接到MS SQL Server
            if (AlistDatabaseProfile[0] == "2")
            {
                SqlConnection LSqlConnection = null;
                try
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", AlistDatabaseProfile[1], AlistDatabaseProfile[2], AlistDatabaseProfile[5], AlistDatabaseProfile[3], AlistDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();

                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                    LIntSelectedReturn = LSqlDataAdapter.Fill(LDataSetSelectReturn);
                    LSqlDataAdapter.Dispose();
                    if (LIntSelectedReturn != 1) { return false; }
                    IStrApplyReturn = LDataSetSelectReturn.Tables[0].Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    LBooReturn = false;
                    IStrApplyReturn = ex.Message;
                }
                finally
                {
                    if (LSqlConnection != null)
                    {
                        if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                        LSqlConnection.Dispose(); LSqlConnection = null;
                    }
                }
            }
            #endregion

            #region 连接到 Oracle
            if (AlistDatabaseProfile[0] == "3")
            {
                OracleConnection LOracleConnection = null;
                try
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", AlistDatabaseProfile[1], AlistDatabaseProfile[2], AlistDatabaseProfile[5], AlistDatabaseProfile[3], AlistDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                    LIntSelectedReturn = LOracleDataAdapter.Fill(LDataSetSelectReturn);
                    LOracleDataAdapter.Dispose();
                    if (LIntSelectedReturn != 1) { return false; }
                    IStrApplyReturn = LDataSetSelectReturn.Tables[0].Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    LBooReturn = false;
                    IStrApplyReturn = ex.Message;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }
            #endregion

            return LBooReturn;
        }

        private bool WriteDatabaseProfile2Xml()
        {
            bool LBoolReturn = true;

            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrXmlFileName = string.Empty;
            string LStrP02 = string.Empty;
            string LStrP04 = string.Empty, LStrP05 = string.Empty, LStrP06 = string.Empty, LStrP07 = string.Empty, LStrP08 = string.Empty;

            try
            {
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabaseParameters = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;
                foreach (XmlNode LXmlNodeSingleDatabaseParameter in LXmlNodeListDatabaseParameters)
                {
                    LXmlNodeSingleDatabaseParameter.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString("0", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP02 = LXmlNodeSingleDatabaseParameter.Attributes["P02"].Value;
                    LStrP02 = EncryptionAndDecryption.EncryptDecryptString(LStrP02, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrP02 != IListStrNewProfile[0]) { continue; }
                    LXmlNodeSingleDatabaseParameter.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP04 = EncryptionAndDecryption.EncryptDecryptString(IListStrNewProfile[1], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP05 = EncryptionAndDecryption.EncryptDecryptString(IListStrNewProfile[2], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP06 = EncryptionAndDecryption.EncryptDecryptString(IListStrNewProfile[5], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP07 = EncryptionAndDecryption.EncryptDecryptString(IListStrNewProfile[3], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP08 = EncryptionAndDecryption.EncryptDecryptString(IListStrNewProfile[4], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LXmlNodeSingleDatabaseParameter.Attributes["P04"].Value = LStrP04;
                    LXmlNodeSingleDatabaseParameter.Attributes["P05"].Value = LStrP05;
                    LXmlNodeSingleDatabaseParameter.Attributes["P06"].Value = LStrP06;
                    LXmlNodeSingleDatabaseParameter.Attributes["P07"].Value = LStrP07;
                    LXmlNodeSingleDatabaseParameter.Attributes["P08"].Value = LStrP08;
                }
                LXmlDocArgs01.Save(LStrXmlFileName);
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                IStrApplyReturn = "WriteDatabaseProfile2Xml()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        #endregion

        #region 数据库参数改变后重新启动Service 01
        private BackgroundWorker IBackgroundWorkerRestartService = null;

        private void BeginRestartService()
        {
            if (IBoolInDoing) { return; }

            IBoolInDoing = true;
            App.ShowCurrentStatus(1, App.GetDisplayCharater("P01004"));
            IBackgroundWorkerRestartService = new BackgroundWorker();
            IBackgroundWorkerRestartService.RunWorkerCompleted += IBackgroundWorkerRestartService_RunWorkerCompleted;
            IBackgroundWorkerRestartService.DoWork += IBackgroundWorkerRestartService_DoWork;
            IBackgroundWorkerRestartService.RunWorkerAsync();
        }

        private void IBackgroundWorkerRestartService_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] LStrArrayWriteLine = new string[4];
                Stream LStreamBatch = File.Create(App.GStrApplicationDirectory + @"\RestarService.bat");
                LStreamBatch.Close();
                LStrArrayWriteLine[0] = "net stop \"UMP Service 01\"";
                LStrArrayWriteLine[1] = "net start \"UMP Service 01\"";
                LStrArrayWriteLine[2] = "net stop \"UMP Service 02\"";
                LStrArrayWriteLine[3] = "net start \"UMP Service 02\"";
                File.WriteAllLines(App.GStrApplicationDirectory + @"\RestarService.bat", LStrArrayWriteLine);
                App.ExecuteBatchCommand(App.GStrApplicationDirectory + @"\RestarService.bat");
                File.Delete(App.GStrApplicationDirectory + @"\RestarService.bat");
            }
            catch { }
        }

        private void IBackgroundWorkerRestartService_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBoolInDoing = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);

            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
            LEventArgs.StrElementTag = "RCDBP";
            if (IOperationEvent != null) { IOperationEvent(this, LEventArgs); }
            MessageBox.Show(App.GetDisplayCharater("M01072"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}
