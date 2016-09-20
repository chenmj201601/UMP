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
using UMP.PF.MAMT.Classes;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_DBOperation.xaml 的交互逻辑
    /// </summary>
    public partial class UC_DBOperation : UserControl
    {
        public UC_LanManager LanManagerWindow;

        public UC_DBOperation(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_DBOperation_Loaded;
        }

        void UC_DBOperation_Loaded(object sender, RoutedEventArgs e)
        {
            imgLanOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000025.ico"), UriKind.RelativeOrAbsolute));
            imgImport.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000020.ico"), UriKind.RelativeOrAbsolute));
            imgExport.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000021.ico"), UriKind.RelativeOrAbsolute));

            dpImport.MouseLeftButtonDown += dpImport_MouseLeftButtonDown;
            dpExport.MouseLeftButtonDown += dpExport_MouseLeftButtonDown;
        }

        //打开导出语言包界面
        void dpExport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_ExportLan uc_ExportLan = new UC_ExportLan();
            LanManagerWindow.dpDetail.Children.Clear();
            LanManagerWindow.dpDetail.Children.Add(uc_ExportLan);
            UC_ConfirmExportLan uc_confirmExpLan = new UC_ConfirmExportLan(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(uc_confirmExpLan);
        }


        /// <summary>
        /// 打开导入语言包界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UC_ImportLan ucImportLan = new UC_ImportLan();
            LanManagerWindow.dpDetail.Children.Clear();
            LanManagerWindow.dpDetail.Children.Add(ucImportLan);
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_SaveImport uc_ConfirmImportLan = new UC_SaveImport(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(uc_ConfirmImportLan);
        }
    }
}
