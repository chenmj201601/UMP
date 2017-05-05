//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    540205ed-9c8c-421a-a683-89ce5e0e0397
//        CLR Version:              4.0.30319.18063
//        Name:                     OperationLogInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                OperationLogInfo
//
//        created by Charley at 2015/10/21 12:09:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 操作日志信息
    /// </summary>
    [DataContract]
    public class OperationLogInfo
    {
        /// <summary>
        /// 操作日志流水号（901，902）
        /// </summary>
        [DataMember]
        public long ID { get; set; }
        /// <summary>
        /// 登录流水号（903，904）
        /// </summary>
        [DataMember]
        public long LoginID { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        [DataMember]
        public int ModuleID { get; set; }
        /// <summary>
        /// 操作编号
        /// </summary>
        [DataMember]
        public long OptID { get; set; }
        /// <summary>
        /// 操作人编号
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        /// <summary>
        /// 操作人的角色编号
        /// </summary>
        [DataMember]
        public long RoleID { get; set; }
        /// <summary>
        /// 租户标识（00000）
        /// </summary>
        [DataMember]
        public string RentToken { get; set; }
        /// <summary>
        /// 操作发生的机器的机器名
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }
        /// <summary>
        /// 操作发生的机器的IP地址
        /// </summary>
        [DataMember]
        public string MachineIP { get; set; }
        /// <summary>
        /// 操作发生的时间（UTC）
        /// </summary>
        [DataMember]
        public DateTime LogTime { get; set; }
        /// <summary>
        /// 操作结果
        /// </summary>
        [DataMember]
        public string LogResult { get; set; }
        /// <summary>
        /// 操作日志语言包的编码
        /// </summary>
        [DataMember]
        public string LangID { get; set; }
        /// <summary>
        /// 操作日志参数
        /// </summary>
        [DataMember]
        public string LogArgs { get; set; }
        /// <summary>
        /// 以字符串形式表示对象信息
        /// 可以是Json串或Xml信息
        /// </summary>
        [DataMember]
        public string StringInfo { get; set; }


        #region 字段名称定义

        public const string PRO_ID = "ID";
        public const string PRO_LOGINID = "LoginID";
        public const string PRO_MODULEID = "ModuleID";
        public const string PRO_OPTID = "OptID";
        public const string PRO_USERID = "UserID";
        public const string PRO_ROLEID = "RoleID";
        public const string PRO_RENTTOKEN = "RentToken";
        public const string PRO_MACHINENAME = "MachineName";
        public const string PRO_MACHINEIP = "MachineIP";
        public const string PRO_LOGTIME = "LogTime";
        public const string PRO_LOGRESULT = "LogResult";
        public const string PRO_LANGID = "LangID";
        public const string PRO_LOGARGS = "LogArgs";

        #endregion

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ID, OptID, UserID);
        }
    }
}
