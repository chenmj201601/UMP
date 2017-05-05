namespace UMPService02
{
    partial class UMPService02
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
            this.UMPServiceLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.UMPServiceLog)).BeginInit();
            // 
            // UMPServiceLog
            // 
            this.UMPServiceLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.UMPServiceLog_EntryWritten);
            // 
            // UMPService02
            // 
            this.ServiceName = "Service1";
            ((System.ComponentModel.ISupportInitialize)(this.UMPServiceLog)).EndInit();

        }

        #endregion

        public System.Diagnostics.EventLog UMPServiceLog;

    }
}
