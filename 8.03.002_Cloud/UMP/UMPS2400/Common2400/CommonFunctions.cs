using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
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
            strTime += source.Substring(12, 2) ;
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

        public static string ConvertByteToHexStr(byte[] ABytesValue)
        {
            string LStrReturn = "";

            if (ABytesValue != null)
            {
                for (int LIntLoopByte = 0; LIntLoopByte < ABytesValue.Length; LIntLoopByte++) { LStrReturn += ABytesValue[LIntLoopByte].ToString("X2"); }
            }

            return LStrReturn;
        }

        public static string ConvertHexToString(string AStrHexValue)
        {
            string LStrReturn = "";
            string LStrSource = string.Empty;

            LStrSource = AStrHexValue;
            while (LStrSource.Length > 0)
            {
                LStrReturn += Convert.ToChar(Convert.ToUInt32(LStrSource.Substring(0, 2), 16)).ToString();
                LStrSource = LStrSource.Substring(2);
            }
            return LStrReturn;
        }

    }
}
