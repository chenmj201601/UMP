using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Controls;

namespace UMPS3105
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
            AppealManageMainView view = new AppealManageMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
        public void SetView(UMPUserControl view)
        {
            BorderContent.Child = view;
        }
    }
}
