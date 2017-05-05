using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
   public class ScoreSheetInfo
    {
        //public long ID { get; set; }
        //public string Name { get; set; }
        //public string FileName { get; set; }
        ///// <summary>
        ///// 0       评分表完整
        ///// 1       评分表不完整
        ///// </summary>
        //public int State { get; set; }
        //public double TotalScore { get; set; }
        ///// <summary>
        ///// 样式
        ///// 0       树形
        ///// 1       交叉表形
        ///// </summary>
        //public int ViewClassic { get; set; }
        ///// <summary>
        ///// 分值类型
        ///// 0       数值型
        ///// 1       百分比型
        ///// 2       纯是非型
        ///// </summary>
        //public int ScoreType { get; set; }
        ///// <summary>
        ///// 使用状况
        ///// </summary>
        //public int UseFlag { get; set; }
        //public int ItemCount { get; set; }
        //public string Description { get; set; }


        public long ScoreSheetID { get; set; }
        public string Title { get; set; }
        public double TotalScore { get; set; }
    }
}
