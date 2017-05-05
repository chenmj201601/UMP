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
using System.IO;

namespace UMPServicePack.UserControls
{
    /// <summary>
    /// UC_License.xaml 的交互逻辑
    /// </summary>
    public partial class UC_License : UserControl
    {
        public MainWindow main = null;
        public UC_License()
        {
            InitializeComponent();
            Loaded += UC_License_Loaded;
        }

        void UC_License_Loaded(object sender, RoutedEventArgs e)
        {
            GetLicenseContent();

            #region 控件事件
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            btnTermination.Click += btnTermination_Click;
            #endregion
        }

        #region Init
        /// <summary>
        /// 获得许可协议的内容
        /// </summary>
        public void GetLicenseContent()
        {
            string strFileName = "Languages/License_{0}.txt";
            strFileName = string.Format(strFileName, App.gStrCurrLang);
            var licenseFile = App.GetResourceStream(new Uri(strFileName,UriKind.Relative));
            if (licenseFile == null)
            {
                App.ShowException(App.GetLanguage("string36", "string36"));
                return;
            }
            StreamReader reader = new StreamReader(licenseFile.Stream);
            txtLicense.Text = reader.ReadToEnd();
            reader.Close();
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTermination_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (chkAgree.IsChecked == false)
            {
                App.ShowException(App.GetLanguage("string35", "string35"));
                return;
            }
            UC_BackupPathChoose uc_Path = new UC_BackupPathChoose();
            uc_Path.main = main;
            main.borderUpdater.Child = uc_Path;
        }

        /// <summary>
        /// 上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            UC_UpdateContent uc_Content = new UC_UpdateContent();
            uc_Content.main = main;
            main.borderUpdater.Child = uc_Content;
            App.WriteLog("previous,to update content page");
        }
        #endregion
       

    }
}
