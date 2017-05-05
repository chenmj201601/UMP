using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService13
{
    public class CFilterCondition
    {
        /// <summary>
        /// c001 筛选策略编码 259.对应t_00_202.c003
        /// </summary>
        public string StrategyCode { get; set; }
        /// <summary>
        /// c002 筛选数据目标(1:录音记录)
        /// </summary>
        public string FilterTarget { get; set; }
        /// <summary>
        /// c003 序号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// c005 字段名
        /// </summary>
        public string ConditionName { get; set; }
        /// <summary>
        /// c006 比较方法（> = in 等）
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// c007 是否为t_00_203中的枚举值 1:是
        /// </summary>
        public string isEnum { get; set; }
        /// <summary>
        /// c008 比较值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// c009 下个条件的 and or
        /// </summary>
        public string Logical { get; set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        //public List<CEnumData> lstEnumdata { get; set; }
    }

    public class CCustomField
    {
        public string CustomName { get; set; } 
        public string DBColumn { get; set; }
    }
}
