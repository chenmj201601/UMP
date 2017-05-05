//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c1fe3132-5b9e-4760-b442-ccd9174c52b2
//        CLR Version:              4.0.30319.18063
//        Name:                     XmlMappingItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                XmlMappingItem
//
//        created by Charley at 2015/9/2 15:50:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    [XmlRoot]
    public class XmlMappingItem
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Column { get; set; }
        [XmlAttribute]
        public int DataType { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        /// <summary>
        /// 编码方式
        /// 0：无
        /// 1：十六进制编码
        /// 2：Base64编码
        /// </summary>
        [XmlAttribute]
        public int Encoding { get; set; }
        /// <summary>
        /// 加密模式，详见VoiceCyber.Common.EncryptionMode枚举
        /// 0：无
        /// </summary>
        [XmlAttribute]
        public int Encryption { get; set; }
    }
}
