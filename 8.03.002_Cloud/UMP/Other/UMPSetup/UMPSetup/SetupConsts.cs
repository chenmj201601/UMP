//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7539ff4c-8ae5-4b3b-819f-1090b25aab91
//        CLR Version:              4.0.30319.18063
//        Name:                     SetupConsts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup
//        File Name:                SetupConsts
//
//        created by Charley at 2015/12/29 10:09:57
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPSetup
{
    public class SetupConsts
    {
        public const string APP_NAME = "UMPSetup";

        public const string DEFAULT_VERSION = "8.03.001";

        public const int STA_EXTRACT = 1;
        public const int STA_WRITEREG = 2;
        public const int STA_INSTALLSERVICE = 3;
        public const int STA_STARTSERVICE = 4;
        public const int STA_INSTALLSHOTCUT = 5;
        public const int STA_INSTALLDRIVER = 6;
        public const int STA_INSTALLWEBSITE = 7;

        public const string STA_NAME_EXTRACT = "Extracting...";
        public const string STA_NAME_WRITEREG = "WriteReg...";
        public const string STA_NAME_INSTALLSERVICE = "InstallService...";
        public const string STA_NAME_STARTSERVICE = "StartService...";
        public const string STA_NAME_INSTALLSHOTCUT = "InstallShotcut...";
        public const string STA_NAME_INSTALLDRIVER = "InstallDriver...";
        public const string STA_NAME_INSTALLWEBSITE = "InstallWebSite...";


        public const int PERCENTAGE_BASE_EXTRACT = 0;
        public const int PERCENTAGE_BASE_WEBSITE = 70;
        public const int PERCENTAGE_BASE_WRITEREG = 80;
        public const int PERCENTAGE_BASE_INSTALLSERVICE = 86;
        public const int PERCENTAGE_BASE_STARTSERVICE = 92;
        public const int PERCENTAGE_BASE_CREATESHOT = 96;
        public const int PERCENTAGE_EXTRACT = 70;
        public const int PERCENTAGE_WEBSITE = 10;
        public const int PERCENTAGE_WRITEREG = 6;
        public const int PERCENTAGE_INSTALLSERVICE = 6;
        public const int PERCENTAGE_STARTSERVICE = 4;
        public const int PERCENTAGE_CREATESHOT = 4;

        public const string DRIVER_FILE_SMO = "Microsoft SQL Server Management Ojbects";
        public const string DRIVER_FILE_SCT = "Microsoft SQL Server System CLR Types";
        public const string DRIVER_FILE_SNC = "SNC SQL 2008 Native Client";
        public const string DRIVER_FILE_ODAC = "ODAC";

        public const string IIS_APPPOOL_NAME_UMP = "UMPAppPool";
        public const string IIS_SITE_NAME_UMP = "UMP.PF";
        public const string IIS_APP_PATH_WCFSERVICE = "WcfServices";
        public const string IIS_APP_PATH_WCF2CLIENT = "Wcf2Client";
        public const string IIS_APP_PATH_WCF1600 = "WCF1600";

    }
}
