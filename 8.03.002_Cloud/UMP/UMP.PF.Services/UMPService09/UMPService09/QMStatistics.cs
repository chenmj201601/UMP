using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UMPService09.DAL;
using UMPService09.Log;
using UMPService09.Model;
using UMPService09.Utility;
using UMPService09.Utility.InterFace;

namespace UMPService09
{
  
   /// <summary>
    ///  关于质检数据的切片类统计
   /// </summary>
    public class QMStatistics : BasicMethod, IStatistics
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

        public void DoAction(DataBaseConfig ADataBaseConfig, ServiceConfigInfo AServiceConfigInfo, GlobalSetting AGolbalSetting) 
        {
            try
            {
                //得到全局参数
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
                foreach (ObjectInfo extension in lstAgentInfo)
                {
                    DoTimeSplitStatistics(extension);
                    Thread.Sleep(10);
                }

                //得到全部的用户
                lstAgentInfo.Clear();
                DALUserInfo.GetAllUserInfo(IDataBaseConfig, IGlobalSetting, ref lstAgentInfo);
                foreach (ObjectInfo user in lstAgentInfo)
                {
                    DoTimeSplitStatistics(user);
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("QMStatistics().DoAction", ex.Message);
            }
        }

        //按时间分开
        private void DoTimeSplitStatistics(ObjectInfo Aobject)
        {
            DateTime EndTime = DateTime.Today;
            DateTime StartTime = IServiceConfigInfo.StartTime.Date;
            DateTimeSplite LDateTimeSpliteTemp;
            while (StartTime < EndTime)
            {
                DateTime TempStartTime = StartTime;
                DateTime TempStopTime = StartTime.AddDays(1);

                ///更新天的数据
                DoRealStatistics(StartTime, StartTime.AddDays(1), Aobject, EnumSliceType.DaySlice);

                ///更新周的数据
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp = GetWeekStartAndStopTime(StartTime, IGlobalSetting);
                DoRealStatistics(LDateTimeSpliteTemp.StartStatisticsTime, LDateTimeSpliteTemp.StopStatisticsTime, Aobject, EnumSliceType.WeekSlice);

                ///更新月的数据
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp = GetMonthStartAndStopTime(StartTime, IGlobalSetting);
                DoRealStatistics(LDateTimeSpliteTemp.StartStatisticsTime, LDateTimeSpliteTemp.StopStatisticsTime, Aobject, EnumSliceType.MonthSlice);

                ///更新年的数据
                DoRealStatistics(new DateTime(StartTime.Year, 1, 1), new DateTime(StartTime.Year + 1, 1, 1), Aobject, EnumSliceType.YearSlice);

                StartTime = StartTime.AddDays(1);
            }
        }

        /// <summary>
        /// 按对象 按功能分
        /// </summary>
        /// <param name="AStarttime">开始时间</param>
        /// <param name="AStoptime">截止时间</param>
        /// <param name="AObjectInfo">统计目标</param>
        /// <param name="ASliceType">切片类型</param>
        private void DoRealStatistics(DateTime AStarttime, DateTime AStoptime, ObjectInfo AObjectInfo, EnumSliceType ASliceType)
        {
            List<DateTimeSplite> LListDateTimeSplite;
            foreach (int item in Enum.GetValues(typeof(EnumQMFunction)))
            {
                //if (item != 8)
                //    continue;
                IDataFirstStatisticsSlice = new DataFirstStatisticsSlice();
                InitDataStatistics(ASliceType, item, AObjectInfo, AStarttime);
                if (AObjectInfo.ObjID >= CombinSourceCode(StatisticsConstDefine.Const_Source_Agent_Begin) && AObjectInfo.ObjID < CombinSourceCode(StatisticsConstDefine.Const_Source_Extension_Begin))
                {
                    IDataFirstStatisticsSlice.ObjectType = 2;
                }
                else if (AObjectInfo.ObjID >= CombinSourceCode(StatisticsConstDefine.Const_Source_Extension_Begin) && AObjectInfo.ObjID < CombinSourceCode(StatisticsConstDefine.Const_Source_TrueExtension_Begin))
                {
                    IDataFirstStatisticsSlice.ObjectType = 1;
                }
                else if (AObjectInfo.ObjID >= CombinSourceCode(StatisticsConstDefine.Const_Source_TrueExtension_Begin) && AObjectInfo.ObjID < CombinSourceCode(StatisticsConstDefine.Const_Source_Role_Begin))
                {
                    IDataFirstStatisticsSlice.ObjectType = 1;
                }
                //得到统计的值
                LListDateTimeSplite = new List<DateTimeSplite>();
                switch (ASliceType)
                {
                    case EnumSliceType.DaySlice:
                        {
                            if (IGlobalSetting.IlogicPartMark == 1)//按月分区了
                            {
                                GetSpliteTime(AStarttime, AStoptime, ref LListDateTimeSplite);
                                foreach (DateTimeSplite dsplite in LListDateTimeSplite)
                                {
                                    IDataFirstStatisticsSlice.Value01 += DALQMInfo.GetQMDayStatisticsInfo(IDataBaseConfig, AObjectInfo, IGlobalSetting, dsplite.StartStatisticsTime, dsplite.StopStatisticsTime, item, IDataFirstStatisticsSlice.ObjectType);
                                }
                            }
                            else
                            {
                                IDataFirstStatisticsSlice.Value01 = DALQMInfo.GetQMDayStatisticsInfo(IDataBaseConfig, AObjectInfo, IGlobalSetting, AStarttime, AStoptime, item, IDataFirstStatisticsSlice.ObjectType);
                            }
                            DALFirstStatisticsQM.InsertQMStatistics(IDataBaseConfig, IGlobalSetting, 2, IDataFirstStatisticsSlice);
                        }
                        break;
                    case EnumSliceType.WeekSlice:
                        {
                            IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsQM.GetWeekQMStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMddHHmmss"), 0), LongParse(AStoptime.ToString("yyyyMMddHHmmss"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                            DALFirstStatisticsQM.InsertQMStatistics(IDataBaseConfig, IGlobalSetting, 3, IDataFirstStatisticsSlice);
                        }
                        break;
                    case EnumSliceType.MonthSlice:
                        {
                            IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsQM.GetMonthQMStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMddHHmmss"), 0), LongParse(AStoptime.ToString("yyyyMMddHHmmss"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                            DALFirstStatisticsQM.InsertQMStatistics(IDataBaseConfig, IGlobalSetting, 4, IDataFirstStatisticsSlice);
                        }
                        break;
                    case EnumSliceType.YearSlice:
                        {
                            IDataFirstStatisticsSlice.Value01 = DALFirstStatisticsQM.GetYearQMStatistics(IDataBaseConfig, AObjectInfo, IGlobalSetting, LongParse(AStarttime.ToString("yyyyMMddHHmmss"), 0), LongParse(AStoptime.ToString("yyyyMMddHHmmss"), 0), item, IDataFirstStatisticsSlice.ObjectType);
                            DALFirstStatisticsQM.InsertQMStatistics(IDataBaseConfig, IGlobalSetting, 5, IDataFirstStatisticsSlice);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// 初始化写入数据库的对象
        /// </summary>
        /// <param name="ASliceType">切片类型</param>
        /// <param name="AFunctype">统计类型</param>
        /// <param name="AObjectInfo">坐席，分机</param>
        /// <param name="AStarttime"></param>
        private void InitDataStatistics(EnumSliceType ASliceType, int AFunctype, ObjectInfo AObjectInfo, DateTime AStarttime)
        {
            IDataFirstStatisticsSlice.ObjectType = AObjectInfo.ObjType;
            IDataFirstStatisticsSlice.StrRent = IGlobalSetting.StrRent;
            IDataFirstStatisticsSlice.ObjectID = AObjectInfo.ObjID;
            switch (AFunctype)
            {
                case 1:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.ScoreNumber;
                        IDataFirstStatisticsSlice.OrderID = 1;
                    }
                    break;
                case 2:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.AppealNumber;
                        IDataFirstStatisticsSlice.OrderID = 2;
                    }
                    break;
                case 3:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.AppealSucNum;
                        IDataFirstStatisticsSlice.OrderID = 3;
                    }
                    break;
                case 4:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.TotalScore;
                        IDataFirstStatisticsSlice.OrderID = 4;
                    }
                    break;
                case 5:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.QaBeAppealedNum;
                        IDataFirstStatisticsSlice.OrderID = 5;
                    }
                    break;
                case 6:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.QaScoreNum; 
                        IDataFirstStatisticsSlice.OrderID = 6;
                    }
                    break;
                case 7:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.QaTaskFinishedNum; 
                        IDataFirstStatisticsSlice.OrderID = 7;
                    }
                    break;
                case 8:
                    {
                        IDataFirstStatisticsSlice.QMFunction = EnumQMFunction.QaTaskReceivedNum;
                        IDataFirstStatisticsSlice.OrderID = 8;
                    }
                    break;
                default:
                    break;
            }
            IDataFirstStatisticsSlice.StartTimeUTC = LongParse(AStarttime.ToUniversalTime().ToString("yyyyMMddHHmmss"), 0);
            //以这个local时间为唯一值
            IDataFirstStatisticsSlice.StartTimeLocal = LongParse(AStarttime.ToString("yyyyMMddHHmmss"), 0);
            IDataFirstStatisticsSlice.UpdateTime = DateTime.Now;
            IDataFirstStatisticsSlice.Year = AStarttime.Year;
            IDataFirstStatisticsSlice.Month = AStarttime.Month;
            IDataFirstStatisticsSlice.Day = AStarttime.Day;

            IDataFirstStatisticsSlice.StatisticsType = ConstStatisticsType.QMStatistics;
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
