using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    //对应T_46_004
    public class KPIFormulaColumn
    {
        public long FormulaCharID { set; get; }
        public string ColumnName { set; get; }
        
        public int ColumnSource { set; get; }//字段来源 1 表 2 存储过程 3常量C004
        public string ApplayName { set; get; }//表名，存储过程名 , 常量值C005
        public int DataType { set; get; }//表内的数据列区分标志 1 无视周期型数据列 2 特定周期型数据C006


        public string ApplyCycle { set; get; }//应用周期
        public int SpecialObjectTypeNumber { set; get; }        //当统计分钟切片数据（切片数据为方案2时对应的FieldNameType如：录音统计结果表_P_方案2.FieldNameType 注:如切片数据为方案2 则要用到此字段，因为分钟切片表此时字段与天以上单位切片不是一个字段）





    }
}
