//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6c7144ee-37c2-427a-a18c-eea1ff07964d
//        CLR Version:              4.0.30319.18408
//        Name:                     NodeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                NodeItem
//
//        created by Charley at 2016/5/9 10:13:19
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPServicePackCommon
{
    /// <summary>
    /// 节点项目
    /// </summary>
    [XmlRoot(ElementName = "Node", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class NodeItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
        /// <summary>
        /// 错误响应
        /// </summary>
        [XmlAttribute]
        public int ErrorReply { get; set; }
    }
}
