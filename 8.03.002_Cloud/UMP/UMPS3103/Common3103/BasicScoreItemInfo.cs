using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    /// <summary>
    /// 评分子项信息
    /// </summary>
    public class BasicScoreItemInfo
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long ScoreItemID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public double Score { get; set; }
        public double RealScore { get; set; }
        /// <summary>
        /// Y 是NA，N 非NA
        /// </summary>
        public string IsNA { get; set; }
    }
}
