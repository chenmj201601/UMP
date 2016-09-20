using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.PF.MAMT.Classes
{
    public static class AboutComputer
    {
        /// <summary>
        /// 获取当前机器操作系统信息
        /// </summary>
        /// <returns></returns>
        public static ComputerInformation GetOSInformation()
        {
            ComputerInformation LInfoReturn = new ComputerInformation();

            try
            {
                LInfoReturn.StrErrorReturn = string.Empty;
                RegistryKey LRegistryKeyLocalMachine = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
                LInfoReturn.StrVersion = LRegistryKeyLocalMachine.GetValue("CurrentVersion").ToString();
                LInfoReturn.StrProductName = LRegistryKeyLocalMachine.GetValue("ProductName").ToString();
                LInfoReturn.StrCSDVersion = LRegistryKeyLocalMachine.GetValue("Service Pack 1").ToString();
                LInfoReturn.StrInstallationType = LRegistryKeyLocalMachine.GetValue("Client").ToString();
                LRegistryKeyLocalMachine.Close();
                LRegistryKeyLocalMachine.Dispose();
            }
            catch (Exception ex)
            {
                LInfoReturn.StrErrorReturn = ex.ToString();
            }

            return LInfoReturn;
        }

        public static IISInformation GetIISInformatation()
        {
            IISInformation LInfoReturn = new IISInformation();

            try
            {
                LInfoReturn.StrErrorReturn = string.Empty;
                RegistryKey LRegistryKeyInetStp = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\InetStp");
                LInfoReturn.StrIISVersion = LRegistryKeyInetStp.GetValue("MajorVersion").ToString() + "." + LRegistryKeyInetStp.GetValue("MinorVersion").ToString();
                LRegistryKeyInetStp.Close();
                LRegistryKeyInetStp.Dispose();
            }
            catch (Exception ex)
            {
                LInfoReturn.StrErrorReturn = ex.ToString();
            }

            return LInfoReturn;
        }
    }

    public class ComputerInformation
    {
        /// <summary>
        /// 如果返回值不为空，说明有错误发生
        /// </summary>
        public string StrErrorReturn { get; set; }
        /// <summary>
        /// 操作系统版本号，如5.1/6.1/6.2
        /// </summary>
        public string StrVersion { get; set; }
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string StrProductName { get; set; }
        /// <summary>
        /// 补丁版本
        /// </summary>
        public string StrCSDVersion { get; set; }
        /// <summary>
        /// 安装类型，只针对版本号 > 6.0 有效。Server代表服务端、Client代表客户端
        /// </summary>
        public string StrInstallationType { get; set; }
    }

    public class IISInformation
    {
        /// <summary>
        /// 如果返回值不为空，说明有错误发生
        /// </summary>
        public string StrErrorReturn { get; set; }
        /// <summary>
        /// IIS 版本号
        /// </summary>
        public string StrIISVersion { get; set; }
    }
}
