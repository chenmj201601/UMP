//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    20b37415-dd23-4774-a6bd-6207874c486d
//        CLR Version:              4.0.30319.18063
//        Name:                     S2501Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common25011
//        File Name:                S2501Codes
//
//        created by Charley at 2015/5/20 13:50:25
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common25011
{
    public enum S2501Codes
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 获取告警消息列表
        /// </summary>
        GetAlarmMessageList = 1,
        /// <summary>
        /// 获取告警信息列表
        /// </summary>
        GetAlarmInfoList = 2,
        /// <summary>
        /// 获取告警接受人列表
        /// </summary>
        GetAlarmReceiverList = 3,

        /// <summary>
        /// 保存告警信息
        /// </summary>
        SaveAlarmInfoList = 101,
        /// <summary>
        /// 删除告警信息
        /// </summary>
        RemoveAlarmInfoList = 102,
        /// <summary>
        /// 保存告警接收人信息
        /// </summary>
        SaveAlarmReceiverList = 103
    }
}
