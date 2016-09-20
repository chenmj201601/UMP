//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4768291a-d873-419a-9f46-8265f8df4872
//        CLR Version:              4.0.30319.18444
//        Name:                     PlayerEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Controls
//        File Name:                PlayerEventArgs
//
//        created by Charley at 2014/12/8 16:45:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Controls
{
    /// <summary>
    /// 播放器事件参数
    /// </summary>
    public class PlayerEventArgs : EventArgs
    {
        /// <summary>
        /// 事件代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public object Data { get; set; }
    }
}
