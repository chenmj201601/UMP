using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    //用于分区表拆分时间
    public class DateTimeSplite
    {
        public DateTime StartStatisticsTime { set; get; }
        public DateTime StopStatisticsTime { set; get; }

    }
}
