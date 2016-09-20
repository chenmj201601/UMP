using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService05
{
    public class UserTasksInfoShow
    {
        /// <summary>
        /// C001 主键自增，任务号
        /// </summary>
        public long TaskID { get; set; }
        /// <summary>
        /// C002 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// C003 任务描述
        /// </summary>
        public string TaskDesc { get; set; }
        /// <summary>
        /// C004 任务类别  1 初检手任务 2初检自动任务，3复检手动任务 4复检自动任务 5推荐录音初检 6推荐录音复检  7 QA质检任务（质检但不计入座席成绩） 8智能任务分配
        /// </summary>
        public int TaskType { get; set; }
        /// <summary>
        /// 任务类别  1 初检手任务 2初检自动任务，3复检手动任务 4复检自动任务 5推荐录音初检 6推荐录音复检  7 QA质检任务（质检但不计入座席成绩） 8智能任务分配
        /// </summary>
        public string TaskTypeName { get; set; }
        /// <summary>
        /// C005 是否共享任务 Y共享任务  N非共享任务
        /// </summary>
        public string IsShare { get; set; }
        /// <summary>
        /// C007 分配人id ,对应 t_11_034_bu.userid
        /// </summary>
        public long AssignUser { get; set; }
        /// <summary>
        /// C008 分配数量
        /// </summary>
        public int AssignNum { get; set; }
        /// <summary>
        /// C010 完成数量
        /// </summary>
        public int AlreadyScoreNum { get; set; }
        /// <summary>
        /// C012 最后一次修改任务的人员ID,对应 T_11_034_BU.UserID
        /// </summary>
        public long? ModifyUser { get; set; }
        /// <summary>
        /// C013 日期还有多少天到期发通知
        /// </summary>
        public int RemindDayTime { get; set; }
        /// <summary>
        /// C014 通知人
        /// </summary>
        public string ReminderIDs { get; set; }
        /// <summary>
        /// C015 任务所属年
        /// </summary>
        public int BelongYear { get; set; }
        /// <summary>
        /// C016 任务所属月
        /// </summary>
        public int BelongMonth { get; set; }
        /// <summary>
        /// C017 Y完成 N未完成
        /// </summary>
        public string IsFinish { get; set; }
        /// <summary>
        /// C019 分配人全名
        /// </summary>
        public string AssignUserFName { get; set; }
        /// <summary>
        /// C020 最后修改人人全名
        /// </summary>
        public string ModifyUserFName { get; set; }
        /// <summary>
        /// FinishUserID任务完成人ID 对应021表的C002
        /// </summary>
        public long FinishUserID { get; set; }
        /// <summary>
        /// FinishUserFName 任务完成人全名 对应021表的C003
        /// </summary>
        public string FinishUserFName { get; set; }

        /// <summary>
        /// C021 任务录音总时长（s）
        /// </summary>
        public long TaskAllRecordLength { get; set; }

        /// <summary>
        /// C021 任务录音总时长 00:00:00
        /// </summary>
        public string strTaskAllRecordLength { get; set; }

        /// <summary>
        /// C022 任务已评分录音总时长（s）
        /// </summary>
        public long TaskFinishRecordLength { get; set; }
        /// <summary>
        /// C006 分配时间
        /// </summary>
        public string AssignTime { get; set; }
        /// <summary>
        /// C018 任务完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// C011 最后一次修改任务的时间
        /// </summary>
        public string ModifyTime { get; set; }
        /// <summary>
        /// C009 完成时间
        /// </summary>
        public string DealLine { get; set; }

    }
}
