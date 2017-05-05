//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    96e21f88-1eb9-4781-94a3-d52b67c0167d
//        CLR Version:              4.0.30319.42000
//        Name:                     RecoverStrategyConfig
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                RecoverStrategyConfig
//
//        Created by Charley at 2016/10/19 16:25:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPS2106.Models
{
    [XmlRoot]
    public class RecoverStrategyConfig
    {
        [XmlAttribute]
        public long SerialNo { get; set; }
       
        [XmlAttribute]
        public string Name { get; set; }
      
        [XmlAttribute]
        public string Description { get; set; }
       
        [XmlAttribute]
        public int State { get; set; }
       
        [XmlAttribute]
        public DateTime BeginTime { get; set; }
      
        [XmlAttribute]
        public DateTime EndTime { get; set; }
      
        [XmlAttribute]
        public string PackagePath { get; set; }
      
        [XmlAttribute]
        public int Flag { get; set; }
       
        [XmlAttribute]
        public int Times { get; set; }
     
        [XmlAttribute]
        public DateTime LastOptTime { get; set; }

        private List<RecoverChannelConfig> mListChannels = new List<RecoverChannelConfig>();

        [XmlArray(ElementName = "Channels")]
        [XmlArrayItem(ElementName = "Channel")]
        public List<RecoverChannelConfig> ListChannels
        {
            get { return mListChannels; }
        }
    }
}
