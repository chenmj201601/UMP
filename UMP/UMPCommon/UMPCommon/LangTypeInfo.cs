//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1fe06e74-cf6e-4280-9d86-18fa5f48dfaf
//        CLR Version:              4.0.30319.18444
//        Name:                     LangTypeInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                LangTypeInfo
//
//        created by Charley at 2014/8/27 15:25:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 语言类别信息
    /// </summary>
    [DataContract]
    public class LangTypeInfo
    {
        /// <summary>
        /// 语言代码（1033，1041，1028，2052等）
        /// </summary>
        [DataMember]
        public int LangID { get; set; }
        /// <summary>
        /// 语言简称（zh-cn，en-us等）
        /// </summary>
        [DataMember]
        public string LangName { get; set; }
        /// <summary>
        /// 显示名称（简体中文，English等）
        /// </summary>
        [DataMember]
        public string Display { get; set; }

        public LangTypeInfo()
        {
            LangID = 0;
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}({2})", LangID, LangName, Display);
        }
    }
}
