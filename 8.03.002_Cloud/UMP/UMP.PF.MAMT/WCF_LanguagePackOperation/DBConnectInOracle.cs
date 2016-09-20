using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using System.IO;
using System.Data;

namespace WCF_LanguagePackOperation
{
    public class DBConnectInOracle
    {
        /// <summary>
        /// 创建Oracle数据库连接
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        private static OracleConnection CreateOracleConn(string strHost, string strPort, string strServiceName, string strUser, string strPwd)
        {
            string strConnString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + strHost + ")(PORT=" + strPort
                + ")))(CONNECT_DATA=(SERVICE_NAME=" + strServiceName + ")));User Id=" + strUser + ";Password=" + strPwd + ";";
            OracleConnection conn = new OracleConnection(strConnString);
            return conn;
        }

        /// <summary>
        /// 获得已经支持的UMP的语种列表
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static ReturnResult GetLanguageTypesSupported(string strHost, string strPort, string strServiceName, string strUser, string strPwd)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                OracleCommand com = conn.CreateCommand();
                com.CommandText = "SELECT T_00_005.C002,T_00_005.C005 FROM T_00_005 WHERE T_00_005.C002 IN (SELECT TO_CHAR(T_00_004.c001) AS C001 FROM T_00_004 WHERE C006=1)";
                OracleDataAdapter adapter = new OracleDataAdapter(com);
                conn.Open();
                adapter.Fill(result.DataSetReturn);
                result.BoolReturn = true;
                result.StringReturn = string.Empty;
                conn.Close();
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 获得所有已经支持和未支持的UMP的语种列表
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static ReturnResult GetLanguageTypes(string strHost, string strPort, string strServiceName, string strUser, string strPwd)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                OracleCommand com = conn.CreateCommand();
                com.CommandText = "SELECT T_00_005.C002,T_00_005.C005 FROM T_00_005 WHERE T_00_005.C002 IN (SELECT TO_CHAR(T_00_004.C001) AS C001 FROM T_00_004)";
                OracleDataAdapter adapter = new OracleDataAdapter(com);
                conn.Open();
                adapter.Fill(result.DataSetReturn);
                result.BoolReturn = true;
                result.StringReturn = string.Empty;
                conn.Close();
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 获得所有的语言
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static ReturnResult GetAllLanguage(string strHost, string strPort, string strServiceName, string strUser, string strPwd)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                string strSql = "SELECT ROWNUM,C001,C002, C005||C006 AS Displaymessage,C007||C008 AS TipMessage,C009,C010,C011,C012 FROM T_00_005";
                OracleDataAdapter adapter = new OracleDataAdapter(strSql, conn);
                conn.Open();
                adapter.Fill(result.DataSetReturn);
                result.BoolReturn = true;
                result.StringReturn = string.Empty;
                conn.Close();
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 更新数据库 修改语言
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <param name="strLanCode"></param>
        /// <param name="strMessageID"></param>
        /// <param name="strDisPaly1"></param>
        /// <param name="strDisPlay2"></param>
        /// <param name="strTip1"></param>
        /// <param name="strTip2"></param>
        /// <param name="strModuleID"></param>
        /// <param name="strChileModuleID"></param>
        /// <param name="strInFrame"></param>
        /// <returns></returns>
        public static ReturnResult UpdateLanguageItem(string strHost, string strPort, string strServiceName, string strUser, string strPwd, string strLanCode
            , string strMessageID, string strDisPaly1, string strDisPlay2, string strTip1, string strTip2)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                OracleCommand com = conn.CreateCommand();
                com.CommandText = "UPDATE T_00_005 SET C005=:pDisPaly1,C006=:pDisPaly2,C007=:pTip1,C008=:pTip2 WHERE C001=TO_NUMBER(:pLanCode) AND C002=:pMessageID";
                com.Parameters.Add("pDisPaly1", OracleDbType.Varchar2).Value = strDisPaly1;
                com.Parameters.Add("pDisPaly2", OracleDbType.Varchar2).Value = strDisPlay2;
                com.Parameters.Add("pTip1", OracleDbType.Varchar2).Value = strTip1;
                com.Parameters.Add("pTip2", OracleDbType.Varchar2).Value = strTip2;
                int iLanCode = 1033;
                int.TryParse(strLanCode, out iLanCode);
                //result.ListStringReturn.Add(iLanCode.ToString());
                com.Parameters.Add("pLanCode", OracleDbType.Int16).Value = iLanCode;
                com.Parameters.Add("pMessageID", OracleDbType.Varchar2).Value = strMessageID;
                conn.Open();
                result.StringReturn = com.ExecuteNonQuery().ToString();
                conn.Close();
                result.BoolReturn = true;
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据语言ID删除语言
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <param name="strLanCode"></param>
        /// <returns></returns>
        public static ReturnResult RemoveLanguagesByLanguageCode(string strHost, string strPort, string strServiceName, string strUser, string strPwd, string strLanCode)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                OracleCommand com = conn.CreateCommand();
                com.CommandText = "DELETE FROM T_00_005 WHERE C001=:pLanguageCode";
                com.Parameters.Add("pLanguageCode", OracleDbType.Int16).Value = int.Parse(strLanCode);
                conn.Open();
                result.StringReturn = com.ExecuteNonQuery().ToString();
                conn.Close();
                result.BoolReturn = true;
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 添加语言包记录（用于导入）
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <param name="strLanCode"></param>
        /// <param name="strMessageID"></param>
        /// <param name="strDis1"></param>
        /// <param name="strDis2"></param>
        /// <param name="strTip1"></param>
        /// <param name="strTip2"></param>
        /// <param name="strModuleID"></param>
        /// <param name="strChildModuleID"></param>
        /// <param name="strFrame"></param>
        /// <returns></returns>
        public static ReturnResult Insert(string strHost, string strPort, string strServiceName, string strUser, string strPwd, string strLanCode, string strMessageID,
            string strDis1, string strDis2, string strTip1, string strTip2, string strModuleID, string strChildModuleID, string strFrame)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            OracleCommand com = conn.CreateCommand();
            try
            {
                string strSql = "INSERT INTO T_00_005(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011)"
                                    + "VALUES(:pLanCode,:pMsgID,0,0,:pDis1,:pDis2,:pTip1,:pTip2,:pModuleID,:pChildModuleID,:pFrame)";
                com.CommandText = strSql;
                com.Parameters.Add("pLanCode", OracleDbType.Int16).Value = int.Parse(strLanCode);
                com.Parameters.Add("pMsgID", OracleDbType.Varchar2).Value = strMessageID;
                com.Parameters.Add("pDis1", OracleDbType.Varchar2).Value = strDis1;
                com.Parameters.Add("pDis2", OracleDbType.Varchar2).Value = strDis2;
                com.Parameters.Add("pTip1", OracleDbType.Varchar2).Value = strTip1;
                com.Parameters.Add("pTip2", OracleDbType.Varchar2).Value = strTip2;
                int iModuleID = 0;
                int.TryParse(strModuleID, out iModuleID);
                com.Parameters.Add("pModuleID", OracleDbType.Int16).Value = iModuleID;
                int iChildmoduleID = 0;
                int.TryParse(strChildModuleID, out iChildmoduleID);
                com.Parameters.Add("pChildModuleID", OracleDbType.Int16).Value = iChildmoduleID;
                com.Parameters.Add("pFrame", OracleDbType.Varchar2).Value = strFrame;
                conn.Open();
                result.StringReturn = com.ExecuteNonQuery().ToString();
                conn.Close();
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 将languagecode设置为已经支持
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <param name="strLanCode"></param>
        /// <returns></returns>
        public static ReturnResult SupportLanguage(string strHost, string strPort, string strServiceName, string strUser, string strPwd, string strLanCode)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                OracleCommand com = conn.CreateCommand();
                com.CommandText = "UPDATE t_00_004 set C005=1,C006=1 where C001=:pLanCode";
                com.Parameters.Add("pLanCode", OracleDbType.Int16).Value = int.Parse(strLanCode);
                conn.Open();
                result.StringReturn = com.ExecuteNonQuery().ToString();
                conn.Close();
                result.BoolReturn = true;
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 尝试连接到数据库服务器
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static ReturnResult ConnectToServer(string strHost, string strPort, string strServiceName, string strUser, string strPwd)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = null;
            try
            {
                conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
                conn.Open();
                result.BoolReturn = true;
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获得所有数据库对象（表、函数、存储过程）
        /// </summary>
        /// <returns></returns>
        public static ReturnResult GetAllDBObjects()
        {
            ReturnResult result = new ReturnResult();
            try
            {
                string strPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"App_Data\DBScripts\Oracle";
                DirectoryInfo lstDBScriptsDir = new DirectoryInfo(strPath);
                if (lstDBScriptsDir == null)
                {
                    result.BoolReturn = false;
                    result.StringReturn = "Error017";
                }
                List<DirectoryInfo> lstChilds = lstDBScriptsDir.GetDirectories().ToList();
                DataTable dt = new DataTable();
                dt.TableName = "ScriptFilesInfo";
                dt.Columns.Add(new DataColumn("FileName"));
                dt.Columns.Add(new DataColumn("DataObjectype"));
                List<FileInfo> lstChildFiles = null;
                string strSub = string.Empty;
                for (int i = 0; i < lstChilds.Count; i++)
                {
                    lstChildFiles = lstChilds[i].GetFiles().ToList();
                    strSub = lstChilds[i].Name.Substring(0,lstChilds[i].Name.IndexOf('-'));
                    DataRow row = null;
                    switch (strSub)
                    {
                        case "1":
                            for (int j = 0; j < lstChildFiles.Count; j++)
                            {
                                row = dt.NewRow();
                                row[0] = lstChildFiles[j].Name;
                                row[1] = "Table";
                                dt.Rows.Add(row);
                            }
                            break;
                        case "2":
                            for (int j = 0; j < lstChildFiles.Count; j++)
                            {
                                row = dt.NewRow();
                                row[0] = lstChildFiles[j].Name;
                                row[1] = "Function";
                                dt.Rows.Add(row);
                            }
                            break;
                        case "3":
                            for (int j = 0; j < lstChildFiles.Count; j++)
                            {
                                row = dt.NewRow();
                                row[0] = lstChildFiles[j].Name;
                                row[1] = "3-Procedure";
                                dt.Rows.Add(row);
                            }
                            break;
                        default:
                            result.StringReturn = strSub;
                            break;
                    }
                }
                result.DataSetReturn.Tables.Add(dt);
                result.BoolReturn = true;
            }
            catch(Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="strType">数据库对象名</param>
        /// <param name="strName">数据库对象类型</param>
        /// <returns></returns>
        public static ReturnResult CreateDatabaseObject(string strType, string strName)
        {
            ReturnResult result = new ReturnResult();

            return result;
        }

        /// <summary>
        /// 获得某个语种的语言
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strServiceName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static ReturnResult GetSingleLanguage(string strHost, string strPort, string strServiceName, string strUser, string strPwd,string strLanCode)
        {
            ReturnResult result = new ReturnResult();
            OracleConnection conn = CreateOracleConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                string strSql = "SELECT ROWNUM,C001,C002, C005||C006 AS Displaymessage,C007||C008 AS TipMessage,C009,C010,C011,C012 FROM T_00_005 WHERE C001=:PLanCode";
                OracleCommand com = conn.CreateCommand();
                com.CommandText = strSql;
                com.Parameters.Add("PLanCode", OracleDbType.Int16).Value = int.Parse(strLanCode);
                OracleDataAdapter adapter = new OracleDataAdapter(com);
                conn.Open();
                adapter.Fill(result.DataSetReturn);
                result.BoolReturn = true;
                result.StringReturn = string.Empty;
                conn.Close();
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }
    }
}