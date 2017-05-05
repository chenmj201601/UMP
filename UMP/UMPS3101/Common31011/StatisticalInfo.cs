//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a57b5332-9994-4766-b97a-daec1da7eebb
//        CLR Version:              4.0.30319.18063
//        Name:                     StatisticalInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31011
//        File Name:                StatisticalInfo
//
//        created by Charley at 2015/11/16 17:49:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31011
{
    /// <summary>
    /// 统计标准信息
    /// </summary>
    public class StatisticalInfo
    {
        /// <summary>
        /// 统计标准ID（312）
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 统计参数ID（311）
        /// </summary>
        public long ParamID { get; set; }
        /// <summary>
        /// 所属部门或技能组ID
        /// </summary>
        public long OwnerID { get; set; }

        /// <summary>
        /// 统计标准名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属机构或技能组名称
        /// </summary>
        public string OwnerName { get; set; }
    }
}
