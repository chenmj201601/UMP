//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0a0d4826-a724-4a4b-a006-ad1ddb98d0e8
//        CLR Version:              4.0.30319.18063
//        Name:                     MssqlDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.DBAccesses
//        File Name:                MssqlDataType
//
//        created by Charley at 2015/7/26 17:30:48
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.DBAccesses
{
    /// <summary>
    /// Sql Server数据库的数据类型
    /// </summary>
    public enum MssqlDataType
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
