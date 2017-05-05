//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    253334bc-7ace-41a8-93f8-b578a1d7965a
//        CLR Version:              4.0.30319.18408
//        Name:                     AlarmMessageInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                AlarmMessageInfo
//
//        created by Charley at 2016/7/12 16:31:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common44101
{
    public class AlarmMessageInfo
    {
        public long SerialID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public int Rank { get; set; }
        public string Value { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public int HoldTime { get; set; }
        public long StateID { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
