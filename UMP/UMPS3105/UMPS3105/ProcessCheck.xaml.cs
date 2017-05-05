// ***********************************************************************
// Assembly         : UMPS3105
// Author           : Luoyihua
// Created          : 12-15-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 12-15-2014
// ***********************************************************************
// <copyright file="ProcessCheck.xaml.cs" company="VoiceCodes">
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
using UMPS3105.Wcf11012;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3105 //複核
{
    /// <summary>
    /// ProcessCheck.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessCheck
    {
        public CCheckData SelCheckData;
        public AppealRecheckMainView ParentPage;
        public List<CAppeaDetail> ListCAppeaDetails;
        public ProcessCheck()
        {
            InitializeComponent();
            this.Loaded += ProcessCheck_Loaded;
        }

        void ProcessCheck_Loaded(object sender, RoutedEventArgs e)
        {
            ListCAppeaDetails = new List<CAppeaDetail>();
            SetPageContent();
            ShowAppealData();
        }

        private void ShowAppealData()
        {
            if (SelCheckData != null)
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetCheckAndReCheckData;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID.ToString());
                webRequest.ListData.Add("1,2");
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
                            ListCAppeaDetails.Add(taskdetail);
                        }
                    }
                    if (ListCAppeaDetails.Count > 0)
                    {
                        lbuseranddate.Content = SelCheckData.AgentName;
                        txtShowAppealContent.Text = ListCAppeaDetails[0].Note;
                    }
                }
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if ((rbtnAccept.IsChecked == true || rbtnRejected.IsChecked == true) && !string.IsNullOrWhiteSpace(txtCheckDisContent.Text))
            {
                long lserialid = ((S3105App)CurrentApp).GetSerialID();
                if (lserialid != -1)
                {
                    try
                    {
                        string AppealFlowItemID = lserialid.ToString();
                        string action = rbtnAccept.IsChecked == true ? "4" : "3";
                        string opttime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        
                        WebRequest webRequest = new WebRequest();
                        webRequest.Code = (int)S3105Codes.SubmitCheckData;
                        webRequest.Session = CurrentApp.Session;
                        webRequest.ListData.Add(SelCheckData.AppealDetailID);//AppealDetailID  0
                        webRequest.ListData.Add(SelCheckData.AppealFlowID);//AppealFlowID   1
                        webRequest.ListData.Add(AppealFlowItemID);//AppealFlowItemID    2
                        webRequest.ListData.Add(txtCheckDisContent.Text);//Note     3
                        webRequest.ListData.Add(action);//ActionID 3驳回 4通过   4
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());//OperationerID      5       
                        webRequest.ListData.Add(opttime);//操作时间     6
                        webRequest.ListData.Add(SelCheckData.ScoreResultID);//T_31_008成績ID C001     7
                        webRequest.ListData.Add(SelCheckData.RecoredReference);//錄音流水號ID        8
                        webRequest.ListData.Add(SelCheckData.ScoreType);//評分成績來源        9
                        webRequest.ListData.Add("0");//新的評分成績ID        10
                        //Service31051Client client = new Service31051Client();
                        Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                        WebReturn webReturn = client.UMPTaskOperation(webRequest);
                        client.Close();
                        string checkdetail = action == "4" ? "3105T00053" : "3105T00028";
                        if (!webReturn.Result)
                        {
                            #region 写操作日志
                            CurrentApp.WriteOperationLog(S3105Consts.OPT_Review.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                            #endregion
                            ShowException(string.Format("{0}.\t{1}\t{2}", CurrentApp.GetLanguageInfo("3105T00023", "Operation Field."), webReturn.Code, webReturn.Message));
                            return;
                        }
                        else
                        {
                            UpdateTable19();
                            ParentPage.ReviewHistory();
                            #region 写操作日志
                            string strLog = string.Format("{0} {1}", Utils.FormatOptLogString(checkdetail), SelCheckData.RecoredReference);
                            CurrentApp.WriteOperationLog(S3105Consts.OPT_Review.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                            #endregion
                            ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));
                            ParentPage.BindData();
                            ParentPage.PopupPanel.IsOpen = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(string.Format("{0}.\t{0}", CurrentApp.GetLanguageInfo("3105T00023", "Operation Field."), ex.Message));
                    }
                }
            }
            else
            {
                ShowException(CurrentApp.GetLanguageInfo("3105T00021", "Incomplete information"));
            }
        }

        private void UpdateTable19()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.UpdateTable19;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID);//AppealDetailID
                if (rbtnAccept.IsChecked == true)
                {
                    webRequest.ListData.Add("4");//C009
                    webRequest.ListData.Add("2");//C008
                }
                else
                {
                    webRequest.ListData.Add("3");//C009
                    webRequest.ListData.Add("2");//C008
                    webRequest.ListData.Add("Y");//C010
                }
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00103", "Update Defeated"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.PopupPanel.IsOpen = false;
        }

        public void SetPageContent()
        {
            lbappealer.Content = CurrentApp.GetLanguageInfo("3105T00014", "Appealer");
            lbappealdis.Content = CurrentApp.GetLanguageInfo("3105T00016", "Description");
            lbcheckdis.Content = CurrentApp.GetLanguageInfo("3105T00025", "ReCheck Opinion");
            lbcheckres.Content = CurrentApp.GetLanguageInfo("3105T00024", "Result");
            rbtnAccept.Content = CurrentApp.GetLanguageInfo("3105T00053", "Accept");
            rbtnRejected.Content = CurrentApp.GetLanguageInfo("3105T00028", "Rejected");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3105T00012", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3105T00013", "Close");
        }
    }
}
