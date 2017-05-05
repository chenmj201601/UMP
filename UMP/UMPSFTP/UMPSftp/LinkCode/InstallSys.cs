using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.ServiceProcess;

namespace UMPCommon
{
    class IntegratedServiceInstaller
    {
        public bool _Exit = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ServiceName"></param>
        /// <param name="DisplayName"></param>
        /// <param name="Description"></param>
        /// <returns>true is service run,false install or remove</returns>
        public bool ServiceInstaller(string[] args, String ServiceName, String DisplayName, String Description)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0].IndexOf("install") != -1)
                    {
                        Install(ServiceName, DisplayName, Description,
                        //System.ServiceProcess.ServiceAccount.LocalService,    // this is more secure, but only available in XP and above and WS-2003 and above
                        System.ServiceProcess.ServiceAccount.LocalSystem,       // this is required for WS-2000
                        System.ServiceProcess.ServiceStartMode.Automatic);

                        System.ServiceProcess.ServiceController controller = null;
                        if (controller == null)
                        {
                            controller = new System.ServiceProcess.ServiceController(String.Format(ServiceName), ".");
                        }

                        return false;
                    }
                    else if (args[0].IndexOf("remove") != -1)
                    {
                        System.ServiceProcess.ServiceController controller = null;
                        if (controller == null)
                        {
                            controller = new System.ServiceProcess.ServiceController(String.Format(ServiceName), ".");
                        }
                        if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            Console.WriteLine("Please stop the service first");
                        }
                        else
                        {
                            Uninstall(ServiceName);
                        }
                        return false;
                    }
                    else if (args[0].IndexOf("service") != -1)
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public void RunCmd()
        {
            Thread thr = new Thread(new ThreadStart(() =>
            {
                Console.WriteLine("press ESC to exit...");
                while (!_Exit)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 27)
                    {
                        Console.WriteLine("Quit ...");
                        _Exit = true;
                    }
                }
            }));
            thr.Start();
        }

        void Install(String ServiceName, String DisplayName, String Description,
            System.ServiceProcess.ServiceAccount Account,
            System.ServiceProcess.ServiceStartMode StartMode)
        {
            System.ServiceProcess.ServiceProcessInstaller ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            ProcessInstaller.Account = Account;

            System.ServiceProcess.ServiceInstaller SINST = new System.ServiceProcess.ServiceInstaller();

            System.Configuration.Install.InstallContext Context = new System.Configuration.Install.InstallContext();
            string processPath = Process.GetCurrentProcess().MainModule.FileName;
            if (processPath != null && processPath.Length > 0)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(processPath);

                String path = String.Format("/assemblypath={0}", fi.FullName);
                String[] cmdline = { path };
                Context = new System.Configuration.Install.InstallContext("", cmdline);
            }

            SINST.Context = Context;
            SINST.DisplayName = String.Format("{0}", DisplayName);
            SINST.Description = String.Format("{0}", Description);
            SINST.ServiceName = String.Format("{0}", ServiceName);
            SINST.StartType = StartMode;
            SINST.Parent = ProcessInstaller;

            //          SINST.ServicesDependedOn = new String[] { "Spooler", "Netlogon", "Netman" };
            SINST.ServicesDependedOn = null;

            System.Collections.Specialized.ListDictionary state = new System.Collections.Specialized.ListDictionary();
            SINST.Install(state);

            using (RegistryKey oKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SYSTEM\CurrentControlSet\Services\{0}", ServiceName), true))
            {
                try
                {
                    //                  Object sValue = oKey.GetValue("ImagePath");
                    Object sValue = oKey.GetValue("ImagePath");
                    string str = string.Format("{0} service", sValue.ToString());
                    oKey.SetValue("ImagePath", str);
                }
                catch (Exception Ex)
                {
                    // System.Windows.Forms.MessageBox.Show(Ex.Message);
                    System.Console.WriteLine("Failed to install {0}", Ex.Message);
                }
            }

        }

        void Uninstall(String ServiceName)
        {
            System.ServiceProcess.ServiceInstaller SINST = new System.ServiceProcess.ServiceInstaller();

            System.Configuration.Install.InstallContext Context = new System.Configuration.Install.InstallContext("c:\\install.log", null);
            SINST.Context = Context;
            SINST.ServiceName = String.Format("{0}", ServiceName);
            SINST.Uninstall(null);
        }
    }
}