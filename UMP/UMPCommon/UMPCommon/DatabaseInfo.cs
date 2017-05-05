//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6ad1ee1c-a6d3-46ce-95f6-87eb784e1f4b
//        CLR Version:              4.0.30319.18444
//        Name:                     DatabaseInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                DatabaseInfo
//
//        created by Charley at 2014/8/29 15:49:39
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 数据库信息
    /// </summary>
    [DataContract]
    public class DatabaseInfo
    {
        /// <summary>
        /// 数据库类型
        /// 0   Unkown
        /// 1   MySQL
        /// 2   Microsoft SQL Server
        /// 3   Oracle
        /// ......
        /// </summary>
        [DataMember]
        public int TypeID { get; set; }
        /// <summary>
        /// 数据库类型缩写
        /// MSSQL
        /// ORCL
        /// MYSQL
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }
        /// <summary>
        /// 数据库服务器主机地址
        /// </summary>
        [DataMember]
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [DataMember]
        public int Port { get; set; }
        /// <summary>
        /// 数据库名，服务名或SID（Oracle）
        /// </summary>
        [DataMember]
        public string DBName { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        [DataMember]
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密码，通常是已经加密的密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 实际密码，这个密码不能序列化传输，是未加密的原始密码
        /// </summary>
        [XmlIgnore]
        public string RealPassword { get; set; }

        /// <summary>
        /// 根据数据库参数获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            string strReturn = string.Empty;
            switch (TypeID)
            {
                case 1:
                    strReturn = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", Host, Port, DBName,
                        LoginName, RealPassword);
                    break;
                case 2:
                    strReturn = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", Host,
                        Port, DBName, LoginName, RealPassword);
                    break;
                case 3:
                    strReturn =
                        string.Format(
                            "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}",
                            Host, Port, DBName, LoginName, RealPassword);
                    break;
            }
            return strReturn;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}:{2}-{3}-{4}", TypeID, Host, Port, DBName, LoginName);
        }
    }
}
