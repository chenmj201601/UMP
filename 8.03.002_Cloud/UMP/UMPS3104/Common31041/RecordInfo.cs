using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class RecordInfo
    {
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public DateTime StopRecordTime { get; set; }
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
        public string Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }


        public long ScoreID { set; get; }
        public string Score { set; get; }
        public long TemplateID { set; get; }
        /// <summary>
        /// 是否被申诉，n/0未申诉，y/1申诉中， c/2表申诉完成
        /// </summary>
        /// 
        public string AppealMark { set; get; }
        /// <summary>
        /// 书签状态。0/3104T00130删除，1/3104T00100正常
        /// </summary>
        public string BookMark { set; get; }

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

        public string WaveFormat { get; set; }

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
        #endregion
    }
}
