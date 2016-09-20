using Microsoft.Win32;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using UMP.MAMT.AdminStatusPasswordSetting;
using UMP.MAMT.BasicControls;
using UMP.MAMT.CertificateSetting;
using UMP.MAMT.CreateDatabaseObject;
using UMP.MAMT.DatabaseSetting;
using UMP.MAMT.IISBindingSetting;
using UMP.MAMT.LicenseServer;
using UMP.MAMT.PublicClasses;
using VoiceCyber.UMP.Communications;

namespace UMP.MAMT.BasicModule
{
    public partial class SystemMainWindow : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        #region 界面使用的四个基本 UserControl
        private UCFootStatusBar IUCStatusBar = null;
        private UCServerObjects IUCServerObjects = null;
        private UCObjectDetails IUCObjectDetails = null;
        private UCObjectOperations IUCObjectOperations = null;
        #endregion

        #region 应用程序使用的等待窗口
        private WaitForApplicationDoing IWaitForApplicationDoing = null;
        #endregion

        #region 主界面逻辑控制变量
        //是否是第一次关闭主窗口
        private bool IBoolIsFirstCloseApplication = true;
        #endregion

        #region BackgroundWorker
        private BackgroundWorker IBackgroundWorkerFirstOpenApplication = null;
        #endregion

        #region 功能、操作分割位置
        private Double IDoubleColumnServerObjectWidth = 0.0;
        private Double IDoubleColumnServerOperationsWidth = 0.0;
        #endregion

        #region 所有连接的服务器信息采用DataSet保存数据
        private List<DataSet> IListDataSetConnectedServer = new List<DataSet>();
        private List<List<string>> IListListStrConnectedArgs = new List<List<string>>();
	    #endregion

        public SystemMainWindow()
        {
            InitializeComponent();
            this.Loaded += SystemMainWindow_Loaded;
            this.Closing += SystemMainWindow_Closing;
        }

        private void SystemMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.GSystemMainWindow = this;
                App.LoadApplicationResources();
                ImageApplicationLog.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\ApplicationInTaskbar.ico"), UriKind.RelativeOrAbsolute));
                this.Icon = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\ApplicationInTaskbar.ico"), UriKind.RelativeOrAbsolute));
                this.Title = LabelApplicationTitle.Content.ToString();
                App.DrawWindowsBackGround(this);

                App.WriteLog("MainLoad", string.Format("Icon loaded."));

                ButtonApplicationMenu.Click += ButtonInHeaderClicked;
                ButtonMinimized.Click += ButtonInHeaderClicked;
                ButtonMaximized.Click += ButtonInHeaderClicked;
                ButtonCloseApp.Click += ButtonInHeaderClicked;

                this.MouseLeftButtonDown += SystemMainWindow_MouseLeftButtonDown;
                this.KeyDown += SystemMainWindow_KeyDown;

                ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();

                App.WriteLog("MainLoad", string.Format("AppMenu loaded."));

                InitObjectBasicDetailOperations();
                InitApplicationStatusBar();

                IUCObjectOperations.AddObjectOperations(true, "000", null, false);

                try
                {
                    IBackgroundWorkerFirstOpenApplication = new BackgroundWorker();
                    IBackgroundWorkerFirstOpenApplication.RunWorkerCompleted += IBackgroundWorkerFirstOpenApplication_RunWorkerCompleted;
                    IBackgroundWorkerFirstOpenApplication.DoWork += IBackgroundWorkerFirstOpenApplication_DoWork;
                    IBackgroundWorkerFirstOpenApplication.RunWorkerAsync();
                }
                catch
                {
                    if (IBackgroundWorkerFirstOpenApplication != null)
                    {
                        IBackgroundWorkerFirstOpenApplication.Dispose(); IBackgroundWorkerFirstOpenApplication = null;
                    }
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("MainLoad", string.Format("Load fail.\t{0}", ex.Message));
            }
        }

        private void IBackgroundWorkerFirstOpenApplication_DoWork(object sender, DoWorkEventArgs e)
        {
            //不做任何操作
        }

        private void IBackgroundWorkerFirstOpenApplication_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                InitObjectItemDetailTempleteTabItem();
                OpenConnectToApplicationServer();
                IBackgroundWorkerFirstOpenApplication.Dispose();
                IBackgroundWorkerFirstOpenApplication = null;
            }
            catch (Exception ex)
            {
                App.WriteLog("FirstOpen", string.Format("Fail.\t{0}", ex.Message));
            }
           
        }

        private void InitObjectItemDetailTempleteTabItem()
        {
            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
            LEventArgs.StrElementTag = "0000000000";
            IUCObjectDetails.OpenServerObjectSingleInformation(this, LEventArgs);
        }

        /// <summary>
        /// 从其他对象发送过来的消息入口
        /// </summary>
        /// <param name="AEventArgs"></param>
        public void ObjectOperationsEvent(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                switch (AEventArgs.StrElementTag)
                {
                    case "FHDB":        //隐藏对象资源列表
                        HideServerBasicOperationObject(false, "B");
                        break;
                    case "FHDO":        //隐藏操作窗口
                        HideServerBasicOperationObject(false, "O");
                        break;
                    case "CLID":        //切换语言
                        ChangeApplicationElementLanguages(AEventArgs);
                        break;
                    case "CSID":        //切换皮护
                        ChangeApplicationStyle(AEventArgs);
                        break;
                    case "SSLC":        //在状态栏中显示当前的操作
                        ShowCurrentStatus(AEventArgs);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { App.ShowExceptionMessage(ex.ToString()); }
        }

        #region 关闭系统主窗口
        private void CloseMainWindow()
        {
            this.Close();
        }

        private void SystemMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IBoolIsFirstCloseApplication)
            {
                if (MessageBox.Show(App.GetDisplayCharater("M01014"), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes) { e.Cancel = true; return; }
                e.Cancel = true;
                IIntDisconnectIndex = 0;
                DisconnectToAllServers();
                System.Threading.Thread.Sleep(500);
            }
        }
        #endregion

        #region 显示或关闭 对象 或 操作 栏
        private void SystemMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8) { HideServerBasicOperationObject(true, "B"); }
            if (e.Key == Key.F9) { HideServerBasicOperationObject(true, "O"); }
        }

        private void HideServerBasicOperationObject(bool ABoolShow, string AStrBasicOrOperation)
        {
            try
            {
                if (AStrBasicOrOperation == "B")
                {
                    if (!ABoolShow)
                    {
                        ColumnServerObject.MinWidth = 0;
                        IDoubleColumnServerObjectWidth = ColumnServerObject.Width.Value;
                        ColumnServerObject.Width = new GridLength(0);
                        ColunmSplitterLeft.Width = new GridLength(0);
                    }
                    else
                    {
                        if (ColumnServerObject.MinWidth != 0) { return; }
                        ColumnServerObject.MinWidth = 200;
                        ColumnServerObject.Width = new GridLength(IDoubleColumnServerObjectWidth);
                        ColunmSplitterLeft.Width = new GridLength(2);
                    }
                }
                if (AStrBasicOrOperation == "O")
                {
                    if (!ABoolShow)
                    {
                        ColumnServerOperations.MinWidth = 0;
                        IDoubleColumnServerOperationsWidth = ColumnServerOperations.Width.Value;
                        ColumnServerOperations.Width = new GridLength(0);
                        ColunmSplitterRight.Width = new GridLength(0);
                    }
                    else
                    {
                        if (ColumnServerOperations.MinWidth != 0) { return; }
                        ColumnServerOperations.MinWidth = 180;
                        ColumnServerOperations.Width = new GridLength(IDoubleColumnServerOperationsWidth);
                        ColunmSplitterRight.Width = new GridLength(2);
                    }
                }
            }
            catch { }
        }
        #endregion

        private void SystemMainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonInHeaderClicked(object sender, RoutedEventArgs e)
        {
            string LStrClickedButton = string.Empty;

            try
            {
                Button LClickedButton = e.Source as Button;
                LStrClickedButton = LClickedButton.Name;
                switch (LStrClickedButton)
                {
                    case "ButtonApplicationMenu":
                        ButtonApplicationMenuClicked(sender);
                        break;
                    case "ButtonMinimized":
                        this.WindowState = System.Windows.WindowState.Minimized;
                        break;
                    case "ButtonMaximized":
                        if (this.WindowState == System.Windows.WindowState.Normal)
                        {
                            this.WindowState = System.Windows.WindowState.Maximized;
                            ButtonMaximized.Style = (Style)App.Current.Resources["ButtonRestoreStyle"];
                        }
                        else
                        {
                            this.WindowState = System.Windows.WindowState.Normal;
                            ButtonMaximized.Style = (Style)App.Current.Resources["ButtonMaximizedStyle"];
                        }
                        this.Hide();this.Show();
                        break;
                    case "ButtonCloseApp":
                        CloseMainWindow();
                        break;
                    default:
                        MessageBox.Show(LStrClickedButton);
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 显示系统下拉菜单
        private void ButtonApplicationMenuClicked(object AObjectSender)
        {
            Button ClickedButton = AObjectSender as Button;
            //目标   
            ClickedButton.ContextMenu.PlacementTarget = ClickedButton;
            //位置   
            ClickedButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ClickedButton.ContextMenu.IsOpen = true;
        }
	    #endregion

        #region 切换语言
        private void ChangeApplicationElementLanguages(MamtOperationEventArgs AEventArgs)
        {
            string LStrLanguageID = AEventArgs.ObjSource as string;
            App.GStrLoginUserCurrentLanguageID = LStrLanguageID;
            App.GClassSessionInfo.LangTypeInfo.LangID = int.Parse(LStrLanguageID);
            App.GClassSessionInfo.LangTypeID = int.Parse(LStrLanguageID);
            App.InitStartedAppliactionData();
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            if (IOperationEvent != null) { IOperationEvent(this, AEventArgs); }
            IUCServerObjects.DisplayElementCharacters(true);
            IUCObjectDetails.DisplayElementCharacters(true);
            IUCObjectOperations.DisplayElementCharacters(true);
        }
        #endregion

        #region 切换皮肤
        private void ChangeApplicationStyle(MamtOperationEventArgs AEventArgs)
        {
            string LStrSeasonCode = AEventArgs.ObjSource as string;
            App.GStrSeasonCode = LStrSeasonCode;
            App.LoadApplicationResources();
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.DrawWindowsBackGround(this);
            if (IOperationEvent != null) { IOperationEvent(this, AEventArgs); }
        }
        #endregion

        #region 初始化 Object 的 Basic、Detail、Operations对象
        private void InitObjectBasicDetailOperations()
        {
            try
            {
                if (IUCServerObjects != null)
                {
                    IUCServerObjects.IOperationEvent -= IUCObjectBDOOperationEvent;
                    GridServerBasic.Children.Clear();
                    IUCServerObjects = null;
                }
                if (IUCObjectDetails != null)
                {
                    IUCObjectDetails.IOperationEvent -= IUCObjectBDOOperationEvent;
                    GridServerDetail.Children.Clear();
                    IUCObjectDetails = null;
                }
                if (IUCObjectOperations != null)
                {
                    IUCObjectOperations.IOperationEvent -= IUCObjectBDOOperationEvent;
                    GridServerOperations.Children.Clear();
                    IUCObjectOperations = null;
                }

                if (IUCServerObjects == null) { IUCServerObjects = new UCServerObjects(); }
                IUCServerObjects.IOperationEvent += IUCObjectBDOOperationEvent;
                GridServerBasic.Children.Add(IUCServerObjects);

                if (IUCObjectDetails == null) { IUCObjectDetails = new UCObjectDetails(); }
                IUCObjectDetails.IOperationEvent += IUCObjectBDOOperationEvent;
                GridServerDetail.Children.Add(IUCObjectDetails);

                if (IUCObjectOperations == null) { IUCObjectOperations = new UCObjectOperations(); }
                IUCObjectOperations.IOperationEvent += IUCObjectBDOOperationEvent;
                GridServerOperations.Children.Add(IUCObjectOperations);
            }
            catch { }
        }
        #endregion

        #region 打开连接到应用服务器的窗口
        private void OpenConnectToApplicationServer()
        {
            if (IListListStrConnectedArgs.Count > 0) { return; }
            ConnectToServer LWindowConnectToServer = new ConnectToServer();
            LWindowConnectToServer.Owner = this;
            LWindowConnectToServer.IOperationEvent += IUCObjectBDOOperationEvent;
            LWindowConnectToServer.ShowDialog();
            this.Activate();
        }
        #endregion

        #region 打开网站绑定配置窗口
        private void OpenIISBindingSetting(MamtOperationEventArgs AEventArgs)
        {
            UMPSiteBindSetting LUMPSiteBindSetting = new UMPSiteBindSetting(App.GetIISBindingProtocol());
            LUMPSiteBindSetting.Owner = this;
            LUMPSiteBindSetting.IOperationEvent += IUCObjectBDOOperationEvent;
            LUMPSiteBindSetting.ShowDialog();
            this.Activate();
        }
        #endregion

        #region 打开安装证书的窗口
        private void OpenCertificateInstalling(MamtOperationEventArgs AEventArgs)
        {
            CertificateInstalling LCertificateInstalling = new CertificateInstalling();
            LCertificateInstalling.Owner = this;
            LCertificateInstalling.IOperationEvent += IUCObjectBDOOperationEvent;
            LCertificateInstalling.ShowDialog();
            this.Activate();
        }
        #endregion

        #region 打开 License Server 配置窗口
        private void OpenLicenseServiceSetting(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = AEventArgs.ObjSource as TreeViewItem;
                DataTable LDataTableLicenseService = LTreeViewItemCurrent.DataContext as DataTable;
                
                LicenseServiceSetting LLicenseServiceSetting = new LicenseServiceSetting(LDataTableLicenseService);
                LLicenseServiceSetting.Owner = this;
                LLicenseServiceSetting.IOperationEvent += IUCObjectBDOOperationEvent;
                LLicenseServiceSetting.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenLicenseServiceSetting()\n" + ex.Message);
            }
        }

        #endregion

        #region 打开创建数据库配置窗口
        
        private void OpenCreatNewDatabaseWindow(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                //未绑定服务器地址跟端口，不能创建数据库
                if (App.GClassSessionInfo.AppServerInfo == null || string.IsNullOrWhiteSpace(App.GClassSessionInfo.AppServerInfo.Address) || App.GClassSessionInfo.AppServerInfo.Address.Equals("127.0.0.1")) { return; }

                CreateDataBaseWindow LCreateDatabase = new CreateDataBaseWindow();
                LCreateDatabase.Owner = this;
                LCreateDatabase.IOperationEvent += IUCObjectBDOOperationEvent;
                LCreateDatabase.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenDatabaseProfieEdit()\n" + ex.Message);
            }
        }


        #endregion

        #region 打开更改数据库连接参数窗口
        private void OpenDatabaseProfieEdit(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = AEventArgs.ObjSource as TreeViewItem;
                DataTable LDataTableDatabaseProfile = LTreeViewItemCurrent.DataContext as DataTable;
                if (LDataTableDatabaseProfile.Rows.Count <= 0)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01066"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DatabaseProfieEdit LDatabaseProfieEdit = new DatabaseProfieEdit(LDataTableDatabaseProfile);
                LDatabaseProfieEdit.Owner = this;
                LDatabaseProfieEdit.IOperationEvent += IUCObjectBDOOperationEvent;
                LDatabaseProfieEdit.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenDatabaseProfieEdit()\n" + ex.Message);
            }
        }
        #endregion

        #region 打开数据库，获取租户列表和对应的逻辑分区列表
        private BackgroundWorker IBackgroundWorkerGetRentLogicPartion = null;
        DataTable IDataTableRentList = null;
        private void OpenSeletedDatabase(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                IDataTableRentList = null;
                DataTable LDataTableDBProfile = IUCServerObjects.ITreeViewItemCurrentSelected.DataContext as DataTable; 
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01073"), true);
                IBackgroundWorkerGetRentLogicPartion = new BackgroundWorker();
                IBackgroundWorkerGetRentLogicPartion.RunWorkerCompleted += IBackgroundWorkerGetRentLogicPartion_RunWorkerCompleted;
                IBackgroundWorkerGetRentLogicPartion.DoWork += IBackgroundWorkerGetRentLogicPartion_DoWork;
                IBackgroundWorkerGetRentLogicPartion.RunWorkerAsync(LDataTableDBProfile);
            }
            catch
            {
                if (IBackgroundWorkerGetRentLogicPartion != null)
                {
                    IBackgroundWorkerGetRentLogicPartion.Dispose();
                    IBackgroundWorkerGetRentLogicPartion = null;
                }
            }
        }

        private void IBackgroundWorkerGetRentLogicPartion_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable LDataTableDBProfile = e.Argument as DataTable;
            IDataTableRentList = App.GetRentInformation(LDataTableDBProfile);
        }

        private void IBackgroundWorkerGetRentLogicPartion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IUCServerObjects.RefreshTreeViewItemAfterOpenDatabase(IDataTableRentList);
        }
        #endregion

        #region 打开修改或重置租户管理员密码窗口
        private void OpenChangeOrResetAdminPassword(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = AEventArgs.ObjSource as TreeViewItem;
                TreeViewItem LTreeViewItemDatabase = (LTreeViewItemCurrent.Parent as TreeViewItem).Parent as TreeViewItem;
                DataRow LDataRowRentInfo = LTreeViewItemCurrent.DataContext as DataRow;
                DataTable LDataTableDatabaseProfile = LTreeViewItemDatabase.DataContext as DataTable;

                ChangeOrResetAdminPassword LChangeOrResetAdminPassword = new ChangeOrResetAdminPassword(LDataTableDatabaseProfile, LDataRowRentInfo);
                LChangeOrResetAdminPassword.Owner = this;
                LChangeOrResetAdminPassword.IOperationEvent += IUCObjectBDOOperationEvent;
                LChangeOrResetAdminPassword.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenChangeOrResetAdminPassword()\n" + ex.Message);
            }
        }
        #endregion

        #region 打开重置租户系统管理员状态窗口
        private void OpenResetRentAdminStatus(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = AEventArgs.ObjSource as TreeViewItem;
                TreeViewItem LTreeViewItemDatabase = (LTreeViewItemCurrent.Parent as TreeViewItem).Parent as TreeViewItem;
                DataRow LDataRowRentInfo = LTreeViewItemCurrent.DataContext as DataRow;
                DataTable LDataTableDatabaseProfile = LTreeViewItemDatabase.DataContext as DataTable;

                ResetAdministartorStatus LResetAdministartorStatus = new ResetAdministartorStatus(LDataTableDatabaseProfile, LDataRowRentInfo);
                LResetAdministartorStatus.Owner = this;
                LResetAdministartorStatus.IOperationEvent += IUCObjectBDOOperationEvent;
                LResetAdministartorStatus.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenResetRentAdminStatus()\n" + ex.Message);
            }
        }
        #endregion

        #region 打开逻辑分区设定窗口
        private void OpenLogicPartitionSetting(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = AEventArgs.ObjSource as TreeViewItem;
                TreeViewItem LTreeViewItemDatabase = ((LTreeViewItemCurrent.Parent as TreeViewItem).Parent as TreeViewItem).Parent as TreeViewItem;
                DataRow LDataRowLogicPartition = LTreeViewItemCurrent.DataContext as DataRow;
                DataTable LDataTableDatabaseProfile = LTreeViewItemDatabase.DataContext as DataTable;
                string LStrRentToken = LTreeViewItemCurrent.Tag as string;

                LogicPartitionSetting.LogicPartitionSetting LLogicPartitionSetting = new LogicPartitionSetting.LogicPartitionSetting(LDataRowLogicPartition, LStrRentToken, LDataTableDatabaseProfile);
                LLogicPartitionSetting.Owner = this;
                LLogicPartitionSetting.IOperationEvent += IUCObjectBDOOperationEvent;
                LLogicPartitionSetting.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("OpenLogicPartitionSetting()\n" + ex.Message);
            }
        }
        #endregion

        #region 从其他对象发送过来的消息入口
        private void IUCObjectBDOOperationEvent(object sender, MamtOperationEventArgs AEventArgs)
        {
            try
            {
                switch (AEventArgs.StrElementTag)
                {
                    case "FLTS":            //打开连接服务器窗口
                        OpenConnectToApplicationServer();
                        break;
                    case "SLTS":            //连接服务器成功后，显示信息
                        ShowConnectedServerInformation(AEventArgs);
                        break;
                    case "FHDB":            //隐藏对象资源列表
                        HideServerBasicOperationObject(false, "B");
                        break;
                    case "FHDO":            //隐藏操作列表
                        HideServerBasicOperationObject(false, "O");
                        break;
                    case "TSCH":            //TreeViewItem SelectedChanged
                        ShowObjectDetailsOperations(AEventArgs);
                        break;
                    case "IISB":            //打开网站绑定配置窗口
                        OpenIISBindingSetting(AEventArgs);
                        break;
                    case "RIISB":           //重新刷新Site绑定信息
                        IUCServerObjects.RefreshTreeViewItemAfterBinding(App.GetIISBindingProtocol());
                        break;
                    case "ICER":            //打开安装证书的窗口
                        OpenCertificateInstalling(AEventArgs);
                        break;
                    case "RICER":           //刷新证书安装后的信息
                        IUCServerObjects.RefreshTreeViewItemAfterInstallCertificate(App.GetCertificateInstalledInfo());
                        break;
                    case "CNDB":            //新建数据库--waves  2016-04-06 15:17:26
                        OpenCreatNewDatabaseWindow(AEventArgs);
                        break;
                    case "CDBP":            //更改数据库连接参数
                        OpenDatabaseProfieEdit(AEventArgs);
                        break;
                    case "RCDBP":           //刷新更改后的数据库连接参数
                        IUCServerObjects.RefreshTreeViewItemAfterChangeDBProfile(App.GetUMPDatabaseProfile());
                        break;
                    case "OPDB":            //打开数据库，获取租户列表和对应的逻辑分区列表
                        OpenSeletedDatabase(AEventArgs);
                        break;
                    case "CAPWD":           //打开修改或重置租户系统管理员登录密码
                        OpenChangeOrResetAdminPassword(AEventArgs);
                        break;
                    case "SADST":           //打开重置租户系统管理员状态窗口
                        OpenResetRentAdminStatus(AEventArgs);
                        break;
                    case "TLPS":            //打开逻辑分区设定窗口
                        OpenLogicPartitionSetting(AEventArgs);
                        break;
                    case "RTLPS":           //刷新更新逻辑分区设定
                        IUCServerObjects.RefreshTreeViewItemAfterLogicPartitionSet(AEventArgs);
                        break;
                    case "LSST":            //打开 License Server 配置窗口
                        OpenLicenseServiceSetting(AEventArgs);
                        break;
                    case "RLSST":           //刷新配置后的 License Server
                        IUCServerObjects.RefreshTreeViewItemAfterSettingLicenseService(App.GetLicenseServerInfo(true));
                        break;
                    default:
                        MessageBox.Show(AEventArgs.ObjSource.GetType().ToString(), AEventArgs.StrElementTag);
                        break;
                }
            }
            catch { }
        }
        #endregion
        
        #region 初始化状态栏
        private void InitApplicationStatusBar()
        {
            try
            {
                if (IUCStatusBar == null) { IUCStatusBar = new UCFootStatusBar(); }
                GridStatusBar.Children.Clear();
                IUCStatusBar.StatusBarShowStart(int.MaxValue, "", App.GStrCompanyName + "   " + App.GStrApplicationVersion);
                IUCStatusBar.StatusBarShowStop(int.MaxValue);
                GridStatusBar.Children.Add(IUCStatusBar);
            }
            catch { }
        }
        #endregion

        #region 断开与所有服务器的连接 *** 功能未实现
        private int IIntDisconnectIndex = 0;
        private void DisconnectToAllServers()
        {
            //string LStrTip = string.Empty;

            //try
            //{
            //    MainPanel.IsEnabled = false;
            //    if (IIntDisconnectIndex >= IListListStrConnectedArgs.Count)
            //    {
            //        this.Closing -= SystemMainWindow_Closing;
            IBoolIsFirstCloseApplication = false;
            Environment.Exit(0);
            //    }
            //    LStrTip = (IIntDisconnectIndex + 1).ToString() + "/" + IListListStrConnectedArgs.Count.ToString();
            //    IUCStatusBar01.StatusBarShowLeftStart(1, App.GetDisplayCharacterFromLanguagePackage(this.Name, "N005002") + " " + LStrTip + " " + IListListStrConnectedArgs[IIntDisconnectIndex][0]);
            //    if (IBackgroundWorkerDisconnectAllServer == null) { IBackgroundWorkerDisconnectAllServer = new BackgroundWorker(); }
            //    IBackgroundWorkerDisconnectAllServer.RunWorkerCompleted += IBackgroundWorkerDisconnectAllServer_RunWorkerCompleted;
            //    IBackgroundWorkerDisconnectAllServer.DoWork += IBackgroundWorkerDisconnectAllServer_DoWork;
            //    IBackgroundWorkerDisconnectAllServer.RunWorkerAsync();
            //}
            //catch { MainPanel.IsEnabled = true; }
        }

        //private void IBackgroundWorkerDisconnectAllServer_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        BasicHttpBinding LBasicHttpBinding = null;
        //        EndpointAddress LEndpointAddress = null;
        //        Service01Return LService01Return = null;
        //        UMPWcfService01Client LService01Client = null;

        //        List<string> LListStrTargetAttribute = new List<string>();
        //        List<string> LListStrTargetValue = new List<string>();

        //        try
        //        {
        //            List<string> LListStrConnectInfo = IListListStrConnectedArgs[IIntDisconnectIndex];
        //            LListStrTargetAttribute.Add(LListStrConnectInfo[2]); LListStrTargetValue.Add("");
        //            LListStrTargetAttribute.Add("LoginStatus"); LListStrTargetValue.Add("0");
        //            if (LListStrConnectInfo[5] == "0")
        //            {
        //                LBasicHttpBinding = App.CreateBasicHttpBinding(int.Parse(LListStrConnectInfo[7]));
        //            }
        //            LEndpointAddress = App.CreateEndpointAddress(LListStrConnectInfo[6], LListStrConnectInfo[0], LListStrConnectInfo[1], "UMPWcfService01");
        //            LService01Return = new Service01Return();
        //            LService01Client = new UMPWcfService01Client(LBasicHttpBinding, LEndpointAddress);
        //            LService01Return = LService01Client.ConnectedOperations("W", "Logins", LListStrTargetAttribute, LListStrTargetValue);
        //            LService01Client.Close();
        //        }
        //        catch { }
        //        finally
        //        {
        //            if (LService01Client != null)
        //            {
        //                if (LService01Client.State == CommunicationState.Opened) { LService01Client.Close(); }
        //            }
        //        }
        //    }
        //    catch { }
        //}

        //private void IBackgroundWorkerDisconnectAllServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    try
        //    {
        //        IUCStatusBar01.StatusBarShowLeftStop(int.MaxValue, App.GetDisplayCharacterFromLanguagePackage("ApplicationShared", "E019995"));
        //    }
        //    catch { }
        //    finally
        //    {
        //        IBackgroundWorkerDisconnectAllServer.Dispose();
        //        IBackgroundWorkerDisconnectAllServer = null;
        //        IIntDisconnectIndex += 1;
        //        DisconnectToAllServers();
        //    }
        //}
        #endregion

        #region 在状态栏中显示当前正在处理的操作提示
        private void ShowCurrentStatus(MamtOperationEventArgs AEventArgs)
        {
            int LIntStatusType = 0;
            string LStrStatusDesc = string.Empty;
            string LStrShowWaitWindow = string.Empty;

            try
            {
                LIntStatusType = int.Parse(AEventArgs.ObjSource as string);
                LStrStatusDesc = AEventArgs.AppenObjeSource1 as string;
                LStrShowWaitWindow = AEventArgs.AppenObjeSource2 as string;

                if (LIntStatusType == int.MaxValue)
                {
                    IUCStatusBar.StatusBarShowStop(LIntStatusType);
                    if (IWaitForApplicationDoing != null)
                    {
                        BorderApplicationWorkArea.IsEnabled = true;
                        IWaitForApplicationDoing.IBoolIsDoing = false;
                        IWaitForApplicationDoing.StatusBarShowStop(LIntStatusType);
                        IWaitForApplicationDoing.CloseThisWindow();
                    }
                }
                else
                {
                    IUCStatusBar.StatusBarShowLeftStart(LIntStatusType, LStrStatusDesc);
                    if (LStrShowWaitWindow == "1")
                    {
                        if (IWaitForApplicationDoing != null) { IWaitForApplicationDoing = null; }
                        BorderApplicationWorkArea.IsEnabled = false;
                        IWaitForApplicationDoing = new WaitForApplicationDoing();
                        IWaitForApplicationDoing.Owner = this;
                        IWaitForApplicationDoing.IBoolIsDoing = true;
                        IWaitForApplicationDoing.StatusBarShowLeftStart(LIntStatusType, LStrStatusDesc);
                        IWaitForApplicationDoing.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.ToString());
            }
            
        }
        #endregion

        #region 显示连接成功的服务器信息
        private void ShowConnectedServerInformation(MamtOperationEventArgs AEventArgs)
        {
            int LIntAllDataSetCount = 0;

            try
            {
                List<string> LListStrConnectedServer = AEventArgs.ObjSource as List<string>;
                DataSet LDataSetServerConnected = AEventArgs.AppenObjeSource1 as DataSet;
                IListListStrConnectedArgs.Add(LListStrConnectedServer);
                IListDataSetConnectedServer.Add(LDataSetServerConnected);
                LIntAllDataSetCount = IListDataSetConnectedServer.Count;
                IUCServerObjects.AddApplicationServer2TreeView(LListStrConnectedServer, IListDataSetConnectedServer[LIntAllDataSetCount - 1]);
                ModifyAuthRootValue();
            }
            catch(Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        #endregion

        #region 成功登录本程序后，立即关闭注册表中的“证书更新服务”
        private void ModifyAuthRootValue()
        {
            try
            {
                string path = @"SOFTWARE\Policies\Microsoft\SystemCertificates\AuthRoot";
                RegistryKey rootKey = Registry.LocalMachine;
                RegistryKey authRootKey = rootKey.CreateSubKey(path);
                if (authRootKey == null) 
                { 
                    rootKey.Close(); return; 
                }
                authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);

                if (Environment.Is64BitOperatingSystem)
                {
                    path = @"SOFTWARE\Wow6432Node\Policies\Microsoft\SystemCertificates\AuthRoot";
                    rootKey = Registry.LocalMachine;
                    authRootKey = rootKey.CreateSubKey(path);
                    if (authRootKey == null)
                    {
                        rootKey.Close(); return;
                    }
                    authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);
                }
                rootKey.Close();
            }
            catch (Exception ex)
            {
            }

        }

        #endregion

        #region 对象资源TreeView的Item选择变化时该处理的操作
        private void ShowObjectDetailsOperations(MamtOperationEventArgs AEventArgs)
        {
            string LStrObjectID = string.Empty;
            string LStrObjectSource = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemSelected = AEventArgs.ObjSource as TreeViewItem;
                if (LTreeViewItemSelected == null) { return; }
                LStrObjectSource = AEventArgs.AppenObjeSource3 as string;

                LStrObjectID = GetObjectID000(AEventArgs.ObjSource);
                IUCObjectOperations.AddObjectOperations(true, LStrObjectID, AEventArgs.ObjSource, false);
                IUCObjectDetails.ShowObjectDetailsInTabItemServerObject(AEventArgs, false);
            }
            catch (Exception ex) { App.ShowExceptionMessage(ex.Message); }
        }

        private string GetObjectID000(object AObjectSource)
        {
            string LStrObjectID = "000";
            string LStrObjectName = string.Empty;

            try
            {
                if (AObjectSource == null) { return "000"; }

                Type LTypeObject = AObjectSource.GetType();
                if (LTypeObject == typeof(TreeViewItem))
                {
                    TreeViewItem LTreeViewItem = AObjectSource as TreeViewItem;
                    LStrObjectName = LTreeViewItem.Name;
                    LStrObjectID = LStrObjectName.Substring(3);
                }
            }
            catch { LStrObjectID = "000"; }

            return LStrObjectID;
        }
        #endregion

    }
}
