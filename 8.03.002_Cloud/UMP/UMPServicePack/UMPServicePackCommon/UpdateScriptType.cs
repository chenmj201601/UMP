//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    72e11941-2441-49ea-aca3-fc06314f945a
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateScriptType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateScriptType
//
//        created by Charley at 2016/5/9 10:50:00
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// Sql语句类型
    /// </summary>
    public enum UpdateScriptType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// Select
        /// </summary>
        Select = 1,
        /// <summary>
        /// Update
        /// </summary>
        Update = 2,
        /// <summary>
        /// Insert
        /// </summary>
        Insert = 3,
        /// <summary>
        /// Delete
        /// </summary>
        Delete = 4,
        /// <summary>
        /// Create
        /// </summary>
        Create = 11,
        /// <summary>
        /// Alter
        /// </summary>
        Alter = 12,
        /// <summary>
        /// Drop
        /// </summary>
        Drop = 13,
    }
}
