using System;
using System.Windows;
using Microsoft.Win32;

namespace UMPSetup
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var args = e.Args;
            if (args.Length > 0)
            {
                string arg1 = args[0];
                if (arg1.ToUpper() == "/M")
                {
                    //StartupUri = new Uri("UninstallWindow.xaml", UriKind.RelativeOrAbsolute);
                }
            }
            base.OnStartup(e);
        }

        private bool CheckInstalled()
        {
            bool bReturn = false;
            try
            {
                bool is64BitOS = Environment.Is64BitOperatingSystem;
                string path;
                if (is64BitOS)
                {
                    path = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\UMP";
                }
                else
                {
                    path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\UMP";
                }
                RegistryKey rootKey = Registry.LocalMachine;
                RegistryKey umpKey = rootKey.OpenSubKey(path);
                if (umpKey != null)
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                bReturn = false;
            }
            return bReturn;
        }
    }
}
