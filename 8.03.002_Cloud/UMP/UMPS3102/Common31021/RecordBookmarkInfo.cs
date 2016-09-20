//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4cd018bd-331f-4bf7-8731-51e23bdc0593
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordBookmarkInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                RecordBookmarkInfo
//
//        created by Charley at 2014/12/10 10:14:58
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    public class RecordBookmarkInfo
    {
        public long SerialID { get; set; }
        public long RecordRowID { get; set; }
        public long RecordID { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public long MarkerID { get; set; }
        public DateTime MarkTime { get; set; }
        public long ModifyerID { get; set; }
        public DateTime ModifyerTime { get; set; }
        public long DeleterID { get; set; }
        public DateTime DeleteTime { get; set; }
        public long RankID { get; set; }
        public string Teminal { get; set; }
        /// <summary>
        /// 是否有音频类型的标签(1为有，0为没有)
        /// </summary>
        public string IsHaveBookMarkRecord { get; set; }
        /// <summary>
        /// 音频类标签的时长(-1表示没有音频)
        /// </summary>
        public string BookmarkTimesLength { get; set; }
    }
}
