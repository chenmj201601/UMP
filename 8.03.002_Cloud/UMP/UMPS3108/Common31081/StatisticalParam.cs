using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    /// <summary>
    /// 统计参数信息,从T_31_050中读取
    /// </summary>
    public class StatisticalParam
    {
        /// <summary>
        /// 统计分析参数ID
        /// </summary>
        public long StatisticalParamID { get; set; }
        /// <summary>
        /// 统计分析参数名
        /// </summary>
        public string StatisticalParamName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsUsed { get; set; }
        /// <summary>
        /// 是否能组合
        /// </summary>
        public string IsCombine { get; set; }
        /// <summary>
        /// 统计参数在界面上显示的东西,和语言包相关
        /// </summary>
        public string Description { get; set; }
       
    }
}
