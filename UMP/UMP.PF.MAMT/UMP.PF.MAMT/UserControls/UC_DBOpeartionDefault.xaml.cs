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
    /// UC_DBOpeartionDefault.xaml 的交互逻辑
    /// </summary>
    public partial class UC_DBOpeartionDefault : UserControl
    {
        public UC_LanManager LanManagerWindow;

        public UC_DBOpeartionDefault(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_DBOpeartionDefault_Loaded;
        }

        void UC_DBOpeartionDefault_Loaded(object sender, RoutedEventArgs e)
        {
            //imgAdd.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000022.ico"), UriKind.RelativeOrAbsolute));
            imgEdit.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000023.ico"), UriKind.RelativeOrAbsolute));
            imgOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000018.ico"), UriKind.RelativeOrAbsolute));
            //dpAdd.MouseLeftButtonDown += dpAdd_MouseLeftButtonDown;
            dpEdit.MouseLeftButtonDown += dpEdit_MouseLeftButtonDown;
        }

        /// <summary>
        /// 打开编辑语言项的界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.GLstLanguageItemInEdit.Count <= 0)
            {
                MessageBox.Show(this.TryFindResource("SelectLanguage").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_EditLanguageItem uc_ExportLan = new UC_EditLanguageItem(LanManagerWindow,App.GLstLanguageItemInEdit);
            LanManagerWindow.dpDetail.Children.Clear();
            LanManagerWindow.dpDetail.Children.Add(uc_ExportLan);
            UC_ConfirmEdit uc_confirmEdit = new UC_ConfirmEdit(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(uc_confirmEdit);
        }

        /// <summary>
        /// 打开添加语言项的界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
