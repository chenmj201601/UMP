using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using UMPS1103.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS1103
{
    public class S1103App : UMPApp
    {
        #region 本模块相关的数据
        public static DataTable IDataTable11005 = null;
        public static DataTable IDataTable11006 = null;
        public static DataTable IDataTable11101 = null;
        public static DataTable IDataTable11009 = null;
        public static DataTable IDataTable11201SA = null;
        public static DataTable IDataTable11201UA = null;
        public static DataTable IDataTable11201UO = null;
        public static DataTable IDataTable11201UU = null;
        #endregion

        public S1103App(bool runAsModule)
            : base(runAsModule)
        {
        }

        public S1103App(IRegionManager regionManager,
         IEventAggregator eventAggregator,
         IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new UCAgentMaintenance();
            CurrentView.PageName = "UCAgentMaintenance";
        }

        #region Init and Load

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1103";
            ModuleID = 1103;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;
            //Session.RoleID = ConstValue.RESOURCE_USER;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "a";
            userInfo.UserName = "a";
            userInfo.Password = "a";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;


            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.7.101";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.7.101";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0509";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "voicecodes";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.9.236";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB20160427";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();


            //分表之类的参数
            //Session.ListPartitionTables.Clear();
            //PartitionTableInfo partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            //Session.ListPartitionTables.Add(partInfo);
        }

        protected override void Init()
        {
            base.Init();
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
            LoadAboutAgentData();
        }

        #endregion

        #region Language
        protected override void InitLanguageInfos()
        {
            try
            {
                base.InitLanguageInfos();
                if (Session == null || Session.LangTypeInfo == null) { return; }
                //ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1103");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfos.Add(langInfo);
                }

                WriteLog(string.Format("AppStart\t\tLanguage loaded"));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
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


        #region 获取与座席相关的数据
        public void LoadAboutAgentData()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode104 = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                IDataTable11005 = new DataTable();
                IDataTable11006 = new DataTable();
                IDataTable11101 = new DataTable();
                IDataTable11009 = new DataTable();
                IDataTable11201SA = new DataTable();
                IDataTable11201UA = new DataTable();
                IDataTable11201UO = new DataTable();
                IDataTable11201UU = new DataTable();

                LListStrWcfArgs.Add(Session.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(Session.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(Session.RentInfo.Token);
                LListStrWcfArgs.Add(Session.UserInfo.UserID.ToString());
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(26, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTable11005 = LWCFOperationReturn.ListDataSetReturn[0].Tables[0];
                    IDataTable11006 = LWCFOperationReturn.ListDataSetReturn[1].Tables[0];
                    IDataTable11101 = LWCFOperationReturn.ListDataSetReturn[2].Tables[0];
                    IDataTable11009 = LWCFOperationReturn.ListDataSetReturn[3].Tables[0];
                    IDataTable11201SA = LWCFOperationReturn.ListDataSetReturn[4].Tables[0];
                    IDataTable11201UA = LWCFOperationReturn.ListDataSetReturn[5].Tables[0];
                    IDataTable11201UO = LWCFOperationReturn.ListDataSetReturn[6].Tables[0];
                    IDataTable11201UU = LWCFOperationReturn.ListDataSetReturn[7].Tables[0];

                    foreach (DataRow LDataRowSingleAgent in IDataTable11101.Rows)
                    {
                        if (LDataRowSingleAgent["C002"].ToString() != "1") { continue; }
                        LDataRowSingleAgent["C017"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleAgent["C017"].ToString(), LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }
        #endregion
    }
}
