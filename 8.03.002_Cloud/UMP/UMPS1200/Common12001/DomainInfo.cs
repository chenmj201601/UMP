//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8eec95b9-e2dc-48bc-8642-c32f2be68282
//        CLR Version:              4.0.30319.18408
//        Name:                     DomainInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                DomainInfo
//
//        created by Charley at 2016/7/22 19:01:14
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12001
{
    public class DomainInfo
    {
        public long ObjID { get; set; }
        public string RentToken { get; set; }
        public string Name { get; set; }
        public int SortID { get; set; }
        public string AdminAccount { get; set; }
        public string RootPath { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public bool AllowAutoLogin { get; set; }
        public string Description { get; set; }
    }
}
