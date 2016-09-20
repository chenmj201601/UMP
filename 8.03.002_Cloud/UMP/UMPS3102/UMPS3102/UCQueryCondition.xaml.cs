//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be6e0872-5a06-4e1f-a5a8-5a730a92e0ca
//        CLR Version:              4.0.30319.18444
//        Name:                     UCQueryCondition
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCQueryCondition
//
//        created by Charley at 2014/11/7 14:53:48
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// UCQueryCondition.xaml 的交互逻辑
    /// </summary>
    public partial class UCQueryCondition
    {
        #region Memebers

        public QMMainView PageParent;
        public QueryCondition QueryCondition;
        public string ConditionString;
        public string ConditionLog;
        public bool IsAutoQuery;//这个是   是否自动查询的判断标识   当用这个的时候会跳过自定义查询
        public bool IsSkip;
        public List<ObjectItem> ListSelectedObjects;
        public List<ObjectItem> ListAllObjects;

        private List<CustomConditionItem> mListCustomConditionItems;
        private List<UCConditionItem> mListUCConditionItems;//tab下的条件项集合，就比如默认情况下在第一个tab下面有10个查询条件项 
        private List<ConditionTabItem> mListConditionTabItems;//条件tab集合
        private List<TabItem> mListTabItems;
        private List<QueryConditionDetail> mListQueryConditionDetails;
        private List<QueryConditionSubItem> mListSubItems;
        private ObservableCollection<QueryCondition> mListQueryConditions;
        private RememberConditionInfo mRememberInfo;
        //private string mManageObjectQueryID;

        public InspectorConditionTree InspectorConditionTree1;
        public SkillGroupTree SkillGroupTree1;
        public KeyWordList KeyWordList1;

        //用户的管理对象插入到临时表生成的临时ID
        public string ManageObjectQueryID;
        //用户的管理对象是否存入临时表
        public bool IsSaveTempTable;

        private string mTempTableContent_Agent;
        private string mTempTableContent_Extension;
        private string mTempTableContent_RealExtension;
        #endregion


        public UCQueryCondition()
        {
            InitializeComponent();

            mListCustomConditionItems = new List<CustomConditionItem>();
            mListConditionTabItems = new List<ConditionTabItem>();
            mListTabItems = new List<TabItem>();
            mListUCConditionItems = new List<UCConditionItem>();
            mListQueryConditionDetails = new List<QueryConditionDetail>();
            mListSubItems = new List<QueryConditionSubItem>();
            mListQueryConditions = new ObservableCollection<QueryCondition>();

            Loaded += UCQueryCondition_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            ComboQueryConditions.ItemsSource = mListQueryConditions;

            CommandBindings.Add(new CommandBinding(UCConditionItem.AddValuesCommand, AddValuesCommand_Executed, (s, ee) => ee.CanExecute = true));

            InspectorConditionTree1 = new InspectorConditionTree();
            InspectorConditionTree1.CurrentApp = CurrentApp;
            SkillGroupTree1 = new SkillGroupTree();
            SkillGroupTree1.CurrentApp = CurrentApp;
            KeyWordList1 = new KeyWordList();
            KeyWordList1.CurrentApp = CurrentApp;
        }

        void UCQueryCondition_Loaded(object sender, RoutedEventArgs e)
        {
            InitQueryConditions();
            InitConditionItems();
            //指定查询条件ID直接查询，跳过显示查询条件面板
            if (IsAutoQuery) //这个为true时,那么这个就是自动查询了,跳过查询面板，这个变量在主界面会来赋值
            {
                InitQueryConditionDetails();
                InitConditionSubItems();
                AutoQuery();
            }
            else
            {
                InitRememberConditions();
                InitQueryConditionDetails();
            }

            CreateConditionItems();
            SetCurrentQueryCondition();

            ChangeLanguage();
        }

        #region Init and Load

        private void InitQueryConditions()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserQueryCondition;//获取用户已保存的查询条件  这个就是  查询模块的那个上面的那个小窗口保存的 也就是快速查询那里保存的一些条件
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mListQueryConditions.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<QueryCondition>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    QueryCondition queryCondition = optReturn.Data as QueryCondition;
                    if (queryCondition != null)
                    {
                        mListQueryConditions.Add(queryCondition);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitConditionItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserCustomConditionItem;//获取用户的自定义查询条件项 这个是从数据库中读取的查询条件
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. WebReturn ListData is null"));
                    return;
                }
                mListCustomConditionItems.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CustomConditionItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CustomConditionItem item = optReturn.Data as CustomConditionItem;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                    mListCustomConditionItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRememberConditions() //这个就是导入记住的查询的条件  这个记住的查询条件是在本地的一个xml里面的
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}/condition.xml", CurrentApp.AppName));
                if (!File.Exists(path)) { return; }
                OperationReturn optReturn = XMLHelper.DeserializeFile<RememberConditionInfo>(path);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                RememberConditionInfo conditionInfo = optReturn.Data as RememberConditionInfo;
                if (conditionInfo == null) { return; }
                mRememberInfo = conditionInfo;
                CbRememberConditions.IsChecked = mRememberInfo.IsRemember;
                if (mRememberInfo.IsRemember)
                {
                    mListQueryConditionDetails = mRememberInfo.ListConditionDetail;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitQueryConditionDetails()
        {
            try
            {
                if (QueryCondition == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetQueryConditionDetail;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(QueryCondition.ID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. WebReturn ListData is null"));
                    return;
                }
                mListQueryConditionDetails.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    QueryConditionDetail item = optReturn.Data as QueryConditionDetail;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. QueryConditionDetail is null"));
                        return;
                    }
                    mListQueryConditionDetails.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitConditionSubItems()
        {
            try
            {
                mListSubItems.Clear();
                for (int i = 0; i < mListQueryConditionDetails.Count; i++)
                {
                    QueryConditionDetail detail = mListQueryConditionDetails[i];
                    if (!detail.IsEnable) { continue; }
                    CustomConditionItem conditionItem =
                        mListCustomConditionItems.FirstOrDefault(c => c.ID == detail.ConditionItemID);
                    if (conditionItem != null)
                    {
                        if (conditionItem.Type == CustomConditionItemType.MultiText)
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Code = (int)S3102Codes.GetConditionSubItem;
                            webRequest.Session = CurrentApp.Session;
                            webRequest.ListData.Add(detail.Value01);
                            webRequest.ListData.Add(detail.QueryID.ToString());
                            webRequest.ListData.Add(detail.ConditionItemID.ToString());
                            Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                               WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                            WebReturn webReturn = client.DoOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                            if (webReturn.ListData == null)
                            {
                                ShowException(string.Format("Fail. WebReturn ListData is null"));
                                return;
                            }
                            for (int j = 0; j < webReturn.ListData.Count; j++)
                            {
                                OperationReturn optReturn = XMLHelper.DeserializeObject<QueryConditionSubItem>(webReturn.ListData[j]);
                                if (!optReturn.Result)
                                {
                                    ShowException(string.Format("Error.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    return;
                                }
                                QueryConditionSubItem item = optReturn.Data as QueryConditionSubItem;
                                if (item == null)
                                {
                                    ShowException(string.Format("Error. SubItem is null"));
                                    return;
                                }
                                mListSubItems.Add(item);
                            }
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


        #region Others

        private void CreateConditionItems()
        {
            try
            {
                mListTabItems.Clear();
                mListUCConditionItems.Clear();
                mListConditionTabItems.Clear();
                //创建Tab页
                for (int i = 0; i < mListCustomConditionItems.Count; i++)
                {
                    CustomConditionItem item = mListCustomConditionItems[i];
                    ConditionTabItem tabItem = mListConditionTabItems.FirstOrDefault(t => t.TabIndex == item.TabIndex);
                    if (tabItem == null)
                    {
                        tabItem = new ConditionTabItem();
                        tabItem.TabIndex = item.TabIndex;
                        tabItem.TabName = item.TabName;
                        if (string.IsNullOrEmpty(tabItem.TabName))
                        {
                            tabItem.TabName = string.Format("Condition {0}", tabItem.TabIndex);
                        }
                        mListConditionTabItems.Add(tabItem);
                    }
                }
                mListConditionTabItems = mListConditionTabItems.OrderBy(t => t.TabIndex).ToList();
                TabControlCondition.Items.Clear();
                for (int i = 0; i < mListConditionTabItems.Count; i++)
                {
                    ConditionTabItem item = mListConditionTabItems[i];
                    TabItem tabItem = new TabItem();
                    tabItem.Tag = item;
                    tabItem.DataContext = item;
                    tabItem.SetResourceReference(StyleProperty, "ConditionTabItem");
                    TabControlCondition.Items.Add(tabItem);
                    mListTabItems.Add(tabItem);
                }
                //创建条件项
                for (int i = 0; i < mListConditionTabItems.Count; i++)
                {
                    var tabItem = TabControlCondition.Items[i] as TabItem;
                    if (tabItem == null)
                    {
                        continue;
                    }
                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    tabItem.Content = scrollViewer;
                    WrapPanel conditionItemPanel = new WrapPanel();
                    conditionItemPanel.Margin = new Thickness(10, 5, 10, 2);
                    conditionItemPanel.Orientation = Orientation.Horizontal;
                    scrollViewer.Content = conditionItemPanel;
                    List<CustomConditionItem> itemsInTab =
                        mListCustomConditionItems.Where(c => c.TabIndex == mListConditionTabItems[i].TabIndex).OrderBy(c => c.SortID).ToList();
                    for (int j = 0; j < itemsInTab.Count; j++)
                    {
                        CustomConditionItem itemInTab = itemsInTab[j];
                        UCConditionItem ucConditionItem = new UCConditionItem();
                        ucConditionItem.PageParent = this;
                        ucConditionItem.ConditionItem = itemInTab;
                        ucConditionItem.ListSelectedObjects = ListSelectedObjects;
                        ucConditionItem.ListAllObjects = ListAllObjects;
                        ucConditionItem.Tag = itemInTab;
                        ucConditionItem.CurrentApp = CurrentApp;
                        QueryConditionDetail detail =
                            mListQueryConditionDetails.FirstOrDefault(d => d.ConditionItemID == itemInTab.ID);
                        if (detail != null)
                        {
                            ucConditionItem.ConditionResult = detail;
                        }
                        conditionItemPanel.Children.Add(ucConditionItem);
                        mListUCConditionItems.Add(ucConditionItem);
                    }
                }
                TabControlCondition.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetCurrentQueryCondition()
        {
            try
            {
                if (QueryCondition == null) { return; }
                QueryCondition temp = mListQueryConditions.FirstOrDefault(q => q.ID == QueryCondition.ID);
                if (temp != null)
                {
                    CbSaveConditions.IsChecked = true;
                    ComboQueryConditions.SelectedItem = temp;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Operations

        private void AutoQuery()
        {
            try
            {
                if (IsSkip)//是否跳过
                {
                    PrepareQueryConditions();
                    //将多值序列写入临时表
                    SaveMultiValues();
                    //创建条件Sql语句
                    CreateConditionString();
                    if (PageParent != null)
                    {
                        PageParent.QueryRecord(ConditionString, ConditionLog, mListQueryConditionDetails, QueryCondition);

                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PrepareQueryConditions()
        {
            try
            {
                for (int i = 0; i < mListQueryConditionDetails.Count; i++)
                {
                    var item = mListQueryConditionDetails[i];

                    //DateTypeFromTo 特殊处理，根据类型计算出正确的时间（Value01，Value02）
                    if (item.ConditionItemID == S3102Consts.CON_TIMETYPEFROMTO)
                    {
                        string strValue03;
                        string strValue04;
                        string strValue05;

                        strValue03 = item.Value03;
                        if (strValue03 == "1")
                        {
                            strValue04 = item.Value04;
                            DateTime now = DateTime.Now;
                            DateTime nextMonth = now.AddMonths(1);
                            int week = Convert.ToInt32(now.DayOfWeek);
                            int daydiff = (-1) * week + 1;
                            int dayadd = 7 - week;
                            switch (strValue04)
                            {
                                case "0":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59",
                                        now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                        DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "1":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddDays(-1).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59",
                                        now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "2":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddDays(daydiff).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59",
                                        now.AddDays(dayadd).ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "3":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                       now.AddDays(-(now.Day) + 1).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59",
                                        nextMonth.AddDays(-(now.Day)).ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                            }
                        }
                        if (strValue03 == "2")
                        {
                            strValue04 = item.Value04;
                            strValue05 = item.Value05;
                            int intCount = Convert.ToInt32(strValue04);
                            DateTime now = DateTime.Now;
                            switch (strValue05)
                            {
                                case "0":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddDays(-intCount).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "1":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddDays(-intCount * 7).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "2":

                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddMonths(-intCount).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "3":
                                    item.Value01 = string.Format("{0} 00:00:00",
                                        now.AddYears(-intCount).ToString("yyyy-MM-dd"));
                                    item.Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                    item.Value01 =
                                      DateTime.Parse(item.Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    item.Value02 =
                                        DateTime.Parse(item.Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateConditionResultList()//这个就是把界面上的查询条件（后来存入了 mListUCConditionItems 里面）,放到mListQueryConditionDetails里面
        {
            try
            {
                mListSubItems.Clear();
                mListQueryConditionDetails.Clear();
                for (int i = 0; i < mListUCConditionItems.Count; i++)
                {
                    UCConditionItem ucConditionItem = mListUCConditionItems[i];
                    ucConditionItem.GetValue();//输入内容判断改为bool返回类型（可否？）
                    List<QueryConditionSubItem> listSubItems = ucConditionItem.GetSubItems();
                    for (int j = 0; j < listSubItems.Count; j++)
                    {
                        mListSubItems.Add(listSubItems[j]);
                    }
                    mListQueryConditionDetails.Add(ucConditionItem.ConditionResult);//mListQueryConditionDetails是把所有输入的查询条件的集合
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateConditionString()
        {
            try
            {
                QueryStateInfo queryStateInfo = new QueryStateInfo();
                var tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo == null)
                {
                    tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.VoiceID);
                    if (tableInfo == null)
                    {
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                            CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        //按录音服务器查询,没有实现，暂时还是按普通方式来
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                            CurrentApp.Session.RentInfo.Token);
                    }
                }
                else
                {
                    //按月查询
                    var timeDetail =
                        mListQueryConditionDetails.FirstOrDefault(d => d.ConditionItemID == S3102Consts.CON_TIMETYPEFROMTO);
                    if (timeDetail == null)
                    {
                        ShowException(string.Format("TimeTypeFromToDetail is null"));
                        return;
                    }
                    DateTime beginTime = Convert.ToDateTime(timeDetail.Value01);
                    DateTime endTime = Convert.ToDateTime(timeDetail.Value02);
                    DateTime baseTime = beginTime;
                    string partTable;
                    int monthCount = Utils.GetTimeMonthCount(beginTime, endTime);
                    for (int i = 0; i <= monthCount; i++)
                    {
                        partTable = baseTime.AddMonths(i).ToString("yyMM");
                        queryStateInfo.TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_STATISTICS,
                            CurrentApp.Session.RentInfo.Token, partTable);
                        //在没查询一个分表之后将其行号  也就是T_21_001的C001字段比较的值置为0 这样才能正常比较
                    }
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.CreateQueryConditionString;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(queryStateInfo.TableName);
                webRequest.ListData.Add(mListQueryConditionDetails.Count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < mListQueryConditionDetails.Count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(mListQueryConditionDetails[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null || webReturn.ListData.Count < 2)
                {
                    ShowException(string.Format("Fail.\tListData is null or count invalid"));
                    return;
                }
                ConditionString = webReturn.ListData[0];
                ConditionLog = webReturn.ListData[1];
                if (CurrentApp.Session.RoleID == ConstValue.ROLE_SYSTEMADMIN)
                {
                    return;
                }

                if (string.IsNullOrEmpty(ManageObjectQueryID) && IsSaveTempTable == false)
                {
                    return;
                }
                //?问下查理这里是什么意思   分机这   分组方式(全局参数来判断的~  要改)
                if (string.IsNullOrEmpty(ManageObjectQueryID))
                {
                    //如果ManageObjectQueryID为空的话,那么带入到查询语句里面的ManageObjectQueryID必须有值,为了避免重复,我这里就让其变成当前时间到公元元年的时间的毫秒数的值
                    ManageObjectQueryID = DateTime.Now.Ticks.ToString();
                }
                string strCondition = string.Empty;
                if (CurrentApp.Session.DBType == 2)
                {
                    GetTempAgent();
                    GetTempExtension();
                    GetTempRealExtension();
                    if (string.IsNullOrWhiteSpace(mTempTableContent_Agent) || string.IsNullOrEmpty(mTempTableContent_Agent))
                    {
                        mTempTableContent_Agent = "''";
                    }
                    if (string.IsNullOrWhiteSpace(mTempTableContent_Extension) || string.IsNullOrEmpty(mTempTableContent_Extension))
                    {
                        mTempTableContent_Extension = "''";
                    }
                    if (string.IsNullOrWhiteSpace(mTempTableContent_RealExtension) || string.IsNullOrEmpty(mTempTableContent_RealExtension))
                    {
                        mTempTableContent_RealExtension = "''";
                    }

                    if (PageParent.GroupingWay.IndexOf("E") >= 0)
                    {
                        strCondition += string.Format("X.C042 IN ({0}) AND ", mTempTableContent_Extension);
                    }
                    if (PageParent.GroupingWay.IndexOf("A") >= 0)
                    {
                        strCondition += string.Format("X.C039 IN ({0}) AND ", mTempTableContent_Agent);
                    }
                    if (PageParent.GroupingWay.IndexOf("R") >= 0)
                    {
                        strCondition += string.Format("X.C058 IN ({0}) AND ", mTempTableContent_RealExtension);
                    }

                    //if (PageParent.GroupingWay == "EA")
                    //{
                    //    strCondition = string.Format("X.C039 IN ({0}) AND X.C042 IN ({1}) AND "
                    //       , mTempTableContent_Agent, mTempTableContent_Extension);
                    //}
                    //if (PageParent.GroupingWay == "E")
                    //{
                    //    strCondition = string.Format(" X.C042 IN ({0}) AND "
                    //        , mTempTableContent_Extension);
                    //}
                    //if (PageParent.GroupingWay == "A")
                    //{
                    //    strCondition = string.Format(" X.C039 IN ({0}) AND "
                    //       , mTempTableContent_Agent);
                    //}
                }
                else
                {
                    if (PageParent.GroupingWay.IndexOf("E") >= 0)
                    {
                        strCondition += string.Format(" X.C042 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={1}) AND "
                           , ManageObjectQueryID, ConstValue.RESOURCE_EXTENSION);
                    }
                    if (PageParent.GroupingWay.IndexOf("A") >= 0)
                    {
                        strCondition += string.Format("X.C039 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 = {1}) AND "
                            , ManageObjectQueryID, ConstValue.RESOURCE_AGENT);
                    }
                    if (PageParent.GroupingWay.IndexOf("R") >= 0)
                    {
                        strCondition += string.Format("X.C058 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 = {1}) AND "
                            , ManageObjectQueryID, ConstValue.RESOURCE_REALEXT);
                    }

                    //if (PageParent.GroupingWay == "EA")
                    //{
                    //    strCondition = string.Format("X.C039 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 = {1}) AND X.C042 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={2}) AND "
                    //        , ManageObjectQueryID, ConstValue.RESOURCE_AGENT, ConstValue.RESOURCE_EXTENSION);
                    //}
                    //if (PageParent.GroupingWay == "E")
                    //{
                    //    strCondition = string.Format(" X.C042 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={1}) AND "
                    //        , ManageObjectQueryID,  ConstValue.RESOURCE_EXTENSION);
                    //}
                    //if (PageParent.GroupingWay == "A")
                    //{
                    //    strCondition = string.Format("X.C039 IN (SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 = {1}) AND "
                    //        , ManageObjectQueryID, ConstValue.RESOURCE_AGENT);
                    //}

                }

                if (strCondition.Length > 0)
                {
                    strCondition = strCondition.Substring(0, strCondition.Length - 4);
                    ConditionString = string.Format("{0} AND {1}", ConditionString, strCondition);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //在mssql的环境中先从临时表里面获得管理的坐席和分机
        private void GetTempAgent()
        {
            try
            {
                string strsql_Agent = string.Format("SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 = {1}",
                    ManageObjectQueryID, ConstValue.RESOURCE_AGENT);
                //string strsql_Extension = string.Format("SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={1}", 
                //    ManageObjectQueryID, ConstValue.RESOURCE_EXTENSION);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.ExecuteStrSql;
                webRequest.ListData.Add(strsql_Agent);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strtemp = string.Empty;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    if (i != webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'" + ",";
                    }
                    if (i == webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'";
                    }
                }
                mTempTableContent_Agent = strtemp;
            }
            catch (Exception ex)
            {

            }
        }

        private void GetTempExtension()
        {
            try
            {
                string strsql_Extension = string.Format("SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={1}",
                    ManageObjectQueryID, ConstValue.RESOURCE_EXTENSION);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.ExecuteStrSql;
                webRequest.ListData.Add(strsql_Extension);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strtemp = string.Empty;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    if (i != webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'" + ",";
                    }
                    if (i == webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'";
                    }
                }
                mTempTableContent_Extension = strtemp;
            }
            catch (Exception ex)
            {

            }
        }

        private void GetTempRealExtension()
        {
            try
            {
                string strsql_Extension = string.Format("SELECT C013 FROM T_00_901 WHERE C001 = {0} AND C011 ={1}",
                    ManageObjectQueryID, ConstValue.RESOURCE_REALEXT);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.ExecuteStrSql;
                webRequest.ListData.Add(strsql_Extension);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strtemp = string.Empty;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    if (i != webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'" + ",";
                    }
                    if (i == webReturn.ListData.Count - 1)
                    {
                        strtemp += "'" + webReturn.ListData[i] + "'";
                    }
                }
                mTempTableContent_RealExtension = strtemp;
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveMultiValues()
        {
            //对于多值型，先将多值保存到临时表中
            try
            {
                for (int i = 0; i < mListQueryConditionDetails.Count; i++)
                {
                    QueryConditionDetail detail = mListQueryConditionDetails[i];
                    if (!detail.IsEnable)
                    {
                        continue;
                    }
                    CustomConditionItem item =
                        mListCustomConditionItems.FirstOrDefault(c => c.ID == detail.ConditionItemID);
                    if (item == null) { continue; }
                    if (item.Type == CustomConditionItemType.MultiText)
                    {
                        List<QueryConditionSubItem> listSubItems =
                            mListSubItems.Where(s => s.ConditionItemID == item.ID).ToList();
                        SaveMultiValues(detail, listSubItems);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveMultiValues(QueryConditionDetail detail, List<QueryConditionSubItem> listSubItems)
        {
            //对于多值型，先将多值保存到临时表中 （坐席和分机这两个来进行查询,这两个可以是多值的）
            try
            {
                if (detail == null || listSubItems.Count <= 0) { return; }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(detail);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.InsertConditionSubItems;
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(listSubItems.Count.ToString());
                for (int i = 0; i < listSubItems.Count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listSubItems[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.Data == null)
                {
                    ShowException(string.Format("Fail.\tWebReturn Data is null"));
                    return;
                }
                optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                QueryConditionDetail temp = optReturn.Data as QueryConditionDetail;
                if (temp == null)
                {
                    ShowException(string.Format("Fail.\tQueryConditionDetail is null"));
                    return;
                }
                //返回临时表的Id和日志信息
                detail.Value01 = temp.Value01;
                detail.Value02 = temp.Value02;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //private bool SaveManageObjectQueryInfos()
        //{
        //    //将当前用户管理的对象保存到临时表中
        //    try
        //    {
        //        mManageObjectQueryID = string.Empty;

        //        //系统管理员身份的用户无需考虑管理权限，他能管理所有分机、坐席
        //        if (CurrentApp.Session.RoleID == ConstValue.ROLE_SYSTEMADMIN) { return false; }

        //        if (ListAllObjects == null) { return true; }

        //        string strType, strID, strName, strInfo;
        //        List<string> listObjectInfos = new List<string>();
        //        for (int i = 0; i < ListAllObjects.Count; i++)
        //        {
        //            ObjectItem item = ListAllObjects[i];
        //            //分机和坐席都控制
        //            if (item.ObjType == ConstValue.RESOURCE_AGENT
        //                || item.ObjType == ConstValue.RESOURCE_EXTENSION)
        //            {
        //                strType = item.ObjType.ToString();
        //                strID = item.ObjID.ToString();
        //                strName = item.Name;
        //                strInfo = string.Format("{0}{1}{2}{1}{3}",
        //                    strType,
        //                    ConstValue.SPLITER_CHAR,
        //                    strID,
        //                    strName);
        //                listObjectInfos.Add(strInfo);
        //            }
        //        }
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = CurrentApp.Session;
        //        webRequest.Code = (int)S3102Codes.InsertManageObjectQueryInfos;
        //        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
        //        webRequest.ListData.Add(listObjectInfos.Count.ToString());
        //        for (int i = 0; i < listObjectInfos.Count; i++)
        //        {
        //            webRequest.ListData.Add(listObjectInfos[i]);
        //        }
        //        Service31021Client client = new Service31021Client(
        //            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
        //            WebHelper.CreateEndpointAddress(
        //                CurrentApp.Session.AppServerInfo,
        //                "Service31021"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return false;
        //        }
        //        mManageObjectQueryID = webReturn.Data;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowException(ex.Message);
        //        return false;
        //    }
        //}

        private void RememberConditions()
        {
            try
            {
                if (mRememberInfo == null)
                {
                    mRememberInfo = new RememberConditionInfo();
                }
                mRememberInfo.IsRemember = false;
                if (CbRememberConditions.IsChecked == true)
                {
                    mRememberInfo.IsRemember = true;
                    mRememberInfo.ListConditionDetail = mListQueryConditionDetails;
                }
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}", CurrentApp.AppName));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path = Path.Combine(dir, "condition.xml");
                OperationReturn optReturn = XMLHelper.SerializeFile(mRememberInfo, path);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\r{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool SaveConditions()
        {
            try
            {
                //保存查询条件~勾选了保存按钮
                if (CbSaveConditions.IsChecked == true)
                {
                    var queryCondition = ComboQueryConditions.SelectedItem as QueryCondition;
                    if (queryCondition == null && string.IsNullOrEmpty(ComboQueryConditions.Text))
                    {
                        ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N018", "Query condition name empty")));
                        return false;
                    }
                    //保存查询条件的名字最大长度为64，超过64就会出错误提示   by 汤澈
                    if (ComboQueryConditions.Text.ToString().Length >= 32)
                    {
                        ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N029", "Query condition name is too long")));
                        return false;
                    }
                    if (queryCondition == null)
                    {
                        queryCondition = new QueryCondition();
                        //新增
                        queryCondition.ID = 0;
                        queryCondition.UserID = CurrentApp.Session.UserID;
                        queryCondition.Creator = CurrentApp.Session.UserID;
                        queryCondition.CreateTime = DateTime.Now;
                        queryCondition.CreateType = "S";
                        queryCondition.LastQueryTime = DateTime.Now;
                        queryCondition.Name = ComboQueryConditions.Text;
                        queryCondition.Description = ComboQueryConditions.Text;
                        queryCondition.IsEnable = true;
                        mListQueryConditions.Add(queryCondition);
                    }
                    QueryCondition = queryCondition;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3102Codes.SaveConditions;
                    webRequest.Session = CurrentApp.Session;
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(QueryCondition);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    webRequest.ListData.Add(mListQueryConditionDetails.Count.ToString());
                    for (int i = 0; i < mListQueryConditionDetails.Count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(mListQueryConditionDetails[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return false;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return false;
                    }
                    QueryCondition.ID = Convert.ToInt64(webReturn.Data);
                    CurrentApp.WriteLog("SaveCondition", string.Format("SaveCondition end.\t{0}", webReturn.Data));
                    return SaveConditionSubItems();
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private bool SaveConditionSubItems()
        {
            try
            {
                List<string> listSubItemInfos = new List<string>();
                List<string> listTempIDs = new List<string>();
                for (int i = 0; i < mListSubItems.Count; i++)
                {
                    QueryConditionSubItem item = mListSubItems[i];
                    QueryConditionDetail detail =
                        mListQueryConditionDetails.FirstOrDefault(
                            d => d.QueryID == item.QueryID && d.ConditionItemID == item.ConditionItemID);
                    if (detail == null)
                    {
                        continue;
                    }
                    if (!listTempIDs.Contains(detail.Value01))
                    {
                        listTempIDs.Add(detail.Value01);
                        string strSubItemInfo = string.Format("{0}{1}{2}{1}{3}"
                            , QueryCondition.ID
                            , ConstValue.SPLITER_CHAR
                            , detail.ConditionItemID, detail.Value01);
                        listSubItemInfos.Add(strSubItemInfo);
                    }
                }
                if (listSubItemInfos.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3102Codes.SaveConditionSubItems;
                    webRequest.ListData.Add(listSubItemInfos.Count.ToString());
                    for (int i = 0; i < listSubItemInfos.Count; i++)
                    {
                        webRequest.ListData.Add(listSubItemInfos[i]);
                    }
                    Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return false;
                    }
                    CurrentApp.WriteLog("SaveCondition", string.Format("SaveConditionSubItems end.\t{0}", webReturn.Data));
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        #endregion

        #region EventHandler

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckQueryCondition()) { return; }

            CreateConditionResultList();
            //将多值序列写入临时表
            SaveMultiValues();
            RememberConditions();
            if (!SaveConditions())
            {
                return;
            }

            //创建条件Sql语句
            CreateConditionString();
            if (PageParent != null)
            {
                PageParent.QueryRecord(ConditionString, ConditionLog, mListQueryConditionDetails, QueryCondition);

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
        }

        #endregion

        void AddValuesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter as UCConditionItem;
            if (param.ConditionItem.ID == 3031401010000000016)
            {
                OpenInspectorConditionTree(param as UCConditionItem);
            }
            if (param.ConditionItem.ID == 3031401010000000019)
            {
                OpenSkillGroupTree(param as UCConditionItem);
            }
            if (param.ConditionItem.ID == 3031401010000000027)
            {
                OpenKeyWordList(param as UCConditionItem);
            }

        }

        private void OpenInspectorConditionTree(UCConditionItem item)
        {
            try
            {
                if (item == null) { return; }
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3102C3031401010000000016", "The List of Inspectors");
                InspectorConditionTree1.CurrentApp = CurrentApp;
                InspectorConditionTree1.PageParent = this;
                InspectorConditionTree1.UC = item;
                PopupPanel.Content = InspectorConditionTree1;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenSkillGroupTree(UCConditionItem item)
        {
            try
            {
                if (item == null) { return; }
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3102C3031401010000000019", "The List of SkillGroup");
                SkillGroupTree1.CurrentApp = CurrentApp;
                SkillGroupTree1.PageParent = this;
                SkillGroupTree1.UC = item;
                PopupPanel.Content = SkillGroupTree1;

                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenKeyWordList(UCConditionItem item)
        {
            try
            {
                if (item == null) { return; }
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3102C3031401010000000027", "The List of KeyWords");
                KeyWordList1.CurrentApp = CurrentApp;
                KeyWordList1.PageParent = this;
                KeyWordList1.UC = item;
                PopupPanel.Content = KeyWordList1;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.Title = CurrentApp.GetLanguageInfo("31021100", "Query Condition");
            }
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Cancel");
            CbRememberConditions.Content = CurrentApp.GetLanguageInfo("31021101", "Remember Conditions");
            CbSaveConditions.Content = CurrentApp.GetLanguageInfo("31021102", "Save Conditions");

            //TabItem
            for (int i = 0; i < mListTabItems.Count; i++)
            {
                TabItem tabItem = mListTabItems[i];
                var tabItemData = tabItem.Tag as ConditionTabItem;
                if (tabItemData != null)
                {
                    if (tabItemData.TabIndex < 10)
                    {
                        tabItem.Header = CurrentApp.GetLanguageInfo(string.Format("3102TAB{0}", tabItemData.TabIndex),
                            tabItemData.TabName);
                    }
                }
            }

            //条件项
            for (int i = 0; i < mListUCConditionItems.Count; i++)
            {
                mListUCConditionItems[i].ChangeLanguage();
            }
        }

        #endregion

        #region px  添加的检查各个查询条件的方法
        private bool CheckQueryCondition()
        {
            for (int i = 0; i < mListUCConditionItems.Count; i++)
            {
                var item = mListUCConditionItems[i];
                var result = item.CheckInput();
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}












































