//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    39a250de-8f68-41bb-89e0-5314c7b5d4a0
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateFileType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateFileType
//
//        created by Charley at 2016/5/9 10:38:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPSPCommon
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum UpdateFileType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 单个文件
        /// </summary>
        File = 1,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory = 2,
    }
}
