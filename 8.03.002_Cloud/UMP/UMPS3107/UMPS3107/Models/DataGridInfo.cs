using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMPS3107.Models
{
    public class DataGridInfo
    {
        public int DurationMin { get; set; }
        public int DurationMax { get; set; }
        /// <summary>
        /// 分配比率，总额100%
        /// </summary>
        public double Rate { get; set; }
        /// <summary>
        /// 是否被删除
        /// </summary>
        //public bool IsDelete { get; set; }
    }
}
