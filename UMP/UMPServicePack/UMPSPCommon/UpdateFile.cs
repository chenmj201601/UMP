//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    31040813-495c-4f89-a343-400cd718d188
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateFile
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateFile
//
//        created by Charley at 2016/5/9 10:15:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPSPCommon
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [XmlRoot(ElementName = "UpdateFile", Namespace = "http://www.voicecyber.com/UMP/Updaters/2016/05")]
    public class UpdateFile : NodeItem
    {
        /// <summary>
        /// 文件类型，文件或者文件夹
        /// </summary>
        [XmlAttribute]
        public int Type { get; set; }
        /// <summary>
        /// 文件或文件夹的名称
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }
        /// <summary>
        /// 源路径
        /// </summary>
        [XmlAttribute]
        public string SourcePath { get; set; }
        /// <summary>
        /// 目标路径
        /// </summary>
        [XmlAttribute]
        public string TargetPath { get; set; }
        /// <summary>
        /// 安装模式（覆盖或删除）
        /// </summary>
        [XmlAttribute]
        public int InstallMode { get; set; }
    }
}
