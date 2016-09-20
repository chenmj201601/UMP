// ***********************************************************************
// Assembly         : UMPS3105
// Author           : Luoyihua
// Created          : 12-16-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 12-17-2014
// ***********************************************************************
// <copyright file="ProcessReCheck.xaml.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
using Common3105;
using UMPS3105.Models;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3105 //審批
{
    /// <summary>
    /// ProcessReCheck.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessReCheck 
    {
        public CCheckData SelCheckData;
        public AppealApprovalMainView ParentPage;
        public List<CAppeaDetail> ListCAppeaDetails;
        public List<CAppeaDetail> ListCCheckDetails;
        /// <summary>
        /// 判斷複核是否完成
        /// </summary>
        public Boolean RecheckIsCom = true;
        public ProcessReCheck()
        {
            InitializeComponent();
            this.Loaded += ProcessReCheck_Loaded;
        }

        void ProcessReCheck_Loaded(object sender, RoutedEventArgs e)
        {
            ListCAppeaDetails = new List<CAppeaDetail>();
            ListCCheckDetails = new List<CAppeaDetail>();
            if (((S3105App)CurrentApp).AppealProcess == "N")
            {
                stpCheck.Visibility = Visibility.Collapsed;
                grid.Height = 220;
                grid.Width = 450;
            }
            SetPageContent();
            ShowAppealData();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string error = string.Empty;
            if ((rbtnKeepScore.IsChecked == true || rbtnReScore.IsChecked == true || rbtnRejected.IsChecked == true) && !string.IsNullOrWhiteSpace(txtCheckDisContent.Text))
            {
                long lserialid = ((S3105App)CurrentApp).GetSerialID();
                if (lserialid != -1)
                {
                    try
                    {
                        string AppealFlowItemID = lserialid.ToString();
                        string action="7";
                        if (rbtnKeepScore.IsChecked == true)
                            action = "5";
                        else if(rbtnReScore.IsChecked == true)
                            action = "6";
                        if (action == "6")
                        {
                            RecheckIsCom = false;
                            ParentPage.ShowTemplatePop(action, AppealFlowItemID, SelCheckData, txtReCheckDisContent.Text);
                            //UpdateTable19(action);
                            //ParentPage.ApprovalHistory();
                        }
                        if (RecheckIsCom)
                        {
                            error = UpdateReCheckData(action, AppealFlowItemID, SelCheckData);
                        }
                        else
                        {
                            return ;
                        }
                        
                        if (!string.IsNullOrEmpty(error))
                        {
                            ShowException(error);
                        }
                        else
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));
                            UpdateTable19(action);
                            ParentPage.ApprovalHistory();
                            ParentPage.BindData();
                            ParentPage.ShowProcessReCheck(false);
                            #region 写操作日志
                            string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), SelCheckData.RecoredReference);
                            CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(string.Format("{0}.\t{0}", CurrentApp.GetLanguageInfo("3105T00023", "Operation Field."), ex.Message));
                        #region 写操作日志
                        string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), SelCheckData.RecoredReference);
                        CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                        #endregion
                    }
                }
            }
            else
            {
                ShowException(CurrentApp.GetLanguageInfo("3105T00021", "Incomplete information"));
            }
        }

        private void UpdateTable19(string action)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.UpdateTable19;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID);//AppealDetailID
                webRequest.ListData.Add(action);//C009
                webRequest.ListData.Add("3");//C008
                webRequest.ListData.Add("Y");//C010
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00103","Update Defeated"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        public string UpdateReCheckData(string action,string AppealFlowItemID,CCheckData SelCheckData)
        {
            string error = string.Empty; 
            try
            {
                string opttime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.SubmitCheckData;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID);//AppealDetailID   0
                webRequest.ListData.Add(SelCheckData.AppealFlowID);//AppealFlowID       1
                webRequest.ListData.Add(AppealFlowItemID);//AppealFlowItemID        2
                webRequest.ListData.Add(txtReCheckDisContent.Text);//Note       3
                webRequest.ListData.Add(action);//ActionID 5审批通过不修改分数，6审批通过重新评分，7审批驳回       5
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());//OperationerID      5
                webRequest.ListData.Add(opttime);//操作时间     6
                webRequest.ListData.Add(SelCheckData.ScoreResultID);//T_31_008成績IDC001      7
                webRequest.ListData.Add(SelCheckData.RecoredReference);//錄音流水號ID        8
                webRequest.ListData.Add(SelCheckData.ScoreType);//評分成績來源        9
                webRequest.ListData.Add("0");//新的評分成績ID        10
                //Service31051Client client = new Service31051Client();
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                string actiondetail = "";
                switch (action)
                {
                    case "5":
                        actiondetail = "3105T00026";
                        break;
                    case "6":
                        actiondetail = "3105T00027";
                        break;
                    case "7":
                        actiondetail = "3105T00019";
                        break;
                }
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                    #endregion

                    ShowException(string.Format("{0}.\t{1}\t{2}",
                        CurrentApp.GetLanguageInfo("3105T00023", "Operation Field."), webReturn.Code, webReturn.Message));
                    return error;
                }
                else
                {
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString(actiondetail), SelCheckData.RecoredReference);
                    CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    //ParentPage.BindData();
                    //ParentPage.ShowProcessReCheck(false);
                    //ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.ShowProcessReCheck(false);
        }

        private void ShowAppealData()
        {
            if (SelCheckData != null)
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetCheckAndReCheckData;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID.ToString());
                webRequest.ListData.Add("1,2,3,4");
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
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
                        OperationReturn optReturn = XMLHelper.DeserializeObject<CAppeaDetail>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        CAppeaDetail taskdetail = optReturn.Data as CAppeaDetail;
                        if (taskdetail != null)
                        {
                            if (taskdetail.ActionID == "1" || taskdetail.ActionID == "2")
                                ListCAppeaDetails.Add(taskdetail);
                            else
                                ListCCheckDetails.Add(taskdetail);
                        }
                    }
                    if (ListCAppeaDetails.Count > 0)
                    {
                        lbuseranddate.Content = SelCheckData.AgentName;
                        txtShowAppealContent.Text = ListCAppeaDetails[0].Note;
                    }
                    if (ListCCheckDetails.Count > 0)
                    {
                        lbchuseranddate.Content = ListCCheckDetails[0].OperationerName;
                        txtCheckDisContent.Text = ListCCheckDetails[0].Note;
                    }
                }
            }
        }

        public void SetPageContent()
        {
            lbappealer.Content = CurrentApp.GetLanguageInfo("3105T00014", "Appealer");
            lbappealdis.Content = CurrentApp.GetLanguageInfo("3105T00016", "Description");

            lbchuser.Content = CurrentApp.GetLanguageInfo("3105T00054", "ReCheck User");
            lbcheckdis.Content = CurrentApp.GetLanguageInfo("3105T00025", "ReCheck Opinion");

            lbrecheckdis.Text = CurrentApp.GetLanguageInfo("3105T00017", "Check Opinion");
            lbrecheckres.Content = CurrentApp.GetLanguageInfo("3105T00018", "ReCheck Opinion");

            rbtnKeepScore.Content = CurrentApp.GetLanguageInfo("3105T00026", "Score Unchanged");
            rbtnReScore.Content = CurrentApp.GetLanguageInfo("3105T00027", "Score Amended");
            rbtnRejected.Content = CurrentApp.GetLanguageInfo("3105T00019", "Rejected");

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3105T00012", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3105T00013", "Close");
        }
    }
}
