using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common2400;
using System.Data;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using PFShareClassesS;

namespace Wcf24011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service24011 : IService24011
    {
        public WebReturn DoOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            if (webRequest == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("WebRequest is null");
                return webReturn;
            }
            SessionInfo session = webRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("SessionInfo is null");
                return webReturn;
            }
            webReturn.Session = session;
            try
            {
                OperationReturn optReturn;
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S2400RequestCode.GetKeyGenServerList:
                        optReturn = GetKeyGenServerList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                    case (int)S2400RequestCode.GetAllMachines:
                        optReturn = GetAllMachines(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2400RequestCode.TryConnToKeyGenServer:
                        optReturn = TryConnectToGeneratorServer(webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Result = true;
                        webReturn.Code = optReturn.Code;
                        break;
                    case (int)S2400RequestCode.AddKeyGenServer:
                        optReturn = AddKeyGenServer(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.EnableDisableKeyGenServer:
                        optReturn = EnabledOrDisabledGenerator(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webRequest.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.ModifyKeyGenServer:
                        optReturn = ModifyKeyGenServer(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.DeleteKeyGenServer:
                        optReturn = DeleteKeyGenServer(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.GetCurrentKeyGenServer:
                        optReturn = GetCurrentKeyGenServer(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S2400RequestCode.GetResourceObjList:
                        optReturn = GetResourceObjList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2400RequestCode.GetVoiceIP_Name201:
                        optReturn = GetVoiceIP_Name201(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2400RequestCode.SetPolicyBindding:
                        optReturn = SetPolicyBindding(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.SaveData224002:
                        optReturn = SaveData224002(session, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S2400RequestCode.DeleteBindedStragegy:
                        optReturn = DeleteBindedStragegy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.ModyfyData224002:
                        optReturn = ModyfyData224002(session, webRequest.ListData, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S2400RequestCode.SendMsgToService00:
                        optReturn = SendMsgToService00(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        /// <summary>
        /// 获得密钥生成器列表
        /// </summary>
        /// <returns></returns>
        private OperationReturn GetKeyGenServerList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
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
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "SELECT * FROM T_24_004  ORDER BY C001 ASC";
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = "SELECT * FROM T_24_004   ORDER BY C001 ASC";
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 获得所有的机器IP 以供选择
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        private OperationReturn GetAllMachines(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
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
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "SELECT C001,C017 FROM t_11_101_00000 WHERE C001 > 2100000000000000000 and C001<2110000000000000000 AND C002=1";
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = "SELECT C001,C017 FROM T_11_101_00000 WHERE C001 > 2100000000000000000 and C001<2110000000000000000 AND C002=1";
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds == null || ds.Tables.Count <= 0)
                {
                    optReturn.Result = true;
                    optReturn.Code = Defines.RET_SUCCESS;
                    optReturn.StringValue = "NO Data";
                    return optReturn;
                }

                List<string> lstData = new List<string>();
                DataRow row;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    row = ds.Tables[0].Rows[i];
                    StringValuePairs obj = new StringValuePairs();
                    obj.Key = row["C001"].ToString();
                    obj.Value = S2400EncryptOperation.EncryptWithM004(S2400EncryptOperation.DecodeEncryptValue(row["C017"].ToString()));
                    optReturn = XMLHelper.SeriallizeObject(obj);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    lstData.Add(optReturn.Data.ToString());
                }
                optReturn.Data = lstData;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listParams"></param>
        /// listParams[0]：IP
        ///listParams[1]：端口
        /// <returns></returns>
        private OperationReturn TryConnectToGeneratorServer(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            Socket LocalSocket = null;
            int LIntRecievedData = 0;
            string LStrRecievedData = string.Empty;
            int LIntCheckKeyPosition, LIntRNKeyPosition;

            try
            {
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                LocalSocket.Connect(listParams[0], int.Parse(listParams[1]));
                byte[] LByteReceiveData = new byte[1024];
                LIntCheckKeyPosition = -1; LIntRNKeyPosition = -1;
                while (true)
                {
                    LIntRecievedData = LocalSocket.Receive(LByteReceiveData);
                    byte[] LByteActRecieveData = new byte[LIntRecievedData];
                    Buffer.BlockCopy(LByteReceiveData, 0, LByteActRecieveData, 0, LIntRecievedData);
                    LStrRecievedData += Encoding.Default.GetString(LByteActRecieveData);

                    LIntCheckKeyPosition = LStrRecievedData.IndexOf("Session");
                    if (LIntCheckKeyPosition < 0) { LIntCheckKeyPosition = -1; continue; }
                    LIntRNKeyPosition = LStrRecievedData.IndexOf("\r\n", LIntCheckKeyPosition);
                    if (LIntRNKeyPosition > 0)
                    {
                        optReturn.Code = 0;
                        optReturn.Result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = -1;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            finally
            {
                if (LocalSocket != null)
                {
                    if (LocalSocket.Connected) { LocalSocket.Close(); }
                }
            }
            return optReturn;
        }

        /// <summary>
        ///  添加密钥生成服务器
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : UserID
        /// listParams[1] : MachineResID
        /// listParams[2] : HostAddress
        /// listParams[3] : HostPort
        /// listParams[4] : IsMain
        /// <returns></returns>
        private OperationReturn AddKeyGenServer(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //先判断参数是否正确
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //尝试连接密钥生成服务器
                List<string> ConnParams = new List<string>();
                ConnParams.Add(listParams[2]);
                ConnParams.Add(listParams[3]);
                optReturn = TryConnectToGeneratorServer(ConnParams);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //获得所有的密钥生成服务器
                optReturn = GetKeyGenServerList(session, listParams);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //检查需要增加的机器地址是否存在
                string strMachineID = listParams[1];
                optReturn = CheckKeyGenServerExists(optReturn, strMachineID);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                string strHostAddress = S2400EncryptOperation.EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, listParams[2]);
                strHostAddress = strHostAddress.Substring(strHostAddress.LastIndexOf(ConstValue.SPLITER_CHAR) + 1);
                int iPort = 0;
                int.TryParse(listParams[3], out iPort);
                int iIsMain = 0;
                int.TryParse(listParams[4], out iIsMain);
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("INSERT INTO T_24_004 VALUES('{0}','{1}',{2},'','','{3}',1,0,0)", strMachineID, strHostAddress, iPort, iIsMain);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("INSERT INTO T_24_004 VALUES('{0}','{1}',{2},'','','{3}',1,0,0)", strMachineID, strHostAddress, iPort, iIsMain);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn = ResourceOperation.AddKeyGenResource(session, listParams);
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

        /// <summary>
        /// 修改密钥生成服务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : ResourceID
        /// listParams[1] : Host
        /// listParams[2] : Port
        /// <returns></returns>
        private OperationReturn ModifyKeyGenServer(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //先判断参数是否正确
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                string strMachineID = listParams[0];
                string strHostAddress = listParams[1];
                int iPort = 0;
                int.TryParse(listParams[2], out iPort);
                //尝试连接密钥生成服务器
                List<string> ConnParams = new List<string>();
                ConnParams.Add(strHostAddress);
                ConnParams.Add(iPort.ToString());
                optReturn = TryConnectToGeneratorServer(ConnParams);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_24_004 SET C003={0} WHERE C001=N'{1}'", iPort, strMachineID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_24_004 SET C003={0} WHERE C001=N'{1}'", iPort, strMachineID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn = ResourceOperation.ModifyKeyGenResource(session, listParams);
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

        /// <summary>
        /// 检查需要添加的密钥服务器是否存在
        /// </summary>
        /// <returns></returns>
        private OperationReturn CheckKeyGenServerExists(OperationReturn opt, string AstrMachineID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            try
            {
                if (opt.Data == null)
                {
                    return optReturn;
                }
                DataSet ds = opt.Data as DataSet;
                if (ds == null || ds.Tables.Count <= 0)
                {
                    return optReturn;
                }
                List<string> lstKeyGenServers = new List<string>();
                DataRow row = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    row = ds.Tables[0].Rows[i];
                    lstKeyGenServers.Add(row["C001"].ToString());
                }
                if (lstKeyGenServers.Contains(AstrMachineID))
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S2400WcfErrorCode.KeyGenServerExists;
                    return optReturn;
                }
                optReturn.Code = 0;
                optReturn.Result = true;
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


        /// <summary>
        /// 启用或禁用密钥生成服务器
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : Host
        /// listParams[1] : Port
        /// listParams[2] : Operation 3 : Enable  4 : Disable 
        /// listParams[3] : MachineID
        /// 以下是启用某服务器时 需要禁用的另一服务器的参数 (预留)
        /// lstParams[4] : 要被禁用的resourceID
        /// lstParams[5] : 要被禁用的HostAddress
        /// lstParams[6] : 要被禁用的Port
        /// 
        /// <returns></returns>
        private OperationReturn EnabledOrDisabledGenerator(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                int iOptType = 0;
                bool bo = int.TryParse(listParams[2], out iOptType);
                if (!bo)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S2400WcfErrorCode.ParamError;
                    optReturn.Message = "Param error";
                    return optReturn;
                }
                string strHostEncrypted = S2400EncryptOperation.EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, listParams[0]);
                strHostEncrypted = strHostEncrypted.Substring(strHostEncrypted.LastIndexOf(ConstValue.SPLITER_CHAR) + 1);
                optReturn = KeyGenServerOperation.EnableDisable(listParams[0], listParams[1], iOptType, strHostEncrypted);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = string.Empty;
                string strIsMain = string.Empty;
                strIsMain = iOptType == 3 ? "1" : "0";
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_24_004 SET C006 = '{0}'  WHERE C001 = N'{1}'", strIsMain, listParams[3]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        //如果是启用服务 且参数数量为7 证明有需要被禁用的服务
                        //if (listParams[2] == "E" && listParams.Count == 7)
                        //{
                        //    optReturn = KeyGenServerOperation.EnableDisable( listParams[5], listParams[6],"D");
                        //    //不管是否成功 都修改数据库的值 因为服务器可能连不上 但仍需禁用
                        //}
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_24_004 SET C006 = '{0}'  WHERE C001 = N'{1}'", strIsMain, listParams[3]);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        //如果是启用服务 且参数数量大于5 证明有需要被禁用的服务
                        //if (listParams[2] == "E" && listParams.Count == 7)
                        //{
                        //    optReturn = KeyGenServerOperation.EnableDisable(listParams[5], listParams[6], "D");
                        //}
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //修改resource

                List<string> lstEnableDisableParam = new List<string>();
                lstEnableDisableParam.Add(string.Empty);
                lstEnableDisableParam.Add(listParams[3]);
                lstEnableDisableParam.Add(listParams[0]);
                lstEnableDisableParam.Add(listParams[1]);
                lstEnableDisableParam.Add(strIsMain);
                optReturn = ResourceOperation.ModifyKeyGenResource(session, lstEnableDisableParam);
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }

            return optReturn;
        }

        /// <summary>
        /// 删除密钥生成服务器
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : MachineID
        /// listParams[1] : HostAddress
        /// listParams[2] : Port
        /// <returns></returns>
        private OperationReturn DeleteKeyGenServer(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //先判断参数是否正确
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strHostEncrypted = S2400EncryptOperation.EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, listParams[1]);
                strHostEncrypted = strHostEncrypted.Substring(strHostEncrypted.LastIndexOf(ConstValue.SPLITER_CHAR) + 1);
                optReturn = KeyGenServerOperation.EnableDisable(listParams[1], listParams[2], (int)OperationType.Disable, strHostEncrypted);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("DELETE FROM  T_24_004 WHERE C001 = N'{0}'", listParams[0]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("DELETE FROM  T_24_004 WHERE C001 = N'{0}'", listParams[0]);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                optReturn = ResourceOperation.DeleteKeyGenResource(session, listParams);
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

        /// <summary>
        /// 获得当前启用的密钥生成服务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        private OperationReturn GetCurrentKeyGenServer(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select * from t_24_004 where C006='1'";
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select * from t_24_004 where C006='1'";
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds==null||ds.Tables.Count <= 0)
                {
                    return optReturn;
                }
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                DataRow row = ds.Tables[0].Rows[0];
                KeyGenServerEntry keyGen = new KeyGenServerEntry();
                keyGen.HostAddress = S2400EncryptOperation.DecryptFromDB(row["C002"].ToString());
                keyGen.HostPort = row["C003"].ToString();
                keyGen.ResourceID = row["C001"].ToString();
                optReturn = XMLHelper.SeriallizeObject(keyGen);
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

        private OperationReturn GetResourceObjList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     资源编号

                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strResourceID = listParams[1];
                string rentID;
                string Rentid = session.RentInfo.ID.ToString();
                if (Rentid.Length == 1)
                {
                    rentID = string.Format("100000000000000000{0}", session.RentInfo.ID.ToString());
                }
                else
                {
                    rentID = Rentid;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT T201.*,CASE WHEN EXISTS (SELECT * FROM T_24_002 T2 WHERE T2.C009=T201.C004 AND T2.C008='1') THEN '1' ELSE '0' END ISEN FROM T_11_201_{0} T201 WHERE C003={1} AND C004 LIKE '221%'", rentToken, rentID, strResourceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT T201.*,CASE WHEN EXISTS (SELECT * FROM T_24_002 T2 WHERE T2.C009=T201.C004 AND T2.C008='1') THEN '1' ELSE '0' END ISEN FROM T_11_201_{0} T201 WHERE C003={1} AND C004 LIKE '221%'", rentToken, rentID, strResourceID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                ResourceInfo item = new ResourceInfo();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID;
                    strID = dr["C004"].ToString() + "|" + dr["ISEN"].ToString();
                    listReturn.Add(strID);
                }
                optReturn.Message = strSql;
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

        private OperationReturn GetVoiceIP_Name201(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * from T_11_101_{0} where C001 like '221%' and C002=1 AND C001 NOT IN(SELECT C001 FROM T_11_101_{0} WHERE C001 LIKE '221%' AND C002=2 AND C012 IN ('2','3'))", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '221%' AND C002=1 AND C001 NOT IN(SELECT C001 FROM T_11_101_{0} WHERE C001 LIKE '221%' AND C002=2 AND C012 IN ('2','3'))", rentToken);
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
                    ResourceInfo item = new ResourceInfo();
                    string[] arrInfo = dr["C017"].ToString().Substring(3).Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length >= 3)
                    {
                        string EnVoiceIP = arrInfo[2];//从数据库里面取出来加密的那个
                        //string strVoiceName = DecryptString(dr["C018"].ToString());
                        string strVoiceIP = DecryptString(EnVoiceIP);
                        string strInfo = string.Format("{0}{1}", ConstValue.SPLITER_CHAR, strVoiceIP);
                        arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length >= 1)
                        {
                            item.ResourceName = arrInfo[0];
                        }
                    }

                    string[] NameInfo = dr["C018"].ToString().Substring(3).Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (NameInfo.Length >= 3)
                    {
                        string EnVoiceIP = NameInfo[2];//从数据库里面取出来加密的那个
                        //string strVoiceName = DecryptString(dr["C018"].ToString());
                        string strVoiceIP = DecryptString(EnVoiceIP);
                        string strInfo = string.Format("{0}{1}", ConstValue.SPLITER_CHAR, strVoiceIP);
                        arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length >= 1)
                        {
                            item.ResourceFullName = arrInfo[0];
                        }
                    }
                    item.ResourceID = Convert.ToInt64(dr["C001"].ToString());
                    item.ResourceCode = 111100001;

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

        private OperationReturn SetPolicyBindding(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //先判断参数是否正确
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                string strIPSource = listParams[0];//ip地址对于的资源编码 221开头
                string strEnable = listParams[1];//1 设置绑定， 0取消绑定
                string strIP = listParams[2];//IP地址
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;

                string enVip = EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, strIP);
                enVip = enVip.Substring(enVip.LastIndexOf(ConstValue.SPLITER_CHAR) + 1);

                #region 第一次启用加密绑定 插入特定数据标记
                try
                {
                    if (strEnable == "1")
                    {
                        switch (session.DBType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_24_002 WHERE C009='{0}' AND C003='4020000000000000000'", strIPSource);
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_24_002 WHERE C009='{0}' AND C003='4020000000000000000'", strIPSource);
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                        }
                        DataSet ds = optReturn.Data as DataSet;
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            switch (session.DBType)
                            {
                                case 2:
                                    strSql = string.Format("insert into T_24_002(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011) values ('1','{0}','4020000000000000000','20150101000000','20150101000000','{1}','{2}','{3}','{4}','{5}','0')",
                                        enVip, session.UserInfo.UserID, DateTime.UtcNow.ToString("yyyyMMddHHmmss"), strEnable, strIPSource, session.UserInfo.Account);
                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    break;
                                case 3:
                                    strSql = string.Format("insert into T_24_002(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011) values ('1','{0}','4020000000000000000','20150101000000','20150101000000','{1}','{2}','{3}','{4}','{5}','0')",
                                        enVip, session.UserInfo.UserID, DateTime.UtcNow.ToString("yyyyMMddHHmmss"), strEnable, strIPSource, session.UserInfo.Account);
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    break;
                            }
                        }
                    }
                }
                catch { }

                #endregion

                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_24_002 SET C008={0} WHERE C009='{1}';UPDATE T_11_101_{2} SET C019 ='{0}' WHERE C001='{1}' AND C002='4'",
                            strEnable, strIPSource, rentToken);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("BEGIN UPDATE T_24_002 SET C008={0} WHERE C009='{1}';UPDATE T_11_101_{2} SET C019 ='{0}' WHERE C001='{1}' AND C002='4';END;",
                            strEnable, strIPSource, rentToken);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        private OperationReturn SaveData224002(SessionInfo session, string strData)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = string.Empty;
            try
            {
                optReturn = XMLHelper.DeserializeObject<CVoiceServerBindStrategy>(strData);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CVoiceServerBindStrategy csbs = optReturn.Data as CVoiceServerBindStrategy;
                if (csbs == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("CVoiceServerBindStrategy Is Null");
                    return optReturn;
                }

                string enVip = EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, csbs.Objectvalue);
                enVip = enVip.Substring(enVip.LastIndexOf(ConstValue.SPLITER_CHAR) + 1);

                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("insert into T_24_002(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                            "1", enVip, csbs.Bindingpolicyid, csbs.Durationbegin, csbs.Durationend, csbs.Setteduserid, csbs.Settedtime, csbs.Grantencryption, csbs.CusFiled1, csbs.CusFiled2, csbs.CusFiled3);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("insert into T_24_002(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                            "1", enVip, csbs.Bindingpolicyid, csbs.Durationbegin, csbs.Durationend, csbs.Setteduserid, csbs.Settedtime, csbs.Grantencryption, csbs.CusFiled1, csbs.CusFiled2, csbs.CusFiled3);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                return optReturn;
            }
            optReturn.Message = strSql;
            return optReturn;
        }

        private OperationReturn DeleteBindedStragegy(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //先判断参数是否正确
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strpoStartTimeNum = listParams[0];
                string strpoID = listParams[1];
                string strIPSourceID = listParams[2];
                string strSql = string.Empty;

                #region 判断是否可删除 C012有值，已经使用，不可删除
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_24_002 WHERE C003='{0}' AND C012 IS NOT NULL ", strpoID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_24_002 WHERE C003='{0}' AND C012 IS NOT NULL ", strpoID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (optReturn.Data != null && (optReturn.Data as DataSet).Tables[0] != null && (optReturn.Data as DataSet).Tables[0].Rows.Count > 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = S2400Const.Msg_StragegyExit;
                    return optReturn;
                }
                #endregion


                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("DELETE FROM  T_24_002 WHERE C004='{0}' AND C003='{1}' AND C009='{2}'", strpoStartTimeNum, strpoID, strIPSourceID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("DELETE FROM  T_24_002 WHERE C004='{0}' AND C003='{1}' AND C009='{2}'", strpoStartTimeNum, strpoID, strIPSourceID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        private OperationReturn ModyfyData224002(SessionInfo session, List<string> listParams, string strData)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = string.Empty;
            try
            {
                if (listParams == null || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strStart = listParams[0];
                string strEnd = listParams[1];
                string strSetuser = listParams[2];
                string strSettime = listParams[3];
                string strSetaccount = listParams[4];
                optReturn = XMLHelper.DeserializeObject<CVoiceServerBindStrategy>(strData);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CVoiceServerBindStrategy csbs = optReturn.Data as CVoiceServerBindStrategy;
                if (csbs == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("CVoiceServerBindStrategy Is Null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_24_002 SET C004='{0}',C005='{1}',C006='{2}',C007='{3}',C010='{4}' WHERE C003='{5}' AND C004='{6}' AND C009='{7}'",
                            strStart, strEnd, strSetuser, strSettime, strSetaccount,
                            csbs.Bindingpolicyid, csbs.Durationbegin, csbs.CusFiled1);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_24_002 SET C004='{0}',C005='{1}',C006='{2}',C007='{3}',C010='{4}' WHERE C003='{5}' AND C004='{6}' AND C009='{7}'",
                            strStart, strEnd, strSetuser, strSettime, strSetaccount,
                            csbs.Bindingpolicyid, csbs.Durationbegin, csbs.CusFiled1);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                return optReturn;
            }
            optReturn.Message = strSql;
            return optReturn;
        }

        private static OperationReturn SendMsgToService00(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            if (listParams == null || listParams.Count < 1)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_PARAM_INVALID;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }
            string LStrSendMessage = string.Empty;
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            string LStrReadMessage = string.Empty;
            string strKeyGenHost = listParams[0];
            try
            {
                LStrSendMessage = Common2400.S2400EncryptOperation.EncryptWithM004("R001");
                LStrSendMessage += ConstValue.SPLITER_CHAR;
                LStrSendMessage += S2400EncryptOperation.EncryptWithM004(strKeyGenHost + ":8009");
                //LTcpClient = new TcpClient("192.168.6.86", 8009);
                LTcpClient = new TcpClient("127.0.0.1", 8009);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    if (LStrReadMessage.Length > 2 && LStrReadMessage.Substring(0, 2) == "OK")
                    {
                        optReturn.Result = true;
                        optReturn.Code = Defines.RET_SUCCESS;
                        optReturn.Message = LStrReadMessage;
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = LStrReadMessage;
                    }
                }
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

        #region OTHER
        private static string EncryptStringForVoiceIP(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
             EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strTemp;
        }

        private static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strTemp;
        }

        public static string EncodeEncryptValue(int iEncryptMode, string strValue)
        {
            string strReturn = string.Empty;
            //加密的
            if (iEncryptMode > 0)
            {
                string strStart = string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR);
                switch (iEncryptMode)
                {
                    case (int)ObjectPropertyEncryptMode.E2Hex:
                        strReturn = string.Format("{0}2{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                        EncryptStringForVoiceIP(strValue));
                        break;
                    case (int)ObjectPropertyEncryptMode.SHA256:
                        strReturn = string.Format("{0}3{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                        EncryptStringForVoiceIP(strValue));
                        break;
                }
            }
            return strReturn;
        }

        private static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private static bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf(ConstValue.SPLITER_CHAR + "End" + ConstValue.SPLITER_CHAR) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(ConstValue.SPLITER_CHAR + "End" + ConstValue.SPLITER_CHAR);
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.ToString();
            }

            return LBoolReturn;
        }

        private static string DecryptFromClient(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        #endregion
    }
}
