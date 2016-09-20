//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3be0406f-b6a9-4a97-a85c-cb5a6b6aed0b
//        CLR Version:              4.0.30319.18063
//        Name:                     DatabaseParam
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                DatabaseParam
//
//        created by Charley at 2014/3/23 9:40:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.Common
{
    /// <summary>
    /// 数据库连接参数
    /// </summary>
    public class DatabaseParam
    {
        /// <summary>
        /// 数据库类型，参考Defines中的定义
        /// </summary>
        public int DBType { get; set; }
        /// <summary>
        /// 数据库类型的名称
        /// </summary>
        public string DBTypeName { get; set; }
        /// <summary>
        /// 数据库服务器地址
        /// </summary>
        public string DBServer { get; set; }
        /// <summary>
        /// 数据库服务器端口
        /// </summary>
        public string DBPort { get; set; }
        /// <summary>
        /// 数据库名称或服务名（Oracle）
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUser { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// 密码加密方式
        /// </summary>
        public string PwdEncryptType { get; set; }
    }
}
