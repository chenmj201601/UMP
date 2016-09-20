//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3be69e68-eea6-48df-93ac-323959588143
//        CLR Version:              4.0.30319.42000
//        Name:                     AppConfigs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                AppConfigs
//
//        created by Charley at 2016/2/3 14:55:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common12001
{
    [XmlRoot]
    public class AppConfigs
    {
        public const string FILE_NAME = "UMP.Server.09.xml";

        private List<AppConfigInfo> mListApps;
        [XmlArray(ElementName = "Apps")]
        public List<AppConfigInfo> ListApps
        {
            get { return mListApps; }
        }

        public AppConfigs()
        {
            mListApps = new List<AppConfigInfo>();
        }
    }
}
