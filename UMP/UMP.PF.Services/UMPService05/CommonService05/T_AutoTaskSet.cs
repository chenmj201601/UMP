using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService05
{
    /// <summary>
    /// 自动任务设置
    /// </summary>
    public class T_AutoTaskSet
    {
        /// <summary>
        /// C001 主键自增,自动任务分配ID
        /// </summary>
        public long AutoTaskID { get; set; }
        /// <summary>
        /// C002 1自动初检任务 2自动复检任务 3智能任务分配（新任务夹）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// C003 启用/禁用  Y/N
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// C004 创建人 UserID 
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// C005 创建人全名
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// C006 是否共享任务 Y共享任务  N非共享任务
        /// </summary>
        public string IsShare { get; set; }
        /// <summary>
        /// C007 是否平均分配 Y平均分配  N 多人发送的一样的录音
        /// </summary>
        public string IsAVGAssign { get; set; }
        /// <summary>
        /// C008 平均分配不完的录音处理办法  Y丢弃 N随机平均到质检任务里
        /// </summary>
        public string IsDropExtendRecord { get; set; }
        /// <summary>
        /// C009 质检人员ID,以逗号隔开,当为任务共享时，可以没质检人员
        /// </summary>
        public string QMIDsOne { get; set; }
        /// <summary>
        /// C010 质检人员ID,以逗号隔开
        /// </summary>
        public string QMIDsTwo { get; set; }
        /// <summary>
        /// C011 质检人员ID,以逗号隔开
        /// </summary>
        public string QMIDsThree { get; set; }
        /// <summary>
        /// C012 创建时该 创建人所属部门ID
        /// </summary>
        public string OrgTenantID { get; set; }
        /// <summary>
        /// C013 查询参数ID,对应 T_31_024_QueryParam.QueryID
        /// </summary>
        public string QueryID { get; set; }
        /// <summary>
        /// C014 运行周期ID,对应T_31_026_RunFrequency.FrequencyID
        /// </summary>
        public string FrequencyID { get; set; }
        /// <summary>
        /// C015 Y为旬设置 N非旬设置
        /// </summary>
        public string IsPeriod { get; set; }
        /// <summary>
        /// C016 Y为当上旬不足，中旬补齐 ，中旬不足时，下旬补齐 N为不补
        /// </summary>
        public string IsDownGet { get; set; }
        /// <summary>
        /// C017 任务名称
        /// </summary>
        public string AutoTaskSetName { get; set; }
        /// <summary>
        /// C018 任务描述
        /// </summary>
        public string AutoTaskSetDesc { get; set; }
        /// <summary>
        /// C019 当该任务为新任务夹时 ，用于两个不同任务夹记录，是否能有相同的记录 Y能，N不能
        /// </summary>
        public string IsTaskPacketShare { get; set; }
        /// <summary>
        /// C020 任务过期时间（天）   7：7天之后任务过期
        /// </summary>
        public int TaskDealLineDay { get; set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        public T_QueryParam CQueryParam { get; set; }
        /// <summary>
        /// 运行赔率设置
        /// </summary>
        public T_RunFrequency CRunFrequency { get; set; }
        /// <summary>
        /// 任务分配时长比率
        /// </summary>
        public List<T_AutoTaskRate> LstAutoTaskRate { get; set; }
        /// <summary>
        /// 最近一次分配时间
        /// </summary>
        public TaskAlloted CTaskAlloted { get; set; }

        /// <summary>
        /// 关键词条件
        /// </summary>
        public string KeyWordContentOne { get; set; }
        public string KeyWordContentTwo { get; set; }
        public string KeyWordContentThree { get; set; }


        /// <summary>
        /// 锁定评分表
        /// </summary>
        public long TemplateID { get; set; }
    }
}
