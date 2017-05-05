using System.ComponentModel;

namespace Common3604
{
    public class FileProperty
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string StrName { get; set; }

        /// <summary>
        /// 路劲
        /// </summary>
        public string StrPath { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public long LSize { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string StrFileType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int IState { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        /// 
        private string mStrImage;
        public string StrImage { get { return mStrImage; }
            set
            {
                mStrImage = value;
                OnPropertyChanged("StrImage");
            }
        }

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
