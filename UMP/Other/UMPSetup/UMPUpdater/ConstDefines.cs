//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    30608f79-4179-461d-bca3-f70dce05c60b
//        CLR Version:              4.0.30319.42000
//        Name:                     ConstDefines
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                ConstDefines
//
//        Created by Charley at 2016/8/30 9:43:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPUpdater
{
    public class ConstDefines
    {
        #region STA

        public const int STA_BACKUPUMP = 1;         //备份UMP文件
        public const int STA_STOPSERVICE = 2;       //停止服务（本服务器）
        public const int STA_UPDATEFILE = 3;        //更新文件
        public const int STA_UPDATEDATABASE = 4;    //更新数据库
        public const int STA_UPDATELANG = 5;        //更新语言包
        public const int STA_UPDATESERVICE = 6;     //更新服务（安装，卸载等）
        public const int STA_STARTSERVICE = 7;      //启动服务（本服务器）
        public const int STA_UPDATESERVER = 8;      //更新其他Logging服务器

        public const string STA_NAME_BACKUPUMP = "BackupUMP";
        public const string STA_NAME_STOPSERVICE = "StopService";
        public const string STA_NAME_UPDATEFILE = "UpdateFile";
        public const string STA_NAME_UPDATEDATABASE = "UpdateDatabase";
        public const string STA_NAME_UPDATELANG = "UpdateLang";
        public const string STA_NAME_UPDATESERVICE = "UpdateService";
        public const string STA_NAME_STARTSERVICE = "StartService";
        public const string STA_NAME_UPDATESERVER = "UpdateServer";

        #endregion

        #region Progress

        public const int PRO_BACKUPUMP = 20;
        public const int PRO_STOPSERVICE = 10;
        public const int PRO_UPDATEFILE = 20;
        public const int PRO_UPDATEDATABASE = 10;
        public const int PRO_UPDATELANG = 10;
        public const int PRO_UPDATESERVICE = 10;
        public const int PRO_STARTSERVICE = 10;
        public const int PRO_UPDATESERVER = 10;

        public const int PRO_BASE_BACKUPUMP = 0;
        public const int PRO_BASE_STOPSERVICE = 20;
        public const int PRO_BASE_UPDATEFILE = 30;
        public const int PRO_BASE_UPDATEDATABASE = 50;
        public const int PRO_BASE_UPDATELANG = 60;
        public const int PRO_BASE_UPDATESERVICE = 70;
        public const int PRO_BASE_STARTSERVICE = 80;
        public const int PRO_BASE_UPDATESERVER = 90;

        #endregion

    }
}
