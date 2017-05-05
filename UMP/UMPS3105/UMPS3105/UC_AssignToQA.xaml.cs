using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Common;
using VoiceCyber.NAudio.Controls;
using Common3105;
using  UMPS3105.Models;
using VoiceCyber.Common;
using UMPS3105.Wcf31031;
using  ColAlias= VoiceCyber.UMP.Common31031;
using CtrolAgent = Common3105.CtrolAgent;
namespace UMPS3105
{
    /// <summary>
    /// UC_AssignToQA.xaml 的交互逻辑d
    /// </summary>
    public partial class UC_AssignToQA
    {

        public CCheckData SelCheckData;
        public AppealApprovalMainView ParentPage;
        public ProcessReCheck PrePage;


        private List<ObjectItem> mListObjectItems;
        private ObjectItem mRootItem;
        private List<CtrolQA> mListCtrolQA;
        string tempName = "QA" + DateTime.Now.ToString("yyyyMMddHHmmss");
        public string AppealFlowItemID= string.Empty; 

        private RecordInfoItem mListCurrentRecord;

        public UC_AssignToQA()
        {
            InitializeComponent();
            SelCheckData = new CCheckData();
            mRootItem = new ObjectItem();
            mListCtrolQA= new List<CtrolQA>();
            mListObjectItems = new List<ObjectItem>();
            mListCurrentRecord= new RecordInfoItem();
            AppealFlowItemID = string.Empty;
            Loaded +=UC_AssignToQA_Loaded;

        }


        void  UC_AssignToQA_Loaded(object sender,RoutedEventArgs  e)
        {
            ChangeLanguage();
            InitCombox();
            InitCtrolValue();
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnPre.Click +=BtnPre_Click;

            mListObjectItems.Clear();
            ClearChildren(mRootItem);
            InitOrgAndQA(mRootItem, S3105App.CurrentOrg);
            TvObjects.ItemsSource = mRootItem.Children;
        }



        private void ClearChildren(ObjectItem item)
        {
            if (item == null) { return; }
            for (int i = 0; i < item.Children.Count; i++)
            {
                var child = item.Children[i] as ObjectItem;
                if (child != null)
                {
                    var temp = mListObjectItems.FirstOrDefault(j => j.ObjID == child.ObjID);
                    if (temp != null) { mListObjectItems.Remove(temp); }
                    ClearChildren(child);
                }
            }
            Dispatcher.Invoke(new Action(() => item.Children.Clear()));
        }


        private void AddChildObjectItem(ObjectItem parent, ObjectItem child)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (parent != null)
                {
                    parent.AddChild(child);
                }
            }));
        }

        //更新座席分机数据
        void InitOrgAndQA(ObjectItem parentItem, string parentID)
        {
            List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
            lstCtrolOrgTemp = S3105App.ListCtrolOrgInfos.Where(p => p.OrgParentID == parentID).ToList();
            foreach (CtrolOrg org in lstCtrolOrgTemp)
            {
                ObjectItem item = new ObjectItem();
                item.ObjType = ConstValue.RESOURCE_ORG;
                item.ObjID = Convert.ToInt64(org.ID);
                item.Display = org.OrgName;
                item.Name = org.OrgName;
                item.Data = org;
                item.IsSelected = false;
                if (org.ID == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "/Themes/Default/UMPS3105/Images/rootorg.ico";
                }
                else
                {
                    item.Icon = "/Themes/Default/UMPS3105/Images/org.ico";
                }
                mListObjectItems.Add(item);
                if(S3105App.ListCtrolOrgInfos.Where(p=>p.OrgParentID == org.ID).Count()>0)
                {
                    InitOrgAndQA(item, org.ID);
                }               
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
                lstCtrolQATemp = S3105App.ListCtrolQAInfos.Where(p => p.OrgID == parentID).ToList();
                foreach (CtrolQA qa in lstCtrolQATemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Display = qa.UserFullName;
                    item.Name = qa.UserFullName;
                    item.ToolTip = qa.UserName;
                    item.Data = qa;
                    item.IsSelected = false;
                    item.Icon = "/Themes/Default/UMPS3105/Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        //前一步
        void BtnPre_Click(object sender, RoutedEventArgs e) 
        {
            if (S3105App.AppealProcess == "N")
            {
                ParentPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                ParentPage.PopupPanel.Content = PrePage;
            }
            else
            {
                ParentPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                ParentPage.PopupPanel.Content = PrePage;
            }
        }

        void BtnClose_Click(object sender,RoutedEventArgs e)
        {
            var parent = ParentPage.PopupPanel as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender,RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTaskName.Text.ToString()) )
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00122", "Task Infomation Is Full."));
                return;
            }
            List<CtrolQA> lstControlQaTemp = new List<CtrolQA>();
            GetQAIsCheck(ref lstControlQaTemp);
            if(lstControlQaTemp.Count==0)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00123", "Please Select a QA"));
                return;
            }
            UserTasksInfoShow userTaskInfoShow = new UserTasksInfoShow();
            userTaskInfoShow.TaskName = txbTaskName.Text.ToString();
            userTaskInfoShow.TaskType = 3;  // 分配任务类型复检手动任务
            userTaskInfoShow.IsShare = "N";
            userTaskInfoShow.AssignTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm");
            userTaskInfoShow.AssignUser = CurrentApp.Session.UserInfo.UserID;

            userTaskInfoShow.AssignUserFName = CurrentApp.Session.UserInfo.UserName;
            userTaskInfoShow.BelongYear = (int)cmbYear.SelectedValue;
            userTaskInfoShow.BelongMonth = (int)cmbMonth.SelectedValue;
            if (DateDeadLineTime.Value == null)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00124", "Expired Time can not be null."));
                return;
            }
            userTaskInfoShow.DealLine = DateTime.Parse(DateDeadLineTime.Value.ToString()).ToUniversalTime().ToString("yyyy/MM/dd HH:mm");

            //查询这条录音的录音信息
            if (!GetRecordInfoByRef(SelCheckData.RecoredReference))
            {
                return;
            }
            List<RecordInfoItem> lstRecordInfoItem= new List<RecordInfoItem>();
            lstRecordInfoItem.Add(mListCurrentRecord);
            //分配到任务
            SaveTask(userTaskInfoShow, lstControlQaTemp, lstRecordInfoItem);
           

            //更改申诉表信息
            string error= PrePage.UpdateReCheckData("8", AppealFlowItemID, SelCheckData);

            if (!string.IsNullOrEmpty(error))
            {
                ShowException(error);
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));
                PrePage.UpdateTable19("8");
                ParentPage.ApprovalHistory();
                ParentPage.BindData();
                ParentPage.ShowProcessReCheck(false);
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), SelCheckData.RecoredReference);
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
            }
        }


        private bool GetRecordInfoByRef(string recordreference)//20140212... 2014年2月12号
        {
            bool ret = false;
            try
            {
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + CurrentApp.Session.RentInfo.Token;
                var tableInfo =
                        CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + recordreference.Substring(0, 4);
                }
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, recordreference);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)ColAlias.S3103Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                webRequest.ListData.Add("mark");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != ColAlias.S3103Consts.Err_TableNotExit)
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. ListData is null"));
                    return false;
                }
                if (webReturn.ListData.Count <= 0) { return false; }

                OperationReturn optReturn = XMLHelper.DeserializeObject<Common3105.RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                Common3105.RecordInfo recordInfo = optReturn.Data as Common3105.RecordInfo;
                if (recordInfo == null)
                {
                    ShowException(string.Format("Fail. RecordInfo is null"));
                    return false;
                }
                mListCurrentRecord = new RecordInfoItem(recordInfo);
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
        }


        private void GetQAIsCheck(ref List<CtrolQA> lstCtrolQa)
        {
            ObjectItem o = TvObjects.SelectedItem as ObjectItem;
            if (o != null && o.ObjType == ConstValue.RESOURCE_USER)
            {
                CtrolQA ctrolqa = new CtrolQA();
                ctrolqa = (CtrolQA)o.Data;
                lstCtrolQa.Add(ctrolqa);
            }
        }


        //保存任务到QA
        void SaveTask(UserTasksInfoShow userTaskInfoShow, List<CtrolQA> listctrolqa, List<RecordInfoItem> lstRecordInfoItem)
        {
            long taskID = 0;
            try
            {
                //提交任务
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)ColAlias.S3103Codes.SaveTask;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(userTaskInfoShow);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                else
                {
                    taskID = Convert.ToInt64(webReturn.Data);
                    userTaskInfoShow.TaskID = taskID;


                    //提交任务QA T_31_021_00000
                    webReturn = new WebReturn();
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)ColAlias.S3103Codes.SaveTaskQA;
                    webRequest.Data = taskID.ToString();
                    foreach (CtrolQA qa in listctrolqa)
                    {
                        optReturn = XMLHelper.SeriallizeObject(qa);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }

                    //client = new Service31031Client();
                    client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    //WebHelper.SetServiceClient(client);
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    //提交任务录音T_31_022_00000
                    webReturn = new WebReturn();
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)ColAlias.S3103Codes.SaveTaskRecord;
                    ColAlias:: TaskInfoDetail taskinfodetail;
                    foreach (RecordInfoItem record in lstRecordInfoItem)
                    {
                        taskinfodetail = new ColAlias.TaskInfoDetail();
                        taskinfodetail.TaskID = taskID;
                        taskinfodetail.RecoredReference = record.SerialID;
                        taskinfodetail.IsLock = "N";
                        taskinfodetail.AllotType = 4;   //对应T_31_022表的C006 4表示录音是从审批通过分配到任务中的
                        taskinfodetail.FromTaskID = -1;
                        taskinfodetail.TaskType = "3"; //对应T-31_022表的C010
                        taskinfodetail.StartRecordTime = Convert.ToDateTime(record.StartRecordTime).ToUniversalTime();
                        taskinfodetail.Duration = record.Duration.ToString();
                        taskinfodetail.Extension = record.Extension;
                        taskinfodetail.CalledID = record.CalledID;
                        taskinfodetail.CallerID = record.CallerID;
                        taskinfodetail.Direction =int.Parse( record.Direction);
                        List<CtrolAgent> templst = new List<CtrolAgent>();
                        //if (S3103App.GroupingWay.Contains("A")) //因本工程没有分组信息所以本处暂时注释掉
                        //{
                        //    templst = S3103App.ListCtrolAgentInfos.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        //else if (S3103App.GroupingWay.Contains("R"))
                        //{
                        //    templst = S3103App.ListCtrolRealityExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        //else if (S3103App.GroupingWay.Contains("E"))
                        //{
                        //    templst = S3103App.ListCtrolExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        templst = S3105App.ListCtrolAgentInfos.Where(p => p.AgentName == record.Agent).ToList();
                        if (templst.Count() > 0)
                        {
                            taskinfodetail.AgtOrExtID = templst[0].AgentID;
                            taskinfodetail.AgtOrExtName = templst[0].AgentName;
                        }
                        else
                        {
                            taskinfodetail.AgtOrExtID = "";
                            taskinfodetail.AgtOrExtName = "";
                        }

                        optReturn = XMLHelper.SeriallizeObject(taskinfodetail);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }

                    webRequest.Data = taskID.ToString();
                    //client = new Service31031Client();
                    client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    string strlogrecords = "";
                    //将任务号更新到录音表
                    var tableInfo = CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == VoiceCyber.UMP.Common.TablePartType.DatetimeRange);
                    if (tableInfo == null)
                    {
                        string tableName = ConstValue.TABLE_NAME_RECORD;
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            strlogrecords += r.SerialID + ",";
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)ColAlias.S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(taskID.ToString());
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("1");
                            webRequest.ListData.Add(r.SerialID.ToString());

                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }
                    else
                    {
                        string tableName = string.Empty;
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            strlogrecords += r.SerialID + ",";
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)ColAlias.S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(taskID.ToString());
                            tableName = ReturnTableName(Convert.ToDateTime(r.StartRecordTime));
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("1");
                            webRequest.ListData.Add(r.SerialID.ToString());


                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            //WebHelper.SetServiceClient(client);
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog(ColAlias.S3103Consts.OPT_TASKASSIGN.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                ShowException(ex.Message);
            }

            return;
        }


        private string ReturnTableName(DateTime stopTime)
        {
            string strTableName = string.Empty;
            strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token, stopTime.ToString("yyMM"));
            return strTableName;
        }

        void InitCtrolValue() 
        {
            DateDeadLineTime.Value = DateTime.Now.Date.AddDays(7);
            txbTaskName.Text = tempName;//现在统一为质检员+日期,这个为临时名字
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
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3105T00012", "Confirm");
                BtnPre.Content = CurrentApp.GetLanguageInfo("3105T000121", "PreNext");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3105T00013", "Close");
                gpQualitytime.Header = CurrentApp.GetLanguageInfo("3105T000126", "Quality Time");
                labYear.Content = CurrentApp.GetLanguageInfo("3105T000117", "Year");
                labMonth.Content = CurrentApp.GetLanguageInfo("3105T000118", "Month");
                labTaskValidTime.Content = CurrentApp.GetLanguageInfo("3105T000119", "Task DeadLine Time");
                labTaskName.Content = CurrentApp.GetLanguageInfo("3105T000120", "Task Name");
            }
            catch (Exception e)
            {

            }
        }
    }
}
