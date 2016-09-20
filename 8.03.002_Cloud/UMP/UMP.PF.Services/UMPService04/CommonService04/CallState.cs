//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2fdfe6de-0678-4f7f-b9f4-31c793093751
//        CLR Version:              4.0.30319.18063
//        Name:                     CallState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                CallState
//
//        created by Charley at 2015/6/25 9:41:11
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 呼叫状态
    /// </summary>
    public enum CallState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,
        /// <summary>
        /// 拨号中（摘机）
        /// </summary>
        Dialing = 1,
        /// <summary>
        /// 振铃中
        /// </summary>
        Ringing = 2,
        /// <summary>
        /// 通话中
        /// </summary>
        Talking = 3,
        /// <summary>
        /// 通话保持
        /// </summary>
        Holding = 4,
        /// <summary>
        /// 转接中（咨询）
        /// </summary>
        Consulting = 5,
        /// <summary>
        /// 建立会议中
        /// </summary>
        Conferencing = 6
    }
}
