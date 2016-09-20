using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMPS6101.SharingClasses
{
    class CalendarFunctions
    {
        public static int Week = S6101App.WeekStart;
        public static int Month = S6101App.MonthStart;

        public static void GetWeek(ref string ls_begin, ref string ls_end)
        {
            DateTime ldt_today, ldt_begin, ldt_end;
            int li_dayofweek, li_begin, li_end;

            ldt_today = DateTime.Today;
            li_dayofweek = (int)ldt_today.DayOfWeek;
            if (Week == li_dayofweek)
            {
                li_begin = 0; li_end = 6;
            }
            else
            {
                li_begin = Week - li_dayofweek - 7;
                li_end = Week - 1 - li_dayofweek;
            }
            ldt_begin = ldt_today.AddDays(li_begin);
            ldt_end = ldt_today;
            //ldt_end = ldt_today.AddDays(li_end);
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");
        }

        public static void GetThisMonth(ref string ls_begin, ref string ls_end)
        {
            DateTime ldt_today, ldt_begin, ldt_end;
            int li_total_day, li_total_day_B;
            
            ldt_today = DateTime.Today;
            li_total_day = DateTime.DaysInMonth(ldt_today.Year, ldt_today.Month);
            int year = ldt_today.Year; int month = ldt_today.Month - 1;
            if (month == 0)
            { month = 12; year -= 1; }
            li_total_day_B = DateTime.DaysInMonth(year, month);
            if (ldt_today.Day >= Month)
            {
                ldt_begin = ldt_today.AddDays(Month - ldt_today.Day);
                //ldt_end = ldt_begin.AddDays(li_total_day - 1);
            }
            else
            {
                ldt_end = ldt_today.AddDays(Month - ldt_today.Day - 1);
                ldt_begin = ldt_end.AddDays(1 - li_total_day_B);
            }
            ldt_end = ldt_today;
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");
        }

        public static void GetPriorMonth(ref string ls_begin, ref string ls_end)
        {
            DateTime ldt_begin; DateTime ldt_end;
            //string Begin = ls_begin; string End = ls_end;

            DateTime ldt_today;
            int li_total_day, li_total_day_B;

            ldt_today = DateTime.Today;
            li_total_day = DateTime.DaysInMonth(ldt_today.Year, ldt_today.Month);
            int year = ldt_today.Year; int month = ldt_today.Month - 1;
            if (month == 0)
            { month = 12; year -= 1; }
            li_total_day_B = DateTime.DaysInMonth(year, month);
            if (ldt_today.Day >= Month)
            {
                ldt_begin = ldt_today.AddDays(Month - ldt_today.Day);
                ldt_end = ldt_begin.AddDays(li_total_day - 1);
            }
            else
            {
                ldt_end = ldt_today.AddDays(Month - ldt_today.Day - 1);
                ldt_begin = ldt_end.AddDays(1 - li_total_day_B);
            }
            //ldt_end = ldt_today;
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");

            //GetThisMonth(ref Begin, ref End);
            ldt_begin = Convert.ToDateTime(ls_begin).AddMonths(-1);
            ldt_end = Convert.ToDateTime(ls_end).AddMonths(-1);
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");
        }

        public static void GetLastestThreeMonth(ref string ls_begin, ref string ls_end)
        {
            DateTime ldt_begin, ldt_end;
            //string Begin = ls_begin; string End = ls_end;

            DateTime ldt_today;
            int li_total_day, li_total_day_B;

            ldt_today = DateTime.Today;
            li_total_day = DateTime.DaysInMonth(ldt_today.Year, ldt_today.Month);
            int year = ldt_today.Year; int month = ldt_today.Month - 1;
            if (month == 0)
            { month = 12; year -= 1; }
            li_total_day_B = DateTime.DaysInMonth(year, month);
            if (ldt_today.Day >= Month)
            {
                ldt_begin = ldt_today.AddDays(Month - ldt_today.Day);
                ldt_end = ldt_begin.AddDays(li_total_day - 1);
            }
            else
            {
                ldt_end = ldt_today.AddDays(Month - ldt_today.Day - 1);
                ldt_begin = ldt_end.AddDays(1 - li_total_day_B);
            }
           // ldt_end = ldt_today;
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");

            //GetThisMonth(ref Begin, ref End);
            ldt_begin = Convert.ToDateTime(ls_begin).AddMonths(-3);
            ldt_end = Convert.ToDateTime(ls_end).AddMonths(-1);
            ls_begin = ldt_begin.ToString("yyyy-MM-dd");
            ls_end = ldt_end.ToString("yyyy-MM-dd");
        }
    }
}
