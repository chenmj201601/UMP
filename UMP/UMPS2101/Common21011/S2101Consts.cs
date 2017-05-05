namespace VoiceCyber.UMP.Common21011
{
    public static class S2101Consts
    {
        
        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;
        public const int OBJTYPE_ROLE = 103;
        public const int OBJTYPE_PERMISSIONS = 104;
        public const int OBJTYPE_FIRSTTASK = 308;
        public const int OBJTYPE_RECHECKTASK = 310;

        //A&R操作编码
        public const long OPT_Filter = 2101;
        /// <summary>
        /// 配置A&R的操作权限
        /// </summary>
        public const long OPT_FilterConfig = 2101001;
        /// <summary>
        /// 更新A&R的操作权限
        /// </summary>
        public const long OPT_FilterUpdate = 2101002;
        /// <summary>
        /// 删除A&R的操作权限
        /// </summary>
        public const long OPT_FilterDelete = 2101003;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const string USER_ADMIN = "102{0}00000000001";
        //特定角色编号
        public const string ROLE_SYSTEMADMIN = "106{0}00000000001";

        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        // 查询的表不存在
        public const string Err_TableNotExit = "ERR_TABLE_NOT_EXIT";
    }
}
