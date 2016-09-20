//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dbaa65ac-aae9-4e89-9091-455b469454f3
//        CLR Version:              4.0.30319.18408
//        Name:                     AgentState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                AgentState
//
//        created by Charley at 2016/6/27 17:32:23
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 坐席状态
    /// </summary>
    public enum AgentState
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 空
        /// </summary>
        Null = 1,
        /// <summary>
        /// 就绪
        /// </summary>
        Ready = 2,
        /// <summary>
        /// 未就绪
        /// </summary>
        NotReady = 3,
        /// <summary>
        /// 事后处理
        /// </summary>
        WorkNotReady = 4,
        /// <summary>
        /// 工作就绪
        /// </summary>
        WorkReady = 5,
    }
}
