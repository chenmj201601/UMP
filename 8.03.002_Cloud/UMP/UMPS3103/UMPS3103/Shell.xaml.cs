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
using VoiceCyber.UMP.Controls;

namespace UMPS3103
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Page
    {
        public Shell()
        {
            InitializeComponent();
            Loaded += Shell_Loaded;
        }
        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            TaskAssign view = new TaskAssign();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }

        public void SetView(UMPUserControl view)
        {
            BorderContent.Child = view;
        }
    }
}
