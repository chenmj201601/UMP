//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    33d5d2e1-75a4-4857-9390-e91591472583
//        CLR Version:              4.0.30319.18444
//        Name:                     BookmarkLineInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                BookmarkLineInfo
//
//        created by Charley at 2014/12/10 15:13:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS3102.Models
{
    public class BookmarkLineInfo
    {
        public int Begin { get; set; }
        public int Duration { get; set; }
        public int TotalDuration { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double TotalWidth { get; set; }
    }
}
