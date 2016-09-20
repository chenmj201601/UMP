//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    08c69735-22b8-43f2-a637-80de062c2d08
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                RecordInfo
//
//        created by Charley at 2014/11/9 11:03:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 记录信息
    /// </summary>
    public class RecordInfo
    {
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string WaveFormat { get; set; }

        public string SkillGroup { get; set; }
        public string RealExtension { get; set; }
        public string ParticipantNum { get; set; }

        public DateTime StopRecordTime { get; set; }

        public string ServiceAttitude { get; set; }

        public string RecordDuritionExcept { get; set; }

        public string RepeatedCall { get; set; }

        public string CallPeak { get; set; }

        public string ProfessionalLevel { get; set; }
        /// <summary>
        /// 2    只有一条最终分
        /// 1    已评分
        /// 0    未评分
        /// </summary>
        public int IsScored { get; set; }

        /// <summary>
        /// 分数[当该录音只有一条有效评分的时候 这个才会有值]
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 初检任务号
        /// </summary>
        public string TaskNumber { get; set; }
        /// <summary>
        /// 媒体类型
        /// 0：录音（带录屏）
        /// 1: 录音
        /// 2：录屏
        /// </summary>
        public int MediaType { get; set; }
        /// <summary>
        /// 加密标识
        /// 0：无加密
        /// 2：二代加密
        /// E：待加密
        /// F: 加密失败
        /// </summary>
        public string EncryptFlag { get; set; }
        /// <summary>
        /// 艺赛旗录屏流水号（SessionID）
        /// </summary>
        public string IsaRefID { get; set; }
        /// <summary>
        /// CTI流水号
        /// </summary>
        public string CTIReference { get; set; }
        /// <summary>
        /// 主叫DTMF
        /// </summary>
        public string CallerDTMF { get; set; }
        /// <summary>
        /// 被叫DTMF
        /// </summary>
        public string CalledDTMF { get; set; }
        /// <summary>
        /// 是否有录音标签或者关键词[1,有 0,没有]
        /// </summary>
        public string IsHaveBookMark { get; set; }
        /// <summary>
        /// 是否有备注[1,有 0,没有]
        /// </summary>
        public string IsHaveMemo { get; set; }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 是否有语音标签[1.有 0.没有]
        /// </summary>
        public string IsHaveVoiceBookMark { get; set; }
        //客户化字段
        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
        public string CustomField05 { get; set; }
        public string CustomField06 { get; set; }
        public string CustomField07 { get; set; }
        public string CustomField08 { get; set; }
        public string CustomField09 { get; set; }
        public string CustomField10 { get; set; }
        public string CustomField11 { get; set; }
        public string CustomField12 { get; set; }
        public string CustomField13 { get; set; }
        public string CustomField14 { get; set; }
        public string CustomField15 { get; set; }
        public string CustomField16 { get; set; }
        public string CustomField17 { get; set; }
        public string CustomField18 { get; set; }
        public string CustomField19 { get; set; }
        public string CustomField20 { get; set; }

    }
}
