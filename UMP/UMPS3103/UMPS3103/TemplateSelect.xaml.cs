// ***********************************************************************
// Assembly         : UMPS3103
// Author           : Luoyihua
// Created          : 11-20-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 11-20-2014
// ***********************************************************************
// <copyright file="TemplateSelect.xaml.cs" company="VoiceCodes">
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
using UMPS3103.Models;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;

namespace UMPS3103
{
    /// <summary>
    /// TemplateSelect.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateSelect
    {
        /// <summary>
        /// 父页面（任务详情页面）
        /// </summary>
        public TaskRecordDetail ParentPage;
        public TaskInfoDetail selTaskRecord;
        public List<BasicScoreSheetItem> mListScoreSheetItems;
        private BasicScoreSheetItem selScoreSheet;
        private long oldTemplateID;
        public TemplateSelect()
        {
            InitializeComponent();
            oldTemplateID=0;
            this.Loaded += TaskRecordDetail_Loaded;
        }

        void TaskRecordDetail_Loaded(object sender, RoutedEventArgs e)
        {
            if (selTaskRecord.TaskType == "4" || selTaskRecord.TaskType=="3")//复检任务才需要去查询初检使用的哪张评分表
            {
                GetScoreTemplateID(selTaskRecord.RecoredReference);
            }
            LoadUserScoreSheetList();
            if (mListScoreSheetItems.Count > 0)
            {
                LoadUserScoreResultList();
                BindCmbTemplates();
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00132", "No Template."));
                ParentPage.ShowPopupPanel(false);
            }
            SetPageContent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (selTaskRecord.TaskType == "4" || selTaskRecord.TaskType == "3")
            {
                if (selScoreSheet.ScoreSheetID != oldTemplateID)
                {
                    var confirmResult = MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00184", "The score sheet don not be used for the last time,continue?"),
                                     CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmResult != MessageBoxResult.Yes) { return; }
                }
            }
            ParentPage.ScoreTaskRecord(selScoreSheet);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.ShowPopupPanel(false);
        }

        private void SetPageContent()
        {
            labTemplate.Content = CurrentApp.GetLanguageInfo("3103T00046", "Template Name");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }

        private void GetScoreTemplateID(long RecoredReference)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetScoreTemplateID;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(selTaskRecord.RecoredReference.ToString());
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return ;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data))
                {
                   ShowException(string.Format("Fail.\tData is null"));
                    return ;
                }
                oldTemplateID = Convert.ToInt64(webReturn.Data);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        public void LoadUserScoreSheetList()
        {
            try
            {
                mListScoreSheetItems = new List<BasicScoreSheetItem>();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(selTaskRecord.RecoredReference.ToString());
                //if (selTaskRecord.TaskType == "2" || selTaskRecord.TaskType == "4")//如果是自动任务分配，在去匹配座席ID
                //{
                //    var item = App.ListCtrolAgentInfos.Where(a => a.AgentName == selTaskRecord.AgtOrExtID).FirstOrDefault();
                //    webRequest.ListData.Add(item.AgentID);
                //}
                //else
                //{
                //}
                webRequest.ListData.Add(selTaskRecord.AgtOrExtID);
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(selTaskRecord.TaskID.ToString());
              //  Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                   ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    if (info == null)
                    {
                       ShowException(string.Format("Fail.\tBaiscScoreSheetInfo is null"));
                        return;
                    }
                    BasicScoreSheetItem item = new BasicScoreSheetItem(info);
                    item.RowNumber = i + 1;
                    item.Background = GetScoreSheetBackground(item);
                    if (selTaskRecord.TaskType == "3" || selTaskRecord.TaskType == "4")
                    {
                        if (item.ScoreSheetID == oldTemplateID) { item.Title = string.Format("({0})", CurrentApp.GetLanguageInfo("3103T00183", "1st Task")) + item.Title; }
                    }
                    if (ParentPage.mViewScore )
                    {
                        if (item.ScoreSheetID == selTaskRecord.TemplateID)
                        {
                            mListScoreSheetItems.Add(item);
                        }
                    }
                    else
                    {
                        mListScoreSheetItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        public void LoadUserScoreResultList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(selTaskRecord.RecoredReference.ToString());
                webRequest.ListData.Add(selTaskRecord.AgtOrExtID);
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(selTaskRecord.ScoreUserID.ToString());
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
                if (webReturn.ListData == null)
                {
                   ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    if (info == null)
                    {
                       ShowException(string.Format("Fail.\tBaiscScoreSheetInfo is null"));
                        return;
                    }
                    long orgID = 0;
                    var agentInfo = S3103App.mListAllObjects.FirstOrDefault(a => a.ObjType == ConstValue.RESOURCE_AGENT && a.Name == selTaskRecord.AgtOrExtName);
                    if (agentInfo != null)
                    {
                        var orgInfo = agentInfo.Parent as ObjectItems;
                        if (orgInfo != null)
                        {
                            if (orgInfo.ObjType == ConstValue.RESOURCE_ORG)
                            {
                                orgID = orgInfo.ObjID;
                            }
                        }
                    }
                    info.OrgID = orgID;
                    BasicScoreSheetItem item =
                        mListScoreSheetItems.FirstOrDefault(s => s.ScoreSheetID == info.ScoreSheetID);
                    if (item != null && info.ScoreResultID == selTaskRecord.OldScoreID)
                    {
                        item.ScoreSheetInfo = info;
                        item.ScoreResultID = info.ScoreResultID;
                        item.Score = info.Score;
                        item.Flag = 1;
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private Brush GetScoreSheetBackground(BasicScoreSheetItem scoreSheetItem)
        {
            int rowNumber = scoreSheetItem.RowNumber;
            if (rowNumber % 2 == 0)
            {
                return Brushes.LightGray;
            }
            return Brushes.Transparent;
        }

        private void BindCmbTemplates()
        {
            this.cmbtemplate.ItemsSource = mListScoreSheetItems;
            this.cmbtemplate.DisplayMemberPath = "Title";
            this.cmbtemplate.SelectedValuePath = "Title";
            this.cmbtemplate.SelectedIndex = 0;
        }

        private void cmbtemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BasicScoreSheetItem seltemp = cmbtemplate.SelectedItem as BasicScoreSheetItem;
            if (seltemp != null)
            {
                selScoreSheet = seltemp;
            }
        }
    }
}
