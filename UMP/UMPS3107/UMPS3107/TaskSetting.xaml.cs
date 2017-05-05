using Common3107;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region 关键词
        public ObservableCollection<CheckComboboxItems> mListCheckComItems;

        #endregion

        public TaskSetting()
        {
            InitializeComponent();
            this.Loaded += TaskSetting_Loaded;
            QAItems = new List<string>();
            mListCheckComItems = new ObservableCollection<CheckComboboxItems>();
            cbFreqWeekTime.SelectionChanged += cbFreqWeekTime_SelectionChanged;
            cbFreqMonthTime.SelectionChanged += cbFreqMonthTime_SelectionChanged;
            TvTaskObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
            cbTaskCondition.SelectionChanged += cbTaskCondition_SelectionChanged;
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
                cbKeyWord.ItemsSource = mListCheckComItems;
                GetTemplate();
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
                        OperationInfo opt = new OperationInfo();
                        comItem = new ComboBoxItem();
                        opt.ID = queryItemList[i].QuerySettingID;
                        comItem.Content = queryItemList[i].QuerySettingName;
                        comItem.DataContext = opt;
                        cbTaskCondition.Items.Add(comItem);
                    }
                    //关键词
                    lbKeyWord.Visibility = Visibility.Collapsed;
                    cbKeyWord.Visibility = Visibility.Collapsed;
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
                        OperationInfo opt = new OperationInfo();
                        comItem = new ComboBoxItem();
                        opt.ID = queryItemList[i].QuerySettingID;
                        comItem.DataContext = opt;
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

                    //关键词
                    if (string.IsNullOrWhiteSpace(TaskItems.LQKeyWordItemsOne))
                    {
                        lbKeyWord.Visibility = Visibility.Collapsed;
                        cbKeyWord.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        lbKeyWord.Visibility = Visibility.Visible;
                        cbKeyWord.Visibility = Visibility.Visible;
                    }

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

        #region 关键词

        void cbTaskCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OperationInfo opt;
            string keywordID = string.Empty;
            string keywordContent = string.Empty;
            var temp = cbTaskCondition.SelectedItem as ComboBoxItem;
            try
            {                
                if (temp != null && queryItemList!=null)
                {
                    mListCheckComItems.Clear();
                    opt = temp.DataContext as OperationInfo;
                    if (!string.IsNullOrWhiteSpace(queryItemList.Where(p => p.QuerySettingID == opt.ID).FirstOrDefault().LQKeyWordItemsOne))
                    {
                        QuerySettingItems tempq=queryItemList.Where(p => p.QuerySettingID == opt.ID).FirstOrDefault();
                        string keyIDStr=tempq.LQKeyWordItemsOne+tempq.LQKeyWordItemsTwo+tempq.LQKeyWordItemsThree;
                        var items = S3107App.ListKeyWordItems.GroupBy(k => k.KeyWordID);
                        lbKeyWord.Visibility = Visibility.Visible;
                        cbKeyWord.Visibility = Visibility.Visible;
                        cbKeyWord.Text = string.Empty;
                        CheckComboboxItems chkCombItem;
                        if (TaskItems != null && !string.IsNullOrWhiteSpace(TaskItems.LQKeyWordItemsOne))
                        {
                            keywordID = TaskItems.LQKeyWordItemsOne + TaskItems.LQKeyWordItemsTwo + TaskItems.LQKeyWordItemsThree;
                        }
                        foreach (var item in items)
                        {
                            long id = 0;
                            string strName = string.Empty;
                            string strDesc = string.Empty;
                            foreach (var content in item)
                            {
                                id = content.KeyWordID;
                                strName = content.KeyWordSrt;
                                strDesc += string.Format("{0};", content.KWContent);
                            }
                            chkCombItem = new CheckComboboxItems();
                            chkCombItem.KeyWordID = id.ToString();
                            chkCombItem.Description = string.Format("{0}({1})", strName, strDesc);
                            chkCombItem.KeyWordContent = strDesc;
                            if (keywordID.Contains(id.ToString()))
                            {
                                chkCombItem.IsSelected = true;
                                keywordContent += strName + ",";
                            }
                            chkCombItem.KeyWordName = strName;
                            if (keyIDStr.Contains(id.ToString())) 
                            {
                                mListCheckComItems.Add(chkCombItem);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(keywordContent))
                        {
                            if (keywordContent.Length > 9)
                            {
                                keywordContent = keywordContent.Substring(0, 9).TrimEnd(',') + "...";
                            }
                            cbKeyWord.Text = keywordContent.TrimEnd(',');
                        }
                    }
                    else
                    {
                        lbKeyWord.Visibility = Visibility.Collapsed;
                        cbKeyWord.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        void ckKWItem_Checked(object sender, RoutedEventArgs e)
        {
            cbKeyWord.Text = string.Empty;
            string keywordName = string.Empty;
            if (mListCheckComItems.Count() <= 0) { return; }
            foreach (CheckComboboxItems item in mListCheckComItems)
            {
                if (keywordName.Length > 25) { break; }
                if (item.IsSelected.Equals(true))
                {
                    if (keywordName.Length > 10)
                    {
                        keywordName = keywordName.TrimEnd(',') + "...";
                    }
                    else
                    {
                        keywordName += string.Format("{0},", item.KeyWordName);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(keywordName) && keywordName.Length > 9)
            {
                keywordName = keywordName.Substring(0, 9).TrimEnd(',') + "...";
            }
            cbKeyWord.Text = keywordName.TrimEnd(',');
        }


        #endregion

        #region BookMark


        #endregion

        #region 评分表
        /// <summary>
        /// 座席过多的情况下，匹配所选座席的交集效率太慢。所以，目前查出所有可用评分表，自动任务在任务中特殊处理
        /// </summary>
        private void GetTemplate()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3107Codes.GetAgentsTemplate;
                webRequest.Session = CurrentApp.Session;
                //webRequest.ListData.Add(agentIDStr);//这句是找到每条录音记录的坐席
                int aaa = webRequest.ListData.Count;
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                //string strtemp = webReturn.ListData[0];
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                ComboBoxItem item;
                for (int i = 0; i < webReturn.ListData.Count;i++ ) 
                {
                    if (!string.IsNullOrWhiteSpace(webReturn.ListData[i]))
                    {
                        string[] array = webReturn.ListData[i].Split(';');
                        OperationInfo info = new OperationInfo();
                        info.ID = Convert.ToInt64(array[0]);
                        info.Description = array[1];
                        item = new ComboBoxItem();
                        item.DataContext = info;
                        item.Content = array[1];
                        cbTemplate.Items.Add(item);
                        if (TaskItems!=null && TaskItems.TemplateID > 0 && TaskItems.TemplateID == info.ID)
                        {
                            cbTemplate.SelectedIndex = i;
                            ckTemplate.IsChecked = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                //评分表绑定
                if (ckTemplate.IsChecked.Equals(true))
                {
                    var item = cbTemplate.SelectedItem as ComboBoxItem;
                    if (item == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3107T00109", "Please Select a Template"));
                        return false;
                    }
                    OperationInfo opt = item.DataContext as OperationInfo;
                    TaskItems.TemplateID = opt.ID;
                }
                else
                {
                    TaskItems.TemplateID = 0;
                }

                #endregion

                #region 关键词
                string keywordContent = string.Empty;
                string keywordID = string.Empty;
                foreach (CheckComboboxItems item in mListCheckComItems)
                {
                    keywordContent += item.KeyWordContent;
                    keywordID += item.KeyWordID + ",";
                }
                //内容
                if (keywordContent.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00107", "KeyWord Is Too larger"));
                    return false;
                }
                if (keywordContent.Length > 4000 && keywordContent.Length < 6000)
                {
                    TaskItems.KeyContentOne = keywordContent.Substring(0, 2000);
                    TaskItems.KeyContentTwo = keywordContent.Substring(2000, 2000);
                    TaskItems.KeyContentThree = keywordContent.Substring(4000, keywordContent.Length - 4);
                }
                if (keywordContent.Length > 2000 && keywordContent.Length <= 4000)
                {
                    TaskItems.KeyContentOne = keywordContent.Substring(0, 2000);
                    TaskItems.KeyContentTwo = keywordContent.Substring(2000, keywordContent.Length - 2000);
                    TaskItems.KeyContentThree = string.Empty;
                }
                if (keywordContent.Length <= 2000)
                {
                    TaskItems.KeyContentOne = keywordContent;
                    TaskItems.KeyContentTwo = string.Empty;
                    TaskItems.KeyContentThree = string.Empty;
                }

                //ID
                if (keywordID.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00107", "KeyWord Is Too larger"));
                    return false;
                }
                if (keywordID.Length > 4000 && keywordID.Length < 6000)
                {
                    TaskItems.LQKeyWordItemsOne = keywordID.Substring(0, 2000);
                    TaskItems.LQKeyWordItemsTwo = keywordID.Substring(2000, 2000);
                    TaskItems.LQKeyWordItemsThree = keywordID.Substring(4000, keywordID.Length - 4);
                }
                if (keywordID.Length > 2000 && keywordID.Length <= 4000)
                {
                    TaskItems.LQKeyWordItemsOne = keywordID.Substring(0, 2000);
                    TaskItems.LQKeyWordItemsTwo = keywordID.Substring(2000, keywordID.Length - 2000);
                    TaskItems.LQKeyWordItemsThree = string.Empty;
                }
                if (keywordID.Length <= 2000)
                {
                    TaskItems.LQKeyWordItemsOne = keywordID;
                    TaskItems.LQKeyWordItemsTwo = string.Empty;
                    TaskItems.LQKeyWordItemsThree = string.Empty;
                }

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
            lbKeyWord.Content = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
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
            ckTemplate.Content = CurrentApp.GetLanguageInfo("3107T00108", "Select A Template");

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
