using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common;

namespace Common6106
{
    public class CommonFunctions
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
            //dt = DateTime.Parse(strTime);
            dt = DateTime.Parse(DateTime.Parse(strTime).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool DateTimeToNumber(DateTime dt, ref long lResult)
        {
            try
            {
                string strTime = dt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
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

        /// <summary>
        /// datetime转换成number （不转换成utc时间）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="lResult"></param>
        /// <returns></returns>
        public static bool DateTimeToNumberWithNoArea(DateTime dt, ref long lResult)
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

        /// <summary>
        /// 获得一定时间段内的所有分表表名（用于按月分表）
        /// </summary>
        /// <param name="strBasicTableName"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="strRentToken"></param>
        /// <returns></returns>
        public static List<string> GetTablesByTime(string strBasicTableName,DateTime dtStart,DateTime dtEnd,string strRentToken)
        {
            int iMonthCount = Utils.GetTimeMonthCount(dtStart, dtEnd);
            List<string> lstTableNames = new List<string>();
            string partTable;
            for (int i = 0; i <= iMonthCount; i++)
            {
                partTable = dtStart.AddMonths(i).ToString("yyMM");
                lstTableNames.Add(string.Format("{0}_{1}_{2}", strBasicTableName,
                strRentToken, partTable));
            }
            return lstTableNames;
        }
    }
}
