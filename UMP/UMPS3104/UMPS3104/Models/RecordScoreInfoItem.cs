using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31041;

namespace UMPS3104.Models
{
    public class RecordScoreInfoItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long ScoreID { get; set; }
        public DateTime ScoreTime { get; set; }
        public double Score { get; set; }
        /// <summary>
        /// 語言包相關\最終分
        /// </summary>
        public string IsFinal { get; set; }
        /// <summary>
        /// 1\0
        /// </summary>
        public string Final { get; set; }
        public string ScorePerson { get; set; }
        public long strScorePerson { get; set; }
        public long TemplateID { get; set; }
        public string TemplateName { get; set; }
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

        /// <summary>
        /// 評分類型
        /// </summary>
        public string ScoreType { set; get; }
        public Brush Background { get; set; }
        public RecordScoreInfo ScoreInfo { get; set; }

        public RecordScoreInfoItem ()
        {
            
        }

        public RecordScoreInfoItem(RecordScoreInfo scoreInfo)
        {
            RowNumber = scoreInfo.RowNumber;
            ScoreID = scoreInfo.ScoreID;
            ScoreTime = scoreInfo.ScoreTime;
            Score = scoreInfo.Score;
            IsFinal = scoreInfo.IsFinal;
            Final = scoreInfo.IsFinal;
            strScorePerson = scoreInfo.ScorePerson;
            TemplateID = scoreInfo.TemplateID;
            TemplateName = scoreInfo.TemplateName;
            AppealMark = scoreInfo.AppealMark;
            sAppealMark = scoreInfo.AppealMark;
            ScoreType = scoreInfo.ScoreType;
            ScoreInfo = scoreInfo;
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
