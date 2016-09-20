//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c93d8bbf-56ef-4b23-9f5c-49deb3b3cad3
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPFileInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                UMPFileInfo
//
//        created by Charley at 2015/12/21 14:23:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace UMPBuilder.Models
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [XmlRoot]
    public class UMPFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        [XmlAttribute]
        public int FileType { get; set; }
        /// <summary>
        /// 分类
        /// 0：未知
        /// 1：WcfServices\bin中的文件
        /// 2：Wcf2Client\bin中的文件
        /// 3：WinServices中的文件
        /// 4：WcfServices中的文件
        /// 5：Wcf2Client中的文件
        /// 6：根目录中的文件
        /// 7：ManagementMaintenance中的文件
        /// 8：WCF1600\bin中的文件
        /// 9：WCF1600中的文件
        /// </summary>
        [XmlAttribute]
        public int Category { get; set; }
        /// <summary>
        /// 源位置路径
        /// </summary>
        [XmlAttribute]
        public string SourcePath { get; set; }
    }
}
