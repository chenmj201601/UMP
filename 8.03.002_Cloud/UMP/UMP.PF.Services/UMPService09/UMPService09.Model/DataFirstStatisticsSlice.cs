using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace UMPService09.Model
{
    //段数据类
    public class DataFirstStatisticsSlice
    {
        /// <summary>
        /// 统计对象类型（1座席 2分机  3用户 4真实分机 ）
        /// </summary>
        public int ObjectType { set; get; }
        public string StrRent { set; get; }
        public long ObjectID { set; get; }

        /// <summary>
        /// 录音表 1、录音时长 2、 通话时长 3、呼入电话数 4、呼出电话数  5、转换次数 6、座席讲话时长 7、8、9、10、
        /// </summary>
        public EnumRecordFunction RecordFunction { set; get; }
        public EnumQMFunction QMFunction { set; get; }
        public long StartTimeUTC { set; get; }
        public long StartTimeLocal { set; get; }

        public DateTime UpdateTime { set; get; }
        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }



        //初次统计表暂时不会用到,以后用于座席分机移部门时用，先不考虑
        public long TrueLocalTimeStart { set; get; }
        public long TrueLocalTimeStop { set; get; }
        public long BelongID { set; get; }



        //表示列的顺序
        public int OrderID { set; get; }
        public long Value01 { set; get; }

        //统计类型 {录音表的统计，QM的统计，AES的统计}
        public ConstStatisticsType StatisticsType { set; get; }

        //切片类型
        public EnumSliceType SliceType { set; get; }


    }
}
