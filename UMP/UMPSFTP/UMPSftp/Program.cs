using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UMPCommon;

namespace UMPSftp
{
    static class Program
    {
        const string SrvName = "UMPSFTP";
        const string DisplayName = "UMP SFTP";
        const string Description = "UMP Sftp";

        static public string ServiceName { get { return SrvName; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            string name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            int id = System.Diagnostics.Process.GetCurrentProcess().Id;
            System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcessesByName(name);
            foreach (System.Diagnostics.Process pr in prc)
            {
                if ((name == pr.ProcessName) && (pr.Id != id))
                {
                    Console.WriteLine(SrvName + " is alread running!");
                    return;
                }
            }
            IntegratedServiceInstaller isi = new IntegratedServiceInstaller();
            if (args.Length > 0)
            {
                if (isi.ServiceInstaller(args, ServiceName, DisplayName, Description))
                {
                    System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] 
                    { 
                        new Service1() 
                    };
                    ServiceBase.Run(ServicesToRun);
                }
            }
            else
            {
                MainService Mainser = new MainService();
                isi.RunCmd();

                Mainser.StartWork();
                while (!isi._Exit)
                {
                    Thread.Sleep(100);
                }
                Mainser.Shutdown();
            }
            LogHelper.InfoLog("Program End.");
        }
    }
}
