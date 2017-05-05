using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    public class RecordKeyWordInfo
    {
        public long SerialID { get; set; }  // T_51_009的C001
        public long RecordID { get; set; } //记录流水号  T_21_0001的C002
        public string RecordReference { set; get; } //64位的那个 T_21_0001的C077

        public int Offset { get; set; }
        public int Duration { get; set; } //时长

        public string Title { get; set; } //关键词
        public string Content { get; set; }//内容
        public long TitleID { set; get; }//关键词ID
        public long ContentID { set; get; }//内容ID

        public long startTimeNumber { set; get; }//开始时间
        public long stopTimeNumber { set; get; }//结束时间
        public long GloryNumber { set; get; }//信誉值

        public string PictureName { set; get; }//关键词图标
        public string PicturePath { set; get; } //关键词路径
    }
}
