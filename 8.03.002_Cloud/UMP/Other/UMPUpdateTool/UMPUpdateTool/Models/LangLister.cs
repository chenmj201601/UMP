//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    0f7c3e94-b047-4d5e-bd13-6f5ecb774b8e
//        CLR Version:              4.0.30319.42000
//        Name:                     LangLister
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdateTool.Models
//        File Name:                LangLister
//
//        Created by Charley at 2016/9/1 13:27:57
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace UMPUpdateTool.Models
{
    [XmlRoot]
    public class LangLister
    {
        [XmlAttribute]
        public int LangID { get; set; }
        [XmlAttribute]
        public string LangName { get; set; }
        [XmlAttribute]
        public string Path { get; set; }

        private List<LanguageInfo> mListLangInfos = new List<LanguageInfo>();

        [XmlArray(ElementName = "Langs")]
        [XmlArrayItem(ElementName = "Lang")]
        public List<LanguageInfo> ListLangInfos
        {
            get { return mListLangInfos; }
        } 
    }
}
