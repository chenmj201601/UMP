namespace UMPService13
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Service13ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.Service13Installer = new System.ServiceProcess.ServiceInstaller();
            // 
            // Service13ProcessInstaller
            // 
            this.Service13ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Service13ProcessInstaller.Password = null;
            this.Service13ProcessInstaller.Username = null;
            this.Service13ProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.Service13ProcessInstaller_AfterInstall);
            // 
            // Service13Installer
            // 
            this.Service13Installer.Description = "UMP Winodws Service 13";
            this.Service13Installer.DisplayName = "UMP Service 13";
            this.Service13Installer.ServiceName = "UMP Service 13";
            this.Service13Installer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.Service13Installer.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.Service13Installer_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Service13ProcessInstaller,
            this.Service13Installer});

        }

        #endregion

        public System.ServiceProcess.ServiceProcessInstaller Service13ProcessInstaller;
        public System.ServiceProcess.ServiceInstaller Service13Installer;
    }
}