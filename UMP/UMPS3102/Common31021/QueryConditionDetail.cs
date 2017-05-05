//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eab2566f-bfe5-4521-aeff-0c54968d2936
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryConditionDetail
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                QueryConditionDetail
//
//        created by Charley at 2014/11/8 12:36:11
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 查询条件的详情
    /// </summary>
    public class QueryConditionDetail
    {
        public long QueryID { get; set; }
        public long ConditionItemID { get; set; }
        public bool IsEnable { get; set; }

        public string Value01 { get; set; }
        public string Value02 { get; set; }
        public string Value03 { get; set; }
        public string Value04 { get; set; }
        public string Value05 { get; set; }
        public string Value06 { get; set; }
        public string Value07 { get; set; }


    }
}
