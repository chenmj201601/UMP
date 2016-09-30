//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    38773291-e4b2-4292-9002-35dbe53962c8
//        CLR Version:              4.0.30319.42000
//        Name:                     MysqlDataType
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.DBAccesses
//        File Name:                MysqlDataType
//
//        Created by Charley at 2016/9/24 15:59:53
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.DBAccesses
{
    /// <summary>
    /// MySql数据类型
    /// </summary>
    public enum MysqlDataType
    {
        /// <summary>
        /// 可变长字符型
        /// </summary>
        Varchar,
        /// <summary>
        /// 可变长字符型
        /// </summary>
        NVarchar,
        /// <summary>
        /// 固定大小字符型
        /// </summary>
        Char,
        /// <summary>
        /// 整型
        /// </summary>
        Int,
        /// <summary>
        /// 长整型
        /// </summary>
        Bigint
    }
}
