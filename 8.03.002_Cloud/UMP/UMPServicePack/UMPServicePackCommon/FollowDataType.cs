//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    23afa2bd-ef4a-4a8b-ab90-bacb2bffb4c4
//        CLR Version:              4.0.30319.18408
//        Name:                     FollowDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                FollowDataType
//
//        created by Charley at 2016/5/9 11:07:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// 后续步骤的数据类型
    /// </summary>
    public enum FollowDataType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 开关
        /// </summary>
        YesNo = 1,
        /// <summary>
        /// 数值
        /// </summary>
        Numeric = 2,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 3,
        /// <summary>
        /// 日期时间
        /// </summary>
        Datetime = 4,
    }
}
