using System;
using System.ComponentModel;

namespace UMPS3105.Models
{
    public class EnumItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public Type Type { get; set; }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; SubPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SubPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
