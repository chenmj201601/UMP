//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8f48a862-e960-4f74-83b8-3e104bf53253
//        CLR Version:              4.0.30319.18408
//        Name:                     MonType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                MonType
//
//        created by Charley at 2016/6/27 18:05:56
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 监视类型
    /// </summary>
    public enum MonType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 状态监视
        /// </summary>
        State = 1,
        /// <summary>
        /// 网络监听
        /// </summary>
        NMon = 2,
        /// <summary>
        /// 监屏
        /// </summary>
        MonScr = 3,
        /// <summary>
        /// 在线回放
        /// </summary>
        LivePlay = 4
    }
}
