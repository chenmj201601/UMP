namespace UMPService06
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
            this.Service06ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.Service06Installer = new System.ServiceProcess.ServiceInstaller();
            // 
            // Service06ProcessInstaller
            // 
            this.Service06ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Service06ProcessInstaller.Password = null;
            this.Service06ProcessInstaller.Username = null;
            // 
            // Service06Installer
            // 
            this.Service06Installer.Description = "UMP Winodws Service 06";
            this.Service06Installer.DisplayName = "UMP Service 06";
            this.Service06Installer.ServiceName = "UMP Service 06";
            this.Service06Installer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Service06ProcessInstaller,
            this.Service06Installer});
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller Service06ProcessInstaller;
        private System.ServiceProcess.ServiceInstaller Service06Installer;
    }
}