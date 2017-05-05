//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cb483803-0c68-428e-aa53-37bad161548d
//        CLR Version:              4.0.30319.18063
//        Name:                     UpdateConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                UpdateConfigInfo
//
//        created by Charley at 2015/10/22 11:24:25
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 更新录音信息字段配置信息
    /// </summary>
    [XmlRoot]
    public class UpdateConfigInfo
    {
        public const string FILE_NAME = "UMP.Server.08.xml";

        /// <summary>
        /// 字段列表
        /// </summary>
        [XmlArray]
        public List<XmlMappingItem> Items
        {
            get { return mItems; }
        }

        private List<XmlMappingItem> mItems;

        public UpdateConfigInfo()
        {
            mItems = new List<XmlMappingItem>();
        }
    }
}
