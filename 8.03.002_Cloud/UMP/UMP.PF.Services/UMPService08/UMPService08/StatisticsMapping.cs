using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService08
{

    public class DateTimeSplite
    {
        public DateTime StartStatisticsTime { set; get; }
        public DateTime StopStatisticsTime { set; get; }

        public DateTime UpdateStartTime { set; get; }
        public DateTime UpdateStopTime { set; get; }

    }


    /// <summary>
    /// 统计配置存的值
    /// </summary>
    class StatisticsSetValue 
    {
        public long MappingIDOrStatisticsID{ set; get; }
        public long StatisticsItemID{ set; get; }
        public string IsStart{ set; get; }
        public string SetValue004{ set; get; }
        public string SetValue005{ set; get; }
        public string SetValue006{ set; get; }
        public string SetValue007{ set; get; }
        public string SetValue008{ set; get; }
    }

    /// <summary>
    /// 统计梆定到机构和部门
    /// </summary>
    class StatisticsMapping
    {
        public long StatisticsMappingID { set; get; }
        public long StatisticsID { set; get; }
        public long OrgIDOrSkillID { set; get; }
        public int DropDown { set; get; }
        public int ApplayAll { set; get; }

        public DateTime StartTime { set; get; }
        public DateTime StopTime { set; get; }
        public int MappingColumnID { set; get; }

        public long FrequencyID { set; get; }
        public int UpdateValueTime { set; get; }//更新数据的时间范围的值
        public int UpdateTimeUnit { set; get; }//更新数据的时间范围单位
    }


    /// <summary>
    /// 统计大项
    /// </summary>
    class StatisticsCatogary 
    {
        public long StatisticsID { set; get; }
        public string StatisticsName { set; get; }

        public int Active { set; get; }
        public int SystemType { set; get; } //系统自带1  手工添加2
        public int IsDelete { set; get; }
        public int ValueType { set; get; }
        public int IsCanCombin { set; get; }
        public int MarkTable { set; get; }

        public bool LogicPartTable { set; get; }
    }


    ///<summary>
    ///高峰类
    /// </summary>
    /// 
    class CallPeakSpliteObject 
    {
        public long ObjectID { set; get; }
        public long StatistcsValue { set; get; }
        public long SliceType { set; get; }
        public long SliceOrder { set; get; }
        public DateTime DateValue { set; get; }
    }


    /// <summary>
    /// 统计小项
    /// </summary>
    class StatisticsItem 
    {
        public long StatisticsItemID { set; get; }
        public string StatisticsItemName { set; get; }
        public int SourceTable { set; get; } //录音1  评分记录2
        public  int  IsCanCombine { set; get; }

        public int ShowType { set; get; }//类型(1:普通文本 2复选框 3:逗号间隔多值文本 4:下拉枚举 5:多选 6:其他)

        public int OrderID { set; get; }
        public int SystemType { set; get; }//系统自带1  手工添加2
        public long StatisticsID { set; get; }
        public int IsAVGOrStandarDev { set; get; } //1、正常 2、平均  3、标准差

        public int SliceTimeValue { set; get; }
        public int SliceTimeUnit { set; get; }//时间单位1、年，2、月 ， 3、周，4、天，5、小时,6、分钟
    }

    /// <summary>
    /// 运行周期
    /// </summary>
    class RunFrequence 
    {
        public long FrequencyID{set;get;}
        public string RunFreq{set;get;}
        public string StrDayTime{set;get;}
        public int  DayOfWeek {set;get;} 
        public int  DayOfMonth {set;get;}
        public int DayOfYear { set; get; }
        public DateTime LastRunTime { set; get; }
    }

    /// <summary>
    /// 技能组信息
    /// </summary>
    class SkillInfo 
    {
        public long SkillID { set; get; }
        public int OrderId { set; get; }
        public string SkillName { set; get; }

    }

    public class ObjInfo
    {
        public long ObjID { set; get; }
        public String ObjName { set; get; }
        public long UTCTime { get; set; }
        public long LocalTime { set; get; }
        public double ObjValue00 { set; get; }//用户求平均值时用
        public double ObjValue01 { set; get; }
        public double ObjValue02 { set; get; }
        public double ObjValue03 { set; get; }
        public double ObjValue04 { set; get; }
        public double ObjValue05 { set; get; }
        public double ObjValue06 { set; get; }
        public double ObjValue07 { set; get; }
        public double ObjValue08 { set; get; }
        public double ObjValue09 { set; get; }
        public double ObjValue10 { set; get; }
        //public DateTime StartTime { set; get; }
        //public DateTime UTCStartTime { set; get; }
        public bool ObjIsFit { set; get; }
        public int ObjFitIntMark { set; get; }
    }


    /// <summary>
    /// 机构信息
    /// </summary>
    class OrgInfo:ObjInfo
    {
        public long ParentOrgID { set; get; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    class UserInfo : ObjInfo
    {
        public long BeyondOrgID { set; get; }
    }

    class AgentInfo : ObjInfo 
    {
        public long BeyondOrgID { set; get; }
        public long BeyondSkillID { set; get; }
    }


}
