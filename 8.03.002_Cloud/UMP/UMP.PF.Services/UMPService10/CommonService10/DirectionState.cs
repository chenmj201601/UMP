//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f5866ae3-970d-448c-9145-b8f207a289cf
//        CLR Version:              4.0.30319.18408
//        Name:                     DirectionState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                DirectionState
//
//        created by Charley at 2016/6/27 17:47:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 呼叫方向状态
    /// </summary>
    public enum DirectionState
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 呼入
        /// </summary>
        Callin = 1,
        /// <summary>
        /// 呼出
        /// </summary>
        Callout = 2,
    }
}
