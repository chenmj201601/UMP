using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace UMPService13
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void Service13ProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void Service13Installer_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
