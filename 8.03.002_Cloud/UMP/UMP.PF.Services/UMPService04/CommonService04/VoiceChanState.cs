//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2d6aee53-0fc6-47d8-9f1a-6968a6007968
//        CLR Version:              4.0.30319.18063
//        Name:                     VoiceChanState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                VoiceChanState
//
//        created by Charley at 2015/6/25 9:48:49
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 录音通道状态信息
    /// </summary>
    public class VoiceChanState
    {
        /// <summary>
        /// 通道的资源编号
        /// </summary>
        public long ChanObjID { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChanID { get; set; }
        /// <summary>
        /// 通道名
        /// </summary>
        public string ChanName { get; set; }
        /// <summary>
        /// 录音服务器的资源编号
        /// </summary>
        public long VoiceObjID { get; set; }
        /// <summary>
        /// 录音服务器ID
        /// </summary>
        public int VoiceID { get; set; }
        /// <summary>
        /// 录音服务器的地址
        /// </summary>
        public string VoiceAddress { get; set; }
        /// <summary>
        /// 分机的资源编码
        /// </summary>
        public long ExtObjID { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 分机名称
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 坐席的资源编码
        /// </summary>
        public long AgentObjID { get; set; }
        /// <summary>
        /// 坐席号
        /// </summary>
        public string AgentID { get; set; }
        /// <summary>
        /// 坐席全名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 所在机构编号
        /// </summary>
        public long OrgObjID { get; set; }
        /// <summary>
        /// 所在机构的名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 登录状态
        /// </summary>
        public LoginState LoginState { get; set; }
        /// <summary>
        /// 呼叫状态
        /// </summary>
        public CallState CallState { get; set; }
        /// <summary>
        /// 录制状态
        /// </summary>
        public RecordState RecordState { get; set; }

        /// <summary>
        /// 记录流水号
        /// </summary>
        public string RecordReference { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public int DirectionFlag { get; set; }
        /// <summary>
        /// 主叫号码
        /// </summary>
        public string CallerID { get; set; }
        /// <summary>
        /// 被叫号码
        /// </summary>
        public string CalledID { get; set; }
        /// <summary>
        /// 开始录音时间
        /// </summary>
        public DateTime StartRecordTime { get; set; }
        /// <summary>
        /// 结束录音时间
        /// </summary>
        public DateTime StopRecordTime { get; set; }
        /// <summary>
        /// 录音时长
        /// </summary>
        public int RecordLength { get; set; }

        /// <summary>
        /// 返回此对象的简略信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]({1})[{2}][{3}][{4}][{5}-{6}-{7}]",
                ChanID,
                ChanObjID,
                VoiceID,
                Extension,
                AgentID,
                LoginState,
                CallState,
                RecordState);
        }
    }
}
