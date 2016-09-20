using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPMainEntrance.BasicModule
{
    public partial class UserLoginPage : Page
    {
        public UserLoginPage()
        {
            InitializeComponent();
            
            this.Loaded += UserLoginPage_Loaded;
            ButtonLoginSystem.Click += ElementButtonClicked;
            ButtonLoginOptions.Click += ElementButtonClicked;
        }

        private void ElementButtonClicked(object sender, RoutedEventArgs e)
        {
            string LStrSender = string.Empty;
            

            try
            {
                LStrSender = (sender as Button).Name;
                if (LStrSender == "ButtonLoginSystem")
                {
                    this.NavigationService.Navigate(new Uri(@"/BasicModule/UMPMainPage.xaml", UriKind.Relative));
                }
                if (LStrSender == "ButtonLoginOptions")
                {
                    //using (System.Drawing.Graphics LGraphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                    //{
                    //    float dpiX = LGraphics.DpiX;
                    //    float dpiY = LGraphics.DpiY;
                    //    MessageBox.Show(dpiX.ToString() + " - " + dpiY.ToString());
                    //}
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UserLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard LStoryboard = this.Resources["AppLogoLeave"] as Storyboard;
            LStoryboard.Begin();
            TextBoxLoginAccount.Focus();

        }
    }
}
