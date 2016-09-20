namespace UMPService02
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
            this.Service02ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.Service02Installer = new System.ServiceProcess.ServiceInstaller();
            // 
            // Service02ProcessInstaller
            // 
            this.Service02ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Service02ProcessInstaller.Password = null;
            this.Service02ProcessInstaller.Username = null;
            // 
            // Service02Installer
            // 
            this.Service02Installer.Description = "UMP Winodws Service 02";
            this.Service02Installer.DisplayName = "UMP Service 02";
            this.Service02Installer.ServiceName = "UMP Service 02";
            this.Service02Installer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Service02ProcessInstaller,
            this.Service02Installer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller Service02ProcessInstaller;
        private System.ServiceProcess.ServiceInstaller Service02Installer;
    }
}