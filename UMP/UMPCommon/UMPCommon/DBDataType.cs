//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a74c6315-c1bd-4737-9186-44e4fe18f121
//        CLR Version:              4.0.30319.18063
//        Name:                     DBDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                DBDataType
//
//        created by Charley at 2015/6/3 17:31:58
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 数据库中某一字段存储的数据的数据类型
    /// </summary>
    public enum DBDataType
    {
        /// <summary>
        /// 不确定
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 短整型
        /// </summary>
        Short = 1,
        /// <summary>
        /// 整形
        /// </summary>
        Int = 2,
        /// <summary>
        /// 长整形
        /// </summary>
        Long = 3,
        /// <summary>
        /// 数值
        /// </summary>
        Number = 4,
        /// <summary>
        /// 单个字符
        /// </summary>
        Char = 11,
        /// <summary>
        /// 单个字符
        /// </summary>
        NChar = 12,
        /// <summary>
        /// 可变长字符
        /// </summary>
        Varchar = 13,
        /// <summary>
        /// 可变长字符
        /// </summary>
        NVarchar = 14,
        /// <summary>
        /// 日期时间
        /// </summary>
        Datetime = 21
    }
}
