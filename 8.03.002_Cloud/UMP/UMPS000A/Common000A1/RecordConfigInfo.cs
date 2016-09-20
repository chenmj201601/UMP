//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c2194884-b7e0-4cdd-8af5-6de9542c9030
//        CLR Version:              4.0.30319.18063
//        Name:                     RecordConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                RecordConfigInfo
//
//        created by Charley at 2015/9/2 15:56:50
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 录音信息字段配置信息
    /// </summary>
    [XmlRoot]
    public class RecordConfigInfo
    {
        public const string FILE_NAME = "UMP.Server.06.xml";

        /// <summary>
        /// 字段列表
        /// </summary>
        [XmlArray]
        public List<XmlMappingItem> Items
        {
            get { return mItems; }
        }

        private List<XmlMappingItem> mItems;

        public RecordConfigInfo()
        {
            mItems = new List<XmlMappingItem>();
        }
    }
}
