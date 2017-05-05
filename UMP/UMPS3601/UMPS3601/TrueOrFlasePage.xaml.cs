using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Common3601;
using UMPS3601.Wcf36011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3601
{
    /// <summary>
    /// TrueOrFlasePage.xaml 的交互逻辑
    /// </summary>

    public struct ToFQuestionInfo
    {
        public int IntNum;
        public CQuestionsParam questionParam;
    }

    public partial class TrueOrFlasePage
    {
        public ToFQuestionInfo SetQuestionInfo { get; set; }
        private string _mLoadPath = null;

        public TrueOrFlasePage()
        {
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            TbQuestion.Text = CurrentApp.GetLanguageInfo("3601T00019", "Question") + ":";
            TbAnswer.Text = CurrentApp.GetLanguageInfo("3601T00136", "Answer") + ":";
            RbutTofTrue.Content = CurrentApp.GetLanguageInfo("3601T00064", "Yes");
            RbutTofFalse.Content = CurrentApp.GetLanguageInfo("3601T00065", "No");
        }

        public void SetInformation()
        {
            try
            {
                if (SetQuestionInfo.IntNum != 0)
                    TbQuestionName.Text = string.Format("{0}、{1}", SetQuestionInfo.IntNum, SetQuestionInfo.questionParam.StrQuestionsContect);
                else
                    TbQuestionName.Text = string.Format("{0}", SetQuestionInfo.questionParam.StrQuestionsContect);

                if (SetQuestionInfo.questionParam.StrAccessoryType == "wav"
                    || SetQuestionInfo.questionParam.StrAccessoryType == "pcm")
                {
                    AccessoryName1.Visibility = Visibility.Visible;
                    if( LoadFiles() )
                        PlayFiles(_mLoadPath, SetQuestionInfo.questionParam.StrAccessoryName);
                }
                if (SetQuestionInfo.questionParam.StrAccessoryType == "gif"
                    || SetQuestionInfo.questionParam.StrAccessoryType == "jpg"
                    || SetQuestionInfo.questionParam.StrAccessoryType == "jpeg"
                    || SetQuestionInfo.questionParam.StrAccessoryType == "png")
                {
                    if (LoadFiles())
                    {
                        string ServerPath = string.Format("{0}://{1}:{2}/{3}", CurrentApp.Session.AppServerInfo.Protocol,
                            CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port, _mLoadPath);
                        AccessoryName2.Visibility = Visibility.Visible;
                        var bitmap1 = new BitmapImage();
                        bitmap1.BeginInit();
                        bitmap1.UriSource = new Uri(
                            string.Format("{0}", ServerPath),
                            UriKind.RelativeOrAbsolute);
                        bitmap1.EndInit();
                        ImageName.Source = bitmap1;
                        CurrentApp.WriteLog("Play File Sucessed!", string.Format("ServerPath : {0} ", ServerPath));
                    }
                }
                if (string.CompareOrdinal(SetQuestionInfo.questionParam.CorrectAnswerOne.ToString(), "T") != 0)
                {
                    RbutTofFalse.IsChecked = true;
                }
                else
                {
                    RbutTofTrue.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool LoadFiles()
        {
            try
            {
                WebRequest webRequest;
                Service36011Client client;
                WebReturn webReturn;
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3601Codes.OperationLoadFile;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(SetQuestionInfo.questionParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(webReturn.Message);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data))
                {
                    return false;
                }
                _mLoadPath = webReturn.Data;

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void PlayFiles(string downloadPath, string Fname)
        {
            string ServerPath =
                System.IO.Path.Combine(
                    string.Format("{0}://{1}:{2}", CurrentApp.Session.AppServerInfo.Protocol, CurrentApp.Session.AppServerInfo.Address,
                        CurrentApp.Session.AppServerInfo.Port), downloadPath);
            //string ServerPath = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                UCPlayBox mUCPlayBox = new UCPlayBox();
                mUCPlayBox.ToFParentPage = this;
                BorderPlayBox.Child = mUCPlayBox;
                mUCPlayBox.PlayUrl = ServerPath;
                mUCPlayBox.CurrentApp = CurrentApp;
                // PanelPlayBox.IsVisible = true;
            }
            catch (Exception ex)
            {
                #region 写操作日志

                string strLog = string.Format("{0} {1}{2}", CurrentApp.Session.UserID.ToString(),
                    Utils.FormatOptLogString("FO3106007"), Fname);
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Play.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog("Play File Failed.",
                    string.Format("ServerPath : {0} \t   Message :{1}", ServerPath, ex.Message));
                ShowException(ex.Message);
            }

            #region 写操作日志

            string strLog1 = string.Format("{0} {1}{2}", CurrentApp.Session.UserID.ToString(),
                Utils.FormatOptLogString("FO3106007"), Fname);
            CurrentApp.WriteOperationLog(S3601Consts.OPT_Play.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog1);

            #endregion

            CurrentApp.WriteLog("Play File Sucessed!", string.Format("ServerPath : {0} ", ServerPath));
        }

    }
}
