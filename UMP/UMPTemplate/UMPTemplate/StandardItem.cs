//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8d42ff3f-337a-47c8-ad0c-629ca340f0fb
//        CLR Version:              4.0.30319.18444
//        Name:                     StantardItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                StantardItem
//
//        created by Charley at 2014/6/9 11:45:51
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class StandardItem
    {
        public long ID { get; set; }
        public int TemplateID { get; set; }
        public long TemplateItemID { get; set; }
        public double Value { get; set; }
        public string Display { get; set; }
        public double DefaultValue { get; set; }
        public bool IsChecked { get; set; }
    }
}
