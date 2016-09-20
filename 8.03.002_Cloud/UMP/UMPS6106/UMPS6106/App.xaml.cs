using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using UMPS6106.Service11012;
using UMPS6106.Service61061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using Common6106;

namespace UMPS6106
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : UMPApp
    {
        //统计时长
        public static int iStatisticsDay = 7;
        //usercontrol在主界面单独显示时的高度和宽度 会根据浏览器高度进行计算 此处是默认值
        public static int UCWidth = 500;
        public static int UCHeight = 500;

        //usercontrol在分组中多个显示时的高度和宽度
        public static int UCWidthInGroup = 300;
        public static int UCHeightInGroup = 300;

        //当前用户所属的机构ID
        public static string OrgID = string.Empty;

        /// <summary>
        /// 设置模块ID和Name
        /// </summary>
        protected override void SetAppInfo()
        {
            base.SetAppInfo();
            AppName = "UMPS6106";
            ModuleID = 6106;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();
            if (Session == null) { return; }
            //分表之类的参数
            Session.ListPartitionTables.Clear();
            PartitionTableInfo partInfo = new PartitionTableInfo();
            partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            partInfo.PartType = TablePartType.DatetimeRange;
            partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            Session.ListPartitionTables.Add(partInfo);

            //partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_OPTLOG;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_OPTLOG_OPERATETIME;
            //Session.ListPartitionTables.Add(partInfo);

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.7";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.6.86";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;
            //Session.AppServerInfo = serverInfo;

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

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0922";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB1225";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "voicecodes";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDBYoung0805";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void Init()
        {
            base.Init();
            try
            {
                if (Session != null)
                {
                    WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
                }
                InitLanguageInfos("6106");
                GetOrgID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WriteLog("App Init()," + ex.Message);
            }
        }

        public static void InitLanguageInfos(string strModuleID)
        {
            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                ListLanguageInfos.Clear();
                VoiceCyber.UMP.Communications.WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("0{0}61", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("0{0}{1}", ConstValue.SPLITER_CHAR, strModuleID));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                MonitorHelper.AddWebReturn(webReturn);
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

                WriteLog("AppLang", string.Format("Init LanguageInfos end,ListLanguageInfos.count =" + ListLanguageInfos.Count));
            }
            catch (Exception ex)
            {
                WriteLog("InitLang", string.Format("InitLang fail.\t{0}", ex.Message));
            }
        }

        public static void GetOrgID()
        {
            Service61061Client client=null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetOrgInfo;
                webRequest.Session = App.Session;
                client = new Service61061Client(WebHelper.CreateBasicHttpBinding(App.Session),
                       WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service61061"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                if (webReturn.Result)
                {
                    OrgID = webReturn.Data.ToString();
                }
            }
            catch
            {
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
    }
}
