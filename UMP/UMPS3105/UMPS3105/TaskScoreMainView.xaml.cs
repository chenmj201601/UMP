using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using UMPS3105.Wcf11012;
using UMPS3105.Wcf31011;
using UMPS3105.Wcf31031;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3105
{
    /// <summary>
    /// TaskScoreMainView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskScoreMainView
    {

        #region members
        public static ObservableCollection<TaskInfoDetail> ListCurrentTaskDetail = new ObservableCollection<TaskInfoDetail>();
        public CCheckData SelTaskRecord;
        public AppealApprovalMainView PageParent;
        public BasicScoreSheetItem selScoreSheet;

        public string action;
        public string AppealFlowItemID;
        public string strDisContent;
        public CCheckData SelCheckData;

        private ObjectItem mRootObject;


        private List<BasicScoreCommentInfo> mListScoreCommentResults;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private List<BasicScoreItemInfo> mListScoreItemResults;
        private ScoreSheet mCurrentScoreSheet;
        private UCPlayBox mUCPlayBox;
        private UCRecordMemo mUCRecordMemo;
        /// <summary>
        /// ScoreType:0查看评分，1评分，2修改分数
        /// </summary>
        public int TaskScoreType;
        //异步线程
        private BackgroundWorker mWorker;

        private DateTime scoreTime;
        #endregion


        public TaskScoreMainView()
        {
            InitializeComponent();

            mRootObject = new ObjectItem();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListScoreItemResults = new List<BasicScoreItemInfo>();
            mListScoreCommentResults = new List<BasicScoreCommentInfo>();
            scoreTime = DateTime.Now;
            TvObjects.ItemsSource = mRootObject.Children;
        }


        /// <summary>
        /// 生成菜单
        /// </summary>
        private void CreateOperationButton()
        {
            WpOperationMenu.Children.Clear();
            AddOperationBtn("IsReturnBack", CurrentApp.GetLanguageInfo("3105T00036", "Return"));
            AddOperationBtn("CalculateScore", CurrentApp.GetLanguageInfo("3105T00038", "Calculate"));
            switch (TaskScoreType)
            {
                case 0:
                    break;
                case 1:
                    AddOperationBtn("SaveScore", CurrentApp.GetLanguageInfo("3105T00039", "Submit"));
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private void SetPageContentLanguage()
        {
            var panel = GetPanleByContentID("PanelPlayBox");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3105T00040", "Player");
            }
            panel = GetPanleByContentID("PanelScoreViewer");
            if (panel != null)
            {
                panel.Title = SelTaskRecord.RecoredReference.ToString();
            }
            panel = GetPanleByContentID("PanelMemo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3105T00041", "Remark");
            }
            panel = GetPanleByContentID("PanelTaskDetail");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3105T00042", "Task Detail");
            }
            PanelScoreViewer.Title = CurrentApp.GetLanguageInfo("3105T00043", "Score Task");
        }

        /// <summary>
        /// 生成单个按钮
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="strDiplay"></param>
        private void AddOperationBtn(string strCode, string strDiplay)
        {
            System.Windows.Controls.Button btnOperation = new System.Windows.Controls.Button();
            btnOperation.FontSize = 12;
            btnOperation.FontFamily = new FontFamily("SimSun");
            btnOperation.Height = 24;
            btnOperation.BorderThickness = new Thickness(0);
            btnOperation.Foreground = Brushes.Black;
            btnOperation.Margin = new Thickness(1, 1, 3, 1);
            btnOperation.Content = "  " + strDiplay + "  ";
            btnOperation.Tag = strCode;
            btnOperation.CommandParameter = strCode;
            if (strDiplay.Length > 0) { btnOperation.ToolTip = strDiplay; } else { btnOperation.ToolTip = strDiplay; }
            btnOperation.Click += new RoutedEventHandler(btnOperation_Click);
            WpOperationMenu.Children.Add(btnOperation);
        }

        /// <summary>
        /// 判断点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnOperation_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = e.Source as System.Windows.Controls.Button;
            if (btn == null) { return; }
            string strCode = btn.Tag.ToString();
            switch (strCode)
            {
                case "IsReturnBack":
                    //NavigationService.Navigate(new Uri("AppealApproval.xaml", UriKind.Relative));
                    AppealApprovalMainView approvalView = new AppealApprovalMainView();
                    approvalView.PageName = "AppealApprovalMainView";
                    var temp1 = CurrentApp as S3105App;
                    if (temp1 != null)
                    {
                        temp1.InitMainView(approvalView);
                    }
                    break;
                case "CalculateScore":
                    CalculateScore();
                    break;
                case "SaveScore":
                    SaveScoreInfo();
                    break;
                default:
                    break;
            }
        }

        private void CalculateScore()
        {
            try
            {
                if (mCurrentScoreSheet != null)
                {
                    var score = mCurrentScoreSheet.CaculateScore();
                    ShowInformation(CurrentApp.GetLanguageInfo("3105T00044", "Score") + ":" + score.ToString("0.00"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 加载评分表
        /// </summary>
        private void LoadTemplateByScoreType()
        {
            BorderTask.Height = 0;
            switch (TaskScoreType)
            {
                case 0://查看分数
                    break;
                case 1://评分
                    ScoreTaskRecord();
                    break;
                case 2://修改分数
                    break;
                default:
                    break;
            }
        }

        private void ScoreTaskRecord()
        {
            try
            {
                if (SelTaskRecord != null && selScoreSheet != null)
                {
                    mRootObject.ClearChildren();
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        LoadScoreItemResultInfo(selScoreSheet);
                        LoadLanguageInfos();
                        LoadScoreSheetData(selScoreSheet);

                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        SetBusy(false, string.Empty);

                        StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                        viewer.ViewMode = 0;
                        viewer.ScoreSheet = mCurrentScoreSheet;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = CurrentApp.Session.LangTypeID;
                        viewer.ViewClassic = mCurrentScoreSheet.ViewClassic;
                        BorderViewer.Child = viewer;
                    };
                    mWorker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveScoreInfo()
        {
            try
            {
                if (mCurrentScoreSheet == null) { return; }
                var score = mCurrentScoreSheet.CaculateScore();
                selScoreSheet.ScoreSheetInfo.Score = score;
                selScoreSheet.Score = selScoreSheet.ScoreSheetInfo.Score;
                selScoreSheet.ScoreSheetInfo.ScoreResultID = selScoreSheet.ScoreResultID;

                if (!SaveScoreSheetResult("7")) { return; }
                //SaveScoreDataResult();
                List<ScoreItem> listItems = new List<ScoreItem>();
                mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                for (int i = 0; i < listItems.Count; i++)
                {
                    var temp = mListScoreItemResults.FirstOrDefault(s => s.ScoreResultID == selScoreSheet.ScoreResultID
                                                                         &&
                                                                         s.ScoreSheetID == selScoreSheet.ScoreSheetID
                                                                         &&
                                                                         s.RecordSerialID ==
                                                                         selScoreSheet.RecordSerialID
                                                                         && s.UserID == selScoreSheet.UserID
                                                                         && s.ScoreItemID == listItems[i].ID);
                    if (temp == null)
                    {
                        temp = new BasicScoreItemInfo();
                        temp.ScoreResultID = selScoreSheet.ScoreResultID;
                        temp.ScoreSheetID = selScoreSheet.ScoreSheetID;
                        temp.RecordSerialID = selScoreSheet.RecordSerialID;
                        temp.UserID = selScoreSheet.UserID;
                        temp.ScoreItemID = listItems[i].ID;
                        mListScoreItemResults.Add(temp);
                    }
                    temp.Score = listItems[i].Score;
                    temp.RealScore = listItems[i].RealScore;
                }
                if (!SaveScoreItemResult())
                {
                    return;
                }
                if (!SaveScoreCommentResult())
                {
                    return;
                }
                else
                {
                    UpdateReCheckData();
                }
                #region 写操作日志

                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00037"), SelTaskRecord.RecoredReference);
                strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3105T00037"), score.ToString("0.00"));
                CurrentApp.WriteLog("TEST", string.Format("{0}", strLog));
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                UpdateTable19();
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Save Score info end"));
                //NavigationService.Navigate(new Uri("AppealApproval.xaml", UriKind.Relative));
                PageParent.ApprovalHistory();
                AppealApprovalMainView approvalView = new AppealApprovalMainView();
                approvalView.PageName = "AppealApprovalMainView";
                var temp1 = CurrentApp as S3105App;
                if (temp1 != null)
                {
                    temp1.InitMainView(approvalView);
                }

            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00037"), SelTaskRecord.RecoredReference);
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(ex.Message);
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
                webRequest.ListData.Add("6");//C009
                webRequest.ListData.Add("3");//C008
                webRequest.ListData.Add("Y");//C010
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

        /*
        private void SaveScoreDataResult()
        {
            try
            {
                //mListCurrentRecord
                if (!GetRecordInfoByRef(SelTaskRecord.RecoredReference.ToString()))
                    return;

                string orgID = "0";
                string agentID = "-1";
                string agentName;
                if (SelTaskRecord == null) { return; }

                agentName = SelTaskRecord.AgentName;
                agentID = SelTaskRecord.AgentID;

                var agentInfo = CurrentApp.ListCtrolAgentInfos.FirstOrDefault(a => a.AgentID == agentID);
                if (agentInfo != null)
                {
                    orgID = agentInfo.AgentOrgID;
                }

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.SaveScoreDataResult;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(selScoreSheet.ScoreSheetInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                optReturn = XMLHelper.SeriallizeObject(mCurrentScoreSheet);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                optReturn = XMLHelper.SeriallizeObject(mListCurrentRecord.RecordInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(orgID.ToString());
                webRequest.ListData.Add(agentID.ToString());
                webRequest.ListData.Add("7");//评分来源。编码待定义（1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分）
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    CurrentApp.WriteLog("SaveScoreDataResult Failed", string.Format("ListDataCount \t {0} \t agentID+orgID\t {1}", webRequest.ListData.Count(),agentID+"--"+orgID));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                CurrentApp.WriteLog("SaveScoreDataResult Failed  ex");
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
                webRequest.Code = (int)S3103Codes.GetRecordData;
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
                    if (webReturn.Message != S3103Consts.Err_TableNotExit)
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
         * */

        public bool UpdateReCheckData() //更行T_31_047申诉详情 跟T_31_041 的字段
        {
            bool ret = false;
            try
            {
                string opttime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3105Codes.SubmitCheckData;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelCheckData.AppealDetailID);//AppealDetailID       0
                webRequest.ListData.Add(SelCheckData.AppealFlowID);//AppealFlowID       1
                webRequest.ListData.Add(AppealFlowItemID);//AppealFlowItemID        2
                webRequest.ListData.Add(strDisContent);//Note       3
                webRequest.ListData.Add(action);//ActionID 5复核通过不修改分数，6复核通过重新评分，7复核驳回       4
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());//OperationerID      5
                webRequest.ListData.Add(opttime);//操作时间     6
                webRequest.ListData.Add(SelCheckData.ScoreResultID);//T_31_008成績IDC001      7
                webRequest.ListData.Add(SelCheckData.RecoredReference);//錄音流水號ID        8
                webRequest.ListData.Add(SelCheckData.ScoreType);//評分成績來源        9
                webRequest.ListData.Add(selScoreSheet.ScoreResultID.ToString());//新的評分成績ID        10
                webRequest.ListData.Add(Convert.ToInt32(DateTime.Now.Subtract(scoreTime).TotalSeconds).ToString());//评分花费时间 进入页面开始计算    11
                OperationReturn optReturn = XMLHelper.SeriallizeObject(mCurrentScoreSheet);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());//评分项    11
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
                    ShowException(string.Format("{0}.\t{1}\t{2}", CurrentApp.GetLanguageInfo("3105T00023", "Operation Field."), webReturn.Code, webReturn.Message));
                }
                else
                {
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString(actiondetail), SelCheckData.RecoredReference);
                    CurrentApp.WriteLog("TEST", string.Format("{0}", strLog));
                    CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    ret = true;
                    //ParentPage.BindData();
                    //ParentPage.ShowProcessReCheck(false);
                    //ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));

                    System.Threading.Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            { }

            return ret;
        }

        /// <summary>
        /// 评分
        /// </summary>1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分
        private bool SaveScoreSheetResult(string scoretype)
        {
            try
            {
                OperationReturn optReturn = XMLHelper.SeriallizeObject(selScoreSheet.ScoreSheetInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3105Codes.SaveScoreSheetResult;
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(scoretype);
                webRequest.ListData.Add(Convert.ToInt32(DateTime.Now.Subtract(scoreTime).TotalSeconds).ToString());//评分花费时间 进入页面开始计算
                Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31051"));
                //Service31051Client client = new Service31051Client();
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("SaveScoreSheetResult Failed", string.Format("ScoreInfo \t {0}", optReturn.Data.ToString()));
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                CurrentApp.WriteLog("SaveScoreSheetResult ", string.Format("ScoreInfo \t {0}", optReturn.Data.ToString()));
                selScoreSheet.ScoreResultID = Convert.ToInt64(webReturn.Data);
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private bool SaveScoreItemResult()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = 9;// (int)S3103Codes.SaveScoreItemResult;
                webRequest.ListData.Add(selScoreSheet.ScoreSheetID.ToString());
                webRequest.ListData.Add(selScoreSheet.ScoreResultID.ToString());
                webRequest.ListData.Add(mListScoreItemResults.Count.ToString());
                for (int i = 0; i < mListScoreItemResults.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(mListScoreItemResults[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private bool SaveScoreCommentResult()
        {
            try
            {
                if (selScoreSheet == null
                    || mCurrentScoreSheet == null)
                {
                    return false;
                }
                List<ScoreObject> listScoreObjects = new List<ScoreObject>();
                mCurrentScoreSheet.GetAllScoreObject(ref listScoreObjects);
                List<BasicScoreCommentInfo> listCommentResults = new List<BasicScoreCommentInfo>();
                for (int i = 0; i < listScoreObjects.Count; i++)
                {
                    var comment = listScoreObjects[i] as Comment;
                    if (comment == null) { continue; }
                    var scoreItem = comment.ScoreItem;
                    if (scoreItem == null) { continue; }
                    var temp =
                        mListScoreCommentResults.FirstOrDefault(s => s.ScoreResultID == selScoreSheet.ScoreResultID
                                                                     && s.ScoreSheetID == mCurrentScoreSheet.ID
                                                                     && s.ScoreItemID == scoreItem.ID
                                                                     && s.ScoreCommentID == comment.ID);
                    if (temp == null)
                    {
                        temp = new BasicScoreCommentInfo();
                        temp.ScoreResultID = selScoreSheet.ScoreResultID;
                        temp.ScoreSheetID = mCurrentScoreSheet.ID;
                        temp.ScoreItemID = scoreItem.ID;
                        temp.ScoreCommentID = comment.ID;
                    }
                    var itemComment = comment as ItemComment;
                    if (itemComment != null
                        && itemComment.SelectItem != null)
                    {
                        temp.CommentItemID = itemComment.SelectItem.ID;
                        temp.CommentItemOrderID = itemComment.SelectItem.OrderID;
                        temp.CommentText = itemComment.SelectItem.Text;
                        listCommentResults.Add(temp);
                    }
                    var textComment = comment as TextComment;
                    if (textComment != null)
                    {
                        temp.CommentText = textComment.Text;
                        listCommentResults.Add(temp);
                    }
                }
                int count = listCommentResults.Count;
                if (count <= 0)
                {
                    return true;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.SaveScoreCommentResultInfos;
                webRequest.ListData.Add(selScoreSheet.ScoreResultID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < listCommentResults.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(listCommentResults[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private void LoadScoreItemResultInfo(BasicScoreSheetItem item)
        {
            try
            {
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 10;// (int)S3103Codes.GetScoreResultList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.RecordSerialID.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(item.ScoreSheetID.ToString());
                webRequest.ListData.Add(item.ScoreResultID.ToString());
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
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
                mListScoreItemResults.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreItemInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreItemInfo info = optReturn.Data as BasicScoreItemInfo;
                    if (info == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    mListScoreItemResults.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadScoreCommentResultInfo(BasicScoreSheetItem item)
        {
            try
            {
                //加载评分备注结果信息
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetScoreCommentResultList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.ScoreResultID.ToString());
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
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
                mListScoreCommentResults.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreCommentInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreCommentInfo info = optReturn.Data as BasicScoreCommentInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBasicScoreCommentResultInfo is null"));
                        return;
                    }
                    mListScoreCommentResults.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitScoreCommentResult(ScoreItem scoreItem)
        {
            if (scoreItem == null) { return; }
            for (int i = 0; i < scoreItem.Comments.Count; i++)
            {
                var comment = scoreItem.Comments[i];
                var temp = mListScoreCommentResults.FirstOrDefault(s => s.ScoreCommentID == comment.ID);
                if (temp != null)
                {
                    var itemComment = comment as ItemComment;
                    if (itemComment != null)
                    {
                        var item = itemComment.ValueItems.FirstOrDefault(s => s.ID == temp.CommentItemID);
                        if (item != null)
                        {
                            itemComment.SelectItem = item;
                        }
                    }
                    var textComment = comment as TextComment;
                    if (textComment != null)
                    {
                        textComment.Text = temp.CommentText;
                    }
                    temp.ScoreItemID = scoreItem.ID;
                    if (scoreItem.ScoreSheet != null)
                    {
                        temp.ScoreSheetID = scoreItem.ScoreSheet.ID;
                    }
                }
            }
        }
        /// <summary>
        /// 播放
        /// </summary>
        private void PlayRecord()
        {
            if (SelTaskRecord != null)
            {
                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.RecoredReference = SelTaskRecord.RecoredReference;
                mUCPlayBox.IsAutoPlay = true;
                BorderPlayer.Child = mUCPlayBox;


                //mUCRecordMemo = new UCRecordMemo();
                //mUCRecordMemo.PageParent = this;
                //mUCRecordMemo.RecoredReference = SelTaskRecord.RecoredReference;
                //BorderMemo.Child = mUCRecordMemo;
            }
        }

        #region 加载评分表
        private void LoadScoreSheetData(BasicScoreSheetItem seltemplate)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 1;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(seltemplate.ScoreSheetID.ToString());
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return;
                }
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.Init();
                mCurrentScoreSheet = scoreSheet;
                //CreateScoreSheetItem(mRootObject, scoreSheet, true, true);
                //Dispatcher.Invoke(new Action(() =>
                //{
                //    CheckScoreSheetValid();
                //    if (mRootObject.Children.Count > 0)
                //    {
                //        mRootObject.Children[0].IsSelected = true;
                //    }
                //}));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion
        public void OpenPasswordPanel(UMPUserControl content)
        {
            try
            {
                PopupPanel.Content = content;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region Basic

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                DockingManagerMain.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        #endregion

        #region init
        /// <summary>
        /// 初始化用户信息以及样式
        /// </summary>
        protected override void Init()
        {
            try
            {
                PageName = "UMPTaskScoreFormPage";
                StylePath = "UMPS3105/MainPageStyle.xaml";

                base.Init();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();

                    ChangeTheme();
                    InitScoreSettings();
                    CreateOperationButton();
                    SetPageContentLanguage();
                    ShowObjectDetail();
                    LoadTemplateByScoreType();
                    PlayRecord();
                    SetBusy(false, string.Empty);

                    CurrentApp.SendLoadedMessage();//加载完各种消息发送登录消息给UMP
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitScoreSettings()
        {
            mListScoreSettings.Clear();

            #region 基本设定信息
            //默认显示风格
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "V_CLASSIC",
                Category = "S",
                Value = "Tree"
            });
            //标题图标是否显示
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "T_ICON_VIS",
                Category = "S",
                Value = "F"
            });
            //打分值的宽度
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "V_WIDTH",
                Category = "S",
                Value = "150"
            });
            //Tip的宽度
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "T_WIDTH",
                Category = "S",
                Value = "50"
            });
            #endregion
        }

        private void LoadLanguageInfos()
        {
            try
            {
                if (CurrentApp.Session == null || CurrentApp.Session.LangTypeInfo == null) { return; }
                mListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("31{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("3101{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                        return;
                    }
                    ScoreLangauge scoreLanguage = new ScoreLangauge();
                    scoreLanguage.LangID = langInfo.LangID;
                    scoreLanguage.Display = langInfo.Display;

                    string name = langInfo.Name;
                    if (name.StartsWith("OBJ301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("PRO301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("PROD301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101GRP301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101Designer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(12);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ToolBar"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(11);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ScoreViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(15);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101PropertyViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(18);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ObjectViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(16);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region  页头操作发消息

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                CreateOperationButton();

                #region panel
                SetPageContentLanguage();
                #endregion

                if (mUCRecordMemo != null)
                {
                    mUCRecordMemo.ChangeLanguage();
                }
                //详细信息
                ShowObjectDetail();
            }
            catch (Exception)
            {

                throw;
            }

        }


        private void ShowObjectDetail()
        {
            try
            {
                CCheckData item = SelTaskRecord;
                if (item == null) { return; }
                ObjectDetail.Title = item.RecoredReference;

                List<PropertyItem> listProperties = new List<PropertyItem>();
                PropertyItem property;
                property = new PropertyItem();
                property.Name = CurrentApp.GetLanguageInfo("3105T00001", "RecoredReference");
                property.ToolTip = property.Name;
                property.Value = item.RecoredReference;
                listProperties.Add(property);

                //property = new PropertyItem();
                //property.Name = CurrentApp.GetLanguageInfo("3105T00002", "AgentID");
                //property.ToolTip = property.Name;
                //property.Value = item.AgentID;
                //listProperties.Add(property);

                property = new PropertyItem();
                property.Name = CurrentApp.GetLanguageInfo("3105T00014", "AgentName");
                property.ToolTip = property.Name;
                property.Value = item.AgentName;
                listProperties.Add(property);

                property = new PropertyItem();
                property.Name = CurrentApp.GetLanguageInfo("3105T00030", "AppealDatetime");
                property.ToolTip = property.Name;
                property.Value = item.AppealDatetime;
                listProperties.Add(property);

                //property = new PropertyItem();
                //property.Name = CurrentApp.GetLanguageInfo("3105T00003", "AppealFlowID");
                //property.ToolTip = property.Name;
                //property.Value = item.AppealFlowID;
                //listProperties.Add(property);

                ObjectDetail.ItemsSource = listProperties;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception ex)
                {
                    // ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3105;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS3105;component/Themes/Default/UMPS3105/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
            try
            {
                string uri = string.Format("/UMPS3105;component/Themes/Default/UMPS3105/QMAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }

        #endregion

    }
}
