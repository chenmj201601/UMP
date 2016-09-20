using System;
using System.Collections.Generic;
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
using UMP.Tools.PublicClasses;

namespace UMP.Tools.BasicControls
{
    public partial class UCFeatureOperationGroup : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCFeatureOperationGroup()
        {
            InitializeComponent();
            this.Loaded += UCFeatureOperationGroup_Loaded;
            GridGroupOperationsName.PreviewMouseLeftButtonDown += GridGroupOperationsName_PreviewMouseLeftButtonDown;
        }

        private void UCFeatureOperationGroup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000016.png"), UriKind.RelativeOrAbsolute));
            }
            catch { }
        }

        private void GridGroupOperationsName_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (StackPanelGroupOperationList.Visibility == Visibility.Collapsed)
                {
                    StackPanelGroupOperationList.Visibility = Visibility.Visible;
                    ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000016.png"), UriKind.RelativeOrAbsolute));
                }
                else
                {
                    StackPanelGroupOperationList.Visibility = Visibility.Collapsed;
                    ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000014.png"), UriKind.RelativeOrAbsolute));
                }
            }
            catch { }
        }

        public void ShowObjectAllOperations(DataTable ADataTableOperationsLoaded, object AObjectSource, string AStrMethod)
        {
            string LStrDisplayC = string.Empty;
            string LStrDisplayI = string.Empty;
            string LStrDisplayT = string.Empty;
            string LStrDataInTag = string.Empty;
            int LIntMoreOperationID = 0;

            string LStrDisplayM = string.Empty;

            try
            {
                if (AObjectSource == null) { LStrDisplayC = ADataTableOperationsLoaded.Rows[0][0].ToString(); }
                else { LStrDisplayC = GetObjectContentForExpanderHeader(AObjectSource); }

                if (LStrDisplayC == "ApplicationName") { LStrDisplayC = App.GStrApplicationReferredTo; }
                //else { if (!string.IsNullOrEmpty(LStrDisplayC)) { LStrDisplayC = App.GetDisplayCharater(LStrDisplayC); } }

                TextBlockGroupOperationsName.Text = LStrDisplayC;
                DataContext = AObjectSource;

                if (AStrMethod == "A") { LStrDisplayM = "(DisplayM = 'A' or DisplayM = 'B' or DisplayM = 'O')"; }
                if (AStrMethod == "B") { LStrDisplayM = "(DisplayM = 'A' or DisplayM = 'B')"; }
                if (AStrMethod == "O") { LStrDisplayM = "(DisplayM = 'A' or DisplayM = 'O')"; }

                DataRow[] LDataRowSubOperations = ADataTableOperationsLoaded.Select("MoreOp >= 0 and MoreOp < 100 and " + LStrDisplayM, "OrderID ASC");
                foreach (DataRow LDataRowSingleOperation in LDataRowSubOperations)
                {
                    LStrDisplayC = LDataRowSingleOperation[0].ToString();
                    LStrDisplayI = LDataRowSingleOperation[1].ToString();
                    LStrDisplayT = LDataRowSingleOperation[2].ToString();
                    LStrDataInTag = LDataRowSingleOperation[3].ToString();
                    LIntMoreOperationID = int.Parse(LDataRowSingleOperation[4].ToString());

                    if (!string.IsNullOrEmpty(LStrDisplayC))
                    {
                        if (LStrDisplayC != "ApplicationName") { LStrDisplayC = App.GetDisplayCharater(LStrDisplayC); }
                        DataRow[] LDataRowMoreOperations = ADataTableOperationsLoaded.Select("MoreOp = " + (100 + LIntMoreOperationID).ToString(), "OrderID ASC");
                        UCFeatureOperationSingle LUCSingleOperation = new UCFeatureOperationSingle();
                        LUCSingleOperation.IOperationEvent += LUCSingleOperationPanel_IOperationEvent;
                        LUCSingleOperation.ShowSingleOperationDetail(LStrDisplayI, LStrDisplayC, LStrDataInTag, AObjectSource, LDataRowMoreOperations);
                        LUCSingleOperation.Margin = new Thickness(0, 2, 0, 1);
                        StackPanelGroupOperationList.Children.Add(LUCSingleOperation);
                    }
                    else
                    {
                        if (StackPanelGroupOperationList.Children.Count == 0) { continue; }
                        //显示一间隔条
                        Label LLableSpliter = new Label();
                        LLableSpliter.Style = (Style)App.Current.Resources["LabelForSpliterStyle"];

                        StackPanelGroupOperationList.Children.Add(LLableSpliter);
                    }
                }
            }
            catch { }
        }

        private string GetObjectContentForExpanderHeader(object AObjectSource)
        {
            string LStrReturn = string.Empty;
            string LStrTableName = string.Empty;

            try
            {
                Type LTypeObject = AObjectSource.GetType();
                if (LTypeObject == typeof(TreeViewItem))
                {
                    TreeViewItem LTreeViewItem = AObjectSource as TreeViewItem;
                    LStrReturn = LTreeViewItem.Header.ToString();
                }
                if (LTypeObject == typeof(DataRow))
                {
                    DataRow LDataRow = AObjectSource as DataRow;
                    LStrReturn = LDataRow[0].ToString();
                }
                if (LTypeObject == typeof(DataTable))
                {
                    DataTable LDataTable = AObjectSource as DataTable;
                    LStrTableName = LDataTable.TableName;
                    if (LStrTableName.Contains("DBObjectType"))
                    {
                        LStrReturn = App.GetConvertedData(LStrTableName);
                    }
                }
            }
            catch { LStrReturn = App.GStrApplicationReferredTo; }
            return LStrReturn;
        }

        private void LUCSingleOperationPanel_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }
    }
}
