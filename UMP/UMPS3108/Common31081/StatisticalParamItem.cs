using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    /// <summary>
    /// 统计参数信息,从T_31_051中读取
    /// </summary>
    public class StatisticalParamItem
    {
        /// <summary>
        /// 统计分析参数子项ID
        /// </summary>
        public long StatisticalParamItemID { get; set; }
        /// <summary>
        /// 统计分析参数名
        /// </summary>
        public string StatisticalParamItemName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUsed { get; set; }
        /// <summary>
        /// 是否能组合
        /// </summary>
        public string IsCombine { get; set; }
        /// <summary>
        /// 统计参数在界面上显示的东西,和语言包相关
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public CombStatiParaItemsFormat Formart { get; set; }
       
        /// <summary>
        /// 统计参数的类型
        /// </summary>
        public StatisticalParamItemType Type { get; set; }
       
        /// <summary>
        /// 统计分析ID (如果是0，则为可供添加的条件)
        /// </summary>
        public long StatisticalParamID { get; set; }
        /// <summary>
        /// tabitem索引,可组合的统计条件大项---这个对应的是服务态度(0)和专业水平(1)
        /// </summary>
        public int TabIndex { get; set; }
        /// <summary>
        /// tabitem名称,可组合的统计条件大项---服务态度和专业水平
        /// </summary>
        public string TabName { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 值类型  1,正常  2,平均  3,标准差  对应T_31_051 的C001字段
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// 统计时长
        /// </summary>
        public string StatisticDuration { get; set; }
        
        /// <summary>
        /// 统计单位
        /// </summary>
        public int StatisticUnit { get; set; }

        /// <summary>
        /// 小项的值
        /// </summary>
        public string Value { get; set; }

        public StatisticalParamItem ConditionItem { get; set; }
    }
}
