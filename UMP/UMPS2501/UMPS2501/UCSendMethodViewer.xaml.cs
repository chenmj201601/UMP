//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3c86eb3f-9d8b-4e33-b30a-39ef8179fdc4
//        CLR Version:              4.0.30319.18063
//        Name:                     UCSendMethodViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501
//        File Name:                UCSendMethodViewer
//
//        created by Charley at 2015/5/28 16:00:03
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UMPS2501.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;

namespace UMPS2501
{
    /// <summary>
    /// UCSendMethodViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCSendMethodViewer
    {

        #region Members

        private ObservableCollection<SendMethodItem> mListSendMethodItems;
        private AlarmReceiverInfo mReceiverInfo;
        private AlarmInfomationItem mAlarmItem;

        #endregion


        public UCSendMethodViewer()
        {
            InitializeComponent();

            mListSendMethodItems = new ObservableCollection<SendMethodItem>();

            Loaded += UCSendMethodViewer_Loaded;
        }

        void UCSendMethodViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainPage != null)
            {
                CurrentApp = MainPage.CurrentApp;
            }
            ListBoxSendMethod.ItemsSource = mListSendMethodItems;

            Init();
            InitOperationButtons();
            InitSendMethodItems();
            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                if (ObjectItem == null) { return; }
                IsSendMethodItemVisiable = ObjectItem.IsChecked == true;
                ObjectItem.SendMothodViewer = this;
                mReceiverInfo = ObjectItem.Data as AlarmReceiverInfo;
                mAlarmItem = ObjectItem.OtherData01 as AlarmInfomationItem;
                if (mReceiverInfo == null && IsSendMethodItemVisiable)
                {
                    mReceiverInfo=new AlarmReceiverInfo();
                    mReceiverInfo.UserID = ObjectItem.ObjID;
                    mReceiverInfo.AlarmInfoID = 0;
                    if (mAlarmItem != null)
                    {
                        mReceiverInfo.AlarmInfoID = mAlarmItem.SerialID;
                    }
                    mReceiverInfo.TenantID = 0;
                    mReceiverInfo.TenantToken = "00000";
                    mReceiverInfo.Method = 0;
                    mReceiverInfo.ReplyMode = 0;
                    ObjectItem.Data = mReceiverInfo;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSendMethodItems()
        {
            try
            {
                mListSendMethodItems.Clear();
                if (mReceiverInfo == null) { return; }
                SendMethodItem item;
                int method = mReceiverInfo.Method;
                if ((method & 1) > 0)
                {
                    //客户端
                    item = new SendMethodItem();
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}001", S2501Consts.BID_ALARM_TERMINAL),
                        "Terminal");
                    item.Icon = "Images/00017.png";
                    item.Info = mReceiverInfo;
                    mListSendMethodItems.Add(item);
                }
                if ((method & 2) > 0)
                {
                    //邮件
                    item = new SendMethodItem();
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}002", S2501Consts.BID_ALARM_TERMINAL),
                        "Email");
                    item.Icon = "Images/00018.png";
                    item.Info = mReceiverInfo;
                    mListSendMethodItems.Add(item);
                }
                if ((method & 4) > 0)
                {
                    //短信
                    item = new SendMethodItem();
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}004", S2501Consts.BID_ALARM_TERMINAL),
                        "Message");
                    item.Icon = "Images/00019.png";
                    item.Info = mReceiverInfo;
                    mListSendMethodItems.Add(item);
                }
                if ((method & 8) > 0)
                {
                    //SNMP
                    item = new SendMethodItem();
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}008", S2501Consts.BID_ALARM_TERMINAL),
                        "SNMP");
                    item.Icon = "Images/00020.png";
                    item.Info = mReceiverInfo;
                    mListSendMethodItems.Add(item);
                }
                if ((method & 16) > 0)
                {
                    //User Application
                    item = new SendMethodItem();
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}016", S2501Consts.BID_ALARM_TERMINAL),
                        "User application");
                    item.Icon = "Images/00021.png";
                    item.Info = mReceiverInfo;
                    mListSendMethodItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitOperationButtons()
        {
            try
            {
                PanelSendMethodButtons.Children.Clear();
                if (ObjectItem == null) { return; }
                if (ObjectItem.Type != S2501Consts.NODE_USER) { return; }
                if (ObjectItem.IsChecked != true) { return; }
                OperationInfo optInfo;
                Button btn;
                //修改发送方式
                optInfo = new OperationInfo();
                optInfo.ID = S2501Consts.OPT_MODIFYSENDMETHOD;
                optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "Reload");
                optInfo.Icon = "Images/00012.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                PanelSendMethodButtons.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Reload()
        {
            Init();
            InitOperationButtons();
            InitSendMethodItems();
        }

        #endregion


        #region ObjectItemProperty

        public static readonly DependencyProperty ObjectItemProperty =
          DependencyProperty.Register("ObjectItem", typeof(ObjectItem), typeof(UCSendMethodViewer), new PropertyMetadata(default(ObjectItem)));

        public ObjectItem ObjectItem
        {
            get { return (ObjectItem)GetValue(ObjectItemProperty); }
            set { SetValue(ObjectItemProperty, value); }
        }

        #endregion


        #region MainPageProperty

        public static readonly DependencyProperty MainPageProperty =
            DependencyProperty.Register("MainPage", typeof(AlarmMessageMainView), typeof(UCSendMethodViewer), new PropertyMetadata(default(AlarmMessageMainView)));

        public AlarmMessageMainView MainPage
        {
            get { return (AlarmMessageMainView)GetValue(MainPageProperty); }
            set { SetValue(MainPageProperty, value); }
        }

        #endregion


        #region IsSendMethodItemVisiableProperty

        public static readonly DependencyProperty IsSendMethodItemVisiableProperty =
          DependencyProperty.Register("IsSendMethodItemVisiable", typeof(bool), typeof(UCSendMethodViewer), new PropertyMetadata(default(bool)));

        public bool IsSendMethodItemVisiable
        {
            get { return (bool)GetValue(IsSendMethodItemVisiableProperty); }
            set { SetValue(IsSendMethodItemVisiableProperty, value); }
        }

        #endregion


        #region EventHandlers

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null) { return; }
            var optItem = btn.DataContext as OperationInfo;
            if (optItem == null) { return; }
            switch (optItem.ID)
            {
                case S2501Consts.OPT_MODIFYSENDMETHOD:
                    ModifySendMethod();
                    break;
            }
        }

        #endregion


        #region Others

        private void ModifySendMethod()
        {
            try
            {
                if (MainPage == null) { return; }
                var popup = MainPage.PopupPanel;
                if (popup != null)
                {
                    string strName = string.Empty;
                    if (ObjectItem != null)
                    {
                        strName = ObjectItem.Name;
                    }
                    popup.Title = string.Format("{0} --- {1}", CurrentApp.GetLanguageInfo("2501201", "Set Send Method"),
                        strName);
                    UCSendMethodEditor editor = new UCSendMethodEditor();
                    editor.CurrentApp = CurrentApp;
                    editor.ObjectItem = ObjectItem;
                    editor.Viewer = this;
                    popup.Content = editor;
                    popup.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangedLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                InitSendMethodItems();
                InitOperationButtons();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Lang", ex.Message);
            }
        }

        #endregion

    }
}
