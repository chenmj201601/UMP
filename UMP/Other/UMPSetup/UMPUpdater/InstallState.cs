//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8fd31786-0f68-4154-900e-759e35750775
//        CLR Version:              4.0.30319.18408
//        Name:                     InstallState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                InstallState
//
//        created by Charley at 2016/5/18 17:27:02
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPUpdater
{
    public class InstallState
    {
        public bool IsUMPInstalled { get; set; }
        public bool IsDatabaseCreated { get; set; }
        public bool IsLogined { get; set; }
        public string LoginAccount { get; set; }

        public string CurrentVersion { get; set; }
        public string UpdateVersion { get; set; }
        public string UMPInstallPath { get; set; }
        public string TempPath { get; set; }

        public int DBType { get; set; }
        public string DBConnectionString { get; set; }

        public bool IsBackupUMP { get; set; }
        public string UMPBackupPath { get; set; }
        public bool IsCompressBackup { get; set; }
        public bool IsUpdateLang { get; set; }
        public int LangUpdateMode { get; set; }
        public bool IsSaveUpdateData { get; set; }

        public string IxPatchName { get; set; }
        public string IPPatchName { get; set; }

        public bool IsOptFail { get; set; }
        public string ErrorMessage { get; set; }

    }
}
