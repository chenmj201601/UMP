//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    375d0888-433e-4553-bbc4-03aa79458cda
//        CLR Version:              4.0.30319.18444
//        Name:                     Comment
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                Comment
//
//        created by Charley at 2014/6/9 11:46:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class Comment
    {
        public long ID { get; set; }
        public long TemplateItemID { get; set; }
        public int TemplateID { get; set; }
        public CommentStyle Style { get; set; }
        public string Title { get; set; }
        public bool IsTemplateOrStandard { get; set; }
        public int OrderID { get; set; }
        public ViewStyle ViewStyle { get; set; }
        public ViewStyle TitleStyle { get; set; }

        public List<CommentItem> Items { get; set; }

        public Comment()
        {
            Items = new List<CommentItem>();
        }
    }
}
