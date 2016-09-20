//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7a3e1267-b4c0-46a1-8a5e-f8a9146d33d8
//        CLR Version:              4.0.30319.18063
//        Name:                     UCSendMethodEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501
//        File Name:                UCSendMethodEditor
//
//        created by Charley at 2015/5/28 15:58:36
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Windows;
using UMPS2501.Models;
using VoiceCyber.UMP.Common25011;
using VoiceCyber.UMP.Controls;

namespace UMPS2501
{
    /// <summary>
    /// UCSendMethodEditor.xaml 的交互逻辑
    /// </summary>
    public partial class UCSendMethodEditor
    {

        #region Members

        public ObjectItem ObjectItem;
        public UCSendMethodViewer Viewer;

        private AlarmReceiverInfo mReceiverInfo;

        #endregion


        public UCSendMethodEditor()
        {
            InitializeComponent();

            Loaded += UCSendMethodEditor_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCSendMethodEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                if (ObjectItem == null) { return; }
                var info = ObjectItem.Data as AlarmReceiverInfo;
                mReceiverInfo = info;
                InitStatus();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStatus()
        {
            try
            {
                if (mReceiverInfo == null) { return; }
                int method = mReceiverInfo.Method;
                int reply = mReceiverInfo.ReplyMode;
                CbUseTerminal.IsChecked = (method & 1) > 0;
                CbUseEmail.IsChecked = (method & 2) > 0;
                CbUseUserApp.IsChecked = (method & 16) > 0;
                CbEmailReply.IsChecked = (method & 2) > 0 && (reply & 2) > 0;
                CbUserAppReply.IsChecked = (method & 16) > 0 && (reply & 16) > 0;
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
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mReceiverInfo == null) { return; }
                bool isUseTerminal = CbUseTerminal.IsChecked == true;
                bool isUseEmail = CbUseEmail.IsChecked == true;
                bool isUseUserApp = CbUseUserApp.IsChecked == true;
                bool isEmailReply = CbEmailReply.IsChecked == true;
                bool isUserAppReply = CbUserAppReply.IsChecked == true;
                int intMethod = (isUseTerminal ? 1 : 0) * 1 +
                                (isUseEmail ? 1 : 0) * 2 +
                                (isUseUserApp ? 1 : 0) * 16;
                mReceiverInfo.Method = intMethod;
                int intReply = (isEmailReply ? 1 : 0) * 2 +
                                (isUserAppReply ? 1 : 0) * 16;
                mReceiverInfo.ReplyMode = intReply;
                if (Viewer != null)
                {
                    Viewer.Reload();
                }
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                    string strName = string.Empty;
                    if (ObjectItem != null)
                    {
                        strName = ObjectItem.Name;
                    }
                    parent.Title = string.Format("{0} --- {1}", CurrentApp.GetLanguageInfo("2501201", "Set Send Method"),
                        strName);
                }

                GroupTerminal.Header = CurrentApp.GetLanguageInfo("2501202", "Alarm Client");
                GroupEmail.Header = CurrentApp.GetLanguageInfo("2501203", "Email");
                GroupUserApp.Header = CurrentApp.GetLanguageInfo("2501207", "User application");
                CbUseTerminal.Content = CurrentApp.GetLanguageInfo("2501204", "Use Alarm Client");
                CbUseEmail.Content = CurrentApp.GetLanguageInfo("2501205", "Use Email");
                CbEmailReply.Content = CurrentApp.GetLanguageInfo("2501206", "Reply");
                CbUseUserApp.Content = CurrentApp.GetLanguageInfo("2501208", "Use user application");
                CbUserAppReply.Content = CurrentApp.GetLanguageInfo("2501206", "Reply");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("2501005", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("2501006", "Close");
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Lang", ex.Message);
            }
        }

        #endregion
    }
}
