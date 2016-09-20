using System;
using System.ComponentModel;
using System.Windows.Media;
using Common3105;
using System.Collections.Generic;

namespace UMPS3105.Models
{
    public class RecordInfoItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public string Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }

        private string mStrDirection;

        public string StrDirection
        {
            get { return mStrDirection; }
            set
            {
                mStrDirection = value;
                OnProeprtyChanged("StrDirection");
            }
        }

        public DateTime StopRecordTime { get; set; }

        public Brush Background { get; set; }

        //2015-9-7 14:04:06 新增
        public int MediaType { get; set; }

        public string EncryptFlag { get; set; }

        public DateTime LocalStartRecordTime { get; set; }

        public DateTime LocalStopRecordTime { get; set; }

        private List<RecordInfo> mListRelativeInfos;

        public List<RecordInfo> ListRelativeInfos
        {
            get { return mListRelativeInfos; }
        }

        public RecordInfoItem()
        {
            mListRelativeInfos = new List<RecordInfo>();
        }

        public RecordInfo RecordInfo { get; set; }


        public RecordInfoItem(RecordInfo recordInfo): this()
        {
            RowID = recordInfo.RowID;
            SerialID = recordInfo.SerialID;
            RecordReference = recordInfo.RecordReference;
            StartRecordTime = recordInfo.StartRecordTime;
            VoiceID = recordInfo.VoiceID;
            ChannelID = recordInfo.ChannelID;
            VoiceIP = recordInfo.VoiceIP;
            Extension = recordInfo.Extension;
            Agent = recordInfo.Agent;
            Duration = recordInfo.Duration;
            Direction = recordInfo.Direction;
            CallerID = recordInfo.CallerID;
            CalledID = recordInfo.CalledID;

            RecordInfo = recordInfo;
            EncryptFlag = recordInfo.EncryptFlag;
            MediaType = recordInfo.MediaType;
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
