using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf11012
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11012 : IService11012
    {
        private const string PATH_DOGGLECONFIG = "GlobalSettings\\UMP.Young.01";

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

        private string DecryptM001(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
            EncryptionAndDecryption.UMPKeyAndIVType.M101);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }

        private OperationReturn GetDoggleNumber()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, PATH_DOGGLECONFIG);
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig file not exist.\t{0}", path);
                    return optReturn;
                }
                string[] listContents = File.ReadAllLines(path);
                if (listContents.Length < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig not exist.");
                    return optReturn;
                }
                string doggleNumber = listContents[0];
                doggleNumber = DecryptM001(doggleNumber);
                optReturn.Data = doggleNumber;
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

        private string GetMD5HasString(string strSource)
        {
            byte[] byteSource = Encoding.Unicode.GetBytes(strSource);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteReturn = md5.ComputeHash(byteSource);
            string strReturn = Converter.Byte2Hex(byteReturn);
            return strReturn;
        }


        private OperationReturn GetUserOptList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     模块代码
                //2     上级模块或操作编号
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string roleID = session.RoleInfo.ID.ToString();
                string moduleID = listParams[1];
                string parentID = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strLog = string.Empty;


                #region 获取当前的狗号

                optReturn = GetDoggleNumber();
                if (!optReturn.Result)
                {
                    strLog += string.Format("Get doggle number fail.");
                    optReturn.Message += strLog;
                    return optReturn;
                }
                string doggleNumber = optReturn.Data.ToString();
                strLog += string.Format("{0};", doggleNumber);

                #endregion


                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C003 = {3} ORDER BY C001, C002",
                                rentToken,
                                moduleID,
                                roleID,
                                parentID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C003 = {3} ORDER BY C001, C002",
                                 rentToken,
                                 moduleID,
                                 roleID,
                                 parentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
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
                strLog += strSql;
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];


                    #region License 控制

                    string strC006 = dr["C006"].ToString();
                    string strC007 = dr["C007"].ToString();
                    string strC008 = dr["C008"].ToString();
                    if (string.IsNullOrEmpty(strC006))
                    {
                        //C006为空的跳过
                        strLog += string.Format("C006 is empty;");
                        continue;
                    }
                    strC006 = DecryptFromDB(strC006);
                    string[] listC006 = strC006.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (listC006.Length < 3)
                    {
                        //C006无效
                        strLog += string.Format("C006 is invalid;");
                        continue;
                    }
                    long optID = Convert.ToInt64(dr["C002"]);
                    long licID = optID + 1000000000;
                    if (licID.ToString() != listC006[0])
                    {
                        //LicenseID与操作ID不对应
                        strLog += string.Format("LicID not equal;");
                        continue;
                    }
                    if (listC006[2] == "Y")
                    {
                        string strC008Hash = GetMD5HasString(strC008);
                        string strC008Hash8 = strC008Hash.Substring(0, 8);
                        string strLicDoggle = string.Format("{0}{1}", licID, doggleNumber);
                        strC008 = DecryptFromDB(strC008);
                        if (strLicDoggle != strC008)
                        {
                            //与C008不匹配
                            strLog += string.Format("C008 not equal;");
                            continue;
                        }
                        string strDecryptC007 = EncryptionAndDecryption.DecryptStringYKeyIV(strC007, strC008Hash8,
                            strC008Hash8);
                        string[] listC007 = strDecryptC007.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.None);
                        if (listC007.Length < 2)
                        {
                            //C007无效
                            strLog += string.Format("C007 is invalid;");
                            continue;
                        }
                        if (listC007[1] != "Y")
                        {
                            //没有许可
                            strLog += string.Format("No license;");
                            continue;
                        }
                    }

                    #endregion


                    OperationInfo opt = new OperationInfo();
                    opt.ID = Convert.ToInt64(dr["C002"]);
                    opt.ParentID = Convert.ToInt64(dr["C003"]);
                    opt.Display = string.Format("Opt({0})", opt.ID);
                    opt.Description = opt.Display;
                    opt.Icon = dr["C013"].ToString();
                    opt.SortID = Convert.ToInt32(dr["C004"]);
                    string strType = dr["C011"].ToString();
                    int intType = 0;
                    switch (strType)
                    {
                        case "M":
                            intType = 0;
                            break;
                        case "B":
                            intType = 1;
                            break;
                        case "T":
                            intType = 2;
                            break;
                        case "H":
                            intType = 3;
                            break;
                    }
                    opt.Type = intType;
                    optReturn = XMLHelper.SeriallizeObject(opt);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOpts.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOpts;
                optReturn.Message = strLog;
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
                switch (webRequest.Code)
                {
                    case (int)RequestCode.WSGetUserOptList:
                        optReturn = GetUserOptList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Request code invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Message = optReturn.Message;
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

        public WebReturn DoOperation(System.Net.WebRequest webRequest)
        {
            throw new NotImplementedException();
        }
    }
}
