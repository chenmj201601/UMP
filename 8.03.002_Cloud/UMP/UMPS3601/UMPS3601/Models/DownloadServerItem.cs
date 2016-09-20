namespace UMPS3601.Models
{
    public class DownloadServerItem
    {
        /// <summary>
        /// 方式
        /// 0       SftpServer
        /// 1       DownloadParam（NAS）
        /// </summary>
        public int Type { get; set; }
        public object Info { get; set; }
    }
}
