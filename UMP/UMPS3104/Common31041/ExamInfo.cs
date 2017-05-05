using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class ExamInfo
    {
        /// <summary>
        /// 考试编号
        /// </summary>
        public string ExamId { get; set; }
        /// <summary>
        /// 试卷描述
        /// </summary>
        public string ExamTitle { get; set; }
        /// <summary>
        /// 考试类型，1：测试 2：课程测试 3：课本测试 4：单元测试 5：小节测试
        /// </summary>
        public string ExamType { get; set; }
        /// <summary>
        ///  考试时间
        /// </summary>
        public int ExamTime { get; set; }
        /// <summary>
        /// 试卷编号
        /// </summary>
        public string PaperID { get; set; }
        /// <summary>
        /// 试卷名称
        /// </summary>
        public string PaperName { get; set; }        
        /// <summary>
        /// 题目类型
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// 题目描述
        /// </summary>
        public string QuestionDec { get; set; }
        /// <summary>
        /// 题目答案
        /// </summary>
        public string QuestionKey { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string DocType { get; set; }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string DocPath { get; set; }
        /// <summary>
        /// 题目分数
        /// </summary>
        public int QuestionScore { get; set; }


    }
}
