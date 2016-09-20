//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2d6aee53-0fc6-47d8-9f1a-6968a6007968
//        CLR Version:              4.0.30319.18063
//        Name:                     ChanState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                ChanState
//
//        created by Charley at 2015/6/25 9:48:49
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 通道状态信息
    /// </summary>
    public class ChanState
    {
        /// <summary>
        /// 通道的资源编号
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 通道的类型（录音通道，录屏通道）
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChanID { get; set; }
        /// <summary>
        /// 所在服务器ID
        /// </summary>
        public int ServerID { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 坐席号
        /// </summary>
        public string AgentID { get; set; }
        /// <summary>
        /// 真实分机号
        /// </summary>
        public string RealExt { get; set; }
        /// <summary>
        /// 所在机构编号
        /// </summary>
        public long OrgObjID { get; set; }

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
        /// 时间偏差（服务器时间与本服务器时间的偏差，以秒为单位）
        /// </summary>
        public double TimeDeviation { get; set; }


        /// 保留字段（共10个），暂不定义，用到时再定义
        public string Other01 { get; set; }         //RealExtension，实际分机
        public string Other02 { get; set; }        
        public string Other03 { get; set; }         //所在服务器地址
        public string Other04 { get; set; }
        public string Other05 { get; set; }
        public string Other06 { get; set; }
        public string Other07 { get; set; }
        public string Other08 { get; set; }
        public string Other09 { get; set; }
        public string Other10 { get; set; }

        /// <summary>
        /// 返回此对象的简略信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]({1})[{2}][{3}][{4}-{5}-{6}]",
                ChanID,
                ObjID,
                Extension,
                AgentID,
                LoginState,
                CallState,
                RecordState);
        }
    }
}
