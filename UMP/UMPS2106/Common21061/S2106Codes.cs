//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    13c09d5d-3e77-480d-b07d-790d43ffdc9f
//        CLR Version:              4.0.30319.42000
//        Name:                     S2106Codes
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common21061
//        File Name:                S2106Codes
//
//        Created by Charley at 2016/10/19 13:57:12
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common21061
{
    /// <summary>
    /// 2106请求码
    /// </summary>
    public enum S2106Codes
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 获取录音服务器列表
        /// </summary>
        GetVoiceList = 1,
        /// <summary>
        /// 获取通道列表
        /// </summary>
        GetChannelList = 2,
        /// <summary>
        /// 获取DEC服务器列表
        /// </summary>
        GetDECList = 3,
        /// <summary>
        /// 获取恢复策略列表
        /// </summary>
        GetRecoverStrategyList = 4,
        /// <summary>
        /// 获取指定策略的恢复通道列表
        /// </summary>
        GetRecoverChannelList = 5,
        /// <summary>
        /// 获取恢复服务器列表
        /// </summary>
        GetRecoverServerList = 6,
        /// <summary>
        /// 获取服务器磁盘驱动器列表
        /// </summary>
        GetDiskDriverList = 7,
        /// <summary>
        /// 获取服务器子目录列表
        /// </summary>
        GetChildDirList = 8,
        /// <summary>
        /// 获取恢复策略执行标识列表
        /// </summary>
        GetStrategyFlagList = 9,
        /// <summary>
        /// 执行恢复策略
        /// </summary>
        ExecuteStrategy = 101,
        /// <summary>
        /// 保存恢复策略信息
        /// </summary>
        SaveRecoverStrategy = 102,
        /// <summary>
        /// 保存恢复策略的通道信息
        /// </summary>
        SaveRecoverChannels = 103,
    }
}
