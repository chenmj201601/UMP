//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9c18450b-31ec-4f59-b93b-d3febfc3ee30
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                ScoreSheetItem
//
//        created by Charley at 2014/10/8 17:26:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPS3101.Models
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

        private string mTipOptExport;

        public string TipOptExport
        {
            get { return mTipOptExport; }
            set { mTipOptExport = value; OnPropertyChanged("TipOptExport"); }
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
