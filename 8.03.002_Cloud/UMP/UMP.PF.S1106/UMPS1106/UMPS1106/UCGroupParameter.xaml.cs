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

namespace UMPS1106
{
    public partial class UCGroupParameter : UserControl, INotifyPropertyChanged, S1106Interface
    {
        public Page00000A IPageParent = null;

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public string IStrGroupID = string.Empty;

        public UCGroupParameter()
        {
            InitializeComponent();
            GridGroupTitle.PreviewMouseLeftButtonDown += GridGroupTitle_PreviewMouseLeftButtonDown;
        }

        public void ShowGroupParameters(DataTable ADataTable11001, string AStrGroupID)
        {
            string LStr11001003 = string.Empty;

            try
            {
                IStrGroupID = AStrGroupID;
                StrGroupName = App.GetDisplayCharater("UCGroupParameter", "G" + IStrGroupID);
                DataRow[] LDataRowAllParameters = ADataTable11001.Select("C004 = " + IStrGroupID, "C005 ASC");
                foreach (DataRow LDataRowSingleParameter in LDataRowAllParameters)
                {
                    LStr11001003 = LDataRowSingleParameter["C003"].ToString();
                    if (LStr11001003 == "11020104") { continue; }
                    if (LStr11001003 == "11030103") { continue; }
                    UCSingleParameter LUCSingleParameter = new UCSingleParameter(LDataRowSingleParameter);
                    LUCSingleParameter.IPageParent = this.IPageParent;
                    LUCSingleParameter.IOperationEvent += LUCSingleParameter_IOperationEvent;
                    LUCSingleParameter.ShowParameterSettedInfo();
                    LUCSingleParameter.Margin = new Thickness(0, 1, 0, 1);
                    StackPanelContainsParameters.Children.Add(LUCSingleParameter);
                }
            }
            catch { }
        }

        private void LUCSingleParameter_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }

        private void GridGroupTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (StackPanelContainsParameters.Visibility == System.Windows.Visibility.Collapsed)
                {
                    StackPanelContainsParameters.Visibility = System.Windows.Visibility.Visible;
                    ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowDownStyle"];
                }
                else
                {
                    StackPanelContainsParameters.Visibility = System.Windows.Visibility.Collapsed;
                    ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowUpStyle"];
                }
            }
            catch { }
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
        private string _StrGroupName;
        public string StrGroupName
        {
            get { return _StrGroupName; }
            set
            {
                _StrGroupName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrGroupName");
                }
            }
        }
        #endregion
    }
}
