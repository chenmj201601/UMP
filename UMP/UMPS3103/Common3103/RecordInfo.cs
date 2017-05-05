using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    public class RecordInfo
    {
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public string ReaExtension { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string IsScore { get; set; }

        public DateTime StopRecordTime { get; set; }
        public string TaskUserID { get; set; }
        public string TaskUserName { get; set; }

        //2015-9-2 11:23:37新增
        public string WaveFormat { get; set; }

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

        #region ABCD
        /// <summary>
        /// 服务态度
        /// </summary>
        public int ServiceAttitude { get; set; }
        /// <summary>
        /// 专业水平
        /// </summary>
        public int ProfessionalLevel { get; set; }
        /// <summary>
        /// 录音时长异常,1异常,2正常
        /// </summary>
        public int RecordDurationError { get; set; }
        /// <summary>
        /// 重复呼入,1 重复,2正常
        /// </summary>
        public int RepeatCallIn { get; set; }
        /// <summary>
        /// 异常分数,1异常,2正常
        /// </summary>
        public int AbnormalScores { get; set; }
        /// <summary>
        ///亊后处理时长异常 ,1异常,2正常
        /// </summary>
        public int AfterEventProcessing { get; set; }
        /// <summary>
        /// 坐席/客人讲话时长比例异常,1异常,2正常
        /// </summary>
        public int SeatAgentSpeechAnomaly { get; set; }
        #endregion
    }
}
