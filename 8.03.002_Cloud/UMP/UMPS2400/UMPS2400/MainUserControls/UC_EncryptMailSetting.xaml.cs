using Common2400;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS2400.Service11012;
using UMPS2400.Service24041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS2400.MainUserControls
{
    /// <summary>
    /// UC_EncryptMailSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptMailSetting
    {
        public EncryptMainPage parentWin = null;
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();
        BackgroundWorker mBackgroundWorker;
        EncryptEmail email = null;
        //在修改之前 是否启用了邮件通知
        int boIsEnableBefore = 0;

        public UC_EncryptMailSetting()
        {
            InitializeComponent();
            Loaded += UC_EncryptMailSetting_Loaded;
            radEnableMailNo.Checked += radEnableMailNo_Checked;
            radEnableMailYes.Checked += radEnableMailYes_Checked;
        }

        #region Load
        void UC_EncryptMailSetting_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentApp.WriteLog("UC_EncryptMailSetting_Loaded");
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
            {
                GetUserOpts();
                GetEmailInfo();
            };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
              {
                  InitLanguage();
                  InitControl();
                  CreateOptButtons(AllListOperations);
                  ChangeLanguage();
              };
            mBackgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Override
        public override void ChangeLanguage()
        {
            CurrentApp.WriteLog("UC_EncryptMailSetting ChangeLanguage");
            base.ChangeLanguage();
            InitLanguage();
        }
        #endregion

        #region Init
        private void InitLanguage()
        {
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("2404L010", "Encryption management -->reminder notification settings ");
            expMail.Header = CurrentApp.GetLanguageInfo("2404H001", "Key or encryption policy due to the expiration of the information mail notification settings");
            lblEnableMail.Content = CurrentApp.GetLanguageInfo("2404L001", "Enable email notification");
            lblEnableMailDesc.Content = CurrentApp.GetLanguageInfo("2404L002", "When there is a new key production or binding policy is about to expire, the system sends a mail notification");
            radEnableMailNo.Content = CurrentApp.GetLanguageInfo("2404R002", "No");
            radEnableMailYes.Content = CurrentApp.GetLanguageInfo("2404R001", "Yes");
            lblSMTP.Content = CurrentApp.GetLanguageInfo("2404L003", "Send mail server (SMTP)");
            lblSMTPPort.Content = CurrentApp.GetLanguageInfo("2404L004", "SMTP server port");
            lblNeedSSL.Content = CurrentApp.GetLanguageInfo("2404L005", "This server requires a secure connection.");
            lblNeedSSLDesc.Content = CurrentApp.GetLanguageInfo("2404L009", "This server requires a secure connection (SSL)");
            lblUser.Content = CurrentApp.GetLanguageInfo("2404L006", "Account");
            lblPwd.Content = CurrentApp.GetLanguageInfo("2404L007", "Password");
            lblNeedVerification.Content = CurrentApp.GetLanguageInfo("2404L011", "This service requires authentication");
            lblEmailAddress.Content = CurrentApp.GetLanguageInfo("2404L008", "Emaill Address");
            radNeedSSLYes.Content = CurrentApp.GetLanguageInfo("2404R001", "Yes");
            radNeedSSLNo.Content = CurrentApp.GetLanguageInfo("2404R002", "No");
            radNeedVerificationYes.Content = CurrentApp.GetLanguageInfo("2404R001", "Yes");
            radNeedVerificationNo.Content = CurrentApp.GetLanguageInfo("2404R002", "No");
            LbOperations.Text = CurrentApp.GetLanguageInfo("2404L012", "Operations");
          //  lblUserDesc.Content = CurrentApp.GetLanguageInfo("2404L013", "Sender mail address");
            lblSMTPDesc.Content = CurrentApp.GetLanguageInfo("2404L014", "Send mail server");
          //  lblPwdDesc.Content = CurrentApp.GetLanguageInfo("2404L015", "Sender mailbox password");
            lblSMTPPortDesc.Content = CurrentApp.GetLanguageInfo("2404L016", "Send mail server port");
            lblNeedVerificationDesc.Content = CurrentApp.GetLanguageInfo("2404L017", "Need to use a secure password to verify");
            lblEmailAddressDesc.Content = CurrentApp.GetLanguageInfo("2404L013", "Sender mail address");
        }

        private void InitControl()
        {
            if (email == null)
            {
                radEnableMailNo.IsChecked = true;
                return;
            }
            if(email.IsEnableEmail == "1" )
            {
                boIsEnableBefore = 1;
                radEnableMailYes.IsChecked = true;
            }
            else
            {
                radEnableMailNo.IsChecked = true;
            }
            txtEmailAddress.Text = email.EmailAddress;
            txtSMTP.Text = email.SMTP;
            txtSMTPPort.Text = email.SMTPPort;
            txtUser.Text = email.Account;
            if (email.NeedAuthentication == "1")
            {
                radNeedVerificationYes.IsChecked = true;
            }
            else
            {
                radNeedVerificationNo.IsChecked = true;
            }
            if (email.NeedSSL == "1")
            {
                radNeedSSLYes.IsChecked = true;
            }
            else
            {
                radNeedSSLNo.IsChecked = true;
            }
        }

        private void GetUserOpts()
        {
            Service11012Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("24");
                webRequest.ListData.Add("2404");
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                AllListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        AllListOperations.Add(optInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        private void CreateOptButtons(ObservableCollection<OperationInfo> InOpts)
        {
            PanelOperationButtons.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < InOpts.Count; i++)
            {
                item = InOpts[i];

                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOperationButtons.Children.Add(btn);

                //如果是“禁用“  就先创建按钮  在按钮下加一条线 
                if (item.ID == S2400Const.OPT_KeyGenServerDisable)
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Background = Brushes.LightGray;
                    txtBlock.Height = 2;
                    txtBlock.Margin = new Thickness(0, 10, 0, 10);
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    PanelOperationButtons.Children.Add(txtBlock);
                }
            }
        }

        private void GetEmailInfo()
        {
            Service24041Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetEmailInfo;
                webRequest.Session = CurrentApp.Session;
                client = new Service24041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24041"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (string.IsNullOrEmpty(webReturn.Data))
                {
                    email = null;
                }
                else
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<EncryptEmail>(webReturn.Data as string);
                    if (!optReturn.Result)
                    {
                        email = null;
                    }
                    else
                    {
                        email = optReturn.Data as EncryptEmail;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 禁用邮件通知的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radEnableMailNo_Checked(object sender, RoutedEventArgs e)
        {
            txtEmailAddress.IsEnabled = false;
            txtPwd.IsEnabled = false;
            txtSMTP.IsEnabled = false;
            txtSMTPPort.IsEnabled = false;
            txtUser.IsEnabled = false;
            radNeedSSLNo.IsEnabled = false;
            radNeedSSLYes.IsEnabled = false;
            radNeedVerificationNo.IsEnabled = false;
            radNeedVerificationYes.IsEnabled = false;
        }

        /// <summary>
        /// 启用邮件通知的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radEnableMailYes_Checked(object sender, RoutedEventArgs e)
        {
            txtEmailAddress.IsEnabled = true;
            txtPwd.IsEnabled = true;
            txtSMTP.IsEnabled = true;
            txtSMTPPort.IsEnabled = true;
            txtUser.IsEnabled = true;
            radNeedSSLNo.IsEnabled = true;
            radNeedSSLYes.IsEnabled = true;
            radNeedVerificationNo.IsEnabled = true;
            radNeedVerificationYes.IsEnabled = true;
            if (string.IsNullOrEmpty(txtSMTPPort.Text))
            {
                txtSMTPPort.Text = "25";
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                UpdateEmail();
            }
        }

        private void UpdateEmail()
        {
            List<string> lstParams = new List<string>();
            string str = radEnableMailYes.IsChecked == true ? "1" : "0";
            string strChangeEmailEnable = str;
            lstParams.Add(str);
            lstParams.Add(txtSMTP.Text);
            lstParams.Add(txtSMTPPort.Text);
            str = radNeedSSLYes.IsChecked == true ? "1" : "0";
            lstParams.Add(str);
            lstParams.Add(txtUser.Text);
            lstParams.Add(txtPwd.Password);
            str = radNeedVerificationYes.IsChecked == true ? "1" : "0";
            lstParams.Add(str);
            lstParams.Add(txtEmailAddress.Text);
            Service24041Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.UpdateEmailInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData = lstParams;
                client = new Service24041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24041"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                string strLog =string.Empty;
                if (!webReturn.Result)
                {
                    strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2404001")), string.Empty);
                    CurrentApp.WriteOperationLog("2404001", ConstValue.OPT_RESULT_FAIL, strLog);
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
               
                strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2404001")), string.Empty);
                if (boIsEnableBefore.ToString() != strChangeEmailEnable)
                {
                    if (strChangeEmailEnable == "1")
                    {
                        strLog += Utils.FormatOptLogString("2404001") + txtEmailAddress.Text;
                    }
                    else
                    {
                        strLog += Utils.FormatOptLogString("2404002");
                    }
                }
                CurrentApp.WriteOperationLog("2404001", ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("COMN001", "Successful operation"));
            }
            catch (Exception ex)
            {
                ShowException(CurrentApp.GetLanguageInfo("COMN002", "Operation failure") + ",error:" + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion

    }
}
