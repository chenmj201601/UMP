//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    58da5de6-5fd0-4e9e-abab-de30c3c7e844
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionSeatInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                RegionSeatInfo
//
//        created by Charley at 2016/6/13 16:57:19
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common44101
{
    public class RegionSeatInfo
    {
        public long RegionID { get; set; }
        public long SeatID { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", RegionID, SeatID, Left, Top);
        }
    }
}
