namespace Common3602
{
    public class CQuestionsParam
    {
        #region 试题
        
        /// <summary>
        /// 主键:试题编号
        /// </summary>
        public long LongNum { get; set; }
        
        /// <summary>
        /// 主键:试题分类编号
        /// </summary>
        public long LongCategoryNum { get; set; }
        
        /// <summary>
        /// 试题分类名称
        /// </summary>
        public string StrCategoryName { get; set; }
        
        /// <summary>
        /// 主键:试题类型
        /// </summary>
        public int IntType { get; set; }

        public string StrType { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string StrQuestionsContect { get; set; }
       
        /// <summary>
        /// 简答题答案
        /// </summary>
        public string StrShortAnswerAnswer { get; set; }

        /// <summary>
        /// 答案A
        /// </summary>
        public string StrAnswerOne { get; set; }
        
        /// <summary>
        /// 答案B
        /// </summary>
        public string StrAnswerTwo { get; set; }

        /// <summary>
        /// 答案C
        /// </summary>
        public string StrAnswerThree { get; set; }

        /// <summary>
        /// 答案D
        /// </summary>
        public string StrAnswerFour { get; set; }

        /// <summary>
        /// 答案D
        /// </summary>
        public string StrAnswerFive { get; set; }

        /// <summary>
        /// 答案D
        /// </summary>
        public string StrAnswerSix { get; set; }
 
        /// <summary>
        /// 正确答案a
        /// </summary>
        public string CorrectAnswerOne { get; set; }

        public string StrConOne { get; set; }

        /// <summary>
        /// 正确答案b
        /// </summary>
        public string CorrectAnswerTwo { get; set; }
        
        /// <summary>
        /// 正确答案c
        /// </summary>
        public string CorrectAnswerThree { get; set; }

       /// <summary>
        /// 正确答案d
        /// </summary>
        public string CorrectAnswerFour { get; set; }

        /// <summary>
        /// 正确答案d
        /// </summary>
        public string CorrectAnswerFive { get; set; }
        
        /// <summary>
        /// 正确答案d
        /// </summary>
        public string CorrectAnswerSix { get; set; }

        /// <summary>
        /// 使用次数
        /// </summary>
        public int IntUseNumber { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string StrAccessoryType { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string StrAccessoryName { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        public string StrAccessoryPath { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long LongFounderId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string StrFounderName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string StrDateTime { get; set; }
        

        #endregion
    }
}
