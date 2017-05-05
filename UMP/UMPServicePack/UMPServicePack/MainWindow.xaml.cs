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
using UMPServicePack.PublicClasses;
using System.Xml;
using UMPServicePack.UserControls;

namespace UMPServicePack
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool mIsWinMax = true;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LabelApplicationTitle.Content = "UMP Service Pack " + App.updateInfo.Version;
            SetBusy(true);
            DrawingBackground.DrawWindowsBackgond(this);
            InitLanguage();
            UC_UpdateContent uc_UpdateContent = new UC_UpdateContent();
            borderUpdater.Child = uc_UpdateContent;
            uc_UpdateContent.main = this;
            InitLanguageMenu();

            #region 绑定事件

            MouseLeftButtonDown += (s, be) => DragMove();
            ButtonApplicationMenu.Click += ButtonApplicationMenu_Click;
            ButtonCloseApp.Click += ButtonCloseApp_Click;
            ButtonMinimized.Click += ButtonMinimized_Click;
            ButtonMaximized.Click += ButtonMaximized_Click;
            #endregion

            ChangeLanguage(App.gStrCurrLang);
        }

        #region init control
        private void InitLanguage()
        {
            lblCurrVersion.Content = App.gStrLastVersion;
            lblUpdateVersion.Content = App.updateInfo.Version;
            lblVersion.Content = App.GetLanguage("string29", "string29") + " " + App.updateInfo.Version;
        }

        /// <summary>
        /// 初始化语言菜单
        /// </summary>
        private void InitLanguageMenu()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = Application.Current.FindResource("English");
            item.Click += LanguageItem_Click;
            item.Tag = "1033";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("Chinese");
            item.Click += LanguageItem_Click;
            item.Tag = "2052";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("ChineseTaiWan");
            item.Click += LanguageItem_Click;
            item.Tag = "1028";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("Japanese");
            item.Click += LanguageItem_Click;
            item.Tag = "1041";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            ButtonApplicationMenu.ContextMenu = menu;
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 语言菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LanguageItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ChangeLanguage(item.Tag.ToString());
            MenuItem LanItem = null;
            for (int i = 0; i < ButtonApplicationMenu.ContextMenu.Items.Count; i++)
            {
                LanItem = ButtonApplicationMenu.ContextMenu.Items[i] as MenuItem;
                if (LanItem.IsChecked)
                {
                    LanItem.IsChecked = false;
                }
            }
            item.IsChecked = true;
            App.gStrCurrLang = item.Tag.ToString();

            //如果是许可协议界面，把许可协议切换语言
            try
            {
                UC_License uc_License = borderUpdater.Child as UC_License;
                uc_License.GetLicenseContent();
            }
            catch
            {
            }

            //如果是更新内容界面 把更新内容切换语言
            try
            {
                UC_UpdateContent uc_Content = borderUpdater.Child as UC_UpdateContent;
                uc_Content.InitUpdateContent();
            }
            catch
            {
            }
        }

        void ButtonApplicationMenu_Click(object sender, RoutedEventArgs e)
        {
            Button LButtonClicked = sender as Button;
            //目标   
            LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
            //位置   
            LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            LButtonClicked.ContextMenu.IsOpen = true;
        }

        void ButtonCloseApp_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(App.GetLanguage("string101", "Confirm exit upgrade?"), "UMP Service Pack", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) { return; }
            Application.Current.Shutdown();
        }

        void ButtonMinimized_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        void ButtonMaximized_Click(object sender, RoutedEventArgs e)
        {
            if (!mIsWinMax)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                ButtonMaximized.SetResourceReference(StyleProperty, "ButtonRestoreStyle");
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                ButtonMaximized.SetResourceReference(StyleProperty, "ButtonMaximizedStyle");
            }
            mIsWinMax = !mIsWinMax;
        }
        #endregion

        #region 被调用的函数
        private void ChangeLanguage(string strLangID)
        {
            try
            {
                string languagefileName = string.Format(@"Languages/{0}.xaml", strLangID);
                if (Application.Current.Resources.MergedDictionaries.Count > 0)
                {
                    Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
                   {
                       Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
                   };
                }


            }
            catch { }
        }

        public void SetBusy(bool bIsBusy)
        {
            if (bIsBusy)
            {
                myWaiter.Visibility = Visibility.Visible;
                docReady.Visibility = Visibility.Collapsed;
            }
            else
            {
                myWaiter.Visibility = Visibility.Collapsed;
                docReady.Visibility = Visibility.Visible;
            }
        }

        //public void SetUpdaterControl(object )
        #endregion
    }
}
