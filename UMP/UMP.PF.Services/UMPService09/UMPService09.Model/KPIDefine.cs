using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
   public class KPIDefine
    {
       public long KpiID { set; get; }
       public string KpiName { set; get; }

       public string ApplyObject { set; get; }//表示可以应用对象 1座席 2应用于分机  3用户 4真实分机 5机构 6技能组 到时通过 1111111111方式来展示(当应用录音，成绩时，专写入成绩表和录音表)
       public int KpiType { set; get; }//1 质检类Kpi 2 录音类kpi  3 cti类kpi  4 acd类kpi 5 wfm类kpi  10其它类kpi


       public string IsSystemContain { set; get; }
       public string IsStart { set; get; }
       public string ValueType { set; get; }

       public string NewFormula { set; get; } //公式

    }
}
