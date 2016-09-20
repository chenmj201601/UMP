using System;
using System.Windows;
using System.Windows.Controls;
using Common3601;
using UMPS3601.Wcf36011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3601
{
    /// <summary>
    /// AddPapers.xaml 的交互逻辑
    /// </summary>
    public partial class AddPapers
    {
        public ExamProductionView ParentPage;
        private CPaperParam _mCAddPaper = new CPaperParam();
        private PaperInfo _mAddPaperInfo;

        public AddPapers()
        {
            _mAddPaperInfo = S3601App.GPaperInfo;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            PaperType temp = PaperType.TypeOne;
            if (_mAddPaperInfo.OptType == S3601Codes.OperationUpdatePaper)
            {
                TbPaperName.Text = _mAddPaperInfo.PaperParam.StrName;
                TbDescribeConnect.Text = _mAddPaperInfo.PaperParam.StrDescribe;
                TbScores.Text = _mAddPaperInfo.PaperParam.IntScores.ToString();
                TbTestTime.Text = _mAddPaperInfo.PaperParam.IntTestTime.ToString();
                TbPassMark.Text = _mAddPaperInfo.PaperParam.IntPassMark.ToString();
                temp = (PaperType)int.Parse(_mAddPaperInfo.PaperParam.CharType.ToString());
            }
            InitPaperType(temp);
            ChangeLanguage();
        }

        private void InitPaperType(PaperType paperType )
        {
            CbPaperType1.Content = CurrentApp.GetLanguageInfo("3601T00070", "1");
            CbPaperType2.Content = CurrentApp.GetLanguageInfo("3601T00071", "2");
            CbPaperType3.Content = CurrentApp.GetLanguageInfo("3601T00072", "3");
            CbPaperType4.Content = CurrentApp.GetLanguageInfo("3601T00073", "4");
            CbPaperType5.Content = CurrentApp.GetLanguageInfo("3601T00074", "5");
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
            PaperName.Text = CurrentApp.GetLanguageInfo("3601T00038", "Paper Name");
            DescribeName.Text = CurrentApp.GetLanguageInfo("3601T00039", "Descride");
            ScoresName.Text = CurrentApp.GetLanguageInfo("3601T00043", "Score");
            TestTimeName.Text = CurrentApp.GetLanguageInfo("3601T00045", "Test Time");
            PassMarkName.Text = CurrentApp.GetLanguageInfo("3601T00044", "Pass Mark");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3601T00068", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3601T00069", "Close");
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

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(TbPassMark.Text) > Convert.ToInt32(TbScores.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00095", "Qualified points is not greater than total score!"));
                    return;
                }
                    

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    S3601App.GPaperInfo = new PaperInfo();
                    if (!SetPaperInfo())
                    {
                        ShowException(string.Format("Set paper information error!"));
                        return;
                    }

                    switch (_mAddPaperInfo.OptType)
                    {
                        case S3601Codes.OperationAddPaper:
                            if (!EnablePaperName())
                                return;
                            long longPaperNum = GetPaperNum();
                            _mCAddPaper.LongNum = longPaperNum;
                            if (longPaperNum == 0)
                            {
                                ShowException(string.Format("Get paper number error"));
                                return;
                            }
                            if (!WritePaperInfo()) return;
                            S3601App.GPaperInfo.PaperParam = _mCAddPaper;
                            S3601App.GPaperInfo.OptType = S3601Codes.OperationAddPaper;
                            ParentPage.AddPaperList();
                            S3601App.GPaperInfo = new PaperInfo();
                            S3601App.GPaperInfo.PaperParam = _mCAddPaper;
                            S3601App.GPaperInfo.OptType = S3601Codes.OperationEditPaper;
                            ParentPage.OpenEditPaperPage();
                            break;
                        case S3601Codes.OperationUpdatePaper:
                            _mCAddPaper.LongNum = _mAddPaperInfo.PaperParam.LongNum;
                            if(!UpdatePaperInfo()) return;
                            S3601App.GPaperInfo.PaperParam = _mCAddPaper;
                            S3601App.GPaperInfo.OptType = S3601Codes.OperationUpdatePaper;
                            ParentPage.AddPaperList();
                            break;
                    }
                    parent.IsOpen = false;
                }
            }
            catch(Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool EnablePaperName()
        {
            try
            {
                WebRequest webRequest;
                Service36011Client client;
                WebReturn webReturn;
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3601Codes.OperationPaperSameName;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
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
                    ShowException(CurrentApp.GetLanguageInfo("3601T00076", "Failed to query the database!"));
                    return false;
                }
                if (webReturn.Message == S3601Consts.PaperExist)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00075", "Paper already exist"));
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
                if (!S3601App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3601");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Service36011Client client =
                        new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                    //var client = new Service36011Client();
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
            try
            {
                WebRequest webRequest;
                Service36011Client client;
                WebReturn webReturn;
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationAddPaper;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
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
                    ShowException(CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"));
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

        private bool UpdatePaperInfo()
        {
            try
            {
                WebRequest webRequest;
                Service36011Client client;
                WebReturn webReturn;
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationUpdatePaper;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mCAddPaper);
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
                    ShowException(CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"));
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

        private bool SetPaperInfo()
        {
            _mCAddPaper = new CPaperParam();
            _mCAddPaper.StrName = TbPaperName.Text;
            _mCAddPaper.StrDescribe = TbDescribeConnect.Text;
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
            _mCAddPaper.LongEditorId = Convert.ToInt64( CurrentApp.Session.UserInfo.UserID );
            _mCAddPaper.StrEditor = CurrentApp.Session.UserInfo.UserName;
            _mCAddPaper.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (_mAddPaperInfo.OptType == S3601Codes.OperationUpdatePaper)
            {
                _mCAddPaper.IntQuestionNum = _mAddPaperInfo.PaperParam.IntQuestionNum;
                _mCAddPaper.IntUsed = _mAddPaperInfo.PaperParam.IntUsed;
                _mCAddPaper.IntAudit = _mAddPaperInfo.PaperParam.IntAudit;
                _mCAddPaper.LongVerifierId = _mAddPaperInfo.PaperParam.LongVerifierId;
                _mCAddPaper.StrVerifier = _mAddPaperInfo.PaperParam.StrVerifier;
            }
            return true;
        }

        private void TbTestTime_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbTestTime.Text) < 0 || Convert.ToInt64(TbTestTime.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                }
            }
            catch
            {

            }
        }

        private void TbPassMark_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbPassMark.Text) < 0 || Convert.ToInt64(TbPassMark.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                }
            }
            catch
            {

            }
        }

        private void TbScores_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbScores.Text) < 0 || Convert.ToInt64(TbScores.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                }
            }
            catch
            {

            }
        }
    }
}
