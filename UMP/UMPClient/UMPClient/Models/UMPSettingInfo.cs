//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    4a7a3366-2c2e-40b2-b437-e5c42591a5a0
//        CLR Version:              4.0.30319.42000
//        Name:                     UMPSettingInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPClient.Models
//        File Name:                UMPSettingInfo
//
//        Created by Charley at 2016/8/23 14:26:34
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPClient.Models
{
    [XmlRoot(ElementName = "UMPSetted")]
    public class UMPSettingInfo
    {
        public const string FILE_NAME = "UMP.Setted.xml";

        [XmlAttribute]
        public int LangID { get; set; }
        [XmlAttribute]
        public string StyleName { get; set; }
        [XmlAttribute]
        public int LaunchUMP { get; set; }
        [XmlAttribute]
        public int LaunchAgent { get; set; }

        private List<UMPServerInfo> mListServerInfos = new List<UMPServerInfo>();

        [XmlArray(ElementName = "UMPServers")]
        [XmlArrayItem(ElementName = "UMPServer")]
        public List<UMPServerInfo> ListServerInfos
        {
            get { return mListServerInfos; }
        }
    }
}
