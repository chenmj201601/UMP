//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5f8f60cb-472e-419f-876d-898a2ebfd612
//        CLR Version:              4.0.30319.18444
//        Name:                     S1110Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                S1110Codes
//
//        created by Charley at 2014/12/19 11:05:55
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    public enum S1110Codes
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 获取所有属性信息列表
        /// </summary>
        GetResourcePropertyInfoList = 1,
        /// <summary>
        /// 获取所有配置对象属性值
        /// </summary>
        GetResourcePropertyValueList = 2,
        /// <summary>
        /// 获取配置对象类型参数
        /// </summary>
        GetResourceTypeParamList = 3,
        /// <summary>
        /// 获取配置对象分组信息
        /// </summary>
        GetResourceGroupParamList = 4,
        /// <summary>
        /// 获取基础信息列表
        /// </summary>
        GetBasicInfoDataList = 5,
        /// <summary>
        /// 获取Sftp用户信息
        /// </summary>
        GetSftpUserList = 6,
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        GetServerInfo = 10,
        /// <summary>
        /// 获取资源对象的简略信息
        /// </summary>
        GetBasicResourceInfoList = 11,
        /// <summary>
        /// 获取通用资源信息
        /// </summary>
        GetResouceObjectList = 12,
        /// <summary>
        /// 获取指定Lisence信息
        /// </summary>
        GetSpecificLincense = 13,

        /// <summary>
        /// 保存资源属性信息
        /// </summary>
        SaveResourcePropertyData = 21,
        /// <summary>
        /// 保存资源子对象信息
        /// </summary>
        SaveResourceChildObjectData = 22,
        /// <summary>
        /// 从数据库中移除被删除的资源对象信息
        /// </summary>
        RemoveResourceObjectData = 23,
        /// <summary>
        /// 生成资源的xml文件
        /// </summary>
        GenerateResourceXml = 31
    }
}
