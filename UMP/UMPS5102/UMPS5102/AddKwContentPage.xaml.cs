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
    /// AddKwContent.xaml 的交互逻辑
    /// </summary>
    public partial class AddKwContentPage
    {
        public MainView ParentPage;
        private KwContentInfoParam _mKwConnectInfo;

        public AddKwContentPage()
        {
            InitializeComponent();
            _mKwConnectInfo = new KwContentInfoParam();
            Loaded += UCCustomSetting_Loaded;
        }
        private void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
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
                };
                worker.RunWorkerAsync();
                if (S5102App.GOptInfo == S5102Codes.OptAdd)
                {
                    _mKwConnectInfo = new KwContentInfoParam();
                }
                if (S5102App.GOptInfo == S5102Codes.OptChange)
                {
                    TxtKwContent.Text = S5102App.GKwConnectInfo.StrKwContent;
                    CbEnable.IsChecked = S5102App.GKwConnectInfo.State != 0;
                    _mKwConnectInfo = S5102App.GKwConnectInfo;
                }
            }
            catch (Exception ex) 
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            TbKwContent.Text = CurrentApp.GetLanguageInfo("5102T00028", "Ketword");
            CbEnable.Content = CurrentApp.GetLanguageInfo("5102T00025", "Enable");
            ButOk.Content = CurrentApp.GetLanguageInfo("5102T00031", "OK");
            ButCancel.Content = CurrentApp.GetLanguageInfo("5102T00027", "Cancel");
        }

        private void ButOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtKwContent.Text.Length > 128)
            {
                MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00066",
                "Add content should not exceed 128 characters."),
                CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                MessageBoxButton.OK);
                return;
            }
          
            bool bEnable = false;
            _mKwConnectInfo.State = Convert.ToInt16(CbEnable.IsChecked);
            if (string.IsNullOrEmpty(TxtKwContent.Text))
            {
                MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00061",
                    "Sure to restore to delete keywords content?"),
                    CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                    MessageBoxButton.OK);
                return;
            }
            _mKwConnectInfo.StrKwContent = TxtKwContent.Text;
            var local = DateTime.Now;
            var utc = local.ToUniversalTime();

            if (S5102App.GOptInfo == S5102Codes.OptAdd)
            {
                _mKwConnectInfo.LongKwContentNum = CreateNum();
                _mKwConnectInfo.StrAddUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                _mKwConnectInfo.StrAddLocalTime = local.ToString("yyyy-MM-dd HH:mm:ss");
                _mKwConnectInfo.StrAddPaperName = CurrentApp.Session.UserInfo.UserName;
                _mKwConnectInfo.LongAddPaperNum = CurrentApp.Session.UserInfo.UserID;
                _mKwConnectInfo.IntDelete = 0;
                bEnable = AddKwContent(_mKwConnectInfo);
            }
            if (S5102App.GOptInfo == S5102Codes.OptChange)
            {
                _mKwConnectInfo.StrChangeUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                _mKwConnectInfo.StrChangeLocalTime = local.ToString("yyyy-MM-dd HH:mm:ss");
                _mKwConnectInfo.StrChangePaperName = CurrentApp.Session.UserInfo.UserName;
                _mKwConnectInfo.LongChangePaperNum = CurrentApp.Session.UserInfo.UserID;
                bEnable = UpdateKwContent(_mKwConnectInfo);
            }

            if (bEnable)
            {
                ParentPage.SetKwContents(_mKwConnectInfo);
                var parant = Parent as PopupPanel;
                if (parant != null)
                {
                    parant.IsOpen = false;
                }
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
                    Service51021Client client =
                        new Service51021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
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

        private bool AddKwContent( KwContentInfoParam kwConnectInfoParam )
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S5102Codes.OptAddKwContent;
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
                ShowException( ex.Message );
                return false;
            }
            return true;
        }

        private bool UpdateKwContent(KwContentInfoParam kwConnectInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5102Codes.OptUpdateKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwConnectInfo);
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

        private void TxtKwContent_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TxtKwContent.Text.Length > 128)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5102T00066",
                    "Add content should not exceed 128 characters."),
                    CurrentApp.GetLanguageInfo("5102T00050", "Warning"),
                    MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                ShowException( ex.Message);
            }
        }
    }
}
