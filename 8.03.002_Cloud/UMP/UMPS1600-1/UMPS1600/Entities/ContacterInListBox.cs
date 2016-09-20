using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UMPS1600.Entities
{
    public class ContacterInListBox : INotifyPropertyChanged
    {
        private long _UserID;

        /// <summary>
        /// ID
        /// </summary>
        public long UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        private string _UserName;
        /// <summary>
        /// Name
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private string _Status;
        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }

        }

        private long _OrgID;

        /// <summary>
        /// 机构ID
        /// </summary>
        public long OrgID
        {
            get { return _OrgID; }
            set { _OrgID = value; }
        }

        private string _OrgName;

        /// <summary>
        /// 机构名
        /// </summary>
        public string OrgName
        {
            get { return _OrgName; }
            set { _OrgName = value; }
        }

        private string _FullName;

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }

        private long _ParentOrgID;

        public long ParentOrgID
        {
            get { return _ParentOrgID; }
            set { _ParentOrgID = value; }
        }

        private Brush _ForegGround;

        public Brush ForegGround
        {
            get { return _ForegGround; }
            set
            {
                _ForegGround = value;
                OnPropertyChanged("ForegGround");
            }
        }
        private string _Icon;

        public string Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                OnPropertyChanged("Icon");
            }
        }

        private string _IMGOpacity;

        /// <summary>
        /// 图片透明度  默认为1 在有消息时 开启线程修改此值 以达到闪动效果
        /// </summary>
        public string IMGOpacity
        {
            get { return _IMGOpacity; }
            set
            {
                _IMGOpacity = value;
                OnPropertyChanged("IMGOpacity");
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
