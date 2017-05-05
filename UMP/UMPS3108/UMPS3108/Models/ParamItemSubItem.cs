using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS3108.Models
{
    public class ParamItemSubItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public long QueryValue { get; set; }

        private bool mIsChecked;
        //表示复选框的的勾选状态的
        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        private string mDisplay;
        //这个是在界面上显示的内容Display
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        ////这个是Combox默认选择的Tag
        //private int mSelectTag;
        //public int SelectTag
        //{
        //    get { return mSelectTag; }
        //    set { mSelectTag = value; OnPropertyChanged("SelectTag"); }
        //}

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