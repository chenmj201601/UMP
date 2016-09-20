using Common2400;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS2400.Service11012;
using UMPS2400.Service24011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using UMPS2400.Entries;
using System.Data;
using UMPS2400.ChildUCs;
using System.ComponentModel;
using System.Threading;

namespace UMPS2400.MainUserControls
{
    /// <summary>
    /// EncryptServersManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptServersManagement
    {
        #region 变量定义
        //listview需要显示的列的集合
        private ObservableCollection<ViewColumnInfo> mListColumnItems;
        //listview要显示的数据ViewID
        private string mViewID;
        //用户操作列表
        public static ObservableCollection<OperationInfo> ListOperationsOnlyAdd = new ObservableCollection<OperationInfo>();
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();
        public static ObservableCollection<KeyGenServerEntryInList> lstAllKeyGenServers = new ObservableCollection<KeyGenServerEntryInList>();
        //“正在使用”的
        //BackgroundWorker 用来做添加或修改的wcf交互 
        BackgroundWorker mBackgroundWorker;

        //父窗口
        public EncryptMainPage parentWin = null;

        bool boLoaded = false;
        BackgroundWorker bgwCheckStatus = null;
        #endregion

        public UC_EncryptServersManagement()
        {
            InitializeComponent();
            Loaded += UC_EncryptServersManagement_Loaded;
            lvGeneratorObject.SelectionChanged += lvGeneratorObject_SelectionChanged;
        }

        #region listview选中行变化事件
        void lvGeneratorObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateOptButtons(AllListOperations);
        }

        #endregion

        public void ShowStausMessage(string strMsg, bool bIsShow)
        {
            parentWin.ShowStausMessage(strMsg, bIsShow);
        }

        void UC_EncryptServersManagement_Loaded(object sender, RoutedEventArgs e)
        {
            mBackgroundWorker = null;
            bgwCheckStatus = null;
            boLoaded = false;
            CurrentApp.WriteLog("UC_EncryptServersManagement_Loaded , boLoaded = " + boLoaded.ToString());

            try
            {
                CurrentApp.WriteLog("UC_EncryptServersManagement_Loaded");
                LbOperations.Text = CurrentApp.GetLanguageInfo("2401L001", "Operations");
                LbCurrentObject.Text = CurrentApp.GetLanguageInfo("2401L004", "Encryption Management->Key generation service management");

                lvGeneratorObject.ItemsSource = lstAllKeyGenServers;
                parentWin.ShowStausMessage(CurrentApp.GetLanguageInfo("2401007", "Loading") + "....", true);
                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) =>
                {
                    //获得需要显示的列
                    LoadViewColumnItems();
                    //获得用户可使用的按钮(操作权限)
                    GetUserOpts();
                };
                mBackgroundWorker.RunWorkerCompleted += (s, de) =>
                    {
                        parentWin.ShowStausMessage(string.Empty, false);
                        //  CreateColumnsItems();
                        //获得需要显示的数据
                        InitKeyGenServerList();
                        CreateOptButtons(ListOperationsOnlyAdd);
                        boLoaded = true;
                        ChangeLanguage();
                        CurrentApp.WriteLog("mBackgroundWorker done");
                    };
                mBackgroundWorker.RunWorkerAsync();
                bgwCheckStatus = new BackgroundWorker();
                bgwCheckStatus.DoWork += (s, de) =>
                    {
                        while (true)
                        {
                            if (!boLoaded)
                            {
                                Thread.Sleep(1000);
                                continue;
                            }
                            CurrentApp.WriteLog("bgwCheckStatus ,boLoaded=true");
                            string strTip = CurrentApp.GetLanguageInfo("2401024", "Checking service status... .");
                            Dispatcher.Invoke(new Action(() =>
                                parentWin.ShowStausMessage(strTip, true)
                                ));
                            InitMachineStatus();
                            break;
                        }
                    };
                bgwCheckStatus.RunWorkerCompleted += (s, de) =>
                    {
                        CurrentApp.WriteLog("bgwCheckStatus done");
                        parentWin.ShowStausMessage(string.Empty, false);
                        bgwCheckStatus.Dispose();
                    };
                bgwCheckStatus.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("UC_EncryptServersManagement_Loaded error:" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void InitMachineStatus()
        {
            for (int i = 0; i < lstAllKeyGenServers.Count; i++)
            {
                KeyGenServerEntryInList server = lstAllKeyGenServers[i];
                server.Status = CheckServerStatus(server.HostAddress, server.HostPort);
            }
        }

        public override void ChangeLanguage()
        {
            CurrentApp.WriteLog("UC_EncryptServersManagement ChangeLanguage");
            base.ChangeLanguage();
            #region 更新当前ursercontrol中控件的显示文字
            //更新listview种列的显示文字
            CreateColumnsItems();
            #endregion
            LbOperations.Text = CurrentApp.GetLanguageInfo("2401L001", "Operations");
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("2401L004", "Encryption Management->Key generation service management");
            #region 循环修改操作按钮的显示文字
            Button btn;
            OperationInfo optInfo;
            ObservableCollection<OperationInfo> listOptInfos = new ObservableCollection<OperationInfo>();
            for (int i = 0; i < PanelOperationButtons.Children.Count; i++)
            {
                try
                {
                    btn = PanelOperationButtons.Children[i] as Button;
                    optInfo = btn.DataContext as OperationInfo;
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    listOptInfos.Add(optInfo);
                }
                catch
                {
                    continue;
                }
            }
            CreateOptButtons(listOptInfos);
            #endregion

            PopupPanel.ChangeLanguage();
        }

        #region 绑定ListView
        /// <summary>
        /// 获得需要显示的列
        /// </summary>
        private void LoadViewColumnItems()
        {
            mViewID = "2401001";
            mListColumnItems = new ObservableCollection<ViewColumnInfo>();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(mViewID);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListColumnItems.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListColumnItems.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 创建listview显示的列
        /// </summary>
        private void CreateColumnsItems()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListColumnItems.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListColumnItems[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        string str = CurrentApp.Session.LangTypeID.ToString();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "EnableIcon")
                        {
                            dt = Resources["EnableIconCellTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "StatusIcon")
                        {
                            dt = Resources["StatusIconCellTemplate"] as DataTemplate;
                        }
                        if (dt == null)
                        {
                            string strColName = columnInfo.ColumnName;
                            gvc.DisplayMemberBinding = new Binding(strColName);
                        }
                        else
                        {
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                int iCount = gv.Columns.Count;
                lvGeneratorObject.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                CurrentApp.WriteLog("UC_EncryptServersManagement CreateColumnsItems error : "+ex.Message);
            }
        }

        /// <summary>
        /// 初始化密钥生成服务器列表
        /// </summary>
        public void InitKeyGenServerList()
        {
            try
            {
                lstAllKeyGenServers.Clear();
                WebReturn webReturn = GetKeyGenServers();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.DataSetData == null)
                {
                    return;
                }
                if (webReturn.DataSetData.Tables.Count <= 0)
                {
                    return;
                }
                int iServerCount = webReturn.DataSetData.Tables[0].Rows.Count;

                KeyGenServerEntryInList server = null;
                DataRow row = null;
                for (int i = 0; i < iServerCount; i++)
                {
                    row = webReturn.DataSetData.Tables[0].Rows[i];
                    server = new KeyGenServerEntryInList();
                    server.ResourceID = row["C001"].ToString();
                    string strC002 = row["C002"].ToString();
                    server.HostAddress = S2400EncryptOperation.DecryptFromDB(strC002);
                    server.HostPort = row["C003"].ToString();
                    server.IsEnable = row["C006"].ToString();
                    server.EnableIcon = row["C006"].ToString() == "1" ? "Images/00002.ico" : "Images/00001.ico";
                    server.Status = false;
                    //  server.StatusIcon = "Images/00001.ico";
                    lstAllKeyGenServers.Add(server);
                    if (i % 2 == 1)
                    {
                        server.Background = Brushes.LightGray;
                    }
                    else
                    {
                        server.Background = Brushes.Transparent;
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("UC_EncryptServersManagement InitKeyGenServerList Error:" + ex.Message);
                ShowException(ex.Message);
            }
        }

        /// <summary>
        ///检查密钥生成服务器是否能连上 
        /// </summary>
        private bool CheckServerStatus(string strHost, string strPort)
        {
            bool boReturn = false;
            try
            {
                //string strStatusMsg = CurrentApp.GetLanguageInfo("2401008", "Trying to connect the server {0} ...");
                //string strConnFailed = CurrentApp.GetLanguageInfo("2401010", "Server {0} connection failed ");
                //string strConnSuccess = CurrentApp.GetLanguageInfo("2401009", "Server {0} connection success ");
                //string strDone = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.TryConnToKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(strHost); ;
                webRequest.ListData.Add(strPort);
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    boReturn = false;
                }
                else
                {
                    boReturn = true;
                }
            }
            catch
            {
                boReturn = false;
            }
            return boReturn;
        }

        public WebReturn GetKeyGenServers()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S2400RequestCode.GetKeyGenServerList;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
            Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            return webReturn;
        }
        #endregion

        #region 获得用户可使用的按钮(操作权限)
        private void GetUserOpts()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)RequestCode.WSGetUserOptList;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
            webRequest.ListData.Add("24");
            webRequest.ListData.Add("2401");
            Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                 WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            ListOperationsOnlyAdd.Clear();
            AllListOperations.Clear();
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                OperationInfo optInfo = optReturn.Data as OperationInfo;
                if (optInfo != null)
                {
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                    AllListOperations.Add(optInfo);
                    if (optInfo.ID == S2400Const.OPT_KeyGenServerAdd)
                    {
                        ListOperationsOnlyAdd.Add(optInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 创建操作的按钮(仅显示基本操作--增加等)
        /// </summary>
        private void CreateOptButtons(ObservableCollection<OperationInfo> InOpts)
        {
            PanelOperationButtons.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < InOpts.Count; i++)
            {
                item = InOpts[i];

                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOperationButtons.Children.Add(btn);

                //如果是“禁用“  就先创建按钮  在按钮下加一条线 
                if (item.ID == S2400Const.OPT_KeyGenServerDisable)
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Background = Brushes.LightGray;
                    txtBlock.Height = 2;
                    txtBlock.Margin = new Thickness(0, 10, 0, 10);
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    PanelOperationButtons.Children.Add(txtBlock);
                }
            }
        }
        #endregion

        #region 操作按钮事件
        /// <summary>
        /// 操作按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                UC_AddKeyGenServer uc_Add=null;
                switch (optItem.ID)
                {
                    case S2400Const.OPT_KeyGenServerDisable:
                        //禁用
                        DisableKeyGenServer();
                        break;
                    case S2400Const.OPT_KeyGenServerEnable:
                        //启用
                        EnableKeyGenServer();
                        break;
                    case S2400Const.OPT_KeyGenServerAdd:
                        //打开添加密钥生成服务器窗口
                        uc_Add = new UC_AddKeyGenServer();
                        uc_Add.CurrentApp = CurrentApp;
                        uc_Add.ChangeLanguage();
                        uc_Add.mainPage = this;
                        uc_Add.iAddOrModify = (int)OperationType.Add;
                        PopupPanel.Content = uc_Add;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("2401T001", "Add key generator server");
                        PopupPanel.IsOpen = true;
                        break;
                    case S2400Const.OPT_KeyGenServerModify:
                        uc_Add = new UC_AddKeyGenServer();
                        uc_Add.CurrentApp = CurrentApp;
                        uc_Add.ChangeLanguage();
                        uc_Add.mainPage = this;
                        uc_Add.iAddOrModify = (int)OperationType.Modify;
                        KeyGenServerEntryInList item = lvGeneratorObject.SelectedItem as KeyGenServerEntryInList;
                        uc_Add.keyGenServerModifying = item;
                        PopupPanel.Content = uc_Add;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("2401T002", "Modify key generation server");
                        PopupPanel.IsOpen = true;
                        break;
                    case S2400Const.OPT_KeyGenServerDelete:
                        DeleteKeyGenServer();
                        break;
                }
            }
        }

        /// <summary>
        /// 禁用密钥生成服务器
        /// </summary>
        private void DisableKeyGenServer()
        {
            //弹出确认禁用窗口
            KeyGenServerEntryInList item = lvGeneratorObject.SelectedItem as KeyGenServerEntryInList;
            if (item.IsEnable == "0")
            {
                string ErrorMsg = CurrentApp.GetLanguageInfo("2401018", "The item is disabled or not connected to the connection, please check it and try again.");
                ShowException(ErrorMsg);
                return;
            }

            //List<KeyGenServerEntry> lstServers = lstAllKeyGenServers.Where(p => p.IsEnable == "1" && p.Status == true).ToList();
            //if (lstServers.Count <= 0)
            //{
            //    string ErrorMsg = CurrentApp.GetLanguageInfo("2401018", "The item is disabled or not connected to the connection, please check it and try again.");
            //    ShowException(ErrorMsg);
            //    return;
            //}
            //
            string strConfirm = CurrentApp.GetLanguageInfo("2401013", "Confirm");
            string strPopupMsg = string.Empty;
            //if (lstServers.Count == 1 && lstServers.First().ResourceID == item.ResourceID)
            //{
            //    strPopupMsg = CurrentApp.GetLanguageInfo("2401012", "There will be no available key generation service if you disabled, are you sure to do it?");
            //}
            //else
            //{
            strPopupMsg = CurrentApp.GetLanguageInfo("2401017", "Are you sure to do this?");
            //}
            MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(item.HostAddress);
                lstParams.Add(item.HostPort);
                int iOptType = (int)OperationType.Disable;
                lstParams.Add(iOptType.ToString());
                lstParams.Add(item.ResourceID);
                EnableDisableKeyGenServer(lstParams);
            }
        }

        /// <summary>
        /// 启用或禁用密钥生成服务
        /// </summary>
        /// lstParams[0] : 服务器IP</param>
        /// lstParams[1] :端口 </param>
        /// lstParams[2] :  启用或禁用</param>
        /// lstParams[3] : 资源ID</param>
        /// 以下是启用某服务器时 需要禁用的另一服务器的参数(预留)
        /// lstParams[4] : 要被禁用的resourceID
        /// lstParams[5] : 要被禁用的HostAddress
        /// lstParams[6] : 要被禁用的Port
        private void EnableDisableKeyGenServer(List<string> lstParams)
        {
            string strStatusMsg = string.Empty;
            int optType = 0;
            int.TryParse(lstParams[2], out optType);
            if (optType == (int)OperationType.Disable)
            {
                strStatusMsg = CurrentApp.GetLanguageInfo("2401014", "Disabling Key generation services, please wait ...");
            }
            else
            {
                strStatusMsg = CurrentApp.GetLanguageInfo("2401016", "Enabling Key generation services, please wait ...");
            }
            parentWin.ShowStausMessage(strStatusMsg, true);
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S2400RequestCode.EnableDisableKeyGenServer;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData = lstParams;
            Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                if (webReturn.Code == Defines.RET_FAIL)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                else if (webReturn.Code == (int)S2400WcfErrorCode.EnableDisableKeyGenServerFailed)
                {
                    string strErrorMsg = CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message);
                    strErrorMsg = string.Format(strErrorMsg, lstParams[0], webReturn.Data);
                    ShowException(strErrorMsg);
                }
                else
                {
                    string strErrorMsg = CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message);
                    ShowException(strErrorMsg);
                }

                string msg = string.Empty;
                if (optType == (int)OperationType.Disable)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO2401002"), lstParams[0]);
                    CurrentApp.WriteOperationLog("2401002", ConstValue.OPT_RESULT_FAIL, msg);
                }
                else
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO2401001"), lstParams[0]);
                    CurrentApp.WriteOperationLog("2401001", ConstValue.OPT_RESULT_FAIL, msg);
                }

                parentWin.ShowStausMessage(string.Empty, false);
                return;
            }
            else
            {
                string msg = string.Empty;
                if (optType == (int)OperationType.Disable)
                {
                    string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO2401002"),lstParams[0]);
                    CurrentApp.WriteOperationLog("2401002", ConstValue.OPT_RESULT_SUCCESS, msg);
                }
                else
                {
                    string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO2401001"),lstParams[0]);
                    CurrentApp.WriteOperationLog("2401001", ConstValue.OPT_RESULT_SUCCESS, msg);
                }
                
                foreach (KeyGenServerEntryInList ser in lstAllKeyGenServers)
                {
                    if (ser.ResourceID == lstParams[3])
                    {
                        switch (optType)
                        {
                            case (int)OperationType.Disable:
                                ser.IsEnable = "0";
                                ser.EnableIcon = "Images/00001.ico";
                                break;
                            case (int)OperationType.Enable:
                                ser.IsEnable = "1";
                                ser.EnableIcon = "Images/00002.ico";
                                break;
                        }
                    }
                }
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2401015", "Success"));
            }
            parentWin.ShowStausMessage(string.Empty, false);
        }

        /// <summary>
        /// 启用密钥生成服务器
        /// </summary>
        private void EnableKeyGenServer()
        {
            //判断当前选中项是否是被禁用状态
            KeyGenServerEntryInList item = lvGeneratorObject.SelectedItem as KeyGenServerEntryInList;
            if (item.IsEnable == "1")
            {
                ShowException(CurrentApp.GetLanguageInfo("2401020", "The current item is enabled"));
                return;
            }
            //判断当前是否有已经启用的密钥生成服务
            List<KeyGenServerEntryInList> lstServers = lstAllKeyGenServers.Where(p => p.IsEnable == "1").ToList();
            //如果当前可用的  则弹出提示告知另一服务正在使用中 如果启用当前项 将会导致其他启用项被禁用
            string strPopupMsg = string.Empty;
            string strConfirm = CurrentApp.GetLanguageInfo("2401013", "Confirm");
            //if (lstServers.Count > 0)
            //{
            //    strPopupMsg = CurrentApp.GetLanguageInfo("2401019", "Since the system has only one key generation service to be used, if the service is enabled, the current use of the service will be disabled, you sure you want to do it?");
            //}
            //else
            //{
            strPopupMsg = CurrentApp.GetLanguageInfo("2401021", "Are you sure to enable this service?");
            //}
            MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(item.HostAddress);
                lstParams.Add(item.HostPort);
                int iOptType = (int)OperationType.Enable;
                lstParams.Add(iOptType.ToString());
                lstParams.Add(item.ResourceID);
                //if (lstServers.Count > 0)
                //{
                //    lstParams.Add(lstServers.First().ResourceID);
                //}
                EnableDisableKeyGenServer(lstParams);
            }
        }

        /// <summary>
        /// 删除KeyGenServer
        /// </summary>
        private void DeleteKeyGenServer()
        {
            string strConfirm = CurrentApp.GetLanguageInfo("2401013", "Confirm");
            string strPopupMsg = string.Empty;
            strPopupMsg = CurrentApp.GetLanguageInfo("2401023", "Are you sure to do this?");
            MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                KeyGenServerEntryInList item = lvGeneratorObject.SelectedItem as KeyGenServerEntryInList;
                if (item == null)
                {
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.DeleteKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.ResourceID);
                webRequest.ListData.Add(item.HostAddress);
                webRequest.ListData.Add(item.HostPort);
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                           WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_FAIL)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    else if (webReturn.Code == (int)S2400WcfErrorCode.EnableDisableKeyGenServerFailed)
                    {
                        string strErrorMsg = CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message);
                        strErrorMsg = string.Format(strErrorMsg, item.HostPort, webReturn.Data);
                        ShowException(strErrorMsg);
                    }
                    else
                    {
                        string strErrorMsg = CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message);
                        ShowException(strErrorMsg);
                    }
                    parentWin.ShowStausMessage(string.Empty, false);
                    return;
                }
                else
                {
                    lstAllKeyGenServers.Remove(item);
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2401015", "Success"));
                    if (lvGeneratorObject.Items.Count > 0)
                    {
                        lvGeneratorObject.SelectedIndex = 0;
                    }
                    else
                    {
                        CreateOptButtons(ListOperationsOnlyAdd);
                    }
                }
            }
        }
        #endregion

        #region 子窗口调用的函数
        public void UpdateKeyGenServerList(KeyGenServerEntryInList server, OperationType optType)
        {
            switch (optType)
            {
                case OperationType.Add:
                    lstAllKeyGenServers.Add(server);
                    break;
                case OperationType.Modify:
                    KeyGenServerEntryInList ser = lstAllKeyGenServers.Where(p => p.ResourceID == server.ResourceID).First();
                    ser = server;
                    break;
                case OperationType.Delete:
                    lstAllKeyGenServers.Remove(server);
                    break;
            }
        }
        #endregion
    }
}
