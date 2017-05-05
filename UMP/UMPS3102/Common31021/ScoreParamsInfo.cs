using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 控制录音能否评分的参数
    /// </summary>
    public class ScoreParamsInfo
    {
        /// <summary>
        /// 类型(1 被分配成任务的录音,2 被申诉的录音)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 能否评分(0 能评分,1 不能评分)
        /// </summary>
        public string Value { get; set; } 
    }
}
