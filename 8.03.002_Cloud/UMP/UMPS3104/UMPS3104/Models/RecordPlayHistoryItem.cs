using System.ComponentModel;
using System;
using System.Windows.Media;
using VoiceCyber.UMP.Common31041;
namespace UMPS3104.Models
{
    public class RecordPlayHistoryItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long PlayHistoryID { get; set; }
        public long RecordReference { get; set; }
        public long longUserID { get; set; }
        public string UserID { get; set; }
        private DateTime mPlayDate;

        public DateTime PlayDate
        {
            get { return mPlayDate; }
            set
            {
                mPlayDate = value;
                OnProeprtyChanged("PlayDate");
            }
        }

        private double mPlayDuration;
        public double PlayDuration
        {
            get { return mPlayDuration; }
            set
            {
                mPlayDuration = value;
                OnProeprtyChanged("PlayDuration");
            }
        }
        public string PlayDurationStr { get; set; }
        public int intType { get; set; }
        public string Type { get; set; }
        private int mPlayTimes;
        public int PlayTimes
        {
            get { return mPlayTimes; }
            set
            {
                mPlayTimes = value;
                OnProeprtyChanged("PlayTimes");
            }
        }

        private double mStartPosition;
        public double StartPosition
        {
            get { return mStartPosition; }
            set
            {
                mStartPosition = value;
                OnProeprtyChanged("StartPosition");
            }
        }
        public string StartPositionStr { get; set; }

        private double mStopPosition;
        public double StopPosition
        {
            get { return mStopPosition; }
            set
            {
                mStopPosition = value;
                OnProeprtyChanged("StopPosition");
            }
        }
        public string StopPositionStr { get; set; }

        public Brush Background { get; set; }

        public RecordPlayHistoryItem(RecordPlayHistoryInfo recordPlayHistoryInfo)
        {
            RowNumber = recordPlayHistoryInfo.RowNumber;
            PlayHistoryID = recordPlayHistoryInfo.PlayHistoryID;
            RecordReference = recordPlayHistoryInfo.RecordReference;
            UserID = recordPlayHistoryInfo.UserID;
            longUserID = recordPlayHistoryInfo.longUserID;
            PlayDate = recordPlayHistoryInfo.PlayDate;
            PlayDuration = recordPlayHistoryInfo.PlayDuration;
            intType = recordPlayHistoryInfo.intType;
            Type = recordPlayHistoryInfo.Type;
            PlayTimes = recordPlayHistoryInfo.PlayTimes;
            StartPosition = recordPlayHistoryInfo.StartPosition;
            StopPosition = recordPlayHistoryInfo.StopPosition;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProeprtyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
