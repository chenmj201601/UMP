//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9b59e697-7358-469d-84f0-5ffb866f5192
//        CLR Version:              4.0.30319.18444
//        Name:                     TreeObjectEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                TreeObjectEventArgs
//
//        created by Charley at 2014/11/27 16:24:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// TreeObjectViewer控件的事件参数
    /// </summary>
    public class TreeObjectEventArgs : EventArgs
    {
        /// <summary>
        /// 事件代码
        /// 100     数据加载完成
        /// 999     异常消息
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
