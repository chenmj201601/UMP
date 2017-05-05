using System;


namespace UMPService09.Utility
{
    public class StatisticsConstDefine
    {

        #region  //用于反射的统计类名

        //默认程序集名称
        public const string DefaultAssemblyName = "UMPService09";
        //acd数据的统计名
        public const string RecordStatisticsName = "RecordStatistics";
        //QM数据统计名
        public const string QMStatisticsName = "QMStatistics";
        #region //各CTI统计类名
        //AES数据统计名
        public const string AESStatisticsName="AESStatistics";
        public const string AVAYAStatisticsName = "AVAYAStatistics";
        public const string CMSERVERStatisticsName = "CMSERVERStatistics";
        public const string CSTAStatisticsName = "CSTAStaitstics";
        #endregion
        //KPIStatistics
        public const string KPIStatisticsName = "KPIStatistics";

        #endregion

        //PM服务读取xml的文件名
        public const string XmlFileName = @"PMStatistics.xml";

        //分表信息
        public const string Const_ColumnName_LPRecord = "LP_21_001.C002";

        //录音表表名
        public const string Const_TableName_Record = "T_21_001";

        //月的全局参数编号
        public const string Const_Week_Config = "12010101";

        //周的全局参数编号
        public const string Const_Month_Config = "12010102";

        //座席分机虚拟分机全局参数的编号
        public const string Const_Agent_Extension = "12010401";


        //资源码拼接后半部分
        public const string Const_Source_EndTrim = "0000000000000001";


        #region
        //机构资源码
        public const string Const_Source_Org_Begin = "101";
        //用户资源码
        public const string Const_Source_User_Begin = "102";
        //座席资源码
        public const string Const_Source_Agent_Begin = "103";        
        //分机资源码
        public const string Const_Source_Extension_Begin = "104";
        //真实分机资源码
        public const string Const_Source_TrueExtension_Begin = "105";
        //真实分机资源码
        public const string Const_Source_Role_Begin = "106";
        #endregion







    }
}
