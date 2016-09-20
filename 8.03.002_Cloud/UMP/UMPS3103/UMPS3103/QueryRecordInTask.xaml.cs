using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using UMPS3103.Models;
using UMPS3103.Wcf31031;
using UMPS3103.Wcf11012;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Common;
using VoiceCyber.Wpf.CustomControls;
using System.Text;
using System.Text.RegularExpressions;
using VoiceCyber.Common;
using System.Windows.Input;
using System.Threading;

namespace UMPS3103
{

    /// <summary>
    /// QueryRecordInTask.xaml 的交互逻辑
    /// </summary>
    public partial class QueryRecordInTask 
    {

        #region Members
        public TaskAssign  PageParent;
        private ObservableCollection<UnitTime> mListUnitTime;
        public ObjectItemTask mRootItem;

        private List<QueryConditionDetail> mListQueryConditionDetails;
        private List<QueryConditionSubItem> mListSubItems;
        private List<CtrolAgent> mListCtrolAgent;
        private List<DateTimeSpliteAsDay> mListDateTime;
        //座席每天
        private bool isAgentday;

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

        public QueryRecordInTask()
        {
            InitializeComponent();
            isAgentday = false;
            mListUnitTime = new ObservableCollection<UnitTime>();
            //mRootItem = new ObjectItemTask();
            mListQueryConditionDetails = new List<QueryConditionDetail>();
            mListSubItems = new List<QueryConditionSubItem>();
            mListCtrolAgent = new List<CtrolAgent>();
            mListDateTime = new List<DateTimeSpliteAsDay>();
            ListSVABCDItem = new List<ABCD_OrgSkillGroup>();
            ListProLABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRDEABCDItem = new List<ABCD_OrgSkillGroup>();
            ListRCIABCDItem = new List<ABCD_OrgSkillGroup>();

            txtNumTime.Value = 1;
            intAgentNum.Value = 3;//默认每个座席3条
            intExtensionNum.Value = 3;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            rdbTime1.Checked += rdbTime1_Checked;
            rdbTime2.Checked += rdbTime2_Checked;
            chkIsStart.Click += chkIsStart_Checked;
            radEveryAgent.Checked += radEveryAgent_Checked;
            radEveryExtension.Checked += radEveryExtension_Checked;
            combUintTimeTask.SelectionChanged += combUintTimeTask_SelectionChanged;
            Loaded += QueryRecordInTask_Loaded;
            TvObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
        }
        
        void QueryRecordInTask_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangeLanguage();
                InitCombUintTime();
                InitCtrol();

                gpbEvery.IsEnabled = false;


                //InitOrgAndAgentAndExtension(mRootItem, App.CurrentOrg);
                TvObjects.ItemsSource = mRootItem.Children;


                //ABCD T_31_050中初始化统计ID
                List<string> temp = new List<string>();
                temp.Add("3110000000000000001");//服务态度
                temp.Add("3110000000000000002");//专业水平 
                temp.Add("3110000000000000006");//录音时长异常
                temp.Add("3110000000000000003");//重复呼入
                InitABCDItem(temp);

            }
            catch (Exception)
            {
            }
        }

        #region 初始化
        /// Author           : YuQiangBo
        ///  Created          : 2015-01-29 15:19:32
        /// <summary>
        /// 加载语言包
        /// </summary>
        private void ChangeLanguage()
        {
            tabRecordInformation.Header = CurrentApp.GetLanguageInfo("3103T00071", "Record Information");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
            chkIsStart.Content = CurrentApp.GetLanguageInfo("3103T00092", "Start");
            radEveryAgent.Content = CurrentApp.GetLanguageInfo("3103T00093", "Every Agent");
            chkEveryDayAgent.Content = chkEveryDayExtension.Content = CurrentApp.GetLanguageInfo("3103T00094", "Every Day");
            radEveryExtension.Content = CurrentApp.GetLanguageInfo("3103T00095", "Every Extension");

            rdbTime1.Content = CurrentApp.GetLanguageInfo("3103T00086", "Record Start And Stop Time");
            rdbTime2.Content = CurrentApp.GetLanguageInfo("3103T00087", "Recent Time");
            chkEnableLengthTime.Content = CurrentApp.GetLanguageInfo("3103T00066", "Record Length");
            chkEnableReference.Content = CurrentApp.GetLanguageInfo("3103T00025", "Record Reference");
            chkEnableCaller.Content = CurrentApp.GetLanguageInfo("3103T00068", "Caller Number");
            chkEnableCalledID.Content = CurrentApp.GetLanguageInfo("3103T00069", "Called Number");
            chkEnableChanel.Content = CurrentApp.GetLanguageInfo("3103T00062", "Record Chanel");
            chkEnableCTIReference.Content = CurrentApp.GetLanguageInfo("3103T00088", "CTI Reference");
            labCallDirection.Content = CurrentApp.GetLanguageInfo("3103T00067", "Call Direction");
            rabDirectionIn.Content = CurrentApp.GetLanguageInfo("3103T00089", "Call In");
            rabDirectionOut.Content = CurrentApp.GetLanguageInfo("3103T00090", "Call Out");
            chkRecordReference.Content =
                chkCaller.Content =
                    chkCalled.Content =
                        chkChanle.Content = chkCTIReference.Content = CurrentApp.GetLanguageInfo("3103T00091", "Obscure Query");
            labEveryAgent.Content = labEveryExtension.Content = CurrentApp.GetLanguageInfo("3103T00111", "Item");

            tbAgentAndExtension.Header = CurrentApp.GetLanguageInfo("3103T00105", "AgentAndExtension");
            //ExpanderAgentAndExtensionSelect.Header = App.GetLanguageInfo("3103T00106", "Agent And Extension Select");
            labAgent.Content = CurrentApp.GetLanguageInfo("3103T00065", "Agent");//chkSelectAgent.Content = 
            labExtension.Content = CurrentApp.GetLanguageInfo("3103T00064", "Extension");//chkSelectExtension.Content = 

            labScore.Content = CurrentApp.GetLanguageInfo("3103T00112", "Score");
            rabHaveScore.Content = CurrentApp.GetLanguageInfo("3103T00113", "Scored");
            rabNotScore.Content = CurrentApp.GetLanguageInfo("3103T00114", "Not Scored");

            rabDirectionAll.Content = CurrentApp.GetLanguageInfo("3103T00131", "All");
            rabScoreAll.Content = CurrentApp.GetLanguageInfo("3103T00131", "All");

            chkServiceAttitude.Content = CurrentApp.GetLanguageInfo("3103T00174", "ServiceAttitude");
            chkProfessionalLevel.Content = CurrentApp.GetLanguageInfo("3103T00175", "ProfessionalLevel");
            chkRecordDurationError.Content = CurrentApp.GetLanguageInfo("3103T00176", "RecordDurationError");
            rbSAAll.Content = rbPLAll.Content = rbRDAll.Content = rbRCAll.Content = CurrentApp.GetLanguageInfo("3103T00131", "All");
            rbSAGood.Content = rbPLGood.Content = CurrentApp.GetLanguageInfo("3103T00169", "Good");
            rbSABad.Content = rbPLBad.Content = CurrentApp.GetLanguageInfo("3103T00170", "Bad");
            rbRDBad.Content = CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
            rbRDGood.Content = CurrentApp.GetLanguageInfo("3103T00172", "Normal");
            chkRepeatCallIn.Content = CurrentApp.GetLanguageInfo("3103T00180", "Repeat Call In");
            rbRCNo.Content = CurrentApp.GetLanguageInfo("3103T00172", "Normal");
            rbRCYes.Content = CurrentApp.GetLanguageInfo("3103T00181", "Repetition");
        }

        void InitCombUintTime()
        {
            mListUnitTime.Clear();
            UnitTime unittime = new UnitTime();
            unittime.ID = 1;
            unittime.Code = CurrentApp.GetLanguageInfo("3103T00118", "Day");
            mListUnitTime.Add(unittime);
            unittime = new UnitTime();
            unittime.ID = 2;
            unittime.Code = CurrentApp.GetLanguageInfo("3103T00119", "Week");
            mListUnitTime.Add(unittime);

            unittime = new UnitTime();
            unittime.ID = 3;
            unittime.Code = CurrentApp.GetLanguageInfo("3103T00120", "Month");
            mListUnitTime.Add(unittime);

            combUintTimeTask.ItemsSource = mListUnitTime;
            combUintTimeTask.DisplayMemberPath = "Code";
            combUintTimeTask.SelectedValuePath = "ID";
            combUintTimeTask.SelectedIndex = 0;
        }

        void InitCtrol() 
        {
            rdbTime1.IsChecked = true;
            txtNumTime.IsEnabled = false;
            combUintTimeTask.IsEnabled = false;

            DateStart.Value= DateTime.Now.Date;
            DateStop.Value = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);

            mtbStart.Text = "00:00:00";
            mtbStop.Text = "00:30:00";
            chkIsStart.IsChecked = false;
            chkEveryDayAgent.IsEnabled = false;
            intAgentNum.IsEnabled = false;
            labEveryAgent.IsEnabled = false;


            chkEveryDayExtension.IsEnabled = false;
            intExtensionNum.IsEnabled = false;
            labEveryExtension.IsEnabled = false;

        }
        #endregion

        #region 触发事件
        private void chkIsStart_Checked(object sender, RoutedEventArgs e) 
        {
            if (chkIsStart.IsChecked.Equals(true))
            {
                gpbEvery.IsEnabled = true;
                if (radEveryExtension.IsChecked.Equals(true))
                {
                    chkEveryDayAgent.IsEnabled = false;
                    intAgentNum.IsEnabled = false;
                    labEveryAgent.IsEnabled = false;


                    chkEveryDayExtension.IsEnabled = true;
                    intExtensionNum.IsEnabled = true;
                    labEveryExtension.IsEnabled = true;
                }
                else if (radEveryAgent.IsChecked.Equals(true))
                {
                    chkEveryDayAgent.IsEnabled = true;
                    intAgentNum.IsEnabled = true;
                    labEveryAgent.IsEnabled = true;


                    chkEveryDayExtension.IsEnabled = false;
                    intExtensionNum.IsEnabled = false;
                    labEveryExtension.IsEnabled = false;
                }
            }
            else 
            {
                gpbEvery.IsEnabled = false;

                chkEveryDayAgent.IsEnabled = false;
                intAgentNum.IsEnabled = false;
                labEveryAgent.IsEnabled = false;


                chkEveryDayExtension.IsEnabled = false;
                intExtensionNum.IsEnabled = false;
                labEveryExtension.IsEnabled = false;
            }
        }
        private void radEveryExtension_Checked(object sender, RoutedEventArgs e)
        {
            if (radEveryExtension.IsChecked.Equals(true) && chkIsStart.IsChecked.Equals(true))
            {
                chkEveryDayAgent.IsEnabled = false;
                intAgentNum.IsEnabled = false;
                labEveryAgent.IsEnabled = false;


                chkEveryDayExtension.IsEnabled = true;
                intExtensionNum.IsEnabled = true;
                labEveryExtension.IsEnabled = true;
            }
        }

        private void radEveryAgent_Checked(object sender,RoutedEventArgs e)
        {
            if (radEveryAgent.IsChecked.Equals(true) && chkIsStart.IsChecked.Equals(true))
            {
                chkEveryDayAgent.IsEnabled = true;
                intAgentNum.IsEnabled = true;
                labEveryAgent.IsEnabled = true;


                chkEveryDayExtension.IsEnabled = false;
                intExtensionNum.IsEnabled = false;
                labEveryExtension.IsEnabled = false;
            }
        }

        private void combUintTimeTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime now = DateTime.Now.Date;
            int  Num = int.Parse( txtNumTime.Value.ToString());
            Num = Num < 1 ? 1 : Num;
            int Type = Int16.Parse(combUintTimeTask.SelectedValue.ToString());
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
        
        private void rdbTime1_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbTime1.IsChecked.Equals(true)) 
            {
                txtNumTime.IsEnabled = false;
                combUintTimeTask.IsEnabled = false;

                DateStart.IsEnabled = true;
                DateStop.IsEnabled = true;
            }
        }
        
        private void rdbTime2_Checked(object sender, RoutedEventArgs e)
        {
            if(rdbTime2.IsChecked.Equals(true))
            {
                DateStart.IsEnabled = false;
                DateStop.IsEnabled = false;

                txtNumTime.IsEnabled = true;
                combUintTimeTask.IsEnabled = true;
            }
        }

        private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(500);//次线程休眠1秒
                Dispatcher.Invoke(new Action(() =>
                {
                    List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                    GetAgentIsCheck(mRootItem, ref lstCtrolAgentTemp,ConstValue.RESOURCE_AGENT);
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
                    else { txtAgent.Text = ""; }

                    List<CtrolAgent> listCtrolRexTemp = new List<CtrolAgent>();
                    GetAgentIsCheck(mRootItem, ref listCtrolRexTemp, ConstValue.RESOURCE_REALEXT);
                    GetAgentIsCheck(mRootItem, ref listCtrolRexTemp, ConstValue.RESOURCE_EXTENSION);
                    if (listCtrolRexTemp.Count > 0)
                    {
                        string stra = "";
                        foreach (CtrolAgent ca in listCtrolRexTemp)
                        {
                            stra += ca.AgentName + ",";
                        }
                        stra = stra.TrimEnd(',');
                        txtExtension.Text = stra;
                    }
                    else { txtExtension.Text = ""; }
                }));
            });
            t.Start();
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {

            //CheckBox selectcheck = e.Source as CheckBox;
            //ObjectItem oooo = selectcheck.Tag as ObjectItem;
        }
        
        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string temp = CurrentApp.GetLanguageInfo("FO3103007", "Record Query");
            if (string.IsNullOrWhiteSpace(DateStart.Value.ToString()) || string.IsNullOrWhiteSpace(DateStop.Value.ToString()))
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "DateTime Is Null."));
                return;
            }
            else if(DateTime.Parse(DateStart.Value.ToString()) .CompareTo(DateTime.Parse(DateStop.Value.ToString())).Equals(1))
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "DateTime Error."));
                return;
            }

            if (rdbTime2.IsChecked == true)//最近时间
            {
                DateStop.Value = DateTime.Now.Date;
                UnitTime ut = combUintTimeTask.SelectedItem as UnitTime;
                if (txtNumTime.Value == null || txtNumTime.Value < 1)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "Parameter Error."));
                    return;
                }
                switch (ut.ID)
                { 
                    case 1:
                        DateStart.Value = DateTime.Now.Date.AddDays(Convert.ToDouble(-1 * txtNumTime.Value));
                        break;
                    case 2:
                        DateStart.Value = DateTime.Now.Date.AddDays(Convert.ToDouble(-7 * txtNumTime.Value));
                        break;
                    case 3:
                        DateStart.Value = DateTime.Now.Date.AddMonths(Convert.ToInt32(-1 * txtNumTime.Value));
                        break;
                    default:
                        return;
                }
            }
            if (DateStart.Value.Value.Year < 1754)
                DateStart.Value = DateTime.Parse("1754-1-1 0:0:0");

            if (intAgentNum.IsEnabled == true && intAgentNum.Value<=0)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "Parameter Error."));
                return;
            }

            if (!CreateQueryCondition()) 
            {
                return;
            }
            if (PageParent != null) 
            {
                PageParent.QueryRecord(isAgentday,mListQueryConditionDetails, mListCtrolAgent, mListDateTime);

                var parent = Parent as PopupPanel;
                if(parent !=null)   
                {
                    parent.IsOpen = false; 
                }
            }
        }
        
        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }
        #endregion

        #region 查询逻辑

        private bool CreateQueryCondition() 
        {

            bool flag = true;
            try
            {
                mListSubItems.Clear();
                mListQueryConditionDetails.Clear();
                mListDateTime.Clear();
                mListCtrolAgent.Clear();
                //匹配纯数字的
                string pattern = @"^\d+$";

                QueryConditionDetail querycontionDetail = new QueryConditionDetail();   

                //录音时长
                if (chkEnableLengthTime.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_DURATIONFROMTO;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.Value01 = Converter.Time2Second(mtbStart.Text).ToString();
                    querycontionDetail.Value02 = Converter.Time2Second(mtbStop.Text).ToString();
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                querycontionDetail = new QueryConditionDetail();
                querycontionDetail.ConditionItemID = S3103Consts.CON_TIMEFROMTO;
                querycontionDetail.IsEnable = true;
                querycontionDetail.Value01 = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                DateTime dtnow = DateTime.Now;
                if (DateTime.Parse(DateStop.Value.ToString()) < dtnow)
                    dtnow = DateTime.Parse(DateStop.Value.ToString());
                querycontionDetail.Value02 = dtnow.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                mListQueryConditionDetails.Add(querycontionDetail);

                //流水号
                if (chkEnableReference.IsChecked.Equals(true))
                {
                    if (chkRecordReference.IsChecked.Equals(true) && txtRecordReference.Text.Trim(' ').Length > 0)
                    {
                        Match m = Regex.Match(txtRecordReference.Text.Trim(' ').ToString(), pattern);
                        if (m.Success)
                        {
                            querycontionDetail = new QueryConditionDetail();
                            querycontionDetail.ConditionItemID = S3103Consts.CON_RECORDREFERENCE_MULTITEXT;
                            querycontionDetail.IsEnable = true;
                            querycontionDetail.IsLike = true;
                            querycontionDetail.Value01 = txtRecordReference.Text.Trim(' ').ToString();
                            mListQueryConditionDetails.Add(querycontionDetail);
                        }
                        else 
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "RecordReference Is Not Legal"));
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
                                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "RecordReference Is Not Legal"));
                                return false;
                            }
                        }

                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_RECORDREFERENCE_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3103Consts.CON_RECORDREFERENCE_MULTITEXT, txtRecordReference.Text.Trim(' ').ToString());
                    }
                }
               
                //caller
                if(chkEnableCaller.IsChecked.Equals(true))
                {
                    if (chkCaller.IsChecked.Equals(true) && txtCaller.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CALLERID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = true;
                        querycontionDetail.Value01 = txtCaller.Text.Trim(' ').ToString();
                        mListQueryConditionDetails.Add(querycontionDetail);
                    }
                    else if (chkCaller.IsChecked.Equals(false) && txtCaller.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CALLERID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3103Consts.CON_CALLERID_LIKETEXT, txtCaller.Text.Trim(' ').ToString());
                    }
                }

                //called
                if (chkEnableCalledID.IsChecked.Equals(true))
                {
                    if (chkCalled.IsChecked.Equals(true) && txtCalled.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CALLEDID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = true;
                        querycontionDetail.Value01 = txtCalled.Text.Trim(' ').ToString();
                        mListQueryConditionDetails.Add(querycontionDetail);
                    }
                    else if (chkCalled.IsChecked.Equals(false) && txtCalled.Text.Trim(' ').Length > 0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CALLEDID_LIKETEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3103Consts.CON_CALLEDID_LIKETEXT, txtCalled.Text.Trim(' ').ToString());
                    }
                }

                //chanel
                if (chkEnableChanel.IsChecked.Equals(true))
                {                  
                    if(chkChanle.IsChecked.Equals(true) && txtChanle.Text.Trim(' ').Length>0)
                    {
                        Match m = Regex.Match(txtChanle.Text.Trim(' ').ToString(), pattern);
                        if (m.Success)
                        {
                            querycontionDetail = new QueryConditionDetail();
                            querycontionDetail.ConditionItemID = S3103Consts.CON_CHANNELID_MULTITEXT;
                            querycontionDetail.IsEnable = true;
                            querycontionDetail.IsLike = true;
                            querycontionDetail.Value01 = txtChanle.Text.Trim(' ').ToString();
                            mListQueryConditionDetails.Add(querycontionDetail);
                        }
                        else 
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "Channel Is Not Legal."));
                            return false;
                        }
                    }
                    else if (chkChanle.IsChecked.Equals(false) && txtChanle.Text.Trim(' ').Length>0)
                    {
                        string [] values= txtChanle.Text.Trim(' ').ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; i++)
                        {
                            Match m = Regex.Match(values[i], pattern);
                            if(!m.Success)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "Channel Is Not Legal."));
                                return false;
                            }
                        }

                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CHANNELID_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3103Consts.CON_CHANNELID_MULTITEXT, txtChanle.Text.Trim(' ').ToString());
                    }
                }


                //cti
                if (chkEnableCTIReference.IsChecked.Equals(true))
                {
                    if(chkCTIReference.IsChecked.Equals(true) && txtCTIReference.Text.Trim( ' ').Length>0)
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CTIREFERENCE_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = true;
                        querycontionDetail.Value01 = txtCTIReference.Text.Trim(' ').ToString();
                        mListQueryConditionDetails.Add(querycontionDetail);
                    }
                    else if (chkCTIReference.IsChecked.Equals(false) && txtCTIReference.Text.Trim(' ').Length > 0) 
                    {
                        querycontionDetail = new QueryConditionDetail();
                        querycontionDetail.ConditionItemID = S3103Consts.CON_CTIREFERENCE_MULTITEXT;
                        querycontionDetail.IsEnable = true;
                        querycontionDetail.IsLike = false;
                        mListQueryConditionDetails.Add(querycontionDetail);
                        SetSubItem(S3103Consts.CON_CTIREFERENCE_MULTITEXT, txtCTIReference.Text.Trim(' ').ToString());
                    }
                }
               

                //call direction
                if (rabDirectionIn.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_DIRECTION;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                else if(rabDirectionOut.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_DIRECTION;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "0";
                    mListQueryConditionDetails.Add(querycontionDetail);
                }

                //score
                if (rabHaveScore.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_SCORE;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "1";//已评分
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                else if (rabNotScore.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_SCORE;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = "0";//未评分
                    mListQueryConditionDetails.Add(querycontionDetail);
                }

                ////screen 
                //if (rabHaveScreen.IsChecked.Equals(true))
                //{
                //    querycontionDetail = new QueryConditionDetail();
                //    querycontionDetail.ConditionItemID = S3103Consts.CON_SCREEN;
                //    querycontionDetail.IsEnable = true;
                //    querycontionDetail.IsLike = false;
                //    querycontionDetail.Value01 = "1";
                //    mListQueryConditionDetails.Add(querycontionDetail);
                //}
                //else if(rabNotScreen.IsChecked.Equals(true))
                //{
                //    querycontionDetail = new QueryConditionDetail();
                //    querycontionDetail.ConditionItemID = S3103Consts.CON_SCREEN;
                //    querycontionDetail.IsEnable = true;
                //    querycontionDetail.IsLike = false;
                //    querycontionDetail.Value01 = "0";
                //    mListQueryConditionDetails.Add(querycontionDetail);
                //}

                //ABCD查询项--服务态度
                if (chkServiceAttitude.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_ServiceAttitude;
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
                    if (cbSvAttitude.SelectedIndex <0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail.Value02 = ListSVABCDItem[cbSvAttitude.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = ListSVABCDItem[cbSvAttitude.SelectedIndex].ManageAgent;
                    querycontionDetail.Value04 = string.Format("SV");
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--专业水平
                if (chkProfessionalLevel.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_ProfessionalLevel;
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
                    if (cbProLevel.SelectedIndex <0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail.Value02 = ListProLABCDItem[cbProLevel.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = ListProLABCDItem[cbProLevel.SelectedIndex].ManageAgent;
                    querycontionDetail.Value04 = string.Format("ProL");
                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--录音时长异常
                if (chkRecordDurationError.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_RecordDurationError;
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
                        rbRDAll.IsChecked=true;
                        querycontionDetail.Value01 = "0";
                    }
                    if (cbRDError.SelectedIndex<0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail.Value02 = ListRDEABCDItem[cbRDError.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = ListRDEABCDItem[cbRDError.SelectedIndex].ManageAgent;
                    querycontionDetail.Value04 = string.Format("RoDE");

                    mListQueryConditionDetails.Add(querycontionDetail);
                }
                //ABCD查询项--重复呼入
                if (chkRepeatCallIn.IsChecked.Equals(true))
                {
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_RepeatCallIn;
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
                    if (cbRepeatCallIn.SelectedIndex < 0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00171", "Select One Org"));
                        return false;
                    }
                    querycontionDetail.Value02 = ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].InColumn.ToString();
                    querycontionDetail.Value03 = ListRCIABCDItem[cbRepeatCallIn.SelectedIndex].ManageAgent;
                    querycontionDetail.Value04 = string.Format("ReCI");

                    mListQueryConditionDetails.Add(querycontionDetail);
                }



                //agent
                List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                GetAgentIsCheck(mRootItem, ref lstCtrolAgentTemp,ConstValue.RESOURCE_AGENT);
                int num = (int)intAgentNum.Value;
                if (lstCtrolAgentTemp.Count == 0)
                {
                    lstCtrolAgentTemp = S3103App.ListCtrolAgentInfos;
                }
                foreach (CtrolAgent ctrolagent in lstCtrolAgentTemp)
                {
                    ctrolagent.EveryAgentNum = num;
                }

                //真实分机 RealityExtexsion
                List<CtrolAgent> lstCtrolRexTemp = new List<CtrolAgent>();
                GetAgentIsCheck(mRootItem, ref lstCtrolRexTemp, ConstValue.RESOURCE_REALEXT);
                if (lstCtrolRexTemp.Count == 0)
                {
                    lstCtrolRexTemp = S3103App.ListCtrolRealityExtension;
                }
                foreach (CtrolAgent ctrolRex in lstCtrolRexTemp)
                {
                    ctrolRex.EveryAgentNum = num;
                }

                //虚拟分机 Extexsion
                List<CtrolAgent> lstCtrolExTemp = new List<CtrolAgent>();
                GetAgentIsCheck(mRootItem, ref lstCtrolExTemp, ConstValue.RESOURCE_EXTENSION);
                if (lstCtrolExTemp.Count == 0)
                {
                    lstCtrolExTemp = S3103App.ListCtrolExtension;
                }
                foreach (CtrolAgent ctrolRex in lstCtrolExTemp)
                {
                    ctrolRex.EveryAgentNum = num;
                }
                if (S3103App.GroupingWay.Contains("A"))
                {
                    mListCtrolAgent = lstCtrolAgentTemp;
                }
                else if (S3103App.GroupingWay.Contains("R"))
                {
                    mListCtrolAgent = lstCtrolRexTemp;
                }
                else if (S3103App.GroupingWay.Contains("E"))
                {
                    mListCtrolAgent = lstCtrolExTemp;
                }
                

                //选中座席每天
                if (chkIsStart.IsChecked.Equals(true) && radEveryAgent.IsChecked.Equals(true))
                {
                    isAgentday = true;
                    if (S3103App.GroupingWay.Contains("A"))
                    {
                        mListCtrolAgent = lstCtrolAgentTemp;
                    }
                    else if (S3103App.GroupingWay.Contains("R"))
                    {
                        mListCtrolAgent = lstCtrolRexTemp;
                    }
                    else if (S3103App.GroupingWay.Contains("E"))
                    {
                        mListCtrolAgent = lstCtrolExTemp;
                    }
                    //2016-06-03 14:46:27 修改  现在改成按已选座席/分机来查询每天几条录音
                    //if (S3103App.GroupingWay.Contains("A"))
                    //{
                    //    mListCtrolAgent = S3103App.ListCtrolAgentInfos;
                    //}
                    //else if (S3103App.GroupingWay.Contains("R"))
                    //{
                    //    mListCtrolAgent = S3103App.ListCtrolRealityExtension;
                    //}
                    //else if (S3103App.GroupingWay.Contains("E"))
                    //{
                    //    mListCtrolAgent = S3103App.ListCtrolExtension;
                    //}
                    foreach (CtrolAgent ctroltemp in mListCtrolAgent)
                    {
                        ctroltemp.EveryAgentNum = num;
                    }
                    if (chkEveryDayAgent.IsChecked.Equals(true))
                    {
                        GetSpliteDay(DateTime.Parse(DateStart.Value.ToString()), DateTime.Parse(DateStop.Value.ToString()), ref mListDateTime);
                    }
                    else 
                    {
                        DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                        dateTimeSpliteAsDay.StartDayTime = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime();
                        dateTimeSpliteAsDay.StopDayTime = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime();
                        mListDateTime.Add(dateTimeSpliteAsDay);
                    }
                }
                else if (chkIsStart.IsChecked.Equals(true) && radEveryExtension.IsChecked.Equals(true))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00126", "Not Support Extension."));
                    return false;

                }
                else  //没启用座席每天 ，将全部的座席存在901表
                {
                    StringBuilder sb = new StringBuilder();
                    //可能没拿到值
                    foreach (CtrolAgent c in S3103App.ListCtrolAgentInfos) 
                    {
                        sb.Append(c.AgentName).Append(",");
                    }

                    if (string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00125", "Not Support Extension."));
                        return false;
                    }
                    querycontionDetail = new QueryConditionDetail();
                    querycontionDetail.ConditionItemID = S3103Consts.CON_AGENT_MULTITEXT;
                    querycontionDetail.IsEnable = true;
                    querycontionDetail.IsLike = false;
                    querycontionDetail.Value01 = SaveMultiAgent(mListCtrolAgent);                     

                    //SetSubItem(S3103Consts.CON_AGENT_MULTITEXT, sb.ToString());
                    mListQueryConditionDetails.Add(querycontionDetail);

                    DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                    dateTimeSpliteAsDay.StartDayTime = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime();
                    dateTimeSpliteAsDay.StopDayTime = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime();
                    mListDateTime.Add(dateTimeSpliteAsDay);
                }
                //遍历多值设置存入临时表
                foreach (QueryConditionDetail detail in mListQueryConditionDetails)
                {
                    if (detail.IsEnable == true && detail.IsLike == false)
                    {
                        List<QueryConditionSubItem> listSubItems = mListSubItems.Where(p => p.ConditionItemID == detail.ConditionItemID).ToList();
                        if (listSubItems.Count > 0)
                        {
                            string Value01 = SaveMultiValues(listSubItems);
                            if (string.IsNullOrWhiteSpace(Value01))
                            {
                                return false;
                            }
                            detail.Value01 = Value01;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return false;
            }
            return flag;
        }

        void SetSubItem(long conditionItemID,string strContent) 
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

        /// <summary>
        /// 获取被管理的座席或者分机---根据传入的资源码来判定
        /// </summary>
        private void GetAgentIsCheck(ObjectItemTask parent, ref List<CtrolAgent> lstCtrolAgent,int ARType) 
        {
            foreach (ObjectItemTask o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ARType)
                {
                    CtrolAgent ctrolAgent = new CtrolAgent();
                    ctrolAgent.AgentID = o.ObjID.ToString();
                    ctrolAgent.AgentName = o.Name;
                    ctrolAgent.AgentFullName = o.Description;
                    lstCtrolAgent.Add(ctrolAgent);
                }
                if( o.ObjType== ConstValue.RESOURCE_ORG && o.Children.Count>0)
                {
                    GetAgentIsCheck(o, ref lstCtrolAgent,ARType);
                }
            }
        }


        //多值插入临时表
        private string SaveMultiAgent(List<CtrolAgent> listAgents)
        {
            string selectID= string.Empty;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.InsertTempData;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(listAgents.Count.ToString());
                for (int i = 0; i < listAgents.Count; i++)
                {
                    webRequest.ListData.Add(listAgents[i].AgentName);
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

        //流水号、呼叫号码等存入临时表
        private string SaveMultiValues(List<QueryConditionSubItem> listSubItems)
        {
            string selectID = string.Empty;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.InsertTempData;
                //(int)RequestCode.WSInsertTempData;
                webRequest.Session = CurrentApp.Session;
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
                //Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                //    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                //WebReturn webReturn = client.DoOperation(webRequest);


                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
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

        public static void GetSpliteDay(DateTime start, DateTime stop, ref List<DateTimeSpliteAsDay> lstDateTimeSpliteAsDay)
        {
            if (start.Date.Equals(stop.Date))
            {
                DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                dateTimeSpliteAsDay.StartDayTime = start.ToUniversalTime();
                dateTimeSpliteAsDay.StopDayTime = stop.ToUniversalTime();
                lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                return;
            }
            else
            {
                DateTime timeTempStart = start.Date.AddDays(1);
                if (timeTempStart.CompareTo(stop).Equals(-1))
                {
                    DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                    dateTimeSpliteAsDay.StartDayTime = start.ToUniversalTime();
                    dateTimeSpliteAsDay.StopDayTime = timeTempStart.ToUniversalTime();
                    lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                    while (timeTempStart < stop && timeTempStart.AddDays(1) < stop)
                    {
                        dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                        dateTimeSpliteAsDay.StartDayTime = timeTempStart.ToUniversalTime();
                        dateTimeSpliteAsDay.StopDayTime = timeTempStart.AddDays(1).ToUniversalTime();
                        lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                        timeTempStart = timeTempStart.AddDays(1);
                    }

                    dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                    dateTimeSpliteAsDay.StartDayTime = timeTempStart.ToUniversalTime();
                    dateTimeSpliteAsDay.StopDayTime = stop.ToUniversalTime();
                    if (lstDateTimeSpliteAsDay.Where(p => p.StartDayTime == dateTimeSpliteAsDay.StartDayTime && p.StopDayTime == dateTimeSpliteAsDay.StopDayTime).Count() == 0)
                    {
                        lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                    }
                }
                else if (timeTempStart.CompareTo(stop).Equals(0))
                {
                    DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                    dateTimeSpliteAsDay.StartDayTime = start.ToUniversalTime();
                    dateTimeSpliteAsDay.StopDayTime = stop.ToUniversalTime();
                    lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                }
                else
                {
                    DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                    dateTimeSpliteAsDay.StartDayTime = start.ToUniversalTime();
                    dateTimeSpliteAsDay.StopDayTime = timeTempStart.ToUniversalTime();
                    lstDateTimeSpliteAsDay.Add(dateTimeSpliteAsDay);
                }

            }
        }
        #endregion

        #region 座席分机
        //更新座席分机数据
        public void InitOrgAndAgentAndExtension(ObjectItemTask parentItem, string parentID)
        {
            List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
            lstCtrolOrgTemp = S3103App.ListCtrolOrgInfos.Where(p=>p.OrgParentID ==parentID).OrderBy(o=>o.OrgName).ToList();
            foreach(CtrolOrg org in lstCtrolOrgTemp)
            {
                ObjectItemTask item = new ObjectItemTask();
                item.ObjType = ConstValue.RESOURCE_ORG;
                item.ObjID = Convert.ToInt64(org.ID);
                item.Name = org.OrgName;
                item.Data = org;
                item.IsChecked = false;
                if (org.ID == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "/Themes/Default/UMPS3103/Images/rootorg.ico";
                }
                else
                {
                    item.Icon = "/Themes/Default/UMPS3103/Images/org.ico";
                }
                InitOrgAndAgentAndExtension(item, org.ID);
                if (S3103App.GroupingWay.Contains("A"))
                {
                    InitControlAgents(item, org.ID);
                }
                else if (S3103App.GroupingWay.Contains("R"))
                {
                    InitControlRealityExtension(item, org.ID);
                }
                else if (S3103App.GroupingWay.Contains("E"))
                {
                    InitControlExtension(item, org.ID);
                }
                AddChildObject(parentItem, item);
            }
        }

        private void AddChildObject(ObjectItemTask parentItem, ObjectItemTask item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void InitControlAgents(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                lstCtrolAgentTemp = S3103App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == parentID).OrderBy(o => o.AgentName).ToList();
                foreach(CtrolAgent agent in lstCtrolAgentTemp)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(agent.AgentID);
                    item.Name = agent.AgentName;
                    item.Description = agent.AgentFullName;
                    item.Data = agent;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3103/Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void InitControlRealityExtension(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolRexTemp = new List<CtrolAgent>();
                lstCtrolRexTemp = S3103App.ListCtrolRealityExtension.Where(p => p.AgentOrgID == parentID).OrderBy(o => o.AgentName).ToList();
                foreach (CtrolAgent rex in lstCtrolRexTemp)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(rex.AgentID);
                    item.Name = rex.AgentName;
                    item.Description = rex.AgentFullName;
                    item.Data = rex;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3103/Images/RealExtension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void InitControlExtension(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                List<CtrolAgent> lstCtrolRexTemp = new List<CtrolAgent>();
                lstCtrolRexTemp = S3103App.ListCtrolExtension.Where(p => p.AgentOrgID == parentID).OrderBy(o => o.AgentName).ToList();
                foreach (CtrolAgent cEx in lstCtrolRexTemp)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(cEx.AgentID);
                    item.Name = cEx.AgentName;
                    item.Description = cEx.AgentFullName;
                    item.Data = cEx;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3103/Images/extension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        #endregion

        #region ABCD
        //初始化ABCD查询配置
        public void InitABCDItem(List<string> OperationList)
        {
            try
            {
                if (OperationList.Count < 1) { return; }
                ComboBoxItem tempBox;
                string LogStr;
                //服务态度
                ListSVABCDItem = InitABCDQuery(OperationList[0]);
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
                ListProLABCDItem = InitABCDQuery(OperationList[1]);
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
                ListRDEABCDItem = InitABCDQuery(OperationList[2]);
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
                ListRCIABCDItem = InitABCDQuery(OperationList[3]);
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
                webRequest.Code = (int)S3103Codes.GetABCD;
                webRequest.ListData.Add(OperationID);
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
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
                    List<CtrolAgent> lstCtrolAgentTemp=new List<CtrolAgent>();
                    if (info.Isdrilling)
                    {
                        GetOrgAgents(info.OrgSkillGroupID.ToString(),ref lstCtrolAgentTemp);
                    }
                    else
                    {
                        if (S3103App.GroupingWay.Contains("A"))
                        {
                            lstCtrolAgentTemp = S3103App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("R"))
                        {
                            lstCtrolAgentTemp = S3103App.ListCtrolRealityExtension.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("E"))
                        {
                            lstCtrolAgentTemp = S3103App.ListCtrolExtension.Where(p => p.AgentOrgID == info.OrgSkillGroupID.ToString()).ToList();
                        }
                    }
                    if (lstCtrolAgentTemp.Count() < 1) { continue; }
                    foreach (CtrolAgent agent in lstCtrolAgentTemp)
                    {
                        tempAgent += string.Format("'{0}',", agent.AgentName);
                    }
                    info.ManageAgent=tempAgent.Substring(0, tempAgent.Length - 1);
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

        /// <summary>
        /// 根据是否钻取获取座席信息，向下钻取的获取该机构下属的所有座席
        /// </summary>
        private void GetOrgAgents(string orgID, ref List<CtrolAgent> lstCtrolAgent)
        {
            List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
            try
            {
                List<CtrolOrg> lstCtrolOrgTemp = S3103App.ListCtrolOrgInfos.Where(p => p.OrgParentID == orgID).ToList();
                foreach (CtrolOrg org in lstCtrolOrgTemp)
                {
                    if (S3103App.GroupingWay.Contains("A"))
                    {
                        lstCtrolAgentTemp = S3103App.ListCtrolAgentInfos.Where(p => p.AgentOrgID == org.ID).ToList();
                    }
                    else if (S3103App.GroupingWay.Contains("R"))
                    {
                        lstCtrolAgentTemp = S3103App.ListCtrolRealityExtension.Where(p => p.AgentOrgID == org.ID).ToList();
                    }
                    else if (S3103App.GroupingWay.Contains("E"))
                    {
                        lstCtrolAgentTemp = S3103App.ListCtrolExtension.Where(p => p.AgentOrgID == org.ID).ToList();
                    }
                    foreach (CtrolAgent temp in lstCtrolAgentTemp)
                    {
                        lstCtrolAgent.Add(temp);
                    }
                    GetOrgAgents(org.ID, ref lstCtrolAgent);
                }
            }
            catch (Exception ex)
            {
                return ;
            }
            return ;
        }

        #endregion
    }
}
