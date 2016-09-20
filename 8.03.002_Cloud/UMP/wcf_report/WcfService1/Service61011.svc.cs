using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.DBAccesses;

namespace Wcf61011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service61011 : IService61011
    {
        public WebReturn DoOperation(WebRequest RResult)
        {
            WebReturn result = new WebReturn();
            result.Result = true;
            string SQL = RResult.Data;
            SessionInfo session = RResult.Session;
            var dbInfo = session.DatabaseInfo;
            if (dbInfo == null)
            {
                result.Result = false;
                result.Code = -1;
                return result;
            }
            string RealPassW = session.DatabaseInfo.Password;
            dbInfo.RealPassword = DecryptString104(RealPassW);
            string Conn = dbInfo.GetConnectionString();
            int num = session.DBType;
            if (RResult == null)
            {
                result.Message = "There is no available parameters";
                result.Result = false;
                return result;
            }
            OperationReturn OpeReturn = new OperationReturn();
            switch (RResult.Code)
            {
                case 100:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                        result.DataSetData = OpeReturn.Data as DataSet;
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    break;
                case 200:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C002");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    break;
                case 102:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C017");
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C018");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    break;
                case 103:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C002");
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C003");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    break;
                case 0:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C006");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        #region 密码
        private DataSet CryptProcess(DataSet ds, string col)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string str = EncryptString(DecryptString(dr[col].ToString()));
                dr[col] = str;
            }
            return ds;
        }
        //加密004
        private static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }
        private static string DecryptString104(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }
        //解密102
        private static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strTemp;
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
        #endregion
        private OperationReturn GetDataSetFromDB(string sql, string strConn, int num)
        {
            OperationReturn OR = new OperationReturn();
            if (num == 3)
                OR = Wcf61011.OracleOperation.GetDataSet(strConn, sql);
            if (num == 2)
                OR = Wcf61011.MssqlOperation.GetDataSet(strConn, sql);
            return OR;
        }
    }
}
