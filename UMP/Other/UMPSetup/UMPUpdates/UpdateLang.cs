//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6757d544-0cb4-49ba-8290-a8960914d50e
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateLang
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                UpdateLang
//
//        created by Charley at 2016/5/27 09:36:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 语言包信息
    /// </summary>
    [XmlRoot(ElementName = "UpdateFile", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateLang : NodeItem
    {
        /// <summary>
        /// 语言编码
        /// </summary>
        [XmlAttribute]
        public int LangID { get; set; }
        /// <summary>
        /// 语言名称
        /// </summary>
        [XmlAttribute]
        public string LangName { get; set; }
        /// <summary>
        /// 安装模式
        /// </summary>
        [XmlAttribute]
        public int InstallMode { get; set; }
        /// <summary>
        /// 语言包文件路径
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }
    }
}
