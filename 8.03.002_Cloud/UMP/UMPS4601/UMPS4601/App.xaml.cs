using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using UMPS4601.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4601
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
            CurrentApp = new S4601App(false);
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
