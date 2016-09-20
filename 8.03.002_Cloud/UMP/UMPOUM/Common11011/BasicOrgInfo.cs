//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4ae9169c-f28e-4bf3-a417-f5d9536ec68c
//        CLR Version:              4.0.30319.18444
//        Name:                     OrganizationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                OrganizationInfo
//
//        created by Charley at 2014/8/29 15:25:21
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.Common11011
{
    public class BasicOrgInfo
    {
        public long OrgID { get; set; }
        public string OrgName { get; set; }
        public int OrgType { get; set; }
        public long ParentID { get; set; }
        public string IsActived { get; set; }
        public string IsDeleted { get; set; }
        public string State { get; set; }
        public string StrStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public string StrEndTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
}
