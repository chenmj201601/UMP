using System.Linq;
using Common3601;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using UMPS3601.Wcf36011;

namespace UMPS3601
{
    /// <summary>
    /// PaperRemovQuestionPage.xaml 的交互逻辑
    /// </summary>
    public partial class PaperRemovQuestionPage
    {
        public ExamProductionView ParentPage;
        private ObservableCollection<CPaperParam> _mListPapersInfoCollection;
        private List<CPaperParam> _mlPaperParamsOld;
        private List<CPaperParam> _mlPaperParamsTemp;
        private List<CPaperQuestionParam> _mlPaperQuestionParams;
        private CPaperParam _mPaperParam;
        private List<CQuestionsParam> _mlQuestionsParams;
        private List<CPaperQuestionParam> _mlPaperQuestionParam;
        private List<long> _mlLongs = new List<long>();


        public PaperRemovQuestionPage()
        {
            InitializeComponent();
            _mListPapersInfoCollection = new ObservableCollection<CPaperParam>();
            PIDocument.ItemsSource = _mListPapersInfoCollection;
            _mlPaperParamsTemp = new List<CPaperParam>();
            _mlPaperParamsOld = new List<CPaperParam>();
            _mPaperParam = new CPaperParam();
            _mlPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlQuestionsParams = S3601App.GListQuestionInfos;
            _mlPaperQuestionParam = new List<CPaperQuestionParam>();
            Init();
        }

        private void Init()
        {
            InitPaperInformationList();

            GetQuestionsOfPaper();
            SetPaperParams();
            GetPaperInfos();
            ChangeLanguage();
        }

        void InitPaperInformationList()
        {
            try
            {
                string[] lans = "3601T00037,3601T00038,3601T00040,3601T00042,3601T00043".Split(',');
                string[] cols = "LongNum,StrName,CharType,IntQuestionNum,IntScores".Split(',');
                int[] colwidths = { 150, 180, 100, 50, 50 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 5; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                PIDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            SearchPaperName.Text = CurrentApp.GetLanguageInfo("3601T00079", "Search Paper");
            BtnDelete.Content = CurrentApp.GetLanguageInfo("3601T00082", "Delete");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3601T00059", "Cancel");
            PaperInfoDocument.Title = CurrentApp.GetLanguageInfo("3601T00080", "Paper Information");
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    EditPaperParam();
                    if (!DeleteDb())
                    {
                        ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00062", "Failed to add Question")));
                        return;
                    }
                    foreach (var paperQuestionParam in _mlPaperQuestionParam)
                    {
                        _mListPapersInfoCollection.Remove(
                            _mListPapersInfoCollection.FirstOrDefault(p => p.LongNum == paperQuestionParam.LongPaperNum));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(string.Format("{0} : {1}", CurrentApp.GetLanguageInfo("3601T00062", "Failed to add Question"), ex.Message));
            }
        }


        private void EditPaperParam()
        {
            _mlPaperQuestionParam.Clear();
            foreach (var questionsParam in _mlQuestionsParams)
            {
                var editPaper = new CPaperQuestionParam();
                editPaper.LongPaperNum = _mPaperParam.LongNum;
                editPaper.LongQuestionNum = questionsParam.LongNum;
                editPaper.IntQuestionType = questionsParam.IntType;
                editPaper.StrQuestions = questionsParam.StrQuestionsContect;
                editPaper.StrAnswerA = questionsParam.StrAnswerOne;
                editPaper.StrAnswerB = questionsParam.StrAnswerTwo;
                editPaper.StrAnswerC = questionsParam.StrAnswerThree;
                editPaper.StrAnswerD = questionsParam.StrAnswerFour;
                editPaper.StrAnswerE = questionsParam.StrAnswerFive;
                editPaper.StrAnswerF = questionsParam.StrAnswerSix;
                editPaper.CorrectAnswerOne = questionsParam.CorrectAnswerOne;
                editPaper.CorrectAnswerTwo = questionsParam.CorrectAnswerTwo;
                editPaper.CorrectAnswerThree = questionsParam.CorrectAnswerThree;
                editPaper.CorrectAnswerFour = questionsParam.CorrectAnswerFour;
                editPaper.CorrectAnswerFive = questionsParam.CorrectAnswerFive;
                editPaper.CorrectAnswerSix = questionsParam.CorrectAnswerSix;
                editPaper.StrAccessoryType = questionsParam.StrAccessoryType;
                editPaper.StrAccessoryName = questionsParam.StrAccessoryName;
                editPaper.StrAccessoryPath = questionsParam.StrAccessoryPath;
                _mlPaperQuestionParam.Add(editPaper);
            }
        }

        private bool GetQuestionsOfPaper()
        {
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            _mlLongs.Clear();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionsOfPaper;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mlQuestionsParams);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36011Client client = new Service36011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return false;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CPaperQuestionParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    CPaperQuestionParam paperQuestionParam = optReturn.Data as CPaperQuestionParam;
                    if (paperQuestionParam == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return false;
                    }
                    _mlLongs.Add(paperQuestionParam.LongPaperNum);
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
            return true;
        }

        private bool DeleteDb()
        {
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationDeletePaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mlPaperQuestionParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36011Client client = new Service36011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00083", "Delete Error"));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
            return false;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch
            {

            }
        }

        private void PiDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                Keyboard.IsKeyDown(Key.RightShift) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.LeftShift))
            {
                return;
            }
            _mPaperParam = new CPaperParam();
            var item = PIDocument.SelectedItem as CPaperParam;
            _mPaperParam = item;
        }

        private void PiDocument_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void SetPaperParams()
        {
            try
            {
                _mlPaperParamsTemp.Clear();
                foreach (var mlLong in _mlLongs)
                {
                    CPaperParam param = new CPaperParam();
                    param.LongNum = mlLong;
                    _mlPaperParamsTemp.Add(param);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void TbSearchPaper_TextChanged(object sender, TextChangedEventArgs e)
        {
            _mlPaperQuestionParam.Clear();
            _mlPaperParamsTemp.Clear();
            _mListPapersInfoCollection.Clear();
            string strTemp = TbSearchPaperName.Text;
            if (string.IsNullOrEmpty(strTemp))
            {
                foreach (var paperParam in _mlPaperParamsOld)
                {
                    _mListPapersInfoCollection.Add(paperParam);
                }
                return;
            }
            foreach (var paperParam in _mlPaperParamsOld)
            {
                string paperName = paperParam.StrName;
                int index = paperName.IndexOf(strTemp);
                if (index != -1)
                {
                    _mlPaperParamsTemp.Add(paperParam);
                }
            }

            foreach (var paperParam in _mlPaperParamsTemp)
            {
                _mListPapersInfoCollection.Add(paperParam);
            }
        }

        private void GetPaperInfos()
        {
            try
            {
                _mListPapersInfoCollection.Clear();
                var webRequest = new WebRequest();
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationSearchPapers;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mlPaperParamsTemp);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    CPaperParam cExamPapers = optReturn.Data as CPaperParam;
                    if (cExamPapers == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    _mListPapersInfoCollection.Add(cExamPapers);
                    _mlPaperParamsOld.Add(cExamPapers);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
