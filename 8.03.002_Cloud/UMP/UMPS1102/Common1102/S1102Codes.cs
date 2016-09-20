using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCyber.UMP.Common11021
{
    public enum S1102Codes
    {
        Unkown = 0,
        GetOperationList = 1,
        GetRoleList=2,
        AddNewRole=3,
        ModifyRole=4,
        DeleteRole=5,
        SubmitRolePermission=6,
        SubmitRoleUser=7,
        GetRolePermission=8,
        GetOrganizationList=9,
        GetUserList = 10,
        GetRoleUsers=11,
        DeleteRolePermission=12,
        DeleteRoleUser=13,
        UpdateUserPerimission=14,
        GetCurrentOperationList=15,
        GetControlAgentInfoList=16
    }
}
