//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7a500a8e-3afa-4435-8285-8d2accae14bf
//        CLR Version:              4.0.30319.18063
//        Name:                     UCAlarmInfoDetail
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501
//        File Name:                UCAlarmInfoDetail
//
//        created by Charley at 2015/5/22 17:46:03
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMPS2501.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;

namespace UMPS2501
{
    /// <summary>
    /// UCAlarmInfoDetail.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmInfoDetail
    {

        #region Members

        public AlarmMessageMainView PageParent;
        public AlarmInfomationItem AlarmInfoItem;
        public List<BasicDataInfo> ListBasicDataInfos;
        public List<AlarmMessageInfo> ListAlarmMessageInfos;
        public IList<AlarmInfomationItem> ListAllAlarmInfoItems; 

        private ObservableCollection<AlarmLevelItem> mListAlarmLevelItems;

        #endregion


        public UCAlarmInfoDetail()
        {
            InitializeComponent();

            mListAlarmLevelItems = new ObservableCollection<AlarmLevelItem>();

            Loaded += UCAlarmInfoDetail_Loaded;
            Drop += UCAlarmInfoDetail_Drop;
            TxtName.TextChanged += TxtName_TextChanged;
            ComboLevel.SelectionChanged += ComboLevel_SelectionChanged;
        }


        void UCAlarmInfoDetail_Loaded(object sender, RoutedEventArgs e)
        {
            ComboLevel.ItemsSource = mListAlarmLevelItems;

            InitAlarmLevelItems();
            Init();
            ChangeLanguage();
        }


        #region Init and Load

        private void InitAlarmLevelItems()
        {
            try
            {
                mListAlarmLevelItems.Clear();
                AlarmLevelItem item = new AlarmLevelItem();
                item.CurrentApp = CurrentApp;
                item.Level = 0;
                item.Display = "Common";
                item.ListBasicDataInfos = ListBasicDataInfos;
                mListAlarmLevelItems.Add(item);
                item = new AlarmLevelItem();
                item.CurrentApp = CurrentApp;
                item.Level = 1;
                item.Display = "Low Level";
                item.ListBasicDataInfos = ListBasicDataInfos;
                mListAlarmLevelItems.Add(item);
                item = new AlarmLevelItem();
                item.CurrentApp = CurrentApp;
                item.Level = 2;
                item.Display = "Middle Level";
                item.ListBasicDataInfos = ListBasicDataInfos;
                mListAlarmLevelItems.Add(item);
                item = new AlarmLevelItem();
                item.CurrentApp = CurrentApp;
                item.Level = 3;
                item.Display = "High Level";
                item.ListBasicDataInfos = ListBasicDataInfos;
                mListAlarmLevelItems.Add(item);
                item = new AlarmLevelItem();
                item.CurrentApp = CurrentApp;
                item.Level = -1;
                item.Display = "Source Level";
                item.ListBasicDataInfos = ListBasicDataInfos;
                mListAlarmLevelItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Init()
        {
            if (AlarmInfoItem == null) { return; }
            try
            {
                AlarmInfomationItem info = AlarmInfoItem;
                TxtName.Text = info.Name;
                AlarmMessageInfo message = info.Message;
                if (message != null)
                {
                    TxtAlarmType.Text = message.AlarmType.ToString();
                    TxtModule.Text = message.ModuleID.ToString();
                    TxtMessage.Text = message.MessageID.ToString();
                    TxtStatus.Text = message.StatusID.ToString();
                }
                for (int i = 0; i < mListAlarmLevelItems.Count; i++)
                {
                    var item = mListAlarmLevelItems[i];
                    if (item.Level == AlarmInfoItem.Level)
                    {
                        ComboLevel.SelectedItem = item;
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

        void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AlarmInfoItem != null)
            {
                AlarmInfoItem.Name = TxtName.Text;
            }
        }

        void ComboLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var levelItem = ComboLevel.SelectedItem as AlarmLevelItem;
            if (levelItem != null)
            {
                if (AlarmInfoItem != null)
                {
                    AlarmInfoItem.Level = levelItem.Level;
                    AlarmInfoItem.SetPropertyDisplay();
                }
            }
        }

        #endregion


        #region ChangeLangugae

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                LbName.Text = CurrentApp.GetLanguageInfo("COL2501001Name", "Name");
                LbAlarmType.Text = CurrentApp.GetLanguageInfo("COL2501001Type", "Alarm Type");
                LbModule.Text = CurrentApp.GetLanguageInfo("COL2501001Module", "Module");
                LbMessage.Text = CurrentApp.GetLanguageInfo("COL2501001Message", "Message");
                LbStatus.Text = CurrentApp.GetLanguageInfo("COL2501001Status", "Status");
                LbLevel.Text = CurrentApp.GetLanguageInfo("COL2501001Level", "Level");

                if (AlarmInfoItem != null)
                {
                    var message = AlarmInfoItem.Message;
                    if (message != null)
                    {
                        string strType = message.AlarmType.ToString();
                        if (ListBasicDataInfos != null)
                        {
                            var info =
                                ListBasicDataInfos.FirstOrDefault(
                                    b => b.InfoID == S2501Consts.BID_ALARM_TYPE && b.Value == strType);
                            if (info != null)
                            {
                                strType = CurrentApp.GetLanguageInfo(
                                    string.Format("BID{0}{1}", S2501Consts.BID_ALARM_TYPE,
                                        info.SortID.ToString("000")),
                                    info.Icon);
                            }
                        }
                        TxtAlarmType.Text = strType;
                        string strModule = message.ModuleID.ToString();
                        if (ListAlarmMessageInfos != null)
                        {
                            var info =
                                ListAlarmMessageInfos.FirstOrDefault(
                                    m =>
                                        m.AlarmType == message.AlarmType && m.ModuleID == message.ModuleID &&
                                        m.MessageID == 0 && m.StatusID == 0);
                            if (info != null)
                            {
                                strModule = CurrentApp.GetLanguageInfo(
                                    string.Format("{0}MSG{1}", CurrentApp.ModuleID, info.SerialID), info.Description);
                            }
                        }
                        TxtModule.Text = strModule;
                        string strMessage = message.MessageID.ToString();
                        if (ListAlarmMessageInfos != null)
                        {
                            var info =
                                ListAlarmMessageInfos.FirstOrDefault(
                                    m =>
                                        m.AlarmType == message.AlarmType && m.ModuleID == message.ModuleID &&
                                        m.MessageID == message.MessageID && m.StatusID == 0);
                            if (info != null)
                            {
                                strMessage = CurrentApp.GetLanguageInfo(
                                    string.Format("{0}MSG{1}", CurrentApp.ModuleID, info.SerialID), info.Description);
                            }
                        }
                        TxtMessage.Text = strMessage;
                        string strStatus = message.StatusID.ToString();
                        if (ListAlarmMessageInfos != null)
                        {
                            var info =
                                ListAlarmMessageInfos.FirstOrDefault(
                                    m =>
                                        m.AlarmType == message.AlarmType && m.ModuleID == message.ModuleID &&
                                        m.MessageID == message.MessageID && m.StatusID == message.StatusID);
                            if (info != null)
                            {
                                strStatus = CurrentApp.GetLanguageInfo(
                                    string.Format("{0}MSG{1}", CurrentApp.ModuleID, info.SerialID), info.Description);
                            }
                        }
                        if (message.StatusID == 0)
                        {
                            strStatus = CurrentApp.GetLanguageInfo("2501004", "All Status");
                        }
                        TxtStatus.Text = strStatus;
                    }

                }

                for (int i = 0; i < mListAlarmLevelItems.Count; i++)
                {
                    mListAlarmLevelItems[i].SetDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region DragDrop

        void UCAlarmInfoDetail_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var data = e.Data;
                if (data == null) { return; }
                var obj = data.GetData(typeof(ObjectItem)) as ObjectItem;
                if (obj == null) { return; }
                var message = obj.Data as AlarmMessageInfo;
                if (message == null) { return; }
                //只能使用消息和状态类别下的告警消息
                if (message.MessageID == 0 && message.StatusID == 0) { return; }
                if (ListAllAlarmInfoItems != null)
                {
                    var temp = ListAllAlarmInfoItems.FirstOrDefault(i => i.MessageID == message.SerialID);
                    if (temp != null)
                    {
                        ShowException(CurrentApp.GetMessageLanguageInfo("002", "Alarm message already exist."));
                        return;
                    }
                }
                if (AlarmInfoItem == null) { return; }
                AlarmInfoItem.MessageID = message.SerialID;
                AlarmInfoItem.InitItem();
                Init();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        public void Reload()
        {
            Init();
            ChangeLanguage();
        }

        #endregion

    }
}
