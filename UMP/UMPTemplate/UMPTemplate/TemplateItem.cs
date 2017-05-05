//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0f1cd95c-5dd1-4e44-bc51-5190886f4fb5
//        CLR Version:              4.0.30319.18444
//        Name:                     TemplateItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Templates
//        File Name:                TemplateItem
//
//        created by Charley at 2014/6/9 11:45:20
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Templates
{
    public class TemplateItem
    {
        public long ID { get; set; }
        public int TemplateID { get; set; }
        public long ParentID { get; set; }
        public int OrderID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalScore { get; set; }
        public bool IsStandard { get; set; }
        public bool IsControl { get; set; }
        public bool IsKeyItem { get; set; }
        public bool AllowNA { get; set; }
        public bool IsJump { get; set; }
        public ScoreItemStyle ItemStyle { get; set; }
        public double PointSystem { get; set; }
        public bool IsSectionBased { get; set; }
        public bool IsAddtion { get; set; }
        public bool IsStartPointSystem { get; set; }
        public string ScoreType { get; set; }
        public string SimpleOrAdvanceControl { get; set; }
        public ViewStyle ViewStyle { get; set; }
        public ViewStyle TitleStyle { get; set; }

        public List<TemplateItem> Items { get; set; }
        public List<Comment> Comments { get; set; }

        public TemplateItem()
        {
            Items = new List<TemplateItem>();
            Comments = new List<Comment>();
        }
    }
}
