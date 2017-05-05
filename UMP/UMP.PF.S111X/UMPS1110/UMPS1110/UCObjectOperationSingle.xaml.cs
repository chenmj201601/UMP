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

namespace UMPS1110
{
    public partial class UCObjectOperationSingle : UserControl, S1110Interface
    {
        private string IStrOperationID = string.Empty;

        private bool IBoolHaveMoreOperations = false;

        private object IObjectCurrent = null;

        private string IStrImageFolder = string.Empty;

        private OperationParameters IOperationParameters = null;

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCObjectOperationSingle(string AStrOperationID, object AObjectCurrent)
        {
            InitializeComponent();
            IStrOperationID = AStrOperationID;
            IObjectCurrent = AObjectCurrent;
            GridObjectOperationSingle.PreviewMouseLeftButtonDown += GridObjectOperationSingle_PreviewMouseLeftButtonDown;
            IStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1110");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AParameters">
        /// OperationParameters.ObjectSource1 权限列表 - DataTable
        /// </param>
        public void ShowOperationDetails(OperationParameters AParameters)
        {
            TextBlockOperationName.Text = App.GetDisplayCharater("FO" + IStrOperationID);
            IOperationParameters = AParameters;
            InitThisContextMenu();
        }

        private void InitThisContextMenu()
        {
            string LStrOperationID = string.Empty;
            string LStrDisplayImageName = string.Empty;

            DataTable LDataTableOperations = IOperationParameters.ObjectSource1 as DataTable;
            DataRow[] LDataRowOperations = LDataTableOperations.Select("C003 = " + IStrOperationID, "C004 ASC");
            if (LDataRowOperations.Length <= 0)
            {
                TextBlockOperationName.Style = (Style)App.Current.Resources["YTextBlockOperationNameNoRightStyle"];
                ImageMoreOperation.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            else
            {
                TextBlockOperationName.Style = (Style)App.Current.Resources["YTextBlockOperationNameWithRightStyle"];
                ImageMoreOperation.Visibility = System.Windows.Visibility.Visible;
                LStrDisplayImageName = System.IO.Path.Combine(IStrImageFolder, "S1110008.png");
                ImageMoreOperation.Source = new BitmapImage(new Uri(LStrDisplayImageName, UriKind.RelativeOrAbsolute));
            }

            ContextMenu LocalContextMenu = new ContextMenu();
            foreach (DataRow LDataRowSingleOperation in LDataRowOperations)
            {
                LStrOperationID = LDataRowSingleOperation["C002"].ToString();
                LStrDisplayImageName = LDataRowSingleOperation["C013"].ToString();
                MenuItem LMenuItemSingleOperation = new MenuItem();
                LMenuItemSingleOperation.Header = App.GetDisplayCharater("FO" + LStrOperationID);
                LMenuItemSingleOperation.DataContext = LStrOperationID;
                LMenuItemSingleOperation.Tag = IOperationParameters;
                if (!string.IsNullOrEmpty(LStrDisplayImageName))
                {
                    Image LImageIcon = new Image();
                    LImageIcon.Height = 16; LImageIcon.Width = 16;
                    LImageIcon.Source = new BitmapImage(new Uri(System.IO.Path.Combine(IStrImageFolder, LStrDisplayImageName), UriKind.RelativeOrAbsolute));
                    LMenuItemSingleOperation.Icon = LImageIcon;
                }
                LMenuItemSingleOperation.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                LMenuItemSingleOperation.Click += LMenuItemSingleOperation_Click;
                LocalContextMenu.Items.Add(LMenuItemSingleOperation);
            }
            GridObjectOperationSingle.ContextMenu = LocalContextMenu;
        }

        private void LMenuItemSingleOperation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem LMenuItemOperationClicked = sender as MenuItem;
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = LMenuItemOperationClicked.DataContext.ToString();
                    LEventArgs.ObjectSource0 = IObjectCurrent;
                    LEventArgs.ObjectSource1 = LMenuItemOperationClicked.Tag;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        private void GridObjectOperationSingle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IBoolHaveMoreOperations)
                {
                    Grid ClickedGrid = sender as Grid;
                    //目标   
                    ClickedGrid.ContextMenu.PlacementTarget = ClickedGrid;
                    //位置   
                    ClickedGrid.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    //显示菜单   
                    ClickedGrid.ContextMenu.IsOpen = true;
                }
                else
                {
                    if (IOperationEvent != null)
                    {
                        OperationEventArgs LEventArgs = new OperationEventArgs();
                        LEventArgs.StrObjectTag = IStrOperationID;
                        LEventArgs.ObjectSource0 = IObjectCurrent;
                        LEventArgs.ObjectSource1 = IOperationParameters;
                        IOperationEvent(this, LEventArgs);
                    }
                }
            }
            catch { }
        }
        
    }
}
