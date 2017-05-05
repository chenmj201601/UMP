//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    c1088670-5b80-473f-9230-05d8a2dfce13
//        CLR Version:              4.0.30319.42000
//        Name:                     RecoverChannelConfig
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                RecoverChannelConfig
//
//        Created by Charley at 2016/10/19 16:25:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace UMPS2106.Models
{
    [XmlRoot]
    public class RecoverChannelConfig
    {
       
        [XmlAttribute]
        public int Number { get; set; }
       
        [XmlAttribute]
        public int VoiceID { get; set; }
     
        [XmlAttribute]
        public int ChannelID { get; set; }
       
        [XmlAttribute]
        public string Extension { get; set; }
     
        [XmlAttribute]
        public string VoiceIP { get; set; }
    }
}
