using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPSPCommon
{
    /// <summary>
    /// ErrorReply的类型
    /// </summary>
    public enum ErrorMode
    {
        /// <summary>
        /// 遇到错误不做任何处理
        /// </summary>
        None = 0,
        /// <summary>
        /// 遇到错误记录到日志文件中以供参考
        /// </summary>
        LogFile = 1,
        /// <summary>
        /// 遇到错误弹窗提示
        /// </summary>
        MessageBox = 2,
    }
}
