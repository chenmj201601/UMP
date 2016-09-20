//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ee80ba05-b1cb-4d5e-a699-0ba52f0e518d
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                MonitorItem
//
//        created by Charley at 2015/7/28 13:47:39
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using VoiceCyber.Common;
using VoiceCyber.SDKs.NMon;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21021;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Controls;

namespace UMPS2102.Models
{
    public class MonitorItem : INotifyPropertyChanged
    {
        public MonitorItem()
        {
            ResetState();
        }

        public UMPApp CurrentApp;

        #region Basic Members

        public long ObjID { get; set; }
        public int ObjType { get; set; }
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }
        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        private string mExtension;

        public string Extension
        {
            get { return mExtension; }
            set { mExtension = value; OnPropertyChanged("Extension"); }
        }

        private string mAgentID;

        public string AgentID
        {
            get { return mAgentID; }
            set { mAgentID = value; OnPropertyChanged("AgentID"); }
        }

        #endregion


        #region State Members

        /// <summary>
        /// 0：未登录
        /// 1：仅登录到VoiceServer
        /// 2：仅登录到ScreenServer
        /// 3：同时登录到VoiceServer和ScreenServer
        /// </summary>
        public int LoginState { get; set; }
        public int CallState { get; set; }
        /// <summary>
        /// 0：无
        /// 1：录音中
        /// 2：录屏中
        /// 3：录音录屏中
        /// </summary>
        public int RecordState { get; set; }

        private string mStrLoginState;

        public string StrLoginState
        {
            get { return mStrLoginState; }
            set { mStrLoginState = value; OnPropertyChanged("StrLoginState"); }
        }

        private string mStrCallState;

        public string StrCallState
        {
            get { return mStrCallState; }
            set { mStrCallState = value; OnPropertyChanged("StrCallState"); }
        }

        private string mStrRecordState;

        public string StrRecordState
        {
            get { return mStrRecordState; }
            set { mStrRecordState = value; OnPropertyChanged("StrRecordState"); }
        }

        #endregion


        #region CallInfo Members

        private string mRecordReference;

        public string RecordReference
        {
            get { return mRecordReference; }
            set { mRecordReference = value; OnPropertyChanged("RecordReference"); }
        }

        private string mStartRecordTime;

        public string StartRecordTime
        {
            get { return mStartRecordTime; }
            set { mStartRecordTime = value; OnPropertyChanged("StartRecordTime"); }
        }

        private string mStopRecordTime;

        public string StopRecordTime
        {
            get { return mStopRecordTime; }
            set { mStopRecordTime = value; OnPropertyChanged("StopRecordTime"); }
        }

        private string mRecordLength;

        public string RecordLength
        {
            get { return mRecordLength; }
            set { mRecordLength = value; OnPropertyChanged("RecordLength"); }
        }

        public double VocRecordLength { get; set; }

        public double ScrRecordLength { get; set; }

        private int mDirectionFlag;

        public int DirectionFlag
        {
            get { return mDirectionFlag; }
            set { mDirectionFlag = value; OnPropertyChanged("DirectionFlag"); }
        }

        private string mStrDirection;

        public string StrDirection
        {
            get { return mStrDirection; }
            set { mStrDirection = value; OnPropertyChanged("StrDirectionFlag"); }
        }

        private string mCallerID;

        public string CallerID
        {
            get { return mCallerID; }
            set { mCallerID = value; OnPropertyChanged("CallerID"); }
        }

        private string mCalledID;

        public string CalledID
        {
            get { return mCalledID; }
            set { mCalledID = value; OnPropertyChanged("CalledID"); }
        }

        private string mRealExtension;

        public string RealExtension
        {
            get { return mRealExtension; }
            set { mRealExtension = value; OnPropertyChanged("RealExtension"); }
        }

        private int mVoiceFormat;

        public int VoiceFormat
        {
            get { return mVoiceFormat; }
            set { mVoiceFormat = value; OnPropertyChanged("VoiceFormat"); }
        }

        private string mStrVoiceFormat;

        public string StrVoiceFormat
        {
            get { return mStrVoiceFormat; }
            set { mStrVoiceFormat = value; OnPropertyChanged("StrVoiceFormat"); }
        }

        #endregion


        #region ServerInfo Members

        private string mServerChannel;

        public string ServerChannel
        {
            get { return mServerChannel; }
            set { mServerChannel = value; OnPropertyChanged("ServerChannel"); }
        }

        private string mChanID;

        public string ChanID
        {
            get { return mChanID; }
            set { mChanID = value; OnPropertyChanged("ChanID"); }
        }

        private string mServerID;

        public string ServerID
        {
            get { return mServerID; }
            set { mServerID = value; OnPropertyChanged("ServerID"); }
        }

        private string mServerAddress;

        public string ServerAddress
        {
            get { return mServerAddress; }
            set { mServerAddress = value; OnPropertyChanged("ServerAddress"); }
        }

        #endregion


        #region Other Members

        public MonitorData Data { get; set; }
        public MonitorObject VoiceChanMonObject { get; set; }
        public MonitorObject ScreenChanMonObject { get; set; }
        public ChanState VoiceChanState { get; set; }
        public ChanState ScreenChanState { get; set; }
        public double TimeDeviation { get; set; }

        public List<UserParamInfo> ListUserParams;

        /// <summary>
        /// 0：无
        /// 1：仅录音
        /// 2：仅录屏
        /// 3：录音录屏
        /// </summary>
        public int Role { get; set; }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        private Brush mDirectionForeground;

        public Brush DirectionForeground
        {
            get { return mDirectionForeground; }
            set { mDirectionForeground = value; OnPropertyChanged("DirectionForeground"); }
        }

        private bool mIsLogged;

        public bool IsLogged
        {
            get { return mIsLogged; }
            set { mIsLogged = value; OnPropertyChanged("IsLogged"); }
        }

        private bool mIsRecording;

        public bool IsRecording
        {
            get { return mIsRecording; }
            set { mIsRecording = value; OnPropertyChanged("IsRecording"); }
        }

        #endregion


        #region Public Functions

        /// <summary>
        /// 根据MonitorData创建一个MonitorItem
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MonitorItem CreateItem(MonitorData data)
        {
            MonitorItem item = new MonitorItem();
            item.ObjID = data.ObjID;
            item.ObjType = data.ObjType;
            item.Name = data.Name;
            switch (data.ObjType)
            {
                case ConstValue.RESOURCE_AGENT:
                    item.Icon = string.Format("Images/00002.png");
                    break;
                case ConstValue.RESOURCE_EXTENSION:
                    item.Icon = string.Format("Images/00003.png");
                    break;
                case  ConstValue.RESOURCE_REALEXT:
                    item.Icon = string.Format("Images/00015.png");
                    break;
            }
            item.Data = data;
            return item;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        public void UpdateState()
        {
            //ResetState();
            Role = 0;
            LoginState = 0;
            RecordState = 0;
            if (VoiceChanMonObject != null)
            {
                if (VoiceChanState != null)
                {
                    Role = (Role | 1);
                }
                if (VoiceChanState != null
                    && VoiceChanState.LoginState == VoiceCyber.UMP.CommonService04.LoginState.LogOn)
                {
                    LoginState = (LoginState | 1);
                }
                if (VoiceChanState != null
                    && VoiceChanState.RecordState == VoiceCyber.UMP.CommonService04.RecordState.Recoding)
                {
                    RecordState = (RecordState | 1);
                }
            }
            if (ScreenChanMonObject != null)
            {
                if (ScreenChanState != null)
                {
                    Role = (Role | 2);
                }
                if (ScreenChanState != null
                    && ScreenChanState.LoginState == VoiceCyber.UMP.CommonService04.LoginState.LogOn)
                {
                    LoginState = (LoginState | 2);
                }
                if (ScreenChanState != null
                    && ScreenChanState.RecordState == VoiceCyber.UMP.CommonService04.RecordState.Recoding)
                {
                    RecordState = (RecordState | 2);
                }
            }
            if (Role == 1)
            {
                if (VoiceChanMonObject != null)
                {
                    ChanID = VoiceChanMonObject.Other01;
                    ServerID = VoiceChanMonObject.Other02;
                    ServerAddress = VoiceChanMonObject.Other03;
                    ServerChannel = string.Format("[{0}]{1}", ChanID, ServerAddress);
                }
                if (VoiceChanState != null)
                {
                    Extension = VoiceChanState.Extension;
                    AgentID = VoiceChanState.AgentID;
                    RecordReference = VoiceChanState.RecordReference;
                    StartRecordTime = GetRecordTime(VoiceChanState.StartRecordTime);
                    StopRecordTime = RecordState == 1
                        ? string.Empty
                        : GetRecordTime(VoiceChanState.StopRecordTime);
                    if (RecordState == 1)
                    {
                        VocRecordLength = DealRecordLength(VoiceChanState);
                        RecordLength = Converter.Second2Time(VocRecordLength);
                    }
                    DirectionFlag = VoiceChanState.DirectionFlag;
                    StrDirection = DirectionFlag == 1
                        ? CurrentApp.GetLanguageInfo("2102014", "Callin")
                        : CurrentApp.GetLanguageInfo("2102015", "Callout");
                    CallerID = VoiceChanState.CallerID;
                    CalledID = VoiceChanState.CalledID;
                    RealExtension = VoiceChanState.Other01;
                }
            }
            if (Role == 2)
            {
                if (ScreenChanMonObject != null)
                {
                    ChanID = ScreenChanMonObject.Other01;
                    ServerID = ScreenChanMonObject.Other02;
                    ServerAddress = ScreenChanMonObject.Other03;
                    ServerChannel = string.Format("[{0}]{1}", ChanID, ServerAddress);
                }
                if (ScreenChanState != null)
                {
                    Extension = ScreenChanState.Extension;
                    AgentID = ScreenChanState.AgentID;
                    RecordReference = ScreenChanState.RecordReference;
                    StartRecordTime = GetRecordTime(ScreenChanState.StartRecordTime);
                    StopRecordTime = RecordState == 2
                        ? string.Empty
                        : GetRecordTime(ScreenChanState.StopRecordTime);
                    if (RecordState == 2)
                    {
                        ScrRecordLength = DealRecordLength(ScreenChanState);
                        RecordLength = Converter.Second2Time(ScrRecordLength);
                    }
                }
            }
            if (Role == 3)
            {
                if (VoiceChanMonObject != null
                    && ScreenChanMonObject != null)
                {
                    var chanID1 = VoiceChanMonObject.Other01;
                    var serverID1 = VoiceChanMonObject.Other02;
                    var serverAddress1 = VoiceChanMonObject.Other03;
                    var serverChannel1 = string.Format("[{0}]{1}", chanID1, serverAddress1);
                    var chanID2 = ScreenChanMonObject.Other01;
                    var serverID2 = ScreenChanMonObject.Other02;
                    var serverAddress2 = ScreenChanMonObject.Other03;
                    var serverChannel2 = string.Format("[{0}]{1}", chanID2, serverAddress2);
                    ChanID = string.Format("V:{0} S:{1}", chanID1, chanID2);
                    ServerID = string.Format("V:{0} S:{1}", serverID1, serverID2);
                    ServerAddress = string.Format("V:{0} S:{1}", serverAddress1, serverAddress2);
                    ServerChannel = string.Format("V:{0} S:{1}", serverChannel1, serverChannel2);
                }
                if (VoiceChanState != null
                    && ScreenChanState != null)
                {
                    Extension = string.Format("V:{0} S:{1}", VoiceChanState.Extension, ScreenChanState.Extension);
                    AgentID = string.Format("V:{0} S:{1}", VoiceChanState.AgentID, ScreenChanState.AgentID);
                    RecordReference = string.Format("V:{0} S:{1}", VoiceChanState.RecordReference,
                        ScreenChanState.RecordReference);
                    var start1 = GetRecordTime(VoiceChanState.StartRecordTime);
                    var start2 = GetRecordTime(ScreenChanState.StartRecordTime);
                    StartRecordTime = string.Format("V:{0} S:{1}", start1, start2);
                    if (RecordState != 0)
                    {
                        StopRecordTime = string.Format("V: S:");
                    }
                    else
                    {
                        var stop1 = (RecordState & 1) != 0
                            ? string.Empty
                            : GetRecordTime(VoiceChanState.StopRecordTime);
                        var stop2 = (RecordState & 2) != 0
                            ? string.Empty
                            : GetRecordTime(ScreenChanState.StopRecordTime);
                        StopRecordTime = string.Format("V:{0} S:{1}", stop1, stop2);
                    }
                    if (RecordState != 0)
                    {
                        if ((RecordState & 1) > 0)
                        {
                            VocRecordLength = DealRecordLength(VoiceChanState);
                        }
                        if ((RecordState & 2) > 0)
                        {
                            ScrRecordLength = DealRecordLength(ScreenChanState);
                        }
                    }
                    RecordLength = string.Format("V:{0} S:{1}", Converter.Second2Time(VocRecordLength),
                        Converter.Second2Time(ScrRecordLength));
                    DirectionFlag = VoiceChanState.DirectionFlag;
                    StrDirection = DirectionFlag == 1
                        ? CurrentApp.GetLanguageInfo("2102014", "Callin")
                        : CurrentApp.GetLanguageInfo("2102015", "Callout");
                    CallerID = VoiceChanState.CallerID;
                    CalledID = VoiceChanState.CalledID;
                    RealExtension = VoiceChanState.Other01;
                }
            }
            StrLoginState = LoginState == 0
                ? string.Empty
                : CurrentApp.GetLanguageInfo(
                    string.Format("BID{0}{1}", S2102Consts.BID_LOGSTATE, LoginState.ToString("000")),
                    LoginState.ToString());
            StrCallState = CallState == 0 ? string.Empty : CurrentApp.GetLanguageInfo(
                   string.Format("BID{0}{1}", S2102Consts.BID_CALLSTATE, CallState.ToString("000")),
                   CallState.ToString());
            StrRecordState = RecordState == 0 ? string.Empty : CurrentApp.GetLanguageInfo(
                    string.Format("BID{0}{1}", S2102Consts.BID_RECORDSTATE, RecordState.ToString("000")),
                    RecordState.ToString());
            try
            {
                StrVoiceFormat = ((EVLVoiceFormat)VoiceFormat).ToString();
            }
            catch { }

            IsLogged = LoginState != 0;
            IsRecording = RecordState != 0;

            Background = Brushes.Transparent;

            #region 设置登录状态

            if (IsLogged)
            {
                Background = Brushes.Wheat;
                if (ListUserParams != null)
                {
                    if (LoginState == 1)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCLOGINSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    if (LoginState == 2)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRLOGINSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    if (LoginState == 3)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRLOGINSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                }

            }

            #endregion


            #region 设置录制状态

            if (IsRecording)
            {
                Background = Brushes.LightCoral;
                if (DirectionFlag == 1)
                {
                    DirectionForeground = Brushes.DarkBlue;
                }
                else
                {
                    DirectionForeground = Brushes.DarkGreen;
                }
                if (ListUserParams != null)
                {
                    if (RecordState == 1)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCRECORDSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    if (RecordState == 2)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRRECORDSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    if (RecordState == 3)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRRECORDSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                Background = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    //设置呼叫方向状态颜色
                    if (DirectionFlag == 1)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLINSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                DirectionForeground = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLOUTSTATE);
                        if (userParam != null)
                        {
                            try
                            {
                                Color color = Utils.GetColorFromRgbString(userParam.ParamValue);
                                DirectionForeground = new SolidColorBrush(color);
                            }
                            catch { }
                        }
                    }
                }
            }

            #endregion
        }

        #endregion


        #region Others

        private void ResetState()
        {
            //重置状态信息
            Role = 0;
            Extension = string.Empty;
            AgentID = string.Empty;
            LoginState = 0;
            CallState = 0;
            RecordState = 0;
            StrLoginState = string.Empty;
            StrCallState = string.Empty;
            StrRecordState = string.Empty;
            RecordReference = string.Empty;
            StartRecordTime = string.Empty;
            StopRecordTime = string.Empty;
            RecordLength = string.Empty;
            DirectionFlag = 0;
            StrDirection = string.Empty;
            CallerID = string.Empty;
            CalledID = string.Empty;
            VoiceFormat = 0;
            StrVoiceFormat = string.Empty;
            ChanID = string.Empty;
            ServerAddress = string.Empty;
            ServerChannel = string.Empty;
        }

        private string GetRecordTime(DateTime dt)
        {
            string strReturn = string.Empty;
            try
            {
                if (dt > DateTime.Parse("2014/1/1"))
                {
                    dt = dt.ToLocalTime();
                    strReturn = dt.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            catch { }
            return strReturn;
        }

        private double DealRecordLength(ChanState chanState)
        {
            double serverTimeDeviation = chanState.TimeDeviation;
            double thisTimeDeviation = TimeDeviation;
            double timeDeviation = serverTimeDeviation + thisTimeDeviation;
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime start = chanState.StartRecordTime;
            if (start > DateTime.Parse("2014/1/1"))
            {
                TimeSpan ts = now - start;
                return (int)(ts.TotalSeconds - timeDeviation);
            }
            return 0;
        }

        #endregion


        #region PropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }
}
