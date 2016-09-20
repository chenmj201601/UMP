namespace UMPClientPackage
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
            this.ClientServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ClientServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ClientServiceProcessInstaller
            // 
            this.ClientServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ClientServiceProcessInstaller.Password = null;
            this.ClientServiceProcessInstaller.Username = null;
            // 
            // ClientServiceInstaller
            // 
            this.ClientServiceInstaller.Description = "Client Componentes Integrity Check Service";
            this.ClientServiceInstaller.DisplayName = "UMP Client Package";
            this.ClientServiceInstaller.ServiceName = "UMP Client Package";
            this.ClientServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ClientServiceProcessInstaller,
            this.ClientServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ClientServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ClientServiceInstaller;
    }
}