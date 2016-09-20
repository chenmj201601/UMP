using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{

    /// <summary>
    /// 查询条件子项，当查询条件项的结果是多值型时，其多个值保存在子项中
    /// </summary>
    public class QueryConditionSubItem
    {
        public long ConditionItemID { get; set; }
        public int Number { get; set; }
        public string Value01 { get; set; }
        public string Value02 { get; set; }
        public string Value03 { get; set; }
        public string Value04 { get; set; }
        public string Value05 { get; set; }
    }
}
