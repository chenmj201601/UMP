using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UMPS2400.Entries
{
    public class KeyGenServerEntryInList : INotifyPropertyChanged
    {
        private string _ResourceID;

        public string ResourceID
        {
            get { return _ResourceID; }
            set { _ResourceID = value; }
        }

        private string _HostAddress;

        public string HostAddress
        {
            get { return _HostAddress; }
            set { _HostAddress = value; }
        }
        private string _HostPort;

        public string HostPort
        {
            get { return _HostPort; }
            set
            {
                _HostPort = value;
                OnPropertyChanged("HostPort");
            }
        }
        private string _IsEnable;

        public string IsEnable
        {
            get { return _IsEnable; }
            set { _IsEnable = value; }
        }
        private bool _Status;

        public bool Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        private string _IsEnableIcon;

        /// <summary>
        /// 启用禁用图标
        /// </summary>
        public string EnableIcon
        {
            get { return _IsEnableIcon; }
            set
            {
                _IsEnableIcon = value;
                OnPropertyChanged("EnableIcon");
            }
        }

        /// <summary>
        /// 状态图标
        /// </summary>
        private string _StatusIcon;

        public string StatusIcon
        {
            get { return _StatusIcon; }
            set
            {
                _StatusIcon = value;
                OnPropertyChanged("StatusIcon");
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
