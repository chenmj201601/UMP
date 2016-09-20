using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    public class BasicScoreSheetInfo
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public string Title { get; set; }
        public double TotalScore { get; set; }
        public double Score { get; set; }
        public long OrgID { get; set; }
        /// <summary>
        /// 修改成绩前的评分成绩ID，新评分的此字段为0
        /// </summary>
        public long OldScoreResultID { get; set; }
    }
}
