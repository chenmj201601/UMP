//======================================================================
//
//　╭══╮　┌════════┐
//╭╯  ©   ║═║ By  Waves   ║
//╰⊙═⊙╯   └══⊙══⊙═┘
//
//created by Waves at  2015-6-24 15:26:58
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using UMPS3103.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Controls;

namespace UMPS3103.DoubleTask
{
    /// <summary>
    /// QueryReCheck.xaml 的交互逻辑
    /// </summary>
    public partial class QueryReCheck
    {
        public DoubleTaskAssign ParentPage;

        private ObjectItemTask mRootQa;
        private BackgroundWorker mWorker;
        private List<DateTimeSpliteAsDay> mListDateTime;

        public string QACondition;


        public QueryReCheck()
        {
            InitializeComponent();
            QACondition = string.Empty;
            mListDateTime=new List<DateTimeSpliteAsDay>();

            BtnClose.Click += BtnClose_Click;
            BtnConfirm.Click += Btnconfirm_Click;
            this.Loaded += QueryReCheck_Loaded;
        }

        void QueryReCheck_Loaded(object sender, RoutedEventArgs e)
        {
            mRootQa = new ObjectItemTask();
            ChangeLanguage();
            DateStart.Value = DateTime.Now.Date;
            DateStop.Value = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
            InitControlObjects();
            TQAObjects.ItemsSource = mRootQa.Children;

        }

        private void ChangeLanguage()
        {
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
            rdbTime1.Content = CurrentApp.GetLanguageInfo("3103T00086", "Record Start And Stop Time");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
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

        #region QA樹

        private void InitControlObjects()
        {
            try
            {
                //ParentPage.SetBusy(true);
                mWorker = new BackgroundWorker();
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    InitOrgAndQA(mRootQa,S3103App.CurrentOrg);
                    CurrentApp.WriteLog("PageLoad", string.Format("Load ControlObject"));
                };
                //当后台操作已完成、被取消或引发异常时发生
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    //ParentPage.SetBusy(false);
                };
                mWorker.RunWorkerAsync();//触发DoWork事件
            }
            catch (Exception)
            {

            }
        }

        void InitOrgAndQA(ObjectItemTask parentItem, string parentID)
        {
            List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
            lstCtrolOrgTemp = S3103App.ListCtrolOrgInfos.Where(p => p.OrgParentID == parentID).ToList();
            foreach (CtrolOrg org in lstCtrolOrgTemp)
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
                InitOrgAndQA(item, org.ID);
                InitControlQA(item, org.ID);
                AddChildObject(parentItem, item);
            }
        }

        private void InitControlQA(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                lstCtrolQATemp = S3103App.ListCtrolQAInfos.Where(p => p.OrgID == parentID).ToList();
                foreach (CtrolQA qa in lstCtrolQATemp)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Name = qa.UserName;
                    item.Description = qa.UserFullName;
                    item.Data = qa;
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

        private void AddChildObject(ObjectItemTask parentItem, ObjectItemTask item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void GetQAIsCheck(ObjectItemTask parent, ref List<CtrolQA> lstCtrolQa)
        {
            //mRootItem
            foreach (ObjectItemTask o in parent.Children)
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

        #region 查詢控制


        private void Btnconfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DateStart.Value.ToString()) || string.IsNullOrWhiteSpace(DateStop.Value.ToString()))
                {
                   ShowException(CurrentApp.GetLanguageInfo("3103T00156", "DateTime Is Wrong"));
                    return;
                }
                else if (DateTime.Parse(DateStart.Value.ToString()).CompareTo(DateTime.Parse(DateStop.Value.ToString())).Equals(1))
                {
                   ShowException(CurrentApp.GetLanguageInfo("3103T00156", "DateTime Is Wrong"));
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
                    ParentPage.QueryDoubleTaskRecord(strQueryCondition, mListDateTime);
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

        private string CreateQueryCondition()
        {
            string strCondition = string.Empty;
            try
            {
                mListDateTime.Clear();
                //匹配纯数字的
                string pattern = @"^\d+$";
                //時間範圍
                if (!CheckInput())
                {
                    return string.Empty;
                } 
                DateTimeSpliteAsDay dateTimeSpliteAsDay = new DateTimeSpliteAsDay();
                dateTimeSpliteAsDay.StartDayTime = DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime();
                dateTimeSpliteAsDay.StopDayTime = DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime();
                mListDateTime.Add(dateTimeSpliteAsDay);
                string timeStr = string.Empty;
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        timeStr = string.Format("T21.C005 >= '{0}' AND T21.C005 <= '{1}'",
                            DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                        break;
                    case 3:
                        timeStr = string.Format("T21.C005 >=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND T21.C005 <=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS')",
                            DateTime.Parse(DateStart.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Parse(DateStop.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                        break;
                    default:
                       ShowException(string.Format("DBType invalid"));
                        return string.Empty;
                }

                //strCondition = string.Format("WHERE {0} AND T308.C009 = 'Y' ", timeStr);
                strCondition = string.Format("WHERE {0} ", timeStr);

                //if (chkScoreRange.IsChecked.Equals(true))
                //{
                //    bool isTrue = true;
                //    int min = 0;
                //    int max = 0;
                //    if (!string.IsNullOrWhiteSpace(txtScoreMin.Text.Trim()))
                //    {
                //        Match n = Regex.Match(txtScoreMin.Text.Trim(' '), pattern);
                //        if (n.Success) min = Convert.ToInt32(txtScoreMin.Text.Trim());
                //        else isTrue = false;
                //    }
                //    if (!String.IsNullOrWhiteSpace(txtScoreMax.Text.Trim()))
                //    {
                //        Match m = Regex.Match(txtScoreMax.Text.Trim(' '), pattern);
                //        if (m.Success) max = Convert.ToInt32(txtScoreMax.Text.Trim());
                //        else isTrue = false;
                //    }
                //    if (String.IsNullOrWhiteSpace(txtScoreMax.Text.Trim())||isTrue.Equals(false))
                //    {
                //        App.ShowExceptionMessage(string.Format(""));
                //        return string.Empty;
                //    }
                //    strCondition += string.Format("AND T308.C004 >= '{0}' AND T308.C004 <= '{1}' ", min, max);
                //}
                string tempQa = string.Empty;
                List<CtrolQA> lstCtrolQaTemp = new List<CtrolQA>();
                GetQAIsCheck(mRootQa, ref lstCtrolQaTemp);
                if (lstCtrolQaTemp.Count > 0)
                {
                    string stra = "";
                    foreach (CtrolQA ca in lstCtrolQaTemp)
                    {
                        stra += ca.UserID + ",";
                    }
                    stra = stra.TrimEnd(',');
                    tempQa = stra;
                }
                if (!string.IsNullOrWhiteSpace(tempQa))
                {
                    strCondition += string.Format("AND T308.C005 IN ({0}) ", tempQa);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return string.Empty;
            }
            return strCondition;
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
                   ShowException(CurrentApp.GetLanguageInfo("3103T00156", "DateTime Is Wrong"));
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
    }
}
