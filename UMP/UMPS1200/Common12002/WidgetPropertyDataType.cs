//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    86d3dc8f-8a01-417a-9f29-a022501a9a92
//        CLR Version:              4.0.30319.18408
//        Name:                     WidgetPropertyDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12002
//        File Name:                WidgetPropertyDataType
//
//        created by Charley at 2016/5/3 16:01:01
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12002
{
    public enum WidgetPropertyDataType
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
