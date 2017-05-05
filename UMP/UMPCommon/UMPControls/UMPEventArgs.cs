//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2f2faab2-dfe3-4012-a639-85c8d5ff0a72
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPEventArgs
//
//        created by Charley at 2015/7/20 10:30:33
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Controls
{
    public class UMPEventArgs : EventArgs
    {
        /// <summary>
        /// 事件代码
        /// 0 ~ 999，一般事件代码（参考VoiceCyber.Common.Defines中的定义）
        /// 1000 ~ 9999，自定义事件代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 文本消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 其他数据
        /// </summary>
        public object Data { get; set; }
    }
}
