//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d81f98f5-0b1b-4d8a-a5d0-1d7e2ab40ff7
//        CLR Version:              4.0.30319.18063
//        Name:                     FilterItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                FilterItem
//
//        created by Charley at 2015/4/25 16:36:04
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;

namespace UMPLanguageManager.Models
{
    public class FilterItem : CheckableItemBase
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Other01 { get; set; }
        public string Other02 { get; set; }
        public string Other03 { get; set; }
        public string Other04 { get; set; }
        public string Other05 { get; set; }
        public int SortID { get; set; }
        public FilterInfo Info { get; set; }

        public static FilterItem CreateItem(FilterInfo info)
        {
            FilterItem item = new FilterItem();
            item.Type = info.Type;
            item.Name = info.Name;
            item.Value = info.Value;
            item.Other01 = info.Other01;
            item.Other02 = info.Other02;
            item.Other03 = info.Other03;
            item.Other04 = info.Other04;
            item.Other05 = info.Other05;
            item.SortID = info.SortID;
            item.Info = info;
            return item;
        }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, Value);
        }
    }
}
