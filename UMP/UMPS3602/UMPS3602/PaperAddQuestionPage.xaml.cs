using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common3602;
using UMPS3602.Wcf36021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;


namespace UMPS3602
{
    /// <summary>
    /// PaperAddQuestion.xaml 的交互逻辑
    /// </summary>
    public partial class PaperAddQuestionPage
    {
        public EditPaperView ParentPage;
        private CPaperParam _mCPaperParam;
        private List<CQuestionsParam> _mLstQuestionsParam;
        private List<CPaperQuestionParam> _mlstPaperQuestionParam;
        private List<CPaperQuestionParam> _mlstChangePaperQuestionParam;

        public PaperAddQuestionPage()
        {
            _mCPaperParam = S3602App.GPaperInfo.PaperParam;
            _mLstQuestionsParam = S3602App.GLstQuestionInfos;
            _mlstPaperQuestionParam = new List<CPaperQuestionParam>();
            _mlstChangePaperQuestionParam = new List<CPaperQuestionParam>();
            InitializeComponent();
            this.Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            RbutFalse.IsChecked = true;
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            SetScore.Text = CurrentApp.GetLanguageInfo("3602T00056", "Set Score");
            ScoreValue.Text = string.Format("{0}( 0 - {1} )", CurrentApp.GetLanguageInfo("3602T00057", "Score"), S3602App.GIntUsableScore);
            ButSet.Content = CurrentApp.GetLanguageInfo("3602T00058", "Set");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("3602T00059", "Cancel");
            EnableChange.Text = CurrentApp.GetLanguageInfo("3602T00063", "Enable Change");
            RbutTrue.Content = CurrentApp.GetLanguageInfo("3602T00064", "Yes");
            RbutFalse.Content = CurrentApp.GetLanguageInfo("3602T00065", "No");
        }

        //private void TbSetScore_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        var textChange = new TextChange[e.Changes.Count];
        //        e.Changes.CopyTo(textChange, 0);
        //        int offset = textChange[0].Offset;
        //        if (textChange[0].AddedLength > 0)
        //        {
        //            double num;
        //            if (!Double.TryParse(TbSetScore.Text, out num))
        //            {
        //                TbSetScore.Text = TbSetScore.Text.Remove(offset, textChange[0].AddedLength);
        //                TbSetScore.Select(offset, 0);
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        private void TbSetScore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (string.IsNullOrEmpty(TbSetScore.Text))
            {
                return;
            }
            if (Convert.ToInt64(TbSetScore.Text) < 0 || Convert.ToInt64(TbSetScore.Text) > S3602App.GIntUsableScore)
            {
                TbSetScore.Text = "0";
                ShowException(string.Format("{0}: 0 - {1}", CurrentApp.GetLanguageInfo("3602T00057", "Score"), S3602App.GIntUsableScore));
            }
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
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

        private void ButSet_OnClick(object sender, RoutedEventArgs e)
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
                    ChangePaperQuestion();
                    S3602App.GlistPaperQuestionParam = _mlstPaperQuestionParam;
                    ParentPage.UpdatePaperContentList();
                    ParentPage.ChangeListTableInfo();
                    ParentPage.SetPaperContentSelect();
                    ParentPage.SetBackValue();
                    ParentPage.MLstChangePaperQuestion = _mlstChangePaperQuestionParam;
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
               ShowException(string.Format("{0} : {1}", CurrentApp.GetLanguageInfo("3602T00062", "Failed to add Question"), ex.Message));
            }
        }

        private bool SetSroce()
        {
            if (string.IsNullOrEmpty(TbSetScore.Text))
            {
                ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3602T00192", "题目分值不能为空！")));
                return false;
            }
            if ((_mLstQuestionsParam.Count * Convert.ToInt32(TbSetScore.Text)) > S3602App.GIntUsableScore)
            {
               ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3602T00061", "The set scores greater than the remaining scores")));
                return false;
            }
            return true;
        }

        private void EditPaperParam()
        {
            _mlstPaperQuestionParam.Clear();
            foreach (var cQuestionsParam in _mLstQuestionsParam)
            {
                var editPaper = new CPaperQuestionParam();
                editPaper.LongPaperNum = _mCPaperParam.LongNum;
                editPaper.LongQuestionNum = cQuestionsParam.LongNum;
                editPaper.IntQuestionType = cQuestionsParam.IntType;
                editPaper.StrQuestionsContect = cQuestionsParam.StrQuestionsContect;
                editPaper.EnableChange = RbutFalse.IsChecked != true ? 1 : 0;
                editPaper.StrEnableChange = editPaper.EnableChange == 0
                    ? CurrentApp.GetLanguageInfo("3602T00119", "N")
                    : CurrentApp.GetLanguageInfo("3602T00118", "Y");
                editPaper.StrAnswerOne = cQuestionsParam.StrAnswerOne;
                editPaper.StrConOne = editPaper.StrAnswerOne == "F"
                    ? CurrentApp.GetLanguageInfo("3602T00119", "F")
                    : CurrentApp.GetLanguageInfo("3602T00118", "T");
                editPaper.StrAnswerTwo = cQuestionsParam.StrAnswerTwo;
                editPaper.StrAnswerThree = cQuestionsParam.StrAnswerThree;
                editPaper.StrAnswerFour = cQuestionsParam.StrAnswerFour;
                editPaper.StrAnswerFive = cQuestionsParam.StrAnswerFive;
                editPaper.StrAnswerSix = cQuestionsParam.StrAnswerSix;
                editPaper.CorrectAnswerOne = cQuestionsParam.CorrectAnswerOne;
                editPaper.CorrectAnswerTwo = cQuestionsParam.CorrectAnswerTwo;
                editPaper.CorrectAnswerThree = cQuestionsParam.CorrectAnswerThree;
                editPaper.CorrectAnswerFour = cQuestionsParam.CorrectAnswerFour;
                editPaper.CorrectAnswerFive = cQuestionsParam.CorrectAnswerFive;
                editPaper.CorrectAnswerSix = cQuestionsParam.CorrectAnswerSix;
                editPaper.IntScore = Convert.ToInt32(TbSetScore.Text);
                editPaper.StrAccessoryType = cQuestionsParam.StrAccessoryType;
                editPaper.StrAccessoryName = cQuestionsParam.StrAccessoryName;
                editPaper.StrAccessoryPath = cQuestionsParam.StrAccessoryPath;
                _mlstPaperQuestionParam.Add(editPaper);
            }
        }

        private bool ChangePaperQuestion()
        {
            try
            {
                _mlstChangePaperQuestionParam = new List<CPaperQuestionParam>();
                foreach (var paperQuestion in ParentPage.MLstPaperQuestionsOld)
                {

                    CPaperQuestionParam cPaperQuestion = new CPaperQuestionParam();
                    cPaperQuestion = _mlstPaperQuestionParam.FirstOrDefault(
                        p => p.LongQuestionNum == paperQuestion.LongQuestionNum);
                    if (null != cPaperQuestion)
                    {
                        _mlstChangePaperQuestionParam.Add(cPaperQuestion);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
            return false;
        }
    }
}
