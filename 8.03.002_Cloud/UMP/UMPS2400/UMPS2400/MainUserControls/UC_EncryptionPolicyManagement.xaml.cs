using Common2400;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using UMPS2400.Entries;
using UMPS2400.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using UMPS2400.ChildUCs;
using UMPS2400.Service24021;
using System.Data;
using UMPS2400.Service24011;

namespace UMPS2400.MainUserControls
{
    /// <summary>
    /// UC_EncryptionPolicyManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptionPolicyManagement
    {
        #region 变量定义
        //listview要显示的数据ViewID
        private string mViewID;
        //listview需要显示的列的集合
        private ObservableCollection<ViewColumnInfo> mListColumnItems;
        public static ObservableCollection<PolicyInfoInList> lstAllPolicies = new ObservableCollection<PolicyInfoInList>();
        public static ObservableCollection<OperationInfo> ListOperationsOnlyAdd = new ObservableCollection<OperationInfo>();
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();

        //正在使用、即将使用、已经过期、被禁用的策略
        public static ObservableCollection<PolicyInfoInList> lstUsing = new ObservableCollection<PolicyInfoInList>();
        public static ObservableCollection<PolicyInfoInList> lstWillBeUse = new ObservableCollection<PolicyInfoInList>();
        public static ObservableCollection<PolicyInfoInList> lstExpried = new ObservableCollection<PolicyInfoInList>();
        public static ObservableCollection<PolicyInfoInList> lstDisabled = new ObservableCollection<PolicyInfoInList>();

        //是否有可用的密钥生成服务
        public  bool HasKeyGenServer = false;
        //可用的密钥生成服务信息
        public KeyGenServerEntry keyGenEntry = null;
        BackgroundWorker mBackgroundWorker = null;

        //保存筛选出来的策略
        // public static ObservableCollection<PolicyInfoInList> lstPolicySelected = null;

        public EncryptMainPage parentWin = null;
        #endregion

        public UC_EncryptionPolicyManagement()
        {
            InitializeComponent();
            this.Loaded += UC_EncryptionPolicyManagement_Loaded;
            lvEncryptionPolicyObject.SelectionChanged += lvEncryptionPolicyObject_SelectionChanged;
            CmbPolicyType.SelectionChanged += CmbPolicyType_SelectionChanged;
        }

        #region 主界面事件
        void lvEncryptionPolicyObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateOptButtons(AllListOperations);
        }

        void UC_EncryptionPolicyManagement_Loaded(object sender, RoutedEventArgs e)
        {
            
            //   lvEncryptionPolicyObject.ItemsSource = lstPolicySelected;
            parentWin.ShowStausMessage(CurrentApp.GetLanguageInfo("2401007", "Loading") + "....", true);
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
            {
                //获得需要显示的列
                LoadViewColumnItems();
                //获得用户可使用的按钮(操作权限)
                GetUserOpts();
                GetAppServerCurrentTime();
                GetAllPolicies();
                GetCurrentKeyGenServer();
            };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
            {
                parentWin.ShowStausMessage(string.Empty, false);
                CreateColumnsItems();
                //获得需要显示的数据
                //InitKeyGenServerList();
                CreateOptButtons(ListOperationsOnlyAdd);

                CmbPolicyType.SelectedIndex = 0;
                if (!HasKeyGenServer)
                {
                    imgStatus.Source = new BitmapImage(new Uri(@"../Themes/Default/UMPS2400/Images/00001.ico", UriKind.Relative));
                    lblStatus.Content = CurrentApp.GetLanguageInfo("2402L027", "No key generation service is available");
                }
                else
                {
                    imgStatus.Source = new BitmapImage(new Uri(@"../Themes/Default/UMPS2400/Images/00002.ico", UriKind.Relative));
                    string str = CurrentApp.GetLanguageInfo("2402L028", "Key generation service currently used:");
                    str += keyGenEntry.HostAddress + "---" + keyGenEntry.HostPort;
                    lblStatus.Content = str;
                }
                InitChildPolicies();
              //  ChangeLanguage();
                InitLanguage();
            };
            mBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// CmbPolicyType的SelectionChangedchange事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CmbPolicyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = CmbPolicyType.SelectedItem as ComboBoxItem;
            string strTag = item.Tag.ToString();
            switch (strTag)
            {
                case "1":
                    lvEncryptionPolicyObject.ItemsSource = lstUsing;
                    break;
                case "2":
                    lvEncryptionPolicyObject.ItemsSource = lstWillBeUse;
                    break;
                case "3":
                    lvEncryptionPolicyObject.ItemsSource = lstExpried;
                    break;
                case "4":
                    lvEncryptionPolicyObject.ItemsSource = lstDisabled;
                    break;
            }
        }
        #endregion

        #region Override
        public override void ChangeLanguage()
        {
            CurrentApp.WriteLog("UC_EncryptionPolicyManagement ChangeLanguage");
            base.ChangeLanguage();
            #region 更新当前ursercontrol中控件的显示文字
            //更新listview种列的显示文字
            CreateColumnsItems();
            #endregion
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
            GetAllPolicies();
            InitChildPolicies();
            InitLanguage();
            PopupPanel.ChangeLanguage();
        }


        #endregion

        #region 初始化
        /// <summary>
        /// 获得需要显示的列
        /// </summary>
        private void LoadViewColumnItems()
        {
            mViewID = "2402001";
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
                listColumns.ForEach(obj => mListColumnItems.Add(obj));
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
                lvEncryptionPolicyObject.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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

        /// <summary>
        /// 获得用户可操作权限
        /// </summary>
        private void GetUserOpts()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)RequestCode.WSGetUserOptList;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
            webRequest.ListData.Add("24");
            webRequest.ListData.Add("2402");
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
                    if (optInfo.ID == S2400Const.OPT_EncryptionPolicyAdd)
                    {
                        ListOperationsOnlyAdd.Add(optInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 获得加密数据库服务器的当前时间
        /// </summary>
        public void GetAppServerCurrentTime()
        {
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetEncryptionDBCurrentTime;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(mViewID);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    S2400App.GolbalCurrentEncryptionDBTime = DateTime.Now;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    S2400App.GolbalCurrentEncryptionDBTime = DateTime.Now;
                }
                S2400App.GolbalCurrentEncryptionDBTime = DateTime.Parse(webReturn.ListData[0]);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 初始化界面上固定的语言 比如label、combobox项等
        /// </summary>
        private void InitLanguage()
        {
            LbOperations.Text = CurrentApp.GetLanguageInfo("2402L002", "Operations");
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("2402L001", "Encryption Management->Polic Management");
            cmbPolicyTypeDisabled.Content = CurrentApp.GetLanguageInfo("2402PolicyDisabled", "Disabled");
            cmbPolicyTypeExpired.Content = CurrentApp.GetLanguageInfo("2402PolicyExpired", "Expired");
            cmbPolicyTypeUsing.Content = CurrentApp.GetLanguageInfo("2402PolicyUsing", "using");
            cmbPolicyTypeWillBeUse.Content = CurrentApp.GetLanguageInfo("2402PolicyWillBeUse", "Will be use");

            if (!HasKeyGenServer)
            {
                imgStatus.Source = new BitmapImage(new Uri(@"../Themes/Default/UMPS2400/Images/00001.ico", UriKind.Relative));
                lblStatus.Content = CurrentApp.GetLanguageInfo("2402L027", "No key generation service is available");
            }
            else
            {
                imgStatus.Source = new BitmapImage(new Uri(@"../Themes/Default/UMPS2400/Images/00002.ico", UriKind.Relative));
                string str = CurrentApp.GetLanguageInfo("2402L028", "Key generation service currently used:");
                str += keyGenEntry.HostAddress + "---" + keyGenEntry.HostPort;
                lblStatus.Content = str;
            }
        }

        /// <summary>
        /// 获得所有的加密策略 
        /// </summary>
        private void GetAllPolicies()
        {
            lstAllPolicies.Clear();
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetAllPolicies;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                if (webReturn.DataSetData.Tables.Count <= 0)
                {
                    return;
                }
                DataTable dt = webReturn.DataSetData.Tables[0];
                PolicyInfoInList policyItem = null;
                foreach (DataRow row in dt.Rows)
                {
                    policyItem = new PolicyInfoInList();
                    policyItem.PolicyID = row["C001"].ToString();
                    policyItem.PolicyName = row["C002"].ToString();
                    //= row["004"].ToString();
                    string strType = row["C004"].ToString();
                    policyItem.PolicyType = strType == "U" ? CurrentApp.GetLanguageInfo("2402003", "Custom (user input)") : CurrentApp.GetLanguageInfo("2402002", "Periodic update key (randomly generated)");
                    if (strType == "C")
                    {
                        string strOccursFrequency = row["C007"].ToString();
                        switch (strOccursFrequency)
                        {
                            case "D":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagD", "Day");
                                break;
                            case "W":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagW", "Week");
                                break;
                            case "M":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagM", "Month");
                                break;
                            case "U":
                                policyItem.PolicyOccursFrequency = row["C010"].ToString() + CurrentApp.GetLanguageInfo("2402ComboTagD", "Day");
                                break;
                        }
                    }
                    else
                    {
                        policyItem.PolicyOccursFrequency = string.Empty;
                    }
                    policyItem.PolicyStartTime = CommonFunctions.StringToDateTime(row["C008"].ToString()).ToString();
                    long longTime = 0;
                    long.TryParse(row["C008"].ToString(), out longTime);
                    policyItem.PolicyStartTimeNumber = longTime;
                    DateTime dtEnd = CommonFunctions.StringToDateTime(row["C009"].ToString());
                    if (dtEnd.ToString("yyyy-MM-dd HH:mm:ss") != "2099-12-31 23:59:59")
                    {
                        policyItem.PolicyEndTime = CommonFunctions.StringToDateTime(row["C009"].ToString()).ToString();
                    }
                    else
                    {
                        policyItem.PolicyEndTime = CurrentApp.GetLanguageInfo("2402RB002", "Never expires");
                    }
                    long.TryParse(row["C009"].ToString(), out longTime);
                    policyItem.PolicyEndTimeNumber = longTime;
                    policyItem.PolicyIsEnabled = row["C003"].ToString();
                    if (strType == "C")
                    {
                        policyItem.IsStrongPwd = row["C012"].ToString() == "1" ? CurrentApp.GetLanguageInfo("2402019", "Yes") : string.Empty;
                    }
                    else
                    {
                        policyItem.IsStrongPwd = string.Empty; 
                    }
                    lstAllPolicies.Add(policyItem);
                }
            }
            catch (Exception ex)
            {
                ShowException("Failed." + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        private void InitChildPolicies()
        {
            lstDisabled.Clear();
            lstExpried.Clear();
            lstUsing.Clear();
            lstWillBeUse.Clear();
            long lNow = 0;
            bool bo = CommonFunctions.DateTimeToNumber(S2400App.GolbalCurrentEncryptionDBTime, ref lNow);
            List<PolicyInfoInList> lstSelected = null;
            lstSelected = lstAllPolicies.Where(p => p.PolicyStartTimeNumber <= lNow && p.PolicyIsEnabled == "1" && p.PolicyEndTimeNumber >= lNow).ToList();
            lstSelected.ForEach(obj => lstUsing.Add(obj));

            lstSelected = lstAllPolicies.Where(p => p.PolicyStartTimeNumber > lNow && p.PolicyIsEnabled == "1").ToList();
            lstSelected.ForEach(obj => lstWillBeUse.Add(obj));

            lstSelected = lstAllPolicies.Where(p => p.PolicyEndTimeNumber < lNow && p.PolicyIsEnabled == "1").ToList();
            lstSelected.ForEach(obj => lstExpried.Add(obj));

            lstSelected = lstAllPolicies.Where(p => p.PolicyIsEnabled == "2").ToList();
            lstSelected.ForEach(obj => lstDisabled.Add(obj));
        }

        private void GetCurrentKeyGenServer()
        {
            Service24011Client client = null;
            try
            {
                WebReturn webReturn = null;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetCurrentKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                if (webReturn.Result && string.IsNullOrEmpty(webReturn.Data))
                {
                    HasKeyGenServer = false;
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<KeyGenServerEntry>(webReturn.Data.ToString());
                if (!optReturn.Result)
                {
                    HasKeyGenServer = false;
                    return;
                }
                KeyGenServerEntry keyGenServer = optReturn.Data as KeyGenServerEntry;
                //尝试连接服务 如果能连接成功 就显示可用的服务
                webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.TryConnToKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(keyGenServer.HostAddress);
                webRequest.ListData.Add(keyGenServer.HostPort);
                client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    HasKeyGenServer = false;
                    return;
                }
            
                HasKeyGenServer = true;
                keyGenEntry = keyGenServer;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion

        #region 按钮操作事件
        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                UC_AddPolicy uc_AddPolicy = null;
                switch (optItem.ID)
                {
                    case S2400Const.OPT_EncryptionPolicyAdd:
                        uc_AddPolicy = new UC_AddPolicy();
                        uc_AddPolicy.CurrentApp = CurrentApp;
                        uc_AddPolicy.ChangeLanguage();
                        uc_AddPolicy.iAddOrModify = (int)OperationType.Add;
                        uc_AddPolicy.mainPage = this;
                        PopupPanel.Content = uc_AddPolicy;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("2402T001", "Add encryption policy");
                        PopupPanel.IsOpen = true;
                        break;
                    case S2400Const.OPT_EncryptionPolicyModify:
                        uc_AddPolicy = new UC_AddPolicy();
                        uc_AddPolicy.CurrentApp = CurrentApp;
                        //根据listview的选中项 获得策略实体类
                        UMPEncryptionPolicy policy = null;
                        if (lvEncryptionPolicyObject.SelectedItem == null)
                        {
                            return;
                        }
                        PolicyInfoInList policyInList = lvEncryptionPolicyObject.SelectedItem as PolicyInfoInList;
                        bool bo = GetSelectedPolicyInfo(ref policy);
                        if (!bo)
                        {
                            return;
                        }
                        uc_AddPolicy.policyModifying = policy;
                        uc_AddPolicy.ChangeLanguage();
                        uc_AddPolicy.iAddOrModify = (int)OperationType.Modify;
                        uc_AddPolicy.mainPage = this;
                        uc_AddPolicy.PolicyModifyingInList = policyInList;
                        PopupPanel.Content = uc_AddPolicy;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("2402T002", "Update encryption policy " + policy.PolicyName);
                        PopupPanel.IsOpen = true;
                        break;
                    case S2400Const.Opt_EncryptionPolicyDisable:
                        EnableDisablePolicy("2");
                        break;
                    case S2400Const.OPT_EncryptionPolicyEnable:
                        EnableDisablePolicy("1");
                        break;
                }
            }
        }

        /// <summary>
        /// 启用或禁用策略
        /// </summary>
        /// <param name="strOptType">1 : 启用   2：禁用</param>
        private void EnableDisablePolicy(string strOptType)
        {
            if (lvEncryptionPolicyObject.SelectedItem == null)
            {
                return;
            }
            PolicyInfoInList policyInList = lvEncryptionPolicyObject.SelectedItem as PolicyInfoInList;
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                switch (strOptType)
                {
                    case "1":
                        if (policyInList.PolicyIsEnabled == "1")
                        {
                            return;
                        }
                        webRequest.Code = (int)S2400RequestCode.EnablePolicy;
                        break;
                    case "2":
                        if (policyInList.PolicyIsEnabled == "2")
                        {
                            return;
                        }
                        webRequest.Code = (int)S2400RequestCode.DisablePolicy;
                        break;
                }
                
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(policyInList.PolicyID);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }

                //写日志
                string strLog = string.Empty;
                if (strOptType == "1")
                {
                    strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402001")), policyInList.PolicyID);
                    CurrentApp.WriteOperationLog("2402001", ConstValue.OPT_RESULT_SUCCESS, strLog);
                }
                else if (strOptType == "2")
                {
                    strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402002")), policyInList.PolicyID);
                    CurrentApp.WriteOperationLog("2402002", ConstValue.OPT_RESULT_SUCCESS, strLog);
                }

                policyInList.PolicyIsEnabled = strOptType;
                if(strOptType == "1")
                {
                     UpdatePolicyList(policyInList, OperationType.Enable);
                }
               else
                {
                     UpdatePolicyList(policyInList, OperationType.Disable);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                string strLog = string.Empty;
                if (strOptType == "1")
                {
                    strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402001")), policyInList.PolicyID);
                    CurrentApp.WriteOperationLog("2402001", ConstValue.OPT_RESULT_FAIL, strLog);
                }
                else if (strOptType == "2")
                {
                    strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402002")), policyInList.PolicyID);
                    CurrentApp.WriteOperationLog("2402002", ConstValue.OPT_RESULT_FAIL, strLog);
                }
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 获得listview选中项的策略信息 
        /// </summary>
        /// <returns></returns>
        private   bool GetSelectedPolicyInfo(ref UMPEncryptionPolicy policy)
        {
            PolicyInfoInList policyInList = lvEncryptionPolicyObject.SelectedItem as PolicyInfoInList;
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetPolicyByID;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(policyInList.PolicyID);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return false;
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<UMPEncryptionPolicy>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, CurrentApp.GetLanguageInfo(optReturn.Code.ToString(), optReturn.Message)));
                    return false;
                }
                policy = optReturn.Data as UMPEncryptionPolicy;
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
            return true;
        }
        #endregion

        #region 子窗口调用函数
        public void ShowStausMessage(string strMsg, bool bIsShow)
        {
            parentWin.ShowStausMessage(strMsg, bIsShow);
        }

        /// <summary>
        /// 在添加、删除、修改策略后 刷新界面
        /// </summary>
        public void UpdatePolicyList(PolicyInfoInList policy, OperationType optType)
        {
            long lNow = 0;
            GetAppServerCurrentTime();
            bool bo = CommonFunctions.DateTimeToNumber(S2400App.GolbalCurrentEncryptionDBTime, ref lNow);
            switch (optType)
            {
                case OperationType.Add:
                    lstAllPolicies.Add(policy);
                    if (policy.PolicyStartTimeNumber <= lNow)
                    {
                        lstUsing.Add(policy);
                        CmbPolicyType.SelectedIndex = 0;
                    }
                    else
                    {
                        lstWillBeUse.Add(policy);
                        CmbPolicyType.SelectedIndex = 1;
                    }
                    break;
                case OperationType.Modify:
                    for (int i = 0; i < lstAllPolicies.Count; i++)
                    {
                        PolicyInfoInList po = lstAllPolicies[i];
                        if (po.PolicyID == policy.PolicyID)
                        {
                            lstAllPolicies[i] = policy;
                        }
                    }
                    break;
                case OperationType.Delete:
                    lstAllPolicies.Remove(policy);
                    break;
                case OperationType.Enable:
                    lstDisabled.Remove(policy);
                    if (policy.PolicyStartTimeNumber >= lNow)
                    {
                        lstWillBeUse.Add(policy);
                        CmbPolicyType.SelectedIndex = 1;
                    }
                    else if (policy.PolicyStartTimeNumber < lNow && policy.PolicyEndTimeNumber >=lNow)
                    {
                        lstUsing.Add(policy);
                        CmbPolicyType.SelectedIndex = 0;
                    }
                    else if (policy.PolicyEndTimeNumber < lNow)
                    {
                        lstExpried.Add(policy);
                        CmbPolicyType.SelectedIndex = 2;
                    }
                    break;
                case OperationType.Disable:
                    if (policy.PolicyStartTimeNumber >= lNow)
                    {
                        lstWillBeUse.Remove(policy);
                    }
                    else if (policy.PolicyStartTimeNumber < lNow && policy.PolicyEndTimeNumber >= lNow)
                    {
                        lstUsing.Remove(policy);
                    }
                    else if (policy.PolicyEndTimeNumber < lNow)
                    {
                        lstExpried.Remove(policy);
                    }
                    lstDisabled.Add(policy);
                    CmbPolicyType.SelectedIndex = 3;
                    break;
            }
        }
        #endregion
    }
}
