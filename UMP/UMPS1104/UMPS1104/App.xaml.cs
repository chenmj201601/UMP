using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using UMPS1104.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS1104
{
    /// <summary>
    /// CurrentApp.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S1104App(false);
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