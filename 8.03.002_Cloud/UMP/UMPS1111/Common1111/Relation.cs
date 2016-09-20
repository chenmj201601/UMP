using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common1111
{
    public class Relation
    {
        /// <summary>
        /// 租户编号
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 资源编号
        /// </summary>
        public long ResourceID { get; set; }
        /// <summary>
        /// 资源开始使用时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 资源使用结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        public Relation()
        { }

        public override string ToString()
        {
            return "{0}:"+StartTime.ToString()+"-"+EndTime.ToString();
        }
    }
}
