using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Threading;
using Common3601;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using UMPS3601.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS3601
{
    public struct QuestionInfo
    {
        public CQuestionsParam QuestionsParam;
        public S3601Codes OptType;
    }

    public struct PaperInfo
    {
        public CPaperParam PaperParam;
        public S3601Codes OptType;
    }

    public class S3601App : UMPApp
    {
        public S3601App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3601App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        #region Members
        public static CCategoryTree GCategoryTree { get; set; }
        public static bool IsModifyScoreSheet;
        public static S3601Codes GMessageSource = new S3601Codes();
        /// <summary>
        /// 查询设置  False 新建 | True 修改
        /// </summary>
        public static bool GQueryModify = false;
        public static QuestionInfo GQuestionInfo = new QuestionInfo();
        public static List<CQuestionsParam> GListQuestionInfos = new List<CQuestionsParam>();
        public static PaperInfo GPaperInfo = new PaperInfo();
        public static List<CPaperParam> GLisPaperParam = new List<CPaperParam>();
        public static List<CPaperQuestionParam> GlistPaperQuestionParam = new List<CPaperQuestionParam>();
        public static int GIntUsableScore = 0;
        public static List<long> GLstCategoryNum = new List<long>();
        public static CCategoryTree GCategoryTreeNode = new CCategoryTree();
        public static  List<CCategoryTree> GLstCCategoryTrees = new List<CCategoryTree>();
        public static int GPageSize = 0;
        #endregion

        #region Init & load Info

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3601";
            AppTitle = string.Format("Question");
            ModuleID = 3601;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "administrator";
            userInfo.Password = "qwe,123";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.69";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;
 
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB_SEF_1";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "PF,123";
            dbInfo.RealPassword = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

//             AppServerInfo serverInfo = new AppServerInfo();
//             serverInfo.Protocol = "http";
//             serverInfo.Address = "192.168.9.223";
//             serverInfo.Port = 8081;
//             serverInfo.SupportHttps = false;
//             Session.AppServerInfo = serverInfo;

//             DatabaseInfo dbInfo = new DatabaseInfo();
//             dbInfo.TypeID = 2;
//             dbInfo.Host = "192.168.9.223";
//             dbInfo.Port = 1433;
//             dbInfo.DBName = "UMPDataDB0711";
//             dbInfo.LoginName = "sa";
//             dbInfo.Password = "voicecodes";
//             Session.DatabaseInfo = dbInfo;
//             Session.DBType = dbInfo.TypeID;
//             Session.DBConnectionString = dbInfo.GetConnectionString();

//             DatabaseInfo dbInfo = new DatabaseInfo();
//             dbInfo.TypeID = 3;
//             dbInfo.TypeName = "ORCL";
//             dbInfo.Host = "192.168.4.182";
//             dbInfo.Port = 1521;
//             dbInfo.DBName = "PFOrcl";
//             dbInfo.LoginName = "PFDEV831";
//             dbInfo.Password = "pfdev831";
//             Session.DatabaseInfo = dbInfo;
//             Session.DBType = dbInfo.TypeID;
//             Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void Init()
        {
            base.Init();

            try
            {
//                 if (Session != null)
//                 {
//                     WriteLog("AppLoad", string.Format("{0}", Session.LogInfo()));
//                 }
// 
//                 if (RunAsModule)
//                 {
//                     ExamProductionView view = new ExamProductionView();
//                     view.CurrentApp = Current;
//                     var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
//                     if (app != null)
//                     {
//                         RegionManager.Regions[app.PanelName].Add(view);
//                     }
//                 }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        protected override void SetView()
        {
            base.SetView();
            CurrentView = new ExamProductionView();
            CurrentView.PageName = "QuestionBank";
        }

        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();
            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                //ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("36");
                webRequest.ListData.Add("3601");
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

        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status !=
            DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }

        public static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
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

        public static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

        #endregion

    }
}
