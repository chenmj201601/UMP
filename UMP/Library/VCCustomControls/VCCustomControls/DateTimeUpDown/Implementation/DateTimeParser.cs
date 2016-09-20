//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    439da9a7-cfa3-486e-b4bf-2e05fddcc0ec
//        CLR Version:              4.0.30319.18444
//        Name:                     DateTimeParser
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.DateTimeUpDown.Implementation
//        File Name:                DateTimeParser
//
//        created by Charley at 2014/7/17 16:43:34
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    internal class DateTimeParser
    {
        public static bool TryParse(string value, string format, DateTime currentDate, CultureInfo cultureInfo, out DateTime result)
        {
            bool success = false;
            result = currentDate;

            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(format))
                return false;

            var dateTimeString = ComputeDateTimeString(value, format, currentDate, cultureInfo).Trim();

            if (!String.IsNullOrEmpty(dateTimeString))
                success = DateTime.TryParse(dateTimeString, cultureInfo.DateTimeFormat, DateTimeStyles.None, out result);

            if (!success)
                result = currentDate;

            return success;
        }

        private static string ComputeDateTimeString(string dateTime, string format, DateTime currentDate, CultureInfo cultureInfo)
        {
            Dictionary<string, string> dateParts = GetDateParts(currentDate, cultureInfo);
            string[] timeParts = new string[3] { currentDate.Hour.ToString(), currentDate.Minute.ToString(), currentDate.Second.ToString() };
            string millisecondsPart = currentDate.Millisecond.ToString();
            string designator = "";
            string[] dateTimeSeparators = new string[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };

            UpdateSortableDateTimeString(ref dateTime, ref format, cultureInfo);

            var dateTimeParts = dateTime.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            var formats = format.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

            //something went wrong
            if (dateTimeParts.Count != formats.Count)
                return string.Empty;

            for (int i = 0; i < formats.Count; i++)
            {
                var f = formats[i];
                if (!f.Contains("ddd") && !f.Contains("GMT"))
                {
                    if (f.Contains("M"))
                        dateParts["Month"] = dateTimeParts[i];
                    else if (f.Contains("d"))
                        dateParts["Day"] = dateTimeParts[i];
                    else if (f.Contains("y"))
                    {
                        dateParts["Year"] = dateTimeParts[i];

                        if (dateParts["Year"].Length == 2)
                            dateParts["Year"] = string.Format("{0}{1}", currentDate.Year / 100, dateParts["Year"]);
                    }
                    else if (f.Contains("h") || f.Contains("H"))
                        timeParts[0] = dateTimeParts[i];
                    else if (f.Contains("m"))
                        timeParts[1] = dateTimeParts[i];
                    else if (f.Contains("s"))
                        timeParts[2] = dateTimeParts[i];
                    else if (f.Contains("f"))
                        millisecondsPart = dateTimeParts[i];
                    else if (f.Contains("t"))
                        designator = dateTimeParts[i];
                }
            }

            var date = string.Join(cultureInfo.DateTimeFormat.DateSeparator, dateParts.Select(x => x.Value).ToArray());
            var time = string.Join(cultureInfo.DateTimeFormat.TimeSeparator, timeParts);
            time += "." + millisecondsPart;

            return String.Format("{0} {1} {2}", date, time, designator);
        }

        private static void UpdateSortableDateTimeString(ref string dateTime, ref string format, CultureInfo cultureInfo)
        {
            if (format == cultureInfo.DateTimeFormat.SortableDateTimePattern)
            {
                format = format.Replace("'", "").Replace("T", " ");
                dateTime = dateTime.Replace("'", "").Replace("T", " ");
            }
            else if (format == cultureInfo.DateTimeFormat.UniversalSortableDateTimePattern)
            {
                format = format.Replace("'", "").Replace("Z", "");
                dateTime = dateTime.Replace("'", "").Replace("Z", "");
            }
        }

        private static Dictionary<string, string> GetDateParts(DateTime currentDate, CultureInfo cultureInfo)
        {
            Dictionary<string, string> dateParts = new Dictionary<string, string>();
            var dateTimeSeparators = new[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };
            var dateFormatParts = cultureInfo.DateTimeFormat.ShortDatePattern.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            dateFormatParts.ForEach(item =>
            {
                string key = string.Empty;
                string value = string.Empty;

                if (item.Contains("M"))
                {
                    key = "Month";
                    value = currentDate.Month.ToString();
                }
                else if (item.Contains("d"))
                {
                    key = "Day";
                    value = currentDate.Day.ToString();
                }
                else if (item.Contains("y"))
                {
                    key = "Year";
                    value = currentDate.Year.ToString("D4");
                }
                dateParts.Add(key, value);
            });
            return dateParts;
        }
    }
}
