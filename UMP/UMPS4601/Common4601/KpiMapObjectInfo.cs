using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common4601
{
    /// <summary>
    /// 主要是T_46_003表的数据，还有T_46_001 和T_46_002的数据
    /// </summary>
    public class KpiMapObjectInfo
    {
        /// <summary>
        /// T_46_003 C001 主键ID 
        /// </summary>
        public string KpiMappingID { get; set; }
        /// <summary>
        /// T_46_003 C002 Kpi的ID 对应T_46_001的C001
        /// </summary>
        public string KpiID { get; set; }
        /// <summary>
        /// Kpi名字 T_46_001的C002字段
        /// </summary>
        public string KpiName { get; set; }
        /// <summary>
        /// 绑定对象的ID t_46_003的C003字段
        /// </summary>
        public string ObjectID { get; set; }
        /// <summary>
        /// 实际应用对象的类型 T_46_003的C004
        /// </summary>
        public string ApplyType { get; set; }
        /// <summary>
        /// 应用周期 T_46_003的C005
        /// </summary>
        public string ApplyCycle { get; set; }
        /// <summary>
        /// 启用/禁用 T_46_003的C006
        /// </summary>
        public string IsActive { get; set; }
        /// <summary>
        /// 开始启用时间 T_46_003的C007
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 启用的结束时间 T_46_003的C008
        /// </summary>
        public string StopTime { get; set; }
        /// <summary>
        /// 平行还是向下钻取 T_46_003的C009
        /// </summary>
        public string DropDown { get; set; }
        /// <summary>
        /// 是否应用到全部 T_46_003的C010
        /// </summary>
        public string ApplyAll { get; set; }
        /// <summary>
        /// 添加人ID 
        /// </summary>
        public string AdderID { get; set; }
        /// <summary>
        /// 添加人的名字 T_46_003的C012是添加人的ID 根据ID去用户表拿添加人名字
        /// </summary>
        public string AdderName { get; set; }
        /// <summary>
        /// 添加时间 T_46_003的C013
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 是否启用目标1 T_46_003的C014
        /// </summary>
        public string IsStartGoal1 { get; set; }
        /// <summary>
        /// 目标1 T_46_003的C015
        /// </summary>
        public string GoldValue1 { get; set; }
        /// <summary>
        /// 目标1的比较符 T_46_003的C016
        /// </summary>
        public string GoalOperation1 { get; set; }
        /// <summary>
        /// 是否启用多区段 T_46_003的C017
        /// </summary>
        public string IsStartMultiRegion1 { get; set; }
        /// <summary>
        /// 是否启用目标2 T_46_003的C018
        /// </summary>
        public string IsStartGoal2 { get; set; }
        /// <summary>
        /// 目标2 T_46_003的C019
        /// </summary>
        public string GoldValue2 { get; set; }
        /// <summary>
        /// 目标2的比较符 T_46_003的C020
        /// </summary>
        public string GoalOperation2 { get; set; }
        /// <summary>
        /// 是否启用多区段 T_46_003的C021
        /// </summary>
        public string IsStartMultiRegion2 { get; set; }
        /// <summary>
        /// 是否启用目标3 T_46_003的C022
        /// </summary>
        public string IsStartGoal3 { get; set; }
        /// <summary>
        /// 目标3 T_46_003的C023
        /// </summary>
        public string GoldValue3 { get; set; }
        /// <summary>
        /// 目标3的比较符 T_46_003的C024
        /// </summary>
        public string GoalOperation3 { get; set; }
        /// <summary>
        /// 是否启用多区段 T_46_003的C025
        /// </summary>
        public string IsStartMultiRegion3 { get; set; }
        /// <summary>
        /// 属于哪个机构或者技能组  [名称] T_46_003的C026是ID，这里是名字
        /// </summary>
        public string BelongOrgSkg { get; set; }

        /// <summary>
        /// 属于单位的名字
        /// </summary>
        public string BelongOrgSkgName { get; set; }

        public KpiMapObjectInfo CloneMumber()
        {
            return (KpiMapObjectInfo)MemberwiseClone();
        }

    }
}
