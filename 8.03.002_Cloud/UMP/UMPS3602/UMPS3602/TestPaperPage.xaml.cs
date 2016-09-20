using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common3602;
using UMPS3602.Wcf36021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3602
{
    /// <summary>
    /// TestPaperPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPaperPage
    {
        public ExamProductionView ExamProductionParentPage;
        public EditPaperView EditPaperParentPage;

        private PaperInfo _mPaperInfo;
        private List<CPaperQuestionParam> _mlPaperQuestionParams;
        private List<CPaperQuestionParam> _mlToFPaperQuestionParams;
        private List<CPaperQuestionParam> _mlScPaperQuestionParams;
        private List<CPaperQuestionParam> _mlMcPaperQuestionParams;
        private int _miPaperScore = 0;

        public TestPaperPage()
        {
            _mlPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlToFPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlScPaperQuestionParams = new List<CPaperQuestionParam>();
            _mlMcPaperQuestionParams = new List<CPaperQuestionParam>();
            _mPaperInfo = S3602App.GPaperInfo;
            InitializeComponent();
            this.Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _miPaperScore = 0;
            Init();
        }

        private void Init()
        {
            if (_mPaperInfo.OptType != S3602Codes.MsgEditPaperPage)
            {
                GetPaperQuesions();
            }
            else
            {
                _mlPaperQuestionParams.Clear();
                _mlPaperQuestionParams = S3602App.GlistPaperQuestionParam;
            }
            GetQuestions();
            InitGrid();
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            PaperName.Text = _mPaperInfo.PaperParam.StrName;
            PaperInfoName.Text = string.Format("{0}{1}{2}  {3}{4}", CurrentApp.GetLanguageInfo("3602T00101", "Time :"),
                _mPaperInfo.PaperParam.IntTestTime,
                CurrentApp.GetLanguageInfo("3602T00105", "Min"),
                CurrentApp.GetLanguageInfo("3602T00102", "Score:"),
               _mPaperInfo.PaperParam.IntScores);

            if (_miPaperScore != _mPaperInfo.PaperParam.IntScores)
            {
                NoteName.Visibility = Visibility.Visible;
                NoteName.Foreground = new SolidColorBrush(Colors.Red);
                NoteName.Text = string.Format("{0}{1}{2}", CurrentApp.GetLanguageInfo("3602T00103", "Time :"),
                _miPaperScore, CurrentApp.GetLanguageInfo("3602T00104", "Score:"));
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
                    newPage.CurrentApp = this.CurrentApp;
                    newPage.SetInformation();
                    grid.Children.Add(newPage);
                    Grid.SetRow(newPage, f);
                    f++;
                    score += param.IntScore;
                }
                TrueOrFalseQuestions.Children.Add(grid);
                TrueOrFalse.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3602T00098", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3602T00100", "Score"));
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
                SingleChoice.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3602T00097", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3602T00100", "Score"));
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
                MultipleChoice.Text = string.Format("{0}({1}{2})", CurrentApp.GetLanguageInfo("3602T00099", "True Or False"),
                    score, CurrentApp.GetLanguageInfo("3602T00100", "Score"));
                _miPaperScore += score;
            }
        }

        private void GetPaperQuesions()
        {
            try
            {
                _mlPaperQuestionParams.Clear();
                var webRequest = new WebRequest();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetPaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPaperInfo.PaperParam);
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
                   ShowException(string.Format("{0}{1}", CurrentApp.GetLanguageInfo("3602T00076", "Insert data failed"), webReturn.Message));
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

        private void GetQuestions()
        {
            _mlToFPaperQuestionParams.Clear();
            _mlScPaperQuestionParams.Clear();
            _mlMcPaperQuestionParams.Clear();
            foreach (var param in _mlPaperQuestionParams)
            {
                switch (param.IntQuestionType)
                {
                    case (int)QuestionType.TrueOrFalse:
                        _mlToFPaperQuestionParams.Add(param);
                        break;
                    case (int)QuestionType.SingleChoice:
                        _mlScPaperQuestionParams.Add(param);
                        break;
                    case (int)QuestionType.MultipleChioce:
                        _mlMcPaperQuestionParams.Add(param);
                        break;
                }
            }
        }

    }

}
