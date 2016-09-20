//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    99a67d35-cfc7-4dc7-87e5-bcbff04a7563
//        CLR Version:              4.0.30319.18063
//        Name:                     RegistryHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                RegistryHelper
//
//        created by Charley at 2014/3/22 18:21:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using Microsoft.Win32;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 操作注册表的帮助类
    /// 这是一个静态类型
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 获取指定注册表路径下指定项的值
        /// </summary>
        /// <param name="type">注册表类型</param>
        /// <param name="registryPath">注册表路径</param>
        /// <param name="itemName">项名称</param>
        /// <returns></returns>
        public static OperationReturn GetItemData(RegistryType type, string registryPath, string itemName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                RegistryKey rootKey;
                if (type == RegistryType.LocalMerchine)
                {
                    rootKey = Registry.LocalMachine;
                }
                else
                {
                    rootKey = Registry.CurrentUser;
                }
                var key = rootKey.OpenSubKey(registryPath);
                if (key == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Registry key not exsit.\t{0}", registryPath);
                    return optReturn;
                }
                var value = key.GetValue(itemName);
                if (value == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Registry item not exsit.\t{0}", itemName);
                    return optReturn;
                }
                optReturn.Message = value.ToString();
                optReturn.Data = value;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
        /// <summary>
        /// 写入注册表值
        /// </summary>
        /// <param name="type">注册表类型</param>
        /// <param name="registryPath">注册表路径</param>
        /// <param name="itemName">项名称</param>
        /// <param name="data">注册表值</param>
        /// <returns></returns>
        public static OperationReturn WriteItemData(RegistryType type, string registryPath, string itemName, object data)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                RegistryKey rootKey;
                if (type == RegistryType.LocalMerchine)
                {
                    rootKey = Registry.LocalMachine;
                }
                else
                {
                    rootKey = Registry.CurrentUser;
                }
                var key = rootKey.CreateSubKey(registryPath);
                if (key != null)
                {
                    key.SetValue(itemName, data);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
    }
}
