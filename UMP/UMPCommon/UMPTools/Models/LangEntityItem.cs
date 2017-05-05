//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5a4bcda8-3c77-47c7-bd5c-a45a04a570ae
//        CLR Version:              4.0.30319.18063
//        Name:                     LangEntityItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                LangEntityItem
//
//        created by Charley at 2015/8/2 13:30:49
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPTools.Models
{
    public class LangEntityItem
    {
        public int Entity { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }

        public const int UNKOWN = 0;
        public const int BASICINFODATA = 1;
        public const int BASICINFODATA_DESC = 2;
        public const int FEATURE_OPERATION = 11;
        public const int VIEW_COLUMN = 12;
        public const int RESOURCE_OBJECT = 21;
        public const int RESOURCE_OBJECT_DESC = 22;
        public const int RESOURCE_PROPERTY = 23;
        public const int RESOURCE_PROPERTY_DESC = 24;
    }
}
