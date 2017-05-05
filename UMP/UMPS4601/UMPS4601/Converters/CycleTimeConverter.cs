using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS4601.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public static class CycleTimeConverter
    {
        /// <summary>
        /// 以时间形式表示秒数
        /// </summary>
        public static string SecsToDateTime(double totalSecond)
        {
            string strReturn;
            int second, minute, hour;
            hour = (int)(totalSecond / (60 * 60));
            totalSecond = totalSecond % (60 * 60);
            minute = (int)(totalSecond / 60);
            totalSecond = totalSecond % 60;
            second = (int)totalSecond;
            strReturn = string.Format("{0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00"));
            return strReturn;
        }

        /// <summary>
        /// 以时间形式表示毫秒
        /// </summary>
        public static string MsecToDateTime(double millisecond)
        {
            string strReturn;
            int minute, hour;
            double second;
            hour = (int)(millisecond / (60 * 60 * 1000));
            millisecond = millisecond % (60 * 60 * 1000);
            minute = (int)(millisecond / (60 * 1000));
            millisecond = millisecond % (60 * 1000);
            second = millisecond / 1000;
            strReturn = string.Format("{0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00.0"));
            return strReturn;
        }

        /// <summary>
        /// 将时间转换成秒,HH:mm:ss格式,不能带有日期
        /// </summary>
        public static double TimeToSecs(string time)
        {
            string[] arrTime = time.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTime.Length != 3)
            {
                throw new Exception("Time string invalid");
            }
            int hour = int.Parse(arrTime[0]);
            int minute = int.Parse(arrTime[1]);
            double second = double.Parse(arrTime[2]);
            return hour * 60 * 60 + minute * 60 + second;
        }

        /// <summary>
        /// 日期转换，如将20140101000000转换成2014-01-01 00:00:00 ,少于14位自动在左侧填充空格补齐
        /// </summary>
        public static DateTime NumberToDatetime(string number)
        {
            number = number.PadLeft(14,'0');
            string year, month, day, hour, minute, second;
            year = number.Substring(0, 4);
            month = number.Substring(4, 2);
            day = number.Substring(6, 2);
            hour = number.Substring(8, 2);
            minute = number.Substring(10, 2);
            second = number.Substring(12, 2);
            string str = string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
            return DateTime.Parse(str);
        }

        /// <summary>
        /// 根据周期类型获取实际的时间范围
        /// 1、年，2、月 ，3、周，4、天 5、1小时 6、 30分钟 7 、15分钟 8、10分钟  9、 5分钟
        /// </summary>
        public static List<long> GetValidTime(DateTime time,long timeType)
        {
            List<long> timeList = new List<long>();
            DateTime startTime,stopTime;
            if (timeType == 1)//年
            {
                startTime = new DateTime(time.Year,1,1);
                timeList.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));
                stopTime = new DateTime(time.Year, 12, 31);
                timeList.Add(Convert.ToInt64(stopTime.ToString("yyyyMMddHHmmss")));
            }
            else if (timeType == 2)//月
            {
                if (S4601App.MonthStartDay > 1)
                {
                    startTime = new DateTime(time.Year, time.Month, S4601App.MonthStartDay);
                    stopTime = new DateTime(time.Year, time.Month + 1, S4601App.MonthStartDay - 1);
                }
                else
                {
                    startTime = new DateTime(time.Year, time.Month, 1);
                    stopTime = time.AddDays(1 - time.Day).AddMonths(1).AddDays(-1);
                }
                timeList.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));
                timeList.Add(Convert.ToInt64(stopTime.ToString("yyyyMMddHHmmss")));
            }
            else if (timeType == 3)//周
            {
                startTime = new DateTime(time.Year, 1, 1);
                int weekDay = int.Parse(startTime.DayOfWeek.ToString("d"));
                int yearWeekDay = 0;
                weekDay = weekDay == 0 ? 7 : weekDay;
                if (weekDay > S4601App.WeekStartDay)
                {
                    yearWeekDay = 7 - weekDay + 1;
                }
                else
                {
                    yearWeekDay = S4601App.WeekStartDay - weekDay;
                }
                weekDay = int.Parse(time.DayOfWeek.ToString("d"));
                if (weekDay > S4601App.WeekStartDay)
                {
                    timeList.Add((time.DayOfYear - yearWeekDay) / 7 + 2);
                }
                else if (weekDay < S4601App.WeekStartDay)
                {
                    timeList.Add((time.DayOfYear - yearWeekDay) / 7+1 );
                }
                else if (weekDay == S4601App.WeekStartDay)
                {
                    timeList.Add((time.DayOfYear - yearWeekDay) / 7 );
                }
            }
            else if (timeType == 4)//天
            {
                startTime = time.Date;
                timeList.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));
                stopTime = time.Date.AddDays(1).AddMilliseconds(-1);
                timeList.Add(Convert.ToInt64(stopTime.ToString("yyyyMMddHHmmss")));
            }
            else if (timeType == 5)//小时
            {
                startTime = time;
                timeList.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));
                stopTime = time.AddHours(1).AddMilliseconds(-1);
                timeList.Add(Convert.ToInt64(stopTime.ToString("yyyyMMddHHmmss")));
            }
            else if (timeType > 5)//分钟
            {
                int tempInt = 0;
                if (timeType == 6) { tempInt = 30; }
                else if (timeType == 7) { tempInt = 15; }
                else if (timeType == 8) { tempInt = 10; }
                else if (timeType == 9) { tempInt = 5; }
                startTime = time;
                timeList.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));
                stopTime = time.AddMinutes(tempInt).AddMilliseconds(-1);
                timeList.Add(Convert.ToInt64(stopTime.ToString("yyyyMMddHHmmss")));
            }
            return timeList;
        }

        /// <summary>
        /// 根据周期跟时间，获取在数据库中具体的字段跟行数(c002)
        /// 仅限于小时之后的周期, 5、1小时 6、 30分钟 7 、15分钟 8、10分钟  9、 5分钟
        /// </summary>
        public static string ReturnMinutesStr(DateTime time, long timeType)
        {
            string dataSrt = string.Empty;
            int tempIntTime , tempMin;
            switch (timeType)
            {
                case 5:
                    tempIntTime = time.Hour;
                    if (tempIntTime < 12)
                    {
                        dataSrt = string.Format("{0},{1}", (tempIntTime / 12) + 1, Convert.ToString(tempIntTime + 1).PadLeft(2,'0'));
                    }
                    else
                    {
                        dataSrt = string.Format("{0},{1}", (tempIntTime / 12) + 1, (tempIntTime-11).ToString().PadLeft(2,'0'));
                    }
                    break;
                case 6:
                    tempIntTime = time.Hour;
                    tempMin=time.Minute==30?1:0;
                    dataSrt = string.Format("{0},{1}", (tempIntTime / 6) + 1, ((tempIntTime % 6) * 2 + tempMin+1).ToString().PadLeft(2,'0'));
                    break;
                case 7:
                    tempIntTime = time.Hour;
                    tempMin = time.Minute / 15;
                    dataSrt = string.Format("{0},{1}", (tempIntTime / 3) + 1, ((tempIntTime % 3) * 4 + tempMin + 1).ToString().PadLeft(2,'0'));
                    break;
                case 8:
                    tempIntTime = time.Hour;
                    tempMin = time.Minute / 10;
                    dataSrt = string.Format("{0},{1}", (tempIntTime / 2) + 1, ((tempIntTime % 2) * 6 + tempMin + 1).ToString().PadLeft(2,'0'));
                    break;
                case 9:
                    tempIntTime = time.Hour;
                    tempMin = time.Minute / 5;
                    dataSrt = string.Format("{0},{1}", tempIntTime+1, (tempMin+1).ToString().PadLeft(2,'0'));
                    break;
            }
            return dataSrt;
        }

        /// <summary>
        /// 根据第几周换算成实际时间，一周开始时间由全局参数配置.
        /// 如果每年1月1日不是一周的开始时间那么就算是去年最后一周的,每年第一周从第一个有一周开始时间的周算起。
        /// </summary>
        public static List<long> WeekTime(int year,int week)
        {
            List<long> weekDateTime = new List<long>();
            DateTime startTime=new DateTime(), stopTime=new DateTime();
            DateTime yeartime = new DateTime(year, 1, 1), endYearTime = new DateTime(year, 12, 31), nextYearTime = new DateTime(year+1, 1, 1);
            //这一年第一条星期几
            int weekDay = int.Parse(yeartime.DayOfWeek.ToString("d"));
            weekDay = weekDay == 0 ? 7 : weekDay;
            //第二年第一天星期几
            int nextYearWeekDay = (int)nextYearTime.DayOfWeek;
            nextYearWeekDay = nextYearWeekDay == 0 ? 7 : nextYearWeekDay;
            //第一年在第二年的残留天数（因为第二年的第一天如果不是第一周的开始，那么第一周开始前的几天都算第一年最后一周的天数）
            int nextYearTempDay = 0;
            if (nextYearWeekDay > S4601App.WeekStartDay)
            {
                nextYearTempDay = 7 - nextYearWeekDay+1;
            }
            else if (nextYearWeekDay < S4601App.WeekStartDay)
            {
                nextYearTempDay = S4601App.WeekStartDay - nextYearWeekDay;
            }
            int weeks = 0;
            if ((int)yeartime.DayOfWeek == S4601App.WeekStartDay)
            {
                weeks = (endYearTime.DayOfYear + nextYearTempDay) / 7;
                if ((endYearTime.DayOfYear + nextYearTempDay) % 7 != 0)
                {
                    weeks += 1999;//除不断说明计算出错
                }
                startTime = yeartime.AddDays(7*(week-1));
                stopTime = yeartime.AddDays(7 * week);
            }
            else if (weekDay > S4601App.WeekStartDay)
            {
                weeks = (endYearTime.DayOfYear + nextYearTempDay - (7 - weekDay+1)) / 7;//一年实际天数应该是，一年的理论天数+第二年不在第一周内的天数—第一年不在第一周内的天数
                if ((endYearTime.DayOfYear + nextYearTempDay - (7 - weekDay+1)) % 7 != 0)
                {
                    weeks += 1999;
                }
                yeartime = new DateTime(year, 1, 7 - weekDay + 2);
                startTime = yeartime.AddDays(7 * (week - 1));
                stopTime = yeartime.AddDays(7 * week);
            }
            else if (weekDay < S4601App.WeekStartDay)
            {
                weeks = (endYearTime.DayOfYear + nextYearTempDay - (S4601App.WeekStartDay - weekDay)) / 7;
                if ((endYearTime.DayOfYear + nextYearTempDay - (S4601App.WeekStartDay - weekDay)) % 7 != 0)
                {
                    weeks += 1999;
                }
                yeartime = new DateTime(year, 1, S4601App.WeekStartDay - weekDay + 1);
                startTime = yeartime.AddDays(7 * (week - 1) );
                stopTime = yeartime.AddDays(7 * week );
            }
            weekDateTime.Add(Convert.ToInt64(startTime.ToString("yyyyMMddHHmmss")));//一周开始时间
            weekDateTime.Add(Convert.ToInt64(stopTime.AddDays(-1).ToString("yyyyMMddHHmmss")));//一周结束时间
            weekDateTime.Add(weeks);//这年多少周
            return weekDateTime;
        }
    }
}
