using System;
using System.Windows.Media;
using VoiceCyber.UMP.Common31031;
using System.ComponentModel;
using System.Collections.Generic;

namespace UMPS3103.Models
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
        /// <summary>
        /// 座席或者分机
        /// </summary>
        public string ReAgent { get; set; }
        /// <summary>
        /// 真实座席
        /// </summary>
        public string Agent { get; set; }
        public string ReaExtension { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string DirShow { get; set; }
        public string CallerID { get; set; }
        public string DecCallerID { get; set; }
        public string CalledID { get; set; }
        public string DecCalledID { get; set; }
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

        //ABCD
        public int ServiceAttitude { get; set; }
        public string StrServiceAttitude { get; set; }
        public int ProfessionalLevel { get; set; }
        public string StrProfessionalLevel { get; set; }
        public int RecordDurationError { get; set; }
        public string StrRecordDurationError { get; set; }
        public int RepeatCallIn { get; set; }
        public string StrRepeatCallIn { get; set; }

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


        public RecordInfoItem(RecordInfo recordInfo):this()
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
            if (S3103App.GroupingWay.Contains("A"))
            {
                ReAgent = recordInfo.Agent;
            }
            else if (S3103App.GroupingWay.Contains("R"))
            {
                ReAgent = recordInfo.ReaExtension;
            }
            else if (S3103App.GroupingWay.Contains("E"))
            {
                ReAgent = recordInfo.Extension;
            }
            ReaExtension = recordInfo.ReaExtension;
            Duration = recordInfo.Duration;
            Direction = recordInfo.Direction;
            CallerID = recordInfo.CallerID;
            DecCallerID = S3103App.DecryptString(recordInfo.CallerID);
            CalledID = recordInfo.CalledID;
            DecCalledID = S3103App.DecryptString(recordInfo.CalledID);
            IsScore = recordInfo.IsScore;
            TaskUserID = recordInfo.TaskUserID;
            TaskUserName = recordInfo.TaskUserName;
            RecordInfo = recordInfo;
            DirShow = "";
            AgtOrExtID = "";
            AgtOrExtName = "";
            StrDuration = Duration == 0 ? "00:00:00" : string.Format("{0:00}:{1:00}:{2:00}", Duration / 3600, (Duration % 3600) / 60, Duration % 60);
            EncryptFlag = recordInfo.EncryptFlag; 
            MediaType = recordInfo.MediaType;

            ServiceAttitude = recordInfo.ServiceAttitude;
            ProfessionalLevel = recordInfo.ProfessionalLevel;
            RecordDurationError = recordInfo.RecordDurationError;
            RepeatCallIn = recordInfo.RepeatCallIn;
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
