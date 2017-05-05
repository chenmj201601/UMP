//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    406ed4f8-0130-42b5-b67a-0e8e6e80ad65
//        CLR Version:              4.0.30319.42000
//        Name:                     UMPServerInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPClient.Models
//        File Name:                UMPServerInfo
//
//        Created by Charley at 2016/8/23 14:26:48
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace UMPClient.Models
{
    [XmlRoot(ElementName = "UMPServer")]
    public class UMPServerInfo
    {
        [XmlAttribute]
        public string Address { get; set; }
        [XmlAttribute]
        public int Port { get; set; }
        [XmlAttribute]
        public bool IsDefault { get; set; }
    }
}
