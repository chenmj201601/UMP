namespace UMPUninstall.Models
{
    /// <summary>
    /// 快捷方式信息
    /// </summary>
    public class ShotcutInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string LinkName { get; set; }
        /// <summary>
        /// 快捷方式路径
        /// </summary>
        public string LinkPath { get; set; }
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Args { get; set; }
        /// <summary>
        /// 说明（提示）
        /// </summary>
        public string Description { get; set; }
    }
}
