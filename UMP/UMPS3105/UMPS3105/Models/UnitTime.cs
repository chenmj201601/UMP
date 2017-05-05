using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS3105.Models
{
   public class UnitTime : INotifyPropertyChanged
    {
        public int ID { set; get; }
        private string code;
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                OnPropertyChanged("code");
            }
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
