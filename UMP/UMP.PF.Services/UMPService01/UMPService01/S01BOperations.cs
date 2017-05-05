using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace UMPService01
{
    public class S01BOperations
    {
        private string IStrSpliterChar = string.Empty;
        private TcpClient ITcpClient = null;

        #region 数据库类型及数据库连接参数
        private static int IIntDBType = 0;
        private static string IStrDBConnectProfile = string.Empty;
        #endregion

        public S01BOperations(int AIntDBType, string AStrDBConnectProfile, TcpClient ATcpClient)
        {
            IStrSpliterChar = AscCodeToChr(27);
            IIntDBType = AIntDBType;
            IStrDBConnectProfile = AStrDBConnectProfile;
            ITcpClient = ATcpClient;
        }

        public S01BOperations(int AIntDBType, string AStrDBConnectProfile)
        {
            IStrSpliterChar = AscCodeToChr(27);
            IIntDBType = AIntDBType;
            IStrDBConnectProfile = AStrDBConnectProfile;
        }

        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="AListStrOperationInfo">
        /// 0：客户端SessionID
        /// 1：模块ID                      2：功能操作编号                3：租户Token（5位）          4：操作用户ID 
        /// 5：当前操作角色                6：机器名                      7：机器IP                    8：操作时间 UTC
        /// 9：操作结果                    10：操作内容对应的语言包ID     11：替换参数                 12：异常错误
        /// </param>
        /// <param name="AStrReturnCode"></param>
        /// <param name="AStrReturnMessage"></param>
        /// <returns></returns>
        public bool S01BOperation01(List<string> AListStrOperationInfo, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;
            string LStrSelectSQL = string.Empty;
            List<string> LListStrReplaceArgs = new List<string>();
            List<string> LListStrException = new List<string>();

            try
            {
                #region 局部变量初始化、定义
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                #endregion

                #region 获取操作日志流水号
                string LStrOperationSerialID = string.Empty;
                if (AListStrOperationInfo[1] == "11000")
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 901, AListStrOperationInfo[3], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 902, AListStrOperationInfo[3], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //系统分配操作日志流水号失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01B01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LStrOperationSerialID = LDBOperationReturn.StrReturn;
                }
                #endregion

                #region 拆分替换参数和异常出错
                string LStrReplaceArgs = string.Empty;
                string LStrException = string.Empty;
                LStrReplaceArgs = AListStrOperationInfo[11];
                LStrException = AListStrOperationInfo[12];
                while (LStrReplaceArgs.Length > 1024)
                {
                    LListStrReplaceArgs.Add(LStrReplaceArgs.Substring(0,1024));
                    LStrReplaceArgs = LStrReplaceArgs.Substring(1024);
                }
                if (!string.IsNullOrEmpty(LStrReplaceArgs)) { LListStrReplaceArgs.Add(LStrReplaceArgs); }
                while (LListStrReplaceArgs.Count < 5) { LListStrReplaceArgs.Add(""); }

                while (LStrException.Length > 1024)
                {
                    LListStrException.Add(LStrException.Substring(0, 1024));
                    LStrException = LStrException.Substring(1024);
                }
                if (!string.IsNullOrEmpty(LStrException)) { LListStrException.Add(LStrException); }
                while (LListStrException.Count < 5) { LListStrException.Add("");}
                #endregion

                #region 将日志写入DataSet中
                //LStrSelectSQL = "SELECT * FROM T_11_901_" + AListStrOperationInfo[3] + " WHERE 1 = 2";
                LStrSelectSQL = "SELECT * FROM T_11_901 WHERE 1 = 2";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                DataSet LDataSetSave2DB = LDBOperationReturn.DataSetReturn;
                LDataSetSave2DB.Tables[0].TableName = "T_11_901";
                DataRow LDataRowNewLog = LDataSetSave2DB.Tables[0].NewRow();
                LDataRowNewLog.BeginEdit();
                LDataRowNewLog["C001"] = long.Parse(LStrOperationSerialID);
                LDataRowNewLog["C002"] = long.Parse(AListStrOperationInfo[0]);
                LDataRowNewLog["C003"] = Int16.Parse(AListStrOperationInfo[1]);
                LDataRowNewLog["C004"] = long.Parse(AListStrOperationInfo[2]);
                LDataRowNewLog["C005"] = long.Parse(AListStrOperationInfo[4]);
                LDataRowNewLog["C006"] = AListStrOperationInfo[6];
                LDataRowNewLog["C007"] = AListStrOperationInfo[7];
                LDataRowNewLog["C008"] = long.Parse((DateTime.Parse(AListStrOperationInfo[8])).ToString("yyyyMMddHHmmss"));
                LDataRowNewLog["C009"] = AListStrOperationInfo[9];
                LDataRowNewLog["C010"] = AListStrOperationInfo[10];
                LDataRowNewLog["C011"] = LListStrReplaceArgs[0];
                LDataRowNewLog["C012"] = LListStrReplaceArgs[1];
                LDataRowNewLog["C013"] = LListStrReplaceArgs[2];
                LDataRowNewLog["C014"] = LListStrReplaceArgs[3];
                LDataRowNewLog["C015"] = LListStrReplaceArgs[4];
                LDataRowNewLog["C016"] = LListStrException[0];
                LDataRowNewLog["C017"] = LListStrException[1];
                LDataRowNewLog["C018"] = LListStrException[2];
                LDataRowNewLog["C019"] = LListStrException[3];
                LDataRowNewLog["C020"] = LListStrException[4];
                LDataRowNewLog["C021"] = long.Parse(AListStrOperationInfo[5]);
                LDataRowNewLog["C022"] = AListStrOperationInfo[3];
                LDataRowNewLog.EndEdit();
                LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewLog);
                #endregion

                #region 将操作日志写入MSSQL数据库
                if (IIntDBType == 2)
                {
                    LSqlConnection = new SqlConnection(IStrDBConnectProfile);
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrSelectSQL, LSqlConnection);
                    SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();

                    LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LSqlCommandBuilder.SetAllValues = false;
                    LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                    LSqlDataAdapter.Update(LDataSetSave2DB, "T_11_901");
                    LDataSetSave2DB.AcceptChanges();
                    LSqlCommandBuilder.Dispose();
                    LSqlDataAdapter.Dispose();
                }
                #endregion

                #region 将操作日志写入Oracle数据库
                if (IIntDBType == 3)
                {
                    LOracleConnection = new OracleConnection(IStrDBConnectProfile);
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrSelectSQL, LOracleConnection);
                    OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                    LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LOracleCommandBuilder.SetAllValues = false;
                    LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                    LOracleDataAdapter.Update(LDataSetSave2DB, "T_11_901");
                    LDataSetSave2DB.AcceptChanges();
                    LOracleCommandBuilder.Dispose();
                    LOracleDataAdapter.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01B99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01BOperations.S01BOperation01()\n" + ex.Message;
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 0：客户端SessionID
        /// 1：模块ID                      2：功能操作编号                3：租户Token（5位）          4：操作用户ID 
        /// 5：当前操作角色                6：机器名                      7：机器IP                    8：操作时间 UTC
        /// 9：操作结果                    10：操作内容对应的语言包ID     11：替换参数                 12：异常错误
        /// </summary>
        /// <param name="AListStrOperationInfo"></param>
        /// <param name="AStrReturnMessage"></param>
        /// <returns></returns>
        public bool S01BOperation01(List<string> AListStrOperationInfo, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;
            string LStrSelectSQL = string.Empty;
            List<string> LListStrReplaceArgs = new List<string>();
            List<string> LListStrException = new List<string>();

            try
            {
                #region 局部变量初始化、定义

                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                #endregion

                #region 获取操作日志流水号
                string LStrOperationSerialID = string.Empty;
                if (AListStrOperationInfo[1] == "11000")
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 901, AListStrOperationInfo[3], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 902, AListStrOperationInfo[3], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //系统分配操作日志流水号失败
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LStrOperationSerialID = LDBOperationReturn.StrReturn;
                }
                #endregion

                #region 拆分替换参数和异常出错
                string LStrReplaceArgs = string.Empty;
                string LStrException = string.Empty;
                LStrReplaceArgs = AListStrOperationInfo[11];
                LStrException = AListStrOperationInfo[12];
                while (LStrReplaceArgs.Length > 1024)
                {
                    LListStrReplaceArgs.Add(LStrReplaceArgs.Substring(0, 1024));
                    LStrReplaceArgs = LStrReplaceArgs.Substring(1024);
                }
                if (!string.IsNullOrEmpty(LStrReplaceArgs)) { LListStrReplaceArgs.Add(LStrReplaceArgs); }
                while (LListStrReplaceArgs.Count < 5) { LListStrReplaceArgs.Add(""); }

                while (LStrException.Length > 1024)
                {
                    LListStrException.Add(LStrException.Substring(0, 1024));
                    LStrException = LStrException.Substring(1024);
                }
                if (!string.IsNullOrEmpty(LStrException)) { LListStrException.Add(LStrException); }
                while (LListStrException.Count < 5) { LListStrException.Add(""); }
                #endregion

                #region 将日志写入DataSet中
                LStrSelectSQL = "SELECT * FROM T_11_901 WHERE 1 = 2";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                DataSet LDataSetSave2DB = LDBOperationReturn.DataSetReturn;
                LDataSetSave2DB.Tables[0].TableName = "T_11_901";
                DataRow LDataRowNewLog = LDataSetSave2DB.Tables[0].NewRow();
                LDataRowNewLog.BeginEdit();
                LDataRowNewLog["C001"] = long.Parse(LStrOperationSerialID);
                LDataRowNewLog["C002"] = long.Parse(AListStrOperationInfo[0]);
                LDataRowNewLog["C003"] = Int16.Parse(AListStrOperationInfo[1]);
                LDataRowNewLog["C004"] = long.Parse(AListStrOperationInfo[2]);
                LDataRowNewLog["C005"] = long.Parse(AListStrOperationInfo[4]);
                LDataRowNewLog["C006"] = AListStrOperationInfo[6];
                LDataRowNewLog["C007"] = AListStrOperationInfo[7];
                LDataRowNewLog["C008"] = long.Parse((DateTime.Parse(AListStrOperationInfo[8])).ToString("yyyyMMddHHmmss"));
                LDataRowNewLog["C009"] = AListStrOperationInfo[9];
                LDataRowNewLog["C010"] = AListStrOperationInfo[10];
                LDataRowNewLog["C011"] = LListStrReplaceArgs[0];
                LDataRowNewLog["C012"] = LListStrReplaceArgs[1];
                LDataRowNewLog["C013"] = LListStrReplaceArgs[2];
                LDataRowNewLog["C014"] = LListStrReplaceArgs[3];
                LDataRowNewLog["C015"] = LListStrReplaceArgs[4];
                LDataRowNewLog["C016"] = LListStrException[0];
                LDataRowNewLog["C017"] = LListStrException[1];
                LDataRowNewLog["C018"] = LListStrException[2];
                LDataRowNewLog["C019"] = LListStrException[3];
                LDataRowNewLog["C020"] = LListStrException[4];
                LDataRowNewLog["C021"] = long.Parse(AListStrOperationInfo[5]);
                LDataRowNewLog["C022"] = AListStrOperationInfo[3];
                LDataRowNewLog.EndEdit();
                LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewLog);
                #endregion

                #region 将操作日志写入MSSQL数据库
                if (IIntDBType == 2)
                {
                    LSqlConnection = new SqlConnection(IStrDBConnectProfile);
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrSelectSQL, LSqlConnection);
                    SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();

                    LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LSqlCommandBuilder.SetAllValues = false;
                    LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                    LSqlDataAdapter.Update(LDataSetSave2DB, "T_11_901");
                    LDataSetSave2DB.AcceptChanges();
                    LSqlCommandBuilder.Dispose();
                    LSqlDataAdapter.Dispose();
                }
                #endregion

                #region 将操作日志写入Oracle数据库
                if (IIntDBType == 3)
                {
                    LOracleConnection = new OracleConnection(IStrDBConnectProfile);
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrSelectSQL, LOracleConnection);
                    OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                    LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LOracleCommandBuilder.SetAllValues = false;
                    LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                    LOracleDataAdapter.Update(LDataSetSave2DB, "T_11_901");
                    LDataSetSave2DB.AcceptChanges();
                    LOracleCommandBuilder.Dispose();
                    LOracleDataAdapter.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnMessage = "S01BOperations.S01BOperation01()\n" + ex.Message;
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LBoolReturn;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }
    }
}
