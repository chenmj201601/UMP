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
using Common3105;
using UMPS3105.Models;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3105
{
    /// <summary>
    /// AppealManageMainView.xaml 的交互逻辑
    /// </summary>
    public partial class AppealManageMainView
    {

        //创建按钮
        private ObservableCollection<OperationInfo> mListBasicOperations;
        //异步线程
        private BackgroundWorker mWorker;

        private int mPageIndex;
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private int mMaxRecords;

        private UCPlayBox mUCPlayBox;
        private UCRecordMemo mUCRecordMemo;
        private ObservableCollection<AppealInfoItems> mListCurrentAppealItems;
        private List<AppealInfoItems> mListAllAppealInfo;
        /// <summary>
        /// 保存单击获得的录音信息 展示申诉详情  更换语言时使用
        /// </summary>
        private AppealInfoItems mListAppealInfoItems;
        private ObservableCollection<AgentAndUserInfoItems> mListAuInfoItems;


        public AppealManageMainView()
        {
            InitializeComponent();

            mListCurrentAppealItems = new ObservableCollection<AppealInfoItems>();
            mListAllAppealInfo = new List<AppealInfoItems>();
            mListAppealInfoItems = new AppealInfoItems();
            mListAuInfoItems = new ObservableCollection<AgentAndUserInfoItems>();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 100;
            mRecordTotal = 0;
            mMaxRecords = 100000;
            Unloaded += AppealManagePage_Unloaded;
            PanelAppealDetail.IsVisible = false;
            //PanelMemo.IsVisible = false;
            //PanelPlayBox.IsVisible = false;
            TxtPage.KeyUp += TxtPage_KeyUp;
            LvAppealData.ItemsSource = mListCurrentAppealItems;
            //LvAppealData.MouseDoubleClick+=LvAppealData_MouseDoubleClick;
            LvAppealData.SelectionChanged += LvAppealData_SelectionChanged;
        }

        void AppealManagePage_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (CurrentApp.mService03Helper != null)
            //{
            //    CurrentApp.mService03Helper.Close();
            //}
        }


        /// <summary>
        /// 初始化用户信息以及样式
        /// </summary>
        protected override void Init() //用了init()方法之后，不能在用this.load的方法了
        {
            try
            {
                PageName = "AppealManage";
                StylePath = "UMPS3105/MainPageStyle.xaml";
                base.Init();

                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    GetAuInfoLists();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    CreatePageButtons();
                    CreateOptButtons();
                    InitAppealColumns();
                    ChangeLanguage();
                    ChangeTheme();
                    CurrentApp.SendLoadedMessage();//加载完各种消息发送登录消息给UMP
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 查询

        public void QueryAppealInfo(string strCondition, List<DateTimeSpliteAsDay> lstDateTimeSplitAsDay)
        {
            mListAllAppealInfo.Clear();
            mListCurrentAppealItems.Clear();
            mListAppealInfoItems = null;
            mRecordTotal = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = S3105Consts.USER_PARAM_PAGESIZE;
            mMaxRecords = S3105Consts.USER_PARAM_MAXRECORDS;
            try
            {
                PanelBasicOpts.IsEnabled = false;
                PanelOtherOpts.IsEnabled = false;
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    CurrentApp.WriteLog("QueryRecord", string.Format("{0}", strCondition));

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3105Codes.GetAppealInfoData;
                    webRequest.ListData.Add(strCondition);
                    //Service31051Client client = new Service31051Client();
                    Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                    WebReturn webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    if (webReturn.ListData.Count <= 0)
                    {
                        SetPageState();
                        return;
                    }
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<AppealInfo>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        AppealInfo appealInfo = optReturn.Data as AppealInfo;
                        if (appealInfo == null)
                        {
                            ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                            return;
                        }
                        AppealInfoItems item = new AppealInfoItems(appealInfo);
                        mRecordTotal = item.RowNumber;
                        item.Background = GetRecordBackground(item);
                        if (item.Appealint > 0)
                        {
                            item.AppealState = CurrentApp.GetLanguageInfo(string.Format("3105T000{0}", 77 + item.Appealint), item.Appealint.ToString());
                        }
                        if (item.AgentNum != 0)
                        {
                            item.AgentName = AgentAndUserFullName(item.AgentNum);
                            item.AgentID = AgentAndUserName(item.AgentNum);
                        }
                        mListAllAppealInfo.Add(item);
                        if (mRecordTotal < mPageSize + 1)
                        {
                            AddNewAppealInfo(item);
                        }
                    }
                    SetPageState();

                };
                //当后台操作已完成、被取消或引发异常时发生
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false,string.Empty);
                    PanelBasicOpts.IsEnabled = true;
                    PanelOtherOpts.IsEnabled = true;
                };
                mWorker.RunWorkerAsync();//触发DoWork事件

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Query.ToString(), ConstValue.OPT_RESULT_FAIL, "");
            }
            #region 写操作日志
            CurrentApp.WriteOperationLog(S3105Consts.OPT_Query.ToString(), ConstValue.OPT_RESULT_SUCCESS, "");
            #endregion
        }

        private Brush GetRecordBackground(AppealInfoItems appealInfoItems)
        {
            try
            {
                int rowNumber = appealInfoItems.RowNumber;
                if (rowNumber % 2 == 0)
                {
                    return Brushes.LightGray;
                }
            }
            catch (Exception) { }
            return Brushes.Transparent;
        }

        private void AddNewAppealInfo(AppealInfoItems appealInfoItems)
        {
            Dispatcher.Invoke(new Action(() => mListCurrentAppealItems.Add(appealInfoItems)));
        }

        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && mListAllAppealInfo.Count != 0)
                {
                    int pageIndex;
                    if (!int.TryParse(TxtPage.Text, out pageIndex))
                    {
                        TxtPage.Text = (mPageIndex + 1).ToString();
                        return;
                    }
                    pageIndex--;
                    if (pageIndex < 0)
                    {
                        pageIndex = 0;
                    }
                    if (pageIndex > mPageCount - 1)
                    {
                        pageIndex = mPageCount - 1;
                    }
                    mPageIndex = pageIndex;
                    FillListView();
                    SetPageState();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void FillListView()
        {
            try
            {
                mListCurrentAppealItems.Clear();
                int intStart = mPageIndex * mPageSize;
                int intEnd = (mPageIndex + 1) * mPageSize;
                for (int i = intStart; i < intEnd && i < mRecordTotal; i++)
                {
                    mListCurrentAppealItems.Add(mListAllAppealInfo[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        /// <summary>
        /// 錄音詳情翻頁
        /// </summary>
        private void SetPageState()
        {
            try
            {
                int pageCount = mRecordTotal / mPageSize;
                int mod = mRecordTotal % mPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                mPageCount = pageCount;
                string temp = CurrentApp.GetLanguageInfo("3105T00096", "Records");
                string strPageInfo = string.Format("{0}/{1} {2} {3}", mPageIndex + 1, mPageCount, mRecordTotal, temp);
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtPageInfo.Text = strPageInfo;
                    TxtPage.Text = (mPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        // 獲取座席、用戶名
        private void GetAuInfoLists()
        {
            try
            {
                mListAuInfoItems = new ObservableCollection<AgentAndUserInfoItems>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3105Codes.GetAuInfoList;
                webRequest.ListData.Add("11");
                //Service31051Client client = new Service31051Client();
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<AgentAndUserInfoList>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    AgentAndUserInfoList auInfoList = optReturn.Data as AgentAndUserInfoList;
                    if (auInfoList == null)
                    {
                        return;
                    }
                    AgentAndUserInfoItems item = new AgentAndUserInfoItems(auInfoList);
                    mListAuInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public string AgentAndUserFullName(long AuID)
        {
            string AuFullName = string.Empty;
            try
            {
                var temp = mListAuInfoItems.FirstOrDefault(m => m.ID == AuID);
                if (temp != null)
                {
                    AuFullName = ((S3105App)CurrentApp).DecryptString(temp.FullName);
                }
                if (string.IsNullOrWhiteSpace(AuFullName))
                {
                    AuFullName = AuID.ToString();
                }
            }
            catch (Exception ex)
            {
                AuFullName = AuID.ToString();
                ShowException(ex.Message);
            }
            return AuFullName;
        }

        public string AgentAndUserName(long AuID)
        {
            string AuName = string.Empty;
            try
            {
                var temp = mListAuInfoItems.FirstOrDefault(m => m.ID == AuID);
                if (temp != null)
                {
                    AuName = ((S3105App)CurrentApp).DecryptString(temp.Name);
                }
                if (string.IsNullOrWhiteSpace(AuName))
                {
                    AuName = AuID.ToString();
                }
            }
            catch (Exception ex)
            {
                AuName = AuID.ToString();
                ShowException(ex.Message);
            }
            return AuName;
        }
        #endregion

        private void InitAppealColumns()
        {
            try
            {
                string[] lans = "3105T00072,3105T00073,3105T00058,3105T00074,3105T00075,3105T00092,3105T00076,3105T00077".Split(',');
                string[] cols = "RowNumber,AppealState,ReferenceID,AppealTime,AgentName,AgentID,Score,TemplateName".Split(',');
                int[] colwidths = { 50, 100, 160, 140, 80, 80, 50, 120 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < cols.Length; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (i == 3)
                    {
                        var binding = new Binding(cols[i]);
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        gvc.DisplayMemberBinding = binding;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvAppealData.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void LvAppealData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowPlayAndMemoPanel(LvAppealData);
        }

        private void ShowPlayAndMemoPanel(ListView lv)
        {
            var item = lv.SelectedItem as AppealInfoItems;
            if (item != null)
            {
                //if (CurrentApp.ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Memo).ToList().Count > 0)//拥有备注权限
                //    ShowMemo(item);
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Play).ToList().Count > 0)//拥有播放权限
                    PlayRecord(item);
            }
        }


        private void PlayRecord(AppealInfoItems seldate)
        {
            try
            {
                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.RecoredReference = seldate.ReferenceID.ToString();
                mUCPlayBox.IsAutoPlay = true;
                //BorderPlayBox.Child = mUCPlayBox;
                //PanelPlayBox.IsVisible = true;

                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), seldate.ReferenceID.ToString());
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Play.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        public void OpenPasswordPanel(UMPUserControl content)
        {
            try
            {
                PopupPanel.Content = content;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region 页头命令

        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage(); 
            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                     "Appeal Management");

                CreateOptButtons();
                CreatePageButtons();
                InitAppealColumns();
                CreatAppealInfoDetail();

                var panel = GetPanleByContentID("PanelAppealList");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00091", "Appeal Result List");
                }
                panel = GetPanleByContentID("PanelAppealDetail");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00095", "AppealInfo Detail");
                }
                panel = GetPanleByContentID("PanelPlayBox");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00031", "Play Box");
                }
                panel = GetPanleByContentID("PanelMemo");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00041", "Memo");
                }

                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("3105T00005", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("3105T00006", "Other Position");
                //列名
            }
            catch (Exception ex)
            {

            }
        }

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        public override void ChangeTheme()
        {
            base.ChangeTheme();
            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception ex)
                {
                    // ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3105;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS3105;component/Themes/Default/UMPS3105/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
            try
            {
                string uri = string.Format("/UMPS3105;component/Themes/Default/UMPS3105/QMAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }

        }

        #endregion

        /// <summary>
        /// 根据权限生成操作按钮
        /// </summary>
        private void CreateOptButtons()
        {
            try
            {
                mListBasicOperations.Clear();
                //查询
                OperationInfo item;
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Query).ToList().Count > 0) //拥有查詢权限
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Query;
                    item.ParentID = S3105Consts.OPT_Query;
                    item.SortID = 0;
                    item.Icon = "Images/search.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00055", "Query");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }

                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Review).ToList().Count > 0) //拥有複核权限
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Review;
                    item.ParentID = S3105Consts.OPT_ProcessAppeal;
                    item.SortID = 0;
                    item.Icon = "Images/keyitem.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00010", "ReCheck");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Approval).ToList().Count > 0) //拥有审批权限
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Approval;
                    item.ParentID = S3105Consts.OPT_ProcessAppeal;
                    item.SortID = 0;
                    item.Icon = "Images/callinfo.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00009", "Check");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                PanelBasicOpts.Children.Clear();
                PanelOtherOpts.Children.Clear();
                Button btn;
                item = new OperationInfo();
                int tempInt = 0;
                for (int i = 0; i < mListBasicOperations.Count; i++)
                {
                    item = mListBasicOperations[i];
                    if (item.ID == S3105Consts.OPT_Review) //复核权限
                    {
                        item.Display = CurrentApp.GetLanguageInfo("3105T00056", "Go to Review");
                        tempInt = 1;
                    }
                    else if (item.ID == S3105Consts.OPT_Approval) //审批权限
                    {
                        item.Display = CurrentApp.GetLanguageInfo("3105T00057", "Go to Approval");
                        tempInt = 1;
                    }
                    else if (item.ID == S3105Consts.OPT_Query)
                    {
                        item.Display = mListBasicOperations[i].Display;
                    }
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    switch (tempInt)
                    {
                        case 0:
                            PanelBasicOpts.Children.Add(btn);
                            break;
                        case 1:
                            PanelOtherOpts.Children.Add(btn);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            try
            {
                if (btn != null)
                {
                    //各操作按纽触发按钮方法 
                    var optItem = btn.DataContext as OperationInfo;
                    switch (optItem.ID)
                    {
                        case S3105Consts.OPT_Query:
                            ProcssQuery();
                            break;
                        case S3105Consts.OPT_Review:
                            //NavigationService.Navigate(new Uri("AppealRecheck.xaml", UriKind.Relative));
                            AppealRecheckMainView recheckView = new AppealRecheckMainView();
                            recheckView.PageName = "AppealManageMainView";
                            var temp1 = CurrentApp as S3105App;
                            if (temp1 != null)
                            {
                                temp1.InitMainView(recheckView);
                            }
                            break;
                        case S3105Consts.OPT_Approval:
                            //NavigationService.Navigate(new Uri("AppealApproval.xaml", UriKind.Relative));
                            AppealApprovalMainView approvalView = new AppealApprovalMainView();
                            approvalView.PageName = "AppealApprovalMainView";
                            var temp = CurrentApp as S3105App;
                            if (temp != null)
                            {
                                temp.InitMainView(approvalView);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        // 翻頁按鈕
        private void CreatePageButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("3105T00085", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3105T00085", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3105T00086", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3105T00086", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3105T00087", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3105T00087", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3105T00088", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3105T00088", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    ToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += PageButton_Click;
                    PanelPageButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = e.Source as Button;
                if (btn != null)
                {
                    var item = btn.DataContext as ToolButtonItem;
                    if (item == null) { return; }
                    switch (item.Name)
                    {
                        case "TB" + "FirstPage":
                            if (mPageIndex > 0)
                            {
                                mPageIndex = 0;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "PrePage":
                            if (mPageIndex > 0)
                            {
                                mPageIndex--;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "NextPage":
                            if (mPageIndex < mPageCount - 1)
                            {
                                mPageIndex++;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "LastPage":
                            if (mPageIndex < mPageCount - 1)
                            {
                                mPageIndex = mPageCount - 1;
                                FillListView();
                                SetPageState();
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void ProcssQuery()
        {
            try
            {
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00055", "Query");
                QueryAppealResult popQuery = new QueryAppealResult();
                popQuery.CurrentApp = CurrentApp;
                popQuery.ParentPage = this;
                PopupPanel.Content = popQuery;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LvAppealData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AppealInfoItems appealInfoItems = LvAppealData.SelectedItem as AppealInfoItems;
                if (appealInfoItems != null && appealInfoItems.ReferenceID != 0 && appealInfoItems.ScoreID != 0)
                {
                    mListAppealInfoItems = appealInfoItems;
                    CreatAppealInfoDetail();
                }
                else
                {
                    PanelAppealDetail.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 申诉详情--右側信息欄
        /// </summary>
        private void CreatAppealInfoDetail()
        {
            try
            {
                stkAppealInfo.Children.Clear();
                if (mListAppealInfoItems != null && mListAppealInfoItems.ReferenceID != 0 && mListAppealInfoItems.ScoreID != 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3105Codes.GetAppealRecordsHistory;
                    webRequest.ListData.Add(mListAppealInfoItems.ReferenceID.ToString());//录音记录ID C003
                    webRequest.ListData.Add(mListAppealInfoItems.ScoreID.ToString());//成绩ID C002
                    //Service31051Client client = new Service31051Client();
                    Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                    WebReturn webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.Message == S3105Consts.AppealOvered)
                    {
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    if (webReturn.ListData.Count <= 0) { return; }
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<AppealInfoDetailItem>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        AppealInfoDetailItem appealInfo = optReturn.Data as AppealInfoDetailItem;
                        if (appealInfo == null)
                        {
                            ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                            return;
                        }
                        if (appealInfo.ID == 1 || appealInfo.ID == 2)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = CurrentApp.GetLanguageInfo("3105T00097", "Appeal");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00098", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00014", "Appeal Person") + ":" + AgentAndUserFullName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00030", "Appeal Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00099", "Appeal Reason") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                        if (appealInfo.ID == 3 || appealInfo.ID == 4)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = CurrentApp.GetLanguageInfo("3105T00010", "ReCheck");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00098", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00100", "Check Person") + "：" + AgentAndUserFullName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00050", "Check Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            if (appealInfo.ID == 3)//复核驳回
                            {
                                txb.Text = CurrentApp.GetLanguageInfo("3105T00024", "ReCheck Result") + "：" + CurrentApp.GetLanguageInfo("3105T00028", "ReCheck-reject");
                            }
                            else//复核通过
                            {
                                txb.Text = CurrentApp.GetLanguageInfo("3105T00024", "ReCheck Result") + "：" + CurrentApp.GetLanguageInfo("3105T00081", "accept");
                            }
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00025", "ReCheck Remarks") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                        if (appealInfo.ID == 5 || appealInfo.ID == 6 || appealInfo.ID == 7)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = CurrentApp.GetLanguageInfo("3105T00009", "Check");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00098", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00015", "Check Person") + "：" + AgentAndUserFullName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00049", "Check Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            if (appealInfo.ID == 5)//审批通过不修改分数
                            {
                                txb.Text = CurrentApp.GetLanguageInfo("3105T00018", "Check Result") + "：" + CurrentApp.GetLanguageInfo("3105T00082", "Review by not modify the score");
                            }
                            if (appealInfo.ID == 6)//审批通过重新评分
                            {
                                txb.Text = CurrentApp.GetLanguageInfo("3105T00018", "Check Result") + "：" + CurrentApp.GetLanguageInfo("3105T00083", "Check through to score");
                            }
                            if (appealInfo.ID == 7)//审批驳回
                            {
                                txb.Text = CurrentApp.GetLanguageInfo("3105T00018", "Check Result") + "：" + CurrentApp.GetLanguageInfo("3105T00084", "ReCheck-reject");
                            }
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = CurrentApp.GetLanguageInfo("3105T00017", "Check Remarks") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                    }
                    PanelAppealDetail.IsVisible = true;
                }
                else
                {
                    PanelAppealDetail.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);
            var code = webRequest.Code;
            switch (code)
            {
                case (int)RequestCode.ACPageHeadLeftPanel:
                    if (GridLeft.Width.Value == 0)
                    {
                        GridLeft.Width = new GridLength(200);
                    }
                    else
                    {
                        GridLeft.Width = new GridLength(0);
                    }
                    break;
            }
        }

    }
}
