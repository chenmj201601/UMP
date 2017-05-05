using PFShareClasses01;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Xml;
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.AdminStatusPasswordSetting
{
    public partial class ChangeOrResetAdminPassword : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        private DataTable IDataTableDatabaseProfile = null;
        private DataRow IDataRowRentInfo = null;

        //租户新增加用户的默认密码
        private string IStrRentNewAccountDefPassword = string.Empty;

        public ChangeOrResetAdminPassword(DataTable ADataTableDatabaseProfile, DataRow ADataRowRentInfo)
        {
            InitializeComponent();
            IDataTableDatabaseProfile = ADataTableDatabaseProfile;
            IDataRowRentInfo = ADataRowRentInfo;

            this.Loaded += ChangeOrResetAdminPassword_Loaded;
            this.Closing += ChangeOrResetAdminPassword_Closing;
            this.MouseLeftButtonDown += ChangeOrResetAdminPassword_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;
            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseChangeReset.Click += WindowsButtonClicked;
            ButtonApplyPassword.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        private void ChangeOrResetAdminPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ChangeOrResetAdminPassword_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void ChangeOrResetAdminPassword_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageChangeOrResetAdminPassword.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000025.ico"), UriKind.RelativeOrAbsolute));

            DisplayElementCharacters(false);

            GetRentNewAccountDefaultPassword();
            RadioButtonDefault.Checked += RadioButtonChangeTypeChecked;
            RadioButtonOther.Checked += RadioButtonChangeTypeChecked;
        }

        private void RadioButtonChangeTypeChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton LRadioButtonChecked = sender as RadioButton;
                string LStrChangeType = LRadioButtonChecked.DataContext.ToString();
                if (LStrChangeType == "D")
                {
                    PasswordBoxPassword01.Password = IStrRentNewAccountDefPassword;
                    PasswordBoxPassword02.Password = IStrRentNewAccountDefPassword;
                    PasswordBoxPassword01.IsEnabled = false;
                    PasswordBoxPassword02.IsEnabled = false;
                }
                else
                {
                    PasswordBoxPassword01.Password = "";
                    PasswordBoxPassword02.Password = "";
                    PasswordBoxPassword01.IsEnabled = true;
                    PasswordBoxPassword02.IsEnabled = true;
                }
            }
            catch { }
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
                    case "ButtonCloseChangeReset":
                        CloseThisWindow();
                        break;
                    case "ButtonApplyPassword":
                        ChangeOrResetAdminPasswordBegin();
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

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelChangeOrResetAdminPassword.Content = App.GetDisplayCharater("M01127");
            TabItemAdminPassword.Header = " " + App.GetDisplayCharater("M01128") + " ";
            LabelCurrentPassword.Content = App.GetDisplayCharater("M01129");

            RadioButtonDefault.Content = App.GetDisplayCharater("M01130");
            RadioButtonOther.Content = App.GetDisplayCharater("M01131");
            LabelLoginPassword01.Content = App.GetDisplayCharater("M01132");
            LabelLoginPassword02.Content = App.GetDisplayCharater("M01133");
            CheckBoxMustChangePassword.Content = App.GetDisplayCharater("M01134");

            ButtonApplyPassword.Content = App.GetDisplayCharater("M01135");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01136");
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

        #region 获取当前租户新增加用户的默认密码
        private BackgroundWorker IBackgroundWorkerGetNewAccountDefaultPassword = null;

        private void GetRentNewAccountDefaultPassword()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01126"));
                IBoolInDoing = true;
                IBackgroundWorkerGetNewAccountDefaultPassword = new BackgroundWorker();
                IBackgroundWorkerGetNewAccountDefaultPassword.RunWorkerCompleted += IBackgroundWorkerGetNewAccountDefaultPassword_RunWorkerCompleted;
                IBackgroundWorkerGetNewAccountDefaultPassword.DoWork += IBackgroundWorkerGetNewAccountDefaultPassword_DoWork;
                IBackgroundWorkerGetNewAccountDefaultPassword.RunWorkerAsync();
            }
            catch
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerGetNewAccountDefaultPassword != null)
                {
                    IBackgroundWorkerGetNewAccountDefaultPassword.Dispose(); IBackgroundWorkerGetNewAccountDefaultPassword = null;
                }
            }
        }

        private void IBackgroundWorkerGetNewAccountDefaultPassword_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrRentToken = IDataRowRentInfo["C021"].ToString();
            IStrRentNewAccountDefPassword = App.GetRentNewAccountDefaultPassword(IDataTableDatabaseProfile, LStrRentToken);
        }

        private void IBackgroundWorkerGetNewAccountDefaultPassword_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                IBackgroundWorkerGetNewAccountDefaultPassword.Dispose();
                IBackgroundWorkerGetNewAccountDefaultPassword = null;
                IIntTryResetCount = 0;
            }
            catch { }
        }
        #endregion

        #region 设置当前租户登录密码
        int IIntTryResetCount = 0;

        private BackgroundWorker IBackgroundWorkerApplyLoginPassword = null;

        private bool VerfiyCurrentLoginPassword()
        {
            bool LBoolReturn = true;

            try
            {
                string LStrCurrentLoginPassword = string.Empty;

                LStrCurrentLoginPassword = PasswordBoxCurrentPassword.Password;
                if (IIntTryResetCount > 3)
                {
                    IIntTryResetCount += 1;
                    MessageBox.Show(App.GetDisplayCharater("M01138"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (LStrCurrentLoginPassword != App.GStrLoginPassword)
                {
                    IIntTryResetCount += 1;
                    MessageBox.Show(App.GetDisplayCharater("M01137"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.ToString());
            }

            return LBoolReturn;
        }

        private void ChangeOrResetAdminPasswordBegin()
        {
            string LStrNewPwd01 = string.Empty, LStrNewPwd02 = string.Empty;
            List<string> LListStrArgs = new List<string>();

            if (!VerfiyCurrentLoginPassword()) {  return; }

            try
            {
                string LStrRentToken = IDataRowRentInfo["C021"].ToString();

                LStrNewPwd01 = PasswordBoxPassword01.Password;
                LStrNewPwd02 = PasswordBoxPassword02.Password;

                if (LStrNewPwd01 != LStrNewPwd02)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01139"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(LStrNewPwd01))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01140"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                

                LListStrArgs.Add(LStrNewPwd01);
                if (CheckBoxMustChangePassword.IsChecked == true) { LListStrArgs.Add("1"); } else { LListStrArgs.Add("0"); }

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01141"));
                IBoolInDoing = true;
                IBackgroundWorkerApplyLoginPassword = new BackgroundWorker();
                IBackgroundWorkerApplyLoginPassword.RunWorkerCompleted += IBackgroundWorkerApplyLoginPassword_RunWorkerCompleted;
                IBackgroundWorkerApplyLoginPassword.DoWork += IBackgroundWorkerApplyLoginPassword_DoWork;
                IBackgroundWorkerApplyLoginPassword.RunWorkerAsync(LListStrArgs);
            }
            catch
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerApplyLoginPassword != null)
                {
                    IBackgroundWorkerApplyLoginPassword.Dispose(); IBackgroundWorkerApplyLoginPassword = null;
                }
            }
        }

        private void IBackgroundWorkerApplyLoginPassword_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrArgs = e.Argument as List<string>;
            string LStrAccountLoginPassword = string.Empty;

            string LStrRentToken = IDataRowRentInfo["C021"].ToString();

            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;
            string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;

            DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();
            DataOperations01 LDataOperations01 = new DataOperations01();

            LStrDBType = IDataTableDatabaseProfile.Rows[0]["DBType"].ToString();
            LStrServerName = IDataTableDatabaseProfile.Rows[0]["ServerHost"].ToString();
            LStrServerPort = IDataTableDatabaseProfile.Rows[0]["ServerPort"].ToString();
            LStrLoginAccount = IDataTableDatabaseProfile.Rows[0]["LoginID"].ToString();
            LStrLoginPwd = IDataTableDatabaseProfile.Rows[0]["LoginPwd"].ToString();
            LStrDBOrServiceName = IDataTableDatabaseProfile.Rows[0]["NameService"].ToString();

            if (LStrDBType == "2")
            {
                LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
            }
            if (LStrDBType == "3")
            {
                LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
            }

            LStrAccountLoginPassword = LListStrArgs[0];
            LStrAccountLoginPassword = App.EncryptionAndDecryptionString("102" + LStrRentToken + "00000000001" + LStrAccountLoginPassword, "SM002");

            if (LListStrArgs[1] == "1")
            {
                LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentToken + " SET C004 = '" + LStrAccountLoginPassword + "', C025 = '1' WHERE C001 = 102" + LStrRentToken + "00000000001";
            }
            else
            {
                LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentToken + " SET C004 = '" + LStrAccountLoginPassword + "' WHERE C001 = 102" + LStrRentToken + "00000000001";
            }

            LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
            if (LDatabaseOperation.BoolReturn)
            {
                e.Result = "";
                ChangeLocalAdminPassword(LListStrArgs[0], LStrRentToken);
            }
            else
            {
                e.Result = LDatabaseOperation.StrReturn;
            }
        }

        private void IBackgroundWorkerApplyLoginPassword_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                IBackgroundWorkerApplyLoginPassword.Dispose(); IBackgroundWorkerApplyLoginPassword = null;
                string LStrApplyReturn = e.Result as string;

                if (string.IsNullOrEmpty(LStrApplyReturn))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01142"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseThisWindow();
                }
                else
                {
                    MessageBox.Show(App.GetDisplayCharater("M01143") + "\n\n" + LStrApplyReturn, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch { }
        }

        private void ChangeLocalAdminPassword(string AStrInputPassword, string AStrRentToken)
        {
            try
            {
                string LStrXmlFileName = string.Empty;
                string LStrA01 = string.Empty;
                string LStrSAPassword = string.Empty;
                string LStrUserID = string.Empty;

                LStrUserID = "102" + AStrRentToken + "00000000001";
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                    if (LStrA01 != LStrUserID) { continue; }
                    LStrSAPassword = App.EncryptionAndDecryptionString(LStrUserID + AStrInputPassword, "SM001");
                    LXmlNodeSingleUser.Attributes["A03"].Value = LStrSAPassword;
                    break;
                }
                LXmlDocArgs02.Save(LStrXmlFileName);
            }
            catch { }
        }
        #endregion
    }
}
