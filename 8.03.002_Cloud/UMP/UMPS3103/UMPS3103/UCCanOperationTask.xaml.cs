// ***********************************************************************
// Assembly         : UMPS3103
// Author           : Luoyihua
// Created          : 01-20-2015
//
// Last Modified By : Luoyihua
// Last Modified On : 01-21-2015
// ***********************************************************************
// <copyright file="UCCanOperationTask.xaml.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;

namespace UMPS3103
{
    /// <summary>
    /// UCCanOperationTask.xaml 的交互逻辑
    /// </summary>
    public partial class UCCanOperationTask
    {
        /// <summary>
        /// 选中的需要移动的任务录音
        /// </summary>
        public string selRemoveRecord;
        /// <summary>
        /// 源任务
        /// </summary>
        public UserTasksInfoShow selfromtask;
        /// <summary>
        /// 移动的数量
        /// </summary>
        public int MoveNum;
        /// <summary>
        /// 移动的总时间
        /// </summary>
        public double MoveTime;

        public TaskRecordDetail ParentPage;
        public static ObservableCollection<UserTasksInfoShow> ListCanOperationTasks = new ObservableCollection<UserTasksInfoShow>();
        public UCCanOperationTask()
        {
            InitializeComponent();
            ChangeTheme();
            this.Loaded += UCCanOperationTask_Loaded;
        }

        void UCCanOperationTask_Loaded(object sender, RoutedEventArgs e)
        {
            SetPageContent();
            InitCanOperationColumns();
            InitCanOperationTasks();
            LVUCanOperation.ItemsSource = ListCanOperationTasks;
            LVUCanOperation.SelectionChanged += LVUCanOperation_SelectedItemChanged;
        }

        private void InitCanOperationTasks()
        {
            try
            {
                string opqa = "";
                foreach (CtrolQA qa in S3103App.ListCtrolQAInfos)
                {
                    opqa += qa.UserID.ToString() + ",";
                }
                opqa = opqa.TrimEnd(',');
                if (opqa == "")
                {
                    return;
                }
                ListCanOperationTasks.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetCanOperationTasks;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(opqa);//QA
                webRequest.ListData.Add(selfromtask.TaskType.ToString());//限定任務類型  --waves

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<UserTasksInfoShow>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        UserTasksInfoShow taskInfo = optReturn.Data as UserTasksInfoShow;
                        if (taskInfo != null)
                        {
                            if (taskInfo.TaskID != selfromtask.TaskID)
                            {
                                taskInfo.IsShare = taskInfo.IsShare == "Y" ? CurrentApp.GetLanguageInfo("3103T00022", "Yes") : CurrentApp.GetLanguageInfo("3103T00023", "No");
                                if (taskInfo.AssignUserFName == CurrentApp.Session.UserID.ToString())
                                {
                                    taskInfo.AssignUserFName = S3103App.mListAuInfoItems.Where(a => a.ID == CurrentApp.Session.UserID).FirstOrDefault().FullName;
                                }
                                ListCanOperationTasks.Add(taskInfo);
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

        private void InitCanOperationColumns()
        {
            string[] lans = "3103T00001,3103T00009,3103T00002,3103T00008,3103T00006".Split(',');
            string[] cols = "TaskName,AssignNum,TaskDesc,AssignUserFName,IsShare".Split(',');
            int[] colwidths = { 130, 75, 120, 120, 75 };
            GridView columngv = new GridView();
            for (int i = 0; i < 5; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LVUCanOperation.View = columngv;
        }

        public void LVUCanOperation_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        { 
            
        }

        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {            
            var CurrentFocusRecord = new UserTasksInfoShow();//目标任务to
            CurrentFocusRecord = (UserTasksInfoShow)LVUCanOperation.SelectedItem;
            if (CurrentFocusRecord != null && !string.IsNullOrWhiteSpace(selRemoveRecord))
            {
                string strrecord = selRemoveRecord; //选中的要移动的录音
                string taskidfrom = selfromtask.TaskID.ToString();              //源任务from ID
                string numfrom = (selfromtask.AssignNum-MoveNum).ToString();          //源任务数量修改成
                string taskidto = CurrentFocusRecord.TaskID.ToString();         //目标任务to ID
                string numto = (CurrentFocusRecord.AssignNum + MoveNum).ToString();     //目标任务数量修改成

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3103Codes.RemoveRecord2Task;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(strrecord);
                    webRequest.ListData.Add(taskidfrom);
                    webRequest.ListData.Add(numfrom);
                    webRequest.ListData.Add(taskidto);
                    webRequest.ListData.Add(numto);
                    webRequest.ListData.Add(selfromtask.AlreadyScoreNum.ToString());//已经评分的数量
                    webRequest.ListData.Add(MoveTime.ToString());//移动的总时间
                    Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        CurrentApp.WriteOperationLog(S3103Consts.OPT_MOVETOOTHERTASK.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                       ShowException(string.Format("{0}.\t{1}\t{2}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), webReturn.Code, webReturn.Message));
                        return;
                    }
                    else
                    {
                        #region 写操作日志
                        string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), strrecord);
                        strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00078"), selfromtask.TaskName);
                        strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00079"), CurrentFocusRecord.TaskName);
                        CurrentApp.WriteOperationLog(S3103Consts.OPT_MOVETOOTHERTASK.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                        CurrentApp.WriteLog("RemoveRecord2Task", string.Format("{0} \t OATaskCount {1} \t MoveCount {2} \t OBTaskCount {3} \t NATaskCount {4} \t NBTaskCount {5}", strLog, selfromtask.AssignNum, MoveNum, CurrentFocusRecord.AssignNum, selfromtask.AssignNum - MoveNum, CurrentFocusRecord.AssignNum + MoveNum));
                        #endregion
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00042", "Operation Succeed."));
                        ParentPage.ShowPopupPanel(false);
                        selfromtask.AssignNum -= MoveNum;
                        ParentPage.InitTaskDetail();
                    }
                }
                catch (Exception ex)
                {
                   ShowException(string.Format("{0}.\t{0}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), ex.Message));
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.ShowPopupPanel(false);
        }

        private void SetPageContent()
        {
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }


    }
}
