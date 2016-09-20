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
using UMPS3105.Wcf31031;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3105
{
    /// <summary>
    /// TemplateSelect.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateSelect
    {
        /// <summary>
        /// 父页面（任务详情页面）
        /// </summary>
        public AppealApprovalMainView ParentPage;
        public CCheckData selTaskRecord;
        private List<BasicScoreSheetItem> mListScoreSheetItems;
        private BasicScoreSheetItem selScoreSheet;
        public string action;
        public string AppealFlowItemID;
        public string strDisContent;
        public CCheckData SelCheckData;
        public TemplateSelect()
        {
            InitializeComponent();
            this.Loaded += TaskRecordDetail_Loaded;
        }

        void TaskRecordDetail_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserScoreSheetList();
            LoadUserScoreResultList();
            SetPageContent();
            BindCmbTemplates();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (selScoreSheet.ScoreSheetID != Convert.ToInt64(selTaskRecord.TemplateID))
            {
                var confirmResult = MessageBox.Show(CurrentApp.GetLanguageInfo("3105T00115", "The score sheet don not be used for the last time,continue?"),
                                 CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmResult != MessageBoxResult.Yes) { return; }
            }
            ParentPage.ScoreTaskRecord(selScoreSheet, action, AppealFlowItemID, SelCheckData, strDisContent);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.ShowPopupPanel(false);
        }

        private void SetPageContent()
        {
            labTemplate.Content = CurrentApp.GetLanguageInfo("3105T00033", "Template Name");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3105T00034", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3105T00035", "Close");
        }
        
        private void LoadUserScoreSheetList()
        {
            try
            {
                mListScoreSheetItems = new List<BasicScoreSheetItem>();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(selTaskRecord.RecoredReference.ToString());
                webRequest.ListData.Add(ParentPage.AgentID);
                webRequest.ListData.Add("0");
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null || webReturn.ListData.Count==0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
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
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    BasicScoreSheetItem item = new BasicScoreSheetItem(info);
                    item.RowNumber = i + 1;
                    item.Background = GetScoreSheetBackground(item);
                    //控制唯一評分表 2016年6月7日 16:37:47 目前修改：列出所有可用评分表，但是提示打分被申诉所用的那张评分表
                    if (item.ScoreSheetID == Convert.ToInt64(selTaskRecord.TemplateID))
                    {
                        item.Title = string.Format("({0})",CurrentApp.GetLanguageInfo("","Last Score")) + item.Title;
                    }
                    mListScoreSheetItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserScoreResultList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(selTaskRecord.RecoredReference.ToString());
                //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(selTaskRecord.AgentID);
                webRequest.ListData.Add("1");
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    if (info == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    BasicScoreSheetItem item =
                        mListScoreSheetItems.FirstOrDefault(s => s.ScoreSheetID == info.ScoreSheetID);
                    if (item != null)
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
            if (seltemp.ScoreResultID == 0 && seltemp.ScoreSheetID.ToString() == SelCheckData.TemplateID)
            {
                seltemp.ScoreResultID = Convert.ToInt64(SelCheckData.ScoreResultID);
            }
            if (seltemp != null)
            {
                selScoreSheet = seltemp;
            }
        }
    }
}
