//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c1746920-f71b-4d58-ab1d-44b4242dc721
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmMessageInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common25011
//        File Name:                AlarmMessageInfo
//
//        created by Charley at 2015/5/20 14:52:23
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common25011
{
    /// <summary>
    /// 告警消息信息
    /// </summary>
    public class AlarmMessageInfo
    {
        public long SerialID { get; set; }
        public int AlarmType { get; set; }
        public int ModuleID { get; set; }
        public int MessageID { get; set; }
        public int StatusID { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
    }
}
