//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    249f31b6-d4c4-490d-8ff9-da6d09d1d56d
//        CLR Version:              4.0.30319.18063
//        Name:                     QueryConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                QueryConfigInfo
//
//        created by Charley at 2015/9/2 15:52:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 查询录音的条件字段配置信息
    /// </summary>
    [XmlRoot]
    public class QueryConfigInfo
    {
        public const string FILE_NAME = "UMP.Server.05.xml";

        /// <summary>
        /// 字段列表
        /// </summary>
        [XmlArray]
        public List<XmlMappingItem> Items
        {
            get { return mItems; }
        }

        private List<XmlMappingItem> mItems;

        public QueryConfigInfo()
        {
            mItems = new List<XmlMappingItem>();
        }
    }
}
