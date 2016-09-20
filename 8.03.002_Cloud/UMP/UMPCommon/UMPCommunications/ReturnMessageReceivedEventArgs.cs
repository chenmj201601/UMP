//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fa8a058f-35c3-4564-aff3-43a13460115a
//        CLR Version:              4.0.30319.18063
//        Name:                     ReturnMessageReceivedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                ReturnMessageReceivedEventArgs
//
//        created by Charley at 2015/9/21 11:25:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// ReturnMessageReceived事件参数
    /// </summary>
    public class ReturnMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 会话名称，通常为SessionID
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public ReturnMessage ReturnMessage { get; set; }
    }
}
