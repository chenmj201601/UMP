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
    public partial class UCFeatureOperationSingle : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private bool IBoolHaveMoreOperation = false;

        public UCFeatureOperationSingle()
        {
            InitializeComponent();

            GridSingleOperation.PreviewMouseLeftButtonDown += GridSingleOperation_PreviewMouseLeftButtonDown;
        }

        private void GridSingleOperation_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IBoolHaveMoreOperation)
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
                        LEventArgs.StrElementTag = this.Tag.ToString();
                        LEventArgs.ObjSource = this.DataContext;
                        IOperationEvent(this, LEventArgs);
                    }
                }
            }
            catch { }
        }

        public void ShowSingleOperationDetail(string AStrImage, string AStrName, string AStrDataInTag, object AObjectSource, DataRow[] ADataRowSubOperations)
        {
            string LStrImageFullPath = string.Empty;

            string LStrDisplayC = string.Empty;
            string LStrDisplayI = string.Empty;
            string LStrDisplayT = string.Empty;
            string LStrDataInTag = string.Empty;

            try
            {
                this.Tag = AStrDataInTag;
                this.DataContext = AObjectSource;
                TextBlockSingleOperation.Text = AStrName;

                if (ADataRowSubOperations.Count() > 0)
                {
                    IBoolHaveMoreOperation = true;
                    LStrImageFullPath = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000015.png");
                    ImageMoreOperation.Source = new BitmapImage(new Uri(LStrImageFullPath, UriKind.RelativeOrAbsolute));

                    ContextMenu LocalContextMenu = new ContextMenu();
                    foreach (DataRow LDataRowSingleOperation in ADataRowSubOperations)
                    {
                        LStrDisplayC = LDataRowSingleOperation[0].ToString();
                        LStrDisplayI = LDataRowSingleOperation[1].ToString();
                        LStrDisplayT = LDataRowSingleOperation[2].ToString();
                        LStrDataInTag = LDataRowSingleOperation[3].ToString();

                        if (!string.IsNullOrEmpty(LStrDisplayC))
                        {
                            MenuItem LMenuItemSingleOperation = new MenuItem();
                            LMenuItemSingleOperation.Header = LStrDisplayC;
                            LMenuItemSingleOperation.Tag = LStrDataInTag;
                            if (!string.IsNullOrEmpty(LStrDisplayT)) { LMenuItemSingleOperation.ToolTip = LStrDisplayT; }
                            LMenuItemSingleOperation.DataContext = AObjectSource;
                            if (!string.IsNullOrEmpty(LStrDisplayI))
                            {
                                Image LImageIcon = new Image();
                                LImageIcon.Height = 16; LImageIcon.Width = 16;
                                LImageIcon.Source = new BitmapImage(new Uri(App.GStrApplicationDirectory + @"\Images\00000008.ico", UriKind.RelativeOrAbsolute));
                                LMenuItemSingleOperation.Icon = LImageIcon;
                            }
                            LMenuItemSingleOperation.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                            LMenuItemSingleOperation.Click += LMenuItemSingleOperation_Click;

                            LocalContextMenu.Items.Add(LMenuItemSingleOperation);
                        }
                        else
                        {
                            LocalContextMenu.Items.Add(new Separator());
                        }
                    }
                    GridSingleOperation.ContextMenu = LocalContextMenu;
                }
                if (!string.IsNullOrEmpty(AStrImage))
                {
                    LStrImageFullPath = System.IO.Path.Combine(App.GStrApplicationDirectory, AStrImage);
                    ImageSingleOperation.Source = new BitmapImage(new Uri(LStrImageFullPath, UriKind.RelativeOrAbsolute));
                }
            }
            catch { }
        }

        private void LMenuItemSingleOperation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem LMenuItemOperationClicked = sender as MenuItem;
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrElementTag = LMenuItemOperationClicked.Tag.ToString();
                    LEventArgs.ObjSource = LMenuItemOperationClicked.DataContext;
                    LEventArgs.AppenObjeSource3 = "OP";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }
    }
}
