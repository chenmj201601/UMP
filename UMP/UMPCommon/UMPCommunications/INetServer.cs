//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    23815711-9e39-40f8-9f92-aa9a9aafffef
//        CLR Version:              4.0.30319.18063
//        Name:                     INetServer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                INetServer
//
//        created by Charley at 2015/9/21 9:52:19
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using VoiceCyber.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 定义Socket通讯服务的基本功能
    /// </summary>
    public interface INetServer
    {
        /// <summary>
        /// 返回当前存在的NetSession列表
        /// </summary>
        IList<INetSession> ListSessions { get; }
        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();
        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
        /// <summary>
        /// 调试信息
        /// </summary>
        event Action<LogMode, string, string> Debug;
    }
}
