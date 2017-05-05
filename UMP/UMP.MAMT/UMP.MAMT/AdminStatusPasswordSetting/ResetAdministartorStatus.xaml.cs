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
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.AdminStatusPasswordSetting
{
    public partial class ResetAdministartorStatus : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        private DataTable IDataTableDatabaseProfile = null;
        private DataRow IDataRowRentInfo = null;

        public ResetAdministartorStatus(DataTable ADataTableDatabaseProfile, DataRow ADataRowRentInfo)
        {
            InitializeComponent();
            IDataTableDatabaseProfile = ADataTableDatabaseProfile;
            IDataRowRentInfo = ADataRowRentInfo;

            this.Loaded += ResetAdministartorStatus_Loaded;
            this.Closing += ResetAdministartorStatus_Closing;
            this.MouseLeftButtonDown += ResetAdministartorStatus_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;
            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseReset.Click += WindowsButtonClicked;
            ButtonResetStatus.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
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
                    case "ButtonCloseReset":
                        CloseThisWindow();
                        break;
                    case "ButtonResetStatus":
                        ResetRentAdminStatus();
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
            LabelResetAdministartorStatusTip.Content = App.GetDisplayCharater("M01115");
            TabItemResetAdministartorStatus.Header = " " + App.GetDisplayCharater("M01116") + " ";
            CheckBoxLock.Content = App.GetDisplayCharater("M01117");
            CheckBoxUnLock.Content = App.GetDisplayCharater("M01118");
            CheckBoxForceOffline.Content = App.GetDisplayCharater("M01119");
            ButtonResetStatus.Content = App.GetDisplayCharater("M01120");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01121");
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

        private void ResetAdministartorStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ResetAdministartorStatus_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void ResetAdministartorStatus_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageResetAdministartorStatus.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000039.ico"), UriKind.RelativeOrAbsolute));

            DisplayElementCharacters(false);

            GetRentAdminStatus();
        }

        #region 读取管理员当前状态
        private BackgroundWorker IBackgroundWorkerReadAdminStatus = null;

        private void GetRentAdminStatus()
        {
            string LStrRentToken = string.Empty;

            try
            {
                LStrRentToken = IDataRowRentInfo["C021"].ToString();

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01122"));
                IBoolInDoing = true;
                IBackgroundWorkerReadAdminStatus = new BackgroundWorker();
                IBackgroundWorkerReadAdminStatus.RunWorkerCompleted += IBackgroundWorkerReadAdminStatus_RunWorkerCompleted;
                IBackgroundWorkerReadAdminStatus.DoWork += IBackgroundWorkerReadAdminStatus_DoWork;
                IBackgroundWorkerReadAdminStatus.RunWorkerAsync(LStrRentToken);
            }
            catch
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerReadAdminStatus != null)
                {
                    IBackgroundWorkerReadAdminStatus.Dispose(); IBackgroundWorkerReadAdminStatus = null;
                }
            }
        }

        private void IBackgroundWorkerReadAdminStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrRentToken = e.Argument as string;

            e.Result = App.GetRentAdminStatus(IDataTableDatabaseProfile, LStrRentToken);
        }

        private void IBackgroundWorkerReadAdminStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                CheckBoxLock.IsChecked = false;
                CheckBoxUnLock.IsChecked = false;
                CheckBoxForceOffline.IsChecked = false;

                CheckBoxLock.IsEnabled = false;
                CheckBoxUnLock.IsEnabled = false;
                CheckBoxForceOffline.IsEnabled = false;

                List<string> LListStrGetReturn = e.Result as List<string>;
                if (LListStrGetReturn.Count <= 0) { return; }

                if (LListStrGetReturn[0] != "N") { CheckBoxUnLock.IsEnabled = true; }
                else { CheckBoxLock.IsEnabled = true; }

                if (LListStrGetReturn[1] != "0")
                {
                    CheckBoxForceOffline.IsEnabled = true;
                }
            }
            catch { }
            finally
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                IBackgroundWorkerReadAdminStatus.Dispose(); IBackgroundWorkerReadAdminStatus = null;
            }
        }

        #endregion

        #region 设置管理员状态
        private BackgroundWorker IBackgroundWorkerApplyAdminStatus = null;

        private void ResetRentAdminStatus()
        {
            //string LStrRentToken = string.Empty;
            List<string> LListStrResetObject = new List<string>();

            try
            {
                if (CheckBoxLock.IsChecked == false && CheckBoxUnLock.IsChecked == false && CheckBoxForceOffline.IsChecked == false) { return; }

                //LStrRentToken = IDataRowRentInfo["C021"].ToString();

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01123"));

                IBoolInDoing = true;
                LListStrResetObject.Add("");
                LListStrResetObject.Add("");

                if (CheckBoxLock.IsChecked == true) { LListStrResetObject[0] = "U"; }
                if (CheckBoxUnLock.IsChecked == true) { LListStrResetObject[0] = "N"; }
                if (CheckBoxForceOffline.IsChecked == true) { LListStrResetObject[1] = "1"; }

                IBackgroundWorkerApplyAdminStatus = new BackgroundWorker();
                IBackgroundWorkerApplyAdminStatus.RunWorkerCompleted += IBackgroundWorkerApplyAdminStatus_RunWorkerCompleted;
                IBackgroundWorkerApplyAdminStatus.DoWork += IBackgroundWorkerApplyAdminStatus_DoWork;
                IBackgroundWorkerApplyAdminStatus.RunWorkerAsync(LListStrResetObject);
            }
            catch
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerApplyAdminStatus != null)
                {
                    IBackgroundWorkerApplyAdminStatus.Dispose(); IBackgroundWorkerApplyAdminStatus = null;
                }
            }
        }

        private void IBackgroundWorkerApplyAdminStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrResetObject = e.Argument as List<string>;
            string LStrRentToken = IDataRowRentInfo["C021"].ToString();

            List<string> LListStrResetReturn = new List<string>();

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

            if (!string.IsNullOrEmpty(LListStrResetObject[0]))
            {
                if (LListStrResetObject[0] == "U")
                {
                    LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentToken + " SET C008 = '1', C009 = '" + LListStrResetObject[0] + "' WHERE C001 = 102" + LStrRentToken + "00000000001";
                }
                else
                {
                    LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentToken + " SET C008 = '0', C009 = '" + LListStrResetObject[0] + "', C024 = 0 WHERE C001 = 102" + LStrRentToken + "00000000001";
                }
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                if (!LDatabaseOperation.BoolReturn) { LListStrResetReturn.Add(LListStrResetObject[0]); }
            }
            if (!string.IsNullOrEmpty(LListStrResetObject[1]))
            {
                LStrDynamicSQL = "UPDATE T_11_002_" + LStrRentToken + " SET C008 = '1' WHERE C001 = 102" + LStrRentToken + "00000000001";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                if (!LDatabaseOperation.BoolReturn) { LListStrResetReturn.Add(LListStrResetObject[1]); }
            }
            e.Result = LListStrResetReturn;
        }

        private void IBackgroundWorkerApplyAdminStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                List<string> LListStrResetReturn = e.Result as List<string>;

                GetRentAdminStatus();
                if (LListStrResetReturn.Count <= 0)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01124"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string LStrMessageBody = App.GetDisplayCharater("M01125") + "\n\n";
                    foreach (string LStrSingleReturn in LListStrResetReturn)
                    {
                        LStrMessageBody += App.GetConvertedData("ResetAdminStatus" + LStrSingleReturn) + "\n";
                    }
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
            finally
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                IBackgroundWorkerApplyAdminStatus.Dispose(); IBackgroundWorkerApplyAdminStatus = null;
            }
            
        }
        #endregion
    }
}
