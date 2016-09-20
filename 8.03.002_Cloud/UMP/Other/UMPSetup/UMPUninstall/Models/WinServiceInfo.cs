namespace UMPUninstall.Models
{
    /// <summary>
    /// Windows服务信息
    /// </summary>
    public class WinServiceInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
