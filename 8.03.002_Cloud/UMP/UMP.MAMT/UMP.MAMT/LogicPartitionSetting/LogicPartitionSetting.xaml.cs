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

namespace UMP.MAMT.LogicPartitionSetting
{
    public partial class LogicPartitionSetting : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        private DataRow IDataRowLogicPartition = null;
        private DataTable IDataTableDatabaseProfile = null;
        private string IStrRentToken = string.Empty;

        public LogicPartitionSetting(DataRow ADataRowLogicPartition, string AStrRentToken, DataTable ADataTableDatabaseProfile)
        {
            InitializeComponent();
            
            IDataRowLogicPartition = ADataRowLogicPartition;
            IStrRentToken = AStrRentToken;
            IDataTableDatabaseProfile = ADataTableDatabaseProfile;

            this.Loaded += LogicPartitionSetting_Loaded;
            this.Closing += LogicPartitionSetting_Closing;
            this.MouseLeftButtonDown += LogicPartitionSetting_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseSetting.Click += WindowsButtonClicked;
            ButtonLogicPartition.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelLogicPartition.Content = App.GetDisplayCharater("M01083");
            TabItemPartitionSetting.Header = " " + App.GetDisplayCharater("M01084") + " ";
            LabelLPGeneral.Content = App.GetDisplayCharater("M01085");
            LabelLPTarget.Content = App.GetDisplayCharater("M01086");
            LabelLPDepent.Content = App.GetDisplayCharater("M01087");
            LabelIsEnabled.Content = App.GetDisplayCharater("M01088");
            RadioButtonStatus1.Content = App.GetDisplayCharater("M01089");
            RadioButtonStatus0.Content = App.GetDisplayCharater("M01090");
            ButtonLogicPartition.Content = App.GetDisplayCharater("M01091");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01092");
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
                        LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
                        LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        LButtonClicked.ContextMenu.IsOpen = true;
                        break;
                    case "ButtonCloseSetting":
                        CloseThisWindow();
                        break;
                    case "ButtonLogicPartition":
                        WriteLogicPartionSetting2Database();
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

        private void LogicPartitionSetting_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageLogicPartition.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000034.ico"), UriKind.RelativeOrAbsolute));

            DisplayElementCharacters(false);

            ShowSettedLogicPartition(false);
        }

        private void ShowSettedLogicPartition(bool ABoolLanguageChange)
        {
            string LStrAlias = string.Empty;
            string LStrTablename = string.Empty;
            string LStrColumnName = string.Empty;
            string LStrS00 = string.Empty, LStrS01 = string.Empty, LStrS02 = string.Empty, LStrS03 = string.Empty;

            LStrAlias = IDataRowLogicPartition["Alias"].ToString();
            LStrTablename = IDataRowLogicPartition["TableName"].ToString();
            LStrColumnName = IDataRowLogicPartition["ColumnName"].ToString();
            LStrS00 = IDataRowLogicPartition["S00"].ToString();
            LStrS01 = IDataRowLogicPartition["S01"].ToString();
            LStrS02 = IDataRowLogicPartition["S02"].ToString();
            LStrS03 = IDataRowLogicPartition["S03"].ToString();
            
            TextBoxLPTarget.Text = App.GetConvertedData("LPT" + LStrAlias);
            TextBoxLPDepent.Text = App.GetConvertedData("LPC" + LStrAlias + "." + LStrColumnName);
            if (!ABoolLanguageChange)
            {
                if (LStrS01 == "1") { RadioButtonStatus1.IsChecked = true; } else { RadioButtonStatus0.IsChecked = true; }
            }
        }

        #region 设置逻辑分区生效
        private BackgroundWorker IBackgroundWorkerApplyChanged = null;
        private string IStrSettedStatus = string.Empty;
        private bool IBoolApplyReturn = false;
        private string IStrApplyReturn = string.Empty;
        private DataTable IDataTableNewLogicPartition = null;

        private void WriteLogicPartionSetting2Database()
        {
            string LStrOldStatus = string.Empty, LStrNewStatus = string.Empty;

            string LStrAlias = string.Empty;
            string LStrTablename = string.Empty;
            string LStrColumnName = string.Empty;
            string LStr00000001 = string.Empty;                 //1:分区名 "LP_" + 表名.Substring(2) + "." + 字段名;

            try
            {
                LStrOldStatus = IDataRowLogicPartition["S01"].ToString();
                if (RadioButtonStatus1.IsChecked == true) { LStrNewStatus = "1"; } else { LStrNewStatus = "0"; }
                if (LStrOldStatus == LStrNewStatus) { return; }
                if (MessageBox.Show(App.GetDisplayCharater("M01096"), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }

                LStrAlias = IDataRowLogicPartition["Alias"].ToString();
                LStrTablename = IDataRowLogicPartition["TableName"].ToString();
                LStrColumnName = IDataRowLogicPartition["ColumnName"].ToString();

                LStr00000001 = "LP_" + LStrTablename.Substring(2) + "." + LStrColumnName;
                IStrSettedStatus = LStrNewStatus;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01093"));
                IBoolInDoing = true;
                IBackgroundWorkerApplyChanged = new BackgroundWorker();
                IBackgroundWorkerApplyChanged.RunWorkerCompleted += IBackgroundWorkerApplyChanged_RunWorkerCompleted;
                IBackgroundWorkerApplyChanged.DoWork += IBackgroundWorkerApplyChanged_DoWork;
                IBackgroundWorkerApplyChanged.RunWorkerAsync(LStr00000001);
            }
            catch
            {
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerApplyChanged != null)
                {
                    IBackgroundWorkerApplyChanged.Dispose();
                    IBackgroundWorkerApplyChanged = null;
                }
            }
        }

        private void IBackgroundWorkerApplyChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStr00000001 = e.Argument as string;
            IBoolApplyReturn = true;
            IStrApplyReturn = string.Empty;
            if (App.GBoolRunAtServer)
            {
                IBoolApplyReturn = AtServerWriteLogicPartionSetting2Database(LStr00000001);
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
                MessageBox.Show(App.GetDisplayCharater("M01094") + "\n" + IStrApplyReturn, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
            LEventArgs.StrElementTag = "RTLPS";
            LEventArgs.ObjSource = IDataTableNewLogicPartition;
            if (IOperationEvent != null) { IOperationEvent(this, LEventArgs); }
            MessageBox.Show(App.GetDisplayCharater("M01095"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool AtServerWriteLogicPartionSetting2Database(string AStr00000001)
        {
            bool LBooReturn = true;
            string LStrSetted = string.Empty;
            string LStrDBType = string.Empty;
            string LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginID = string.Empty, LStrLoginPwd = string.Empty;
            string LStrServiceName = string.Empty;
            string LStrDepentColumn = string.Empty;

            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                #region 创建数据库连接串
                LStrDBType = IDataTableDatabaseProfile.Rows[0]["DBType"].ToString();
                LStrServerName = IDataTableDatabaseProfile.Rows[0]["ServerHost"].ToString();
                LStrServerPort = IDataTableDatabaseProfile.Rows[0]["ServerPort"].ToString();
                LStrLoginID = IDataTableDatabaseProfile.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = IDataTableDatabaseProfile.Rows[0]["LoginPwd"].ToString();
                LStrServiceName = IDataTableDatabaseProfile.Rows[0]["NameService"].ToString();
                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrServiceName, LStrLoginID, LStrLoginPwd);
                }
                else
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrServiceName, LStrLoginID, LStrLoginPwd);
                }
                #endregion

                #region 将设置数据写入数据库
                LStrSetted = IDataRowLogicPartition["S00"].ToString();
                LStrDepentColumn = IDataRowLogicPartition["P03"].ToString();
                if (LStrSetted == "0")
                {
                    if (LStrDBType == "2")
                    {
                        LStrDynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C006, C007, C008) VALUES('" + IStrRentToken + "', '" + AStr00000001 + "', '" + App.GStrApplicationVersion + "', 'LP', '" + IStrSettedStatus + "', GETUTCDATE(), GETUTCDATE(), '1', '" + LStrDepentColumn + "')";
                    }
                    else
                    {
                        LStrDynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C006, C007, C008) VALUES('" + IStrRentToken + "', '" + AStr00000001 + "', '" + App.GStrApplicationVersion + "', 'LP', '" + IStrSettedStatus + "', F_00_004(), F_00_004(), '1', '" + LStrDepentColumn + "')";
                    }
                }
                else
                {
                    if (LStrDBType == "2")
                    {
                        LStrDynamicSQL = "UPDATE T_00_000 SET C004 = '" + IStrSettedStatus + "', C006 = GETUTCDATE() WHERE C000 = '" + IStrRentToken + "' AND C001 = '" + AStr00000001 + "'";
                    }
                    else
                    {
                        LStrDynamicSQL = "UPDATE T_00_000 SET C004 = '" + IStrSettedStatus + "', C006 = F_00_004() WHERE C000 = '" + IStrRentToken + "' AND C001 = '" + AStr00000001 + "'";
                    }
                }
                
                LDatabaseOperation = LDataOperations01.ExecuteDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                LBooReturn = LDatabaseOperation.BoolReturn;
                IStrApplyReturn = LDatabaseOperation.StrReturn;
                #endregion

                #region 从数据库中读取新的设置信息
                LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + IStrRentToken + "' AND C001 = '" + AStr00000001 + "'";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                IDataTableNewLogicPartition = LDatabaseOperation.DataSetReturn.Tables[0];
                #endregion
            }
            catch (Exception ex)
            {
                LBooReturn = false;
                IStrApplyReturn = "AtServerWriteLogicPartionSetting2Database()\n" + ex.ToString();
            }

            return LBooReturn;
        }
        #endregion

        #region 窗口初始化一些事件，无关紧要
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

        private void LogicPartitionSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void LogicPartitionSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
                ShowSettedLogicPartition(true);
            }

            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
        }
        #endregion
        
    }
}
