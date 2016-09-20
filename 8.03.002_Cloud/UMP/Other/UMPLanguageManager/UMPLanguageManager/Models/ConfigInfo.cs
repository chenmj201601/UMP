//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b5d8ba6b-c255-4834-bf82-8ae51a3ce9f6
//        CLR Version:              4.0.30319.18063
//        Name:                     ConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                ConfigInfo
//
//        created by Charley at 2015/4/25 16:21:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;

namespace UMPLanguageManager.Models
{
    [XmlRoot]
    public class LangConfigInfo
    {
        [XmlArray]
        public List<FilterInfo> ListFilterInfos { get; set; }

        public DatabaseInfo DataBaseInfo { get; set; }
        public DatabaseInfo SyncDBInfo { get; set; }

        public LangConfigInfo()
        {
            ListFilterInfos = new List<FilterInfo>();
        }
    }
}
