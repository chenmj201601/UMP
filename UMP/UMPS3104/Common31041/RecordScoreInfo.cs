using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class RecordScoreInfo
    {
        public int RowNumber { get; set; }
        public long ScoreID { get; set; }
        public DateTime ScoreTime { get; set; }
        public double Score { get; set; }
        public string IsFinal { get; set; }
        public long ScorePerson { get; set; }
        public long TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string AppealMark { get; set; }
        /// <summary>
        /// 評分類型
        /// </summary>
        public string ScoreType { set; get; }
    }
}
