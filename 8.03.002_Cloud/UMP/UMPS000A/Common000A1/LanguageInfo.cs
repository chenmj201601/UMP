//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1692e3b0-0d1b-4b0f-85ed-cd895034515a
//        CLR Version:              4.0.30319.18063
//        Name:                     LanguageInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                LanguageInfo
//
//        created by Charley at 2015/7/30 10:27:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 语言信息
    /// </summary>
    [DataContract]
    public class LanguageInfo
    {
        /// <summary>
        /// 语言类型的编码
        /// </summary>
        [DataMember]
        public int LangID { get; set; }
        /// <summary>
        /// 所在模块
        /// </summary>
        [DataMember]
        public int Module { get; set; }
        /// <summary>
        /// 所在子模块
        /// </summary>
        [DataMember]
        public int SubModule { get; set; }
        /// <summary>
        /// 所在页面
        /// </summary>
        [DataMember]
        public string Page { get; set; }
        /// <summary>
        /// 名称（内容编码)
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [DataMember]
        public string Display { get; set; }
        /// <summary>
        /// 语言ID
        /// </summary>
        [DataMember]
        public long SysID { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}({2})", LangID, Name, Display);
        }
    }
}
