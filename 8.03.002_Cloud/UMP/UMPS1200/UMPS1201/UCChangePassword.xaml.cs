//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2d45cbdc-1dff-40f1-8241-d40e8a9b2f7a
//        CLR Version:              4.0.30319.18408
//        Name:                     UCChangePassword
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                UCChangePassword
//
//        created by Charley at 2016/4/10 19:21:41
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using UMPS1201.Wcf00000;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1201
{
    /// <summary>
    /// UCChangePassword.xaml 的交互逻辑
    /// </summary>
    public partial class UCChangePassword
    {
        public Shell PageParent;
        public string LoginReturnCode;

        private bool mIsInited;
        private string mOptResult;

        public const string RESULT_CANCEL = "Cancel";
        public const string RESULT_SUCC = "Succ";

        public UCChangePassword()
        {
            InitializeComponent();

            Loaded += UCChangePassword_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnCancel.Click += BtnCancel_Click;
            TxtOldPassword.KeyDown += TxtOldPassword_KeyDown;
            TxtNewPassword.KeyDown += TxtNewPassword_KeyDown;
            TxtConfirmPassword.KeyDown += TxtConfirmPassword_KeyDown;
        }

        void UCChangePassword_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                TxtTip.Text = string.Format("Change password for user {0}", App.Session.UserInfo.Account);
                TxtOldPassword.Focus();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            mOptResult = RESULT_CANCEL;
            if (PageParent != null)
            {
                PageParent.OnChangePasswordResult(mOptResult);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) { return; }
            try
            {
                string strOldPass = TxtOldPassword.Password.Trim();
                string strNewPass = TxtNewPassword.Password.Trim();

                List<string> listArgs = new List<string>();
                listArgs.Add(App.Session.RentInfo.Token);
                listArgs.Add(App.Session.UserID.ToString());
                listArgs.Add(strOldPass);
                listArgs.Add(strNewPass);
                listArgs.Add(string.Empty);

                for (int i = 0; i < listArgs.Count; i++)
                {
                    listArgs[i] = App.EncryptString(listArgs[i]);
                }

                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs retArgs = client.OperationMethodA(14, listArgs);
                client.Close();
                string strReturn = retArgs.StringReturn;
                App.WriteLog("ChangePassword", string.Format("Change password return: {0}", strReturn));
                if (!retArgs.BoolReturn)
                {
                    ShowException(App.GetLanguageInfo("S0000039", "Change password fail."));
                    return;
                }
                strReturn = App.DecryptString(strReturn);
                if (strReturn != "S01A01")
                {

                    #region 错误消息

                    //ShowException(CurrentApp.GetLanguageInfo("S0000039", "Change password fail."));
                    switch (strReturn)
                    {
                        case "W000E02":
                            ShowException(App.GetLanguageInfo("S0000060", ""));
                            break;
                        case "W000E03":
                            ShowException(App.GetLanguageInfo("S0000061", ""));
                            break;
                        case "W000E04":
                            ShowException(App.GetLanguageInfo("S0000062", ""));
                            break;
                        case "W000E05":
                            ShowException(App.GetLanguageInfo("S0000063", ""));
                            break;
                        case "W000E06":
                            ShowException(App.GetLanguageInfo("S0000064", ""));
                            break;
                        case "W000E08":
                            ShowException(App.GetLanguageInfo("S0000078", "Get password history fail."));
                            break;
                        case "W000E09":
                            ShowException(App.GetLanguageInfo("S0000079", "Get security settings fail."));
                            break;
                        case "W000E10":
                            ShowException(App.GetLanguageInfo("S0000080", "Save password to database fail."));
                            break;
                        case "W000E11":
                            ShowException(App.GetLanguageInfo("S0000081", ""));
                            break;
                        case "W000E12":
                            ShowException(App.GetLanguageInfo("S0000082", ""));
                            break;
                    }

                    #endregion

                    return;
                }
                MessageBox.Show(App.GetLanguageInfo("S0000040", "Change password successful"), App.AppTitle,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                App.Session.UserInfo.Password = App.EncryptString(strNewPass);


                #region 通知密码修改

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)RequestCode.SCGlobalSettingChanged;
                    webRequest.ListData.Add(ConstValue.GS_KEY_PARAM_PASSWORD);
                    webRequest.ListData.Add(App.Session.UserInfo.Password);
                    webRequest.ListData.Add(string.Empty);
                    if (PageParent != null)
                    {
                        PageParent.PublishEvent(webRequest);
                    }
                }
                catch (Exception ex)
                {
                    App.WriteLog("ChangePassword",
                        string.Format("Send change password notification fail.\t{0}", ex.Message));
                }

                #endregion


                mOptResult = RESULT_SUCC;
                if (PageParent != null)
                {
                    PageParent.OnChangePasswordResult(mOptResult);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtConfirmPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnConfirm_Click(this, new RoutedEventArgs());
            }
        }

        void TxtNewPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtConfirmPassword.Focus();
            }
        }

        void TxtOldPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtNewPassword.Focus();
            }
        }

        private bool CheckInput()
        {
            string strOldPass = TxtOldPassword.Password.Trim();
            string strNewPass = TxtNewPassword.Password.Trim();
            string strConPass = TxtConfirmPassword.Password.Trim();
            string strPass = App.DecryptString(CurrentApp.Session.UserInfo.Password);
            if (!strOldPass.Equals(strPass))
            {
                ShowException(CurrentApp.GetLanguageInfo("S0000038", "Old password error!"));
                return false;
            }
            if (!strNewPass.Equals(strConPass))
            {
                ShowException(App.GetLanguageInfo("S0000037", "Password not equal!"));
                return false;
            }
            return true;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = App.GetLanguageInfo("S0000030", "Change password");
                }

                TxtTip.Text = string.Format(App.GetLanguageInfo("S0000031", "Change password for user {0}"),
                    App.Session.UserInfo.Account);

                LbOldPassword.Text = App.GetLanguageInfo("S0000032", "Old password");
                LbNewPassword.Text = App.GetLanguageInfo("S0000033", "New password");
                LbConfirmPassword.Text = App.GetLanguageInfo("S0000034", "Confirm password");

                BtnConfirm.Content = App.GetLanguageInfo("S0000035", "Confirm");
                BtnCancel.Content = App.GetLanguageInfo("S0000036", "Cancel");
            }
            catch { }
        }
    }
}
