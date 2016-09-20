//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6e7063d5-cb54-43e4-8e36-31dcc56450ae
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdateTool.Models
//        File Name:                UpdateConfig
//
//        created by Charley at 2016/6/1 14:44:13
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace UMPUpdateTool.Models
{
    [XmlRoot]
    public class UpdateConfig
    {
        [XmlAttribute]
        public string CurrentVersion { get; set; }

        private List<ModuleInfo> mListModuleInfos = new List<ModuleInfo>();
        [XmlArray(ElementName = "Modules")]
        [XmlArrayItem(ElementName = "Module")]
        public List<ModuleInfo> ListModuleInfos
        {
            get { return mListModuleInfos; }
        }

        [XmlElement]
        public DatabaseInfo DatabaseInfo { get; set; }

        public const string FILE_NAME = "UpdateConfig.xml";
    }
}
