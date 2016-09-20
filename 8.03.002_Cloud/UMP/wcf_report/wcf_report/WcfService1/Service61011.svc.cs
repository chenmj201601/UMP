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

namespace Wcf61012
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service61011 : IService61011
    {
        public WebReturn DoOperation(WebRequest RResult)
        {
           // bool flag = false;
            WebReturn result = new WebReturn();
            result.Result = true;
            string SQL = RResult.Data;
            SessionInfo session = RResult.Session;
            string Conn = session.DBConnectionString;
            int num = session.DBType;
            if (RResult == null)
            {
                result.Message = "There is no available parameters";
                result.Result = false;
                return result;
            }
            OperationReturn OpeReturn = new OperationReturn();
            //string where = RResult.StringValue;
            //int number = RResult.IntValue;
            switch (RResult.Code)
            {
                case 701:
                    OpeReturn = GetDataSetFromDB(SQL, Conn, num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C003");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    //flag = false;
                    break;
                case 100:
                    OpeReturn = GetDataSetFromDB(SQL, Conn,num);
                    if (OpeReturn.Result)
                        result.DataSetData = OpeReturn.Data as DataSet;
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    //flag = false;
                    break;
                case 200:
                    OpeReturn = GetDataSetFromDB(SQL, Conn,num);
                    if (OpeReturn.Result)
                    {
                        result.DataSetData = CryptProcess(OpeReturn.Data as DataSet, "C002");
                    }
                    else
                    {
                        result.Message = OpeReturn.Message;
                        result.Result = false;
                    }
                    //flag = false;
                    break;
                case 102:
                    OpeReturn = GetDataSetFromDB(SQL, Conn,num);
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
                    //flag = false;
                    break;
                case 103:
                    OpeReturn = GetDataSetFromDB(SQL, Conn,num);
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
                    //flag = false;
                    break;
                //case 201:
                //    OperationReturn ORTemp = GetDataSetFromDB(string.Format("SELECT C017 FROM T_11_101_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1}",session.RentInfo.Token,session.UserID), Conn, num);
                //    DataSet ds_temp = new DataSet();
                //    if (ORTemp.Result)
                //        ds_temp = CryptProcess(ORTemp.Data as DataSet, "C017");                 
                //    break;
                default:
                    break;
            }
            #region 加载相应的报表
            //if (flag)
            //{
            //    result.DataSetData = GetReportData(RResult, OpeReturn);
            //}
            //else
            //{
            //    result.StringValue = "敬请期待";
            //    result.BoolValue = false;
            //}         
            return result;
            #endregion
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

        private OperationReturn GetDataSetFromDB(string sql, string strConn,int num)
        {
            OperationReturn OR = new OperationReturn();
            if(num==3)
            OR = Wcf61012.OracleOperation.GetDataSet(strConn, sql);
            if(num==2)
                OR = Wcf61012.MssqlOperation.GetDataSet(strConn, sql);            
            return OR;
        }

        //private DataSet GetReportData(ReportResult result, OperationReturn OpeReturn)
        //{
        //    string sql = OpeReturn.StringValue;
        //    string sql1 = OpeReturn.StringValue2;
        //    result.BoolValue = true;
        //    string ip = result.ListStrValue[0];
        //    string database = result.ListStrValue[1];
        //    string uid = result.ListStrValue[2];
        //    string pwd = result.ListStrValue[3];
        //    string strConn = "";
        //    int number = result.IntValue;
        //    if (number == 1 || number == 2 || number == 4 || number == 5)
        //    {
        //        //string strConn = string.Format("server={0};database={1};uid={2};pwd={3};", "192.168.4.182", "PFOrcl","PFDEV", "PF,123");
        //        strConn = string.Format("server={0};database={1};uid={2};pwd={3};", ip, database, uid, pwd);
        //    }

        //       //自己的数据库，以后删掉
        //    else
        //    {
        //        //strConn = "server=127.0.0.1;database=px;uid=sa;pwd=123456;";
        //        strConn = string.Format("server={0};database={1};uid={2};pwd={3};", "192.168.4.182", "UMPDataDB", "sa", "PF,123");
        //    }

        //    SqlConnection connection = null;
        //    DataSet dataSet = null;
        //    SqlDataAdapter dataAdapter = null;

        //    try
        //    {
        //        connection = new SqlConnection(strConn);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.StringValue = ex.Message.ToString();
        //        result.BoolValue = false;
        //    }

        //    if (number == 2 || number == 14)
        //    {
        //        DataSet dataSet1 = new DataSet();
        //        dataAdapter = new SqlDataAdapter(sql1, connection);
        //        dataAdapter.Fill(dataSet1);
        //        result.DataSet_Value = dataSet1;
        //    }

        //    try
        //    {
        //        dataAdapter = new SqlDataAdapter(sql, connection);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("连接数据库出错：" + ex.ToString());
        //        result.StringValue = ex.Message.ToString();
        //        result.BoolValue = false;
        //    }
        //    dataSet = new DataSet();

        //    try
        //    {
        //        dataAdapter.Fill(dataSet);
        //    }

        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("数据加载出错：" + ex.ToString());
        //        result.StringValue = ex.Message.ToString();
        //        result.BoolValue = false;
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return dataSet;
        //}
    }
}
