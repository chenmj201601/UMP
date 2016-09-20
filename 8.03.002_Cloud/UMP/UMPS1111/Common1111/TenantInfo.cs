using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common1111
{
    public class TenantInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 租户编号
        /// </summary>
        public long RentID { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string RentName { get; set; }

        public TenantInfo()
        { }

        public override string ToString()
        {
            return string.Format("{0}", RentName);
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
