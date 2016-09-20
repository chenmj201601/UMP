using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31041;

namespace UMPS3104.Models
{
    public class RecordPlayItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long SerialID { get; set; }
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
