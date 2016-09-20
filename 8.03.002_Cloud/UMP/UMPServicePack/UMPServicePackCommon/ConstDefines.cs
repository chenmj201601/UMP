using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePackCommon
{
    public class ConstDefines
    {
        #region Service Name
        public const string Service00Service = "UMP Service 00";
        public const string Service01Service = "UMP Service 01";
        public const string Service02Service = "UMP Service 02";
        public const string Service03Service = "UMP Service 03";
        public const string Service04Service = "UMP Service 04";
        public const string Service05Service = "UMP Service 05";
        public const string Service06Service = "UMP Service 06";
        public const string Service07Service = "UMP Service 07";
        public const string Service08Service = "UMP Service 08";
        public const string Service09Service = "UMP Service 09";

        public const string UMPCRYPTIONService = "UMPCRYPTION";
        public const string UMPDBbridgeServerService = "UMPDBbridgeServer";
        public const string UMPDECService = "UMPDEC";
        public const string UMPFSService = "UMPFS";
        public const string UMPKEYGENService = "UMPKEYGEN";
        public const string UMPSFTPService = "UMPSFTP";
        public const string UMPTellServerService = "UMPTellServer";
        public const string UMPVoiceServerService = "UMPVoiceServer";
        public const string UMPScreenService = "UMPScreen";
        public const string UMPSpeechAnalysisService = "UMPSpeechAnalysis";
        public const string UMPAlarmMonitorService = "UMPAlarmMonitor";
        public const string UMPAlarmServerServiceService = "UMPAlarmServer";
        public const string UMPCTIDBBService = "UMPCTIDBB";
        public const string UMPCTIHUBService = "UMPCTIHUB";
        public const string UMPCMServerService = "UMPCMServer";
        public const string UMPLicenseServerService = "UMPLicenseServer";
        public const string UMPSIPServerService = "UMPSIPServer";
        public const string UMPScreenCenterService = "UMPScreenCenter";
        public const string UMPParamServerService = "UMPParamServer";
        public const string UMPSadrsServerService = "UMPSadrsServer";
        public const string UMPUploadRecordService = "UMPUploadRecord";
        #endregion

        #region App Names
        public const string UMPAlarmServer = "UMPAlarmServer";
        public const string UMPAlarmClient = "UMPAlarmClient";
        public const string UMPLicenseManager = "UMPLicenseManager";
        public const string UMPAgentClient = "UMPAgentClient";
        public const string UMPCMServer = "UMPCMServer";
        public const string UMPCQC = "UMPCQC";
        public const string UMPCTIHub = "UMPCTIHub";
        public const string UMPDEC = "UMPDEC";
        public const string UMPLicenseServer = "UMPLicenseServer";
        public const string UMPScreenServer = "UMPScreenServer";
        public const string UMPSoftRecord = "UMPSoftRecord";
        public const string UMPSpeechAnalysis = "UMPSpeechAnalysis";
        public const string IxPatch = "IxPatch";
        public const string VCLogWebSDK = "VCLogWebSDK";
        public const string UMPVoice = "UMPVoice";
        public const string ASM = "ASM";
        public const string UMP = "UMP";
        public const string UMPSFTP = "UMPSFTP";
        #endregion

        #region App GUIDs
        public const string UMPAlarmServerGUID = "{87DC3662-2D5E-43BF-85CD-B94D2796BB55}";
        public const string UMPAlarmClientGUID = "{C8555711-03D2-47A2-BF51-069693BA8D17}";
        public const string UMPLicenseManagerGUID = "{3BD7D6CC-216F-4179-9DF5-A6E71677CD59}";
        public const string UMPAgentClientGUID = "{ED832382-6E5E-41EF-A490-57567060BCDD}";
        public const string UMPCMServerGUID = "{CD53D7CC-F2B7-4BBC-9E76-34C4AC81DA3C}";
        public const string UMPCQCGUID = "{DDD3C417-0570-45E6-8034-1A27140019CD}";
        public const string UMPCTIHubGUID = "{5BA9788D-5D61-4844-AB4B-F5097747517D}";
        public const string UMPDECGUID = "{C2776AB1-A485-4E2B-B9E4-341574DF4ED5}";
        public const string UMPLicenseServerGUID = "{820A82DE-3697-4CFC-968C-662F549C9ADE}";
        public const string UMPScreenServerGUID = "{C05BA5D3-6D0A-4C9F-AD87-FA7E274CED9A}";
        public const string UMPSoftRecordGUID = "{4D466165-3B16-4351-8169-A341E171C1BE}";
        public const string UMPSpeechAnalysisGUID = "{D24DB725-121B-4206-8744-F769767C4EF0}";
        public const string IxPatchGUID = "{E5299795-AE1B-400C-96AB-5FAC1761F544}";
        public const string VCLogWebSDKGUID = "{6C5007CD-336A-41FE-97A4-47413C17ADFC}";
        public const string UMPVoiceGUID = "{76FCCB1C-CECA-454D-A739-E2C04B40A358}";
        public const string ASMGUID = "{6664EAEB-7488-43FC-A4A5-DDBE1F00E755}";
        public const string UMPGUID = "{B983BB4A-39D8-4878-8E62-B36805CA8E30}";
        public const string UMPSFTPGUID = "{FA200F45-9B3B-4C92-9928-7DFC897290F7}";
        #endregion

        #region 错误码
        //数据库配置为空
        public const int RET_Database_Null = 405;
        //获得数据库配置信息异常
        public const int Get_Database_Info_Exception = 406;
        //获得用户名密码异常
        public const int Get_UserName_Pwd_Exception = 1001;
        //用户不存在或密码错误
        public const int UserName_Or_Pwd_Not_Exists = 1002;
        //检查用户状态时发生异常
        public const int Check_User_Exception = 1003;
        //用户已过期
        public const int User_Overdue = 1004;
        //获得用户角色失败
        public const int Get_User_Role_Failed = 1005;
        //用户不是管理员角色
        public const int User_Not_Admin = 1006;
        //检查已安装的版本出现异常
        public const int Get_Version_Exception = 1007;
        //查询T_00_000表失败
        public const int Get_T000_Failed = 1008;
        //T_00_000是空的
        public const int T000_Is_Null = 1009;
        //已经安装了比补丁包更新的版本
        public const int Installed_Newer = 1010;
        //更新文件丢失
        public const int Upgrade_File_Not_Found = 1011;
        //要压缩的文件不存在
        public const int Compress_Src_Not_Found = 1012;
        //压缩过程中出现异常
        public const int Compress_Exception = 1013;
        //拷贝文件时的源文件不存在
        public const int Copy_Res_Not_Found = 1014;
        //拷贝文件出现异常
        public const int Copy_File_Exception = 1015;
        //服务不存在（获取状态失败）
        public const int Service_Not_Exists = 1016;
        //停止服务出现异常
        public const int Stop_Service_Exception = 1017;
        //杀进程出现异常
        public const int Kill_Process_Exception = 1018;
        //更新文件异常
        public const int Update_Files_Exception = 1019;
        //检查依赖文件出现异常
        public const int Check_Depend_File_Exception = 1020;
        //获得文件路径出现异常
        public const int Get_File_Path_Exception = 1021;
        //删除文件夹出现异常
        public const int Delete_Directory_Exception = 1022;
        //删除文件出现异常
        public const int Delete_File_Exception = 1023;
        //要读取的脚本文件不存在
        public const int Script_File_Not_Exists = 1024;
        //执行sql语句出现异常
        public const int ExcuteSql_Exception = 1025;
        public const int Write_Version_To_DB_Exception = 1026;
        //启动服务出现异常
        public const int Start_Service_Exception = 1027;
        //更新服务出现异常
        public const int Update_Service_Exception = 1028;
        //从注册表读取Installutil路径出错
        public const int Get_Installutil_Path_Exception = 1029;
        //ServiceOperator出现异常 
        public const int Service_Operator_Exception = 1030;
        //重命名ntidrv文件失败
        public const int Rename_File_Exception = 1031;
        #endregion

        #region 加密密钥
        public static string VCT_KEY128_LOW1 = "UvjCat20^q23df7X";					    // 低保密等级加密密钥1，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_LOW2 = "&3jFs^%340KfncS_";					    // 低保密等级加密密钥2，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_LOW3 = "!2&fnj*03JvcnRa$";					    // 低保密等级加密密钥3，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_NORMAL1 = "^kgCtk%1K)ej4Afa";					// 一般保密等级加密密钥1，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_NORMAL2 = "~cYs&50LKgm)2@Sh";					// 一般保密等级加密密钥2，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_NORMAL3 = "%isA67VGf@c9>,+z";					// 一般保密等级加密密钥3，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_HIGH1 = "4sYda3(24safafQ%";					    // 高保密等级加密密钥1，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY128_HIGH2 = "Fh*3,>)r7X_ag!39";					    // 高保密等级加密密钥2，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY128_HIGH3 = "kG56*20+&fc[3Jas";					    // 高保密等级加密密钥3，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_LOW1 = "74kjs&33$2240JsfkUgalaujYRDCasa#";	    // 低保密等级加密密钥1，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_LOW2 = "4sfc[3J24safafQ%kG56*20+xfc[3Jas";	    // 低保密等级加密密钥2，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_LOW3 = "724safafQ%kG56*2033YRDCr7X_(asa#";	    // 低保密等级加密密钥3，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_NORMAL1 = "%^2kfgXCgtHk2%719Kf)1edj^4jAdfRa";	// 一般保密等级加密密钥1，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_NORMAL2 = "$k2%fgcnRa$k2%4jA1edj^4at2ad#8d.";	// 一般保密等级加密密钥2，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_NORMAL3 = "*tk%k2d*3cC1edBej4d%3#-x3()feL4a";	// 一般保密等级加密密钥3，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_HIGH1 = "&4jskYBdfa%33()234'sjasfFabfTQA%";	    // 高保密等级加密密钥1，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_HIGH2 = "&$ofm*fM<dgta%z0%>*?hFa!24A)j*Sl";	    // 高保密等级加密密钥2，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_HIGH3 = "#$szL3sl(>!e3uAczX?!8)cc36+,a7h@";	    // 高保密等级加密密钥3，对VCT以外保密，用于客户数据、加密系统密钥的加密
        #endregion

        #region 加密向量
        public static string VCT_ENCRYPTION_AES_IVEC_OLD = "0000000000000000";	        // cbc模式加密向量(兼容以前老的程序)
        public static string VCT_ENCRYPTION_AES_IVEC_LOW = "4I!9V6X8Af98^5bC";	        // cbc模式加密向量(低保密等级,仅可开放给VCC)
        public static string VCT_ENCRYPTION_AES_IVEC_NORMAL = ")26V!7Cwh6i5Y1Qd";	    // cbc模式加密向量(一般保密等级,对VCT以外保密)
        public static string VCT_ENCRYPTION_AES_IVEC_HIGH = "ev04Z0VDLn!58}]o";         // cbc模式加密向量(高保密等级，对VCT以外保密)
        #endregion

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

        public const char SPLITER_CHAR = '';
        public const string strUNLIMITED = "UNLIMITED";
    }
}
