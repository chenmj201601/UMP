//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3acf3aa8-b36c-43fd-a009-88e8dcbac740
//        CLR Version:              4.0.30319.18444
//        Name:                     BasicScoreSheetInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31011
//        File Name:                BasicScoreSheetInfo
//
//        created by Charley at 2014/10/9 14:09:51
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31011
{
    public class BasicScoreSheetInfo
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// 0       评分表完整
        /// 1       评分表不完整
        /// </summary>
        public int State { get; set; }
        public double TotalScore { get; set; }
        /// <summary>
        /// 样式
        /// 0       树形
        /// 1       交叉表形
        /// </summary>
        public int ViewClassic { get; set; }
        /// <summary>
        /// 分值类型
        /// 0       数值型
        /// 1       百分比型
        /// 2       纯是非型
        /// </summary>
        public int ScoreType { get; set; }
        /// <summary>
        /// 使用状况
        /// </summary>
        public int UseFlag { get; set; }
        public int ItemCount { get; set; }
        public string Description { get; set; }
    }
}
