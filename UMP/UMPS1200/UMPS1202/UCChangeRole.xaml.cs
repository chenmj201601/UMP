//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    41221a84-f0ca-4b28-a1cd-73b310b96bf4
//        CLR Version:              4.0.30319.42000
//        Name:                     UCChangeRole
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1202
//        File Name:                UCChangeRole
//
//        created by Charley at 2016/3/31 18:40:06
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1202
{
    /// <summary>
    /// UCChangeRole.xaml 的交互逻辑
    /// </summary>
    public partial class UCChangeRole 
    {

        public LoginView PageParent;
        public List<RoleInfo> ListRoleInfos; 

        private bool mIsInited;
        private string mOptResult;
        private ObservableCollection<RoleItem> mListRoleItems; 

        public const string RESULT_CANCEL = "Cancel";
        public const string RESULT_SUCC = "Succ";


        public UCChangeRole()
        {
            InitializeComponent();

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
                if (ListRoleInfos == null) { return;}
                mListRoleItems.Clear();
                for (int i = 0; i < ListRoleInfos.Count; i++)
                {
                    RoleInfo info = ListRoleInfos[i];

                    RoleItem item=new RoleItem();
                    item.RoleID = info.ID;
                    item.Name = info.Name;
                    item.IsChecked = false;
                    item.Info = info;
                    GetRoleName(item);
                    mListRoleItems.Add(item);
                }
                var current = mListRoleItems.FirstOrDefault(r => r.RoleID == CurrentApp.Session.RoleID);
                if (current == null)
                {
                    current = mListRoleItems.FirstOrDefault();
                    if (current == null)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("S0000056", "RoleItem is null"));
                        return;
                    }
                }
                current.IsChecked = true;
                BtnConfirm.Focus();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetRoleName(RoleItem roleItem)
        {
            try
            {
                var info = roleItem.Info;
                if (info != null)
                {
                    roleItem.Name = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", info.ID), info.Name);
                }
            }
            catch { }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = ListBoxRoles.SelectedItem as RoleItem;
                if (item == null) { return;}
                var roleInfo = item.Info;
                if (roleInfo == null) { return;}
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

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("S0000045", "Select a role");
                }

                for (int i = 0; i < mListRoleItems.Count; i++)
                {
                    var item = mListRoleItems[i];
                    GetRoleName(item);
                }

                TxtTip.Text = string.Format(CurrentApp.GetLanguageInfo("S0000046", "{0} has these roles"),
                    CurrentApp.Session.UserInfo.Account);
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("S0000047", "Confirm");
                BtnCancel.Content = CurrentApp.GetLanguageInfo("S0000048", "Cancel");
            }
            catch { }
        }
    }
}
