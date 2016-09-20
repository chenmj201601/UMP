using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using VoiceCyber.VCLDAP;

namespace UMPService01
{
    public class S01AOperations
    {
        private string IStrSpliterChar = string.Empty;
        private TcpClient ITcpClient = null;

        #region 数据库类型及数据库连接参数
        private static int IIntDBType = 0;
        private static string IStrDBConnectProfile = string.Empty;
        #endregion

        #region 许可信息
        private int IIntP01 = 1;
        private int IIntP02 = 1;
        private string IStrP03 = string.Empty;
        private string IStrIsReadedLicense = string.Empty;
        #endregion

        public S01AOperations(int AIntDBType, string AStrDBConnectProfile, TcpClient ATcpClient)
        {
            IStrSpliterChar = AscCodeToChr(27);
            IIntDBType = AIntDBType;
            IStrDBConnectProfile = AStrDBConnectProfile;
            ITcpClient = ATcpClient;
        }

        public void S01ASetLicenseInfo(int AIntP01, int AIntP02, string AStrP03, string AStrLastReadedResult)
        {
            IIntP01 = AIntP01; IIntP02 = AIntP02; IStrP03 = AStrP03;
            IStrIsReadedLicense = AStrLastReadedResult;
        }

        public bool S01AOperation01(string AStrUserAccount, string AStrPassword, string AStrLoginMethod, string AStrLoginAsSaRole, List<string> AListStrOtherInfo, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode103 = string.Empty;
            string LStrUserAccount = string.Empty;
            string LStrUserInputPwd = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrRentCodeDB = string.Empty;
            string LStrRentCode5 = string.Empty;
            string LStrRentTableCode = string.Empty;

            DataTable LDataTableParameters = new DataTable();
            DataTable LDataTableRentInfo = new DataTable();
            DataTable LDataTableUserInfo = new DataTable();
            DataTable LDataTableLoginTurnOver = new DataTable();

            string LStrSelectSQL = string.Empty;
            string LStrDynamicSQL = string.Empty;

            string LStrWorkGroup = string.Empty;
            bool LBoolIsLDAPAccount = false;
            bool LBoolIsAutoLogin = false;

            string LStrCurrentStep = string.Empty;

            try
            {
                LStrCurrentStep = "Step 01";
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode103 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M103);
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(AStrUserAccount, LStrVerificationCode104,
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserInputPwd = EncryptionAndDecryption.EncryptDecryptString(AStrPassword, LStrVerificationCode104,
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                if (!LStrUserAccount.Contains("\\") && !LStrUserInputPwd.Contains(AscCodeToChr(30) + AscCodeToChr(30)))
                {
                    string[] LStrArrayUserAccount = LStrUserAccount.Split('@');
                    LStrUserAccount = LStrArrayUserAccount[0];
                    LStrRentToken = "NULL";
                    if (LStrArrayUserAccount.Length > 1)
                    {
                        LStrRentToken = LStrArrayUserAccount[1];
                    }
                    if (string.IsNullOrEmpty(LStrRentToken))
                    {
                        LStrRentToken = "NULL";
                    }
                    LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount,
                        LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LStrRentToken, LStrVerificationCode002,
                        EncryptionAndDecryption.UMPKeyAndIVType.M002);
                }
                else
                {
                    LBoolIsLDAPAccount = true;

                    #region 域帐户登录（手动输入域帐户 和 UMP中的密码）-- 代码未实现

                    if (LStrUserAccount.Contains("\\"))
                    {
                        LBoolIsAutoLogin = false;
                        string[] LStrArrayTempA = LStrUserAccount.Split('\\');
                        LStrWorkGroup = LStrArrayTempA[0];
                        LStrWorkGroup = EncryptionAndDecryption.EncryptDecryptString(LStrWorkGroup,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                            "SELECT * FROM T_00_012 WHERE C003 = '" + LStrWorkGroup + "' AND C009 = '0'");
                        if (LDBOperationReturn.StrReturn != "1")
                        {
                            //获取域信息失败
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            AStrReturnMessage = LDBOperationReturn.StrReturn;
                            return false;
                        }
                        LStrUserAccount = LStrUserAccount.Replace("\\", "@");
                        string[] LStrUserInfo = LStrUserAccount.Split('@');
                        LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);

                        LStrRentCode5 = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C002"].ToString();
                        LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);

                        try
                        {
                            string strADPath = string.Format("LDAP://{0}", LStrUserInfo[0]);
                            ADUtility utility = new ADUtility(strADPath, LStrUserInfo[1], LStrUserInputPwd);
                            ADUserCollection users = utility.GetAllUsers();
                            if (users != null)

                                LBoolIsAutoLogin = true;
                        }
                        catch (Exception ex)
                        {
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            AStrReturnMessage = ex.Message;
                            return false;
                        }

                    }

                    #endregion

                    #region 域帐户登录（系统自动登录）

                    if (LStrUserInputPwd.Contains(AscCodeToChr(30) + AscCodeToChr(30)))
                    {
                        LBoolIsAutoLogin = true;

                        string LStrWorkGroupAndRentCode5 = LStrUserInputPwd.Substring(2);
                        string[] LStrArrayTempA = LStrWorkGroupAndRentCode5.Split(AscCodeToChr(30).ToCharArray());
                        LStrWorkGroup = LStrArrayTempA[0];
                        LStrRentCode5 = LStrArrayTempA[1];

                        LStrUserAccount = LStrWorkGroup + "@" + LStrUserAccount;
                        LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);

                        LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    }

                    #endregion
                }

                #region 获取租户信息

                LStrCurrentStep = "Step 02";
                if (!LBoolIsLDAPAccount)
                {
                    LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                        "SELECT * FROM T_00_121 WHERE C022 = '" + LStrRentToken + "'");
                }
                else
                {
                    LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                        "SELECT * FROM T_00_121 WHERE C021 = '" + LStrRentCode5 + "'");
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //查找租户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A01", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    //没有找到对应的租户信息（租户不存在）
                    if (LDBOperationReturn.DataSetReturn.Tables[0].Rows.Count <= 0)
                    {
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A02", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                LStrCurrentStep = "Step 02-1";
                LStrRentCodeDB = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C021"].ToString();
                LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCodeDB, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (LStrRentCode5 == LStrRentCodeDB)
                {
                    //取租户唯一编码(5位，表名使用)失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A03", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                LStrCurrentStep = "Step 02-2";
                string LStrRentBegin = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C011"].ToString();
                string LStrRentEnd = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C012"].ToString();
                LStrRentBegin = EncryptionAndDecryption.EncryptDecryptString(LStrRentBegin, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrRentEnd = EncryptionAndDecryption.EncryptDecryptString(LStrRentEnd, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (DateTime.Parse(LStrRentBegin) > DateTime.UtcNow || DateTime.Parse(LStrRentEnd) < DateTime.UtcNow)
                {
                    //租户不在允许的租赁时间范围内
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A04", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                LStrCurrentStep = "Step 02-3";
                LDataTableRentInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                LStrCurrentStep = "Step 02-4";

                #endregion

                #region 判断许可信息，是否超过 许可期限，还没有考虑多租户的情况，下一版本要修改 2015-04-21 19:45:00

                LStrCurrentStep = "Step 02 - 01";
                if (IStrP03.ToUpper() == "Unlimited".ToUpper())
                {
                    IStrP03 = "2199-12-31 23:59:59";
                }
                if (IStrP03.ToUpper() == "Invalid".ToUpper() || IStrP03.ToUpper() == "Expired".ToUpper())
                {
                    IStrP03 = "2015-01-01 00:00:00";
                }
                if (DateTime.UtcNow > DateTime.Parse(IStrP03))
                {
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A06", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                #endregion

                #region 获取安全策略信息

                LStrCurrentStep = "Step 03";
                string LStr11010202 = string.Empty; //密码最长使用期限
                string LStr11010203 = string.Empty; //提示用户在密码过期之前进行更改
                string LStr11020301 = string.Empty; //允许同一账户登录多个UMP系统
                string LStr11020302 = string.Empty; //同一计算机尝试登录次数
                string LStr11030101 = string.Empty; //账户锁定阀值
                string LStr11030102 = string.Empty; //账户锁定时间
                string LStr11030103 = string.Empty; //在此后复位账户锁定计数器
                string LStr11020501 = string.Empty; //该用户已经超过XX天未登录UMP

                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    "SELECT * FROM T_11_001_" + LStrRentCode5 + " WHERE C003 >= 11010000 AND C003 <= 11039999");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取安全策略信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A11", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableParameters = LDBOperationReturn.DataSetReturn.Tables[0];

                LStr11010202 = LDataTableParameters.Select("C003 = 11010202").FirstOrDefault().Field<string>("C006");
                LStr11010202 = EncryptionAndDecryption.EncryptDecryptString(LStr11010202, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010202 = LStr11010202.Substring(8);

                LStr11010203 = LDataTableParameters.Select("C003 = 11010203").FirstOrDefault().Field<string>("C006");
                LStr11010203 = EncryptionAndDecryption.EncryptDecryptString(LStr11010203, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010203 = LStr11010203.Substring(8);

                LStr11020301 = LDataTableParameters.Select("C003 = 11020301").FirstOrDefault().Field<string>("C006");
                LStr11020301 = EncryptionAndDecryption.EncryptDecryptString(LStr11020301, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020301 = LStr11020301.Substring(8);

                LStr11020302 = LDataTableParameters.Select("C003 = 11020302").FirstOrDefault().Field<string>("C006");
                LStr11020302 = EncryptionAndDecryption.EncryptDecryptString(LStr11020302, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020302 = LStr11020302.Substring(8);

                LStr11030101 = LDataTableParameters.Select("C003 = 11030101").FirstOrDefault().Field<string>("C006");
                LStr11030101 = EncryptionAndDecryption.EncryptDecryptString(LStr11030101, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030101 = LStr11030101.Substring(8);

                LStr11030102 = LDataTableParameters.Select("C003 = 11030102").FirstOrDefault().Field<string>("C006");
                LStr11030102 = EncryptionAndDecryption.EncryptDecryptString(LStr11030102, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030102 = LStr11030102.Substring(8);

                LStr11030103 = LDataTableParameters.Select("C003 = 11030103").FirstOrDefault().Field<string>("C006");
                LStr11030103 = EncryptionAndDecryption.EncryptDecryptString(LStr11030103, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030103 = LStr11030103.Substring(8);

                LStr11020501 = LDataTableParameters.Select("C003 = 11020501").FirstOrDefault().Field<string>("C006");
                LStr11020501 = EncryptionAndDecryption.EncryptDecryptString(LStr11020501, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020501 = LStr11020501.Substring(8);

                #endregion

                #region 判断客户端尝试登录次数是否超过设置的阀值

                LStrCurrentStep = "Step 06";
                int LInt11020302 = 0;
                bool LBoolExistLessThan30Min = false;
                string LStrLoginGuid = string.Empty;

                LInt11020302 = int.Parse(LStr11020302);

                if (LInt11020302 > 0)
                {
                    lock (UMPService01.IListDateTimeClient)
                    {
                        int LIntAllLoginClient = UMPService01.IListStrLoginClient.Count;

                        //检测该客户端在 30 分钟内是否有登录的连接
                        for (int LIntLoopLoginClient = LIntAllLoginClient - 1;
                            LIntLoopLoginClient >= 0;
                            LIntLoopLoginClient--)
                        {
                            if (UMPService01.IListDateTimeClient[LIntLoopLoginClient].AddMinutes(30) > DateTime.UtcNow)
                            {
                                if (UMPService01.IListStrLoginClient[LIntLoopLoginClient] != AListStrOtherInfo[2])
                                {
                                    continue;
                                }
                                LBoolExistLessThan30Min = true;
                                break;
                            }
                        }

                        //如果不存在 30 分钟的登录连接，清楚所有该客户端 30 分钟前的连接信息
                        if (!LBoolExistLessThan30Min)
                        {
                            for (int LIntLoopLoginClient = LIntAllLoginClient - 1;
                                LIntLoopLoginClient >= 0;
                                LIntLoopLoginClient--)
                            {
                                if (UMPService01.IListStrLoginClient[LIntLoopLoginClient] != AListStrOtherInfo[2])
                                {
                                    continue;
                                }
                                UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                                UMPService01.IListStrLoginGuid.RemoveAt(LIntLoopLoginClient);
                                UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                            }
                        }

                        LIntAllLoginClient = 0;
                        LStrLoginGuid = Guid.NewGuid().ToString();
                        UMPService01.IListStrLoginClient.Add(AListStrOtherInfo[2]);
                        UMPService01.IListStrLoginGuid.Add(LStrLoginGuid);
                        UMPService01.IListDateTimeClient.Add(DateTime.UtcNow);
                        foreach (string LStrSingleClient in UMPService01.IListStrLoginClient)
                        {
                            if (LStrSingleClient == AListStrOtherInfo[2])
                            {
                                LIntAllLoginClient += 1;
                            }
                        }

                        if (LIntAllLoginClient > LInt11020302)
                        {
                            for (int LIntLoopClient = 0;
                                LIntLoopClient < UMPService01.IListStrLoginClient.Count;
                                LIntLoopClient++)
                            {
                                if (UMPService01.IListStrLoginClient[LIntLoopClient] == AListStrOtherInfo[2])
                                {
                                    UMPService01.IEventLog.WriteEntry(
                                        UMPService01.IListDateTimeClient[LIntLoopClient].ToString("G") + "   " +
                                        UMPService01.IListStrLoginClient[LIntLoopClient],
                                        System.Diagnostics.EventLogEntryType.Warning);
                                }
                            }

                            //同一计算机尝试登录次数超过系统设置阀值
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A41",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                    }
                }

                #endregion

                #region 获取用户信息

                LStrCurrentStep = "Step 04";

                string LStrLockedTime = string.Empty;

                LStrSelectSQL = "SELECT * FROM T_11_005_" + LStrRentCode5 + " WHERE C002 = '" + LStrUserAccount + "'";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取用户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableUserInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                if (LDataTableUserInfo.Rows.Count <= 0)
                {
                    //用户不存在
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrUserAccount;
                    return false;
                }

                //登录用户系统唯一19位编码 
                string LStr11005001 = LDataTableUserInfo.Rows[0]["C001"].ToString();

                //判断用户多久未登入
                string LStr11005007 = LDataTableUserInfo.Rows[0]["C007"].ToString();
                if (LStr11005007 != "S" && LStr11005007 != "H" && LDataTableUserInfo.Rows[0]["C025"].ToString() != "1")
                {
                    string LStr11005013Temp = LDataTableUserInfo.Rows[0]["C013"].ToString();
                    LStr11005013Temp = EncryptionAndDecryption.EncryptDecryptString(LStr11005013Temp,
                        LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);

                    DateTime nowTime = DateTime.Now.ToUniversalTime();
                    DateTime lastLoginTime = Convert.ToDateTime(LStr11005013Temp);
                    lastLoginTime = lastLoginTime.AddDays(Convert.ToInt16(LStr11020501));
                    TimeSpan midTime = nowTime - lastLoginTime;
                    if (midTime.TotalDays > 0)
                    {
                        LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentCode5 + " SET C009 = 'D', C008 = '1' WHERE C001 = " + LStr11005001;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A53", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LStr11020501 = EncryptionAndDecryption.EncryptDecryptString(LStr11020501, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnCode += IStrSpliterChar + LStr11020501;
                        return false;
                    }
                }

                if (LDataTableUserInfo.Rows[0]["C011"].ToString() == "1")
                {
                    //用户处于删除状态
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A23", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                if (LDataTableUserInfo.Rows[0]["C010"].ToString() == "0")
                {
                    //用户处于非活动状态
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A24", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                string LStrAllowLoginBegin = LDataTableUserInfo.Rows[0]["C017"].ToString();
                string LStrAllowLoginEnd = LDataTableUserInfo.Rows[0]["C018"].ToString();
                LStrAllowLoginBegin = EncryptionAndDecryption.EncryptDecryptString(LStrAllowLoginBegin,
                    LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrAllowLoginEnd = EncryptionAndDecryption.EncryptDecryptString(LStrAllowLoginEnd,
                    LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (LStrAllowLoginEnd == "UNLIMITED")
                {
                    LStrAllowLoginEnd = "2199-12-31 23:59:59";
                }
                if (DateTime.Parse(LStrAllowLoginBegin) > DateTime.UtcNow ||
                    DateTime.Parse(LStrAllowLoginEnd) < DateTime.UtcNow)
                {
                    //用户处于非允许登录时间范围内
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A25", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                if (LDataTableUserInfo.Rows[0]["C008"].ToString() == "1")
                {
                    if (LDataTableUserInfo.Rows[0]["C009"].ToString() == "U")
                    {
                        //用户处于锁定状态(管理员锁)
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A22", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                    if (LDataTableUserInfo.Rows[0]["C009"].ToString() == "L")
                    {
                        if (LStr11030102 == "0")
                        {
                            //用户处于锁定状态(违反安全策略锁),等待管理员解锁
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A29",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                        LStrLockedTime = LDataTableUserInfo.Rows[0]["C026"].ToString();
                        LStrLockedTime = EncryptionAndDecryption.EncryptDecryptString(LStrLockedTime,
                            LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        if (DateTime.Parse(LStrLockedTime).AddMinutes(int.Parse(LStr11030102)) > DateTime.UtcNow)
                        {
                            //用户处于锁定状态(违反安全策略锁)
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A27",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                        else
                        {
                            LDataTableUserInfo.Rows[0]["C008"] = "0";
                            LDataTableUserInfo.Rows[0]["C024"] = 0;
                            LDataTableUserInfo.Rows[0]["C026"] = "";
                            LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentCode5 +
                                             " SET C008 = '0', C024 = 0, C026 = '' WHERE C001 = " + LStr11005001;
                            LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                            LDataTableUserInfo.Rows[0]["C024"] = 0;
                        }
                    }
                }

                LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentCode5 + " SET C024 = C024 + 1 WHERE C001 = " +
                                 LStr11005001;
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                //登录用户全名
                string LStr11005003 = LDataTableUserInfo.Rows[0]["C003"].ToString();
                LStr11005003 = EncryptionAndDecryption.EncryptDecryptString(LStr11005003, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);

                //用户登录密码
                if (!LBoolIsAutoLogin)
                {
                    string LStr11005004 = LDataTableUserInfo.Rows[0]["C004"].ToString();
                    if (!LBoolIsLDAPAccount)
                    {
                        LStrUserInputPwd = EncryptionAndDecryption.EncryptStringSHA512(LStr11005001 + LStrUserInputPwd,
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        if (LStrUserInputPwd != LStr11005004)
                        {
                            //用户密码错误
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            AStrReturnMessage = LStrUserInputPwd;
                            return false;
                        }
                    }
                    else
                    {
                        LStr11005004 = EncryptionAndDecryption.EncryptDecryptString(LStr11005004,
                            LStrVerificationCode103, EncryptionAndDecryption.UMPKeyAndIVType.M103);
                        if (LStr11005001 + LStrUserInputPwd != LStr11005004)
                        {
                            //用户密码错误
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26",
                                LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            AStrReturnMessage = LStrUserInputPwd;
                            return false;
                        }
                    }
                }

                #endregion

                #region 判断登录次数是否已经超过系统设置的阀值

                LStrCurrentStep = "Step 05";
                int LInt11030101 = 0; //账户锁定阀值
                int LInt11005024 = 0;
                if (LStr11030101 != "0")
                {
                    LInt11030101 = int.Parse(LStr11030101);
                    LInt11005024 = int.Parse(LDataTableUserInfo.Rows[0]["C024"].ToString()) + 1;
                    if (LInt11005024 > LInt11030101)
                    {
                        //尝试登录次数超过 账户锁定阀值
                        LStrLockedTime = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString(),
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentCode5 + " SET C008 = '1', C009 = 'L', C024 = " +
                                         LInt11005024.ToString() + ", C026 = '" + LStrLockedTime + "' WHERE C001 = " +
                                         LStr11005001;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                        //锁定用户(违反安全策略锁)
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A28", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }

                #endregion

                #region 判断当前用户是否是系统超级管理员

                if (AStrLoginAsSaRole == "1")
                {
                    LStrCurrentStep = "Step 05 - 01";
                    LStrSelectSQL = "SELECT * FROM T_11_201_" + LStrRentCode5 + " WHERE C004 = " + LStr11005001 +
                                    " AND C003 = 1060000000000000001";
                    LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                        LStrSelectSQL);
                    if (!LDBOperationReturn.BoolReturn)
                    {
                        //获取用户角色信息失败
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A13", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LDBOperationReturn.StrReturn;
                        return false;
                    }
                    if (LDBOperationReturn.StrReturn != "1")
                    {
                        //该用户不允许以系统超级管理员身份登录
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A07", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LDBOperationReturn.StrReturn;
                        return false;
                    }
                }

                #endregion

                #region 获取用户历史登录流水信息

                LStrCurrentStep = "Step 07";
                string LStr11002002 = string.Empty; //登录时间
                string LStr11002004 = string.Empty; //登录机器名
                string LStr11002005 = string.Empty; //登录机器IP
                LStrSelectSQL = "SELECT * FROM T_11_002_" + LStrRentCode5 + " WHERE C001 = " + LStr11005001;
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取用户历史登录流水信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A31", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableLoginTurnOver = LDBOperationReturn.DataSetReturn.Tables[0];
                DataRow[] LDataRow11002 = LDataTableLoginTurnOver.Select("C001 =" + LStr11005001 + " AND C008 = '0'");
                if (LDataRow11002.Length > 0 && LStr11020301 != "1" && AStrLoginMethod != "F")
                {
                    //如果已经在其他机器上登录，且系统设置“允许同一账户打开多个UMP系统”的值为“不允许”
                    //该用户已经在其他机器上登录
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A32", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002002 = LDataRow11002[0]["C002"].ToString();
                    LStr11002004 = LDataRow11002[0]["C004"].ToString();
                    LStr11002005 = LDataRow11002[0]["C005"].ToString();
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnCode += IStrSpliterChar + LStr11002002 + IStrSpliterChar + LStr11002004 + IStrSpliterChar +
                                      LStr11002005;
                    return false;
                }
                string LStr11002009 = string.Empty;
                if (LDataRow11002.Length > 0 && AStrLoginMethod == "F")
                {
                    LStr11002009 =
                        EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"),
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStrDynamicSQL = "UPDATE T_11_002_" + LStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 +
                                     "', C010 = 'F' WHERE C001 = " + LStr11005001;
                    LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    if (!LDBOperationReturn.BoolReturn)
                    {
                        //排他性登录失败。
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A33", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LDBOperationReturn.StrReturn;
                        return false;
                    }
                }

                #endregion

                #region 判断许可信息，是否超过同时在线数，还没有考虑多租户的情况，下一版本要修改 2015-04-21 19:45:00

                LStrCurrentStep = "Step 07-01";
                LStrSelectSQL = "SELECT * FROM T_11_002_" + LStrRentCode5 +
                                " WHERE C001 > 1020000000000000000 AND C001 < 1030000000000000000 AND C008 <> '1'";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取当前在线数失败。
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A12", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (int.Parse(LDBOperationReturn.StrReturn) >= IIntP02)
                {
                    //超过系统最大在线许可数
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A05", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                #endregion

                #region 获取登录流水号

                LStrCurrentStep = "Step 08";
                string LStrLoginSessionID = string.Empty;
                if (AListStrOtherInfo[0] == "11000")
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11,
                        903, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11,
                        904, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //登录验证成功，系统分配SessionID失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A51", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LStrLoginSessionID = LDBOperationReturn.StrReturn;
                }

                #endregion

                #region 登录成功，回写信息至数据库

                LStrCurrentStep = "Step 09";
                LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(AListStrOtherInfo[1],
                    LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(AListStrOtherInfo[2],
                    LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                string LStr11005013 =
                    EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                        LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_005_" + LStrRentCode5 + " SET C013 = '" + LStr11005013 + "', C014 = '" +
                                 LStr11002004 + "', C015 = '" + LStr11002005 + "', C016 = " + AListStrOtherInfo[0] +
                                 ", C024 = 0 WHERE C001 = " + LStr11005001;
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //登录验证成功，回写数据库信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A52", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }

                LStr11002009 = EncryptionAndDecryption.EncryptDecryptString("NULL", LStrVerificationCode002,
                    EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "INSERT INTO T_11_002_" + LStrRentCode5 + " VALUES(" + LStr11005001 + ", '" +
                                 LStr11005013 + "', '00000000000000000000000000000000', '" + LStr11002004 + "', '" +
                                 LStr11002005 + "', " + LStrLoginSessionID + ", '0', '" + LStr11002009 + "', '0', '" +
                                 LStr11005013 + "', " + AListStrOtherInfo[0] + ")";
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                #endregion

                #region 登录成功，从当前登录列表中清除登录信息

                LStrCurrentStep = "Step 11";
                if (!string.IsNullOrEmpty(LStrLoginGuid))
                {
                    lock (UMPService01.IListDateTimeClient)
                    {
                        int LIntAllLoginClient = UMPService01.IListStrLoginClient.Count;
                        for (int LIntLoopLoginClient = LIntAllLoginClient - 1;
                            LIntLoopLoginClient >= 0;
                            LIntLoopLoginClient--)
                        {
                            if (UMPService01.IListStrLoginGuid[LIntLoopLoginClient] != LStrLoginGuid)
                            {
                                continue;
                            }
                            UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                            UMPService01.IListStrLoginGuid.RemoveAt(LIntLoopLoginClient);
                            UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                            break;
                        }
                    }
                }

                lock (UMPService01.IListDateTimeClient)
                {
                    int LIntAllLoginClient = UMPService01.IListStrLoginClient.Count;
                    for (int LIntLoopLoginClient = LIntAllLoginClient - 1;
                        LIntLoopLoginClient >= 0;
                        LIntLoopLoginClient--)
                    {
                        if (UMPService01.IListStrLoginClient[LIntLoopLoginClient] != AListStrOtherInfo[2])
                        {
                            continue;
                        }
                        UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                        UMPService01.IListStrLoginGuid.RemoveAt(LIntLoopLoginClient);
                        UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                    }
                }

                #endregion

                #region 登录各项验证过后，返回成功登录信息

                LStrCurrentStep = "Step 10";
                int LInt11020202 = 0; //密码最长使用期限
                int LInt11020203 = 0; //提示用户在密码过期之前进行更改
                DateTime LDateTime11005023;

                //0-登录成功
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01A00", LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //1-租户编码（5位）
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5, LStrVerificationCode004,
                                      EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //2-用户编码（19位）
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStr11005001, LStrVerificationCode004,
                                      EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //3-SessionID
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStrLoginSessionID,
                                      LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //4-用户全名
                AStrReturnCode += IStrSpliterChar + LStr11005003;
                if (!LBoolIsAutoLogin)
                {
                    if (LDataTableUserInfo.Rows[0]["C025"].ToString() == "1")
                    {
                        //5-为新用户，强制修改登录密码
                        AStrReturnCode += IStrSpliterChar +
                                          EncryptionAndDecryption.EncryptDecryptString("S01A01", LStrVerificationCode004,
                                              EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    }
                    else
                    {
                        LStrCurrentStep = "Step 10, LStr11010202 = " + LStr11010202;
                        LInt11020202 = int.Parse(LStr11010202);
                        if (LInt11020202 > 0)
                        {
                            LDateTime11005023 = DateTime.Parse(LDataTableUserInfo.Rows[0]["C023"].ToString());
                            TimeSpan LTimeSpan = DateTime.UtcNow.Subtract(LDateTime11005023);
                            int LIntDaysDiff = LTimeSpan.Days;
                            if (LIntDaysDiff > LInt11020202)
                            {
                                //5-密码已经过期，强制修改登录密码
                                AStrReturnCode += IStrSpliterChar +
                                                  EncryptionAndDecryption.EncryptDecryptString("S01A02",
                                                      LStrVerificationCode004,
                                                      EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            }
                            else
                            {
                                LStrCurrentStep = "Step 10, LInt11020203 = " + LInt11020203;
                                LInt11020203 = int.Parse(LStr11010203);
                                if ((LInt11020202 - LIntDaysDiff) < LInt11020203)
                                {
                                    //5-密码即将过期，建议修改登录密码
                                    AStrReturnCode += IStrSpliterChar +
                                                      EncryptionAndDecryption.EncryptDecryptString("S01A03",
                                                          LStrVerificationCode004,
                                                          EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    AStrReturnCode += IStrSpliterChar +
                                                      EncryptionAndDecryption.EncryptDecryptString(
                                                          (LInt11020202 - LIntDaysDiff + 1).ToString(),
                                                          LStrVerificationCode004,
                                                          EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation01()\n" + LStrCurrentStep + "\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation02(string AStrRentCode5, string AStrUserID, string AStrLoginSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr11002009 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStr11002009 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_002_" + AStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 + "', C010 = 'U' WHERE C001 = " + AStrUserID + " AND C006 = " + AStrLoginSessionID;
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //退出系统失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn == "1")
                {
                    //成功退出UMP系统，无警告
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    //成功退出UMP系统，可能有错误发生
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation02()\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation03(string AStrRentCode5, string AStrUserID, string AStrLoginSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr11002011 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStr11002011 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_002_" + AStrRentCode5 + " SET C011 = '" + LStr11002011 + "' WHERE C001 = " + AStrUserID + " AND C006 = " + AStrLoginSessionID + " AND C010 = '0'";
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //设置用户在线状态失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AB1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn == "1")
                {
                    //设置用户在线状态成功
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AB1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    //设置用户在线状态失败，未找到对应的登录信息
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AB2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation03()\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation04(string AStrRentCode5, string AStrUserID, string AStrOldPassword, string AStrNewPassword, string AStrSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            List<string> LListStrArgs = new List<string>();
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode003 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrUserAccount = string.Empty;
            string LStrOldPasswordDB = string.Empty;
            string LStrOldPasswordIn = string.Empty;
            string LStrUpdDBPassword = string.Empty;
            string LStrUtcNow = string.Empty;
            string LStrIsNew = string.Empty;
            bool LBoolIsLDAPAccount = false;

            try
            {
                #region 局部变量初始化
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode003 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M003);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrArgs.Add(IIntDBType.ToString());        //0
                LListStrArgs.Add(IStrDBConnectProfile);         //1
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AStrRentCode5, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));       //2
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AStrUserID, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));          //3
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AStrOldPassword, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));     //4
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AStrNewPassword, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));     //5

                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                GlobalParametersOperations LGlobalParametersOperation = new GlobalParametersOperations();
                OperationsReturn LGlobalOperationReturn = new OperationsReturn();
                #endregion

                #region 获取当前用户的基本信息
                LStrDynamicSQL = "SELECT * FROM T_11_005_" + LListStrArgs[2] + " WHERE C001 = " + LListStrArgs[3];
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E07", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDatabaseOperation01Return.StrReturn;
                    return false;
                }
                DataTable LDataTable11005 = LDatabaseOperation01Return.DataSetReturn.Tables[0];
                LStrIsNew = LDataTable11005.Rows[0]["C025"].ToString();
                LStrUserAccount = LDataTable11005.Rows[0]["C002"].ToString();
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (LStrUserAccount.Contains("@"))
                {
                    LBoolIsLDAPAccount = true;
                    LStrUpdDBPassword = EncryptionAndDecryption.EncryptDecryptString(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode003, EncryptionAndDecryption.UMPKeyAndIVType.M003);
                }
                else
                {
                    LStrUpdDBPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                }
                #endregion

                #region 获取当前用户所有的历史修改密码信息
                LStrDynamicSQL = "SELECT * FROM T_00_002 WHERE C000 = '" + LListStrArgs[2] + "' AND C001 = " + LListStrArgs[3] + " AND C003 = 'C004' ORDER BY C004 DESC";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E08", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDatabaseOperation01Return.StrReturn;
                    return false;
                }
                #endregion
                DataTable LDataTable00002 = LDatabaseOperation01Return.DataSetReturn.Tables[0];

                List<string> LListStr11001 = new List<string>();
                //0：复杂性要求；1：最短密码长度；2：最短密码使用期限；3：最长使用期限；4：强制密码历史（个数）；5：强制密码历史（天数）
                #region 获取需要用到的安全策略信息
                List<string> LListStr11001Source = new List<string>();
                LListStr11001Source.Add("11010101");
                LListStr11001Source.Add("11010102");
                LListStr11001Source.Add("11010201");
                LListStr11001Source.Add("11010202");
                LListStr11001Source.Add("11010301");
                LListStr11001Source.Add("11010302");
                foreach (string LStrSingle11001 in LListStr11001Source)
                {
                    LGlobalOperationReturn = LGlobalParametersOperation.GetParameterSettedValue(int.Parse(LListStrArgs[0]), LListStrArgs[1], LListStrArgs[2], LStrSingle11001);
                    if (!LGlobalOperationReturn.BoolReturn)
                    {
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E09", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LGlobalOperationReturn.StrReturn;
                        return false;
                    }
                    LListStr11001.Add(LGlobalOperationReturn.StrReturn);
                }
                #endregion

                #region 判断原密码是否正确
                LStrOldPasswordDB = LDataTable11005.Rows[0]["C004"].ToString();
                if (!LBoolIsLDAPAccount)
                {
                    LStrOldPasswordIn = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[4], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                }
                else
                {
                    LStrOldPasswordIn = EncryptionAndDecryption.EncryptDecryptString(LListStrArgs[3] + LListStrArgs[4], LStrVerificationCode003, EncryptionAndDecryption.UMPKeyAndIVType.M003);
                }
                if (LStrOldPasswordDB != LStrOldPasswordIn)
                {
                    //原密码错误
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                #endregion

                #region 判断最短密码使用期限
                if (LStrIsNew != "1")
                {
                    DateTime LDateTimeLastChanged = DateTime.Parse(LDataTable11005.Rows[0]["C023"].ToString());
                    DateTime LDateTimeCanChange = LDateTimeLastChanged.AddDays(int.Parse(LListStr11001[2]));
                    if (LDateTimeCanChange > DateTime.UtcNow)
                    {
                        //不满足密码策略中的“密码最短使用期限”
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                #endregion

                #region 判断新密码长度
                if (LListStrArgs[5].Length < int.Parse(LListStr11001[1]))
                {
                    //不满足密码策略中的“最短密码长度”
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                #endregion

                #region 判断强制密码历史（个数）
                bool LBoolExistEqual = false;
                int LIntCheckedCount = 0;
                string LStr00002006 = string.Empty;

                if (LListStr11001[4] != "0" && LStrIsNew != "1")
                {
                    foreach (DataRow LDataRowSingleChange in LDataTable00002.Rows)
                    {
                        LIntCheckedCount += 1;
                        LStr00002006 = LDataRowSingleChange["C006"].ToString();
                        if (LStr00002006 == LStrUpdDBPassword)
                        {
                            LBoolExistEqual = true; break;
                        }
                    }
                    if (LBoolExistEqual)
                    {
                        if (LIntCheckedCount <= int.Parse(LListStr11001[4]))
                        {
                            //不满足密码策略中的“强制密码历史（个数）”
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E04", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                    }
                }
                #endregion

                #region 判断强制密码历史（天数）
                if (LListStr11001[5] != "0" && LStrIsNew != "1")
                {
                    DateTime LDateTimeInspectionPeriod = DateTime.UtcNow.AddDays(int.Parse(LListStr11001[5]) * -1);

                    foreach (DataRow LDataRowSingleChange in LDataTable00002.Rows)
                    {
                        DateTime LDateTime00002004 = DateTime.Parse(LDataRowSingleChange["C004"].ToString());
                        if (LDateTime00002004 < LDateTimeInspectionPeriod) { continue; }
                        LStr00002006 = LDataRowSingleChange["C006"].ToString();
                        if (LStr00002006 == LStrUpdDBPassword)
                        {
                            LBoolExistEqual = true; break;
                        }
                    }
                    if (LBoolExistEqual)
                    {
                        //不满足密码策略中的“强制密码历史（天数）”
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E05", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                #endregion

                #region 判断复杂性要求
                if (LListStr11001[0] == "1")
                {
                    string LStrFalseReturn = string.Empty;
                    bool LBoolMeetComplexity = PasswordVerifyOptions.MeetComplexityRequirements(LListStrArgs[5], int.Parse(LListStr11001[1]), 64, "", ref LStrFalseReturn);

                    if (!LBoolMeetComplexity)
                    {
                        //不满足密码策略中的“密码必须符合复杂性要求”
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E06", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }

                }
                #endregion

                #region 将修改后的密码保存到 T_11_005

                LStrUtcNow = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                LStrDynamicSQL = "UPDATE T_11_005_" + LListStrArgs[2] + " SET C004 = '" + LStrUpdDBPassword + "', C025 = '0', ";
                if (LListStrArgs[0] == "3")
                {
                    LStrDynamicSQL += "C023 = TO_DATE('" + LStrUtcNow + "', 'yyyy/MM/dd HH24:mi:ss') ";
                }
                else
                {
                    LStrDynamicSQL += "C023 = CONVERT(DATETIME, '" + LStrUtcNow + "') ";
                }
                LStrDynamicSQL += "WHERE C001 = " + LListStrArgs[3];
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    //将密码更新到数据库失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E10", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDatabaseOperation01Return.StrReturn;
                    return false;
                }
                #endregion

                #region 修改XML文件的administrator的密码
                string LStrXmlFileName = string.Empty;
                string LStrA01 = string.Empty;
                string LStrSAPassword = string.Empty;
                try
                {
                    LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                    XmlDocument LXmlDocArgs02 = new XmlDocument();
                    LXmlDocArgs02.Load(LStrXmlFileName);
                    XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                    {
                        LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                        if (LStrA01 != LListStrArgs[3]) { continue; }
                        LStrSAPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                        LXmlNodeSingleUser.Attributes["A03"].Value = LStrSAPassword;
                        break;
                    }
                    LXmlDocArgs02.Save(LStrXmlFileName);
                }
                catch (Exception ex)
                {
                    //更新应用服务器的超级管理员密码失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E11", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = ex.Message;
                    return false;
                }
                #endregion

                #region 将修改记录保存到T_00_002中
                LStrDynamicSQL = "INSERT INTO T_00_002 VALUES('" + LListStrArgs[2] + "', " + LListStrArgs[3] + ", 102, 'C004', ";
                if (LListStrArgs[0] == "2")
                {
                    LStrDynamicSQL += " CONVERT(DATETIME, '" + LStrUtcNow + "'), '" + AscCodeToChr(27) + "', '" + LStrUpdDBPassword + "' , 2002, " + LListStrArgs[3] + ")";
                }
                else
                {
                    LStrDynamicSQL += " TO_DATE('" + LStrUtcNow + "', 'yyyy/MM/dd HH24:mi:ss'), '" + AscCodeToChr(27) + "', '" + LStrUpdDBPassword + "' , 2002, " + LListStrArgs[3] + ")";
                }
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    //将密码更新记录保存到数据库失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("W000E12", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDatabaseOperation01Return.StrReturn;
                    return false;
                }
                #endregion

                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01A01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation04()\n" + ex.Message;
            }
            return LBoolReturn;

        }

        public bool S01AOperation11(string AStrAgentAccount, string AStrPassword, string AStrLoginMethod, List<string> AListStrOtherInfo, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;

            string LStrAgentCode = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrRentCodeDB = string.Empty;
            string LStrRentCode5 = string.Empty;
            string LStrRentTableCode = string.Empty;

            DataTable LDataTableParameters = new DataTable();
            DataTable LDataTableRentInfo = new DataTable();
            DataTable LDataTableAgentInfo = new DataTable();
            DataTable LDataTableLoginTurnOver = new DataTable();

            string LStrCurrentStep = string.Empty;

            try
            {
                LStrCurrentStep = "Step 01";
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrAgentCode = EncryptionAndDecryption.EncryptDecryptString(AStrAgentAccount, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                string[] LStrArrayUserAccount = LStrAgentCode.Split('@');
                LStrAgentCode = LStrArrayUserAccount[0];
                LStrRentToken = "NULL";
                if (LStrArrayUserAccount.Length > 1) { LStrRentToken = LStrArrayUserAccount[1]; }
                if (string.IsNullOrEmpty(LStrRentToken)) { LStrRentToken = "NULL"; }
                LStrAgentCode = EncryptionAndDecryption.EncryptDecryptString(LStrAgentCode, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LStrRentToken, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                #region 获取租户信息
                LStrCurrentStep = "Step 02";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, "SELECT * FROM T_00_121 WHERE C022 = '" + LStrRentToken + "'");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //查找租户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    //没有找到对应的租户信息（租户不存在）
                    if (LDBOperationReturn.DataSetReturn.Tables[0].Rows.Count <= 0)
                    {
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                LStrRentCodeDB = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C021"].ToString();
                LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCodeDB, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (LStrRentCode5 == LStrRentCodeDB)
                {
                    //取租户唯一编码(5位，表名使用)失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                string LStrRentBegin = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C011"].ToString();
                string LStrRentEnd = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C012"].ToString();
                LStrRentBegin = EncryptionAndDecryption.EncryptDecryptString(LStrRentBegin, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrRentEnd = EncryptionAndDecryption.EncryptDecryptString(LStrRentEnd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (DateTime.Parse(LStrRentBegin) > DateTime.UtcNow || DateTime.Parse(LStrRentEnd) < DateTime.UtcNow)
                {
                    //租户不在允许的租赁时间范围内
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A04", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                LDataTableRentInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                #endregion

                #region 获取安全策略信息
                LStrCurrentStep = "Step 03";
                string LStr11010202 = string.Empty;             //密码最长使用期限
                string LStr11010203 = string.Empty;             //提示用户在密码过期之前进行更改
                string LStr11020301 = string.Empty;             //允许同一账户登录多个UMP系统
                string LStr11020302 = string.Empty;             //同一计算机尝试登录次数
                string LStr11030101 = string.Empty;             //账户锁定阀值
                string LStr11030102 = string.Empty;             //账户锁定时间
                string LStr11030103 = string.Empty;             //在此后复位账户锁定计数器

                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, "SELECT * FROM T_11_001_" + LStrRentCode5 + " WHERE C003 >= 11010000 AND C003 <= 11039999");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取安全策略信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A11", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableParameters = LDBOperationReturn.DataSetReturn.Tables[0];

                LStr11010202 = LDataTableParameters.Select("C003 = 11010202").FirstOrDefault().Field<string>("C006");
                LStr11010202 = EncryptionAndDecryption.EncryptDecryptString(LStr11010202, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010202 = LStr11010202.Substring(8);

                LStr11010203 = LDataTableParameters.Select("C003 = 11010203").FirstOrDefault().Field<string>("C006");
                LStr11010203 = EncryptionAndDecryption.EncryptDecryptString(LStr11010203, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010203 = LStr11010203.Substring(8);

                LStr11020301 = LDataTableParameters.Select("C003 = 11020301").FirstOrDefault().Field<string>("C006");
                LStr11020301 = EncryptionAndDecryption.EncryptDecryptString(LStr11020301, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020301 = LStr11020301.Substring(8);

                LStr11020302 = LDataTableParameters.Select("C003 = 11020302").FirstOrDefault().Field<string>("C006");
                LStr11020302 = EncryptionAndDecryption.EncryptDecryptString(LStr11020302, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020302 = LStr11020302.Substring(8);

                LStr11030101 = LDataTableParameters.Select("C003 = 11030101").FirstOrDefault().Field<string>("C006");
                LStr11030101 = EncryptionAndDecryption.EncryptDecryptString(LStr11030101, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030101 = LStr11030101.Substring(8);

                LStr11030102 = LDataTableParameters.Select("C003 = 11030102").FirstOrDefault().Field<string>("C006");
                LStr11030102 = EncryptionAndDecryption.EncryptDecryptString(LStr11030102, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030102 = LStr11030102.Substring(8);

                LStr11030103 = LDataTableParameters.Select("C003 = 11030103").FirstOrDefault().Field<string>("C006");
                LStr11030103 = EncryptionAndDecryption.EncryptDecryptString(LStr11030103, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030103 = LStr11030103.Substring(8);

                #endregion

                #region 判断客户端尝试登录次数是否超过设置的阀值, ********这个地方逻辑判断需要再细化
                LStrCurrentStep = "Step 06";
                int LInt11020302 = 0;
                bool LBoolExistLessThan30Min = false;
                string LStrLoginGuid = string.Empty;

                LInt11020302 = int.Parse(LStr11020302);

                if (LInt11020302 > 0)
                {
                    lock (UMPService01.IListDateTimeClient)
                    {
                        int LIntAllLoginClient = UMPService01.IListStrLoginClient.Count;

                        //检测该客户端在 30 分钟内是否有登录的连接
                        for (int LIntLoopLoginClient = LIntAllLoginClient - 1; LIntLoopLoginClient >= 0; LIntLoopLoginClient--)
                        {
                            if (UMPService01.IListDateTimeClient[LIntLoopLoginClient].AddMinutes(30) > DateTime.UtcNow)
                            {
                                if (UMPService01.IListStrLoginClient[LIntLoopLoginClient] != AListStrOtherInfo[2]) { continue; }
                                LBoolExistLessThan30Min = true;
                                break;
                            }
                            //if (UMPService01.IListDateTimeClient[LIntLoopLoginClient].AddMinutes(30) > DateTime.UtcNow) { continue; }
                            //UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                            //UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                        }

                        //如果不存在 30 分钟的登录连接，清楚所有该客户端 30 分钟前的连接信息
                        if (!LBoolExistLessThan30Min)
                        {
                            for (int LIntLoopLoginClient = LIntAllLoginClient - 1; LIntLoopLoginClient >= 0; LIntLoopLoginClient--)
                            {
                                if (UMPService01.IListStrLoginClient[LIntLoopLoginClient] != AListStrOtherInfo[2]) { continue; }
                                UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                                UMPService01.IListStrLoginGuid.RemoveAt(LIntLoopLoginClient);
                                UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                            }
                        }

                        LIntAllLoginClient = 0;
                        LStrLoginGuid = Guid.NewGuid().ToString();
                        UMPService01.IListStrLoginClient.Add(AListStrOtherInfo[2]);
                        UMPService01.IListStrLoginGuid.Add(LStrLoginGuid);
                        UMPService01.IListDateTimeClient.Add(DateTime.UtcNow);
                        foreach (string LStrSingleClient in UMPService01.IListStrLoginClient)
                        {
                            if (LStrSingleClient == AListStrOtherInfo[2]) { LIntAllLoginClient += 1; }
                        }
                        if (LIntAllLoginClient > LInt11020302)
                        {
                            //同一计算机尝试登录次数超过系统设置阀值
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A41", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                    }
                }
                #endregion

                #region 获取用户信息
                LStrCurrentStep = "Step 04";
                string LStrSelectSQL = string.Empty;
                string LStrDynamicSQL = string.Empty;
                string LStrLockedTime = string.Empty;

                LStrSelectSQL = "SELECT * FROM T_11_101_" + LStrRentCode5 + " WHERE C017 = '" + LStrAgentCode + "'";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取座席信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableAgentInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                if (LDataTableAgentInfo.Rows.Count <= 0)
                {
                    //座席不存在
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrAgentCode;
                    return false;
                }

                //登录用户系统唯一19位编码 
                string LStr11101001 = LDataTableAgentInfo.Rows[0]["C001"].ToString();
                //重新获取座席信息
                LStrSelectSQL = "SELECT * FROM T_11_101_" + LStrRentCode5 + " WHERE C001 = " + LStr11101001 + " ORDER BY C002 ASC";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取座席信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableAgentInfo = LDBOperationReturn.DataSetReturn.Tables[0];

                if (LDataTableAgentInfo.Rows[0]["C012"].ToString() != "1")
                {
                    //座席被删除或被禁用（处于非活动状态）
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A24", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                //string LStrAllowLoginBegin = LDataTableAgentInfo.Rows[0]["C017"].ToString();
                //string LStrAllowLoginEnd = LDataTableAgentInfo.Rows[0]["C018"].ToString();
                //LStrAllowLoginBegin = EncryptionAndDecryption.EncryptDecryptString(LStrAllowLoginBegin, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                //LStrAllowLoginEnd = EncryptionAndDecryption.EncryptDecryptString(LStrAllowLoginEnd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                //if (LStrAllowLoginEnd == "UNLIMITED") { LStrAllowLoginEnd = "2199-12-31 23:59:59"; }
                //if (DateTime.Parse(LStrAllowLoginBegin) > DateTime.UtcNow || DateTime.Parse(LStrAllowLoginEnd) < DateTime.UtcNow)
                //{
                //    //用户处于非允许登录时间范围内
                //    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A25", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //    return false;
                //}

                if (LDataTableAgentInfo.Rows[0]["C014"].ToString() == "1")
                {
                    if (LDataTableAgentInfo.Rows[0]["C015"].ToString() == "U")
                    {
                        //用户处于锁定状态(管理员锁)
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A22", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                    if (LDataTableAgentInfo.Rows[0]["C015"].ToString() == "L")
                    {
                        LStrLockedTime = LDataTableAgentInfo.Rows[1]["C014"].ToString();
                        LStrLockedTime = EncryptionAndDecryption.EncryptDecryptString(LStrLockedTime, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        if (DateTime.Parse(LStrLockedTime).AddMinutes(int.Parse(LStr11030102)) > DateTime.UtcNow)
                        {
                            //用户处于锁定状态(违反安全策略锁)
                            AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A27", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            return false;
                        }
                        else
                        {
                            LDataTableAgentInfo.Rows[0]["C014"] = "0";
                            LDataTableAgentInfo.Rows[1]["C012"] = 0;
                            LDataTableAgentInfo.Rows[1]["C014"] = "";
                            LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C014 = '0' WHERE C001 = " + LStr11101001 + " AND C002 = 1";
                            LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                            LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C012 = 0, C014 = '' WHERE C001 = " + LStr11101001 + " AND C002 = 2";
                            LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                            LDataTableAgentInfo.Rows[1]["C012"] = 0;
                        }
                    }
                }

                string LStr11101012B = LDataTableAgentInfo.Rows[1]["C012"].ToString();

                LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C012 = " + LStr11101012B + " + 1 WHERE C001 = " + LStr11101001 + " AND C002 = 2";
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                //登录用户全名
                string LStr11101018A = LDataTableAgentInfo.Rows[0]["C018"].ToString();
                LStr11101018A = EncryptionAndDecryption.EncryptDecryptString(LStr11101018A, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);

                //用户登录密码
                string LStr11101020 = LDataTableAgentInfo.Rows[0]["C020"].ToString();

                string LStrUserInputPwd = EncryptionAndDecryption.EncryptDecryptString(AStrPassword, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserInputPwd = EncryptionAndDecryption.EncryptStringSHA512(LStr11101001 + LStrUserInputPwd, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                if (LStrUserInputPwd != LStr11101020)
                {
                    //用户密码错误
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrUserInputPwd;
                    return false;
                }
                #endregion

                #region 判断登录次数是否已经超过系统设置的阀值
                LStrCurrentStep = "Step 05";
                int LInt11030101 = 0;               //账户锁定阀值
                int LInt11101002B = 0;
                if (LStr11030101 != "0")
                {
                    LInt11030101 = int.Parse(LStr11030101);
                    LInt11101002B = int.Parse(LDataTableAgentInfo.Rows[1]["C012"].ToString()) + 1;
                    if (LInt11101002B > LInt11030101)
                    {
                        //尝试登录次数超过 账户锁定阀值
                        LStrLockedTime = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString(), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C014 = '1', C015 = 'L' WHERE C001 = " + LStr11101001 + " AND C002 = 1";
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                        LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C012 = " + LInt11101002B.ToString() + ", C014 = '" + LStrLockedTime + "' WHERE C001 = " + LStr11101001 + " AND C002 = 1";
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                        
                        //锁定用户(违反安全策略锁)
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A28", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                #endregion

                #region 获取用户历史登录流水信息
                LStrCurrentStep = "Step 07";
                string LStr11002002 = string.Empty;             //登录时间
                string LStr11002004 = string.Empty;             //登录机器名
                string LStr11002005 = string.Empty;             //登录机器IP
                LStrSelectSQL = "SELECT * FROM T_11_002_" + LStrRentCode5 + " WHERE C001 = " + LStr11101001;
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取用户历史登录流水信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A31", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableLoginTurnOver = LDBOperationReturn.DataSetReturn.Tables[0];
                DataRow[] LDataRow11002 = LDataTableLoginTurnOver.Select("C001 =" + LStr11101001 + " AND C008 = '0'");
                if (LDataRow11002.Length > 0 && LStr11020301 != "1" && AStrLoginMethod != "F")
                {
                    //如果已经在其他机器上登录，且系统设置“允许同一账户打开多个UMP系统”的值为“不允许”
                    //该用户已经在其他机器上登录
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A32", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002002 = LDataRow11002[0]["C002"].ToString();
                    LStr11002004 = LDataRow11002[0]["C004"].ToString();
                    LStr11002005 = LDataRow11002[0]["C005"].ToString();
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnCode += IStrSpliterChar + LStr11002002 + IStrSpliterChar + LStr11002004 + IStrSpliterChar + LStr11002005;
                    return false;
                }
                string LStr11002009 = string.Empty;
                if (LDataRow11002.Length > 0 && AStrLoginMethod == "F")
                {
                    LStr11002009 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStrDynamicSQL = "UPDATE T_11_002_" + LStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 + "', C010 = 'F' WHERE C001 = " + LStr11101001;
                    LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    if (!LDBOperationReturn.BoolReturn)
                    {
                        //排他性登录失败。
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A33", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LDBOperationReturn.StrReturn;
                        return false;
                    }
                }
                #endregion

                #region 获取登录流水号
                LStrCurrentStep = "Step 08";
                string LStrLoginSessionID = string.Empty;
                if (AListStrOtherInfo[0] == "11000")
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 903, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11, 904, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //登录验证成功，系统分配SessionID失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A51", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LStrLoginSessionID = LDBOperationReturn.StrReturn;
                }
                #endregion

                #region 登录成功，回写信息至数据库
                LStrCurrentStep = "Step 09";
                LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(AListStrOtherInfo[1], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(AListStrOtherInfo[2], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                string LStr11101017B = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C017 = '" + LStr11101017B + "', C018 = '" + LStr11002004 + "', C019 = '" + LStr11002005 + "', C012 = 0 WHERE C001 = " + LStr11101001 + " AND C002 = 2";
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //登录验证成功，回写数据库信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A52", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }

                LStr11002009 = EncryptionAndDecryption.EncryptDecryptString("NULL", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "INSERT INTO T_11_002_" + LStrRentCode5 + " VALUES(" + LStr11101001 + ", '" + LStr11101017B + "', '00000000000000000000000000000000', '" + LStr11002004 + "', '" + LStr11002005 + "', " + LStrLoginSessionID + ", '0', '" + LStr11002009 + "', '0', '" + LStr11101017B + "', " + AListStrOtherInfo[0] + ")";
                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                #endregion

                #region 登录成功，从当前登录列表中清除登录信息
                LStrCurrentStep = "Step 11";
                if (!string.IsNullOrEmpty(LStrLoginGuid))
                {
                    lock (UMPService01.IListDateTimeClient)
                    {
                        int LIntAllLoginClient = UMPService01.IListStrLoginClient.Count;
                        for (int LIntLoopLoginClient = LIntAllLoginClient - 1; LIntLoopLoginClient >= 0; LIntLoopLoginClient--)
                        {
                            if (UMPService01.IListStrLoginGuid[LIntLoopLoginClient] != LStrLoginGuid) { continue; }
                            UMPService01.IListStrLoginClient.RemoveAt(LIntLoopLoginClient);
                            UMPService01.IListStrLoginGuid.RemoveAt(LIntLoopLoginClient);
                            UMPService01.IListDateTimeClient.RemoveAt(LIntLoopLoginClient);
                            break;
                        }
                    }
                }
                #endregion

                #region 登录各项验证过后，返回成功登录信息
                LStrCurrentStep = "Step 10";
                int LInt11020202 = 0;             //密码最长使用期限
                int LInt11020203 = 0;             //提示用户在密码过期之前进行更改
                DateTime LDateTime11101011B;

                //0-登录成功
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //1-租户编码（5位）
                AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //2-用户编码（19位）
                AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString(LStr11101001, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //3-SessionID
                AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString(LStrLoginSessionID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //4-用户全名
                AStrReturnCode += IStrSpliterChar + LStr11101018A;
                if (LDataTableAgentInfo.Rows[0]["C013"].ToString() == "1")
                {
                    //5-为新用户，强制修改登录密码
                    AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString("S01A01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    LStrCurrentStep = "Step 10, LStr11010202 = " + LStr11010202;
                    LInt11020202 = int.Parse(LStr11010202);
                    if (LInt11020202 > 0)
                    {
                        LDateTime11101011B = DateTime.Parse(LDataTableAgentInfo.Rows[1]["C011"].ToString());
                        TimeSpan LTimeSpan = DateTime.UtcNow.Subtract(LDateTime11101011B);
                        int LIntDaysDiff = LTimeSpan.Days;
                        if (LIntDaysDiff > LInt11020202)
                        {
                            //5-密码已经过期，强制修改登录密码
                            AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString("S01A02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        }
                        else
                        {
                            LStrCurrentStep = "Step 10, LInt11020203 = " + LInt11020203;
                            LInt11020203 = int.Parse(LStr11010203);
                            if ((LInt11020202 - LIntDaysDiff) < LInt11020203)
                            {
                                //5-密码即将过期，建议修改登录密码
                                AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString("S01A03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                AStrReturnCode += IStrSpliterChar + EncryptionAndDecryption.EncryptDecryptString((LInt11020202 - LIntDaysDiff + 1).ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation11()\n" + LStrCurrentStep + "\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation12(string AStrRentCode5, string AStrAgentID, string AStrLoginSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr11002009 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStr11002009 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_002_" + AStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 + "', C010 = 'U' WHERE C001 = " + AStrAgentID + " AND C006 = " + AStrLoginSessionID;
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //退出系统失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn == "1")
                {
                    //成功退出UMP系统，无警告
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    //成功退出UMP系统，可能有错误发生
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation12()\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation13(string AStrRentCode5, string AStrAgentID, string AStrLoginSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr11002011 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStr11002011 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_002_" + AStrRentCode5 + " SET C011 = '" + LStr11002011 + "' WHERE C001 = " + AStrAgentID + " AND C006 = " + AStrLoginSessionID + " AND C010 = '0'";
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //设置用户在线状态失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AB1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn == "1")
                {
                    //设置用户在线状态成功
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AB1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    //设置用户在线状态失败，未找到对应的登录信息
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AB2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation13()\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation14(string AStrAgentexAccount, string AStrPassword, string AStrLoginMethod, List<string> AListStrOtherInfo, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrAgentexAccount = string.Empty;
            string LStrUserInputPwd = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrRentCodeDB = string.Empty;
            string LStrRentCode5 = string.Empty;
            string LStrRentTableCode = string.Empty;

            DataTable LDataTableParameters = new DataTable();
            DataTable LDataTableRentInfo = new DataTable();
            DataTable LDataTableAgentexInfo = new DataTable();
            DataTable LDataTableLoginTurnOver = new DataTable();

            string LStrCurrentStep = string.Empty;

            try
            {
                LStrCurrentStep = "Step 01";
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrAgentexAccount = EncryptionAndDecryption.EncryptDecryptString(AStrAgentexAccount,
                    LStrVerificationCode104,
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserInputPwd = EncryptionAndDecryption.EncryptDecryptString(AStrPassword, LStrVerificationCode104,
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);

                string[] LStrArrayUserAccount = LStrAgentexAccount.Split('@');
                LStrAgentexAccount = LStrArrayUserAccount[0];
                LStrRentToken = "NULL";
                if (LStrArrayUserAccount.Length > 1) { LStrRentToken = LStrArrayUserAccount[1]; }
                if (string.IsNullOrEmpty(LStrRentToken)) { LStrRentToken = "NULL"; }
                LStrAgentexAccount = EncryptionAndDecryption.EncryptDecryptString(LStrAgentexAccount,
                    LStrVerificationCode002,
                    EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LStrRentToken, LStrVerificationCode002,
                    EncryptionAndDecryption.UMPKeyAndIVType.M002);
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                #region 获取租户信息

                LStrCurrentStep = "Step 02";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    "SELECT * FROM T_00_121 WHERE C022 = '" + LStrRentToken + "'");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //查找租户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A01", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    //没有找到对应的租户信息（租户不存在）
                    if (LDBOperationReturn.DataSetReturn.Tables[0].Rows.Count <= 0)
                    {
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A02", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        return false;
                    }
                }
                LStrRentCodeDB = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C021"].ToString();
                LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCodeDB, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (LStrRentCode5 == LStrRentCodeDB)
                {
                    //取租户唯一编码(5位，表名使用)失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A03", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                string LStrRentBegin = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C011"].ToString();
                string LStrRentEnd = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C012"].ToString();
                LStrRentBegin = EncryptionAndDecryption.EncryptDecryptString(LStrRentBegin, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrRentEnd = EncryptionAndDecryption.EncryptDecryptString(LStrRentEnd, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (DateTime.Parse(LStrRentBegin) > DateTime.UtcNow || DateTime.Parse(LStrRentEnd) < DateTime.UtcNow)
                {
                    //租户不在允许的租赁时间范围内
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A04", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                LDataTableRentInfo = LDBOperationReturn.DataSetReturn.Tables[0];

                #endregion

                #region 获取安全策略信息

                LStrCurrentStep = "Step 03";
                string LStr11010202 = string.Empty; //密码最长使用期限
                string LStr11010203 = string.Empty; //提示用户在密码过期之前进行更改
                string LStr11020301 = string.Empty; //允许同一账户登录多个UMP系统
                string LStr11020302 = string.Empty; //同一计算机尝试登录次数
                string LStr11030101 = string.Empty; //账户锁定阀值
                string LStr11030102 = string.Empty; //账户锁定时间
                string LStr11030103 = string.Empty; //在此后复位账户锁定计数器

                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    "SELECT * FROM T_11_001_" + LStrRentCode5 + " WHERE C003 >= 11010000 AND C003 <= 11039999");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取安全策略信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A11", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableParameters = LDBOperationReturn.DataSetReturn.Tables[0];

                LStr11010202 = LDataTableParameters.Select("C003 = 11010202").FirstOrDefault().Field<string>("C006");
                LStr11010202 = EncryptionAndDecryption.EncryptDecryptString(LStr11010202, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010202 = LStr11010202.Substring(8);

                LStr11010203 = LDataTableParameters.Select("C003 = 11010203").FirstOrDefault().Field<string>("C006");
                LStr11010203 = EncryptionAndDecryption.EncryptDecryptString(LStr11010203, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11010203 = LStr11010203.Substring(8);

                LStr11020301 = LDataTableParameters.Select("C003 = 11020301").FirstOrDefault().Field<string>("C006");
                LStr11020301 = EncryptionAndDecryption.EncryptDecryptString(LStr11020301, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020301 = LStr11020301.Substring(8);

                LStr11020302 = LDataTableParameters.Select("C003 = 11020302").FirstOrDefault().Field<string>("C006");
                LStr11020302 = EncryptionAndDecryption.EncryptDecryptString(LStr11020302, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11020302 = LStr11020302.Substring(8);

                LStr11030101 = LDataTableParameters.Select("C003 = 11030101").FirstOrDefault().Field<string>("C006");
                LStr11030101 = EncryptionAndDecryption.EncryptDecryptString(LStr11030101, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030101 = LStr11030101.Substring(8);

                LStr11030102 = LDataTableParameters.Select("C003 = 11030102").FirstOrDefault().Field<string>("C006");
                LStr11030102 = EncryptionAndDecryption.EncryptDecryptString(LStr11030102, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030102 = LStr11030102.Substring(8);

                LStr11030103 = LDataTableParameters.Select("C003 = 11030103").FirstOrDefault().Field<string>("C006");
                LStr11030103 = EncryptionAndDecryption.EncryptDecryptString(LStr11030103, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11030103 = LStr11030103.Substring(8);

                #endregion

                #region 获取用户信息

                LStrCurrentStep = "Step 04";
                string LStrSelectSQL = string.Empty;
                string LStrDynamicSQL = string.Empty;
                string LStrLockedTime = string.Empty;

                LStrSelectSQL = "SELECT * FROM T_11_101_" + LStrRentCode5 + " WHERE C017 = '" + LStrAgentexAccount + "'";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取坐席信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableAgentexInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                if (LDataTableAgentexInfo.Rows.Count <= 0)
                {
                    //坐席不存在
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrAgentexAccount;
                    return false;
                }

                //登录用户系统唯一19位编码 
                string LStr11101001 = LDataTableAgentexInfo.Rows[0]["C001"].ToString();
                //重新获取座席信息
                LStrSelectSQL = "SELECT * FROM T_11_101_" + LStrRentCode5 + " WHERE C001 = " + LStr11101001 +
                                " ORDER BY C002 ASC";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取坐席失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }

                LDataTableAgentexInfo = LDBOperationReturn.DataSetReturn.Tables[0];

                if (LDataTableAgentexInfo.Rows[0]["C012"].ToString() == "0")
                {
                    //坐席处于非活动状态
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A24", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }

                if (LDataTableAgentexInfo.Rows[0]["C014"].ToString() == "1")
                {
                    if (LDataTableAgentexInfo.Rows[0]["C015"].ToString() == "L")
                    {
                        LStrLockedTime = LDataTableAgentexInfo.Rows[1]["C014"].ToString();
                        LStrLockedTime = EncryptionAndDecryption.EncryptDecryptString(LStrLockedTime,
                            LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LDataTableAgentexInfo.Rows[0]["C014"] = "0";
                        LDataTableAgentexInfo.Rows[1]["C012"] = 0;
                        LDataTableAgentexInfo.Rows[1]["C014"] = "";
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                        LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 +
                                         " SET C012 = 0, C014 = '' WHERE C001 = " + LStr11101001 + " AND C002 = 2";
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                        LDataTableAgentexInfo.Rows[1]["C012"] = 0;
                    }
                }

                string LStr11101012B = LDataTableAgentexInfo.Rows[1]["C012"].ToString();
                LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentCode5 + " SET C012 = " + LStr11101012B +
                                 " + 1 WHERE C001 = " + LStr11101001 + " AND C002 = 2";

                LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                //登录坐席全名
                string LStr11101018A = LDataTableAgentexInfo.Rows[0]["C018"].ToString();
                LStr11101018A = EncryptionAndDecryption.EncryptDecryptString(LStr11101018A, LStrVerificationCode102,
                    EncryptionAndDecryption.UMPKeyAndIVType.M102);

                //坐席登录密码
                string LStr11101020 = LDataTableAgentexInfo.Rows[0]["C020"].ToString();
                string LStrInputPwd = EncryptionAndDecryption.EncryptDecryptString(AStrPassword,
                    LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserInputPwd = EncryptionAndDecryption.EncryptStringSHA512(LStr11101001 + LStrUserInputPwd,
                    LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);

                if (LStrUserInputPwd != LStr11101020)
                {
                    //用户密码错误
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26",
                        LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrUserInputPwd;
                    return false;
                }

                #endregion

                #region 获取用户历史登录流水信息

                LStrCurrentStep = "Step 07";
                string LStr11002002 = string.Empty; //登录时间
                string LStr11002004 = string.Empty; //登录机器名
                string LStr11002005 = string.Empty; //登录机器IP
                LStrSelectSQL = "SELECT * FROM T_11_002_" + LStrRentCode5 + " WHERE C001 = " + LStr11101001;
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile,
                    LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取用户历史登录流水信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A31", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                LDataTableLoginTurnOver = LDBOperationReturn.DataSetReturn.Tables[0];
                DataRow[] LDataRow11002 = LDataTableLoginTurnOver.Select("C001 =" + LStr11101001 + " AND C008 = '0'");
                if (LDataRow11002.Length > 0 && LStr11020301 != "1" && AStrLoginMethod != "F")
                {
                    //如果已经在其他机器上登录，且系统设置“允许同一账户打开多个UMP系统”的值为“不允许”
                    //该用户已经在其他机器上登录
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A32", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002002 = LDataRow11002[0]["C002"].ToString();
                    LStr11002004 = LDataRow11002[0]["C004"].ToString();
                    LStr11002005 = LDataRow11002[0]["C005"].ToString();
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode102,
                        EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStr11002002 = EncryptionAndDecryption.EncryptDecryptString(LStr11002002, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002004 = EncryptionAndDecryption.EncryptDecryptString(LStr11002004, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStr11002005 = EncryptionAndDecryption.EncryptDecryptString(LStr11002005, LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnCode += IStrSpliterChar + LStr11002002 + IStrSpliterChar + LStr11002004 + IStrSpliterChar +
                                      LStr11002005;
                    return false;
                }
                string LStr11002009 = string.Empty;
                if (LDataRow11002.Length > 0 && AStrLoginMethod == "F")
                {
                    LStr11002009 =
                        EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"),
                            LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStrDynamicSQL = "UPDATE T_11_002_" + LStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 +
                                     "', C010 = 'F' WHERE C001 = " + LStr11101001;
                    LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    if (!LDBOperationReturn.BoolReturn)
                    {
                        //排他性登录失败。
                        AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A33", LStrVerificationCode004,
                            EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        AStrReturnMessage = LDBOperationReturn.StrReturn;
                        return false;
                    }
                }

                #endregion

                #region 获取登录流水号

                LStrCurrentStep = "Step 08";
                string LStrLoginSessionID = string.Empty;
                if (AListStrOtherInfo[0] == "11000")
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11,
                        903, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(IIntDBType, IStrDBConnectProfile, 11,
                        904, LStrRentCode5, DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                }
                if (!LDBOperationReturn.BoolReturn)
                {
                    //登录验证成功，系统分配SessionID失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A51", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LStrLoginSessionID = LDBOperationReturn.StrReturn;
                }

                #endregion

                #region 登录各项验证过后，返回成功登录信息

                LStrCurrentStep = "Step 10";
                int LInt11020202 = 0; //密码最长使用期限
                int LInt11020203 = 0; //提示用户在密码过期之前进行更改
                DateTime LDateTime11101011B;

                //0-登录成功
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01A00", LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //1-租户编码（5位）
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5, LStrVerificationCode004,
                                      EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //2-用户编码（19位）
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStr11101001, LStrVerificationCode004,
                                      EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //3-SessionID
                AStrReturnCode += IStrSpliterChar +
                                  EncryptionAndDecryption.EncryptDecryptString(LStrLoginSessionID,
                                      LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //4-用户全名
                AStrReturnCode += IStrSpliterChar + LStr11101018A;

                #endregion
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation01()\n" + LStrCurrentStep + "\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation15(string AStrRentCode5, string AStrAgentexID, string AStrLoginSessionID, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr11002009 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStr11002009 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_002_" + AStrRentCode5 + " SET C008 = '1', C009 = '" + LStr11002009 + "', C010 = 'U' WHERE C001 = " + AStrAgentexID + " AND C006 = " + AStrLoginSessionID;
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //退出系统失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn == "1")
                {
                    //成功退出UMP系统，无警告
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    //成功退出UMP系统，可能有错误发生
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01AA2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation12()\n" + ex.Message;
            }

            return LBoolReturn;
        }

        public bool S01AOperation21(string AStrRentCode5, string AStrUserID, string AStrPassword, string AStrC001002077Value, string AStrColumnName, string AStrPartionInfo, ref string AStrReturnCode, ref string AStrReturnMessage)
        {
            bool LBoolReturn = true;

            string LStrDynamicSQL = string.Empty;
            string LStr00121021 = string.Empty;
            string LStrUserAccount = string.Empty;
            bool LBoolIsLDAPAccount = false;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode003 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            DataTable LDataTableUserInfo = new DataTable();
            DataTable LDataTable21001 = new DataTable();
            string LStrCurrentStep = string.Empty;

            try
            {
                LStrCurrentStep = "Step 01";
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode003 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M003);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                #region 验证租户信息
                LStrCurrentStep = "Step 02";
                LStr00121021 = EncryptionAndDecryption.EncryptDecryptString(AStrRentCode5, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, "SELECT * FROM T_00_121 WHERE C021 = '" + LStr00121021 + "'");
                if (!LDBOperationReturn.BoolReturn)
                {
                    //查找租户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn != "1")
                {
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                #endregion

                #region 获取用户、座席信息
                LStrCurrentStep = "Step 03";
                if (AStrUserID.Substring(0, 3) == "102")
                {
                    LStrDynamicSQL = "SELECT C001, C004, C002 FROM T_11_005_" + AStrRentCode5 + " WHERE C001 = " + AStrUserID;
                }
                else
                {
                    LStrDynamicSQL = "SELECT C001, C020 AS C004 FROM T_11_101_" + AStrRentCode5 + " WHERE C001 = " + AStrUserID + " AND C002 = 1";
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //获取用户信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A21", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                else
                {
                    LDataTableUserInfo = LDBOperationReturn.DataSetReturn.Tables[0];
                    if (AStrUserID.Substring(0, 3) == "102")
                    {
                        LStrUserAccount = LDataTableUserInfo.Rows[0]["C002"].ToString();
                        LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        if (LStrUserAccount.Contains("@")) { LBoolIsLDAPAccount = true; }
                    }
                }
                #endregion

                #region 验证用户密码
                LStrCurrentStep = "Step 04";
                //登录用户系统唯一19位编码 
                string LStr11005001 = LDataTableUserInfo.Rows[0]["C001"].ToString();
                //用户登录密码
                string LStr11005004 = LDataTableUserInfo.Rows[0]["C004"].ToString();
                string LStrUserInputPwd = string.Empty;

                if (LStr11005001 == "1020000000000000002")
                {
                    LStrUserInputPwd = EncryptionAndDecryption.EncryptDecryptString(AStrPassword, LStrVerificationCode003, EncryptionAndDecryption.UMPKeyAndIVType.M003);
                }
                else
                {
                    if (!LBoolIsLDAPAccount)
                    {
                        LStrUserInputPwd = EncryptionAndDecryption.EncryptStringSHA512(LStr11005001 + AStrPassword, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    }
                    else
                    {
                        LStrUserInputPwd = EncryptionAndDecryption.EncryptDecryptString(LStr11005001 + AStrPassword, LStrVerificationCode003, EncryptionAndDecryption.UMPKeyAndIVType.M003);
                    }
                }
                if (LStrUserInputPwd != LStr11005004)
                {
                    //用户密码错误
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A26", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = EncryptionAndDecryption.EncryptDecryptString(AStrPassword, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    return false;
                }
                #endregion

                #region 获取录音文件信息
                LStrCurrentStep = "Step 05";
                if (string.IsNullOrEmpty(AStrPartionInfo))
                {
                    LStrDynamicSQL = "SELECT C033, C035, C036 FROM T_21_001_" + AStrRentCode5 + " WHERE ";
                }
                else
                {
                    LStrDynamicSQL = "SELECT C033, C035, C036 FROM T_21_001_" + AStrRentCode5 + "_" + AStrPartionInfo + " WHERE ";
                }
                if (AStrColumnName == "C077")
                {
                    LStrDynamicSQL = LStrDynamicSQL + AStrColumnName + " = '" + AStrC001002077Value + "'";
                }
                else
                {
                    LStrDynamicSQL = LStrDynamicSQL + AStrColumnName + " = " + AStrC001002077Value;
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    //查找录音信息失败
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A61", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LDBOperationReturn.StrReturn;
                    return false;
                }
                if (LDBOperationReturn.StrReturn != "1")
                {
                    //没有查询到对应的录音信息
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A62", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    AStrReturnMessage = LStrDynamicSQL;
                    return false;
                }
                LDataTable21001 = LDBOperationReturn.DataSetReturn.Tables[0];
                if (LDataTable21001.Rows[0]["C033"].ToString().ToUpper() == "Y")
                {
                    //录音文件已从本地删除
                    AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A63", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return false;
                }
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("S01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                string LStr21001035 = string.Empty, LStr21001036 = string.Empty;
                LStr21001035 = LDataTable21001.Rows[0]["C035"].ToString();
                LStr21001036 = LDataTable21001.Rows[0]["C036"].ToString();
                AStrReturnCode += IStrSpliterChar + LStr21001035 + IStrSpliterChar + LStr21001036;
                #endregion
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturnCode = EncryptionAndDecryption.EncryptDecryptString("E01A99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                AStrReturnMessage = "S01AOperations.S01AOperation21()\n" + LStrCurrentStep + ex.Message;
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
