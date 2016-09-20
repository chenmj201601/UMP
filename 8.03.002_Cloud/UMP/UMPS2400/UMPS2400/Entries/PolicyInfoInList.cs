using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS2400.Entries
{
    /// <summary>
    /// 加密策略实体类 用于做listview的显示 
    /// </summary>
    public class PolicyInfoInList : INotifyPropertyChanged
    {
        private string _PolicyID;

        public string PolicyID
        {
            get { return _PolicyID; }
            set { _PolicyID = value; }
        }
        private string _PolicyName;

        public string PolicyName
        {
            get { return _PolicyName; }
            set
            {
                _PolicyName = value;
                OnPropertyChanged("PolicyName");
            }
        }
        private string _PolicyOccursFrequency;

        public string PolicyOccursFrequency
        {
            get { return _PolicyOccursFrequency; }
            set { _PolicyOccursFrequency = value;
            OnPropertyChanged("PolicyOccursFrequency");
            }
        }
        private string _PolicyType;

        public string PolicyType
        {
            get { return _PolicyType; }
            set { _PolicyType = value; }
        }
        private string _PolicyStartTime;

        public string PolicyStartTime
        {
            get { return _PolicyStartTime; }
            set { _PolicyStartTime = value;
            OnPropertyChanged("PolicyStartTime");
            }
        }
        //此属性值与数据库中值一直 数字型的开始时间 用于时间比较
        private long _PolicyStartTimeNumber;

        /// <summary>
        /// //此属性值与数据库中值一直 数字型的开始时间 用于时间比较
        /// </summary>
        public long PolicyStartTimeNumber
        {
            get { return _PolicyStartTimeNumber; }
            set { _PolicyStartTimeNumber = value; }
        }

        private long _PolicyEndTimeNumber;

        /// <summary>
        /// 此属性值与数据库中值一直 数字型的开始时间 用于时间比较
        /// </summary>
        public long PolicyEndTimeNumber
        {
            get { return _PolicyEndTimeNumber; }
            set
            {
                _PolicyEndTimeNumber = value;
            }
        }

        private string _PolicyEndTime;

        public string PolicyEndTime
        {
            get { return _PolicyEndTime; }
            set
            {
                _PolicyEndTime = value;
                OnPropertyChanged("PolicyEndTime");
            }
        }
        private string _IsStrongPwd;

        public string IsStrongPwd
        {
            get { return _IsStrongPwd; }
            set
            {
                _IsStrongPwd = value;
                OnPropertyChanged("IsStrongPwd");
            }
        }
        private string _PolicyIsEnabled;

        public string PolicyIsEnabled
        {
            get { return _PolicyIsEnabled; }
            set
            {
                _PolicyIsEnabled = value;
                OnPropertyChanged("PolicyIsEnabled");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
