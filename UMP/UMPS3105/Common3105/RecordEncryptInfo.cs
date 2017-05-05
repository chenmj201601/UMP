using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    /// <summary>
    /// 录音加密信息
    /// </summary>
    public class RecordEncryptInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        public bool IsRemember { get; set; }
        /// <summary>
        /// 输入密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 实际密码
        /// </summary>
        public string RealPassword { get; set; }
    }
}
