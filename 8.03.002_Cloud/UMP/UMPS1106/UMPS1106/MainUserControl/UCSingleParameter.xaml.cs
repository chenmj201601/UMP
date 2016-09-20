using PFShareClassesC;
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

namespace UMPS1106.MainUserControl
{
    /// <summary>
    /// UCSingleParameter.xaml 的交互逻辑
    /// </summary>
    public partial class UCSingleParameter : INotifyPropertyChanged, S1106Interface
    {
        public MainView00000A IPageParent = null;

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCSingleParameter(DataRow ADataRowPolicyInfo)
        {
            InitializeComponent();
            IDataRow11001 = ADataRowPolicyInfo;
            GridSingleParameterPanel.PreviewMouseLeftButtonDown += GridSingleParameterPanel_PreviewMouseLeftButtonDown;
        }

        public void ShowParameterSettedInfo()
        {
            string LStr11001003 = string.Empty;     //参数编码
            string LStr11001009 = string.Empty;     //显示格式转换
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11001003 = IDataRow11001["C003"].ToString();
                StrParameterName = S1106App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                LStrParameterValueDB = IDataRow11001["C006"].ToString();
                StrParameterValue = IPageParent.ActValue2Display(IDataRow11001);
            }
            catch { }
        }

        private void GridSingleParameterPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "M001";
                    LEventArgs.ObjectSource0 = IDataRow11001;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region 属性定义
        private string _StrParameterName;
        public string StrParameterName
        {
            get { return _StrParameterName; }
            set
            {
                _StrParameterName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrParameterName");
                }
            }
        }

        private string _StrParameterValue;
        public string StrParameterValue
        {
            get { return _StrParameterValue; }
            set
            {
                _StrParameterValue = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrParameterValue");
                }
            }
        }

        /// <summary>
        /// 当前功能的信息，对应表T_11_001中的一行数据
        /// </summary>
        public DataRow IDataRow11001 { get; set; }
        #endregion
    }
}
