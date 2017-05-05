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
using UMPS1106.WCFService00000;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;

namespace UMPS1106
{
    class S1106App : UMPApp
    {
        #region 所有信息全部保存在 SessionInfo 中
        public static SessionInfo GClassSessionInfo = null;

        #endregion

        #region Url请求参数
        private static string GStrQueryString = "UMPS1106";
        #endregion

        public static string GStrCurrentOperation = string.Empty;

        public static MainView00000A IPageMainOpend = null;

        #region 本模块使用的语言包
        private static DataTable IDataTableLanguage = null;
        #endregion

        #region 本模块使用的安全策略参数
        public static DataTable GDataTable11001 = null;
        public static DataTable GDataTable00003 = null;
        #endregion


        /// <summary>
        /// S1106的子模块  这个对应的是t_11_003表里面 当C009为‘UMPS1106.xbap’的 C010字段 
        /// </summary>
        public enum S1106Module
        {
            PasswordPolicy = 1106,
            InteractiveLogin = 1107,
            LockingStrategy = 1108,
            GlobalParameter = 1109
        }

        //当前加载的子模块
        public static S1106Module CurrentLoadingModule;

        public S1106App(bool runAsModule)
            : base(runAsModule)//base这里的作用是复用父类的构造函数【把子类的构造函数的参数传递给父类处理】
        {

        }

        public S1106App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1106";
            AppTitle = string.Format("BasicModel");
            ModuleID = 1106;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.37";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV832";
            //dbInfo.Password = "pfdev832";
            //dbInfo.RealPassword = "pfdev832";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //StartArgs = "1107";
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB12011";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            dbInfo.RealPassword = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
            StartArgs = "1106";
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new MainView00000A();
            CurrentView.PageName = "MainView00000A";
        }


        //加载所有的参数的内容（从T_11_001里面获取）
        public  void LoadSecurityPlicy()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                GDataTable11001 = new DataTable();

                LListStrWcfArgs.Add(Session.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(Session.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(Session.RentInfo.Token);
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(17, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    GDataTable11001 = LWCFOperationReturn.DataSetReturn.Tables[0];
                    GDataTable00003 = LWCFOperationReturn.ListDataSetReturn[0].Tables[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        /// <summary>
        /// 加载本应用程序需要使用的资源字典
        /// </summary>
        public  void LoadStyleDictionary()
        {
            string LStrLocalResourcePath = string.Empty;

            try
            {
                LStrLocalResourcePath = System.IO.Path.Combine(Session.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes\\Style01", "Style1106.xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrLocalResourcePath, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
            }
            catch
            { }
        }

        public static string GetDisplayCharater(string AStrMessageID)
        {
            string LStrReturn = string.Empty;
            string LStrC005 = string.Empty;
            string LStrC006 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C002 = '" + AStrMessageID + "'");
                if (ObjectLanguageRow.Count() == 0) { return LStrReturn; }
                LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                LStrReturn = LStrC005 + LStrC006;
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetDisplayCharater(string AStrObjectName, string AStrTargetName)
        {
            string LStrReturn = string.Empty;
            string LStrC005 = string.Empty;
            string LStrC006 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C011 = '" + AStrObjectName + "' AND C012 Like '" + AStrTargetName + "%'");
                LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                LStrReturn = LStrC005 + LStrC006;
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetLangIDByPageAndName(string AStrObjectName, string AStrTargetName)
        {
            string strReturn = string.Empty;
            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C011 = '" + AStrObjectName + "' AND C012 Like '" + AStrTargetName + "%'");
                string strLangID = ObjectLanguageRow[0]["C002"].ToString();
                strReturn = strLangID;
            }
            catch { }
            return strReturn;
        }

        public static string GetDisplayCharater(string AStrObjectName, string AStrTargetName,string strDefault)
        {
            string LStrReturn = string.Empty;
            string LStrC005 = string.Empty;
            string LStrC006 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C011 = '" + AStrObjectName + "' AND C012 Like '" + AStrTargetName + "%'");
                if (ObjectLanguageRow.Length <= 0)
                {
                    LStrReturn = strDefault;
                }
                else
                {
                    LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                    LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                    if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                    if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                    LStrReturn = LStrC005 + LStrC006;
                }
            }
            catch { LStrReturn = strDefault; }

            return LStrReturn;
        }

        /// <summary>
        /// 创建分割符
        /// </summary>
        /// <param name="AsciiCode">Ascii码</param>
        /// <returns></returns>
        public static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);

            return (strCharacter);
        }



        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();

            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTableLanguage = new DataTable();

                LListStrWcfArgs.Add(Session.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(Session.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(Session.LangTypeInfo.LangID.ToString());
                LListStrWcfArgs.Add("M22");
                LListStrWcfArgs.Add("11");
                LListStrWcfArgs.Add("1106");
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(8, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTableLanguage = LWCFOperationReturn.DataSetReturn.Tables[0];
                }

            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }




        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        public static string DecryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        public static string DecryptString001(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
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


        #region 获取参数可配置的最小值、最大值、默认值（正整型数据）
        public static bool GetParameterMinMaxDefValue(string AStrParameterID, ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            bool LBoolReturn = true;

            try
            {
                if (AStrParameterID == "11010102") { Get11010102MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11010201") { Get11010201MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11010202") { Get11010202MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11010203") { Get11010203MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11010301") { Get11010301MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11010302") { Get11010302MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11020201") { Get11020201MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11020202") { Get11020202MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11020302") { Get11020302MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11030101") { Get11030101MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11030102") { Get11030102MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11030103") { Get11030103MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
                if (AStrParameterID == "11020501") { Get11020501MinMaxDefValue(ref AIntMinValue, ref AIntMaxValue, ref AIntDefValue); return LBoolReturn; }
            }
            catch { LBoolReturn = false; }

            return LBoolReturn;
        }

        private static void Get11020501MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            AIntMinValue = 0; AIntMaxValue = 999999; AIntDefValue = 0;
        }

        private static void Get11010102MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {

            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11010101");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);
            if (LStrParameterValueDB == "0")
            {
                AIntMinValue = 0; AIntMaxValue = 32; AIntDefValue = 6;
            }
            else
            {
                AIntMinValue = 6; AIntMaxValue = 32; AIntDefValue = 6;
            }
        }

        private static void Get11010201MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11010202");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            if (LStrParameterValueDB == "0")
            {
                AIntMinValue = 1;
                AIntMaxValue = 998;
                AIntDefValue = 1;
            }
            else
            {
                AIntMinValue = 0;
                AIntMaxValue = int.Parse(LStrParameterValueDB)-1;
                AIntDefValue = 0;
            }
        }

        private static void Get11010202MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11010201");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            if (LStrParameterValueDB == "0")
            {
                AIntMinValue = 1;
                AIntMaxValue = 999;
                AIntDefValue = 60;
            }
            else
            {
                AIntMinValue = int.Parse(LStrParameterValueDB) + 1;
                AIntMaxValue = 999;
                AIntDefValue = 60;
                if (AIntMinValue > AIntDefValue) { AIntDefValue = AIntMinValue; }
            }
        }

        private static void Get11010203MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11010202");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            AIntMinValue = 1;
            if (LStrParameterValueDB == "0")
            {
                AIntMaxValue = 998;
                AIntDefValue = 7;
            }
            else
            {
                AIntMaxValue = int.Parse(LStrParameterValueDB) - 1;
                AIntDefValue = AIntMaxValue;
                if (AIntMaxValue > 7) { AIntDefValue = 7; }
            }
        }

        private static void Get11010301MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            AIntMinValue = 0; AIntMaxValue = 24; AIntDefValue = 24;
        }

        private static void Get11010302MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            AIntMinValue = 0; AIntMaxValue = 999; AIntDefValue = 15;
        }

        private static void Get11020201MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            AIntMinValue = 1; AIntMaxValue = 16; AIntDefValue = 1;
        }

        private static void Get11020202MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            AIntMinValue = 1; AIntMaxValue = 60; AIntDefValue = 20;
        }

        private static void Get11020302MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11030101");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            if (LStrParameterValueDB == "0")
            {
                AIntMinValue = 1; AIntMaxValue = 10; AIntDefValue = 5;
            }
            else
            {
                AIntMinValue = int.Parse(LStrParameterValueDB); AIntMaxValue = AIntMinValue * 2; AIntDefValue = AIntMinValue;
            }
        }

        private static void Get11030101MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11020302");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            if (LStrParameterValueDB == "0")
            {
                AIntMinValue = 1; AIntMaxValue = 10; AIntDefValue = 3;
            }
            else
            {
                AIntMinValue = 1; AIntMaxValue = int.Parse(LStrParameterValueDB); AIntDefValue = AIntMaxValue;
            }
        }

        private static void Get11030102MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11030103");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            AIntMinValue = int.Parse(LStrParameterValueDB); AIntMaxValue = 1440; AIntDefValue = AIntMinValue;
        }

        private static void Get11030103MinMaxDefValue(ref int AIntMinValue, ref int AIntMaxValue, ref int AIntDefValue)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            DataRow[] LDataRow11010101 = GDataTable11001.Select("C003 = 11030102");
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            LStrParameterValueDB = LDataRow11010101[0]["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueDB = LStrParameterValueDB.Substring(8);

            AIntMinValue = 1; AIntMaxValue = int.Parse(LStrParameterValueDB); AIntDefValue = AIntMaxValue;
        }
        #endregion
       
    }
}
