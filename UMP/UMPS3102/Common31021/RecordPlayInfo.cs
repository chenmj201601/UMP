//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1ac4e1c3-8647-4319-82d9-02b9e068fd93
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordPlayInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                RecordPlayInfo
//
//        created by Charley at 2014/11/10 17:26:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 播放历史信息
    /// </summary>
    public class RecordPlayInfo
    {
        public long SerialID { get; set; }
        public long RecordID { get; set; }
        public string RecordReference { get; set; }
        public double StartPosition { get; set; }
        public double StopPosition { get; set; }
        public double Duration { get; set; }
        public int PlayTimes { get; set; }
        public long Player { get; set; }
        public DateTime PlayTime { get; set; }
        /// <summary>
        /// 1       UMP QM
        /// 2       CQC
        /// </summary>
        public int PlayTerminal { get; set; }
    }
}
