//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    474ec760-24bd-4760-9099-6f8eb846fb85
//        CLR Version:              4.0.30319.18444
//        Name:                     BasicScoreItemInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                BasicScoreItemInfo
//
//        created by Charley at 2014/11/17 10:38:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 评分子项信息
    /// </summary>
    public class BasicScoreItemInfo
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long ScoreItemID { get; set; }
        public double Score { get; set; }
        public double RealScore { get; set; }
        /// <summary>
        /// Y 是NA，N 非NA
        /// </summary>
        public string IsNA { get; set; }

      
    }
}
