//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    819570d0-c000-45cf-9333-8cf081d24293
//        CLR Version:              4.0.30319.18408
//        Name:                     S4410Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                S4410Codes
//
//        created by Charley at 2016/5/10 14:48:33
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common44101
{
    public enum S4410Codes
    {
        Unkown = 0,
        /// <summary>
        /// 获取区域信息列表
        /// </summary>
        GetRegionInfoList = 1,
        /// <summary>
        /// 获取用户管理的区域列表
        /// </summary>
        GetUserRegionList = 2,
        /// <summary>
        /// 获取区域的管理人员列表
        /// </summary>
        GetRegionUserList = 3,
        /// <summary>
        /// 获取座位信息列表
        /// </summary>
        GetSeatInfoList = 4,
        /// <summary>
        /// 获取区域座位列表
        /// </summary>
        GetRegionSeatList = 5,
        /// <summary>
        /// 获取坐席状态列表
        /// </summary>
        GetAgentStateList = 6,
        /// <summary>
        /// 获取告警消息列表
        /// </summary>
        GetAlarmMessageList = 7,
        /// <summary>
        /// 获取告警使用者列表
        /// </summary>
        GetAlarmUserList = 8,


        /// <summary>
        /// 保存区域信息
        /// </summary>
        SaveRegionInfo = 101,
        /// <summary>
        /// 删除区域信息
        /// </summary>
        DeleteRegionInfo = 102,
        /// <summary>
        /// 设置区域管理权限
        /// </summary>
        SetRegionMmt = 103,
        /// <summary>
        /// 保存座位信息
        /// </summary>
        SaveSeatInfo = 104,
        /// <summary>
        /// 保存区域座位信息
        /// </summary>
        SaveRegionSeatInfo = 105,
        /// <summary>
        /// 保存坐席状态信息
        /// </summary>
        SaveAgentStateInfo = 106,
        /// <summary>
        /// 保存告警消息
        /// </summary>
        SaveAlarmMessage = 107,
        /// <summary>
        /// 保存告警使用者
        /// </summary>
        SaveAlarmUser = 108,
    }
}
