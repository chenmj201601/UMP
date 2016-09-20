//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fc3ad1f0-0276-4c6e-b187-f66da55c16bd
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateSqlType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateSqlType
//
//        created by Charley at 2016/5/9 10:47:51
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// Sql脚本的类型
    /// </summary>
    public enum UpdateSqlType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// DML语句
        /// </summary>
        DML = 1,
        /// <summary>
        /// DDL语句
        /// </summary>
        DDL = 2,
        /// <summary>
        /// 脚本文件
        /// </summary>
        File = 3,
    }
}
