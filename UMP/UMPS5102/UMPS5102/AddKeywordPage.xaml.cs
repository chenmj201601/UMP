using System;
using System.Windows;
using System.Windows.Controls;
using Common5102;
using UMPS5102.Wcf51021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.ComponentModel;

namespace UMPS5102
{
    /// <summary>
    /// AddKeyword.xaml 的交互逻辑
    /// </summary>
    public partial class AddKeywordPage
    {
        public MainView ParentPage;
        public KeywordInfoParam _mKwInfo;
        public bool _mbSetAsKwConten;
        private KwContentInfoParam _mKwConnectInfo;

        public AddKeywordPage()
        {
            _mKwInfo = new KeywordInfoParam();
            _mKwConnectInfo = new KwContentInfoParam();
            _mbSetAsKwConten = false;
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        private void UCCustomSetting_Loaded( object sender, RoutedEventArgs e )
        {
            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            try
            {
                 BackgroundWorker worker = new BackgroundWorker();
                 worker.DoWork += (s, de) =>
                 {
                   
                 };
                 worker.RunWorkerCompleted += (s, re) =>
                 {
                     worker.Dispose();
                     if (S5102App.GOptInfo == S5102Codes.OptAdd)
                     {
                         CmbIcan.SelectedIndex = 0;
                         BorderKwInfo.Visibility = Visibility.Visible;
                     }
                     if (S5102App.GOptInfo == S5102Codes.OptChange)
                     {
                         _mKwInfo = S5102App.GKwInfo;
                         TxtKeyword.Text = S5102App.GKwInfo.StrKw;
                         CbEnable.IsChecked = S5102App.GKwInfo.State != 0;
                         SetImage();
                         BorderKwInfo.Visibility = Visibility.Collapsed;
                     }
                 };
                 worker.RunWorkerAsync();
            }
            catch (Exception ex) 
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            TbKeyword.Text = CurrentApp.GetLanguageInfo("5102T00007", "Ketword");
            TbIcan.Text = CurrentApp.GetLanguageInfo("5102T00023", "Level");
            CbEnableKwConnect.Content = CurrentApp.GetLanguageInfo("5102T00024", "Is the key words cotent");
            CbEnable.Content = CurrentApp.GetLanguageInfo("5102T00025", "Enable");
            ButNext.Content = CurrentApp.GetLanguageInfo("5102T00026", "Enable");
            ButCancel.Content = CurrentApp.GetLanguageInfo("5102T00027", "Cancel");
        }

        private void ButNext_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtKeyword.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                    MessageBoxButton.OK);
                    return;
                }

                bool bEnable = false;
                _mKwInfo.State = Convert.ToInt16(CbEnable.IsChecked);
                if (string.IsNullOrEmpty(TxtKeyword.Text))
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00060",
                        "Sure to restore to delete keywords content?"),
                        CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                        MessageBoxButton.OK);
                    return;
                }
                _mKwInfo.StrKw = TxtKeyword.Text;
                var local = DateTime.Now;
                var utc = local.ToUniversalTime();

                switch (CmbIcan.SelectedIndex)
                {
                    case 0:
                        _mKwInfo.StrImage = "00011.png";
                        _mKwInfo.StrImagePath = "/PublicFiles/S5102/00011.png";
                        break;
                    case 1:
                        _mKwInfo.StrImage = "00012.png";
                        _mKwInfo.StrImagePath = "/PublicFiles/S5102/00012.png";
                        break;
                    case 2:
                        _mKwInfo.StrImage = "00013.png";
                        _mKwInfo.StrImagePath = "/PublicFiles/S5102/00013.png";
                        break;
                    case 3:
                        _mKwInfo.StrImage = "00014.png";
                        _mKwInfo.StrImagePath = "/PublicFiles/S5102/00014.png";
                        break;
                    case 4:
                        _mKwInfo.StrImage = "00015.png";
                        _mKwInfo.StrImagePath = "/PublicFiles/S5102/00015.png";
                        break;
                }

                if (S5102App.GOptInfo == S5102Codes.OptAdd)
                {
                    _mKwInfo.LongKwNum = CreateNum();
                    _mKwInfo.StrAddUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                    _mKwInfo.StrAddLocalTime = local.ToString("yyyy-MM-dd HH:mm:ss");
                    _mKwInfo.StrAddPaperName = CurrentApp.Session.UserInfo.UserName;
                    _mKwInfo.LongAddPaperNum = CurrentApp.Session.UserInfo.UserID;
                    _mKwInfo.IntDelete = 0;
                    bEnable = AddKeyword(_mKwInfo);
                }
                if (S5102App.GOptInfo == S5102Codes.OptChange)
                {
                    _mKwInfo.StrChangeUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                    _mKwInfo.StrChangeLocalTime = local.ToString("yyyy-MM-dd HH:mm:ss");
                    _mKwInfo.StrChangePaperName = CurrentApp.Session.UserInfo.UserName;
                    _mKwInfo.LongChangePaperNum = CurrentApp.Session.UserInfo.UserID;
                    bEnable = UpdateKw(_mKwInfo);
                }

                if (bEnable)
                {
                    ParentPage.SetKeywords(_mKwInfo);
                    var parant = Parent as PopupPanel;
                    if (parant != null)
                    {
                        parant.IsOpen = false;
                    }

                    S5102App.GLongKeywordNum = _mKwInfo.LongKwNum;
                    OpenEditKwInfoPage();
                }

                if (CbEnableKwConnect.IsChecked.Value)
                {
                    CreateKwContent();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var parant = Parent as PopupPanel;
                if (parant != null)
                {
                    parant.IsOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void OpenEditKwInfoPage()
        {
            S5102App.GQueryModify = false;
            S5102App.GOptInfo = S5102Codes.OptEditKwContent;
            EditKwInfoPage newPage = new EditKwInfoPage
            {
                ParentPage = ParentPage,
                CurrentApp = CurrentApp
            };
            ParentPage.PopupPanel.Content = newPage;
            ParentPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("5102T00032", "Edit Content");
            ParentPage.PopupPanel.IsOpen = true;
        }

        private long CreateNum()
        {
            try
            {
                if (!S5102App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    WebRequest webRequest = new WebRequest
                    {
                        Session = CurrentApp.Session,
                        Code = (int)RequestCode.WSGetSerialID
                    };
                    webRequest.ListData.Add("51");
                    webRequest.ListData.Add("5102");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Service51021Client client = new Service51021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                         WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51021"));
                    //var client = new Service51021Client();
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

        private bool AddKeyword(KeywordInfoParam keywordInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5102Codes.OptAddKeyword;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(keywordInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51021"));
                //var client = new Service51021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00029", "Create"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5102T00029"),
                        Utils.FormatOptLogString("5102T00038"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5102Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(webReturn.Message);
                    return false;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5102T00029"),
                    Utils.FormatOptLogString("5102T00039"));
                CurrentApp.WriteOperationLog(S5102Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00039", "Create Success"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool UpdateKw(KeywordInfoParam kwInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5102Codes.OptUpdateKw;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51021"));
                //var client = new Service51021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5102T00045"),
                        Utils.FormatOptLogString("5102T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5102Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5102T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5102T00045"),
                    Utils.FormatOptLogString("5102T00047"));
                CurrentApp.WriteOperationLog(S5102Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void CreateKwContent()
        {
            _mKwConnectInfo.State = 1;
            _mKwConnectInfo.StrKwContent = TxtKeyword.Text;
            var local = DateTime.Now;
            var utc = local.ToUniversalTime();
            _mKwConnectInfo.LongKwContentNum = CreateNum();
            _mKwConnectInfo.StrAddUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
            _mKwConnectInfo.StrAddLocalTime = local.ToString("yyyy-MM-dd HH:mm:ss");
            _mKwConnectInfo.StrAddPaperName = CurrentApp.Session.UserInfo.UserName;
            _mKwConnectInfo.LongAddPaperNum = CurrentApp.Session.UserInfo.UserID;
            _mKwConnectInfo.IntDelete = 0;
           if (AddKwContent(_mKwConnectInfo))
            {
                ParentPage.SetKwContents(_mKwConnectInfo);
            }
        }

        private bool AddKwContent(KwContentInfoParam kwConnectInfoParam)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5102Codes.OptAddKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwConnectInfoParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51021"));
             //   var client = new Service51021Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00029", "Create"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5102T00029"),
                        Utils.FormatOptLogString("5102T00038"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5102Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(webReturn.Message);
                    return false;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5102T00029"),
                    Utils.FormatOptLogString("5102T00039"));
                CurrentApp.WriteOperationLog(S5102Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5102T00039", "Create Success"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void SetImage()
        {
            switch (S5102App.GKwInfo.StrImage)
            {
                case "00011.png":
                    CmbIcan.SelectedIndex = 0;
                    break;
                case "00012.png":
                    CmbIcan.SelectedIndex = 1;
                    break;
                case "00013.png":
                    CmbIcan.SelectedIndex = 2;
                    break;
                case "00014.png":
                    CmbIcan.SelectedIndex = 3;
                    break;
                case "00015.png":
                    CmbIcan.SelectedIndex = 4;
                    break;
            }
        }

        private void TxtKeyword_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TxtKeyword.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                    MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
