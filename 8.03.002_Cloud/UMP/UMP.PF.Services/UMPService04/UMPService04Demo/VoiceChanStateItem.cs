//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6894d094-8221-49e3-a208-d1714f96fd9c
//        CLR Version:              4.0.30319.18063
//        Name:                     VoiceChanStateItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04Demo
//        File Name:                VoiceChanStateItem
//
//        created by Charley at 2015/6/25 17:27:06
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using VoiceCyber.UMP.CommonService04;

namespace UMPService04Demo
{
    public class VoiceChanStateItem:INotifyPropertyChanged
    {
        private string mExtension;
        private string mAgentID;

        private string mStrLoginState;
        private string mStrCallState;
        private string mStrRecordState;

        private string mRecordReference;
        private string mStrDirectionFlag;
        private string mCallerID;
        private string mCalledID;
        private string mStrStartRecordTime;
        private string mStrStopRecordTime;
        private string mStrRecordLength;


        public long ChanObjID { get; set; }
        public int ChanID { get; set; }
        public int VoiceID { get; set; }

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

        public LoginState LoginState { get; set; }
        public CallState CallState { get; set; }
        public RecordState RecordState { get; set; }

        public string RecordReference
        {
            get { return mRecordReference; }
            set { mRecordReference = value; OnPropertyChanged("RecordReference"); }
        }

        public int DirectionFlag { get; set; }

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
        public DateTime StartRecordTime { get; set; }
        public DateTime StopRecordTime { get; set; }
        public int RecordLength { get; set; }

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
            get {return mStrStopRecordTime; }
            set { mStrStopRecordTime = value; OnPropertyChanged("StrStopRecordTime"); }
        }

        public string StrRecordLength
        {
            get { return mStrRecordLength; }
            set { mStrRecordLength = value; OnPropertyChanged("StrRecordLength"); }
        }

        public ChanState Info { get; set; }
        public MonitorObject MonitorObject { get; set; }

        public static VoiceChanStateItem CreateItem(ChanState info)
        {
            VoiceChanStateItem item=new VoiceChanStateItem();
            item.ChanObjID = info.ObjID;
            item.ChanID = info.ChanID;
            item.VoiceID = info.ServerID;
            item.Extension = info.Extension;
            item.AgentID = info.AgentID;
            item.LoginState = info.LoginState;
            item.CallState = info.CallState;
            item.RecordState = info.RecordState;
            item.RecordReference = info.RecordReference;
            item.DirectionFlag = info.DirectionFlag;
            item.CallerID = info.CallerID;
            item.CalledID = info.CalledID;
            item.StartRecordTime = info.StartRecordTime;
            item.StopRecordTime = info.StopRecordTime;
            item.RecordLength = info.RecordLength;

            item.StrLoginState = item.LoginState.ToString();
            item.StrCallState = item.CallState.ToString();
            item.StrRecordState = item.RecordState.ToString();
            item.StrDirection = item.DirectionFlag.ToString();
            item.StrStartRecordTime = item.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            item.StrStopRecordTime = item.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            item.StrRecordLength = item.RecordLength.ToString();

            item.Info = info;

            return item;
        }

        public void UpdateInfo(ChanState info)
        {
            Extension = info.Extension;
            AgentID = info.AgentID;
            LoginState = info.LoginState;
            CallState = info.CallState;
            RecordState = info.RecordState;
            RecordReference = info.RecordReference;
            DirectionFlag = info.DirectionFlag;
            CallerID = info.CallerID;
            CalledID = info.CalledID;
            StartRecordTime = info.StartRecordTime;
            StopRecordTime = info.StopRecordTime;
            RecordLength = info.RecordLength;

            StrLoginState = LoginState.ToString();
            StrCallState = CallState.ToString();
            StrRecordState = RecordState.ToString();
            StrDirection = DirectionFlag.ToString();
            StrStartRecordTime = StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            StrStopRecordTime = StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            StrRecordLength = RecordLength.ToString();

            Info = info;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
