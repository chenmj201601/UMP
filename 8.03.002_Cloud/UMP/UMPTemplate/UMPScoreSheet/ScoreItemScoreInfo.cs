//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f4eb95b1-4459-43a0-88bc-ed0917f90a7b
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItemScoreInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreItemScoreInfo
//
//        created by Charley at 2014/8/8 15:01:25
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    public class ScoreItemScoreInfo
    {
        public ScoreItem ScoreItem { get; set; }
        public long ScoreItemID { get; set; }
        public string Scorer { get; set; }
        public double Score { get; set; }
    }
}
