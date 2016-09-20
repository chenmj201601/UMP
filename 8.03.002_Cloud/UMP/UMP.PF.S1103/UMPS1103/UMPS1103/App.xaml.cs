using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using UMPS1103.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS1103
{
    public partial class App : Application
    {
        #region 与主框架通讯用对象
        private static NetPipeHelper GNetPipeHelper;
        #endregion

        #region 所有信息全部保存在 SessionInfo 中
        public static SessionInfo GClassSessionInfo = null;
        #endregion

        #region Url请求参数
        private static string GStrQueryString = "UMPS1100";
        #endregion

        #region 本模块使用的语言包
        private static DataTable IDataTableLanguage = null;
        #endregion

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

        public static string GStrCurrentOperation = string.Empty;

        public static PageMainEntrance IPageMainOpend = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GClassSessionInfo = new SessionInfo();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                NameValueCollection LNameValueCollection = new NameValueCollection();
                LNameValueCollection = HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                GStrQueryString = LNameValueCollection.ToString();
            }
            GClassSessionInfo.SessionID = GStrQueryString;
            GClassSessionInfo.AppName = "UMPS1103";
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                SendCloseAppplicationMessage();
                if (GNetPipeHelper != null) { GNetPipeHelper.Stop(); GNetPipeHelper = null; }
                base.OnExit(e);
            }
            catch { }
        }

        private static void CreateNetPipeClient()
        {
            try
            {
                GNetPipeHelper = new NetPipeHelper(false, GClassSessionInfo.SessionID);
                GNetPipeHelper.DealMessageFunc += GNetPipeHelper_DealMessageFunc;
                GNetPipeHelper.Start();
            }
            catch (Exception ex)
            {
                if (GNetPipeHelper != null) { GNetPipeHelper.Stop(); }
                MessageBox.Show(ex.ToString());
            }
        }

        #region 获取与座席相关的数据
        public static void LoadAboutAgentData()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode104 = string.Empty;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                IDataTable11005 = new DataTable();
                IDataTable11006 = new DataTable();
                IDataTable11101 = new DataTable();
                IDataTable11009 = new DataTable();
                IDataTable11201SA = new DataTable();
                IDataTable11201UA = new DataTable();
                IDataTable11201UO = new DataTable();
                IDataTable11201UU = new DataTable();

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.RentInfo.Token);
                LListStrWcfArgs.Add(GClassSessionInfo.UserInfo.UserID.ToString());
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
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

        #region 处理主框架发送过来的消息
        private static WebReturn GNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn LWebReturn = new WebReturn();
            int LIntReciveCode = 0;

            try
            {
                LIntReciveCode = arg.Code;
                switch (LIntReciveCode)
                {
                    case (int)RequestCode.CSModuleClose:
                        //CloseThisApplication();
                        break;
                    case 22101:
                        DealMainSendedMessage(arg.Data);
                        break;
                    case (int)RequestCode.SCLanguageChange:
                        ChangeElementLanguage(arg.Data);
                        break;
                    case (int)RequestCode.SCThemeChange:
                        App.GClassSessionInfo.ThemeInfo.Name = arg.Data;
                        LoadStyleDictionary();
                        break;
                    case 91002:
                        IPageMainOpend.StartStopTimer(false);
                        break;
                    case 91003:
                        IPageMainOpend.StartStopTimer(true);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LWebReturn.Result = false;
                LWebReturn.Code = Defines.RET_FAIL;
                LWebReturn.Message = ex.ToString();
            }

            return LWebReturn;
        }

        private static void DealMainSendedMessage(string AStrData)
        {
            WebRequest LWebRequestClientLoaded = new WebRequest();
            LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
            LWebRequestClientLoaded.Session = App.GClassSessionInfo;
            LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
            IPageMainOpend.DoingMainSendMessage(AStrData);
            SendNetPipeMessage(LWebRequestClientLoaded);
        }
        #endregion

        #region 主框架发送改变语言的消息进行的语言切换处理

        private static BackgroundWorker IBWReloadLanguage = null;
        private static void ChangeElementLanguage(string AStrLanguageID)
        {
            try
            {
                GClassSessionInfo.LangTypeInfo.LangID = int.Parse(AStrLanguageID);

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBWReloadLanguage = new BackgroundWorker();
                IBWReloadLanguage.RunWorkerCompleted += IBWReloadLanguage_RunWorkerCompleted;
                IBWReloadLanguage.DoWork += IBWReloadLanguage_DoWork;
                IBWReloadLanguage.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWReloadLanguage != null)
                {
                    IBWReloadLanguage.Dispose(); IBWReloadLanguage = null;
                }
                MessageBox.Show(ex.ToString());
            }
        }

        private static void IBWReloadLanguage_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadApplicationLanguages();
        }

        private static void IBWReloadLanguage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IPageMainOpend.ApplicationLanguageChanged();
            WebRequest LWebRequestClientLoading = new WebRequest();
            LWebRequestClientLoading.Code = 12112;
            WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
        }
        #endregion

        public static WebReturn SendNetPipeMessage(WebRequest AWebRequest)
        {
            WebReturn LWebReturn = new WebReturn();

            try
            {
                if (GNetPipeHelper == null) { CreateNetPipeClient(); }
                LWebReturn.Result = true;
                LWebReturn.Code = 0;

                if (GNetPipeHelper == null)
                {
                    LWebReturn.Result = false;
                    LWebReturn.Code = Defines.RET_OBJECT_NULL;
                    LWebReturn.Message = string.Format("NetPipe Service is null");
                    return LWebReturn;
                }
                WebReturn LWebReturnSend = GNetPipeHelper.SendMessage(AWebRequest, string.Empty);
                return LWebReturnSend;
            }
            catch (Exception ex)
            {
                LWebReturn.Result = false;
                LWebReturn.Code = Defines.RET_FAIL;
                LWebReturn.Message = ex.Message;
                return LWebReturn;
            }
        }

        public static void SendCloseAppplicationMessage()
        {
            WebReturn LWebReturn = new WebReturn();
            try
            {
                if (GNetPipeHelper != null)
                {
                    WebRequest LWebRequestClient = new WebRequest();
                    LWebRequestClient.Code = (int)RequestCode.CSModuleClose;
                    LWebRequestClient.Session = new SessionInfo();
                    LWebRequestClient.Session.SessionID = App.GClassSessionInfo.SessionID;
                    LWebReturn = SendNetPipeMessage(LWebRequestClient);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SendCloseAppplicationMessage");
            }
        }

        /// <summary>
        /// 加载本应用程序需要使用的资源字典
        /// </summary>
        public static void LoadStyleDictionary()
        {
            string LStrLocalResourcePath = string.Empty;

            try
            {
                LStrLocalResourcePath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, "Style1103.xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrLocalResourcePath, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
            }
            catch { }
        }

        /// <summary>
        /// 加载语言包
        /// </summary>
        public static void LoadApplicationLanguages()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTableLanguage = new DataTable();

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.LangTypeInfo.LangID.ToString());
                LListStrWcfArgs.Add("M22");
                LListStrWcfArgs.Add("11");
                LListStrWcfArgs.Add("1103");
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
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
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C011 = '" + AStrObjectName + "' AND C012 = '" + AStrTargetName + "'");
                LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                LStrReturn = LStrC005 + LStrC006;
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
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
    }
}
