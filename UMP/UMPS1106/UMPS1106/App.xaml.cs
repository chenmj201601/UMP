using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using UMPS1106.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1106
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        #region Memebers

        public static UMPApp CurrentApp;

        /// <summary>
        /// 分组管理方式，E虚拟分机，A座席，R真实分机；
        /// </summary>
        public static string GroupingWay;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S1106App(false);
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
