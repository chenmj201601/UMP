//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d812cd33-988d-45b3-990b-6f5fb118de54
//        CLR Version:              4.0.30319.18408
//        Name:                     RecordInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                RecordInfoItem
//
//        created by Charley at 2016/7/7 13:33:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;


namespace UMPS4411.Models
{
    public class RecordInfoItem:INotifyPropertyChanged
    {
        private string mSerialNo;

        public string SerialNo
        {
            get { return mSerialNo; }
            set { mSerialNo = value; OnPropertyChanged("SerialNo"); }
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

        private string mDirection;

        public string Direction
        {
            get { return mDirection; }
            set { mDirection = value; OnPropertyChanged("Direction"); }
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

        private string mRecordLength;

        public string RecordLength
        {
            get { return mRecordLength; }
            set { mRecordLength = value; OnPropertyChanged("RecordLength"); }
        }

        public UMPRecordInfo Info { get; set; }


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
