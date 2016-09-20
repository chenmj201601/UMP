using System.Windows.Controls;

namespace Common5101
{
    public class ContentInfoParam
    {
        #region 关键词类
        //是否被启用
        public CheckBox CbEnable { get; set; }
        public bool BEnable { get; set; }

        //关键词Num
        public long LongKwNum { get; set; }

        //关键词
        public string StrKw { get; set; }

        //是否删除
        public bool BDelete { get; set; }

        //添加时间
        public string StrAddUtcTime { get; set; }
        public string StrAddLocalTime { get; set; }

        //添加人ID
        public long LongAddPaperNum { get; set; }

        //添加人
        public string StrAddPaperName { get; set; }

        //删除时间
        public string StrDeleteUtcTime { get; set; }
        public string StrDeleteLoaclTime { get; set; }

        //删除人ID
        public long LongDeletePaperNum { get; set; }

        //删除人
        public string StrDeletePaperName { get; set; }

        //修改时间
        public string StrChangeUtcTime { get; set; }
        public string StrChangeLoaclTime { get; set; }

        //修改人ID
        public long LongChangePaperNum { get; set; }

        //修改人
        public string StrChangePaperName { get; set; }

        #endregion
    }
}
