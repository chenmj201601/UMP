//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    67ebad2a-8224-456c-a2d0-958825ef680c
//        CLR Version:              4.0.30319.42000
//        Name:                     ToolButtonItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                ToolButtonItem
//
//        created by Charley at 2016/3/11 17:15:25
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPS1206.Models
{
    public class ToolButtonItem : INotifyPropertyChanged
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

        public object Data { get; set; }

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
