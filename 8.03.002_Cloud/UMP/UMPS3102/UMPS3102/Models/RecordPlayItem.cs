//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ae5cba8b-42c3-440b-b4e5-2ab5a5e599f4
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordPlayItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordPlayItem
//
//        created by Charley at 2014/11/10 17:25:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class RecordPlayItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long SerialID { get; set; }
        public long RecordID { get; set; }
        public string RecordReference { get; set; }
        public double StartPosition
        {
            get { return mStartPosition; }
            set { mStartPosition = value; OnPropertyChanged("StartPosition"); }
        }

        public double StopPosition
        {
            get { return mStopPosition; }
            set { mStopPosition = value; OnPropertyChanged("StopPosition"); }
        }

        public double Duration
        {
            get { return mDuration; }
            set { mDuration = value; OnPropertyChanged("Duration"); }
        }
        public int PlayTimes { get; set; }
        public long Player { get; set; }
        public DateTime PlayTime { get; set; }
        public int PlayTerminal { get; set; }

        private double mStartPosition;
        private double mStopPosition;
        private double mDuration;

        public Brush Background { get; set; }

        public RecordPlayInfo RecordPlayInfo { get; set; }
        public RecordInfoItem RecordInfoItem { get; set; }

        public RecordPlayItem()
        {

        }

        public RecordPlayItem(RecordPlayInfo recordPlayInfo)
        {
            SerialID = recordPlayInfo.SerialID;
            RecordID = recordPlayInfo.RecordID;
            RecordReference = recordPlayInfo.RecordReference;
            mStartPosition = recordPlayInfo.StartPosition;
            mStopPosition = recordPlayInfo.StopPosition;
            mDuration = recordPlayInfo.Duration;
            PlayTimes = recordPlayInfo.PlayTimes;
            Player = recordPlayInfo.Player;
            PlayTime = recordPlayInfo.PlayTime;
            PlayTerminal = recordPlayInfo.PlayTerminal;

            RecordPlayInfo = recordPlayInfo;
        }

        public void SetPlayInfo()
        {
            RecordPlayInfo.StartPosition = StartPosition;
            RecordPlayInfo.StopPosition = StopPosition;
            RecordPlayInfo.Duration = StopPosition - StartPosition;
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
