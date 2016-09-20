//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    67d87f4d-5c8e-438f-ba10-73266441bba7
//        CLR Version:              4.0.30319.18444
//        Name:                     OperationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                OperationInfo
//
//        created by Charley at 2014/8/29 15:24:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11011
{
    public class OperationInfo
    {
        public long ID { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public long ParentID { get; set; }
    }
}
