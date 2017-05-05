//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b493e502-0201-4fdb-af57-7aed5e805802
//        CLR Version:              4.0.30319.18408
//        Name:                     UCSeatDetail
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCSeatDetail
//
//        created by Charley at 2016/7/4 14:34:04
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Linq;
using System.Windows;
using UMPS4411.Models;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;

namespace UMPS4411
{
    /// <summary>
    /// UCSeatDetail.xaml 的交互逻辑
    /// </summary>
    public partial class UCSeatDetail
    {

        public UCSeatDetail()
        {
            InitializeComponent();

            Loaded += UCSeatDetail_Loaded;
        }

        void UCSeatDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                if (RegionSeatItem == null) { return; }
                CurrentApp = RegionSeatItem.CurrentApp;
                RegionSeatItem.DetailViewer = this;
                TxtExtension.Text = RegionSeatItem.Extension;
                SetInfos();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetStateChanged()
        {
            Dispatcher.Invoke(new Action(SetInfos));
        }

        private void SetInfos()
        {
            try
            {
                if (RegionSeatItem == null) { return; }
                TxtExtension.Text = RegionSeatItem.Extension;
                var extInfo = RegionSeatItem.ExtensionInfo;
                if (extInfo == null)
                {
                    TxtAgentID.Text = string.Empty;
                    TxtStatus.Text = string.Empty;
                    TxtStartTime.Text = string.Empty;
                    TxtDirection.Text = string.Empty;
                    TxtCallerID.Text = string.Empty;
                    TxtCalledID.Text = string.Empty;
                }
                else
                {
                    string strAgentID = string.Empty;
                    string strStatus = string.Empty;
                    string strStartTime = string.Empty;
                    string strDirection = string.Empty;
                    string strCallerID = string.Empty;
                    string strCalledID = string.Empty;
                    string strValue;
                    ExtStateInfo stateInfo;

                    stateInfo =
                       extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_LOGIN);
                    if (stateInfo != null)
                    {
                        if (stateInfo.State > 0)
                        {
                            strValue =
                                CurrentApp.GetLanguageInfo(
                                    string.Format("4411022{0}", stateInfo.State.ToString("000")),
                                    ((LoginState)stateInfo.State).ToString());
                            strStatus += string.Format("{0}; ", strValue);
                            strAgentID = extInfo.AgentID;
                        }
                    }

                    stateInfo =
                        extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_CALL);
                    if (stateInfo != null)
                    {
                        if (stateInfo.State > 0)
                        {
                            strValue =
                               CurrentApp.GetLanguageInfo(
                                   string.Format("4411021{0}", stateInfo.State.ToString("000")),
                                   ((CallState)stateInfo.State).ToString());

                            strStatus += string.Format("{0}; ", strValue);

                            if (stateInfo.State == (int)CallState.Ringing
                                || stateInfo.State == (int)CallState.Talking)
                            {
                                strStartTime = extInfo.VocStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                strCallerID = extInfo.CallerID;
                                strCalledID = extInfo.CalledID;
                            }
                        }
                    }
                    stateInfo =
                       extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                    if (stateInfo != null)
                    {
                        if (stateInfo.State > 0)
                        {
                            strValue =
                              CurrentApp.GetLanguageInfo(
                                  string.Format("4411024{0}", stateInfo.State.ToString("000")),
                                  ((RecordState)stateInfo.State).ToString());

                            strStatus += string.Format("{0}; ", strValue);
                        }
                    }
                    stateInfo =
                      extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                    if (stateInfo != null)
                    {
                        if (stateInfo.State > 0)
                        {
                            strValue =
                             CurrentApp.GetLanguageInfo(
                                 string.Format("4411020{0}", stateInfo.State.ToString("000")),
                                 ((DirectionState)stateInfo.State).ToString());

                            strStatus += string.Format("{0}; ", strValue);
                            strDirection = string.Format("{0}", strValue);
                        }
                    }
                    stateInfo =
                     extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_AGNET);
                    if (stateInfo != null)
                    {
                        if (stateInfo.State > 0)
                        {
                            strValue =
                            CurrentApp.GetLanguageInfo(
                                string.Format("4411023{0}", stateInfo.State.ToString("000")),
                                ((AgentState)stateInfo.State).ToString());

                            strStatus += string.Format("{0}; ", strValue);
                        }
                    }
                    TxtAgentID.Text = strAgentID;
                    TxtStatus.Text = strStatus;
                    TxtStartTime.Text = strStartTime;
                    TxtDirection.Text = strDirection;
                    TxtCallerID.Text = strCallerID;
                    TxtCalledID.Text = strCalledID;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region RegionSeatItem

        public static readonly DependencyProperty RegionSeatItemProperty =
           DependencyProperty.Register("RegionSeatItem", typeof(RegionSeatItem), typeof(UCSeatDetail), new PropertyMetadata(default(RegionSeatItem)));

        public RegionSeatItem RegionSeatItem
        {
            get { return (RegionSeatItem)GetValue(RegionSeatItemProperty); }
            set { SetValue(RegionSeatItemProperty, value); }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                LbAgent.Text = CurrentApp.GetLanguageInfo("4411002", "Agent");
                LbExtension.Text = CurrentApp.GetLanguageInfo("4411003", "Extension");
                LbStatus.Text = CurrentApp.GetLanguageInfo("4411004", "Status");
                LbStartTime.Text = CurrentApp.GetLanguageInfo("4411005", "Start Time");
                LbDirection.Text = CurrentApp.GetLanguageInfo("4411006", "Direction");
                LbCallerID.Text = CurrentApp.GetLanguageInfo("4411007", "Caller");
                LbCalledID.Text = CurrentApp.GetLanguageInfo("4411008", "Called");
            }
            catch (Exception ex) { }
        }

        #endregion

    }
}
