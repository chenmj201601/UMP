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
    public partial class UCSingleGroup : UserControl, INotifyPropertyChanged,S0000Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private string IStrGroupID = string.Empty;
        private string IStr11011005 = string.Empty;
        private DataTable IDataTalbeUserFeature = new DataTable();

        public UCSingleGroup(DataTable ADataTableUserFeature, string AStrGroupID, string AStr11011005)
        {
            InitializeComponent();
            IStrGroupID = AStrGroupID;
            IDataTalbeUserFeature = ADataTableUserFeature;
            IStr11011005 = AStr11011005;
            this.Loaded += UCSingleGroup_Loaded;
        }

        private void UCSingleGroup_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFeatureIntoWrapPanel();
        }

        private void LoadFeatureIntoWrapPanel()
        {
            string LStrCurrentStyleFolder = string.Empty;
            string LStr11003013 = string.Empty;
            string LStr11003002 = string.Empty;

            try
            {
                WrapPanelContainsFeatures.Children.Clear();
                LStrCurrentStyleFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name);
                StrFeatureGroupName = App.GetDisplayCharater(IStrGroupID + "Content");
                DataRow[] LDataRowThisGroup = IDataTalbeUserFeature.Select("C005 = '" + IStrGroupID + "'", "C004 ASC");
                foreach (DataRow LDataRowSingleFeature in LDataRowThisGroup)
                {
                    LStr11003002 = LDataRowSingleFeature["C002"].ToString();
                    LStr11003013 = LDataRowSingleFeature["C013"].ToString();

                    UCSingleFeature LUCSingleFeature = new UCSingleFeature(IStr11011005);
                    LUCSingleFeature.StrFeatureImageSource = System.IO.Path.Combine(LStrCurrentStyleFolder, LStr11003013);
                    LUCSingleFeature.StrFeatureContent = App.GetDisplayCharater("FO" + LStr11003002);
                    LUCSingleFeature.IDataRow11003 = LDataRowSingleFeature;
                    LUCSingleFeature.IOperationEvent += LUCSingleFeature_IOperationEvent;
                    LUCSingleFeature.Width = 64;
                    LUCSingleFeature.Height = 108;
                    LUCSingleFeature.Margin = new Thickness(15,3,10,2);
                    WrapPanelContainsFeatures.Children.Add(LUCSingleFeature);

                    if (LStr11003002 == IStr11011005)
                    {
                        App.GBoolExistDefaultFeature = true;
                        App.GDataRowDefaultFeature = LDataRowSingleFeature;
                        App.GStrImageDefaultFeature = System.IO.Path.Combine(LStrCurrentStyleFolder, LStr11003013);
                    }
                }
            }
            catch(Exception ex){MessageBox.Show(ex.ToString()); }
        }

        private void LUCSingleFeature_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
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

        #region 属性，xaml用到的绑定
        private string _StrFeatureGroupName;
        /// <summary>
        /// 功能组的名称
        /// </summary>
        public string StrFeatureGroupName
        {
            get { return _StrFeatureGroupName; }
            set
            {
                _StrFeatureGroupName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureGroupName");
                }
            }
        }
        #endregion

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
        
    }
}
