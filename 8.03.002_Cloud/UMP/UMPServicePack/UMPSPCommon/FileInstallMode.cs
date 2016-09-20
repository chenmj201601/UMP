using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPSPCommon
{
    /// <summary>
    /// 文件安装模式
    /// </summary>
    public enum FileInstallMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 复制，如果存在则替换
        /// </summary>
        CopyReplace = 1,
        /// <summary>
        /// 复制，如果存在忽略
        /// </summary>
        CopyIgnore = 2,
        /// <summary>
        /// 复制，如果存在报告错误
        /// </summary>
        CopyError = 3,
        /// <summary>
        /// 删除文件，如果不存在则忽略
        /// </summary>
        RemoveIgnore = 11,
        /// <summary>
        /// 删除文件，如果不存在报告错误
        /// </summary>
        RemoveError = 12,
    }
}
