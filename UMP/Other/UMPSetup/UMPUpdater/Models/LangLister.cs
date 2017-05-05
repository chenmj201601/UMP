//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    30455bbe-0a2d-494c-b108-d3afeee24e39
//        CLR Version:              4.0.30319.18408
//        Name:                     LangLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                LangLister
//
//        created by Charley at 2016/8/4 09:37:33
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace UMPUpdater.Models
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
