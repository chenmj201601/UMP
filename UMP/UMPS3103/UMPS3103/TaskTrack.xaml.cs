using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Controls;
using UMPS3103.Commands;
using VoiceCyber.UMP.Communications;
using UMPS3103.Wcf31031;
using System.Timers;
using UMPS3103.DoubleTask;


namespace UMPS3103
{
    /// <summary>
    /// TaskTrack.xaml 的交互逻辑
    /// </summary>
    public partial class TaskTrack 
    {
        //创建按钮
        private ObservableCollection<OperationInfo> mListBasicOperations;
        //异步线程
        private BackgroundWorker mWorker;

        public static ObservableCollection<UserTasksInfoShow> ListCurrentUserTasks = new ObservableCollection<UserTasksInfoShow>();
        private TaskRecordDetail mPageTaskRecordDetail = null;


        public TaskTrack()
        {
            InitializeComponent();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            LVTaskList.ItemsSource = ListCurrentUserTasks;
            LVTaskList.SelectionChanged += LVTaskList_SelectedItemChanged;
        }

        protected override void Init()
        {
            try
            {
                PageName = "UMP TaskTrack";
                StylePath = "UMPS3103/MainPageStyle.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                InitOperationButton();
                AddQueryBtnOperation();
                BindCommands();
                InitTaskColumns();
                GetCurrentOperationFromApp(S3103Consts.OPT_TASKTRACK);
                string dtstart = DateTime.Now.AddMonths(-1).ToUniversalTime().ToString();
                string dtend = DateTime.Now.AddMonths(1).ToUniversalTime().ToString();
                InitTasks("2", "0", string.Empty, string.Empty);//默认显示所有未完成的任务记录
                //触发状态栏动画
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //具体加载任务数据的方法
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, "");

                    ChangeTheme();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        void AddQueryBtnOperation()
        {
            OperationInfo itemquery = new OperationInfo();
            itemquery.ID = S3103Consts.OPT_TASKQUERY;
            itemquery.ParentID = 1102;
            itemquery.SortID = 0;
            itemquery.Icon = "Images/search.png";
            itemquery.Display = CurrentApp.GetLanguageInfo("3103T00133", "Query Task");
            itemquery.Description = null;
            mListBasicOperations.Add(itemquery);
        }
        
        /// <summary>
        /// 获取跳轉初檢、复检权限
        /// </summary>
        private void InitOperationButton()
        {
            PanelOherOpts.Children.Clear();
            Button btn;
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKASSIGN).Count() > 0) //初檢權限
            {
                btn = new Button();
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                var opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00107", "Move To Task Assign");
                opt.Icon = "Images/controltarget.png";
                btn.DataContext = opt;
                btn.Click += btn2TaskTrack_Click_1;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOherOpts.Children.Add(btn);
            }
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_DoubleTask).Count() > 0) //複檢權限
            {
                btn = new Button();
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                var opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00148", "Move To Recheck Task Assign");
                opt.Icon = "Images/controltarget.png";
                btn.DataContext = opt;
                btn.Click += btn2RechckTaskTrack_Click_1;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOherOpts.Children.Add(btn);
            }
        }

        /// <summary>
        /// 加载当前用户的任务信息(分出的任务以及自己的任务)
        /// </summary>
        public void InitTasks(string assign,string finished,string start,string end)
        {
            try
            {
                string tasktype = "0";
                if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_CheckTask).Count() > 0) //初檢權限
                {
                    tasktype += string.Format(",1,2");
                }
                if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_RecheckTask).Count() > 0) //複檢權限
                {
                    tasktype += string.Format(",3,4");
                }

                ListCurrentUserTasks.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetCurrentUserTasks;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(assign);//0:我分配的 1:分配给我的 2:全部
                webRequest.ListData.Add(finished);//0:未完成 1:已完成 2全部
                webRequest.ListData.Add(tasktype); //初檢 1；複檢 3
                webRequest.ListData.Add(start); //开始时间
                webRequest.ListData.Add(end); //截止时间

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
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
                        OperationReturn optReturn = XMLHelper.DeserializeObject<UserTasksInfoShow>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        UserTasksInfoShow taskInfo = optReturn.Data as UserTasksInfoShow;
                        if (taskInfo != null && ListCurrentUserTasks.Where(p => p.TaskID == taskInfo.TaskID).Count()==0)
                        {
                            //if (ListCurrentUserTasks.Where(p => p.TaskID == taskInfo.TaskID).Count() > 0 && taskInfo.FinishUserID==0)
                            //{ 
                            //}
                            long tempdur = taskInfo.TaskAllRecordLength;
                            taskInfo.TaskTypeName = GetTaskTypeName(taskInfo.TaskType);
                            taskInfo.IsFinish = taskInfo.IsFinish == "Y" ? CurrentApp.GetLanguageInfo("3103T00019", "Finished") : CurrentApp.GetLanguageInfo("3103T00020", "Unfinished");
                            taskInfo.IsShare = taskInfo.IsShare == "Y" ? CurrentApp.GetLanguageInfo("3103T00022", "Yes") : CurrentApp.GetLanguageInfo("3103T00023", "No");
                            taskInfo.strTaskAllRecordLength = string.Format("{0:00}:{1:00}:{2:00}", tempdur / 3600, (tempdur % 3600) / 60, tempdur % 60);
                            ListCurrentUserTasks.Add(taskInfo);
                        }
                    }
                }
                CurrentApp.WriteLog("TaskQuery", string.Format("{0}", webReturn.Message));
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        //任务类别 1 初检手任务 2初检自动任务，3复检手动任务 4复检自动任务 5推荐录音初检 6推荐录音复检  7 QA质检任务（质检但不计入座席成绩） 8智能任务分配
        private string GetTaskTypeName(int type)
        { 
            string tname = "";
            switch (type)
            { 
                case 1:
                    tname = CurrentApp.GetLanguageInfo("3103T00137", "1st Level Manual-task");
                    break;
                case 2:
                    tname = CurrentApp.GetLanguageInfo("3103T00139", "1st Level Auto-task");
                    break;
                case 3:
                    tname = CurrentApp.GetLanguageInfo("3103T00138", "Calibration Manual-task");                    
                    break;
                case 4:
                    break;
                case 5:
                    tname = CurrentApp.GetLanguageInfo("3103T00140", "Recommend Record first examine"); 
                    break;
                case 6:
                    tname = CurrentApp.GetLanguageInfo("3103T00141", "Recommend Record Double Check"); 
                    break;
                case 7:
                    tname = CurrentApp.GetLanguageInfo("3103T00142", "Quality inspection tasks");
                    break;
                case 8:
                    tname = CurrentApp.GetLanguageInfo("3103T00143", "Intelligent tasks");
                    break;
            }
            return tname;
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 初始化任务列
        /// </summary>
        private void InitTaskColumns()
        {
            string[] lans = "3103T00001,3103T00002,3103T00004,3103T00005,3103T00006,3103T00007,3103T00008,3103T00009,3103T00010,3103T00011,3103T00014,3103T00012,3103T00015,3103T00016,3103T00017,3103T00021".Split(',');
            string[] cols = "TaskName,TaskDesc,strTaskAllRecordLength,TaskTypeName,IsShare,AssignTime,AssignUserFName,AssignNum,DealLine,AlreadyScoreNum,ModifyTime,ModifyUserFName,BelongYear,BelongMonth,IsFinish,FinishTime".Split(',');
            int[] colwidths = { 120, 120, 100, 140, 75, 150, 100, 75, 150, 100, 150, 120, 50, 50, 75, 150 };
            GridView columngv = new GridView();
            for (int i = 0; i < 16; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LVTaskList.View = columngv;
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
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

        /// <summary>
        /// 点击任务记录 生成对应按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LVTaskList_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mListBasicOperations.Clear();
                AddQueryBtnOperation();
                var CurrentFocusRecord = new UserTasksInfoShow();
                CurrentFocusRecord = (UserTasksInfoShow)LVTaskList.SelectedItem;
                DateTime deadline=Convert.ToDateTime(CurrentFocusRecord.DealLine);
                if (CurrentFocusRecord != null)
                {
                    if (CurrentFocusRecord.AssignUser == CurrentApp.Session.UserID)//分配人是当前用户
                    {
                        if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_MODIFYTASK).ToList().Count > 0)//拥有修改权限
                        {
                            OperationInfo item = new OperationInfo();
                            item.ID = S3103Consts.OPT_MODIFYTASK;
                            item.ParentID = 1102;
                            item.SortID = 0;
                            item.Icon = "Images/modify.png";
                            item.Display = CurrentApp.GetLanguageInfo("3103T00034", "Change Expiration Date");
                            item.Description = null;
                            mListBasicOperations.Add(item);
                        }
                    }
                    if (CurrentFocusRecord.FinishUserID == CurrentApp.Session.UserID && DateTime.Compare(deadline,DateTime.Now)>=0)//任务人是当前用户并且任务没有过有效期
                    {
                        if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_VIEWTASKDETAIL).ToList().Count > 0)//拥有查看任务权限
                        {
                            OperationInfo item1 = new OperationInfo();
                            item1.ID = S3103Consts.OPT_VIEWTASKDETAIL;
                            item1.ParentID = 1102;
                            item1.SortID = 0;
                            item1.Icon = "Images/add.png";
                            item1.Display = CurrentApp.GetLanguageInfo("3103T00039", "View Task");
                            item1.Description = null;
                            mListBasicOperations.Add(item1);
                            LVTaskList.MouseDoubleClick += LVTaskList_MouseDoubleClick;
                        }
                    } 
                }
                else
                {
                    return;
                }
                CreateOptButtons();
            }
            catch (Exception )
            {                
            }
        }

        //目前存在下面BUG：选中一条可以评分的任务，然后双击不能评分的任务居然就进入任务详情界面了。
        //原因：先运行的是SelectedItemChanged事件,而当前选中的是前一条被选中的任务，所以就给评分权限了，然后再运行的是MouseDoubleClick事件，然后再将被选中的第二条任务设为SelectedItem。
        void LVTaskList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserTasksInfoShow tempSelected = LVTaskList.SelectedItem as UserTasksInfoShow;
            if (tempSelected == null) { return; }
            DateTime deadline = Convert.ToDateTime(tempSelected.DealLine);
            if (tempSelected.FinishUserID == CurrentApp.Session.UserID)
            {
                if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_VIEWTASKDETAIL).ToList().Count > 0 && DateTime.Compare(deadline, DateTime.Now) >= 0)//拥有查看任务权限
                {
                    Jump2TaskDetail(false);
                }
            }
        }
        
        private void GetCurrentOperationFromApp(long CurrentOperationID)
        {
            if (S3103App.ListOperationInfos.Where(p => p.ID == CurrentOperationID).Count() > 0)
            {
                S3103App.ListOperationInfos.Add(S3103App.ListOperationInfos.Where(p => p.ID == CurrentOperationID).First());

                List<OperationInfo> lstOpTemp = S3103App.ListOperationInfos.Where(p => p.ParentID == CurrentOperationID).ToList();
                if (lstOpTemp != null && lstOpTemp.Count>0)
                {
                    foreach (OperationInfo o in lstOpTemp)
                    {
                        GetCurrentOperationFromApp(o.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 根据权限生成操作按钮
        /// </summary>
        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            Button btn;
            OperationInfo item;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                if (item.ID == S3103Consts.OPT_MODIFYTASK)//修改任务权限
                    item.Display = CurrentApp.GetLanguageInfo("3103T00073", "Modify Task");
                else if (item.ID == S3103Consts.OPT_VIEWTASKDETAIL)//查看任务权限
                    item.Display = CurrentApp.GetLanguageInfo("3103T00039", "View Task");
                else if (item.ID == S3103Consts.OPT_TASKQUERY)//任务查询
                    item.Display = CurrentApp.GetLanguageInfo("3103T00133", "Query Task");
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                //各操作按纽触发按钮方法 
                var optItem = btn.DataContext as OperationInfo;
                switch (optItem.ID)
                {
                    case S3103Consts.OPT_MODIFYTASK://調整任務錄音
                        Jump2TaskDetail(true);
                        break;
                    case S3103Consts.OPT_VIEWTASKDETAIL://查看任務錄音
                        Jump2TaskDetail(false);
                        break;
                    case S3103Consts.OPT_TASKQUERY:
                        QueryTask();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 任务查询
        /// </summary>
        private void QueryTask()
        {
            PopupTaskSearch.Title = CurrentApp.GetLanguageInfo("3103T00133", "Query Task");
            UCTaskQueryCondition taskquery = new UCTaskQueryCondition();
            taskquery.CurrentApp = CurrentApp;
            taskquery.PageParent = this;
            PopupTaskSearch.Content = taskquery;
            PopupTaskSearch.IsOpen = true;
        }

        /// <summary>
        /// 跳转到任务完成页面
        /// </summary>
        private void Jump2TaskDetail(bool hasModifyPremission)
        {
            UserTasksInfoShow CurrentFocusRecord = new UserTasksInfoShow();
            CurrentFocusRecord = (UserTasksInfoShow)LVTaskList.SelectedItem;
            if (CurrentFocusRecord != null)
            {
                #region 写操作日志
                string lancode = hasModifyPremission ? "3103T00073" : "3103T00039";
                string opid = hasModifyPremission ? S3103Consts.OPT_MODIFYTASK.ToString() : S3103Consts.OPT_VIEWTASKDETAIL.ToString();
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString(lancode), CurrentFocusRecord.TaskName);
                CurrentApp.WriteOperationLog(opid, ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                mPageTaskRecordDetail = new TaskRecordDetail();
                mPageTaskRecordDetail.PageParent = this;
                CurrentApp.CurrentView = mPageTaskRecordDetail;
                mPageTaskRecordDetail.PageName = "TaskRecordDetail";
                TaskRecordDetail.SelectTask = CurrentFocusRecord;
                TaskRecordDetail.IsModify = hasModifyPremission;
                var temp = CurrentApp as S3103App;
                if (temp != null)
                {
                    temp.InitMainView(mPageTaskRecordDetail);
                }
            }
        }

        //梆定操作命令
        private void BindCommands()
        {
            CommandBindings.Add(
                new CommandBinding(URMainPageCommands.ModifyTaskFinishTimeCommand,
                    ModifyTaskFinishTimeCommand_Executed,
                    (s, e) => e.CanExecute = true));
        }

        /// <summary>
        /// 修改任务完成时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyTaskFinishTimeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //具体的修改任务完成时间代码
        }
        
        #region 按钮操作及生成
        private void btn2TaskTrack_Click_1(object sender, RoutedEventArgs e)
        {
            var temp = CurrentApp as S3103App;
            TaskAssign taskAssignView=new TaskAssign();
            CurrentApp.CurrentView = taskAssignView;
            taskAssignView.PageName = "taskAssignView";
            if (temp != null)
            {
                temp.InitMainView(taskAssignView);
                //temp.InitMainView(0);
            }
        }
        private void btn2RechckTaskTrack_Click_1(object sender, RoutedEventArgs e)
        {
            S3103App.NativePageFlag = false;
            var temp = CurrentApp as S3103App;
            DoubleTaskAssign doubleTaskView = new DoubleTaskAssign();
            CurrentApp.CurrentView = doubleTaskView;
            doubleTaskView.PageName = "DoubleTaskAssign";
            if (temp != null)
            {
                temp.InitMainView(doubleTaskView);
                //temp.InitMainView(0);
            }
        }
        #endregion

        #region 页头命令


        void SetTaskLanguage()
        {
            foreach (UserTasksInfoShow taskInfo in ListCurrentUserTasks)
            {
                taskInfo.TaskTypeName = GetTaskTypeName(taskInfo.TaskType);
            }
        }


        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                   "Task Management");
                //Operation
                for (int i = 0; i < S3103App.ListOperationInfos.Count; i++)
                {
                    S3103App.ListOperationInfos[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3103App.ListOperationInfos[i].ID),
                        S3103App.ListOperationInfos[i].ID.ToString());
                }
                CreateOptButtons();
                InitTaskColumns();
                InitOperationButton();
                

                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("3103T00052", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("3103T00053", "Other Position");
                //列名
                SetTaskLanguage();
            }
            catch { }
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
                    string uri = string.Format("/UMPS3103;component/Themes/{0}/{1}",
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/MainPageStatic.xaml");
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/QMAvalonDock.xaml");
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
