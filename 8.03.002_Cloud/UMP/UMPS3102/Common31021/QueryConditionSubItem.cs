//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e1270466-cb5a-4ecf-a61e-7fa5fe3aeb88
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryConditionSubItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                QueryConditionSubItem
//
//        created by Charley at 2014/11/8 12:38:27
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 查询条件子项，当查询条件项的结果是多值型时，其多个值保存在子项中
    /// </summary>
   public class QueryConditionSubItem
    {
       public long QueryID { get; set; }
       public long ConditionItemID { get; set; }
       public int Number { get; set; }
       public string Value01 { get; set; }
       public string Value02 { get; set; }
       public string Value03 { get; set; }
       public string Value04 { get; set; }
       public string Value05 { get; set; }
    }
}
