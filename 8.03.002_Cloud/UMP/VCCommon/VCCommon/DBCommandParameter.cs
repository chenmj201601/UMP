//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffe68d28-4e31-4ba9-a04d-e7bc284eadad
//        CLR Version:              4.0.30319.18063
//        Name:                     DBCommandParameter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                DBCommandParameter
//
//        created by Charley at 2014/3/24 11:58:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.Data;

namespace VoiceCyber.Common
{
    /// <summary>
    /// Sql命令参数
    /// </summary>
    public class DBCommandParameter
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public int DBType { get; set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public DBCommandParamType ParamType { get; set; }
        /// <summary>
        /// 参数数据长度
        /// </summary>
        public int ParamLength { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object ParamValue { get; set; }
        /// <summary>
        /// 参数的输入输出方向
        /// </summary>
        public ParameterDirection Direction { get; set; }
    }
    /// <summary>
    /// Sql命令参数类型
    /// </summary>
    public enum DBCommandParamType
    {
        /// <summary>
        /// 整形
        /// </summary>
        Int,
        /// <summary>
        /// 数值型
        /// </summary>
        Number,
        /// <summary>
        /// 字符型
        /// </summary>
        VarChar,
        /// <summary>
        /// 字符型
        /// </summary>
        NVarChar,
        /// <summary>
        /// 日期时间型
        /// </summary>
        DateTime
    }
}
