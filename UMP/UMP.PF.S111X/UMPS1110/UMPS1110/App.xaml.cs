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
using UMPS1110.WCFService00000;
using UMPS1110.WCFService11111;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS1110
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
        private static string GStrQueryString = "UMPS1110";
        #endregion

        #region 本模块使用的语言包
        private static DataTable IDataTableLanguage = null;
        #endregion

        #region 本模块权限包
        public static DataTable IDataTableOperation = null;
        #endregion

        #region 所有资源列表
        public static DataTable IDataTable00009 = null;
        public static DataTable IDataTable00010 = null;
        public static List<DataSet> IListDataSetReturn = null;
        #endregion

        public static string GStrCurrentOperation = string.Empty;

        public static Page11100A IPageMainOpend = null;

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
            GClassSessionInfo.AppName = "UMPS1110";
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

        #region 加载与资源管理相关的初始化数据
        public static void LoadResourceManagementData()
        {
            Service11111Client LService11111Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTable00009 = new DataTable();
                IDataTable00010 = new DataTable();
                IListDataSetReturn = new List<DataSet>();
                LBasicHttpBinding = WebHelper.CreateBasicHttpBingding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service11111");
                OperationDataArgs11111 LWCFOperationReturn = new OperationDataArgs11111();
                LService11111Client = new Service11111Client(LBasicHttpBinding, LEndpointAddress);

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.RentInfo.Token);
                LWCFOperationReturn = LService11111Client.OperationMethodA(1, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTable00010 = LWCFOperationReturn.DataSetReturn.Tables[0];
                }
                LoadResourcePropertiesDifine();
                LoadResourceType("211");             //0    License Server
                LoadResourceType("212");             //1    Dec Server
                LoadResourceType("213");             //2    CTI Hub Server
            }
            catch { }
            finally
            {
                if (LService11111Client != null)
                {
                    if (LService11111Client.State == CommunicationState.Opened) { LService11111Client.Close(); }
                }
            }
        }

        private static void LoadResourcePropertiesDifine()
        {
            Service11111Client LService11111Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                LBasicHttpBinding = WebHelper.CreateBasicHttpBingding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service11111");
                OperationDataArgs11111 LWCFOperationReturn = new OperationDataArgs11111();
                LService11111Client = new Service11111Client(LBasicHttpBinding, LEndpointAddress);

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LWCFOperationReturn = LService11111Client.OperationMethodA(0, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTable00009 = LWCFOperationReturn.DataSetReturn.Tables[0];
                }
                else { IDataTable00009 = null; }
            }
            catch { }
            finally
            {
                if (LService11111Client != null)
                {
                    if (LService11111Client.State == CommunicationState.Opened) { LService11111Client.Close(); }
                }
            }
        }

        private static void LoadResourceType(string AStrTypeID)
        {
            Service11111Client LService11111Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                LBasicHttpBinding = WebHelper.CreateBasicHttpBingding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service11111");
                OperationDataArgs11111 LWCFOperationReturn = new OperationDataArgs11111();
                LService11111Client = new Service11111Client(LBasicHttpBinding, LEndpointAddress);

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.RentInfo.Token);
                LListStrWcfArgs.Add(AStrTypeID);
                LWCFOperationReturn = LService11111Client.OperationMethodA(2, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IListDataSetReturn.Add(LWCFOperationReturn.DataSetReturn);
                }
                else { IListDataSetReturn.Add(null); }
            }
            catch { }
            finally
            {
                if (LService11111Client != null)
                {
                    if (LService11111Client.State == CommunicationState.Opened) { LService11111Client.Close(); }
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
                    LWebRequestClient.Code = (int)RequestCode.CSHome;
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
                LStrLocalResourcePath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, "Style1110.xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrLocalResourcePath, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "LoadStyleDictionary"); }
        }

        /// <summary>
        /// 加载本应用程序的权限列表
        /// </summary>
        public static void LoadThidModuleOperation()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTableOperation = new DataTable();

                LBasicHttpBinding = WebHelper.CreateBasicHttpBingding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.RentInfo.Token);
                LListStrWcfArgs.Add(GClassSessionInfo.UserInfo.UserID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.RoleInfo.ID.ToString());
                LListStrWcfArgs.Add("11");
                LWCFOperationReturn = LService00000Client.OperationMethodA(16, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTableOperation = LWCFOperationReturn.DataSetReturn.Tables[0];
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
                LBasicHttpBinding = WebHelper.CreateBasicHttpBingding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                IDataTableLanguage = new DataTable();
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.LangTypeInfo.LangID.ToString());
                LListStrWcfArgs.Add("M22");
                LListStrWcfArgs.Add("11");
                LListStrWcfArgs.Add("1110");
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
