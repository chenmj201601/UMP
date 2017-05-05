//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    790739e2-09f0-4eec-801a-b11131ec682d
//        CLR Version:              4.0.30319.18444
//        Name:                     LanguageInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                LanguageInfo
//
//        created by Charley at 2014/8/26 15:09:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
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
        /// <summary>
        /// 对象名称，某些语言会使用对象名称标识一条语言
        /// </summary>
        [DataMember]
        public string ObjName { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}({2})", LangID, Name, Display);
        }
    }
}
