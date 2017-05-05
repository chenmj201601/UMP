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
    /// UC_ServerOperation.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ServerOperation : UserControl
    {
        public UC_LanManager LanManagerWindow;

        public UC_ServerOperation(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_ServerOperation_Loaded;
        }

        void UC_ServerOperation_Loaded(object sender, RoutedEventArgs e)
        {
            imgOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000018.ico"), UriKind.RelativeOrAbsolute));
            imgConnToUmp.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000012.ico"), UriKind.RelativeOrAbsolute));
            imgDisConnToUmp.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000011.ico"), UriKind.RelativeOrAbsolute));
            imgConnToDB.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000019.ico"), UriKind.RelativeOrAbsolute));
            imgRefersh.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000013.ico"), UriKind.RelativeOrAbsolute));

            //定义单击事件
            dpConnToDB.MouseLeftButtonDown += dpConnToDB_MouseLeftButtonDown;
            dpConnToUmp.MouseLeftButtonDown += dpConnToUmp_MouseLeftButtonDown;
            dpDisConnToUmp.MouseLeftButtonDown += dpDisConnToUmp_MouseLeftButtonDown;
            dpRefersh.MouseLeftButtonDown += dpRefersh_MouseLeftButtonDown;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpRefersh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanManagerWindow.lblState.Content = TryFindResource("Refreshing").ToString();
            //如果需要刷新语言包
            if (LanManagerWindow.dpData.Children.Count > 0)
            {
                UC_LanguageData uc_data = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
                Common.RefershData(uc_data.lvLanguage);
                uc_data.lstViewrSource = uc_data.lvLanguage.ItemsSource as List<LanguageInfo>;
            }
            LanManagerWindow.InitControl();
            LanManagerWindow.lblState.Content = TryFindResource("RefreshDone").ToString();
        }

        /// <summary>
        /// 断开UMP服务器的连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpDisConnToUmp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// 连接到UMP服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpConnToUmp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ConfigurationTypeSelect se = new ConfigurationTypeSelect();
            //se.ShowDialog();
        }

        /// <summary>
        /// 连接到新数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpConnToDB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
