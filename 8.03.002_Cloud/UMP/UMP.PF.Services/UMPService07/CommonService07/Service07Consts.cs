//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eff93346-e5d3-4bd1-8a46-05238ec0206d
//        CLR Version:              4.0.30319.18063
//        Name:                     Service07Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService07
//        File Name:                Service07Consts
//
//        created by Charley at 2015/11/24 13:40:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService07
{
    public class Service07Consts
    {
        //特定资源编码
        public const long OBJID_ORG_ROOT = 1010000000000000001;     //跟机构

        //特定模块编号
        public const int MODULE_BASEMODULE = 11;        //基础模块

        //资源码
        public const int RESOURCE_EXTENSION = 104;
        public const int RESOURCE_REALEXT = 105;
    
        public const int RESOURCE_VOICESERVER = 221;
        public const int RESOURCE_SCREENSERVER = 231;

        public const int RESOURCE_VOICECHANNEL = 225;
        public const int RESOURCE_SCREENCHANNEL = 232;

        public const int RESOURCE_PBXDEVICE = 220;


        //全局参数编号
        public const int GP_GROUP_PASSWORD = 110104;

        public const int GP_DEFULT_PASSWORD = 11010501;


        //Monitor 指令
        public const int MONITOR_COMMAND_GETRESOURCEOBJECT = 101;

        //配置选项
        /// <summary>
        /// 同步分机数据库时间间隔，单位：秒
        /// 范围 10 ~ 60 * 60，即10秒到1小时，默认30秒
        /// </summary>
        public const string GS_KEY_INTERVAL_SYNCEXTENSION = "SyncExtensionInterval";
        /// <summary>
        /// 同步真实分机数据库时间间隔，单位：秒
        /// 范围 10 ~ 60 * 60，即10秒到1小时，默认30秒
        /// </summary>
        public const string GS_KEY_INTERVAL_SYNCREALEXT = "SyncRealExtInterval";
        /// <summary>
        /// 回删21_998已加密数据，单位：分钟
        /// 范围 1 ~ 60 * 24 ，即1分钟到1天，默认30分钟
        /// </summary>
        public const string GS_KEY_INTERVAL_DELETEENCRYPTION = "DeleteEncryptionInterval";

    }
}
