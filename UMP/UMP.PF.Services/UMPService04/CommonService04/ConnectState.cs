//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    05a612cf-38ca-4206-8e54-f75666396437
//        CLR Version:              4.0.30319.42000
//        Name:                     ConnectState
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                ConnectState
//
//        Created by Charley at 2016/12/14 10:17:46
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 线路连接状态
    /// </summary>
    public enum ConnectState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 线路已连接
        /// </summary>
        Connected = 1,
        /// <summary>
        /// 线路已断开
        /// </summary>
        DisConnected = 2
    }
}
