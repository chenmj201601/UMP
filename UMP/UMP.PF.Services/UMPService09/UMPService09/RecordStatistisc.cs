using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPService09.Utility;
using UMPService09.Utility.InterFace;
using UMPService09.Model;
using UMPService09.DAL;
using UMPService09.Log;
using System.Threading;

namespace UMPService09
{
    /// <summary>
    /// 录音的统计
    /// </summary>
    public class RecordStatistics : BasicMethod,IStatistics
    {
        private DataBaseConfig IDataBaseConfig;

        //服务统计末时间
        private ServiceConfigInfo IServiceConfigInfo;


        //全局参数
        private GlobalSetting IGlobalSetting;

        #region  //初始化切片数据时用
        //分钟切片类
        private DataFirstStatisticsSlice IDataFirstStatisticsSlice;

        //天切片
        #endregion

        /// <summary>
        /// 实际的统计
        /// </summary>
        public void DoAction(DataBaseConfig ADataBaseConfig,ServiceConfigInfo AServiceConfigInfo,GlobalSetting AGolbalSetting) 
        {
            this.IDataBaseConfig = ADataBaseConfig;
            this.IServiceConfigInfo = AServiceConfigInfo;
            this.IGlobalSetting = AGolbalSetting;

            //得到全部的座席
            List<ObjectInfo> lstAgentInfo = new List<ObjectInfo>();
            DALAgentInfo.GetAllAgentInfo(IDataBaseConfig, ref lstAgentInfo, IGlobalSetting);
            foreach (ObjectInfo agent in lstAgentInfo)
            {
                DoTimeSplitStatistics(agent);
                Thread.Sleep(10);
            }

            //得到全部的分机
            lstAgentInfo.Clear();
            DALExtensionInfo.GetAllExtensionInfo(IDataBaseConfig, ref lstAgentInfo, IGlobalSetting);
            foreach(ObjectInfo extension in lstAgentInfo)
            {
                DoTimeSplitStatistics(extension);
                Thread.Sleep(10);
            }           
        }

        //按时间分开
        private void DoTimeSplitStatistics(ObjectInfo Aobject)
        {
            DateTime EndTime = DateTime.Today;
            DateTime StartTime= IServiceConfigInfo.StartTime.Date;
            DateTimeSplite LDateTimeSpliteTemp;
            while(StartTime<EndTime)
            {
                DateTime TempStartTime = StartTime;
                DateTime TempStopTime= StartTime.AddMinutes(5);
                for (int i = 1; i <= 288;i++ )
                {
                    //5分钟累加
                    DoRealStatistics(TempStartTime, TempStopTime, Aobject, EnumSliceType.MiniteSlice,i);
                   
                    TempStartTime=TempStartTime.AddMinutes(5);
                    TempStopTime=TempStopTime.AddMinutes(5);
                }
                ///更新天的数据
                DoRealStatistics(StartTime, StartTime.AddDays(1), Aobject, EnumSliceType.DaySlice);
               

                //更新周的数据
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp = GetWeekStartAndStopTime(StartTime, IGlobalSetting);
                DoRealStatistics(LDateTimeSpliteTemp.StartStatisticsTime, LDateTimeSpliteTemp.StopStatisticsTime, Aobject, EnumSliceType.WeekSlice);
              

                //更新月的数据
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp = GetMonthStartAndStopTime(StartTime, IGlobalSetting);
                DoRealStatistics(LDateTimeSpliteTemp.StartStatisticsTime, LDateTimeSpliteTemp.StopStatisticsTime, Aobject, EnumSliceType.MonthSlice);
               

                //更新年的数据
                DoRealStatistics(new DateTime(StartTime.Year,1,1),new DateTime(StartTime.Year+1,1,1), Aobject, EnumSliceType.YearSlice);
                
               
                StartTime=StartTime.AddDays(1);
            }
        }

        //按对象 按功能分
        private void DoRealStatistics(DateTime AStarttime,DateTime AStoptime,ObjectInfo AObjectInfo,EnumSliceType ASliceType,int AIntOrderId=-1)
        {
            List<DateTimeSplite> LListDateTimeSplite;
            foreach (int  item in Enum.GetValues(typeof(EnumRecordFunction)))
            {
                IDataFirstStatisticsSlice = new DataFirstStatisticsSlice();
                InitDataStatistics(ASliceType, item, AObjectInfo, AStarttime);
                 
                //得到统计的值
                 LListDateTimeSplite = new List<DateTimeSplite>();
                 switch (ASliceType)
                 {
                     case EnumSliceType.MiniteSlice:
                         {
                             if (IGlobalSetting.IlogicPartMark == 1)//按月分区了
                             {
                                 GetSpliteTime(AStarttime, AStoptime, ref LListDateTimeSplite);
                                 foreach (DateTimeSplite dsplite in LListDateTimeSplite)
                                 {
                                     IDataFirstStatisticsSlice.Value01 += DALRecordInfo.GetAllRecordStatisticsInfo(IDataBaseConfig, AObjectInfo, IGlobalSetting, dsplite.StartStatisticsTime, dsplite.StopStatisticsTime, item);
                                 }
                             }
                             else
                             {
                                 IDataFirstStatisticsSlice.Value01 = DALRecordInfo.GetAllRecordStatisticsInfo(IDataBaseConfig, AObjectInfo, IGlobalSetting, AStarttime, AStoptime, item);
                             }
                         }
                         break;
                     case EnumSliceType.DaySlice:
                         {
                             IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsRecord.GetDayRecordStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMdd000000"), 0), LongParse(AStoptime.ToString("yyyyMMdd000000"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                         }
                         break;
                     case EnumSliceType.WeekSlice:
                         {
                             IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsRecord.GetWeekRecordStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMdd000000"), 0), LongParse(AStoptime.ToString("yyyyMMdd000000"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                         }
                         break;
                     case EnumSliceType.MonthSlice:
                         {
                             IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsRecord.GetMonthRecordStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMdd000000"), 0), LongParse(AStoptime.ToString("yyyyMMdd000000"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                         }
                         break;
                     case EnumSliceType.YearSlice:
                         {
                             IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsRecord.GetYearRecordStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMdd000000"), 0), LongParse(AStoptime.ToString("yyyyMMdd000000"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                         }
                         break;
                     default:
                         break;
                 }

                //将数据写入数据库
                 switch (ASliceType)
                 {
                     case EnumSliceType.MiniteSlice:
                         {
                             IDataFirstStatisticsSlice.OrderID = AIntOrderId;
                             DALFirstStatisticsRecord.InsertRecordStatistics(IDataBaseConfig, IGlobalSetting, 1, IDataFirstStatisticsSlice);
                         }
                         break;
                     case EnumSliceType.DaySlice:
                         DALFirstStatisticsRecord.InsertRecordStatistics(IDataBaseConfig, IGlobalSetting, 2, IDataFirstStatisticsSlice);
                         break;
                     case EnumSliceType.WeekSlice:
                         DALFirstStatisticsRecord.InsertRecordStatistics(IDataBaseConfig, IGlobalSetting, 3, IDataFirstStatisticsSlice);
                         break;
                     case EnumSliceType.MonthSlice:
                         DALFirstStatisticsRecord.InsertRecordStatistics(IDataBaseConfig, IGlobalSetting, 4, IDataFirstStatisticsSlice);
                         break;
                     case EnumSliceType.YearSlice:
                         DALFirstStatisticsRecord.InsertRecordStatistics(IDataBaseConfig, IGlobalSetting, 5, IDataFirstStatisticsSlice);
                         break;
                     default:
                         break;
                 }
            }
        }


        //初始化写入数据库的对象
        private void InitDataStatistics(EnumSliceType ASliceType, int AFunctype, ObjectInfo AObjectInfo,DateTime AStarttime) 
        {
            IDataFirstStatisticsSlice.ObjectType = AObjectInfo.ObjType;
            IDataFirstStatisticsSlice.StrRent = IGlobalSetting.StrRent;
            IDataFirstStatisticsSlice.ObjectID = AObjectInfo.ObjID;
            switch (AFunctype)
            {
                case 1:
                    IDataFirstStatisticsSlice.RecordFunction = EnumRecordFunction.RecordLength;
                    break;
                case 2:
                    IDataFirstStatisticsSlice.RecordFunction = EnumRecordFunction.RecordNumber;
                    break;
                case 3:
                    IDataFirstStatisticsSlice.RecordFunction = EnumRecordFunction.RingTime;
                    break;
                case 4:
                    IDataFirstStatisticsSlice.RecordFunction = EnumRecordFunction.HoldTime;
                    break;
                default:
                    break;
            }
            IDataFirstStatisticsSlice.StartTimeUTC = LongParse(AStarttime.ToUniversalTime().ToString("yyyyMMdd000000"), 0);
            //以这个local时间为唯一值
            IDataFirstStatisticsSlice.StartTimeLocal = LongParse(AStarttime.ToString("yyyyMMdd000000"), 0);
            IDataFirstStatisticsSlice.UpdateTime = DateTime.Now;
            IDataFirstStatisticsSlice.Year = AStarttime.Year;
            IDataFirstStatisticsSlice.Month = AStarttime.Month;
            IDataFirstStatisticsSlice.Day = AStarttime.Day;

            IDataFirstStatisticsSlice.StatisticsType = ConstStatisticsType.RecordStaitics;
            IDataFirstStatisticsSlice.SliceType = ASliceType;
        }

        /// <summary>
        /// 拆分开始时间和结束时间的查询时间的list
        /// </summary>
        /// <param name="AStartTime">本地开始时间</param>
        /// <param name="AStopTime">本地结束时间</param>
        /// <param name="AListDateTimeSplite">UTC的查询时间的list</param>
        private void GetSpliteTime(DateTime AStartTime, DateTime AStopTime, ref List<DateTimeSplite> AListDateTimeSplite)
        {
            DateTime LDateTimeStartUTC = AStartTime.ToUniversalTime();
            DateTime LDateTimeStopUTC = AStopTime.ToUniversalTime();
            DateTimeSplite LDateTimeSpliteTemp;
            if (new DateTime(LDateTimeStartUTC.Year, LDateTimeStartUTC.Month, 1) != new DateTime(LDateTimeStopUTC.Year, LDateTimeStopUTC.Month, 1))
            {
                DateTime LDateTimeUTCTemp = new DateTime(LDateTimeStartUTC.Year, LDateTimeStartUTC.Month + 1, 1);
                while (LDateTimeUTCTemp <= LDateTimeStopUTC)
                {
                    LDateTimeSpliteTemp = new DateTimeSplite();
                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC.ToLocalTime();
                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeUTCTemp.ToLocalTime();

                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);

                    LDateTimeStartUTC = LDateTimeUTCTemp;
                    LDateTimeUTCTemp = LDateTimeUTCTemp.AddMonths(1);   
;
                }

                if (LDateTimeUTCTemp > LDateTimeStopUTC)
                {
                    LDateTimeSpliteTemp = new DateTimeSplite();
                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC.ToLocalTime();
                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeStopUTC.ToLocalTime();
                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                  
                }
            }
            else
            {
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC.ToLocalTime();
                LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeStopUTC.ToLocalTime();
                AListDateTimeSplite.Add(LDateTimeSpliteTemp);
            }
        }
    }
}
