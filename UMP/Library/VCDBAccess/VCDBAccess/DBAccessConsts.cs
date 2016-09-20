//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c60a7ea-4439-4e7f-aeb2-1b64569e21af
//        CLR Version:              4.0.30319.18063
//        Name:                     DBAccessConsts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.DBAccesses
//        File Name:                DBAccessConsts
//
//        created by Charley at 2015/9/17 16:44:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.DBAccesses
{
    /// <summary>
    /// 相关常量定义
    /// </summary>
    public class DBAccessConsts
    {

        #region 错误号定义

        /// <summary>
        /// 表不存在
        /// </summary>
        public const int ERR_TABLE_NOT_EXIST = 1001;

        #endregion


        #region MSSQL 错误号

        internal const int MSSQL_ERR_OBJECT_NOT_EXIST = 208;  //表不存在

        #endregion


        #region ORCL 错误号

        internal const int ORCL_ERR_OBJECT_NOT_EXIST = 942;     //表不存在

        #endregion

    }
}
