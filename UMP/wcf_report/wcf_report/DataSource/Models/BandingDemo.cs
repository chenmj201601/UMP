using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace UMPS6101.Models
{
    public class BandingDemo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string ex;

        public string EX
        {
            get { return ex; }
            set
            {
                ex = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("EX"));
                }
            }
        }

    }
}
