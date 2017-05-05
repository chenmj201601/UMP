using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS3103.Models
{
    public class ScoreSheetItem : INotifyPropertyChanged
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
        public double TotalScore { get; set; }
        /// <summary>
        /// 子项总数（包括类别，评分标准）
        /// </summary>
        public int ItemCount { get; set; }
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
        public string Description { get; set; }
        public object Data { get; set; }

        private string mTipState;

        public string TipState
        {
            get { return mTipState; }
            set
            {
                mTipState = value;
                OnPropertyChanged("TipState");
            }
        }

        private string mTipViewClassic;

        public string TipViewClassic
        {
            get { return mTipViewClassic; }
            set { mTipViewClassic = value; OnPropertyChanged("TipViewClassic"); }
        }

        private string mTipScoreType;

        public string TipScoreType
        {
            get { return mTipScoreType; }
            set { mTipScoreType = value; OnPropertyChanged("TipScoreType"); }
        }

        private string mTipOptMofify;

        public string TipOptModify
        {
            get { return mTipOptMofify; }
            set { mTipOptMofify = value; OnPropertyChanged("TipOptModify"); }
        }

        private string mTipOptDelete;

        public string TipOptDelete
        {
            get { return mTipOptDelete; }
            set { mTipOptDelete = value; OnPropertyChanged("TipOptDelete"); }
        }

        private string mTipOptSetUser;

        public string TipOptSetUser
        {
            get { return mTipOptSetUser; }
            set { mTipOptSetUser = value; OnPropertyChanged("TipOptSetUser"); }
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
