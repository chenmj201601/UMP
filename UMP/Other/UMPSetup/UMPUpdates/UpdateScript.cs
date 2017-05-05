//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f446a99-3d61-4ab1-866d-d174eca4585e
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateScript
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateScript
//
//        created by Charley at 2016/5/9 10:21:13
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 更新的数据库脚本
    /// </summary>
    [XmlRoot(ElementName = "UpdateScript", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateScript
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        [XmlAttribute]
        public int DBType { get; set; }
        /// <summary>
        /// 脚本内容
        /// </summary>
        [XmlAttribute]
        public string Text { get; set; }
        /// <summary>
        /// 对应的脚本文件的路径
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }
    }
}
