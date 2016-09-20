//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    31702507-dc1e-470b-b907-d3780184b2c1
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAlarmPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCAlarmPanel
//
//        created by Charley at 2016/7/16 14:09:21
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using UMPS4411.Models;
using VoiceCyber.UMP.Common44101;

namespace UMPS4411
{
    /// <summary>
    /// UCAlarmPanel.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmPanel
    {
        public StateDurationInfo StateDuration;
        public StateAlarmInfo StateAlarmInfo;
        public UCWorkRegionMap PageParent;

        private bool mIsInited;

        public UCAlarmPanel()
        {
            InitializeComponent();

            Loaded += UCAlarmPanel_Loaded;
            BtnClose.Click += BtnClose_Click;
        }

        void UCAlarmPanel_Loaded(object sender, RoutedEventArgs e)
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
                if (StateDuration == null) { return; }
                if (StateAlarmInfo == null) { return; }
                TxtTitle.Text = StateDuration.Extension;
                var alarmMsg = StateAlarmInfo.AlarmMessage;
                string strContent = string.Empty;
                if (alarmMsg != null)
                {
                    strContent = alarmMsg.Content;
                }
                FormatAlarmContent(strContent);
                TxtAlarmTime.Text = StateAlarmInfo.AlarmTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void FormatAlarmContent(string strContent)
        {
            try
            {
                string strExt = string.Empty;
                string strAgt = string.Empty;
                double doubleLength = 0.0;
                if (StateDuration != null)
                {
                    var seatItem = StateDuration.SeatItem;
                    if (seatItem != null)
                    {
                        strExt = seatItem.Extension;
                        var extInfo = seatItem.ExtensionInfo;
                        if (extInfo != null)
                        {
                            strAgt = extInfo.AgentID;
                        }
                    }
                }
                if (StateAlarmInfo != null)
                {
                    var alarmMsg = StateAlarmInfo.AlarmMessage;
                    if (alarmMsg != null)
                    {
                        if (double.TryParse(alarmMsg.Value, out doubleLength))
                        {
                            
                        }
                    }
                }
                strContent = strContent.Replace(S4410Consts.ALARM_CONTENT_KEYWORD_AGENTID, strAgt);
                strContent = strContent.Replace(S4410Consts.ALARM_CONTENT_KEYWORD_EXTENSION, strExt);
                strContent = strContent.Replace(S4410Consts.ALARM_CONTENT_KEYWORD_TIMEOUTLENGTH,
                    doubleLength.ToString("0.0"));
                TxtContent.Text = strContent;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PageParent != null)
                {
                    PageParent.RemoveAlarmPanel(this);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
