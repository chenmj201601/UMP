using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    public class ServiceConfigInfo
    {
        public string StatistcsName { set; get; }
        public bool IsStart { set; get; }
        public DateTime StartTime { set; get; }

        //用于保存到xml中
        public DateTime CurrentStatisticsTime { set; get; }
    }
}
