using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS3104.Models;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3104
{
    /// <summary>
    /// ClientQuery.xaml 的交互逻辑
    /// </summary>
    public partial class ClientQuery
    {
        #region Members
        public AgentIntelligentClient PageParent;
        private ObservableCollection<UnitTime> mListUnitTime;
        private ObjectItemClient mRootItem;
        private List<DateTimeSpliteAsDay> mListDateTime;

        private List<QueryConditionDetail> mListQueryConditionDetails;
        private List<QueryConditionSubItem> mListSubItems;
        private List<CtrolAgent> mListCtrolAgent;

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
        #endregion

        public ClientQuery()
        {
            InitializeComponent();
            mListUnitTime = new ObservableCollection<UnitTime>();
            mListDateTime = new List<DateTimeSpliteAsDay>();
            mRootItem = new ObjectItemClient();
            mListQueryConditionDetails = new List<QueryConditionDetail>();
            mListSubItems = new List<QueryConditionSubItem>();
            mListCtrolAgent = new List<CtrolAgent>();
            ListSVABCDItem = new List<ABCD_OrgSkillGroup>();
            ListProLABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRDEABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRCIABCDItem = new List<ABCD_OrgSkillGroup>();
            ChangeLanguage();
            Loaded += ClientQuery_Loaded;
            //TvObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
        }

        void ClientQuery_Loaded(object sender, RoutedEventArgs e)
        {
            InitCombUintTime();

            combUintTime.ItemsSource = mListUnitTime;
            combUintTime.DisplayMemberPath = "Code";
            combUintTime.SelectedValuePath = "ID";


            InitOrgAndAgentAndExtension(mRootItem, App.CurrentOrg);
            //TvObjects.ItemsSource = mRootItem.Children;

            InitCtrol();
            InitABCDItem();
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            rdbTime1.Checked += rdbTime1_Checked;
            rdbTime2.Checked += rdbTime2_Checked;
            combUintTime.SelectionChanged += combUintTime_SelectionChanged;
        }

        #region 初始化数据

        private void ChangeLanguage()
        {
            tabRecordInformation.Header = App.GetLanguageInfo("3104T00039", "Record Information");
            rdbTime1.Content = App.GetLanguageInfo("3104T00065", "Record Start And Stop Time") + ":";
            rdbTime2.Content = App.GetLanguageInfo("3104T00066", "Recent Time") + ":";
            chkEnableLengthTime.Content = App.GetLanguageInfo("3104T00029", "Record Length") + ":";
            chkEnableReference.Content = App.GetLanguageInfo("3104T00001", "Record Reference") + ":";
            chkEnableCaller.Content = App.GetLanguageInfo("3104T00026", "Caller Number") + ":";
            chkEnableCalledID.Content = App.GetLanguageInfo("3104T00027", "Called Number") + ":";
            chkEnableChanel.Content = App.GetLanguageInfo("3104T00047", "Record Channel") + ":";
            //chkEnableCTIReference.Content = App.GetLanguageInfo("3104T00067", "CTI Reference") + ":";
            chkRecordReference.Content =
                chkCaller.Content =
                    chkCalled.Content =
                        chkChanle.Content = App.GetLanguageInfo("3104T00055", "Obscure Query");//chkCTIReference.Content =
            labCallDirection.Content = App.GetLanguageInfo("3104T00028", "Call Direction") + ":";
            rabDirectionAll.Content = App.GetLanguageInfo("3104T00054", "All");//rabScreenAll.Content = 
            rabDirectionIn.Content = App.GetLanguageInfo("3104T00033", "Call In");
            rabDirectionOut.Content = App.GetLanguageInfo("3104T00034", "Call Out");
            //labScreen.Content = App.GetLanguageInfo("3104T00063", "Screen Mark") + ":";
            //rabHaveScreen.Content = App.GetLanguageInfo("3104T00064", "Have Screen");
            //rabNotScreen.Content = App.GetLanguageInfo("3104T00068", "No Screen");
            labAppeal.Content = App.GetLanguageInfo("3104T00069", "Has Appeal");
            labScore.Content = App.GetLanguageInfo("3104T00035", "Has Score");
            labBookMark.Content = App.GetLanguageInfo("3104T00100", "Has BookMark");
            //chkEnableRange.Content = App.GetLanguageInfo("3104T00074", "Score Range") + ":";

            BtnConfirm.Content = App.GetLanguageInfo("3104T00070", "Confirm");
            BtnClose.Content = App.GetLanguageInfo("3104T00071", "Close");

            //tabAgentAndExtensionInfo.Header = App.GetLanguageInfo("3104T00072", "AgentAndExtension");
            //expanderAgentAndExtension.Header = App.GetLanguageInfo("3104T00073", "Agent And Extension Select");
            //chkSelectAgent.Content = labAgent.Content = App.GetLanguageInfo("3104T00030", "Agent");
            //chkSelectExtension.Content = labExtension.Content = App.GetLanguageInfo("3104T00048", "Extension");


            chkServiceAttitude.Content = App.GetLanguageInfo("3104T00174", "ServiceAttitude");
            chkProfessionalLevel.Content = App.GetLanguageInfo("3104T00175", "ProfessionalLevel");
            chkRecordDurationError.Content = App.GetLanguageInfo("3104T00176", "RecordDurationError");
            rbSAAll.Content = rbPLAll.Content = rbRDAll.Content = rbRCAll.Content = App.GetLanguageInfo("3104T00054", "All");
            rbSAGood.Content = rbPLGood.Content = App.GetLanguageInfo("3104T00182", "Good");
            rbSABad.Content = rbPLBad.Content = App.GetLanguageInfo("3104T00170", "Bad");
            rbRDBad.Content = App.GetLanguageInfo("3104T00173", "Abnormal");
            rbRDGood.Content = App.GetLanguageInfo("3104T00172", "Normal");
            chkRepeatCallIn.Content = App.GetLanguageInfo("3104T00180", "Repeat Call In");
            rbRCNo.Content = App.GetLanguageInfo("3104T00172", "Normal");
            rbRCYes.Content = App.GetLanguageInfo("3104T00181", "Repetition");
        }

        private void combUintTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTimeReturn();
        }

        public void DateTimeReturn()
        {
            try
            {
                if (rdbTime2.IsChecked.Equals(true))
                {
                    DateTime now = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
                    int Num = int.Parse(txtNumTime.Value.ToString());
                    int Type = Int16.Parse(combUintTime.SelectedValue.ToString());
                    int DayNum = 0;
                    switch (Type)
                    {
                        case 1:
                            DayNum = Num;
                            break;
                        case 2:
                            DayNum = Num * 7;
                            break;
                        case 3:
                            DayNum = Num * 30;
                            break;
                    }
                    DateTime timestart = now.AddDays(DayNum * (-1));

                    DateStart.Value = timestart;
                    DateStop.Value = now;
                }
            }
            catch (Exception ex)
            {
                DateStart.Value = DateTime.Now.Date.AddDays(360);
                DateStop.Value = DateTime.Now.Date;
            }
        }

        private void rdbTime1_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbTime1.IsChecked.Equals(true))
            {
                txtNumTime.IsEnabled = false;
                combUintTime.IsEnabled = false;

                DateStart.IsEnabled = true;
                DateStop.IsEnabled = true;
            }
        }
        private void rdbTime2_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbTime2.IsChecked.Equals(true))
            {
                DateStart.IsEnabled = false;
                DateStop.IsEnabled = false;

                txtNumTime.IsEnabled = true;
                combUintTime.IsEnabled = true;
                combUintTime.Text = App.GetLanguageInfo("3104T00101", "Day");
            }
        }

        void InitCtrol()
        {
            txtNumTime.Value = 0;
            rdbTime1.IsChecked = true;
            txtNumTime.IsEnabled = false;
            combUintTime.IsEnabled = false;

            DateStart.Value = DateTime.Now.Date;
            DateStop.Value = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);

            mtbStart.Text = "00:00:00";
            mtbStop.Text = "00:30:00";


            //labExtension.IsEnabled = false;
            //txtExtension.IsEnabled = false;
        }

        #region 座席分机数据
        //更新座席分机数据
        void InitOrgAndAgentAndExtension(ObjectItemClient parentItem, string parentID)
        {
            List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
            lstCtrolOrgTemp = App.ListCtrolOrgInfos.Where(p => p.ID == parentID).ToList();
            foreach (CtrolOrg org in lstCtrolOrgTemp)
            {
                ObjectItemClient item = new ObjectItemClient();
                item.ObjType = ConstValue.RESOURCE_ORG;
                item.ObjID = Convert.ToInt64(org.ID);
                item.Name = org.OrgName;
                item.Data = org;
                if (org.ID == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "/Themes/Default/UMPS3104/Images/rootorg.ico";
                }
                else
                {
                    item.Icon = "/Themes/Default/UMPS3104/Images/org.ico";
                }
                InitControlAgents(item, org.ID);
                AddChildObject(parentItem, item);
            }
        }

        private void InitControlAgents(ObjectItemClient parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                lstCtrolAgentTemp = App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == parentID).ToList();
                foreach (CtrolAgent agent in lstCtrolAgentTemp)
                {
                    ObjectItemClient item = new ObjectItemClient();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(agent.AgentID);
                    item.Name = agent.AgentName;
                    item.Description = agent.AgentFullName;
                    item.Data = agent;
                    item.Icon = "/Themes/Default/UMPS3104/Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void InitCombUintTime()
        {
            mListUnitTime.Clear();
            UnitTime unittime = new UnitTime();
            unittime.ID = 1;
            unittime.Code = App.GetLanguageInfo("3104T00101", "Day");
            mListUnitTime.Add(unittime);
            unittime = new UnitTime();
            unittime.ID = 2;
            unittime.Code = App.GetLanguageInfo("3104T00102", "Week");
            mListUnitTime.Add(unittime);

            unittime = new UnitTime();
            unittime.ID = 3;
            unittime.Code = App.GetLanguageInfo("3104T00103", "Month");
            mListUnitTime.Add(unittime);
        }


        //private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Thread t = new Thread(() =>
        //    {
        //        Thread.Sleep(500);//次线程休眠1秒
        //        Dispatcher.Invoke(new Action(() =>
        //        {
        //            List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
        //            GetAgentIsCheck(mRootItem, ref lstCtrolAgentTemp);
        //            if (lstCtrolAgentTemp.Count > 0)
        //            {
        //                string stra = "";
        //                foreach (CtrolAgent ca in lstCtrolAgentTemp)
        //                {
        //                    stra += ca.AgentName + ",";
        //                }
        //                stra = stra.TrimEnd(',');
        //                txtAgent.Text = stra;
        //            }
        //            else
        //                txtAgent.Text = "";
        //        }));
        //    });
        //    t.Start();
        //}

        private void GetAgentIsCheck(ObjectItemClient parent, ref List<CtrolAgent> lstCtrolAgent)
        {
            foreach (ObjectItemClient o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ConstValue.RESOURCE_AGENT)//座席編號103
                {
                    CtrolAgent ctrolAgent = new CtrolAgent();
                    ctrolAgent.AgentID = o.ObjID.ToString();
                    ctrolAgent.AgentName = o.Name;
                    ctrolAgent.AgentFullName = o.Description;
                    lstCtrolAgent.Add(ctrolAgent);
                }
                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetAgentIsCheck(o, ref lstCtrolAgent);
                }
            }
        }


        private void AddChildObject(ObjectItemClient parentItem, ObjectItemClient item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
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
                ListSVABCDItem = InitABCDQuery(S3104Consts.WDE_ServiceAttitude);
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
                    App.WriteLog("服务态度", string.Format("{0}", LogStr));
                }

                //专业水平
                ListProLABCDItem = InitABCDQuery(S3104Consts.WDE_ProfessionalLevel);
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
                    App.WriteLog("专业水平", string.Format("{0}", LogStr));
                }

                //录音时长异常
                ListRDEABCDItem = InitABCDQuery(S3104Consts.WDE_RecordDurationError);
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
                    App.WriteLog("录音时长异常", string.Format("{0}", LogStr));
                }

                //重复呼入
                ListRCIABCDItem = InitABCDQuery(S3104Consts.WDE_RepeatCallIn);
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
                    App.WriteLog("重复呼入", string.Format("{0}", LogStr));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private List<ABCD_OrgSkillGroup> InitABCDQuery(long OperationID)
        {
            List<ABCD_OrgSkillGroup> tempItems = new List<ABCD_OrgSkillGroup>();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetABCD;
                webRequest.ListData.Add(OperationID.ToString());
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                string tempAgent;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ABCD_OrgSkillGroup>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return tempItems;
                    }
                    ABCD_OrgSkillGroup info = optReturn.Data as ABCD_OrgSkillGroup;
                    tempAgent = string.Empty;
                    List<CtrolAgent> lstCtrolAgentTemp = null;
                    lstCtrolAgentTemp = App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                    if (lstCtrolAgentTemp.Count() < 1) { return tempItems; }
                    if (lstCtrolAgentTemp.Where(p => p.AgentID == App.Session.UserID.ToString()).Count() > 0)
                    {
                        tempItems.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return tempItems;
            }
            return tempItems;
        }
        
        #endregion
        #endregion

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DateStart.Value.ToString()) || string.IsNullOrWhiteSpace(DateStop.Value.ToString()))
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00123", "DateTime Is Null"));
                    return;
                }
                else if (DateTime.Parse(DateStart.Value.ToString()).CompareTo(DateTime.Parse(DateStop.Value.ToString())).Equals(1))
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00124", "DateTime Is Wrong"));
                    return;
                }

                if (!CreateQueryCondition())
                {
                    return;
                }
                if (PageParent != null)
                {
                    PageParent.QueryRecord(mListQueryConditionDetails, mListCtrolAgent, mListDateTime);
                    App.ListQueryConditionDetails = mListQueryConditionDetails;
                    App.ListCtrolAgent = mListCtrolAgent;
                    App.ListDateTime = mListDateTime;
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                }
            }
            catch (Exception)
            {

            }

        }
        private bool CreateQueryCondition()
        {
            bool flag = true;
            try
            {
                mListSubItems.Clear();
                mListQueryConditionDetails.Clear();
                mListCtrolAgent.Clear();
                //匹配纯数字的
                string pattern = @"^\d+$";
                //時間範圍
                if (!CheckInput()) { return false; }

                DateTimeReturn();
                DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                dateTimeSpliteAsDay.StartDayTime = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime();
                dateTimeSpliteAsDay.StopDayTime = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime();
                mListDateTime.Add(dateTimeSpliteAsDay);

                QueryConditionDetail querycontionDetail = new QueryConditionDetail();

                //最近時間
                querycontionDetail = new QueryConditionDetail();
                querycontionDetail.ConditionItemID = S3104Consts.CON_TIMEFROMTO;
                querycontionDetail.IsEnable = true;
                querycontionDetail.Value01 = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                querycontionDetail.Value02 = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                mListQueryConditionDetails.Add(querycontionDetail);

                //if (rdbTime1.IsChecked.Equals(true))
                //{
                //}

                //if (rdbTime2.IsChecked.Equals(true))
                //{
                //    querycontionDetail = new QueryConditionDetail();
                //    querycontionDetail.ConditionItemID = S3104Consts.CON_TIMEFROMTO;
                //    querycontionDetail.IsEnable = true;
                //    querycontionDetail.Value01 = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                //    querycontionDetail.Value02 = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                //    mListQueryConditionDetails.Add(querycontionDetail);
                //}

                //录音时长
                if (chkEnableLengthTime.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_DURATIONFROMTO;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.Value01 = Converter.Time2Second(mtbStart.Text).ToString();
                    querycontionDetail.Value02 = Converter.Time2Second(mtbStop.Text).ToString();
                    mListQueryConditionDetails.Add(querycontionDetail);
                }

                //流水号
                if (chkEnableReference.IsChecked.Equals(true))
                {
                    if (chkRecordReference.IsChecked.Equals(true) && txtRecordReference.Text.Trim(' ').Length > 0)
                    {
                        Match m = Regex.Match(txtRecordReference.Text.Trim(' ').ToString(), pattern);
                        if (m.Success)
                        {
                            querycontionDetail = new QueryConditionDetail();
                            querycontionDetail.ConditionItemID = S3104Consts.CON_RECORDREFERENCE_MULTITEXT;
                            querycontionDetail.IsEnable = true;
                            querycontionDetail.IsLike = true;
                            querycontionDetail.Value01 = txtRecordReference.Text.Trim(' ').ToString();
                            mListQueryConditionDetails.Add(querycontionDetail);
                        }
                        else
                        {
                            App.ShowExceptionMessage(App.GetLanguageInfo("3104T00125", "RecordReference Is Not Legal"));
                            return false;
                        }
                    }
                    else if (chkRecordReference.IsChecked.Equals(false) && txtRecordReference.Text.Trim(' ').Length > 0)
                    {
                        string[] values = txtRecordReference.Text.Trim(' ').ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; i++)
                        {
                            Match m = Regex.Match(values[i], pattern);
                            if (!m.Success)
                            {
                                App.ShowExceptionMessage(App.GetLanguageInfo("3104T00125", "RecordReference Is Not Legal"));
                                return false;
                            }
                        }

                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_RECORDREFERENCE_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3104Consts.CON_RECORDREFERENCE_MULTITEXT, txtRecordReference.Text.Trim(' ').ToString());
                    }
                }



                //caller
                if (chkEnableCaller.IsChecked.Equals(true))
                {
                    if (chkCaller.IsChecked.Equals(true) && txtCaller.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_CALLERID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = true;
                        querycontionDetail.Value01 = txtCaller.Text.Trim(' ').ToString();
                        mListQueryConditionDetails.Add(querycontionDetail);
                    }
                    else if (chkCaller.IsChecked.Equals(false) && txtCaller.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_CALLERID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3104Consts.CON_CALLERID_LIKETEXT, txtCaller.Text.Trim(' ').ToString());
                    }
                }

                //called
                if (chkEnableCalledID.IsChecked.Equals(true))
                {
                    if (chkCalled.IsChecked.Equals(true) && txtCalled.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_CALLEDID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = true;
                        querycontionDetail.Value01 = txtCalled.Text.Trim(' ').ToString();
                        mListQueryConditionDetails.Add(querycontionDetail);
                    }
                    else if (chkCalled.IsChecked.Equals(false) && txtCalled.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_CALLEDID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3104Consts.CON_CALLEDID_LIKETEXT, txtCalled.Text.Trim(' ').ToString());
                    }
                }

                //chanel
                if (chkEnableChanel.IsChecked.Equals(true))
                {
                    if (chkChanle.IsChecked.Equals(true) && txtChanle.Text.Trim(' ').Length > 0)
                    {
                        Match m = Regex.Match(txtChanle.Text.Trim(' ').ToString(), pattern);
                        if (m.Success)
                        {
                            querycontionDetail = new QueryConditionDetail();
                            querycontionDetail.ConditionItemID = S3104Consts.CON_CHANNELID_MULTITEXT;
                            querycontionDetail.IsEnable = true;
                            querycontionDetail.IsLike = true;
                            querycontionDetail.Value01 = txtChanle.Text.Trim(' ').ToString();
                            mListQueryConditionDetails.Add(querycontionDetail);
                        }
                        else
                        {
                            App.ShowExceptionMessage(App.GetLanguageInfo("3104T00126", "Channel Is Not Legal"));
                            return false;
                        }
                    }
                    else if (chkChanle.IsChecked.Equals(false) && txtChanle.Text.Trim(' ').Length > 0)
                    {
                        string[] values = txtChanle.Text.Trim(' ').ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; i++)
                        {
                            Match m = Regex.Match(values[i], pattern);
                            if (!m.Success)
                            {
                                App.ShowExceptionMessage(App.GetLanguageInfo("3104T00126", "Channel Is Not Legal"));
                                return false;
                            }
                        }

                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3104Consts.CON_CHANNELID_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3104Consts.CON_CHANNELID_MULTITEXT, txtChanle.Text.Trim(' ').ToString());
                    }
                }


                //cti
                //if (chkEnableCTIReference.IsChecked.Equals(true))
                //{
                //    if (chkCTIReference.IsChecked.Equals(true) && txtCTIReference.Text.Trim(' ').Length > 0)
                //    {
                //        querycontionDetail = new QueryConditionDetail();
                //        querycontionDetail.ConditionItemID = S3104Consts.CON_CTIREFERENCE_MULTITEXT;
                //        querycontionDetail.IsEnable = true;
                //        querycontionDetail.IsLike = true;
                //        querycontionDetail.Value01 = txtChanle.Text.Trim(' ').ToString();
                //        mListQueryConditionDetails.Add(querycontionDetail);
                //    }
                //    else if (chkCTIReference.IsChecked.Equals(false) && txtCTIReference.Text.Trim(' ').Length > 0)
                //    {
                //        querycontionDetail = new QueryConditionDetail();
                //        querycontionDetail.ConditionItemID = S3104Consts.CON_CTIREFERENCE_MULTITEXT;
                //        querycontionDetail.IsEnable = true;
                //        querycontionDetail.IsLike = false;
                //        mListQueryConditionDetails.Add(querycontionDetail);
                //        SetSubItem(S3104Consts.CON_CTIREFERENCE_MULTITEXT, txtChanle.Text.Trim(' ').ToString());
                //    }
                //}


                //call direction
                if (rabDirectionIn.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_DIRECTION;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                else if (rabDirectionOut.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_DIRECTION;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "0";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }


                //查申诉
                if (chkAppeal.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_HasAppeal;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }


                //查是否评分
                if (chkScore.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_HasScore;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }

                //查是否有书签
                if (chkBookMark.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3104Consts.CON_BookMark;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                
                #region ABCD
                //ABCD查询项--服务态度
                if (chkServiceAttitude.IsChecked.Equals(true))
                {
                    if (cbSvAttitude.SelectedIndex < 0)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = Convert.ToInt64(S3104Consts.WDE_ServiceAttitude);
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    if (rbSAGood.IsChecked.Equals(true))//1 好
                    {
                        querycontionDetail.Value01 = "1";
                    }
                    else if (rbSABad.IsChecked.Equals(true))//2 坏
                    {
                        querycontionDetail.Value01 = "2";
                    }
                    else if (rbSAAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        querycontionDetail.Value01 = "0";
                    }
                    if (string.IsNullOrWhiteSpace(querycontionDetail.Value01))//如果没有选择哪一项，默认勾选全部
                    {
                        rbSAAll.IsChecked = true;
                        querycontionDetail.Value01 = "0";
                    }
                    querycontionDetail.Value02 = ListSVABCDItem[cbSvAttitude.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = string.Format("SV");
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--专业水平
                if (chkProfessionalLevel.IsChecked.Equals(true))
                {
                    if (cbProLevel.SelectedIndex < 0)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = Convert.ToInt64(S3104Consts.WDE_ProfessionalLevel);
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    if (rbPLGood.IsChecked.Equals(true))//1 好
                    {
                        querycontionDetail.Value01 = "1";
                    }
                    else if (rbPLBad.IsChecked.Equals(true))//2 坏
                    {
                        querycontionDetail.Value01 = "2";
                    }
                    else if (rbPLAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        querycontionDetail.Value01 = "0";
                    }
                    if (string.IsNullOrWhiteSpace(querycontionDetail.Value01))//如果没有选择哪一项，默认勾选全部
                    {
                        rbPLAll.IsChecked = true;
                        querycontionDetail.Value01 = "0";
                    }
                    querycontionDetail.Value02 = ListProLABCDItem[cbProLevel.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = string.Format("ProL");
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--录音时长异常
                if (chkRecordDurationError.IsChecked.Equals(true))
                {
                    if (cbRDError.SelectedIndex < 0)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = Convert.ToInt64(S3104Consts.WDE_RecordDurationError);
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    if (rbRDGood.IsChecked.Equals(true))//1 正常
                    {
                        querycontionDetail.Value01 = "1";
                    }
                    else if (rbRDBad.IsChecked.Equals(true))//2 异常
                    {
                        querycontionDetail.Value01 = "2";
                    }
                    else if (rbRDAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        querycontionDetail.Value01 = "0";
                    }
                    if (string.IsNullOrWhiteSpace(querycontionDetail.Value01))//如果没有选择哪一项，默认勾选全部
                    {
                        rbRDAll.IsChecked = true;
                        querycontionDetail.Value01 = "0";
                    }
                    querycontionDetail.Value02 = ListRDEABCDItem[cbRDError.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = string.Format("RoDE");

                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--重复呼入
                if (chkRepeatCallIn.IsChecked.Equals(true))
                {
                    if (cbRepeatCallIn.SelectedIndex < 0)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = Convert.ToInt64(S3104Consts.WDE_RepeatCallIn);
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    if (rbRCNo.IsChecked.Equals(true))//1 非重复呼入
                    {
                        querycontionDetail.Value01 = "1";
                    }
                    else if (rbRCYes.IsChecked.Equals(true))//2 重复呼入
                    {
                        querycontionDetail.Value01 = "2";
                    }
                    else if (rbRCAll.IsChecked.Equals(true))//0 全部(1,2)
                    {
                        querycontionDetail.Value01 = "0";
                    }
                    if (string.IsNullOrWhiteSpace(querycontionDetail.Value01))//如果没有选择哪一项，默认勾选全部
                    {
                        rbRCAll.IsChecked = true;
                        querycontionDetail.Value01 = "0";
                    }
                    querycontionDetail.Value02 = ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = string.Format("ReCI");

                    mListQueryConditionDetails.Add(querycontionDetail);
                }



                #endregion

                StringBuilder sb = new StringBuilder();
                //可能没拿到值,座席名
                foreach (CtrolAgent c in App.ListCtrolAgentInfos)
                {
                    sb.Append(c.AgentName).Append(",");
                }

                querycontionDetail = new QueryConditionDetail();
                querycontionDetail.ConditionItemID = S3104Consts.CON_AGENT_MULTITEXT;
                querycontionDetail.IsEnable = true;
                querycontionDetail.IsLike = false;
                SetSubItem(S3104Consts.CON_AGENT_MULTITEXT, sb.ToString());
                mListQueryConditionDetails.Add(querycontionDetail);


                foreach (QueryConditionDetail detail in mListQueryConditionDetails)
                {
                    if (detail.IsEnable == true && detail.IsLike == false)
                    {
                        List<QueryConditionSubItem> listSubItems = mListSubItems.Where(p => p.ConditionItemID == detail.ConditionItemID).ToList();
                        if (listSubItems.Count > 0)
                        {
                            string Value01 = SaveMultiValues(listSubItems);
                            detail.Value01 = Value01;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return false;
            }
            return flag;
        }
        /// <summary>
        /// 判斷時間（年限）有沒有超過
        /// by Ice
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            try
            {
                DateTime dtValue;
                if (!DateTime.TryParse(DateStart.Text.ToString(), out dtValue)
                    || !DateTime.TryParse(DateStop.Text.ToString(), out dtValue)
                    || DateTime.Parse(DateStart.Text.ToString()) >= DateTime.Parse("2100-12-31 23:59:59")
                    || DateTime.Parse(DateStop.Text.ToString()) >= DateTime.Parse("2100-12-31 23:59:59")
                    || DateTime.Parse(DateStart.Text.ToString()) <= DateTime.Parse("1999-01-01 00:00:00")
                    || DateTime.Parse(DateStop.Text.ToString()) <= DateTime.Parse("1999-01-01 00:00:00")
                    || DateTime.Parse(DateStart.Text.ToString()) > DateTime.Parse(DateStop.Text.ToString())
                    )
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00124", "DateTime Is Wrong"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return false;
            }
            return true;
        }

        private string SaveMultiValues(List<QueryConditionSubItem> listSubItems)
        {
            string selectID = string.Empty;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.InsertTempData;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(listSubItems.Count.ToString());
                for (int i = 0; i < listSubItems.Count; i++)
                {
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}"
                        , listSubItems[i].Value01
                        , ConstValue.SPLITER_CHAR
                        , listSubItems[i].Value02
                        , listSubItems[i].Value03
                        , listSubItems[i].Value04
                        , listSubItems[i].Value05);
                    webRequest.ListData.Add(strInfo);
                }
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);


                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                selectID = webReturn.Data;
                return selectID;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            return string.Empty;
        }

        void SetSubItem(long conditionItemID, string strContent)
        {
            try
            {
                List<string> listValue01 = new List<string>();
                string[] arrContent = strContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrContent.Length; i++)
                {
                    listValue01.Add(arrContent[i]);
                }
                for (int i = 0; i < listValue01.Count; i++)
                {
                    QueryConditionSubItem subItem = mListSubItems.FirstOrDefault(s => s.Value01 == listValue01[i] && s.ConditionItemID == conditionItemID);
                    if (subItem == null)
                    {
                        //增加
                        subItem = new QueryConditionSubItem();
                        subItem.ConditionItemID = conditionItemID;
                        subItem.Value01 = listValue01[i];
                        subItem.Number = i;
                        mListSubItems.Add(subItem);
                    }
                }
            }
            catch (Exception)
            {

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
    }
}
