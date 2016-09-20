using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePackCommon
{
    /// <summary>
    /// UMP安装程序信息类
    /// </summary>
    public class UMPAppInfo
    {
        private string _AppName;

        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName
        {
            get { return _AppName; }
            set { _AppName = value; }
        }
        private string _AppInstallPath;

        /// <summary>
        /// 安装路径
        /// </summary>
        public string AppInstallPath
        {
            get { return _AppInstallPath; }
            set { _AppInstallPath = value; }
        }
        private string _AppVersion;

        /// <summary>
        /// 安装版本
        /// </summary>
        public string AppVersion
        {
            get { return _AppVersion; }
            set { _AppVersion = value; }
        }
    }
}
