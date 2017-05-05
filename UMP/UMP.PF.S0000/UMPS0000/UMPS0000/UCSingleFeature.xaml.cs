using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace UMPS0000
{
    public partial class UCSingleFeature : UserControl, INotifyPropertyChanged, S0000Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private string IStr11011005 = string.Empty;

        public UCSingleFeature(string AStr11011005)
        {
            InitializeComponent();
            IStr11011005 = AStr11011005;
            GridUCSingleFeature.PreviewMouseLeftButtonDown += GridUCSingleFeature_PreviewMouseLeftButtonDown;
        }

        private void GridUCSingleFeature_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "FeatureLoad";
                    LEventArgs.ObjectSource0 = IDataRow11003;
                    LEventArgs.ObjectSource1 = _StrFeatureImageSource;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

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

        #region 属性定义
        
        private string _StrFeatureImageSource;
        /// <summary>
        /// 功能的图标
        /// </summary>
        public string StrFeatureImageSource
        {
            get { return _StrFeatureImageSource; }
            set
            {
                _StrFeatureImageSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureImageSource");
                }
            }
        }

        private string _StrFeatureContent;
        /// <summary>
        /// 功能显示的文字
        /// </summary>
        public string StrFeatureContent
        {
            get { return _StrFeatureContent; }
            set
            {
                _StrFeatureContent = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureContent");
                }
            }
        }

        /// <summary>
        /// 当前功能的信息，对应表T_11_003中的一行数据
        /// </summary>
        public DataRow IDataRow11003 { get; set; }
        #endregion
    }
}
