using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using UMPS3104.Commands;
using UMPS3104.Models;
using UMPS3104.S3102Codes;
using UMPS3104.Wcf11012;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Common31041.Common3102;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;

namespace UMPS3104
{
    /// <summary>
    /// UCScoreDetail.xaml 的交互逻辑
    /// </summary>
    public partial class UCScoreDetail
    {
        public AgentIntelligentClient PageParent;
        public RecordInfoItem RecordInfoItem;
        public RecordScoreInfoItem aScoreInfoItem;
        public List<ObjectItemClient> ListAllObjects;

        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public Service03Helper Service03Helper;
        public List<RecordEncryptInfo> ListEncryptInfo;


        private ScoreSheet mCurrentScoreSheet;
        private List<BasicScoreItemInfo> mListScoreItemResults;
        private List<ScoreSetting> mListScoreSettings;
        private UCPlayBox mPlayBox;
        private List<ScoreLangauge> mListLanguageInfos;
        
        private List<BasicScoreCommentInfo> mListScoreCommentResults;
        public UCScoreDetail()
        {
            InitializeComponent();

            mListScoreSettings = new List<ScoreSetting>();
            mListScoreItemResults = new List<BasicScoreItemInfo>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListScoreCommentResults = new List<BasicScoreCommentInfo>();

            Loaded += UCScoreDetail_Loaded;
        }

        private void UCScoreDetail_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PageParent.MyWaiter.Visibility = Visibility.Visible;
                BackgroundWorker mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    Thread.Sleep(500);
                    InitScoreSettings();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    PageParent.MyWaiter.Visibility = Visibility.Collapsed;
                    LoadLanguageInfos();
                    Init();
                    PlayRecord();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception)
            {

            }

        }

        private void InitScoreSettings()
        {
            try
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
            catch (Exception)
            {

            }
        }

        private void Init()
        {
            try
            {
                if (aScoreInfoItem == null) { return; }
                PageParent.SetBusy(true, string.Format("Get the ScoreResultList....."));
                BackgroundWorker mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    LoadScoreSheetInfo(aScoreInfoItem);
                    LoadScoreItemResultInfo(aScoreInfoItem);
                    LoadScoreCommentResultInfo(aScoreInfoItem);

                };
                mWorker.RunWorkerCompleted += (w, re) =>
                {
                    mWorker.Dispose();
                    PageParent.SetBusy(false, "");
                    if (mCurrentScoreSheet == null) { return; }
                    InitScoreCommentResult(mCurrentScoreSheet);
                    List<ScoreItem> listItems = new List<ScoreItem>();
                    mCurrentScoreSheet.GetAllScoreItem(ref listItems);
                    WriteLog.WriteLogToFile("ScoreInfo \t listItems.Count", listItems.Count.ToString());
                    for (int i = 0; i < listItems.Count; i++)
                    {
                        ScoreItem scoreItem = listItems[i];
                        var temp = mListScoreItemResults.FirstOrDefault(s => s.ScoreResultID == aScoreInfoItem.ScoreID
                                                                         && s.ScoreSheetID == aScoreInfoItem.TemplateID
                                                                         && s.ScoreItemID == scoreItem.ID);
                        try
                        {
                            WriteLog.WriteLogToFile(string.Format("ScoreInfo \t listItems{0} \t ScoreResultID", i), mListScoreItemResults[i].ScoreResultID + "\t" + aScoreInfoItem.ScoreID);
                            WriteLog.WriteLogToFile(string.Format("ScoreInfo \t listItems{0} \t ScoreSheetID", i), mListScoreItemResults[i].ScoreSheetID + "\t" + aScoreInfoItem.TemplateID);
                            WriteLog.WriteLogToFile(string.Format("ScoreInfo \t listItems{0} \t ScoreItemID", i), mListScoreItemResults[i].ScoreItemID + "\t" + scoreItem.ID);
                        }
                        catch { }
                        if (temp != null)
                        {
                            scoreItem.Score = temp.Score;
                        }
                        InitScoreCommentResult(scoreItem);
                    }

                    StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                    viewer.ViewMode = 2;//0   评分模式/// 1   修改模式/// 2   查看模式
                    viewer.ScoreSheet = mCurrentScoreSheet;
                    viewer.Settings = mListScoreSettings;
                    viewer.Languages = mListLanguageInfos;
                    viewer.LangID = App.Session.LangTypeID;
                    viewer.ViewClassic = mCurrentScoreSheet.ViewClassic;
                    BorderScoreSheetViewer.Child = viewer;
                };
                mWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogToFile("(ಥ_ಥ)", ex.Message + "\t" + ex.StackTrace);
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadScoreSheetInfo(RecordScoreInfoItem item)
        {
            try
            {
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.GetScoreSheetInfo;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(item.TemplateID.ToString());
                webRequest.ListData.Add(item.ScoreID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client = new Service31041Client();
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00127", string.Format("Fail.\tScoreSheet is null")));
                    return;
                }
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.Init();
                mCurrentScoreSheet = scoreSheet;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadScoreItemResultInfo(RecordScoreInfoItem item)
        {
            try
            {
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.GetScoreResultList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(item.ScoreID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                WriteLog.WriteLogToFile("ScoreInfo \t StrQuery", webReturn.Message);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail.\tListData is null")));
                    return;
                }
                mListScoreItemResults.Clear();
                WriteLog.WriteLogToFile("ScoreInfo \t GetScoreResultList 0", webReturn.ListData.Count.ToString());
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    WriteLog.WriteLogToFile("ScoreInfo \t GetScoreResultList 1", strInfo);
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreItemInfo>(strInfo);
                    WriteLog.WriteLogToFile("ScoreInfo \t GetScoreResultList 2", optReturn.StringValue);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreItemInfo info = optReturn.Data as BasicScoreItemInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail.\tBasicScoreItemInfo is null")));
                        return;
                    }
                    mListScoreItemResults.Add(info);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        //加载评分备注结果信息
        private void LoadScoreCommentResultInfo(RecordScoreInfoItem item)
        {
            try
            {
                if (item == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.GetScoreCommentResultList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(item.ScoreID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session),WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListScoreCommentResults.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreCommentInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreCommentInfo info = optReturn.Data as BasicScoreCommentInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tBasicScoreCommentResultInfo is null"));
                        return;
                    }
                    mListScoreCommentResults.Add(info);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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

        private void LoadLanguageInfos()
        {
            try
            {
                if (App.Session == null || App.Session.LangTypeInfo == null) { return; }
                mListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("31{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("3101{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session)
                    , WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        App.ShowExceptionMessage(string.Format("LanguageInfo is null"));
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
                App.ShowExceptionMessage(ex.Message);
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
                    VoicePlayBox.ListDownloadParams = ListDownloadParams;
                    VoicePlayBox.Service03Helper = Service03Helper;
                    VoicePlayBox.ListEncryptInfo = ListEncryptInfo;
                    VoicePlayBox.RecordInfoItem = RecordInfoItem;
                    VoicePlayBox.IsAutoPlay = true;
                    VoicePlayBox.Play(true);
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }
        }


    }
}
