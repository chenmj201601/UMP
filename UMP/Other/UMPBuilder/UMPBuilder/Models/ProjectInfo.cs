//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4751c8cf-9b05-4639-a079-253a621dc578
//        CLR Version:              4.0.30319.18063
//        Name:                     ProjectInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                ProjectInfo
//
//        created by Charley at 2015/12/21 14:13:12
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPBuilder.Models
{
    /// <summary>
    /// 项目信息
    /// </summary>
    [XmlRoot]
    public class ProjectInfo
    {
        /// <summary>
        /// 项目类型
        /// </summary>
        [XmlAttribute]
        public int ProjectType { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [XmlAttribute]
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目目录
        /// </summary>
        [XmlAttribute]
        public string ProjectDir { get; set; }
        /// <summary>
        /// 项目文件名
        /// </summary>
        [XmlAttribute]
        public string ProjectFile { get; set; }
    }
}
