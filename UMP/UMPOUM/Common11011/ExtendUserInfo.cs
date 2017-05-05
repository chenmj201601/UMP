//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    513384ea-8821-4b75-864a-8cd4eb903233
//        CLR Version:              4.0.30319.18444
//        Name:                     ExtendUserInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                ExtendUserInfo
//
//        created by Charley at 2014/9/3 16:09:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common11011
{
    /// <summary>
    /// 扩展用户信息
    /// </summary>
    public class ExtendUserInfo
    {
        public long UserID { get; set; }
        public string MailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Birthday { get; set; }
        public string BirthCity { get; set; }
        public string HeadIcon { get; set; }
    }
}
