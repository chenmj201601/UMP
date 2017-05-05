using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common000A1;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace Wcf000A1
{
    public partial class Service000A1
    {

        private OperationReturn GetLogRecordData(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 8)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMediaType = listParams[1];
                string strEncryptFlag = listParams[2];
                string strBeginTime = listParams[3];
                string strEndTime = listParams[4];
                string strMaxNum = listParams[5];
                string strOrderMode = listParams[6];
                string strCondition = listParams[7];
                WriteOperationLog(
                    string.Format(
                        "GetLogRecordData:\tUserID:{0};MediaType:{1};EncryptFlag:{2};BeginTime:{3};EndTime:{4};MaxNum:{5};OrderMode:{6};Condition:{7}",
                        strUserID,
                        strMediaType,
                        strEncryptFlag,
                        strBeginTime,
                        strEndTime,
                        strMaxNum,
                        strOrderMode,
                        strCondition));

                #endregion


                #region 检查参数有效性

                int intMaxNum;
                if (!int.TryParse(strMaxNum, out intMaxNum)
                    || intMaxNum < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("MaxNum invalid");
                    return optReturn;
                }

                DateTime dtBegin, dtEnd;
                if (!DateTime.TryParse(strBeginTime, out dtBegin)
                    || !DateTime.TryParse(strEndTime, out dtEnd)
                    || dtBegin > dtEnd)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("BeginTime or EndTime invalid");
                    return optReturn;
                }

                #endregion


                #region 加载查询条件项目

                List<XmlMappingItem> listConditions = new List<XmlMappingItem>();

                //RowID，SerialID和RecordReference字段内置
                XmlMappingItem queryItem = new XmlMappingItem();
                queryItem.Name = UMPRecordInfo.PRO_ROWID;
                queryItem.Column = "C001";
                queryItem.DataType = (int) DBDataType.Long;
                listConditions.Add(queryItem);
                queryItem = new XmlMappingItem();
                queryItem.Name = UMPRecordInfo.PRO_SERIALID;
                queryItem.Column = "C002";
                queryItem.DataType = (int)DBDataType.NVarchar;
                listConditions.Add(queryItem);
                queryItem = new XmlMappingItem();
                queryItem.Name = UMPRecordInfo.PRO_RECORDREFERENCE;
                queryItem.Column = "C077";
                queryItem.DataType = (int)DBDataType.NVarchar;
                listConditions.Add(queryItem);

                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, QueryConfigInfo.FILE_NAME);
                if (File.Exists(path))
                {
                    optReturn = XMLHelper.DeserializeFile<QueryConfigInfo>(path);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    QueryConfigInfo queryInfo = optReturn.Data as QueryConfigInfo;
                    if (queryInfo == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("QueryConfigInfo is null");
                        return optReturn;
                    }
                    for (int i = 0; i < queryInfo.Items.Count; i++)
                    {
                        var temp = listConditions.FirstOrDefault(c => c.Column == queryInfo.Items[i].Column);
                        if (temp == null)
                        {
                            listConditions.Add(queryInfo.Items[i]);
                        }
                    }
                }
                WriteOperationLog(string.Format("QueryConfigInfo count:{0}", listConditions.Count));

                #endregion


                #region 加载记录字段信息

                List<XmlMappingItem> listRecordConfigs = new List<XmlMappingItem>();
                path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, RecordConfigInfo.FILE_NAME);
                if (File.Exists(path))
                {
                    optReturn = XMLHelper.DeserializeFile<RecordConfigInfo>(path);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecordConfigInfo info = optReturn.Data as RecordConfigInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("RecordConfigInfo is null");
                        return optReturn;
                    }
                    for (int i = 0; i < info.Items.Count; i++)
                    {
                        listRecordConfigs.Add(info.Items[i]);
                    }
                }
                WriteOperationLog(string.Format("RecordConfig count:{0}", listRecordConfigs.Count));

                #endregion


                #region 创建查询语句

                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                WriteOperationLog(string.Format("DatabaseInfo:{0}", dbInfo));
                string strConn = dbInfo.GetConnectionString();
                string strSql;
                string strTemp;
                string strRentToken = "00000";
                DataSet objDataSet;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = string.Empty;
                        strSql += string.Format("SELECT {0}* FROM T_21_001_{1} ",
                            intMaxNum > 0 ? string.Format("TOP {0} ", intMaxNum) : string.Empty,
                            strRentToken);


                        #region 基本条件

                        strSql += string.Format("WHERE C001 > 0 ");
                        strSql += string.Format("AND C005 >= '{0}' AND C005 <= '{1}' ",
                            dtBegin.ToString("yyyy-MM-dd HH:mm:ss"),
                            dtEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                        if (!string.IsNullOrEmpty(strMediaType)
                            && strMediaType != "0")
                        {
                            if (strMediaType == "1")
                            {
                                strSql += string.Format("AND (C014 = '0' OR C014 = '1') ");
                            }
                            if (strMediaType == "2")
                            {
                                strSql += string.Format("AND C014 = '1' ");
                            }
                            if (strMediaType == "3")
                            {
                                strSql += string.Format("AND C014 = '2' ");
                            }
                        }

                        ////注释，由于录音表中不会出现待加密或加密失败的记录，所以加密标识的条件暂时不用 2015-10-23 by charley
                        //strTemp = string.Format("(C025 = '0' OR C025 = '2' ");
                        //if (strEncryptFlag == "1")
                        //{
                        //    strTemp += string.Format("OR C025 = 'E' ");
                        //}
                        //if (strEncryptFlag == "2")
                        //{
                        //    strTemp += string.Format("OR C025 = 'E' OR C025 = 'F' ");
                        //}
                        //strTemp += string.Format(")");
                        //strSql += string.Format("AND {0} ", strTemp);

                        #endregion


                        #region 其他条件

                        if (!string.IsNullOrEmpty(strCondition))
                        {
                            JsonObject json = new JsonObject(strCondition);
                            var keys = json.GetPropertyNames();
                            for (int i = 0; i < listConditions.Count; i++)
                            {
                                var condition = listConditions[i];
                                var key = keys.FirstOrDefault(k => k.ToUpper() == condition.Name.ToUpper());
                                if (string.IsNullOrEmpty(key)) { continue;}
                                if (json[key] != null)
                                {
                                    var column = condition.Column;
                                    var dataType = condition.DataType;
                                    int count;
                                    switch (dataType)
                                    {
                                        case (int)DBDataType.Short:
                                        case (int)DBDataType.Int:
                                        case (int)DBDataType.Long:
                                        case (int)DBDataType.Number:
                                            //数值型
                                            count = json[key].Count;
                                            if (count > 1)
                                            {
                                                WriteOperationLog(string.Format("Count:{0}", count));
                                                string arr = string.Empty;
                                                for (int j = 0; j < count; j++)
                                                {
                                                    var temp = json[key][j].Number;
                                                    arr += string.Format(" {0},", temp);
                                                }
                                                arr = arr.Substring(0, arr.Length - 1);     //去除多余的逗号
                                                strSql += string.Format("AND {0} in ( {1} ) ", column, arr);
                                            }
                                            else
                                            {
                                                var numberValue = json[key].Number;
                                                strSql += string.Format("AND {0} = {1} ", column, numberValue);
                                            }
                                            break;
                                        default:
                                            //字符串型
                                            count = json[key].Count;
                                            if (count > 1)
                                            {
                                                WriteOperationLog(string.Format("Count:{0}", count));
                                                string arr = string.Empty;
                                                for (int j = 0; j < count; j++)
                                                {
                                                    var temp = json[key][j].Value;
                                                    arr += string.Format(" '{0}',", temp);
                                                }
                                                arr = arr.Substring(0, arr.Length - 1);     //去除多余的逗号
                                                strSql += string.Format("AND {0} in ( {1} ) ", column, arr);
                                            }
                                            else
                                            {
                                                var strValue = json[key].Value;
                                                strSql += string.Format("AND {0} = '{1}' ", column, strValue);
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        #endregion


                        #region 排序

                        strTemp = string.Format("ORDER BY C001, C005");
                        if (!string.IsNullOrEmpty(strOrderMode)
                            && strOrderMode != "0")
                        {
                            if (strOrderMode == "1")
                            {
                                strTemp = string.Format("ORDER BY C005, C001");
                            }
                            if (strOrderMode == "2")
                            {
                                strTemp = string.Format("ORDER BY C005 DESC, C001 DESC");
                            }
                        }
                        strSql += string.Format("{0} ", strTemp);

                        #endregion


                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Message += string.Format("\r\n{0}", strSql);
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Empty;
                        strSql += string.Format("SELECT * FROM T_21_001_{0} ", strRentToken);


                        #region 基本条件

                        strSql += string.Format("WHERE C001 > 0 ");
                        strSql += string.Format("AND C005 >= TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND C005 <= TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS') ",
                            dtBegin.ToString("yyyy-MM-dd HH:mm:ss"),
                            dtEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                        if (!string.IsNullOrEmpty(strMediaType)
                            && strMediaType != "0")
                        {
                            if (strMediaType == "1")
                            {
                                strSql += string.Format("AND (C014 = '0' OR C014 = '1') ");
                            }
                            if (strMediaType == "2")
                            {
                                strSql += string.Format("AND C014 = '1' ");
                            }
                            if (strMediaType == "3")
                            {
                                strSql += string.Format("AND C014 = '2' ");
                            }
                        }

                        ////注释，由于录音表中不会出现待加密或加密失败的记录，所以加密标识的条件暂时不用 2015-10-23 by charley
                        //strTemp = string.Format("(C025 = '0' OR C025 = '2' ");
                        //if (strEncryptFlag == "1")
                        //{
                        //    strTemp += string.Format("OR C025 = 'E' ");
                        //}
                        //if (strEncryptFlag == "2")
                        //{
                        //    strTemp += string.Format("OR C025 = 'E' OR C025 = 'F' ");
                        //}
                        //strTemp += string.Format(")");
                        //strSql += string.Format("AND {0} ", strTemp);

                        #endregion


                        #region 其他条件

                        if (!string.IsNullOrEmpty(strCondition))
                        {
                            JsonObject json = new JsonObject(strCondition);
                            for (int i = 0; i < listConditions.Count; i++)
                            {
                                var condition = listConditions[i];
                                if (json[condition.Name] != null)
                                {
                                    var name = condition.Name;
                                    var column = condition.Column;
                                    var dataType = condition.DataType;
                                    switch (dataType)
                                    {
                                        case (int)DBDataType.Short:
                                        case (int)DBDataType.Int:
                                        case (int)DBDataType.Long:
                                        case (int)DBDataType.Number:
                                            //数值型
                                            if (json[name].Count > 1)
                                            {
                                                string arr = string.Empty;
                                                for (int j = 0; j < json[name].Count; j++)
                                                {
                                                    var temp = json[name][j].Number;
                                                    arr += string.Format(" {0},", temp);
                                                }
                                                arr = arr.Substring(0, arr.Length - 1);
                                                strSql += string.Format("AND {0} in ( {1} ) ", column, arr);
                                            }
                                            else
                                            {
                                                var numberValue = json[name].Number;
                                                strSql += string.Format("AND {0} = {1} ", column, numberValue);
                                            }
                                            break;
                                        default:
                                            //字符串型
                                            if (json[name].Count > 1)
                                            {
                                                string arr = string.Empty;
                                                for (int j = 0; j < json[name].Count; j++)
                                                {
                                                    var temp = json[name][j].Value;
                                                    arr += string.Format(" '{0}',", temp);
                                                }
                                                arr = arr.Substring(0, arr.Length - 1);
                                                strSql += string.Format("AND {0} in ( {1} ) ", column, arr);
                                            }
                                            else
                                            {
                                                var strValue = json[name].Value;
                                                strSql += string.Format("AND {0} = '{1}' ", column, strValue);
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        #endregion


                        #region 排序

                        strTemp = string.Format("ORDER BY C001, C005");
                        if (!string.IsNullOrEmpty(strOrderMode)
                            && strOrderMode != "0")
                        {
                            if (strOrderMode == "1")
                            {
                                strTemp = string.Format("ORDER BY C005, C001");
                            }
                            if (strOrderMode == "2")
                            {
                                strTemp = string.Format("ORDER BY C005 DESC, C001 DESC");
                            }
                        }
                        strSql += string.Format("{0} ", strTemp);
                        if (intMaxNum > 0)
                        {
                            strSql = string.Format("SELECT * FROM ({0}) WHERE ROWID <= {1}", strSql, intMaxNum);
                        }

                        #endregion


                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Message += string.Format("\r\n{0}", strSql);
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid.\t{0}", dbInfo.TypeID);
                        return optReturn;
                }
                WriteOperationLog(string.Format("QueryString:{0}", strSql));

                #endregion


                #region 填充记录数据

                if (objDataSet == null
                    || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or table not found");
                    return optReturn;
                }
                optReturn.Message = strSql;
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    UMPRecordInfo item = new UMPRecordInfo();
                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = dr["C002"].ToString();
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]);
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.ServerID = Convert.ToInt32(dr["C037"]);
                    item.ServerIP = dr["C020"].ToString();
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();

                    //Json信息
                    optReturn = GetJsonObjectFromRecordInfo(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    JsonObject json = optReturn.Data as JsonObject;
                    if (json == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("JsonObject is null");
                        return optReturn;
                    }

                    //其他字段
                    for (int j = 0; j < listRecordConfigs.Count; j++)
                    {
                        var recordConfig = listRecordConfigs[j];
                        json[recordConfig.Name] = new JsonProperty(string.Format("\"{0}\"", dr[recordConfig.Column]));
                    }

                    string jsonString = json.ToString();
                    item.StringInfo = jsonString;

                    listReturn.Add(jsonString);
                }
                WriteOperationLog(string.Format("Record count:{0}", listReturn.Count));

                optReturn.Data = listReturn;
                optReturn.Message = strSql;

                #endregion

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetLogRecordUrl(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strSerialID = listParams[2];
                string strDecryptPassword = listParams[3];
                string strLoginPassword = listParams[4];
                string strOption = listParams[5];

                WriteOperationLog(
                    string.Format(
                        "GetLogRecordUrl:\tUserID:{0};Method:{1};SerialID:{2};DecryptPassword:***;LoginPassword:***;Option:{3}",
                        strUserID,
                        strMethod,
                        strSerialID,
                        strOption));

                List<string> listReturn = new List<string>();
                string strRecordInfo;
                string strFileName;
                int intMethod;
                long serialID;
                int MediaType;
                string encryptFlag;
                JsonObject jsonRecord;
                JsonObject jsonOption;
                UMPRecordInfo recordInfo;

                string strPrefer = "0";
                string strDecryptRecord = "0";
                string strConvertWaveFormat = "0";

                #endregion


                #region 检查参数有效性

                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid.\t{0}", strMethod);
                    return optReturn;
                }
                if (!string.IsNullOrEmpty(strOption))
                {
                    try
                    {
                        jsonOption = new JsonObject(strOption);
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Option param invalid.\t{0}", ex.Message);
                        return optReturn;
                    }
                    if (jsonOption[S000AConsts.OPTION_FIELD_DOWNLOADPREFERENCE] != null)
                    {
                        strPrefer = jsonOption[S000AConsts.OPTION_FIELD_DOWNLOADPREFERENCE].Value;
                    }
                    if (jsonOption[S000AConsts.OPTION_FIELD_DECRYPTRECORD] != null)
                    {
                        strDecryptRecord = jsonOption[S000AConsts.OPTION_FIELD_DECRYPTRECORD].Value;
                    }
                    if (jsonOption[S000AConsts.OPTION_FIELD_CONVERTWAVEFORMAT] != null)
                    {
                        strConvertWaveFormat = jsonOption[S000AConsts.OPTION_FIELD_CONVERTWAVEFORMAT].Value;
                    }
                }

                #endregion


                #region 获取RecordInfo，如果Method为0、1、2，通过SerialID查询得到RecordInfo

                if (intMethod < 10)
                {
                    List<string> listTemp = new List<string>();
                    listTemp.Add(strUserID);
                    listTemp.Add("0");
                    listTemp.Add("0");
                    listTemp.Add(DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss"));
                    listTemp.Add(DateTime.Parse("2199/12/31").ToString("yyyy-MM-dd HH:mm:ss"));
                    listTemp.Add("0");
                    listTemp.Add("0");
                    switch (intMethod)
                    {
                        case 0:
                        case 1:
                            if (!long.TryParse(strSerialID, out serialID))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("SerialID param invalid.\t{0}", strSerialID);
                                return optReturn;
                            }
                            JsonObject jsonSerialID = new JsonObject();
                            jsonSerialID[UMPRecordInfo.PRO_SERIALID] =
                                new JsonProperty(string.Format("\"{0}\"", serialID));
                            listTemp.Add(jsonSerialID.ToString());
                            break;
                        case 2:
                            if (!long.TryParse(strSerialID, out serialID))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("SerialID param invalid.\t{0}", strSerialID);
                                return optReturn;
                            }
                            JsonObject jsonRowID = new JsonObject();
                            jsonRowID[UMPRecordInfo.PRO_ROWID] = new JsonProperty(string.Format("{0}", serialID));
                            listTemp.Add(jsonRowID.ToString());
                            break;
                        case 3:
                            JsonObject jsonRefID = new JsonObject();
                            jsonRefID[UMPRecordInfo.PRO_RECORDREFERENCE] =
                                new JsonProperty(string.Format("\"{0}\"", strSerialID));
                            listTemp.Add(jsonRefID.ToString());
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid.\t{0}", intMethod);
                            return optReturn;
                    }
                    optReturn = GetLogRecordData(listTemp);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    List<string> listRecordInfos = optReturn.Data as List<string>;
                    if (listRecordInfos == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ListRecordInfos is null");
                        return optReturn;
                    }
                    if (listRecordInfos.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("RecordInfo not exist");
                        return optReturn;
                    }
                    strRecordInfo = listRecordInfos[0];
                }
                else if (intMethod == 10)
                {
                    strRecordInfo = strSerialID;
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.\t{0}", intMethod);
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strRecordInfo))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SerialID or RecordInfo param invalid.\t{0}", strSerialID);
                    return optReturn;
                }
                try
                {
                    jsonRecord = new JsonObject(strRecordInfo);
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RecordInfo param invalid.\t{0}", ex.Message);
                    return optReturn;
                }
                optReturn = GetRecordInfoFromJsonObject(jsonRecord);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                recordInfo = optReturn.Data as UMPRecordInfo;
                if (recordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                WriteOperationLog(string.Format("RecordInfo:{0}", recordInfo.StringInfo));
                MediaType = recordInfo.MediaType;
                encryptFlag = recordInfo.EncryptFlag;

                #endregion


                #region AppServerInfo

                optReturn = ReadAppServerInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AppServerInfo appServerInfo = optReturn.Data as AppServerInfo;
                if (appServerInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AppServer Info is null");
                    return optReturn;
                }
                string service03Address = appServerInfo.Address;
                int service03Port = appServerInfo.Port - 3;
                WriteOperationLog(string.Format("Service03:{0}:{1}", service03Address, service03Port));

                #endregion


                #region 连接UMPService03下载文件

                if (mService03Helper == null)
                {
                    mService03Helper = new Service03Helper();
                    mService03Helper.Debug += mService03Helper_Debug;
                }
                mService03Helper.HostAddress = service03Address;
                mService03Helper.HostPort = service03Port;

                RequestMessage request = new RequestMessage();
                request.Command = (int)Service03Command.DownloadRecord;
                request.ListData.Add(strUserID);
                request.ListData.Add(strLoginPassword);
                request.ListData.Add(recordInfo.StringInfo);
                request.ListData.Add(strPrefer);
                optReturn = mService03Helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    optReturn.Message = string.Format("Service03Error\t{0}", optReturn.Message);
                    return optReturn;
                }
                ReturnMessage retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ReturnMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                strFileName = retMessage.Data;
                WriteOperationLog(string.Format("Download end.\t{0}", strFileName));

                #endregion


                #region 解密文件

                if (MediaType == 1
                    && encryptFlag == "2")
                {
                    //已经加密的录音文件可以进行解密处理
                    if (string.IsNullOrEmpty(strDecryptPassword))
                    {
                        //如果解密密码为空
                        if (strDecryptRecord == "1")
                        {
                            //如果指定了要解密文件而输入的解密密码为空，返回错误（参数无效）
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DecryptPassword is empty");
                            return optReturn;
                        }
                    }
                    else
                    {
                        //如果解密密码不为空
                        if (strDecryptRecord != "2")
                        {
                            //如果输入的解密密码不为空，而且没有明确指定不解密文件，则需要做解密操作
                            request = new RequestMessage();
                            request.Command = (int)Service03Command.DecryptRecordFile;
                            request.ListData.Add(strFileName);
                            request.ListData.Add(strDecryptPassword);
                            optReturn = mService03Helper.DoRequest(request);
                            if (!optReturn.Result)
                            {
                                optReturn.Message = string.Format("Service03Error\t{0}", optReturn.Message);
                                return optReturn;
                            }
                            retMessage = optReturn.Data as ReturnMessage;
                            if (retMessage == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("ReturnMessage is null");
                                return optReturn;
                            }
                            if (!retMessage.Result)
                            {
                                optReturn.Result = false;
                                optReturn.Code = retMessage.Code;
                                optReturn.Message = retMessage.Message;
                                return optReturn;
                            }
                            strFileName = retMessage.Data;
                            WriteOperationLog(string.Format("Decrypt end.\t{0}", strFileName));
                        }
                    }
                }

                #endregion


                #region 原始解密

                if (MediaType == 1
                    && encryptFlag == "0")
                {
                    //只有原始加密或未加密的录音文件可以做原始解密操作
                    request = new RequestMessage();
                    request.Command = (int)Service03Command.OriginalDecryptFile;
                    request.ListData.Add(strFileName);
                    optReturn = mService03Helper.DoRequest(request);
                    if (!optReturn.Result)
                    {
                        optReturn.Message = string.Format("Service03Error\t{0}", optReturn.Message);
                        return optReturn;
                    }
                    retMessage = optReturn.Data as ReturnMessage;
                    if (retMessage == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ReturnMessage is null");
                        return optReturn;
                    }
                    if (!retMessage.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = retMessage.Code;
                        optReturn.Message = retMessage.Message;
                        return optReturn;
                    }
                    strFileName = retMessage.Data;
                    WriteOperationLog(string.Format("OriginalDecrypt end.\t{0}", strFileName));
                }

                #endregion


                #region 转换格式

                if (MediaType == 1)
                {
                    if (strConvertWaveFormat == "0"
                        || strConvertWaveFormat == "1")
                    {
                        if (jsonRecord[UMPRecordInfo.PRO_WAVEFORMAT] != null
                            && jsonRecord[UMPRecordInfo.PRO_WAVEFORMAT].Value == "G729a")
                        {
                            //只有G.729A需要做转换格式的操作
                            request = new RequestMessage();
                            request.Command = (int)Service03Command.ConvertWaveFormat;
                            request.ListData.Add(strFileName);
                            request.ListData.Add("1");
                            optReturn = mService03Helper.DoRequest(request);
                            if (!optReturn.Result)
                            {
                                optReturn.Message = string.Format("Service03Error\t{0}", optReturn.Message);
                                return optReturn;
                            }
                            retMessage = optReturn.Data as ReturnMessage;
                            if (retMessage == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("ReturnMessage is null");
                                return optReturn;
                            }
                            if (!retMessage.Result)
                            {
                                optReturn.Result = false;
                                optReturn.Code = retMessage.Code;
                                optReturn.Message = retMessage.Message;
                                return optReturn;
                            }
                            strFileName = retMessage.Data;
                            WriteOperationLog(string.Format("ConvertWaveFormat end.\t{0}", strFileName));
                        }
                    }
                    else if (strConvertWaveFormat == "3")
                    {
                        request = new RequestMessage();
                        request.Command = (int)Service03Command.ConvertWaveFormat;
                        request.ListData.Add(strFileName);
                        request.ListData.Add("85");     //音频格式，mp3
                        request.ListData.Add("3");      //转换工具，使用ffmpeg工具
                        request.ListData.Add("mp3");    //文件扩展名
                        optReturn = mService03Helper.DoRequest(request);
                        if (!optReturn.Result)
                        {
                            optReturn.Message = string.Format("Service03Error\t{0}", optReturn.Message);
                            return optReturn;
                        }
                        retMessage = optReturn.Data as ReturnMessage;
                        if (retMessage == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ReturnMessage is null");
                            return optReturn;
                        }
                        if (!retMessage.Result)
                        {
                            optReturn.Result = false;
                            optReturn.Code = retMessage.Code;
                            optReturn.Message = retMessage.Message;
                            return optReturn;
                        }
                        strFileName = retMessage.Data;
                        WriteOperationLog(string.Format("ConvertWaveFormat end.\t{0}", strFileName));
                    }
                }

                #endregion


                listReturn.Add(strFileName);
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn UpdateLogRecordInfo(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strSerialID = listParams[2];
                string strUpdateInfo = listParams[3];

                WriteOperationLog(
                    string.Format(
                        "UpdateLogRecordInfo:\tUserID:{0};Method:{1};SerialID:{2};UpdateInfo:{3}",
                        strUserID,
                        strMethod,
                        strSerialID,
                        strUpdateInfo));

                string strRecordInfo;
                int intMethod;
                long serialID;
                JsonObject jsonRecord;
                JsonObject jsonUpdate;
                UMPRecordInfo recordInfo;

                #endregion


                #region 检查参数有效性

                if (!int.TryParse(strMethod, out intMethod)
                   || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid.\t{0}", strMethod);
                    return optReturn;
                }
                try
                {
                    jsonUpdate = new JsonObject(strUpdateInfo);
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UpdateInfo param invalid.\t{0}", ex.Message);
                    return optReturn;
                }

                #endregion


                #region 获取RecordInfo，如果Method为0、1、2，通过SerialID查询得到RecordInfo

                if (intMethod < 10)
                {
                    List<string> listTemp = new List<string>();
                    listTemp.Add(strUserID);
                    listTemp.Add("0");
                    listTemp.Add("0");
                    listTemp.Add(DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss"));
                    listTemp.Add(DateTime.Parse("2199/12/31").ToString("yyyy-MM-dd HH:mm:ss"));
                    listTemp.Add("0");
                    listTemp.Add("0");
                    switch (intMethod)
                    {
                        case 0:
                        case 1:
                            if (!long.TryParse(strSerialID, out serialID))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("SerialID param invalid.\t{0}", strSerialID);
                                return optReturn;
                            }
                            JsonObject jsonSerialID = new JsonObject();
                            jsonSerialID[UMPRecordInfo.PRO_SERIALID] =
                                new JsonProperty(string.Format("\"{0}\"", serialID));
                            listTemp.Add(jsonSerialID.ToString());
                            break;
                        case 2:
                            if (!long.TryParse(strSerialID, out serialID))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("SerialID param invalid.\t{0}", strSerialID);
                                return optReturn;
                            }
                            JsonObject jsonRowID = new JsonObject();
                            jsonRowID[UMPRecordInfo.PRO_ROWID] = new JsonProperty(string.Format("{0}", serialID));
                            listTemp.Add(jsonRowID.ToString());
                            break;
                        case 3:
                            JsonObject jsonRefID = new JsonObject();
                            jsonRefID[UMPRecordInfo.PRO_RECORDREFERENCE] =
                                new JsonProperty(string.Format("\"{0}\"", strSerialID));
                            listTemp.Add(jsonRefID.ToString());
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid.\t{0}", intMethod);
                            return optReturn;
                    }
                    optReturn = GetLogRecordData(listTemp);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    List<string> listRecordInfos = optReturn.Data as List<string>;
                    if (listRecordInfos == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ListRecordInfos is null");
                        return optReturn;
                    }
                    if (listRecordInfos.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("RecordInfo not exist");
                        return optReturn;
                    }
                    strRecordInfo = listRecordInfos[0];
                }
                else if (intMethod == 10)
                {
                    strRecordInfo = strSerialID;
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.\t{0}", intMethod);
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strRecordInfo))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SerialID or RecordInfo param invalid.\t{0}", strSerialID);
                    return optReturn;
                }
                try
                {
                    jsonRecord = new JsonObject(strRecordInfo);
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RecordInfo param invalid.\t{0}", ex.Message);
                    return optReturn;
                }
                optReturn = GetRecordInfoFromJsonObject(jsonRecord);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                recordInfo = optReturn.Data as UMPRecordInfo;
                if (recordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                strSerialID = recordInfo.SerialID;
                WriteOperationLog(string.Format("RecordInfo:{0}", recordInfo.StringInfo));

                #endregion


                #region 加载更新字段信息

                List<XmlMappingItem> listUpdateInfos = new List<XmlMappingItem>();
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, UpdateConfigInfo.FILE_NAME);
                if (File.Exists(path))
                {
                    optReturn = XMLHelper.DeserializeFile<UpdateConfigInfo>(path);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    UpdateConfigInfo info = optReturn.Data as UpdateConfigInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("UpdateConfigInfo is null");
                        return optReturn;
                    }
                    for (int i = 0; i < info.Items.Count; i++)
                    {
                        listUpdateInfos.Add(info.Items[i]);
                    }
                }
                WriteOperationLog(string.Format("UpdateConfigInfo count:{0}", listUpdateInfos.Count));

                #endregion


                #region 创建更新的Sql语句,并执行更新语句

                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                WriteOperationLog(string.Format("DatabaseInfo:{0}", dbInfo));
                string strConn = dbInfo.GetConnectionString();
                string strSql;
                string strUpdateString;
                string strRentToken = "00000";
                int validField = 0;
                List<string> listReturn = new List<string>();
                var keys = jsonUpdate.GetPropertyNames();
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strUpdateString = string.Empty;
                        for (int i = 0; i < listUpdateInfos.Count; i++)
                        {
                            var info = listUpdateInfos[i];
                            var key = keys.FirstOrDefault(k => k.ToUpper() == info.Name.ToUpper());
                            if (string.IsNullOrEmpty(key)) { continue;}
                            if (jsonUpdate[key] != null)
                            {
                                if (info.DataType == (int)DBDataType.Short
                                    || info.DataType == (int)DBDataType.Int
                                    || info.DataType == (int)DBDataType.Long
                                    || info.DataType == (int)DBDataType.Number)
                                {
                                    strUpdateString += string.Format(" {0} = {1},", info.Column,
                                        jsonUpdate[key].Number);
                                    validField++;
                                }
                                else
                                {
                                    strUpdateString += string.Format(" {0} = '{1}',", info.Column,
                                        jsonUpdate[key].Value);
                                    validField++;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(strUpdateString))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_EXIST;
                            optReturn.Message = string.Format("No update field");
                            return optReturn;
                        }
                        strUpdateString = strUpdateString.Substring(0, strUpdateString.Length - 1);     //去掉多余的逗号
                        strSql = string.Format("Update T_21_001_{0} set {1} where C002 = {2}", strRentToken,
                            strUpdateString, strSerialID);
                        optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Message = strSql;
                            return optReturn;
                        }
                        listReturn.Add(strSerialID);
                        listReturn.Add(validField.ToString());
                        optReturn.Data = listReturn;
                        optReturn.Message = strSql;
                        break;
                    case 3:
                        strUpdateString = string.Empty;
                        for (int i = 0; i < listUpdateInfos.Count; i++)
                        {
                            var info = listUpdateInfos[i];
                            var key = keys.FirstOrDefault(k => k.ToUpper() == info.Name.ToUpper());
                            if (string.IsNullOrEmpty(key)) { continue; }
                            if (jsonUpdate[key] != null)
                            {
                                if (info.DataType == (int)DBDataType.Short
                                    || info.DataType == (int)DBDataType.Int
                                    || info.DataType == (int)DBDataType.Long
                                    || info.DataType == (int)DBDataType.Number)
                                {
                                    strUpdateString += string.Format(" {0} = {1},", info.Column,
                                        jsonUpdate[key].Number);
                                    validField++;
                                }
                                else
                                {
                                    strUpdateString += string.Format(" {0} = '{1}',", info.Column,
                                        jsonUpdate[key].Value);
                                    validField++;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(strUpdateString))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_EXIST;
                            optReturn.Message = string.Format("No update field");
                            return optReturn;
                        }
                        strUpdateString = strUpdateString.Substring(0, strUpdateString.Length - 1);     //去掉多余的逗号
                        strSql = string.Format("Update T_21_001_{0} set {1} where C002 = {2}", strRentToken,
                            strUpdateString, strSerialID);
                        optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Message = strSql;
                            return optReturn;
                        }
                        listReturn.Add(strSerialID);
                        listReturn.Add(validField.ToString());
                        optReturn.Data = listReturn;
                        optReturn.Message = strSql;
                        break;
                }

                #endregion

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn InsertLogRecord(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 21)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRefID = listParams[1];
                string strStartTime = listParams[2];
                string strStopTime = listParams[3];
                string strMediaType = listParams[4];
                string strEncryptFlag = listParams[5];
                string strExtension = listParams[6];
                string strAgent = listParams[7];
                string strCallerID = listParams[8];
                string strCalledID = listParams[9];
                string strDirection = listParams[10];
                string strDuration = listParams[11];
                string strServerID = listParams[12];
                string strServerIP = listParams[13];
                string strChannelID = listParams[14];
                string strContinent = listParams[15];
                string strCountry = listParams[16];
                string strSiteID = listParams[17];
                string strRentToken = listParams[18];
                string strIsaRefID = listParams[19];
                string strOtherInfo = listParams[20];

                WriteOperationLog(
                    string.Format(
                        "InsertLogRecord:\tUserID:{0};RefID:{1};StartTime:{2};StopTime:{3};MediaType:{4};EncryptFlag:{5};IsaRefID:{6};OtherInfo:{7}",
                        strUserID,
                        strRefID,
                        strStartTime,
                        strStopTime,
                        strMediaType,
                        strEncryptFlag,
                        strIsaRefID,
                        strOtherInfo));

                #endregion


                #region 检查参数有效性

                if (string.IsNullOrEmpty(strSiteID))
                {
                    strSiteID = "0";
                }
                if (string.IsNullOrEmpty(strRentToken))
                {
                    strRentToken = "00000";
                }
                DateTime dtStartTime;
                DateTime dtStopTime;
                if (!DateTime.TryParse(strStartTime, out dtStartTime)
                    || !DateTime.TryParse(strStopTime, out dtStopTime))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("DateTime value invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strDirection))
                {
                    strDirection = "0";
                }

                JsonObject jsonOtherInfo = null;
                try
                {
                    if (!string.IsNullOrEmpty(strOtherInfo))
                    {
                        jsonOtherInfo = new JsonObject(strOtherInfo);
                    }
                }
                catch (Exception ex)
                {
                    WriteOperationLog(string.Format("GetOtherInfo fail.\t{0}", ex.Message));
                }

                #endregion


                #region 加载更新字段信息

                List<XmlMappingItem> listUpdateInfos = new List<XmlMappingItem>();
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, UpdateConfigInfo.FILE_NAME);
                if (File.Exists(path))
                {
                    optReturn = XMLHelper.DeserializeFile<UpdateConfigInfo>(path);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    UpdateConfigInfo info = optReturn.Data as UpdateConfigInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("UpdateConfigInfo is null");
                        return optReturn;
                    }
                    for (int i = 0; i < info.Items.Count; i++)
                    {
                        listUpdateInfos.Add(info.Items[i]);
                    }
                }
                WriteOperationLog(string.Format("UpdateConfigInfo count:{0}", listUpdateInfos.Count));

                #endregion


                #region 创建记录信息字符串

                string strInsertString = string.Empty;

                strInsertString += string.Format("c003{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strRentToken);
                strInsertString += string.Format("c004{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c005{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c006{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStartTime.ToString("yyyyMMddHHmmss"));
                strInsertString += string.Format("c007{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStartTime.AddYears(-1600).Ticks);
                strInsertString += string.Format("c008{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStopTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c009{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStopTime.ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c010{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStopTime.ToString("yyyyMMddHHmmss"));
                strInsertString += string.Format("c011{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, dtStopTime.AddYears(-1600).Ticks);
                strInsertString += string.Format("c012{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strDuration);
                strInsertString += string.Format("c014{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strMediaType);
                strInsertString += string.Format("c015{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c020{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strServerIP);
                strInsertString += string.Format("c021{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strExtension);
                strInsertString += string.Format("c022{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strSiteID);
                strInsertString += string.Format("c023{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strContinent);
                strInsertString += string.Format("c024{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strCountry);
                strInsertString += string.Format("c025{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strEncryptFlag);
                strInsertString += string.Format("c031{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c033{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "N");
                strInsertString += string.Format("c035{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "N");
                strInsertString += string.Format("c037{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strServerID);
                strInsertString += string.Format("c038{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strChannelID);
                strInsertString += string.Format("c039{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strAgent);
                strInsertString += string.Format("c040{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strCallerID);
                strInsertString += string.Format("c041{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strCalledID);
                strInsertString += string.Format("c042{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strExtension);
                strInsertString += string.Format("c045{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strDirection);
                strInsertString += string.Format("c056{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c057{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c059{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c060{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c061{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c063{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c064{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c065{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c066{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c077{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strRefID);
                strInsertString += string.Format("c109{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, strIsaRefID);


                //其他信息
                if (jsonOtherInfo != null)
                {
                    var keys = jsonOtherInfo.GetPropertyNames();
                    for (int i = 0; i < listUpdateInfos.Count; i++)
                    {
                        var info = listUpdateInfos[i];
                        var key = keys.FirstOrDefault(k => k.ToUpper() == info.Name.ToUpper());     //忽略字段名的大小写
                        if (string.IsNullOrEmpty(key)) { continue;}
                        var jsonField = jsonOtherInfo[key];
                        if (jsonField != null)
                        {
                            string strColumn = info.Column;
                            string strValue = jsonField.Value;
                            if (info.Encoding > 0)
                            {
                                if (info.Encoding == 1)
                                {
                                    //十六进制编码的解码一下
                                    try
                                    {
                                        strValue = Converter.Hex2String(strValue);
                                    }
                                    catch { }
                                }
                            }
                            strInsertString += string.Format("{2}{0}{1}{0}{0}", ConstValue.SPLITER_CHAR,
                                strValue, strColumn);
                        }
                    }
                }

                WriteOperationLog(string.Format("InsertInfo:{0}", strInsertString));

                #endregion


                #region 将记录信息插入数据库

                //如果过长，截断成10个字段
                string[] arrInertString = new string[10];
                arrInertString.Initialize();
                string strTemp = strInsertString;
                int num = 0;
                do
                {
                    int length = Math.Min(strTemp.Length, 1024);
                    if (num < 10)
                    {
                        arrInertString[num] = strTemp.Substring(0, length);
                    }
                    num++;
                    strTemp = strTemp.Substring(length);
                } while (!string.IsNullOrEmpty(strTemp)); 

                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                WriteOperationLog(string.Format("DatabaseInfo:{0}", dbInfo));
                string strConn = dbInfo.GetConnectionString();
                int errNum = 0;
                string errMsg = string.Empty;
                List<string> listReturn = new List<string>();
                switch (dbInfo.TypeID)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam02", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam03", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam04", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam05", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam06", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam07", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam08", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam09", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam10", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@aouterrornumber", MssqlDataType.Bigint, 0),
                            MssqlOperation.GetDbParameter("@aouterrorstring", MssqlDataType.Varchar, 200)
                        };
                        mssqlParameters[0].Value = arrInertString[0];
                        mssqlParameters[1].Value = arrInertString[1];
                        mssqlParameters[2].Value = arrInertString[2];
                        mssqlParameters[3].Value = arrInertString[3];
                        mssqlParameters[4].Value = arrInertString[4];
                        mssqlParameters[5].Value = arrInertString[5];
                        mssqlParameters[6].Value = arrInertString[6];
                        mssqlParameters[7].Value = arrInertString[7];
                        mssqlParameters[8].Value = arrInertString[8];
                        mssqlParameters[9].Value = arrInertString[9];
                        mssqlParameters[10].Value = errNum;
                        mssqlParameters[11].Value = errMsg;
                        mssqlParameters[10].Direction = ParameterDirection.Output;
                        mssqlParameters[11].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(strConn, "P_21_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[10].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[10].Value, mssqlParameters[11].Value);
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam02", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam03", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam04", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam05", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam06", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam07", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam08", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam09", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam10", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("errornumber", OracleDataType.Int32, 0),
                            OracleOperation.GetDbParameter("errorstring", OracleDataType.Varchar2, 200)
                        };
                        orclParameters[0].Value = arrInertString[0];
                        orclParameters[1].Value = arrInertString[1];
                        orclParameters[2].Value = arrInertString[2];
                        orclParameters[3].Value = arrInertString[3];
                        orclParameters[4].Value = arrInertString[4];
                        orclParameters[5].Value = arrInertString[5];
                        orclParameters[6].Value = arrInertString[6];
                        orclParameters[7].Value = arrInertString[7];
                        orclParameters[8].Value = arrInertString[8];
                        orclParameters[9].Value = arrInertString[9];
                        orclParameters[10].Value = errNum;
                        orclParameters[11].Value = errMsg;
                        orclParameters[10].Direction = ParameterDirection.Output;
                        orclParameters[11].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(strConn, "P_21_001",
                            orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[10].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[10].Value, orclParameters[11].Value);
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type invalid.\t{0}", dbInfo.TypeID);
                        return optReturn;
                }

                #endregion


                listReturn.Add(strRefID);
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

    }
}