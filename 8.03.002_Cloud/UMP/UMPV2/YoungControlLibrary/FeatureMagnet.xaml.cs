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
using YoungClassesLibrary;

namespace YoungControlLibrary
{
    public partial class FeatureMagnet : UserControl, INotifyPropertyChanged, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        #region 属性定义

        public string StrFeatureID { get; set; }

        /// <summary>
        /// 功能的图标
        /// </summary>
        private string _StrFeatureImageSource;
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

        /// <summary>
        /// 功能显示的文字
        /// </summary>
        private string _StrFeatureContent;
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
        /// 功能提示显示的样式
        /// </summary>
        private Style _StyleLabelFontStyle;
        public Style StyleLabelFontStyle
        {
            get { return _StyleLabelFontStyle; }
            set
            {
                _StyleLabelFontStyle = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StyleLabelFontStyle");
                }
            }
        }

        /// <summary>
        /// 显示文字的Label高度
        /// </summary>
        private double _DoubleLabelHeight;
        public double DoubleLabelHeight
        {
            get { return _DoubleLabelHeight; }
            set
            {
                _DoubleLabelHeight = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("DoubleLabelHeight");
                }
            }
        }

        #endregion

        public FeatureMagnet()
        {
            InitializeComponent();
            GridFeatureImageLabel.PreviewMouseLeftButtonDown += GridFeatureImageLabel_PreviewMouseLeftButtonDown;
        }

        private void GridFeatureImageLabel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "LOAD";
                    LEventArgs.ObjectSource0 = StrFeatureID;
                    LEventArgs.ObjectSource1 = _StrFeatureImageSource;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
        
    }
}
