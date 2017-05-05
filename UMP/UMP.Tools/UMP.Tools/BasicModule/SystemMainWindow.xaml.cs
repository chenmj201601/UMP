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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.Tools.BasicControls;
using UMP.Tools.LanguageMaintenance;
using UMP.Tools.OnlineUserManagement;
using UMP.Tools.PublicClasses;
using UMP.Tools.ThirdPartyApplications;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.BasicModule
{
    public partial class SystemMainWindow : Window, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        #region 所有 BackgroundWorker
        //首次打开应用的BackgroundWorker
        private BackgroundWorker IBackgroundWorkerFirstOpenApplication = null;

        //断开与所有服务器的连接
        private BackgroundWorker IBackgroundWorkerA = null;
        private BackgroundWorker IBackgroundWorkerC = null;

        //向已连接成功的服务器发送心跳消息
        private BackgroundWorker IBackgroundWorkerB = null;

        //启用/禁用语言包
        private BackgroundWorker IBackgroundWorkerD = null;

        //读取语言包数据
        private BackgroundWorker IBackgroundWorkerE = null;

        //加载其他管理单元数据
        private BackgroundWorker IBackgroundWorkerF = null;

        //刷新一个租户的在线用户
        private BackgroundWorker IBackgroundWorkerG = null;
        #endregion

        #region 界面使用的四个基本 UserControl
        private UCFootStatusBar IUCStatusBar = null;
        private UCFeatureObjects IUCServerObjects = null;
        private UCFeatureDetails IUCObjectDetails = null;
        private UCFeatureOperations IUCObjectOperations = null;
        #endregion

        #region 功能、操作分割位置
        private Double IDoubleColumnServerObjectWidth = 0.0;
        private Double IDoubleColumnServerOperationsWidth = 0.0;
        #endregion

        #region 所有连接的服务器信息采用DataSet保存数据
        /// <summary>
        /// 0：数据库连接参数
        /// 1：支持的语言列表
        /// 2：租户列表
        /// 3：在线用户清单
        /// 4：第三方应用
        /// </summary>
        private List<DataSet> IListDataSetConnectedServer = new List<DataSet>();
        
        private List<List<string>> IListListStrConnectedArgs = new List<List<string>>();
        #endregion

        #region 应用程序使用的等待窗口
        private WaitForApplicationDoing IWaitForApplicationDoing = null;
        #endregion

        #region 系统已经读取的语言包
        private List<DataTable> IListDataTableLanguagePackages = new List<DataTable>();
        #endregion

        public SystemMainWindow()
        {
            InitializeComponent();
            this.Loaded += SystemMainWindow_Loaded;
            this.Closing += SystemMainWindow_Closing;
        }

        private void SystemMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.GSystemMainWindow = this;
            App.LoadApplicationResources();
            ImageApplicationLogo.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\UMP.Tools.png"), UriKind.RelativeOrAbsolute));

            this.Icon = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\UMP.Tools.ico"), UriKind.RelativeOrAbsolute));
            this.Title = LabelApplicationTitle.Content.ToString();
            App.DrawWindowsBackGround(this);

            ButtonTaskDoingTip.Click += ButtonInHeaderClicked;
            ButtonApplicationMenu.Click += ButtonInHeaderClicked;
            ButtonMinimized.Click += ButtonInHeaderClicked;
            ButtonMaximized.Click += ButtonInHeaderClicked;
            ButtonCloseApp.Click += ButtonInHeaderClicked;

            this.MouseLeftButtonDown += SystemMainWindow_MouseLeftButtonDown;
            this.KeyDown += SystemMainWindow_KeyDown;
            LabelApplicationTitle.PreviewMouseDoubleClick += LabelApplicationTitle_PreviewMouseDoubleClick;

            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();

            InitObjectBasicDetailOperations();
            InitApplicationStatusBar();

            IUCObjectOperations.AddObjectOperations(true, "000", null, false);

            SendHeartbeat2ApplicationServer();

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
            }
            catch { }
            finally
            {
                IBackgroundWorkerFirstOpenApplication.Dispose();
                IBackgroundWorkerFirstOpenApplication = null;
            }
        }

        private void InitObjectItemDetailTempleteTabItem()
        {
            OperationEventArgs LEventArgs = new OperationEventArgs();
            LEventArgs.StrElementTag = "0000000000";
            IUCObjectDetails.OpenServerObjectSingleInformation(this, LEventArgs);
        }

        #region 打开连接到应用服务器的窗口
        private void OpenConnectToApplicationServer()
        {
            if (IListListStrConnectedArgs.Count > 0) { return; }
            ConnectToAppServer LWindowConnectToServer = new ConnectToAppServer();
            LWindowConnectToServer.Owner = this;
            LWindowConnectToServer.IOperationEvent += IUCObjectBDOOperationEvent;
            LWindowConnectToServer.ShowDialog();
            this.Activate();
        }
        #endregion

        #region 标题栏五个按钮事件
        private void ButtonInHeaderClicked(object sender, RoutedEventArgs e)
        {
            string LStrClickedButton = string.Empty;

            try
            {
                Button LClickedButton = e.Source as Button;
                LStrClickedButton = LClickedButton.Name;
                switch (LStrClickedButton)
                {
                    case "ButtonTaskDoingTip":
                        MessageBox.Show("该功能未实现", App.GStrApplicationReferredTo);
                        break;
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
                        this.Hide(); this.Show();this.Focus();
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

        private void SystemMainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LabelApplicationTitle_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
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
            this.Hide(); this.Show(); this.Focus();
        }
        #endregion

        #region 关闭系统主窗口
        private void CloseMainWindow()
        {
            this.Close();
        }

        private void SystemMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(App.GetDisplayCharater("M01014"), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes) { e.Cancel = true; return; }
            e.Cancel = true;
            DisconnectToAllServers(false);
            System.Threading.Thread.Sleep(500);
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

                ButtonTaskDoingTip.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch { }
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
                    GridFeature.Children.Clear();
                    IUCServerObjects = null;
                }
                if (IUCObjectDetails != null)
                {
                    IUCObjectDetails.IOperationEvent -= IUCObjectBDOOperationEvent;
                    GridDetail.Children.Clear();
                    IUCObjectDetails = null;
                }
                if (IUCObjectOperations != null)
                {
                    IUCObjectOperations.IOperationEvent -= IUCObjectBDOOperationEvent;
                    GridOperations.Children.Clear();
                    IUCObjectOperations = null;
                }

                if (IUCServerObjects == null) { IUCServerObjects = new UCFeatureObjects(); }
                IUCServerObjects.IOperationEvent += IUCObjectBDOOperationEvent;
                GridFeature.Children.Add(IUCServerObjects);

                if (IUCObjectDetails == null) { IUCObjectDetails = new UCFeatureDetails(); }
                IUCObjectDetails.IOperationEvent += IUCObjectBDOOperationEvent;
                GridDetail.Children.Add(IUCObjectDetails);

                if (IUCObjectOperations == null) { IUCObjectOperations = new UCFeatureOperations(); }
                IUCObjectOperations.IOperationEvent += IUCObjectBDOOperationEvent;
                GridOperations.Children.Add(IUCObjectOperations);
            }
            catch { }
        }
        #endregion

        #region 从其他对象发送过来的消息入口
        /// <summary>
        /// 从其他对象发送过来的消息入口
        /// 函数 实现方法
        /// </summary>
        /// <param name="AEventArgs"></param>
        public void ObjectOperationsEvent(OperationEventArgs AEventArgs)
        {
            try
            {
                //if (App.GBoolApplicationIsBusing) { MessageBox.Show(App.GetDisplayCharater("M00001"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning); return; }

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
                    case "TSCH":
                        ShowObjectDetailsOperations(AEventArgs);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { App.ShowExceptionMessage(ex.ToString()); }
        }

        /// <summary>
        /// 从其他对象发送过来的消息入口
        /// 事件 实现方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="AEventArgs"></param>
        private void IUCObjectBDOOperationEvent(object sender, OperationEventArgs AEventArgs)
        {
            try
            {
                switch (AEventArgs.StrElementTag)
                {
                    case "FLTS":            //打开连接服务器窗口
                        OpenConnectToApplicationServer();
                        break;
                    case "SLTS":            //成功连接到UMP服务器
                        ShowConnectedServerInformation(AEventArgs);
                        break;
                    case "FDTS":            //断开与服务器的连接
                        DisconnectToAllServers(true);
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
                    case "ENABL":           //启用语言包
                        EnableOrDisableLanguages("E");
                        break;
                    case "DISEL":           //禁用语言包
                        EnableOrDisableLanguages("D");
                        break;
                    case "READL":           //读取语言包对应的数据
                        ReadLanguagePackageFromDatabase();
                        break;
                    case "SAVEL":           //保存修改后的语言包
                        SaveLanguagePackage2Database();
                        break;
                    case "CSLANG":          //语言包保存数据库成功后，更新已经读取的语言包
                        RefreshReadedLanguagePackage(AEventArgs);
                        break;
                    case "EXPTL":           //导出指定的语言包
                        ExportSpecifiedLanguagepack();
                        break;
                    case "IMPTL":           //导入语言包
                        ImportSpecifiedLanguagepack();
                        break;
                    case "LOMU":            //加载其他管理单元
                        LoadOtherManagementUnit();
                        break;
                    case "FEXIT":           //注销在线用户
                        CancellationOnlineUser();
                        break;
                    case "RONLINE":         //刷新在线用户
                        RefreshRentOnlineUser();
                        break;
                    case "OPENOL":          //打开离线语言包文件
                        OpenOfflineLanguagePackage();
                        break;
                    case "SOLANG":          //显示离线语言包文件的内容
                        ShowOfflineLanguagePackage(AEventArgs);
                        break;
                    case "SAVEOL":          //保存离线语言包数据
                        SaveOfflineLanguagePackage();
                        break;
                    case "SSOXL":           //保存离线语言包数据，重新改TreeViewItem的DataContext
                        ResetOfflineLanguageTreeViewItem(AEventArgs);
                        break;
                    case "SETTP":           //打开设置第三方应用参数窗口
                        ThirdPartyApplicationSetting(AEventArgs);
                        break;
                    case "RSETTP":          //成功设置第三方应用参数，刷新窗口
                        RefreshThirdPartyApplications(AEventArgs);
                        break;
                    default:
                        MessageBox.Show(App.GetDisplayCharater("M00999") + "\n" + AEventArgs.StrElementTag, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                }
            }
            catch { }
        }
        #endregion
        
        #region 断开与所有服务器的连接
        /// <summary>
        /// 断开与所有服务器的连接
        /// </summary>
        /// <param name="ABoolIsManual">
        /// 是否是手动断开
        /// </param>
        private void DisconnectToAllServers(bool ABoolIsManual)
        {
            string LStrIsManual = string.Empty;
            try
            {
                LStrIsManual = "0";
                if (IListListStrConnectedArgs.Count == 0)
                {
                    if (!ABoolIsManual)
                    {
                        Environment.Exit(0); return;
                    }
                }
                if (ABoolIsManual)
                {
                    LStrIsManual = "1";
                    if (MessageBox.Show(string.Format(App.GetDisplayCharater("M00002"), IListListStrConnectedArgs[0][0]), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes) { return; }
                }
                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M00003"), IListListStrConnectedArgs[0][0]));
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync(LStrIsManual);
            }
            catch (Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E000001") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                string LStrIsManual = e.Argument as string;
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(IListListStrConnectedArgs[0][0], IListListStrConnectedArgs[0][1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(IListListStrConnectedArgs[0][10]); LListWcfArgs.Add("D");
                I00003OperationReturn = LService00003Client.OperationMethodA(4, LListWcfArgs);
                e.Result = LStrIsManual;
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E001" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IListListStrConnectedArgs.Clear();
            IListDataSetConnectedServer.Clear();
            if (IBackgroundWorkerA != null)
            {
                IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
            }
            if (e.Result.ToString() != "1") { Environment.Exit(0); }
            else
            {
                IUCServerObjects.RemoveConnectedServerFromTreeView();

                IBackgroundWorkerC = new BackgroundWorker();
                IBackgroundWorkerC.RunWorkerCompleted += IBackgroundWorkerC_RunWorkerCompleted;
                IBackgroundWorkerC.DoWork += IBackgroundWorkerC_DoWork;
                IBackgroundWorkerC.RunWorkerAsync();
            }
        }

        private void IBackgroundWorkerC_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void IBackgroundWorkerC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitObjectBasicDetailOperations();
            IUCObjectOperations.AddObjectOperations(true, "000", null, false);
            MessageBox.Show(App.GetDisplayCharater("M00004"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            InitObjectItemDetailTempleteTabItem();
            IBackgroundWorkerC.Dispose();
            IBackgroundWorkerC = null;
        }
        #endregion

        #region 切换语言
        private void ChangeApplicationElementLanguages(OperationEventArgs AEventArgs)
        {
            string LStrLanguageID = AEventArgs.ObjSource as string;
            App.GStrLoginUserCurrentLanguageID = LStrLanguageID;
            App.InitStartedAppliactionData();
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            if (IOperationEvent != null) { IOperationEvent(this, AEventArgs); }
            IUCServerObjects.DisplayElementCharacters(true);
            IUCObjectDetails.DisplayElementCharacters(true);
            IUCObjectOperations.DisplayElementCharacters(true);
        }
        #endregion

        #region 切换皮护
        private void ChangeApplicationStyle(OperationEventArgs AEventArgs)
        {
            string LStrSeasonCode = AEventArgs.ObjSource as string;
            App.GStrSeasonCode = LStrSeasonCode;
            App.LoadApplicationResources();
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.DrawWindowsBackGround(this);
            if (IOperationEvent != null) { IOperationEvent(this, AEventArgs); }
        }
        #endregion

        #region 在状态栏中显示当前正在处理的操作提示
        private void ShowCurrentStatus(OperationEventArgs AEventArgs)
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
        private void ShowConnectedServerInformation(OperationEventArgs AEventArgs)
        {
            try
            {
                IListDataSetConnectedServer.Clear();
                IListListStrConnectedArgs.Clear();

                List<string> LListStrConnectedServer = AEventArgs.ObjSource as List<string>;
                DataSet LDataSetServerConnected = AEventArgs.AppenObjeSource1 as DataSet;
                List<DataSet> LListDataSetOther = AEventArgs.AppenObjeSource2 as List<DataSet>;
                IListListStrConnectedArgs.Add(LListStrConnectedServer);
                IListDataSetConnectedServer.Add(LDataSetServerConnected);
                if (LListDataSetOther.Count > 0)
                {
                    foreach (DataSet LDataSetSingle in LListDataSetOther) { IListDataSetConnectedServer.Add(LDataSetSingle); }
                }
                IUCServerObjects.AddApplicationServer2TreeView(LListStrConnectedServer, IListDataSetConnectedServer);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("ShowConnectedServerInformation()\n\n" + ex.Message);
            }
        }
        #endregion

        #region 向已连接成功的服务器发送心跳消息
        private void SendHeartbeat2ApplicationServer()
        {
            IBackgroundWorkerB = new BackgroundWorker();
            IBackgroundWorkerB.RunWorkerCompleted += IBackgroundWorkerB_RunWorkerCompleted;
            IBackgroundWorkerB.DoWork += IBackgroundWorkerB_DoWork;
            IBackgroundWorkerB.RunWorkerAsync();
        }

        private void IBackgroundWorkerB_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                if (IListListStrConnectedArgs.Count == 0) { return; }
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(IListListStrConnectedArgs[0][0], IListListStrConnectedArgs[0][1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(IListListStrConnectedArgs[0][10]); LListWcfArgs.Add("H");
                LService00003Client.OperationMethodA(4, LListWcfArgs);
                System.Threading.Thread.Sleep(180000);
            }
            catch { }
        }

        private void IBackgroundWorkerB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IBackgroundWorkerB != null)
            {
                IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
            }
            
            SendHeartbeat2ApplicationServer();
        }
        #endregion

        #region 对象资源TreeView的Item选择变化时该处理的操作
        private void ShowObjectDetailsOperations(OperationEventArgs AEventArgs)
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

        #region 重新显示对象的操作
        private void ResetObjectOperations(OperationEventArgs AEventArgs)
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
            }
            catch (Exception ex) { App.ShowExceptionMessage(ex.Message); }
        }
        #endregion

        #region 启用/禁用语言包
        private void EnableOrDisableLanguages(string AStrMethod)
        {
            string LStrLanguageID = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemCurrentFocused = IUCServerObjects.ITreeViewItemCurrentSelected;
                if (LTreeViewItemCurrentFocused == null) { return; }
                LStrLanguageID = LTreeViewItemCurrentFocused.Tag.ToString();

                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01035"), LTreeViewItemCurrentFocused.Header.ToString()), true);

                if (IBackgroundWorkerD == null) { IBackgroundWorkerD = new BackgroundWorker(); }
                IBackgroundWorkerD.RunWorkerCompleted += IBackgroundWorkerD_RunWorkerCompleted;
                IBackgroundWorkerD.DoWork += IBackgroundWorkerD_DoWork;
                IBackgroundWorkerD.RunWorkerAsync(AStrMethod + App.GStrSpliterChar + LStrLanguageID);

            }
            catch (Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerD != null)
                {
                    IBackgroundWorkerD.Dispose(); IBackgroundWorkerD = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E000002") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerD_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            string LStrMethodAndLanguageID = string.Empty;
            

            try
            {
                LStrMethodAndLanguageID = e.Argument as string;
                string[] LStrArrayMethodAndID = LStrMethodAndLanguageID.Split(App.GStrSpliterChar.ToCharArray());
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(IListListStrConnectedArgs[0][0], IListListStrConnectedArgs[0][1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(LStrArrayMethodAndID[1]);
                List<string> LListStrDBProfile = GetCurrentDatabaseProfile();
                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add(LStrArrayMethodAndID[0]);
                I00003OperationReturn = LService00003Client.OperationMethodA(5, LListWcfArgs);
                e.Result = LStrArrayMethodAndID[0];
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E002" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;
            string LStrTVIName = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LStrTVIName = IUCServerObjects.ITreeViewItemCurrentSelected.Name;
                LStrTVIName = "TVI202" + LStrTVIName.Substring(6, 1);
                if (e.Result.ToString() == "E")
                {
                    LStrTVIName += "1";
                    TreeViewItemProps.SetItemImageName(IUCServerObjects.ITreeViewItemCurrentSelected, App.GStrApplicationDirectory + @"\Images\00000024.ico");
                }
                else
                {
                    LStrTVIName += "0";
                    TreeViewItemProps.SetItemImageName(IUCServerObjects.ITreeViewItemCurrentSelected, App.GStrApplicationDirectory + @"\Images\00000025.ico");
                }
                
                IUCServerObjects.ITreeViewItemCurrentSelected.Name = LStrTVIName;
                
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "TSCH";
                LEventArgs.ObjSource = IUCServerObjects.ITreeViewItemCurrentSelected;
                LEventArgs.AppenObjeSource3 = "TV";
                IUCObjectBDOOperationEvent(this, LEventArgs);
            }
            catch { }
            finally
            {
                if (IBackgroundWorkerD != null)
                {
                    IBackgroundWorkerD.Dispose(); IBackgroundWorkerD = null;
                }
            }
        }
        #endregion

        #region 读取语言包数据
        private void ReadLanguagePackageFromDatabase()
        {
            string LStrLanguageID = string.Empty;
            string LStrTVIName = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemCurrentFocused = IUCServerObjects.ITreeViewItemCurrentSelected;
                if (LTreeViewItemCurrentFocused == null) { return; }
                LStrTVIName = LTreeViewItemCurrentFocused.Name;
                LStrLanguageID = LTreeViewItemCurrentFocused.Tag.ToString();

                if (LStrTVIName.Substring(6, 1) == "1")
                {
                    OperationEventArgs LOperationEventArgs = new OperationEventArgs();
                    LOperationEventArgs.StrElementTag = "SLANG";
                    foreach (DataTable LDataTableSingleLanguage in IListDataTableLanguagePackages)
                    {
                        if (LDataTableSingleLanguage.TableName == "T_" + LStrLanguageID)
                        {
                            LOperationEventArgs.ObjSource = LDataTableSingleLanguage;
                            break;
                        }
                    }
                    IOperationEvent(this, LOperationEventArgs);
                    return;
                }

                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01036"), LTreeViewItemCurrentFocused.Header.ToString()), true);

                if (IBackgroundWorkerE == null) { IBackgroundWorkerE = new BackgroundWorker(); }
                IBackgroundWorkerE.RunWorkerCompleted += IBackgroundWorkerE_RunWorkerCompleted;
                IBackgroundWorkerE.DoWork += IBackgroundWorkerE_DoWork;
                IBackgroundWorkerE.RunWorkerAsync(LStrLanguageID);
            }
            catch (Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerE != null)
                {
                    IBackgroundWorkerE.Dispose(); IBackgroundWorkerE = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E000003") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerE_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            string LStrLanguageID = string.Empty;

            try
            {
                LStrLanguageID = e.Argument as string;
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(IListListStrConnectedArgs[0][0], IListListStrConnectedArgs[0][1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(LStrLanguageID);
                List<string> LListStrDBProfile = GetCurrentDatabaseProfile();
                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add("R");
                I00003OperationReturn = LService00003Client.OperationMethodA(5, LListWcfArgs);
                e.Result = LStrLanguageID;
                if (!I00003OperationReturn.BoolReturn) { return; }
                WriteLanguageData2ReadedList(LStrLanguageID);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E003" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }

        }

        private void IBackgroundWorkerE_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;
            string LStrTVIName = string.Empty;
            string LStrLanguageID = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                LStrLanguageID = e.Result.ToString();
                LStrTVIName = IUCServerObjects.ITreeViewItemCurrentSelected.Name;
                LStrTVIName = "TVI2021" + LStrTVIName.Substring(7, 1);
                IUCServerObjects.ITreeViewItemCurrentSelected.Name = LStrTVIName;
                IUCServerObjects.ITreeViewItemCurrentSelected.DataContext = IListDataTableLanguagePackages[IListDataTableLanguagePackages.Count - 1];

                OperationEventArgs LOperationEventArgs = new OperationEventArgs();
                LOperationEventArgs.StrElementTag = "SLANG";
                LOperationEventArgs.ObjSource = IListDataTableLanguagePackages[IListDataTableLanguagePackages.Count - 1];
                IOperationEvent(this, LOperationEventArgs);

                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "TSCH";
                LEventArgs.ObjSource = IUCServerObjects.ITreeViewItemCurrentSelected;
                LEventArgs.AppenObjeSource3 = "TV";
                ResetObjectOperations(LEventArgs);
            }
            catch { }
            finally
            {
                if (IBackgroundWorkerE != null)
                {
                    IBackgroundWorkerE.Dispose(); IBackgroundWorkerE = null;
                }
            }
        }

        private void WriteLanguageData2ReadedList(string AStrLanguageID)
        {
            DataTable LDataTableLanguage = new DataTable();
            string LStrMessageID = string.Empty;

            try
            {
                LDataTableLanguage.TableName = "T_" + AStrLanguageID;
                LDataTableLanguage.Columns.Add("C001", typeof(int));
                LDataTableLanguage.Columns.Add("C002", typeof(string));
                LDataTableLanguage.Columns.Add("C003", typeof(int));
                LDataTableLanguage.Columns.Add("C004", typeof(int));
                LDataTableLanguage.Columns.Add("C005", typeof(string));
                LDataTableLanguage.Columns.Add("C006", typeof(string));
                LDataTableLanguage.Columns.Add("C007", typeof(string));
                LDataTableLanguage.Columns.Add("C008", typeof(string));
                LDataTableLanguage.Columns.Add("C009", typeof(int));
                LDataTableLanguage.Columns.Add("C010", typeof(int));
                LDataTableLanguage.Columns.Add("C011", typeof(string));
                LDataTableLanguage.Columns.Add("C012", typeof(string));
                LDataTableLanguage.Columns.Add("CIsChanged", typeof(string));
                LDataTableLanguage.Columns.Add("TIsChanged", typeof(string));

                foreach (DataRow LDataRowSingleLanguage in I00003OperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStrMessageID = LDataRowSingleLanguage["C002"].ToString();
                    DataRow LDataRow = LDataTableLanguage.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = int.Parse(LDataRowSingleLanguage["C001"].ToString());
                    LDataRow["C002"] = LDataRowSingleLanguage["C002"].ToString();
                    LDataRow["C003"] = int.Parse(LDataRowSingleLanguage["C003"].ToString());
                    LDataRow["C004"] = int.Parse(LDataRowSingleLanguage["C004"].ToString());
                    LDataRow["C005"] = LDataRowSingleLanguage["C005"].ToString();
                    LDataRow["C006"] = LDataRowSingleLanguage["C006"].ToString();
                    LDataRow["C007"] = LDataRowSingleLanguage["C007"].ToString();
                    LDataRow["C008"] = LDataRowSingleLanguage["C008"].ToString();
                    LDataRow["C009"] = int.Parse(LDataRowSingleLanguage["C009"].ToString());
                    LDataRow["C010"] = int.Parse(LDataRowSingleLanguage["C010"].ToString());
                    LDataRow["C011"] = LDataRowSingleLanguage["C011"].ToString();
                    LDataRow["C012"] = LDataRowSingleLanguage["C012"].ToString();
                    LDataRow["CIsChanged"] = "0";
                    LDataRow["TIsChanged"] = "0";
                    LDataRow.EndEdit();
                    LDataTableLanguage.Rows.Add(LDataRow);
                }
                IListDataTableLanguagePackages.Add(LDataTableLanguage);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E003" + App.GStrSpliterChar + LStrMessageID + "\n" + ex.Message;
            }
        }
        #endregion

        #region 保存语言包数据
        private void SaveLanguagePackage2Database()
        {
            OperationEventArgs LOperationEventArgs = new OperationEventArgs();
            LOperationEventArgs.StrElementTag = "SAVEL";
            IOperationEvent(this, LOperationEventArgs);
        }
        #endregion

        #region 语言包保存数据库成功后,更新已经读取的语言包
        private void RefreshReadedLanguagePackage(OperationEventArgs AEventArgs)
        {
            string LStrLanguageID = string.Empty;
            string LStrMessageID = string.Empty;

            try
            {
                ListViewItemSingle LListViewItemSingleLanguage = AEventArgs.ObjSource as ListViewItemSingle;
                LStrLanguageID = LListViewItemSingleLanguage.LanguageCode.ToString();
                LStrMessageID = LListViewItemSingleLanguage.MessageID;
                foreach (DataTable LDataTableSingleLanguage in IListDataTableLanguagePackages)
                {
                    if (LDataTableSingleLanguage.TableName == "T_" + LStrLanguageID)
                    {
                        foreach (DataRow LDataRowSingle in LDataTableSingleLanguage.Rows)
                        {
                            if (LDataRowSingle["C002"].ToString() == LStrMessageID)
                            {
                                LDataRowSingle["C005"] = LListViewItemSingleLanguage.MessageContentText01;
                                LDataRowSingle["C006"] = LListViewItemSingleLanguage.MessageContentText02;
                                LDataRowSingle["C007"] = LListViewItemSingleLanguage.MessageTipDisplay01;
                                LDataRowSingle["C008"] = LListViewItemSingleLanguage.MessageTipDisplay02;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 导出语言包
        private void ExportSpecifiedLanguagepack()
        {
            string LStrLanguageID = string.Empty;
            string LStrLanguageName = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemExported = IUCServerObjects.ITreeViewItemCurrentSelected;
                LStrLanguageID = LTreeViewItemExported.Tag.ToString();
                LStrLanguageName = LTreeViewItemExported.Header.ToString().Trim();
                LanguageExportMain LLLanguageExportMain = new LanguageExportMain();
                LLLanguageExportMain.Owner = this;
                LLLanguageExportMain.InitArguments(LStrLanguageID, LStrLanguageName);
                LLLanguageExportMain.ShowDialog();
            }
            catch { }
        }
        #endregion

        #region 导入语言包
        private void ImportSpecifiedLanguagepack()
        {
            try
            {
                LanguageImportMain LLanguageImportMain = new LanguageImportMain();
                LLanguageImportMain.Owner = this;
                LLanguageImportMain.ShowDialog();
            }
            catch { }
        }
        #endregion

        #region 打开离线语言包文件窗口
        private void OpenOfflineLanguagePackage()
        {
            try
            {
                OfflineLanguageOpenForEdit LOfflineLanguageOpenForEdit = new OfflineLanguageOpenForEdit();
                LOfflineLanguageOpenForEdit.IOperationEvent += IUCObjectBDOOperationEvent;
                LOfflineLanguageOpenForEdit.Owner = this;
                LOfflineLanguageOpenForEdit.ShowDialog();
            }
            catch { }
        }
        #endregion

        #region 显示离线语言包内容
        private void ShowOfflineLanguagePackage(OperationEventArgs AEventArgs)
        {
            string LStrLanguagePackageBody = string.Empty;

            try
            {
                OperationEventArgs LEventArgsData = new OperationEventArgs();
                LEventArgsData.StrElementTag = AEventArgs.StrElementTag;
                LEventArgsData.AppenObjeSource1 = AEventArgs.AppenObjeSource1;
                LEventArgsData.AppenObjeSource2 = AEventArgs.AppenObjeSource2;
                LEventArgsData.AppenObjeSource3 = AEventArgs.AppenObjeSource3;
                LEventArgsData.AppenObjeSource4 = AEventArgs.AppenObjeSource4;

                LStrLanguagePackageBody = AEventArgs.ObjSource as string;
                Stream LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(LStrLanguagePackageBody));
                DataSet LDataSetXmlData = new DataSet();
                LStreamXmlBody.Seek(0, SeekOrigin.Begin);
                LDataSetXmlData.ReadXmlSchema(LStreamXmlBody);
                LStreamXmlBody.Seek(0, SeekOrigin.Begin);
                LDataSetXmlData.ReadXml(LStreamXmlBody);
                LEventArgsData.ObjSource = LDataSetXmlData.Tables[1];

                IUCServerObjects.ITreeViewItemCurrentSelected.DataContext = LEventArgsData;

                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "TSCH";
                LEventArgs.ObjSource = IUCServerObjects.ITreeViewItemCurrentSelected;
                LEventArgs.AppenObjeSource3 = "TV";
                ObjectOperationsEvent(LEventArgs);
            }
            catch { }
        }
        #endregion

        #region 保存离线语言包数据
        private void SaveOfflineLanguagePackage()
        {
            OperationEventArgs LOperationEventArgs = new OperationEventArgs();
            LOperationEventArgs.StrElementTag = "SAVEOL";
            IOperationEvent(this, LOperationEventArgs);
        }
        #endregion

        #region 保存离线语言包数据成功后，更新TreeViewItem的DataContext
        private void ResetOfflineLanguageTreeViewItem(OperationEventArgs AEventArgs)
        {
            OperationEventArgs LEventArgsData = (OperationEventArgs)IUCServerObjects.ITreeViewItemCurrentSelected.DataContext;
            LEventArgsData.ObjSource = AEventArgs.ObjSource;
            IUCServerObjects.ITreeViewItemCurrentSelected.DataContext = LEventArgsData;
        }
        #endregion

        #region 加载其他管理单元数据
        private void LoadOtherManagementUnit()
        {
            if (IBackgroundWorkerF == null) { IBackgroundWorkerF = new BackgroundWorker(); }
            IBackgroundWorkerF.RunWorkerCompleted += IBackgroundWorkerF_RunWorkerCompleted;
            IBackgroundWorkerF.WorkerReportsProgress = true;
            IBackgroundWorkerF.ProgressChanged += IBackgroundWorkerF_ProgressChanged;
            IBackgroundWorkerF.DoWork += IBackgroundWorkerF_DoWork;
            IBackgroundWorkerF.RunWorkerAsync();
        }

        private void IBackgroundWorkerF_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrCallReturn = string.Empty;
            string LStrRentBegin = string.Empty, LStrRentEnd = string.Empty;
            string LStrRentToken = string.Empty;

            try
            {
                BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;
                I00003OperationReturn.BoolReturn = true;

                #region 获取租户列表
                LBackgroundWorker.ReportProgress(1);
                DataSet LDataSetRentList = OnlineUserOperations.GetRentList(ref LStrCallReturn);
                if (!string.IsNullOrEmpty(LStrCallReturn))
                {
                    I00003OperationReturn.BoolReturn = false;
                    I00003OperationReturn.StringReturn = LStrCallReturn;
                    return;
                }
                IListDataSetConnectedServer.Add(LDataSetRentList);
                #endregion

                #region 获取每个租户的在线用户
                LBackgroundWorker.ReportProgress(2);
                DataSet LDataSetOnlineUser = new DataSet();
                foreach (DataRow LDataRowSingleRent in LDataSetRentList.Tables[0].Rows)
                {
                    LStrRentBegin = LDataRowSingleRent["C011"].ToString();
                    LStrRentEnd = LDataRowSingleRent["C012"].ToString();
                    if (DateTime.Parse(LStrRentBegin) > DateTime.UtcNow || DateTime.Parse(LStrRentEnd) < DateTime.UtcNow) { continue; }
                    LStrRentToken = LDataRowSingleRent["C021"].ToString();
                    DataTable LDataTableOnlineUser = OnlineUserOperations.GetRentOnlineUser(LStrRentToken, ref LStrCallReturn);
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        I00003OperationReturn.BoolReturn = false;
                        I00003OperationReturn.StringReturn = LStrCallReturn;
                        return;
                    }
                    LDataSetOnlineUser.Tables.Add(LDataTableOnlineUser);
                }
                IListDataSetConnectedServer.Add(LDataSetOnlineUser);
                #endregion

                #region 读取第三方应用
                LBackgroundWorker.ReportProgress(3);
                ReadThirdPartyApplications(ref LStrCallReturn);
                if (!string.IsNullOrEmpty(LStrCallReturn))
                {
                    I00003OperationReturn.BoolReturn = false;
                    I00003OperationReturn.StringReturn = LStrCallReturn;
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E007" + App.GStrSpliterChar + ex.Message;
            }

        }

        private void IBackgroundWorkerF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string LStrShowTip = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);

                int LIntCurrentStep = e.ProgressPercentage;

                LStrShowTip = App.GetConvertedData("LoadOMU" + LIntCurrentStep.ToString("00"));

                App.ShowCurrentStatus(1, LStrShowTip, true);
            }
            catch { }
        }

        private void IBackgroundWorkerF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;
            List<DataSet> LListDataSet = new List<DataSet>();

            App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
            IBackgroundWorkerF.Dispose(); IBackgroundWorkerF = null;
            if (!I00003OperationReturn.BoolReturn)
            {
                string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                LStrMessageBody += "\n" + LStrOperationReturn[1];
                MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LListDataSet.Add(IListDataSetConnectedServer[2]);
            LListDataSet.Add(IListDataSetConnectedServer[3]);
            LListDataSet.Add(IListDataSetConnectedServer[4]);
            IUCServerObjects.AppendOtherManagementUnit(LListDataSet);
        }
        #endregion

        #region 注销在线用户
        private void CancellationOnlineUser()
        {
            OperationEventArgs LOperationEventArgs = new OperationEventArgs();
            LOperationEventArgs.StrElementTag = "FEXIT";
            IOperationEvent(this, LOperationEventArgs);
        }
        #endregion

        #region 设置第三方应用
        private void ThirdPartyApplicationSetting(OperationEventArgs AEventArgs)
        {
            string LStrThirdPartyAppName = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemThirdParty = AEventArgs.ObjSource as TreeViewItem;
                DataRow LDataRowThirdPartyInfo = LTreeViewItemThirdParty.DataContext as DataRow;
                LStrThirdPartyAppName = LDataRowThirdPartyInfo["Attribute00"].ToString();

                if (LStrThirdPartyAppName == "ASM")
                {
                    ThirdPartyASMSetting LThirdPartyASMSetting = new ThirdPartyASMSetting();
                    LThirdPartyASMSetting.IOperationEvent += IUCObjectBDOOperationEvent;
                    LThirdPartyASMSetting.Owner = this;
                    LThirdPartyASMSetting.ShowThirdPartyAlreadSetting(LDataRowThirdPartyInfo);
                    LThirdPartyASMSetting.ShowDialog();
                }
            }
            catch { }
            //MessageBox.Show(AEventArgs.ObjSource.GetType().ToString());
        }
        #endregion

        #region 刷新租户在线用户
        private void RefreshRentOnlineUser()
        {
            try
            {
                TreeViewItem LTreeViewItemCurrent = IUCServerObjects.ITreeViewItemCurrentSelected;
                DataTable LDataTableSingleRent = LTreeViewItemCurrent.DataContext as DataTable;
                I00003OperationReturn.BoolReturn = true;
                App.ShowCurrentStatus(1, App.GetConvertedData("LoadOMU02"), true);
                if (IBackgroundWorkerG == null) { IBackgroundWorkerG = new BackgroundWorker(); }
                IBackgroundWorkerG.RunWorkerCompleted += IBackgroundWorkerG_RunWorkerCompleted;
                IBackgroundWorkerG.DoWork += IBackgroundWorkerG_DoWork;
                IBackgroundWorkerG.RunWorkerAsync(LDataTableSingleRent);
            }
            catch (Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerG != null)
                {
                    IBackgroundWorkerG.Dispose(); IBackgroundWorkerG = null;
                }
                App.ShowExceptionMessage("RefreshRentOnlineUser()\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerG_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrCallReturn = string.Empty;
            
            try
            {
                DataTable LDataTableSingleRent = e.Argument as DataTable;
                string LStrRentToken = LDataTableSingleRent.TableName.Substring(6);
                DataTable LDataTableOnlineUser = OnlineUserOperations.GetRentOnlineUser(LStrRentToken, ref LStrCallReturn);
                if (!string.IsNullOrEmpty(LStrCallReturn))
                {
                    I00003OperationReturn.BoolReturn = false;
                    I00003OperationReturn.StringReturn = LStrCallReturn;
                    return;
                }
                for (int LIntLoopOnlineUserTable = 0; LIntLoopOnlineUserTable < IListDataSetConnectedServer[3].Tables.Count; LIntLoopOnlineUserTable++)
                {
                    if (IListDataSetConnectedServer[3].Tables[LIntLoopOnlineUserTable].TableName == LDataTableOnlineUser.TableName)
                    {
                        IListDataSetConnectedServer[3].Tables.RemoveAt(LIntLoopOnlineUserTable);
                        IListDataSetConnectedServer[3].Tables.Add(LDataTableOnlineUser);
                        break;
                    }
                }
                e.Result = LDataTableOnlineUser;
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E007" + App.GStrSpliterChar + ex.Message;
            }
        }

        private void IBackgroundWorkerG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                IUCServerObjects.RefreshOnlineUser(e.Result as DataTable);
            }
            catch { }
            finally
            {
                
                if (IBackgroundWorkerG != null)
                {
                    IBackgroundWorkerG.Dispose(); IBackgroundWorkerG = null;
                }
            }
        }
        #endregion

        #region 读取第三方应用
        private void ReadThirdPartyApplications(ref string AStrReturn)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;
            List<string> LListWcfArgs = new List<string>();

            try
            {
                AStrReturn = string.Empty;

                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add("00000");

                I00003OperationReturn = LService00003Client.OperationMethodA(10, LListWcfArgs);
                if (!I00003OperationReturn.BoolReturn)
                {
                    AStrReturn = I00003OperationReturn.StringReturn;
                }
                else
                {
                    IListDataSetConnectedServer.Add(I00003OperationReturn.DataSetReturn);
                }
            }
            catch (Exception ex)
            {
                AStrReturn = "UMP004E001" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }
        #endregion

        #region 第三方应用参数设置完成
        private void RefreshThirdPartyApplications(OperationEventArgs AEventArgs)
        {
            try
            {
                List<string> LListStrArguments = AEventArgs.ObjSource as List<string>;

                if (LListStrArguments[0] == "ASM")
                {
                    DataRow LDataRowASMInfo = IUCServerObjects.ITreeViewItemCurrentSelected.DataContext as DataRow;
                    LDataRowASMInfo["Attribute01"] = LListStrArguments[1];
                    LDataRowASMInfo["Attribute02"] = LListStrArguments[2];
                    LDataRowASMInfo["Attribute03"] = LListStrArguments[3];
                    LDataRowASMInfo["Attribute11"] = LListStrArguments[4];

                    IUCServerObjects.ITreeViewItemCurrentSelected.DataContext = LDataRowASMInfo;
                }

                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "TSCH";
                LEventArgs.ObjSource = IUCServerObjects.ITreeViewItemCurrentSelected;
                LEventArgs.AppenObjeSource3 = "TV";

                IUCObjectBDOOperationEvent(this, LEventArgs);
            }
            catch { }
        }
        #endregion

        #region 获取当前数据库连接参数，以LIST的方式返回
        public List<string> GetCurrentDatabaseProfile()
        {
            List<string> LListStrDBProfile = new List<string>();

            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["DBType"].ToString());
            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["ServerHost"].ToString());
            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["ServerPort"].ToString());
            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["LoginID"].ToString());
            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["LoginPwd"].ToString());
            LListStrDBProfile.Add(IListDataSetConnectedServer[0].Tables[0].Rows[0]["NameService"].ToString());
            return LListStrDBProfile;
        }
        #endregion

        #region 获取当前连接的应用服务器的IP和Port
        public List<string> GetCurrentAppServerConnection()
        {
            List<string> LListStrAppServer = new List<string>();
            LListStrAppServer.Add(IListListStrConnectedArgs[0][0]);
            LListStrAppServer.Add(IListListStrConnectedArgs[0][1]);
            return LListStrAppServer;
        }
        #endregion
    }
}
