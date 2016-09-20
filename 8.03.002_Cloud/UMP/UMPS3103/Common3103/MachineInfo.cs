using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    /// <summary>
    /// Sftp服务器信息
    /// </summary>
    public class SftpServerInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string HostAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int HostPort { get; set; }
    }
}
