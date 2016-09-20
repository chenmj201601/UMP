using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    public class TaskSettingItems
    {
        public int RowNumber { get; set; }

        #region 任务设置
        /// <summary>
        /// 自动任务分配ID
        /// </summary>
        public long TaskSettingID { get; set; }
        /// <summary>
        /// 查询参数ID,对应 T_31_024_QueryParam.QueryID
        /// </summary>
        public long QueryID{ get; set; }
        /// <summary>
        /// 查询参数表 名称
        /// </summary>
        public string QueryName { get; set; }
        /// <summary>
        /// 任务类型 1 自动初检任务 2自动复检任务 3智能任务分配（新任务夹）
        /// </summary>
        public int TaskType { get; set; }
        public string StrTaskType { get; set; }

        /// <summary>
        /// 启用/禁用  Y/N
        /// </summary>
        public string Status { get; set; }
        public string StrStatus { get; set; }

        /// <summary>
        /// 创建人ID，对应 T_11_034_BU.UserID 
        /// </summary>
        public long Creator { get; set; }
        /// <summary>
        /// 创建人全名
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 是否共享任务, Y共享任务  N非共享任务
        /// </summary>
        public string IsTaskShare { get; set; }
        /// <summary>
        /// 是否平均分配 ,Y平均分配  N 多人发送的一样的录音
        /// </summary>
        public string IsTaskAVGAssign { get; set; }
        /// <summary>
        /// 平均分配不完的录音处理办法  Y丢弃 N随机平均到质检任务里
        /// </summary>
        public string IsDisposed { get; set; }
        /// <summary>
        /// 质检员1
        /// </summary>
        public string QMIDOne { get; set; }
        /// <summary>
        /// 质检员2
        /// </summary>
        public string QMIDTwo { get; set; }
        /// <summary>
        /// 质检员3
        /// </summary>
        public string QMIDThree { get; set; }
        /// <summary>
        /// 创建时该创建人所属部门T_11_031_BOT.OrgTenantID
        /// </summary>
        public long OrgTenantID { get; set; }
        /// <summary>
        /// Y为旬设置 N非旬设置
        /// </summary>
        public string IsTaskPeriod { get; set; }
        /// <summary>
        /// Y为当上旬不足，中旬补齐 ，中旬不足时，下旬补齐 ; N为不补
        /// </summary>
        public string IsDownGet { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string AutoTaskName { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string AutoTaskDesc { get; set; }
        /// <summary>
        /// 当该任务为新任务夹时，用于两个不同任务夹记录，是否能有相同的记录。 Y能，N不能
        /// </summary>
        public string IsTaskPacketShare { get; set; }
        /// <summary>
        /// 任务过期时间
        /// </summary>
        public int TaskDeadline { get; set; }
        #endregion
        
        #region 运行周期
        /// <summary>
        /// 运行周期ID 
        /// </summary>
        public long FrequencyID { get; set; }
        /// <summary>
        /// 运行周期 D:天,W:周,P:旬,M:月,S:季 Y:年  O:只运行一次
        /// </summary>
        public string RunFreq { get; set; }
        public string StrRunFreq { get; set; }

        /// <summary>
        /// 运行时间(02:00:00)
        /// </summary>
        public string DayTime { get; set; }
        /// <summary>
        /// 每周的星期几运行 1~7
        /// </summary>
        public int DayOfWeek { get; set; }
        /// <summary>
        /// 每月的第几天运行1~31,那个月没有29，30，31则那个月的最后一天运行
        /// </summary>
        public int DayOfMonth { get; set; }
        /// <summary>
        /// 是否分别设置旬运行 Y旬分开设置 N统一旬设置
        /// </summary>
        public string IsUniteSetOfPeriod { get; set; }
        /// <summary>
        /// 统一设置的时间 1~11, 如果那个旬没有9，10，11则旬的最后一天运行
        /// </summary>
        public string UniteSetPeriod{ get; set; }
        /// <summary>
        ///上旬的第几天运行1~10
        /// </summary>
        public string DayOfFirstPeriod { get; set; }
        /// <summary>
        /// 中旬的第几天运行1~10
        /// </summary>
        public string DayOfSecondPeriod { get; set; }
        /// <summary>
        /// 下旬的第几天运行1~11，下旬没有9，10，11的，按最后一天运行
        /// </summary>
        public string DayOfThirdPeriod { get; set; }
        /// <summary>
        /// 是否分别设置旬运行 Y季分开设置 N统一季设置
        /// </summary>
        public string IsUniteSetOfSeason { get; set; }
        /// <summary>
        ///统一设置的时间 0~92, 如果那个季没有90，91，92，则取那个季的最后一天运行
        /// </summary>
        public int UniteSetSeason { get; set; }
        /// <summary>
        /// 第一季的第几天
        /// </summary>
        public int DayOfFirstSeaSon { get; set; }
        /// <summary>
        /// 第二季的第几天
        /// </summary>
        public int DayOfSecondSeason { get; set; }
        /// <summary>
        /// 第三季的第几天
        /// </summary>
        public int DayOfThirdSeason { get; set; }
        /// <summary>
        /// 第四季的第几天
        /// </summary>
        public int DayOfFourSeason { get; set; }
        /// <summary>
        /// 年的第几天运行（0~366）
        /// </summary>
        public int DayOfYear { get; set; }
        #endregion
    }
}
