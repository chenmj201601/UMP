using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 录音会话信息,从T_51_002表拿数据
    /// </summary>
    public class ConversationInfo
    {
        /// <summary>
        /// 记录流水号,T_21_001表里面的行号，如果这个为0,代表为实时录音
        /// </summary>
        public string SerialID { get; set; }
        /// <summary>
        /// 录音流水号
        /// </summary>
        public string RecordReference { get; set; }
        /// <summary>
        /// 开始录音时间
        /// </summary>
        public DateTime StartRecordTime { get; set; }
        /// <summary>
        /// 相对于开始录音时间的偏移量
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// 结束录音时间
        /// </summary>
        public DateTime EndRecordTime { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 坐席或者客户的声音 (0 表示坐席,1表示客户)
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumber { get; set; }
        /// <summary>
        /// 说话的文本内容
        /// </summary>
        public string Content { get; set; }
    }
}
