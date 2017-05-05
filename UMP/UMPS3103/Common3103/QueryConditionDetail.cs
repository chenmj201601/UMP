using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    /// <summary>
    /// 查询条件的详情
    /// </summary>
    public class QueryConditionDetail
    {
        public long ConditionItemID { get; set; }
        public bool IsEnable { get; set; }
        public bool IsLike { set; get; }
        public string Value01 { get; set; }
        public string Value02 { get; set; }
        public string Value03 { get; set; }
        public string Value04 { get; set; }
        public string Value05 { get; set; }
    }
}
