using System.ComponentModel;

namespace Common5102
{
    public class KeywordInfoParam
    {
        #region 关键词类
        //启用状态
        private int mState;
        public int State
        {
            get { return mState; }
            set
            {
                mState = value;
                OnPropertyChanged("State");
            }
        }

        //图片
        private string mStrImage;

        public string StrImage
        {
            get { return mStrImage; }
            set
            {
                mStrImage = value;
                OnPropertyChanged("StrImage");
            }
        }

        //关键词图标路径
        public string StrImagePath { get; set; }

        //关键词Num
        public long LongKwNum { get; set; }

        //关键词
        public string StrKw { get; set; }

        //关键词内容
        public string StrKwContent { get; set; }

        //是否删除
        public int IntDelete { get; set; }
        public string StrDeleteState { get; set; }

        //添加时间
        public string StrAddUtcTime { get; set; }
        public string StrAddLocalTime { get; set; }
        
        //添加人ID
        public long LongAddPaperNum { get; set; }
        
        //添加人
        public string StrAddPaperName { get; set; }

        //删除时间
        public string StrDeleteUtcTime { get; set; }
        public string StrDeleteLocalTime { get; set; }

        //删除人ID
        public long LongDeletePaperNum { get; set; }

        //删除人
        public string StrDeletePaperName { get; set; }

        //修改时间
        public string StrChangeUtcTime { get; set; }
        public string StrChangeLocalTime { get; set; }

        //修改人ID
        public long LongChangePaperNum { get; set; }

        //修改人
        public string StrChangePaperName { get; set; }

        //恢复时间
        public string StrRestoreUtcTime { get; set; }
        public string StrRestoreLocalTime { get; set; }

        //恢复人ID
        public long LongRestorePaperNum { get; set; }

        //恢复人
        public string StrRestorePaperName { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
