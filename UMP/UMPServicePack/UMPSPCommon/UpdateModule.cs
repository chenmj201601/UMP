//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    410a746e-1b14-4b52-a3a5-0e0f65ba9f60
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateModule
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateModule
//
//        created by Charley at 2016/5/9 10:15:44
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPSPCommon
{
    /// <summary>
    /// 更新的功能模块
    /// </summary>
    [XmlRoot(ElementName = "UpdateModule", Namespace = "http://www.voicecyber.com/UMP/Updaters/2016/05")]
    public class UpdateModule : NodeItem
    {
        /// <summary>
        /// 类型（新增/改进/Bug）
        /// </summary>
        [XmlAttribute]
        public int Type { get; set; }
        /// <summary>
        /// 等级（1 - 10）
        /// </summary>
        [XmlAttribute]
        public int Level { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }
        /// <summary>
        /// 语言包ID
        /// </summary>
        [XmlAttribute]
        public string Lang { get; set; }
    }
}
