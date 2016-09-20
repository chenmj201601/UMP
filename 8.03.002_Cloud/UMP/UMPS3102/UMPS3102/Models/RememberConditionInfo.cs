//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8932d214-03d6-48a6-b5ed-1abf89cb94b8
//        CLR Version:              4.0.30319.18444
//        Name:                     RememberConditionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RememberConditionInfo
//
//        created by Charley at 2014/11/9 13:57:24
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    [XmlRoot]
    public class RememberConditionInfo
    {
        [XmlAttribute]
        public bool IsRemember { get; set; }
        [XmlArray]
        public List<QueryConditionDetail> ListConditionDetail { get; set; }
    }
}
