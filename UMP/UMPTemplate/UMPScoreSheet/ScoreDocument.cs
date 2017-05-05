//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    93de0c84-7b02-4609-b378-57a6885a251d
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreDocument
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreDocument
//
//        created by Charley at 2014/7/16 9:57:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分表文档
    /// </summary>
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ScoreDocument
    {
        /// <summary>
        /// 名称（文件名）
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// 完整路径
        /// </summary>
        [XmlAttribute]
        public string FullPath { get; set; }
        /// <summary>
        /// 所在目录
        /// </summary>
        [XmlAttribute]
        public string FolderPath { get; set; }
        /// <summary>
        /// 评分对象（评分表）
        /// </summary>
        [XmlElement]
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [XmlAttribute]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlAttribute]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最后修改人
        /// </summary>
        [XmlAttribute]
        public string LastModifier { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [XmlAttribute]
        public DateTime ModifyTime { get; set; }
    }
}
