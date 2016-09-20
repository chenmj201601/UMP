using PFShareClassesC;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UMPS0000.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS0000
{
    public partial class Page00000A : Page,INotifyPropertyChanged
    {
        private DragHelper IDragHelperPassword;
        private DragHelper IDragHelperRoles;
        private DragHelper IDraghelperFeatures;
        private DragHelper IDraghelperSetDefaultLanguage;

        private bool IBoolConnectedDB = true;
        private bool IBoolCanLogin = true;

        private string IStrMessageSource = string.Empty;

        #region 最后加载的xbap、11003C002（最后打开的Feature）
        private string IStrLastLoadedXbap = string.Empty;
        private string IStr11003002 = string.Empty;
        #endregion

        #region 登录后将要进行的操作代码
        private string IStrWillDoingMethod = string.Empty;
        #endregion

        #region 调用WCF服务过程中使用的临时变量
        private bool IBoolIsBusy = false;
        private bool IBoolCallReturn = true;
        private string IStrCallReturn = string.Empty;
        #endregion

        #region 用户包含的权限
        private DataTable IDataTableUser11003 = new DataTable();
        private List<string> IListStrFeatureGroup = new List<string>();
        #endregion

        #region 当前用户个人参数
        /// <summary>
        /// 邮件、消息显示语言
        /// </summary>
        private string IStr1100101 = string.Empty;

        /// <summary>
        /// 当前用户所属机构
        /// </summary>
        private string IStr1105006 = string.Empty;

        /// <summary>
        /// 当前用户默认首页参数
        /// </summary>
        private string IStr11011005 = string.Empty;
        #endregion

        #region 第三方应用信息
        /// <summary>
        /// ASM信息
        /// </summary>
        private List<string> IListStrASMInformation = new List<string>();
        #endregion

        public Page00000A()
        {
            InitializeComponent();
            this.Loaded += Page00000A_Loaded;

            IDragHelperPassword = new DragHelper();
            IDragHelperRoles = new DragHelper();
            IDraghelperFeatures = new DragHelper();
            IDraghelperSetDefaultLanguage = new DragHelper();

            ButtonLoginSystem.Click += ElementButtonClicked;
            ButtonLoginOptions.Click += ElementButtonClicked;
            ButtonMainOptions.Click += ElementButtonClicked;
            LabelSettingsShow.PreviewMouseLeftButtonDown += LabelSettingsShow_PreviewMouseLeftButtonDown;

            GridLoginBody.KeyDown += MainGrid_KeyDown;
            GridPasswordInfo.KeyDown += MainGrid_KeyDown;
            GridRolesList.KeyDown += MainGrid_KeyDown;

            ButtonReset.Click += ElementButtonClicked;
            ButtonCloseReset.Click += ElementButtonClicked;
            ButtonCloseResetTop.Click += ElementButtonClicked;

            ButtonSelected.Click += ElementButtonClicked;
            ButtonCloseRoles.Click += ElementButtonClicked;
            ButtonCloseRole.Click += ElementButtonClicked;

            ButtonSetLanguage.Click += ElementButtonClicked;
            ButtonCloseLanguage.Click += ElementButtonClicked;
            ButtonCloseSetLanguage.Click += ElementButtonClicked;

            ButtonMaxMinDragPanelInF.Click += ElementButtonClicked;
            ButtonMaxMinDragPanelInA.Click += ElementButtonClicked;

            ButtonBackHome.Click += ElementButtonClicked;
        }

        private void LabelSettingsShow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ButtonMainOptions.ContextMenu == null) { return; }
            //目标   
            ButtonMainOptions.ContextMenu.PlacementTarget = ButtonMainOptions;
            //位置   
            ButtonMainOptions.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ButtonMainOptions.ContextMenu.IsOpen = true;
            return;
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                IIntIdleSecond = 0;
            }
            catch { }
            try
            {
                if (e.Key != Key.Enter) { return; }
                Grid LGridSender = sender as Grid;
                if (LGridSender.Name != "GridRolesList")
                {
                    var uie = e.OriginalSource as UIElement;
                    if (uie.GetType() == typeof(PasswordBox))
                    {
                        PasswordBox LPasswordBoxCurrent = uie as PasswordBox;
                        if (LPasswordBoxCurrent.Name == "PasswordBoxLoginPassword")
                        {
                            if (!IBoolCanLogin) { return; }
                            if (IBoolConnectedDB)
                            {
                                IStrWillDoingMethod = string.Empty;
                                UserLoginSystemN();
                                return;
                            }
                            else
                            {
                                UserLoginSystemMAMT();
                                return;
                            }
                        }
                    }
                    else
                    {
                        uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                    }
                }
                else
                {
                    SelectedSingleRole();
                }
            }
            catch { }
        }

        private void ElementButtonClicked(object sender, RoutedEventArgs e)
        {
            if (IBoolIsBusy || !IBoolCallReturn) { return; }

            Button LButtonClicked = sender as Button;

            if (LButtonClicked.Name == "ButtonLoginOptions" || LButtonClicked.Name == "ButtonMainOptions")
            {
                if (LButtonClicked.ContextMenu == null) { return; }
                //目标   
                LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
                //位置   
                LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                //显示菜单   
                LButtonClicked.ContextMenu.IsOpen = true;
                return;
            }

            if (LButtonClicked.Name == "ButtonLoginSystem")
            {
                if (!IBoolCanLogin) { return; }
                if (IBoolConnectedDB)
                {
                    IStrWillDoingMethod = string.Empty;
                    UserLoginSystemN();
                    return;
                }
                else
                {
                    UserLoginSystemMAMT();
                    return;
                }
            }

            if ((LButtonClicked.Name == "ButtonCloseRoles" || LButtonClicked.Name == "ButtonCloseRole") && IStrWillDoingMethod == "ACT00")
            {
                MessageBox.Show(App.GetDisplayCharater("S0000053"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if ((LButtonClicked.Name == "ButtonCloseRoles" || LButtonClicked.Name == "ButtonCloseRole") && IStrWillDoingMethod == "ACT04")
            {
                DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
                if (StackPanelContainsFeatureGroup.Children.Count > 0)
                {
                    DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                    DragPanelUserFeatures.BringIntoView();
                }
                return;
            }

            if ((LButtonClicked.Name == "ButtonCloseReset" || LButtonClicked.Name == "ButtonCloseResetTop") && IStrWillDoingMethod == "ACT01")
            {
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
                AfterLoginFollowAction("ACT00", string.Empty); return;
            }
            
            if ((LButtonClicked.Name == "ButtonCloseReset" || LButtonClicked.Name == "ButtonCloseResetTop") && IStrWillDoingMethod == "ACT02")
            {
                MessageBox.Show(LabelShowSetMessage.Content.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if ((LButtonClicked.Name == "ButtonCloseReset" || LButtonClicked.Name == "ButtonCloseResetTop") && IStrWillDoingMethod == "ACT03")
            {
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
                if (StackPanelContainsFeatureGroup.Children.Count > 0)
                {
                    DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                    DragPanelUserFeatures.BringIntoView();
                }
                return;
            }

            if (LButtonClicked.Name == "ButtonReset")
            {
                ChangeUserLoginPassword();
                return;
            }

            if (LButtonClicked.Name == "ButtonSelected")
            {
                SelectedSingleRole();
                return;
            }

            if (LButtonClicked.Name == "ButtonMaxMinDragPanelInF" || LButtonClicked.Name == "ButtonMaxMinDragPanelInA")
            {
                FullOrNamalView(); return;
            }

            if (LButtonClicked.Name == "ButtonBackHome")
            {
                ClosePopupApplication(); return;
            }

            if (LButtonClicked.Name == "ButtonCloseLanguage" || LButtonClicked.Name == "ButtonCloseSetLanguage")
            {
                DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                DragPanelUserFeatures.BringIntoView();
                return;
            }

            if (LButtonClicked.Name == "ButtonSetLanguage")
            {
                SaveUserDisplayLanguage(); return;
            }
        }

        private void Page00000A_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Unified Management Portal";
            this.WindowTitle = "Unified Management Portal";

            IDragHelperRoles.Init(MainGrid, DragPanelUserContainsRole);
            IDragHelperPassword.Init(MainGrid, DragPanelResetUserPassword);
            IDraghelperFeatures.Init(GridUserFeatures, DragPanelUserFeatures);
            IDraghelperSetDefaultLanguage.Init(MainGrid, DragPanelUserDefaultLanguage);

            #region 使系统Logo移动
            try
            {
                Storyboard LStoryboard = this.Resources["AppLogoLeave"] as Storyboard;
                LStoryboard.Begin();
            }
            catch { }
            #endregion

            #region 如果未配置Application Server 信息，不可以登录
            try
            {
                if (string.IsNullOrEmpty(App.GClassSessionInfo.AppServerInfo.Address))
                {
                    TextBoxLoginAccount.IsEnabled = false;
                    PasswordBoxLoginPassword.IsEnabled = false;
                    return;
                }
            }
            catch { }
            #endregion

            LoadApplicationBasicInformation();

            ITimerCheckUserIdle.Elapsed += ITimerCheckUserIdle_Elapsed;
            ITimerCheckAfterLogin.Elapsed += ITimerCheckAfterLogin_Elapsed;
            ITimerCheckCurrentLoginIDStatus.Elapsed += ITimerCheckCurrentLoginIDStatus_Elapsed;
        }

        #region 最大化或恢复功能窗口
        private void FullOrNamalView()
        {
            if (!IBoolIsFullView)
            {
                IBoolIsFullView = true;

                DragPanelUserFeatures.Margin = new Thickness(2, 5, 2, 5);
                DragPanelUserFeatures.AllowDrop = false;
                ButtonMaxMinDragPanelInF.Style = (Style)App.Current.Resources["ButtonMaxDragPanelStyle"];
                ButtonMaxMinDragPanelInA.Style = (Style)App.Current.Resources["ButtonMaxDragPanelStyle"];
            }
            else
            {
                IBoolIsFullView = false;
                DragPanelUserFeatures.Margin = new Thickness(250, 100, 250, 100);
                DragPanelUserFeatures.AllowDrop = true;
                ButtonMaxMinDragPanelInF.Style = (Style)App.Current.Resources["ButtonMinDragPanelStyle"];
                ButtonMaxMinDragPanelInA.Style = (Style)App.Current.Resources["ButtonMinDragPanelStyle"];
            }
        }
        #endregion

        #region 初始化系统数据
        private BackgroundWorker IBWReadInformationFromApplicationServer = null;
        private void LoadApplicationBasicInformation()
        {
            try
            {
                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridWaitProgress.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();
                IBoolIsBusy = true;
                TextBoxLoginAccount.IsEnabled = false;
                PasswordBoxLoginPassword.IsEnabled = false;
                IBWReadInformationFromApplicationServer = new BackgroundWorker();
                IBWReadInformationFromApplicationServer.WorkerReportsProgress = true;
                IBWReadInformationFromApplicationServer.RunWorkerCompleted += IBWReadInformationFromApplicationServer_RunWorkerCompleted;
                IBWReadInformationFromApplicationServer.DoWork += IBWReadInformationFromApplicationServer_DoWork;
                IBWReadInformationFromApplicationServer.ProgressChanged += IBWReadInformationFromApplicationServer_ProgressChanged;
                IBWReadInformationFromApplicationServer.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                IBoolIsBusy = false;
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
                if (IBWReadInformationFromApplicationServer != null)
                {
                    IBWReadInformationFromApplicationServer.Dispose(); IBWReadInformationFromApplicationServer = null;
                }
                TextBoxLoginAccount.IsEnabled = true;
                PasswordBoxLoginPassword.IsEnabled = true;
            }
        }

        private void IBWReadInformationFromApplicationServer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //目前该代码未实现
        }

        private void IBWReadInformationFromApplicationServer_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            string LStrTemp = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            List<string> LListStrDBProfile = new List<string>();
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;

                LStrVerificationCode104 = App.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 创建WCF00000 Client - 01
                LBackgroundWorker.ReportProgress(1);
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                #endregion

                #region 获取应用程序简称-11
                LBackgroundWorker.ReportProgress(11);
                LWCFOperationReturn = LService00000Client.OperationMethodA(1, null);
                if (LWCFOperationReturn.BoolReturn)
                {
                    App.GClassSessionInfo.AppName = LWCFOperationReturn.StringReturn;
                }
                else
                {
                    IBoolCallReturn = false;
                    IStrCallReturn = LWCFOperationReturn.StringReturn;
                    return;
                }
                #endregion

                #region 获取数据库信息 - 21
                LBackgroundWorker.ReportProgress(21);
                LWCFOperationReturn = LService00000Client.OperationMethodA(5, null);
                if (LWCFOperationReturn.BoolReturn)
                {
                    if (LWCFOperationReturn.DataSetReturn.Tables[0].Rows.Count == 0)
                    {
                        IBoolConnectedDB = false;
                        CreateLocakNetPipe();
                        App.LoadMamtApplicationData();
                        return;
                    }
                    App.GClassSessionInfo.DatabaseInfo.TypeID = int.Parse(LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["DBType"].ToString());
                    LListStrDBProfile.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());

                    LStrTemp = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["ServerHost"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    App.GClassSessionInfo.DatabaseInfo.Host = LStrTemp;

                    LStrTemp = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["ServerPort"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    App.GClassSessionInfo.DatabaseInfo.Port = int.Parse(LStrTemp);

                    LStrTemp = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["NameService"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    App.GClassSessionInfo.DatabaseInfo.DBName = LStrTemp;

                    LStrTemp = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["LoginID"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    App.GClassSessionInfo.DatabaseInfo.LoginName = LStrTemp;

                    LStrTemp = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["LoginPwd"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    App.GClassSessionInfo.DatabaseInfo.Password = LStrTemp;

                    LListStrDBProfile.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    App.GClassSessionInfo.DBConnectionString = App.GClassSessionInfo.DatabaseInfo.GetConnectionString();

                    App.GClassSessionInfo.DBType = App.GClassSessionInfo.DatabaseInfo.TypeID;
                }
                else
                {
                    IBoolConnectedDB = false;
                    App.LoadMamtApplicationData();
                    IBoolCallReturn = false;
                    IStrCallReturn = LWCFOperationReturn.StringReturn;
                    return;
                }
                #endregion

                #region 获取支持的语言列表 - 31
                LBackgroundWorker.ReportProgress(31);
                LWCFOperationReturn = LService00000Client.OperationMethodA(7, LListStrDBProfile);
                if (LWCFOperationReturn.BoolReturn)
                {
                    foreach (DataRow LDataRowSingleLanguage in LWCFOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LangTypeInfo LLanguageType = new LangTypeInfo();

                        LStrTemp = LDataRowSingleLanguage[1].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        LLanguageType.LangID = int.Parse(LDataRowSingleLanguage[0].ToString());
                        LLanguageType.Display = LStrTemp;
                        App.GClassSessionInfo.SupportLangTypes.Add(LLanguageType);
                    }
                }
                else
                {
                    IBoolConnectedDB = false;
                    CreateLocakNetPipe();
                    App.LoadMamtApplicationData();
                    IBoolCallReturn = false;
                    IStrCallReturn = LWCFOperationReturn.StringReturn;
                    return;
                }

                for (int LIntLoopLangID = 0; LIntLoopLangID < App.GClassSessionInfo.SupportLangTypes.Count; LIntLoopLangID++)
                {
                    if (App.GClassSessionInfo.SupportLangTypes[LIntLoopLangID].LangID == App.GClassSessionInfo.LangTypeInfo.LangID)
                    {
                        App.GClassSessionInfo.LangTypeInfo.Display = App.GClassSessionInfo.SupportLangTypes[LIntLoopLangID].Display;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(App.GClassSessionInfo.LangTypeInfo.Display))
                {
                    for (int LIntLoopLangID = 0; LIntLoopLangID < App.GClassSessionInfo.SupportLangTypes.Count; LIntLoopLangID++)
                    {
                        if (App.GClassSessionInfo.SupportLangTypes[LIntLoopLangID].LangID == 2052)
                        {
                            App.GClassSessionInfo.LangTypeInfo.LangID = App.GClassSessionInfo.SupportLangTypes[LIntLoopLangID].LangID;
                            App.GClassSessionInfo.LangTypeInfo.Display = App.GClassSessionInfo.SupportLangTypes[LIntLoopLangID].Display;
                            break;
                        }
                    }
                }
                #endregion

                #region 加载语言包 - 41
                LBackgroundWorker.ReportProgress(41);
                App.LoadApplicationLanguages();
                #endregion

                #region 加载LDAP登录信息 - 51
                if (App.GStrLoginWorkgroup.ToUpper() != Environment.MachineName.ToUpper())
                {
                    LBackgroundWorker.ReportProgress(51);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(App.GStrLoginWorkgroup);
                    LWCFOperationReturn = LService00000Client.OperationMethodA(10, LListStrWcfArgs);
                    if (!LWCFOperationReturn.BoolReturn)
                    {
                        App.GBoolCanAutoLogin = false;
                        MessageBox.Show(LWCFOperationReturn.StringReturn);
                    }
                    else
                    {
                        if (LWCFOperationReturn.StringReturn != "1") { App.GBoolCanAutoLogin = false; }
                        else
                        {
                            App.GBoolCanAutoLogin = true;
                            App.GStrAutoLoginRentCode5 = LWCFOperationReturn.ListStringReturn[0];
                        }
                    }
                    //MessageBox.Show("51-B-" + App.GBoolCanAutoLogin.ToString());
                }
                #endregion

                #region 创建本机应用程序间通讯 NetPipe
                CreateLocakNetPipe();
                #endregion
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        private void IBWReadInformationFromApplicationServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolIsBusy = false;
                TextBoxLoginAccount.IsEnabled = true;
                PasswordBoxLoginPassword.IsEnabled = true;
                GridWaitProgress.Children.Clear();
                if (!IBoolCallReturn && IBoolConnectedDB)
                {
                    MessageBox.Show(IStrCallReturn);
                    TextBoxLoginAccount.IsEnabled = false;
                    PasswordBoxLoginPassword.IsEnabled = false;
                    return;
                }
                if (!IBoolConnectedDB)
                {
                    IBoolCallReturn = true;
                    InitMAMTOptionsContextMenu();
                    GridWaitProgress.Children.Clear();
                    UCStatusTipTool LUCStatusTipTool = new UCStatusTipTool(Brushes.YellowGreen);
                    LUCStatusTipTool.ShowStatusTipTool(App.GetDisplayCharaterMAMT("M01002"));
                    GridWaitProgress.Children.Add(LUCStatusTipTool);
                }
                else
                {
                    InitLoginOptionsContextMenu();
                    InitMainPageOptionsContextMenu();
                    ShowFrameElementLanguage();
                }
                TextBoxLoginAccount.Focus();

                if (App.GBoolCanAutoLogin && IBoolConnectedDB)
                {
                    TextBoxLoginAccount.Text = App.GStrLoginComputer;
                    PasswordBoxLoginPassword.Password = App.AscCodeToChr(30) + App.AscCodeToChr(30) + App.GStrLoginWorkgroup + App.AscCodeToChr(30) + App.GStrAutoLoginRentCode5;
                    
                    IStrWillDoingMethod = string.Empty;
                    UserLoginSystemN();
                }
            }
            catch { }
            finally
            {
                if (IBWReadInformationFromApplicationServer != null)
                {
                    IBWReadInformationFromApplicationServer.Dispose(); IBWReadInformationFromApplicationServer = null;
                }
            }
        }

        private void CreateLocakNetPipe()
        {
            try
            {
                IBoolCanLogin = true;
                App.GNetPipeHelper = new NetPipeHelper(true, string.Empty);
                App.GNetPipeHelper.DealMessageFunc += DealOtherApplicationMessage;
                App.GNetPipeHelper.Start();
            }
            catch
            {
                IBoolCanLogin = false;
                if (IBoolConnectedDB)
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000071"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    MessageBox.Show(App.GetDisplayCharaterMAMT("M00001"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }
        #endregion

        #region 初始化登录选项
        private void InitLoginOptionsContextMenu()
        {
            string LStrThemesPath = string.Empty;
            string LStrStyleName = string.Empty;

            try
            {
                ButtonLoginOptions.ContextMenu = null;

                ContextMenu LContextMenuLoginOptions = new ContextMenu();

                LContextMenuLoginOptions.Opacity = 0.8;
                
                #region 强制登录
                MenuItem LMenuItemForcedLogin = new MenuItem();
                LMenuItemForcedLogin.Header = App.GetDisplayCharater("S0000003");
                LMenuItemForcedLogin.DataContext = "F-001";
                LMenuItemForcedLogin.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemForcedLogin.Click += LMenuItemOptionsClick;
                LContextMenuLoginOptions.Items.Add(LMenuItemForcedLogin);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 系统支持的语言列表
                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name);
                MenuItem LMenuItemSurportLanguage = new MenuItem();
                LMenuItemSurportLanguage.Header = App.GetDisplayCharater("S0000001");
                LMenuItemSurportLanguage.DataContext = "M100";
                Image LImageIcon1 = new Image();
                LImageIcon1.Height = 16; LImageIcon1.Width = 16;
                LImageIcon1.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000004.ico"), UriKind.Absolute));
                LMenuItemSurportLanguage.Icon = LImageIcon1;
                LMenuItemSurportLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                for (int LIntLoopLanguge = 0; LIntLoopLanguge < App.GClassSessionInfo.SupportLangTypes.Count; LIntLoopLanguge++)
                {
                    MenuItem LMenuItemSingle = new MenuItem();
                    //LMenuItemSingle.Header = "(" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString() + ")" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    LMenuItemSingle.Header = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    LMenuItemSingle.DataContext = "L-" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString();
                    LMenuItemSingle.Tag = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    if (App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID == App.GClassSessionInfo.LangTypeInfo.LangID) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemOptionsClick;
                    LMenuItemSurportLanguage.Items.Add(LMenuItemSingle);
                }

                LContextMenuLoginOptions.Items.Add(LMenuItemSurportLanguage);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 系统可以选择的样式
                App.GClassSessionInfo.SupportThemes.Clear();
                MenuItem LMenuItemSurportStyles = new MenuItem();
                LMenuItemSurportStyles.Header = App.GetDisplayCharater("S0000002");
                LMenuItemSurportStyles.DataContext = "M200";
                Image LImageIcon2 = new Image();
                LImageIcon2.Height = 16; LImageIcon2.Width = 16;
                LImageIcon2.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000008.png"), UriKind.Absolute));
                LMenuItemSurportStyles.Icon = LImageIcon2;
                LMenuItemSurportStyles.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes");

                DirectoryInfo LDirInfo = new DirectoryInfo(LStrThemesPath);
                DirectoryInfo[] LDirectoryInfoSubDirectories = LDirInfo.GetDirectories();
                foreach (DirectoryInfo LDirectoryInfoSingle in LDirectoryInfoSubDirectories)
                {
                    LStrStyleName = LDirectoryInfoSingle.Name;
                    if (!LStrStyleName.Contains("Style")) { continue; }

                    MenuItem LMenuItemSingle = new MenuItem();
                    LMenuItemSingle.Header = App.GetDisplayCharater("S0000" + LStrStyleName);
                    LMenuItemSingle.DataContext = "S-" + LStrStyleName;
                    if (App.GClassSessionInfo.ThemeInfo.Name == LStrStyleName) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemOptionsClick;
                    LMenuItemSurportStyles.Items.Add(LMenuItemSingle);

                    ThemeInfo LThemeInfoSigle = new ThemeInfo();
                    LThemeInfoSigle.Name = LStrStyleName;
                    App.GClassSessionInfo.SupportThemes.Add(LThemeInfoSigle);
                }
                LContextMenuLoginOptions.Items.Add(LMenuItemSurportStyles);
                #endregion

                ButtonLoginOptions.ContextMenu = LContextMenuLoginOptions;

                ButtonLoginSystem.ToolTip = App.GetDisplayCharater("S0000069");
                ButtonLoginOptions.ToolTip = App.GetDisplayCharater("S0000070");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "456"); }
        }

        private void LMenuItemOptionsClick(object sender, RoutedEventArgs e)
        {
            string LStrClickedData = string.Empty;
            string LStrLanguageID = string.Empty;
            string LStrLanguageDisplay = string.Empty;
            string LStrStyleID = string.Empty;
            string LStrOtherData = string.Empty;

            try
            {
                if (IBoolIsBusy) { return; }
                MenuItem LMenuItemClicked = sender as MenuItem;
                LStrClickedData = LMenuItemClicked.DataContext.ToString();
                if (LStrClickedData.Substring(0, 2) == "S-")
                {
                    LStrStyleID = LStrClickedData.Substring(2);
                    if (LStrStyleID == App.GClassSessionInfo.ThemeInfo.Name) { return; }
                    App.GClassSessionInfo.ThemeInfo.Name = LStrStyleID;
                    App.LoadApplicationResources();
                    InitLoginOptionsContextMenu();
                    InitMainPageOptionsContextMenu();
                    if (IListStrFeatureGroup.Count > 0) { ShowUserWorkArea(false); }
                    if (!string.IsNullOrEmpty(IStrLastLoadedXbap))
                    {
                        WebRequest LWebRequestSend = new WebRequest();
                        LWebRequestSend.Code = (int)RequestCode.SCThemeChange;
                        LWebRequestSend.Data = LStrStyleID;
                        WebReturn LWebReturnSend = SendMessageToClient(LWebRequestSend);
                    }
                    return;
                }
                if (LStrClickedData.Substring(0, 2) == "L-")
                {
                    LStrLanguageID = LStrClickedData.Substring(2);
                    LStrLanguageDisplay = LMenuItemClicked.Tag.ToString();
                    ChangeDisplayLanguage(int.Parse(LStrLanguageID), LStrLanguageDisplay);
                    return;
                }
                if (LStrClickedData.Substring(0, 2) == "F-")
                {
                    if (LMenuItemClicked.IsChecked == true)
                    {
                        LMenuItemClicked.IsChecked = false;
                        IStrLoginType = "N";
                    }
                    else
                    {
                        LMenuItemClicked.IsChecked = true;
                        IStrLoginType = "F";
                    }
                    return;
                }
                if (LStrClickedData.Substring(0, 2) == "P-")
                {
                    AfterLoginFollowAction("ACT03", string.Empty);
                    return;
                }

                if (LStrClickedData.Substring(0, 2) == "R-")
                {
                    AfterLoginFollowAction("ACT04", string.Empty);
                    return;
                }

                if (LStrClickedData.Substring(0, 2) == "E-")
                {
                    AfterLoginFollowAction("ACT05", string.Empty);
                    return;
                }

                if (LStrClickedData.Substring(0, 2) == "O-")
                {
                    LStrOtherData = LStrClickedData.Substring(2);
                    SetMessageDisplayLanguage();
                    return;
                }
            }
            catch { }
        }
        #endregion

        #region 初始化主界面选项（下拉菜单）
        private void InitMainPageOptionsContextMenu()
        {
            string LStrThemesPath = string.Empty;
            string LStrStyleName = string.Empty;

            try
            {
                ButtonMainOptions.ContextMenu = null;

                ContextMenu LContextMenuLoginOptions = new ContextMenu();

                LContextMenuLoginOptions.Opacity = 0.8;

                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name);

                #region 修改密码
                MenuItem LMenuItemChangePassword = new MenuItem();
                LMenuItemChangePassword.Header = App.GetDisplayCharater("S0000049");
                LMenuItemChangePassword.DataContext = "P-001";
                LMenuItemChangePassword.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemChangePassword.Click += LMenuItemOptionsClick;
                LContextMenuLoginOptions.Items.Add(LMenuItemChangePassword);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 切换角色
                MenuItem LMenuItemChangeRole = new MenuItem();
                LMenuItemChangeRole.Header = App.GetDisplayCharater("S0000050");
                LMenuItemChangeRole.DataContext = "R-001";
                Image LImageIcon0 = new Image();
                LImageIcon0.Height = 16; LImageIcon0.Width = 16;
                LImageIcon0.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000006.png"), UriKind.Absolute));
                LMenuItemChangeRole.Icon = LImageIcon0;
                LMenuItemChangeRole.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemChangeRole.Click += LMenuItemOptionsClick;
                LContextMenuLoginOptions.Items.Add(LMenuItemChangeRole);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 系统支持的语言列表
                
                MenuItem LMenuItemSurportLanguage = new MenuItem();
                LMenuItemSurportLanguage.Header = App.GetDisplayCharater("S0000001");
                LMenuItemSurportLanguage.DataContext = "M100";
                Image LImageIcon1 = new Image();
                LImageIcon1.Height = 16; LImageIcon1.Width = 16;
                LImageIcon1.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000004.ico"), UriKind.Absolute));
                LMenuItemSurportLanguage.Icon = LImageIcon1;
                LMenuItemSurportLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                for (int LIntLoopLanguge = 0; LIntLoopLanguge < App.GClassSessionInfo.SupportLangTypes.Count; LIntLoopLanguge++)
                {
                    MenuItem LMenuItemSingle = new MenuItem();
                    //LMenuItemSingle.Header = "(" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString() + ")" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    LMenuItemSingle.Header = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    LMenuItemSingle.DataContext = "L-" + App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString();
                    LMenuItemSingle.Tag = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    if (App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID == App.GClassSessionInfo.LangTypeInfo.LangID) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemOptionsClick;
                    LMenuItemSurportLanguage.Items.Add(LMenuItemSingle);
                }
                LContextMenuLoginOptions.Items.Add(LMenuItemSurportLanguage);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 系统可以选择的样式
                MenuItem LMenuItemSurportStyles = new MenuItem();
                LMenuItemSurportStyles.Header = App.GetDisplayCharater("S0000002");
                LMenuItemSurportStyles.DataContext = "M200";
                Image LImageIcon2 = new Image();
                LImageIcon2.Height = 16; LImageIcon2.Width = 16;
                LImageIcon2.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000008.png"), UriKind.Absolute));
                LMenuItemSurportStyles.Icon = LImageIcon2;
                LMenuItemSurportStyles.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes");

                DirectoryInfo LDirInfo = new DirectoryInfo(LStrThemesPath);
                DirectoryInfo[] LDirectoryInfoSubDirectories = LDirInfo.GetDirectories();
                foreach (DirectoryInfo LDirectoryInfoSingle in LDirectoryInfoSubDirectories)
                {
                    LStrStyleName = LDirectoryInfoSingle.Name;
                    if (!LStrStyleName.Contains("Style")) { continue; }

                    MenuItem LMenuItemSingle = new MenuItem();
                    LMenuItemSingle.Header = App.GetDisplayCharater("S0000" + LStrStyleName);
                    LMenuItemSingle.DataContext = "S-" + LStrStyleName;
                    if (App.GClassSessionInfo.ThemeInfo.Name == LStrStyleName) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemOptionsClick;
                    LMenuItemSurportStyles.Items.Add(LMenuItemSingle);
                }
                LContextMenuLoginOptions.Items.Add(LMenuItemSurportStyles);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 其他
                MenuItem LMenuItemOther = new MenuItem();
                LMenuItemOther.Header = App.GetDisplayCharater("S0000083");
                LMenuItemOther.DataContext = "M900";
                LMenuItemOther.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                MenuItem LMenuItemUserDefaultLanguage = new MenuItem();
                LMenuItemUserDefaultLanguage.Header = App.GetDisplayCharater("S0000084");
                LMenuItemUserDefaultLanguage.DataContext = "O-001";
                LMenuItemUserDefaultLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemUserDefaultLanguage.Click += LMenuItemOptionsClick;
                LMenuItemOther.Items.Add(LMenuItemUserDefaultLanguage);
                LContextMenuLoginOptions.Items.Add(LMenuItemOther);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 切换用户
                MenuItem LMenuItemChangeUser = new MenuItem();
                LMenuItemChangeUser.Header = App.GetDisplayCharater("S0000052");
                LMenuItemChangeUser.DataContext = "E-001";
                LMenuItemChangeUser.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemChangeUser.Click += LMenuItemOptionsClick;
                LContextMenuLoginOptions.Items.Add(LMenuItemChangeUser);
                #endregion

                ButtonMainOptions.ContextMenu = LContextMenuLoginOptions;
                LContextMenuLoginOptions.Opened += LContextMenuLoginOptions_Opened;
                LContextMenuLoginOptions.Closed += LContextMenuLoginOptions_Closed;
                DisableSwitchRoleMenu();
            }
            catch { }
        }

        private void LContextMenuLoginOptions_Closed(object sender, RoutedEventArgs e)
        {
            GridUserFeatures.Visibility = System.Windows.Visibility.Visible;
        }

        private void LContextMenuLoginOptions_Opened(object sender, RoutedEventArgs e)
        {
            GridUserFeatures.Visibility = System.Windows.Visibility.Collapsed;

        }
        #endregion

        #region 初始化MAMT登录选项
        private void InitMAMTOptionsContextMenu()
        {
            string LStrThemesPath = string.Empty;
            string LStrStyleName = string.Empty;
            string LStrStyleViewName = string.Empty;
            string LStrLangID = string.Empty;

            try
            {
                ButtonLoginOptions.ContextMenu = null;

                ContextMenu LContextMenuLoginOptions = new ContextMenu();

                LContextMenuLoginOptions.Opacity = 0.8;

                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name);

                #region 系统支持的语言列表
                MenuItem LMenuItemSurportLanguage = new MenuItem();
                LMenuItemSurportLanguage.Header = App.GetDisplayCharaterMAMT("M01000");
                LMenuItemSurportLanguage.DataContext = "M100";
                Image LImageIcon1 = new Image();
                LImageIcon1.Height = 16; LImageIcon1.Width = 16;
                LImageIcon1.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000004.ico"), UriKind.Absolute));
                LMenuItemSurportLanguage.Icon = LImageIcon1;
                LMenuItemSurportLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                foreach (DataRow LDataRowSingleLanguage in App.IDataTableMAMTSupportL.Rows)
                {
                    MenuItem LMenuItemSingle = new MenuItem();
                    LStrLangID = LDataRowSingleLanguage["C004"].ToString();
                    LMenuItemSingle.Header = "(" + LStrLangID + ")" + LDataRowSingleLanguage["C002"].ToString();
                    LMenuItemSingle.DataContext = "L-" + LStrLangID;
                    LMenuItemSingle.Tag = LDataRowSingleLanguage["C002"].ToString();
                    if (LStrLangID == App.GClassSessionInfo.LangTypeInfo.LangID.ToString()) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemMAMTOptionsClick;
                    LMenuItemSurportLanguage.Items.Add(LMenuItemSingle);
                }

                LContextMenuLoginOptions.Items.Add(LMenuItemSurportLanguage);
                #endregion

                LContextMenuLoginOptions.Items.Add(new Separator());

                #region 系统可以选择的样式
                App.GClassSessionInfo.SupportThemes.Clear();
                MenuItem LMenuItemSurportStyles = new MenuItem();
                LMenuItemSurportStyles.Header = App.GetDisplayCharaterMAMT("M01001");
                LMenuItemSurportStyles.DataContext = "M200";
                Image LImageIcon2 = new Image();
                LImageIcon2.Height = 16; LImageIcon2.Width = 16;
                LImageIcon2.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrThemesPath, @"Images\00000008.png"), UriKind.Absolute));
                LMenuItemSurportStyles.Icon = LImageIcon2;
                LMenuItemSurportStyles.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                LStrThemesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes");

                DirectoryInfo LDirInfo = new DirectoryInfo(LStrThemesPath);
                DirectoryInfo[] LDirectoryInfoSubDirectories = LDirInfo.GetDirectories();
                foreach (DirectoryInfo LDirectoryInfoSingle in LDirectoryInfoSubDirectories)
                {
                    LStrStyleName = LDirectoryInfoSingle.Name;
                    if (!LStrStyleName.Contains("Style")) { continue; }

                    MenuItem LMenuItemSingle = new MenuItem();
                    LMenuItemSingle.Header = App.GetStyleShowName(LStrStyleName);
                    LMenuItemSingle.DataContext = "S-" + LStrStyleName;
                    if (App.GClassSessionInfo.ThemeInfo.Name == LStrStyleName) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemMAMTOptionsClick;
                    LMenuItemSurportStyles.Items.Add(LMenuItemSingle);

                    ThemeInfo LThemeInfoSigle = new ThemeInfo();
                    LThemeInfoSigle.Name = LStrStyleName;
                    App.GClassSessionInfo.SupportThemes.Add(LThemeInfoSigle);
                }
                LContextMenuLoginOptions.Items.Add(LMenuItemSurportStyles);
                #endregion

                ButtonLoginOptions.ContextMenu = LContextMenuLoginOptions;

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void LMenuItemMAMTOptionsClick(object sender, RoutedEventArgs e)
        {
            string LStrClickedData = string.Empty;
            string LStrLanguageID = string.Empty;
            string LStrStyleID = string.Empty;

            try
            {
                if (IBoolIsBusy) { return; }
                MenuItem LMenuItemClicked = sender as MenuItem;
                LStrClickedData = LMenuItemClicked.DataContext.ToString();
                if (LStrClickedData.Substring(0, 2) == "S-")
                {
                    LStrStyleID = LStrClickedData.Substring(2);
                    if (LStrStyleID == App.GClassSessionInfo.ThemeInfo.Name) { return; }
                    App.GClassSessionInfo.ThemeInfo.Name = LStrStyleID;
                    App.LoadApplicationResources();
                    InitMAMTOptionsContextMenu();
                    return;
                }
                if (LStrClickedData.Substring(0, 2) == "L-")
                {
                    LStrLanguageID = LStrClickedData.Substring(2);
                    ChangeDisplayLanguage(int.Parse(LStrLanguageID));
                    return;
                }
            }
            catch { }
        }
        #endregion

        #region 切换显示语言
        private void ChangeDisplayLanguage(int AIntLanguageID, string AStrLanguageDisplay)
        {
            try
            {
                if (AIntLanguageID == App.GClassSessionInfo.LangTypeInfo.LangID) { return; }
                App.GClassSessionInfo.LangTypeInfo.LangID = AIntLanguageID;
                App.GClassSessionInfo.LangTypeInfo.Display = AStrLanguageDisplay;
                App.LoadApplicationLanguages();
                InitLoginOptionsContextMenu();
                InitMainPageOptionsContextMenu();
                ShowFrameElementLanguage();
                if (IListStrFeatureGroup.Count > 0) { ShowUserWorkArea(true); }
                if (!string.IsNullOrEmpty(IStr11003002)) { LabelApplicationHeader.Content = App.GetDisplayCharater("FO" + IStr11003002); }
                //if (!string.IsNullOrEmpty(IStrLastLoadedXbap) && IStrMessageSource == "S01")
                //{
                if (!string.IsNullOrEmpty(IStrLastLoadedXbap))
                {
                    WebRequest LWebRequestSend = new WebRequest();
                    LWebRequestSend.Code = (int)RequestCode.SCLanguageChange;
                    LWebRequestSend.Data = AIntLanguageID.ToString();
                    WebReturn LWebReturnSend = SendMessageToClient(LWebRequestSend);
                }
                IStrMessageSource = string.Empty;
            }
            catch { }
        }
        #endregion

        #region 切换显示语言MAMT
        private void ChangeDisplayLanguage(int AIntLanguageID)
        {
            try
            {
                if (AIntLanguageID == App.GClassSessionInfo.LangTypeInfo.LangID) { return; }
                App.GClassSessionInfo.LangTypeInfo.LangID = AIntLanguageID;
                App.LoadMamtApplicationData();
                InitMAMTOptionsContextMenu();
            }
            catch { }
        }
        #endregion

        #region 用户登录MAMT
        private BackgroundWorker IBWUserLoginSystemMAMT = null;
        private string IStrLoginAccountMAMT = string.Empty;
        private void UserLoginSystemMAMT()
        {
            string LStrUserAccount = string.Empty;
            string LStrUserPassword = string.Empty;
            string LStrHostName = string.Empty;

            List<string> LListStrLoginArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;

            try
            {
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;

                IStrLoginAccountMAMT = string.Empty;
                LStrUserAccount = TextBoxLoginAccount.Text.Trim();
                LStrUserPassword = PasswordBoxLoginPassword.Password.Trim();

                if (string.IsNullOrEmpty(LStrUserAccount)) { TextBoxLoginAccount.Focus(); return; }
                if (string.IsNullOrEmpty(LStrUserPassword)) { LStrUserPassword = ""; }

                IStrLoginAccount = LStrUserAccount;
                LStrVerificationCode004 = App.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrUserPassword = EncryptionAndDecryption.EncryptDecryptString(LStrUserPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrHostName = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.LocalMachineInfo.StrHostName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LListStrLoginArgs.Add(LStrUserAccount);
                LListStrLoginArgs.Add(LStrUserPassword);
                LListStrLoginArgs.Add(LStrHostName);

                GridWaitProgress.Children.Clear();
                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridWaitProgress.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                IBoolIsBusy = true;
                IBWUserLoginSystemMAMT = new BackgroundWorker();
                IBWUserLoginSystemMAMT.RunWorkerCompleted += IBWUserLoginSystemMAMT_RunWorkerCompleted;
                IBWUserLoginSystemMAMT.DoWork += IBWUserLoginSystemMAMT_DoWork;
                IBWUserLoginSystemMAMT.RunWorkerAsync(LListStrLoginArgs);
            }
            catch
            {
                IBoolIsBusy = false;
                if (IBWUserLoginSystemMAMT != null)
                {
                    IBWUserLoginSystemMAMT.Dispose(); IBWUserLoginSystemMAMT = null;
                }
            }
        }

        private void IBWUserLoginSystemMAMT_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                List<string> LListStrLoginArgs = e.Argument as List<string>;
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(32, LListStrLoginArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        private void IBWUserLoginSystemMAMT_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrVerificationCode104 = string.Empty;

            try
            {
                IBoolIsBusy = false;
                GridWaitProgress.Children.Clear();
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (!IBoolCallReturn)
                {
                    IBoolCallReturn = true;
                    MessageBox.Show(App.GetDisplayCharaterMAMT("M01003"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string[] LStrLoginReturn = IStrCallReturn.Split(App.GStrSpliterChar.ToCharArray());
                for (int LIntLoopLoginReturn = 0; LIntLoopLoginReturn < LStrLoginReturn.Length; LIntLoopLoginReturn++)
                {
                    LStrLoginReturn[LIntLoopLoginReturn] = EncryptionAndDecryption.EncryptDecryptString(LStrLoginReturn[LIntLoopLoginReturn], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }
                switch (LStrLoginReturn[0])
                {
                    case "M01S00":          //登录成功
                        UserLoginMAMTSystem(LStrLoginReturn);
                        break;
                    case "M01E00":          //登录失败，用户不存在或密码错误
                        GridWaitProgress.Children.Clear();
                        UCStatusTipTool LUCStatusTipTool = new UCStatusTipTool(Brushes.LightSteelBlue);
                        LUCStatusTipTool.ShowStatusTipTool(App.GetDisplayCharaterMAMT("M01E00"));
                        GridWaitProgress.Children.Add(LUCStatusTipTool);
                        break;
                    default:
                        break;
                }
            }
            catch { }
            finally
            {
                IBoolIsBusy = false;
                if (IBWUserLoginSystemMAMT != null)
                {
                    IBWUserLoginSystemMAMT.Dispose(); IBWUserLoginSystemMAMT = null;
                }
            }
        }

        private void UserLoginMAMTSystem(string[] AStrLoginReturn)
        {
            string LStrApplicationServerBaseUrl = string.Empty;
            string LStrApplicationUrl = string.Empty;
            string LStrApplicationGuid = string.Empty;

            try
            {
                App.GClassSessionInfo.RentInfo.Token = "00000";
                App.GClassSessionInfo.UserInfo.UserID = long.Parse(AStrLoginReturn[1]);
                App.GClassSessionInfo.SessionID = AStrLoginReturn[2];
                App.GClassSessionInfo.UserInfo.Account = IStrLoginAccount;

                if (App.GClassSessionInfo.AppServerInfo.SupportHttps)
                {
                    LStrApplicationServerBaseUrl = "https://" + App.GClassSessionInfo.AppServerInfo.Address + ":" + App.GClassSessionInfo.AppServerInfo.Port.ToString();
                }
                else
                {
                    LStrApplicationServerBaseUrl = "http://" + App.GClassSessionInfo.AppServerInfo.Address + ":" + App.GClassSessionInfo.AppServerInfo.Port.ToString();
                }
                LStrApplicationUrl = LStrApplicationServerBaseUrl + "/UMPS0001.xbap";

                GridLoginPanel.Visibility = System.Windows.Visibility.Collapsed;
                AfterLoginPanel.Visibility = System.Windows.Visibility.Collapsed;
                GridLoadOtherApplication.Visibility = System.Windows.Visibility.Visible;

                StrFeatureImageSource = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S0000\S0000012.png");

                ImageBackGround.Visibility = System.Windows.Visibility.Visible;
                ImageLoadingFeature.Visibility = System.Windows.Visibility.Visible;
                SprocketControlLoading.Visibility = System.Windows.Visibility.Visible;

                if (!string.IsNullOrEmpty(IStrLastLoadedXbap))
                {
                    //    SendCloseMessage2Application();
                }
                LStrApplicationGuid = Guid.NewGuid().ToString();
                IStrLastLoadedXbap = LStrApplicationUrl;
                //WebBrowserFeature.Visibility = System.Windows.Visibility.Visible;
                WebBrowserFeature.Navigate(LStrApplicationUrl + "?" + LStrApplicationGuid);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region 用户登录
        private string IStrLoginType = "N";
        private BackgroundWorker IBWUserLoginSystem = null;
        private string IStrLoginAccount = string.Empty;
        private string IStrLoginPassword = string.Empty;

        private void UserLoginSystemN()
        {
            string LStrUserAccount = string.Empty;
            string LStrUserPassword = string.Empty;
            string LStrLoginType = string.Empty;
            string LStrHostName = string.Empty;

            List<string> LListStrLoginArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            try
            {
                IStrLoginAccount = string.Empty;
                IStrLoginPassword = string.Empty;
                if (IBoolIsBusy) { return; }
                if (TextBoxLoginAccount.IsEnabled == false) { return; }
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                LStrUserAccount = TextBoxLoginAccount.Text.Trim();
                LStrUserPassword = PasswordBoxLoginPassword.Password.Trim();
                if (LStrUserPassword.Contains(App.AscCodeToChr(30) + App.AscCodeToChr(30))) { App.GBoolIsLDAPLogin = true; } else { App.GBoolIsLDAPLogin = false; }
                if (string.IsNullOrEmpty(LStrUserAccount))
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000004"), App.GClassSessionInfo.AppName);
                    return;
                }
                if (string.IsNullOrEmpty(LStrUserPassword)) { LStrUserPassword = ""; }

                IStrLoginAccount = LStrUserAccount; IStrLoginPassword = LStrUserPassword;

                LStrVerificationCode004 = App.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrUserPassword = EncryptionAndDecryption.EncryptDecryptString(LStrUserPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrLoginType = EncryptionAndDecryption.EncryptDecryptString(IStrLoginType, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrHostName = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.LocalMachineInfo.StrHostName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LListStrLoginArgs.Add(LStrUserAccount);
                LListStrLoginArgs.Add(LStrUserPassword);
                LListStrLoginArgs.Add(LStrLoginType);
                LListStrLoginArgs.Add(LStrHostName);
                
                GridWaitProgress.Children.Clear();
                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridWaitProgress.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                IBoolIsBusy = true;
                IBWUserLoginSystem = new BackgroundWorker();
                IBWUserLoginSystem.RunWorkerCompleted += IBWUserLoginSystem_RunWorkerCompleted;
                IBWUserLoginSystem.DoWork += IBWUserLoginSystem_DoWork;
                IBWUserLoginSystem.RunWorkerAsync(LListStrLoginArgs);
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
                if (IBWUserLoginSystem != null)
                {
                    IBWUserLoginSystem.Dispose(); IBWUserLoginSystem = null;
                }
            }
        }

        private void IBWUserLoginSystem_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                List<string> LListStrLoginArgs = e.Argument as List<string>;
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(11, LListStrLoginArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                if (IBoolCallReturn)
                {
                    string[] LStrLoginReturn = IStrCallReturn.Split(App.GStrSpliterChar.ToCharArray());
                    string LStrVerificationCode104 = string.Empty;
                    List<string> LListStrWcfArgs = new List<string>();

                    LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    for (int LIntLoopLoginReturn = 0; LIntLoopLoginReturn < LStrLoginReturn.Length; LIntLoopLoginReturn++)
                    {
                        LStrLoginReturn[LIntLoopLoginReturn] = EncryptionAndDecryption.EncryptDecryptString(LStrLoginReturn[LIntLoopLoginReturn], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    }
                    if (LStrLoginReturn[0] == "S01A00")
                    {
                        string LStrRentToken = LStrLoginReturn[1];
                        string LStrUserID19 = LStrLoginReturn[2];

                        #region 获取逻辑分区表信息
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(LStrRentToken);

                        LWCFOperationReturn = LService00000Client.OperationMethodA(9, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            App.GClassSessionInfo.ListPartitionTables.Clear();
                            foreach (string LStrSinglePartitionTalble in LWCFOperationReturn.ListStringReturn)
                            {
                                PartitionTableInfo LPartitionSingle = new PartitionTableInfo();
                                string[] LStrPartionInformation = LStrSinglePartitionTalble.Split(App.GStrSpliterChar.ToCharArray());
                                LPartitionSingle.TableName = LStrPartionInformation[0];
                                LPartitionSingle.Other1 = LStrPartionInformation[1];
                                LPartitionSingle.PartType = TablePartType.DatetimeRange;
                                App.GClassSessionInfo.ListPartitionTables.Add(LPartitionSingle);
                            }
                        }
                        #endregion

                        #region 获取登录后强制注销、无操作强制注销 11020201，11020202
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(LStrRentToken);
                        LListStrWcfArgs.Add("11020201"); LListStrWcfArgs.Add("11020202");
                        LWCFOperationReturn = LService00000Client.OperationMethodA(51, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            IIntAllowOnlineSecond = int.Parse(LWCFOperationReturn.ListStringReturn[0]) * 3600;
                            IIntAllowIdleSecond = int.Parse(LWCFOperationReturn.ListStringReturn[1]) * 60;
                        }
                        else
                        {
                            IIntAllowOnlineSecond = 0;
                            IIntAllowIdleSecond = 20 * 60;
                        }
                        #endregion

                        #region 获取用户邮件、消息默认显示语言
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(LStrRentToken);
                        LListStrWcfArgs.Add(LStrUserID19);
                        LListStrWcfArgs.Add("1100101");
                        LListStrWcfArgs.Add("R");
                        LListStrWcfArgs.Add(App.GClassSessionInfo.LangTypeInfo.LangID.ToString());
                        LWCFOperationReturn = LService00000Client.OperationMethodA(53, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            IStr1100101 = LWCFOperationReturn.DataSetReturn.Tables[0].Rows[0]["C005"].ToString();
                        }
                        else
                        {
                            IStr1100101 = App.GClassSessionInfo.LangTypeInfo.LangID.ToString();
                        }
                        #endregion

                        #region 获取用户所属机构
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(LStrRentToken);
                        LListStrWcfArgs.Add(LStrUserID19);
                        LListStrWcfArgs.Add("C006");
                        LWCFOperationReturn = LService00000Client.OperationMethodA(54, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            IStr1105006 = LWCFOperationReturn.StringReturn;
                        }
                        else
                        {
                            IStr1105006 = "0";
                        }
                        #endregion

                        #region 获取ASM第三方信息
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add("ASM");
                        LWCFOperationReturn = LService00000Client.OperationMethodA(61, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            IListStrASMInformation.Clear();
                            foreach (string LStrInfomationSingle in LWCFOperationReturn.ListStringReturn)
                            {
                                IListStrASMInformation.Add(LStrInfomationSingle);
                            }
                        }
                        #endregion

                        #region 获取用户个人首页参数
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(LStrRentToken);
                        LListStrWcfArgs.Add(LStrUserID19);
                        LListStrWcfArgs.Add("1100201");
                        LWCFOperationReturn = LService00000Client.OperationMethodA(55, LListStrWcfArgs);
                        if (LWCFOperationReturn.BoolReturn)
                        {
                            IStr11011005 = LWCFOperationReturn.StringReturn;
                            string[] LStrSpliter = IStr11011005.Split(';');
                            if (LStrSpliter.Length >= 2) { IStr11011005 = LStrSpliter[1]; }
                        }
                        else
                        {
                            IStr11011005 = string.Empty;
                        }
                        #endregion

                        #region LDAP登录，获取用户在UMP中的密码
                        if (App.GBoolIsLDAPLogin)
                        {
                            LListStrWcfArgs.Clear();
                            LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                            LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                            LListStrWcfArgs.Add(LStrRentToken);
                            LListStrWcfArgs.Add(LStrUserID19);
                            LWCFOperationReturn = LService00000Client.OperationMethodA(19, LListStrWcfArgs);
                            if (LWCFOperationReturn.BoolReturn)
                            {
                                IStrLoginAccount = App.GStrLoginWorkgroup + "\\" + App.GStrLoginComputer;
                                IStrLoginPassword = LWCFOperationReturn.StringReturn;
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        private void IBWUserLoginSystem_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrVerificationCode104 = string.Empty;

            try
            {
                IBoolIsBusy = false;
                
                GridWaitProgress.Children.Clear();
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (!IBoolCallReturn)
                {
                    IBoolCallReturn = true;
                    MessageBox.Show(App.GetDisplayCharater("S0000028"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string[] LStrLoginReturn = IStrCallReturn.Split(App.GStrSpliterChar.ToCharArray());
                for (int LIntLoopLoginReturn = 0; LIntLoopLoginReturn < LStrLoginReturn.Length; LIntLoopLoginReturn++)
                {
                    LStrLoginReturn[LIntLoopLoginReturn] = EncryptionAndDecryption.EncryptDecryptString(LStrLoginReturn[LIntLoopLoginReturn], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }

                InitUserDefaultLanguageList();
                
                switch (LStrLoginReturn[0])
                {
                    case "S01A00":
                        MainGrid.MouseMove += MainGrid_MouseMove;
                        IIntAfterLoginCount = 0;
                        ITimerCheckUserIdle.Start();
                        ITimerCheckAfterLogin.Start();
                        ITimerCheckCurrentLoginIDStatus.Start();
                        UserLoginReturnS01A01(LStrLoginReturn);
                        break;
                    case "E01A32":
                        ShowUserLoginedInformation(LStrLoginReturn);
                        break;
                    default:
                        MessageBox.Show(App.GetDisplayCharater("S0000029") + "\n\n" + App.GetDisplayCharater("Page00000A", LStrLoginReturn[0]), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch { }
            finally
            {
                if (IBWUserLoginSystem != null)
                {
                    IBWUserLoginSystem.Dispose(); IBWUserLoginSystem = null;
                }
            }
        }

        /// <summary>
        /// 用户成功登录
        /// </summary>
        /// <param name="AStrLoginReturn">
        /// 1-租户编码（5位）
        /// 2-用户编码（19位）
        /// 3-SessionID
        /// 4-用户全名
        /// 5-（S01A01为新用户，强制修改登录密码 | S01A02密码已经过期，强制修改登录密码 | S01A03密码即将过期，建议修改登录密码
        /// </param>
        private void UserLoginReturnS01A01(string[] AStrLoginReturn)
        {
            string LStrShowMessage = string.Empty;

            try
            {
                App.GClassSessionInfo.RentInfo.Token = AStrLoginReturn[1];
                App.GClassSessionInfo.UserInfo.UserID = long.Parse(AStrLoginReturn[2]);
                App.GClassSessionInfo.SessionID = AStrLoginReturn[3];
                App.GClassSessionInfo.UserInfo.UserName = AStrLoginReturn[4];
                App.GClassSessionInfo.UserInfo.Account = IStrLoginAccount;
                App.GClassSessionInfo.UserInfo.Password = IStrLoginPassword;

                LabelLoginAccountShow.Content = IStrLoginAccount + " (" + AStrLoginReturn[4] + ")";

                if (AStrLoginReturn.Length > 5)
                {
                    if (AStrLoginReturn[5] == "S01A01")         //为新用户，强制修改登录密码
                    {
                        LStrShowMessage = App.GetDisplayCharater("Page00000A", AStrLoginReturn[5]);
                        AfterLoginFollowAction("ACT02", LStrShowMessage);        //强制修改密码
                    }
                    if (AStrLoginReturn[5] == "S01A02")         //密码已经过期，强制修改登录密码
                    {
                        LStrShowMessage = App.GetDisplayCharater("Page00000A", AStrLoginReturn[5]);
                        AfterLoginFollowAction("ACT02", LStrShowMessage);        //强制修改密码
                    }
                    if (AStrLoginReturn[5] == "S01A03")         //密码即将过期
                    {
                        LStrShowMessage = string.Format(App.GetDisplayCharater("Page00000A", AStrLoginReturn[5]), AStrLoginReturn[6]);
                        AfterLoginFollowAction("ACT01", LStrShowMessage);    //修改密码，可以不修改 
                    }
                }
                else
                {
                    AfterLoginFollowAction("ACT00", string.Empty);        //正常登录，没任何提示的后续操作
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowUserLoginedInformation(string[] AStrLoginReturn)
        {
            string LStrShowMessage = string.Empty;

            try
            {
                LStrShowMessage = string.Format(App.GetDisplayCharater("Page00000A", AStrLoginReturn[0]), DateTime.Parse( AStrLoginReturn[1]).ToLocalTime().ToString("G"), AStrLoginReturn[2], AStrLoginReturn[3]);
                MessageBox.Show(LStrShowMessage, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region 登录后续动作
        /// <summary>
        /// 成功登录系统后的后续操作
        /// </summary>
        /// <param name="AStrActionCode">
        /// ACT00：正常步骤
        /// ACT01：修改密码，可以不修改，继续进行正常步骤
        /// ACT02：强制修改密码，密码不修改不可以继续下面的步骤
        /// </param>
        private void AfterLoginFollowAction(string AStrActionCode, string AStrMessage)
        {
            try
            {
                //if (IStrWillDoingMethod == AStrActionCode) { return; }
                if (AStrActionCode == "ACT03" || AStrActionCode == "ACT04" || AStrActionCode == "ACT05")
                {
                    ResetElementShowObject();
                }
                IStrWillDoingMethod = AStrActionCode;
                switch (IStrWillDoingMethod)
                {
                    case "ACT00":
                        LoadUserContainsRoles();
                        break;
                    case "ACT01":
                        FollowAction02(AStrMessage);
                        break;
                    case "ACT02":
                        FollowAction02(AStrMessage);
                        break;
                    case "ACT03":
                        FollowAction03();
                        break;
                    case "ACT04":
                        FollowAction04();
                        break;
                    case "ACT05":
                        FollowAction05(false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetElementShowObject()
        {
            DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
            DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
            DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Collapsed;

            if (StackPanelContainsFeatureGroup.Children.Count > 0)
            {
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                DragPanelUserFeatures.BringIntoView();
            }
        }

        private void FollowAction02(string AStrMessage)
        {
            try
            {
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Visible;

                GridLoginPanel.IsEnabled = false;
                PasswordOldPassword.Password = App.GClassSessionInfo.UserInfo.Password;
                PasswordOldPassword.IsEnabled = false;
                PasswordNewPassword.Password = "";
                PasswordConPassword.Password = "";
                TabItemPasswordInfo.Header = " " + string.Format(App.GetDisplayCharater("DragPanelResetUserPassword", "TabItemPasswordInfo"), App.GClassSessionInfo.UserInfo.Account) + " ";
                if (!string.IsNullOrEmpty(AStrMessage))
                {
                    LabelOldPassword.Visibility = System.Windows.Visibility.Collapsed;
                    PasswordOldPassword.Visibility = System.Windows.Visibility.Collapsed;
                    LabelShowSetMessage.Visibility = System.Windows.Visibility.Visible;
                    LabelShowSetMessage.Content = AStrMessage;
                    LabelShowSetMessage.Foreground = Brushes.LightCoral;
                }

                PasswordNewPassword.Loaded += (ASender, AArgs) => { PasswordNewPassword.Focus(); };

                DragPanelResetUserPassword.BringIntoView();
            }
            catch { }
        }

        /// <summary>
        /// 修改密码，已经登录系统后进行的修改操作
        /// </summary>
        private void FollowAction03()
        {
            try
            {
                DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Collapsed;

                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Visible;
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelResetUserPassword.BringIntoView();
                LabelOldPassword.Visibility = System.Windows.Visibility.Visible;
                PasswordOldPassword.Visibility = System.Windows.Visibility.Visible;
                PasswordOldPassword.IsEnabled = true;
                LabelShowSetMessage.Visibility = System.Windows.Visibility.Collapsed;
                PasswordOldPassword.Password = "";
                PasswordNewPassword.Password = "";
                PasswordConPassword.Password = "";
                TabItemPasswordInfo.Header = " " + string.Format(App.GetDisplayCharater("DragPanelResetUserPassword", "TabItemPasswordInfo"), App.GClassSessionInfo.UserInfo.Account) + " ";
                PasswordOldPassword.Focus();
                PasswordOldPassword.Loaded += (ASender, AArgs) => { PasswordOldPassword.Focus(); };
            }
            catch { }
        }

        /// <summary>
        /// 切换角色
        /// </summary>
        private void FollowAction04()
        {
            int LIntRolesCount = 0;

            try
            {
                GridWaitProgress.Children.Clear();
                LIntRolesCount = App.GClassSessionInfo.RoleInfo.ListIntRoleID.Count;
                if (LIntRolesCount <= 1) { return; }
                
                TabItemRolesList.Header = " " + string.Format(App.GetDisplayCharater("DragPanelUserContainsRole", "TabItemRolesList"), App.GClassSessionInfo.UserInfo.Account) + " ";
                GridLoginPanel.IsEnabled = false;
                DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Visible;
                StackPanelRolesList.Children.Clear();
                for (int LIntLoopRoles = 0; LIntLoopRoles < LIntRolesCount; LIntLoopRoles++)
                {
                    RadioButton LRadioButtonSingleRole = new RadioButton();
                    LRadioButtonSingleRole.Style = (Style)App.Current.Resources["RadioButtonSelectRolesStyle"];
                    if (App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles] < 1061400000000000001)
                    {
                        LRadioButtonSingleRole.Content = App.GetDisplayCharater("DragPanelUserContainsRole", App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles].ToString());
                    }
                    else
                    {
                        LRadioButtonSingleRole.Content = App.GClassSessionInfo.RoleInfo.ListStrRoleName[LIntLoopRoles];
                    }
                    LRadioButtonSingleRole.DataContext = App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles];

                    if (App.GClassSessionInfo.RoleInfo.ID == App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles])
                    {
                        LRadioButtonSingleRole.IsChecked = true;
                        LRadioButtonSingleRole.Loaded += (ASender, AArgs) => { LRadioButtonSingleRole.Focus(); };
                    }
                    
                    StackPanelRolesList.Children.Add(LRadioButtonSingleRole);
                    LRadioButtonSingleRole.GotFocus += LRadioButtonSingleRole_GotFocus;
                }
                DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Visible;
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelUserContainsRole.BringIntoView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 切换用户
        /// </summary>
        private BackgroundWorker IBWUserSignOutSystem = null;
        private void FollowAction05(bool ABoolIsForceLogoff)
        {
            try
            {
                
                if (!ABoolIsForceLogoff)
                {
                    MessageBoxResult LMessageBoxResult = MessageBox.Show(App.GetDisplayCharater("S0000054"), App.GClassSessionInfo.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (LMessageBoxResult != MessageBoxResult.Yes) { IStrWillDoingMethod = string.Empty; return; }
                    ShowTipOnLoginPannel("");
                }

                try
                {
                    MainGrid.MouseMove -= MainGrid_MouseMove;
                }
                catch { }

                StopAllTimer();

                try
                {
                    WebRequest LWebRequestSend = new WebRequest();
                    LWebRequestSend.Code = 91002;
                    SendMessageToClient(LWebRequestSend);

                    LWebRequestSend.Code = (int)RequestCode.SCIdleCheckStop;
                    SendMessageToClient(LWebRequestSend);
                }
                catch { }

                try
                {
                    WebRequest LWebRequestSend = new WebRequest();
                    LWebRequestSend.Code = (int)RequestCode.SCIdleCheckStop;
                    SendMessageToClient(LWebRequestSend);
                }
                catch { }

                IIntAllowOnlineSecond = 0;
                IIntAllowIdleSecond = 0;

                if (StackPanelContainsFeatureGroup.Children.Count > 0)
                {
                    BorderUserFeatures.Visibility = System.Windows.Visibility.Visible;
                }
                BorderYoungApplicationsA.Visibility = System.Windows.Visibility.Collapsed;
                

                App.GNetPipeHelper.ListClients.Clear();
                IStrLastLoadedXbap = string.Empty;

                StackPanelContainsFeatureGroup.Children.Clear();
                //BorderUserFeatures.Visibility = System.Windows.Visibility.Visible;
                //BorderYoungApplicationsA.Visibility = System.Windows.Visibility.Collapsed;

                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;

                IBoolIsBusy = true;

                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridShowCurrentStatus.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                IBWUserSignOutSystem = new BackgroundWorker();
                IBWUserSignOutSystem.RunWorkerCompleted += IBWUserSignOutSystem_RunWorkerCompleted;
                IBWUserSignOutSystem.DoWork += IBWUserSignOutSystem_DoWork;
                IBWUserSignOutSystem.RunWorkerAsync();

            }
            catch(Exception ex)
            {
                if (IBWUserSignOutSystem != null)
                {
                    IBWUserSignOutSystem.Dispose(); IBWUserSignOutSystem = null;
                }
                GridShowCurrentStatus.Children.Clear();
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                IBoolIsBusy = false;
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IBWUserSignOutSystem_DoWork(object sender, DoWorkEventArgs e)
        {
            IBoolCallReturn = App.UserSignOutSystem("", ref IStrCallReturn);
        }

        private void IBWUserSignOutSystem_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                GridShowCurrentStatus.Children.Clear();
                if (!IBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000055"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                App.GClassSessionInfo.UserInfo.UserID = -1;
                App.GClassSessionInfo.UserInfo.Account = string.Empty;

                App.GClassSessionInfo.RoleInfo.ID = -1;
                App.GClassSessionInfo.RoleInfo.Name = string.Empty;

                IListStrFeatureGroup.Clear();

                GridLoginPanel.IsEnabled = true;
                TextBoxLoginAccount.Text = string.Empty;
                PasswordBoxLoginPassword.Password = string.Empty;
                

                AfterLoginPanel.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
                GridLoadOtherApplication.Visibility = System.Windows.Visibility.Collapsed;

                GridLoginPanel.Visibility = System.Windows.Visibility.Visible;

                TextBoxLoginAccount.Focus();
            }
            catch { }
            finally
            {
                IBoolIsBusy = false;
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                if (IBWUserSignOutSystem != null)
                {
                    IBWUserSignOutSystem.Dispose(); IBWUserSignOutSystem = null;
                }
            }
        }
        #endregion

        #region 显示界面语言
        private void ShowFrameElementLanguage()
        {
            try
            {
                //修改密码界面
                LabelTitlePassword.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "LabelTitlePassword");
                LabelOldPassword.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "LabelOldPassword");
                LabelNewPassword.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "LabelNewPassword");
                LabelConPassword.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "LabelConPassword");
                ButtonReset.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "ButtonReset");
                ButtonCloseReset.Content = App.GetDisplayCharater("DragPanelResetUserPassword", "ButtonCloseReset");

                //选择角色
                LabelTitleRoles.Content = App.GetDisplayCharater("DragPanelUserContainsRole", "LabelTitleRoles");
                ButtonSelected.Content = App.GetDisplayCharater("DragPanelUserContainsRole", "ButtonSelected");
                ButtonCloseRole.Content = App.GetDisplayCharater("DragPanelUserContainsRole", "ButtonCloseRole");

                LabelFeatureHeader.Content = App.GetDisplayCharater("S0000051");

                if (App.GClassSessionInfo.RoleInfo.ID >= 0 && App.GClassSessionInfo.RoleInfo.ID < 1061400000000000001)
                {
                    LabelLoginRoleShow.Content = App.GetDisplayCharater("DragPanelUserContainsRole", App.GClassSessionInfo.RoleInfo.ID.ToString());
                    TabItemPasswordInfo.Header = " " + string.Format(App.GetDisplayCharater("DragPanelResetUserPassword", "TabItemPasswordInfo"), App.GClassSessionInfo.UserInfo.Account) + " ";
                }

                LabelSettingsShow.Content = App.GetDisplayCharater("S0000058");

                //设置默认语言界面
                LabelTitleSetLanguage.Content = App.GetDisplayCharater("S0000085");
                TabItemSetDefaultLanguage.Header = " " + App.GetDisplayCharater("S0000086") + " ";
                ButtonSetLanguage.Content = App.GetDisplayCharater("S0000087");
                ButtonCloseLanguage.Content = App.GetDisplayCharater("S0000088");
            }
            catch { }
        }
        #endregion

        #region 修改密码-DB
        private BackgroundWorker IBWChangeUserLoginPassword = null;
        private void ChangeUserLoginPassword()
        {
            string LStrOldPassword = string.Empty;
            string LStrNewPassword = string.Empty;
            string LStrConPassword = string.Empty;
            List<string> LListChangeArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrOldPassword = PasswordOldPassword.Password;
                LStrNewPassword = PasswordNewPassword.Password;
                LStrConPassword = PasswordConPassword.Password;
                if (string.IsNullOrEmpty(LStrNewPassword)) { LStrNewPassword = ""; }
                if (string.IsNullOrEmpty(LStrConPassword)) { LStrConPassword = ""; }
                if (LStrNewPassword != LStrConPassword)
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000037"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //LListChangeArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                //LListChangeArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListChangeArgs.Add(EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.RentInfo.Token, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                LListChangeArgs.Add(EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.UserInfo.UserID.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                LListChangeArgs.Add(EncryptionAndDecryption.EncryptDecryptString(LStrOldPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                LListChangeArgs.Add(EncryptionAndDecryption.EncryptDecryptString(LStrNewPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                LListChangeArgs.Add(EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.SessionID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));

                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;

                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridWaitProgress.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;

                IBWChangeUserLoginPassword = new BackgroundWorker();
                IBWChangeUserLoginPassword.RunWorkerCompleted += IBWChangeUserLoginPassword_RunWorkerCompleted;
                IBWChangeUserLoginPassword.DoWork += IBWChangeUserLoginPassword_DoWork;
                IBWChangeUserLoginPassword.RunWorkerAsync(LListChangeArgs);
            }
            catch(Exception ex)
            {
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Visible;
                GridWaitProgress.Children.Clear();
                if (IBWChangeUserLoginPassword != null)
                {
                    IBWChangeUserLoginPassword.Dispose(); IBWChangeUserLoginPassword = null;
                }
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IBWChangeUserLoginPassword_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            string LStrVerificationCode104 = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListChangeArgs = e.Argument as List<string>;
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(14, LListChangeArgs);

                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                if (IBoolCallReturn)
                {
                    LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrCallReturn = EncryptionAndDecryption.EncryptDecryptString(IStrCallReturn, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    if (LStrCallReturn == "S01A01")
                    {
                        App.GClassSessionInfo.UserInfo.Password = EncryptionAndDecryption.EncryptDecryptString(LListChangeArgs[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        try
                        {
                            WebRequest LWebRequestSend = new WebRequest();
                            LWebRequestSend.Code = (int)RequestCode.SCChangePassword;
                            LWebRequestSend.Data = App.GClassSessionInfo.UserInfo.Password;// EncryptionAndDecryption.EncryptDecryptString(LListChangeArgs[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                            SendMessageToClient(LWebRequestSend);
                            //App.GClassSessionInfo.UserInfo.Password = LWebRequestSend.Data;
                        }
                        catch { }

                    }
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        private void IBWChangeUserLoginPassword_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrVerificationCode104 = string.Empty;

            try
            {
                GridWaitProgress.Children.Clear();
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                if (!IBoolCallReturn)
                {
                    DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Visible;
                    MessageBox.Show(App.GetDisplayCharater("S0000039") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    IStrCallReturn = EncryptionAndDecryption.EncryptDecryptString(IStrCallReturn, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    GridLoginPanel.IsEnabled = true;
                    if (IStrCallReturn == "S01A01")
                    {
                        MessageBox.Show(App.GetDisplayCharater("S0000040"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                        //MessageBox.Show("修改后密码,测试用：\n" + App.GClassSessionInfo.UserInfo.Password, App.GClassSessionInfo.AppName);
                        if (IStrWillDoingMethod == "ACT01" || IStrWillDoingMethod == "ACT02") { AfterLoginFollowAction("ACT00", string.Empty); return; }
                        if (IStrWillDoingMethod == "ACT03")
                        {
                            DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
                            DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                            DragPanelUserFeatures.BringIntoView();
                        }
                    }
                    else
                    {
                        DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Visible;
                        MessageBox.Show(App.GetDisplayCharater("S0000039") + "\n" + App.GetDisplayCharater("DragPanelResetUserPassword", IStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch { }
            finally
            {
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                if (IBWChangeUserLoginPassword != null)
                {
                    IBWChangeUserLoginPassword.Dispose(); IBWChangeUserLoginPassword = null;
                }
            }
        }
        #endregion

        #region 加载用户角色
        private BackgroundWorker IBWLoadUserContainsRoles = null;
        private void LoadUserContainsRoles()
        {
            List<string> LListLoadArgs = new List<string>();

            try
            {
                LListLoadArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListLoadArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListLoadArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                LListLoadArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());

                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;

                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridWaitProgress.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;

                IBWLoadUserContainsRoles = new BackgroundWorker();
                IBWLoadUserContainsRoles.RunWorkerCompleted += IBWLoadUserContainsRoles_RunWorkerCompleted;
                IBWLoadUserContainsRoles.DoWork += IBWLoadUserContainsRoles_DoWork;
                IBWLoadUserContainsRoles.RunWorkerAsync(LListLoadArgs);
            }
            catch (Exception ex)
            {
                GridWaitProgress.Children.Clear();
                if (IBWLoadUserContainsRoles != null)
                {
                    IBWLoadUserContainsRoles.Dispose(); IBWLoadUserContainsRoles = null;
                }
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IBWLoadUserContainsRoles_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                List<string> LListLoadArgs = e.Argument as List<string>;

                App.GClassSessionInfo.RoleInfo.ListIntRoleID.Clear();
                App.GClassSessionInfo.RoleInfo.ListStrRoleName.Clear();

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(15, LListLoadArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    foreach (string LStrSingleRole in LWCFOperationReturn.ListStringReturn)
                    {
                        string[] LStrArrayRole = LStrSingleRole.Split(App.GStrSpliterChar.ToCharArray());
                        if (LStrArrayRole[0] == "106" +App.GClassSessionInfo.RentInfo.Token + "00000000004") { continue; }
                        App.GClassSessionInfo.RoleInfo.ListIntRoleID.Add(long.Parse(LStrArrayRole[0]));
                        App.GClassSessionInfo.RoleInfo.ListStrRoleName.Add(LStrArrayRole[1]);
                    }
                }
                else
                {
                    IBoolCallReturn = false;
                    IStrCallReturn = LWCFOperationReturn.StringReturn;
                    return;
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
        }

        private void IBWLoadUserContainsRoles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int LIntRolesCount = 0;

            try
            {
                GridWaitProgress.Children.Clear();
                if (!IBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000044") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LIntRolesCount = App.GClassSessionInfo.RoleInfo.ListIntRoleID.Count;
                if (LIntRolesCount > 1)
                {
                    TabItemRolesList.Header = " " + string.Format(App.GetDisplayCharater("DragPanelUserContainsRole", "TabItemRolesList"), App.GClassSessionInfo.UserInfo.Account) + " ";
                    GridLoginPanel.IsEnabled = false;
                    DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Visible;
                    StackPanelRolesList.Children.Clear();
                    for(int LIntLoopRoles = 0; LIntLoopRoles < LIntRolesCount; LIntLoopRoles++)
                    {
                        RadioButton LRadioButtonSingleRole = new RadioButton();
                        LRadioButtonSingleRole.Style = (Style)App.Current.Resources["RadioButtonSelectRolesStyle"];
                        if(App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles] < 1061400000000000001)
                        {
                            LRadioButtonSingleRole.Content = App.GetDisplayCharater("DragPanelUserContainsRole", App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles].ToString());
                        }
                        else
                        {
                            LRadioButtonSingleRole.Content = App.GClassSessionInfo.RoleInfo.ListStrRoleName[LIntLoopRoles];
                        }
                        LRadioButtonSingleRole.DataContext = App.GClassSessionInfo.RoleInfo.ListIntRoleID[LIntLoopRoles];

                        if (LIntLoopRoles == 0)
                        {
                            LRadioButtonSingleRole.IsChecked = true;
                            LRadioButtonSingleRole.Loaded += (ASender, AArgs) => { LRadioButtonSingleRole.Focus(); };
                        }
                        LRadioButtonSingleRole.GotFocus += LRadioButtonSingleRole_GotFocus;
                        StackPanelRolesList.Children.Add(LRadioButtonSingleRole);
                    }
                    DragPanelUserContainsRole.BringIntoView();
                }
                else if (LIntRolesCount == 1)
                {
                    App.GClassSessionInfo.RoleInfo.ID = App.GClassSessionInfo.RoleInfo.ListIntRoleID[0];
                    if (App.GClassSessionInfo.RoleInfo.ListIntRoleID[0] < 1061400000000000001)
                    {
                        App.GClassSessionInfo.RoleInfo.Name = App.GetDisplayCharater("DragPanelUserContainsRole", App.GClassSessionInfo.RoleInfo.ListIntRoleID[0].ToString());
                    }
                    else
                    {
                        App.GClassSessionInfo.RoleInfo.Name = App.GClassSessionInfo.RoleInfo.ListStrRoleName[0];
                    }
                    LabelLoginRoleShow.Content = App.GClassSessionInfo.RoleInfo.Name;

                    AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
                    DragPanelUserFeatures.BringIntoView();
                    LoadUserFeaturesByRole();
                }
                else
                {
                    App.GClassSessionInfo.RoleInfo.ID = -1;
                    LabelLoginRoleShow.Content = App.GetDisplayCharater("S0000056");
                    LoadUserFeaturesByRole();
                }
            }
            catch { }
            finally
            {
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                if (IBWLoadUserContainsRoles != null)
                {
                    IBWLoadUserContainsRoles.Dispose(); IBWLoadUserContainsRoles = null;
                }
            }
        }

        private void LRadioButtonSingleRole_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton LRadioButtonFocused = sender as RadioButton;
                if (LRadioButtonFocused.IsChecked == false) { LRadioButtonFocused.IsChecked = true; }
            }
            catch { }
        }
        #endregion

        #region 用户选择角色后进入主界面
        private void SelectedSingleRole()
        {
            string LStrRoleID = string.Empty;
            string LStrRoleName = string.Empty;

            try
            {
                foreach (RadioButton LRadioButtonSingleRole in StackPanelRolesList.Children)
                {
                    if (LRadioButtonSingleRole.IsChecked == true)
                    {
                        LStrRoleID = LRadioButtonSingleRole.DataContext.ToString();
                        LStrRoleName = LRadioButtonSingleRole.Content.ToString().Trim();
                    }
                }
                if (string.IsNullOrEmpty(LStrRoleID)) { return; }

                
                if (App.GClassSessionInfo.RoleInfo.ID != long.Parse(LStrRoleID))
                {
                    if (IStrWillDoingMethod == "ACT00")
                    {
                        DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
                        AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
                        DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                        DragPanelUserFeatures.BringIntoView();
                    }
                    App.GClassSessionInfo.RoleInfo.ID = long.Parse(LStrRoleID);
                    App.GClassSessionInfo.RoleInfo.Name = LStrRoleName;
                    LabelLoginRoleShow.Content = LStrRoleName;
                    LoadUserFeaturesByRole();
                    IStrLastLoadedXbap = string.Empty;
                }
                else
                {
                    DragPanelUserContainsRole.Visibility = System.Windows.Visibility.Collapsed;
                    DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                    DragPanelUserFeatures.BringIntoView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 根据用户角色加载用户功能权限
        private BackgroundWorker IBWLoadRoleContainsFO = null;

        private void DisableSwitchRoleMenu()
        {
            try
            {
                ContextMenu LContextMenuLoginOptions = ButtonMainOptions.ContextMenu;
                foreach (object LMenuItemSingle in LContextMenuLoginOptions.Items)
                {
                    if (LMenuItemSingle.GetType() == typeof(MenuItem))
                    {
                        MenuItem LMenuItemSingleOperation = LMenuItemSingle as MenuItem;
                        if (LMenuItemSingleOperation.DataContext.ToString() != "R-001") { continue; }
                        if (App.GClassSessionInfo.RoleInfo.ListIntRoleID.Count > 1)
                        {
                            LMenuItemSingleOperation.IsEnabled = true;
                        }
                        else
                        {
                            LMenuItemSingleOperation.IsEnabled = false;
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadUserFeaturesByRole()
        {
            GridLoginPanel.Visibility = System.Windows.Visibility.Collapsed;
            AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
            DisableSwitchRoleMenu();
            if (App.GClassSessionInfo.RoleInfo.ID < 0)
            {
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            
            AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
            AfterLoginPanel.BringIntoView();

            DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
            DragPanelUserFeatures.BringIntoView();

            LoadUserFeaturesAndOperations();
        }

        private void LoadUserFeaturesAndOperations()
        {
            List<string> LListLoadArgs = new List<string>();

            try
            {
                LListLoadArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListLoadArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListLoadArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                LListLoadArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                LListLoadArgs.Add(App.GClassSessionInfo.RoleInfo.ID.ToString());

                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridShowCurrentStatus.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();

                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;

                IBoolIsBusy = true;
                AfterLoginPanel.IsEnabled = false;
                IBWLoadRoleContainsFO = new BackgroundWorker();
                IBWLoadRoleContainsFO.RunWorkerCompleted += IBWLoadRoleContainsFO_RunWorkerCompleted;
                IBWLoadRoleContainsFO.DoWork += IBWLoadRoleContainsFO_DoWork;
                IBWLoadRoleContainsFO.RunWorkerAsync(LListLoadArgs);
            }
            catch (Exception ex)
            {
                GridShowCurrentStatus.Children.Clear();
                IBoolIsBusy = false;
                AfterLoginPanel.IsEnabled = true;
                if (IBWLoadRoleContainsFO != null)
                {
                    IBWLoadRoleContainsFO.Dispose(); IBWLoadRoleContainsFO = null;
                }
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IBWLoadRoleContainsFO_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                List<string> LListLoadArgs = e.Argument as List<string>;

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(16, LListLoadArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTableUser11003 = LWCFOperationReturn.DataSetReturn.Tables[0];
                    IListStrFeatureGroup = LWCFOperationReturn.ListStringReturn;
                }
                else
                {
                    IBoolCallReturn = false;
                    IStrCallReturn = LWCFOperationReturn.StringReturn;
                    return;
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
        }

        private void IBWLoadRoleContainsFO_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                GridShowCurrentStatus.Children.Clear();
                if (!IBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("S0000057") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                App.GBoolExistDefaultFeature = false;
                ShowUserWorkArea(false);
                UserExistDefaultFeature();
                
                
            }
            catch { }
            finally
            {
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                IBoolIsBusy = false;
                AfterLoginPanel.IsEnabled = true;
                if (IBWLoadRoleContainsFO != null)
                {
                    IBWLoadRoleContainsFO.Dispose(); IBWLoadRoleContainsFO = null;
                }
            }
        }

        private void ShowUserWorkArea(bool ABoolChangeLanguage)
        {
            StackPanelContainsFeatureGroup.Children.Clear();
            foreach (string LStrSingleGroupID in IListStrFeatureGroup)
            {
                UCSingleGroup LUCSingleGroup = new UCSingleGroup(IDataTableUser11003, LStrSingleGroupID, IStr11011005);
                LUCSingleGroup.IOperationEvent += LUCSingleGroup_IOperationEvent;
                StackPanelContainsFeatureGroup.Children.Add(LUCSingleGroup);
            }
            if (!ABoolChangeLanguage)
            {
                
            }
        }

        #endregion

        #region 判断用户是否有首页
        private BackgroundWorker IBWLoadRoleContainsDF = null;
        private void UserExistDefaultFeature()
        {
            try
            {
                IBWLoadRoleContainsDF = new BackgroundWorker();
                IBWLoadRoleContainsDF.RunWorkerCompleted += IBWLoadRoleContainsDF_RunWorkerCompleted;
                IBWLoadRoleContainsDF.DoWork += IBWLoadRoleContainsDF_DoWork;
                IBWLoadRoleContainsDF.RunWorkerAsync();
            }
            catch
            {
                if (IBWLoadRoleContainsDF != null)
                {
                    IBWLoadRoleContainsDF.Dispose(); IBWLoadRoleContainsDF = null;
                }
            }
        }

        private void IBWLoadRoleContainsDF_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
        }

        private void IBWLoadRoleContainsDF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (App.GBoolExistDefaultFeature)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "FeatureLoad";
                LEventArgs.ObjectSource0 = App.GDataRowDefaultFeature;
                LEventArgs.ObjectSource1 = App.GStrImageDefaultFeature;
                LUCSingleGroup_IOperationEvent(this, LEventArgs);
            }
            else
            {
                //MessageBox.Show(":(");
            }

            IBWLoadRoleContainsDF.Dispose(); IBWLoadRoleContainsDF = null;
        }
        #endregion

        private void ShowOrCloseFootWaitProgressBar(bool ABoolShow)
        {
            GridShowCurrentStatus.Children.Clear();
            if (ABoolShow)
            {
                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridShowCurrentStatus.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();
                GridShowCurrentStatus.BringIntoView();
            }
        }

        #region 处理其他系统发送过来的消息
        private WebReturn DealOtherApplicationMessage(WebRequest arg)
        {
            WebReturn LWebReturn = new WebReturn();
            string LStrSendArguments = string.Empty;

            try
            {
                LWebReturn.Result = true;
                LWebReturn.Code = 0;

                #region 模块加载完毕
                if (arg.Code == (int)RequestCode.CSModuleLoaded)
                {
                    if (!StringIsGuidType(arg.Session.SessionID))
                    {
                        new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WebBrowserApp.Visibility = System.Windows.Visibility.Visible;
                            GridLoadApplicationInterface.Visibility = System.Windows.Visibility.Collapsed;
                            GridBackHome.Visibility = System.Windows.Visibility.Visible;
                        }))).Start();
                    }
                    else
                    {
                        new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WebBrowserFeature.Visibility = System.Windows.Visibility.Visible;
                            ImageBackGround.Visibility = System.Windows.Visibility.Collapsed;
                            ImageLoadingFeature.Visibility = System.Windows.Visibility.Collapsed;
                            SprocketControlLoading.Visibility = System.Windows.Visibility.Collapsed;
                            WebBrowserFeature.BringIntoView();
                        }))).Start();
                    }
                }
                #endregion

                #region 模块加载中，请求全局参数
                if (arg.Code == (int)RequestCode.CSModuleLoading)
                {
                    SessionInfo LSessionInfoClient = arg.Session;
                    
                    App.GNetPipeHelper.ListClients.Clear();
                    App.GNetPipeHelper.ListClients.Add(LSessionInfoClient.SessionID);

                    LSessionInfoClient.AppName = App.GClassSessionInfo.AppName;
                    LSessionInfoClient.AppServerInfo = App.GClassSessionInfo.AppServerInfo;
                    LSessionInfoClient.DatabaseInfo = App.GClassSessionInfo.DatabaseInfo;
                    LSessionInfoClient.DBConnectionString = App.GClassSessionInfo.DBConnectionString;
                    LSessionInfoClient.DBType = App.GClassSessionInfo.DBType;
                    LSessionInfoClient.LangTypeInfo = App.GClassSessionInfo.LangTypeInfo;
                    LSessionInfoClient.SupportLangTypes = App.GClassSessionInfo.SupportLangTypes;
                    LSessionInfoClient.LocalMachineInfo = App.GClassSessionInfo.LocalMachineInfo;
                    LSessionInfoClient.RentInfo = App.GClassSessionInfo.RentInfo;
                    LSessionInfoClient.RoleInfo = App.GClassSessionInfo.RoleInfo;
                    LSessionInfoClient.ThemeInfo = App.GClassSessionInfo.ThemeInfo;
                    LSessionInfoClient.UserInfo = App.GClassSessionInfo.UserInfo;
                    LSessionInfoClient.SupportThemes = App.GClassSessionInfo.SupportThemes;
                    LSessionInfoClient.ListPartitionTables = App.GClassSessionInfo.ListPartitionTables;
                    //LSessionInfoClient.ModuleID = App.GClassSessionInfo.ModuleID;

                    if (IDataRowCurrentSelected != null)
                    {
                        LStrSendArguments = IDataRowCurrentSelected["C010"].ToString();
                        if (!string.IsNullOrEmpty(LStrSendArguments))
                        {
                            LWebReturn.Data = LStrSendArguments;
                        }
                    }
                    else
                    {
                        LWebReturn.Data = "InitDB";
                    }
                    LWebReturn.Session = LSessionInfoClient;
                }
                #endregion

                #region 子程序发送改变语言消息
                if (arg.Code == (int)RequestCode.CSLanguageChange)
                {
                    //App.GClassSessionInfo.LangTypeInfo.LangID = int.Parse(arg.Data);
                    //new System.Threading.Thread(() =>
                    //    this.Dispatcher.Invoke(new Action(() =>
                    //    {
                    //        App.LoadApplicationLanguages();
                    //    }))).Start();
                    IStrMessageSource = "S01";
                    new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ChangeDisplayLanguage(int.Parse(arg.Data), string.Empty);
                            //App.LoadApplicationLanguages();
                        }))).Start();
                }
                #endregion

                #region 子程序发送改变样式消息
                if (arg.Code == (int)RequestCode.CSThemeChange)
                {
                    App.GClassSessionInfo.ThemeInfo.Name = arg.Data;
                    new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            App.LoadApplicationResources();
                            InitLoginOptionsContextMenu();
                            InitMainPageOptionsContextMenu();
                        }))).Start();
                }
                #endregion

                #region 子程序发送切换角色消息
                if (arg.Code == (int)RequestCode.CSRoleChange)
                {

                }
                #endregion

                #region 子程序发送修改当前用户登录密码消息
                if (arg.Code == (int)RequestCode.CSChangePassword)
                {
                }
                #endregion

                #region 子程序发送返回主功能界面消息
                if (arg.Code == (int)RequestCode.CSHome)
                {
                    new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            AfterLoginPanel.Visibility = System.Windows.Visibility.Visible;
                            GridLoadOtherApplication.Visibility = System.Windows.Visibility.Collapsed;
                        }))).Start();
                }
                #endregion

                #region 子程序发送应用已经关闭消息，目前不处理
                if (arg.Code == (int)RequestCode.CSModuleClose)
                {
                    if (!StringIsGuidType(arg.Session.SessionID))
                    {
                    }
                }
                #endregion

                #region 客户端发送显示的提示消息
                if (arg.Code == 12111)
                {
                    new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                            {
                                ShowOrCloseFootWaitProgressBar(true);
                            }))).Start();
                }
                #endregion

                #region 客户端关闭提示消息
                if (arg.Code == 12112)
                {
                    new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ShowOrCloseFootWaitProgressBar(false);
                        }))).Start();
                }
                #endregion

                #region 客户端发送“忙”消息
                if (arg.Code == (int)RequestCode.CSIdleCheck)
                {
                    try
                    {
                        IIntIdleSecond = int.Parse(arg.Data);
                    }
                    catch { }
                }
                #endregion

                #region 客户端发送“忙”消息
                if (arg.Code == 91001)
                {
                    try
                    {
                        IIntIdleSecond = int.Parse(arg.Data);
                    }
                    catch { }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LWebReturn.Result = false;
                LWebReturn.Code = Defines.RET_FAIL;
                LWebReturn.Message = "DealOtherApplicationMessage()\n" + ex.Message;
            }

            return LWebReturn;
        }
        #endregion

        #region 发送消息给应用程序
        private WebReturn SendMessageToClient(WebRequest AWebRequest)
        {
            WebReturn LWebReturn = new WebReturn();

            try
            {
                LWebReturn.Result = true;
                LWebReturn.Code = 0;
                return App.GNetPipeHelper.SendMessage(AWebRequest, App.GNetPipeHelper.ListClients[0]);
            }
            catch (Exception ex)
            {
                LWebReturn.Result = false;
                LWebReturn.Code = Defines.RET_FAIL;
                LWebReturn.Message = ex.ToString();
            }

            return LWebReturn;
        }

        /// <summary>
        /// 发送关系消息给应用程序
        /// </summary>
        private void SendCloseMessage2Application()
        {
            WebRequest LWebRequestSend = new WebRequest();

            try
            {
                BorderYoungApplicationsA.Visibility = System.Windows.Visibility.Collapsed;
                BorderUserFeatures.Visibility = System.Windows.Visibility.Visible;
                BorderUserFeatures.BringIntoView();
                
                if (App.GNetPipeHelper.ListClients.Count <= 0) { return; }
                LWebRequestSend.Code = (int)RequestCode.CSModuleClose;
                WebReturn LWebReturnSend = SendMessageToClient(LWebRequestSend);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region 关闭已经打开的应用系统，仅限定打开方式为 "P" 
        private void ClosePopupApplication()
        {
            BorderYoungApplicationsA.Visibility = System.Windows.Visibility.Collapsed;
            BorderUserFeatures.Visibility = System.Windows.Visibility.Visible;
            BorderUserFeatures.BringIntoView();

            WebRequest LWebRequestSend = new WebRequest();
            LWebRequestSend.Code = 91002;
            SendMessageToClient(LWebRequestSend);

            LWebRequestSend.Code = (int)RequestCode.SCIdleCheckStop;
            SendMessageToClient(LWebRequestSend);
        }
        #endregion

        #region 属性定义

        /// <summary>
        /// 功能的图标
        /// </summary>
        private string _StrFeatureImageSource;
        public string StrFeatureImageSource
        {
            get { return _StrFeatureImageSource; }
            set
            {
                _StrFeatureImageSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureImageSource");
                }
            }
        }

        /// <summary>
        /// 功能编号
        /// </summary>
        private string _StrFeatureID = string.Empty;
        public string StrFeatureID
        {
            get { return _StrFeatureID; }
            set { _StrFeatureID = value; }
        }

        /// <summary>
        /// 当前功能窗口是否为满屏显示
        /// </summary>
        private bool IBoolIsFullView = false;
        
        #endregion

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion

        #region 处理我的工作内容发过来的消息
        DataRow IDataRowCurrentSelected = null;
        private void LUCSingleGroup_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrApplicationGuid = string.Empty;
            string LStrApplicationServerBaseUrl = string.Empty;
            string LStrApplicationUrl = string.Empty;
            string LStr11003011 = string.Empty;         //模块打开方式

            try
            {
                DataRow LDataRowFeatureInfo = e.ObjectSource0 as DataRow;
                IDataRowCurrentSelected = LDataRowFeatureInfo;

                StrFeatureImageSource = e.ObjectSource1 as string;

                LStrApplicationUrl = LDataRowFeatureInfo["C009"].ToString();
                if (LStrApplicationUrl == "?")
                {
                    MessageBox.Show("This feature is about to open, so stay tuned...", App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                IStr11003002 = LDataRowFeatureInfo["C002"].ToString();
                
                if (App.GClassSessionInfo.AppServerInfo.SupportHttps)
                {
                    LStrApplicationServerBaseUrl = "https://" + App.GClassSessionInfo.AppServerInfo.Address + ":" + App.GClassSessionInfo.AppServerInfo.Port.ToString();
                }
                else
                {
                    LStrApplicationServerBaseUrl = "http://" + App.GClassSessionInfo.AppServerInfo.Address + ":" + App.GClassSessionInfo.AppServerInfo.Port.ToString();
                }

                LStrApplicationUrl = LStrApplicationServerBaseUrl + "/" + LStrApplicationUrl;
                LStr11003011 = LDataRowFeatureInfo["C011"].ToString();

                #region 在我的工作内容中打开
                if (LStr11003011 == "P")
                {
                    LabelApplicationHeader.Content = App.GetDisplayCharater("FO" + IStr11003002);
                    BorderUserFeatures.Visibility = System.Windows.Visibility.Collapsed;
                    BorderYoungApplicationsA.Visibility = System.Windows.Visibility.Visible;
                    WebBrowserApp.Visibility = System.Windows.Visibility.Collapsed;
                    GridBackHome.Visibility = System.Windows.Visibility.Collapsed;
                    GridLoadApplicationInterface.Visibility = System.Windows.Visibility.Visible;
                    if (LStrApplicationUrl != IStrLastLoadedXbap)
                    {
                        if (!string.IsNullOrEmpty(IStrLastLoadedXbap)) 
                        {
                        //    SendCloseMessage2Application(); 
                        }
                        LStrApplicationGuid = LDataRowFeatureInfo["C015"].ToString();
                        WebBrowserApp.Navigate(LStrApplicationUrl + "?" + LStrApplicationGuid);
                        IStrLastLoadedXbap = LStrApplicationUrl;
                    }
                    else
                    {
                        WebRequest LWebRequestSend = new WebRequest();
                        LWebRequestSend.Code = 22101;
                        LWebRequestSend.Data = LDataRowFeatureInfo["C010"].ToString();
                        WebReturn LWebReturnSend = SendMessageToClient(LWebRequestSend);

                        LWebRequestSend.Code = 91003;
                        SendMessageToClient(LWebRequestSend);

                        LWebRequestSend.Code = (int)RequestCode.SCIdleCheckStart;
                        SendMessageToClient(LWebRequestSend);
                    }
                    return;
                }
                #endregion

                #region 全屏打开
                if (LStr11003011 == "F")
                {
                    AfterLoginPanel.Visibility = System.Windows.Visibility.Collapsed;
                    GridLoadOtherApplication.Visibility = System.Windows.Visibility.Visible;
                    if (LStrApplicationUrl != IStrLastLoadedXbap)
                    {
                        WebBrowserFeature.Visibility = System.Windows.Visibility.Collapsed;
                        ImageBackGround.Visibility = System.Windows.Visibility.Visible;
                        ImageLoadingFeature.Visibility = System.Windows.Visibility.Visible;
                        SprocketControlLoading.Visibility = System.Windows.Visibility.Visible;

                        if (!string.IsNullOrEmpty(IStrLastLoadedXbap))
                        {
                        //    SendCloseMessage2Application();
                        }
                        LStrApplicationGuid = Guid.NewGuid().ToString();
                        WebBrowserFeature.Navigate(LStrApplicationUrl + "?" + LStrApplicationGuid);
                        IStrLastLoadedXbap = LStrApplicationUrl;
                    }
                    else
                    {
                        WebRequest LWebRequestSend = new WebRequest();
                        LWebRequestSend.Code = 21010;
                        LWebRequestSend.Data = LDataRowFeatureInfo["C010"].ToString();
                        WebReturn LWebReturnSend = SendMessageToClient(LWebRequestSend);
                        
                        LWebRequestSend.Code = 91003;
                        SendMessageToClient(LWebRequestSend);

                        LWebRequestSend.Code = (int)RequestCode.SCIdleCheckStart;
                        SendMessageToClient(LWebRequestSend);

                        new System.Threading.Thread(() =>
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WebBrowserFeature.Visibility = System.Windows.Visibility.Visible;
                            ImageBackGround.Visibility = System.Windows.Visibility.Collapsed;
                            ImageLoadingFeature.Visibility = System.Windows.Visibility.Collapsed;
                            SprocketControlLoading.Visibility = System.Windows.Visibility.Collapsed;
                            WebBrowserFeature.BringIntoView();
                        }))).Start();
                    }
                    return;
                }
                #endregion

                #region 启动IE 打开ASM
                if (LStr11003011 == "A")
                {
                    string LStrVerificationCode004 = App.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    try
                    {
                        string LStrASMUrl = LDataRowFeatureInfo["C009"].ToString();
                        string LStrUserName = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.UserInfo.UserName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        string LStrUserId = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.UserInfo.UserID.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        string LStrRoleId = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.RoleInfo.ID.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        string LStrDeptId = EncryptionAndDecryption.EncryptDecryptString(IStr1105006, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        string LStrPassword = EncryptionAndDecryption.EncryptDecryptString(App.GClassSessionInfo.UserInfo.Password, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                        string LStrNewUrl = IListStrASMInformation[0] + IListStrASMInformation[1] + ":" + IListStrASMInformation[2] + string.Format(LStrASMUrl, LStrUserName, LStrUserId, App.GClassSessionInfo.LangTypeInfo.LangID, LStrRoleId, IStr1105006, LStrPassword);
                        //MessageBox.Show(LStrNewUrl);
                        Process LProcess = new Process();
                        LProcess.StartInfo.FileName = "iexplore.exe";
                        LProcess.StartInfo.Verb = "open";
                        LProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        LProcess.StartInfo.Arguments = LStrNewUrl;
                        LProcess.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region 判断字符串是否为GUID
        private bool StringIsGuidType(string AStrSource)
        {
            bool LBoolReturn = true;

            Guid LGuidTemp = Guid.Empty;

            LBoolReturn = Guid.TryParseExact(AStrSource, "D", out LGuidTemp);

            return LBoolReturn;
        }
        #endregion

        #region 无操作强制注销
        int IIntIdleSecond = 0;
        int IIntAllowIdleSecond = 0;

        private Timer ITimerCheckUserIdle = new Timer(1000);
        private delegate void IDelegateCheckIdlefterLoginSystem();

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                IIntIdleSecond = 0;
            }
            catch { }
        }

        private void ITimerCheckUserIdle_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IIntAllowIdleSecond == 0) { return; }
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateCheckIdlefterLoginSystem(CheckIdleAfterLoginSystem));
        }

        private void CheckIdleAfterLoginSystem()
        {
            try
            {
                IIntIdleSecond += 1;
                if (IIntIdleSecond <= IIntAllowIdleSecond) { return; }
                StopAllTimer();
                ShowTipOnLoginPannel(string.Format(App.GetDisplayCharater("S0000066"), (IIntAllowIdleSecond / 60).ToString()));
                FollowAction05(true);
            }
            catch { }
        }

        #endregion

        #region 超过登录时间后强制注销
        int IIntAfterLoginCount = 0;
        int IIntAllowOnlineSecond = 0;

        private Timer ITimerCheckAfterLogin = new Timer(1000);
        private delegate void IDelegateCheckAfterLogin();

        private void ITimerCheckAfterLogin_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IIntAllowOnlineSecond == 0) { return; }
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateCheckAfterLogin(CheckCountTimeAfterLogin));
        }

        private void CheckCountTimeAfterLogin()
        {
            try
            {
                IIntAfterLoginCount += 1;
                if (IIntAfterLoginCount <= IIntAllowOnlineSecond) { return; }
                StopAllTimer();
                //ShowTipOnLoginPannel(string.Format(App.GetDisplayCharater("S0000065"), (IIntAllowOnlineSecond / 3600).ToString()));
                ShowTipOnLoginPannel(App.GetDisplayCharater("S0000065"));
                FollowAction05(true);
            }
            catch { }
        }

        #endregion

        #region 判断当前登录在系统中是否有效
        private Timer ITimerCheckCurrentLoginIDStatus = new Timer(2000);
        private bool IBoolTimerIsBusy = false;
        private delegate void IDelegateCheckLoginIDStatus();

        private void ITimerCheckCurrentLoginIDStatus_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateCheckLoginIDStatus(CheckSystemLoginIDStatus));
        }

        private void CheckSystemLoginIDStatus()
        {
            List<string> LListStrWcfArgs = new List<string>();

            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            try
            {
                if (IBoolTimerIsBusy) { return; }

                IBoolTimerIsBusy = true;
                LListStrWcfArgs.Clear();
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                LListStrWcfArgs.Add(App.GClassSessionInfo.SessionID.ToString());

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(52, LListStrWcfArgs);
                if (LWCFOperationReturn.StringReturn != "1")
                {
                    StopAllTimer();
                    ShowTipOnLoginPannel(string.Format(App.GetDisplayCharater("S0000067"), App.GClassSessionInfo.UserInfo.Account));
                    FollowAction05(true);
                }
            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
                IBoolTimerIsBusy = false;
            }
        }
        #endregion

        #region 停止所有的定时器
        private void StopAllTimer()
        {
            try
            {
                try { if (ITimerCheckUserIdle.Enabled == true) { ITimerCheckUserIdle.Stop(); } }
                catch { }
                try { if (ITimerCheckAfterLogin.Enabled == true) { ITimerCheckAfterLogin.Stop(); } }
                catch { }
                try { if (ITimerCheckCurrentLoginIDStatus.Enabled == true) { ITimerCheckCurrentLoginIDStatus.Stop(); } }
                catch { }
            }
            catch { }
        }
        #endregion

        #region 在登录界面显示文字提示
        private void ShowTipOnLoginPannel(string AStrTip)
        {
            GridWaitProgress.Children.Clear();
            UCStatusTipTool LUCStatusTipTool = new UCStatusTipTool(Brushes.YellowGreen);
            LUCStatusTipTool.ShowStatusTipTool(AStrTip);
            GridWaitProgress.Children.Add(LUCStatusTipTool);
        }
        #endregion

        #region 初始化邮件、消息显示语言
        private void InitUserDefaultLanguageList()
        {
            try
            {
                StackPanelLanguageList.Children.Clear();
                for (int LIntLoopLanguge = 0; LIntLoopLanguge < App.GClassSessionInfo.SupportLangTypes.Count; LIntLoopLanguge++)
                {
                    RadioButton LRadioButtonSingleLanguage = new RadioButton();
                    LRadioButtonSingleLanguage.Style = (Style)App.Current.Resources["RadioButtonSelectRolesStyle"];
                    LRadioButtonSingleLanguage.Content = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].Display;
                    LRadioButtonSingleLanguage.DataContext = App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString();
                    if (App.GClassSessionInfo.SupportLangTypes[LIntLoopLanguge].LangID.ToString() == IStr1100101)
                    {
                        LRadioButtonSingleLanguage.IsChecked = true;
                        LRadioButtonSingleLanguage.Loaded += (ASender, AArgs) => { LRadioButtonSingleLanguage.Focus(); };
                    }
                    StackPanelLanguageList.Children.Add(LRadioButtonSingleLanguage);
                }
            }
            catch { }
        }
        #endregion

        #region 显示 / 设置 邮件、告警消息显示的语言 的界面
        private void SetMessageDisplayLanguage()
        {
            ResetElementShowObject();
            DragPanelUserFeatures.Visibility = System.Windows.Visibility.Collapsed;
            DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Visible;
            DragPanelUserDefaultLanguage.BringIntoView();
            return;
        }

        private BackgroundWorker IBWSetUserDefaultLanguage = null;
        private void SaveUserDisplayLanguage()
        {
            string LStrLanguageID = string.Empty;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                foreach (RadioButton LRadioButtonSingleLanguage in StackPanelLanguageList.Children)
                {
                    if (LRadioButtonSingleLanguage.IsChecked == true)
                    {
                        LStrLanguageID = LRadioButtonSingleLanguage.DataContext.ToString();
                    }
                }
                if (string.IsNullOrEmpty(LStrLanguageID)) { return; }

                if (LStrLanguageID != IStr1100101)
                {
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                    LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                    LListStrWcfArgs.Add("1100101");
                    LListStrWcfArgs.Add("W");
                    LListStrWcfArgs.Add(LStrLanguageID);

                    IBoolCallReturn = true;
                    IStrCallReturn = string.Empty;

                    WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                    GridWaitProgress.Children.Add(LWaitPorgressBar);
                    LWaitPorgressBar.StartAnimation();

                    DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Collapsed;

                    IBWSetUserDefaultLanguage = new BackgroundWorker();
                    IBWSetUserDefaultLanguage.RunWorkerCompleted += IBWSetUserDefaultLanguage_RunWorkerCompleted;
                    IBWSetUserDefaultLanguage.DoWork += IBWSetUserDefaultLanguage_DoWork;
                    IBWSetUserDefaultLanguage.RunWorkerAsync(LListStrWcfArgs);
                }
                else
                {
                    DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Collapsed;
                    DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                    DragPanelUserFeatures.BringIntoView();
                }
            }
            catch (Exception ex)
            {
                DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Visible;
                GridWaitProgress.Children.Clear();
                if (IBWSetUserDefaultLanguage != null)
                {
                    IBWSetUserDefaultLanguage.Dispose(); IBWSetUserDefaultLanguage = null;
                }
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void IBWSetUserDefaultLanguage_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                List<string> LListChangeArgs = e.Argument as List<string>;

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(53, LListChangeArgs);

                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                if (IBoolCallReturn) { IStr1100101 = LListChangeArgs[6];}
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        private void IBWSetUserDefaultLanguage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                GridWaitProgress.Children.Clear();
                if (!IBoolCallReturn)
                {
                    DragPanelUserDefaultLanguage.Visibility = System.Windows.Visibility.Visible;
                    MessageBox.Show(App.GetDisplayCharater("S0000089") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DragPanelResetUserPassword.Visibility = System.Windows.Visibility.Collapsed;
                DragPanelUserFeatures.Visibility = System.Windows.Visibility.Visible;
                DragPanelUserFeatures.BringIntoView();
            }
            catch { }
            finally
            {
                IBoolCallReturn = true;
                IStrCallReturn = string.Empty;
                if (IBWSetUserDefaultLanguage != null)
                {
                    IBWSetUserDefaultLanguage.Dispose(); IBWSetUserDefaultLanguage = null;
                }
            }
        }
        #endregion
    }
}


