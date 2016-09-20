//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c6479db-1002-452a-97f1-61fcccbb52b3
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreLanguageManager
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreLanguageManager
//
//        created by Charley at 2014/7/9 16:57:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ScoreLanguageManager
    {
        /// <summary>
        /// 语言列表
        /// </summary>
        [XmlArray]
        public List<ScoreLangauge> Languages { get; set; }
        /// <summary>
        /// ScoreLanguageManager
        /// </summary>
        public ScoreLanguageManager()
        {
            Languages = new List<ScoreLangauge>();
        }
    }
}
