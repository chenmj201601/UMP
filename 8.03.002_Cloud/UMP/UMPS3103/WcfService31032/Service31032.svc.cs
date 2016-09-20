using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.ServiceModel.Activation;
using PFShareClassesS;

namespace WcfService31032
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
        [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service31032 : IService31032
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        ////数据库字符串拼接
        //public string GetConnectionString(string dbType, string dbURL, string dbUserName, string dbPWD)
        //{
        //    string str = string.Empty;
        //    switch (dbType)
        //    {
        //        case "2":
        //            return string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", new object[] { this.Host, this.Port, this.DBName, this.LoginName, this.Password });

        //        case "3":
        //            return string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}", new object[] { this.Host, this.Port, this.DBName, this.LoginName, this.Password });
        //    }
        //    return str;
        //}


        //得到所属机构
        public Service02Return GetUserControlOrg(string dbType, string dbURL,  string UserID,string ParentID) 
        {
            Service02Return service02Retrun = new Service02Return();
            service02Retrun.ReturnValueBool = true;
            service02Retrun.ErrorFlag = "T";
            try
            {
                string strUserID = UserID;
                string strParentID = ParentID;
                string rentToken = "00000";
                string strSql;
                DataSet objDataSet;
                switch (dbType)
                {
                    case "2":
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
                        }
                        objDataSet = MssqlOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;
                        break;
                    case "3":
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
                        }
                        objDataSet = OracleOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;
                        break;
                    default:
                        service02Retrun.ReturnValueBool = false;
                        service02Retrun.ErrorFlag = "F";
                        service02Retrun.ErrorMessage = string.Format("Database type not surpport.\t{0}", dbType);
                        return service02Retrun;
                }
                if (objDataSet ==null)
                {
                    service02Retrun.ReturnValueBool = false;
                    service02Retrun.ErrorMessage = "objDataSet is null";
                    service02Retrun.ErrorFlag = "F";
                    return service02Retrun;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID = dr["C001"].ToString();
                    string strName = dr["C002"].ToString();
                    strName = DecryptFromDB(strName);
                    string StrParentId = dr["C004"].ToString();
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, AscCodeToChr(36), strName, StrParentId);
                    listReturn.Add(strInfo);
                }
                service02Retrun.ReturnValueListString = listReturn;
            }
            catch (Exception ex)
            {
                service02Retrun.ReturnValueBool = false;
                service02Retrun.ErrorMessage = ex.Message.ToString();
                service02Retrun.ErrorFlag = "F";
                return service02Retrun;
            }


            return service02Retrun;
        }

        public Service02Return GetUserControlAgentOrExtension(string dbType, string dbURL,  string UserID,string OrgID,string ObjectType)
        {
            Service02Return service02Retrun = new Service02Return();
            service02Retrun.ReturnValueBool = true;
            service02Retrun.ErrorFlag = "T";
            try
            {
                ////ListParam
                ////0      用户编号
                ////1      所属机构编号
                ////2     A,代表座席,E,虚拟分机,R,代表真实分机
                
                string strUserID = UserID;
                string strParentID = OrgID;
                string path1 = string.Empty;
                string path2 = string.Empty;
                if (ObjectType == "A")
                {
                    path1 = string.Format("1030000000000000000");
                    path2 = string.Format("1040000000000000000");
                }
                if (ObjectType == "E")
                {
                    path1 = string.Format("1040000000000000000");
                    path2 = string.Format("1050000000000000000");
                }
                if (ObjectType == "R")
                {
                    path1 = string.Format("1050000000000000000");
                    path2 = string.Format("1060000000000000000");
                }
                string rentToken ="00000";
                string strSql;
                DataSet objDataSet;
                switch (dbType)
                {
                    case "2":
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and C001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID, path1, path2);
                        objDataSet = MssqlOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;                        
                        break;
                    case "3":
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID, path1, path2);
                        objDataSet = OracleOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;
                        
                        break;
                    default:
                        service02Retrun.ReturnValueBool = false;
                        service02Retrun.ErrorFlag = "F";
                        service02Retrun.ErrorMessage = string.Format("Database type not surpport.\t{0}", dbType);
                        return service02Retrun;
                }
                if (objDataSet == null)
                {
                    service02Retrun.ReturnValueBool = false;
                    service02Retrun.ErrorMessage = "objDataSet is null";
                    service02Retrun.ErrorFlag = "F";
                    return service02Retrun;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName= string.Empty;
                    if (ObjectType.Equals("E") || ObjectType.Equals("R"))
                    {
                        strName = DecryptFromDB(strName);
                        string[] values = strName.Split(AscCodeToChr(27).ToArray());
                        if (values.Length > 1) 
                        {
                            strName = values[0];
                            strFullName = values[1];
                        }
                    }
                    else 
                    {
                        strName = DecryptFromDB(strName);
                        strFullName = dr["C018"].ToString();
                        strFullName = DecryptFromDB(strFullName);
                    }
                    
                   
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, AscCodeToChr(36), strName, strFullName);
                    listReturn.Add(strInfo);
                }
                service02Retrun.ReturnValueListString = listReturn;
            }
            catch (Exception ex)
            {
                service02Retrun.ReturnValueBool = false;
                service02Retrun.ErrorMessage = ex.Message.ToString();
                service02Retrun.ErrorFlag = "F";
                return service02Retrun;
            }

            return service02Retrun;
        }


        //得到个人的播放权限下载
        public Service02Return GetUserOperation(string dbType, string dbURL, string UserID) 
        {
            Service02Return service02Retrun = new Service02Return();
            service02Retrun.ReturnValueBool = true;
            service02Retrun.ErrorFlag = "T";


            string strUserID = UserID;
            string rentToken = "00000";
            string strSql;
            DataSet objDataSet;
            //3102001查询 3102002播放 3102008下载
            try
            {
                switch (dbType)
                {
                    case "2":
                        {
                            strSql = string.Format("SELECT DISTINCT C002 FROM T_11_202_00000  WHERE  C001  IN (SELECT C003  FROM T_11_201_00000 WHERE C004={0} AND  C003>=1060000000000000000 AND C003<1070000000000000000) AND C002 IN (3102001,3102002,3102008) ORDER BY C002", strUserID);
                            objDataSet = MssqlOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;
                        }
                        break;
                    case "3":
                        {
                            strSql = string.Format("SELECT DISTINCT C002 FROM T_11_202_00000 WHERE C001  IN (SELECT C003  FROM T_11_201_00000 WHERE C004={0} AND  C003>=1060000000000000000 AND C003<1070000000000000000) AND C002 IN (3102001,3102002,3102008) ORDER BY C002", strUserID);
                            objDataSet = OracleOperation.GetDataSet(dbURL, strSql).ReturnValueDataSet;
                        }
                        break;
                    default:
                    service02Retrun.ReturnValueBool = false;
                    service02Retrun.ErrorFlag = "F";
                    service02Retrun.ErrorMessage = string.Format("Database type not surpport.\t{0}", dbType);
                    return service02Retrun;
                }
                if (objDataSet == null)
                {
                    service02Retrun.ReturnValueBool = false;
                    service02Retrun.ErrorMessage = "objDataSet is null";
                    service02Retrun.ErrorFlag = "F";
                    return service02Retrun;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID = dr["C002"].ToString();
                    listReturn.Add(strID);
                }
                string StrOperation = string.Empty;
                if(listReturn.Count>0)
                {
                    foreach(string s in listReturn)
                    {
                        StrOperation += s + ",";
                    }
                }
                if (!string.IsNullOrWhiteSpace(StrOperation))
                {
                    service02Retrun.ReturnValueString = StrOperation.TrimEnd(',');
                }
                else 
                {
                    service02Retrun.ReturnValueString = string.Empty;
                }
            }
            catch (Exception ex)
            {
                service02Retrun.ReturnValueBool = false;
                service02Retrun.ErrorMessage = ex.Message.ToString();
                service02Retrun.ErrorFlag = "F";
                return service02Retrun;
            }

            return service02Retrun;

        }
       

     


        #region
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                int intRand = random.Next(0, 14);
                strTemp = intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "VCT");
                intRand = random.Next(0, 17);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "UMP");
                intRand = random.Next(0, 20);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string EncryptToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
              EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }

        private string EncryptToClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptFromClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strReturn;
        }

        private string EncryptShaToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptStringSHA512(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
             EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion
    }
}
