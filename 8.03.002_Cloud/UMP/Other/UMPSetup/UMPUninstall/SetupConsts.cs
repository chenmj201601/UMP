namespace UMPUninstall
{
    public class SetupConsts
    {
        public const string APP_NAME = "UMPUninstall";

        public const string DEFAULT_VERSION = "8.03.001";


        public const int STA_STOPSERVICE = 11;
        public const int STA_REMOVESERVICE = 12;
        public const int STA_REMOVESHOTCUT = 13;
        public const int STA_REMOVEREGISTRY = 14;
        public const int STA_REMOVEFILES = 15;
        public const int STA_REMOVEWEBSITE = 17;


        public const string STA_NAME_STOPSERVICE = "StopService...";
        public const string STA_NAME_REMOVESERVICE = "RemoveService...";
        public const string STA_NAME_REMOVESHOTCUT = "RemoveShotcut...";
        public const string STA_NAME_REMOVEREGISTRY = "RemoveRegistry...";
        public const string STA_NAME_REMOVEFILES = "RemoveFiles...";
        public const string STA_NAME_REMOVEWEBSITE = "RemoveWebSite...";


        public const int PERCENTAGE_BASE_STOPSERVICE = 0;
        public const int PERCENTAGE_BASE_REMOVESERVICE = 20;
        public const int PERCENTAGE_BASE_REMOVESHOTCUT = 40;
        public const int PERCENTAGE_BASE_REMOVEREGISTRY = 45;
        public const int PERCENTAGE_BASE_REMOVEWEBSITE = 50;
        public const int PERCENTAGE_BASE_REMOVEFILES = 60;
        public const int PERCENTAGE_STOPSERVICE = 20;
        public const int PERCENTAGE_REMOVESERVICE = 20;
        public const int PERCENTAGE_REMOVESHOTCUT = 5;
        public const int PERCENTAGE_REMOVEREGISTRY = 5;
        public const int PERCENTAGE_REMOVEWEBSITE = 10;
        public const int PERCENTAGE_REMOVEFILES = 40;

        public const string IIS_APPPOOL_NAME_UMP = "UMPAppPool";
        public const string IIS_SITE_NAME_UMP = "UMP.PF";
    }
}
