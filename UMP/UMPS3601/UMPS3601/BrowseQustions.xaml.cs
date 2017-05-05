using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Common3601;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3601
{
    /// <summary>
    /// BrowseQustions.xaml 的交互逻辑
    /// </summary>

    public partial class BrowseQustions
    {

        #region Members
        public ExamProductionView EpParentPage;
        public CreateQuestionsView CqParentPage;
        private static List<CQuestionsParam> _mListQuestionInfos;
        private int _mICountQuestionItem = 0;
        private CQuestionsParam _mCExamQuestionsTemp;
        protected CustomWaiter MyWaiter;
        #endregion

        public BrowseQustions()
        {
            _mCExamQuestionsTemp = new CQuestionsParam();
            _mListQuestionInfos = S3601App.GListQuestionInfos;
            InitializeComponent();
            this.Loaded += UCCustomSetting_Loaded;
            ButButton.Content = "<";
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitInterface();
            ChangeLanguages();
        }

        public void ChangeLanguages()
        {
            lbQuestionNumName.Content = CurrentApp.GetLanguageInfo("3601T00016", "Number") + ":";
            lbCategoryNumName.Content = CurrentApp.GetLanguageInfo("3601T00107", "Number") + ":";
            lbCategoryNameName.Content = CurrentApp.GetLanguageInfo("3601T00017", "Number") + ":";
            lbTypeName.Content = CurrentApp.GetLanguageInfo("3601T00018", "Number") + ":";
            lbFounderIDName.Content = CurrentApp.GetLanguageInfo("3601T00108", "Number") + ":";
            lbFounderNameName.Content = CurrentApp.GetLanguageInfo("3601T00031", "Number") + ":";
            lbDateTimeName.Content = CurrentApp.GetLanguageInfo("3601T00047", "Number") + ":";
            lbUseNumberName.Content = CurrentApp.GetLanguageInfo("3601T00027", "Number") + ":";
            lbAccessoryTypeName.Content = CurrentApp.GetLanguageInfo("3601T00028", "Number") + ":";
            lbAccessoryNameName.Content = CurrentApp.GetLanguageInfo("3601T00029", "Number") + ":";
            lbAccessoryPathName.Content = CurrentApp.GetLanguageInfo("3601T00030", "Number") + ":";
        }

        private void InitBut()
        {
            OperationBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += UpDateQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00137","Change");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);
        }

        private void InitBut1()
        {
            OperationBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += UpDateQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00137", "Change");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);

            btn = new Button();
            btn.Click += NextQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00138", "Next");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);
        }

        private void InitBut2()
        {
            OperationBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += UpDateQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00137", "Change");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);

            btn = new Button();
            btn.Click += PreviousQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00139", "last");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);
        }

        private void InitBut3()
        {
            OperationBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += UpDateQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00137", "Change");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);

            btn = new Button();
            btn.Click += PreviousQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00139", "last");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);

            btn = new Button();
            btn.Click += NextQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00138", "Next");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            OperationBut.Children.Add(btn);
        }

        private void InitRadioToF(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                TrueOrFlasePage newPage = new TrueOrFlasePage();
                ToFQuestionInfo tempParam = new ToFQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRadioSingle(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                SingleChoicePage newPage = new SingleChoicePage();
                SCQuestionInfo tempParam = new SCQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }  
        }

        private void InitRadioMultiple(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                MultipleChoicePage newPage = new MultipleChoicePage();
                MCQuestionInfo tempParam = new MCQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitParam(CQuestionsParam cExamQuestion)
        {
            lbQuestionNum.Text = cExamQuestion.LongNum.ToString();
            lbCategoryNum.Text = cExamQuestion.LongCategoryNum.ToString();
            lbCategoryName.Text = cExamQuestion.StrCategoryName;
            switch ((QuestionType)cExamQuestion.IntType)
            {
                case QuestionType.TrueOrFalse:
                    lbType.Text = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
                    break;
                case QuestionType.SingleChoice:
                    lbType.Text = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
                    break;
                case QuestionType.MultipleChioce:
                    lbType.Text = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Chioce");
                    break;
            }
            lbFounderID.Text = cExamQuestion.LongFounderId.ToString();
            lbFounderName.Text = cExamQuestion.StrFounderName;
            lbDateTime.Text = cExamQuestion.StrDateTime;
            lbUseNumber.Text = cExamQuestion.IntUseNumber.ToString();
            lbAccessoryType.Text = cExamQuestion.StrAccessoryType;
            lbAccessoryName.Text = cExamQuestion.StrAccessoryName;
            lbAccessoryPath.Text = cExamQuestion.StrAccessoryPath;

            //DownloadFiles(cExamQuestion.StrAccessoryPath, cExamQuestion.StrAccessoryName);

            switch (cExamQuestion.IntType)
            {
                case (int)QuestionType.TrueOrFalse:
                    InitRadioToF(cExamQuestion);
                    break;
                case (int)QuestionType.SingleChoice:
                    InitRadioSingle(cExamQuestion);
                    break;
                case (int)QuestionType.MultipleChioce:
                    InitRadioMultiple(cExamQuestion);
                    break;
            }
        }

        private void InitInterface()
        {

            
            if (_mListQuestionInfos.Count > 1)
            {
                InitBut1();
                _mICountQuestionItem = 0;
                CQuestionsParam cExamQuestion = _mListQuestionInfos[0];
                if (cExamQuestion != null)
                {
                    _mCExamQuestionsTemp = cExamQuestion;
                    InitParam(cExamQuestion);
                }
            }
            else if (_mListQuestionInfos.Count == 1)
            {
                InitBut();
                CQuestionsParam cExamQuestion = _mListQuestionInfos[0];
                if (cExamQuestion != null)
                {
                    _mCExamQuestionsTemp = cExamQuestion;
                    InitParam(cExamQuestion);
                }
            }
            else
            {
                try
                {
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        ShowException(string.Format("Failed initialization parameters"));
                        parent.IsOpen = false;
                    }
                }
                catch
                {

                }
            }
        }

        #region Click


        private void UpDateQuestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    S3601App.GQuestionInfo.QuestionsParam = _mCExamQuestionsTemp;
                    S3601App.GQuestionInfo.OptType = S3601Codes.OperationUpdateQuestion;
                    switch( S3601App.GMessageSource)
                    {
                        case S3601Codes.MessageCreatequestionspage:
                            CqParentPage.RefreshQuestion();
                            break;
                        case S3601Codes.MessageExamproduction:
                            EpParentPage.UpdateQuestion();
                            break;
                    }
                    
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
            
        }

        private void PreviousQuestion_Click(object sender, RoutedEventArgs e )
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                _mICountQuestionItem--;
                CQuestionsParam cExamQuestion = _mListQuestionInfos[_mICountQuestionItem];
                if (cExamQuestion != null)
                {
                    _mCExamQuestionsTemp = cExamQuestion;
                    InitParam(cExamQuestion);
                }
                if (_mICountQuestionItem == 0)
                {
                    InitBut1();
                }
                else
                {
                    InitBut3();
                }
            }
        }

        private void NextQuestion_Click( object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                _mICountQuestionItem++;
                CQuestionsParam cExamQuestion = _mListQuestionInfos[_mICountQuestionItem];
                if (cExamQuestion != null)
                {
                    _mCExamQuestionsTemp = cExamQuestion;
                    InitParam(cExamQuestion);
                }
                if (_mICountQuestionItem >= (_mListQuestionInfos.Count-1) )
                {
                    InitBut2();
                }
                else
                {
                    InitBut3();
                }
            }
        }


        #endregion


        /// <summary>
        /// 下载文件
        /// </summary>
        bool DownloadFiles(string downloadPath, string Fname)
        {
            System.Net.HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            Fname = downloadPath.Substring(downloadPath.LastIndexOf(Fname));
            string LStrFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Fname);
            string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port - 1), downloadPath);
            //string path = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                LStreamCertificateFile = new FileStream(LStrFileFullName, FileMode.Create);
                LHttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ServerPath);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);
                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DownLoad File To Disk  Failed.", string.Format("FileName : {0} \t  SavePath : {1} \t   ServerPath :{2} \t  Message :{3}", Fname, LStrFileFullName, ServerPath, ex.Message));
                ShowException(ex.Message);
                return false;
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
            }
            CurrentApp.WriteLog("DownLoad File To Disk  Sucessed!", string.Format("SavePath : {0} \t   ServerPath :{1} \t  ", LStrFileFullName, ServerPath));
            return true;
        }

        private void ButButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GridQuestionInfo.Visibility == Visibility.Collapsed)
            {
                GridQuestionInfo.Visibility = Visibility.Visible;
                ButButton.Content = ">>";
            }
            else
            {
                GridQuestionInfo.Visibility = Visibility.Collapsed;
                ButButton.Content = "<<";
            }
        }
    }
}
