//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3c567b12-57c0-4e79-8994-58b41211715c
//        CLR Version:              4.0.30319.18444
//        Name:                     ViewStyle
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                ViewStyle
//
//        created by Charley at 2014/6/9 11:47:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class ViewStyle
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public int TemplateItem { get; set; }
        public long TemplateItemID { get; set; }
        public long CommentID { get; set; }
        public int FontSize { get; set; }
        public string FontWeight { get; set; }
        public string FontFamily { get; set; }
        public string ForeColor { get; set; }
        public string BackColor { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
