//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    362050dc-238b-465c-829d-f51240ff53c6
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreResult
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreResult
//
//        created by Charley at 2014/8/8 11:40:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.ScoreSheets
{
    public class ScoreResult
    {
        public ScoreSheet ScoreSheet { get; set; }
        public List<ScoreItemScoreInfo> ScoreInfos { get; set; } 
    }
}
