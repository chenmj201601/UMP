//======================================================================
//
//　╭══╮　┌════════┐
//╭╯  ©   ║═║ By  Waves   ║
//╰⊙═⊙╯   └══⊙══⊙═┘
//
//created by Waves at 2015-6-12 16:03:18
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS3105.Models;
using UMPS3105.Wcf31021;
using UMPS3105.Wcf31031;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;
using CtrolAgent = UMPS3105.Models.CtrolAgent;
using DateTimeSpliteAsDay = UMPS3105.Models.DateTimeSpliteAsDay;

namespace UMPS3105
{
    /// <summary>
    /// QueryAppealResult.xaml 的交互逻辑  審批Approval  複核review
    /// </summary>
    public partial class QueryAppealResult
    {
        public AppealManageMainView ParentPage;

        private CtrolAgent mListAgents;
        private ObservableCollection<UnitTime> mListUnitTime;
        private List<DateTimeSpliteAsDay> mListDateTime;
        private BackgroundWorker mWorker;

        public string AgentsStr;

        public QueryAppealResult()
        {
            InitializeComponent();

            mListUnitTime = new ObservableCollection<UnitTime>();
            mListDateTime = new List<DateTimeSpliteAsDay>();
            AgentsStr = string.Empty;

            BtnClose.Click += BtnClose_Click;
            BtnConfirm.Click += Btnconfirm_Click;
            TagenObjects.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
            this.Loaded += QueryAppealResult_Loaded;
        }

        void QueryAppealResult_Loaded(object sender, RoutedEventArgs e)
        {
            mListAgents = new CtrolAgent();
            ChangeLanguage();
            InitControlTime();
            InitControlObjects();
            InitChkClick();
            TagenObjects.ItemsSource = mListAgents.Children;
        }

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                rdbTime1.Content = CurrentApp.GetLanguageInfo("3105T00060", "Appealed Time");
                rdbTime2.Content = CurrentApp.GetLanguageInfo("3105T00061", "Recent Time");
                chkEnableReference.Content = CurrentApp.GetLanguageInfo("3105T00058", "ReferenceID");
                chkRecordReference.Content = CurrentApp.GetLanguageInfo("3105T00062", "Fuzzy Search");
                chkall.Content = CurrentApp.GetLanguageInfo("3105T00063", "All Select");
                chkNrecheck.Content = CurrentApp.GetLanguageInfo("3105T00064", "To review");
                chkNexamine.Content = CurrentApp.GetLanguageInfo("3105T00065", "To Approval");
                chkReexamineed.Content = CurrentApp.GetLanguageInfo("3105T00066", "Approvaled");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3105T00012", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3105T00013", "Close");
            }
            catch (Exception ex)
            {

            }
        }

        private void Btnconfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DateStart.Value.ToString()) || string.IsNullOrWhiteSpace(DateStop.Value.ToString()))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00070", "DateTime Is Wrong"));
                    return;
                }
                else if (DateTime.Parse(DateStart.Value.ToString()).CompareTo(DateTime.Parse(DateStop.Value.ToString())).Equals(1))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00070", "DateTime Is Wrong"));
                    return;
                }
                string strQueryCondition = string.Empty;
                strQueryCondition = CreateQueryCondition();
                if (string.IsNullOrWhiteSpace(strQueryCondition))
                {
                    return;
                }
                if (ParentPage != null)
                {
                    ParentPage.QueryAppealInfo(strQueryCondition, mListDateTime);
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


        #region 座席樹

        private void InitControlObjects()
        {
            try
            {
                ParentPage.SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    InitControlOrgs(mListAgents, "-1");
                    CurrentApp.WriteLog("PageLoad", string.Format("Load ControlObject"));
                };
                //当后台操作已完成、被取消或引发异常时发生
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    ParentPage.SetBusy(false, string.Empty);
                };
                mWorker.RunWorkerAsync();//触发DoWork事件
            }
            catch (Exception)
            {

            }
        }
        private void InitControlOrgs(CtrolAgent parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = 10;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[1];
                    string strName = arrInfo[0];
                    CtrolAgent item = new CtrolAgent();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.IsChecked = true;
                    item.IsExpanded = true;
                    item.Data = strInfo;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    InitControlOrgs(item, strID);
                    InitControlAgents(item, strID);
                    AddChildObject(parentItem, item);
                }
                List<CtrolAgent> lstCtrolOrgTemp = new List<CtrolAgent>();
                GetOrgIsCheck(mListAgents, ref lstCtrolOrgTemp);
                if (lstCtrolOrgTemp.Count > 0)
                {
                    AgentsStr = SaveMultiAgent(lstCtrolOrgTemp);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlAgents(CtrolAgent parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = 11;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    CtrolAgent item = new CtrolAgent();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.IsChecked = true;
                    item.IsExpanded = true;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    if (item.Name != "N/A")
                    {
                        AddChildObject(parentItem, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddChildObject(CtrolAgent parentItem, CtrolAgent item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }


        private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Thread t = new Thread(() =>
                {
                    Thread.Sleep(500);//次线程休眠1秒
                    Dispatcher.Invoke(new Action(() =>
                    {
                        List<CtrolAgent> lstCtrolOrgTemp = new List<CtrolAgent>();
                        GetOrgIsCheck(mListAgents, ref lstCtrolOrgTemp);
                        if (lstCtrolOrgTemp.Count > 0)
                        {
                            AgentsStr = SaveMultiAgent(lstCtrolOrgTemp);
                        }
                    }));
                });
                t.Start();
            }
            catch (Exception ex)
            {
                ShowException("获取机构信息失败");
            }
        }


        private void GetOrgIsCheck(CtrolAgent parent, ref List<CtrolAgent> lstCtrolAgent)
        {
            foreach (CtrolAgent o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ConstValue.RESOURCE_AGENT)
                {
                    CtrolAgent ctrolOrg = new CtrolAgent();
                    ctrolOrg.ObjID = o.ObjID;
                    ctrolOrg.Name = o.Name;
                    lstCtrolAgent.Add(ctrolOrg);
                }
                if (o.ObjType != 0 && o.Children.Count > 0)
                {
                    GetOrgIsCheck(o, ref lstCtrolAgent);
                }
            }
        }

        #endregion

        #region 時間控制

        private void InitControlTime()
        {
            InitCombUintTime();
            combUintTime.ItemsSource = mListUnitTime;
            combUintTime.DisplayMemberPath = "Code";
            combUintTime.SelectedValuePath = "ID";

            txtNumTime.Value = 0;
            rdbTime1.IsChecked = true;
            txtNumTime.IsEnabled = false;
            combUintTime.IsEnabled = false;

            DateStart.Value = DateTime.Now.Date;
            DateStop.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            rdbTime1.Checked += rdbTime1_Checked;
            rdbTime2.Checked += rdbTime2_Checked;
            combUintTime.SelectionChanged += combUintTime_SelectionChanged;
        }

        void InitCombUintTime()
        {
            mListUnitTime.Clear();
            UnitTime unittime = new UnitTime();
            unittime.ID = 1;
            unittime.Code = CurrentApp.GetLanguageInfo("3105T00067", "Day");
            mListUnitTime.Add(unittime);
            unittime = new UnitTime();
            unittime.ID = 2;
            unittime.Code = CurrentApp.GetLanguageInfo("3105T00068", "Week");
            mListUnitTime.Add(unittime);

            unittime = new UnitTime();
            unittime.ID = 3;
            unittime.Code = CurrentApp.GetLanguageInfo("3105T00069", "Month");
            mListUnitTime.Add(unittime);
        }

        private void rdbTime1_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbTime1.IsChecked.Equals(true))
            {
                txtNumTime.IsEnabled = false;
                combUintTime.IsEnabled = false;

                DateStart.IsEnabled = true;
                DateStop.IsEnabled = true;

                DateStart.Value = DateTime.Now.Date;
                DateStop.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
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
                combUintTime.Text = CurrentApp.GetLanguageInfo("3105T00067", "Day");
            }
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
                    DateTime now = DateTime.Now.Date.AddDays(1);
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
                    ShowException(CurrentApp.GetLanguageInfo("3105T00070", "DateTime Is Wrong"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 查詢條件控制項

        private void InitChkClick()
        {
            chkall.Click += chkall_Click;
            chkNrecheck.Click += chkNrecheck_Click;
            chkNexamine.Click += chkNrecheck_Click;
            chkReexamineed.Click += chkNrecheck_Click;
        }

        void chkNrecheck_Click(object sender, RoutedEventArgs e)
        {
            if (chkNrecheck.IsChecked.Equals(true) || chkNexamine.IsChecked.Equals(true) || chkReexamineed.IsChecked.Equals(true))
            {
                chkall.IsChecked = false;
            }
        }

        void chkall_Click(object sender, RoutedEventArgs e)
        {
            if (chkall.IsChecked.Equals(true))
            {
                chkNrecheck.IsChecked = false;
                chkNexamine.IsChecked = false;
                chkReexamineed.IsChecked = false;
            }
        }

        private string CreateQueryCondition()
        {
            string strCondition = string.Empty;
            try
            {
                //匹配纯数字的
                string pattern = @"^\d+$";
                //時間範圍
                if (!CheckInput())
                {
                    return string.Empty;
                }

                DateTimeReturn();
                DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                dateTimeSpliteAsDay.StartDayTime = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime();
                dateTimeSpliteAsDay.StopDayTime = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime();
                mListDateTime.Add(dateTimeSpliteAsDay);
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        strCondition = string.Format("SELECT T319.C009,T319.C002,T319.C003,T319.C012,T319.C004,T318.C004 AS Score," +
                        "T311.C002 AS TemplateName FROM T_31_019_{2} T319 LEFT JOIN T_31_008_{2} T318 ON T319.C002 = T318.C001 " +
                        "LEFT JOIN T_31_001_{2} T311 ON T318.C003=T311.C001 WHERE T319.C012>='{0}' AND T319.C012<='{1}' ",
                        dateTimeSpliteAsDay.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), dateTimeSpliteAsDay.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"), CurrentApp.Session.RentInfo.Token);
                        break;
                    case 3:
                        strCondition = string.Format("SELECT T319.C009,T319.C002,T319.C003,T319.C012,T319.C004,T318.C004 AS Score," +
                        "T311.C002 AS TemplateName FROM T_31_019_{2} T319 LEFT JOIN T_31_008_{2} T318 ON T319.C002 = T318.C001 " +
                        "LEFT JOIN T_31_001_{2} T311 ON T318.C003=T311.C001 WHERE T319.C012>=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND T319.C012<=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS') ",
                        dateTimeSpliteAsDay.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), dateTimeSpliteAsDay.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"), CurrentApp.Session.RentInfo.Token);
                        break;
                    default:
                        ShowException(string.Format("DBType invalid"));
                        return string.Empty;
                }

                //记录流水号
                if (chkRecordReference.IsChecked.Equals(true) && txtRecordReference.Text.Trim(' ').Length > 0)
                {
                    Match m = Regex.Match(txtRecordReference.Text.Trim(' ').ToString(), pattern);
                    if (m.Success)
                    {
                        strCondition += string.Format("AND T319.C003 LIKE '%{0}%' ", txtRecordReference.Text.Trim(' '));
                    }
                    else
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00071", "ReferenceID Is Not Legal"));
                        return string.Empty;
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
                            ShowException(CurrentApp.GetLanguageInfo("3105T00071", "ReferenceID Is Not Legal"));
                            return string.Empty;
                        }
                    }
                    strCondition += string.Format("AND T319.C003 IN ({0}) ", txtRecordReference.Text.Trim(' '));
                }

                //申诉流程
                if (chkNrecheck.IsChecked.Equals(true) || chkNexamine.IsChecked.Equals(true) || chkReexamineed.IsChecked.Equals(true) || chkall.IsChecked.Equals(true))
                {
                    string AppealStateStr = string.Empty;
                    bool IsOr = false;//是否需要用or連接限制條件
                    if (chkNrecheck.IsChecked.Equals(true))//待复核
                    {
                        AppealStateStr += "AND T319.C009 IN (1,2)";
                        IsOr = true;
                    }
                    if (chkNexamine.IsChecked.Equals(true))//待审批
                    {
                        if (IsOr)
                        {
                            AppealStateStr += "OR T319.C009 IN (4)";
                        }
                        else
                        {
                            AppealStateStr += "AND T319.C009 IN (4)";
                        }
                        IsOr = true;
                    }
                    if (chkReexamineed.IsChecked.Equals(true))//已申诉完成
                    {
                        if (IsOr)
                        {
                            AppealStateStr += "OR T319.C009 IN (3,5,6,7)";
                        }
                        else
                        {
                            AppealStateStr += "AND T319.C009 IN (3,5,6,7)";
                        }
                    }
                    if (chkall.IsChecked.Equals(true))
                    {
                        AppealStateStr = "AND T319.C009 IN (1,2,3,4,5,6,7)";
                    }
                    strCondition += AppealStateStr;
                }

                //座席
                if (!string.IsNullOrWhiteSpace(AgentsStr))
                {
                    strCondition += string.Format("AND T319.C004 IN  (SELECT C011  FROM T_00_901 WHERE C001={0} )", AgentsStr);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return string.Empty;
            }
            return strCondition;
        }

        //多值插入临时表
        private string SaveMultiAgent(List<CtrolAgent> listAgents)
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
                    webRequest.ListData.Add(listAgents[i].ObjID.ToString());
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

        #region 删除字符串中重复的元素
        public string DelSameString(string TempStr)
        {
            string newStr = string.Empty;
            try
            {
                string[] TempArray = TempStr.Split(',');
                for (int i = 0; i < TempArray.Length; i++)
                {
                    if (!newStr.Contains(TempArray[i]) || TempArray[i].Equals(','))
                    {
                        newStr += TempArray[i];
                    }
                }
            }
            catch (Exception)
            {

            }
            return newStr;
        }
        #endregion

        #endregion
    }
}
