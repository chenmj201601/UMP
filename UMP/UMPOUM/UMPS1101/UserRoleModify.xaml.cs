//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5785c68a-214f-4097-91cf-b0742332ebad
//        CLR Version:              4.0.30319.18444
//        Name:                     UserRoleModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101
//        File Name:                UserRoleModify
//
//        created by Charley at 2014/9/22 14:31:09
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// UserRoleModify.xaml 的交互逻辑
    /// </summary>
    public partial class UserRoleModify
    {
        #region Memembers

        public OUMMainView PageParent;
        public ObjectItem UserItem;

        private RoleItem mRootItem;
        private List<string> mUserRoles;
        private BackgroundWorker mWorder;
        private List<RoleItem> mListRoleItems;
        //public S1101App CurrentApp;
        #endregion


        public UserRoleModify()
        {
            InitializeComponent();

            mRootItem = new RoleItem();
            mRootItem.RoleId = 0;
            mUserRoles = new List<string>();
            mListRoleItems = new List<RoleItem>();
            Loaded += UserRoleModify_Loaded;
        }

        void UserRoleModify_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            TvRoles.ItemsSource = mRootItem.Children;

            Init();
            ChangeLanguage();
        }


        #region Init and load

        private void Init()
        {
            if (UserItem == null) { return; }
            if (PageParent != null)
            {
                PageParent.SetBusy(true,string.Empty);
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) =>
            {
                LoadAvaliableRoles();
                LoadUserControledRoles();
            };
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                mWorder.Dispose();
                if (PageParent != null)
                {
                    PageParent.SetBusy(false, string.Empty);
                }
                mRootItem.IsChecked = false;
                SetRoleCheckState();
            };
            mWorder.RunWorkerAsync();
        }

        private void LoadAvaliableRoles()
        {
            ClearChildren(mRootItem);
            mListRoleItems.Clear();
            LoadAvaliableRoles(mRootItem);
        }

        private void LoadAvaliableRoles(RoleItem parentItem)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetUserRoleList;
                webRequest.ListData.Add(parentItem.RoleId.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());

                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string roleInfo = webReturn.ListData[i];
                    string[] listRoleInfos = roleInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (listRoleInfos.Length != 2)
                    {
                        ShowException(string.Format("RoleInfo invalid.\t{0}", roleInfo));
                        return;
                    }
                    string roleID = listRoleInfos[0];
                    string roleName = listRoleInfos[1];
                    if (roleID == S1101Consts.ROLE_SYSTEMADMIN.ToString())
                    {
                        roleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", roleID), "Administrator");
                        
                    }
                    if (roleID == "1060000000000000004")
                    {
                        roleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", roleID), "Agent");
                        
                    }
                    
                    RoleItem roleItem = new RoleItem();
                    roleItem.RoleId = Convert.ToInt64(roleID);
                    roleItem.Name = roleName;
                    AddChildItem(parentItem, roleItem);
                    mListRoleItems.Add(roleItem);
                    LoadAvaliableRoles(roleItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserControledRoles()
        {
            if (UserItem == null) { return; }
            BasicUserInfo userInfo = UserItem.Data as BasicUserInfo;
            if (userInfo == null) { return; }
            Dispatcher.Invoke(new Action(() => mUserRoles.Clear()));

            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetUserRoleList;
                webRequest.ListData.Add("-1");
                webRequest.ListData.Add(userInfo.UserID.ToString());
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string roleID = webReturn.ListData[i];
                    if (string.IsNullOrEmpty(roleID))
                    {
                        ShowException(string.Format("RoleID invalid.\t{0}", roleID));
                        return;
                    }
                    mUserRoles.Add(roleID);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void ClearChildren(RoleItem parentItem)
        {
            Dispatcher.Invoke(new Action(() => parentItem.Children.Clear()));
        }

        private void AddChildItem(RoleItem parent, RoleItem item)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(item)));
        }

        private void SetRoleCheckState()
        {
            SetRoleCheckState(mRootItem);
        }

        private void SetRoleCheckState(RoleItem parentItem)
        {
            if (parentItem == null) { return; }
            try
            {
                if (parentItem.Children.Count > 0)
                {
                    for (int i = 0; i < parentItem.Children.Count; i++)
                    {
                        SetRoleCheckState(parentItem.Children[i] as RoleItem);
                    }
                }
                else
                {
                    if (mUserRoles.Contains(parentItem.RoleId.ToString()))
                    {
                        parentItem.IsChecked = true;
                    }
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
            SetUserRole();
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

        private void SetUserRole()
        {
            if (UserItem == null) { return; }
            BasicUserInfo userInfo = UserItem.Data as BasicUserInfo;
            if (userInfo == null) { return; }
            List<string> listRoleState = new List<string>();
            SetUserRole(mRootItem, ref listRoleState);
            if (listRoleState.Count > 0)
            {
                int count = listRoleState.Count;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mWorder = new BackgroundWorker();
                mWorder.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1101Codes.SetUserRoles;
                        webRequest.ListData.Add(userInfo.UserID.ToString());
                        webRequest.ListData.Add(count.ToString());
                        for (int i = 0; i < count; i++)
                        {
                            webRequest.ListData.Add(listRoleState[i]);
                        }
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
                            return;
                        }
                        //App.ShowInfoMessage(string.Format("Set user role end."));

                        #region 记录日志

                        string strAdded = string.Empty;
                        string strRemoved = string.Empty;
                        List<string> listLogParams = new List<string>();
                        if (webReturn.ListData != null && webReturn.ListData.Count > 0)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                string strInfo = webReturn.ListData[i];
                                string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                    StringSplitOptions.RemoveEmptyEntries);
                                if (arrInfos.Length >= 2)
                                {
                                    if (arrInfos[0] == "A")
                                    {
                                        var roleItem =
                                            mListRoleItems.FirstOrDefault(r => r.RoleId.ToString() == arrInfos[1]);
                                        if (roleItem != null)
                                        {
                                            strAdded += roleItem.Name + ",";
                                        }
                                        else
                                        {
                                            strAdded += arrInfos[1] + ",";
                                        }
                                    }
                                    if (arrInfos[0] == "D")
                                    {
                                        var roleItem =
                                           mListRoleItems.FirstOrDefault(r => r.RoleId.ToString() == arrInfos[1]);
                                        if (roleItem != null)
                                        {
                                            strRemoved += roleItem.Name + ",";
                                        }
                                        else
                                        {
                                            strRemoved += arrInfos[1] + ",";
                                        }
                                    }
                                }
                            }
                            strAdded = strAdded.TrimEnd(new[] {','});
                            strRemoved = strRemoved.TrimEnd(new[] {','});
                        }
                        listLogParams.Add(userInfo.FullName);
                        listLogParams.Add(strAdded);
                        listLogParams.Add(strRemoved);
                        //App.WriteOperationLog(S1101Consts.OPT_SETUSERROLE.ToString(), ConstValue.OPT_RESULT_SUCCESS, "1101Log0003", listLogParams);
                        CurrentApp.WriteOperationLog(S1101Consts.OPT_SETUSERROLE.ToString(), ConstValue.OPT_RESULT_SUCCESS, "LOG1101001", listLogParams);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorder.RunWorkerCompleted += (s, re) =>
                {
                    mWorder.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                };
                mWorder.RunWorkerAsync();
            }
        }

        private void SetUserRole(RoleItem roleItem, ref List<string> listRoleState)
        {
            for (int i = 0; i < roleItem.Children.Count; i++)
            {
                RoleItem child = roleItem.Children[i] as RoleItem;
                if (child == null) { continue; }
                if (child.IsChecked == true)
                {
                    listRoleState.Add(string.Format("{0}{1}{2}", child.RoleId, ConstValue.SPLITER_CHAR, "1"));
                }
                else if (child.IsChecked == false)
                {
                    listRoleState.Add(string.Format("{0}{1}{2}", child.RoleId, ConstValue.SPLITER_CHAR, "0"));
                }

                SetUserRole(child, ref listRoleState);
            }
        }

        #endregion


        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("11011300", "Set User Role");
                }
                for (int i = 0; i < mRootItem.Children.Count; i++)
                {
                    RoleItem child = mRootItem.Children[i] as RoleItem;
                    if (child == null) { continue; }
                    
                    if (child.RoleId.ToString() == S1101Consts.ROLE_SYSTEMADMIN.ToString())
                    {
                        child.Name = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", child.RoleId.ToString()), "Administrator");
                    }
                    if (child.RoleId.ToString() == "1060000000000000004")
                    {
                        child.Name = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", child.RoleId.ToString()), "Agent");
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}
