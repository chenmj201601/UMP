//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f472cc1a-a6cf-4c5e-90a9-9d56a4e4f4ba
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                RegionInfo
//
//        created by Charley at 2016/5/10 14:24:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common44101
{
    public class RegionInfo
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ParentObjID { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string BgColor { get; set; }
        public string BgImage { get; set; }
        public long CreateID { get; set; }
        public DateTime CreateTime { get; set; }
        public long ModifierID { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsDefault { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ObjID, Name, Type);
        }
    }
}
