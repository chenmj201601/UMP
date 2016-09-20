namespace Common3603
{
    public class TestUserParam
    {
        #region 参考人员

        /// <summary>
        /// 主键:考试编号
        /// </summary>
        public long LongTestNum { get; set; }

        /// <summary>
        /// 主键:考生编号
        /// </summary>
        public long LongTestUserNum { get; set; }

        /// <summary>
        /// 考生姓名
        /// </summary>
        public string StrTestUserName { get; set; }

        /// <summary>
        /// 试卷编号
        /// </summary>
        public long LongPaperNum { get; set; }

        /// <summary>
        /// 试卷名
        /// </summary>
        public string StrPaperName { get; set; }

        /// <summary>
        /// 考试分数
        /// </summary>
        public int IntScore { get; set; }

        /// <summary>
        /// 签卷开考时间 
        /// </summary>
        public string StrStartTime { get; set; }

        /// <summary>
        /// 签卷结束时间
        /// </summary>
        public string StrEndTime { get; set; }

        /// <summary>
        /// 考试状态
        /// </summary>
        public string StrTestStatue { get; set; }

        /// <summary>
        /// 考试状态
        /// </summary>
        public int IntEable { get; set; }

        #endregion
    }
}
