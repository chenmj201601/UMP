//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    38bfe99a-8062-42e0-af48-847129f311cc
//        CLR Version:              4.0.30319.18063
//        Name:                     RecordEncryptConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordEncryptConfig
//
//        created by Charley at 2015/8/3 19:16:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace UMPS3102.Models
{
    public class RecordEncryptConfig
    {
        public const string FILE_NAME = "RecordEncryptConfig.xml";

        private List<EncryptInfoConfig> mListEncryptInfos; 
        [XmlArray]
        public List<EncryptInfoConfig> ListEncryptInfos
        {
            get { return mListEncryptInfos; }
        }

        public RecordEncryptConfig()
        {
            mListEncryptInfos = new List<EncryptInfoConfig>();
        }
    }
}
