//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0e5ba661-9cf8-44ea-acfe-b245f75491b4
//        CLR Version:              4.0.30319.18444
//        Name:                     UserInfoModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101
//        File Name:                UserInfoModify
//
//        created by Charley at 2014/9/4 9:38:28
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// UserInfoModify.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoModify
    {
        #region Members

        public OUMMainView PageParent;
        public ObjectItem ObjParent;
        public ObjectItem UserItem;
        public BasicOrgInfo OrgInfo;
        public BasicUserInfo BasicUserInfo;
        public ExtendUserInfo ExtUserInfo;
        public bool IsAddUser;

        private bool mIsExtInfoChanged;
        private BackgroundWorker mBackgroundWorker;
        private bool mAsyncResult;
        private ObservableCollection<HeadIconInfo> mListHeadIcons;

        #endregion
        //public S1101App CurrentApp;

        public UserInfoModify()
        {
            InitializeComponent();

            mListHeadIcons = new ObservableCollection<HeadIconInfo>();

            Loaded += UserInfoModify_Loaded;
            mIsExtInfoChanged = false;

        }

        void UserInfoModify_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            TxtEmail.TextChanged += (s, re) => mIsExtInfoChanged = true;
            TxtPhone.TextChanged += (s, re) => mIsExtInfoChanged = true;
            TxtBirthday.ValueChanged += (s, re) => mIsExtInfoChanged = true;
            ComboHeadIcon.SelectionChanged += (s, re) => mIsExtInfoChanged = true;

            ComboHeadIcon.ItemsSource = mListHeadIcons;
            

            Init();
            InitHeadIcons();
           
            mIsExtInfoChanged = false;
            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
        {
            if (IsAddUser)
            {
                if (ObjParent == null) { return; }
                OrgInfo = ObjParent.Data as BasicOrgInfo;
                TxtAccount.Text = string.Empty;
                TxtFullName.Text = string.Empty;
                if (OrgInfo != null)
                {
                    TxtOrg.Text = OrgInfo.OrgName;
                }
                TxtEmail.Text = string.Empty;
                TxtPhone.Text = string.Empty;
                TxtBirthday.Text = string.Empty;
                //px屏蔽
                //ComboHeadIcon.SelectedItem = null;
                //Px+
                ComboHeadIcon.SelectedIndex = 0;

                chkIsActive.IsChecked = true;
                BorderSetting.Visibility = Visibility.Collapsed;
                dtValidTime.Value = DateTime.Now;
                dtInValidTime .Value= DateTime.Parse(S1101Consts.Default_StrEndTime);
            }
            else
            {
                if (UserItem == null) { return; }
                BasicUserInfo = UserItem.Data as BasicUserInfo;
                if (BasicUserInfo != null)
                {
                    TxtAccount.Text = BasicUserInfo.Account;
                    TxtFullName.Text = BasicUserInfo.FullName;
                    if (BasicUserInfo.SourceFlag == "L")
                    {
                        TxtAccount.IsEnabled = false;
                        TxtAccount.Text = BasicUserInfo.Account.Replace("@", @"\");
                        //TxtFullName.IsEnabled = false;
                    }
                    if(BasicUserInfo .IsLocked !="0")
                    {
                        CbLock.IsChecked = true;
                    }
                    else
                    {
                        CbLock.IsChecked = false;
                    }

                    //if (BasicUserInfo.LockMethod == "U")
                    //{
                    //    CbLock.IsChecked = true;
                    //}
                    //else
                    //{
                    //    CbLock.IsChecked = false;
                    //}
                    CbResetPwd.IsChecked = false;
                    chkIsActive.IsChecked = BasicUserInfo.IsActived.Equals("1");
                    //dtValidTime.Value = DateTime.Parse(BasicUserInfo.StrStartTime);
                    dtValidTime.Value = BasicUserInfo.StartTime;
                    //BasicUserInfo.StartTime
                    if (BasicUserInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                    {
                        dtInValidTime.Value = DateTime.Parse(S1101Consts.Default_StrEndTime);
                    }
                    else 
                    {
                        dtInValidTime.Value = BasicUserInfo.EndTime;
                            ///DateTime.Parse(BasicUserInfo.StrEndTime);
                    }

                    if (PageParent != null)
                    {
                        PageParent.SetBusy(true, string.Empty);
                    }
                    mBackgroundWorker = new BackgroundWorker();
                    mBackgroundWorker.DoWork += (s, de) => LoadUserExtInfo();
                    mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mBackgroundWorker.Dispose();
                        if (PageParent != null)
                        {
                            PageParent.SetBusy(false, string.Empty);
                        }
                        if (ExtUserInfo != null)
                        {
                            TxtEmail.Text = ExtUserInfo.MailAddress;
                            TxtPhone.Text = ExtUserInfo.PhoneNumber;
                            DateTime birthday;
                            if (DateTime.TryParse(ExtUserInfo.Birthday, out birthday))
                            {
                                TxtBirthday.Value = birthday;
                            }
                            else
                            {
                                TxtBirthday.Value = null;
                            }
                            var headIcon = mListHeadIcons.FirstOrDefault(h => h.Icon == ExtUserInfo.HeadIcon);
                            if (headIcon != null)
                            {
                                ComboHeadIcon.SelectedItem = headIcon;
                            }
                            //px+
                            else
                            {
                                ComboHeadIcon.SelectedIndex = 0;
                            }
                            //end
                        }
                        mIsExtInfoChanged = false;
                    };
                    mBackgroundWorker.RunWorkerAsync();
                }
                ObjParent = UserItem.Parent as ObjectItem;
                if (ObjParent != null)
                {
                    OrgInfo = ObjParent.Data as BasicOrgInfo;
                    if (OrgInfo != null)
                    {
                        TxtOrg.Text = OrgInfo.OrgName;
                    }
                }
                BorderSetting.Visibility = Visibility.Visible;
                IsSelf();
            }
        }

        private void InitHeadIcons()
        {
            try
            {
                mListHeadIcons.Clear();
                for (int i = 0; i < 100; i++)
                {
                    HeadIconInfo item = new HeadIconInfo();
                    item.Icon = i.ToString();
                    item.Path = string.Format("{0}://{1}:{2}/HeadIcons/{3}.bmp",
                        CurrentApp.Session.AppServerInfo.Protocol,
                        CurrentApp.Session.AppServerInfo.Address,
                        CurrentApp.Session.AppServerInfo.Port,
                        i);
                    mListHeadIcons.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserExtInfo()
        {
            if (BasicUserInfo == null) { return; }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetUserExtInfo;
                webRequest.Data = BasicUserInfo.UserID.ToString();

                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code != Defines.RET_NOT_EXIST)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    ExtUserInfo = new ExtendUserInfo();
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ExtendUserInfo>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ExtUserInfo = optReturn.Data as ExtendUserInfo;
                if (ExtUserInfo == null)
                {
                    ShowException(string.Format("Fail.\t{0}", "ExtUserInfo is null"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) { return; }
            if (IsAddUser)
            {
                AddNewUser();
            }
            else
            {
                ModifyUserInfo();
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        #endregion


        #region Operations

        private void AddNewUser()
        {
            try
            {
                string strLog = string.Empty;
                BasicUserInfo basicUserInfo = new BasicUserInfo();

                if (TxtAccount.Text.Length > 64 || TxtFullName.Text.Length > 64)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N024", "User Name is invalid."));
                    return;
                }
                if (TxtAccount.Text.Replace(" ", "") == string.Empty || TxtFullName.Text.Replace(" ", "") == string.Empty)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N026", "User Name is invalid."));
                    return;
                }
                if (TxtAccount.Text.Contains("@") || TxtFullName.Text.Contains("@"))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N041", "User Name is Contain @."));
                    return;
                }
                if (TxtAccount.Text.Contains(@"\") || TxtFullName.Text.Contains(@"\"))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N042", @"User Name is Contain \."));
                    return;
                }
                basicUserInfo.Account = TxtAccount.Text.Trim();
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10201"), TxtAccount.Text);
                basicUserInfo.FullName = TxtFullName.Text.Trim();
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10202"), TxtFullName.Text.Trim());
                if (OrgInfo != null)
                {
                    basicUserInfo.OrgID = OrgInfo.OrgID;
                }
                string strIsActive = string.Empty;
                strIsActive=chkIsActive.IsChecked == true ? "1" : "0";
                basicUserInfo.IsActived = strIsActive;
                if (strIsActive == "1")
                {
                    strLog += string.Format("{0}", Utils.FormatOptLogString("1101T10109"));
                }

                basicUserInfo.SourceFlag = "U";

                basicUserInfo.StartTime = DateTime.Parse(dtValidTime.Value.ToString());
                basicUserInfo.EndTime = DateTime.Parse(dtInValidTime.Value.ToString());
                basicUserInfo.StrStartTime = basicUserInfo.StartTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                basicUserInfo.StrEndTime = basicUserInfo.EndTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");

                ExtendUserInfo extUserInfo = new ExtendUserInfo();
                if (mIsExtInfoChanged)
                {
                    extUserInfo.MailAddress = TxtEmail.Text.Trim();
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10203"), TxtEmail.Text);
                    extUserInfo.PhoneNumber = TxtPhone.Text.Trim();
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10204"), TxtPhone.Text);
                    extUserInfo.Birthday = TxtBirthday.Value.ToString();
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10205"), TxtBirthday.Value);
                    var headIcon = ComboHeadIcon.SelectedItem as HeadIconInfo;
                    if (headIcon != null)
                    {
                        extUserInfo.HeadIcon = headIcon.Icon;
                        strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10214"), headIcon.Icon);
                    }
                }
                mAsyncResult = false;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) =>
                {
                    AddNewUser(basicUserInfo);
                    if (mAsyncResult
                        && mIsExtInfoChanged
                        && !string.IsNullOrEmpty(basicUserInfo.UserID.ToString()))
                    {
                        extUserInfo.UserID = basicUserInfo.UserID;
                        mAsyncResult = false;
                        ModifyExtUserInfo(extUserInfo);
                    }
                };
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    if (mAsyncResult)
                    {
                        #region 写操作日志

                        CurrentApp.WriteOperationLog(S1101Consts.OPT_ADDUSER.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                        #endregion

                        if (PageParent != null)
                        {
                            PageParent.ReloadData(ObjParent);
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                };
                mBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddNewUser(BasicUserInfo basicUserInfo)
        {
            try
            {
                OperationReturn optReturn = XMLHelper.SeriallizeObject(basicUserInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.AddNewUser;
                webRequest.Data = optReturn.Data.ToString();
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        ShowException(CurrentApp.GetMessageLanguageInfo("007", "User account already exist"));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                basicUserInfo.UserID = Convert.ToInt64(webReturn.Data);
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyUserInfo()
        {
            try
            {
                if (BasicUserInfo == null) { return; }
                string account, fullName,  strReset;
                string strLog = string.Empty;
                string strIsActive = string.Empty;
                string strLock = string.Empty;
                string UserName=TxtAccount.Text.Trim();
                if (UserName.Contains("@"))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N041", "User Name is invalid."));
                    return;
                }
                if (TxtAccount.Text.Length > 64 || TxtFullName.Text.Length > 64)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N024", "User Name is invalid."));
                    return;
                }
                if (TxtAccount.Text.Replace(" ", "") == string.Empty || TxtFullName.Text.Replace(" ", "") == string.Empty)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N026", "User Name is invalid."));
                    return;
                }
                account = TxtAccount.Text.Trim();
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10201"), account);
                fullName = TxtFullName.Text.Trim();
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10202"), fullName);
                //if (BasicUserInfo.LockMethod == "N" && CbLock.IsChecked == true)
                //{
                //    strLock = "U";
                //}
                //else if (BasicUserInfo.LockMethod == "U" && CbLock.IsChecked == false)
                //{
                //    strLock = "C";
                //}
                //else
                //{
                //    strLock = string.Empty;
                //}

                //1为锁，0为未锁
                if (BasicUserInfo.IsLocked == "1")
                {
                    if (CbLock.IsChecked == false)
                    {
                        if (BasicUserInfo.LockMethod == "L")
                        {
                            strLock = "CL";
                        }
                        else 
                        {
                            strLock = "CU";
                        }
                    }
                    else 
                    {
                        strLock = String.Empty;
                    }
                }
                else if(BasicUserInfo.IsLocked =="0")
                {
                    if (CbLock.IsChecked == true)
                    {
                        strLock = "U";
                    }
                    else 
                    {
                        strLock = String.Empty;
                    }
                }
                else 
                {
                    strLock = string.Empty;
                }


                if (CbLock.IsChecked == true)
                //if (!string.IsNullOrEmpty(strLock))
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("11011100"));
                }
                strReset = CbResetPwd.IsChecked == true ? "1" : "0";
                if (strReset == "1")
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("11011101"));
                }

                DateTime validTime = DateTime.Parse(dtValidTime.Value.ToString()).ToUniversalTime();
                strLog += string.Format("{0} {1}", Utils.FormatOptLogString("1101T10105"),validTime);
                DateTime inValidTime = DateTime.Parse(dtInValidTime.Value.ToString()).ToUniversalTime();
                strLog += string.Format("{0} {1}", Utils.FormatOptLogString("1101T10106"), inValidTime);

                
                //0     UserID
                //1     Account
                //2     FullName
                //3     Locked
                //4     ResetPassword
                //5     IsActive
                //6     ValidTime
                //7     InValidTime
                List<string> listParams = new List<string>();
                listParams.Add(BasicUserInfo.UserID.ToString());
                listParams.Add(account);
                listParams.Add(fullName);
                listParams.Add(strLock);
                listParams.Add(strReset);
                strIsActive = chkIsActive.IsChecked == true ? "1" : "0";
                listParams.Add(strIsActive);
                listParams.Add(validTime.ToString("yyy/MM/dd HH:mm:ss"));
                listParams.Add(inValidTime.ToString("yyy/MM/dd HH:mm:ss"));
                if(strIsActive =="1")
                {
                    strLog += string.Format("{0}", Utils.FormatOptLogString("1101T10109"));
                }

                ExtendUserInfo extUserInfo = new ExtendUserInfo();
                if (mIsExtInfoChanged)
                {
                    extUserInfo.UserID = BasicUserInfo.UserID;
                    extUserInfo.MailAddress = TxtEmail.Text;
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10203"), TxtEmail.Text);
                    extUserInfo.PhoneNumber = TxtPhone.Text;
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10204"), TxtPhone.Text);
                    extUserInfo.Birthday = TxtBirthday.Value.ToString();
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10205"), TxtBirthday.Value);
                    var headIcon = ComboHeadIcon.SelectedItem as HeadIconInfo;
                    if (headIcon != null)
                    {
                        extUserInfo.HeadIcon = headIcon.Icon;
                        strLog += string.Format("{0} {1}  ", Utils.FormatOptLogString("1101T10214"), headIcon.Icon);
                    }
                }
                mAsyncResult = false;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) =>
                {
                    ModifyUserInfo(listParams);
                    if (mAsyncResult && mIsExtInfoChanged)
                    {
                        mAsyncResult = false;
                        ModifyExtUserInfo(extUserInfo);
                        //px+
                        if (extUserInfo.UserID == CurrentApp.Session.UserID)
                        {
                            //PageParent.RefreshHeadIcon();
                        }
                        //end
                    }
                };
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    if (mAsyncResult)
                    {
                        #region 写操作日志

                        CurrentApp.WriteOperationLog(S1101Consts.OPT_MODIFYUSER.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                        #endregion

                        if (PageParent != null)
                        {
                            PageParent.ReloadData(ObjParent);
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                };
                mBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyUserInfo(List<string> listParams)
        {
            try
            {
                if (listParams == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.ModifyUserInfo;
                webRequest.ListData = listParams;

                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        ShowException(CurrentApp.GetMessageLanguageInfo("007", "User account already exist"));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyExtUserInfo(ExtendUserInfo extUserInfo)
        {
            try
            {
                if (extUserInfo == null) { return; }

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.ModifyUserExtInfo;
                //0     UserID
                //1     Email
                //2     Phone
                //3     Birthday
                //4     HeadIcon
                webRequest.ListData.Add(extUserInfo.UserID.ToString());
                webRequest.ListData.Add(extUserInfo.MailAddress);
                webRequest.ListData.Add(extUserInfo.PhoneNumber);
                webRequest.ListData.Add(extUserInfo.Birthday);
                webRequest.ListData.Add(extUserInfo.HeadIcon);

                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Basic

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(TxtAccount.Text.Trim()))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("008", "Account is empty"));
                return false;
            }
            if (string.IsNullOrEmpty(TxtFullName.Text.Trim()))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("009", "FullName is empty"));
                return false;
            }
            if (string.IsNullOrEmpty(TxtOrg.Text.Trim()))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("010", "ParentOrg is empty"));
                return false;
            }
            try
            {
                DateTime starttiime = DateTime.Parse(dtValidTime.Value.ToString());
                try
                {
                    DateTime stoptime = DateTime.Parse(dtInValidTime.Value.ToString());
                    if (starttiime >= stoptime)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N023", "Start Time Must Smaller Than The Valid Time"));
                        return false;
                    }
                }
                catch (Exception)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N021", "Convert Valid Time Error"));
                    return false;
                }              
              
            }
            catch (Exception)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N022", "Convert inValid Time Error"));
                return false;
            }
            
            return true;
        }

        //private void ShowStausMessage(string msg)
        //{
        //    if (PageParent != null)
        //    {
        //        PageParent.SetStatuMessage(msg);
        //    }
        //}

        private void IsSelf()
        {
            if (BasicUserInfo.UserID == CurrentApp.Session.UserID)
            {
                this.TxtAccount.IsEnabled = false;
                this.TxtFullName.IsEnabled = false;
                this.TxtOrg.IsEnabled = false;
                this.dtInValidTime.IsEnabled = false;
                this.dtValidTime.IsEnabled = false;
                this.CbLock.IsEnabled = false;
                this.CbResetPwd.IsEnabled = false;
                this.chkIsActive.IsEnabled = false;
            }
        }

        #endregion


        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                LabelAccount.Content = CurrentApp.GetLanguageInfo("1101T10201", "Account");
                LabelFullName.Content = CurrentApp.GetLanguageInfo("1101T10202", "Full Name");
                LabelParentOrg.Content = CurrentApp.GetLanguageInfo("1101T10213", "Orgnization");
                LabelEmail.Content = CurrentApp.GetLanguageInfo("1101T10203", "Email Address");
                LabelPhone.Content = CurrentApp.GetLanguageInfo("1101T10204", "Phone Number");
                LabelBirthday.Content = CurrentApp.GetLanguageInfo("1101T10205", "Birthday");
                LabelHead.Content = CurrentApp.GetLanguageInfo("1101T10214", "Head Icon");

                LabelValidTime.Content = CurrentApp.GetLanguageInfo("1101T10105", "Valid Time");
                LabelInValidTime.Content = CurrentApp.GetLanguageInfo("1101T10106", "Invalid Time");

                CbLock.Content = CurrentApp.GetLanguageInfo("11011100", "Lock");
                CbResetPwd.Content = CurrentApp.GetLanguageInfo("11011101", "Reset Password");
                chkIsActive.Content = CurrentApp.GetLanguageInfo("1101T10109", "Is Active");
                BtnMore.Content = CurrentApp.GetLanguageInfo("11011102", "More");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (IsAddUser)
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("11011103", "Add User");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("11011104", "Modify User");
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}
