//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    68a9a041-b14a-40cd-8eef-620b83769d47
//        CLR Version:              4.0.30319.18063
//        Name:                     ConditionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                ConditionInfo
//
//        created by Charley at 2015/6/5 10:49:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPLanguageManager.Models
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    public class ConditionInfo
    {
        /// <summary>
        /// 语言ID，0表示所有类型语言
        /// </summary>
        public int LangID { get; set; }
        /// <summary>
        /// 大模块号，-1 表示不启用，0 表示基础模块
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 小模块号，-1 表示不启用，0 表示基础模块
        /// </summary>
        public int SubModuleID { get; set; }
        /// <summary>
        /// 前缀，空表示不启用
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 分类，空表示不启用
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public int DBType { get; set; }
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 查询语句
        /// </summary>
        public string QueryString { get; set; }

        public void Reset()
        {
            LangID = 0;
            ModuleID = -1;
            SubModuleID = -1;
            Prefix = string.Empty;
            Category = string.Empty;
            DBType = 0;
            ConnectionString = string.Empty;
            QueryString = string.Empty;
        }

        public void GetQueryString()
        {
            string strSql = string.Empty;
            string strCondition = string.Empty;
            switch (DBType)
            {
                case 2:
                    if (LangID > 0)
                    {
                        strCondition += string.Format(" AND C001 = {0}", LangID);
                    }
                    if (ModuleID >= 0)
                    {
                        strCondition += string.Format(" AND C009 = {0}", ModuleID);
                    }
                    if (SubModuleID >= 0)
                    {
                        strCondition += string.Format(" AND C010 = {0}", SubModuleID);
                    }
                    if (!string.IsNullOrEmpty(Prefix))
                    {
                        strCondition += string.Format(" AND C002 LIKE '{0}%'", Prefix);
                    }
                    if (!string.IsNullOrEmpty(Category))
                    {
                        if (ModuleID > 0)
                        {
                            if (SubModuleID > 0)
                            {
                                strCondition += string.Format(" AND C002 LIKE '{0}{1}%'", SubModuleID, Category);
                            }
                            else
                            {
                                strCondition += string.Format(" AND C002 LIKE '{0}__{1}%'", ModuleID, Category);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(strCondition))
                    {
                        strSql = string.Format("SELECT * FROM T_00_005 ORDER BY C001,C002");
                    }
                    else
                    {
                        strCondition = strCondition.Substring(4);
                        strSql = string.Format("SELECT * FROM T_00_005 WHERE {0} ORDER BY C001,C002", strCondition);
                    }
                    break;
                case 3:
                    if (LangID > 0)
                    {
                        strCondition += string.Format(" AND C001 = {0}", LangID);
                    }
                    if (ModuleID >= 0)
                    {
                        strCondition += string.Format(" AND C009 = {0}", ModuleID);
                    }
                    if (SubModuleID >= 0)
                    {
                        strCondition += string.Format(" AND C010 = {0}", SubModuleID);
                    }
                    if (!string.IsNullOrEmpty(Prefix))
                    {
                        strCondition += string.Format(" AND C002 LIKE '{0}%'", Prefix);
                    }
                    if (!string.IsNullOrEmpty(Category))
                    {
                        if (ModuleID > 0)
                        {
                            if (SubModuleID > 0)
                            {
                                strCondition += string.Format(" AND C002 LIKE '{0}{1}%'", SubModuleID, Category);
                            }
                            else
                            {
                                strCondition += string.Format(" AND C002 LIKE '{0}__{1}%'", ModuleID, Category);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(strCondition))
                    {
                        strSql = string.Format("SELECT * FROM T_00_005 ORDER BY C001,C002");
                    }
                    else
                    {
                        strCondition = strCondition.Substring(4);
                        strSql = string.Format("SELECT * FROM T_00_005 WHERE {0} ORDER BY C001,C002", strCondition);
                    }
                    break;
            }
            QueryString = strSql;
        }
    }
}
