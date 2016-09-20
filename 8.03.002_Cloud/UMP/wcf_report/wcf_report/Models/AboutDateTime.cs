using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS6101.Models
{
    public class AboutDateTime
    {
        //Sign是作为一个标志,根据这个标志来判断是生成哪种类型的报表 "D" "W" "M" "Y"
        public String Sign { get; set; }

        //BeginDateTime,就是在每截取的那段时间内开始时间 比如 我选择"D",那么我在界面上显示的时间分成好多天，然后这里就是每一次的开始时间 
        //public List<DateTime> BeginDateTime { get; set; }
        public List<DateTime> BeginDateTime = new List<DateTime>();
        //EndDateTime,
        //public List<DateTime> EndDateTime { get; set; }
        public List<DateTime> EndDateTime = new List<DateTime>();
    }
}
