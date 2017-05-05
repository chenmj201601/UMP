//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3611290a-152b-4f72-859a-acf28bcf8c66
//        CLR Version:              4.0.30319.18063
//        Name:                     BasicScoreCommentInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31031
//        File Name:                BasicScoreCommentInfo
//
//        created by Charley at 2015/10/30 17:44:51
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Common31031
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
