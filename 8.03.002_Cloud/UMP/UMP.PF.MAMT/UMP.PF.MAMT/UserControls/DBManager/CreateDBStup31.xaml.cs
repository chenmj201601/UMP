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
using System.Windows.Shapes;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup31.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup31 : Window
    {
        public CreateDBStup31()
        {
            InitializeComponent();
            this.Loaded += CreateDBStup31_Loaded;
        }

        void CreateDBStup31_Loaded(object sender, RoutedEventArgs e)
        {
            ImageCreateDatabase.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonCloseWindowTop.Click += ButtonCloseWindowTop_Click;
        }

        void ButtonCloseWindowTop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void ButtonCloseWindowButtom_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
