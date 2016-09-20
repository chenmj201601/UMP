//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0bb53f51-662d-4a65-a718-663fa8fab04a
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceGroupType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ResourceGroupType
//
//        created by Charley at 2015/1/22 13:15:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 参数组的类型
    /// </summary>
    public enum ResourceGroupType
    {
        /// <summary>
        /// 不确定
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 普通参数组
        /// </summary>
        Group = 1,
        /// <summary>
        /// 子对象集合
        /// </summary>
        ChildList = 2,
        /// <summary>
        /// 带参数的子对象集合
        /// </summary>
        ParamWithList = 3
    }
}
