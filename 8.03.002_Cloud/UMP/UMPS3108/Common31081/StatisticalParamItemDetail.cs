using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    /// <summary>
    /// 参数子项详情  对应  T_31_044
    /// </summary>
    public class StatisticalParamItemDetail
    {
        /// <summary>
        /// 参数ID,对应T_31_044 的 C001 或者 T_31_051 的 C010
        /// 此项不能为0,必须是19位的ID,这样才能写入这个表
        /// </summary>
        public long StatisticalParamID { get; set; }
        /// <summary>
        /// 参数子项ID,对应T_31_044 的 C001 或者 T_31_051 的 C001
        /// </summary>
        public long StatisticalParamItemID { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsUsed { get; set; }
        /// <summary>
        /// 值类型  1,正常  2,平均  3,标准差
        /// </summary>
        public string ValueType { get; set; }
        /// <summary>
        /// 此条件的值 Value1写入的值放在T_31_044  
        /// </summary>
        public string Value1 { get; set; }
        /// <summary>
        /// Value2写入的值是统计时间的数值,放入T_31_051的C012
        /// </summary>
        public string Value2 { get; set; }
        /// <summary>
        /// Value3写入的值是统计时间的单位 放入T_31_051的C013
        /// </summary>
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
    }
}
