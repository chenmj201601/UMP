using System.Windows;
using System.Windows.Controls;

namespace UMPS3604
{
    /// <summary>
    /// Interaction logic for Shell.xaml
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
            MaterialLibraryView view = new MaterialLibraryView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
