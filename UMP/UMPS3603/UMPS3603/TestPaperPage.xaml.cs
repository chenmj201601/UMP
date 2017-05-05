using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common3603;
using UMPS3603.Wcf36031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3603
{
    /// <summary>
    /// TestPaperPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPaperPage
    {
        public ExamProductionView ExamProductionParentPage;

        private CPaperParam _mPaperParam;
        private List<CPaperQuestionParam> _mlPaperQuestionParams;
        private List<CPaperQuestionParam> _mlToFPaperQuestionParams;
        private List<CPaperQuestionParam> _mlScPaperQuestionParams;
        private List<CPaperQuestionParam> _mlMcPaperQuestionParams;
        private int _miPaperScore;

        public TestPaperPage()
        {
            _mlPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlToFPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlScPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlMcPaperQuestionParams = new List<CPaperQuestionParam>();
            _mPaperParam = S3603App.GPaperParam;
            _miPaperScore = 0;
            InitializeComponent();
            this.Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _miPaperScore = 0;
            GetPaperQuesions();
            GetQuestions();
            InitGrid();
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            PaperName.Text = _mPaperParam.StrName;
            PaperInfoName.Text = string.Format("{0}{1} {2}  {3}{4}", CurrentApp.GetLanguageInfo("3603T00056", "Time :"),
                _mPaperParam.IntTestTime,
                CurrentApp.GetLanguageInfo("3603T00088", "Min"),
                CurrentApp.GetLanguageInfo("3603T00058", "Score:"),
                _mPaperParam.IntScores);

            if (_miPaperScore != _mPaperParam.IntScores)
            {
                NoteName.Visibility = Visibility.Visible;
                NoteName.Foreground = new SolidColorBrush(Colors.Red);
                NoteName.Text = string.Format("{0}{1}{2}", CurrentApp.GetLanguageInfo("3603T00059", "Time :"),
                    _miPaperScore, CurrentApp.GetLanguageInfo("3603T00060", "Score:"));
            }
        }

        private void InitGrid()
        {
            TrueOrFalseGrid();
            SingleChioseGrid();
            MultipleChioseGrid();
        }

        private void TrueOrFalseGrid()
        {
            int iCount = _mlToFPaperQuestionParams.Count;
            if (iCount > 0)
            {
                TrueOrFalse.Visibility = Visibility.Visible;
                Grid grid = new Grid();
                for (int i = 0; i < iCount; i++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);
                }

                int f = 0;
                int score = 0;
                foreach (var param in _mlToFPaperQuestionParams)
                {
                    TrueOrFlasePage newPage = new TrueOrFlasePage();
                    ToFQuestionInfo tempParam = new ToFQuestionInfo();
                    tempParam.IntNum = (f + 1);
                    tempParam.questionParam = param;
                    newPage.SetQuestionInfo = tempParam;
                    newPage.CurrentApp = CurrentApp;
                    newPage.SetInformation();
                    grid.Children.Add(newPage);
                    Grid.SetRow(newPage, f);
                    f++;
                    score += param.IntScore;
                }
                TrueOrFalseQuestions.Children.Add(grid);
                TrueOrFalse.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3603T00061", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3603T00064", "Score"));
                _miPaperScore += score;
            }
        }

        private void SingleChioseGrid()
        {
            int iCount = _mlScPaperQuestionParams.Count;
            if (iCount > 0)
            {
                SingleChoice.Visibility = Visibility.Visible;
                Grid grid = new Grid();
                for (int i = 0; i < iCount; i++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);
                }
                int f = 0;
                int score = 0;
                foreach (var param in _mlScPaperQuestionParams)
                {
                    SingleChoicePage newPage = new SingleChoicePage();
                    SCQuestionInfo tempParam = new SCQuestionInfo();
                    tempParam.IntNum = (f + 1);
                    tempParam.questionParam = param;
                    newPage.SetQuestionInfo = tempParam;
                    newPage.CurrentApp = CurrentApp;
                    newPage.SetInformation();
                    grid.Children.Add(newPage);
                    Grid.SetRow(newPage, f);
                    f++;
                    score += param.IntScore;
                }
                SingleChoiceQuestions.Children.Add(grid);
                SingleChoice.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3603T00062", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3603T00064", "Score"));
                _miPaperScore += score;
            }
        }

        private void MultipleChioseGrid()
        {
            int iCount = _mlMcPaperQuestionParams.Count;
            if (iCount > 0)
            {
                MultipleChoice.Visibility = Visibility.Visible;
                Grid grid = new Grid();
                for (int i = 0; i < iCount; i++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);
                }
                int f = 0;
                int score = 0;
                foreach (var param in _mlMcPaperQuestionParams)
                {
                    MultipleChoicePage newPage = new MultipleChoicePage();
                    MCQuestionInfo tempParam = new MCQuestionInfo();
                    tempParam.IntNum = (f + 1);
                    tempParam.questionParam = param;
                    newPage.SetQuestionInfo = tempParam;
                    newPage.CurrentApp = CurrentApp;
                    newPage.SetInformation();
                    grid.Children.Add(newPage);
                    Grid.SetRow(newPage, f);
                    f++;
                    score += param.IntScore;
                }
                MultipleChoiceQuestions.Children.Add(grid);
                MultipleChoice.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3603T00063", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3603T00064", "Score"));
                _miPaperScore += score;
            }
        }

        private void GetPaperQuesions()
        {
            try
            {
                _mlPaperQuestionParams.Clear();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3603Codes.GetPaperQuestions;
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
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("3603T00053", "Paper Search failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    return;
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

        private void GetQuestions()
        {
            _mlToFPaperQuestionParams.Clear();
            _mlScPaperQuestionParams.Clear();
            _mlMcPaperQuestionParams.Clear();
            foreach (var param in _mlPaperQuestionParams)
            {
                switch (param.IntQuestionType)
                {
                    case (int) QuestionType.Trueorfalse:
                        _mlToFPaperQuestionParams.Add(param);
                        break;
                    case (int) QuestionType.SingleChoice:
                        _mlScPaperQuestionParams.Add(param);
                        break;
                    case (int) QuestionType.MultipleChioce:
                        _mlMcPaperQuestionParams.Add(param);
                        break;
                }
            }
        }
    }
}
