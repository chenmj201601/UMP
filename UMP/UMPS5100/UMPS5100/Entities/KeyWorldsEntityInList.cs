using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UMPS5100.Entities
{
    public class KeyWorldsEntityInList : INotifyPropertyChanged
    {
        private string _KeyWorldID;

        /// <summary>
        /// 关键词ID
        /// </summary>
        public string KeyWorldID
        {
            get { return _KeyWorldID; }
            set { _KeyWorldID = value; }
        }
        private string _KeyWorldContent;

        /// <summary>
        /// 关键词内容
        /// </summary>
        public string KeyWorldContent
        {
            get { return _KeyWorldContent; }
            set { _KeyWorldContent = value;
            OnPropertyChanged("KeyWorldContent");
            }
        }
        private string _BookmarkLevelID;

        /// <summary>
        /// 关键词对应的等级ID
        /// </summary>
        public string BookmarkLevelID
        {
            get { return _BookmarkLevelID; }
            set { _BookmarkLevelID = value; }
        }
        private string _BookmarkLevelColor;

        /// <summary>
        /// 关键词对应的等级颜色
        /// </summary>
        public string BookmarkLevelColor
        {
            get { return _BookmarkLevelColor; }
            set { _BookmarkLevelColor = value;
            OnPropertyChanged("BookmarkLevelColor");
            }
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

        private string _LevelName;

        public string LevelName
        {
            get { return _LevelName; }
            set { _LevelName = value; }
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
