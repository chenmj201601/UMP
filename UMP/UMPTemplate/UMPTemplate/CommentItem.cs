//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dd77f991-4424-410e-95c0-a36c0afbad42
//        CLR Version:              4.0.30319.18444
//        Name:                     CommentItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                CommentItem
//
//        created by Charley at 2014/6/9 11:46:33
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class CommentItem
    {
        public long ID { get; set; }
        public long CommentID { get; set; }
        public int TemplateID { get; set; }
        public long TemplateItemID { get; set; }
        public int ContentLength { get; set; }
        public bool IsChecked { get; set; }
        public int OrderID { get; set; }
    }
}
