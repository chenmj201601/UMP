//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29fd869c-975c-4f80-ad64-910fe054cb08
//        CLR Version:              4.0.30319.18063
//        Name:                     SystemConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                SystemConfig
//
//        created by Charley at 2015/8/2 14:02:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace UMPTools.Models
{
    public class SystemConfig
    {
        public const string CONFIG_NAME = "config.xml";

        private List<ModuleInfo> mListModuleInfos;
        [XmlArray]
        public List<ModuleInfo> ListModuleInfos
        {
            get { return mListModuleInfos; }
        }

        public SystemConfig()
        {
            mListModuleInfos = new List<ModuleInfo>();
        }
    }
}
