using Common3107;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// QuerySetting.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySetting 
    {
        public QueryConditionMainView ParentPage;
        private BackgroundWorker mWorker;

        private ObjectItem mRootItem;
        public QuerySettingItems QueryItem;
        /// <summary>
        /// 保存修改查詢參數時獲取的座席信息
        /// </summary>
        private List<string> AgentItems;

        /// <summary>
        /// 服务态度
        /// </summary>
        private List<ABCD_OrgSkillGroup> ListSVABCDItem;
        /// <summary>
        /// 专业水平
        /// </summary>
        private List<ABCD_OrgSkillGroup> ListProLABCDItem;
        /// <summary>
        /// 录音时长异常
        /// </summary>
        private List<ABCD_OrgSkillGroup> ListRDEABCDItem;
        /// <summary>
        /// 重复呼入
        /// </summary>
        private List<ABCD_OrgSkillGroup> ListRCIABCDItem;

        #region 关键词

        private ObjectItem mRootKeyWord;

        /// <summary>
        /// 关键词内容
        /// </summary>
        private string KWCotent;

        /// <summary>
        /// 关键词ID
        /// </summary>
        private string KWID;
        #endregion

        public QuerySetting()
        {
            InitializeComponent();
            AgentItems = new List<string>();
            ListSVABCDItem = new List<ABCD_OrgSkillGroup>();
            ListProLABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRDEABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRCIABCDItem = new List<ABCD_OrgSkillGroup>();
            KWCotent = string.Empty;
            KWID = string.Empty;
            Loaded += QuerySetting_Loaded;
            TvObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
            Tvkeyword.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tvKeyWord_MouseLeftButtonDown), true);
        }

        void QuerySetting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mRootItem = new ObjectItem();
                mRootKeyWord = new ObjectItem();
                ParentPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    ParentPage.SetBusy(false, string.Empty);
                    InitCtrol();
                    ChangeLanguage();
                    InitOrgAndAgentAndExtension(mRootItem, S3107App.CurrentOrg);
                };
                mWorker.RunWorkerAsync();
                Tvkeyword.ItemsSource = mRootKeyWord.Children;
                TvObjects.ItemsSource = mRootItem.Children;
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
                InitABCDItem();
                if (!S3107App.QueryModify)
                {
                    #region 查询
                    rbOnTime.IsChecked = false;
                    rbOffTime.IsChecked = true;
                    ckBookMark.IsChecked = false;
                    rabDirectionAll.IsChecked = true;
                    rbIsUse.IsChecked = true;
                    DateStart.Value = DateTime.Now;
                    DateStop.Value = DateTime.Now.AddDays(1);
                    DateRStart.Value = DateTime.Now.Date;
                    DateRStop.Value = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
                    RecordSelected.IsEnabled = false;
                    //RecordsRate.IsEnabled = false;
                    #endregion

                    #region 关键词

                    ObjectItem rootK = new ObjectItem();
                    rootK.ObjID = 0;
                    rootK.Name = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
                    rootK.Description = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
                    mRootKeyWord.AddChild(rootK);
                    var items = S3107App.ListKeyWordItems.GroupBy(k => k.KeyWordID);
                    foreach (var item in items)
                    {
                        ObjectItem temp = new ObjectItem();
                        long id = 0;
                        string strName = string.Empty;
                        string strDesc = string.Empty;
                        foreach (var content in item)
                        {
                            id = content.KeyWordID;
                            strName = content.KeyWordSrt;
                            strDesc += string.Format("{0};", content.KWContent);
                        }
                        temp.ObjID = id;
                        temp.Name = strName;
                        temp.Description = strDesc;
                        rootK.AddChild(temp);

                    }
                    #endregion
                }
                else//修改
                {
                    #region 查询
                    tbSettingName.Text = QueryItem.QuerySettingName;
                    DateStart.Value = Convert.ToDateTime(QueryItem.QueryStartTime);
                    DateStop.Value = Convert.ToDateTime(QueryItem.QueryStopTime);
                    if(QueryItem.IsRecentTime=="Y")
                    {
                        rbOnTime.IsChecked = true;
                        //DateStart.IsEnabled = false;
                        //DateStop.IsEnabled = false;
                        ibTimeInput.Text = QueryItem.RecentTimeNum;
                        switch(QueryItem.RecentTimeType)
                        {
                            case "Y":
                                cbRecentTime.SelectedIndex = 0;
                                break;
                            case "M":
                                cbRecentTime.SelectedIndex = 1;
                                break;
                            case "D":
                                cbRecentTime.SelectedIndex = 2;
                                break;
                            case "H":
                                cbRecentTime.SelectedIndex = 3;
                                break;
                        }
                    }
                    else { rbOffTime.IsChecked = true; }
                    DateRStart.Value = Convert.ToDateTime(QueryItem.StartRecordTime);
                    DateRStop.Value = Convert.ToDateTime(QueryItem.StopRecordTime);
                    switch(QueryItem.CallDirection)
                    {
                        case "2":
                            rabDirectionAll.IsChecked = true;
                            break;
                        case "1":
                            rabDirectionIn.IsChecked = true;
                            break;
                        case "0":
                            rabDirectionOut.IsChecked = true;
                            break;
                    }
                    if(QueryItem.IsUsed == "Y" )
                    {
                         rbIsUse.IsChecked = true;
                    }
                    else
                    {
                        rbNoUse.IsChecked = true;
                    }
                    Txt01.Text = QueryItem.DurationMin.ToString();
                    Txt02.Text = QueryItem.DurationMax.ToString();
                    if (!string.IsNullOrWhiteSpace(QueryItem.BookMarkStr))
                    {
                        ckBookMark.IsChecked = true;
                        tbBookMark.Text = QueryItem.BookMarkStr;
                    }
                    #endregion 

                    #region 座席
                    string agent = QueryItem.AgentsIDOne + QueryItem.AgentsIDTwo + QueryItem.AgentsIDThree;
                    txtAgent.Text = agent;
                    string[] tempAgent = agent.Split(new[] { ","},StringSplitOptions.RemoveEmptyEntries);
                    AgentItems = new List<string>(tempAgent);
                    cbAllotType.SelectedIndex = QueryItem.AgentAssType==0?0:1;
                    RecordSelected.Text = QueryItem.AgentAssNum;
                    //RecordsRate.Text = QueryItem.AgentAssRate;
                    #endregion

                    #region ABCD查询项的处理
                    if (!string.IsNullOrWhiteSpace(QueryItem.ABCDSetting))
                    {
                        string[] tempArr = QueryItem.ABCDSetting.Split(',');
                        foreach (string tempList in tempArr)
                        {
                            string[] tempABCD = tempList.Split('-');
                            if (tempABCD[0] == S3107Consts.WDE_ServiceAttitude)
                            {
                                chkServiceAttitude.IsChecked = true;
                                int index = ListSVABCDItem.FindIndex(f => f.OrgSkillGroupID == Convert.ToInt64(tempABCD[1]));
                                cbSvAttitude.SelectedIndex = index;
                                switch (tempABCD[2])
                                {
                                    case"0":
                                        rbSAGood.IsChecked = true;
                                        break;
                                    case "1":
                                        rbSABad.IsChecked = true;
                                        break;
                                    case "2":
                                        rbSAAll.IsChecked = true;
                                        break;
                                }
                            }
                            if (tempABCD[0] == S3107Consts.WDE_ProfessionalLevel)
                            {
                                chkProfessionalLevel.IsChecked = true;
                                int index = ListProLABCDItem.FindIndex(f => f.OrgSkillGroupID == Convert.ToInt64(tempABCD[1]));
                                cbProLevel.SelectedIndex = index;
                                switch (tempABCD[2])
                                {
                                    case "0":
                                        rbPLGood.IsChecked = true;
                                        break;
                                    case "1":
                                        rbPLBad.IsChecked = true;
                                        break;
                                    case "2":
                                        rbPLAll.IsChecked = true;
                                        break;
                                }
                            }
                            if (tempABCD[0] == S3107Consts.WDE_RecordDurationError)
                            {
                                chkRecordDurationError.IsChecked = true;
                                int index = ListRDEABCDItem.FindIndex(f => f.OrgSkillGroupID == Convert.ToInt64(tempABCD[1]));
                                cbRDError.SelectedIndex = index;
                                switch (tempABCD[2])
                                {
                                    case "0":
                                        rbRDGood.IsChecked = true;
                                        break;
                                    case "1":
                                        rbRDBad.IsChecked = true;
                                        break;
                                    case "2":
                                        rbRDAll.IsChecked = true;
                                        break;
                                }
                            }
                            if (tempABCD[0] == S3107Consts.WDE_RepeatCallIn)
                            {
                                chkRepeatCallIn.IsChecked = true;
                                int index = ListRCIABCDItem.FindIndex(f => f.OrgSkillGroupID == Convert.ToInt64(tempABCD[1]));
                                cbRepeatCallIn.SelectedIndex = index;
                                switch (tempABCD[2])
                                {
                                    case "0":
                                        rbRCNo.IsChecked = true;
                                        break;
                                    case "1":
                                        rbRCYes.IsChecked = true;
                                        break;
                                    case "2":
                                        rbRCAll.IsChecked = true;
                                        break;
                                }
                            }
                        }
                    }


                    #endregion

                    #region 关键词
                    KWID = QueryItem.LQKeyWordItemsOne + QueryItem.LQKeyWordItemsTwo + QueryItem.LQKeyWordItemsThree;
                    ObjectItem rootK = new ObjectItem();
                    rootK.ObjID = 0;
                    rootK.Name = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
                    rootK.Description = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
                    mRootKeyWord.AddChild(rootK);
                    var items = S3107App.ListKeyWordItems.GroupBy(k => k.KeyWordID);
                    foreach (var item in items)
                    {
                        ObjectItem temp = new ObjectItem();
                        long id = 0;
                        string strName = string.Empty;
                        string strDesc = string.Empty;
                        foreach (var content in item)
                        {
                            id = content.KeyWordID;
                            strName = content.KeyWordSrt;
                            strDesc += string.Format("{0};", content.KWContent);
                        }
                        if (KWID.Contains(id.ToString()))
                        {
                            temp.IsChecked = true;
                        }
                        temp.ObjID = id;
                        temp.Name = strName;
                        temp.Description = strDesc;
                        rootK.AddChild(temp);

                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 座席/分机
        void InitOrgAndAgentAndExtension(ObjectItem parentItem, string parentID)
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
                InitOrgAndAgentAndExtension(item, org.ID);
                if (S3107App.GroupingWay.Contains("A"))
                {
                    InitControlAgents(item, org.ID);
                }
                else if (S3107App.GroupingWay.Contains("R"))
                {
                    InitControlRealityExtension(item, org.ID);
                }
                else if (S3107App.GroupingWay.Contains("E"))
                {
                    InitControlExtension(item, org.ID);
                }
                AddChildObject(parentItem, item);
            }
        }

        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void InitControlAgents(ObjectItem parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                lstCtrolAgentTemp = S3107App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == parentID).ToList();
                foreach (CtrolAgent agent in lstCtrolAgentTemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(agent.AgentID);
                    item.Name = agent.AgentName;
                    if(AgentItems.Where(p=>p==item.Name).Count()>0)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                    item.Description = agent.AgentFullName;
                    item.Data = agent;
                    item.Icon = "/Themes/Default/UMPS3107/Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealityExtension(ObjectItem parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolRexTemp = new List<CtrolAgent>();
                lstCtrolRexTemp = S3107App.ListCtrolRealityExtension.Where(p => p.AgentOrgID == parentID).ToList();
                foreach (CtrolAgent rex in lstCtrolRexTemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(rex.AgentID);
                    item.Name = rex.AgentName;
                    item.Description = rex.AgentFullName;
                    item.Data = rex;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3107/Images/RealExtension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtension(ObjectItem parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolRexTemp = new List<CtrolAgent>();
                lstCtrolRexTemp = S3107App.ListCtrolExtension.Where(p => p.AgentOrgID == parentID).ToList();
                foreach (CtrolAgent cEx in lstCtrolRexTemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(cEx.AgentID);
                    item.Name = cEx.AgentName;
                    item.Description = cEx.AgentFullName;
                    item.Data = cEx;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3107/Images/extension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //座席树触发事件
        private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(500);//次线程休眠1秒
                Dispatcher.Invoke(new Action(() =>
                {
                    List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                    int ARType = 0;
                    if (S3107App.GroupingWay.Contains("A"))
                    {
                        ARType=ConstValue.RESOURCE_AGENT;
                    }
                    else if (S3107App.GroupingWay.Contains("R"))
                    {
                        ARType=ConstValue.RESOURCE_REALEXT;
                    }
                    else if (S3107App.GroupingWay.Contains("E"))
                    {
                        ARType=ConstValue.RESOURCE_EXTENSION;
                    }
                    GetAgentIsCheck(mRootItem, ref lstCtrolAgentTemp, ARType);
                    if (lstCtrolAgentTemp.Count > 0)
                    {
                        string stra = "";
                        foreach (CtrolAgent ca in lstCtrolAgentTemp)
                        {
                            stra += ca.AgentName + ",";
                        }
                        stra = stra.TrimEnd(',');
                        txtAgent.Text = stra;
                    }
                    else
                        txtAgent.Text = "";
                }));
            });
            t.Start();
        }

        private void GetAgentIsCheck(ObjectItem parent, ref List<CtrolAgent> lstCtrolAgent, int ARType)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ARType)
                {
                    CtrolAgent ctrolAgent = new CtrolAgent();
                    ctrolAgent.AgentID = o.ObjID.ToString();
                    ctrolAgent.AgentName = o.Name;
                    ctrolAgent.AgentFullName = o.Description;
                    lstCtrolAgent.Add(ctrolAgent);
                }
                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetAgentIsCheck(o, ref lstCtrolAgent, ARType);
                }
            }
        }


        #endregion

        #region ABCD

        //初始化ABCD查询配置
        public void InitABCDItem()
        {
            try
            {
                ComboBoxItem tempBox;
                string LogStr;
                //服务态度
                ListSVABCDItem = InitABCDQuery(S3107Consts.WDE_ServiceAttitude);
                if (ListSVABCDItem.Count() > 0)
                {
                    LogStr = string.Empty;
                    for (int i = 0; i < ListSVABCDItem.Count(); i++)
                    {
                        tempBox = new ComboBoxItem();
                        tempBox.Content = ListSVABCDItem[i].OrgSkillGroupName;
                        LogStr += string.Format("{0}--{1}\t", ListSVABCDItem[i].OrgSkillGroupName, ListSVABCDItem[i].OrgSkillGroupID);
                        cbSvAttitude.Items.Add(tempBox);
                    }
                    CurrentApp.WriteLog("服务态度", string.Format("{0}", LogStr));
                }

                //专业水平
                ListProLABCDItem = InitABCDQuery(S3107Consts.WDE_ProfessionalLevel);
                if (ListProLABCDItem.Count() > 0)
                {
                    LogStr = string.Empty;
                    for (int i = 0; i < ListProLABCDItem.Count(); i++)
                    {
                        tempBox = new ComboBoxItem();
                        tempBox.Content = ListProLABCDItem[i].OrgSkillGroupName;
                        LogStr += string.Format("{0}--{1}\t", ListProLABCDItem[i].OrgSkillGroupName, ListProLABCDItem[i].OrgSkillGroupID);
                        cbProLevel.Items.Add(tempBox);
                    }
                    CurrentApp.WriteLog("专业水平", string.Format("{0}", LogStr));
                }

                //录音时长异常
                ListRDEABCDItem = InitABCDQuery(S3107Consts.WDE_RecordDurationError);
                if (ListRDEABCDItem.Count() > 0)
                {
                    LogStr = string.Empty;
                    for (int i = 0; i < ListRDEABCDItem.Count(); i++)
                    {
                        tempBox = new ComboBoxItem();
                        tempBox.Content = ListRDEABCDItem[i].OrgSkillGroupName;
                        LogStr += string.Format("{0}--{1}\t", ListRDEABCDItem[i].OrgSkillGroupName, ListRDEABCDItem[i].OrgSkillGroupID);
                        cbRDError.Items.Add(tempBox);
                    }
                    CurrentApp.WriteLog("录音时长异常", string.Format("{0}", LogStr));
                }

                //重复呼入
                ListRCIABCDItem = InitABCDQuery(S3107Consts.WDE_RepeatCallIn);
                if (ListRCIABCDItem.Count() > 0)
                {
                    LogStr = string.Empty;
                    for (int i = 0; i < ListRCIABCDItem.Count(); i++)
                    {
                        tempBox = new ComboBoxItem();
                        tempBox.Content = ListRCIABCDItem[i].OrgSkillGroupName;
                        LogStr += string.Format("{0}--{1}\t", ListRCIABCDItem[i].OrgSkillGroupName, ListRCIABCDItem[i].OrgSkillGroupID);
                        cbRepeatCallIn.Items.Add(tempBox);
                    }
                    CurrentApp.WriteLog("重复呼入", string.Format("{0}", LogStr));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private List<ABCD_OrgSkillGroup> InitABCDQuery(string OperationID)
        {
            List<ABCD_OrgSkillGroup> tempItems = new List<ABCD_OrgSkillGroup>();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.GetABCD;
                webRequest.ListData.Add(OperationID);
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                string tempAgent;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ABCD_OrgSkillGroup>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return tempItems;
                    }
                    ABCD_OrgSkillGroup info = optReturn.Data as ABCD_OrgSkillGroup;
                    tempAgent = string.Empty;
                    List<CtrolAgent> lstCtrolAgentTemp = null;
                    if (S3107App.GroupingWay.Contains("A"))
                    {
                        lstCtrolAgentTemp = S3107App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                    }
                    else if (S3107App.GroupingWay.Contains("R"))
                    {
                        lstCtrolAgentTemp = S3107App.ListCtrolRealityExtension.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                    }
                    else if (S3107App.GroupingWay.Contains("E"))
                    {
                        lstCtrolAgentTemp = S3107App.ListCtrolExtension.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                    }
                    if (lstCtrolAgentTemp.Count() < 1) { return tempItems; }
                    foreach (CtrolAgent agent in lstCtrolAgentTemp)
                    {
                        tempAgent += string.Format("{0},", agent.AgentName);
                    }
                    info.ManageAgent = tempAgent.Substring(0, tempAgent.Length - 1);
                    tempItems.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return tempItems;
            }
            return tempItems;
        }

        #endregion

        #region 关键词

        /// <summary>
        /// 关键词触发事件
        /// </summary>
        private void tvKeyWord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(500);//次线程休眠1秒
                Dispatcher.Invoke(new Action(() =>
                {
                    KWCotent = string.Empty;
                    KWID = string.Empty;
                    GetKWCondition(mRootKeyWord);
                }));
            });
            t.Start();
        }

        private void GetKWCondition(ObjectItem parent)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjID > 0)
                {
                    KWCotent += o.Description;
                    if (!KWID.Contains(o.ObjID.ToString()))
                    {
                        KWID += o.ObjID.ToString()+";";
                    }
                }
                GetKWCondition(o);
            }
        }
        #endregion

        #region Click
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
                if(!CreateQueryCondition())return;
                if(!InsertQuerySetting(QueryItem))return;
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    ParentPage.InitQueryDetail();
                    #region 写操作日志
                    string OpID = S3107App.QueryModify == true ? "3107T00004" : "3107T00003";
                    string strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString(OpID), Utils.FormatOptLogString("3107T00028"), QueryItem.QuerySettingName);
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


        private void cbAllotType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordSelected.IsEnabled = true;
            //RecordSelected.Text = string.Empty;
            //RecordsRate.Text = string.Empty;
            //switch (cbAllotType.SelectedIndex)
            //{
            //    case 0:
            //    case 2:
            //        RecordSelected.IsEnabled = true;
            //        RecordsRate.IsEnabled = false;
            //        break;
            //    case 1:
            //    case 3:
            //        RecordSelected.IsEnabled = false;
            //        RecordsRate.IsEnabled = true;
            //        break;

            //}
        }


        private void tbRecord_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal)//数字键盘区域|| e.Key.ToString() == "Tab"
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

        #endregion

        private bool CreateQueryCondition()
        {
            if (!S3107App.QueryModify)
            {
                QueryItem = new QuerySettingItems();
            }
            bool flag = true;
            try 
            {
                if (!string.IsNullOrWhiteSpace(tbSettingName.Text))
                {
                    QueryItem.QuerySettingName = tbSettingName.Text;
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00083", "Please Input QueryName")); 
                    return false;
                }

                #region 查询设置
                #region 时间
                if (rbOnTime.IsChecked==true)//将“最近时间”转换为具体的时间格式，赋值给DateTimePicker控件，然后生效-失效时间从DateTimePicker中获取；最近时间--往前推数天
                {
                    QueryItem.IsRecentTime = "Y";
                    if (string.IsNullOrWhiteSpace(ibTimeInput.Text) || cbRecentTime.SelectedItem==null)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00084", "Please selet datetime"));
                        return false;
                    }
                    QueryItem.IsRecentTime="Y";
                    QueryItem.RecentTimeNum = ibTimeInput.Value.ToString();
                    DateRStop.Value = DateTime.Now.Date;
                    switch(cbRecentTime.SelectedIndex)
                    {
                        case 0:
                            QueryItem.RecentTimeType="Y";
                            DateRStart.Value = DateTime.Now.Date.AddYears(Convert.ToInt32(-ibTimeInput.Value));
                            break;
                        case 1:
                            QueryItem.RecentTimeType="M";
                            DateRStart.Value = DateTime.Now.Date.AddMonths(Convert.ToInt32(-ibTimeInput.Value));
                            break;
                        case 2:
                            QueryItem.RecentTimeType="D";
                            DateRStart.Value = DateTime.Now.Date.AddDays(Convert.ToDouble(-ibTimeInput.Value));
                            break;
                        case 3:
                            QueryItem.RecentTimeType="H";
                            DateRStart.Value = DateTime.Now.Date.AddHours(Convert.ToDouble(-ibTimeInput.Value));
                            break;
                    }
                }
                else
                    QueryItem.IsRecentTime = "N";
                if (DateStart.Value.Value.Year < 1970 || DateStop.Value.Value.Year < 1970)
                {
                    DateStart.Value = DateTime.Parse("1970-1-1 0:0:0");
                    DateStop.Value = DateTime.Parse("1970-1-1 0:0:0");
                }
                if (DateStart.Value > DateStop.Value)//开始时间大于结束时间的处理
                {
                    DateStart.Value = DateStop.Value;
                }
                if (DateRStart.Value > DateRStop.Value)
                {
                    DateRStart.Value = DateRStop.Value;
                }
                QueryItem.QueryStartTime = DateTime.Parse(DateStart.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");//.ToUniversalTime()
                QueryItem.QueryStopTime = DateTime.Parse(DateStop.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");//.ToUniversalTime()

                QueryItem.StartRecordTime = DateTime.Parse(DateRStart.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");//.ToUniversalTime()
                QueryItem.StopRecordTime = DateTime.Parse(DateRStop.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");//.ToUniversalTime()

                if (string.IsNullOrWhiteSpace(Txt01.Text) || string.IsNullOrWhiteSpace(Txt02.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00086", "Please Input QueryDuration"));
                    return false;
                }
                QueryItem.DurationMin = Convert.ToInt32(Txt01.Text);
                QueryItem.DurationMax = Convert.ToInt32(Txt02.Text);
                if (QueryItem.DurationMin > QueryItem.DurationMax)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00085", "Please Input Right Duration"));
                    return false;
                }
                #endregion

                #region RadioButton 控件
                if (rabDirectionAll.IsChecked == true)
                {
                    QueryItem.CallDirection = "2";
                }
                if (rabDirectionIn.IsChecked == true)
                {
                    QueryItem.CallDirection = "1";
                }
                if (rabDirectionOut.IsChecked == true)
                {
                    QueryItem.CallDirection ="0";
                }
                QueryItem.IsUsed = rbIsUse.IsChecked == true ? "Y" : "N";
                #endregion

                if (ckBookMark.IsChecked.Equals(true))
                {
                    QueryItem.BookMarkStr = tbBookMark.Text;
                }
                else
                {
                    QueryItem.BookMarkStr = string.Empty;
                }
                #endregion
                
                #region ABCD
                string abcdSQL = string.Empty;//保存全部查询项的sql串
                string tempSql = string.Empty;//保存单一查询项的sql串
                string abcdSetting = string.Empty;
                string tempColumns = string.Empty;
                if (chkServiceAttitude.IsChecked.Equals(true))//服务态度项
                {
                    if (cbSvAttitude.SelectedIndex < 0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00096", "Select One Org"));
                        return false;
                    }
                    tempColumns = ReturnColumn(ListSVABCDItem[cbSvAttitude.SelectedIndex].InColumn);
                    if (string.IsNullOrWhiteSpace(tempColumns))
                    {
                        ShowException(string.Format("Get ServiceAttitude Columns Error!"));
                        return false;
                    }
                    if (rbSAGood.IsChecked.Equals(true))//1 好
                    {
                        tempSql = string.Format("T354.{0}='1' AND", tempColumns);
                        abcdSetting = string.Format("{0}-{1}-{2},", S3107Consts.WDE_ServiceAttitude, ListSVABCDItem[cbSvAttitude.SelectedIndex].OrgSkillGroupID, 0);
                    }
                    else if (rbSABad.IsChecked.Equals(true))//2 坏
                    {
                        tempSql = string.Format("T354.{0}='2' AND", tempColumns);
                        abcdSetting = string.Format("{0}-{1}-{2},", S3107Consts.WDE_ServiceAttitude, ListSVABCDItem[cbSvAttitude.SelectedIndex].OrgSkillGroupID, 1);
                    }
                    else if (rbSAAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        tempSql = string.Format("T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting = string.Format("{0}-{1}-{2},", S3107Consts.WDE_ServiceAttitude, ListSVABCDItem[cbSvAttitude.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    if (string.IsNullOrWhiteSpace(tempSql))
                    {
                        rbSAAll.IsChecked = true;
                        tempSql = string.Format("T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting = string.Format("{0}-{1}-{2},", S3107Consts.WDE_ServiceAttitude, ListSVABCDItem[cbSvAttitude.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    abcdSQL = tempSql;
                }

                if (chkProfessionalLevel.IsChecked.Equals(true))//专业水平
                {
                    tempSql = string.Empty;
                    if (cbProLevel.SelectedIndex < 0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00096", "Select One Org"));
                        return false;
                    }
                    tempColumns = ReturnColumn(ListProLABCDItem[cbProLevel.SelectedIndex].InColumn);
                    if (string.IsNullOrWhiteSpace(tempColumns))
                    {
                        ShowException(string.Format("Get ProfessionalLevel Columns Error!"));
                        return false;
                    }
                    if (rbPLGood.IsChecked.Equals(true))//1 好
                    {
                        tempSql = string.Format("T354.{0}='1' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_ProfessionalLevel, ListProLABCDItem[cbProLevel.SelectedIndex].OrgSkillGroupID, 0);
                    }
                    else if (rbPLBad.IsChecked.Equals(true))//2 坏
                    {
                        tempSql = string.Format(" T354.{0}='2' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_ProfessionalLevel, ListProLABCDItem[cbProLevel.SelectedIndex].OrgSkillGroupID, 1);
                    }
                    else if (rbPLAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_ProfessionalLevel, ListProLABCDItem[cbProLevel.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    if (string.IsNullOrWhiteSpace(tempSql))
                    {
                        rbPLAll.IsChecked = true;
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_ProfessionalLevel, ListProLABCDItem[cbProLevel.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    abcdSQL += tempSql;
                }

                if (chkRecordDurationError.IsChecked.Equals(true))//录音时长异常
                {
                    tempSql = string.Empty;
                    if (cbRDError.SelectedIndex < 0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00096", "Select One Org"));
                        return false;
                    }
                    tempColumns = ReturnColumn(ListRDEABCDItem[cbRDError.SelectedIndex].InColumn);
                    if (string.IsNullOrWhiteSpace(tempColumns))
                    {
                        ShowException(string.Format("Get RecordDurationError Columns Error!"));
                        return false;
                    }
                    if (rbRDGood.IsChecked.Equals(true))//1 正常
                    {
                        tempSql = string.Format("T354.{0}='1' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RecordDurationError, ListRDEABCDItem[cbRDError.SelectedIndex].OrgSkillGroupID, 0);
                    }
                    else if (rbRDBad.IsChecked.Equals(true))//2 异常
                    {
                        tempSql = string.Format(" T354.{0}='2' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RecordDurationError, ListRDEABCDItem[cbRDError.SelectedIndex].OrgSkillGroupID, 1);
                    }
                    else if (rbRDAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RecordDurationError, ListRDEABCDItem[cbRDError.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    if (string.IsNullOrWhiteSpace(tempSql))
                    {
                        rbRDAll.IsChecked = true;
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RecordDurationError, ListRDEABCDItem[cbRDError.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    abcdSQL += tempSql;
                }

                if (chkRepeatCallIn.IsChecked.Equals(true))//重复呼入
                {
                    tempSql = string.Empty;
                    if (cbRepeatCallIn.SelectedIndex < 0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3107T00096", "Select One Org"));
                        return false;
                    }
                    tempColumns = ReturnColumn(ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].InColumn);
                    if (string.IsNullOrWhiteSpace(tempColumns))
                    {
                        ShowException(string.Format("Get RepeatCallIn Columns Error!"));
                        return false;
                    }
                    if (rbRCNo.IsChecked.Equals(true))//1 非重复呼入
                    {
                        tempSql = string.Format(" T354.{0}='1' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RepeatCallIn, ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].OrgSkillGroupID, 0);
                    }
                    else if (rbRCYes.IsChecked.Equals(true))//2 重复呼入
                    {
                        tempSql = string.Format("T354.{0}='2' AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RepeatCallIn, ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].OrgSkillGroupID, 1);
                    }
                    else if (rbRCAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RepeatCallIn, ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    if (string.IsNullOrWhiteSpace(tempSql))
                    {
                        rbRCAll.IsChecked = true;
                        tempSql = string.Format(" T354.{0} IN('1','2') AND", tempColumns);
                        abcdSetting += string.Format("{0}-{1}-{2},", S3107Consts.WDE_RepeatCallIn, ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].OrgSkillGroupID, 2);
                    }
                    abcdSQL += tempSql;
                }
                QueryItem.ABCDSetting = abcdSetting.Trim(',');
                if (!string.IsNullOrWhiteSpace(abcdSQL))
                {
                    QueryItem.ABCDSql = abcdSQL.Remove(abcdSQL.LastIndexOf("AND"));
                }
                else { QueryItem.ABCDSql = string.Empty; }

                #endregion

                #region 座席 /分机
                if (string.IsNullOrWhiteSpace(txtAgent.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00087", "Please Selet Agent"));
                    return false;
                }
                string TempABCDAgent = string.Empty;//保存abcd选中的机构下的座席、分机
                if (chkServiceAttitude.IsChecked.Equals(true))
                {
                    TempABCDAgent += ListSVABCDItem[cbSvAttitude.SelectedIndex].ManageAgent + ",";
                }
                if (chkProfessionalLevel.IsChecked.Equals(true))
                {
                    TempABCDAgent += ListProLABCDItem[cbProLevel.SelectedIndex].ManageAgent + ",";
                }
                if (chkRecordDurationError.IsChecked.Equals(true))
                {
                    TempABCDAgent += ListRDEABCDItem[cbRDError.SelectedIndex].ManageAgent + ",";
                }
                if (chkRepeatCallIn.IsChecked.Equals(true))
                {
                    TempABCDAgent += ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].ManageAgent + ",";
                }
                TempABCDAgent = TempABCDAgent.Trim(',');
                string[] tempArr = TempABCDAgent.Split(',');//取选中的座席/分机跟选中的abcd项机构下属座席/分机的并集
                if (tempArr.Count() > 0)
                {
                    foreach (string tempAgent in tempArr)
                    {
                        if (!txtAgent.Text.Contains(tempAgent))
                        {
                            txtAgent.Text += "," + tempAgent;
                        }
                    }
                }
                if (txtAgent.Text.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00088", "Agent Is Too larger"));
                    return false;
                }
                if(txtAgent.Text.Length>4000 && txtAgent.Text.Length<6000)
                {
                    QueryItem.AgentsIDOne = txtAgent.Text.Substring(0, 2000);
                    QueryItem.AgentsIDTwo = txtAgent.Text.Substring(2000, 2000);
                    QueryItem.AgentsIDThree = txtAgent.Text.Substring(4000, txtAgent.Text.Length - 4);
                }
                if (txtAgent.Text.Length > 2000 && txtAgent.Text.Length <= 4000)
                {
                    QueryItem.AgentsIDOne = txtAgent.Text.Substring(0, 2000);
                    QueryItem.AgentsIDTwo = txtAgent.Text.Substring(2000, txtAgent.Text.Length - 2000);
                    QueryItem.AgentsIDThree = string.Empty;
                }
                if (txtAgent.Text.Length <= 2000)
                {
                    QueryItem.AgentsIDOne = txtAgent.Text;
                    QueryItem.AgentsIDTwo = string.Empty;
                    QueryItem.AgentsIDThree = string.Empty;
                }

                if (cbAllotType.SelectedItem==null)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3107T00089", "cbAllotType is null"));
                    return false;
                }
                //QueryItem.AgentAssType = cbAllotType.SelectedIndex;
                switch(cbAllotType.SelectedIndex)
                {
                    case 0:
                        QueryItem.AgentAssType = 0;
                        if (string.IsNullOrWhiteSpace(RecordSelected.Text))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00090", "Please Input Record Num"));
                            RecordSelected.Focus();
                            return false;
                        }
                        else
                        {
                            QueryItem.AgentAssNum = RecordSelected.Text;
                        }
                        break;
                    case 1:
                        QueryItem.AgentAssType = 2;
                        if (string.IsNullOrWhiteSpace(RecordSelected.Text))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3107T00090", "Please Input Record Num"));
                            RecordSelected.Focus();
                            return false;
                        }
                        else
                        {
                            QueryItem.AgentAssNum = RecordSelected.Text;
                        }
                        break;
                    //case 1:
                    //case 3:
                    //    if(string.IsNullOrWhiteSpace(RecordsRate.Text))
                    //    {
                    //        RecordsRate.Focus();
                    //        return false;
                    //    }
                    //    else
                    //    {
                    //        QueryItem.AgentAssRate = RecordsRate.Text;
                    //    }
                    //    break;
                }
                #endregion

                #region 关键词
                //内容
                if (KWCotent.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00107", "KeyWord Is Too larger"));
                    return false;
                }
                if (KWCotent.Length > 4000 && KWCotent.Length < 6000)
                {
                    QueryItem.KeyContentOne = KWCotent.Substring(0, 2000);
                    QueryItem.KeyContentTwo = KWCotent.Substring(2000, 2000);
                    QueryItem.KeyContentThree = KWCotent.Substring(4000, KWCotent.Length - 4);
                }
                if (KWCotent.Length > 2000 && KWCotent.Length <= 4000)
                {
                    QueryItem.KeyContentOne = KWCotent.Substring(0, 2000);
                    QueryItem.KeyContentTwo = KWCotent.Substring(2000, KWCotent.Length - 2000);
                    QueryItem.KeyContentThree = string.Empty;
                }
                if (KWCotent.Length <= 2000)
                {
                    QueryItem.KeyContentOne = KWCotent;
                    QueryItem.KeyContentTwo = string.Empty;
                    QueryItem.KeyContentThree = string.Empty;
                }

                //ID
                if (KWID.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00107", "KeyWord Is Too larger"));
                    return false;
                }
                if (KWID.Length > 4000 && KWID.Length < 6000)
                {
                    QueryItem.LQKeyWordItemsOne = KWID.Substring(0, 2000);
                    QueryItem.LQKeyWordItemsTwo = KWID.Substring(2000, 2000);
                    QueryItem.LQKeyWordItemsThree = KWID.Substring(4000, KWID.Length - 4);
                }
                if (KWID.Length > 2000 && KWID.Length <= 4000)
                {
                    QueryItem.LQKeyWordItemsOne = KWID.Substring(0, 2000);
                    QueryItem.LQKeyWordItemsTwo = KWID.Substring(2000, KWID.Length - 2000);
                    QueryItem.LQKeyWordItemsThree = string.Empty;
                }
                if (KWID.Length <= 2000)
                {
                    QueryItem.LQKeyWordItemsOne = KWID;
                    QueryItem.LQKeyWordItemsTwo = string.Empty;
                    QueryItem.LQKeyWordItemsThree = string.Empty;
                }
                #endregion
            }
            catch(Exception ex)
            {
                return false;
            }
            return flag;
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (isNumber(e.Text.Trim()))
            {
              //  e.Handled = true;
            }
            else
            {
                RecordSelected.Text = RecordSelected.Text.Replace(e.Text, null);
                //MessageBox.Show("输入错误");
            }
        }
        public bool isNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }                
            }
            return true;
        }

        public string ReturnColumn(int column)
        {
            string columstr = string.Empty;
            try
            {
                switch (column.ToString().Length)//判断T_31_054的列值
                {
                    case 1:
                        columstr = string.Format("C00{0}", column);
                        break;
                    case 2:
                        columstr = string.Format("C0{0}", column);
                        break;
                    case 3:
                        columstr = string.Format("C{0}", column);
                        break;
                    default:
                        columstr = string.Format("C{0}", column);
                        break;
                }
            }
            catch (Exception)
            {
                return columstr;                
            }
            return columstr;
        }

        private bool InsertQuerySetting(QuerySettingItems Item)
        {
            bool flag = true;
            WebRequest webRequest;
            Service31071Client client;
            WebReturn webReturn;
            try
            {
                if (!S3107App.QueryModify)
                {
                    //生成新的查询配置表主键
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("371");
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
                        return false;
                    }
                    QueryItem.QuerySettingID = Convert.ToInt64(strNewResultID);
                }

                //插入T_31_024表 查询配置参数
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3107Codes.QuerySettingDBO;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(QueryItem);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                string tempFlag = S3107App.QueryModify == true ? "T" : "F";
                webRequest.ListData.Add(tempFlag);
                client = new Service31071Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31071"));
                webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return false;
                }
                ParentPage.InitTaskDetail();
            }
            catch (Exception ex)
            {
                return false;
            }
            return flag;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            querySetting.Header = CurrentApp.GetLanguageInfo("3107T00028", "Query Setting");
            tabAgent.Header = CurrentApp.GetLanguageInfo("3107T00104", "Agent/Extension");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3107T00018","OK");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3107T00019","Close");

            lbSettingName.Content = CurrentApp.GetLanguageInfo("3107T00009", "Query Setting Name");
            lbSettingStartTime.Content = CurrentApp.GetLanguageInfo("3107T00013","Setting Start Time");
            lbSettingStopTime.Content = CurrentApp.GetLanguageInfo("3107T00014", "Setting Stop Time");
            lbLastTime.Content = CurrentApp.GetLanguageInfo("3107T00030","Set Last Time");
            rbOnTime.Content = CurrentApp.GetLanguageInfo("3107T00011","Yes");
            rbOffTime.Content = CurrentApp.GetLanguageInfo("3107T00012","No");
            lbRecentTime.Content = CurrentApp.GetLanguageInfo("3107T00031","Recent Time");
            cbRecentTime1.Content = CurrentApp.GetLanguageInfo("3107T00036", "Years");
            cbRecentTime2.Content = CurrentApp.GetLanguageInfo("3107T00037", "Months");
            cbRecentTime3.Content = CurrentApp.GetLanguageInfo("3107T00038", "Days");
            cbRecentTime4.Content = CurrentApp.GetLanguageInfo("3107T00039", "Hours");
            tbRecordStartTime.Content = CurrentApp.GetLanguageInfo("3107T00015", "Recording Start Time");
            lbRecordStopTime.Content = CurrentApp.GetLanguageInfo("3107T00016", "Recording Stop Time");
            labLengthTime.Content = CurrentApp.GetLanguageInfo("3107T00032", "Record Length");
            lbCallDiretion.Content = CurrentApp.GetLanguageInfo("3107T00017", "Call Diretion");
            rabDirectionAll.Content = CurrentApp.GetLanguageInfo("3107T00033", "All");
            rabDirectionIn.Content = CurrentApp.GetLanguageInfo("3107T00034", "Call In");
            rabDirectionOut.Content = CurrentApp.GetLanguageInfo("3107T00035", "Call Out");
            lbIsUse.Content = CurrentApp.GetLanguageInfo("3107T00010","IsUse");
            rbIsUse.Content = CurrentApp.GetLanguageInfo("3107T00011","Yes");
            rbNoUse.Content = CurrentApp.GetLanguageInfo("3107T00012","No");

            labAgent.Content = CurrentApp.GetLanguageInfo("3107T00104", "Agent/Extension");
            lbAllotType.Content = CurrentApp.GetLanguageInfo("3107T00020","Allot Record Type");
            lbselectRecord.Content = CurrentApp.GetLanguageInfo("3107T00021", "Agent Assign Records");
            //lbRecordRate.Content = CurrentApp.GetLanguageInfo("3107T00022", "Agent Assign Rate");
            cbAllotType1.Content = CurrentApp.GetLanguageInfo("3107T00040", "Every Day Assign X Records");
            //cbAllotType2.Content = CurrentApp.GetLanguageInfo("3107T00041", "Every Day Assign X Rate");
            cbAllotType3.Content = CurrentApp.GetLanguageInfo("3107T00042", "Assign X Records");
            //cbAllotType4.Content = CurrentApp.GetLanguageInfo("3107T00043", "Assign X Rate");

            chkServiceAttitude.Content = CurrentApp.GetLanguageInfo("3107T00099", "ServiceAttitude");
            chkProfessionalLevel.Content = CurrentApp.GetLanguageInfo("3107T00100", "ProfessionalLevel");
            chkRecordDurationError.Content = CurrentApp.GetLanguageInfo("3107T00101", "RecordDurationError");
            chkRepeatCallIn.Content = CurrentApp.GetLanguageInfo("3107T00102", "Repeat Call In");
            rbSAAll.Content = rbPLAll.Content = rbRDAll.Content = rbRCAll.Content = CurrentApp.GetLanguageInfo("3107T00033", "All");
            rbSAGood.Content = rbPLGood.Content = CurrentApp.GetLanguageInfo("3107T00094", "Good");
            rbSABad.Content = rbPLBad.Content = CurrentApp.GetLanguageInfo("3107T00095", "Bad");
            rbRDBad.Content = CurrentApp.GetLanguageInfo("3107T00098", "Abnormal");
            rbRDGood.Content = CurrentApp.GetLanguageInfo("3107T00097", "Normal");
            rbRCNo.Content = CurrentApp.GetLanguageInfo("3107T00097", "Normal");
            rbRCYes.Content = CurrentApp.GetLanguageInfo("3107T00103", "Repetition");
            tabKeyWord.Header = CurrentApp.GetLanguageInfo("3107T00106", "All Keyword");
        }
    }
}
