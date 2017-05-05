//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1b4cebda-4213-4a08-b914-94492f443674
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmReceiverInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common25011
//        File Name:                AlarmReceiverInfo
//
//        created by Charley at 2015/5/20 14:57:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common25011
{
    public class AlarmReceiverInfo
    {
        public long UserID { get; set; }
        public long AlarmInfoID { get; set; }
        public long TenantID { get; set; }
        public string TenantToken { get; set; }
        public int Method { get; set; }
        public int ReplyMode { get; set; }
    }
}
