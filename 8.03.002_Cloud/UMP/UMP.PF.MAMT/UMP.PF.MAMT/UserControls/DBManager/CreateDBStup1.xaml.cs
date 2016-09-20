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
using UMP.PF.MAMT.Classes;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup1.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup1 : Window
    {
        public CreateDBStup1()
        {
            InitializeComponent();
            this.Loaded += CreateDBStup1_Loaded;
        }

        void CreateDBStup1_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ImageCreateDatabase.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            ButtonCloseWindowTop.Click += ButtonCloseWindowTop_Click;
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonNextStep.Click += ButtonNextStep_Click;
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonNextStep_Click(object sender, RoutedEventArgs e)
        {
            //判断要创建的数据库是什么类型  MSSQL进入CreateDBStup21  Oracle进入CreateDBStup22
            if (RadMSSQL.IsChecked == true)
            {
                App.GCreatingDBInfo.DbType = (int)Enums.DBType.MSSQL;
                CreateDBStup21 winStup21 = new CreateDBStup21();
                winStup21.Show();
                this.Close();
                return;
            }
            else if (RadOracle.IsChecked == true)
            {
                App.GCreatingDBInfo.DbType = (int)Enums.DBType.Oracle;
                CreateDBStup22 winStup22 = new CreateDBStup22();
                winStup22.Show();
                this.Close();
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
