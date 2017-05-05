using System.Windows;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App 
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S4412App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Current != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }
    }
}
