//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    723b18e7-c2e8-4724-9dbe-5f735b9ea3be
//        CLR Version:              4.0.30319.18444
//        Name:                     WebCodes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                WebCodes
//
//        created by Charley at 2014/8/29 15:31:02
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11011
{
    public enum S1101Codes
    {
        Unkown = 0,
        GetOrganizationList = 2,
        AddNewOrgInfo = 3,
        DeleteOrgInfo = 4,
        ModifyOrgInfo = 5,
        GetUserList = 6,
        GetUserExtInfo = 7,
        AddNewUser = 8,
        ModifyUserInfo = 9,
        ModifyUserExtInfo = 10,
        DeleteUserInfo = 11,
        MoveUser = 12,
        GetUserRoleList = 13,
        SetUserRoles = 14,
        GetResourceProperty = 15,
        GetControlObjectList = 17,
        SetUserControlObject = 18,
        GetObjectBasicInfoList = 20,
        GetOrgTypeList = 21,

        GetParameter = 22,
        GetControlAgentInfoList = 23,
        GetControlExtensionInfoList = 24,
        GetControlRealExtensionInfoList=25,

        //自己模块的wcf方法编号
        GetResourceObjList = 1101001,
        GetAgentOrExt = 26,
        GetABCDList = 27,
        GetABCDItemList = 28,
        GetOrgItemRelation = 29,
        MoveAgent = 30,
        LoadNewUser = 31,
        MotifyAgentNameByAccount=32,
        ModifyUserPasswordM003=33,
        GetDomainInfo=34,
        
    }
}
