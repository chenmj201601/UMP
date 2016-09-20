using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPService00
{
    /// <summary>
    /// 操作资源表
    /// </summary>
    public class ResourceOperation
    {
        /// <summary>
        /// 主机接替备机时 更新资源表
        /// </summary>
        /// <param name="dbInfo">数据库连接信息</param>
        /// <param name="strRelpaceModuleNumber">要接替的主机key</param>
        /// <param name="strResourceKey">备机key</param>
        /// <returns></returns>
        public static OperationReturn UpdateReplaceModuleNumberInDB(DatabaseInfo dbInfo,string strRelpaceModuleNumber,string strResourceKey)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strConnectString = dbInfo.GetConnectionString();
                //先找到备机对应的资源ID
                string strSql = string.Empty;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "select * from t_11_101_{0} where C001 >2210000000000000000 and C001 <2220000000000000000 and C002 =1 and C014 in( {1},{2})";
                        strSql = string.Format(strSql, "00000", strResourceKey, strRelpaceModuleNumber);
                        optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "select * from t_11_101_{0} where C001 >2210000000000000000 and C001 <2220000000000000000 and C002 =1 and C014 in( {1},{2})";
                        strSql = string.Format(strSql, "00000", strResourceKey, strRelpaceModuleNumber);
                        optReturn = OracleOperation.GetDataSet(strConnectString, strSql);
                        break;
                }
                
                if (!optReturn.Result)
                {
                   
                    UMPService00.WriteLog(LogMode.Error, "Get resource by modulenumber failed: " + optReturn.Message);
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
          
                    UMPService00.WriteLog(LogMode.Warn, "Modulenumber = " + strResourceKey + " not exists");
                    optReturn.Result = false;
                    return optReturn;
                }
                //定义两个变量 来保存主备机器的资源ID
                string strMainResID = string.Empty;
                string strBackupResID = string.Empty;
                string strKey = string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    strKey = row["C012"].ToString();
                    if (strKey == strResourceKey)
                    {
                        strBackupResID = row["C001"].ToString();
                        continue;
                    }
                    else if (strKey == strRelpaceModuleNumber)
                    {
                        strMainResID = row["C001"].ToString();
                    }
                }

                //判断备机资源的standbyrole是不是3 
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "select * from t_11_101_{0} where C001 ={1} and C002 = 2";
                        strSql = string.Format(strSql, "00000", strBackupResID);
                        optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "select * from t_11_101_{0} where C001 ={1} and C002 = 2";
                        strSql = string.Format(strSql, "00000", strBackupResID);
                        optReturn = OracleOperation.GetDataSet(strConnectString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    UMPService00.WriteLog(LogMode.Error, "Get standbyrole by resource failed : " + optReturn.Message);
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    UMPService00.WriteLog("standbyrole  not exists");

                    optReturn.Result = false;
                    return optReturn;
                }
                string strStandbyrole = ds.Tables[0].Rows[0]["C012"].ToString();
                if (strStandbyrole != "3")
                {
                    UMPService00.WriteLog(LogMode.Error, "standbyrole = " + strStandbyrole + ", return false");
                    optReturn.Result = false;
                    return optReturn;
                }
                //更新备机的RelpaceModuleNumber
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "update t_11_101_{0} set C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql,"00000", strRelpaceModuleNumber, strBackupResID);
                        optReturn = MssqlOperation.ExecuteSql(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "UPDATE t_11_101_{0} SET C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql,"00000", strRelpaceModuleNumber, strBackupResID);
                        optReturn = OracleOperation.ExecuteSql(strConnectString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    //UMPService00.IEventLog.WriteEntry("Update mainserver RelpaceModuleNumber failed: " + "\n" + strSql + "\n" + optReturn.Message, EventLogEntryType.Error);
                    UMPService00.WriteLog(LogMode.Error, "Update mainserver RelpaceModuleNumber failed: " + "\n" + strSql + "\n" + optReturn.Message);
                    return optReturn;
                }

                //更新主机的RelpaceModuleNumber
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "update t_11_101_{0} set C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strResourceKey, strMainResID);
                        optReturn = MssqlOperation.ExecuteSql(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "UPDATE t_11_101_{0} SET C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strResourceKey, strMainResID);
                        optReturn = OracleOperation.ExecuteSql(strConnectString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    //UMPService00.IEventLog.WriteEntry("Update standby server RelpaceModuleNumber failed: " + "\n" + strSql + "\n" + optReturn.Message, EventLogEntryType.Error);
                    UMPService00.WriteLog(LogMode.Error, "Update standby server RelpaceModuleNumber failed: " + "\n" + strSql + "\n" + optReturn.Message);
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
  
                UMPService00.WriteLog(LogMode.Error, "UpdateResource error : " + ex.Message);
            }

            return optReturn;
        }

        /// <summary>
        /// 获得所有物理机器的key和replaceModuleNumber
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public static OperationReturn GetAllMachines(DatabaseInfo dbInfo)
        {
            Dictionary<int, MachineInfo> lstMachines = new Dictionary<int, MachineInfo>();
            string strConnectString = dbInfo.GetConnectionString();
            OperationReturn optReturn = new OperationReturn();
            string strSql = string.Empty;
            switch (dbInfo.TypeID)
            {
                case 2:
                    strSql = "select a.C001,a.C012,b.c011,a.c002 as A002 ,b.c002 as B002,a.C017,c.C012 as D012  from t_11_101_{0} a " +
                                "left join t_11_101_{0} b on a.c001 = b.c001 "+
                                "left join t_11_101_{0} c on a.c001 = c.c001 " +
                                 "where a.C001 >2210000000000000000 and a.C001 <2220000000000000000 and a.C002 =1 "+
                                 "and  b.C001 >2210000000000000000 and b.C001 <2220000000000000000 and b.C002 =3 " +
                                 " and c.C001 >2210000000000000000 and c.C001 <2220000000000000000 and c.C002 =2 ";
                    strSql = string.Format(strSql, "00000");
                    optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                    break;
                case 3:
                    strSql = "select a.C001,a.C012,b.c011,a.c002 as A002 ,b.c002 as B002,a.C017,c.C012 as D012  from t_11_101_{0} a " +
                                 "left join t_11_101_{0} b on a.c001 = b.c001 " +
                                 "left join t_11_101_{0} c on a.c001 = c.c001 " +
                                  "where a.C001 >2210000000000000000 and a.C001 <2220000000000000000 and a.C002 =1 " +
                                  "and  b.C001 >2210000000000000000 and b.C001 <2220000000000000000 and b.C002 =3 " +
                                  " and c.C001 >2210000000000000000 and c.C001 <2220000000000000000 and c.C002 =2 ";
                    strSql = string.Format(strSql, "00000", "00000");
                    optReturn = OracleOperation.GetDataSet(strConnectString, strSql);
                    break;
            }
            if (!optReturn.Result)
            {
                optReturn.Message ="GetAllMachines error ,sql = " + strSql;
                return optReturn;
            }
            DataSet ds = optReturn.Data as DataSet;
            if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                optReturn.Data = lstMachines;
                return optReturn;
            }
            MachineInfo machine = null;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                try
                {
                    machine = new MachineInfo();
                    machine.Key = int.Parse(row["C012"].ToString());
                    machine.ReplaceModuleNumber = row["C011"].ToString();
                    machine.ResID = long.Parse(row["C001"].ToString());
                    machine.Host = Common.DecodeEncryptValue(row["C017"].ToString());
                    machine.StandByRole = row["D012"].ToString();
                    lstMachines.Add(machine.Key, machine);
                }
                catch (Exception ex)
                {
                    //UMPService00.IEventLog.WriteEntry("convert error : " + row["C012"].ToString() + " ; " + row["C001"].ToString());
                    UMPService00.WriteLog("convert error : " + row["C012"].ToString() + " ; " + row["C001"].ToString());
                }
                //lstMachines.Add(int.Parse(row["C012"].ToString()), row["C011"].ToString());
            }
            optReturn.Data = lstMachines;
            return optReturn ;
        }

        /// <summary>
        /// 根据机器的key来更新RelpaceModuleNumber
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="iKey">机器的key</param>
        /// <param name="strRelpaceModuleNumber">机器的RelpaceModuleNumber</param>
        /// <returns></returns>
        public static OperationReturn UpdateReplaceModuleNumberInDB(DatabaseInfo dbInfo, long lResID, string strRelpaceModuleNumber)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strConnectString = dbInfo.GetConnectionString();
                //先找到备机对应的资源ID
                string strSql = string.Empty;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "update t_11_101_{0} set C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strRelpaceModuleNumber, lResID);
                        optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                        break;
                    case 3:
                         strSql = "update t_11_101_{0} set C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strRelpaceModuleNumber, lResID);
                        optReturn = OracleOperation.GetDataSet(strConnectString, strSql);
                        break;
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }

    public class MachineInfo
    {
        private int _Key;

        public int Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        private string _Host;

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        private long _ResID;

        public long ResID
        {
            get { return _ResID; }
            set { _ResID = value; }
        }
        private string _ReplaceModuleNumber;

        public string ReplaceModuleNumber
        {
            get { return _ReplaceModuleNumber; }
            set { _ReplaceModuleNumber = value; }
        }

        private string _StandByRole;

        public string StandByRole
        {
            get { return _StandByRole; }
            set { _StandByRole = value; }
        }
    }
}
