using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common21011
{
    public class StrategyInfo
    {
        /// <summary>
        /// 筛选数据目标(1:录音记录)
        /// </summary>
        public int StrategyType { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public long ID { get; set; }
        public string FieldName { get; set; }
        /// <summary>
        /// 数据类型（1：smallint；2：int；3：bigint；4：number；11：char；12：nchar；13：varchar；14：nvarchar；21：datetime）
        /// </summary>
        public int FieldType { get; set; }
        /// <summary>
        /// 允许匹配的条件.中间用'|'分开.如 >|<|>=|in.所有字符全部大写
        /// </summary>
        public string AllowCondition { get; set; }
        /// <summary>
        /// T_21_001_00000.C001
        /// </summary>
        public string ConditionName { get; set; }

        public string Display { get; set; }

    }
}
