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
using System.Data;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup42.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup42 : Window
    {
        /// <summary>
        /// 所有数据库对象列表
        /// </summary>
        private DataTable dt_AllDataObjects = null;
        public CreateDBStup42(DataTable dt)
        {
            InitializeComponent();
            dt_AllDataObjects = dt;
            this.Loaded += CreateDBStup42_Loaded;
        }

        void CreateDBStup42_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ImageCreateDatabase.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonCloseWindowTop.Click += ButtonCloseWindowButtom_Click;
            ButtonNextStep.Click += ButtonNextStep_Click;
            ButtonSkipCreate.Click += ButtonSkipCreate_Click;
            
        }

        

        /// <summary>
        /// 跳过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonSkipCreate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 下一步 创建数据库完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonNextStep_Click(object sender, RoutedEventArgs e)
        {
            
        }

        void ButtonCloseWindowButtom_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
