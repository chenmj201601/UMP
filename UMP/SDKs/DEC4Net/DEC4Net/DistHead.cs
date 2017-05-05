//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e51af1e4-6ff2-4748-9094-cc606572ef22
//        CLR Version:              4.0.30319.18063
//        Name:                     DistHead
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                DistHead
//
//        created by Charley at 2015/6/19 10:35:51
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.SDKs.DEC
{
    public class DistHead
    {
        public int PackType { get; set; }
        public int FollowSize { get; set; }
        public ulong SourceID { get; set; }
        public ulong TargetID { get; set; }
        public DateTime Time { get; set; }
        public int BaseType { get; set; }
        public int BaseSize { get; set; }
        public int ExtType { get; set; }
        public int ExtSize { get; set; }
        public int DataSize { get; set; }
        public int ModuleID { get; set; }
        public int Number { get; set; }
        public MessageString Message { get; set; }

        internal static DistHead FromHead(_TAG_NETPACK_DISTINGUISHHEAD_VER1 ver1Head)
        {
            DistHead info = new DistHead();
            info.PackType = ver1Head._packtype;
            info.FollowSize = ver1Head._followsize;
            info.SourceID = ver1Head._source;
            info.TargetID = ver1Head._target;
            info.Time = Helpers.GetTimeFromTimestamp((long)ver1Head._timestamp);
            info.BaseType = ver1Head._basehead;
            info.BaseSize = ver1Head._basesize;
            info.ExtType = ver1Head._exthead;
            info.ExtSize = ver1Head._extsize;
            info.DataSize = ver1Head._datasize;
            info.ModuleID = ver1Head._moduleid;
            info.Number = ver1Head._number;
            info.Message = MessageString.FromMessage(ver1Head._message);
            return info;
        }
    }
}
