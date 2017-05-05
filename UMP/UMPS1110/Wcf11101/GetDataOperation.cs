using System;
using System.Collections.Generic;
using System.Data;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;

namespace Wcf11101
{
    public partial class Service11101
    {
        private OperationReturn GetResourcePropertyInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_00_009 ORDER BY C001, C002");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_009 ORDER BY C001, C002");
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                int intValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ObjectPropertyInfo item = new ObjectPropertyInfo();
                    item.ObjType = Convert.ToInt32(dr["C001"]);
                    item.PropertyID = Convert.ToInt32(dr["C002"]);
                    item.DataType = (ObjectPropertyDataType)Convert.ToInt32(dr["C003"]);
                    item.ConvertFormat = (ObjectPropertyConvertFormat)Convert.ToInt32(dr["C006"]);
                    item.GroupID = Convert.ToInt32(dr["C007"]);
                    item.SortID = Convert.ToInt32(dr["C008"]);
                    item.DefaultValue = dr["C009"].ToString();
                    item.IsParam = dr["C004"].ToString() == "1";
                    item.SourceID = Convert.ToInt32(dr["C010"]);
                    item.MinValue = dr["C012"].ToString();
                    item.MaxValue = dr["C013"].ToString();
                    item.Description = dr["C018"].ToString();
                    item.EncryptMode = (ObjectPropertyEncryptMode)Convert.ToInt32(dr["C019"]);
                    item.AttributeName = dr["C020"].ToString();
                    item.IsLocked = dr["C021"].ToString() == "1";       //被锁定的属性
                    if (int.TryParse(dr["C022"].ToString(), out intValue))
                    {
                        item.ControledPropID = intValue;        //被指定的属性控制
                    }
                    if (int.TryParse(dr["C023"].ToString(), out intValue))
                    {
                        item.MultiValueMode = intValue;     //多值模式
                    }
                    item.IsKeyProperty = dr["C024"].ToString() == "1";  //是否关键属性
                    if (int.TryParse(dr["C025"].ToString(), out intValue))
                    {
                        item.AuthField = intValue;          //是否认证字段
                    }
                    if (int.TryParse(dr["C026"].ToString(), out intValue))
                    {
                        item.BatchModify = intValue;
                    }
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetResourcePropertyValueList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      方式（0：根据资源类型；1：根据资源编号）
                //1      资源类型编号或资源编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strMethod = listParams[0];
                string strObjType;
                string strObjID;
                int intObjType = 0;
                long longObjID = 0;
                if (strMethod == "0")
                {
                    strObjType = listParams[1];
                    if (!int.TryParse(strObjType, out intObjType))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("ObjectType invalid");
                        return optReturn;
                    }
                }
                else if (strMethod == "1")
                {
                    strObjID = listParams[1];
                    if (!long.TryParse(strObjID, out longObjID))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("ObjectID invalid");
                        return optReturn;
                    }
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strMethod == "0")
                        {
                            if (intObjType == 0)
                            {
                                strSql =
                                     string.Format(
                                         "SELECT * FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 ORDER BY C001, C002"
                                         , rentToken);
                            }
                            else
                            {
                                strSql =
                                   string.Format(
                                       "SELECT * FROM T_11_101_{0} WHERE C001 >= {1} AND C001 < {2} ORDER BY C001, C002"
                                       , rentToken
                                       , string.Format("{0}0000000000000000", intObjType)
                                       , string.Format("{0}0000000000000000", intObjType + 1));
                            }
                        }
                        else
                        {
                            if (longObjID == 0)
                            {
                                strSql =
                                     string.Format(
                                         "SELECT * FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 ORDER BY C001, C002"
                                         , rentToken);
                            }
                            else
                            {
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002"
                                        , rentToken
                                        , longObjID);
                            }
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
                            if (intObjType == 0)
                            {
                                strSql =
                                     string.Format(
                                         "SELECT * FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 ORDER BY C001, C002"
                                         , rentToken);
                            }
                            else
                            {
                                strSql =
                                   string.Format(
                                       "SELECT * FROM T_11_101_{0} WHERE C001 >= {1} AND C001 < {2} ORDER BY C001, C002"
                                       , rentToken
                                       , string.Format("{0}0000000000000000", intObjType)
                                       , string.Format("{0}0000000000000000", intObjType + 1));
                            }
                        }
                        else
                        {
                            if (longObjID == 0)
                            {
                                strSql =
                                     string.Format(
                                         "SELECT * FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 ORDER BY C001, C002"
                                         , rentToken);
                            }
                            else
                            {
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002"
                                        , rentToken
                                        , longObjID);
                            }
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ResourceProperty item = new ResourceProperty();
                    long objID = Convert.ToInt64(dr["C001"]);
                    int objType = objID.ToString().Length < 3 ? 0 : Convert.ToInt32(objID.ToString().Substring(0, 3));
                    int rowNumber = Convert.ToInt32(dr["C002"]);
                    item.ObjID = objID;
                    item.ObjType = objType;
                    int propertyID;
                    //数据库里每行保存10个属性，分别取出
                    for (int j = 1; j <= 10; j++)
                    {
                        propertyID = (rowNumber - 1) * 10 + j;
                        item.PropertyID = propertyID;
                        string strValue = dr[string.Format("C{0}", (j + 10).ToString("000"))].ToString();
                        //重置
                        item.EncryptMode = ObjectPropertyEncryptMode.Unkown;
                        item.Value = string.Empty;
                        item.ListOtherValues.Clear();
                        DecodeEncryptValue(strValue, item);

                        optReturn = XMLHelper.SeriallizeObject(item);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        listReturn.Add(optReturn.Data.ToString());
                    }
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetResourceTypeParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_00_010 ORDER BY C001, C003");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_00_010 ORDER BY C001, C003");
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ResourceTypeParam item = new ResourceTypeParam();
                    item.TypeID = Convert.ToInt32(dr["C001"]);
                    item.ParentID = Convert.ToInt32(dr["C002"]);
                    item.OrderID = Convert.ToInt32(dr["C003"]);
                    item.IsShow = dr["C004"].ToString() == "1";
                    item.Icon = dr["C005"].ToString();
                    item.Description = dr["C010"].ToString();
                    item.IsMasterSlaver = dr["C011"].ToString() == "1";
                    item.HasChildList = dr["C012"].ToString() == "1";
                    item.NodeName = dr["C020"].ToString();
                    if (item.HasChildList)
                    {
                        item.ChildType = Convert.ToInt32(dr["C013"]);
                        item.IntValue01 = Convert.ToInt32(dr["C014"]);
                        item.IntValue02 = Convert.ToInt32(dr["C015"]);
                    }
                    item.IsAuthention = dr["C016"].ToString() == "1";
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetResourceGroupParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_00_011 ORDER BY C001,  C003, C004");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_00_011 ORDER BY C001,  C003, C004");
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ResourceGroupParam item = new ResourceGroupParam();
                    item.TypeID = Convert.ToInt32(dr["C001"]);
                    item.GroupID = Convert.ToInt32(dr["C002"]);
                    item.ParentGroup = Convert.ToInt32(dr["C003"]);
                    item.SortID = Convert.ToInt32(dr["C004"]);
                    item.IsShow = dr["C005"].ToString() == "1";
                    item.Icon = dr["C006"].ToString();
                    item.Description = dr["C010"].ToString();
                    item.GroupType = string.IsNullOrEmpty(dr["C011"].ToString())
                        ? ResourceGroupType.Unkown
                        : (ResourceGroupType)Convert.ToInt32(dr["C011"]);
                    item.IsCaculateCount = dr["C012"].ToString() == "1";
                    item.ChildType = string.IsNullOrEmpty(dr["C013"].ToString()) ? -1 : Convert.ToInt32(dr["C013"]);
                    item.IntValue01 = string.IsNullOrEmpty(dr["C014"].ToString()) ? -1 : Convert.ToInt32(dr["C014"]);
                    item.IntValue02 = string.IsNullOrEmpty(dr["C015"].ToString()) ? -1 : Convert.ToInt32(dr["C015"]);
                    item.NodeName = dr["C020"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetBasicInfoDataList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     类型，     0：通过InfoID获取列表；
                //                 1：通过上级InfoID获取列表；
                //                 2：通过上级InfoID范围和上级InfoValue获取列表
                //                 3：通过InfoID范围(前4位，即模块号）获取整个模块下的列表
                //1     InfoID(或范围)
                //2     InfoValue
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strType = listParams[0];
                string strInfoID = listParams[1];
                string strInfoValue = listParams[2];
                int infoID;
                if (!int.TryParse(strInfoID, out infoID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("InfoID invalid");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strType == "1")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 = {0} ORDER BY C001, C002", infoID);
                        }
                        else if (strType == "2")
                        {
                            //计算范围基数
                            infoID = infoID / 1000 * 1000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 IN (SELECT C001 FROM T_00_003 WHERE C001 > {0} AND C001 < {1} AND C006 = '{2}') ORDER BY C001,C002",
                                    infoID, infoID + 1000, strInfoValue);
                        }
                        else if (strType == "3")
                        {
                            //计算范围基数
                            infoID = infoID / 100000 * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001,C002",
                                    infoID, infoID + 100000);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002", strInfoID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strType == "1")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 = {0} ORDER BY C001, C002", infoID);
                        }
                        else if (strType == "2")
                        {
                            infoID = infoID / 1000 * 1000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 IN (SELECT C001 FROM T_00_003 WHERE C001 > {0} AND C001 < {1} AND C006 = '{2}') ORDER BY C001,C002",
                                    infoID, infoID + 1000, strInfoValue);
                        }
                        else if (strType == "3")
                        {
                            //计算范围基数
                            infoID = infoID / 100000 * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001,C002",
                                    infoID, infoID + 100000);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002", strInfoID);
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicInfoData item = new BasicInfoData();
                    item.InfoID = Convert.ToInt32(dr["C001"]);
                    item.SortID = Convert.ToInt32(dr["C002"]);
                    item.ParentID = Convert.ToInt32(dr["C003"]);
                    item.IsEnable = dr["C004"].ToString() == "1";
                    item.EncryptVersion = Convert.ToInt32(dr["C005"]);
                    item.Value = dr["C006"].ToString();
                    item.Icon = dr["C007"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetSftpUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_005_{0} WHERE C007 = 'H' ORDER BY C001",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_005_{0} WHERE C007 = 'H' ORDER BY C001",
                               rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicUserInfo item = new BasicUserInfo();
                    item.UserID = Convert.ToInt64(dr["C001"]);
                    item.Account = DecryptFromDB(dr["C002"].ToString());
                    item.FullName = DecryptFromDB(dr["C003"].ToString());
                    item.Password = DecryptFromDB003(dr["C004"].ToString());
                    item.Source = dr["C007"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetBasicResourceInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      资源类型，0 表示获取所有类型资源的属性值(仅限210~300编码的资源）
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strResourceType = listParams[1];
                int intResourceType;
                if (string.IsNullOrEmpty(strUserID)
                    || !int.TryParse(strResourceType, out intResourceType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid.");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (intResourceType == 0)
                        {
                            strSql =
                                 string.Format(
                                     "SELECT C001, C002, C011, C012, C013 FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 AND C002 = 1 ORDER BY C001, C002"
                                     , rentToken);
                        }
                        else
                        {
                            strSql =
                               string.Format(
                                   "SELECT C001, C002, C011, C012, C013 FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002"
                                   , rentToken
                                   , string.Format("{0}0000000000000000", intResourceType)
                                   , string.Format("{0}0000000000000000", intResourceType + 1));
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (intResourceType == 0)
                        {
                            strSql =
                                 string.Format(
                                     "SELECT C001, C002, C011, C012, C013 FROM T_11_101_{0} WHERE C001 > 2100000000000000000 AND C001 < 3000000000000000000 AND C002 = 1 ORDER BY C001, C002"
                                     , rentToken);
                        }
                        else
                        {
                            strSql =
                              string.Format(
                                  "SELECT C001, C002, C011, C012, C013 FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002"
                                  , rentToken
                                  , string.Format("{0}0000000000000000", intResourceType)
                                  , string.Format("{0}0000000000000000", intResourceType + 1));
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicResourceInfo item = new BasicResourceInfo();
                    long objID = Convert.ToInt64(dr["C001"]);
                    item.ObjectID = objID;
                    item.Key = string.IsNullOrEmpty(dr["C011"].ToString()) ? 0 : Convert.ToInt32(dr["C011"]);
                    item.ID = string.IsNullOrEmpty(dr["C012"].ToString()) ? 0 : Convert.ToInt32(dr["C012"]);
                    item.ParentID = string.IsNullOrEmpty(dr["C013"].ToString()) ? 0 : Convert.ToInt64(dr["C013"]);
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetResourceObjectList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                /*
                 * ***获取通用资源对象信息***
                 * 
                 * 1、获取筛选条件信息
                 * ...
                 * 
                 * 返回类型为ResourceObject
                 * 根据ObjType判断资源类型，参考UMPCommon的ConstValue中或Common11101中S1110Consts中的定义
                 * 
                 * 各资源保留字段意义的定义：
                 * 
                 * 257（筛选条件）
                 *      Other01：C001（筛选类型，1 录音记录）
                 *      Other02：C002（策略类型，1 归档策略；2 备份策略；3 回删策略）
                 *      Other10：C012（备注信息）
                 * 
                 */


                //ListParam
                //0     用户编号（0 表示不考虑用户管理权限）
                //1     资源类型码
                //2     上级资源编码（0 表示不考虑父级资源编码）
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strObjType = listParams[1];
                string strParentID = listParams[2];
                int intObjType;
                if (!int.TryParse(strObjType, out intObjType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ObjType invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        switch (intObjType)
                        {
                            case S1110Consts.RESOURCE_FILTERCONDITION:
                                strSql = string.Format("SELECT * FROM T_00_202 ORDER BY C003");
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSet = optReturn.Data as DataSet;
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                                optReturn.Message = string.Format("ObjType not impliment");
                                return optReturn;
                        }
                        break;
                    case 3:
                        switch (intObjType)
                        {
                            case S1110Consts.RESOURCE_FILTERCONDITION:
                                strSql = string.Format("SELECT * FROM T_00_202 ORDER BY C003");
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSet = optReturn.Data as DataSet;
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                                optReturn.Message = string.Format("ObjType not impliment");
                                return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                string strName;
                string strDeleted;
                string strDisabled;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ResourceObject item = new ResourceObject();
                    switch (intObjType)
                    {
                        case S1110Consts.RESOURCE_FILTERCONDITION:
                            item.ObjID = Convert.ToInt64(dr["C003"]);
                            item.ObjType = S1110Consts.RESOURCE_FILTERCONDITION;
                            strName = dr["C004"].ToString();
                            strName = DecryptFromDB(strName);
                            item.Name = strName;
                            item.FullName = item.Name;
                            strDeleted = dr["C006"].ToString();
                            strDisabled = dr["C005"].ToString();
                            item.State = ((strDeleted == "1" ? 1 : 0) | (strDisabled == "0" ? 2 : 0));
                            item.Other01 = dr["C001"].ToString();
                            item.Other02 = dr["C002"].ToString();
                            item.Other10 = dr["C012"].ToString();
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                            optReturn.Message = string.Format("ObjType not impliment");
                            return optReturn;
                    }
                    if (item.State != 0)
                    {
                        //如果资源的状态不是正常的状态，则不获取此资源
                        continue;
                    }
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private void DecodeMultiValues(string strValue, ResourceProperty item)
        {
            //是复合值（以连续三个char30开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR_2)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR_2 },
                    StringSplitOptions.None);
                string strType;
                if (arrContent.Length > 0)
                {
                    strType = arrContent[0];
                    int intValue;
                    if (int.TryParse(strType, out intValue))
                    {
                        item.MultiValueMode = intValue;
                    }
                }
                if (arrContent.Length > 1)
                {
                    item.Value = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    for (int i = 2; i < arrContent.Length; i++)
                    {
                        item.ListOtherValues.Add(arrContent[i]);
                    }
                }
            }
            else
            {
                item.Value = strValue;
            }
        }

        private void DecodeEncryptValue(string strValue, ResourceProperty item)
        {
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                string strVersion = string.Empty, strMode = string.Empty, strPass = strValue;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                if (strVersion == "2" && strMode == "hex")
                {
                    item.EncryptMode = ObjectPropertyEncryptMode.E2Hex;
                    DecodeMultiValues(DecryptFromDB(strPass), item);
                }
                else if (strVersion == "3" && strMode == "hex")
                {
                    item.EncryptMode = ObjectPropertyEncryptMode.SHA256;
                    DecodeMultiValues(DecryptFromDB(strPass), item);
                }
                else
                {
                    DecodeMultiValues(strPass, item);
                }
            }
            else
            {
                DecodeMultiValues(strValue, item);
            }
        }
    }
}