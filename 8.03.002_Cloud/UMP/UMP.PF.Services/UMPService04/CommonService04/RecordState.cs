//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    333ad0b3-903e-4421-8fbe-3477769b3ac8
//        CLR Version:              4.0.30319.18063
//        Name:                     RecordState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                RecordState
//
//        created by Charley at 2015/6/25 9:45:26
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 录制状态
    /// </summary>
    public enum RecordState
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 录制中（录音或录屏）
        /// </summary>
        Recoding = 1
    }
}
