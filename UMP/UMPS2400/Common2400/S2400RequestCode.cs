using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common2400
{
    public enum S2400RequestCode
    {
        Unkown = 0,
        GetKeyGenServerList = 24011,
        GetAllMachines = 24012,
        TryConnToKeyGenServer = 24013,
        AddKeyGenServer = 24014,
        EnableDisableKeyGenServer = 24015,
        ModifyKeyGenServer=24016,
        DeleteKeyGenServer=24017,
        GetEncryptionDBCurrentTime=24018,
        AddEncryptionPolicy=24019,
        GetAllPolicies=24020,
        GetPolicyByID=24021,
        ModifyPolicy=24022,
        EnablePolicy=24023,
        DisablePolicy=24024,
        //获得当前启用的加密服务
        GetCurrentKeyGenServer=24025,
        GetPolicyBinding=24026,
        GetPolicyKey = 24027,
        GetResourceObjList = 240301,
        GetVoiceIP_Name201 = 240302,
        SetPolicyBindding = 240303,
        GetPoliciesByVoiceIPSource = 240304,
        SaveData224002 = 240305,
        DeleteBindedStragegy = 240306,
        ModyfyData224002 = 240307,
        SendMsgToService00 = 240308,
		GetEmailInfo = 24028,
        UpdateEmailInfo = 24029

    }

    public enum S2400WcfErrorCode
    {
        //密钥生成服务器已经存在
        KeyGenServerExists = 2401901,
        //密钥生成服务器连接失败
        KeyGenServerConnFailed = 2401902,
        //参数错误 [不加入语言包]
        ParamError = 2401903,
        //启用、禁用密钥服务器失败
        EnableDisableKeyGenServerFailed = 2401904
    }

    /// <summary>
    /// 属性加密模式
    /// </summary>
    public enum ObjectPropertyEncryptMode
    {
        /// <summary>
        /// 未知(不加密)
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 加密版本2，模式hex，AES加密
        /// </summary>
        E2Hex = 21,
        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256 = 31
    }

    public enum OperationType
    {
        Add = 0,
        Modify = 1,
        Delete = 2,
        Enable = 3,
        Disable = 4
    }
}
