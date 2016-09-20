//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    642fcd49-a025-4831-8dc8-550eccc47669
//        CLR Version:              4.0.30319.18444
//        Name:                     UserScoreInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                UserScoreInfo
//
//        created by Charley at 2014/8/12 10:15:25
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 用户成绩
    /// </summary>
    public class UserScoreInfo
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 打分项
        /// </summary>
        public ScoreItem ScoreItem { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public double Score { get; set; }
    }
}
