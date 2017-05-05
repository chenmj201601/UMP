//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3d7634dd-7689-43dc-b687-04250a5f086d
//        CLR Version:              4.0.30319.18408
//        Name:                     SeatInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                SeatInfo
//
//        created by Charley at 2016/6/7 14:48:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common44101
{
    public class SeatInfo
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int State { get; set; }
        public int Level { get; set; }
        public string Extension { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}", ObjID, Name, Extension);
        }
    }
}
