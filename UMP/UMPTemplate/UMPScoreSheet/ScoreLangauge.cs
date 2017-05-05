//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d2858e90-f291-45b4-b45e-b57c8f47460f
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreLangauge
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreLangauge
//
//        created by Charley at 2014/7/9 15:53:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 语言信息
    /// </summary>
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ScoreLangauge
    {
        /// <summary>
        /// 语言类型
        /// </summary>
        [XmlAttribute]
        public int LangID { get; set; }
        /// <summary>
        /// 语言代码
        /// </summary>
        [XmlAttribute]
        public string Code { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [XmlAttribute]
        public string Category { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [XmlAttribute]
        public string Display { get; set; }
    }
}
