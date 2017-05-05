using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.ScoreSheets;

namespace Wcf31011
{
    public partial class Service31011
    {
        #region RemoveScoreObject

        private OperationReturn RemoveScoreSheet(SessionInfo session, string strScoreSheetID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C001 = {1}", rentToken, strScoreSheetID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C001 = {1}", rentToken, strScoreSheetID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("NoExist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        long useCount;
                        long.TryParse(dr["C017"].ToString(), out useCount);
                        if (useCount == 0)
                        {
                            dr.Delete();
                            strMsg += string.Format("Remove:{0};", strID);

                            #region 删除依赖对象

                            optReturn = RemoveScoreItems(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveComments(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveControlItems(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveVisualStyles(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveScoreSheetManagement(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                        }
                        else
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_EXIST;
                            optReturn.Message = string.Format("BeUsed");
                            return optReturn;
                        }

                        

                        #endregion

                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveScoreItems(SessionInfo session, ScoreGroup scoreGroup)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (scoreGroup == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreGroup or ScoreSheet is null");
                    return optReturn;
                }

                #region ScoreItem

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C004 = {1}", rentToken, scoreGroup.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C004 = {1}", rentToken, scoreGroup.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }

                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C002"].ToString();
                        ScoreItem scoreItem = scoreGroup.Items.FirstOrDefault(l => l.ID.ToString() == strID);
                        if (scoreItem == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            strMsg += string.Format("Remove {0};", strID);

                            #region 删除以来对象

                            optReturn = RemoveScoreItems(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveComments(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveVisualStyles(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveStandardItems(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            #endregion

                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveComments(SessionInfo session, ScoreItem scoreItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (scoreItem == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreItem is null");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C002 = {1}", rentToken, scoreItem.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C002 = {1}", rentToken, scoreItem.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        Comment temp = scoreItem.Comments.FirstOrDefault(c => c.ID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            strMsg += string.Format("Remove {0};", strID);

                            #region 删除依赖对象

                            optReturn = RemoveVisualStyles(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            optReturn = RemoveCommentItems(session, strID);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strMsg += optReturn.Data;

                            #endregion

                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveVisualStyle(SessionInfo session, string parentID, string type)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C003 = {1} AND C005 = '{2}'",
                            rentToken,
                            parentID,
                            type);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C003 = {1} AND C005 = '{2}'",
                            rentToken,
                            parentID,
                            type);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove {0};", strID);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveStandardItems(SessionInfo session, ItemStandard itemStandard)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (itemStandard == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreItem is null");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C002 = {1}", rentToken, itemStandard.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C002 = {1}", rentToken, itemStandard.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        StandardItem temp = itemStandard.ValueItems.FirstOrDefault(c => c.ID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            strMsg += string.Format("Remove {0};", strID);
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveCommentItems(SessionInfo session, ItemComment itemComment)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (itemComment == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ItemComment is null");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C002 = {1}", rentToken, itemComment.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C002 = {1}", rentToken, itemComment.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        CommentItem temp = itemComment.ValueItems.FirstOrDefault(c => c.ID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            strMsg += string.Format("Remove {0};", strID);
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveControlItems(SessionInfo session, ScoreSheet scoreSheet)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C002 = {1}", rentToken, scoreSheet.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C002 = {1}", rentToken, scoreSheet.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        ControlItem temp = scoreSheet.ControlItems.FirstOrDefault(c => c.ID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            strMsg += string.Format("Remove {0};", strID);
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveScoreItems(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C004 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C004 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C002"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);

                        #region 依赖对象

                        optReturn = RemoveScoreItems(session, strID);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strMsg += optReturn.Data;

                        optReturn = RemoveComments(session, strID);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strMsg += optReturn.Data;

                        optReturn = RemoveVisualStyles(session, strID);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strMsg += optReturn.Data;

                        #endregion

                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveComments(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);

                        #region 依赖对象

                        optReturn = RemoveVisualStyles(session, strID);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strMsg += optReturn.Data;

                        #endregion

                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveVisualStyles(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C003 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C003 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveStandardItems(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C004 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C004 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveCommentItems(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveControlItems(SessionInfo session, string parentID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C002 = {1}", rentToken, parentID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        dr.Delete();
                        strMsg += string.Format("Remove:{0};", strID);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn RemoveScoreSheetManagement(SessionInfo session, string scoreSheetID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}", rentToken, scoreSheetID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}", rentToken, scoreSheetID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        dr.Delete();
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion
    }
}