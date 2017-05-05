//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0de77fbb-e690-40aa-833b-be3669c92bb6
//        CLR Version:              4.0.30319.18408
//        Name:                     StateSettingItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                StateSettingItem
//
//        created by Charley at 2016/6/30 16:16:35
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace UMPS4411.Models
{
    [XmlRoot]
    public class StateSettingItem
    {
        [XmlAttribute]
        public int Type { get; set; }
        [XmlAttribute]
        public int Number { get; set; }
        public int Value { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
    }
}
