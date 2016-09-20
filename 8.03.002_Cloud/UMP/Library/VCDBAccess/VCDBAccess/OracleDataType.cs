//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c7bb7c4-4e91-42ba-9dcb-4420780ec100
//        CLR Version:              4.0.30319.18063
//        Name:                     OracleDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.DBAccesses
//        File Name:                OracleDataType
//
//        created by Charley at 2015/7/26 17:32:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.DBAccesses
{
    /// <summary>
    /// Oracle数据库的数据类型
    /// </summary>
    public enum OracleDataType
    {
        /// <summary>
        /// 可变长字符型
        /// </summary>
        Varchar2,
        /// <summary>
        /// 可变长字符型
        /// </summary>
        Nvarchar2,
        /// <summary>
        /// 整型
        /// </summary>
        Int32,
        /// <summary>
        /// 固定大小字符型
        /// </summary>
        Char,
        /// <summary>
        /// 日期型
        /// </summary>
        Date,
        /// <summary>
        /// 游标指针
        /// </summary>
        RefCursor
    }
}
