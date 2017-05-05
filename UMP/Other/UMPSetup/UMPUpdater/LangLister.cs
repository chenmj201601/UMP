//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a549ebcc-a3ad-4d0e-9f4d-06341f3678d5
//        CLR Version:              4.0.30319.18408
//        Name:                     LangLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                LangLister
//
//        created by Charley at 2016/5/27 09:55:06
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace UMPUpdater
{
    /// <summary>
    /// 语言列表
    /// </summary>
    [XmlRoot]
    public class LangLister
    {
        /// <summary>
        /// 语言编码
        /// </summary>
        [XmlAttribute]
        public int LangID { get; set; }
        /// <summary>
        /// 语言名称
        /// </summary>
        [XmlAttribute]
        public string LangName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }

        private List<LanguageInfo> mListLangInfos = new List<LanguageInfo>();
        /// <summary>
        /// 语言列表
        /// </summary>
        [XmlArray(ElementName = "Langs")]
        [XmlArrayItem(ElementName = "Lang")]
        public List<LanguageInfo> ListLangInfos
        {
            get { return mListLangInfos; }
        }
    }
}
