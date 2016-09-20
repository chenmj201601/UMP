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
using Common3107;
using UMPS3107.Wcf31071;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3107
{
    /// <summary>
    /// QueryConditionMainView.xaml 的交互逻辑
    /// </summary>
    public partial class QueryConditionMainView
    {

        #region Members
        private BackgroundWorker mWorker;
        private ObservableCollection<QuerySettingItems> ListQueryItems = new ObservableCollection<QuerySettingItems>();
        private ObservableCollection<TaskSettingItems> ListTaskItems = new ObservableCollection<TaskSettingItems>();


        #endregion

        public QueryConditionMainView()
        {
            InitializeComponent();

            mWorker = new BackgroundWorker();
            this.Loaded += QueryConditionPage_Loaded;
            LvQueryCondition.SelectionChanged += LvQueryCondition_SelectionChanged;
            LvQueryCondition.ItemsSource = ListQueryItems;
            LvTaskDetail.SelectionChanged += LvTaskDetail_SelectionChanged;
            LvTaskDetail.ItemsSource = ListTaskItems;
        }

        void QueryConditionPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitQueryDetailColumns();
            InitQueryDetail();
            InitTaskDetailColumns();
            InitTaskDetail();
            CreatoptButtons();
        }


        #region 初始化 & 全局消息

        protected override void Init()
        {
            try
            {
                PageName = "QueryConditionSetting";
                StylePath = "UMPS3107/MainPageStyle.xaml";
                base.Init();

                CurrentApp.SendLoadedMessage();
                ChangeTheme();
                ChangeLanguage();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitQueryDetailColumns()
        {
            try
            {
                string[] lans = "3107T00008,3107T00009,3107T00010,3107T00013,3107T00014,3107T00015,3107T00016,3107T00017,3107T00020,3107T00021".Split(',');
                string[] cols = "RowNumber,QuerySettingName,strUsed,QueryStartTime,QueryStopTime,StartRecordTime,StopRecordTime,StrCall,StrAssT,AgentAssNum".Split(',');
                int[] colwidths = { 40, 150, 60, 140, 140, 140, 140, 60, 110, 100 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 10; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (i >= 3 && i <= 6)
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
                LvQueryCondition.View = ColumnGridView;
            }
            catch (Exception)
            {

            }
        }

        public void InitQueryDetail()
        {
            ListQueryItems.Clear();
            try
            {
                //获取查询设置参数表
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.GetQueryDetail;
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<QuerySettingItems>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    QuerySettingItems queryItem = optReturn.Data as QuerySettingItems;
                    if (queryItem == null)
                    {
                        ShowException(string.Format("Fail. queryItem is null"));
                        return;
                    }
                    queryItem.RowNumber = i + 1;
                    queryItem.strUsed = queryItem.IsUsed == "Y" ? CurrentApp.GetLanguageInfo("3107T00011", queryItem.IsUsed) : CurrentApp.GetLanguageInfo("3107T00012", queryItem.IsUsed);
                    switch (queryItem.CallDirection)
                    {
                        case "2":
                            queryItem.StrCall = CurrentApp.GetLanguageInfo("3107T00033", queryItem.CallDirection);
                            break;
                        case "0"://呼出
                            queryItem.StrCall = CurrentApp.GetLanguageInfo("3107T00035", queryItem.CallDirection);
                            break;
                        case "1":
                            queryItem.StrCall = CurrentApp.GetLanguageInfo("3107T00034", queryItem.CallDirection);
                            break;
                    }
                    queryItem.StrAssT = CurrentApp.GetLanguageInfo(string.Format("3107T0004{0}", queryItem.AgentAssType), queryItem.AgentAssType.ToString());
                    ListQueryItems.Add(queryItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitTaskDetailColumns()
        {
            try
            {
                string[] lans = "3107T00008,3107T00023,3107T00024,3107T00010,3107T00009,3107T00025,3107T00026,3107T00027".Split(',');
                string[] cols = "RowNumber,AutoTaskName,StrTaskType,StrStatus,QueryName,TaskDeadline,StrRunFreq,DayTime".Split(',');
                int[] colwidths = { 40, 150, 100, 60, 180, 80, 100, 100 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 8; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    ColumnGridView.Columns.Add(gvc);
                }
                LvTaskDetail.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitTaskDetail()
        {
            ListTaskItems.Clear();
            try
            {
                //获取查询设置参数表
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.GetTaskDetail;
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<TaskSettingItems>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    TaskSettingItems taskItem = optReturn.Data as TaskSettingItems;
                    if (taskItem == null)
                    {
                        ShowException(string.Format("Fail. taskItem is null"));
                        return;
                    }
                    taskItem.RowNumber = i + 1;
                    taskItem.StrTaskType = CurrentApp.GetLanguageInfo(string.Format("3107T0004{0}", taskItem.TaskType + 3), taskItem.TaskType.ToString());
                    taskItem.StrStatus = taskItem.Status == "Y" ? CurrentApp.GetLanguageInfo("3107T00011", taskItem.Status) : CurrentApp.GetLanguageInfo("3107T00012", taskItem.Status);
                    switch (taskItem.RunFreq)
                    {
                        case "D":
                            taskItem.StrRunFreq = CurrentApp.GetLanguageInfo("3107T00072", taskItem.RunFreq);
                            break;
                        case "W":
                            taskItem.StrRunFreq = CurrentApp.GetLanguageInfo("3107T00060", taskItem.RunFreq);
                            break;
                        case "M":
                            taskItem.StrRunFreq = CurrentApp.GetLanguageInfo("3107T00061", taskItem.RunFreq);
                            break;
                        case "S":
                            taskItem.StrRunFreq = CurrentApp.GetLanguageInfo("3107T00062", taskItem.RunFreq);
                            break;
                        case "Y":
                            taskItem.StrRunFreq = CurrentApp.GetLanguageInfo("3107T00063", taskItem.RunFreq);
                            break;
                    }
                    ListTaskItems.Add(taskItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region 样式&语言
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
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3107;component/Themes/{0}/{1}",
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
                string uri = string.Format("/UMPS3107;component/Themes/Default/UMPS3107/FormStyle.xaml");
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
                string uri = string.Format("/UMPS3107;component/Themes/Default/UMPS3107/AvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }


        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                    "Query Condition Configuration");
                
                CreatoptButtons();
                InitQueryDetailColumns();
                InitTaskDetailColumns();

                PanelQuerySettingList.Title = CurrentApp.GetLanguageInfo("3107T00006", "Query Setting List");
                PanelTaskSettingList.Title = CurrentApp.GetLanguageInfo("3107T00007", "Task Setting List");

                //Other
                ExpQueryOpt.Header = CurrentApp.GetLanguageInfo("3107T00001", "Query Setting Operations");
                ExpTaskOpt.Header = CurrentApp.GetLanguageInfo("3107T00002", "Task Setting Position");
            }
            catch (Exception)
            {
            }
        }
        #endregion


        void LvQueryCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuerySettingItems Item = LvQueryCondition.SelectedItem as QuerySettingItems;
            if (Item != null)
            {
                CreatoptButtons();
            }
        }

        void LvTaskDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskSettingItems Item = LvTaskDetail.SelectedItem as TaskSettingItems;
            if (Item != null)
            {
                CreatoptButtons();
            }
        }


        private void CreatoptButtons()
        {
            try
            {
                #region 查询
                PanelBasicOpts.Children.Clear();
                Button btn;
                OperationInfo opt;
                btn = new Button();
                btn.Click += BasicOpt_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3107T00003", "New");
                opt.ID = S3107Consts.WDE_QueryNew;
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);

                if (LvQueryCondition.SelectedItem != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3107T00004", "Modify");
                    opt.ID = S3107Consts.WDE_QueryUpdate;
                    opt.Icon = "Images/modify.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }

                btn = new Button();
                btn.Click += BasicOpt_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3107T00005", "Delete");
                opt.ID = S3107Consts.WDE_QueryDelete;
                opt.Icon = "Images/Delete.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
                #endregion

                #region 任务
                PanelTakOpts.Children.Clear();
                btn = new Button();
                btn.Click += TaskOpt_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3107T00003", "New");
                opt.ID = S3107Consts.WDE_TaskNew;
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelTakOpts.Children.Add(btn);

                if (LvTaskDetail.SelectedItem != null)
                {
                    btn = new Button();
                    btn.Click += TaskOpt_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3107T00004", "Modify");
                    opt.ID = S3107Consts.WDE_TaskUpdate;
                    opt.Icon = "Images/modify.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelTakOpts.Children.Add(btn);
                }

                btn = new Button();
                btn.Click += TaskOpt_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3107T00005", "Delete");
                opt.ID = S3107Consts.WDE_TaskDelete;
                opt.Icon = "Images/Delete.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelTakOpts.Children.Add(btn);

                #endregion
            }
            catch (Exception ex)
            {

            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    QuerySettingItems mCurrentQueryItem;
                    switch (optItem.ID)
                    {
                        case S3107Consts.WDE_QueryNew:
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("3107T00028", "QuerySetting");
                            S3107App.QueryModify = false;
                            QuerySetting querySetting = new QuerySetting();
                            querySetting.CurrentApp = CurrentApp;
                            querySetting.ParentPage = this;
                            PopupPanel.Content = querySetting;
                            PopupPanel.IsOpen = true;
                            break;
                        case S3107Consts.WDE_QueryUpdate:
                            S3107App.QueryModify = true;
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("3107T00028", "QuerySetting");
                            mCurrentQueryItem = (QuerySettingItems)LvQueryCondition.SelectedItem;
                            if (mCurrentQueryItem == null)
                            {
                                CreatoptButtons();
                                return;
                            }
                            querySetting = new QuerySetting();
                            querySetting.CurrentApp = CurrentApp;
                            querySetting.ParentPage = this;
                            querySetting.QueryItem = mCurrentQueryItem;
                            PopupPanel.Content = querySetting;
                            PopupPanel.IsOpen = true;
                            break;
                        case S3107Consts.WDE_QueryDelete:
                            mCurrentQueryItem = (QuerySettingItems)LvQueryCondition.SelectedItem;
                            if (mCurrentQueryItem == null)
                            {
                                return;
                            }
                            DeleteDBO(mCurrentQueryItem);
                            break;
                    }
                }
            }
        }

        void TaskOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    TaskSettingItems mCurrentTaskItem;
                    switch (optItem.ID)
                    {
                        case S3107Consts.WDE_TaskNew:
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("3107T00053", "TaskSetting");
                            S3107App.TaskModify = false;
                            TaskSetting taskSetting = new TaskSetting();
                            taskSetting.CurrentApp = CurrentApp;
                            taskSetting.ParentPage = this;
                            taskSetting.queryItemList = new List<QuerySettingItems>();
                            for (int i = 0; i < ListQueryItems.Count(); i++)
                            {
                                if (ListQueryItems[i].IsUsed == "Y")
                                {
                                    taskSetting.queryItemList.Add(ListQueryItems[i]);
                                }
                            }
                            PopupPanel.Content = taskSetting;
                            PopupPanel.IsOpen = true;
                            break;
                        case S3107Consts.WDE_TaskUpdate:
                            S3107App.TaskModify = true;
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("3107T00053", "TaskSetting");
                            mCurrentTaskItem = (TaskSettingItems)LvTaskDetail.SelectedItem;
                            if (mCurrentTaskItem == null)
                            {
                                CreatoptButtons();
                                return;
                            }
                            taskSetting = new TaskSetting();
                            taskSetting.CurrentApp = CurrentApp;
                            taskSetting.ParentPage = this;
                            taskSetting.queryItemList = new List<QuerySettingItems>();
                            for (int i = 0; i < ListQueryItems.Count(); i++)
                            {
                                if (ListQueryItems[i].IsUsed == "Y")
                                {
                                    taskSetting.queryItemList.Add(ListQueryItems[i]);
                                }
                            }
                            taskSetting.TaskItems = mCurrentTaskItem;
                            PopupPanel.Content = taskSetting;
                            PopupPanel.IsOpen = true;
                            break;
                        case S3107Consts.WDE_TaskDelete:
                            mCurrentTaskItem = (TaskSettingItems)LvTaskDetail.SelectedItem;
                            if (mCurrentTaskItem == null)
                            {
                                return;
                            }
                            DeleteDBO(mCurrentTaskItem);
                            break;
                    }
                }
            }
        }

        void DeleteDBO(QuerySettingItems Item)
        {
            try
            {
                string strLog;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.DeleteDBO;
                webRequest.ListData.Add(Item.QuerySettingID.ToString());
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                //Service31071Client client = new Service31071Client();
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                    #region 写操作日志
                    strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00028"), Item.QuerySettingName);
                    CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    return;
                }
                if (webReturn.Message == S3107Consts.HadUse)// 该查询条件被使用无法删除
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00093", "Can't Delete"));
                    return;
                }
                else
                {
                    ListQueryItems.Remove(Item);
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00091", "Delete Sucessed"));
                    CreatoptButtons();
                    #region 写操作日志
                    strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00028"), Item.QuerySettingName);
                    CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DeleteDBO(TaskSettingItems Item)
        {
            try
            {
                string strLog;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.DeleteDBO;
                webRequest.ListData.Add(Item.TaskSettingID.ToString());
                webRequest.ListData.Add(Item.FrequencyID.ToString());
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                    #region 写操作日志
                    strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00053"), Item.AutoTaskName);
                    CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    return;
                }
                ListTaskItems.Remove(Item);
                CreatoptButtons();
                ShowInformation(CurrentApp.GetLanguageInfo("3107T00091", "Delete Sucessed"));
                #region 写操作日志
                strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00053"), Item.AutoTaskName);
                CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
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
