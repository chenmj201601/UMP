//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fa0b81ba-2485-486d-a4bd-d7bdb6777080
//        CLR Version:              4.0.30319.18408
//        Name:                     InstallProduct
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                InstallProduct
//
//        created by Charley at 2016/6/8 16:45:03
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace VoiceCyber.UMP.Updates
{
    [XmlRoot(ElementName = "InstallProduct", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class InstallProduct
    {
        [XmlAttribute]
        public string Package { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        [XmlAttribute]
        public string InstallPath { get; set; }

        [XmlAttribute]
        public string ProductGuid { get; set; }

        [XmlAttribute]
        public string ProductName { get; set; }
        [XmlAttribute]
        public string DisplayName { get; set; }
        

        private List<InstallComponent> mListComponents = new List<InstallComponent>();

        [XmlArray(ElementName = "Components")]
        [XmlArrayItem(ElementName = "Component")]
        public List<InstallComponent> ListComponents
        {
            get { return mListComponents; }
        }
    }
}
