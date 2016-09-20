//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    891855b5-ed7d-48c2-b7d8-7631a0007cd9
//        CLR Version:              4.0.30319.18408
//        Name:                     LicModel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LicenseMonitor
//        File Name:                LicModel
//
//        created by Charley at 2016/4/26 11:10:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace LicenseMonitor
{
    public class LicModel
    {
        public int MasterID { get; set; }
        public long OptID { get; set; }
        public long ParentID { get; set; }
        public int SortID { get; set; }
        public int LicNo { get; set; }
        public long LicID { get; set; }
        public string OptName { get; set; }
    }
}
