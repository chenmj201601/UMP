//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    37be8be9-8c4f-4d0c-b339-d04fddf65b9d
//        CLR Version:              4.0.30319.18408
//        Name:                     InstallInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                InstallInfo
//
//        created by Charley at 2016/6/8 16:39:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace VoiceCyber.UMP.Updates
{
    [XmlRoot(ElementName = "InstallInfo", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class InstallInfo
    {
        [XmlAttribute]
        public string SessionID { get; set; }
        [XmlAttribute]
        public string InstallTime { get; set; }
        [XmlAttribute]
        public string BeginTime { get; set; }
        [XmlAttribute]
        public string EndTime { get; set; }
        /// <summary>
        /// 安装包类型，完整安装包，累积补丁包，增量补丁包等
        /// </summary>
        [XmlAttribute]
        public int Type { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        [XmlAttribute]
        public string MachineName { get; set; }
        [XmlAttribute]
        public string OSVersion { get; set; }
        [XmlAttribute]
        public string OSAccount { get; set; }

        private List<InstallProduct> mListProducts = new List<InstallProduct>();
        [XmlArray(ElementName = "Products")]
        [XmlArrayItem(ElementName = "Product")]
        public List<InstallProduct> ListProducts
        {
            get { return mListProducts; }
        }
        [XmlElement(ElementName = "UpdateInfo")]
        public UpdateInfo UpdateInfo { get; set; }

        public const string FILE_NAME = "InstallInfo.xml";
    }
}
