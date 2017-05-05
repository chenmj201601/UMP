using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService08
{
    //统计项对应的ID
     public class UMPService08Const
     {

         #region//表和列
         public const string Const_TableName_Record = "T_21_001";
         public const string Const_TableName_RecordStatistics = "T_31_054";
         public const string Const_TableName_ScoreStatistics = "T_31_055";
         public const string Const_ColumnName_LPRecord = "LP_21_001.C002";
         #endregion

         #region 资源
         public const int Const_Source_Rent=100;
         public const int Const_Source_Org=101;
         public const int Const_Source_User=102;
         public const int Const_Source_Agent=103;
         public const int Const_Source_Extension=104;
         public const int Const_Source_Role=106;
         public const int  Const_Source_StatisticsID = 311;
         public const int Const_Source_StatisticsItem = 314;
        #endregion


        #region
         public const string Const_Week_Config = "12010101";
         public const string Const_Month_Config = "12010102";

         public const string Const_Agent_Extension = "12010401";
        #endregion

         #region 统计子项ID

         //冲突时长
         public const long Const_Item_CollisionDuration = 3140000000000000001;
         //冲突时长百分比
         public const long Const_Item_CollisionPercent = 3140000000000000002;
         //Hold时长
         public const long Const_Item_HoldDuration = 3140000000000000003;
         //Hold时长占百分比
         public const long Const_Item_HoldPercent = 3140000000000000004;
         //Hold次数
         public const long Const_Item_HoldTimes = 3140000000000000005;
         //转接次数
         public const long Const_Item_TransferTimes = 3140000000000000006;
         //是否座席先挂机
         public const long Const_Item_IsAgentHanged = 3140000000000000007;
         //事后处理时长
         public const long Const_Item_AfterDealDurationSec = 3140000000000000008;
         //事后处理时长大于平均处理时长多少(s)
         public const long Const_Item_AfDeDurMoreAvaDeDurSec = 3140000000000000009;
         //通话时长比较(平均值)
         public const long Const_Item_CallDurationCompareAva = 3140000000000000010;
         //通话时长比较(%)
         public const long Const_Item_CallDurationComparePec = 3140000000000000011;
         //重复呼入
         public const long Const_Item_RepeatedCallinTimes = 3140000000000000012;
         //呼叫高峰
         public const long Const_Item_CallPeak= 3140000000000000013;
         //坐席/客人讲话时长比例异常
         public const long Const_Item_ACSpeExceptProportion= 3140000000000000014;
         // 录音时长异常 
         public const long Const_Item_RecordDurationExcept = 3140000000000000015;
         //事后处理时长异常
         public const long Const_Item_AfterDealDurationExcept= 3140000000000000016;
         //异常分数
         public const long Const_Item_ExceptionScore = 3140000000000000017;
        #endregion


        #region 统计大项ID

         //服务态度
         public const long Const_ServiceAttitude=3110000000000000001;
         //专业水平
         public const long Const_ProfessionalLevel=3110000000000000002;
         //重复呼入
         public const long Const_RepeatedCallin=3110000000000000003;
         //呼叫高峰期
         public const long Const_CallPeak=3110000000000000004;
         //座席/客户讲话时长比例异常
         public const long Const_ACSpeExceptProportion=3110000000000000005;
         //录音时长异常
         public const long Const_RecordDurationExcept=3110000000000000006;
         //事后处理时长异常
         public const long Const_AfterDealDurationExcept=3110000000000000007;
         //异常分数
         public const long Const_ExceptionScore = 3110000000000000008;
        #endregion


        #region abcd大项写值录音统计表和评分统计表
         public const string Const_Statistics_Fill = "1";
         public const string Const_Statistics_NotFill = "2";


         public const string Const_Sta_CallPeak_Hign = "1";
         public const string Const_Sta_CallPeak_Flat = "2";
         public const string Const_Sta_CallPeak_Low = "3";

        #endregion

        #region
         //表示统计数据写入到统计录音表
         public const int Const_Statistics_MarkTable_Record = 1;
         //表统计数据写放到统计成绩表
         public const int Const_Statistics_MarkTable_Score = 2;
        #endregion
    }
}
