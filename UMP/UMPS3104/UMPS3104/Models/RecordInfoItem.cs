using System;
using System.ComponentModel;
using VoiceCyber.UMP.Common31041;
using System.Windows.Media;
using System.Collections.Generic;

namespace UMPS3104.Models
{
    public class RecordInfoItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 序號
        /// </summary>
        public int RowNumber { get; set; }
        /// <summary>
        /// 錄音編號\T_21_001.C001
        /// </summary>
        public long RowID { get; set; }
        /// <summary>
        /// 錄音流水號\T_21_001.C002
        /// </summary>
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public DateTime StopRecordTime { get; set; }

        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int Duration { get; set; }
        public string DurationStr { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public string Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }

        public long ScoreID { set; get; }

        private string mScore;
        public string Score
        {
            get { return mScore; }
            set
            {
                mScore = value;
                OnProeprtyChanged("Score");
            }
        }
        /// <summary>
        /// 1 已評分，0未評分
        /// </summary>
        public string IsScored { set; get; }

        public long TemplateID { set; get; }

        private string mAppealMark;
        /// <summary>
        /// 是否被申诉，n/0未申诉，y/1申诉中， c/2表申诉完成
        /// </summary>
        /// 
        public string AppealMark
        {
            get { return mAppealMark; }
            set
            {
                mAppealMark = value;
                OnProeprtyChanged("AppealMark");
            }
        }

        private string sAppealmark;

        public string sAppealMark
        {
            get { return sAppealmark; }
            set
            {
                sAppealmark = value;
                OnProeprtyChanged("sAppealMark");
            }
        }

        private string mBookMark;

        /// <summary>
        /// 书签状态。0删除，1正常
        /// </summary>
        public string BookMark
        {
            get { return mBookMark; }
            set
            {
                mBookMark = value;
                OnProeprtyChanged("BookMark");
            }
        }

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
        /// 媒体类型
        /// 0：录音（带录屏）
        /// 1: 录音
        /// 2：录屏
        /// </summary>
        public int MediaType { get; set; }

        /// <summary>
        /// 加密标识
        /// 0：无加密
        /// 2：二代加密
        /// E：待加密
        /// F: 加密失败
        /// </summary>
        public string EncryptFlag { get; set; }


        //ABCD
        public int ServiceAttitude { get; set; }
        public string StrServiceAttitude { get; set; }
        public int ProfessionalLevel { get; set; }
        public string StrProfessionalLevel { get; set; }
        public int RecordDurationError { get; set; }
        public string StrRecordDurationError { get; set; }
        public int RepeatCallIn { get; set; }
        public string StrRepeatCallIn { get; set; }

        public Brush Background { get; set; }

        public RecordInfo RecordInfo { get; set; }


        private List<RecordInfo> mListRelativeInfos;
        public List<RecordInfo> ListRelativeInfos
        {
            get { return mListRelativeInfos; }
        }
        public RecordInfoItem()
        {
            mListRelativeInfos = new List<RecordInfo>();
        }

        public RecordInfoItem(RecordInfo recordInfo): this()
        {
            RowID = recordInfo.RowID;
            SerialID = recordInfo.SerialID;
            RecordReference = recordInfo.RecordReference.ToString();
            StartRecordTime = recordInfo.StartRecordTime;
            StopRecordTime = recordInfo.StopRecordTime;
            VoiceID = recordInfo.VoiceID;
            ChannelID = recordInfo.ChannelID;
            VoiceIP = recordInfo.VoiceIP;
            Extension = recordInfo.Extension;
            Agent = recordInfo.Agent;
            Duration = recordInfo.Duration;
            Direction = recordInfo.Direction;
            CallerID = App.DecryptString(recordInfo.CallerID);
            CalledID =App.DecryptString( recordInfo.CalledID);

            AppealMark = recordInfo.AppealMark;
            sAppealMark = recordInfo.AppealMark;
            Score = recordInfo.Score;
            IsScored = string.IsNullOrWhiteSpace(recordInfo.Score) ? "0" : "1";
            ScoreID = recordInfo.ScoreID;
            TemplateID = recordInfo.TemplateID;
            BookMark = recordInfo.BookMark;

            EncryptFlag = recordInfo.EncryptFlag;
            MediaType = recordInfo.MediaType;

            ServiceAttitude = recordInfo.ServiceAttitude;
            if (ServiceAttitude != 0)
            {
                StrServiceAttitude = recordInfo.ServiceAttitude == 1 ? App.GetLanguageInfo("3104T00182", "Good") : App.GetLanguageInfo("3104T00170", "Bad");
            }
            ProfessionalLevel = recordInfo.ProfessionalLevel;
            if (ProfessionalLevel != 0)
            {
                StrProfessionalLevel = recordInfo.ProfessionalLevel == 1 ? App.GetLanguageInfo("3104T00182", "Good") : App.GetLanguageInfo("3104T00170", "Bad");
            }
            RecordDurationError = recordInfo.RecordDurationError;
            if (RecordDurationError != 0)
            {
                StrRecordDurationError = recordInfo.RecordDurationError == 2 ? App.GetLanguageInfo("3104T00172", "Normal") : App.GetLanguageInfo("3104T00173", "Abnormal");
            }
            RepeatCallIn = recordInfo.RepeatCallIn;
            if (RepeatCallIn != 0)
            {
                StrRepeatCallIn = recordInfo.RepeatCallIn == 2 ? App.GetLanguageInfo("3104T00172", "Normal") : App.GetLanguageInfo("3104T00181", "Repetition");
            }

            RecordInfo = recordInfo;
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
