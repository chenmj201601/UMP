//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4f99890f-435a-4c1f-aa6a-6c7adc58f1ff
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectPropertyDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ObjectPropertyDataType
//
//        created by Charley at 2014/12/19 10:32:29
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 属性的数据类型
    /// </summary>
    public enum ObjectPropertyDataType
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
