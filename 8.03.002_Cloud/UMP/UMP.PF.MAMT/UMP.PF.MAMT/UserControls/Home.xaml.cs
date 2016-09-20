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
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        public event IntoModel IntoModel_Click;
        public Home()
        {
            InitializeComponent();
            this.Loaded += Home_Loaded;
        }

        void Home_Loaded(object sender, RoutedEventArgs e)
        {
            InitControl();
        }

        /// <summary>
        /// 初始化界面上的控件
        /// </summary>
        private void InitControl()
        {
            //配置数据库的img
            StackPanel sp = CreateModel(@"Images\00000007.ico", "DBManager");
            sp.MouseLeftButtonDown += DBManagerMouseLeftButtonDown;
            wpContent.Children.Add(sp);
            sp = CreateModel(@"Images\00000008.png", "IISManager");
            sp.MouseLeftButtonDown += IISManager_MouseLeftButtonDown;
            wpContent.Children.Add(sp);
            if (!AboutIIS.VerifyWebSiteIsExist())
            {
                sp.Visibility = Visibility.Collapsed;
            }
            sp = CreateModel(@"Images\00000009.ico", "LanguageManager");
            sp.MouseLeftButtonDown += LanguageManager_MouseLeftButtonDown;
            wpContent.Children.Add(sp);

        }

        /// <summary>
        /// 创建功能模块的图和文字
        /// </summary>
        /// <param name="strImgUrl"></param>
        /// <param name="strModelName"></param>
        /// <returns></returns>
        private StackPanel CreateModel(string strImgUrl, string strModelName)
        {
            StackPanel sp = new StackPanel();
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, strImgUrl), UriKind.RelativeOrAbsolute));
            img.Width = 128;
            img.Height = 128;
            img.Margin = new Thickness(20);
            sp.SetResourceReference(StackPanel.StyleProperty, "ModelStackPanelStyle");
            sp.Children.Add(img);
            Label lbl = new Label();
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbl.SetResourceReference(Label.ContentProperty, strModelName);
            lbl.SetResourceReference(Label.StyleProperty, "ControlBaseStyle");
            sp.Children.Add(lbl);
            return sp;
        }

        /// <summary>
        /// 进入语言包管理模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LanguageManager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntoModel_Click(sender, Enums.Model.LanguageModel);
        }

        /// <summary>
        /// 进入IIS设置模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IISManager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntoModel_Click(sender, Enums.Model.IISModel);
        }

        /// <summary>
        /// 进入数据库管理模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DBManagerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntoModel_Click(sender, Enums.Model.DBModel);
        }
    }

    public delegate void IntoModel(object sender, Enums.Model e);
}
