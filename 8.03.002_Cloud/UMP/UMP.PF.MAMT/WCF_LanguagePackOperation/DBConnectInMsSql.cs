using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WCF_LanguagePackOperation
{
    public class DBConnectInMsSql
    {
        /// <summary>
        /// 创建mssql数据库的连接
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strDBName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static SqlConnection CreateMSSQLConn(string strHost, string strPort, string strDBName, string strUser, string strPwd)
        {
            string strSql = "Data Source = {0},{1};Initial Catalog = {2};User Id = {3};Password = {4};";
            strSql = string.Format(strSql, strHost, strPort, strDBName, strUser, strPwd);
            SqlConnection sqlConn = new SqlConnection(strSql);
            return sqlConn;
        }

        /// <summary>
        /// 判断mssql是否可连接
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strDBName"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public bool ConnToMsSql(string strHost, string strPort, string strDBName, string strUser, string strPwd)
        {
            bool bIsConnected = false;
            SqlConnection sqlConn = CreateMSSQLConn(strHost, strPort, strDBName, strUser, strPwd);
            try
            {
                sqlConn.Open();
                bIsConnected = true;
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                bIsConnected = false;
            }
            finally
            {
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
            return bIsConnected;
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                SqlCommand com = conn.CreateCommand();
                com.CommandText = "SELECT T_00_005.C002,T_00_005.C005 FROM T_00_005 WHERE T_00_005.C002 IN (SELECT CONVERT(varchar(128), T_00_004.c001) FROM T_00_004 WHERE C006=1)";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                SqlCommand com = conn.CreateCommand();
                com.CommandText = " SELECT T_00_005.C002,T_00_005.C005 FROM T_00_005 WHERE T_00_005.C002 IN (SELECT CONVERT(varchar(128), T_00_004.c001) FROM T_00_004 )";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                string strSql = "SELECT C001,C002,ISNULL(C005,'')+ISNULL(C006,'') AS Displaymessage,ISNULL(C007,'')+ISNULL(C008,'') AS TipMessage,C009,C010,C011,C012 FROM T_00_005";
                SqlDataAdapter adapter = new SqlDataAdapter(strSql, conn);
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                SqlCommand com = conn.CreateCommand();
                com.CommandText = "UPDATE T_00_005 SET C005=@pDisPaly1,C006=@pDisPaly2,C007=@pTip1,C008=@pTip2 WHERE C001=@pLanCode AND C002=@pMessageID";
                com.Parameters.Add("@pDisPaly1", SqlDbType.VarChar).Value = strDisPaly1;
                com.Parameters.Add("@pDisPaly2", SqlDbType.VarChar).Value = strDisPlay2;
                com.Parameters.Add("@pTip1", SqlDbType.VarChar).Value = strTip1;
                com.Parameters.Add("@pTip2", SqlDbType.VarChar).Value = strTip2;
                int iLanCode = 1033;
                int.TryParse(strLanCode, out iLanCode);
                //result.ListStringReturn.Add(iLanCode.ToString());
                com.Parameters.Add("@pLanCode", SqlDbType.SmallInt).Value = iLanCode;
                com.Parameters.Add("@pMessageID", SqlDbType.VarChar).Value = strMessageID;
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                SqlCommand com = conn.CreateCommand();
                com.CommandText = "DELETE FROM T_00_005 WHERE C001=@pLanguageCode";
                com.Parameters.Add("@pLanguageCode", SqlDbType.SmallInt).Value = int.Parse(strLanCode);
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            SqlCommand com = conn.CreateCommand();
            try
            {
                string strSql = "INSERT INTO T_00_005(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011)"
                                    + "VALUES(@pLanCode,@pMsgID,0,0,@pDis1,@pDis2,@pTip1,@pTip2,@pModuleID,@pChildModuleID,@pFrame)";
                com.CommandText = strSql;
                com.Parameters.Add("@pLanCode", SqlDbType.SmallInt).Value = int.Parse(strLanCode);
                com.Parameters.Add("@pMsgID", SqlDbType.VarChar).Value = strMessageID;
                com.Parameters.Add("@pDis1", SqlDbType.VarChar).Value = strDis1;
                com.Parameters.Add("@pDis2", SqlDbType.VarChar).Value = strDis2;
                com.Parameters.Add("@pTip1", SqlDbType.VarChar).Value = strTip1;
                com.Parameters.Add("@pTip2", SqlDbType.VarChar).Value = strTip2;
                int iModuleID = 0;
                int.TryParse(strModuleID, out iModuleID);
                com.Parameters.Add("pModuleID", SqlDbType.SmallInt).Value = iModuleID;
                int iChildmoduleID = 0;
                int.TryParse(strChildModuleID, out iChildmoduleID);
                com.Parameters.Add("pChildModuleID", SqlDbType.SmallInt).Value = iChildmoduleID;
                com.Parameters.Add("pFrame", SqlDbType.VarChar).Value = strFrame;
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                SqlCommand com = conn.CreateCommand();
                com.CommandText = "UPDATE t_00_004 set C005=1,C006=1 where C001=@pLanCode";
                com.Parameters.Add("@pLanCode", SqlDbType.SmallInt).Value = int.Parse(strLanCode);
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
            SqlConnection conn = null;
            try
            {
                conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
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
        /// 获得所有的语言
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
            SqlConnection conn = CreateMSSQLConn(strHost, strPort, strServiceName, strUser, strPwd);
            try
            {
                string strSql = "SELECT C001,C002,ISNULL(C005,'')+ISNULL(C006,'') AS Displaymessage,ISNULL(C007,'')+ISNULL(C008,'') AS TipMessage,C009,C010,C011,C012 FROM T_00_005 WHERE C002 = @LanCode";
                SqlCommand com = conn.CreateCommand();
                com.CommandText = strSql;
                com.Parameters.Add("@LanCode", SqlDbType.SmallInt).Value = int.Parse(strLanCode);
                SqlDataAdapter adapter = new SqlDataAdapter(com);
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