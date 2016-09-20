namespace Common3603
{
    public class CPaperQuestionParam
    {
        #region 试卷

        /// <summary>
        /// 主键:试卷编号
        /// </summary>
        public long LongPaperNum { get; set; }

        /// <summary>
        /// 主键:试题
        /// </summary>
        public long LongQuestionNum { get; set; }

        /// <summary>
        /// 主键:试题
        /// </summary>
        public string StrQuestionCategory { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        public int IntQuestionType { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string StrQuestionsContect { get; set; }

        /// <summary>
        /// 题目答案是否可以修改
        /// </summary>
        public int EnableChange { get; set; }

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
        /// 分数
        /// </summary>
        public int IntScore { get; set; }

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
        #endregion
    }
}
