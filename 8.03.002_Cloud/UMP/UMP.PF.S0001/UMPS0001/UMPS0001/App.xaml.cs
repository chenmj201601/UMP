using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using UMPS0001.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS0001
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
        private static string GStrQueryString = "UMPS0001";
        #endregion

        #region 本模块使用的语言包
        private static DataTable IDataTableLanguage = null;
        #endregion

        #region 系统默认分割符
        public static string GStrSpliterChar = string.Empty;
        #endregion

        public static string GStrCurrentOperation = string.Empty;

        public static Page000001A IPageMainOpend = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GStrSpliterChar = AscCodeToChr(27);
            GClassSessionInfo = new SessionInfo();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                NameValueCollection LNameValueCollection = new NameValueCollection();
                LNameValueCollection = HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                GStrQueryString = LNameValueCollection.ToString();
            }
            GClassSessionInfo.SessionID = GStrQueryString;
            GClassSessionInfo.AppName = "UMPS0001";
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

        #region 处理主框架发送过来的消息
        private static WebReturn GNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn LWebReturn = new WebReturn();

            //try
            //{
            //    LIntReciveCode = arg.Code;
            //    switch (LIntReciveCode)
            //    {
            //        case (int)RequestCode.CSModuleClose:
            //            //CloseThisApplication();
            //            break;
            //        case 22101:
            //            DealMainSendedMessage(arg.Data);
            //            break;
            //        case (int)RequestCode.SCLanguageChange:
            //            ChangeElementLanguage(arg.Data);
            //            break;
            //        case (int)RequestCode.SCThemeChange:
            //            App.GClassSessionInfo.ThemeInfo.Name = arg.Data;
            //            LoadStyleDictionary();
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LWebReturn.Result = false;
            //    LWebReturn.Code = Defines.RET_FAIL;
            //    LWebReturn.Message = ex.ToString();
            //}

            return LWebReturn;
        }

        private static void DealMainSendedMessage(string AStrData)
        {
            //WebRequest LWebRequestClientLoaded = new WebRequest();
            //LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
            //LWebRequestClientLoaded.Session = App.GClassSessionInfo;
            //LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
            //IPageMainOpend.DoingMainSendMessage(AStrData);
            //SendNetPipeMessage(LWebRequestClientLoaded);
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
                LStrLocalResourcePath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, "Style0001.xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrLocalResourcePath, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "LoadStyleDictionary"); }
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
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                IDataTableLanguage = new DataTable();
                LListStrWcfArgs.Add(GClassSessionInfo.LangTypeInfo.LangID.ToString());
                LWCFOperationReturn = LService00000Client.OperationMethodA(33, LListStrWcfArgs);
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

            try
            {
                DataRow[] LDataRowObjectLanguages = IDataTableLanguage.Select("C001 = '" + AStrMessageID + "'");
                if (LDataRowObjectLanguages.Length <= 0) { return LStrReturn; }
                LStrReturn = LDataRowObjectLanguages[0]["C002"].ToString();
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetDisplayCharater(string AStrObjectName, string AStrTargetName)
        {
            string LStrReturn = string.Empty;
            string LStrC002 = string.Empty;
            string LStrC003 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C004 = '" + AStrObjectName + "' AND C005 = '" + AStrTargetName + "'");
                LStrC002 = ObjectLanguageRow[0]["C002"].ToString();
                LStrC003 = ObjectLanguageRow[0]["C003"].ToString();
                if (string.IsNullOrEmpty(LStrC002)) { LStrC002 = ""; }
                if (string.IsNullOrEmpty(LStrC003)) { LStrC003 = ""; }
                LStrReturn = LStrC002 + LStrC003;
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
