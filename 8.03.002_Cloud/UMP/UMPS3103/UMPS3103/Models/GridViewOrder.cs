using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS3103.Models
{
   public class GridViewOrder
    {
       /// <summary>
       /// 语言ID
       /// </summary>
        public string Lans { get; set; }
       /// <summary>
       /// 绑定类名
       /// </summary>
        public string cols { get; set; }
       /// <summary>
       /// 宽度
       /// </summary>
        public double widths { get; set; }
       /// <summary>
       /// 列名
       /// </summary>
        public string Header { get; set; }
    }
}
