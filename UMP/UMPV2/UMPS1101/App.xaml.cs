using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace UMPS1101
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_LoadCompleted(object sender, NavigationEventArgs e)
        {
            MessageBox.Show("Application_LoadCompleted");
            //this.StartupUri = new Uri( "S1101MainPage.xaml", UriKind.Relative);
        }
    }
}
