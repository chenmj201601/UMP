using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common1111
{
    public class ResourceInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        public long ResourceID { get; set; }

        /// <summary>
        /// 资源编号（自用，如：110900001）
        /// </summary>
        public long ResourceCode { get; set; }
        /// <summary>
        /// 资源名称：
        /// (录音服务器IP)
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// 资源全名
        /// </summary>
        public string ResourceFullName { get; set; }
        /// <summary>
        /// 是否被分配
        /// </summary>
        private bool mIsUsed;
        public bool IsUsed
        {
            get { return mIsUsed; }
            set { mIsUsed = value; OnPropertyChanged("IsUsed"); }
        }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string TenantName { get; set; }
        /// <summary>
        /// 租户编号
        /// </summary>
        public long TenantID { get; set; }

        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public ResourceInfo()
        {
            IsUsed = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override string ToString()
        {
            if (IsUsed)
            {
                return string.Format("{0}[{1}][{2}]", ResourceFullName, ResourceName,TenantName);
            }
            else
            return string.Format("{0}[{1}]", ResourceFullName, ResourceName);
        }
        public string Tostring()
        {
            return string.Format("{0}[{1}]", ResourceFullName, ResourceName);
        }
    }
}
