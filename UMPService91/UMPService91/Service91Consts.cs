//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    04d52536-6b5e-41f2-a872-e86e071bdfa3
//        CLR Version:              4.0.30319.42000
//        Name:                     Service91Consts
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                Service91Consts
//
//        Created by Charley at 2016/8/18 11:21:09
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
{
    public class Service91Consts
    {

        //Monitor 指令
        public const int MONITOR_COMMAND_GETRESOURCEOBJECT = 101;


        #region 错误码

        public const int DECRYPT_PASSWORD_ERROR = 1001;     //解密密码错误

        #endregion


        #region 配置选项

        public const string GS_KEY_S91_ISA_SCR_MODE = "IsaScrMode";

        #endregion

    }
}
