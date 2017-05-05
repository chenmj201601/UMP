//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e2477827-3363-4781-99e7-ffa651af7a5a
//        CLR Version:              4.0.30319.18063
//        Name:                     ImageButtonItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                ImageButtonItem
//
//        created by Charley at 2015/7/20 10:07:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 图标按钮
    /// </summary>
    public class ImageButtonItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mToolTip;
        /// <summary>
        /// 悬停文本
        /// </summary>
        public string ToolTip
        {
            get { return mToolTip; }
            set { mToolTip = value; OnPropertyChanged("ToolTip"); }
        }

        private string mIcon;
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }
}
