using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using UMPService00;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Service00Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //string strExeFileDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            //InvokeMethodOptions OperationMethodOptions = new InvokeMethodOptions(null, new TimeSpan(0, 10, 0));
            //ManagementBaseObject LOutStopService;

            //string AStrServiceName = "UMPVoiceServer";
            //ManagementObjectCollection ObjectCollection;
            //ObjectCollection = GetServiceCollection("SELECT * FROM Win32_Service WHERE Name = 'UMPVoiceServer'");
            //int i = ObjectCollection.Count;
            //string LStrProcessId = string.Empty;
            //string LStrDisplayName = string.Empty;
            //int LIntReturnValue = -1;

            //foreach (ManagementObject SingleCollenction in ObjectCollection)
            //{
            //    LStrProcessId = SingleCollenction["ProcessId"].ToString();
            //    LStrDisplayName = SingleCollenction["DisplayName"].ToString();
            //    LOutStopService = SingleCollenction.InvokeMethod("StopService", null, OperationMethodOptions);
            //    LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
            //    if (LIntReturnValue != 0)
            //    {
            //       int  LIntWaitCount = 0;
            //      string  LStrServiceCurrentStatus = GetComputerServiceStatus(AStrServiceName);
            //       while (LStrServiceCurrentStatus != "0" && LIntWaitCount < 30)
            //       {
            //           System.Threading.Thread.Sleep(1000);
            //           LIntWaitCount += 1;
            //          // LStrServiceCurrentStatus = GetComputerServiceStatus(AStrServiceName);
            //       }
            //       if (LStrServiceCurrentStatus == "0") { LIntReturnValue = 0; }
            //       else { KillProcess(LStrProcessId); LIntReturnValue = 0; }
            //    }
            //}

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.9.236";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB20160427";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "voicecodes";
          
            //UpdateReplaceModuleNumberInDB(dbInfo, "1", "2");

            //GetVoiceServerHostByModuleNumber(0);

            //StartBackupMachine("2", "1");

            GetAllMachines(dbInfo);
        }
        private void KillProcess(string AStrProcessId)
        {
            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("Select * from win32_process where ProcessID = '" + AStrProcessId + "'");
                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    SingleCollenction.InvokeMethod("Terminate", null);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static OperationReturn GetAllMachines(DatabaseInfo dbInfo)
        {
            Dictionary<int, MachineInfo> lstMachines = new Dictionary<int, MachineInfo>();
            string strConnectString = dbInfo.GetConnectionString();
            OperationReturn optReturn = new OperationReturn();
            string strSql = string.Empty;
            switch (dbInfo.TypeID)
            {
                case 2:
                    strSql = "select a.C001,a.C012,b.c011,a.c002 as A002 ,b.c002 as B002,a.C017,c.C012 as D012 from t_11_101_{0} a " +
                                 "left join t_11_101_{0} b on a.c001 = b.c001 " +
                                 "left join t_11_101_{0} c on a.c001 = c.c001 " +
                                  "where a.C001 >2210000000000000000 and a.C001 <2220000000000000000 and a.C002 =1 " +
                                  "and  b.C001 >2210000000000000000 and b.C001 <2220000000000000000 and b.C002 =3 " +
                                  " and c.C001 >2210000000000000000 and c.C001 <2220000000000000000 and c.C002 =2 ";
                    strSql = string.Format(strSql, "00000", "00000");
                    optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                    break;
                case 3:
                    strSql = "select a.C001,a.C012,b.c011,a.c002 as A002 ,b.c002 as B002,a.C017,c.C012 as D012 from t_11_101_{0} a " +
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
                optReturn.Message = "GetAllMachines error ,sql = " + strSql;
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
                }
                //lstMachines.Add(int.Parse(row["C012"].ToString()), row["C011"].ToString());
            }
            optReturn.Data = lstMachines;
            return optReturn;
        }

        /// <summary>
        /// 主机接替备机时 更新资源表
        /// </summary>
        /// <param name="dbInfo">数据库连接信息</param>
        /// <param name="strRelpaceModuleNumber">要接替的主机key</param>
        /// <param name="strResourceKey">备机key</param>
        /// <returns></returns>
        public static OperationReturn UpdateReplaceModuleNumberInDB(DatabaseInfo dbInfo, string strRelpaceModuleNumber, string strResourceKey)
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
                        strSql = string.Format(strSql, "00000", strResourceKey,strRelpaceModuleNumber);
                        optReturn = MssqlOperation.GetDataSet(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "select * from t_11_101_{0} where C001 >2210000000000000000 and C001 <2220000000000000000 and C002 =1 and C014 in( {1},{2})";
                        strSql = string.Format(strSql, "00000", strResourceKey,strRelpaceModuleNumber);
                        optReturn = OracleOperation.GetDataSet(strConnectString, strSql);
                        break;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
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
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    return optReturn;
                }
                string strStandbyrole = ds.Tables[0].Rows[0]["C012"].ToString();
                if (strStandbyrole != "3")
                {
                    optReturn.Result = false;
                    return optReturn;
                }
                //更新备机的RelpaceModuleNumber
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "update t_11_101_{0} set C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strRelpaceModuleNumber, strBackupResID);
                        optReturn = MssqlOperation.ExecuteSql(strConnectString, strSql);
                        break;
                    case 3:
                        strSql = "UPDATE t_11_101_{0} SET C011 = '{1}' where C001 = {2} and C002 = 3";
                        strSql = string.Format(strSql, "00000", strRelpaceModuleNumber, strBackupResID);
                        optReturn = OracleOperation.ExecuteSql(strConnectString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
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
            }

            return optReturn;
        }

        public static string GetVoiceServerHostByModuleNumber(int strModuleNumber)
        {
            string strHost = string.Empty;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
                if (!dir.Exists)
                {
                    return strHost;
                }
                string strFileName = string.Format("umpparam_voc{0:0000}.xml", strModuleNumber);
                string strVoiceleFilePath = dir.FullName + "\\" + strFileName;
                FileInfo fileInfo = new FileInfo(strVoiceleFilePath);
                if (!fileInfo.Exists)
                {
                    return strHost;
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strVoiceleFilePath);
                XMLOperator xmlOperator = new XMLOperator(xmlDoc);
                XmlNode hostNode = xmlOperator.SelectNode("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer/HostAddress", "");
                strHost = xmlOperator.SelectAttrib(hostNode, "Value");
                string LStrVerificationCode101 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                strHost = EncryptionAndDecryption.EncryptDecryptString(strHost, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                string str = strHost;
            }
            catch (Exception ex)
            {
            }
            return strHost;
        }

        public static void StartBackupMachine(string strSourceKey, string strTargetKey)
        {
            DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
            if (!dir.Exists)
            {
                return;
            }
            FileInfo[] lstFileList = dir.GetFiles("*.xml");
            XmlDocument xmlDoc = null;
            XMLOperator xmlOperator = null;
            foreach (FileInfo file in lstFileList)
            {
                //如果不是参数的xml 跳过
                if (!file.Name.ToLower().StartsWith("umpparam_"))
                {
                    continue;
                }
                xmlDoc = new XmlDocument();
                xmlDoc.Load(file.FullName);
                xmlOperator = new XMLOperator(xmlDoc);
                #region 修改备机xml部分
                XmlNode node = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "ModuleNumber", strSourceKey);
                if (node != null)
                {
                    string strStandByRole = xmlOperator.SelectAttrib(node, "StandByRole");
                    if (!strStandByRole.Equals("3"))
                    {
                        break;
                    }
                    //查找这个属性 如果返回值为空 表示没有这个属性
                    string strAttrContent = xmlOperator.SelectAttrib(node, "ReplaceModuleNumber");
                    if (string.IsNullOrEmpty(strAttrContent))
                    {
                        //没有这个属性 则增加
                        xmlOperator.InsertAttrib(node, "ReplaceModuleNumber", strTargetKey);
                    }
                    else
                    {
                        bool bo = xmlOperator.UpdateAttrib(node, "ReplaceModuleNumber", strTargetKey);
                    }
                }
               
                #endregion

                #region 修改主机xml部分
                node = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "ModuleNumber", strTargetKey);
                if (node != null)
                {
                    string strStandByRole = xmlOperator.SelectAttrib(node, "StandByRole");
                    if (!strStandByRole.Equals("0"))
                    {
                        break;
                    }
                    //查找这个属性 如果返回值为空 表示没有这个属性
                    string strAttrContent = xmlOperator.SelectAttrib(node, "ReplaceModuleNumber");
                    if (string.IsNullOrEmpty(strAttrContent))
                    {
                        //没有这个属性 则增加
                        xmlOperator.InsertAttrib(node, "ReplaceModuleNumber", strSourceKey);
                    }
                    else
                    {
                        bool bo = xmlOperator.UpdateAttrib(node, "ReplaceModuleNumber", strSourceKey);
                    }
                }
                
                #endregion

                xmlOperator.Save(file.FullName);
            }
        }

        private string GetComputerServiceStatus(string AStrServiceName)
        {
            string LStrSatus = string.Empty;

            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_Service WHERE Name = '" + AStrServiceName + "'");
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    try
                    {
                        if (ObjectSingleReturn["Started"].Equals(true))
                            LStrSatus = "1";
                        else
                            LStrSatus = "0";
                        break;
                    }
                    catch (Exception ex)
                    {
                        LStrSatus = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                LStrSatus = string.Empty;
            }
            return LStrSatus;
        }

        private ManagementObjectCollection GetServiceCollection(string AStrQuery)
        {
            ConnectionOptions ComputerConnect = new ConnectionOptions();

            ManagementScope ComputerManagement = new ManagementScope("\\\\localhost\\root\\cimv2", ComputerConnect);
            ComputerConnect.Timeout = new TimeSpan(0, 10, 0);
            ComputerManagement.Connect();
            ObjectQuery VoiceServiceQuery = new ObjectQuery(AStrQuery);
            ManagementObjectSearcher ObjectSearcher = new ManagementObjectSearcher(ComputerManagement, VoiceServiceQuery);
            ManagementObjectCollection ReturnCollection = ObjectSearcher.Get();
            return ReturnCollection;
        }

        private void GetLicense()
        {
            string strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\";
            LicenseServerOperation licOperation = new LicenseServerOperation(strLicXmlPath);
            List<LicenseServer> lstLicenseServers = licOperation.GetLicenseServers();
            TcpClient clientToLicServer = null;
            SslStream sslStreamToLicServer = null;
            bool bIsConneted = false;
            int iConnResult = 0;

            foreach (LicenseServer licServer in lstLicenseServers)
            {
                iConnResult = licOperation.ConnectToLicenseServer(licServer.Host, licServer.Port, ref sslStreamToLicServer, ref clientToLicServer);
                //如果返回值为0 则跳出循环
                if (iConnResult == 0)
                {
                    bIsConneted = true;
                    break;
                }
                //如果返回值不为0 则关掉sslstream和client
                if (sslStreamToLicServer != null)
                {
                    if (sslStreamToLicServer.CanRead)
                    {
                        sslStreamToLicServer.Close();
                    }
                }
                if (clientToLicServer != null)
                {
                    if (clientToLicServer.Connected)
                    {
                        clientToLicServer.Close();
                    }
                }
              
            }
            if (!bIsConneted)
            {
                return;
            }

            //string strResult = licOperation.GetLicenseInfo(sslStreamToLicServer, clientToLicServer);
            try
            {
                string strasdf = licOperation.GetLicenseInfo(sslStreamToLicServer, clientToLicServer);
                MessageBox.Show(strasdf);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            string LStrVerificationCode004 = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (RBG001.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG002.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G002", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG003.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G003", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG004.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G004", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("C:\\", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBR001.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("R001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("192.168.6.49:8009", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("192.168.6.69:8009", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG006.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G006", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("192.168.4.205", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG007.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G007", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString(@"C:\ProgramData\VoiceCyber\UMP\config", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBG008.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G008", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBC001.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("C001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("192.168.9.238", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("3070", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBN001.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("N001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("192.168.9.136", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else if (RBN004.IsChecked == true)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("N004", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("0:1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage += AscCodeToChr(27);
                    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("1:0", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }

                LTcpClient = new TcpClient("192.168.7.102", 8009);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    TextBoxResult.Text = LStrReadMessage;
                }
                else
                {
                    MessageBox.Show(LStrReadMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
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
                    if (LStringBuilderData.ToString().IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27)) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27));
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

        private void ClientMessageG003(string[] AStrArrayInfo)
        {
            try
            {
                ManagementObjectCollection ObjectCollection;
                //ObjectCollection = GetServiceCollection("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE (MACAddress IS NOT NULL) AND IPEnabled = true");
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    try
                    {

                        //SendMessageToClient(ObjectSingleReturn["Description"].ToString() + Common.AscCodeToChr(27) + ObjectSingleReturn["SettingID"].ToString());
                    }
                    catch { }
                }
               // SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
               // SendMessageToClient("ErrorG003");
                //SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
               // UMPService00.IEventLog.WriteEntry("ClientMessageG003()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString(), EventLogEntryType.Error);
            }
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
    }
    public struct _TAG_NETPACK_DISTINGUISHHEAD
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] _flag;			// 包头识别标识
        public byte _version;			// 版本
        public byte _cbsize;			// 头大小
    }

    public struct _TAG_NETPACK_DISTINGUISHHEAD_VER1
    {
        // 0-3, 4 bytes
        public _TAG_NETPACK_DISTINGUISHHEAD _dist;				// 识别头
        // 4-5, 2 bytes
        public byte _sequence;			            // 序号,0-255循环，保证数据包顺序(UDP时使用,或者非点对点时使用)
        public byte _packtype;			            // 包类型，区分通讯协议还是应用层消息包
        // 6-7, 2 bytes
        public ushort _followsize;		            // 本完整数据包除去包识别头后续数据大小
        // 8-15, 8 bytes
        public Int64 _source;			            // 发送源(global id)
        // 16-23, 8 bytes
        public Int64 _target;			            // 接收者(global id)，如果是0xffffffffffffffff则为广播包
        // 24-31, 8 bytes
        public Int64 _timestamp;			        // 时间戳，UTC时间(高精度的time_t，精度为100ns)，从1970.1.1 0:00:00
        // 32-35, 4 bytes
        public ushort _basehead;			        // 基本头类型标志, 0-无
        public ushort _basesize;			        // 基本头大小
        // 36-39, 4 bytes
        public ushort _exthead;			            // 扩展头类型标志，0-无
        public ushort _extsize;			            // 扩展头大小
        // 40-43, 4 bytes
        public ushort _datasize;                    //数据区大小
        public ushort _state;				        // 状态标志，不使用保持0
        // 44-47, 4 bytes
        public ushort _moduleid;			        // 模块ID
        public ushort _number;			            // 模块编号
        // 48-63, 16 bytes
        public _TAG_NETPACK_MESSAGE _message;			            // 消息id，可用于消息订阅，通常情况_packtype应为NETPACK_PACKTYPE_USER
    }

    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public struct _TAG_NETPACK_MESSAGE
    {
        [FieldOffset(0)]
        public Int64 _adressLong;
        [FieldOffset(0)]
        public Adress _adress;
        [FieldOffset(8)]
        public Int64 _identifyLong;
        [FieldOffset(8)]
        public identify identify;
    }

    public struct Adress
    {
        public MesTarget target;
        public MesSource source;
    }

    public struct identify
    {
        public ushort _number;	                    // 消息编号
        public ushort _small;	                    // 消息小类型
        public ushort _middle;	                    // 消息中类型
        public ushort _large;                        //消息大类型
    }

    public struct MesTarget
    {
        public short _number;	                    // 目标编号。不确定编号，则使用0xffff填充
        public short _module;	                    // 目标模块
    };

    public struct MesSource
    {
        public short _number;	                    // 源编号
        public short _module;	                    // 源模块
    };


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
