//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    05bc3d40-c39e-45a3-9e49-0dd23410cc8e
//        CLR Version:              4.0.30319.18408
//        Name:                     RecordState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                RecordState
//
//        created by Charley at 2016/6/27 17:25:35
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 录制状态
    /// 按位，依次是Voice，Screen
    /// </summary>
    [Flags]
    public enum RecordState
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 仅录音
        /// </summary>
        Voice = 1,
        /// <summary>
        /// 仅录屏
        /// </summary>
        Screen = 2,
        /// <summary>
        /// 录音录屏
        /// </summary>
        All = 3,
    }
}
