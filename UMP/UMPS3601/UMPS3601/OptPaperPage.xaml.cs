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
    /// OptPaperPage.xaml 的交互逻辑
    /// </summary>
    public partial class OptPaperPage
    {
        public ExamProductionView ParentPage;
        private ObservableCollection<CPaperParam> _mListPapersInfoCollection;
        private List<CPaperParam> _mlPaperParamsOld;
        private List<CPaperParam> _mlPaperParamsTemp;
        private List<CPaperQuestionParam> _mlPaperQuestionParams; 
        private CPaperParam _mPaperParam;
        private int _mIntScore = 0;
        private List<CQuestionsParam> _mlQuestionsParams;
        private List<CPaperQuestionParam> _mlPaperQuestionParam;

        public OptPaperPage()
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
            GetPaperInfos();
            ChangeLanguage();
        }

        private void UsableScore()
        {
            int allScore = _mPaperParam.IntScores;
            int intTemp = 0;
            if (_mlPaperQuestionParams.Count > 0)
                foreach (var cEditPaper in _mlPaperQuestionParams)
                {
                    intTemp += cEditPaper.IntScore;
                }
            _mIntScore = allScore - intTemp;
        }

        void InitPaperInformationList()
        {
            try
            {
                string[] lans = "3601T00037,3601T00038,3601T00040,3601T00042,3601T00043".Split(',');
                string[] cols = "LongNum,StrName,CharType,IntQuestionNum,IntScores".Split(',');
                int[] colwidths = { 150, 180, 100, 50, 50};
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
            SetScore.Text = CurrentApp.GetLanguageInfo("3601T00056", "Set Score");
            ScoreValue.Text = string.Format("{0}({1} - {2})", CurrentApp.GetLanguageInfo("3601T00057", "Score"), 0, 0);
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3601T00058", "Add");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3601T00059", "Cancel");
            EnableChange.Text = CurrentApp.GetLanguageInfo("3601T00063", "Enable Change");
            RbutTrue.Content = CurrentApp.GetLanguageInfo("3601T00064", "Yes");
            RbutFalse.Content = CurrentApp.GetLanguageInfo("3601T00065", "No");
            PaperInfoDocument.Title = CurrentApp.GetLanguageInfo("3601T00080", "Paper Information");
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (!SetSroce())
                    {
                        return;
                    }
                    EditPaperParam();
                    if (QuestionExist())
                    {
                        if (!WriteDb())
                        {
                            ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00062", "Failed to add Question")));
                            return;
                        }
                        ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00081", "Success")));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(string.Format("{0} : {1}", CurrentApp.GetLanguageInfo("3601T00062", "Failed to add Question"), ex.Message));
            }
        }

        private bool SetSroce()
        {
            if (Convert.ToInt32(TbSetScore.Text) > _mIntScore)
            {
                ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00061", "The set scores greater than the remaining scores")));
                return false;
            }

            return true;
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
                editPaper.EnableChange = RbutFalse.IsChecked != true ? 1 : 0;
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
                editPaper.IntScore = Convert.ToInt32(TbSetScore.Text);
                editPaper.StrAccessoryType = questionsParam.StrAccessoryType;
                editPaper.StrAccessoryName = questionsParam.StrAccessoryName;
                editPaper.StrAccessoryPath = questionsParam.StrAccessoryPath;
                _mlPaperQuestionParam.Add(editPaper);
            }
        }

        private bool QuestionExist()
        {
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationPaperQuestionsExist;
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
                    return false;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<long>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    long questionNum = optReturn.Data is long ? (long)optReturn.Data : 0;
                    if (questionNum > 0)
                    {
                        ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00067", "Question exist!")));
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }


            return true;
        }

        private bool WriteDb()
        {
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationAddPaperQuestions;
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
                    ShowException(CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"));
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

        private void GetPaperQustions()
        {
            try
            {
                _mlPaperQuestionParams.Clear();
                var webRequest = new WebRequest();
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetPaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPaperParam);
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
                    optReturn = XMLHelper.DeserializeObject<CPaperQuestionParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cEditPaper = optReturn.Data as CPaperQuestionParam;
                    if (cEditPaper == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }

                    _mlPaperQuestionParams.Add(cEditPaper);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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

        private void TbSetScore_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

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
            GetPaperQustions();
            UsableScore();
            ScoreValue.Text = string.Format("{0}({1} - {2})", CurrentApp.GetLanguageInfo("3601T00057", "Score"), _mIntScore, _mPaperParam.IntScores);
        }

        private void PiDocument_KeyDown(object sender, KeyEventArgs e)
        {
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
            int index = 0; 
            foreach (var paperParam in _mlPaperParamsOld)
            {
                string paperName = paperParam.StrName;
                index = paperName.IndexOf(strTemp);
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
                webRequest.Code = (int)S3601Codes.OperationGetPapers;

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
                    OperationReturn optReturn = new OperationReturn();
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
