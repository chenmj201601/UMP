//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    efd33d9c-7b7f-4013-b339-8afbd0908c24
//        CLR Version:              4.0.30319.42000
//        Name:                     LoginView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1202
//        File Name:                LoginView
//
//        created by Charley at 2016/1/22 11:15:33
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UMPS1202.Wcf00000;
using UMPS1202.Wcf11012;
using UMPS1202.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;

namespace UMPS1202
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView
    {

        #region Memebers

        private bool mIsForceLogin;
        private List<LangTypeInfo> mSupportLangTypes;
        private List<RoleInfo> mListUserRoles;
        private List<GlobalParamInfo> mListGlobalParamInfos;
        private List<BasicDomainInfo> mListDomainInfos;
        private Thread mThreadCheckUserStatus;
        private string mLoginReturnCode;
        private string mLogoutReason;
        private int mIdleTimout;
        private string mUserPassword;       //域帐号登陆时，从数据库返回的用户的系统密码，不是域账户密码

        public const string LOGOUT_REASON_CLOSEBROWSER = "LR01";    //关闭浏览器
        public const string LOGOUT_REASON_NORMAL = "LR02";          //普通点击注销按钮
        public const string LOGOUT_REASON_TIMEOUT = "LR03";         //超出最大登录时间自动注销
        public const string LOGOUT_REASON_OTHERLOGIN = "LR04";      //从其他地方登录
        public const string LOGOUT_REASON_IDLETIMEOUT = "LR05";     //超出空闲时间自动注销

        #endregion


        public LoginView()
        {
            InitializeComponent();

            mSupportLangTypes = new List<LangTypeInfo>();
            mListUserRoles = new List<RoleInfo>();
            mListGlobalParamInfos = new List<GlobalParamInfo>();
            mListDomainInfos = new List<BasicDomainInfo>();

            BtnLoginSystem.Click += BtnLoginSystem_Click;
            BtnLoginOption.Click += BtnLoginOption_Click;
            TxtLoginAccount.KeyDown += TxtLoginAccount_KeyDown;
            TxtLoginPassword.KeyDown += TxtLoginPassword_KeyDown;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "LoginView";
                StylePath = "UMPS1202/LoginView.xaml";

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadSupportLanguageTypes();
                    InitSupportLangTypes();
                    LoadDomainInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitLoginOptionMenu();
                    ChangeLanguage();

                    //LDAP 自动登录判断
                    CheckLDAPLogin();

                    TxtLoginAccount.Focus();
                };
                worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadSupportLanguageTypes()
        {
            try
            {
                mSupportLangTypes.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetSupportLangTypeList;
                webRequest.ListData.Add("0");
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("LoadLangType",
                        string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    CurrentApp.WriteLog("LoadLangType", string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<LangTypeInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadLangType",
                            string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LangTypeInfo info = optReturn.Data as LangTypeInfo;
                    if (info == null)
                    {
                        CurrentApp.WriteLog("LoadLangType", string.Format("Fail.\tLangTypeInfo is null"));
                        return;
                    }
                    mSupportLangTypes.Add(info);
                }

                CurrentApp.WriteLog("LoadLangType", string.Format("Load end.\t{0}", mSupportLangTypes.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserRoles()
        {
            try
            {
                mListUserRoles.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserRoleList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RoleInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RoleInfo info = optReturn.Data as RoleInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("RoleInfo is null"));
                        return;
                    }
                    long roleID = info.ID;
                    if (roleID == 1060000000000000004)
                    {
                        //坐席角色是内置角色，此角色不允许登录UMP，过滤掉     by charley at 2016/6/1
                        continue;
                    }
                    mListUserRoles.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadGlobalParamInfos()
        {
            try
            {
                mListGlobalParamInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList2;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(ConstValue.GP_GROUP_USER_LOGOUT.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo info = optReturn.Data as GlobalParamInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("GlobalParamInfo is null"));
                        return;
                    }
                    mListGlobalParamInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadGlobalParams", string.Format("Load end.\t{0}", mListGlobalParamInfos.Count));

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadDomainInfos()
        {
            try
            {
                mListDomainInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetDomainInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<BasicDomainInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicDomainInfo info = optReturn.Data as BasicDomainInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("DomainInfo is null"));
                        return;
                    }
                    mListDomainInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadDomainInfos", string.Format("End.\t{0}", mListDomainInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnLoginOption_Click(object sender, RoutedEventArgs e)
        {
            if (BtnLoginOption.ContextMenu != null)
            {
                BtnLoginOption.ContextMenu.PlacementTarget = BtnLoginOption;
                BtnLoginOption.ContextMenu.Placement = PlacementMode.Bottom;
                BtnLoginOption.ContextMenu.IsOpen = true;
            }
        }

        void BtnLoginSystem_Click(object sender, RoutedEventArgs e)
        {
            DoLoginSystem();
        }

        void TxtLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    DoLoginSystem();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtLoginAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TxtLoginPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void DoLoginSystem()
        {
            try
            {
                string strAccount = TxtLoginAccount.Text.Trim();
                string strPassword = TxtLoginPassword.Password.Trim();
                if (string.IsNullOrEmpty(strAccount))
                {
                    ShowException(CurrentApp.GetLanguageInfo("S0000004", "Account can not be empty!"));
                    return;
                }
                S1202App.IsLDAPAccount = strAccount.Contains("\\");
                if (S1202App.IsLDAPAccount)
                {
                    string[] arrInfos = strAccount.Split(new[] { '\\' }, StringSplitOptions.None);
                    if (arrInfos.Length > 0)
                    {
                        string strDomainName = arrInfos[0];
                        var domain = mListDomainInfos.FirstOrDefault(d => d.Name.ToLower().Equals(strDomainName.ToLower()));
                        if (domain == null)
                        {
                            ShowException(string.Format("Domain not exist.\t{0}", strDomainName));
                            return;
                        }
                        S1202App.LoginDomainName = strDomainName;

                        if (CurrentApp != null
                           && CurrentApp.Session != null)
                        {
                            var domainInfo = CurrentApp.Session.DomainInfo;
                            if (domainInfo == null)
                            {
                                domainInfo = new DomainInfo();
                            }
                            domainInfo.DomainID = domain.ObjID;
                            domainInfo.Name = strDomainName;
                            domainInfo.FullName = strDomainName;
                            domainInfo.AllowAutoLogin = domain.AllowAutoLogin;
                            CurrentApp.Session.DomainInfo = domainInfo;
                            CurrentApp.Session.DomainID = domainInfo.DomainID;
                        }
                    }
                }
                string strLoginType = mIsForceLogin ? "F" : "N";
                DoLoginSystem(strAccount, strPassword, strLoginType);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoLoginSystem(string strAccount, string strPassword, string strLoginType)
        {
            try
            {
                string strHostName = Environment.MachineName;

                List<string> listArgs = new List<string>();
                listArgs.Add(strAccount);
                listArgs.Add(strPassword);
                listArgs.Add(strLoginType);
                listArgs.Add(strHostName);

                for (int i = 0; i < listArgs.Count; i++)
                {
                    listArgs[i] = S1202App.EncryptString(listArgs[i]);
                }

                MyWaiter.Visibility = Visibility.Visible;
                OperationDataArgs optReturn = null;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service00000Client client = new Service00000Client(
                            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                        optReturn = client.OperationMethodA(11, listArgs);
                        if (S1202App.IsLDAPAccount)
                        {
                            if (!S1202App.IsLDAPAutoLogin)
                            {
                                //LDAP账户非自动登录，获取账户在UMP系统中的密码
                                string strUser = strAccount.Replace("\\", "@");
                                OperationReturn optReturn1 = GetLDAPUserInfo(strUser);
                                if (!optReturn1.Result)
                                {
                                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn1.Code, optReturn1.Message));
                                    return;
                                }
                                UserInfo userInfo = optReturn1.Data as UserInfo;
                                if (userInfo == null)
                                {
                                    ShowException(string.Format("UserInfo is null"));
                                    return;
                                }
                                string str = userInfo.Password;
                                str = S1202App.DecryptString(str);
                                if (str.Length <= 19)
                                {
                                    ShowException(string.Format("Password length invalid."));
                                    return;
                                }
                                mUserPassword = str.Substring(19);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;

                    try
                    {
                        if (optReturn == null) { return; }
                        if (!optReturn.BoolReturn)
                        {
                            ShowException(CurrentApp.GetLanguageInfo("S0000029", "Login failed!"));
                            return;
                        }
                        string strReturn = optReturn.StringReturn;
                        CurrentApp.WriteLog("LoginSystem", string.Format("LoginReturn:{0}", strReturn));
                        string[] arrReturn = strReturn.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                        List<string> listReturn = new List<string>();
                        for (int i = 0; i < arrReturn.Length; i++)
                        {
                            listReturn.Add(S1202App.DecryptString(arrReturn[i]));
                        }
                        if (listReturn.Count < 1)
                        {
                            ShowException(string.Format("{0}\r\n\r\n{1}",
                                CurrentApp.GetLanguageInfo("S0000029", "Login failed!"), "ReturnArgs count invalid"));
                            return;
                        }
                        string strReturnCode = listReturn[0];

                        if (strReturnCode.StartsWith("E01"))
                        {

                            #region 登录错误消息

                            //ShowException(string.Format("Login failed.\t{0}", strReturnCode));
                            switch (strReturnCode)
                            {
                                case "E01A01":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000005", "Get RentInfo failed!"));
                                    break;
                                case "E01A02":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000006", "RentInfo not exist!"));
                                    break;
                                case "E01A03":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000007", "Get RentInfo ID failed!"));
                                    break;
                                case "E01A04":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000008", "RentInfo not in valid time!"));
                                    break;
                                case "E01A05":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000072", "Out online user count!"));
                                    break;
                                case "E01A06":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000073", "License expired!"));
                                    break;
                                case "E01A11":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000009", "Get Security failed!"));
                                    break;
                                case "E01A13":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000010", "Get UserInfo failed!"));
                                    break;
                                case "E01A21":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000010", "Get UserInfo failed!"));
                                    break;
                                case "E01A22":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000014", "User is locked by administror!"));
                                    break;
                                case "E01A23":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000011", "User is deleted!"));
                                    break;
                                case "E01A24":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000012", "User is not actived!"));
                                    break;
                                case "E01A25":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000013", "User not in valid time!"));
                                    break;
                                case "E01A26":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000016", "User not exist or password error!"));
                                    break;
                                case "E01A27":
                                case "E01A28":
                                case "E01A29":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000017", "User is locked by system!"));
                                    break;
                                case "E01A31":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000019", "Get LoginID failed!"));
                                    break;
                                case "E01A32":
                                    string strTime = string.Empty;
                                    string strName = string.Empty;
                                    string strAddress = string.Empty;
                                    if (listReturn.Count > 1)
                                    {
                                        strTime = listReturn[1];
                                    }
                                    if (listReturn.Count > 2)
                                    {
                                        strName = listReturn[2];
                                    }
                                    if (listReturn.Count > 3)
                                    {
                                        strAddress = listReturn[3];
                                    }
                                    DateTime dtValue;
                                    if (DateTime.TryParse(strTime, out dtValue))
                                    {
                                        strTime = dtValue.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    ShowException(
                                        string.Format(
                                            CurrentApp.GetLanguageInfo("S0000020", "User is logined on other machine!"),
                                            strTime, strName, strAddress));
                                    break;
                                case "E01A33":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000021", "Force login failed!"));
                                    break;
                                case "E01A41":
                                    ShowException(CurrentApp.GetLanguageInfo("S0000018", "Out times!"));
                                    break;
                                case "E01A53":
                                    string strDays = string.Empty;
                                    if (listReturn.Count > 1)
                                    {
                                        strDays = listReturn[1];
                                    }
                                    ShowException(
                                        string.Format(
                                            CurrentApp.GetLanguageInfo("S0000090", "No logined for prefered days"),
                                            strDays));
                                    break;
                                default:
                                    var str = GetErrorMessage(strReturnCode);
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        ShowException(str);
                                    }
                                    else
                                    {
                                        ShowException(string.Format("{0}\r\n\r\n{1}",
                                            CurrentApp.GetLanguageInfo("S0000029", "Login failed!"),
                                            strReturnCode));
                                    }
                                    break;
                            }
                            return;

                            #endregion

                        }
                        if (strReturnCode.StartsWith("S01"))
                        {
                            //ListReturn
                            //0     Code(S01A00)
                            //1     租户编码（5位）
                            //2     用户编码（19位102）
                            //3     登录流水号（19位903）
                            //4     用户全名
                            //5     [S01A01(新用户，强制修改密码);S01A02(密码过期，强制修改密码);S01A03(密码即将过期，建议修改密码)]
                            //6     密码过期剩余天数
                            if (listReturn.Count < 5)
                            {
                                ShowException(string.Format("Return UserInfo count invalid"));
                                return;
                            }
                            string strRentToken = listReturn[1];
                            string strUserID = listReturn[2];
                            string strLoginID = listReturn[3];
                            string strUserFullName = listReturn[4];

                            long userID;
                            long loginID;
                            if (!long.TryParse(strUserID, out userID)
                                || !long.TryParse(strLoginID, out loginID))
                            {
                                ShowException(string.Format("Return UserInfo invalid"));
                                return;
                            }


                            #region 登录成功，更新用户信息

                            //登录成功，更新用户信息
                            S1202App.LoginSessionID = strLoginID;
                            S1202App.IsLogined = true;
                            S1202App.LastActiveTime = DateTime.Now;
                            if (S1202App.IsLDAPAutoLogin)
                            {
                                CurrentApp.Session.UserInfo.Account = string.Format("{0}@{1}", S1202App.LoginDomainName,
                                    strAccount);
                            }
                            else
                            {
                                if (S1202App.IsLDAPAccount)
                                {
                                    CurrentApp.Session.UserInfo.Account = strAccount.Replace("\\", "@");
                                }
                                else
                                {
                                    CurrentApp.Session.UserInfo.Account = strAccount;
                                }
                            }
                            if (S1202App.IsLDAPAccount)
                            {
                                CurrentApp.Session.UserInfo.Password = S1202App.EncryptString(mUserPassword);
                            }
                            else
                            {
                                CurrentApp.Session.UserInfo.Password = S1202App.EncryptString(strPassword);
                            }
                            CurrentApp.Session.RentInfo.Token = strRentToken;
                            CurrentApp.Session.UserInfo.UserID = userID;
                            CurrentApp.Session.UserInfo.UserName = strUserFullName;
                            CurrentApp.Session.UserID = userID;
                            var loginInfo = CurrentApp.Session.LoginInfo;
                            if (loginInfo == null)
                            {
                                loginInfo = new LoginInfo();
                            }
                            loginInfo.LoginID = loginID;
                            loginInfo.UserID = userID;
                            loginInfo.Host = Environment.MachineName;
                            loginInfo.LoginTime = DateTime.Now.ToUniversalTime();
                            CurrentApp.Session.LoginInfo = loginInfo;
                            CurrentApp.Session.LoginID = loginID;

                            CurrentApp.WriteLog("LoginSystem",
                                string.Format("Login successful.\tUserID:{0};\tLoginID:{1}", userID, loginID));

                            #endregion


                            //启动检查线程
                            CreateCheckUserStatusThread();

                            ////For Test
                            //strReturnCode = "S01A01";

                            string strUserStateCode = string.Empty;
                            if (listReturn.Count > 5)
                            {
                                strUserStateCode = listReturn[5];
                            }
                            mLoginReturnCode = strUserStateCode;
                            CurrentApp.WriteLog("LoginSystem", string.Format("UserStateCode:{0}", strUserStateCode));
                            string strDays = string.Empty;
                            if (listReturn.Count > 6)
                            {
                                strDays = listReturn[6];
                            }
                            int intDays;
                            if (int.TryParse(strDays, out intDays))
                            {

                            }

                            BackgroundWorker worker2 = new BackgroundWorker();
                            worker2.DoWork += (s2, de2) =>
                            {
                                LoadGlobalParamInfos();
                            };
                            worker2.RunWorkerCompleted += (s2, re2) =>
                            {
                                if (strUserStateCode == "S01A01"
                                    || strUserStateCode == "S01A02"
                                    || strUserStateCode == "S01A03")
                                {

                                    #region 修改密码

                                    if (strUserStateCode == "S01A03")
                                    {
                                        var result =
                                            MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("S0000027",
                                                "Password will expired, do you want to change password?"), intDays),
                                                CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                        if (result != MessageBoxResult.Yes)
                                        {
                                            SelectUserRole();
                                            return;
                                        }
                                    }
                                    if (strUserStateCode == "S01A01")
                                    {
                                        MessageBox.Show(
                                             CurrentApp.GetLanguageInfo("S0000025",
                                                 "New user will change password."),
                                             CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                    }
                                    if (strUserStateCode == "S01A02")
                                    {
                                        MessageBox.Show(
                                            CurrentApp.GetLanguageInfo("S0000026",
                                                "Password has expired! You need change password."),
                                            CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                    }

                                    try
                                    {
                                        UCChangePassword uc = new UCChangePassword();
                                        uc.PageParent = this;
                                        uc.CurrentApp = CurrentApp;
                                        uc.LoginReturnCode = strUserStateCode;
                                        PopupPanel.Title = CurrentApp.GetLanguageInfo("S0000030", "Change password");
                                        PopupPanel.Content = uc;
                                        PopupPanel.IsOpen = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowException(ex.Message);
                                    }

                                    #endregion

                                }
                                else
                                {
                                    SelectUserRole();
                                }
                            };
                            worker2.RunWorkerAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SelectUserRole()
        {
            //获取用户所属的角色，如果没有角色，返回错误，如果只有一个角色，无需选择角色，如果不止一个加上，则需要弹出角色选择页面

            BackgroundWorker worker2 = new BackgroundWorker();
            worker2.DoWork += (s2, de2) => LoadUserRoles();
            worker2.RunWorkerCompleted += (s2, re2) =>
            {
                worker2.Dispose();

                if (mListUserRoles.Count <= 0)
                {
                    //没有任何角色，返回错误
                    ShowException(CurrentApp.GetLanguageInfo("S0000056", string.Format("No Role")));
                    return;
                }
                if (mListUserRoles.Count == 1)
                {
                    //只有一个角色，无需弹出角色选择页面
                    RoleInfo roleInfo = mListUserRoles[0];
                    CurrentApp.Session.RoleInfo = roleInfo;
                    CurrentApp.Session.RoleID = roleInfo.ID;

                    #region 通知角色变更

                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)RequestCode.SCGlobalSettingChanged;
                        webRequest.ListData.Add(ConstValue.GS_KEY_PARAM_ROLE);
                        webRequest.ListData.Add(roleInfo.ID.ToString());
                        webRequest.ListData.Add(roleInfo.Name);
                        webRequest.ListData.Add(string.Format("1"));        //“1” 代表是登录时选择的角色，空或“1” 代表登录后切换角色（见UMPS1201的ChangeRole）
                        CurrentApp.PublishEvent(webRequest);
                    }
                    catch (Exception ex)
                    {
                        CurrentApp.WriteLog("ChangeRole",
                        string.Format("Send change role notification fail.\t{0}", ex.Message));
                    }

                    #endregion

                    NavigetMainPage();

                    return;
                }
                //不止一个角色，弹出角色选择窗口
                try
                {
                    UCChangeRole uc = new UCChangeRole();
                    uc.PageParent = this;
                    uc.CurrentApp = CurrentApp;
                    uc.ListRoleInfos = mListUserRoles;
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("S0000045", "Select a role");
                    PopupPanel.Content = uc;
                    PopupPanel.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            };
            worker2.RunWorkerAsync();
           
        }

        private void NavigetMainPage()
        {
            try
            {
                WebRequest request = new WebRequest();
                request.Session = CurrentApp.Session;
                request.Code = (int)RequestCode.ACLoginLoginSystem;
                CurrentApp.PublishEvent(request);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoLogoutSystem()
        {
            try
            {
                TxtLoginAccount.Text = string.Empty;
                TxtLoginPassword.Password = string.Empty;
                TxtLoginAccount.Focus();

                string strRentToken = CurrentApp.Session.RentInfo.Token;
                string strUserID = CurrentApp.Session.UserInfo.UserID.ToString();
                string strLoginID = S1202App.LoginSessionID;

                if (string.IsNullOrEmpty(strLoginID))
                {
                    ShowException(string.Format("Not logined"));
                    return;
                }

                List<string> listArgs = new List<string>();
                listArgs.Add(strRentToken);
                listArgs.Add(strUserID);
                listArgs.Add(strLoginID);

                for (int i = 0; i < listArgs.Count; i++)
                {
                    listArgs[i] = S1202App.EncryptString(listArgs[i]);
                }

                MyWaiter.Visibility = Visibility.Visible;
                OperationDataArgs optReturn = null;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service00000Client client = new Service00000Client(
                            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                        optReturn = client.OperationMethodA(12, listArgs);
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;

                    if (optReturn == null) { return; }

                    CurrentApp.WriteLog("Logout", string.Format("LogoutReturn:{0}", optReturn.StringReturn));

                    if (!optReturn.BoolReturn)
                    {
                        ShowException(string.Format("Logout fail."));
                        return;
                    }

                    S1202App.LoginSessionID = string.Empty;
                    S1202App.IsLogined = false;
                    S1202App.IsLDAPAccount = false;
                    S1202App.IsLDAPAutoLogin = false;
                    S1202App.LoginDomainName = string.Empty;

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.ACLoginLogout;
                    webRequest.ListData.Add(mLogoutReason);
                    webRequest.ListData.Add(mIdleTimout.ToString());
                    CurrentApp.PublishEvent(webRequest);

                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoUserOnline()
        {
            try
            {
                if (!S1202App.IsLogined
                    || string.IsNullOrEmpty(S1202App.LoginSessionID)) { return; }

                //CurrentApp.WriteLog("DoUserOnline", string.Format("XXXX"));

                #region 检查是否空闲超时



                #endregion

                string strRentToken = CurrentApp.Session.RentInfo.Token;
                string strUserID = CurrentApp.Session.UserInfo.UserID.ToString();
                string strLoginID = S1202App.LoginSessionID;

                List<string> listArgs = new List<string>();
                listArgs.Add(strRentToken);
                listArgs.Add(strUserID);
                listArgs.Add(strLoginID);

                for (int i = 0; i < listArgs.Count; i++)
                {
                    listArgs[i] = S1202App.EncryptString(listArgs[i]);
                }

                OperationDataArgs optReturn = null;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service00000Client client = new Service00000Client(
                            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                        optReturn = client.OperationMethodA(13, listArgs);
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (optReturn == null) { return; }

                    CurrentApp.WriteLog("UserOnline", string.Format("UserOnlineReturn:{0}", optReturn.StringReturn));

                    if (!optReturn.BoolReturn)
                    {
                        ShowException(string.Format("Set UserOnline fail."));
                        return;
                    }

                    S1202App.LastActiveTime = DateTime.Now;

                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnExitSystem()
        {
            try
            {
                if (S1202App.IsLogined)
                {
                    string strRentToken = CurrentApp.Session.RentInfo.Token;
                    string strUserID = CurrentApp.Session.UserInfo.UserID.ToString();
                    string strLoginID = S1202App.LoginSessionID;

                    List<string> listArgs = new List<string>();
                    listArgs.Add(strRentToken);
                    listArgs.Add(strUserID);
                    listArgs.Add(strLoginID);

                    for (int i = 0; i < listArgs.Count; i++)
                    {
                        listArgs[i] = S1202App.EncryptString(listArgs[i]);
                    }

                    Service00000Client client = new Service00000Client(
                          WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                          WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                    OperationDataArgs optReturn = client.OperationMethodA(12, listArgs);
                    client.Close();
                    CurrentApp.WriteLog("Logout", string.Format("LogoutReturn:{0}", optReturn.StringReturn));
                    S1202App.LoginSessionID = string.Empty;
                    S1202App.IsLogined = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnChangePasswordResult(string strResult)
        {
            try
            {
                PopupPanel.IsOpen = false;
                if (strResult == UCChangePassword.RESULT_SUCC)
                {
                    SelectUserRole();
                }
                else
                {
                    if (mLoginReturnCode == "S01A03")
                    {
                        SelectUserRole();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnChangeRoleResult(string strResult)
        {
            try
            {
                PopupPanel.IsOpen = false;

                if (strResult == UCChangeRole.RESULT_SUCC)
                {
                    NavigetMainPage();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region CheckUserStatus

        private void CreateCheckUserStatusThread()
        {
            if (mThreadCheckUserStatus != null)
            {
                try
                {
                    mThreadCheckUserStatus.Abort();
                }
                catch { }
                mThreadCheckUserStatus = null;
            }
            try
            {
                mThreadCheckUserStatus = new Thread(DoCheckUserStatus);
                mThreadCheckUserStatus.Start();
                CurrentApp.WriteLog("CheckUserStatus",
                    string.Format("Create CheckUserStatus thread end.\t{0}", mThreadCheckUserStatus.ManagedThreadId));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CheckUserStatus",
                    string.Format("Create CheckUserStatusThread fail.\t{0}", ex.Message));
            }
        }

        private void StopCheckUserStatusThread()
        {
            if (mThreadCheckUserStatus != null)
            {
                try
                {
                    mThreadCheckUserStatus.Abort();
                    CurrentApp.WriteLog("CheckUserStatus", string.Format("StopCheckUserStatusThread end"));
                }
                catch (Exception ex)
                {
                    CurrentApp.WriteLog("CheckUserStatus", string.Format("StopCheckUserStatusThread fail.\t{0}", ex.Message));
                }
                mThreadCheckUserStatus = null;
            }
        }

        private void DoCheckUserStatus()
        {
            while (true)
            {
                try
                {
                    if (S1202App.IsLogined)
                    {

                        #region 检查用户状态

                        var dbInfo = CurrentApp.Session.DatabaseInfo;
                        dbInfo.RealPassword = S1202App.DecryptString(dbInfo.Password);
                        string strConnString = dbInfo.GetConnectionString();
                        List<string> listArgs = new List<string>();
                        listArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                        listArgs.Add(strConnString);
                        listArgs.Add(CurrentApp.Session.RentInfo.Token);
                        listArgs.Add(S1202App.LoginSessionID);
                        Service00000Client client =
                            new Service00000Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                        OperationDataArgs retArgs = client.OperationMethodA(52, listArgs);
                        CurrentApp.WriteLog("CheckUserStatus", string.Format("CheckReturn:{0}", retArgs.StringReturn));
                        if (retArgs.StringReturn != "1")
                        {
                            mLogoutReason = LOGOUT_REASON_OTHERLOGIN;
                            CurrentApp.WriteLog("CheckUserStatus", string.Format("Login on other terminal"));
                            Dispatcher.Invoke(new Action(DoLogoutSystem));
                            return;
                        }

                        #endregion


                        #region 检查最长登录时间

                        var globalParamTimeout = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == ConstValue.GP_USER_LOGOUT_TIMEOUT);
                        if (globalParamTimeout == null)
                        {
                            CurrentApp.WriteLog("CheckTimeout", string.Format("TimoutParam is null"));
                        }
                        else
                        {
                            string strValue = globalParamTimeout.ParamValue;
                            if (strValue.Length < 9)
                            {
                                CurrentApp.WriteLog("CheckTimeout", string.Format("ParamValue length invalid.\t{0}", strValue));
                            }
                            else
                            {
                                string strParamValue = strValue.Substring(8);
                                int intValue;
                                if (!int.TryParse(strParamValue, out intValue))
                                {
                                    CurrentApp.WriteLog("CheckTimeout", string.Format("TimoutParam invalid"));
                                }
                                else
                                {
                                    if (intValue > 0)
                                    {
                                        DateTime oldTime = S1202App.LastActiveTime;
                                        DateTime now = DateTime.Now;

                                        //超出最大登录时间，自动注销
                                        if (now - oldTime > TimeSpan.FromHours(intValue))
                                        {
                                            mLogoutReason = LOGOUT_REASON_TIMEOUT;
                                            CurrentApp.WriteLog("CheckTimeout", string.Format("Login timeout."));
                                            Dispatcher.Invoke(new Action(DoLogoutSystem));
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion


                        #region 检查空闲时间

                        var globalParamIdle = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == ConstValue.GP_USER_LOGOUT_IDLE);
                        if (globalParamIdle == null)
                        {
                            CurrentApp.WriteLog("CheckIdle", string.Format("IdleParam is null"));
                        }
                        else
                        {
                            string strValue = globalParamIdle.ParamValue;
                            if (strValue.Length < 9)
                            {
                                CurrentApp.WriteLog("CheckIdle", string.Format("ParamValue length invalid.\t{0}", strValue));
                            }
                            else
                            {
                                string strParamValue = strValue.Substring(8);
                                int intValue;
                                if (!int.TryParse(strParamValue, out intValue))
                                {
                                    CurrentApp.WriteLog("CheckIdle", string.Format("IdleParam invalid"));
                                }
                                else
                                {
                                    if (intValue > 0)
                                    {
                                        mIdleTimout = intValue;
                                        DateTime oldTime = S1202App.LastActiveTime;
                                        DateTime now = DateTime.Now;

                                        //超出空闲时间，自动注销
                                        if (now - oldTime > TimeSpan.FromMinutes(intValue))
                                        {
                                            mLogoutReason = LOGOUT_REASON_IDLETIMEOUT;
                                            CurrentApp.WriteLog("CheckIdle", string.Format("Idle timeout."));
                                            Dispatcher.Invoke(new Action(DoLogoutSystem));
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    CurrentApp.WriteLog("CheckUserStatus", string.Format("CheckUserStatus fail.\t{0}", ex.Message));
                }

                Thread.Sleep(5 * 1000);
            }
        }

        #endregion


        #region Others

        private void InitSupportLangTypes()
        {
            try
            {
                CurrentApp.Session.SupportLangTypes.Clear();
                for (int i = 0; i < mSupportLangTypes.Count; i++)
                {
                    CurrentApp.Session.SupportLangTypes.Add(mSupportLangTypes[i]);
                }
                var lang =
                    CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID == CurrentApp.Session.LangTypeID);
                if (lang != null)
                {
                    CurrentApp.Session.LangTypeInfo = lang;
                    CurrentApp.Session.LangTypeID = lang.LangID;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitLoginOptionMenu()
        {
            try
            {
                ContextMenu contextMenu = new ContextMenu();

                MenuItem item = new MenuItem();
                item.Name = "BtnForceLogin";
                item.Header = CurrentApp.GetLanguageInfo("S0000003", "Force Login");
                item.Click += LoginOptionItem_Click;
                item.IsChecked = mIsForceLogin;
                contextMenu.Items.Add(item);
                contextMenu.Items.Add(new Separator());
                item = new MenuItem();
                item.Name = "BtnLanguages";
                item.Header = CurrentApp.GetLanguageInfo("S0000001", "Languages");
                for (int i = 0; i < CurrentApp.Session.SupportLangTypes.Count; i++)
                {
                    var langType = CurrentApp.Session.SupportLangTypes[i];
                    MenuItem subItem = new MenuItem();
                    subItem.Name = string.Format("BtnLang{0}", langType.LangID);
                    subItem.Header = langType.Display;
                    subItem.Click += LoginOptionItem_Click;
                    if (langType.LangID == CurrentApp.Session.LangTypeID)
                    {
                        subItem.IsChecked = true;
                    }
                    item.Items.Add(subItem);
                }
                contextMenu.Items.Add(item);
                contextMenu.Items.Add(new Separator());
                item = new MenuItem();
                item.Name = "BtnThemes";
                item.Header = CurrentApp.GetLanguageInfo("S0000002", "Languages");
                for (int i = 0; i < CurrentApp.Session.SupportThemes.Count; i++)
                {
                    var theme = CurrentApp.Session.SupportThemes[i];
                    MenuItem subItem = new MenuItem();
                    subItem.Name = string.Format("BtnTheme{0}", theme.Name);
                    subItem.Header = CurrentApp.GetLanguageInfo(string.Format("S0000{0}", theme.Name), theme.Name);
                    subItem.Click += LoginOptionItem_Click;
                    if (theme.Name == CurrentApp.Session.ThemeName)
                    {
                        subItem.IsChecked = true;
                    }
                    item.Items.Add(subItem);
                }
                contextMenu.Items.Add(item);

                BtnLoginOption.ContextMenu = contextMenu;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void LoginOptionItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as MenuItem;
                if (btn == null) { return; }
                string strName = btn.Name;
                if (strName == "BtnForceLogin")
                {
                    mIsForceLogin = !mIsForceLogin;
                }
                if (strName.StartsWith("BtnLang"))
                {
                    string strLangID = strName.Substring(7);
                    int langID;
                    if (int.TryParse(strLangID, out langID))
                    {
                        var lang = CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID == langID);
                        if (lang != null)
                        {
                            CurrentApp.Session.LangTypeInfo = lang;
                            CurrentApp.Session.LangTypeID = lang.LangID;
                            DoChangeLanguage();
                        }
                    }
                }
                if (strName.StartsWith("BtnTheme"))
                {
                    string strThemeName = strName.Substring(8);
                    if (!string.IsNullOrEmpty(strThemeName))
                    {
                        var theme = CurrentApp.Session.SupportThemes.FirstOrDefault(t => t.Name == strThemeName);
                        if (theme != null)
                        {
                            CurrentApp.Session.ThemeInfo = theme;
                            CurrentApp.Session.ThemeName = theme.Name;
                            DoChangeTheme();
                        }
                    }
                }
                InitLoginOptionMenu();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeTheme()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.CSThemeChange;
                webRequest.Data = CurrentApp.Session.ThemeName;
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoChangeLanguage()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.CSLanguageChange;
                webRequest.Data = CurrentApp.Session.LangTypeID.ToString();
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void Close()
        {
            base.Close();

            StopCheckUserStatusThread();
        }

        private string GetErrorMessage(string errCode)
        {
            var list = CurrentApp.ListLanguageInfos;
            var lang = list.FirstOrDefault(l => l.LangID == CurrentApp.Session.LangTypeID && l.ObjName == errCode);
            if (lang == null)
            {
                return string.Empty;
            }
            return lang.Display;
        }

        #endregion


        #region LDAP Operation

        private void CheckLDAPLogin()
        {
            try
            {
                string strDomainName = Environment.UserDomainName.ToLower();
                if (string.IsNullOrEmpty(strDomainName)) { return; }

                CurrentApp.WriteLog("CheckLDAPLogin", string.Format("Domain name is {0}", strDomainName));

                var domain = mListDomainInfos.FirstOrDefault(d => d.Name.ToUpper() == strDomainName.ToUpper());
                if (domain == null)
                {
                    CurrentApp.WriteLog("CheckLDAPLogin", string.Format("No domain.\t{0}", strDomainName));
                    return;
                }
                if (!domain.AllowAutoLogin)
                {
                    CurrentApp.WriteLog("CheckLDAPLogin", string.Format("Not allow autologin.\t{0}", strDomainName));
                    return;
                }
                string strAccount = Environment.UserName.ToLower();

                CurrentApp.WriteLog("CheckLDAPLogin", string.Format("Domain user name is {0}", strAccount));

                string strUser = string.Format("{0}@{1}", strDomainName, strAccount);
                bool isFail = true;
                string strError = string.Empty;
                string strPassword = string.Empty;
                OperationReturn optReturn;
                MyWaiter.Visibility = Visibility.Visible;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        optReturn = GetLDAPUserInfo(strUser);
                        if (!optReturn.Result)
                        {
                            strError = string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message);
                            return;
                        }
                        UserInfo userInfo = optReturn.Data as UserInfo;
                        if (userInfo == null)
                        {
                            strError = string.Format("UserInfo is null");
                            return;
                        }
                        string str = userInfo.Password;
                        str = S1202App.DecryptString(str);
                        if (str.Length <= 19)
                        {
                            strError = string.Format("Password length invalid.");
                            return;
                        }
                        strPassword = str.Substring(19);
                        mUserPassword = strPassword;
                        isFail = false;
                        S1202App.IsLDAPAutoLogin = true;
                        S1202App.IsLDAPAccount = true;
                        S1202App.LoginDomainName = strDomainName;

                        if (CurrentApp != null
                            && CurrentApp.Session != null)
                        {
                            var domainInfo = CurrentApp.Session.DomainInfo;
                            if (domainInfo == null)
                            {
                                domainInfo = new DomainInfo();
                            }
                            domainInfo.DomainID = domain.ObjID;
                            domainInfo.Name = strDomainName;
                            domainInfo.FullName = strDomainName;
                            domainInfo.AllowAutoLogin = domain.AllowAutoLogin;
                            CurrentApp.Session.DomainInfo = domainInfo;
                            CurrentApp.Session.DomainID = domainInfo.DomainID;
                        }
                    }
                    catch (Exception ex)
                    {
                        strError = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    try
                    {
                        worker.Dispose();
                        MyWaiter.Visibility = Visibility.Collapsed;

                        if (isFail)
                        {
                            ShowException(string.Format("Auto login fail.\t{0}", strError));
                            return;
                        }

                        TxtLoginAccount.Text = strUser;
                        TxtLoginPassword.Password = strPassword;

                        CurrentApp.WriteLog("CheckLDAPLogin", string.Format("Try auto login by LDAP.\t{0}\t{1}", strDomainName, strUser));

                        strPassword = string.Format("{0}{0}{1}{0}{2}", ConstValue.SPLITER_CHAR_2, strDomainName,
                            CurrentApp.Session.RentInfo.Token);
                        DoLoginSystem(strAccount, strPassword, "F");
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn GetLDAPUserInfo(string strUser)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetLDAPUserList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(strUser);
                Service12001Client client =
                    new Service12001Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListData is null");
                    return optReturn;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("No ladp user info.\t{0}", strUser);
                    return optReturn;
                }
                string strInfo = webReturn.ListData[0];
                optReturn = XMLHelper.DeserializeObject<UserInfo>(strInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                UserInfo userInfo = optReturn.Data as UserInfo;
                if (userInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("UserInfo is null");
                    return optReturn;
                }
                optReturn.Data = userInfo;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region AppEvent

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);

            try
            {
                int code = webRequest.Code;

                //CurrentApp.WriteLog("Test", string.Format("Code:{0}", (RequestCode)code));

                switch (code)
                {
                    case (int)RequestCode.ACPageHeadLogout:
                        mLogoutReason = LOGOUT_REASON_NORMAL;
                        Dispatcher.Invoke(new Action(DoLogoutSystem));
                        break;
                    case (int)RequestCode.ACLoginOnline:
                        Dispatcher.Invoke(new Action(DoUserOnline));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Theme and Languages

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name,
                        StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1202;component/Themes/{0}/{1}",
                        "Default",
                        StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = "UMP";

                BtnLoginSystem.ToolTip = CurrentApp.GetLanguageInfo("S0000069", "Login");
                BtnLoginOption.ToolTip = CurrentApp.GetLanguageInfo("S0000070", "Option");

                InitLoginOptionMenu();

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
