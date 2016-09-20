//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ddced9f0-5529-4b1a-b00a-f22efd29bd9e
//        CLR Version:              4.0.30319.18063
//        Name:                     LangFileInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                LangFileInfo
//
//        created by Charley at 2015/7/12 17:01:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;

namespace UMPLanguageManager.Models
{
    public class LangFileInfo
    {
        [XmlAttribute]
        public DateTime GenerateTime { get; set; }
        [XmlAttribute]
        public int TotalCount { get; set; }
        [XmlAttribute]
        public int LangType { get; set; }
        [XmlAttribute]
        public int ModuleID { get; set; }
        [XmlAttribute]
        public int SubModuleID { get; set; }
        [XmlAttribute]
        public string Prefix { get; set; }
        [XmlAttribute]
        public string Category { get; set; }

        private List<LanguageInfo> mListLangInfos;

        [XmlArray]
        public List<LanguageInfo> ListLangInfos
        {
            get { return mListLangInfos; }
        }

        public LangFileInfo()
        {
            GenerateTime = DateTime.Now;
            mListLangInfos = new List<LanguageInfo>();
        }
    }
}
