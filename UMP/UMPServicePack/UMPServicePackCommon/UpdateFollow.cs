//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    58fa44cb-acca-4f9f-8059-7595628beee3
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateFollow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateFollow
//
//        created by Charley at 2016/5/9 11:09:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPServicePackCommon
{
    /// <summary>
    /// 后续操作
    /// </summary>
    [XmlRoot(ElementName = "UpdateFollow", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateFollow : NodeItem
    {
        /// <summary>
        /// 操作名编号
        /// </summary>
        [XmlAttribute]
        public int Key { get; set; }
        /// <summary>
        /// 值的数据类型
        /// </summary>
        [XmlAttribute]
        public int DataType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }
    }
}
