using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPS1110
{
    public partial class UCTextBoxForSettedDataView : UserControl, INotifyPropertyChanged
    {
        public UCTextBoxForSettedDataView()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        #region 属性定义
        private string _StrDataView;
        public string StrDataView
        {
            get { return _StrDataView; }
            set
            {
                _StrDataView = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrDataView"); }
            }
        }
        #endregion

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion
    }
}
