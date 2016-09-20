//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    06ec29c5-a300-4398-bdd0-144b67dd8822
//        CLR Version:              4.0.30319.18408
//        Name:                     CallState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                CallState
//
//        created by Charley at 2016/6/27 17:23:30
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 话机呼叫状态
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
