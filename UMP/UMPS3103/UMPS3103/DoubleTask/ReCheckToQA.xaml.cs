﻿using System;
using System.Collections.Generic;
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
using UMPS3103.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Controls;

namespace UMPS3103.DoubleTask
{
    /// <summary>
    /// ReCheckToQA.xaml 的交互逻辑
    /// </summary>
    public partial class ReCheckToQA 
    {
        public DoubleTaskAssign PageParent;
        private ObjectItemTask mRootItem;
        private List<CtrolQA> mListCtrolQA;
        /// <summary>
        /// 只保存被勾選的初檢錄音的任務人信息
        /// </summary>
        public List<RecordInfoItem> ReCheckRecordInfoItem;
        string tempName = "QA" + DateTime.Now.ToString("yyyyMMddHHmmss");

        public ReCheckToQA()
        {
            InitializeComponent();
            mRootItem = new ObjectItemTask();
            mListCtrolQA = new List<CtrolQA>();
            this.Loaded += ReCheckToQA_Loaded;
        }

        void ReCheckToQA_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitCombox();
            InitCtrolValue();
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            InitOrgAndQA(mRootItem, S3103App.CurrentOrg);
            TvObjects.ItemsSource = mRootItem.Children;
        }

        /// Author           : YuQiangBo
        ///  Created          : 2015-01-29 16:15:32
        /// <summary>
        /// 加载语言包
        /// </summary>
        private void ChangeLanguage()
        {
            gpQualitytime.Header = CurrentApp.GetLanguageInfo("3103T00104", "Quality Time");
            labYear.Content = CurrentApp.GetLanguageInfo("3103T00015", "Year");
            labMonth.Content = CurrentApp.GetLanguageInfo("3103T00016", "Month");
            labTaskValidTime.Content = CurrentApp.GetLanguageInfo("3103T00096", "Task DeadLine Time");
            labTaskName.Content = CurrentApp.GetLanguageInfo("3103T00102", "Task Name");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }

        #region QA樹
        //更新座席分机数据
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

        private void AddChildObject(ObjectItemTask parentItem, ObjectItemTask item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void InitControlQA(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                lstCtrolQATemp = S3103App.ListCtrolQA.Where(p => p.OrgID == parentID).ToList();
                foreach (CtrolQA qa in lstCtrolQATemp)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    bool IsAdd = true;
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Name = qa.UserFullName;
                    item.Description = qa.UserName;
                    item.Data = qa;
                    item.IsChecked = false;
                    item.Icon = "/Themes/Default/UMPS3103/Images/agent.ico";
                    foreach (RecordInfoItem infoItem in ReCheckRecordInfoItem)
                    {
                        if (qa.UserID == infoItem.TaskUserID) IsAdd = false;
                    }
                    if (IsAdd){AddChildObject(parentItem, item);}
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }


        private bool CreateQueryCondition()
        {
            bool flag = true;
            try
            {
                mListCtrolQA.Clear();
                List<CtrolQA> lstCtrolQaTemp = new List<CtrolQA>();
                GetQAIsCheck(mRootItem, ref lstCtrolQaTemp);
                mListCtrolQA = lstCtrolQaTemp;
                string name = string.Empty;
                foreach (CtrolQA tempQA in lstCtrolQaTemp)
                {
                    name += tempQA.UserName;
                }
                //if (txbTaskName.Text == tempName)
                //{
                //    string tempStr = name + DateTime.Now.ToString("yyyyMMddHHmmss");
                //    if (tempStr.Length > 64)
                //    {
                //        tempStr = tempStr.Substring(0, 64);
                //    }
                //    txbTaskName.Text = tempStr;
                //}
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return false;
            }
            return flag;
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


        void InitCombox()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            List<int> lstYear = new List<int>();
            for (int i = 2014; i <= 2199; i++)
            {
                lstYear.Add(i);
            }
            cmbYear.ItemsSource = lstYear;
            cmbYear.SelectedItem = Year;

            List<int> lstMonth = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                lstMonth.Add(i);
            }
            cmbMonth.ItemsSource = lstMonth;
            cmbMonth.SelectedItem = Month;
        }
        void InitCtrolValue()
        {
            DateDeadLineTime.Value = DateTime.Now.Date.AddDays(7);
            txbTaskName.Text = tempName;//现在统一为质检员+日期,这个为临时名字
        }
        
        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTaskName.Text.ToString()))
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00123", "Task Name Is Null."));
                return;
            }

            if (!CreateQueryCondition())
            {
                return;
            }
            if (PageParent != null)
            {
                UserTasksInfoShow userTaskInfoShow = new UserTasksInfoShow();
                userTaskInfoShow.TaskName = txbTaskName.Text.ToString();
                userTaskInfoShow.TaskType = 3;
                userTaskInfoShow.IsShare = "N";
                userTaskInfoShow.AssignTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                userTaskInfoShow.AssignUser = CurrentApp.Session.UserInfo.UserID;

                userTaskInfoShow.AssignUserFName = CurrentApp.Session.UserInfo.UserName;
                userTaskInfoShow.BelongYear = (int)cmbYear.SelectedValue;
                userTaskInfoShow.BelongMonth = (int)cmbMonth.SelectedValue;
                if (DateDeadLineTime.Value == null)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00116", "Expired Time can not be null."), CurrentApp.AppName);
                    return;
                }
                userTaskInfoShow.DealLine = DateTime.Parse(DateDeadLineTime.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                if (mListCtrolQA.Count == 0)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00115", "Please select one scoring user at least."), CurrentApp.AppName);
                    return;
                }
                if (mListCtrolQA.Count == 0)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("", "Please select only one scoring user ."), CurrentApp.AppName);
                    return;
                }
                else
                {
                    PageParent.SaveRecheckTask2QA(userTaskInfoShow, mListCtrolQA);

                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
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

    }
}
