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
using UMPS3105.Wcf31031;

namespace UMPS3105
{
    /// <summary>
    /// AppealApprovalMainView.xaml 的交互逻辑
    /// 审批
    /// </summary>
    public partial class AppealApprovalMainView
    {

        public AppealManageMainView PageParent;

        //创建按钮
        private ObservableCollection<OperationInfo> mListBasicOperations;


        public static ObservableCollection<CCheckData> ListReCheckDatas = new ObservableCollection<CCheckData>();
        public static ObservableCollection<CCheckData> ListApprovalHistoryDatas = new ObservableCollection<CCheckData>();
        //异步线程
        private BackgroundWorker mWorker;

        private UCPlayBox mUCPlayBox;
        private UCRecordMemo mUCRecordMemo;
        public CCheckData CurrentFocusReCheckData;

        public string AgentID;


        public AppealApprovalMainView()
        {
            InitializeComponent();

            mListBasicOperations = new ObservableCollection<OperationInfo>();

            PanelMemo.IsVisible = false;
            PanelPlayBox.IsVisible = false;
            LvApprovalList.ItemsSource = ListApprovalHistoryDatas;
        }


        /// <summary>
        /// 初始化用户信息以及样式
        /// </summary>
        protected override void Init()
        {
            try
            {
                PageName = "AppealApproval";
                StylePath = "UMPS3105/MainPageStyle.xaml";

                base.Init();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateOptButtons();
                    BindData();
                    ApprovalHistory();
                    ChangeTheme();
                    ChangeLanguage();
                    CurrentApp.SendLoadedMessage();//加载完各种消息发送登录消息给UMP
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateOptButtons()
        {
            mListBasicOperations.Clear();
            try
            {
                OperationInfo item;
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Query).ToList().Count > 0) //查詢
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Query;
                    item.ParentID = S3105Consts.OPT_Query;
                    item.SortID = 1;
                    item.Icon = "Images/search.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00059", "Go to Query");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Review).ToList().Count > 0) //拥有複核权限
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Review;
                    item.ParentID = S3105Consts.OPT_ProcessAppeal;
                    item.SortID = 1;
                    item.Icon = "Images/callinfo.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00056", "Go to Review");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Approval).ToList().Count > 0) //拥有审批权限
                {
                    item = new OperationInfo();
                    item.ID = S3105Consts.OPT_Approval;
                    item.ParentID = S3105Consts.OPT_ProcessAppeal;
                    item.SortID = 0;
                    item.Icon = "Images/keyitem.png";
                    item.Display = CurrentApp.GetLanguageInfo("3105T00009", "Approval");
                    item.Description = null;
                    mListBasicOperations.Add(item);

                    //item = new OperationInfo();
                    //item.ID = S3105Consts.OPT_ViewScore;
                    //item.ParentID = S3105Consts.OPT_ProcessAppeal;
                    //item.SortID = 0;
                    //item.Icon = "Images/objectbox.png";
                    //item.Display = CurrentApp.GetLanguageInfo("3105T00114", "ViewScore");
                    //item.Description = null;
                    //mListBasicOperations.Add(item);
                }

                PanelBasicOpts.Children.Clear();
                PanelOtherOpts.Children.Clear();
                Button btn;
                item = new OperationInfo();
                for (int i = 0; i < mListBasicOperations.Count; i++)
                {
                    item = mListBasicOperations[i];
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    switch (item.SortID)
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
                            //NavigationService.Navigate(new Uri("AppealManagePage.xaml", UriKind.Relative));
                            AppealManageMainView appealView = new AppealManageMainView();
                            appealView.PageName = "AppealManageMainView";
                            var temp = CurrentApp as S3105App;
                            if (temp != null)
                            {
                                temp.InitMainView(appealView);
                            }

                            break;
                        case S3105Consts.OPT_Review:
                            //NavigationService.Navigate(new Uri("AppealRecheck.xaml", UriKind.Relative));
                            AppealRecheckMainView recheckView = new AppealRecheckMainView();
                            recheckView.PageName = "AppealRecheckMainView";
                            var temp1 = CurrentApp as S3105App;
                            if (temp1 != null)
                            {
                                temp1.InitMainView(recheckView);
                            }
                            break;
                        case S3105Consts.OPT_Approval:
                            ProcessApproval();
                            break;
                        case S3105Consts.OPT_ViewScore:
                            
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message + ex.StackTrace + ex.Source);
            }
        }

        private void ProcessApproval()
        {
            try
            {
                CurrentFocusReCheckData = new CCheckData();
                CurrentFocusReCheckData = (CCheckData)LvReCheck.SelectedItem;
                AgentID = CurrentFocusReCheckData.AgentID;
                if (S3105App.ListScoreParam.Where(p => p.ParamID == 31010404).Count() != 0)
                {
                    DateTime dealTime = Convert.ToDateTime(CurrentFocusReCheckData.AppealDatetime).AddHours(Convert.ToInt32((S3105App.ListScoreParam.Where(p => p.ParamID == 31010404).First().ParamValue.Substring(9, 4))));//申诉后任务处理截至日期
                    if (S3105App.ListScoreParam.Where(p => p.ParamID == 31010404).First().ParamValue.Substring(8, 1) == "1" && DateTime.Compare(dealTime, DateTime.Now) < 0) { ShowInformation(CurrentApp.GetLanguageInfo("3105T00116", "Has exceeded the task processing time.")); return; }
                }
                if (!string.IsNullOrWhiteSpace(CurrentFocusReCheckData.RecoredReference) && CurrentFocusReCheckData != null)
                {
                    if (((S3105App)CurrentApp).AppealProcess == "N")
                    {
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                        ProcessReCheck popProcessCheck = new ProcessReCheck();
                        popProcessCheck.CurrentApp = CurrentApp;
                        popProcessCheck.SelCheckData = CurrentFocusReCheckData;
                        popProcessCheck.ParentPage = this;
                        PopupPanel.Content = popProcessCheck;
                        PopupPanel.IsOpen = true;
                    }
                    else
                    {
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                        ProcessReCheck popProcessCheck = new ProcessReCheck();
                        popProcessCheck.CurrentApp = CurrentApp;
                        popProcessCheck.SelCheckData = CurrentFocusReCheckData;
                        popProcessCheck.ParentPage = this;
                        PopupPanel.Content = popProcessCheck;
                        PopupPanel.IsOpen = true;
                    }
                }
                else
                    ShowInformation(CurrentApp.GetLanguageInfo("3105T00052", "Please select one record at least."));
            }
            catch (Exception)
            {

            }
        }
        public void ShowProcessReCheck(bool isshow)
        {
            PopupPanel.IsOpen = isshow;
            PopupPanel.IsOpen = isshow;
        }

        public void ShowTemplatePop(string action, string AppealFlowItemID, CCheckData SelCheckData, string strDisContent)
        {
            try
            {
                if (CurrentFocusReCheckData != null)
                {
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00032", "Choose Scoring Template");
                    TemplateSelect popSelTemplate = new TemplateSelect();
                    popSelTemplate.CurrentApp = CurrentApp;
                    popSelTemplate.ParentPage = this;
                    popSelTemplate.selTaskRecord = CurrentFocusReCheckData;

                    popSelTemplate.action = action;
                    popSelTemplate.AppealFlowItemID = AppealFlowItemID;
                    popSelTemplate.SelCheckData = SelCheckData;
                    popSelTemplate.strDisContent = strDisContent;

                    PopupPanel.Content = popSelTemplate;
                    PopupPanel.IsOpen = true;
                }

            }
            catch (Exception)
            {

            }
        }

        public void ScoreTaskRecord(BasicScoreSheetItem selScoreSheet, string action, string AppealFlowItemID, CCheckData SelCheckData, string strDisContent)
        {
            try
            {
                TaskScoreMainView mScoreForm = new TaskScoreMainView();
                mScoreForm = new TaskScoreMainView();
                mScoreForm.PageParent = this;
                mScoreForm.selScoreSheet = selScoreSheet;
                mScoreForm.SelTaskRecord = CurrentFocusReCheckData;
                mScoreForm.TaskScoreType = 1;

                mScoreForm.action = action;
                mScoreForm.AppealFlowItemID = AppealFlowItemID;
                mScoreForm.strDisContent = strDisContent;
                mScoreForm.SelCheckData = SelCheckData;

                //NavigationService.Navigate(mScoreForm);
                mScoreForm.PageName = "TaskScoreMainView";
                var temp1 = CurrentApp as S3105App;
                if (temp1 != null)
                {
                    temp1.InitMainView(mScoreForm);
                }
            }
            catch (Exception)
            {

            }
        }
        public void ShowPopupPanel(bool isshow)
        {
            PopupPanel.IsOpen = isshow;
        }

        public void BindData()
        {
            try
            {
                string type = ((S3105App)CurrentApp).AppealProcess == "Y" ? "2" : "3";
                ListReCheckDatas.Clear();
                InitApprovalColumns();
                GetCheckDataByDeptIDs(type, "4", "5,6,7", ListReCheckDatas);//審批
                LvReCheck.ItemsSource = ListReCheckDatas;
                //LvReCheck.MouseDoubleClick += LvReCheck_MouseDoubleClick;

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitApprovalColumns()
        {
            string[] lans = "3105T00072,3105T00001,3105T00048,3105T00014,3105T00030,3105T00050".Split(',');
            string[] cols = "RowNumber,RecoredReference,Score,AgentName,AppealDatetime,OperationTime".Split(',');
            int[] colwidths = { 50, 150, 75, 120, 150, 150 };
            GridView columngv = new GridView();
            for (int i = 0; i < 6; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LvReCheck.View = columngv;
        }
        private void InitApprovaledColumns()
        {
            string[] lans = "3105T00072,3105T00073,3105T00001,3105T00048,3105T00037,3105T00014,3105T00030,3105T00049".Split(',');
            string[] cols = "RowNumber,AppealState,RecoredReference,Score,NewScore,AgentName,AppealDatetime,OperationTime".Split(',');
            int[] colwidths = { 50, 100, 150, 75, 75, 120, 150, 150 };
            GridView columngv = new GridView();
            for (int i = 0; i < 8; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LvApprovalList.View = columngv;
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-12-12 16:36:00
        /// <summary>
        /// 初始化GridView
        /// </summary>
        /// <param name="columnname">数据列名</param>
        /// <param name="ColumnGridView">GridView</param>
        /// <param name="langid">列名对应的语言包ID</param>
        /// <param name="diaplay">默认显示</param>
        /// <param name="datatemplate">列样式信息</param>
        /// <param name="width">列宽度</param>
        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-12-09 17:02:49
        /// <summary>
        /// Gets the check data by dept i ds.
        /// </summary>
        /// <param name="type">获取数据类型：1：复核数据，2：审批数据，3：无复核过程的审批数据</param>
        /// <param name="strcheck">The strcheck.</param>
        /// <param name="strrecheck">The strrecheck.</param>
        /// <param name="lstdata">The lstdata.</param>
        private void GetCheckDataByDeptIDs(string type, string strcheck, string strrecheck, ObservableCollection<CCheckData> lstdata)
        {
            string deptids = string.Empty;
            if (S3105App.lsAgentInfos.Count > 0)
            {
                deptids = SaveMultiAgent(S3105App.lsAgentInfos);
                deptids = string.Format("SELECT C011  FROM T_00_901 WHERE C001={0}", deptids);
            }
            if (deptids == "") { return; }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetCheckDatasOrRecheckDatas;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(deptids);
                webRequest.ListData.Add(strcheck);
                webRequest.ListData.Add(strrecheck);
                webRequest.ListData.Add(type);
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<CCheckData>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        CCheckData taskdetail = optReturn.Data as CCheckData;
                        if (taskdetail != null && !lstdata.Contains(taskdetail))
                        {
                            var curentagent = S3105App.ListCtrolAgentInfos.Where(p => p.AgentID == taskdetail.AgentID).ToList();
                            if (curentagent.Count > 0)
                                taskdetail.AgentName = curentagent.First().AgentName;
                            else
                                taskdetail.AgentName = "";
                            taskdetail.Score = taskdetail.Score;
                            lstdata.Add(taskdetail);
                        }
                    }
                }
                CurrentApp.WriteLog("審批Sql", string.Format("{0}", webReturn.Message));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void ApprovalHistory()
        {
            string deptids = string.Empty;
            if (S3105App.lsAgentInfos.Count > 0)
            {
                deptids = SaveMultiAgent(S3105App.lsAgentInfos);
                deptids = string.Format("SELECT C011  FROM T_00_901 WHERE C001={0}", deptids);
            }
            if (deptids == "")
            {
                return;
            }
            InitApprovaledColumns();
            ListApprovalHistoryDatas.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetAppealProcessHistory;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(deptids);
                webRequest.ListData.Add("5,6,7");
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<CCheckData>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        CCheckData taskdetail = optReturn.Data as CCheckData;
                        if (taskdetail != null && !ListApprovalHistoryDatas.Contains(taskdetail))
                        {
                            var curentagent = S3105App.ListCtrolAgentInfos.Where(p => p.AgentID == taskdetail.AgentID).ToList();
                            if (curentagent.Count > 0)
                                taskdetail.AgentName = curentagent.First().AgentName;
                            else
                                taskdetail.AgentName = "";
                            taskdetail.Score = taskdetail.Score;
                            taskdetail.AppealState =
                                CurrentApp.GetLanguageInfo(string.Format("3105T000{0}", 77 + taskdetail.AppealInt),
                                    taskdetail.AppealInt.ToString());
                            taskdetail.NewScore = GetNewScore(taskdetail);
                            ListApprovalHistoryDatas.Add(taskdetail);
                        }
                    }
                }
                CurrentApp.WriteLog("审批历史Sql", string.Format("{0}", webReturn.Message));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string GetNewScore(CCheckData tempData)
        {
            string result = string.Empty;
            if (tempData.AppealInt != 6 || string.IsNullOrWhiteSpace(tempData.RecoredReference) || string.IsNullOrWhiteSpace(tempData.TemplateID)) { return string.Empty; }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetNewScore;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(tempData.RecoredReference);
                webRequest.ListData.Add(tempData.TemplateID);
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                //Service31051Client client = new Service31051Client();
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    //ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(webReturn.Data))
                {
                    result = webReturn.Data;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return string.Empty;
            }
            return result;
        }

        private void LvApprovalList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PanelMemo.IsVisible = false;
            PanelPlayBox.IsVisible = false;
            var item = LvApprovalList.SelectedItem as CCheckData;
            if (item != null)
            {
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Play).ToList().Count > 0)//拥有播放权限
                    PlayRecord(item);
            }
        }


        private void LvReCheck_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowPlayAndMemoPanel(LvReCheck);
        }
        private void ShowPlayAndMemoPanel(ListView lv)
        {
            var item = lv.SelectedItem as CCheckData;
            if (item != null)
            {
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Memo).ToList().Count > 0)//拥有备注权限
                    ShowMemo(item);
                if (((S3105App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3105Consts.OPT_Play).ToList().Count > 0)//拥有播放权限
                    PlayRecord(item);
            }
        }

        private void ShowMemo(CCheckData cditem)
        {
            mUCRecordMemo = new UCRecordMemo();
            mUCRecordMemo.CurrentApp = CurrentApp;
            mUCRecordMemo.RecoredReference = cditem.RecoredReference;
            BorderMemo.Child = mUCRecordMemo;
            PanelMemo.IsVisible = true;
        }

        private void PlayRecord(CCheckData seldate)
        {
            try
            {
                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.ParentPage3 = this;
                mUCPlayBox.RecoredReference = seldate.RecoredReference;
                mUCPlayBox.IsAutoPlay = true;
                BorderPlayBox.Child = mUCPlayBox;
                PanelPlayBox.IsVisible = true;

                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), seldate.RecoredReference);
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
        private string SaveMultiAgent(List<string> listAgents)
        {
            string selectID = string.Empty;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 23;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(listAgents.Count.ToString());
                for (int i = 0; i < listAgents.Count; i++)
                {
                    webRequest.ListData.Add(listAgents[i]);
                }
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);

                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                selectID = webReturn.Data;
                return selectID;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return string.Empty;
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
            try
            {
                base.ChangeLanguage();
                InitApprovalColumns();
                InitApprovaledColumns();
                CreateOptButtons();

                #region panel
                var panel = GetPanleByContentID("PanelReCheck");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00007", "Approval");
                }
                panel = GetPanleByContentID("PanelApprovalList");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3105T00094", "Approval Result List");
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
                #endregion

                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("3105T00005", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("3105T00006", "Other Position");

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
