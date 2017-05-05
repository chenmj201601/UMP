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
using UMP.PF.MAMT.WCF_LanPackOperation;
using UMP.PF.MAMT.Classes;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup22.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup22 : Window
    {
        public CreateDBStup22()
        {
            InitializeComponent();
            this.Loaded += CreateDBStup22_Loaded;
        }

        void CreateDBStup22_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ImageDBConnectProfile.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonCloseWindowTop.Click += ButtonCloseWindowTop_Click;
            ButtonNextStep.Click += ButtonNextStep_Click;
            txtServerHost.Text = "192.168.4.182";
            txtServicName.Text = "PFOrcl";
            txtLoginName.Text = "PFDEV";
            txtLoginPassword.Password = "PF,123";
        }

        /// <summary>
        /// 下一步  检测是否能连上数据库服务器 如果能连上 则进入下一步 不能则报错
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonNextStep_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerHost.Text) || string.IsNullOrEmpty(txtServerPort.Text) ||
                string.IsNullOrEmpty(txtLoginName.Text) || string.IsNullOrEmpty(txtLoginPassword.Password))
            {
                MessageBox.Show(TryFindResource("Error014").ToString(), TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            App.GCreatingDBInfo.Host = txtServerHost.Text;
            App.GCreatingDBInfo.Port = txtServerPort.Text;
            App.GCreatingDBInfo.ServiceName = txtServicName.Text;
            App.GCreatingDBInfo.LoginName = txtLoginName.Text;
            App.GCreatingDBInfo.Password = txtLoginPassword.Password;
            ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 8, App.GCreatingDBInfo);
            if (result.BoolReturn)
            {
                CreateDBStup32 stup32 = new CreateDBStup32();
                stup32.Show();
                this.Close();
            }
            else
            {
                string strMsg = string.Format(TryFindResource("Error015").ToString(),result.StringReturn);
                MessageBox.Show(strMsg, TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
