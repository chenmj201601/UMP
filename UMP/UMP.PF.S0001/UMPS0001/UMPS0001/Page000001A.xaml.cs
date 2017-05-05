using PFShareClassesC;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS0001.CreateDatabaseObject;
using UMPS0001.WCFService00001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS0001
{
    public partial class Page000001A : Page, S0001Interface, S0001ChangeLanguageInterface
    {
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetAncestor(IntPtr hwnd, int flags);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        public event EventHandler<OperationEventArgs> IOperationEvent;
        public event EventHandler<OperationEventArgs> IChangeLanguageEvent;

        private DragHelper IDragHelperCreateDatabase;

        public string IStrImageFolder = string.Empty;

        private string IStrVerificationCode004 = string.Empty;
        private string IStrVerificationCode104 = string.Empty;

        private DataTable IDataTableDatabaseParameters = null;

        private OperationDataArgs IWCFOperationDataArgs = null;

        private bool IBoolIsBusy = false;

        public Page000001A()
        {
            InitializeComponent();

            IStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            IStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            this.Loaded += Page000001A_Loaded;
        }

        private void Page000001A_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.IPageMainOpend = this;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = (int)RequestCode.CSModuleLoading;
                LWebRequestClientLoading.Session = App.GClassSessionInfo;
                LWebRequestClientLoading.Session.SessionID = App.GClassSessionInfo.SessionID;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (LWebReturn.Result)
                {
                    App.GClassSessionInfo.AppName = LWebReturn.Session.AppName;
                    App.GClassSessionInfo.AppServerInfo = LWebReturn.Session.AppServerInfo;
                    App.GClassSessionInfo.DatabaseInfo = LWebReturn.Session.DatabaseInfo;
                    App.GClassSessionInfo.DBConnectionString = LWebReturn.Session.DBConnectionString;
                    App.GClassSessionInfo.DBType = LWebReturn.Session.DBType;
                    App.GClassSessionInfo.LangTypeInfo = LWebReturn.Session.LangTypeInfo;
                    App.GClassSessionInfo.LocalMachineInfo = LWebReturn.Session.LocalMachineInfo;
                    App.GClassSessionInfo.RentInfo = LWebReturn.Session.RentInfo;
                    App.GClassSessionInfo.RoleInfo = LWebReturn.Session.RoleInfo;
                    App.GClassSessionInfo.ThemeInfo = LWebReturn.Session.ThemeInfo;
                    App.GClassSessionInfo.UserInfo = LWebReturn.Session.UserInfo;
                    if (!string.IsNullOrEmpty(LWebReturn.Data)) { App.GStrCurrentOperation = LWebReturn.Data; }
                    App.LoadStyleDictionary();
                    App.LoadApplicationLanguages();
                    LoadThisModuleData();
                }

                DoingMainSendMessage(App.GStrCurrentOperation);

                WebRequest LWebRequestClientLoaded = new WebRequest();
                LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
                LWebRequestClientLoaded.Session = App.GClassSessionInfo;
                LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
                App.SendNetPipeMessage(LWebRequestClientLoaded);

                IStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S0001");

                #region 初始化隐藏、显示的Panel
                DragPanelCreateDatabase.Visibility = System.Windows.Visibility.Collapsed;
                #endregion

                #region 初始化DragPanel
                IDragHelperCreateDatabase = new DragHelper();
                IDragHelperCreateDatabase.Init(MainPanel, DragPanelCreateDatabase);
                #endregion

                #region 初始化控件事件
                ButtonCloseCreateDatabase.Click += ButtonControlClicked;
                ButtonSkip.Click += ButtonControlClicked;
                ButtonBack.Click += ButtonControlClicked;
                ButtonNext.Click += ButtonControlClicked;
                ButtonExit.Click += ButtonControlClicked;
                #endregion

                ShowAllDatabaseObjects();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "UMPS0001");
            }
        }

        private void ButtonControlClicked(object sender, RoutedEventArgs e)
        {
            Button LButtonClicked = sender as Button;
            string LStrButtonName = LButtonClicked.Name;

            if (IBoolIsBusy) { return; }

            if (LStrButtonName == "ButtonCloseCreateDatabase" || LStrButtonName == "ButtonExit")
            {
                if (!string.IsNullOrEmpty(IStrLocateFolderType))
                {
                    IStrLocateFolderType = string.Empty;
                    GridCreateDatabaseDetail.Children.Clear();
                    GridCreateDatabaseDetail.Children.Add(IUCDatabaseBasicInformation);

                    ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                    ButtonBack.Visibility = System.Windows.Visibility.Visible;
                    ButtonSkip.Content = App.GetDisplayCharater("BT0003");
                    ButtonBack.Content = App.GetDisplayCharater("BT0002");
                    ButtonNext.Content = App.GetDisplayCharater("BT0001");
                    ButtonExit.Content = App.GetDisplayCharater("BT0004");
                }
                else
                {
                    try
                    {
                        IUCDatabaseBasicInformation.IOperationEvent -= LUCObjectOperationIOperationEvent;
                    }
                    catch { }
                    GridObjectAllViewPanel.IsEnabled = true;
                    DragPanelCreateDatabase.Visibility = System.Windows.Visibility.Collapsed;
                }

                return;
            }

            if (LStrButtonName == "ButtonBack" || LStrButtonName == "ButtonNext" || LStrButtonName == "ButtonSkip")
            {
                if (LStrButtonName == "ButtonSkip" && !string.IsNullOrEmpty(IStrLocateFolderType))
                {
                    IUCLocateServerFolder.ResetReloadOptions();
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "C001";
                    LEventArgs.ObjectSource0 = IUCLocateServerFolder.IStrCurrentFolderType;
                    LEventArgs.ObjectSource1 = string.Empty;

                    IUCLocateServerFolder.IOperationEvent -= LUCObjectOperationIOperationEvent;
                    IUCLocateServerFolder = null;
                    IStrLocateFolderType = string.Empty;
                    
                    ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                    ButtonBack.Visibility = System.Windows.Visibility.Visible;
                    ButtonSkip.Content = App.GetDisplayCharater("BT0003");
                    ButtonBack.Content = App.GetDisplayCharater("BT0002");
                    ButtonNext.Content = App.GetDisplayCharater("BT0001");
                    ButtonExit.Content = App.GetDisplayCharater("BT0004");

                    ShowLocateFolder(LEventArgs);
                }
                else if (LStrButtonName == "ButtonNext" && !string.IsNullOrEmpty(IStrLocateFolderType))
                {
                    string LStrCallReturn = string.Empty;

                    List<string> LListStrSelectFolder = IUCLocateServerFolder.GetSettedData(ref LStrCallReturn);
                    if (LListStrSelectFolder.Count <= 0) { return; }
                    IUCDatabaseBasicInformation.ShowSelectedLocateFolder(LListStrSelectFolder[0], IStrLocateFolderType);
                    IStrLocateFolderType = string.Empty;
                    GridCreateDatabaseDetail.Children.Clear();
                    GridCreateDatabaseDetail.Children.Add(IUCDatabaseBasicInformation);

                    ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                    ButtonBack.Visibility = System.Windows.Visibility.Visible;
                    ButtonSkip.Content = App.GetDisplayCharater("BT0003");
                    ButtonBack.Content = App.GetDisplayCharater("BT0002");
                    ButtonNext.Content = App.GetDisplayCharater("BT0001");
                    ButtonExit.Content = App.GetDisplayCharater("BT0004");

                    try
                    {
                        IUCLocateServerFolder.IOperationEvent -= LUCObjectOperationIOperationEvent;
                        IUCLocateServerFolder = null;
                    }
                    catch { }
                }
                else
                {
                    CreateDatabaseStepIn(LButtonClicked.DataContext.ToString());
                }
                return;
            }
        }

        public void DoingMainSendMessage(string AStrData)
        {
            try
            {
                IChangeLanguageEvent = null;
                switch (AStrData)
                {
                    case "InitDB":
                        OpenApplicationFromLoginFrame();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OpenApplicationFromLoginFrame()
        {
            GridMainOptions.Visibility = System.Windows.Visibility.Collapsed;
            GridHomeOptions.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// 加载本应用程序需要的数据
        /// </summary>
        private void LoadThisModuleData()
        {
            Service00001Client LService00001Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;

            try
            {
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);

                IDataTableDatabaseParameters = new DataTable();
                LWCFOperationReturn = LService00001Client.OperationMethodA(1, null);
                if (LWCFOperationReturn.BoolReturn)
                {
                    if (LWCFOperationReturn.StringReturn == "1")
                    {
                        IDataTableDatabaseParameters = LWCFOperationReturn.DataSetReturn.Tables[0];
                    }
                    else { IDataTableDatabaseParameters = null; }
                }
                else { IDataTableDatabaseParameters = null; }
            }
            catch { }
            finally
            {
                if (LService00001Client != null)
                {
                    if (LService00001Client.State == CommunicationState.Opened) { LService00001Client.Close(); }
                }
            }

        }

        private void ShowAllDatabaseObjects()
        {
            ShowNormatlLabelContent();
            if (IDataTableDatabaseParameters == null)
            {
                ShowDatabaseNotCreateContentOperations();
                return;
            }
        }

        private void ShowNormatlLabelContent()
        {
            LabelDatabaseManagementCenter.Content = App.GetDisplayCharater("M02000");
            LabelDatabaseObjectList.Content = App.GetDisplayCharater("M02001");
            LabelDatabaseObjectOperation.Content = App.GetDisplayCharater("M02002");

            LabelLoginAccountShow.Content = App.GClassSessionInfo.UserInfo.Account + " (Administrator)";
            LabelLoginRoleShow.Content = App.GetDisplayCharater("M01001");
        }

        private void ShowDatabaseNotCreateContentOperations()
        {
            LabelCurrentObjectName.Content = App.GetDisplayCharater("M02003");
            LabelCurrentObjectName.Style = (Style)App.Current.Resources["LabelCurrentObjectNameStyle0"];
            StackPanelObjectOperations.Children.Clear();
            UCObjectOperationCreateDB LUCObjectOperationCreateDB = new UCObjectOperationCreateDB("0101");
            LUCObjectOperationCreateDB.ShowOperationDetails(null);
            LUCObjectOperationCreateDB.Margin = new Thickness(0, 1, 0, 1);
            LUCObjectOperationCreateDB.IOperationEvent += LUCObjectOperationIOperationEvent;
            StackPanelObjectOperations.Children.Add(LUCObjectOperationCreateDB);
        }

        #region 操作发送过来的消息
        private void LUCObjectOperationIOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrObjectTag = string.Empty;

            LStrObjectTag = e.StrObjectTag;
            switch (LStrObjectTag)
            {
                case "0101":
                    ObjectOperation0101(e);
                    break;
                case "C001":
                    ShowLocateFolder(e);
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void ObjectOperation0101(OperationEventArgs AEventArgs)
        {
            GridObjectAllViewPanel.IsEnabled = false;
            DragPanelCreateDatabase.Visibility = System.Windows.Visibility.Visible;
            
            LabelTitleCreateDatabase.Content = App.GetDisplayCharater("FO0101");
            DragPanelCreateDatabase.BringIntoView();

            CreateDatabaseStep00();
        }

        #region 创建数据库 - 公用1
        private string IStrCreateDBCurrentStep = string.Empty;

        public string IStrDatabaseType = string.Empty;
        //当前数据库中对象的版本
        private string IStrObjectInDBVersion = string.Empty;
        //挡墙创建对象的版本
        private string IStrObjectCreatedVersion = string.Empty;

        private UCSelectDatabaseType IUCSelectDatabaseType = new UCSelectDatabaseType();

        //创建MS SQL Server 数据库使用的UserControl
        private UCConnect2SQLServer IUCConnect2SQLServer = new UCConnect2SQLServer();
        private UCDatabaseBasicInformation IUCDatabaseBasicInformation = new UCDatabaseBasicInformation();
        private UCLocateServerFolder IUCLocateServerFolder = null;
        private UCCreateLoginAccount IUCCreateLoginAccount = new UCCreateLoginAccount();
        private UCConfirmConnectionProfile IUCConfirmConnectionProfile = new UCConfirmConnectionProfile();

        //创建Oracle数据库使用的UserControl
        private UCConnect2Oracle IUCConnect2Oracle = new UCConnect2Oracle();

        //创建数据库公用部分
        private UCBasicRentUserData IUCBasicRentUserData = new UCBasicRentUserData();
        private UCCreateDatabaseObject IUCCreateDatabaseObject = new UCCreateDatabaseObject();
        private UCInitTableData IUCInitTableData = new UCInitTableData();
        private UCDatabaseConnectionProfile IUCDatabaseConnectionProfile = new UCDatabaseConnectionProfile();

        private OperationParameters IParameterStep00 = new OperationParameters();

        private OperationParameters IParameterStep21 = new OperationParameters();
        private OperationParameters IParameterStep22 = new OperationParameters();
        private OperationParameters IParameterStep23 = new OperationParameters();
        private OperationParameters IParameterStep24 = new OperationParameters();

        private OperationParameters IParameterStep34 = new OperationParameters();

        private OperationParameters IParameterStep01 = new OperationParameters();

        private BackgroundWorker IBackgroundWorkerGetDB2Information = null;
        private BackgroundWorker IBackgroundWorkerLocateServerFolder = null;
        private BackgroundWorker IBackgroundWorkerCreateDB2Database = null;
        private BackgroundWorker IBackgroundWorkerCreateLoginAccount = null;
        private BackgroundWorker IBackgroundWorkerConfirmConnectDatabase = null;
        private BackgroundWorker IBackgroundWorkerObtainCreateObjects = null;
        private BackgroundWorker IBackgroundWorkerCreateDatabaseObjects = null;
        private BackgroundWorker IBackgroundWorkerInitDatabaseObjects = null;
        private BackgroundWorker IBackgroundWorkerFinishCreateDatabase = null;

        private BackgroundWorker IBackgroundWorkerConfirmConnectOracle = null;

        private void CreateDatabaseStepIn(string AStrStepOperation)
        {
            if (IStrCreateDBCurrentStep == "00" && AStrStepOperation == "N") { CreateDatabaseStep00Next(); return; }

            if ((IStrCreateDBCurrentStep == "21" || IStrCreateDBCurrentStep == "34") && AStrStepOperation == "B") { CreateDatabaseStep00(); return; }

            #region 在 MS SQL Server 中创建的过程
            if ((IStrCreateDBCurrentStep == "21") && AStrStepOperation == "N") { CreateDatabaseStep21Next(); return; }

            if (IStrCreateDBCurrentStep == "22" && AStrStepOperation == "B") { CreateDatabaseStep00Next(); return; }
            if (IStrCreateDBCurrentStep == "22" && AStrStepOperation == "N") { CreateDatabaseStep22Next(); return; }
            if (IStrCreateDBCurrentStep == "22" && AStrStepOperation == "S") { CreateDatabaseStep23(); return; }

            if (IStrCreateDBCurrentStep == "23" && AStrStepOperation == "B") { CreateDatabaseStep22(true); return; }
            if (IStrCreateDBCurrentStep == "23" && AStrStepOperation == "N") { CreateDatabaseStep23Next(); return; }
            if (IStrCreateDBCurrentStep == "23" && AStrStepOperation == "S") { CreateDatabaseStep24(false); return; }

            if (IStrCreateDBCurrentStep == "24" && AStrStepOperation == "B") { CreateDatabaseStep23(); return; }
            if (IStrCreateDBCurrentStep == "24" && AStrStepOperation == "N") { CreateDatabaseStep24Next(); return; }
            #endregion

            #region 在 Oracle 中创建的过程
            if ((IStrCreateDBCurrentStep == "34") && AStrStepOperation == "N") { CreateDatabaseStep34Next(); return; }
            #endregion

            #region 创建数据库结束后的过程
            if (IStrCreateDBCurrentStep == "01" && AStrStepOperation == "B")
            {
                List<string> LListStrSelectedDBType = IParameterStep00.ObjectSource0 as List<string>;
                if (LListStrSelectedDBType[0] == "2") { CreateDatabaseStep24(true); } else { CreateDatabaseStep00(); }
                return;
            }

            if (IStrCreateDBCurrentStep == "01" && AStrStepOperation == "N") { CreateDatabaseStep01Next(); return; }

            if (IStrCreateDBCurrentStep == "02" && AStrStepOperation == "B") { CreateDatabaseStep01(true); return; }
            if (IStrCreateDBCurrentStep == "02" && AStrStepOperation == "N") { CreateDatabaseStep02Next(); return; }
            if (IStrCreateDBCurrentStep == "02" && AStrStepOperation == "S") { CreateDatabaseStep03(false); return; }

            if (IStrCreateDBCurrentStep == "03" && AStrStepOperation == "B") { CreateDatabaseStep02(true); return; }
            if (IStrCreateDBCurrentStep == "03" && AStrStepOperation == "N") { CreateDatabaseStep03Next(); return; }
            if (IStrCreateDBCurrentStep == "03" && AStrStepOperation == "S") { CreateDatabaseStep04(false); return; }

            if (IStrCreateDBCurrentStep == "04" && AStrStepOperation == "B") { CreateDatabaseStep03(true); return; }
            if (IStrCreateDBCurrentStep == "04" && AStrStepOperation == "N") { CreateDatabaseStep04Next(); return; }
            #endregion
            
        }

        private void CreateDatabaseStep00()
        {
            IStrCreateDBCurrentStep = "00";
            ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
            ButtonBack.Visibility = System.Windows.Visibility.Collapsed;
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            GridCreateDatabaseDetail.Children.Add(IUCSelectDatabaseType);
        }

        private void CreateDatabaseStep00Next()
        {
            ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
            ButtonBack.Visibility = System.Windows.Visibility.Visible;
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();

            List<string> LListStrDBType = IUCSelectDatabaseType.GetSettedData();
            IStrObjectInDBVersion = LListStrDBType[0];
            if (LListStrDBType[0] == "2")
            {
                IStrCreateDBCurrentStep = "21";
                GridCreateDatabaseDetail.Children.Add(IUCConnect2SQLServer);
            }
            else
            {
                IStrCreateDBCurrentStep = "34";
                GridCreateDatabaseDetail.Children.Add(IUCConnect2Oracle);
            }
            IParameterStep00.ObjectSource0 = LListStrDBType;
            IStrDatabaseType = LListStrDBType[0];
        }
        #endregion

        #region 在MS SQL Server中创建数据库
        private string IStrLocateFolderType = string.Empty;
        private string IStrParentServerFolder = string.Empty;

        private void CreateDatabaseStep21Next()
        {
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrProfile = IUCConnect2SQLServer.GetSettedData(ref LStrCallReturn);

                if (LListStrProfile.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                IParameterStep21.ObjectSource0 = LListStrProfile;

                ShowWaitProgressBar(true);
                IBackgroundWorkerGetDB2Information = new BackgroundWorker();
                IBackgroundWorkerGetDB2Information.RunWorkerCompleted += IBackgroundWorkerGetDB2Information_RunWorkerCompleted;
                IBackgroundWorkerGetDB2Information.DoWork += IBackgroundWorkerGetDB2Information_DoWork;
                IBackgroundWorkerGetDB2Information.RunWorkerAsync(LListStrProfile);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerGetDB2Information != null)
                {
                    IBackgroundWorkerGetDB2Information.Dispose();
                    IBackgroundWorkerGetDB2Information = null;
                }
            }
        }

        private void IBackgroundWorkerGetDB2Information_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrProfile = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(201, LListStrProfile);
            if (IWCFOperationDataArgs.BoolReturn)
            {
                IParameterStep21.StrErrorMessage = string.Empty;
                IParameterStep21.ObjectSource1 = IWCFOperationDataArgs.ListStringReturn;
                IParameterStep21.ObjectSource2 = IWCFOperationDataArgs.DataSetReturn;
            }
            else { IParameterStep21.StrErrorMessage = IWCFOperationDataArgs.StringReturn; }
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerGetDB2Information_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrServerName = string.Empty;

            try
            {
                ShowWaitProgressBar(false);

                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerName = ((List<string>)(IParameterStep21.ObjectSource0))[0];
                LStrServerName = EncryptionAndDecryption.EncryptDecryptString(LStrServerName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                if (e.Error != null)
                {
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02016"), LStrServerName) + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(IParameterStep21.StrErrorMessage))
                {
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02016"), LStrServerName) + "\n\n" + IParameterStep21.StrErrorMessage, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                CreateDatabaseStep22(false);
            }
            catch { }
            finally
            {
                IBackgroundWorkerGetDB2Information.Dispose();
                IBackgroundWorkerGetDB2Information = null;
            }
        }

        private void CreateDatabaseStep22(bool ABoolIsBack)
        {
            IStrCreateDBCurrentStep = "22";
            ButtonSkip.Visibility = System.Windows.Visibility.Visible;
            ButtonBack.Visibility = System.Windows.Visibility.Visible;
            ButtonSkip.Content = App.GetDisplayCharater("BT0003");
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            IUCDatabaseBasicInformation.IOperationEvent += LUCObjectOperationIOperationEvent;
            if (!ABoolIsBack) { IUCDatabaseBasicInformation.ShowDatabaseInformation(IParameterStep21); }
            GridCreateDatabaseDetail.Children.Clear();
            GridCreateDatabaseDetail.Children.Add(IUCDatabaseBasicInformation);
        }

        private void ShowLocateFolder(OperationEventArgs AEventArgs)
        {
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IBackgroundWorkerLocateServerFolder = new BackgroundWorker();
                List<string> LListStrProfile = IParameterStep21.ObjectSource0 as List<string>;
                foreach (string LStrSingleProfile in LListStrProfile) { LListStrWcfArgs.Add(LStrSingleProfile); }
                IStrLocateFolderType = AEventArgs.ObjectSource0.ToString();
                IStrParentServerFolder = AEventArgs.ObjectSource1.ToString();
                LListStrWcfArgs.Add(IStrParentServerFolder);

                ShowWaitProgressBar(true);
                IBackgroundWorkerLocateServerFolder = new BackgroundWorker();
                IBackgroundWorkerLocateServerFolder.RunWorkerCompleted += IBackgroundWorkerLocateServerFolder_RunWorkerCompleted;
                IBackgroundWorkerLocateServerFolder.DoWork += IBackgroundWorkerLocateServerFolder_DoWork;
                IBackgroundWorkerLocateServerFolder.RunWorkerAsync(LListStrWcfArgs);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerLocateServerFolder != null)
                {
                    IBackgroundWorkerLocateServerFolder.Dispose();
                    IBackgroundWorkerLocateServerFolder = null;
                }
            }
        }

        private void IBackgroundWorkerLocateServerFolder_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrWcfArgs = e.Argument as List<string>;

            e.Result = EnumAvailableMedia(LListStrWcfArgs);
        }

        private void IBackgroundWorkerLocateServerFolder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable LDataTableAvailableMedia = e.Result as DataTable;

            ShowWaitProgressBar(false);

            if (string.IsNullOrEmpty(IStrParentServerFolder))
            {
                IUCLocateServerFolder = new UCLocateServerFolder(IStrLocateFolderType);
                ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                ButtonBack.Visibility = System.Windows.Visibility.Collapsed;
                ButtonSkip.Content = App.GetDisplayCharater("BT0006");
                ButtonNext.Content = App.GetDisplayCharater("BT0007");
                ButtonExit.Content = App.GetDisplayCharater("BT0004");
                IUCLocateServerFolder.IOperationEvent += LUCObjectOperationIOperationEvent;
                GridCreateDatabaseDetail.Children.Clear();
                GridCreateDatabaseDetail.Children.Add(IUCLocateServerFolder);
            }
            IUCLocateServerFolder.ShowSubServerFolderEnd(LDataTableAvailableMedia);
        }

        private DataTable EnumAvailableMedia(List<string> AListStrWcfArgs)
        {
            DataTable LDataTableAvailableMedia = new DataTable();
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            try
            {
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
                LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
                IWCFOperationDataArgs = LService00001Client.OperationMethodA(206, AListStrWcfArgs);
                if (IWCFOperationDataArgs.BoolReturn)
                {
                    LDataTableAvailableMedia = IWCFOperationDataArgs.DataSetReturn.Tables[0];
                }
                else { LDataTableAvailableMedia = null; }
            }
            catch
            {
                LDataTableAvailableMedia = null;
            }
            finally
            {
                if (LService00001Client.State == CommunicationState.Opened)
                {
                    LService00001Client.Close();
                    LService00001Client = null;
                }
            }
            return LDataTableAvailableMedia;
        }

        private void CreateDatabaseStep22Next()
        {
            string LStrCallReturn = string.Empty;
            List<string> LListStrDBProfileAndCreateOptions = new List<string>();

            try
            {
                List<string> LListStrCreateDBOptions = IUCDatabaseBasicInformation.GetSettedData(ref LStrCallReturn);

                if (LListStrCreateDBOptions.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                IParameterStep22.ObjectSource0 = LListStrCreateDBOptions;

                List<string> LListStrDatabaseProfle = IParameterStep21.ObjectSource0 as List<string>;

                foreach (string LStrProfileSingle in LListStrDatabaseProfle) { LListStrDBProfileAndCreateOptions.Add(LStrProfileSingle); }
                LListStrDBProfileAndCreateOptions.Add(App.GStrSpliterChar);
                foreach (string LStrOptionsSingle in LListStrCreateDBOptions) { LListStrDBProfileAndCreateOptions.Add(LStrOptionsSingle); }

                ShowWaitProgressBar(true);
                IBackgroundWorkerCreateDB2Database = new BackgroundWorker();
                IBackgroundWorkerCreateDB2Database.RunWorkerCompleted += IBackgroundWorkerCreateDB2Database_RunWorkerCompleted;
                IBackgroundWorkerCreateDB2Database.DoWork += IBackgroundWorkerCreateDB2Database_DoWork;
                IBackgroundWorkerCreateDB2Database.RunWorkerAsync(LListStrDBProfileAndCreateOptions);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerCreateDB2Database != null)
                {
                    IBackgroundWorkerCreateDB2Database.Dispose();
                    IBackgroundWorkerCreateDB2Database = null;
                }
            }
        }

        private void IBackgroundWorkerCreateDB2Database_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrDBProfileAndCreateOptions = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(202, LListStrDBProfileAndCreateOptions);
            IParameterStep22.ObjectSource1 = IWCFOperationDataArgs.ListStringReturn;
            IParameterStep22.StrErrorMessage = IWCFOperationDataArgs.StringReturn;
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerCreateDB2Database_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrDatabaseName = string.Empty;

            try
            {
                ShowWaitProgressBar(false);
                LStrDatabaseName = ((List<string>)(IParameterStep22.ObjectSource0))[0];

                if (e.Error != null)
                {
                    IParameterStep22.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02035"), LStrDatabaseName) + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(IParameterStep22.StrErrorMessage))
                {
                    IParameterStep22.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02035"), LStrDatabaseName) + "\n\n" + IParameterStep22.StrErrorMessage, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                List<string> LListStrCreateReturn = IParameterStep22.ObjectSource1 as List<string>;
                if (LListStrCreateReturn.Count > 0)
                {
                    IParameterStep22.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater(LListStrCreateReturn[0]), LStrDatabaseName), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CreateDatabaseStep23();
            }
            catch { }
            finally
            {
                IBackgroundWorkerCreateDB2Database.Dispose();
                IBackgroundWorkerCreateDB2Database = null;
            }
        }

        private void CreateDatabaseStep23()
        {
            IStrCreateDBCurrentStep = "23";
            ButtonSkip.Visibility = System.Windows.Visibility.Visible;
            ButtonBack.Visibility = System.Windows.Visibility.Visible;
            ButtonSkip.Content = App.GetDisplayCharater("BT0003");
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            GridCreateDatabaseDetail.Children.Add(IUCCreateLoginAccount);
        }

        private void CreateDatabaseStep23Next()
        {
            string LStrCallReturn = string.Empty;
            List<string> LListStrDBProfileAndCreateOptions = new List<string>();

            try
            {
                List<string> LListStrCreateLoginAccount = IUCCreateLoginAccount.GetSettedData(ref LStrCallReturn);

                if (LListStrCreateLoginAccount.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                if (IParameterStep22.ObjectSource0 != null)
                {
                    List<string> LListStrCreateDBOptions = IParameterStep22.ObjectSource0 as List<string>;
                    LListStrCreateLoginAccount[3] = LListStrCreateDBOptions[0];
                }
                IParameterStep23.ObjectSource0 = LListStrCreateLoginAccount;

                List<string> LListStrDatabaseProfle = IParameterStep21.ObjectSource0 as List<string>;

                foreach (string LStrProfileSingle in LListStrDatabaseProfle) { LListStrDBProfileAndCreateOptions.Add(LStrProfileSingle); }
                LListStrDBProfileAndCreateOptions.Add(App.GStrSpliterChar);
                foreach (string LStrOptionsSingle in LListStrCreateLoginAccount) { LListStrDBProfileAndCreateOptions.Add(LStrOptionsSingle); }

                ShowWaitProgressBar(true);
                IBackgroundWorkerCreateLoginAccount = new BackgroundWorker();
                IBackgroundWorkerCreateLoginAccount.RunWorkerCompleted += IBackgroundWorkerCreateLoginAccount_RunWorkerCompleted;
                IBackgroundWorkerCreateLoginAccount.DoWork += IBackgroundWorkerCreateLoginAccount_DoWork;
                IBackgroundWorkerCreateLoginAccount.RunWorkerAsync(LListStrDBProfileAndCreateOptions);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerCreateLoginAccount != null)
                {
                    IBackgroundWorkerCreateLoginAccount.Dispose();
                    IBackgroundWorkerCreateLoginAccount = null;
                }
            }
        }

        private void IBackgroundWorkerCreateLoginAccount_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrDBProfileAndCreateOptions = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(203, LListStrDBProfileAndCreateOptions);
            IParameterStep23.ObjectSource1 = IWCFOperationDataArgs.ListStringReturn;
            IParameterStep23.StrErrorMessage = IWCFOperationDataArgs.StringReturn;
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerCreateLoginAccount_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrLoginAccount = string.Empty;

            try
            {
                ShowWaitProgressBar(false);
                LStrLoginAccount = ((List<string>)(IParameterStep23.ObjectSource0))[0];

                if (e.Error != null)
                {
                    IParameterStep23.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02043"), LStrLoginAccount) + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(IParameterStep22.StrErrorMessage))
                {
                    IParameterStep23.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater("M02043"), LStrLoginAccount) + "\n\n" + IParameterStep23.StrErrorMessage, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                List<string> LListStrCreateReturn = IParameterStep23.ObjectSource1 as List<string>;
                if (LListStrCreateReturn.Count > 0)
                {
                    IParameterStep23.ObjectSource0 = null;
                    MessageBox.Show(string.Format(App.GetDisplayCharater(LListStrCreateReturn[0]), LStrLoginAccount), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CreateDatabaseStep24(false);
            }
            catch { }
            finally
            {
                IBackgroundWorkerCreateLoginAccount.Dispose();
                IBackgroundWorkerCreateLoginAccount = null;
            }
        }

        private void CreateDatabaseStep24(bool ABoolIsBack)
        {
            IStrCreateDBCurrentStep = "24";
            ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
            ButtonBack.Visibility = System.Windows.Visibility.Visible;
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            IUCConfirmConnectionProfile.IPageParent = this;
            if (!ABoolIsBack) { IUCConfirmConnectionProfile.ShowConfirmInformation((List<string>)(IParameterStep21.ObjectSource0), IParameterStep22, IParameterStep23); }
            GridCreateDatabaseDetail.Children.Add(IUCConfirmConnectionProfile);
        }

        private void CreateDatabaseStep24Next()
        {
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrDatabaseConfirmConnectProfile = IUCConfirmConnectionProfile.GetSettedData(ref LStrCallReturn);
                if (LListStrDatabaseConfirmConnectProfile.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                IParameterStep24.ObjectSource0 = LListStrDatabaseConfirmConnectProfile;
                
                //0-数据库服务器
                //1-端口
                //2-LogingID
                //3-Login Password
                //4-数据库名
                //5-系统管理员LoginID
                //6-系统管理员Login Password
                List<string> LListStrDatabaseProfile = new List<string>(LListStrDatabaseConfirmConnectProfile.ToArray()); //0~4
                List<string> LListStrStep21 = IParameterStep21.ObjectSource0 as List<string>; 
                LListStrDatabaseProfile.Add(LListStrStep21[2]);     //5
                LListStrDatabaseProfile.Add(LListStrStep21[3]);     //6

                ShowWaitProgressBar(true);
                IBackgroundWorkerConfirmConnectDatabase = new BackgroundWorker();
                IBackgroundWorkerConfirmConnectDatabase.RunWorkerCompleted += IBackgroundWorkerConfirmConnectDatabase_RunWorkerCompleted;
                IBackgroundWorkerConfirmConnectDatabase.DoWork += IBackgroundWorkerConfirmConnectDatabase_DoWork;
                IBackgroundWorkerConfirmConnectDatabase.RunWorkerAsync(LListStrDatabaseProfile);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerConfirmConnectDatabase != null)
                {
                    IBackgroundWorkerConfirmConnectDatabase.Dispose();
                    IBackgroundWorkerConfirmConnectDatabase = null;
                }
            }
        }

        private void IBackgroundWorkerConfirmConnectDatabase_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrDatabaseProfile = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(205, LListStrDatabaseProfile);
            IParameterStep24.ObjectSource1 = IWCFOperationDataArgs.ListStringReturn;
            IStrObjectInDBVersion = IWCFOperationDataArgs.ListStringReturn[0];
            IParameterStep24.ObjectSource2 = IWCFOperationDataArgs.DataSetReturn;
            IParameterStep24.StrErrorMessage = IWCFOperationDataArgs.StringReturn;
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerConfirmConnectDatabase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowWaitProgressBar(false);
                if (e.Error != null)
                {
                    IParameterStep24.ObjectSource0 = null;
                    MessageBox.Show(App.GetDisplayCharater("M02053") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IWCFOperationDataArgs.BoolReturn)
                {
                    if (IWCFOperationDataArgs.ListStringReturn.Count > 0)
                    {
                        MessageBox.Show(App.GetDisplayCharater(IWCFOperationDataArgs.ListStringReturn[0]), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(App.GetDisplayCharater("M02053") + "\n\n" + IWCFOperationDataArgs.StringReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                CreateDatabaseStep01(false);
            }
            catch { }
            finally
            {
                IBackgroundWorkerConfirmConnectDatabase.Dispose();
                IBackgroundWorkerConfirmConnectDatabase = null;
            }

        }
        #endregion

        #region 在 Oracle 中创建数据库
        private void CreateDatabaseStep34Next()
        {
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrDatabaseConnectOracle = IUCConnect2Oracle.GetSettedData(ref LStrCallReturn);
                if (LListStrDatabaseConnectOracle.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                IParameterStep34.ObjectSource0 = LListStrDatabaseConnectOracle;

                ShowWaitProgressBar(true);
                IBackgroundWorkerConfirmConnectOracle = new BackgroundWorker();
                IBackgroundWorkerConfirmConnectOracle.RunWorkerCompleted += IBackgroundWorkerConfirmConnectOracle_RunWorkerCompleted;
                IBackgroundWorkerConfirmConnectOracle.DoWork += IBackgroundWorkerConfirmConnectOracle_DoWork;
                IBackgroundWorkerConfirmConnectOracle.RunWorkerAsync(LListStrDatabaseConnectOracle);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerConfirmConnectOracle != null)
                {
                    IBackgroundWorkerConfirmConnectOracle.Dispose();
                    IBackgroundWorkerConfirmConnectOracle = null;
                }
            }
        }

        private void IBackgroundWorkerConfirmConnectOracle_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrDatabaseProfile = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(305, LListStrDatabaseProfile);
            if (IWCFOperationDataArgs.BoolReturn)
            {
                IParameterStep34.ObjectSource1 = IWCFOperationDataArgs.ListStringReturn;
                IStrObjectInDBVersion = IWCFOperationDataArgs.ListStringReturn[0];
                IParameterStep34.ObjectSource2 = IWCFOperationDataArgs.DataSetReturn;
                IParameterStep34.StrErrorMessage = IWCFOperationDataArgs.StringReturn;
            }
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerConfirmConnectOracle_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowWaitProgressBar(false);
                if (e.Error != null)
                {
                    IParameterStep34.ObjectSource0 = null;
                    MessageBox.Show(App.GetDisplayCharater("M02053") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IWCFOperationDataArgs.BoolReturn)
                {
                    if (IWCFOperationDataArgs.ListStringReturn.Count > 0)
                    {
                        MessageBox.Show(App.GetDisplayCharater(IWCFOperationDataArgs.ListStringReturn[0]), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(App.GetDisplayCharater("M02053") + "\n1\n" + IWCFOperationDataArgs.StringReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                CreateDatabaseStep01(false);
            }
            catch { }
            finally
            {
                IBackgroundWorkerConfirmConnectOracle.Dispose();
                IBackgroundWorkerConfirmConnectOracle = null;
            }
        }
        #endregion

        #region 创建数据库 - 公用2
        private void CreateDatabaseStep01(bool ABoolIsBack)
        {
            IStrCreateDBCurrentStep = "01";
            ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
            ButtonBack.Visibility = System.Windows.Visibility.Visible;
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            IUCBasicRentUserData.IPageParent = this;
            if (!ABoolIsBack) { IUCBasicRentUserData.ObtainNeededData(); }
            GridCreateDatabaseDetail.Children.Add(IUCBasicRentUserData);
        }

        private void CreateDatabaseStep01Next()
        {
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrRentDataSetted = IUCBasicRentUserData.GetSettedData(ref LStrCallReturn);
                if (LListStrRentDataSetted.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }
                IParameterStep01.ObjectSource0 = LListStrRentDataSetted;

                List<string> LListStrWcfArguments = new List<string>();
                LListStrWcfArguments.Add(IStrDatabaseType);
                LListStrWcfArguments.Add(IStrObjectInDBVersion);

                ShowWaitProgressBar(true);
                IBackgroundWorkerObtainCreateObjects = new BackgroundWorker();
                IBackgroundWorkerObtainCreateObjects.RunWorkerCompleted += IBackgroundWorkerObtainCreateObjects_RunWorkerCompleted;
                IBackgroundWorkerObtainCreateObjects.DoWork += IBackgroundWorkerObtainCreateObjects_DoWork;
                IBackgroundWorkerObtainCreateObjects.RunWorkerAsync(LListStrWcfArguments);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerObtainCreateObjects != null)
                {
                    IBackgroundWorkerObtainCreateObjects.Dispose();
                    IBackgroundWorkerObtainCreateObjects = null;
                }
            }
        }

        private void IBackgroundWorkerObtainCreateObjects_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrWcfArguments = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(11, LListStrWcfArguments);
            IParameterStep01.ObjectSource2 = IWCFOperationDataArgs.DataSetReturn;
            IParameterStep01.StrErrorMessage = IWCFOperationDataArgs.StringReturn;
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerObtainCreateObjects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowWaitProgressBar(false);
                if (e.Error != null)
                {
                    IParameterStep24.ObjectSource0 = null;
                    MessageBox.Show(App.GetDisplayCharater("M02066") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IWCFOperationDataArgs.BoolReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02066") + "\n\n" + IWCFOperationDataArgs.StringReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                CreateDatabaseStep02(false);
            }
            catch { }
            finally
            {
                IBackgroundWorkerObtainCreateObjects.Dispose();
                IBackgroundWorkerObtainCreateObjects = null;
            }
        }

        private void CreateDatabaseStep02(bool ABoolIsBack)
        {
            DataTable LDataTableCreateObjects = (IParameterStep01.ObjectSource2 as DataSet).Tables[0];
            IStrCreateDBCurrentStep = "02";
            if (!ABoolIsBack)
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
                ButtonBack.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                ButtonBack.Visibility = System.Windows.Visibility.Collapsed;
            }
            ButtonSkip.Content = App.GetDisplayCharater("BT0003");
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            IUCCreateDatabaseObject.IPageParent = this;
            if (!ABoolIsBack) { IUCCreateDatabaseObject.ShowWillCreateObjects(LDataTableCreateObjects); }
            else { IUCCreateDatabaseObject.IIntCurrentObject = 0; }
            GridCreateDatabaseDetail.Children.Add(IUCCreateDatabaseObject);
        }

        private void CreateDatabaseStep02Next()
        {
            string LStrObjectIsCreated = string.Empty;
            List<string> LListStrWcfArguments = new List<string>();
            List<string> LListStrDatabaseProfile = new List<string>();

            try
            {
                ShowWaitProgressBar(false);
                if (IStrDatabaseType == "2")
                {
                    LListStrDatabaseProfile = IParameterStep24.ObjectSource0 as List<string>;
                }
                if (IStrDatabaseType == "3")
                {
                    LListStrDatabaseProfile = IParameterStep34.ObjectSource0 as List<string>;
                }
                List<string> LListStrRentData = IParameterStep01.ObjectSource0 as List<string>;

                DataRow LDataRowCreateObject = IUCCreateDatabaseObject.GetCreateObject(ref LStrObjectIsCreated);
                if (LDataRowCreateObject == null)
                {
                    CreateDatabaseStep03(false);
                    return;
                }

                IStrObjectCreatedVersion = LDataRowCreateObject["C004"].ToString();

                IUCCreateDatabaseObject.SetObjectCurrentAction("2", string.Empty);

                ShowWaitProgressBar(true);

                LListStrWcfArguments.Add(LStrObjectIsCreated);                      //0
                LListStrWcfArguments.Add(IStrDatabaseType);                         //1
                LListStrWcfArguments.Add(LListStrDatabaseProfile[0]);               //2
                LListStrWcfArguments.Add(LListStrDatabaseProfile[1]);               //3
                LListStrWcfArguments.Add(LListStrDatabaseProfile[2]);               //4
                LListStrWcfArguments.Add(LListStrDatabaseProfile[3]);               //5
                LListStrWcfArguments.Add(LListStrDatabaseProfile[4]);               //6
                LListStrWcfArguments.Add(LDataRowCreateObject["C002"].ToString());  //7
                LListStrWcfArguments.Add(LDataRowCreateObject["C003"].ToString());  //8
                LListStrWcfArguments.Add(LListStrRentData[1]);                      //9

                IBackgroundWorkerCreateDatabaseObjects = new BackgroundWorker();
                IBackgroundWorkerCreateDatabaseObjects.RunWorkerCompleted += IBackgroundWorkerCreateDatabaseObjects_RunWorkerCompleted;
                IBackgroundWorkerCreateDatabaseObjects.DoWork += IBackgroundWorkerCreateDatabaseObjects_DoWork;
                IBackgroundWorkerCreateDatabaseObjects.RunWorkerAsync(LListStrWcfArguments);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerCreateDatabaseObjects != null)
                {
                    IBackgroundWorkerCreateDatabaseObjects.Dispose();
                    IBackgroundWorkerCreateDatabaseObjects = null;
                }
            }
        }

        private void IBackgroundWorkerCreateDatabaseObjects_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrWcfArguments = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(12, LListStrWcfArguments);
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerCreateDatabaseObjects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (IWCFOperationDataArgs.BoolReturn)
                {
                    if (IWCFOperationDataArgs.StringReturn == App.GStrSpliterChar)
                    {
                        IUCCreateDatabaseObject.SetObjectCurrentAction(App.GStrSpliterChar, IWCFOperationDataArgs.StringReturn);
                    }
                    else
                    {
                        IUCCreateDatabaseObject.SetObjectCurrentAction("1", IWCFOperationDataArgs.StringReturn);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(IWCFOperationDataArgs.StringReturn))
                    {
                        IUCCreateDatabaseObject.SetObjectCurrentAction("1", "?");
                    }
                    else
                    {
                        IUCCreateDatabaseObject.SetObjectCurrentAction("0", IWCFOperationDataArgs.StringReturn);
                        //MessageBox.Show(IWCFOperationDataArgs.StringReturn);
                    }
                }
                IBackgroundWorkerCreateDatabaseObjects.Dispose();
                IBackgroundWorkerCreateDatabaseObjects = null;
                IWCFOperationDataArgs = null;
                CreateDatabaseStep02Next();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Exception"); }
            
        }

        private void CreateDatabaseStep03(bool ABoolIsBack)
        {
            IStrCreateDBCurrentStep = "03";

            if (!ABoolIsBack)
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
                ButtonBack.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                ButtonBack.Visibility = System.Windows.Visibility.Collapsed;
            }
            ButtonSkip.Content = App.GetDisplayCharater("BT0003");
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0001");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            GridCreateDatabaseDetail.Children.Clear();
            IUCInitTableData.IPageParent = this;
            if (!ABoolIsBack) { IUCInitTableData.ObtainInitTablesData(IStrObjectInDBVersion); }
            GridCreateDatabaseDetail.Children.Add(IUCInitTableData);
        }

        private void CreateDatabaseStep03Next()
        {
            string LStrObjectIsInited = string.Empty;
            List<string> LListStrWcfArguments = new List<string>();
            List<string> LListStrDatabaseProfile = new List<string>();

            try
            {
                ShowWaitProgressBar(false);
                if (IStrDatabaseType == "2")
                {
                    LListStrDatabaseProfile = IParameterStep24.ObjectSource0 as List<string>;
                }
                if (IStrDatabaseType == "3")
                {
                    LListStrDatabaseProfile = IParameterStep34.ObjectSource0 as List<string>;
                }

                List<string> LListStrRentData = IParameterStep01.ObjectSource0 as List<string>;

                DataRow LDataRowInitDataObject = IUCInitTableData.GetInitObject(ref LStrObjectIsInited);
                if (LDataRowInitDataObject == null)
                {
                    
                    CreateDatabaseStep04(false);
                    return;
                }

                IUCInitTableData.SetObjectCurrentAction("2", "", "");

                ShowWaitProgressBar(true);

                LListStrWcfArguments.Add(LStrObjectIsInited);
                LListStrWcfArguments.Add(IStrDatabaseType);
                LListStrWcfArguments.Add(LListStrDatabaseProfile[0]);
                LListStrWcfArguments.Add(LListStrDatabaseProfile[1]);
                LListStrWcfArguments.Add(LListStrDatabaseProfile[2]);
                LListStrWcfArguments.Add(LListStrDatabaseProfile[3]);
                LListStrWcfArguments.Add(LListStrDatabaseProfile[4]);
                LListStrWcfArguments.Add("");
                LListStrWcfArguments.Add(LDataRowInitDataObject["C002"].ToString());
                LListStrWcfArguments.Add(LListStrRentData[1]);

                IBackgroundWorkerInitDatabaseObjects = new BackgroundWorker();
                IBackgroundWorkerInitDatabaseObjects.RunWorkerCompleted += IBackgroundWorkerInitDatabaseObjects_RunWorkerCompleted;
                IBackgroundWorkerInitDatabaseObjects.DoWork += IBackgroundWorkerInitDatabaseObjects_DoWork;
                IBackgroundWorkerInitDatabaseObjects.RunWorkerAsync(LListStrWcfArguments);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerInitDatabaseObjects != null)
                {
                    IBackgroundWorkerInitDatabaseObjects.Dispose();
                    IBackgroundWorkerInitDatabaseObjects = null;
                }
            }
        }

        private void IBackgroundWorkerInitDatabaseObjects_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            Service00001Client LService00001Client = null;

            List<string> LListStrWcfArguments = e.Argument as List<string>;
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(14, LListStrWcfArguments);
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerInitDatabaseObjects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (IWCFOperationDataArgs.BoolReturn)
                {
                    if (IWCFOperationDataArgs.StringReturn == App.GStrSpliterChar)
                    {
                        IUCInitTableData.SetObjectCurrentAction(App.GStrSpliterChar, IWCFOperationDataArgs.StringReturn, string.Empty);
                    }
                    else
                    {
                        IUCInitTableData.SetObjectCurrentAction("1", string.Empty, IWCFOperationDataArgs.StringReturn);
                    }
                }
                else
                {
                    IUCInitTableData.SetObjectCurrentAction("0", IWCFOperationDataArgs.StringReturn, string.Empty);
                    MessageBox.Show(IWCFOperationDataArgs.StringReturn);
                }
                IBackgroundWorkerInitDatabaseObjects.Dispose();
                IBackgroundWorkerInitDatabaseObjects = null;
                CreateDatabaseStep03Next();
            }
            catch { }
        }

        private void CreateDatabaseStep04(bool ABoolIsBack)
        {
            List<string> LListStrDatabaseProfile = new List<string>();

            IStrCreateDBCurrentStep = "04";

            if (!ABoolIsBack)
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Collapsed;
                ButtonBack.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ButtonSkip.Visibility = System.Windows.Visibility.Visible;
                ButtonBack.Visibility = System.Windows.Visibility.Collapsed;
            }
            ButtonSkip.Content = App.GetDisplayCharater("BT0003");
            ButtonBack.Content = App.GetDisplayCharater("BT0002");
            ButtonNext.Content = App.GetDisplayCharater("BT0005");
            ButtonExit.Content = App.GetDisplayCharater("BT0004");

            if (IStrDatabaseType == "2")
            {
                LListStrDatabaseProfile = IParameterStep24.ObjectSource0 as List<string>;
            }
            if (IStrDatabaseType == "3")
            {
                LListStrDatabaseProfile = IParameterStep34.ObjectSource0 as List<string>;
            }

            GridCreateDatabaseDetail.Children.Clear();
            IUCDatabaseConnectionProfile.IPageParent = this;
            IUCDatabaseConnectionProfile.InitCreatedDatabaseInformation(LListStrDatabaseProfile);
            GridCreateDatabaseDetail.Children.Add(IUCDatabaseConnectionProfile);
        }

        private void CreateDatabaseStep04Next()
        {
            List<List<string>> LListListStrWcfArguments = new List<List<string>>();
            List<string> LListStrDatabaseProfile = new List<string>();

            try
            {
                if (IStrDatabaseType == "2")
                {
                    LListStrDatabaseProfile = new List<string>((IParameterStep24.ObjectSource0 as List<string>).ToArray());
                }
                if (IStrDatabaseType == "3")
                {
                    LListStrDatabaseProfile = new List<string>((IParameterStep34.ObjectSource0 as List<string>).ToArray());
                }

                LListStrDatabaseProfile.Insert(0, IStrDatabaseType);
                List<string> LListStrRentData = IParameterStep01.ObjectSource0 as List<string>;
                LListListStrWcfArguments.Add(LListStrDatabaseProfile);
                LListListStrWcfArguments.Add(LListStrRentData);

                ShowWaitProgressBar(true);
                
                IBackgroundWorkerFinishCreateDatabase = new BackgroundWorker();
                IBackgroundWorkerFinishCreateDatabase.RunWorkerCompleted += IBackgroundWorkerFinishCreateDatabase_RunWorkerCompleted;
                IBackgroundWorkerFinishCreateDatabase.DoWork += IBackgroundWorkerFinishCreateDatabase_DoWork;
                IBackgroundWorkerFinishCreateDatabase.RunWorkerAsync(LListListStrWcfArguments);
            }
            catch
            {
                ShowWaitProgressBar(false);
                if (IBackgroundWorkerFinishCreateDatabase != null)
                {
                    IBackgroundWorkerFinishCreateDatabase.Dispose();
                    IBackgroundWorkerFinishCreateDatabase = null;
                }
            }
        }

        private void IBackgroundWorkerFinishCreateDatabase_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00001Client LService00001Client = null;

            List<List<string>> LListListStrWcfArguments = e.Argument as List<List<string>>;
            List<string> LListStrMethod15 = new List<string>();
            List<string> LListStrMethod90 = new List<string>();

            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);

            foreach (string LStrSingleArg in LListListStrWcfArguments[0]) { LListStrMethod15.Add(LStrSingleArg); LListStrMethod90.Add(LStrSingleArg); }
            foreach (string LStrSingleArg in LListListStrWcfArguments[1]) { LListStrMethod15.Add(LStrSingleArg); }

            IWCFOperationDataArgs = new OperationDataArgs();
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(15, LListStrMethod15);
            if (!IWCFOperationDataArgs.BoolReturn) { return; }

            IWCFOperationDataArgs = null;
            IWCFOperationDataArgs = new OperationDataArgs();
            LListStrMethod90.Add(IStrObjectCreatedVersion);
            IWCFOperationDataArgs = LService00001Client.OperationMethodA(90, LListStrMethod90);

            LService00001Client.Close();
            LService00001Client = null;

        }

        private void IBackgroundWorkerFinishCreateDatabase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowWaitProgressBar(false);
                if (e.Error != null)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02082") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IWCFOperationDataArgs.BoolReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02082") + "\n\n" + IWCFOperationDataArgs.StringReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show(App.GetDisplayCharater("M02083"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
                IntPtr ieHwnd = GetAncestor(wih.Handle, 2);
                PostMessage(ieHwnd, 0x10, IntPtr.Zero, IntPtr.Zero);
                Application.Current.Shutdown();
            }
            catch { }
            finally
            {
                IBackgroundWorkerFinishCreateDatabase.Dispose();
                IBackgroundWorkerFinishCreateDatabase = null;
            }
        }
        #endregion

        #region 显示/关闭 Wait Progress Bar
        public void ShowWaitProgressBar(bool ABoolShow)
        {
            if (ABoolShow)
            {
                IBoolIsBusy = true;
                WaitPorgressBar LWaitPorgressBar = new WaitPorgressBar();
                GridShowCurrentStatus.Children.Add(LWaitPorgressBar);
                LWaitPorgressBar.StartAnimation();
            }
            else
            {
                GridShowCurrentStatus.Children.Clear();
                IBoolIsBusy = false;
            }
        }
        #endregion
    }
}
