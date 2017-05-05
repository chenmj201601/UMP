using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    //全局信息
    public  class GlobalSetting
    {
        /// 得到该租户的月和周的设定
        /// 12010101每周开始于默认值为0
        /// 0为周日，1星期一，6为星期六
        /// 12010102每月开始于默认值为1
        /// 1为自然月,2为2号,最大28为28号
        /// 12010401 为分机和座席 E为分机 A为座席 E char(27)A为座席+分机 R为真实分机


        //1 租户
        public string StrRent { set; get; }


        //2 月的开始时间
        public string StrMonthStart { set; get; }

        //3 周的开始时间
        public string StrWeekStart { set; get; }
        

        //4 AER的配置信息 
        public string StrConfigAER { set; get; }


        //5 分表信息 1表示有按月分表，2表示无按月分表
        public int IlogicPartMark { set; get; } //

        //6 分表的录音表表名
        public List<string> LStrRecordName { set; get; }
        
    }
}
