//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3622f88f-27bf-4d90-8f47-93f2bc6c0e33
//        CLR Version:              4.0.30319.18408
//        Name:                     LoginWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                LoginWindow
//
//        created by Charley at 2016/7/26 14:07:12
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace UMPUpdater
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow
    {

        #region Members

        public DatabaseInfo DatabaseInfo;
        public bool IsLogined;
        public UserInfo UserInfo;

        private bool mIsInited;

        #endregion
       

        public LoginWindow()
        {
            InitializeComponent();

            MouseLeftButtonDown += (s, e) => DragMove();
            Loaded += LoginWindow_Loaded;

            BtnAppClose.Click += BtnAppClose_Click;
            BtnAppMenu.Click += BtnAppMenu_Click;
            BtnAppMinimize.Click += BtnAppMinimize_Click;

            BtnLogin.Click += BtnLogin_Click;
            BtnCancel.Click += BtnCancel_Click;
        }


        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
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
                TxtAccount.Text = string.Empty;
                TxtPassword.Password = string.Empty;
                if (DatabaseInfo == null) { return; }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        private void Login()
        {
            try
            {
                if (DatabaseInfo == null)
                {
                    ShowException(string.Format("Fail.\t DatabaseInfo is null"));
                    return;
                }

                string strAccount = TxtAccount.Text.Trim();
                string strPassword = TxtPassword.Password.Trim();

                if (string.IsNullOrEmpty(strAccount)
                    || string.IsNullOrEmpty(strPassword))
                {
                    ShowException(string.Format("Account or password empty!"));
                    return;
                }

                SetBusy(true, App.GetLanguageInfo("N008",string.Format("Checking login information, please wait for a moment...")));
                bool isFail = true;
                string strError = string.Empty;
                OperationReturn optReturn;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        string strAccountEncrypt = App.EncryptStringM002(strAccount);
                        int dbType = DatabaseInfo.TypeID;
                        string strSql;
                        string strConn = DatabaseInfo.GetConnectionString();
                        string strRentToken = string.Format("00000");
                        switch (dbType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C002 = '{1}'",
                                    strRentToken,
                                    strAccountEncrypt);
                                optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C002 = '{1}'",
                                   strRentToken,
                                   strAccountEncrypt);
                                optReturn = OracleOperation.GetDataSet(strConn, strSql);
                                break;
                            default:
                                strError = string.Format("Database type not support.\t{0}", dbType);
                                return;
                        }
                        if (!optReturn.Result)
                        {
                            strError = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message);
                            return;
                        }
                        DataSet objDataSet = optReturn.Data as DataSet;
                        if (objDataSet == null 
                            || objDataSet.Tables.Count <= 0)
                        {
                            strError = string.Format("DataSet is null");
                            return;
                        }
                        if (objDataSet.Tables[0].Rows.Count <= 0)
                        {
                            strError = string.Format("Account not exist.\t{0}", strAccount);
                            return;
                        }
                        DataRow dr = objDataSet.Tables[0].Rows[0];
                        long userID = Convert.ToInt64(dr["C001"]);
                        string strPass = dr["C004"].ToString();
                        string strTemp = string.Format("{0}{1}", userID, strPassword);
                        byte[] byteTemp = ServerHashEncryption.EncryptBytes(Encoding.Unicode.GetBytes(strTemp),
                            EncryptionMode.SHA512V00Hex);
                        var aes = ServerAESEncryption.EncryptBytes(byteTemp, EncryptionMode.AES256V02Hex);
                        strTemp = ServerEncryptionUtils.Byte2Hex(aes);
                        if (!strTemp.Equals(strPass))
                        {
                            strError = string.Format("Password error.");
                            return;
                        }
                        IsLogined = true;
                        UserInfo userInfo=new UserInfo();
                        userInfo.UserID = userID;
                        userInfo.Account = strAccount;
                        userInfo.Password = strPassword;
                        UserInfo = userInfo;
                        isFail = false;
                    }
                    catch (Exception ex)
                    {
                        strError = string.Format("Fail.\t{0}", ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    if (isFail)
                    {
                        ShowException(strError);
                        return;
                    }
                    DialogResult = true;
                    Close();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region AppBtn

        void BtnAppMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void BtnAppMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnAppClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion


        #region EventHandlers

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        #endregion


        #region Basics

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetBusy(bool isWork, string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                WorkWaiter.Visibility = isWork ? Visibility.Visible : Visibility.Collapsed;
                TxtStatusContent.Text = msg;
            }));
        }

        #endregion


        #region ChangeLanguage

        public void ChangeLanguage()
        {
            try
            {
                TxtTitle.Text = App.GetLanguageInfo("T019", "Login UMP");
                LbInputAccount.Text = App.GetLanguageInfo("T020", "Please input administrator's login information of UMP");
                LbAccount.Text = App.GetLanguageInfo("T021", "Account");
                LbPassword.Text = App.GetLanguageInfo("T022", "Password");

                BtnLogin.Content = App.GetLanguageInfo("B007", "Login");
                BtnCancel.Content = App.GetLanguageInfo("B008", "Cancel");
            }
            catch { }
        }

        #endregion

    }
}
