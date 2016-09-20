//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f9425ee3-4af8-4c49-b38d-0ee220c3f596
//        CLR Version:              4.0.30319.18063
//        Name:                     GlobalSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                GlobalSetting
//
//        created by Charley at 2015/5/18 17:59:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 全局设定信息
    /// </summary>
    [DataContract]
    public class GlobalSetting
    {
        /// <summary>
        /// 键名
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Key { get; set; }
        /// <summary>
        /// 设定值
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Key, Value);
        }
    }
}
