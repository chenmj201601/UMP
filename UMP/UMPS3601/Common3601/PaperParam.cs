namespace Common3601
{
    public class CPaperParam
    {
        #region 试题
        
        /// <summary>
        /// 主键:试卷编号
        /// </summary>
        public long LongNum { get; set; }
        
        /// <summary>
        /// 试卷名称
        /// </summary>
        public string StrName { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string StrDescribe { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public char CharType { get; set; }

        /// <summary>
        /// 课程、课本、章节对应相应的编号
        /// </summary>
        public long LongSource { get; set; }

        /// <summary>
        /// 权重（跟着课本的权重来）
        /// </summary>
        public int IntWeight { get; set; }

        /// <summary>
        /// 试题数目
        /// </summary>
        public int IntQuestionNum { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public int IntScores { get; set; }

        /// <summary>
        /// 合格分
        /// </summary>
        public int IntPassMark { get; set; }

        /// <summary>
        /// 答题时长（分钟）
        /// </summary>
        public int IntTestTime { get; set; }
        
        /// <summary>
        /// 编辑人ID
        /// </summary>
        public long LongEditorId { get; set; }
        
        /// <summary>
        /// 编辑人
        /// </summary>
        public string StrEditor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public string StrDateTime { get; set; }
        
        /// <summary>
        /// 是否已经使用
        /// </summary>
        public int IntUsed { get; set; }
        
        /// <summary>
        /// 是否审核通过  0：未审核 1审核通过
        /// </summary>
        public int IntAudit { get; set; }

        /// <summary>
        /// 审核人ID
        /// </summary>
        public long LongVerifierId { get; set; }
        

        /// <summary>
        /// 审核人
        /// </summary>
        public string StrVerifier { get; set; }
        

        #endregion
    }
}
