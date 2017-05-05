//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f9aeb1f6-b6c6-40ae-a30c-440263346b70
//        CLR Version:              4.0.30319.42000
//        Name:                     UCChangePassword
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1202
//        File Name:                UCChangePassword
//
//        created by Charley at 2016/3/31 11:06:42
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using UMPS1202.Wcf00000;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1202
{
    /// <summary>
    /// UCChangePassword.xaml 的交互逻辑
    /// </summary>
    public partial class UCChangePassword
    {

        public LoginView PageParent;
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
                TxtTip.Text = string.Format("Change password for user {0}", CurrentApp.Session.UserInfo.Account);
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
                listArgs.Add(CurrentApp.Session.RentInfo.Token);
                listArgs.Add(CurrentApp.Session.UserID.ToString());
                listArgs.Add(strOldPass);
                listArgs.Add(strNewPass);
                listArgs.Add(S1202App.LoginSessionID);

                for (int i = 0; i < listArgs.Count; i++)
                {
                    listArgs[i] = S1202App.EncryptString(listArgs[i]);
                }

                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs retArgs = client.OperationMethodA(14, listArgs);
                client.Close();
                string strReturn = retArgs.StringReturn;
                CurrentApp.WriteLog("ChangePassword", string.Format("Change password return: {0}", strReturn));
                if (!retArgs.BoolReturn)
                {
                    ShowException(CurrentApp.GetLanguageInfo("S0000039", "Change password fail."));
                    return;
                }
                strReturn = S1202App.DecryptString(strReturn);
                if (strReturn != "S01A01")
                {

                    #region 错误消息

                    //ShowException(CurrentApp.GetLanguageInfo("S0000039", "Change password fail."));
                    switch (strReturn)
                    {
                        case "W000E02":
                            ShowException(CurrentApp.GetLanguageInfo("S0000060", ""));
                            break;
                        case "W000E03":
                            ShowException(CurrentApp.GetLanguageInfo("S0000061", ""));
                            break;
                        case "W000E04":
                            ShowException(CurrentApp.GetLanguageInfo("S0000062", ""));
                            break;
                        case "W000E05":
                            ShowException(CurrentApp.GetLanguageInfo("S0000063", ""));
                            break;
                        case "W000E06":
                            ShowException(CurrentApp.GetLanguageInfo("S0000064", ""));
                            break;
                        case "W000E08":
                            ShowException(CurrentApp.GetLanguageInfo("S0000078", "Get password history fail."));
                            break;
                        case "W000E09":
                            ShowException(CurrentApp.GetLanguageInfo("S0000079", "Get security settings fail."));
                            break;
                        case "W000E10":
                            ShowException(CurrentApp.GetLanguageInfo("S0000080", "Save password to database fail."));
                            break;
                        case "W000E11":
                            ShowException(CurrentApp.GetLanguageInfo("S0000081", ""));
                            break;
                        case "W000E12":
                            ShowException(CurrentApp.GetLanguageInfo("S0000082", ""));
                            break;
                    }

                    #endregion

                    return;
                }
                MessageBox.Show(CurrentApp.GetLanguageInfo("S0000040", "Change password successful"), CurrentApp.AppTitle,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentApp.Session.UserInfo.Password = S1202App.EncryptString(strNewPass);


                #region 通知密码修改

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.SCGlobalSettingChanged;
                    webRequest.ListData.Add(ConstValue.GS_KEY_PARAM_PASSWORD);
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.Password);
                    webRequest.ListData.Add(string.Empty);
                    CurrentApp.PublishEvent(webRequest);
                }
                catch (Exception ex)
                {
                    CurrentApp.WriteLog("ChangePassword",
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
            try
            {
                string strOldPass = TxtOldPassword.Password.Trim();
                string strNewPass = TxtNewPassword.Password.Trim();
                string strConPass = TxtConfirmPassword.Password.Trim();
                string strPass = S1202App.DecryptString(CurrentApp.Session.UserInfo.Password);
                if (!strOldPass.Equals(strPass))
                {
                    ShowException(CurrentApp.GetLanguageInfo("S0000038", "Old password error!"));
                    return false;
                }
                if (!strNewPass.Equals(strConPass))
                {
                    ShowException(CurrentApp.GetLanguageInfo("S0000037", "Password not equal!"));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("S0000030", "Change password");
                }

                TxtTip.Text = string.Format(CurrentApp.GetLanguageInfo("S0000031", "Change password for user {0}"),
                    CurrentApp.Session.UserInfo.Account);

                LbOldPassword.Text = CurrentApp.GetLanguageInfo("S0000032", "Old password");
                LbNewPassword.Text = CurrentApp.GetLanguageInfo("S0000033", "New password");
                LbConfirmPassword.Text = CurrentApp.GetLanguageInfo("S0000034", "Confirm password");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("S0000035", "Confirm");
                BtnCancel.Content = CurrentApp.GetLanguageInfo("S0000036", "Cancel");
            }
            catch { }
        }
    }
}
