using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using PFShareClassesC;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11021;
using VoiceCyber.UMP.Communications;
using UMPS1102.Wcf11012;
using UMPS1102.Wcf11901;
using UMPS1102.Wcf11021;
using VoiceCyber.UMP.Controls;

namespace UMPS1102
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S1102App(false);
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
