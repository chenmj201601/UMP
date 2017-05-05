//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    53227423-6fda-4f07-8714-0d05a2160f4d
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryCondition
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                QueryCondition
//
//        created by Charley at 2014/11/7 10:33:43
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class QueryCondition
    {
        //这个类的作用就是存储快速查询的条件   对应T_31_028这张表
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long UserID { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateType { get; set; }
        public int SortID { get; set; }
        public DateTime LastQueryTime { get; set; }
        public bool IsEnable { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UseCount { get; set; }
        /// <summary>
        /// 最后一次查询返回的记录数
        /// </summary>
        public int RecordCount { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
