using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    /// <summary>
    /// 评分项备注信息
    /// </summary>
    public class BasicScoreCommentInfo
    {
        /// <summary>
        /// 成绩ID
        /// </summary>
        public long ScoreResultID { get; set; }
        /// <summary>
        /// 评分表ID
        /// </summary>
        public long ScoreSheetID { get; set; }
        /// <summary>
        /// 评分项ID
        /// </summary>
        public long ScoreItemID { get; set; }
        /// <summary>
        /// 备注ID
        /// </summary>
        public long ScoreCommentID { get; set; }
        /// <summary>
        /// 备注内容（文本型备注）
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// 备注内容ID（多值型备注）
        /// </summary>
        public long CommentItemID { get; set; }
        /// <summary>
        /// 备注内容的OrderID（多值型备注）
        /// </summary>
        public int CommentItemOrderID { get; set; }
    }
}
