using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using PFShareClassesC;
using UMPS1101.Models;
using UMPS1101.Wcf11012;
using UMPS1101.Wcf11901;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S1101App(false);
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
