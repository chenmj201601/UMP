using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFShareClassesS;
using UMPService09.Model;
using System.Text.RegularExpressions;


namespace UMPService09.Utility
{
    public class BasicMethod
    {
        public static string IStrSpliterChar { get { return AscCodeToChr(27); } }

        public static string IStrVerificationCode102
        { 
            get {return CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102); }
        }
        public static string IStrVerificationCode002 
        {
            get { return CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002); }
        }

        public BasicMethod()
        {
        }

        public static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        public static int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            int.TryParse(str, out outRet);

            return outRet;
        }

        public static long LongParse(string str, long defaultValue)
        {
            long outRet = defaultValue;
            long.TryParse(str, out outRet);

            return outRet;
        }

        public static double DoubleParse(string str, Double defaultValue)
        {
            double outRet = defaultValue;
            double.TryParse(str, out outRet);
            return outRet;
        }

        public static DateTime DateTimeParse(string str, DateTime defaultValue)
        {
            DateTime outRet = defaultValue;
            DateTime.TryParse(str, out outRet);
            return outRet;
        }

        public static decimal DecimalParse(string str, decimal defaultValue) 
        {
            decimal outRet = defaultValue;
            decimal.TryParse(str, out outRet);
            return outRet;
        }

        /// <summary>
        /// 将字符串转日期
        /// </summary>
        /// <param name="source">日期字符串</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string source)
        {
            if (source.Length < 14)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += source.Substring(8, 2) + ":";
            strTime += source.Substring(10, 2) + ":";
            strTime += source.Substring(12, 2);

            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }


        /// <summary>
        /// 创建加解密验证码
        /// </summary>
        /// <param name="AKeyIVID"></param>
        /// <returns></returns>
        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { return LStrReturn = string.Empty; }

            return LStrReturn;
        }


        /// <summary>
        ///得到该年该月的天数
        /// </summary>
        /// <param name="AYear">年</param>
        /// <param name="AMonth">月</param>
        /// <returns></returns>
        public static int GetMonthMaxDay(int AYear, int AMonth)
        {
            int maxday = 0;
            int month = AMonth;
            int year = AYear;
            if(year<=0 || month<=0 ||month>12)
            {
                return 0;
            }
            switch (month)
            {
                case 2:
                    {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                        {
                            maxday = 29;
                        }
                        else
                            maxday = 28;
                    }
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    maxday = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    maxday = 30;
                    break;

            }
            return maxday;
        }


        /// <summary>
        /// 根据SourceCode拼流水号起始值
        /// </summary>
        /// <param name="SouceCode"></param>
        /// <returns></returns>
        public static long CombinSourceCode(string SouceCode) 
        {
            long SourceNumber = 0;

            if(SouceCode.Length !=3)
            {
                return SourceNumber;

            }
            SourceNumber = LongParse(SouceCode + StatisticsConstDefine.Const_Source_EndTrim, 0);

            return SourceNumber;
        }

        /// <summary>
        /// 根据一个日期得到一个星期的开始时间和起始时间
        /// </summary>
        /// <param name="ADatetime"></param>
        /// <returns></returns>
        public static DateTimeSplite GetWeekStartAndStopTime(DateTime ADatetime,GlobalSetting AGlodbalSetting) 
        {
            DateTimeSplite LDateTimeSplite = new DateTimeSplite();

            int LIntWeekStartSet = IntParse(AGlodbalSetting.StrWeekStart, 0);//0为星期天，1为星期1，6为星期6
            int LIntWeek = 0;
            switch (ADatetime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    LIntWeek = 1;
                    break;
                case DayOfWeek.Tuesday:
                    LIntWeek = 2;
                    break;
                case DayOfWeek.Wednesday:
                    LIntWeek = 3;
                    break;
                case DayOfWeek.Thursday:
                    LIntWeek = 4;
                    break;
                case DayOfWeek.Friday:
                    LIntWeek = 5;
                    break;
                case DayOfWeek.Saturday:
                    LIntWeek = 6;
                    break;
                case DayOfWeek.Sunday:
                    LIntWeek = 0;
                    break;
                default:
                    break;
            }
            if (LIntWeekStartSet <= LIntWeek)
            {
                LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(LIntWeekStartSet - LIntWeek).AddDays(7);
                LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(-7);
            }
            else if (LIntWeekStartSet > LIntWeek)
            {
                LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(LIntWeekStartSet - LIntWeek);
                LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(-7);
            }

            return LDateTimeSplite;
        }  


        /// <summary>
        /// 根据一月得到一个月的开始时间和起始时间
        /// </summary>
        /// <param name="ADatetime"></param>
        /// <param name="AGlodbalSetting"></param>
        /// <returns></returns>
        public static DateTimeSplite GetMonthStartAndStopTime(DateTime ADatetime, GlobalSetting AGlodbalSetting) 
        {
            DateTimeSplite LDateTimeSplite = new DateTimeSplite();
            int LIntMonthStartSet = IntParse(AGlodbalSetting.StrMonthStart, 0);
            //1为1号 2为2号 26为26号
            DateTime LDateTimeTemp = new DateTime(ADatetime.Year, ADatetime.Month, LIntMonthStartSet);
            if (ADatetime < LDateTimeTemp)
            {
                LDateTimeSplite.StopStatisticsTime = LDateTimeTemp.AddMonths(1);
                LDateTimeSplite.StartStatisticsTime = LDateTimeTemp;
            }
            else
            {
                LDateTimeSplite.StartStatisticsTime =LDateTimeTemp;
                LDateTimeSplite.StopStatisticsTime = LDateTimeTemp.AddMonths(1);
            }
           
            return LDateTimeSplite;
        }

        /// <summary>
        /// 某时间在该年的天数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetDayInYear(DateTime dt)
        {
            int day_in_year = 0;
            for (int i = 1; i < dt.Month; i++)
            {
                day_in_year = day_in_year + DateTime.DaysInMonth(dt.Year, i);
            }

            day_in_year = day_in_year + dt.Day;
            return day_in_year;
        }


        /// <summary>
        /// 得到这一年这天所处的周的顺序
        /// </summary>
        /// <param name="AWeekStartDatetime">周的开始时间</param>
        /// <param name="AGlodbalSetting"></param>
        /// <returns></returns>
        public static int GetWeekOrder(DateTime AWeekStartDatetime, GlobalSetting AGlodbalSetting) 
        {
            int NumOrder = 0;
            //得到这一年的起始，以这一年最早的一个周开始时间为第一周,其它类推
            int LIntWeekStartSet = IntParse(AGlodbalSetting.StrWeekStart, 0);//0为星期天，1为星期1，6为星期6
            DateTime dtYearStartTime = new DateTime(AWeekStartDatetime.Year, 1, 1);
            DateTime dtTrueYearStartTime = dtYearStartTime;
            int LIntWeek = 0;
            switch (dtYearStartTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    LIntWeek = 1;
                    break;
                case DayOfWeek.Tuesday:
                    LIntWeek = 2;
                    break;
                case DayOfWeek.Wednesday:
                    LIntWeek = 3;
                    break;
                case DayOfWeek.Thursday:
                    LIntWeek = 4;
                    break;
                case DayOfWeek.Friday:
                    LIntWeek = 5;
                    break;
                case DayOfWeek.Saturday:
                    LIntWeek = 6;
                    break;
                case DayOfWeek.Sunday:
                    LIntWeek = 0;
                    break;
                default:
                    break;
            }
            if (LIntWeekStartSet < LIntWeek)
            {
                dtTrueYearStartTime = dtYearStartTime.AddDays(LIntWeekStartSet-LIntWeek).AddDays(7);
            }
            else if (LIntWeekStartSet > LIntWeek)
            {
                dtTrueYearStartTime = dtYearStartTime.AddDays(LIntWeekStartSet-LIntWeek);
            }
            TimeSpan tsTemp = AWeekStartDatetime - dtTrueYearStartTime;
            NumOrder = tsTemp.Days / 7 + 1;

            return NumOrder;
        }


        /// <summary>
        /// 根据日期得到KPI计算要更新多长时间的数据
        /// </summary>
        /// <param name="ADatetime"></param>
        /// <param name="AGlodbalSetting"></param>
        /// <param name="ASliceType">1、年，2、月 ，3、周，4、天 5、1小时 6、 30分钟 7 、15分钟 8、10分钟  9、 5分钟</param>
        /// <returns></returns>
        public static DateTimeSplite GetCycleStartAndStopTime(DateTime ADatetime, GlobalSetting AGlodbalSetting,int ASliceType)
        {
            DateTimeSplite LDateTimeSplite = new DateTimeSplite();
            if (ASliceType < 1 || ASliceType > 10) { return null; }
            switch (ASliceType)
            {
                case 1:
                    {
                        LDateTimeSplite.StartStatisticsTime = new DateTime(ADatetime.Year, 1, 1);
                        LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddYears(1);
                    }
                    break;
                case 2:
                    {
                        int LIntMonthStartSet = IntParse(AGlodbalSetting.StrMonthStart, 0);
                        //1为1号 2为2号 26为26号
                        DateTime LDateTimeTemp = new DateTime(ADatetime.Year, ADatetime.Month, LIntMonthStartSet);
                        if (ADatetime < LDateTimeTemp)
                        {
                            LDateTimeSplite.StartStatisticsTime = LDateTimeTemp.AddMonths(1);
                            LDateTimeSplite.StopStatisticsTime = LDateTimeTemp;

                        }
                        else
                        {
                            LDateTimeSplite.StartStatisticsTime = LDateTimeTemp;
                            LDateTimeSplite.StopStatisticsTime = LDateTimeTemp.AddMonths(1);
                        }
                    }
                    break;
                case 3:
                    {
                        int LIntWeekStartSet = IntParse(AGlodbalSetting.StrWeekStart, 0);//0为星期天，1为星期1，6为星期6
                        int LIntWeek = 0;
                        switch (ADatetime.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                LIntWeek = 1;
                                break;
                            case DayOfWeek.Tuesday:
                                LIntWeek = 2;
                                break;
                            case DayOfWeek.Wednesday:
                                LIntWeek = 3;
                                break;
                            case DayOfWeek.Thursday:
                                LIntWeek = 4;
                                break;
                            case DayOfWeek.Friday:
                                LIntWeek = 5;
                                break;
                            case DayOfWeek.Saturday:
                                LIntWeek = 6;
                                break;
                            case DayOfWeek.Sunday:
                                LIntWeek = 0;
                                break;
                            default:
                                break;
                        }
                        if (LIntWeekStartSet < LIntWeek)
                        {
                            LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(LIntWeekStartSet - LIntWeek).AddDays(7);
                            LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(-7);
                        }
                        else if (LIntWeekStartSet > LIntWeek)
                        {
                            LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(LIntWeekStartSet - LIntWeek);
                            LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(-7);
                        }
                        else if (LIntWeekStartSet == LIntWeek) 
                        {
                            LDateTimeSplite.StartStatisticsTime = ADatetime;
                            LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(7);
                        }
                    }
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    { 
                         LDateTimeSplite.StartStatisticsTime =ADatetime.Date;
                         LDateTimeSplite.StopStatisticsTime = ADatetime.AddDays(1);
                    }
                    break;
                default:
                    break;
            }

            return LDateTimeSplite;
        }


        /// <summary>
        /// 得到数据库所有列，用于sum
        /// </summary>
        /// <param name="DatabaseType">数据库类型</param>
        /// <returns></returns>
        public static string GetAllColumnName() 
        {
            string ColumnName = string.Empty;
            //if (DatabaseType != 2 && DatabaseType != 3)
            //{
            //    return ColumnName;
            //}
            for (int i = 1; i <= 288; i++)
            {
                    ColumnName += string.Format("{0}+",GetColumnName(i));
            }
            ColumnName = ColumnName.TrimEnd('+');
            return ColumnName;
        }

        /// <summary>
        /// 根据5分钟的顺序得到它在一行里的顺序，然后得到相应的列
        /// </summary>
        /// <param name="OrderID">1~288</param>
        /// <returns></returns>
        public static string GetColumnName(int OrderID) 
        {
            string ColumnName = "C";
            if (OrderID < 1 || OrderID > 288) 
            {
                return "";
            }
            int tempH = OrderID / 12;
            int tempM = OrderID % 12;
            if(tempH>0 && tempM==0)
            {
                tempH--;
                tempM = 12;
            }
            ColumnName = ColumnName + tempH.ToString("00") + ((tempM - 1) * 5).ToString("00");
            return ColumnName;                 
        }

        /// <summary>
        /// 根据实际值，目标，和比较符号来得到metTheGoal
        /// </summary>
        /// <param name="goal">目标</param>
        /// <param name="actualvalue">实际值</param>
        /// <param name="sign">比较符1为小于，其它是大于</param>
        /// <returns></returns>
        public static decimal CompareSign(decimal goal, decimal actualvalue, string sign)
        {
            decimal metTheGoal = 0;
            switch (sign)
            {
                case "<=":
                case "<":  //1.< 
                    {
                        if (goal != 0)
                        {
                            metTheGoal = 1 + (actualvalue - goal) / goal * (-1);
                        }

                        break;
                    }
                case ">":
                case ">=":
                    {
                        if (goal != 0)
                        {
                            metTheGoal = 1 + (actualvalue - goal) / goal * (1);
                        }
                    }
                    break;
                default:
                    break;
            }
            return metTheGoal;
        }

        /// <summary>
        /// 根据连续几期的值确定趋势
        /// </summary>
        /// <param name="lstTrack"></param>
        /// <param name="trendCyclesNum"></param>
        /// <returns></returns>
        public static short CalculateValueTrend(List<decimal> lstTrack, int trendCyclesNum = 3)
        {
            short trend = 2;
            if (lstTrack.Count < trendCyclesNum)
            {
                return trend;
            }
            int sumUp = 0;
            int sumDown = 0;
            int sumHold = 0;
            trendCyclesNum = trendCyclesNum == 0 ? lstTrack.Count : trendCyclesNum;
            //foreach (decimal dec in lstTrack)
            for (int i = 0; i < trendCyclesNum; i++)
            {
                decimal dec = lstTrack[i];
                if (dec < 0) sumDown++;
                else if (dec > 0) sumUp++;
                else sumHold++;
            }
            if (sumDown == trendCyclesNum) trend = -1;
            else if (sumUp == trendCyclesNum) trend = 1;
            else if (sumHold == trendCyclesNum) trend = 0;
            return trend;
        }

        public static List<string> GetFieldNameList(string formula)
        {
            Regex re = new Regex(@"[a-zA-Z]", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            MatchCollection mc = re.Matches(formula);
            List<string> lstfor = new List<string>();
            for (int i = 0; i < mc.Count; i++)
            {
                lstfor.Insert(i, mc[i].Value);
            }
            return lstfor;
        }
    }
}
