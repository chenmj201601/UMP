using System;
using System.Windows;
using System.Windows.Controls;
using Common3602;
using UMPS3602.Wcf36021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
namespace UMPS3602
{
    /// <summary>
    /// AddPaperPage.xaml 的交互逻辑
    /// </summary>
    public partial class AddPaperPage
    {
        public ExamProductionView ParentExamPage;
        public EditPaperView ParentEditPage;
        private CPaperParam _mCAddPaper = new CPaperParam();
        private PaperInfo _mAddPaperInfo;
        private int _mIntScoreOld;
        private int _mIntQuestionNum;

        public AddPaperPage()
        {
            _mIntScoreOld = 0;
            _mIntQuestionNum = 0;
            _mAddPaperInfo = S3602App.GPaperInfo;
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            Init(); 
        }

        private void Init()
        {
            PaperType temp = PaperType.TypeOne;
            if (_mAddPaperInfo.OptType == S3602Codes.OptUpdatePaper)
            {
                TbPaperName.Text = _mAddPaperInfo.PaperParam.StrName;
                TbDescribeConnect.Text = _mAddPaperInfo.PaperParam.StrDescribe;
                TbScores.Text = _mAddPaperInfo.PaperParam.IntScores.ToString();
                _mIntScoreOld = Convert.ToInt32(_mAddPaperInfo.PaperParam.IntScores.ToString());
                _mIntQuestionNum = Convert.ToInt32(_mAddPaperInfo.PaperParam.IntQuestionNum.ToString());
                TbTestTime.Text = _mAddPaperInfo.PaperParam.IntTestTime.ToString();
                TbPassMark.Text = _mAddPaperInfo.PaperParam.IntPassMark.ToString();
                temp = (PaperType)int.Parse(_mAddPaperInfo.PaperParam.CharType.ToString());
            }
            InitPaperType(temp);
            ChangeLanguage();
        }

        private void InitPaperType(PaperType paperType)
        {
            CbPaperType1.Content = CurrentApp.GetLanguageInfo("3602T00070", "1");
            CbPaperType2.Content = CurrentApp.GetLanguageInfo("3602T00071", "2");
            CbPaperType3.Content = CurrentApp.GetLanguageInfo("3602T00072", "3");
            CbPaperType4.Content = CurrentApp.GetLanguageInfo("3602T00073", "4");
            CbPaperType5.Content = CurrentApp.GetLanguageInfo("3602T00074", "5");
            switch (paperType)
            {
                case PaperType.TypeOne:
                    CbPaperType.SelectedIndex = 0;
                    break;
                case PaperType.TypeTwo:
                    CbPaperType.SelectedIndex = 1;
                    break;
                case PaperType.TypeThree:
                    CbPaperType.SelectedIndex = 2;
                    break;
                case PaperType.TypeFour:
                    CbPaperType.SelectedIndex = 3;
                    break;
                case PaperType.TypeFive:
                    CbPaperType.SelectedIndex = 4;
                    break;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            PaperName.Text = CurrentApp.GetLanguageInfo("3602T00038", "Paper Name");
            TestType.Text = CurrentApp.GetLanguageInfo("3602T00146", "Test Type");
            DescribeName.Text = CurrentApp.GetLanguageInfo("3602T00039", "Descride");
            ScoresName.Text = CurrentApp.GetLanguageInfo("3602T00043", "Score");
            TestTimeName.Text = CurrentApp.GetLanguageInfo("3602T00045", "Test Time");
            PassMarkName.Text = CurrentApp.GetLanguageInfo("3602T00044", "Pass Mark");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3602T00068", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3602T00069", "Close");
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

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               if(!CheckAllText())
                   return;
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    S3602App.GPaperInfo = new PaperInfo();
                    if (!SetPaperInfo())
                    {
                       ShowException("Set paper information error!");
                        return;
                    }

                    switch (_mAddPaperInfo.OptType)
                    {
                        case S3602Codes.OptAddPaper:
                            if (!EnablePaperName())
                                return;
                            long longPaperNum = GetPaperNum();
                            _mCAddPaper.LongNum = longPaperNum;
                            if (longPaperNum == 0)
                            {
                                ShowException("Get paper number error");
                                return;
                            }
                            if (!WritePaperInfo()) return;
                            S3602App.GPaperInfo.PaperParam = _mCAddPaper;
                            S3602App.GPaperInfo.OptType = S3602Codes.OptAddPaper;
                            ParentExamPage.AddPaperList();
                            S3602App.GPaperInfo = new PaperInfo
                            {
                                PaperParam = _mCAddPaper,
                                OptType = S3602Codes.OptEditPaper
                            };
                            ParentExamPage.OpenEditPaperPage();
                            break;
                        case S3602Codes.OptUpdatePaper:
                            if (_mIntQuestionNum != 0)
                            {
                                if (_mIntScoreOld != Convert.ToInt32(TbScores.Text))
                                {
                                    MessageBox.Show(
                                        CurrentApp.GetLanguageInfo("3602T00142",
                                            "Paper topic has finished can score distribution, to modify the score is less than the original score, whether or not to assign scores?"),
                                        CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                                        MessageBoxButton.OK);
                                }
                            }

                            _mCAddPaper.LongNum = _mAddPaperInfo.PaperParam.LongNum;
                            if (!UpdatePaperInfo()) return;
                            S3602App.GPaperInfo.PaperParam = _mCAddPaper;
                            S3602App.GPaperInfo.OptType = S3602Codes.OptUpdatePaper;
                            ParentEditPage.ChangePaperInfo(S3602App.GPaperInfo);
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

        private bool EnablePaperName()
        {
            try
            {
                var webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3602Codes.OptPaperSameName
                };
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //client = new Service36021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("{0}{1}",CurrentApp.GetLanguageInfo("3602T00076", "Failed to query the database!"), webReturn.Message));
                    return false;
                }
                if (webReturn.Message == S3602Consts.PaperExist)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00075", "Paper already exist"));
                    return false;
                }

            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private long GetPaperNum()
        {
            try
            {
                if (!S3602App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    WebRequest webRequest = new WebRequest
                    {
                        Session = CurrentApp.Session,
                        Code = (int) RequestCode.WSGetSerialID
                    };
                    //Service36021Client client = new Service36021Client();
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3602");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
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

        private bool WritePaperInfo()
        {
            string strLog;
            try
            {
                var webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3602Codes.OptAddPaper
                };
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));

                //client = new Service36021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00006", "Add Paper"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00006"),
                        Utils.FormatOptLogString("3602T00164"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}{1}", CurrentApp.GetLanguageInfo("3602T00066", "Insert data failed"),
                        webReturn.Message));
                    return false;
                }

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00006"),
                    Utils.FormatOptLogString("3602T00163"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00163", "Add Success"));
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00006"),
                    Utils.FormatOptLogString("3602T00164"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
                return false;
            }
            
            return true;
        }

        private bool SetPaperInfo()
        {
            _mCAddPaper = new CPaperParam
            {
                StrName = TbPaperName.Text.Trim(),
                StrDescribe = TbDescribeConnect.Text.Trim()
            };
            if (CbPaperType.SelectedIndex < 0)
            {
                return false;
            }
            switch (CbPaperType.SelectedIndex)
            {
                case 0:
                    _mCAddPaper.CharType = '1';
                    break;
                case 1:
                    _mCAddPaper.CharType = '2';
                    break;
                case 2:
                    _mCAddPaper.CharType = '3';
                    break;
                case 3:
                    _mCAddPaper.CharType = '4';
                    break;
                case 4:
                    _mCAddPaper.CharType = '5';
                    break;

            }
            _mCAddPaper.IntScores = Convert.ToInt32(TbScores.Text);
            _mCAddPaper.IntPassMark = Convert.ToInt32(TbPassMark.Text);
            _mCAddPaper.IntTestTime = Convert.ToInt32(TbTestTime.Text);
            _mCAddPaper.LongEditorId = Convert.ToInt64(CurrentApp.Session.UserInfo.UserID);
            _mCAddPaper.StrEditor = CurrentApp.Session.UserInfo.UserName;
            _mCAddPaper.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (_mAddPaperInfo.OptType == S3602Codes.OptUpdatePaper)
            {
                _mCAddPaper.IntQuestionNum = _mAddPaperInfo.PaperParam.IntQuestionNum;
                _mCAddPaper.IntUsed = _mAddPaperInfo.PaperParam.IntUsed;
                _mCAddPaper.IntAudit = _mAddPaperInfo.PaperParam.IntAudit;
                _mCAddPaper.LongVerifierId = _mAddPaperInfo.PaperParam.LongVerifierId;
                _mCAddPaper.StrVerifier = _mAddPaperInfo.PaperParam.StrVerifier;
                _mCAddPaper.IntIntegrity = _mIntScoreOld != Convert.ToInt32(TbScores.Text) ? 0 : _mAddPaperInfo.PaperParam.IntIntegrity;
            }
            return true;
        }

        private bool UpdatePaperInfo()
        {
            string strLog;
            try
            {
                var webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3602Codes.OptUpdatePaper
                };
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //client = new Service36021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00008", "Change"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00008"),
                        Utils.FormatOptLogString("3602T00161"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("3602T00076", "Insert data failed"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00008"),
                    Utils.FormatOptLogString("3602T00162"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00162", "Change Success"));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00008"),
                    Utils.FormatOptLogString("3602T00161"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void TbTestTime_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt32(TbTestTime.Text) <= 0 || Convert.ToInt32(TbTestTime.Text) > 1440)
                {
                    TbTestTime.Text = "60";
                    ShowException(CurrentApp.GetLanguageInfo("3602T00158", "The test time setting range (10-1440)"));
                }
            }
            catch {  TbTestTime.Text = "60";}
        }

        private void TbScores_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbScores.Text) < 0 || Convert.ToInt64(TbScores.Text) > 500)
                {
                    TbScores.Text = "500";
                    ShowException(CurrentApp.GetLanguageInfo("3602T00157", "The scope of the score is 10 - 500"));
                }
            }
            catch { }
        }

        private void TbPassMark_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt32(TbPassMark.Text) < 0 || Convert.ToInt32(TbPassMark.Text) > Convert.ToInt32(TbScores.Text))
                {
                    TbPassMark.Text = TbScores.Text;
                    ShowException(CurrentApp.GetLanguageInfo("3602T00159", "Exam qualification points set range (10-500)"));
                }
            }
            catch { TbPassMark.Text = TbScores.Text; }
        }

        private void TbPaperName_OnTextChanged(object sender, TextChangedEventArgs e)
        {           
                if (TbPaperName.Text.Length > 128)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00183", "Paper name cannot be more than 128 characters!"));
                }                 
        }

        private void TbDescribeConnect_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbDescribeConnect.Text.Length > 2048)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3602T00184", "Paper describes cannot exceed 2048 characters!"));
            }
        }

        private bool CheckAllText()
        {
            try
            {
                if (string.IsNullOrEmpty(TbPaperName.Text.Trim()))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00176", "The examination paper name cannot be empty!!"));
                    TbPaperName.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(TbDescribeConnect.Text.Trim()))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00175", "Describe the content cannot be empty, please fill out the relevant description!"));
                    TbDescribeConnect.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(TbScores.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00190", "总分不能为空!!"));
                    return false;
                }
                if (string.IsNullOrEmpty(TbPassMark.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00191", "及格分不能为空!!"));
                    return false;
                }

                if (TbPaperName.Text.Length > 128)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00183", "Paper name cannot be more than 128 characters!"));
                    return false;
                }

                if (TbDescribeConnect.Text.Length > 2048)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00184", "Paper describes cannot exceed 2048 characters!"));
                    return false;
                }

                if (Convert.ToInt64(TbScores.Text) < 10 || Convert.ToInt64(TbScores.Text) > 500)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3602T00157", "The scope of the score is 10 - 500"));
                    return false;
                }


                if (Convert.ToInt32(TbTestTime.Text) < 10 || Convert.ToInt32(TbTestTime.Text) > 1440)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3602T00158", "The test time setting range (10-1440)"));
                    return false;
                }

                if (Convert.ToInt32(TbPassMark.Text) < 10 || Convert.ToInt32(TbPassMark.Text) > Convert.ToInt32(TbScores.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3602T00159", "Exam qualification points set range (10-500)"));
                    return false;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }

            return true;
        }

        
    }
}
