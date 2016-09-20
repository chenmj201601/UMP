using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS4601.Models
{
    public class CheckComboboxItems : INotifyPropertyChanged
    {
        public bool IsSelected { get; set; }
        public string KPIMappingID { get; set; }
        public string Description { get; set; }
        public string KPICycle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
