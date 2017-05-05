using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS3103.Models
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
