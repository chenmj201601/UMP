using System;
using System.Windows;
using System.Windows.Controls;
using UMPS3603.Wcf36031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using Common3603;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPS3603
{
    /// <summary>
    /// AddTestInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AddTestInfo
    {
        public ExamProductionView ParentPage;
        private PaperInfoParam _mPaperInfoParam;

        public AddTestInfo()
        {
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            TxtPaperName.IsReadOnly = true;
            TxtPaperNum.IsReadOnly = true;
            _mPaperInfoParam = S3603App.GPaperInfoParamo;
            TxtPaperName.Text = _mPaperInfoParam.PaperParam.StrName;
            TxtPaperNum.Text = _mPaperInfoParam.PaperParam.LongNum.ToString();
            DttbTestTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            TbPaperName.Text = CurrentApp.GetLanguageInfo("3603T00027", "Paper Name");
            TbPaperNum.Text = CurrentApp.GetLanguageInfo("3603T00028", "Paper Num");
            TbTestTime.Text = CurrentApp.GetLanguageInfo("3603T00044", "Test Time");
            TbExplain.Text = CurrentApp.GetLanguageInfo("3603T00050", "Explain");
            ButOk.Content = CurrentApp.GetLanguageInfo("3603T00051", "OK");
            ButClose.Content = CurrentApp.GetLanguageInfo("3603T00052", "Close");
        }

        private long CreateTestNum()
        {
            try
            {
                if (!S3603App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    WebRequest webRequest = new WebRequest
                    {
                        Session = CurrentApp.Session,
                        Code = (int) RequestCode.WSGetSerialID
                    };
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3603");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                         WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                    //var client = new Service36031Client();
                    WebReturn webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                        return 0;
                    string strNewResultId = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultId))
                        return 0;
                    return Convert.ToInt64(strNewResultId);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return 0;
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
                // ignored
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            S3603App.GTestInformation = null;
            TestInfoParam testInformation = new TestInfoParam();
            try
            {
                if (TxtExplain.Text.Length > 1000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00097", "Exam content that cannot exceed 1000 characters."));
                    return;
                }

                testInformation.StrPaperName = _mPaperInfoParam.PaperParam.StrName;
                testInformation.LongPaperNum = _mPaperInfoParam.PaperParam.LongNum;
                testInformation.StrExplain = TxtExplain.Text;
                testInformation.StrTestTime = DttbTestTime.ToString();
                testInformation.LongEditorId = CurrentApp.Session.UserInfo.UserID;
                testInformation.StrEditor = CurrentApp.Session.UserInfo.UserName;
                testInformation.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    switch (_mPaperInfoParam.OptType)
                    {
                        case S3603Codes.OptAddInfo:
                            long longTestNum = CreateTestNum();
                            testInformation.LongTestNum = longTestNum;
                            if (longTestNum == 0)
                            {
                                ShowException("Get paper number error");
                                return;
                            }
                            if (!WriteTestInfo(testInformation)) return;
                            S3603App.GTestInformation = testInformation;
                            ParentPage.UpdateTestInfoTable();
                            break;
                        case S3603Codes.OptChangeInfo:
                           
                            break;
                        case S3603Codes.OptDeleteInfo:

                            break;
                    }
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool WriteTestInfo(TestInfoParam testInformation)
        {
            string strLog;
            try
            {
                var webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3603Codes.OptAddTestInfo
                };
                OperationReturn optReturn = XMLHelper.SeriallizeObject(testInformation);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00006", "Create"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00006"),
                        Utils.FormatOptLogString("3603T00091"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(webReturn.Message);
                    return false;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3603T00006"),
                    Utils.FormatOptLogString("3603T00092"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00092", "Create Success"));
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00006"),
                    Utils.FormatOptLogString("3603T00091"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void TxtExplain_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtExplain.Text.Length > 1000)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00097", "Exam content that cannot exceed 1000 characters."));
            }
        }
    }
}
