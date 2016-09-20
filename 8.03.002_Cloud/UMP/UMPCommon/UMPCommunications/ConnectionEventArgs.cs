//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5c50d2a8-a0a4-44da-85ea-6c46beaf2385
//        CLR Version:              4.0.30319.18063
//        Name:                     ConnectionEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                ConnectionEventArgs
//
//        created by Charley at 2015/9/21 11:22:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// ConnectionEvent事件的参数
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// 会话名称，通常为SessionID
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 事件代码，参考VoiceCyber.Common.Defines中的定义
        /// 101     连接建立
        /// 102     连接断开
        /// 103     认证（成功）
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }
}
