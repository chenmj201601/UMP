using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCyber.UMP.Common11021
{
    public static class S1102Consts
    {
        //操作编码
        public const long OPT_ADDORG = 1101001;
        public const long OPT_DELETEORG = 1101002;
        public const long OPT_MODIFYORG = 1101003;
        public const long OPT_ADDUSER = 1101004;
        public const long OPT_DELETEUSER = 1101005;
        public const long OPT_MODIFYUSER = 1101006;


        //操作角色编码
        public const long OPT_AddRole = 1102001;
        public const long OPT_ModifyRole = 1102002;
        public const long OPT_DeleteRole = 1102003;
        public const long OPT_SubmitRolePermissions = 1102004;
        public const long OPT_SubmitRoleUsers = 1102005;



        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;
        public const int OBJTYPE_ROLE = 103;
        public const int OBJTYPE_PERMISSIONS = 104;

        //超级管理员
        public const string USER_ADMIN = "102{0}00000000001";

        //特定角色编号(座席和超级管理员)
        public const string ROLE_SYSTEMAGENT = "106{0}00000000004";
        public const string ROLE_SYSTEMADMIN = "106{0}00000000001";

        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        //默认使用结束期限
        public const string Default_StrStartTime = "2014/01/01 00:00:00";
        public const string Default_StrEndTime = "2199/12/31 23:59:59";

        //删除角色下限,小于这个值的角色不能删除
        public const long RoleID_Limit = 1061400000000000001;

    }
}
