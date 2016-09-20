//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c1425bba-fd4b-45b0-8da1-a5f19d008db2
//        CLR Version:              4.0.30319.42000
//        Name:                     Boot
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                Boot
//
//        created by Charley at 2016/1/22 10:38:51
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using VoiceCyber.UMP.Common;

namespace UMPS1201
{
    class Boot : UnityBootstrapper
    {
        private Shell mShell;

        protected override DependencyObject CreateShell()
        {
            mShell = ServiceLocator.Current.GetInstance<Shell>();
            return mShell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            ////For Wpf Desktop
            //Application.Current.MainWindow = (Shell)Shell;

            ////For Wpf Browser
            Application.Current.MainWindow.Content = Shell;

            Application.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog moduleCatalog = new ModuleCatalog();

            if (App.AppConfigs != null
                && App.AppConfigs.ListApps != null)
            {
                for (int i = 0; i < App.AppConfigs.ListApps.Count; i++)
                {
                    var app = App.AppConfigs.ListApps[i];

                    ModuleInfo moduleInfo = new ModuleInfo();
                    moduleInfo.ModuleName = app.ModuleName;
                    moduleInfo.ModuleType = app.ModuleType;
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        string.Format("Apps\\UMPS{0}\\UMPS{0}.exe", app.ModuleID));
                    moduleInfo.Ref = string.Format("file://{0}", path);
                    moduleInfo.InitializationMode = InitializationMode.OnDemand;
                    moduleCatalog.AddModule(moduleInfo);
                }
            }

            return moduleCatalog;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType(typeof(IAppControlService), typeof(AppControlService),
                new ContainerControlledLifetimeManager());
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);

            if (mShell != null)
            {
                mShell.Init();
            }
        }

        public void Close()
        {
            if (mShell != null)
            {
                mShell.Close();
            }
        }
    }
}
