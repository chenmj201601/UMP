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

namespace PFShareControls
{
    public partial class UCTreeViewItem : UserControl, INotifyPropertyChanged, PFShareInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private string IStrItemImage = string.Empty;

        public UCTreeViewItem(bool? ABoolItemChecked, string AStrItemImage, string AStrItemContent)
        {
            InitializeComponent();
            BoolIsChecked = ABoolItemChecked;
            
            IStrItemImage = AStrItemImage;
            StrItemContent = AStrItemContent;

            this.Loaded += UCTreeViewItem_Loaded;
        }

        private void UCTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            ImageItem.Source = new BitmapImage(new Uri(IStrItemImage, UriKind.RelativeOrAbsolute));
            CheckBoxIsChecked.Click += CheckBoxIsChecked_Click;
        }

        private void CheckBoxIsChecked_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxStatusChanged();
        }

        public void CheckBoxStatusChanged()
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "";
                LEventArgs.ObjectSource = this;
                IOperationEvent(this, LEventArgs);
            }
        }

        public void SetIsChecked(bool? ABoolValue)
        {
            BoolIsChecked = ABoolValue;
        }

        #region 属性定义
        private bool? _BoolIsChecked;
        public bool? BoolIsChecked
        {
            get { return _BoolIsChecked; }
            set
            {
                _BoolIsChecked = value;
                if (this.PropertyChanged != null) { NotifyPropertyChanged("BoolIsChecked"); }
            }
        }

        private string _StrItemContent;
        public string StrItemContent
        {
            get { return _StrItemContent; }
            set
            {
                _StrItemContent = value;
                if (this.PropertyChanged != null) { NotifyPropertyChanged("StrItemContent"); }
            }
        }

        public object IObjectThisData;
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

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
    }
}
