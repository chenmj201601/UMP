//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3d3fa443-3c85-4ea3-92dc-92c847df6397
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorObjectConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                MonitorObjectConfig
//
//        created by Charley at 2015/6/30 18:30:03
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace UMPS2102.Models
{
    [XmlRoot]
    public class MonitorObjectConfig
    {
        public const string FILE_NAME = "MonitorObjectConfig.xml";

        private List<UserMonitorObjectConfig> mListUserMonObjConfig;
        [XmlArray]
        public List<UserMonitorObjectConfig> ListUserMonObjConfig
        {
            get { return mListUserMonObjConfig; }
        }

        public MonitorObjectConfig()
        {
            mListUserMonObjConfig = new List<UserMonitorObjectConfig>();
        }
    }
}
