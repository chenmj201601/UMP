namespace UMPService01
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
            this.Service01ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.Service01Installer = new System.ServiceProcess.ServiceInstaller();
            // 
            // Service01ProcessInstaller
            // 
            this.Service01ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Service01ProcessInstaller.Password = null;
            this.Service01ProcessInstaller.Username = null;
            // 
            // Service01Installer
            // 
            this.Service01Installer.Description = "UMP Winodws Service 01";
            this.Service01Installer.DisplayName = "UMP Service 01";
            this.Service01Installer.ServiceName = "UMP Service 01";
            this.Service01Installer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Service01ProcessInstaller,
            this.Service01Installer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller Service01ProcessInstaller;
        private System.ServiceProcess.ServiceInstaller Service01Installer;
    }
}