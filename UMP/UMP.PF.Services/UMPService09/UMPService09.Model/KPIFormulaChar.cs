using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    //对应T_46_005
    public class KPIFormulaChar
    {
        public long KpiID { set; get; }//T_46_001.C001字段
        public long FormulaCharID { set; get; } //T_46_004.C001字段
        public string MappingChar { set; get; }//a~z这间的字符
    }
}
