//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d0c636b0-e96e-4d74-9c62-168d696d28fd
//        CLR Version:              4.0.30319.18408
//        Name:                     FileInstallMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                FileInstallMode
//
//        created by Charley at 2016/5/9 10:41:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// 文件安装模式
    /// </summary>
    public enum FileInstallMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 复制，如果存在则直接复制（保留原有文件）
        /// </summary>
        CopyReplace = 1,
        /// <summary>
        /// 复制，如果存在忽略
        /// </summary>
        CopyIgnore = 2,
        /// <summary>
        /// 复制，如果存在报告错误
        /// </summary>
        CopyError = 3,
        /// <summary>
        /// 复制，如果存在，先删除， 再复制（不保留原有文件）
        /// </summary>
        CopyDelete=4,
        /// <summary>
        /// 删除文件，如果不存在则忽略
        /// </summary>
        RemoveIgnore = 11,
        /// <summary>
        /// 删除文件，如果不存在报告错误
        /// </summary>
        RemoveError = 12,
    }
}
