//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    56128373-b44d-4510-916b-04d343bb92aa
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateUpgrade
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                UpdateUpgrade
//
//        created by Charley at 2016/8/4 10:29:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;


namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 升级的新功能，在升级程序界面展示
    /// </summary>
    [XmlRoot(ElementName = "UpdateUpgrade", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateUpgrade : NodeItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        [XmlAttribute]
        public string SerialNo { get; set; }
        /// <summary>
        /// 所属模块编号
        /// </summary>
        [XmlAttribute]
        public int ModuleID { get; set; }
        /// <summary>
        /// 所属模块名称
        /// </summary>
        [XmlAttribute]
        public string ModuleName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [XmlAttribute]
        public string Content { get; set; }
        /// <summary>
        /// 内容对应的语言ID
        /// </summary>
        [XmlAttribute]
        public string LangID { get; set; }
        /// <summary>
        /// 模块对应语言ID
        /// </summary>
        [XmlAttribute]
        public string ModuleLangID { get; set; }
    }
}
