using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common;

namespace UMPS3108.Common31081
{
    public class QualityParam : GlobalParamInfo, INotifyPropertyChanged
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        /// <summary>
        /// 参数描述
        /// </summary>
        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        /// <summary>
        /// 最后修改时间C018
        /// </summary>
        public string ModifyTime { get; set; }

        /// <summary>
        /// 最后修改人C019
        /// </summary>
        public long ModifyMan { get; set; }

        /// <summary>
        /// 数值类型C007
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 展示格式（都是105）
        /// </summary>
        public int DisplayFormat { get; set; }

        /// <summary>
        /// 树里的父辈编号
        /// </summary>
        public string ParentTreeID { get; set; }


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
