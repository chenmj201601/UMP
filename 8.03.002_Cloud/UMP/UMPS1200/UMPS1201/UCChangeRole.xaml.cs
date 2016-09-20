//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e3dd199-5e1c-43fc-9969-9bef41b1be63
//        CLR Version:              4.0.30319.18408
//        Name:                     UCChangeRole
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                UCChangeRole
//
//        created by Charley at 2016/4/10 19:47:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UMPS1201.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1201
{
    /// <summary>
    /// UCChangeRole.xaml 的交互逻辑
    /// </summary>
    public partial class UCChangeRole
    {

        public Shell PageParent;

        private bool mIsInited;
        private string mOptResult;
        private List<RoleInfo> mListUserRoles;
        private ObservableCollection<RoleItem> mListRoleItems;

        public const string RESULT_CANCEL = "Cancel";
        public const string RESULT_SUCC = "Succ";

        public UCChangeRole()
        {
            InitializeComponent();

            mListUserRoles = new List<RoleInfo>();
            mListRoleItems = new ObservableCollection<RoleItem>();

            BtnCancel.Click += BtnCancel_Click;
            BtnConfirm.Click += BtnConfirm_Click;
            Loaded += UCChangeRole_Loaded;
            ListBoxRoles.KeyDown += ListBoxRoles_KeyDown;
        }

        void UCChangeRole_Loaded(object sender, RoutedEventArgs e)
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
                ListBoxRoles.ItemsSource = mListRoleItems;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserRoleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    mListRoleItems.Clear();
                    for (int i = 0; i < mListUserRoles.Count; i++)
                    {
                        RoleInfo info = mListUserRoles[i];

                        RoleItem item = new RoleItem();
                        item.RoleID = info.ID;
                        item.Name = info.Name;
                        item.IsChecked = false;
                        item.Info = info;
                        GetRoleName(item);
                        mListRoleItems.Add(item);
                    }
                    var current = mListRoleItems.FirstOrDefault(r => r.RoleID == App.Session.RoleID);
                    if (current == null)
                    {
                        current = mListRoleItems.FirstOrDefault();
                        if (current == null)
                        {
                            ShowException(App.GetLanguageInfo("S0000056", "RoleItem is null"));
                            return;
                        }
                    }
                    current.IsChecked = true;

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserRoleInfos()
        {
            try
            {
                mListUserRoles.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1200Codes.GetUserRoleList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service12001"));
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

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = ListBoxRoles.SelectedItem as RoleItem;
                if (item == null) { return; }
                var roleInfo = item.Info;
                if (roleInfo == null) { return; }
                App.Session.RoleInfo = roleInfo;
                App.Session.RoleID = roleInfo.ID;

                #region 通知角色变更

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)RequestCode.SCGlobalSettingChanged;
                    webRequest.ListData.Add(ConstValue.GS_KEY_PARAM_ROLE);
                    webRequest.ListData.Add(roleInfo.ID.ToString());
                    webRequest.ListData.Add(roleInfo.Name);
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
                    PageParent.OnChangeRoleResult(mOptResult);
                }
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
                PageParent.OnChangeRoleResult(mOptResult);
            }
        }

        void ListBoxRoles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnConfirm_Click(this, new RoutedEventArgs());
            }
        }

        private void GetRoleName(RoleItem roleItem)
        {
            try
            {
                var info = roleItem.Info;
                if (info != null)
                {
                    roleItem.Name = App.GetLanguageInfo(string.Format("COMR{0}", info.ID), info.Name);
                }
            }
            catch { }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = App.GetLanguageInfo("S0000045", "Select a role");
                }

                for (int i = 0; i < mListRoleItems.Count; i++)
                {
                    var item = mListRoleItems[i];
                    GetRoleName(item);
                }

                TxtTip.Text = string.Format(App.GetLanguageInfo("S0000046", "{0} has these roles"),
                    App.Session.UserInfo.Account);
                BtnConfirm.Content = App.GetLanguageInfo("S0000047", "Confirm");
                BtnCancel.Content = App.GetLanguageInfo("S0000048", "Cancel");
            }
            catch { }
        }
    }
}
