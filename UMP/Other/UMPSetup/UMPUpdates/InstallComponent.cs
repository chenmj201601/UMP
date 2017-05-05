//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    af081b4e-5ce6-40be-a4e7-d7044e1ff794
//        CLR Version:              4.0.30319.18408
//        Name:                     InstallComponent
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                InstallComponent
//
//        created by Charley at 2016/6/8 16:47:51
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace VoiceCyber.UMP.Updates
{
    [XmlRoot(ElementName = "InstallProduct", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class InstallComponent
    {
        [XmlAttribute]
        public int ModuleID { get; set; }
        [XmlAttribute]
        public string ModuleName { get; set; }
    }
}
