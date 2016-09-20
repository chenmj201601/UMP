//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5a1964af-c227-4289-9e95-c9886b68667e
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                MonitorType
//
//        created by Charley at 2015/6/26 9:34:37
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 监视对象的监视方式
    /// </summary>
    public enum MonitorType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = 0,
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
