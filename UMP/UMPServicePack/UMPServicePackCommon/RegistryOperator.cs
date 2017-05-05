using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using VoiceCyber.Common;

namespace UMPServicePackCommon
{
    /// <summary>
    /// 操作注册表的类
    /// </summary>
    public class RegistryOperator
    {
        /// <summary>
        /// 根据GUID判断安装包安装的位置
        /// </summary>
        /// <param name="strGUID"></param>
        /// <returns></returns>
        public static OperationReturn GetAppInfoByGUID(string strGUID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = Defines.RET_SUCCESS;
            optReturn.Result = true;
            try
            {
                RegistryKey key = Registry.LocalMachine;
                string strSubKey = string.Empty;
                //如果是64位系统
                if (System.Environment.Is64BitOperatingSystem)
                {
                    strSubKey = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strGUID;
                }
                else
                {
                    strSubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + strGUID;
                }
                RegistryKey keyAppInfo = key.OpenSubKey(strSubKey);
                UMPAppInfo appInfo = new UMPAppInfo();
                appInfo.AppName = keyAppInfo.GetValue("DisplayName").ToString();
                appInfo.AppInstallPath = keyAppInfo.GetValue("InstallLocation").ToString();
                optReturn.Data = appInfo;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        public static OperationReturn GetInstallUtilInstallPath()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                RegistryKey key = Registry.LocalMachine;
                string strSubKey = string.Empty;
                strSubKey = "SOFTWARE\\Microsoft\\.NETFramework";
                RegistryKey keyNetInfo = key.OpenSubKey(strSubKey);
                string strInstallPath = keyNetInfo.GetValue("InstallRoot").ToString();
                strInstallPath += "v4.0.30319\\InstallUtil.exe";
                optReturn.Data = strInstallPath;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Get_Installutil_Path_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }
    }
}
