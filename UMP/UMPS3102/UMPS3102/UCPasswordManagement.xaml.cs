//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    45f57fdb-ffd2-487d-b882-beed3276eeea
//        CLR Version:              4.0.30319.18063
//        Name:                     UCPasswordManagement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCPasswordManagement
//
//        created by Charley at 2015/8/3 17:25:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// UCPasswordManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UCPasswordManagement
    {
        #region Members

        public QMMainView MainPage;
        public List<RecordEncryptInfo> ListRecordEncryptInfos;
        public List<SettingInfo> ListUserSettingInfos;
        public RecordInfoItem RecordInfoItem;

        #endregion

        static UCPasswordManagement()
        {
            PasswordManagerEventEvent = EventManager.RegisterRoutedEvent("PasswordManagerEvent", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<UMPEventArgs>), typeof(UCPasswordManagement));
        }

        public UCPasswordManagement()
        {
            InitializeComponent();

            Loaded += UCPasswordManagement_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCPasswordManagement_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                if (ListRecordEncryptInfos == null) { return; }
                string strAddress = RecordInfoItem.VoiceIP;
                TxtServerAddress.Text = strAddress;
                var temp = ListRecordEncryptInfos.FirstOrDefault(e => e.ServerAddress == strAddress);
                if (temp != null && temp.IsRemember)
                {
                    TxtPassword.Password = temp.Password;
                    CbRemember.IsChecked = true;
                    TxtExpireTime.Value = temp.EndTime.ToLocalTime();
                    DateTime now = DateTime.Now.ToUniversalTime();
                    if (temp.EndTime > now)
                    {
                        if (ListUserSettingInfos != null)
                        {
                            var setting =
                                ListUserSettingInfos.FirstOrDefault(
                                    s => s.ParamID == S3102Consts.USER_PARAM_SKIPPASSWORDPANEL);
                            if (setting != null && setting.StringValue == "1")
                            {
                                if (!CheckInput()) { return; }
                                OnPasswordManagerEventEvent(PasswordManagerEventCode.PASS_SETTED, temp);
                                var parent = Parent as PopupPanel;
                                if (parent != null)
                                {
                                    parent.IsOpen = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    TxtPassword.Password = string.Empty;
                    CbRemember.IsChecked = false;
                    TxtExpireTime.Value = DateTime.Now.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnPasswordManagerEventEvent(PasswordManagerEventCode.PASS_CANCEL, string.Empty);
                var panel = Parent as PopupPanel;
                if (panel != null)
                {
                    panel.IsOpen = false;
                }
            }
            catch { }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckInput()) { return; }
                string strAddress = TxtServerAddress.Text;
                DateTime expireTime = DateTime.Now.AddDays(1).ToUniversalTime();  //默认有效期一天
                if (TxtExpireTime.Value != null)
                {
                    expireTime = ((DateTime)TxtExpireTime.Value).ToUniversalTime();
                }
                bool isRemember = CbRemember.IsChecked == true;
                string strPassword = TxtPassword.Password;
                if (ListRecordEncryptInfos != null)
                {
                    bool isAdd = false;
                    var temp = ListRecordEncryptInfos.FirstOrDefault(s => s.ServerAddress == strAddress);
                    if (temp == null)
                    {
                        temp = new RecordEncryptInfo();
                        isAdd = true;
                    }
                    temp.UserID = CurrentApp.Session.UserID;
                    temp.ServerAddress = strAddress;
                    temp.StartTime = DateTime.Now.ToUniversalTime();
                    temp.EndTime = expireTime;
                    temp.Password = strPassword;
                    temp.IsRemember = isRemember;
                    if (isRemember)
                    {
                        if (isAdd)
                        {
                            ListRecordEncryptInfos.Add(temp);
                        }
                        SaveRecordEncryptConfig();
                    }
                    OnPasswordManagerEventEvent(PasswordManagerEventCode.PASS_SETTED, temp);
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SaveRecordEncryptConfig()
        {
            try
            {
                if (ListRecordEncryptInfos == null) { return; }
                List<UserParamInfo> listInfos = new List<UserParamInfo>();
                for (int i = 0; i < ListRecordEncryptInfos.Count; i++)
                {
                    var item = ListRecordEncryptInfos[i];
                    if (!item.IsRemember) { continue; }
                    UserParamInfo up = new UserParamInfo();
                    up.UserID = CurrentApp.Session.UserID;
                    up.ParamID = S3102Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000 + 1 + i;
                    up.GroupID = S3102Consts.USER_PARAM_GROUP_ENCRYPTINFO;
                    up.SortID = i;
                    up.DataType = DBDataType.NVarchar;
                    string strValue = string.Format("{0}{1}{2}{1}{3}",
                        item.ServerAddress,
                        ConstValue.SPLITER_CHAR_3,
                        item.Password,
                        item.EndTime.ToString("yyyyMMddHHmmss"));
                    up.ParamValue = strValue;
                    listInfos.Add(up);
                }
                int cout = listInfos.Count;
                if (cout <= 0) { return; }
                OperationReturn optReturn;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSSaveUserParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(cout.ToString());
                for (int i = 0; i < cout; i++)
                {
                    var up = listInfos[i];
                    optReturn = XMLHelper.SeriallizeObject(up);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SaveEncryptConfig", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private bool CheckInput()
        {
            string strAddress = TxtServerAddress.Text;
            DateTime expireTime = DateTime.Now.AddDays(1).ToUniversalTime();  //默认有效期一天
            if (TxtExpireTime.Value != null)
            {
                expireTime = ((DateTime)TxtExpireTime.Value).ToUniversalTime();
            }
            string strPassword = TxtPassword.Password;
            if (string.IsNullOrEmpty(strAddress)
                || string.IsNullOrEmpty(strPassword))
            {
                ShowException(CurrentApp.GetLanguageInfo("3102N016", "Input invalid"));
                return false;
            }
            if (expireTime <= DateTime.Now.ToUniversalTime())
            {
                ShowException(CurrentApp.GetLanguageInfo("3102N046", "Expire Time Invalid"));
                return false;
            }
            return true;
        }

        #endregion


        #region 加密管理器事件

        public static readonly RoutedEvent PasswordManagerEventEvent;

        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PasswordManagerEvent
        {
            add { AddHandler(PasswordManagerEventEvent, value); }
            remove { RemoveHandler(PasswordManagerEventEvent, value); }
        }

        private void OnPasswordManagerEventEvent(int code, object data)
        {
            if (PasswordManagerEventEvent != null)
            {
                UMPEventArgs args = new UMPEventArgs();
                args.Code = code;
                args.Data = data;
                RoutedPropertyChangedEventArgs<UMPEventArgs> a = new RoutedPropertyChangedEventArgs<UMPEventArgs>(
                    null, args);
                a.RoutedEvent = PasswordManagerEventEvent;
                RaiseEvent(a);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("31022101", "Input Password");
                }
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Cancel");

                LbServerAddress.Content = CurrentApp.GetLanguageInfo("31022102", "Server Address");
                LbExireTime.Content = CurrentApp.GetLanguageInfo("31022103", "Expire Time");
                LbPassword.Content = CurrentApp.GetLanguageInfo("31022104", "Password");
                LbRemember.Content = CurrentApp.GetLanguageInfo("31022105", "Remeber Password");
            }
            catch { }
        }

        #endregion


    }

    public class PasswordManagerEventCode
    {
        public const int PASS_SETTED = 1001;
        public const int PASS_CANCEL = 2001;
    }
}
