using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS3105.Models
{
    /// <summary>
    /// 目前用來判斷有沒有時間範圍設定，裡面的數據內容沒什麼實際意義
    /// </summary>
    public class DateTimeSpliteAsDay
    {
        public DateTime StartDayTime { set; get; }
        public DateTime StopDayTime { set; get; }
    }
}
