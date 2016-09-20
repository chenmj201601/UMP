//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    255f9048-a344-4768-90c8-144a231b9ffd
//        CLR Version:              4.0.30319.18444
//        Name:                     Template
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                Template
//
//        created by Charley at 2014/6/9 11:43:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class Template
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TemplateType Type { get; set; }
        public double TotalScore { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Status { get; set; }
        public DateTime DateForm { get; set; }
        public DateTime DateEnd { get; set; }
        public long UseTag { get; set; }
        public double QualifiedLine { get; set; }
        public ScoreType ScoreType { get; set; }
        public long OrgTenantID { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }
        public ViewStyle ViewStyle { get; set; }
        public ViewStyle TitleStyle { get; set; }

        public List<TemplateItem> Items { get; set; }
        public List<Comment> Comments { get; set; }

        public Template()
        {
            Items = new List<TemplateItem>();
            Comments = new List<Comment>();
        }
    }
}
