//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0b71eb42-0174-4557-a3c7-f73b2c4e57f2
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                MonitorObjectItem
//
//        created by Charley at 2015/6/29 17:36:31
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

namespace UMPS2102.Models
{
    public class MonitorObjectItem : INotifyPropertyChanged
    {

        #region Private Members

        private string mName;
        private string mIcon;
        private string mExtension;
        private string mAgentID;


        private LoginState mLoginState;
        private CallState mCallState;
        private RecordState mRecordState;

        private string mRecordReference;
        private int mDirectionFlag;
        private string mCallerID;
        private string mCalledID;
        private DateTime mStartRecordTime;
        private DateTime mStopRecordTime;
        private int mRecordLength;

        private int mChanID;
        private int mServerID;
        private string mServerAddress;
        private string mServerChannel;

        private int mVoiceFormat;

        #endregion


        #region String Members

        private string mStrIcon;
        private string mStrLoginState;
        private string mStrCallState;
        private string mStrRecordState;
        private string mStrDirectionFlag;
        private string mStrStartRecordTime;
        private string mStrStopRecordTime;
        private string mStrRecordLength;
        private string mStrVoiceFormat;

        #endregion


        #region Other Members

        private Brush mBackground;
        private bool mIsLogged;
        private bool mIsRecording;

        #endregion


        #region Public Properties

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        public string Extension
        {
            get { return mExtension; }
            set { mExtension = value; OnPropertyChanged("Extension"); }
        }

        public string AgentID
        {
            get { return mAgentID; }
            set { mAgentID = value; OnPropertyChanged("AgentID"); }
        }

        public LoginState LoginState
        {
            get { return mLoginState; }
            set { mLoginState = value; OnPropertyChanged("LoginState"); }
        }

        public CallState CallState
        {
            get { return mCallState; }
            set { mCallState = value; OnPropertyChanged("CallState"); }
        }

        public RecordState RecordState
        {
            get { return mRecordState; }
            set { mRecordState = value; OnPropertyChanged("RecordState"); }
        }

        public string RecordReference
        {
            get { return mRecordReference; }
            set { mRecordReference = value; OnPropertyChanged("RecordReference"); }
        }

        public int DirectionFlag
        {
            get { return mDirectionFlag; }
            set { mDirectionFlag = value; OnPropertyChanged("DirectionFlag"); }
        }

        public string CallerID
        {
            get { return mCallerID; }
            set { mCallerID = value; OnPropertyChanged("CallerID"); }
        }

        public string CalledID
        {
            get { return mCalledID; }
            set { mCalledID = value; OnPropertyChanged("CalledID"); }
        }

        public DateTime StartRecordTime
        {
            get { return mStartRecordTime; }
            set { mStartRecordTime = value; OnPropertyChanged("StartRecordTime"); }
        }

        public DateTime StopRecordTime
        {
            get { return mStopRecordTime; }
            set { mStopRecordTime = value; OnPropertyChanged("StopRecordTime"); }
        }

        public int RecordLength
        {
            get { return mRecordLength; }
            set { mRecordLength = value; OnPropertyChanged("RecordLength"); }
        }

        public int ChanID
        {
            get { return mChanID; }
            set { mChanID = value; OnPropertyChanged("ChanID"); }
        }

        public int ServerID
        {
            get { return mServerID; }
            set { mServerID = value; OnPropertyChanged("ServerID"); }
        }

        public string ServerAddress
        {
            get { return mServerAddress; }
            set { mServerAddress = value; OnPropertyChanged("ServerAddress"); }
        }

        public string ServerChannel
        {
            get { return mServerChannel; }
            set { mServerChannel = value; OnPropertyChanged("ServerChannel"); }
        }

        public int VoiceFormat
        {
            get { return mVoiceFormat; }
            set { mVoiceFormat = value; OnPropertyChanged("VoiceFormat"); }
        }

        #endregion


        #region Public String Properties

        public string StrIcon
        {
            get { return mStrIcon; }
            set { mStrIcon = value; OnPropertyChanged("StrIcon"); }
        }

        public string StrLoginState
        {
            get { return mStrLoginState; }
            set { mStrLoginState = value; OnPropertyChanged("StrLoginState"); }
        }

        public string StrCallState
        {
            get { return mStrCallState; }
            set { mStrCallState = value; OnPropertyChanged("StrCallState"); }
        }

        public string StrRecordState
        {
            get { return mStrRecordState; }
            set { mStrRecordState = value; OnPropertyChanged("StrRecordState"); }
        }

        public string StrDirection
        {
            get { return mStrDirectionFlag; }
            set { mStrDirectionFlag = value; OnPropertyChanged("StrDirection"); }
        }

        public string StrStartRecordTime
        {
            get { return mStrStartRecordTime; }
            set { mStrStartRecordTime = value; OnPropertyChanged("StrStartRecordTime"); }
        }

        public string StrStopRecordTime
        {
            get { return mStrStopRecordTime; }
            set { mStrStopRecordTime = value; OnPropertyChanged("StrStopRecordTime"); }
        }

        public string StrRecordLength
        {
            get { return mStrRecordLength; }
            set { mStrRecordLength = value; OnPropertyChanged("StrRecordLength"); }
        }

        public string StrVoiceFormat
        {
            get { return mStrVoiceFormat; }
            set { mStrVoiceFormat = value; OnPropertyChanged("StrVoiceFormat"); }
        }

        #endregion


        #region Other Public Properties

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        public bool IsLogged
        {
            get { return mIsLogged; }
            set { mIsLogged = value; OnPropertyChanged("IsLogged"); }
        }

        public bool IsRecording
        {
            get { return mIsRecording; }
            set { mIsRecording = value; OnPropertyChanged("IsRecording"); }
        }

        #endregion


        #region Others

        public long ObjID { get; set; }
        public int ObjType { get; set; }
        public long ChanObjID { get; set; }
        public double TimeDeviation { get; set; }

        public List<UserParamInfo> ListUserParams; 

        public MonitorObject Info { get; set; }
        public ChanState ChanState { get; set; }

        #endregion
      

        /// <summary>
        /// 创建一个新项目
        /// </summary>
        /// <param name="monObj"></param>
        /// <returns></returns>
        public static MonitorObjectItem CreateItem(MonitorObject monObj)
        {
            MonitorObjectItem item=new MonitorObjectItem();
            item.ObjID = monObj.ObjID;
            item.ObjType = monObj.ObjType;
            item.Name = monObj.ObjValue;
            item.Info = monObj;
            switch (monObj.ObjType)
            {
                case ConstValue.RESOURCE_AGENT:
                    item.Icon = string.Format("Images/00002.png");
                    break;
                case ConstValue.RESOURCE_EXTENSION:
                    item.Icon = string.Format("Images/00003.png");
                    break;
            }
            return item;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="monObj"></param>
        public void UpdateInfo(MonitorObject monObj)
        {
            int intValue;

            ChanObjID = monObj.ChanObjID;
            if (int.TryParse(monObj.Other01, out intValue))
            {
                ChanID = intValue;
            }
            if (int.TryParse(monObj.Other02, out intValue))
            {
                ServerID = intValue;
            }
            ServerAddress = monObj.Other03;
            ServerChannel = string.Format("{0} [{1}]", ServerAddress, ChanID);

            Info = monObj;
        }

        /// <summary>
        /// 更新状态信息
        /// </summary>
        public void UpdateState()
        {
            if (Info == null) { return; }
            switch (ObjType)
            {
                case ConstValue.RESOURCE_AGENT:
                case ConstValue.RESOURCE_EXTENSION:
                    if (Info.ChanObjID <= 0)
                    {
                        LoginState = LoginState.LogOff;
                        CallState = CallState.Idle;
                        RecordState = RecordState.None;
                    }
                    else
                    {
                        if (ChanState != null)
                        {
                            Extension = ChanState.Extension;
                            AgentID = ChanState.AgentID;
                            LoginState = ChanState.LoginState;
                            CallState = ChanState.CallState;
                            RecordState = ChanState.RecordState;
                            RecordReference = ChanState.RecordReference;
                            DirectionFlag = ChanState.DirectionFlag;
                            CallerID = ChanState.CallerID;
                            CalledID = ChanState.CalledID;
                            StartRecordTime = ChanState.StartRecordTime;
                            StopRecordTime = ChanState.StopRecordTime;
                            DealRecordLength(ChanState);
                        }
                    }
                    break;
            }
            StrIcon = Icon;
            StrLoginState = LoginState == LoginState.LogOff
                ? string.Empty
                : App.GetLanguageInfo(
                    string.Format("BID{0}{1}", S2102Consts.BID_LOGSTATE, ((int) LoginState).ToString("000")),
                    LoginState.ToString());
            StrCallState = CallState == CallState.Idle ? string.Empty : App.GetLanguageInfo(
                    string.Format("BID{0}{1}", S2102Consts.BID_CALLSTATE, ((int)CallState).ToString("000")),
                    CallState.ToString());
            StrRecordState = RecordState == RecordState.None ? string.Empty : App.GetLanguageInfo(
                    string.Format("BID{0}{1}",S2102Consts.BID_RECORDSTATE, ((int)RecordState).ToString("000")),
                    RecordState.ToString());
            StrDirection = DirectionFlag == 1
                ? App.GetLanguageInfo("2102014", "Callin")
                : App.GetLanguageInfo("2102015", "Callout");
            StrStartRecordTime = StartRecordTime > DateTime.Parse("2014/1/1")
                ? StartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                : string.Empty;
            StrStopRecordTime = StopRecordTime > DateTime.Parse("2014/1/1")
                ? StopRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                : string.Empty;
            StrRecordLength = RecordLength > 0 ? Converter.Second2Time(RecordLength) : "00:00:00";

            Background = Brushes.Transparent;
            if (LoginState == LoginState.LogOn)
            {
                Background = Brushes.Wheat;
                if (ListUserParams != null)
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
            }
            if (RecordState == RecordState.Recoding)
            {
                Background = Brushes.Thistle;
                if (DirectionFlag == 1)
                {
                    Background = Brushes.Thistle;
                }
                else
                {
                    Background = Brushes.Violet;
                }
                if (ListUserParams != null)
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
                    if (DirectionFlag == 1)
                    {
                        userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLINSTATE);
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
                    else
                    {
                        userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLOUTSTATE);
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

            IsLogged = LoginState == LoginState.LogOn;
            IsRecording = RecordState == RecordState.Recoding;

            if (IsRecording)
            {
                try
                {
                    StrRecordLength = Converter.Second2Time(RecordLength);
                }catch{}
            }
            else
            {
                StrRecordLength = string.Empty;
            }

            try
            {
                StrVoiceFormat = ((EVLVoiceFormat) VoiceFormat).ToString();
            }catch{}
        }

        private void DealRecordLength(ChanState chanState)
        {
            double serverTimeDeviation = chanState.TimeDeviation;
            double thisTimeDeviation = TimeDeviation;
            double timeDeviation = serverTimeDeviation + thisTimeDeviation;
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime start = StartRecordTime;
            if (start > DateTime.Parse("2014/1/1"))
            {
                TimeSpan ts = now - start;
                RecordLength = (int)(ts.TotalSeconds - timeDeviation);
            }
        }


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
