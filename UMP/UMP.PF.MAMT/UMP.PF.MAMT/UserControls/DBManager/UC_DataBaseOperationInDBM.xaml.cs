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
using UMP.PF.MAMT.UserControls.DBManager;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// UC_DataBaseOperationInDBM.xaml 的交互逻辑
    /// </summary>
    public partial class UC_DataBaseOperationInDBM : UserControl
    {
        public UC_DataBaseOperationInDBM()
        {
            InitializeComponent();
            this.Loaded += UC_DataBaseOperationInDBM_Loaded;
        }

        void UC_DataBaseOperationInDBM_Loaded(object sender, RoutedEventArgs e)
        {
            imgCreateDB.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000028.ico"), UriKind.RelativeOrAbsolute));
            dpCreateDB.MouseLeftButtonDown += dpCreateDB_MouseLeftButtonDown;
        }

        void dpCreateDB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateDBStup1 stup1 = new CreateDBStup1();
            stup1.Show();
        }
    }
}
