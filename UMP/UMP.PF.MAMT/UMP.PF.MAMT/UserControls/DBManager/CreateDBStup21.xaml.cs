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
using System.ServiceModel;
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_LanPackOperation;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup2.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup21 : Window
    {
        public CreateDBStup21()
        {
            InitializeComponent();
            this.Loaded += CreateDBStup21_Loaded;
        }

        void CreateDBStup21_Loaded(object sender, RoutedEventArgs e)
        {
            ImageDBConnectProfile.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ButtonCloseWindowTop.Click += ButtonCloseWindowTop_Click;
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonNextStep.Click += ButtonNextStep_Click;
        }

        void ButtonNextStep_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtServerName.Text) || string.IsNullOrEmpty(txtServerPort.Text) ||
                string.IsNullOrEmpty(txtLoginName.Text) || string.IsNullOrEmpty(txtLoginPassword.Password))
            {
                MessageBox.Show(TryFindResource("").ToString(), TryFindResource("").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        void ButtonCloseWindowButtom_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void ButtonCloseWindowTop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
