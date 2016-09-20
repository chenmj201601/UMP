//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ab8c46a8-3081-4bd6-9bb3-20589ec4fbcb
//        CLR Version:              4.0.30319.42000
//        Name:                     S1200Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                S1200Codes
//
//        created by Charley at 2016/1/22 14:13:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 请求码
    /// </summary>
    public enum S1200Codes
    {
        /// <summary>
        /// 获取指定App的文件列表
        /// </summary>
        GetAppFileList = 1,
        /// <summary>
        /// 获取应用配置信息（UMP.Server.09.xml）
        /// </summary>
        GetAppConfigList = 2,
        /// <summary>
        /// 获取应用模块基本信息（T_11_003)
        /// </summary>
        GetAppBasicInfoList = 11,
        /// <summary>
        /// 获取操作日志信息（T_11_901)
        /// </summary>
        GetOptLogInfoList = 12,
        /// <summary>
        /// 获取操作日志内容的显示语言
        /// </summary>
        GetOptContentLangList = 13,
        /// <summary>
        /// 获取模块使用历史记录信息
        /// </summary>
        GetAppUsageInfoList = 14,
        /// <summary>
        /// 获取支持的语言类型
        /// </summary>
        GetSupportLangTypeList = 15,
        /// <summary>
        /// 从xml文件加载语言信息
        /// </summary>
        GetLanguageInfoListXml = 16,
        /// <summary>
        /// 获取用户所属角色信息
        /// </summary>
        GetUserRoleList = 17,
        /// <summary>
        /// 获取用户部件列表
        /// </summary>
        GetUserWidgetList = 18,
        /// <summary>
        /// 获取第三方应用配置信息
        /// </summary>
        GetThirdPartyAppList = 19,
        /// <summary>
        /// 获取基础信息列表
        /// </summary>
        GetBasicInfoList = 20,
        /// <summary>
        /// 获取部件属性信息列表
        /// </summary>
        GetWidgetPropertyInfoList = 21,
        /// <summary>
        /// 获取用户部件参数值列表
        /// </summary>
        GetUserWidgetPropertyValueList = 22,
        /// <summary>
        /// 获取所有可用的小部件列表
        /// </summary>
        GetAllWidgetList = 23,
        /// <summary>
        /// 获取全局参数列表
        /// </summary>
        GetLogicalPartTableList = 24,
        /// <summary>
        /// 获取域信息列表
        /// </summary>
        GetDomainInfoList = 25,
        /// <summary>
        /// 获取LDAP账户的登录密码
        /// </summary>
        GetLDAPUserList = 26,

        /// <summary>
        /// 记录模块使用历史记录
        /// </summary>
        SetAppUsageInfo = 101,
        /// <summary>
        /// 保存部件参数信息
        /// </summary>
        SaveUserWidgetPropertyValues = 102,
        /// <summary>
        /// 保存用户自定义的部件配置信息
        /// </summary>
        SaveUserWidgetInfos = 103,
    }
}
