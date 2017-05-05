// ***********************************************************************
// Assembly         : UMPS3103
// Author           : Luoyihua
// Created          : 11-20-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 11-20-2014
// ***********************************************************************
// <copyright file="TaskScoreForm.xaml.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS3103.Codes;
using UMPS3103.Commands;
using UMPS3103.Converters;
using UMPS3103.Models;
using UMPS3103.Wcf11012;
using UMPS3103.Wcf31011;
using UMPS3103.Wcf31021;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.PropertyGrids;
using VoiceCyber.Wpf.PropertyGrids.Definitions;
using VoiceCyber.Wpf.PropertyGrids.Editors;

namespace UMPS3103
{
    /// <summary>
    /// TaskScoreForm.xaml 的交互逻辑
    /// </summary>
    public partial class TaskScoreForm
    {
        #region members
        public static ObservableCollection<TaskInfoDetail> ListCurrentTaskDetail = new ObservableCollection<TaskInfoDetail>();
        public TaskInfoDetail SelTaskRecord;
        public TaskRecordDetail PageParent;
        public BasicScoreSheetItem selScoreSheet;
        public List<ObjectItems> ListAllObjects;
        private RecordInfoItem mListCurrentRecord;

        private ObjectItem mRootObject;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private List<BasicScoreItemInfo> mListScoreItemResults;
        private ScoreSheet mCurrentScoreSheet;
        private DateTime mDtScore;
        private List<BasicScoreCommentInfo> mListScoreCommentResults;
        private ObservableCollection<CallInfoPropertyItem> mListCallInfoPropertyItems;

        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public Service03Helper Service03Helper;
        public List<RecordEncryptInfo> ListEncryptInfo;
        /// <summary>
        /// 是否允许评分，超级管理员只能查看qa的分数不能修改分数
        /// </summary>
        public Boolean isEnableScore=true;

        //异步线程
        private BackgroundWorker mWorker;
        #endregion
        public TaskScoreForm()
        {
            InitializeComponent();
            mDtScore = DateTime.Now;
            mRootObject = new ObjectItem();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListScoreItemResults = new List<BasicScoreItemInfo>();
            mListScoreCommentResults = new List<BasicScoreCommentInfo>();
            mListCallInfoPropertyItems = new ObservableCollection<CallInfoPropertyItem>();
            ListAllObjects = new List<ObjectItems>();
            mListCurrentRecord = new RecordInfoItem();
            mWorker = new BackgroundWorker();
            ListAllObjects = S3103App.mListAllObjects;
            ListBoxCallInfo.ItemsSource = mListCallInfoPropertyItems;
        }
        
        protected override void Init() //用了init()方法之后，不能在用this.load的方法了
        {
            try
            {
                PageName = "UMP TaskScore";
                StylePath = "UMPS3103/MainPageStyle.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                CreateOperationButton();
                InitScoreSettings();
                LoadLanguageInfos();
                InitTaskScoreTemplate();
                PlayRecord();
                SetPageContentLanguage();
                ShowObjectDetail();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, "");
                    ChangeLanguage();
                    ChangeTheme();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        
        /// <summary>
        /// 生成菜单
        /// </summary>
        private void CreateOperationButton()
        {
            WpOperationMenu.Children.Clear();
            AddOperationBtn("IsReturnBack", CurrentApp.GetLanguageInfo("3103T00024", "Return"));
            AddOperationBtn("CalculateScore", CurrentApp.GetLanguageInfo("3103T00045", "Calculate"));
            if (isEnableScore)
            {
                AddOperationBtn("SaveScore", CurrentApp.GetLanguageInfo("3103T00054", "Submit"));
            }
        }

        private void SetPageContentLanguage()
        {
            var panel = GetPanleByContentID("PanelPlayBox");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3103T00047", "Player");
            }
            panel = GetPanleByContentID("PanelScoreViewer");
            if (panel != null)
            {
                panel.Title = SelTaskRecord.RecoredReference.ToString();
            }
            //panel = GetPanleByContentID("PanelMemo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3103T00051", "Record Remark");
            }
            panel = GetPanleByContentID("PanelTaskDetail");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3103T00056", "Task Detail");
            }
            PanelScoreViewer.Title = CurrentApp.GetLanguageInfo("3103T00040", "Score Task");
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
                    var temp = CurrentApp as S3103App;
                    TaskRecordDetail taskDetailView = new TaskRecordDetail();
                    CurrentApp.CurrentView = taskDetailView;
                    taskDetailView.PageName = "TaskRecordDetail";
                    if (temp != null)
                    {
                        temp.InitMainView(taskDetailView);
                        //temp.InitMainView(0);
                    }
                    break;
                case "CalculateScore":
                    CalculateScore();
                    break;
                case "SaveScore":
                    WpOperationMenu.IsEnabled = false;//一旦提交成绩，按钮面板不可用，因为提交成功会直接关闭评分页面
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
                    var result = mCurrentScoreSheet.CheckInputValid();
                    if (result.Code != 0)
                    {
                       ShowException(string.Format("Check input valid fail.\t{0}", result.Code));
                        return;
                    }
                    var score = mCurrentScoreSheet.CaculateScore();
                    var viewer = BorderViewer.Child as StatisticalScoreSheetViewer;
                    if (viewer != null)
                    {
                        viewer.CaculateScore();
                    }
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00055", "Score") + ":" + score.ToString("0.00"));
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }

            //try
            //{
            //    if (mCurrentScoreSheet != null)
            //    {
            //        var score = mCurrentScoreSheet.CaculateScore();
            //        App.ShowInfoMessage(App.GetLanguageInfo("3103T00055", "Score") + ":" + score.ToString("0.00"));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    App.ShowExceptionMessage(ex.Message);
            //}
        }
        
        /// <summary>
        /// 加载评分表
        /// </summary>
        private void InitTaskScoreTemplate()
        {
            try
            {
                if (SelTaskRecord != null && selScoreSheet != null)
                {
                    mRootObject.ClearChildren();
                    if (!PageParent.mViewScore)//没有在任务中评分的记录--
                    {
                        LoadScoreSheetInfo(selScoreSheet);
                        if (mCurrentScoreSheet == null) { return; }
                        StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                        viewer.ViewMode = 0;
                        viewer.ScoreSheet = mCurrentScoreSheet;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = CurrentApp.Session.LangTypeID;
                        viewer.ViewClassic = mCurrentScoreSheet.ViewClassic;
                        BorderViewer.Child = viewer;
                    }
                    else//在任务中评过分的记录
                    {
                        LoadScoreSheetInfo(selScoreSheet);
                        LoadScoreItemResultInfo(selScoreSheet);
                        LoadScoreCommentResultInfo(selScoreSheet);
                        if (mCurrentScoreSheet == null) { return; }
                        InitScoreCommentResult(mCurrentScoreSheet);
                        List<ScoreItem> listItems = new List<ScoreItem>();
                        mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                        for (int i = 0; i < listItems.Count; i++)
                        {
                            ScoreItem scoreItem = listItems[i];
                            var temp = mListScoreItemResults.FirstOrDefault(p => p.ScoreResultID == selScoreSheet.ScoreResultID
                                                                             && p.ScoreSheetID == selScoreSheet.ScoreSheetID
                                                                             && p.ScoreItemID == scoreItem.ID);
                            if (temp != null)
                            {
                                scoreItem.Score = temp.Score;
                            }
                            InitScoreCommentResult(scoreItem);
                        }
                        StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                        viewer.ViewMode = 1;
                        viewer.ScoreSheet = mCurrentScoreSheet;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = CurrentApp.Session.LangTypeID;
                        viewer.ViewClassic = mCurrentScoreSheet.ViewClassic;
                        BorderViewer.Child = viewer;
                    }
                }
                else
                {
                    ShowInformation(string.Format("SelTaskRecord or selScoreSheet is null"));
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void SaveScoreInfo()
        {
            try
            {
                if (mCurrentScoreSheet == null) 
                {
                    WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
                    return; 
                }
                double tempScoreTime = DateTime.Now.Subtract(mDtScore).TotalSeconds;//评分时间 进入页面开始计算
                string totalSec = Convert.ToInt32(tempScoreTime).ToString();

                //1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分 8申诉审批分配到复检
                int scoreType=0;
                //判斷任務類型
                if (SelTaskRecord.TaskType == "1" || SelTaskRecord.TaskType =="2")//任務來源  初檢 
                {
                    scoreType = 4;
                }
                if (SelTaskRecord.TaskType == "3" || SelTaskRecord.TaskType == "4")//任務來源  複檢
                {
                    scoreType = 6;
                }

                var score = mCurrentScoreSheet.CaculateScore();
                selScoreSheet.ScoreSheetInfo.Score = score;
                selScoreSheet.Score = selScoreSheet.ScoreSheetInfo.Score;
                if (PageParent.mViewScore)
                {
                    selScoreSheet.ScoreSheetInfo.OldScoreResultID = 1;//新增分数说明是修改分数
                    if (!SaveScoreSheetResult(scoreType, totalSec))
                    {
                        WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
                        return; 
                    }
                }
                else
                {
                    if (!SaveScoreSheetResult(scoreType - 1, totalSec)) 
                    { 
                        WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
                        return; 
                    }
                }
                SaveScoreDataResult();
                List<ScoreItem> listItems = new List<ScoreItem>();
                mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                mListScoreItemResults.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    var temp = mListScoreItemResults.FirstOrDefault(s => s.ScoreResultID == selScoreSheet.ScoreResultID
                                                                         && s.ScoreSheetID == selScoreSheet.ScoreSheetID
                                                                         &&  s.RecordSerialID == selScoreSheet.RecordSerialID
                                                                         && s.UserID == selScoreSheet.UserID
                                                                         && s.ScoreItemID == listItems[i].ID);
                    if (temp == null)
                    {
                        temp=new BasicScoreItemInfo();
                        temp.ScoreResultID = selScoreSheet.ScoreResultID;
                        temp.ScoreSheetID = selScoreSheet.ScoreSheetID;
                        temp.RecordSerialID = selScoreSheet.RecordSerialID;
                        temp.UserID = selScoreSheet.UserID;
                        temp.ScoreItemID = listItems[i].ID;
                        temp.IsNA = listItems[i].IsNA ? "Y" : "N";
                        mListScoreItemResults.Add(temp);
                    }
                    temp.Score = listItems[i].Score;
                    temp.RealScore = listItems[i].RealScore;
                }
                if (!SaveScoreItemResult())
                {
                    WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
                    return;
                }
                if (!SaveScoreCommentResult())
                {
                    WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
                    return;
                }
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00042", "Operation Succeed."));
                var tempview = CurrentApp as S3103App;
                TaskRecordDetail taskDetailView = new TaskRecordDetail();
                CurrentApp.CurrentView = taskDetailView;
                taskDetailView.PageName = "TaskRecordDetail";
                if (tempview != null)
                {
                    tempview.InitMainView(taskDetailView);
                    //temp.InitMainView(0);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message); 
                WpOperationMenu.IsEnabled = true;//如果提交过程中报错，应该把按钮面板放出来
            }
        }

        /// <summary>
        /// 评分
        /// </summary>
        /// <param name="scoretype">评分类型：3初检评分,4初检修改得分</param>
        /// <param name="totalSec">评分花费时间</param>
        private bool SaveScoreSheetResult(int scoretype, string totalSec)
        {
            try
            {
                if (!GetRecordInfoByRef(SelTaskRecord.RecoredReference.ToString()))
                    return false;

                if (SelTaskRecord == null) { return false; }
                long orgID = ConstValue.ORG_ROOT;
                var agentInfo = S3103App.mListAuInfoItems.FirstOrDefault(a => a.ID.ToString() == SelTaskRecord.AgtOrExtID);
                if (agentInfo != null)
                {
                    orgID = agentInfo.OrgID;
                }
                selScoreSheet.ScoreSheetInfo.OrgID = orgID;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(selScoreSheet.ScoreSheetInfo);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.SaveScoreSheetResult;
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(scoretype.ToString());
                webRequest.ListData.Add(SelTaskRecord.TaskID.ToString());
                webRequest.ListData.Add(totalSec);
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                if (Int32.Parse(webReturn.ListData[0]) < 2 && selScoreSheet.ScoreResultID != Convert.ToInt64(webReturn.Data))//1、更新任务表操作完成,   2、修改成绩表操作完成, 3、更新评分表次数完成
                {
                    WebRequest webRequest1 = new WebRequest();
                    webRequest1.Session = CurrentApp.Session;
                    webRequest1.Code = (int)S3103Codes.DeleteErrorDB;
                    webRequest1.ListData.Add(webReturn.Data);
                    Service31031Client client1 = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn1 = client.UMPTaskOperation(webRequest);
                    client.Close(); 
                    if (!webReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    return false;
                }
                selScoreSheet.ScoreResultID = Convert.ToInt64(webReturn.Data);
                selScoreSheet.ScoreSheetInfo.ScoreResultID = selScoreSheet.ScoreResultID;
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
                webRequest.Code = (int)S3103Codes.SaveScoreItemResult;
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
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKRECORDSCORE.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00055"), mCurrentScoreSheet.CaculateScore().ToString("0.00"));
                strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), selScoreSheet.RecordSerialID.ToString());
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKRECORDSCORE.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                return true;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return false;
            }
        }
        private void LoadScoreSheetInfo(BasicScoreSheetItem item)
        {
            try
            {
                if (item == null) 
                {
                   ShowException(string.Format("Fail.\tselScoreSheet.ScoreSheetID is null"));
                    return; 
                }
                string scoreSheetID = item.ScoreSheetID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 15;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(scoreSheetID);
                webRequest.ListData.Add(item.ScoreResultID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                //WebHelper.SetServiceClient(client);
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
                   ShowException(string.Format("Fail.\tScoreSheet is null"));
                    return;
                }
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.Init();
                mCurrentScoreSheet = scoreSheet;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void LoadScoreItemResultInfo(BasicScoreSheetItem item)
        {
            try
            {
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetScoreResultList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.RecordSerialID.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(item.ScoreSheetID.ToString());
                webRequest.ListData.Add(selScoreSheet.ScoreResultID.ToString());
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //Service31031Client client = new Service31031Client();
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                   ShowException(CurrentApp.GetLanguageInfo("3103T00153", "Score Error,Please check scoreitem and submit"));
                    return;
                }
                if (webReturn.ListData == null)
                {
                   ShowException(string.Format("Fail.\tListData is null"));
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
                       ShowException(string.Format("Fail.\tBasicScoreItemInfo is null"));
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

        private void SaveScoreDataResult()
        {
            try
            {
                //mListCurrentRecord
                if (!GetRecordInfoByRef(SelTaskRecord.RecoredReference.ToString()))
                    return;

                long orgID = ConstValue.ORG_ROOT;
                string agentID = "-1";
                string agentName;
                if (SelTaskRecord == null) { return; }
                //var cc = App.ListCtrolOrgInfos;

                agentName = SelTaskRecord.AgtOrExtName;
                agentID = SelTaskRecord.AgtOrExtID;

                //2016-06-02 15:42:17 修改   现在在GetAuInfoList(）方法里增加了获取机构的方法，所以往父级遍历机构的方法就退休了
                //var agentInfo = S3103App.mListAllObjects.FirstOrDefault(a => a.ObjType == ConstValue.RESOURCE_AGENT && a.Name == agentName);
                //if (agentInfo != null)
                //{
                //    var orgInfo = agentInfo.Parent as ObjectItems;
                //    if (orgInfo != null)
                //    {
                //        if (orgInfo.ObjType == ConstValue.RESOURCE_ORG)
                //        {
                //            orgID = orgInfo.ObjID != null ? orgInfo.ObjID : ConstValue.ORG_ROOT;
                //        }
                //    }
                //}
                var agentInfo = S3103App.mListAuInfoItems.FirstOrDefault(a => a.ID.ToString() == agentID);
                if (agentInfo != null)
                {
                    orgID = agentInfo.OrgID;
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
                if(SelTaskRecord.TaskType=="1" || SelTaskRecord.TaskType=="2")//初检
                {
                    webRequest.ListData.Add("3");//评分来源。编码待定义（1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分）
                }
                else if (SelTaskRecord.TaskType == "3" || SelTaskRecord.TaskType == "4")//复检
                {
                    webRequest.ListData.Add("5");//评分来源。编码待定义（1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分）
                }
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(Convert.ToInt32(DateTime.Now.Subtract(mDtScore).TotalSeconds).ToString());//评分花费时间 进入页面开始计算
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
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

                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
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

        /// <summary>
        /// 播放
        /// </summary>
        private void PlayRecord()
        {
            if (SelTaskRecord != null)
            {
                //mUCPlayBox = new UCPlayBox();
                VoicePlayBox.TaskPage2 = this;
                //VoicePlayBox.ListEncryptInfo = App.mListRecordEncryptInfos;
                ////mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                //VoicePlayBox.ListSftpServers = App.mListSftpServers;
                //VoicePlayBox.ListDownloadParams = App.mListDownloadParams;
                //VoicePlayBox.Service03Helper = App.mService03Helper;
                VoicePlayBox.RecordInfoItem = PageParent.recorditem;
                VoicePlayBox.CurrentApp = CurrentApp;
                //VoicePlayBox.IsAutoPlay = true;
                VoicePlayBox.Play(true);
                //BorderPlayer.Child = mUCPlayBox;

                //mUCRecordMemo = new UCRecordMemo();
                //mUCRecordMemo.PageParent = this;
                //mUCRecordMemo.RecordInfoItem = SelTaskRecord;
                //BorderMemo.Child = mUCRecordMemo;
                
            }
        }


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
                //WebHelper.SetServiceClient(client);
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
                   ShowException(string.Format("ScoreSheet is null"));
                    return;
                }
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.Init();
                mCurrentScoreSheet = scoreSheet;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        #endregion


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
                        
        #region Init

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
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                //WebHelper.SetServiceClient(client);
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
                       ShowException(string.Format("LanguageInfo is null"));
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

        
        #region other
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                CreateOperationButton();

                #region panel
                SetPageContentLanguage();
                #endregion

                //详细信息
                ShowObjectDetail();
            }
            catch { }
        }
        
        private void ShowObjectDetail()
        {
            try
            {
                TaskInfoDetail taskItem = SelTaskRecord;
                if (taskItem == null) { return; }

                mListCallInfoPropertyItems.Clear();
                CallInfoPropertyItem item = new CallInfoPropertyItem();
                item.Value = taskItem.TaskName;
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00025", "Record Reference");
                item.Value = taskItem.RecoredReference.ToString();
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                string temp = string.Empty;
                temp = S3103App.GroupingWay.Contains("A") ? "3103T00185" : "3103T00186";
                item.Name = CurrentApp.GetLanguageInfo(temp, "AgentID Or ExtID");
                item.Value = taskItem.AgtOrExtName.ToString();
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                temp = S3103App.GroupingWay.Contains("A") ? "3103T00187" : "3103T00188";
                item.Name = CurrentApp.GetLanguageInfo(temp, "AgentFullName");
                item.Value = taskItem.AgentFullName;
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00068", "CallerID");
                item.Value = S3103App.DecryptString(taskItem.CallerID);
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00069", "CalledID");
                item.Value = S3103App.DecryptString(taskItem.CalledID);
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00059", "RecordStartTime");
                item.Value = taskItem.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00066", "RecordDuration");
                item.Value = taskItem.Duration;
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00029", "AllotType");
                item.Value = PageParent.GetAllotTypeName(taskItem.AllotType);
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00030", "FromTaskName");
                item.Value = taskItem.FromTaskName;
                mListCallInfoPropertyItems.Add(item);
                item = new CallInfoPropertyItem();
                item.Name = CurrentApp.GetLanguageInfo("3103T00055", "Score");
                item.Value = taskItem.TaskScore == null ? "" : taskItem.TaskScore.ToString();
                mListCallInfoPropertyItems.Add(item);
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
                    string uri = string.Format("/UMPS3103;component/Themes/{0}/{1}",
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/MainPageStatic.xaml");
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/QMAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }

        public void SetBusy(bool flag)
        {
            if (flag)
            {
                SetBusy(true, "");
            }
            else
            {
                SetBusy(false, "");
            }
        }

        #endregion
    }
}
