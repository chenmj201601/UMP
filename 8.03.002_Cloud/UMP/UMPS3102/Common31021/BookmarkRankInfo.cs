//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e9938bce-4d7c-40ef-92eb-8f122901b3a3
//        CLR Version:              4.0.30319.18444
//        Name:                     BookmarkRankInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                BookmarkRankInfo
//
//        created by Charley at 2014/12/11 11:56:28
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    public class BookmarkRankInfo
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int OrderID { get; set; }
        public string Color { get; set; }
        /// <summary>
        /// BM  Bookmark的等级
        /// KW  关键词的等级
        /// </summary>
        public string RankType { get; set; }
    }
}
