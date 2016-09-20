using Common3107;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using UMPS3107.Models;
using UMPS3107.Wcf31071;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3107
{
    /// <summary>
    /// TaskSetting.xaml 的交互逻辑
    /// </summary>
    public partial class TaskSetting
    {

        public QueryConditionMainView ParentPage;
        private ObjectItem mRootQA;
        /// <summary>
        /// 獲取查詢參數表 List
        /// </summary>
        public List<QuerySettingItems> queryItemList;
        string strQAList = string.Empty;
        /// <summary>
        /// 任务设置详情
        /// </summary>
        public TaskSettingItems TaskItems;
        /// <summary>
        /// 任务运行时长比率
        /// </summary>
        public List<TaskDurationRate> TaskRate;
        List<DataGridInfo> dataGridList;
        /// <summary>
        /// 保存修改任務參數時獲取的QA信息
        /// </summary>
        public List<string> QAItems;
        
        public TaskSetting()
        {
            InitializeComponent();
            this.Loaded += TaskSetting_Loaded;
            QAItems = new List<string>();
            cbFreqWeekTime.SelectionChanged += cbFreqWeekTime_SelectionChanged;
            cbFreqMonthTime.SelectionChanged += cbFreqMonthTime_SelectionChanged;
            TvTaskObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
        }
        
        void TaskSetting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mRootQA = new ObjectItem();
                ChangeLanguage();
                InitCtrol();
                InitOrgAndQA(mRootQA, S3107App.CurrentOrg);
                TvTaskObjects.ItemsSource = mRootQA.Children;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            
        }
        void InitCtrol()
        {
            try
            {
                if (!S3107App.TaskModify)
                {
                    #region 任务设置
                    yesUse.IsChecked = true;
                    //yesShare.IsChecked = true;
                    //yesAverage.IsChecked = true;
                    //yesDispose.IsChecked = true;
                    ComboBoxItem comItem;
                    for(int i=0;i<queryItemList.Count();i++)
                    {
                        comItem = new ComboBoxItem();
                        comItem.Content = queryItemList[i].QuerySettingName;
                        cbTaskCondition.Items.Add(comItem);
                    }

                    #endregion

                    #region 分配比率
                    //时长分配比率
                    dataGridList = new List<DataGridInfo>();
                    dataGrid.ItemsSource = dataGridList;
                    #endregion

                    #region 运行周期
                    //运行时间初始化为本机当前时间
                    txRuntime.Text = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    #endregion

                }
                else//修改
                {
                    #region 任務設置
                    tbTaskName.Text = TaskItems.AutoTaskName;
                    cbTaskType.SelectedIndex = TaskItems.TaskType - 1;
                    tbTaskRemark.Text = GetStringFDB(TaskItems.AutoTaskDesc);
                    if (TaskItems.Status=="Y")//是否启用
                    {yesUse.IsChecked = true;}
                    else{noUse.IsChecked=true;}
                    //if(TaskItems.IsTaskShare=="Y")//是否共享
                    //{yesShare.IsChecked=true;}
                    //else{noShare.IsChecked=true;}
                    //if(TaskItems.IsTaskAVGAssign=="Y")//是否平均分配
                    //{yesAverage.IsChecked=true;}
                    //else{noAverage.IsChecked=true;}
                    //if(TaskItems.IsDisposed=="Y")//多余录音的处理办法
                    //{yesDispose.IsChecked=true;}
                    //else{noDispose.IsChecked=true;}

                    ComboBoxItem comItem;
                    for (int i = 0; i < queryItemList.Count(); i++)
                    {
                        comItem = new ComboBoxItem();
                        comItem.Content = queryItemList[i].QuerySettingName;
                        cbTaskCondition.Items.Add(comItem);
                    }
                    for(int i=0;i<queryItemList.Count();i++)
                    {
                        if(queryItemList[i].QuerySettingID==TaskItems.QueryID)
                        {
                            cbTaskCondition.SelectedIndex=i;
                            break;
                        }
                    }
                    tbDeadline.Text=TaskItems.TaskDeadline.ToString();

                    #region QA
                    strQAList = TaskItems.QMIDOne + TaskItems.QMIDTwo + TaskItems.QMIDThree;
                    string[] tempQA = strQAList.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    QAItems = new List<string>(tempQA);
                    #endregion

                    #endregion

                    #region 分配比率
                    dataGridList = new List<DataGridInfo>();
                    GetRateDetail();
                    if(TaskRate.Count>0)
                    {
                        ckDataGrid.IsChecked=true;
                        DataGridInfo temp;
                        foreach(TaskDurationRate Item in TaskRate)
                        {
                            temp=new DataGridInfo();
                            temp.DurationMin=Item.DurationMin;
                            temp.DurationMax=Item.DurationMax;
                            temp.Rate=Item.Rate;
                            dataGridList.Add(temp);
                        }
                    }
                    dataGrid.ItemsSource = dataGridList;
                    #endregion

                    #region 運行週期
                    switch(TaskItems.RunFreq)
                    {
                        case "D":
                            cbRunFreq.SelectedIndex=0;
                            break;
                        case "W":
                            cbRunFreq.SelectedIndex=1;
                            cbFreqWeekTime.SelectedIndex=TaskItems.DayOfWeek-1;
                            break;
                        case "M":
                            cbRunFreq.SelectedIndex=2;
                            cbFreqMonthTime.SelectedIndex=TaskItems.DayOfMonth-1;
                            break;
                        case "S":
                            cbRunFreq.SelectedIndex=3;
                            tbFirstRunSeasonTime.Text=TaskItems.UniteSetSeason.ToString();
                            break;
                        case "Y":
                            cbRunFreq.SelectedIndex=4;
                            tbFreqRunYearTime.Text=TaskItems.DayOfYear.ToString();
                            break;
                    }
                    txRuntime.Text=TaskItems.DayTime;
                    #endregion


                }
                //每月的日期初始化
                ComboBoxItem combItem;
                for (int i = 1; i <= 31; i++)
                {
                    combItem = new ComboBoxItem();
                    combItem.Content = string.Format("{0}{1}", i, CurrentApp.GetLanguageInfo("3107T00038", "日"));
                    cbFreqMonthTime.Items.Add(combItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        #region QA樹
        //更新座席分机数据
        void InitOrgAndQA(ObjectItem parentItem, string parentID)
        {
            List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
            lstCtrolOrgTemp = S3107App.ListCtrolOrgInfos.Where(p => p.OrgParentID == parentID).ToList();
            foreach (CtrolOrg org in lstCtrolOrgTemp)
            {
                ObjectItem item = new ObjectItem();
                item.ObjType = ConstValue.RESOURCE_ORG;
                item.ObjID = Convert.ToInt64(org.ID);
                item.Name = org.OrgName;
                item.Data = org;
                item.IsChecked = false;
                if (org.ID == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "/Themes/Default/UMPS3107/Images/rootorg.ico";
                }
                else
                {
                    item.Icon = "/Themes/Default/UMPS3107/Images/org.ico";
                }
                InitOrgAndQA(item, org.ID);
                InitControlQA(item, org.ID);
                AddChildObject(parentItem, item);
            }
        }

        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void InitControlQA(ObjectItem parentItem, string parentID)
        {
            try
            {
                List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                lstCtrolQATemp = S3107App.ListCtrolQAInfos.Where(p => p.OrgID == parentID).ToList();
                foreach (CtrolQA qa in lstCtrolQATemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Name = qa.UserName;
                    if (QAItems.Where(p => p == item.ObjID.ToString()).Count() > 0)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                    item.Description = qa.UserFullName;
                    item.Data = qa;
                    item.Icon = "/Themes/Default/UMPS3107/Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(500);//次线程休眠1秒
                Dispatcher.Invoke(new Action(() =>
                {
                    List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                    GetQAIsCheck(mRootQA, ref lstCtrolQATemp);
                    if (lstCtrolQATemp.Count > 0)
                    {
                        string stra = "";
                        foreach (CtrolQA ca in lstCtrolQATemp)
                        {
                            stra += ca.UserID + ",";
                        }
                        stra = stra.TrimEnd(',');
                        strQAList = stra;
                    }
                    else
                        strQAList=string.Empty;
                }));
            });
            t.Start();
        }

        private void GetQAIsCheck(ObjectItem parent, ref List<CtrolQA> lstCtrolQa)
        {
            //mRootQA
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ConstValue.RESOURCE_USER)
                {
                    CtrolQA ctrolqa = new CtrolQA();
                    ctrolqa = (CtrolQA)o.Data;
                    lstCtrolQa.Add(ctrolqa);
                }

                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetQAIsCheck(o, ref lstCtrolQa);
                }
            }

        }
        #endregion

        void GetRateDetail()
        {
            try
            {
                if (TaskItems.TaskSettingID <= 0) { return; }
                TaskRate = new List<TaskDurationRate>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.GetRateDetail;
                webRequest.ListData.Add(TaskItems.TaskSettingID.ToString());
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<TaskDurationRate>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    TaskDurationRate rateItem = optReturn.Data as TaskDurationRate;
                    if (rateItem == null)
                    {
                        ShowException(string.Format("Fail. rateItem is null"));
                        return;
                    }
                    TaskRate.Add(rateItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception)
            {

            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!CreateTaskCondition()){ return; }
                if (!TaskDBOperation()) { return; }
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    ParentPage.InitTaskDetail();
                    #region 写操作日志
                    string OpID = S3107App.TaskModify == true ? "3107T00004" : "3107T00003";
                    string strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString(OpID), Utils.FormatOptLogString("3107T00053"), TaskItems.AutoTaskName);
                    CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    parent.IsOpen = false;
                }
            }
            catch (Exception)
            {
                CurrentApp.WriteOperationLog(S3107Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, "");
            }
        }

        private bool CreateTaskCondition()
        {
            bool flag = true;
            if (!S3107App.TaskModify)
            {
                TaskItems = new TaskSettingItems();
            }
            try
            {
                
                #region 任务分配表 T_31_023
                if (string.IsNullOrWhiteSpace(tbTaskName.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00073", "Please Input TaskName"));
                    return false;
                }
                TaskItems.AutoTaskName = tbTaskName.Text;
                if (cbTaskType.SelectedItem==null)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00074", "Please Select TaskType"));
                    return false;
                }
                TaskItems.TaskType = cbTaskType.SelectedIndex + 1;
                TaskItems.AutoTaskDesc = tbTaskRemark.Text;
                TaskItems.Status = yesUse.IsChecked == true ? "Y" : "N";
                //TaskItems.IsTaskShare = yesShare.IsChecked == true ? "Y" : "N";
                TaskItems.IsTaskShare = "N";
                //TaskItems.IsTaskAVGAssign = yesAverage.IsChecked == true ? "Y" : "N";
                TaskItems.IsTaskAVGAssign = "Y";
                //TaskItems.IsDisposed = yesDispose.IsChecked == true ? "Y" : "N";
                TaskItems.IsDisposed = "Y";
                if (cbTaskCondition.SelectedItem==null)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00075", "Please Select QueryCondition")); 
                    return false;
                }
                TaskItems.QueryID = queryItemList[cbTaskCondition.SelectedIndex].QuerySettingID;

                //旬
                TaskItems.IsTaskPeriod = "N";
                TaskItems.IsDownGet = "N";

                //QA
                if (string.IsNullOrWhiteSpace(strQAList))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00076", "Please Select QA"));
                    return false;
                }
                if(strQAList.Length>6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00077", "QA Is Too Larger"));
                    return false;
                }
                if (strQAList.Length > 4000 && strQAList.Length < 6000)
                {
                    TaskItems.QMIDOne = strQAList.Substring(0, 2000);
                    TaskItems.QMIDTwo = strQAList.Substring(2000, 2000);
                    TaskItems.QMIDThree = strQAList.Substring(4000, strQAList.Length - 4);
                }
                if (strQAList.Length > 2000 && strQAList.Length <= 4000)
                {
                    TaskItems.QMIDOne = strQAList.Substring(0, 2000);
                    TaskItems.QMIDTwo = strQAList.Substring(2000, strQAList.Length - 2000);
                    TaskItems.QMIDThree = string.Empty;
                }
                if (strQAList.Length <= 2000)
                {
                    TaskItems.QMIDOne = strQAList;
                    TaskItems.QMIDTwo = string.Empty;
                    TaskItems.QMIDThree = string.Empty;
                }
                //过期时间
                if (string.IsNullOrWhiteSpace(tbDeadline.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00078", "Please Input TaskDeadline"));
                    return false;
                }
                int temp=Convert.ToInt32(tbDeadline.Text);
                if (temp > 500) { tbDeadline.Text = "500"; }
                if (temp <0) { tbDeadline.Text = "1"; }
                TaskItems.TaskDeadline =Convert.ToInt32(tbDeadline.Text);

                #endregion

                #region  运行周期 T_31_026
                if (cbRunFreq.SelectedItem==null)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00079", "Please Select RunFreq"));
                    return false;
                }
                switch(cbRunFreq.SelectedIndex)
                {
                    case 0:
                        TaskItems.RunFreq = "D";
                        break;
                    case 1:
                        if (cbFreqWeekTime.SelectedItem==null)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00080", "Please Input RunTime")); 
                            return false;
                        }
                        TaskItems.RunFreq = "W";
                        break;
                    case 2:
                        if (cbFreqMonthTime.SelectedItem==null)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00080", "Please Input RunTime")); 
                            return false;
                        }
                        TaskItems.RunFreq = "M";
                        break;
                    case 3:
                        if (string.IsNullOrWhiteSpace(tbFirstRunSeasonTime.Text))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00080", "Please Input RunTime"));
                            return false;
                        }
                        TaskItems.RunFreq = "S";
                        break;
                    case 4:
                        if (string.IsNullOrWhiteSpace(tbFreqRunYearTime.Text))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00080", "Please Input RunTime")); 
                            return false;
                        }
                        TaskItems.RunFreq = "Y";
                        break;                    
                }
                if (string.IsNullOrWhiteSpace(txRuntime.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00080", "Please Input RunTime"));
                    return false;
                }
                TaskItems.DayTime = txRuntime.Text;
                if(cbFreqWeekTime.SelectedItem!=null)
                {
                    TaskItems.DayOfWeek = cbFreqWeekTime.SelectedIndex + 1;
                }
                if(cbFreqMonthTime.SelectedItem!=null)
                {
                    TaskItems.DayOfMonth = cbFreqMonthTime.SelectedIndex + 1;
                }
                if(!string.IsNullOrWhiteSpace(tbFirstRunSeasonTime.Text))
                {
                    TaskItems.UniteSetSeason = Convert.ToInt32(tbFirstRunSeasonTime.Text);
                    if (TaskItems.UniteSetSeason < 1 || TaskItems.UniteSetSeason>92)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00081", "Please Input Right RunTime"));
                        return false;
                    }
                    TaskItems.IsUniteSetOfSeason = "N";
                }
                if(!string.IsNullOrWhiteSpace(tbFreqRunYearTime.Text))
                {
                    TaskItems.DayOfYear = Convert.ToInt32(tbFreqRunYearTime.Text);
                    if (TaskItems.DayOfYear < 1 || TaskItems.DayOfYear > 366)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00081", "Please Input Right RunTime"));
                        return false;
                    }
                }

                #endregion

                //时长分配比率 T_31_048  在插入数据库时获取自动任务分配ID时读取
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return flag;
        }

        private bool TaskDBOperation()
        {
            bool flag = true;
            WebRequest webRequest;
            Service31071Client client;
            WebReturn webReturn;
            try
            {
                if (!S3107App.TaskModify)//新建數據需要獲取任務分配ID、運行週期ID
                {
                    //生成新的任务分配ID
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("372");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        return false;
                    }
                    string strNewResultID = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultID))
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3107T00082", "strNewResultID1 Is Null"));
                        return false;
                    }
                    TaskItems.TaskSettingID = Convert.ToInt64(strNewResultID);

                    //生成新的运行周期ID
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("313");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        return false;
                    }
                    strNewResultID = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultID))
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3107T00082", "strNewResultID2 Is Null"));
                        return false;
                    }
                    TaskItems.FrequencyID = Convert.ToInt64(strNewResultID);
                }

                //往任务设置中插入用户信息
                TaskItems.Creator = CurrentApp.Session.UserID;
                TaskItems.CreatorName = CurrentApp.Session.UserInfo.UserName;

                #region  时长分配比率 T_31_048  在插入数据库时获取自动任务分配ID时读取
                
                TaskRate = new List<TaskDurationRate>();
                if (ckDataGrid.IsChecked == true)
                {
                    if (dataGrid.Items.Count < 0) { return false; }
                    TaskDurationRate rateItem;
                    double ratecount = 0.0;
                    foreach (DataGridInfo tempRate in dataGrid.ItemsSource)
                    {
                        rateItem = new TaskDurationRate();
                        rateItem.DurationMin = tempRate.DurationMin;
                        rateItem.DurationMax = tempRate.DurationMax;
                        if (tempRate.DurationMin>tempRate.DurationMax)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00085", "Please Input Right Duration"));
                            return false;
                        }
                        rateItem.Rate = tempRate.Rate;
                        rateItem.TaskSettingID = TaskItems.TaskSettingID;
                        ratecount += tempRate.Rate;
                        TaskRate.Add(rateItem);
                    }
                    if (ratecount!=100.0 ) 
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00085", "Please Input Right Duration"));
                        return false;
                    }

                }


                #endregion


                //往T_31_023、T_31_026、T_31_048中插入数据
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.TaskSettingDBO;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(TaskItems);//任务设置    0
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                string tempFlag = S3107App.TaskModify == true ? "T" : "F";
                webRequest.ListData.Add(tempFlag);//   1

                webRequest.ListData.Add(TaskRate.Count.ToString());//2
                if (TaskRate.Count > 0 && ckDataGrid.IsChecked == true)
                {
                    for (int i = 0; i < TaskRate.Count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(TaskRate[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return false;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());//时长比率 2+i
                    }
                }
                client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return flag;
        }

        //运行周期设置
        private void cbRunFreq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //每变动一下周期设置，都要清除每周、每月、每季、每年设置
            cbFreqWeekTime.SelectedItem = null;
            cbFreqMonthTime.SelectedItem = null;
            tbFirstRunSeasonTime.Text = string.Empty;
            tbFreqRunYearTime.Text = string.Empty;
            switch (cbRunFreq.SelectedIndex)
            {
                case 0:
                    cbFreqWeekTime.IsEnabled = false;
                    cbFreqMonthTime.IsEnabled = false;
                    tbFirstRunSeasonTime.IsEnabled = false;
                    tbFreqRunYearTime.IsEnabled=false;
                    break;
                case 1:
                    cbFreqWeekTime.IsEnabled = true;
                    cbFreqMonthTime.IsEnabled = false;
                    tbFirstRunSeasonTime.IsEnabled = false;
                    tbFreqRunYearTime.IsEnabled=false;
                    break;
                case 2:
                    cbFreqWeekTime.IsEnabled = false;
                    cbFreqMonthTime.IsEnabled = true;
                    tbFirstRunSeasonTime.IsEnabled = false;
                    tbFreqRunYearTime.IsEnabled=false;
                    break;
                case 3:
                    cbFreqWeekTime.IsEnabled = false;
                    cbFreqMonthTime.IsEnabled = false;
                    tbFirstRunSeasonTime.IsEnabled = true;
                    tbFreqRunYearTime.IsEnabled=false;
                    break;
                case 4:
                    cbFreqWeekTime.IsEnabled = false;
                    cbFreqMonthTime.IsEnabled = false;
                    tbFirstRunSeasonTime.IsEnabled = false;
                    tbFreqRunYearTime.IsEnabled = true;
                    break;
            }
        }

        void cbFreqWeekTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbFreqMonthTime.IsEnabled = false;
            int i = cbFreqWeekTime.SelectedIndex;
        }

        void cbFreqMonthTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbFreqWeekTime.IsEnabled = false;
            
        }

        /// <summary>
        /// 时间输入控制
        /// </summary>
        private void tbDeadline_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")//数字键盘区域
            {
                if (e.Key == Key.Decimal || txt.Text.Length > 4 || (txt.Text.Length == 0 && e.Key == Key.NumPad0))//排除掉"."符号，限制长度大于5,第一个字符不能为0
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)//字母键盘区域
            {
                if (e.Key == Key.OemPeriod || txt.Text.Length > 4 || (txt.Text.Length == 0 && e.Key == Key.D0))//排除掉"."符号，限制长度大于5,第一个字符不能为0
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                if (e.Key.ToString() != "RightCtrl")
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 对于值可能为null值的处理
        /// </summary>
        private string GetStringFDB(string temp)
        {
            if (string.IsNullOrWhiteSpace(temp)) temp = string.Empty;
            return temp;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            taskSetting.Header = CurrentApp.GetLanguageInfo("3107T00053", "Task Setting");
            allotRate.Header = CurrentApp.GetLanguageInfo("3107T00054", "Allot Rate");
            operationTime.Header = CurrentApp.GetLanguageInfo("3107T00055", "Operation Cycle Time");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3107T00018", "OK");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3107T00019", "Close");

            lbTaskName.Content = CurrentApp.GetLanguageInfo("3107T00023", "Task Name");
            lbTaskType.Content = CurrentApp.GetLanguageInfo("3107T00024", "Task Type");
            cbTaskType0.Content = CurrentApp.GetLanguageInfo("3107T00044", "Check Task");
            //cbTaskType1.Content = CurrentApp.GetLanguageInfo("3107T00045", "Recheck Task");
            //cbTaskType2.Content = CurrentApp.GetLanguageInfo("3107T00046", "Smart Task");
            lbTaskRemark.Content = CurrentApp.GetLanguageInfo("3107T00071", "Task Remarks");
            lbTIsUse.Content = CurrentApp.GetLanguageInfo("3107T00010","Is Use");
            yesUse.Content = CurrentApp.GetLanguageInfo("3107T00011", "Yes");
            noUse.Content = CurrentApp.GetLanguageInfo("3107T00012", "No");
            //lbIsShare.Content = CurrentApp.GetLanguageInfo("3107T00047","Is Share");
            //lbIsAverage.Content = CurrentApp.GetLanguageInfo("3107T00048", "Is Average");
            //yesAverage.Content = yesShare .Content= CurrentApp.GetLanguageInfo("3107T00011", "Yes");
            //noAverage.Content = noShare.Content = CurrentApp.GetLanguageInfo("3107T00012", "No");
            //lbRecordDispose.Content = CurrentApp.GetLanguageInfo("3107T00049","Is Dispose");
            //yesDispose.Content = CurrentApp.GetLanguageInfo("3107T00050", "Yes");
            //noDispose.Content = CurrentApp.GetLanguageInfo("3107T00051", "No");
            lbTaskCondition.Content = CurrentApp.GetLanguageInfo("3107T00009", "Task Query Condition");
            lbTaskDeadline.Content = CurrentApp.GetLanguageInfo("3107T00025", "Task Deadline");
            lbDeadline.Content = CurrentApp.GetLanguageInfo("3107T00038", "Days");
            epSA.Header = CurrentApp.GetLanguageInfo("3107T00052","Select QA");

            ckDataGrid.Content = CurrentApp.GetLanguageInfo("3107T00056", "Set Duration Rate");
            dataGrid.Columns[0].Header = CurrentApp.GetLanguageInfo("3107T00057", "DurationMin");
            dataGrid.Columns[1].Header = CurrentApp.GetLanguageInfo("3107T00058", "DurationMax");
            dataGrid.Columns[2].Header = CurrentApp.GetLanguageInfo("3107T00059", "Rate"); 

            lbRunFreq.Content = CurrentApp.GetLanguageInfo("3107T00026","Task Run Time");
            cbRunFreq0.Content = CurrentApp.GetLanguageInfo("3107T00072", "Every Day");
            cbRunFreq1.Content= lbFreqRunWeekTime.Content = CurrentApp.GetLanguageInfo("3107T00060", "Every Week");
            cbRunFreq2.Content = lbFreqRunMonthTime.Content = CurrentApp.GetLanguageInfo("3107T00061", "Every Month");
            cbRunFreq3.Content = lbFirstRunSeasonTime .Content= CurrentApp.GetLanguageInfo("3107T00062", "Every Season");
            cbRunFreq4.Content = lbFreqRunYearTime.Content = CurrentApp.GetLanguageInfo("3107T00063", "Every Year");
            lbRunTime.Content = CurrentApp.GetLanguageInfo("3107T00027","Run Tme");
            cbFreqWeekTime0.Content = CurrentApp.GetLanguageInfo("3107T00064", "Monday");
            cbFreqWeekTime1.Content = CurrentApp.GetLanguageInfo("3107T00065", "Tuesday");
            cbFreqWeekTime2.Content = CurrentApp.GetLanguageInfo("3107T00066", "Wednesday");
            cbFreqWeekTime3.Content = CurrentApp.GetLanguageInfo("3107T00067", "Thursday");
            cbFreqWeekTime4.Content = CurrentApp.GetLanguageInfo("3107T00068", "Friday");
            cbFreqWeekTime5.Content = CurrentApp.GetLanguageInfo("3107T00069", "Saturday");
            cbFreqWeekTime6.Content = CurrentApp.GetLanguageInfo("3107T00070", "Sunday");
        }

    }
}
