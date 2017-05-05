//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f324616b-51a1-4c8d-aad6-137236b33f1d
//        CLR Version:              4.0.30319.18408
//        Name:                     ModuleInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdateTool.Models
//        File Name:                ModuleInfo
//
//        created by Charley at 2016/6/1 14:41:30
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPUpdateTool.Models
{
    [XmlRoot]
    public class ModuleInfo
    {
        [XmlAttribute]
        public int ModuleID { get; set; }
        [XmlAttribute]
        public int MasterID { get; set; }
        [XmlAttribute]
        public string ModuleName { get; set; }
        [XmlAttribute]
        public string Display { get; set; }
    }
}
