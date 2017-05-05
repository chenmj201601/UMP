//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6d59dc8a-5f9f-4943-b45d-fa3ca839fa9d
//        CLR Version:              4.0.30319.18408
//        Name:                     PackType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                PackType
//
//        created by Charley at 2016/5/9 10:25:26
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 安装包或补丁包的类型
    /// </summary>
    public enum PackType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 完整的产品安装包
        /// </summary>
        Product = 1,
        /// <summary>
        /// 累积补丁包
        /// </summary>
        Accumulate = 2,
        /// <summary>
        /// 增量补丁包
        /// </summary>
        Increment = 3,
        /// <summary>
        /// 升级包
        /// </summary>
        Upgrade = 4,
    }
}
