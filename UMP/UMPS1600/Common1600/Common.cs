using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common1600
{
    public class CommonFuncs
    {
        /// <summary>
        /// 字符串转datetime
        /// </summary>
        /// <param name="source"></param>
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
        /// 时间转数字
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool DateTimeToNumber(DateTime dt, ref long lResult)
        {
            try
            {
                string strTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                strTime = strTime.Replace("-", "");
                strTime = strTime.Replace(":", "");
                strTime = strTime.Replace(" ", "");
                long.TryParse(strTime, out lResult);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
