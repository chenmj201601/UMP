using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService05
{
    public class TaskInfoDetail
    {
        /// <summary>
        /// C001 任务号 对应 T_31_020_00000.C001
        /// </summary>
        public long TaskID { get; set; }
        /// <summary>
        /// C002 录音流水号,对应 对应T_21_000.RecoredReference
        /// </summary>
        public long RecoredReference { get; set; }
        /// <summary>
        /// C003 Y被锁定（当任务为共享时）, N没被锁定
        /// </summary>
        public string IsLock { get; set; }
        /// <summary>
        /// C004 锁定人,对应 T_11_005_00000.C001
        /// </summary>
        public long? UserID { get; set; }
        /// <summary>
        /// C005 锁定时间
        /// </summary>
        public string LockTime { get; set; }
        /// <summary>
        /// C006 1任务分配过来，2从其它任务移动过来的 3推荐录音
        /// </summary>
        public int? AllotType { get; set; }
        /// <summary>
        /// 1任务分配过来，2从其它任务移动过来的 3推荐录音
        /// </summary>
        public string AllotTypeName { get; set; }
        /// <summary>
        /// C007 如果该录音从其它任务调整过来的，为调整任务ID
        /// </summary>
        public long? FromTaskID { get; set; }
        /// <summary>
        /// C008 如果该录音从其它任务调整过来的，为调整任务名称
        /// </summary>
        public string FromTaskName { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// C009 用户全名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// C010 任务分数
        /// </summary>
        public double? TaskScore { get; set; }
        /// <summary>
        /// 录音所属的座席id或者分机id 
        /// </summary>
        public string AgtOrExtID { get; set; }
        /// <summary>
        /// 录音对应的座席工号或者分机号 
        /// </summary>
        public string AgtOrExtName { get; set; }
        /// <summary>
        /// 錄音對應的評分表ID
        /// </summary>
        public long? TemplateID { get; set; }
        /// <summary>
        /// 錄音對應的任務類型
        /// 任务类别  1 初检手任务 2初检自动任务，3复检手动任务 4复检自动任务 5推荐录音初检 6推荐录音复检  7 QA质检任务（质检但不计入座席成绩） 8智能任务分配
        /// --by waves
        /// </summary>
        public string TaskType { get; set; }

        public string Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string strDirection { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string Extension { get; set; }
        public DateTime StartRecordTime { get; set; }
        public string AgentFullName { get; set; }
    }
}
