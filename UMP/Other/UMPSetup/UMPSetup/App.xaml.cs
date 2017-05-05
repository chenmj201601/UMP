using System;
using System.Windows;
using Microsoft.Win32;
using VoiceCyber.UMP.Updates;

namespace UMPSetup
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var isInstalled = CheckInstalled();
            if (isInstalled)
            {
                MessageBox.Show(string.Format("UMP has installed on this machine, please update UMP or uninstall it!"));
                Shutdown(0);
                return;
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
                    path = string.Format(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0}",
                        UpdateConsts.PACKAGE_GUID_UMP);
                }
                else
                {
                    path = string.Format(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0}",
                        UpdateConsts.PACKAGE_GUID_UMP);
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
