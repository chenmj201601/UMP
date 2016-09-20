using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common61011
{
    public class QueryCondition
    {
        public long QueryCode { get; set; }

        public long UserID { get; set; }

        public long ReportCode { get; set; }

        public DateTime SetTime { get; set; }

        public char Source { get; set; }

        public int Priority { get; set; }

        public DateTime LastUseTime { set; get; }

        //private string mName;
        //public string Name
        //{
        //    get { return mName; }
        //    set { mName = value; OnPropertyChanged("Name"); }
        //}

        //private string mDescription;
        //public string Description
        //{
        //    get { return mDescription; }
        //    set { mDescription = value; OnPropertyChanged("Description"); }
        //}

        public bool IsUse { get; set; }

        public string mName { get; set; }

        public string mDescription { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string property)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(property));
        //    }
        //}

        public override string ToString()
        {
            return mName;
        }
    }
}
