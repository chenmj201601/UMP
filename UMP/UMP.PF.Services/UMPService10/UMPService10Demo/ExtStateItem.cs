//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d97ddf85-690c-4e03-bf72-2d27a2d637e3
//        CLR Version:              4.0.30319.18408
//        Name:                     ExtStateItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10Demo
//        File Name:                ExtStateItem
//
//        created by Charley at 2016/6/29 10:13:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using VoiceCyber.UMP.CommonService10;


namespace UMPService10Demo
{
    public class ExtStateItem : INotifyPropertyChanged
    {

        private string mExtension;

        public string Extension
        {
            get { return mExtension; }
            set { mExtension = value; OnPropertyChanged("Extension"); }
        }

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

        private string mStrDirectionState;

        public string StrDirectionState
        {
            get { return mStrDirectionState; }
            set { mStrDirectionState = value; OnPropertyChanged("StrDirectionState"); }
        }

        private string mStrAgentState;

        public string StrAgentState
        {
            get { return mStrAgentState; }
            set { mStrAgentState = value; OnPropertyChanged("StrAgentState"); }
        }

        private string mAgentID;

        public string AgentID
        {
            get { return mAgentID; }
            set { mAgentID = value; OnPropertyChanged("AgentID"); }
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

        public string MonID { get; set; }


        public ExtensionInfo ExtensionInfo { get; set; }

        public MonitorObject MonObject { get; set; }


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
