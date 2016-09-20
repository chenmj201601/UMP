using System.Windows;
using VoiceCyber.UMP.Controls;

namespace UMPS3602
{
    
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        #region Memebers

        public static UMPApp CurrentApp;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S3602App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (CurrentApp != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }
    }
}
