//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29729a3b-f0ab-4646-9e85-7a23046c2ef5
//        CLR Version:              4.0.30319.18063
//        Name:                     NotifyMessageReceivedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NotifyMessageReceivedEventArgs
//
//        created by Charley at 2015/9/21 11:26:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// NotifyMessageReceived事件参数
    /// </summary>
    public class NotifyMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 会话名称，通常为SessionID
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 通知消息
        /// </summary>
        public NotifyMessage NotifyMessage { get; set; }
    }
}
