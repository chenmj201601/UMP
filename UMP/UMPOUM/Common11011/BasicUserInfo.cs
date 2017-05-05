//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    39e51490-19f8-4d93-a242-effa46078c0d
//        CLR Version:              4.0.30319.18444
//        Name:                     BasicUserInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                BasicUserInfo
//
//        created by Charley at 2014/9/3 16:09:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common11011
{
    /// <summary>
    /// 基本用户信息
    /// </summary>
    public class BasicUserInfo
    {
        public long UserID { get; set; }
        public string Account { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public int EncryptFlag { get; set; }
        public long OrgID { get; set; }
        public string SourceFlag { get; set; }
        public string IsLocked { get; set; }
        public string LockMethod { get; set; }
        public string IsActived { get; set; }
        public string IsDeleted { get; set; }
        public string State { get; set; }
        public string StrStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public string StrEndTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string StrCreateTime { get; set; }
        public string IsOrgManagement { get; set; }
    }
}
