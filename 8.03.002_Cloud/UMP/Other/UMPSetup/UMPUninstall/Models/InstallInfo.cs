using System;

namespace UMPUninstall.Models
{
    /// <summary>
    /// 安装信息
    /// </summary>
    public class InstallInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 产品唯一标识
        /// </summary>
        public string ProductGuid { get; set; }
        /// <summary>
        /// 发布者
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 安装目录
        /// </summary>
        public string InstallDirectory { get; set; }
        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallTime { get; set; }
        /// <summary>
        /// 卸载指令
        /// </summary>
        public string UninstallString { get; set; }
        /// <summary>
        /// 更新指令
        /// </summary>
        public string ModifyPath { get; set; }
        /// <summary>
        /// 安装源
        /// </summary>
        public string InstallSource { get; set; }
    }
}
