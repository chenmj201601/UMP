using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common21011
{
    public class COperator
    {
        public string Show { get; set; }
        public string Description { get; set; }
    }

    public class CLogical
    {
        public int ID { get; set; }
        public string Show { get; set; }
        public string Description { get; set; }
    }

    public class CColumnInfo
    {
        public int ID { get; set; }
        public string Show { get; set; }
        public string Description { get; set; }
    }

    public class CStrategyType
    {
        public int ID { get; set; }
        public string Show { get; set; }
    }

    /// <summary>
    /// 203
    /// </summary>
    public class CEnumData
    {
        /// <summary>
        /// c001 枚举记录值id, 922
        /// </summary>
        public long StrategyCode { get; set; }
        /// <summary>
        /// c003 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 204
    /// </summary>
    public class CFilterCondition
    {
        /// <summary>
        /// c001 筛选策略编码 257.对应t_00_202.c003
        /// </summary>
        public long StrategyCode { get; set; }
        /// <summary>
        /// c002 筛选数据目标(1:录音记录)
        /// </summary>
        public int FilterTarget { get; set; }
        /// <summary>
        /// c003 序号
        /// </summary>
        public int ID { get; set; }
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
        public List<CEnumData> lstEnumdata { get; set; }
    }

    /// <summary>
    /// 202
    /// </summary>
    public class AllFilterData
    {
        /// <summary>
        /// c001 筛选类型 
        /// </summary>
        public int FilterType { get; set; }
        /// <summary>
        /// c002 1:归档策略；2:备份策略; 3:回删策略; 
        /// </summary>
        public string StrategyType { get; set; }
        /// <summary>
        /// c003 筛选策略编码 257
        /// </summary>
        public long StrategyCode { get; set; }
        /// <summary>
        /// c004 筛选策略名称,加密保存
        /// </summary>
        public string StrategyName { get; set; }
        /// <summary>
        /// c005 是否启用 1启用  0禁用
        /// </summary>
        public string IsValid { get; set; }
        /// <summary>
        /// c005 是否启用 启用  禁用
        /// </summary>
        public string StrIsValid { get; set; }
        /// <summary>
        /// c006 是否删除
        /// </summary>
        public string IsDelete { get; set; }
        /// <summary>
        /// c007 创建人
        /// </summary>
        public long Creator { get; set; }
        /// <summary>
        /// c008 创建时间utc,yyyymmddhh24mmss
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// c009 筛选条数
        /// </summary>
        public int FilterNumber { get; set; }
        /// <summary>
        /// c010 筛选有效时间 开始 utc yyyymmddhh24mmss
        /// </summary>
        public string DateStart { get; set; }
        /// <summary>
        /// c011 筛选有效时间 结束 utc yyyymmddhh24mmss
        /// </summary>
        public string DateEnd { get; set; }
        /// <summary>
        /// c012 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 创建人（显示用）
        /// </summary>
        public string CreatorName { get; set; }

        public List<CFilterCondition> listFilterCondition { get; set; }

    }
}
