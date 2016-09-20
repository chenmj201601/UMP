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

namespace UMPServicePackCommon
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [XmlRoot(ElementName = "UpdateFile", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
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

        /// <summary>
        /// 所属安装包(安装包写在注册表中的名字)
        /// </summary>
        [XmlAttribute]
        public string Package { get; set; }

        /// <summary>
        /// 目标路径类型
        /// 系统文件夹 或 安装目录文件夹
        /// 这个文件或其依赖的文件 的安装路径类型 是装在系统文件夹下 还是安装包的安装路径下
        /// 系统文件夹又分多种，需一个个判断 每种值对应一个路径
        /// </summary>
        [XmlAttribute]
        public int TargetPathType { get; set; }

        /// <summary>
        /// 依赖的文件
        /// 如果是新增文件：写文件所属模块的主文件相对于TargetPathType路径的相对路径
        /// 如果是修改文件：可以与TargetPath相同
        /// 此文件需与依赖文件在同级目录
        /// </summary>
        [XmlAttribute]
        public string DependFile { get; set; }
    }
}
