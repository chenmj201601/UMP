using PFShareControls;
using System;
using System.Collections.Generic;
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

namespace PFShareControlsDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PFShareControls.UCTreeViewYoung LUCTreeViewYoung = new UCTreeViewYoung();
            GridTreeView.Children.Add(LUCTreeViewYoung);

            TreeViewItem LTreeViewItemRoot1 = LUCTreeViewYoung.AddTreeViewItem(null, false, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "RootItem1", 1020000000000000001);

            TreeViewItem LTreeViewItemSub101 = LUCTreeViewYoung.AddTreeViewItem(LTreeViewItemRoot1, false, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "SubItem101", 1020000000000000002);
            TreeViewItem LTreeViewItemSub102 = LUCTreeViewYoung.AddTreeViewItem(LTreeViewItemRoot1, false, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "SubItem102", 1020000000000000003);

            TreeViewItem LTreeViewItemRoot2 = LUCTreeViewYoung.AddTreeViewItem(null, false, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "RootItem1", 1020000000000000101);

            //TreeViewItem LTreeViewItemSub201 = LUCTreeViewYoung.AddTreeViewItem(LTreeViewItemRoot2, true, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "SubItem101", 1020000000000000102);
            //TreeViewItem LTreeViewItemSub202 = LUCTreeViewYoung.AddTreeViewItem(LTreeViewItemRoot2, false, @"C:\ProgramData\UMP.Client\Themes\Style04\Images\S1103\S1103000.ico", "SubItem102", 1020000000000000103);

            LUCTreeViewYoung.SetItemCheckedStatus(true, 1020000000000000003);
            LUCTreeViewYoung.SetItemCheckedStatus(true, 1020000000000000101);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UCTreeViewYoung LUCTreeView = GridTreeView.Children[0] as UCTreeViewYoung;
            List<object> LListObjectSelected = LUCTreeView.GetCheckedOrUnCheckedItem("1");
            return;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UCTreeViewYoung LUCTreeView = GridTreeView.Children[0] as UCTreeViewYoung;
            List<object> LListObjectSelected = LUCTreeView.GetCheckedOrUnCheckedItem("0");
            return;
        }

       
    }
}
