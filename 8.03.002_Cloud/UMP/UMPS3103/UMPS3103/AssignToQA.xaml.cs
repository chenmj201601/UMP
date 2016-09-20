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
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Common;
using VoiceCyber.NAudio.Controls;

namespace UMPS3103
{
    /// <summary>
    /// AssignToQA.xaml 的交互逻辑
    /// </summary>
    public partial class AssignToQA 
    {
        public TaskAssign PageParent;
        private ObjectItemTask mRootItem;
        private List<CtrolQA> mListCtrolQA;
        string tempName = "QA"+ DateTime.Now.ToString("yyyyMMddHHmmss");
        
        public AssignToQA()
        {
            InitializeComponent();
            mRootItem = new ObjectItemTask();
            mListCtrolQA = new List<CtrolQA>();
            Loaded+=AssignToQA_Loaded;
        }

        void AssignToQA_Loaded(object sender, RoutedEventArgs e)
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
            //chkIsAVGAssign.Content = App.GetLanguageInfo("3103T00101", "IsAVGAssign");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }

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
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Name = qa.UserFullName;
                    item.Description = qa.UserName;
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


        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }


        //private void chkIsAVGAssign_Click(object sender, RoutedEventArgs e)
        //{
        //    if (chkIsAVGAssign.IsChecked.Equals(true))
        //    {
        //        txbTaskName.IsEnabled = false;
        //    }
        //    else 
        //    {
        //        txbTaskName.IsEnabled = true;
        //    }
        //}


        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTaskName.Text.ToString()) )
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
                userTaskInfoShow.TaskType = 1;
                userTaskInfoShow.IsShare = "N";
                userTaskInfoShow.AssignTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm");
                userTaskInfoShow.AssignUser = CurrentApp.Session.UserInfo.UserID;

                userTaskInfoShow.AssignUserFName = CurrentApp.Session.UserInfo.UserName;
                userTaskInfoShow.BelongYear =(int)cmbYear.SelectedValue;
                userTaskInfoShow.BelongMonth = (int)cmbMonth.SelectedValue;
                if (DateDeadLineTime.Value == null)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00116", "Expired Time can not be null."), CurrentApp.AppName);
                    return;
                }
                userTaskInfoShow.DealLine = DateTime.Parse(DateDeadLineTime.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm");
                if (mListCtrolQA.Count == 0)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00115", "Please select one scoring user at least."), CurrentApp.AppName);
                    return;
                }
                else
                {
                    #region 现都平均分配
                    //if (chkIsAVGAssign.IsChecked.Equals(true))
                    //{
                    //    int tempNum = PageParent.mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count();
                    //    if (mListCtrolQA.Count > tempNum)//选择的录音数量少与QA数量，不能平均分配
                    //    {
                    //        MessageBox.Show(App.GetLanguageInfo("3103T00155", "The record is less than the number of quality inspector."), App.AppName);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        PageParent.SaveTask2QA(userTaskInfoShow, mListCtrolQA, 2);
                    //    }
                    //}
                    //else 
                    //{
                    //    PageParent.SaveTask2QA(userTaskInfoShow, mListCtrolQA, 1);
                    //}
                    #endregion 
                    PageParent.SaveTask2QA(userTaskInfoShow, mListCtrolQA, 2);
                   
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                }
               
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
                //        tempStr = tempStr.Substring(0,64);
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
                    CtrolQA ctrolqa= new CtrolQA();
                    ctrolqa = (CtrolQA)o.Data;
                    lstCtrolQa.Add(ctrolqa);
                }

                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetQAIsCheck(o, ref lstCtrolQa);
                }
            }

        }

        void InitCombox()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            List<int> lstYear = new List<int>();
            for(int i=2014;i<=2199;i++)
            {
                lstYear.Add(i);
            }
            cmbYear.ItemsSource = lstYear;
            cmbYear.SelectedItem = Year;

            List<int> lstMonth = new List<int>();
            for (int i = 1; i <= 12;i++ )
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

       
    }
}
