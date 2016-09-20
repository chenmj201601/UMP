//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    51371dc6-ae91-412d-af34-c7da40995838
//        CLR Version:              4.0.30319.18444
//        Name:                     UCScoreDetail
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCScoreDetail
//
//        created by Charley at 2014/11/18 10:46:15
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMPS3102.Codes;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;

namespace UMPS3102
{
    /// <summary>
    /// UCScoreDetail.xaml 的交互逻辑
    /// </summary>
    public partial class UCScoreDetail
    {
        public QMMainView PageParent;
        public BasicScoreSheetItem ScoreSheetItem;
        public RecordInfoItem RecordInfoItem;
        public bool IsAddScore;
        public List<ObjectItem> ListAllObjects;
        public List<SettingInfo> ListUserSettingInfos;
        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public Service03Helper Service03Helper;
        public List<RecordEncryptInfo> ListEncryptInfo;

        private ScoreSheet mCurrentScoreSheet;
        private List<BasicScoreItemInfo> mListScoreItemResults;
        private List<BasicScoreCommentInfo> mListScoreCommentResults;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;

        private DateTime mScoreStartTime;

        public UCScoreDetail()
        {
            InitializeComponent();

            mListScoreItemResults = new List<BasicScoreItemInfo>();
            mListScoreCommentResults = new List<BasicScoreCommentInfo>();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            Loaded += UCScoreDetail_Loaded;
        }

        void UCScoreDetail_Loaded(object sender, RoutedEventArgs e)
        {
            VoicePlayBox.CurrentApp = CurrentApp;
            CreateScoreDetailButtons();
            InitScoreSettings();
            LoadLanguageInfos();
            //LoadScoreSheetParams();
            Init();
            mScoreStartTime = DateTime.Now;
            PlayRecord();
        }

        public void Close()
        {
            try
            {
                VoicePlayBox.Close();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Init()
        {
            if (ScoreSheetItem == null) { return; }
            if (IsAddScore)
            {
                LoadScoreSheetInfo(ScoreSheetItem);
                if (mCurrentScoreSheet == null) { return; }
                TheWayYouAre();
                StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                viewer.ViewMode = 0;
                viewer.ScoreSheet = mCurrentScoreSheet;
                viewer.Settings = mListScoreSettings;
                viewer.Languages = mListLanguageInfos;
                viewer.LangID = CurrentApp.Session.LangTypeID;
                viewer.ViewClassic = mCurrentScoreSheet.ViewClassic;
                BorderScoreSheetViewer.Child = viewer;
            }
            else
            {
                LoadScoreSheetInfo(ScoreSheetItem);
                LoadScoreItemResultInfo(ScoreSheetItem);
                LoadScoreCommentResultInfo(ScoreSheetItem);
                if (mCurrentScoreSheet == null) { return; }
                TheWayYouAre();
                InitScoreCommentResult(mCurrentScoreSheet);
                List<ScoreItem> listItems = new List<ScoreItem>();
                mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                for (int i = 0; i < listItems.Count; i++)
                {
                    ScoreItem scoreItem = listItems[i];
                    var temp = mListScoreItemResults.FirstOrDefault(s => s.ScoreResultID == ScoreSheetItem.ScoreResultID
                                                                     && s.ScoreSheetID == ScoreSheetItem.ScoreSheetID
                                                                     && s.ScoreItemID == scoreItem.ID);
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
                BorderScoreSheetViewer.Child = viewer;
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

        private void LoadScoreSheetInfo(BasicScoreSheetItem item)
        {
            try
            {
                if (item == null) { return; }
                string scoreSheetID = item.ScoreSheetID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetScoreSheetInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(scoreSheetID);
                webRequest.ListData.Add(item.ScoreResultID.ToString());//评分成绩ID T_31_008.c001
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
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
                //加载评分项成绩信息
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetScoreResultList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.ScoreResultID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                webRequest.Code = (int)S3102Codes.GetScoreCommentResultList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(item.ScoreResultID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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

        private void SaveScoreInfo()
        {
            try
            {
                DateTime scoreStopTime = DateTime.Now;
                double aaa = (scoreStopTime - mScoreStartTime).TotalSeconds;
                //MessageBox.Show(aaa.ToString());
                ScoreSheetItem.ScoreSheetInfo.WasteTime = aaa;
                //MessageBox.Show("sss");
                if (mCurrentScoreSheet == null) { return; }
                mCurrentScoreSheet.CaculateScore();
                ScoreSheetItem.ScoreSheetInfo.Score = mCurrentScoreSheet.Score;
                ScoreSheetItem.Score = ScoreSheetItem.ScoreSheetInfo.Score;
                if (!SaveScoreSheetResult()) { return; }
                SaveScoreDataResult();
                List<ScoreItem> listItems = new List<ScoreItem>();
                mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                for (int i = 0; i < listItems.Count; i++)
                {
                    var temp = mListScoreItemResults.FirstOrDefault(s => s.ScoreResultID == ScoreSheetItem.ScoreResultID
                                                                         &&
                                                                         s.ScoreSheetID == ScoreSheetItem.ScoreSheetID
                                                                         && s.ScoreItemID == listItems[i].ID);
                    if (temp == null)
                    {
                        temp = new BasicScoreItemInfo();
                        temp.ScoreResultID = ScoreSheetItem.ScoreResultID;
                        temp.ScoreSheetID = ScoreSheetItem.ScoreSheetID;
                        temp.ScoreItemID = listItems[i].ID;
                        temp.IsNA = listItems[i].IsNA ? "Y" : "N";
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

                #region 写操作日志

                string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102001RecordReference"), ScoreSheetItem.RecordSerialID);
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102003Title"), ScoreSheetItem.Title);
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102003Score"), ScoreSheetItem.Score);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_SCORERECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion
                if (RecordInfoItem != null)
                {
                    RecordInfoItem.IsScored = 1;
                    RecordInfoItem.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", RecordInfoItem.IsScored), RecordInfoItem.IsScored.ToString());
                }

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("005", "Save Score info end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool SaveScoreSheetResult()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveScoreSheetResult;
                if (IsAddScore)
                {
                    ScoreSheetItem.ScoreSheetInfo.Flag = 1;
                }
                else
                {
                    ScoreSheetItem.ScoreSheetInfo.Flag = 2;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(ScoreSheetItem.ScoreSheetInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                //保存成绩之后当前成绩ID变为老成绩ID，而新生成的成绩ID为当前的成绩ID
                ScoreSheetItem.OldScoreResultID = ScoreSheetItem.ScoreResultID;
                ScoreSheetItem.ScoreResultID = Convert.ToInt64(webReturn.Data);
                ScoreSheetItem.ScoreSheetInfo.OldScoreResultID = ScoreSheetItem.OldScoreResultID;
                ScoreSheetItem.ScoreSheetInfo.ScoreResultID = ScoreSheetItem.ScoreResultID;
                //更新子项的成绩ID
                for (int i = 0; i < mListScoreItemResults.Count; i++)
                {
                    mListScoreItemResults[i].ScoreResultID = ScoreSheetItem.ScoreResultID;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private void SaveScoreDataResult()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveScoreDataResult;
                if (IsAddScore)
                {
                    ScoreSheetItem.ScoreSheetInfo.Flag = 1;
                }
                else
                {
                    ScoreSheetItem.ScoreSheetInfo.Flag = 2;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(ScoreSheetItem.ScoreSheetInfo);
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
                if (RecordInfoItem == null) { return; }
                optReturn = XMLHelper.SeriallizeObject(RecordInfoItem.RecordInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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

        private bool SaveScoreItemResult()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveScoreItemResult;
                webRequest.ListData.Add(ScoreSheetItem.ScoreSheetID.ToString());
                webRequest.ListData.Add(ScoreSheetItem.ScoreResultID.ToString());
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
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                if (ScoreSheetItem == null
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
                        mListScoreCommentResults.FirstOrDefault(s => s.ScoreResultID == ScoreSheetItem.ScoreResultID
                                                                     && s.ScoreSheetID == mCurrentScoreSheet.ID
                                                                     && s.ScoreItemID == scoreItem.ID
                                                                     && s.ScoreCommentID == comment.ID);
                    if (temp == null)
                    {
                        temp = new BasicScoreCommentInfo();
                        temp.ScoreResultID = ScoreSheetItem.ScoreResultID;
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
                webRequest.Code = (int)S3102Codes.SaveScoreCommentResultInfos;
                webRequest.ListData.Add(ScoreSheetItem.ScoreResultID.ToString());
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
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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

        private void CreateScoreDetailButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "BT" + "Caculate";
                item.Display = CurrentApp.GetLanguageInfo("3102B015", "Caculate");
                item.Tip = CurrentApp.GetLanguageInfo("3102B015", "Caculate");
                item.Icon = "Images/calculator.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "SaveScore";
                item.Display = CurrentApp.GetLanguageInfo("3102B012", "SaveScore");
                item.Tip = CurrentApp.GetLanguageInfo("3102B012", "SaveScore");
                item.Icon = "Images/save.png";
                listBtns.Add(item);

                PanelScoreDetailButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    ToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += ToolButton_Click;
                    PanelScoreDetailButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var toolBtn = e.Source as Button;
            if (toolBtn != null)
            {
                ToolButtonItem item = toolBtn.DataContext as ToolButtonItem;
                if (item != null)
                {
                    switch (item.Name)
                    {
                        case "BTCaculate":
                            CaculateScore();
                            break;
                        case "BTSaveScore":
                            SaveScoreInfo();
                            if (PageParent != null)
                            {
                                PageParent.ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                                PageParent.ReloadScoreSheetList();
                            }
                            break;
                    }
                }
            }
        }

        private void CaculateScore()
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
                    var viewer = BorderScoreSheetViewer.Child as StatisticalScoreSheetViewer;
                    if (viewer != null)
                    {
                        viewer.CaculateScore();
                    }
                    CurrentApp.ShowInfoMessage(string.Format("{0}\t{1}",
                        CurrentApp.GetMessageLanguageInfo("010", "Caculate score end."),
                        score.ToString("0.00")));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PlayRecord()
        {
            if (RecordInfoItem != null)
            {
                try
                {
                    VoicePlayBox.MainPage = PageParent;
                    VoicePlayBox.ListSftpServers = ListSftpServers;
                    VoicePlayBox.ListUserSettingInfos = ListUserSettingInfos;
                    VoicePlayBox.ListDownloadParams = ListDownloadParams;
                    VoicePlayBox.Service03Helper = Service03Helper;
                    VoicePlayBox.ListEncryptInfo = ListEncryptInfo;
                    VoicePlayBox.RecordInfoItem = RecordInfoItem;
                    VoicePlayBox.IsAutoPlay = true;
                    //VoicePlayBox.Play(true);      //由于是自动播放，无需再调用Play方法
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        //自动评分
        private void TheWayYouAre()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetAutoScore;
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                List<Standard> listScoreObjectTemp = new List<Standard>();
                mCurrentScoreSheet.GetAllStandards(ref listScoreObjectTemp);
                //Standard standard = null;
                for (int i = 0; i < listScoreObjectTemp.Count; i++)
                {
                    if (listScoreObjectTemp[i].IsAutoStandard == true)
                    {
                        webRequest.ListData.Add(listScoreObjectTemp[i].StatisticalID.ToString());
                    }
                }
                if (webRequest.ListData.Count < 2)
                {
                    return;
                }
                //Service31021Client client = new Service31021Client();
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strGrade = arrInfo[0];
                    string strStatisticID = arrInfo[1];
                    //AutoScoreToUI(arrInfo[0], arrInfo[1]);
                    //ScoreItem temp = mCurrentScoreSheet.Items.FirstOrDefault(o => o..HasAutoStandard == true);
                    List<Standard> listScoreObjectTemp_ = new List<Standard>();
                    mCurrentScoreSheet.GetAllStandards(ref listScoreObjectTemp_);
                    for (int n = 0; n < listScoreObjectTemp_.Count; n++)
                    {
                        if (strStatisticID == listScoreObjectTemp_[n].StatisticalID.ToString())
                        {
                            if (strGrade == "1")
                            {
                                listScoreObjectTemp_[n].Score = 0;
                            }
                            if (strGrade == "2")
                            {
                                listScoreObjectTemp_[n].Score = listScoreObjectTemp_[n].TotalScore;
                            }
                            if (strGrade == "N/A")
                            {
                                listScoreObjectTemp_[n].IsNA = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





    }
}
