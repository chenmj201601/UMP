using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS3107.Models
{
    public class CheckComboboxItems : INotifyPropertyChanged
    {
        public bool IsSelected { get; set; }
        public string KeyWordID { get; set; }
        public string Description { get; set; }
        public string KeyWordContent { get; set; }
        /// <summary>
        /// checkkcombox.text
        /// </summary>
        public string KeyWordName{ get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
