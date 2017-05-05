//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    497ddfc9-9f97-4c73-961d-4c9f423b72d3
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateModuleType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateModuleType
//
//        created by Charley at 2016/5/9 10:33:47
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// 更新类型
    /// </summary>
    public enum UpdateModuleType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 新增
        /// </summary>
        New = 1,
        /// <summary>
        /// 改进
        /// </summary>
        Improve = 2,
        /// <summary>
        /// Bug
        /// </summary>
        Bug = 3,
    }
}
