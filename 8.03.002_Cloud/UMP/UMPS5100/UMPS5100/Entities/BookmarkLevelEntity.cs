using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UMPS5100.Entities
{
    public class BookmarkLevelEntityInList : INotifyPropertyChanged
    {
        /// <summary>
        /// ID
        /// </summary>
        private string _BookmarkLevelID;

        public string BookmarkLevelID
        {
            get { return _BookmarkLevelID; }
            set { _BookmarkLevelID = value;
            OnPropertyChanged("BookmarkLevelID");
            }
        }
        private string _BookmarkLevelName;

        /// <summary>
        /// 标签等级名称
        /// </summary>
        public string BookmarkLevelName
        {
            get { return _BookmarkLevelName; }
            set { _BookmarkLevelName = value;
            OnPropertyChanged("BookmarkLevelName");
            }
        }
        private string _BookmarkLevelColor;

        /// <summary>
        /// 标签等级颜色
        /// </summary>
        public string BookmarkLevelColor
        {
            get { return _BookmarkLevelColor; }
            set { _BookmarkLevelColor = value;
            OnPropertyChanged("BookmarkLevelColor");
            }
        }
        private string _BookmarkLevelStatusIcon;

        /// <summary>
        /// 等级是否可用 图标路径
        /// </summary>
        public string BookmarkLevelStatusIcon
        {
            get { return _BookmarkLevelStatusIcon; }
            set { _BookmarkLevelStatusIcon = value;
            OnPropertyChanged("BookmarkLevelStatusIcon");
            }
        }
        private string _BookmarkLevelStatus;

        /// <summary>
        /// 等级是否可用 可用：1 不可用：0
        /// </summary>
        public string BookmarkLevelStatus
        {
            get { return _BookmarkLevelStatus; }
            set { _BookmarkLevelStatus = value; }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set
            {
                mBackground = value;
                OnPropertyChanged("Background");
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
