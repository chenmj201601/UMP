using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using VoiceCyber.UMP.Common31031;

namespace UMPS3601.Models
{
    public class RecordInfoItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public string StartRecordTime { get; set; }
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
        public int Direction { get; set; }
        public string DirShow { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string IsScore { get; set; }

        private bool mIsCheck;
        public bool IsCheck
        {
            get { return mIsCheck; }
            set
            {
                mIsCheck = value;
                OnProeprtyChanged("IsCheck");
            }
        }


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

        /// <summary>
        /// 录音所属的座席id或者分机id 
        /// </summary>
        public string AgtOrExtID { get; set; }
        /// <summary>
        /// 录音对应的座席工号或者分机号 
        /// </summary>
        public string AgtOrExtName { get; set; }

        /// <summary>
        /// 录音时长 00:01:12
        /// </summary>
        public string StrDuration { get; set; }

        public string StopRecordTime { get; set; }

        public string TaskUserID { get; set; }

        public string TaskUserName { get; set; }

        public Brush Background { get; set; }

        //2015-9-2 11:22:54 新增
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


        public RecordInfoItem(RecordInfo recordInfo)
            : this()
        {
            RowID = recordInfo.RowID;
            SerialID = recordInfo.SerialID;
            RecordReference = recordInfo.RecordReference;
            StartRecordTime = recordInfo.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            StopRecordTime = recordInfo.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
            VoiceID = recordInfo.VoiceID;
            ChannelID = recordInfo.ChannelID;
            VoiceIP = recordInfo.VoiceIP;
            Extension = recordInfo.Extension;
            Agent = recordInfo.Agent;
            Duration = recordInfo.Duration;
            Direction = recordInfo.Direction;
            CallerID = recordInfo.CallerID;
            CalledID = recordInfo.CalledID;
            IsScore = recordInfo.IsScore;
            TaskUserID = recordInfo.TaskUserID;
            TaskUserName = recordInfo.TaskUserName;
            RecordInfo = recordInfo;
            DirShow = "";
            AgtOrExtID = "";
            AgtOrExtName = "";
            StrDuration = Duration == 0 ? "" : string.Format("{0:00}:{1:00}:{2:00}", Duration / 3600, (Duration % 3600) / 60, Duration % 60);
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
