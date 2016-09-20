//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bb958dca-e536-4c3b-a94a-d2f77e247299
//        CLR Version:              4.0.30319.18063
//        Name:                     ConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                ConfigInfo
//
//        created by Charley at 2015/10/27 13:16:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common
{
    [XmlRoot]
    public class ConfigInfo
    {
        private string mFileName;
        [XmlAttribute]
        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        private string mPath;
        [XmlAttribute]
        public string Path
        {
            get { return mPath; }
            set { mPath = value; }
        }

        private List<GlobalSetting> mListSettings;
        [XmlArray]
        public List<GlobalSetting> ListSettings
        {
            get { return mListSettings; }
        }

        public ConfigInfo()
        {
            mFileName = string.Empty;
            mPath = string.Empty;
            mListSettings = new List<GlobalSetting>();
        }

        public ConfigInfo(string fileName)
            : this()
        {
            mFileName = fileName;
        }

        public ConfigInfo(string fileName, string path)
            : this(fileName)
        {
            mPath = path;
        }
    }
}
