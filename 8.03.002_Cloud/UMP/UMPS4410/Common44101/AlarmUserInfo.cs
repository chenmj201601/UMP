//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ec69c0f8-ca26-42e2-9a2c-29e94033f186
//        CLR Version:              4.0.30319.18408
//        Name:                     AlarmUserInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                AlarmUserInfo
//
//        created by Charley at 2016/7/12 16:32:04
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.Common44101
{
    public class AlarmUserInfo
    {
        public long AlarmID { get; set; }
        public long UserID { get; set; }
        public int UserType { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", AlarmID, UserID, UserType);
        }
    }
}
