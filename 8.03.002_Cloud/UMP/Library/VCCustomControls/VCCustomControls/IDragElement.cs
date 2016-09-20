//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    785d8337-e9c2-4137-8e06-b6bd813ad22f
//        CLR Version:              4.0.30319.18063
//        Name:                     IDragElement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                IDragElement
//
//        created by Charley at 2014/4/4 12:14:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 可拖动元素
    /// </summary>
    public interface IDragElement
    {
        /// <summary>
        /// 开始移动
        /// </summary>
        event Action MoveStarted;
        /// <summary>
        /// 移动
        /// </summary>
        event Action Moved;
        /// <summary>
        /// 停止移动
        /// </summary>
        event Action MoveStopped;
    }
}
