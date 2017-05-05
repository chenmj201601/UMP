//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    60dadaba-d50f-4abf-b5c7-402750b0dbd6
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmInfomationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common25011
//        File Name:                AlarmInfomationInfo
//
//        created by Charley at 2015/5/20 14:55:17
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.Common25011
{
    /// <summary>
    /// 告警信息详情
    /// </summary>
    public class AlarmInfomationInfo
    {
        public long SerialID { get; set; }
        public long MessageID { get; set; }
        public int Level { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public long Creator { get; set; }
        public DateTime LastModifyTime { get; set; }
        public long LastModifyUser { get; set; }
    }
}
