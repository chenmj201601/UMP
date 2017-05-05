using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using UMPS6101.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls.Wcf11012;
using PFShareClassesC;
using System.ServiceModel;
using Common61011;
using UMPS6101.Wcf61011;
using System.IO;
using UMPS6101.Wcf11901;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using UMPS6101.Wcf31021;
using VoiceCyber.UMP.ScoreSheets;
using UMPS6101.DataSource.Models;
using UMPS6101.Sharing_Classes;
using UMPS6101.Wcf61012;
using UMPS6101.SharingClasses;
using UMPS1101.Models;
using VoiceCyber.UMP.Controls;

namespace UMPS6101
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S6101App(false);
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