//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    55c52371-0b34-469a-be38-f278485aa30e
//        CLR Version:              4.0.30319.42000
//        Name:                     LangLister
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPClient.Models
//        File Name:                LangLister
//
//        Created by Charley at 2016/8/23 13:21:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace UMPClient.Models
{
    [XmlRoot]
    public class LangLister
    {
        [XmlAttribute]
        public int LangID { get; set; }
        [XmlAttribute]
        public string LangName { get; set; }

        private List<LanguageInfo> mListLanguageInfos = new List<LanguageInfo>();
        [XmlArray(ElementName = "Langs")]
        [XmlArrayItem(ElementName = "Lang")]
        public List<LanguageInfo> ListLanguageInfos
        {
            get { return mListLanguageInfos; }
        }

    }
}
