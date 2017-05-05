using Common61011;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UMPS1101.Models;
using UMPS6101.DataSource.Models;
using UMPS6101.Models;
using UMPS6101.Sharing_Classes;
using UMPS6101.SharingClasses;
using UMPS6101.Wcf11012;
using UMPS6101.Wcf31021;
using UMPS6101.Wcf61011;
using UMPS6101.Wcf61012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS6101
{
    public class S6101App : UMPApp
    {
        public S6101App(bool runAsModule)
            : base(runAsModule)
        {
        }

        public S6101App(IRegionManager regionManager,
         IEventAggregator eventAggregator,
         IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new ReportDisplayPage();
            CurrentView.PageName = "ReportDisplayPage";
            WriteLog("startaargs = " + StartArgs);
        }

        #region Members

        public static string Path16 = string.Empty;
        public static string Path18 = string.Empty;
        public static string Path19 = string.Empty;
        public static string Path23 = string.Empty;

        public static AboutDateTime ADT = new AboutDateTime();
        private static int GetDataNum = 500;//每次获取数据条数
        public static int Multiple = 10000;//分数的放大倍数
        public static int NumAfterPoint = 3;//保留小数点后几位
        public static int TopNum = 3;

        //list for agent and inspecter
        public static List<string> AgentList = new List<string>();
        public static List<string> AgentNameList = new List<string>();
        public static List<string> InspecterList = new List<string>();
        public static List<string> UserList = new List<string>();
        //get parameters
        public static int WeekStart = 5;
        public static int MonthStart = 15;
        public static string PacketMode = "E";
        public static bool Flag18 = true;
        //get DataSet
        public static DataSet ds_userid;
        public static DataSet dslangdata;
        public static DataSet ds_agent;
        public static DataSet ds_ext;
        public static DataSet ds_realext;
        public static DataSet ds_comm;
        public static DataSet ds_gn;
        public static DataSet ds_ap;
        public static DataSet dslangFO;
        public static DataSet ds_31019;
        public static DataSet ds_comm46;
        public static DataSet ds_keywords;
        public static DataSet ds_agenttenure;

        public static int COUNT = 0;
        public static List<ScoreGroup> ListScoreSheet;
        //打标
        public static DataSet DataSetMark = new DataSet();
        public static List<string> ListMarkInfo = new List<string>();

        //查询条件记住
        public static List<QueryConditionInfo> mListQueryConditions;
        public static List<QueryConditionItem> mListQueryItems;
        public static List<QueryConditionItem> mListQueryConditionItems;

        #endregion

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS6101";
            ModuleID = 6101;
            AppTitle = Current.GetLanguageInfo(string.Format("FO{0}", ModuleID), "UMPS6101");
        }

        protected override void Init()
        {
            base.Init();
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
            InitObjects();
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }
            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "aaa";
            Session.RoleID = ConstValue.ROLE_SYSTEMADMIN;
            Session.RoleInfo = roleInfo;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.37";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB11292";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

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
            //dbInfo.Host = "192.168.9.236";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB_taiwan";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.9.224";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0913";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //dbInfo.RealPassword = "voicecodes";
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.7.101";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0509";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;

            //Session.InstallPath = @"C:\UMPRelease";
            //WriteLog("AppInit", string.Format("SessionInfo inited."));

            //Session.ListPartitionTables.Clear();
            //PartitionTableInfo partInfo = new PartitionTableInfo();

            //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            //Session.ListPartitionTables.Add(partInfo);

            //partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_OPTLOG;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_OPTLOG_OPERATETIME;
            //Session.ListPartitionTables.Add(partInfo);
        }

        public void InitObjects()
        {
            mListQueryConditions = new List<QueryConditionInfo>();
            mListQueryConditionItems = new List<QueryConditionItem>();
            mListQueryItems = new List<QueryConditionItem>();
            //加载31-028里的数据到mListqueryConditions
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)WebCodes.GetQueryConditions;
            webRequest.Session = Session;
            Service61012Client client = new Service61012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service61012"));
            WebHelper.SetServiceClient(client); 
            //Service61012Client client = new Service61012Client();
            WebReturn webReturn = client.UMPReportOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            webReturn.ListData.Sort();
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<QueryCondition>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                QueryCondition QueryInfo = optReturn.Data as QueryCondition;
                QueryConditionInfo Info = new QueryConditionInfo();
                Info.ChangeToInfo(QueryInfo);
                mListQueryConditions.Add(Info);
            }
            //加载31-044里的数据到mListqueryItems
            WebRequest webRequest1 = new WebRequest();
            webRequest1.Code = (int)WebCodes.GetQueryItems;
            webRequest1.Session = Session;
            Service61012Client client1 = new Service61012Client(WebHelper.CreateBasicHttpBinding(Session),
                WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service61012"));
            WebHelper.SetServiceClient(client1);
            WebReturn webReturn1 = client1.UMPReportOperation(webRequest1);
            client1.Close();
            if (!webReturn1.Result)
            {
                ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn1.Code, webReturn1.Message));
                return;
            }
            webReturn1.ListData.Sort();
            for (int i = 0; i < webReturn1.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<QueryConditionItem>(webReturn1.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                QueryConditionItem QueryInfo = optReturn.Data as QueryConditionItem;
                mListQueryItems.Add(QueryInfo);
            }
            //加载31-045里的数据到mListqueryConditionItems
            WebRequest webRequest2 = new WebRequest();
            webRequest2.Code = (int)WebCodes.GetQueryConditionItems;
            webRequest2.Session = Session;
            Service61012Client client2 = new Service61012Client(WebHelper.CreateBasicHttpBinding(Session),
                WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service61012"));
            WebHelper.SetServiceClient(client2);
            WebReturn webReturn2 = client2.UMPReportOperation(webRequest2);
            client2.Close();
            if (!webReturn2.Result)
            {
                ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn2.Code, webReturn2.Message));
                return;
            }
            webReturn2.ListData.Sort();
            for (int i = 0; i < webReturn2.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<QueryConditionItem>(webReturn2.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                QueryConditionItem QueryInfo = optReturn.Data as QueryConditionItem;
                mListQueryConditionItems.Add(QueryInfo);
            }
        }

        #region Language

        protected override void InitLanguageInfos()
        {
            try
            {
                base.InitLanguageInfos();
                if (Session == null || Session.LangTypeInfo == null) { return; }
                //ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("61");
                webRequest.ListData.Add("6101");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
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
                WriteLog("AppInit", string.Format("LanguageInfos inited."));
            }
            catch (Exception ex)
            {
                // ShowExceptionMessage(ex.Message);
            }
        }

        public string GetLanguageInfo(string name, string display)
        {
            try
            {
                LanguageInfo lang =
                    ListLanguageInfos.FirstOrDefault(l => l.LangID == Session.LangTypeInfo.LangID && l.Name == name);
                if (lang == null)
                {
                    return display;
                }
                return lang.Display;
            }
            catch (Exception ex)
            {
                return display;
            }
        }

        #endregion

        #region 报表
        public DataSet GetR1DataSet(string StrQ, int t)
        {
            DataTable dt = new DataTable(); DataSet ds = new DataSet(); DataColumn col_exten = new DataColumn("C042", typeof(string));
            DataColumn col_IP = new DataColumn("C020", typeof(string)); DataColumn col_name = new DataColumn("C019", typeof(string));
            DataColumn col_cin = new DataColumn("P100", typeof(int)); DataColumn col_cout = new DataColumn("P101", typeof(int));
            DataColumn col_cintime = new DataColumn("P102", typeof(string)); DataColumn col_couttime = new DataColumn("P103", typeof(string));
            DataColumn col_time = new DataColumn("P104", typeof(string)); DataColumn col_avrragetime = new DataColumn("P105", typeof(string));
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); DataColumn col_call = new DataColumn("P108", typeof(int));
            DataColumn col_calltime = new DataColumn("P109", typeof(string));
            dt.Columns.Add(col_exten); dt.Columns.Add(col_IP); dt.Columns.Add(col_name); dt.Columns.Add(col_cin); dt.Columns.Add(col_cout); dt.Columns.Add(col_call);
            dt.Columns.Add(col_cintime); dt.Columns.Add(col_couttime); dt.Columns.Add(col_time); dt.Columns.Add(col_avrragetime); dt.Columns.Add(col_absdate); dt.Columns.Add(col_calltime);
            List<string> TableName = new List<string>(); DataSet DS_temp = new DataSet();
            string sql = string.Empty; DataTable DT = dt.Clone(); bool first_time = true;
            List<string> ExtenList = new List<string>(); List<string> AbsDateList = new List<string>();
            List<string> IPList = new List<string>(); List<string> NameList = new List<string>(); COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;
            TableName = TableSec("T_21_001", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")); DataSet _ds = new DataSet();
            for (int s = 0; s < TableName.Count; s++)
            {
                int PreRowNum = GetDataNum; int AfterRowNum = 0; bool IsNull = true;
                do
                {
                    IsNull = false;

                    if (Session.DBType == 2)
                    {
                        sql = string.Format("SELECT TOP {1} A.C001,A.C042,A.C045,A.C020,A.C019,A.C012,A.C005 FROM {0} A WHERE A.C001>{5} AND A.C005 >= '{2}' AND A.C005 <= '{3}' {4}  ORDER BY A.C001,A.C005"
                        , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), StrQ, AfterRowNum);
                    }
                    else
                    {
                        sql = string.Format("SELECT C001,C042,C045,C020,C019,C012,C005 FROM (SELECT A.* FROM {0} A WHERE "
                                + "A.C005 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND A.C005 <= TO_DATE('"
                                + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')" + StrQ + " ORDER BY A.C001,A.C005) WHERE C001>" + AfterRowNum + " AND ROWNUM<={1} ORDER BY C001,C005 DESC ", TableName[s], GetDataNum);
                    }
                    if (first_time)
                    {
                        _ds = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (_ds == null)
                        {
                            continue;
                        }
                        else if (_ds.Tables != null)
                        {
                            if (_ds.Tables.Count != 0 && _ds.Tables[0].Rows.Count != 0)
                            {
                                IsNull = true; CiShuSum++; first_time = false;
                            }
                        }
                    }
                    else
                    {
                        DS_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (DS_temp == null)
                        {
                            continue;
                        }
                        if (DS_temp.Tables == null)
                        {
                            continue;
                        }
                        else if (DS_temp.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++;

                            if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                    _ds.Tables[0].ImportRow(dr);
                        }
                    }
                    if (IsNull)
                    {
                        COUNT = _ds.Tables[0].Rows.Count - 1;
                        AfterRowNum = Convert.ToInt32(_ds.Tables[0].Rows[COUNT]["C001"]);
                        WriteLog(string.Format("  (1):{0} \n {1}", sql, COUNT));
                    }
                }
                while (IsNull);
            }
            WriteLog(string.Format("(1):{0}", sql));

            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0];
                #region 数据统计
                foreach (DataRow exten in _dt.Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < ExtenList.Count; i++)
                    {
                        if (exten["C042"].ToString() == ExtenList[i] && exten["C020"].ToString() == IPList[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        ExtenList.Add(exten["C042"].ToString());
                        IPList.Add(exten["C020"].ToString());
                        NameList.Add(exten["C019"].ToString());
                        string StrTemp = Convert.ToDateTime(exten["C005"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        AbsDateList.Add(AbsoluteDate(StrTemp.Substring(0, StrTemp.Length - 8)));
                    }
                }
                WriteLog(string.Format("(1):Ext OK"));
                TransData(ExtenList, dt, "C042", 1); TransData(IPList, dt, "C020", 2); TransData(NameList, dt, "C019", 2); TransData(AbsDateList, dt, "P107", 2);
                WriteLog(string.Format("(1):Put Data in DataSet ok"));
                for (int j = 0; j < ExtenList.Count; j++)
                {
                    int cin = 0; int cout = 0; int CinTime = 0; int CoutTime = 0;
                    foreach (DataRow exten in _dt.Rows)
                    {
                        if (ExtenList[j] == exten["C042"].ToString() && IPList[j] == exten["C020"].ToString())
                        {
                            if (exten["C045"].ToString() == "1")
                            {
                                cin += 1;
                                CinTime += Convert.ToInt32(exten["C012"]);
                            }
                            else
                            {
                                cout += 1;
                                CoutTime += Convert.ToInt32(exten["C012"]);
                            }
                        }
                    }
                    WriteLog(string.Format("(1):Statistic {0} ", ExtenList[j]));
                    dt.Rows[j]["P100"] = cin; dt.Rows[j]["P101"] = cout; dt.Rows[j]["P102"] = TransTimeForm(CinTime); dt.Rows[j]["P103"] = TransTimeForm(CoutTime);
                    dt.Rows[j]["P104"] = TransTimeForm(CinTime + CoutTime); dt.Rows[j]["P108"] = cin + cout;
                    if (cin + cout == 0)
                    {
                        dt.Rows[j]["P105"] = "00:00:00";
                    }
                    else
                        dt.Rows[j]["P105"] = TransTimeForm((CinTime + CoutTime) / (cin + cout));
                }
                #endregion
                WriteLog(string.Format("(1):Data OK"));
                foreach (DataRow dr in dt.Rows)
                    DT.ImportRow(dr);
                ds.Tables.Add(DT);
                WriteLog(string.Format("(1):Put data in New dataset OK"));
            }
            return ds;
        }

        public DataSet GetR2DataSet(string StrQ, int t)
        {
            DataTable dt = new DataTable(); DataSet ds = new DataSet(); DataTable DT = new DataTable(); DataTable DT_Chart = new DataTable();
            DataColumn col_agentid = new DataColumn("C039", typeof(string)); DataColumn col_agentname = new DataColumn("P200", typeof(string));
            DataColumn col_part = new DataColumn("P201", typeof(string)); DataColumn col_cin = new DataColumn("P202", typeof(int));
            DataColumn col_cout = new DataColumn("P203", typeof(int)); DataColumn col_cintime = new DataColumn("P204", typeof(string));
            DataColumn col_couttime = new DataColumn("P205", typeof(string)); DataColumn col_cinatime = new DataColumn("P206", typeof(string));
            DataColumn col_coutatime = new DataColumn("P207", typeof(string)); DataColumn col_time = new DataColumn("P208", typeof(string));
            DataColumn col_avrragetime = new DataColumn("P209", typeof(string)); DataColumn col_allcintime = new DataColumn("P210", typeof(string));
            DataColumn col_allcouttime = new DataColumn("P211", typeof(string)); DataColumn col_allcinatime = new DataColumn("P212", typeof(string));
            DataColumn col_allcoutatime = new DataColumn("P213", typeof(string)); DataColumn col_alltime = new DataColumn("P214", typeof(string));
            DataColumn col_allatime = new DataColumn("P215", typeof(string)); DataColumn col_allc = new DataColumn("P216", typeof(int));
            DataColumn col_absdate = new DataColumn("P107", typeof(string));
            dt.Columns.Add(col_agentid); dt.Columns.Add(col_cin); dt.Columns.Add(col_cout);
            DT_Chart = dt.Clone(); dt.Columns.Add(col_part); dt.Columns.Add(col_agentname); dt.Columns.Add(col_absdate);
            DataTable _dt1 = ds_agent.Tables[0]; COUNT = 0;
            dt.Columns.Add(col_cintime); dt.Columns.Add(col_couttime); dt.Columns.Add(col_cinatime); dt.Columns.Add(col_coutatime);
            dt.Columns.Add(col_time); dt.Columns.Add(col_avrragetime); dt.Columns.Add(col_allcintime); dt.Columns.Add(col_allcouttime);
            dt.Columns.Add(col_allcinatime); dt.Columns.Add(col_allcoutatime); dt.Columns.Add(col_alltime); dt.Columns.Add(col_allatime);
            dt.Columns.Add(col_allc); DT = dt.Clone(); List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;
            TableName = TableSec("T_21_001", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")); DataSet _ds = new DataSet();
            for (int s = 0; s < TableName.Count; s++)
            {
                int AfterRowNum = 0; bool IsNull = true;
                do
                {
                    IsNull = false;

                    if (Session.DBType == 2)
                    {
                        sql = string.Format("SELECT TOP {1} C001,C039,C012,C005,C045,C020,C019 FROM {0} WHERE C001>{5} AND C005 >= '{2}' AND C005 <= '{3}' {4}  ORDER BY C001,C005"
                            , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), StrQ, AfterRowNum);
                    }
                    else
                    {
                        sql = string.Format("SELECT C001,C039,C012,C005,C045,C020,C019 FROM (SELECT A.* FROM {0} A WHERE "
                            + "C005 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND C005 <= TO_DATE('"
                            + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')" + StrQ + " ORDER BY C001,C005) WHERE C001 >" + AfterRowNum + " AND ROWNUM<={1} ORDER BY C001,C005 DESC ", TableName[s], GetDataNum);
                    }
                    if (first_time)
                    {
                        _ds = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (_ds == null)
                        {
                            continue;
                        }
                        if (_ds.Tables != null && _ds.Tables.Count != 0)
                            if (_ds.Tables[0].Rows.Count != 0)
                            {
                                IsNull = true; CiShuSum++; first_time = false;
                            }
                    }
                    else
                    {
                        DS_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (DS_temp == null)
                        {
                            continue;
                        }
                        if (DS_temp.Tables == null)
                        {
                            continue;
                        }
                        else if (DS_temp.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++;
                        }
                        if (DS_temp != null)
                        {
                            if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                    _ds.Tables[0].ImportRow(dr);
                        }
                    }
                    if (IsNull)
                    {
                        COUNT = _ds.Tables[0].Rows.Count - 1;
                        AfterRowNum = Convert.ToInt32(_ds.Tables[0].Rows[COUNT]["C001"]);
                    }
                } while (IsNull);
            }

            WriteLog(string.Format("(2):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0];
                #region 统计数据
                List<string> AgentList2 = new List<string>();
                foreach (DataRow agent in _dt.Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < AgentList2.Count; i++)
                    {
                        if (agent["C039"].ToString() == AgentList2[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        AgentList2.Add(agent["C039"].ToString());
                    }
                }
                TransData(AgentList2, dt, "C039", 1);
                int allcintime = 0; int allcouttime = 0; int alltime = 0; int allcinatime = 0; int allcoutatime = 0; int allatime = 0; int allcin = 0; int allcout = 0;
                for (int i = 0; i < AgentList2.Count; i++)
                {
                    foreach (DataRow dr in _dt1.Rows)
                    {
                        if (AgentList2[i] == dr["C017"].ToString())
                        {
                            dt.Rows[i]["P200"] = dr["C018"];
                            dt.Rows[i]["P201"] = dr["C012"];
                            break;
                        }
                    }
                    int cin = 0; int cout = 0; int cintime = 0; int couttime = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (AgentList2[i] == dr["C039"].ToString())
                        {
                            string StrTemp = Convert.ToDateTime(dr["C005"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            dt.Rows[i]["P107"] = AbsoluteDate(StrTemp.Substring(0, StrTemp.Length - 8));
                            if (dr["C045"].ToString() == "1")
                            {
                                cin += 1;
                                cintime += Convert.ToInt32(dr["C012"]);
                            }
                            else
                            {
                                cout += 1;
                                couttime += Convert.ToInt32(dr["C012"]);
                            }
                        }
                    }
                    dt.Rows[i]["P210"] = cin + cout; dt.Rows[i]["P202"] = cin; dt.Rows[i]["P203"] = cout; dt.Rows[i]["P204"] = TransTimeForm(cintime); dt.Rows[i]["P205"] = TransTimeForm(couttime);
                    if (cin == 0)
                    {
                        dt.Rows[i]["P206"] = "00:00:00";
                    }
                    else
                        dt.Rows[i]["P206"] = TransTimeForm(cintime / cin);
                    if (cout == 0)
                    {
                        dt.Rows[i]["P207"] = "00:00:00";
                    }
                    else
                        dt.Rows[i]["P207"] = TransTimeForm(couttime / cout); dt.Rows[i]["P208"] = TransTimeForm(cintime + couttime);
                    if (cin + cout == 0)
                    {
                        dt.Rows[i]["P209"] = "00:00:00";
                    }
                    else
                        dt.Rows[i]["P209"] = TransTimeForm((cintime + couttime) / (cin + cout));
                    allcintime += cintime; allcouttime += couttime; allcin += cin; allcout += cout;
                }
                if (allcin != 0)
                {
                    allcinatime = allcintime / allcin;
                }
                if (allcout != 0)
                {
                    allcoutatime = allcouttime / allcout;
                }
                if (allcout + allcin != 0)
                {
                    alltime = allcintime + allcouttime; allatime = alltime / (allcin + allcout);
                }
                if (dt.Rows.Count != 0)
                {
                    DataRow data_row = dt.NewRow(); data_row["P204"] = TransTimeForm(allcintime); data_row["P205"] = TransTimeForm(allcouttime); data_row["P206"] = TransTimeForm(allcinatime);
                    data_row["P207"] = TransTimeForm(allcoutatime); data_row["P208"] = TransTimeForm(alltime); data_row["P209"] = TransTimeForm(allatime); data_row["P202"] = allcin;
                    data_row["P203"] = allcout; data_row["P210"] = allcin + allcout; data_row["P200"] = GetLanguageInfo("6101120209", "汇总组");
                    dt.Rows.Add(data_row);
                }
                #endregion
                foreach (DataRow dr in dt.Rows)
                    DT.ImportRow(dr);
                ds.Tables.Add(DT);
                DataRow[] DR = dt.Select("C039<>''");
                for (int i = 0; i < DR.Count(); i++)
                {
                    DT_Chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(DT_Chart);
            }
            return ds;
        }

        public DataSet GetR3DataSet(string StrQ, int t)
        {
            DataSet DS = new DataSet(); DataSet ds = new DataSet(); List<string> TableName = new List<string>(); string sql = string.Empty; DataTable dt = new DataTable(); DataTable DT = new DataTable();
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); bool first_time = true; COUNT = 0;
            TableName = TableSec("T_11_901", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")); int CiShuSum = 0; int count_num = 0;
            for (int s = 0; s < TableName.Count; s++)
            {
                int PreRowNum = GetDataNum; string AfterRowNum = "0";
                sql = "SELECT COUNT(0) FROM " + TableName[s] + " WHERE  C008>=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND C008<=" + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ;
                DataSet ds_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                if (ds_temp != null)
                {
                    if (ds_temp.Tables.Count != 0)
                    {
                        count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
                    } int CiShu = count_num % GetDataNum;
                    if (CiShu != 0)
                        CiShu = count_num / GetDataNum + 1;
                    else
                        CiShu = count_num / GetDataNum; CiShuSum += CiShu;
                    for (int j = 1; j <= CiShu; j++)
                    {
                        if (Session.DBType == 2)
                        {
                            sql = string.Format("SELECT TOP {1} C001,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015 FROM {0} WHERE C008 >= '{2}' AND C008 <= '{3}' {4}  ORDER BY C001,C008"
                                   , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"), StrQ + " AND C001 >" + AfterRowNum);
                        }
                        else
                        {
                            sql = string.Format("SELECT C001,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015 FROM (SELECT A.* FROM {0} A WHERE "
                                + " C008 >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND C008 <=" + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ
                                + " ORDER BY C001,C008) WHERE C001>" + AfterRowNum + " AND ROWNUM<=" + GetDataNum + " ORDER BY C001,C008 DESC ", TableName[s]);
                        }
                        if (first_time)
                        {
                            ds = GetDataSetFromDB(sql, 100, TableName[s]); dt = ds.Tables[0]; first_time = false;
                        }
                        else
                        {
                            DataSet DS_temp = GetDataSetFromDB(sql, 100);
                            if (DS_temp != null)
                            {
                                if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                    foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                        dt.ImportRow(dr);
                            }
                        }
                        if (dt.Rows.Count > 0)
                        {
                            COUNT = dt.Rows.Count - 1;
                            AfterRowNum = Convert.ToString(dt.Rows[COUNT]["C001"]);
                        }
                    }
                }
            }

            WriteLog(string.Format("(3):{0}", sql));

            if (CiShuSum != 0)
            {
                int countrow = dt.Rows.Count; int countcol = dt.Columns.Count;
                #region 数据收集
                List<string> strlistOM = new List<string>();
                List<string> strlistOA = new List<string>();
                List<string> strlistOC = new List<string>();
                List<string> strlistUserID = new List<string>();
                List<string> strlistIsOk = new List<string>();
                string strtemp;
                foreach (DataRow dr in dt.Rows)
                {
                    string maname = dr["C006"].ToString();

                    if (maname.Length == 64)
                    {
                        dr["C006"] = string.Empty;
                    }
                    strtemp = "FO" + dr["C004"].ToString();
                    strlistOA.Add(strtemp);
                    strtemp = dr["C005"].ToString();
                    strlistUserID.Add(strtemp);
                    strtemp = dr["C010"].ToString();
                    strlistOC.Add(strtemp);
                    strtemp = "OP" + dr["C009"].ToString();
                    strlistIsOk.Add(strtemp);
                    if (dr["C003"].ToString() != "11")
                    {
                        strtemp = string.Format("FO{0}", dr["C004"].ToString().Substring(0, 4));
                    }
                    else
                    {
                        strtemp = string.Format("FO{0}", dr["C003"].ToString());
                    }
                    strlistOM.Add(strtemp);
                }
                DataColumn col = new DataColumn("P300", typeof(string));
                DataColumn coloc1 = new DataColumn("P301", typeof(string));
                DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
                DataColumn coloc2 = new DataColumn("P302", typeof(string));
                DataColumn coloc3 = new DataColumn("P303", typeof(string)); dt.Columns.Add(coloc3);
                DataColumn coloc4 = new DataColumn("P304", typeof(DateTime)); dt.Columns.Add(coloc4);
                dt.Columns.Add(col); dt.Columns.Add(coloc1); dt.Columns.Add(coloc2);
                for (int i = 0; i < countrow; i++)
                {
                    string tempom = strlistOM[i].ToString();
                    string tempoa = strlistOA[i].ToString();
                    string tempuserid = strlistUserID[i].ToString();
                    string tempisok = strlistIsOk[i].ToString();
                    string tempoc = strlistOC[i].ToString();
                    DateTime dtTime = DateTime.ParseExact(dt.Rows[i]["C008"].ToString(), "yyyyMMddHHmmss", null).ToLocalTime();
                    string time = dtTime.ToString();
                    dt.Rows[i]["P107"] = AbsoluteDate(time);
                    dt.Rows[i]["P304"] = dtTime;
                    for (int j = 0; j < dslangFO.Tables[0].Rows.Count; j++)
                    {
                        string tempoa_table = dslangFO.Tables[0].Rows[j]["C002"].ToString();
                        if (tempoa == tempoa_table)
                        {
                            dt.Rows[i][col] = dslangFO.Tables[0].Rows[j]["C005"];
                            break;
                        }
                    }
                    for (int j = 0; j < dslangFO.Tables[0].Rows.Count; j++)
                    {
                        string tempoa_table = dslangFO.Tables[0].Rows[j]["C002"].ToString();
                        if (tempom == tempoa_table)
                        {
                            string OM = dslangFO.Tables[0].Rows[j]["C005"].ToString();
                            dt.Rows[i][coloc3] = OM;
                            break;
                        }
                    }
                    if (tempuserid.Length > 3 &&
                        tempuserid.Substring(0, 3) == "103")
                    {
                        for (int k = 0; k < ds_agent.Tables[0].Rows.Count; k++)
                        {
                            string tempoc_table = ds_agent.Tables[0].Rows[k]["C001"].ToString();
                            if (tempuserid == tempoc_table)
                            {
                                dt.Rows[i][coloc1] = ds_agent.Tables[0].Rows[k][1].ToString();
                                dt.Rows[i][coloc2] = ds_agent.Tables[0].Rows[k][2].ToString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < ds_userid.Tables[0].Rows.Count; k++)
                        {
                            string tempoc_table = ds_userid.Tables[0].Rows[k]["C001"].ToString();
                            if (tempuserid == tempoc_table)
                            {
                                dt.Rows[i][coloc1] = DecryptString(ds_userid.Tables[0].Rows[k][1].ToString());
                                dt.Rows[i][coloc2] = DecryptString(ds_userid.Tables[0].Rows[k][2].ToString());
                                break;
                            }
                        }
                    }
                    for (int o = 0; o < dslangdata.Tables[0].Rows.Count; o++)
                    {
                        string tempisok_table = dslangdata.Tables[0].Rows[o]["C002"].ToString();
                        if (tempisok == tempisok_table)
                        {
                            dt.Rows[i]["C009"] = dslangdata.Tables[0].Rows[o]["C005"];
                            break;
                        }
                    }
                    for (int p = 0; p < dslangdata.Tables[0].Rows.Count; p++)
                    {
                        string tempoc_table = dslangdata.Tables[0].Rows[p]["C002"].ToString();
                        string StrPram = dt.Rows[i]["C011"].ToString() + dt.Rows[i]["C012"].ToString() + dt.Rows[i]["C013"] + dt.Rows[i]["C014"] + dt.Rows[i]["C015"];
                        if (tempoc == tempoc_table)
                        {
                            string temp = dslangdata.Tables[0].Rows[p]["C005"].ToString();

                            string split1 = string.Format("{0}{0}{0}", (char)30);
                            string[] listCategory = StrPram.Split(new[] { split1 }, StringSplitOptions.None);
                            string strReg = @"\{\d+\}";
                            Regex regex = new Regex(strReg);
                            var match = regex.Match(temp);
                            int index = 0;
                            string strReplace;
                            while (match.Success)
                            {
                                if (listCategory.Length >= index + 1)
                                {
                                    strReplace = listCategory[index];
                                    strReplace = DecodeContent(strReplace);
                                    temp = regex.Replace(temp, strReplace, 1);
                                }
                                else
                                {
                                    temp = regex.Replace(temp, string.Empty, 1);
                                }
                                index++; match = match.NextMatch();
                            }
                            dt.Rows[i]["C010"] = temp;
                            break;
                        }
                    }
                }
                #endregion
                DT = dt.Clone();
                foreach (DataRow dr in dt.Rows)
                    DT.ImportRow(dr);
                DT.DefaultView.Sort = "P304 DESC";
                DT = DT.DefaultView.ToTable();
                DS.Tables.Add(DT);
            }
            return DS;
        }

        public DataSet GetR4DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty; DataSet DS_temp = new DataSet();
            DataColumn col_agentname = new DataColumn("P400", typeof(string)); dt.Columns.Add(col_agentname);
            DataColumn col_checkavrage = new DataColumn("P402", typeof(decimal)); DataColumn col_checkmax = new DataColumn("P403", typeof(decimal));
            DataColumn col_checkmin = new DataColumn("P404", typeof(decimal)); dt.Columns.Add(col_checkavrage); dt.Columns.Add(col_checkmax);
            dt.Columns.Add(col_checkmin); DataTable dt_chart = dt.Clone();
            DataColumn col_checknum = new DataColumn("P401", typeof(int)); dt.Columns.Add(col_checknum);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            DataColumn col_agentid = new DataColumn("C013", typeof(string)); DataColumn col_part = new DataColumn("C012", typeof(string));
            dt.Columns.Add(col_agentid); dt.Columns.Add(col_part); bool first_time = true; string TimeQuery = string.Empty; COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet();
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = "SELECT COUNT(0) FROM T_31_008_" + Session.RentInfo.Token + " WHERE  C009='Y' AND " + TimeQuery + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C012,C013,C004,C006 P107 FROM T_31_008_{0} WHERE {2} AND C009='Y' ORDER BY C001,P107"
                                  , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C012,C013,C004,C006 P107 FROM (SELECT A.* FROM T_31_008_{0} A WHERE " + TimeQuery + StrQ + " AND C009='Y' ORDER BY C001," + ColName + ") WHERE C001>" + AfterRowNum + " AND ROWNUM<="
                        + GetDataNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }

            WriteLog(string.Format("(4):{0}", sql));

            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0];
                DataTable dtM = _dt.Clone();
                foreach (DataRow drM in _dt.Rows)
                {
                    dtM.ImportRow(drM);
                }
                DataSetMark.Tables.Add(dtM);
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                List<string> AgentList4 = new List<string>();
                List<string> PartList = new List<string>();
                foreach (DataRow agent in _dt.Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < AgentList4.Count; i++)
                    {
                        if (agent["C013"].ToString() == AgentList4[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        AgentList4.Add(agent["C013"].ToString());
                        PartList.Add(agent["C012"].ToString());
                    }
                }
                TransData(AgentList4, dt, "C013", 1); TransData(PartList, dt, "C012", 2);

                for (int i = 0; i < AgentList4.Count; i++)
                {
                    List<string> Taks = new List<string>(); DataTable DT_Final = _dt.Clone();
                    int cnum = 0; decimal cmax = 0; decimal cmin = 1000 * Multiple; decimal call = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (AgentList4[i] == dr["C013"].ToString())
                        {
                            bool Flag = true;
                            for (int j = 0; j < Taks.Count; j++)
                            {
                                if (Taks[j] == dr["C002"].ToString())
                                {
                                    Flag = false;
                                    break;
                                }
                            }
                            if (Flag)
                            {
                                Taks.Add(dr["C002"].ToString());
                            }
                        }
                        if (AgentList4[i] == dr["C013"].ToString())
                        {
                            cnum += 1; call += decimal.Parse(dr["C004"].ToString()); decimal score = decimal.Parse(dr["C004"].ToString());
                            if (score > cmax)
                                cmax = score;
                            if (score < cmin)
                                cmin = score;
                        }
                        DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                        string table_date = DTTEMP.ToString("yyyyMMddHHmmss");

                        dt.Rows[i]["P107"] = AbsoluteDate(table_date);
                    }
                  
                    dt.Rows[i]["P401"] = cnum; dt.Rows[i]["P403"] = decimal.Round(cmax, NumAfterPoint);
                    if (cmin != 1000 * Multiple)
                        dt.Rows[i]["P404"] = decimal.Round(cmin, NumAfterPoint);
                    else
                        dt.Rows[i]["P404"] = 0;
                    if (cnum != 0)
                        dt.Rows[i]["P402"] = decimal.Round(call / cnum, NumAfterPoint);
                    else
                        dt.Rows[i]["P402"] = 0;
                }
                DataTable _dt1 = ds_agent.Tables[0];
                for (int i = 0; i < AgentList4.Count; i++)
                {
                    foreach (DataRow dr in _dt1.Rows)
                    {
                        if (AgentList4[i] == dr["C001"].ToString())
                        {
                            dt.Rows[i]["C013"] = dr["C017"].ToString();
                            dt.Rows[i]["P400"] = dr["C018"].ToString();
                            dt.Rows[i]["C012"] = dr["C012"].ToString();
                            break;
                        }
                    }
                }
                #endregion
                ds.Tables.Add(dt);
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR5DataSet(string StrQ, int t, string ColName)
        {
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet(); bool first_time = true; COUNT = 0;
            string sql = "SELECT COUNT(0) FROM T_31_041_" + Session.RentInfo.Token + " WHERE " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                + " AND C005 = '1' AND  " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100); DataSet ds = new DataSet();
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C017,C015,C005,C100,C003,C004,C002,C007,C000," + ColName
                        + " P107 FROM T_31_041_{0} WHERE C005 = '1' AND  " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                        + " AND " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " {2} ORDER BY C001,P107"
                                      , Session.RentInfo.Token, GetDataNum, StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C017,C015,C005,C100,C003,C004,C002,C007,C000,{2} P107 FROM (SELECT A.* FROM T_31_041_{0} A WHERE C005 = '1' AND  {2} >="
                        + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND {2} <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ + " ORDER BY C001,{2}) WHERE ROWNUM<={1} AND C001>" + AfterRowNum
                        + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(5):{0}", sql));
            if (CiShuSum != 0)
            {
                #region 统计数据
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable();
                DataTable dtM = _dt.Clone();
                DataColumn col_agentname = new DataColumn("P500", typeof(string)); dt.Columns.Add(col_agentname);
                DataColumn col_comment = new DataColumn("P501", typeof(string)); dt.Columns.Add(col_comment);
                DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
                DataColumn col_agentid = new DataColumn("C017", typeof(string)); dt.Columns.Add(col_agentid);
                DataColumn col_part = new DataColumn("C015", typeof(string)); dt.Columns.Add(col_part);
                DataColumn col_score = new DataColumn("C100", typeof(decimal)); dt.Columns.Add(col_score);
                DataColumn col_inspecttime = new DataColumn("C003", typeof(string)); dt.Columns.Add(col_inspecttime);
                DataColumn col_inspect = new DataColumn("C004", typeof(string)); dt.Columns.Add(col_inspect);
                DataColumn col_recordid = new DataColumn("C002", typeof(string)); dt.Columns.Add(col_recordid);
                DataColumn col_appeal = new DataColumn("C007", typeof(string)); dt.Columns.Add(col_appeal);
                DataColumn col_mark = new DataColumn("C000", typeof(string)); dt.Columns.Add(col_mark);
                foreach (DataRow row in _dt.Rows)
                {
                    dt.ImportRow(row); dtM.ImportRow(row);
                }
                DataSetMark.Tables.Add(dtM);
                ListMarkInfo.Add("T_31_041");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                foreach (DataRow dr in dt.Rows)
                {
                    string table_date = DateTime.ParseExact(dr["P107"].ToString(), "yyyyMMddHHmmss", null).ToLocalTime().ToString();
                    dr["P107"] = AbsoluteDate(table_date);
                    dr["C100"] = decimal.Round(decimal.Parse(dr["C100"].ToString()) / Multiple, NumAfterPoint);
                    dr["C003"] = dr["C003"].ToString().Substring(0, 4) + "/" + dr["C003"].ToString().Substring(4, 2) + "/" + dr["C003"].ToString().Substring(6, 2);
                    if (dr["C007"].ToString() != "0")
                    {
                        dr["C007"] = GetLanguageInfo("6101200501", "Yes");
                    }
                    else
                    {
                        dr["C007"] = GetLanguageInfo("6101200502", "No");
                    }
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (dr["C017"].ToString() == dr_ap["C001"].ToString())
                        {
                            dr["C017"] = dr_ap["C017"];
                            dr["P500"] = dr_ap["C018"];
                            //dr["C015"] = dr_ap["C012"];
                            break;
                        }
                    }
                    foreach (DataRow dr_ap in ds_ap.Tables[0].Rows)
                    {
                        if (dr["C015"].ToString() == dr_ap["C001"].ToString())
                        {
                            dr["C015"] = DecryptString(dr_ap["C002"].ToString());
                            break;
                        }
                    }
                    DataRow[] CommRow = ds_comm46.Tables[0].Select(string.Format("C002={0} and C003={1}", dr["C002"].ToString(), dr["C004"].ToString()));
                    //foreach (DataRow dr_comm in CommRow)
                    //{
                    if (CommRow != null && CommRow.Count() != 0)
                        dr["P501"] = CommRow[0]["C005"].ToString();
                    //}
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (dr["C004"].ToString() == dr_ap["C001"].ToString())
                        {
                            dr["C004"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                }
                ds.Tables.Add(dt);
                #endregion
            }
            return ds;
        }

        public DataSet GetR6DataSet(string StrQ, int t, string ColName)
        {
            DataSet DS_temp = new DataSet(); bool first_time = true; int CiShuSum = 0; int count_num = 0; COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            DataSet _ds = new DataSet(); DataSet ds = new DataSet(); string TimeQuery = string.Empty;
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            string sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} WHERE C009='Y' AND {1} {2}", Session.RentInfo.Token, TimeQuery, StrQ);
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; double AfterRowNum = 0;
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C012,C004,C006 P107 FROM T_31_008_{0} WHERE {3} {2} AND C009='Y' ORDER BY C001,P107"
                                        , Session.RentInfo.Token, GetDataNum, StrQ + " AND C001 >" + AfterRowNum, TimeQuery);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C012,C004,C006 P107 FROM (SELECT A.* FROM T_31_008_{0} A WHERE  {2}"
                        + StrQ + " AND C009='Y' ORDER BY C001," + ColName + ") WHERE ROWNUM<={1} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum, TimeQuery);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToDouble(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(6):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable(); DataTable dtM = _dt.Clone();
                foreach (DataRow drM in _dt.Rows)
                {
                    dtM.ImportRow(drM);
                }
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 数据统计
                DataColumn col_checkaverage = new DataColumn("P601", typeof(decimal)); dt.Columns.Add(col_checkaverage);
                DataColumn col_depart = new DataColumn("C012", typeof(string)); dt.Columns.Add(col_depart); DataTable dt_chart = dt.Clone();
                DataColumn col_checkmax = new DataColumn("P602", typeof(decimal)); dt.Columns.Add(col_checkmax);
                DataColumn col_checkmin = new DataColumn("P603", typeof(decimal)); dt.Columns.Add(col_checkmin);
                DataColumn col_checknum = new DataColumn("P600", typeof(int)); dt.Columns.Add(col_checknum);
                DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
                List<string> PartList = new List<string>(); List<string> TimeList = new List<string>(); List<string> PartIdList = new List<string>();
                foreach (DataRow part in _dt.Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < PartList.Count; i++)
                    {
                        if (part["C012"].ToString() == PartIdList[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        bool IsTrans = false;
                        foreach (DataRow dr in ds_ap.Tables[0].Rows)
                        {
                            if (part["C012"].ToString() == dr["C001"].ToString())
                            {
                                PartIdList.Add(part["C012"].ToString());
                                PartList.Add(DecryptString(dr["C002"].ToString()));

                                string table_date = string.Empty;
                                DateTime DTTEMP = Convert.ToDateTime(part["P107"].ToString()).ToLocalTime();
                                table_date = DTTEMP.ToString("yyyyMMddHHmmss");
                                TimeList.Add(AbsoluteDate(table_date)); IsTrans = true;
                                break;
                            }
                        }
                        if (!IsTrans)
                        {
                            PartIdList.Add(part["C012"].ToString());
                            PartList.Add(string.Empty);
                        }
                    }
                }

                TransData(PartList, dt, "C012", 1); TransData(TimeList, dt, "P107", 2);
                for (int i = 0; i < PartIdList.Count; i++)
                {

                    List<string> RecorderID = new List<string>(); DataTable DT_Final = _dt.Clone();
                    //foreach (DataRow dr in _dt.Rows)
                    //{
                    //    if (PartIdList[i] == dr["C012"].ToString())
                    //    {
                    //        bool Flag = true;
                    //        for (int j = 0; j < RecorderID.Count; j++)
                    //        {
                    //            if (RecorderID[j] == dr["C002"].ToString())
                    //            {
                    //                Flag = false;
                    //                break;
                    //            }
                    //        }
                    //        if (Flag)
                    //        {
                    //            RecorderID.Add(dr["C002"].ToString());
                    //        }
                    //    }
                    //}
                   
                    int cnum = 0; decimal cmax = 0; decimal cmin = Multiple * 1000; decimal call = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (PartIdList[i] == dr["C012"].ToString())
                        {
                            decimal tempScore = decimal.Parse(dr["C004"].ToString());
                            cnum += 1; call += tempScore; decimal score = tempScore;
                            if (score > cmax)
                                cmax = score;
                            if (score < cmin)
                                cmin = score;
                        }
                    }
                    dt.Rows[i]["P600"] = cnum; dt.Rows[i]["P602"] = decimal.Round(cmax, NumAfterPoint);
                    if (cmin != 1000)
                        dt.Rows[i]["P603"] = decimal.Round(cmin, NumAfterPoint);
                    else
                        dt.Rows[i]["P603"] = 0;
                    if (cnum != 0)
                        dt.Rows[i]["P601"] = decimal.Round((call / cnum), NumAfterPoint);
                    else
                        dt.Rows[i]["P601"] = 0;
                }
                #endregion
                ds.Tables.Add(dt);
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR7DataSet(string StrQ1, string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty; DataTable dt_chart = new DataTable();
            DataColumn col_inspectorname = new DataColumn("P710", typeof(string)); dt.Columns.Add(col_inspectorname);
            DataColumn col_inspectornum = new DataColumn("P701", typeof(int)); dt.Columns.Add(col_inspectornum); dt_chart = dt.Clone();
            DataColumn col_tasknum = new DataColumn("P702", typeof(int)); dt.Columns.Add(col_tasknum);
            DataColumn col_inspectorid = new DataColumn("P700", typeof(string)); dt.Columns.Add(col_inspectorid);
            DataColumn col_finish = new DataColumn("P703", typeof(int)); dt.Columns.Add(col_finish);
            DataColumn col_rectime = new DataColumn("P704", typeof(string)); dt.Columns.Add(col_rectime);
            DataColumn col_respond = new DataColumn("P705", typeof(int)); dt.Columns.Add(col_respond);
            DataColumn col_processed = new DataColumn("P706", typeof(int)); dt.Columns.Add(col_processed);
            DataColumn col_avarage = new DataColumn("P707", typeof(decimal)); dt.Columns.Add(col_avarage);
            DataColumn col_max = new DataColumn("P708", typeof(decimal)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P709", typeof(decimal)); dt.Columns.Add(col_min);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; bool first_time = true; DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet();
            string TimeQuery = string.Empty; COUNT = 0;
            //if (App.Session.DBType == 2)
            //    sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} B,T_31_020_{0} D WHERE B.C009='Y'AND B.C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 IN (SELECT C001 FROM T_11_202_{0} WHERE C002=3102005 AND C001>1060000000000000000 AND C001<1070000000000000000 )) AND B.C002 IN (SELECT C002 FROM T_31_022_{0} WHERE C001=D.C001) AND "
            //    + ColName + " >= '" + ADT.BeginDateTime[t] + "' AND " + ColName + " <= '" + ADT.EndDateTime[t] + "' " + StrQ, Session.RentInfo.Token);
            //else
            //    sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} B,T_31_020_{0} D WHERE B.C009='Y'AND B.C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 IN (SELECT C001 FROM T_11_202_{0} WHERE C002=3102005 AND C001>1060000000000000000 AND C001<1070000000000000000 )) AND B.C002 IN (SELECT C002 FROM T_31_022_{0} WHERE C001=D.C001) AND "
            //        + ColName + " >= TO_DATE('" + ADT.BeginDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') " + StrQ, Session.RentInfo.Token);

            if (Session.DBType == 2)
                sql = string.Format("SELECT COUNT(0) FROM T_31_021_{0} B,T_31_020_{0} D WHERE D.C005='N' AND B.C004='Q' AND D.C001=B.C001 AND "
                + ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' {1}", Session.RentInfo.Token, StrQ1);
            else
                sql = string.Format("SELECT COUNT(0) FROM T_31_021_{0} B,T_31_020_{0} D WHERE D.C005='N' AND B.C004='Q' AND D.C001=B.C001 AND "
                    + ColName + " >= TO_DATE('" + ADT.BeginDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') {1}", Session.RentInfo.Token, StrQ1);

            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} B.C001,B.C002,D.C021,D.C008,D.C017,D.C010,B.C003," + ColName
                                 + " P107 FROM T_31_021_{0} B,T_31_020_{0} D WHERE D.C005='N' AND B.C004='Q' AND D.C001=B.C001 AND "
                                 + ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")
                                 + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")
                                 + "' {2} ORDER BY C001,P107"
                                 , Session.RentInfo.Token, GetDataNum, " AND B.C001 >" + AfterRowNum + StrQ1);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C003,C021,C008,C010,C017,P107 FROM (SELECT B.C001,B.C002,D.C021,D.C008,D.C017,D.C010,B.C003,{1} P107 FROM T_31_021_{0} B,T_31_020_{0} D WHERE D.C005='N' AND B.C004='Q' AND D.C001=B.C001 AND {1} >= TO_DATE('"
                        + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND {1} <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') "
                        + "{2} ORDER BY B.C001,{1})WHERE ROWNUM<=" + GetDataNum + " AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, ColName, StrQ1);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(7):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable MarkDT = new DataTable();
                DataSetMark.Tables.Add(MarkDT);
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                List<string> inspectorname = new List<string>(); List<string> NameList = new List<string>();
                List<string> Task = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    string name = dr["C002"].ToString();
                    string find_name = inspectorname.FirstOrDefault(p => p == name);
                    if (find_name == null)
                    {
                        inspectorname.Add(name);
                        NameList.Add(dr["C003"].ToString());
                    }
                }
                TransData(inspectorname, dt, "P700", 1); TransData(NameList, dt, "P710", 2);
                foreach (DataRow row in dt.Rows)
                {
                    int task = 0; int inspect = 0; int finish = 0; int rec = 0;
                    int respond = 0; int processed = 0; decimal max = 0; decimal min = 1000 * Multiple; decimal score = 0;
                    DataSet ds_RecorderSorce = new DataSet();
                    List<DataRow> RowList = _ds.Tables[0].Select(string.Format("C002={0}", row["P700"].ToString())).ToList();
                    for (int i = 0; i < RowList.Count; i++)
                    {
                        task++;
                        inspect += int.Parse(RowList[i]["C010"].ToString());
                        if (RowList[i]["C017"].ToString() == "Y")
                        { finish++; }
                        rec += int.Parse(RowList[i]["C021"].ToString());
                        string Time = Convert.ToDateTime(RowList[i]["P107"]).ToLocalTime().ToString();
                        row["P107"] = AbsoluteDate(Time);
                        //for (int p = 0; p < Task.Count; p++)
                        //{
                        string Sql = string.Format("SELECT C005,C014,C004,C001,C006,C002 FROM T_31_008_{0} WHERE C010>=3 AND C010<=6 AND C005={1} AND C002 IN (SELECT C002 FROM T_31_022_{0} WHERE C001={2} AND C003='N') {3}"
                                , Session.RentInfo.Token, RowList[i]["C002"].ToString(), RowList[i]["C001"], StrQ);
                        if (i == 0)
                        {
                            ds_RecorderSorce = GetDataSetFromDB(Sql, 100);
                        }
                        else
                        {
                            DataSet dsT = GetDataSetFromDB(Sql, 100);
                            foreach (DataRow drT in dsT.Tables[0].Rows)
                            {
                                ds_RecorderSorce.Tables[0].ImportRow(drT);
                            }
                        }
                    }
                    
                    if (ds_RecorderSorce != null && ds_RecorderSorce.Tables.Count != 0)
                    {
                        DataTable DT_Final = ds_RecorderSorce.Tables[0].Clone(); DataTable dtM = DT_Final.Clone();
                        List<string> RecorderID = new List<string>();
                        foreach (DataRow dr in ds_RecorderSorce.Tables[0].Rows)
                        {
                            string name = dr["C002"].ToString();
                            string findname = RecorderID.FirstOrDefault(p => p == name);
                            if (findname == null)
                            {
                                RecorderID.Add(name);
                            }
                        }
                        for (int k = 0; k < RecorderID.Count; k++)
                        {
                            List<DataRow> RecorderRows = ds_RecorderSorce.Tables[0].Select(string.Format("C002={0}", RecorderID[k])).ToList();
                            DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                            for (int q = 0; q < RecorderRows.Count; q++)
                            {
                                DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["C006"].ToString());
                                TimeSpan TS = DTRow - DTTemp;
                                if (TS.TotalSeconds > 0)
                                {
                                    DTTemp = DTRow;
                                    Mark = q;
                                }
                            }
                            if (Mark != -1)
                            {
                                DT_Final.ImportRow(RecorderRows[Mark]);
                            }
                        }
                        foreach (DataRow markrow in DT_Final.Rows)
                        {
                            DataSetMark.Tables[0].ImportRow(markrow);
                        }
                        if (DT_Final != null && DT_Final.Rows.Count != 0)
                        {
                            foreach (DataRow dr_RS in DT_Final.Rows)
                            {
                                if (dr_RS["C014"].ToString() != "0")
                                {
                                    respond += 1;
                                }
                                if (dr_RS["C014"].ToString() == "2")
                                //for (int k = 0; k < ds_31019.Tables[0].Rows.Count; k++)
                                //{
                                //    if (dr_RS["C001"].ToString() == ds_31019.Tables[0].Rows[k]["C002"].ToString())
                                //    {
                                //        if (Convert.ToInt32(ds_31019.Tables[0].Rows[k]["C009"].ToString()) == 6 ||
                                //            Convert.ToInt32(ds_31019.Tables[0].Rows[k]["C009"].ToString()) == 5)
                                {
                                    processed += 1;
                                }
                                //    }
                                //}
                                if (decimal.Parse(dr_RS["C004"].ToString()) > max)
                                {
                                    max = decimal.Parse(dr_RS["C004"].ToString());
                                }
                                if (decimal.Parse(dr_RS["C004"].ToString()) < min)
                                {
                                    min = decimal.Parse(dr_RS["C004"].ToString());
                                }
                                score += decimal.Parse(dr_RS["C004"].ToString());
                            }
                        }
                    }
                    
                    row["P701"] = inspect; row["P702"] = task; row["P703"] = finish; row["P704"] = TransTimeForm(rec);
                    row["P705"] = respond; row["P706"] = processed; row["P708"] = decimal.Round(max, NumAfterPoint);
                    if (min == 1000 * Multiple)
                        row["P709"] = 0;
                    else
                        row["P709"] = decimal.Round(min, NumAfterPoint);
                    if (inspect != 0)
                    {
                        row["P707"] = decimal.Round((score / inspect), NumAfterPoint);
                    }
                    else
                    {
                        row["P707"] = 0;
                    }
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (row["P700"].ToString() == dr_ap["C001"].ToString())
                        {
                            row["P700"] = DecryptString(dr_ap["C002"].ToString());
                            row["P710"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                }
                #endregion
                ds.Tables.Add(dt);
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR8DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); DataTable dt_chart = new DataTable(); string sql = string.Empty;
            DataColumn col_agentname = new DataColumn("P800", typeof(string)); dt.Columns.Add(col_agentname);
            DataColumn col_sd = new DataColumn("P806", typeof(decimal)); dt.Columns.Add(col_sd); dt_chart = dt.Clone();
            DataColumn col_agentid = new DataColumn("P801", typeof(string)); dt.Columns.Add(col_agentid);
            DataColumn col_part = new DataColumn("P802", typeof(string)); dt.Columns.Add(col_part);
            DataColumn col_max = new DataColumn("P804", typeof(decimal)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P805", typeof(decimal)); dt.Columns.Add(col_min);
            DataColumn col_avarage = new DataColumn("P803", typeof(string)); dt.Columns.Add(col_avarage);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            DataSet DS_temp = new DataSet(); bool first_time = true; string TimeQuery = string.Empty; COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet();
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = "SELECT COUNT(0) FROM T_31_008_" + Session.RentInfo.Token + " WHERE C009='Y' AND " + TimeQuery + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100); int PreRowNum = GetDataNum; string AfterRowNum = "0";
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu;
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C012,C013,C004,C006 P107 FROM T_31_008_{0} WHERE  {2} AND C009='Y' ORDER BY C001,P107"
                            , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C012,C013,C004,C006 P107 FROM (SELECT A.* FROM T_31_008_{0} A WHERE  "
                         + TimeQuery + StrQ + " AND C009='Y' ORDER BY C001,{2}) WHERE ROWNUM<={1} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(8):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0]; DataTable dtM = _dt.Clone();
                foreach (DataRow drM in _dt.Rows)
                {
                    dtM.ImportRow(drM);
                }
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                List<string> AgentList8 = new List<string>(); List<string> PartList = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    bool Isfind = true;
                    string findagent = dr["C013"].ToString();
                    string findpart = dr["C012"].ToString();
                    for (int i = 0; i < AgentList8.Count; i++)
                    {
                        if (findagent == AgentList8[i] && findpart == PartList[i])
                        {
                            Isfind = false; break;
                        }
                    }
                    if (Isfind)
                    {
                        AgentList8.Add(dr["C013"].ToString());
                        PartList.Add(dr["C012"].ToString());
                    }
                }
                TransData(AgentList8, dt, "P801", 1); TransData(PartList, dt, "P802", 2);

                for (int i = 0; i < AgentList8.Count; i++)
                {
                    List<string> RecorderID = new List<string>(); DataTable DT_Final = _dt.Clone();
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (AgentList8[i] == dr["C013"].ToString() && PartList[i] == dr["C012"].ToString())
                        {
                            string findrecord = RecorderID.FirstOrDefault(p => p == dr["C002"].ToString());
                            if (findrecord == null)
                            {
                                RecorderID.Add(dr["C002"].ToString());
                            }
                        }
                    }
                    //for (int k = 0; k < RecorderID.Count; k++)
                    //{
                    //    List<DataRow> RecorderRows = _dt.Select(string.Format("C002={0} AND C012={2} AND C013={1}", RecorderID[k], AgentList8[i],PartList[i])).ToList();
                    //    DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                    //    for (int q = 0; q < RecorderRows.Count; q++)
                    //    {
                    //        DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["P107"].ToString());
                    //        TimeSpan TS = DTRow - DTTemp;
                    //        if (TS.TotalSeconds > 0)
                    //        {
                    //            DTTemp = DTRow;
                    //            Mark = q;
                    //        }
                    //    }
                    //    if (Mark != -1)
                    //        DT_Final.ImportRow(RecorderRows[Mark]);
                    //}

                    decimal max = 0; decimal min = 1000 * Multiple; decimal all = 0; int count = 0; List<decimal> numlist = new List<decimal>(); decimal sd = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (dr["C013"].ToString() == AgentList8[i] && dr["C012"].ToString() == PartList[i])
                        {
                            count++; all += decimal.Parse(dr["C004"].ToString()); numlist.Add(decimal.Parse(dr["C004"].ToString()));
                            string time = string.Empty;

                            DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                            time = DTTEMP.ToString("yyyyMMddHHmmss");

                            dt.Rows[i]["P107"] = AbsoluteDate(time);
                            if (decimal.Parse(dr["C004"].ToString()) > max)
                            {
                                max = decimal.Parse(dr["C004"].ToString());
                            }
                            if (decimal.Parse(dr["C004"].ToString()) < min)
                            {
                                min = decimal.Parse(dr["C004"].ToString());
                            }
                        }
                    }
                    if (count != 0)
                    {
                        decimal avage = all / count;
                        dt.Rows[i]["P803"] = decimal.Round(avage, NumAfterPoint);
                        for (int j = 0; j < numlist.Count; j++)
                        {
                            decimal sd_ = (numlist[j] - all / count);
                            sd_ *= sd_;
                            sd += sd_;
                        }
                        sd = sd / count; double tempNum = Math.Sqrt((double)sd); dt.Rows[i]["P806"] = decimal.Round(decimal.Parse(tempNum.ToString()), NumAfterPoint);
                    }
                    else
                    {
                        dt.Rows[i]["P803"] = 0; dt.Rows[i]["P806"] = 0;
                    }
                    dt.Rows[i]["P804"] = decimal.Round(max, NumAfterPoint);
                    if (min != 1000 * Multiple)
                        dt.Rows[i]["P805"] = decimal.Round(min, NumAfterPoint);
                    else
                        dt.Rows[i]["P805"] = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataRow dr_agent in ds_agent.Tables[0].Rows)
                        {
                            if (dr["P801"].ToString() == dr_agent["C001"].ToString())
                            {
                                dr["P800"] = dr_agent["C018"];
                                dr["P801"] = dr_agent["C017"];
                            }
                        }
                        foreach (DataRow dr_aprt in ds_ap.Tables[0].Rows)
                        {
                            if (dr["P802"].ToString() == dr_aprt["C001"].ToString())
                            {
                                dr["P802"] = DecryptString(dr_aprt["C002"].ToString());
                            }
                        }
                    }
                }
                #endregion
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(dt); ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR9DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty;
            DataColumn col_inspectorid = new DataColumn("P900", typeof(string)); dt.Columns.Add(col_inspectorid);
            DataColumn col_inspectorname = new DataColumn("P908", typeof(string)); dt.Columns.Add(col_inspectorname);
            DataColumn col_inspectornum = new DataColumn("P901", typeof(int)); dt.Columns.Add(col_inspectornum);
            DataColumn col_rectime = new DataColumn("P902", typeof(string)); dt.Columns.Add(col_rectime);
            DataColumn col_respond = new DataColumn("P903", typeof(int)); dt.Columns.Add(col_respond);//处理
            DataColumn col_processed = new DataColumn("P904", typeof(int)); dt.Columns.Add(col_processed);
            DataColumn col_resppercnet = new DataColumn("P905", typeof(double)); dt.Columns.Add(col_resppercnet);
            DataColumn col_processedpercent = new DataColumn("P906", typeof(double)); dt.Columns.Add(col_processedpercent);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            DataColumn col_scorename = new DataColumn("P907", typeof(string)); dt.Columns.Add(col_scorename);
            DataSet DS_temp = new DataSet(); bool first_time = true; string TimeQuery = string.Empty; COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet();
            string BT = ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"); string ET = ADT.EndDateTime[t].ToString("yyyyMMddHHmmss");
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} WHERE C009='Y' AND C010<7 AND C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 IN (SELECT C001 FROM T_11_202_{0} WHERE C002=3102005 AND C001>1060000000000000000 AND C001<1070000000000000000 )) AND " + TimeQuery + StrQ, Session.RentInfo.Token);
            DataSet ds_temp = GetDataSetFromDB(sql, 100); int PreRowNum = GetDataNum; string AfterRowNum = "0";
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu;
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C003,C005,C014,C006 P107 FROM T_31_008_{0} WHERE C010<7 AND C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 IN (SELECT C001 FROM T_11_202_{0} WHERE C002=3102005 AND C001>1060000000000000000 AND C001<1070000000000000000 ))AND {2} ORDER BY C001,P107"
                                         , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C003,C005,C014,P107 FROM(SELECT C001,C002,C003,C005,C014,C006 P107 FROM T_31_008_{0} WHERE  C010<7 AND C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 IN (SELECT C001 FROM T_11_202_{0} WHERE C002=3102005 AND C001>1060000000000000000 AND C001<1070000000000000000 )) AND "
                        + TimeQuery + StrQ + " ORDER BY C001,P107 )WHERE ROWNUM<={1} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(9):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable dtM = _ds.Tables[0].Clone();
                foreach (DataRow drM in _ds.Tables[0].Rows)
                {
                    dtM.ImportRow(drM);
                }
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                List<string> inspectorname = new List<string>(); List<string> GnList = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    bool flag = true; string name = dr["C005"].ToString();
                    for (int i = 0; i < inspectorname.Count; i++)
                    {
                        if (name == inspectorname[i])
                        {
                            for (int j = 0; j < GnList.Count; j++)
                            {
                                if (GnList[j] == dr["C003"].ToString())
                                { flag = false; break; }
                            }

                        }
                    }
                    if (flag)
                    {
                        inspectorname.Add(name); GnList.Add(dr["C003"].ToString());
                    }
                }
                TransData(inspectorname, dt, "P900", 1); TransData(GnList, dt, "P907", 2);

                foreach (DataRow row in dt.Rows)
                {
                    List<string> RecorderID = new List<string>(); DataTable DT_Final = _ds.Tables[0].Clone();
                    foreach (DataRow dr in _ds.Tables[0].Rows)
                    {
                        if (row["P900"].ToString() == dr["C005"].ToString() && row["P907"].ToString() == dr["C003"].ToString())
                        {
                            bool Flag = true;
                            for (int j = 0; j < RecorderID.Count; j++)
                            {
                                if (RecorderID[j] == dr["C002"].ToString())
                                {
                                    Flag = false;
                                    break;
                                }
                            }
                            if (Flag)
                            {
                                RecorderID.Add(dr["C002"].ToString());
                            }
                        }
                    }
                    for (int k = 0; k < RecorderID.Count; k++)
                    {
                        List<DataRow> RecorderRows = _ds.Tables[0].Select(string.Format("C002={0} AND C003={2} AND C005={1}", RecorderID[k], row["P900"].ToString(), row["P907"].ToString())).ToList();
                        DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                        for (int q = 0; q < RecorderRows.Count; q++)
                        {
                            DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["P107"].ToString());
                            TimeSpan TS = DTRow - DTTemp;
                            if (TS.TotalSeconds > 0)
                            {
                                DTTemp = DTRow;
                                Mark = q;
                            }
                        }
                        if (Mark != -1)
                            DT_Final.ImportRow(RecorderRows[Mark]);
                    }

                    //List<DataRow> RowList = _ds.Tables[0].Select(string.Format("C005={0} AND C003={1}", row["P900"], row["P907"])).ToList();
                    decimal inspect = 0; int rec = 0; decimal respond = 0; decimal processed = 0;
                    foreach (DataRow dr in DT_Final.Rows)
                    {
                        if (row["P900"].ToString() == dr["C005"].ToString() && row["P907"].ToString() == dr["C003"].ToString())
                        {
                            string time = string.Empty;

                            DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                            time = DTTEMP.ToString("yyyyMMddHHmmss");

                            row["P107"] = AbsoluteDate(time);

                            //inspect += int.Parse(dr["C008"].ToString()); 
                            inspect++;
                            rec += Convert.ToInt32(GetDataFromRecorderTable(BT, ET, dr["C002"].ToString(), "C012"));

                            if (dr["C014"].ToString() != "0")
                            {
                                respond += 1;
                                if (dr["C014"].ToString() == "2")
                                //for (int j = 0; j < ds_31019.Tables[0].Rows.Count; j++)
                                //{
                                //    if (ds_31019.Tables[0].Rows[j]["C002"].ToString() == dr["C001"].ToString())
                                //    {
                                //        if ((ds_31019.Tables[0].Rows[j]["C009"].ToString()) == "6" ||
                                //       (ds_31019.Tables[0].Rows[j]["C009"].ToString()) == "5")
                                {
                                    processed += 1;
                                    //break;
                                }
                                //    }
                                //}
                            }
                        }
                    }
                    row["P901"] = inspect; row["P902"] = Converter.Second2Time(rec);
                    row["P903"] = respond; row["P904"] = processed;
                    if (inspect != 0)
                    {
                        row["P905"] = decimal.Round(respond / inspect, NumAfterPoint); row["P906"] = decimal.Round(processed / inspect, NumAfterPoint);
                    }
                    else
                    {
                        row["P905"] = 0; row["P906"] = 0;
                    }
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (row["P900"].ToString() == dr_ap["C001"].ToString())
                        {
                            row["P900"] = DecryptString(dr_ap["C002"].ToString());
                            row["P908"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                }
                for (int i = 0; i < GnList.Count; i++)
                {
                    foreach (DataRow dr_gn in ds_gn.Tables[0].Rows)
                    {
                        if (GnList[i] == dr_gn["C001"].ToString())
                        {
                            dt.Rows[i]["P907"] = dr_gn["C002"];
                        }
                    }
                }
                #endregion
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR10DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataSet DS_temp = new DataSet(); DataTable dt = new DataTable();
            bool first_time = true; string sql = string.Empty;
            DataColumn col_agentid = new DataColumn("P1001", typeof(string)); dt.Columns.Add(col_agentid);
            DataColumn col_agentname = new DataColumn("P1002", typeof(string)); dt.Columns.Add(col_agentname);
            DataColumn col_part = new DataColumn("P1003", typeof(string)); dt.Columns.Add(col_part);
            DataColumn col_inspectnum = new DataColumn("P1004", typeof(int)); dt.Columns.Add(col_inspectnum);
            DataColumn col_avarage = new DataColumn("P1005", typeof(decimal)); dt.Columns.Add(col_avarage);
            DataColumn col_max = new DataColumn("P1006", typeof(decimal)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P1007", typeof(decimal)); dt.Columns.Add(col_min);
            DataColumn col_appeal = new DataColumn("P1008", typeof(int)); dt.Columns.Add(col_appeal);
            DataColumn col_appealpercent = new DataColumn("P1009", typeof(decimal)); dt.Columns.Add(col_appealpercent);
            DataColumn col_handle = new DataColumn("P1010", typeof(int)); dt.Columns.Add(col_handle);
            DataColumn col_handlepercent = new DataColumn("P1011", typeof(decimal)); dt.Columns.Add(col_handlepercent);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); string TimeQuery = string.Empty; COUNT = 0;
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = "C006 >= TO_DATE('" + ADT.BeginDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') AND C006 <= TO_DATE('" + ADT.EndDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') ";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} WHERE  C009='Y' AND " + TimeQuery + StrQ, Session.RentInfo.Token);
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu;
            int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C013,C004,C012,C014,C006 P107 FROM T_31_008_{0} WHERE {2} AND C009='Y' ORDER BY C001,P107"
                                      , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C013,C004,C012,C014,P107 FROM (SELECT C001,C002,C013,C004,C012,C014,C006 P107 FROM T_31_008_{0} WHERE  "
                         + TimeQuery + StrQ + " AND C009='Y' ORDER BY C001,P107) WHERE ROWNUM<={1} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(10):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable dtM = _ds.Tables[0].Clone();
                foreach (DataRow drM in _ds.Tables[0].Rows)
                {
                    dtM.ImportRow(drM);
                }
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据

                List<string> AgentidList = new List<string>(); List<string> PartList = new List<string>();
                foreach (DataRow dr_ in _ds.Tables[0].Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < AgentidList.Count; i++)
                    {
                        if (AgentidList[i] == dr_["C013"].ToString())
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        AgentidList.Add(dr_["C013"].ToString());
                        PartList.Add(dr_["C012"].ToString());
                    }
                }
                TransData(AgentidList, dt, "P1001", 1); TransData(PartList, dt, "P1003", 2);
                for (int i = 0; i < AgentidList.Count; i++)
                {

                    List<string> RecorderID = new List<string>(); DataTable DT_Final = _ds.Tables[0].Clone();
                    //foreach (DataRow dr in _ds.Tables[0].Rows)
                    //{
                    //    if (AgentidList[i] == dr["C013"].ToString() && PartList[i] == dr["C012"].ToString())
                    //    {
                    //        bool Flag = true;
                    //        for (int j = 0; j < RecorderID.Count; j++)
                    //        {
                    //            if (RecorderID[j] == dr["C002"].ToString())
                    //            {
                    //                Flag = false;
                    //                break;
                    //            }
                    //        }
                    //        if (Flag)
                    //        {
                    //            RecorderID.Add(dr["C002"].ToString());
                    //        }
                    //    }
                    //}
                    //for (int k = 0; k < RecorderID.Count; k++)
                    //{
                    //    List<DataRow> RecorderRows = _ds.Tables[0].Select(string.Format("C002={0} AND C012={2} AND C013={1}", RecorderID[k], AgentidList[i], PartList[i])).ToList();
                    //    DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                    //    for (int q = 0; q < RecorderRows.Count; q++)
                    //    {
                    //        DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["P107"].ToString());
                    //        TimeSpan TS = DTRow - DTTemp;
                    //        if (TS.TotalSeconds > 0)
                    //        {
                    //            DTTemp = DTRow;
                    //            Mark = q;
                    //        }
                    //    }
                    //    if (Mark != -1)
                    //        DT_Final.ImportRow(RecorderRows[Mark]);
                    //}

                    bool isFind = false; bool IsFindPart = false;
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (AgentidList[i] == dr_ap["C001"].ToString())
                        {
                            dt.Rows[i]["P1001"] = dr_ap["C017"];
                            dt.Rows[i]["P1002"] = dr_ap["C018"];
                            if (IsFindPart)
                            { break; }
                            isFind = true;
                        }
                        if (PartList[i] == dr_ap["C011"].ToString())
                        {
                            dt.Rows[i]["P1003"] = dr_ap["C012"];
                            if (isFind)
                            { break; } IsFindPart = true;
                        }
                    }
                    decimal num = 0; decimal max = 0; decimal min = 1000 * Multiple; decimal all = 0; decimal appeal = 0; decimal handle = 0;
                    foreach (DataRow dr_ in _ds.Tables[0].Rows)
                    {
                        if (dr_["C013"].ToString() == AgentidList[i])
                        {
                            num++; all += decimal.Parse(dr_["C004"].ToString()); string time = string.Empty;

                            DateTime DTTEMP = Convert.ToDateTime(dr_["P107"].ToString()).ToLocalTime();
                            time = DTTEMP.ToString("yyyyMMddHHmmss");
                            dt.Rows[i]["P107"] = AbsoluteDate(time);

                            if (decimal.Parse(dr_["C004"].ToString()) > max)
                            {
                                max = decimal.Parse(dr_["C004"].ToString());
                            }
                            if (decimal.Parse(dr_["C004"].ToString()) < min)
                            {
                                min = decimal.Parse(dr_["C004"].ToString());
                            }
                            if (dr_["C014"].ToString() != "0")
                            {
                                appeal++;
                                if (dr_["C014"].ToString() == "2")
                                //for (int k = 0; k < ds_31019.Tables[0].Rows.Count; k++)
                                //{
                                //    if (dr_["C001"].ToString() == ds_31019.Tables[0].Rows[k]["C002"].ToString())
                                //    {
                                //        if (Convert.ToInt32(ds_31019.Tables[0].Rows[k]["C009"].ToString()) == 6 ||
                                //            Convert.ToInt32(ds_31019.Tables[0].Rows[k]["C009"].ToString()) == 5)
                                {
                                    handle++;
                                }
                                //    }
                                //}
                            }
                        }
                    }
                    dt.Rows[i]["P1004"] = num; dt.Rows[i]["P1008"] = appeal; dt.Rows[i]["P1010"] = handle;
                    if (min != 1000 * Multiple)
                        dt.Rows[i]["P1007"] = decimal.Round(min, NumAfterPoint);
                    else
                        dt.Rows[i]["P1007"] = 0;
                    dt.Rows[i]["P1006"] = decimal.Round(max, NumAfterPoint);
                    if (num != 0)
                    {
                        dt.Rows[i]["P1005"] = decimal.Round((all / num), NumAfterPoint);
                        dt.Rows[i]["P1009"] = decimal.Round(appeal / num, NumAfterPoint);
                        dt.Rows[i]["P1011"] = decimal.Round(handle / num, NumAfterPoint);
                    }
                    else
                    {
                        dt.Rows[i]["P1005"] = 0; dt.Rows[i]["P1009"] = 0; dt.Rows[i]["P1011"] = 0;
                    }
                }
                #endregion
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR11DataSet(string StrQ, int t)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); DataTable dt_chart = new DataTable(); string sql = string.Empty;
            DataColumn col_agentname = new DataColumn("P1101", typeof(string)); dt.Columns.Add(col_agentname);
            DataColumn col_sd = new DataColumn("P1107", typeof(decimal)); dt.Columns.Add(col_sd);
            DataColumn col_agentid = new DataColumn("C039", typeof(string)); dt.Columns.Add(col_agentid);
            DataColumn col_part = new DataColumn("P1102", typeof(string)); dt.Columns.Add(col_part);
            DataColumn col_max = new DataColumn("P1103", typeof(string)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P1104", typeof(string)); dt.Columns.Add(col_min);
            DataColumn col_avarage = new DataColumn("P1105", typeof(string)); dt.Columns.Add(col_avarage);
            DataColumn col_Call = new DataColumn("P1106", typeof(int)); dt.Columns.Add(col_Call);
            DataColumn col_absdate = new DataColumn("C005", typeof(string)); dt.Columns.Add(col_absdate);
            DataColumn col_Cagent = new DataColumn("C039", typeof(string)); dt_chart.Columns.Add(col_Cagent);
            DataColumn col_timelen = new DataColumn("P1107", typeof(decimal)); dt_chart.Columns.Add(col_timelen);
            DataSet DS_temp = new DataSet(); bool first_time = true; COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            List<string> TableName = TableSec("T_21_001", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"));
            DataSet _ds = new DataSet(); int CiShuSum = 0;
            for (int s = 0; s < TableName.Count; s++)
            {
                int AfterRowNum = 0; bool IsNull = true;
                do
                {
                    IsNull = false;
                    if (Session.DBType == 2)
                    {
                        sql = string.Format("SELECT TOP {1} A.C001,A.C039,A.C012,A.C013,A.C005 FROM {0} A WHERE A.C001>{5} AND A.C005 >= '{2}' AND A.C005 <= '{3}' {4}  ORDER BY A.C001,A.C005"
                             , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), StrQ, AfterRowNum);
                    }
                    else
                    {
                        sql = string.Format("SELECT C001,C039,C012,C013,C005 FROM (SELECT A.* FROM {0} A WHERE A.C005 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")
                            + "','YYYY-MM-DD HH24:MI:SS') AND A.C005 <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')"
                            + StrQ + " ORDER BY A.C001,A.C005) WHERE C001>" + AfterRowNum + " AND ROWNUM<=" + GetDataNum + " ORDER BY C001,C005 DESC ", TableName[s]);
                    }
                    if (first_time)
                    {
                        _ds = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (_ds == null)
                        {
                            continue;
                        }
                        if (_ds.Tables != null && _ds.Tables.Count != 0)
                            if (_ds.Tables[0].Rows.Count != 0)
                            {
                                IsNull = true; CiShuSum++; first_time = false;
                            }
                    }
                    else
                    {
                        DS_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (DS_temp == null)
                        {
                            continue;
                        }
                        if (DS_temp.Tables == null)
                        {
                            continue;
                        }
                        else if (DS_temp.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++;

                            if (DS_temp.Tables[0].Rows.Count != 0)
                            {
                                if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                    foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                        _ds.Tables[0].ImportRow(dr);
                            }
                        }
                    }
                    if (IsNull)
                    {
                        COUNT = _ds.Tables[0].Rows.Count - 1;
                        AfterRowNum = Convert.ToInt32(_ds.Tables[0].Rows[COUNT]["C001"]);
                    }
                }
                while (IsNull);
            }
            WriteLog(string.Format("(11):{0}", sql));
            if (CiShuSum != 0)
            {

                DataTable _dt = _ds.Tables[0];
                #region 统计数据
                List<string> AgentList11 = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < AgentList11.Count; i++)
                    {
                        if (AgentList11[i] == dr["C039"].ToString())
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        AgentList11.Add(dr["C039"].ToString());
                    }
                }
                TransData(AgentList11, dt, "C039", 1);
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (dr["C039"].ToString() == dr_ap["C017"].ToString())
                        {
                            dr["P1101"] = dr_ap["C018"]; dr["P1102"] = dr_ap["C012"];
                        }
                    }
                }

                for (int i = 0; i < AgentList11.Count; i++)
                {
                    int count = 0; int all = 0; int max = 0; int min = 1000 * Multiple; decimal sd = 0; int avarage = 0;
                    string max_text = "00:00:00"; string min_text = "00:00:00"; List<int> numlist = new List<int>();
                    foreach (DataRow dr in _ds.Tables[0].Rows)
                    {
                        if (dr["C039"].ToString() == AgentList11[i])
                        {
                            count++; all += Convert.ToInt32(dr["C012"]); numlist.Add(Convert.ToInt32(dr["C012"]));
                            string time = Convert.ToDateTime(dr["C005"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            dt.Rows[i]["C005"] = AbsoluteDate(time);
                            if (int.Parse(dr["C012"].ToString()) >= max)
                            {
                                max = Convert.ToInt32(dr["C012"]); max_text = dr["C013"].ToString();
                            }
                            if (Convert.ToInt32(dr["C012"]) < min)
                            {
                                min = Convert.ToInt32(dr["C012"]); min_text = dr["C013"].ToString();
                            }
                        }
                    }
                    if (count != 0)
                    {
                        avarage = all / count;
                        for (int j = 0; j < numlist.Count; j++)
                        {
                            decimal sd_ = (numlist[j] - avarage);
                            sd_ *= sd_;
                            sd += sd_;
                        }
                        sd = sd / count; double tempNum = Math.Sqrt((double)sd);
                        if (avarage != 0)
                            dt.Rows[i]["P1107"] = decimal.Round((decimal)tempNum / avarage, NumAfterPoint);
                        else
                            dt.Rows[i]["P1107"] = 0;
                    }
                    else
                    {
                        dt.Rows[i]["P1107"] = 0;
                    }
                    dt.Rows[i]["P1106"] = count; dt.Rows[i]["P1105"] = TransTimeForm(avarage);
                    dt.Rows[i]["P1103"] = max_text; dt.Rows[i]["P1104"] = min_text;
                }
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (dt_chart.Rows[i]["C039"].ToString() == dr_ap["C017"].ToString())
                        {
                            dt_chart.Rows[i]["C039"] = dr_ap["C018"];
                        }
                    }
                }
                #endregion

                ds.Tables.Add(dt); ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR12DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataSet DS_temp = new DataSet(); DataTable dt = new DataTable(); bool first_time = true; string sql = string.Empty;
            DataColumn col_inspectname = new DataColumn("P1201", typeof(string)); dt.Columns.Add(col_inspectname);
            DataColumn col_inspectnum = new DataColumn("P1202", typeof(int)); dt.Columns.Add(col_inspectnum);
            DataColumn col_avarage = new DataColumn("P1203", typeof(decimal)); dt.Columns.Add(col_avarage);
            DataColumn col_max = new DataColumn("P1204", typeof(decimal)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P1205", typeof(decimal)); dt.Columns.Add(col_min);
            DataColumn col_sd = new DataColumn("P1206", typeof(decimal)); dt.Columns.Add(col_sd);
            DataColumn col_success = new DataColumn("P1207", typeof(int)); dt.Columns.Add(col_success);
            DataColumn col_inspectid = new DataColumn("C005", typeof(string)); dt.Columns.Add(col_inspectid);
            DataColumn col_gn = new DataColumn("C003", typeof(string)); dt.Columns.Add(col_gn);
            DataColumn col_applient = new DataColumn("C014", typeof(string)); dt.Columns.Add(col_applient);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate); COUNT = 0;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); string TimeQuery = string.Empty;
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = "SELECT COUNT(0) FROM T_31_008_" + Session.RentInfo.Token + " WHERE C010 IN (3,4,5,6) AND " + TimeQuery + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C005,C009,C004,C014,C003,C006 P107 FROM T_31_008_{0} WHERE  {2} AND C010 IN (3,4,5,6) ORDER BY C001,P107"
                                   , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C005,C009,C004,C014,C003,P107 FROM (SELECT C001,C002,C005,C009,C004,C014,C003,{1} P107 FROM T_31_008_{0} WHERE  "
                        + TimeQuery + StrQ + "AND C010 IN (3,4,5,6) ORDER BY C001,{1})WHERE ROWNUM<={2} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, ColName, GetDataNum);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(12):{0}", sql));
            if (CiShuSum != 0)
            {
                #region 统计数据
                List<string> InspectList = new List<string>(); List<string> GnList = new List<string>();
                DataTable dtM = _ds.Tables[0].Clone();
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                foreach (DataRow dr_ in _ds.Tables[0].Rows)
                {
                    dtM.ImportRow(dr_);
                    string findInspect = dr_["C005"].ToString();
                    string findGn = dr_["C003"].ToString(); bool Isfind = true;
                    for (int i = 0; i < InspectList.Count; i++)
                    {
                        if (findInspect == InspectList[i] && findGn == GnList[i])
                        {
                            Isfind = false; break;
                        }
                    }
                    if (Isfind)
                    {
                        InspectList.Add(dr_["C005"].ToString());
                        GnList.Add(dr_["C003"].ToString());
                    }
                }
                DataSetMark.Tables.Add(dtM);
                TransData(InspectList, dt, "C005", 1);
                for (int i = 0; i < InspectList.Count; i++)
                {
                    List<string> RecorderID = new List<string>();
                    List<DataRow> ds_RecorderSorce = _ds.Tables[0].Select(string.Format("C005={0} AND C003={1} ", InspectList[i], GnList[i])).ToList();
                    DataTable DT_Final = _ds.Tables[0].Clone();
                    foreach (DataRow dr in ds_RecorderSorce)
                    {
                        string findrecord = RecorderID.FirstOrDefault(p => p == dr["C002"].ToString());
                        if (findrecord == null)
                        {
                            RecorderID.Add(dr["C002"].ToString());
                        }
                    }
                    for (int k = 0; k < RecorderID.Count; k++)
                    {
                        DataRow[] RecorderRows = _ds.Tables[0].Select(string.Format("C002={0} AND C005={1} AND C003={2}", RecorderID[k], InspectList[i], GnList[i]));
                        DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                        for (int q = 0; q < RecorderRows.Count(); q++)
                        {
                            DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["P107"].ToString());
                            TimeSpan TS = DTRow - DTTemp;
                            if (TS.TotalSeconds > 0)
                            {
                                DTTemp = DTRow;
                                Mark = q;
                            }
                        }
                        if (Mark != -1)
                            DT_Final.ImportRow(RecorderRows[Mark]);
                    }
                    foreach (DataRow dr_gn in ds_gn.Tables[0].Rows)
                    {
                        if (GnList[i] == dr_gn["C001"].ToString())
                        {
                            dt.Rows[i]["C003"] = dr_gn["C002"].ToString(); break;
                        }
                    }
                    int num = DT_Final.Rows.Count; decimal max = 0; decimal min = 10000 * Multiple; decimal all = 0;
                    decimal appeal = 0; decimal success = 0; decimal sd = 0;
                    List<decimal> numlist = new List<decimal>();

                    for (int j = 0; j < num; j++)
                    {
                        DataRow dr = DT_Final.Rows[j];
                        Decimal score = decimal.Parse(dr["C004"].ToString());
                        all += score; string time = string.Empty; numlist.Add(score);
                        DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                        time = DTTEMP.ToString("yyyyMMddHHmmss");
                        dt.Rows[i]["P107"] = AbsoluteDate(time);
                        if (score > max)
                        {
                            max = score;
                        }
                        if (score < min)
                        {
                            min = score;
                        }
                        if (dr["C014"].ToString() != "0")
                        {
                            appeal++;
                            if (dr["C014"].ToString() == "2")
                            {
                                success++;
                            }
                        }
                    }
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (InspectList[i] == dr_ap["C001"].ToString())
                        {
                            dt.Rows[i]["C005"] = DecryptString(dr_ap["C002"].ToString());
                            dt.Rows[i]["P1201"] = DecryptString(dr_ap["C003"].ToString()); break;
                        }
                    }
                    dt.Rows[i]["P1202"] = num; dt.Rows[i]["C014"] = appeal; dt.Rows[i]["P1207"] = success;
                    if (min != 10000 * Multiple)
                        dt.Rows[i]["P1205"] = decimal.Round(min, NumAfterPoint);
                    else
                        dt.Rows[i]["P1205"] = 0;
                    dt.Rows[i]["P1204"] = decimal.Round(max, NumAfterPoint);
                    if (num != 0)
                    {
                        dt.Rows[i]["P1203"] = decimal.Round(all / num, NumAfterPoint);
                        for (int j = 0; j < numlist.Count; j++)
                        {
                            decimal sd_ = (numlist[j] - all / num);
                            sd_ *= sd_;
                            sd += sd_;
                        }
                        sd = sd / num; double tempNum = Math.Sqrt((double)sd); sd = (decimal)tempNum;
                        dt.Rows[i]["P1206"] = decimal.Round(sd * num / all, NumAfterPoint);
                    }
                    else
                    {
                        dt.Rows[i]["P1203"] = 0;
                        dt.Rows[i]["P1206"] = 0;
                    }
                }
                #endregion
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR13DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty; DataTable dt_chart = new DataTable();
            DataColumn col_inspectorname = new DataColumn("P710", typeof(string)); dt.Columns.Add(col_inspectorname);
            DataColumn col_finish = new DataColumn("P703", typeof(int)); dt.Columns.Add(col_finish); dt_chart = dt.Clone();
            DataColumn col_rectime = new DataColumn("P704", typeof(string)); dt.Columns.Add(col_rectime);
            DataColumn col_inspectorid = new DataColumn("P700", typeof(string)); dt.Columns.Add(col_inspectorid);
            DataColumn col_respond = new DataColumn("P705", typeof(int)); dt.Columns.Add(col_respond);
            DataColumn col_processed = new DataColumn("P706", typeof(int)); dt.Columns.Add(col_processed);
            DataColumn col_avarage = new DataColumn("P707", typeof(decimal)); dt.Columns.Add(col_avarage);
            DataColumn col_max = new DataColumn("P708", typeof(decimal)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P709", typeof(decimal)); dt.Columns.Add(col_min);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; bool first_time = true; string TimeQuery = string.Empty;
            DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet(); COUNT = 0;
            string BT = ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"); string ET = ADT.EndDateTime[t].ToString("yyyyMMddHHmmss");
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(BT, ET, "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} WHERE (C010<=2 OR C010=7) AND C009='Y' AND " + TimeQuery + StrQ, Session.RentInfo.Token);
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C002,C003,C004,C005,C010,C014,C006 P107 FROM T_31_008_{0} WHERE {2} AND (C010<=2 OR C010=7) AND C009='Y' ORDER BY C001,P107"
                               , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format(
                        "SELECT C001,C004,C005,C002,C014,P107 FROM (SELECT C001,C002,C003,C004,C005,C010,C014,C006 P107 FROM T_31_008_{0} WHERE  "
                        + TimeQuery + StrQ + " AND (C010<=2 OR C010=7) AND C009='Y' ORDER BY C001,{1})WHERE ROWNUM<=" + GetDataNum
                        + " AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(13):{0}", sql));
            if (CiShuSum != 0)
            {
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                DataTable dtM = _ds.Tables[0].Clone();
                List<string> inspectorname = new List<string>(); List<string> timeList = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    dtM.ImportRow(dr);
                    string name = dr["C005"].ToString();
                    string findname = inspectorname.FirstOrDefault(p => p == name);
                    if (findname == null)
                    {
                        inspectorname.Add(name);
                    }
                }
                DataSetMark.Tables.Add(dtM);
                TransData(inspectorname, dt, "P700", 1);
                foreach (DataRow row in dt.Rows)
                {
                    List<DataRow> TempRows = _ds.Tables[0].Select(string.Format("C005={0}", row["P700"]), "C010,P107").ToList();
                    //    List<string> RecorderID = new List<string>(); DataTable DT_Final = _ds.Tables[0].Clone();
                    //    //foreach (DataRow dr in TempRows)
                    //    DataTable ListTempRows = _ds.Tables[0].Clone();
                    //    int drnum = (TempRows.Count() - 1);
                    //    for (; drnum >= 0; drnum--)
                    //    {
                    //        //if (row["P700"].ToString() == dr["C005"].ToString())
                    //        DataRow dr = TempRows[drnum];
                    //        if (dr["C010"].ToString() == "2" || dr["C010"].ToString() == "4" || dr["C010"].ToString() == "6")
                    //        {
                    //            bool Flag = true;
                    //            for (int recordnum = 0; recordnum < ListTempRows.Rows.Count; recordnum++)
                    //            {
                    //                if (dr["C002"].ToString() == ListTempRows.Rows[recordnum]["C002"].ToString() &&
                    //                    dr["C003"].ToString() == ListTempRows.Rows[recordnum]["C003"].ToString())
                    //                {
                    //                    Flag = false;
                    //                    break;
                    //                }
                    //            }
                    //            if (Flag)
                    //            {
                    //                //ListTempRows.Add(dr);
                    //                ListTempRows.ImportRow(dr);
                    //            }
                    //        }
                    //    }
                    //for (int recordid = 0; recordid < ListTempRows.Rows.Count; recordid++)
                    //   //for (int recordid = 0; recordid < ListTempRows.Rows.Count; recordid++)
                    //   {
                    //       for (int tempnum = 0; tempnum < TempRows.Count(); tempnum++)
                    //       {
                    //           if (TempRows[tempnum]["C002"].ToString() == ListTempRows.Rows[recordid]["C002"].ToString() &&
                    //               TempRows[tempnum]["C003"].ToString() == ListTempRows.Rows[recordid]["C003"].ToString())
                    //           {
                    //               //TempRows[tempnum].Delete();
                    //               TempRows.Remove(TempRows[tempnum]);
                    //           }
                    //       }
                    //   }
                    //foreach (DataRow drTemp in TempRows)
                    //{
                    //    //ListTempRows.Add(drTemp);
                    //    ListTempRows.ImportRow(drTemp);
                    //}
                    //DataRow[] temp = TempRows.Select(string.Format("C002={0} AND C003={1}", Recordcode, Scroecode));

                    //for (int j = 0; j < RecorderID.Count; j++)
                    //{
                    //    if (RecorderID[j] == dr["C002"].ToString())
                    //    {
                    //        Flag = false;
                    //        break;
                    //    }
                    //}
                    //if (Flag)
                    //{
                    //    RecorderID.Add(dr["C002"].ToString());
                    //}

                    int task = 0; int rec = 0; List<string> task_list = new List<string>();
                    int respond = 0; int processed = 0; decimal max = 0; decimal min = 1000 * Multiple; decimal score = 0;
                    for (int Rnum = 0; Rnum < TempRows.Count; Rnum++)
                    //foreach (DataRow dr in _ds.Tables[0].Rows)
                    {
                        //if (row["P700"].ToString() == dr["C005"].ToString())
                        {
                            DataRow dr = TempRows[Rnum];
                            if (dr == null || dr.ItemArray == null || dr.ItemArray.Count() == 0) { continue; }
                            task++;
                            string recTime = GetDataFromRecorderTable(BT, ET, dr["C002"].ToString(), "C012");
                            if (recTime != null)
                                rec += Convert.ToInt32(recTime);
                            if (dr["C014"].ToString() != "0")
                            {
                                respond += 1;
                                if (dr["C014"].ToString() == "2")
                                {
                                    processed += 1;
                                }

                            }
                            if (decimal.Parse(dr["C004"].ToString()) > max)
                            {
                                max = decimal.Parse(dr["C004"].ToString());
                            }
                            if (decimal.Parse(dr["C004"].ToString()) < min)
                            {
                                min = decimal.Parse(dr["C004"].ToString());
                            }
                            score += decimal.Parse(dr["C004"].ToString()); string time = string.Empty;
                            DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                            time = DTTEMP.ToString("yyyyMMddHHmmss");
                            row["P107"] = AbsoluteDate(time);
                        }
                    }
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (row["P700"].ToString() == dr_ap["C001"].ToString())
                        {
                            row["P700"] = DecryptString(dr_ap["C002"].ToString());
                            row["P710"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                    row["P703"] = task; row["P704"] = Converter.Second2Time(rec);
                    row["P705"] = respond; row["P706"] = processed; row["P708"] = decimal.Round(max, NumAfterPoint);
                    if (min == 1000 * Multiple)
                        row["P709"] = 0;
                    else
                        row["P709"] = decimal.Round(min, NumAfterPoint);
                    if (task != 0)
                    {
                        row["P707"] = decimal.Round((score / task), NumAfterPoint);
                    }
                    else
                    {
                        row["P707"] = 0;
                    }
                }
                #endregion
                ds.Tables.Add(dt);
                DataRow[] DR = dt.Select();
                for (int i = 0; i < DR.Length; i++)
                {
                    dt_chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(dt_chart);
            }
            return ds;
        }

        public DataSet GetR14DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty;
            DataColumn col_inspectorname = new DataColumn("P1401", typeof(string)); dt.Columns.Add(col_inspectorname);
            DataColumn col_inspectornum = new DataColumn("P1400", typeof(string)); dt.Columns.Add(col_inspectornum);
            DataColumn col_tasknum = new DataColumn("P1402", typeof(int)); dt.Columns.Add(col_tasknum);
            DataColumn col_finish = new DataColumn("P1403", typeof(int)); dt.Columns.Add(col_finish);
            DataColumn col_recnum = new DataColumn("P1404", typeof(int)); dt.Columns.Add(col_recnum);
            DataColumn col_rectime = new DataColumn("P1405", typeof(string)); dt.Columns.Add(col_rectime);
            DataColumn col_inspectime = new DataColumn("P1406", typeof(string)); dt.Columns.Add(col_inspectime);
            DataColumn col_timeratio = new DataColumn("P1407", typeof(decimal)); dt.Columns.Add(col_timeratio);
            DataColumn col_max = new DataColumn("P1408", typeof(int)); dt.Columns.Add(col_max);
            DataColumn col_min = new DataColumn("P1409", typeof(int)); dt.Columns.Add(col_min);
            DataColumn col_avarage = new DataColumn("P1410", typeof(decimal)); dt.Columns.Add(col_avarage);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; bool first_time = true; string TimeQuery = string.Empty; COUNT = 0;
            DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet();
            if (Session.DBType == 2)
                sql = string.Format("SELECT COUNT(0) FROM T_31_022_{0} T322 LEFT JOIN T_31_020_{0} T320 ON T322.C001=T320.C001 LEFT JOIN T_31_008_{0} T308 ON T308.C002=T322.C002 LEFT JOIN T_31_021_{0} T321 ON T322.C001=T321.C001 WHERE T320.C005='N' AND T321.C004='Q' AND T322.C003='N' AND T308.C010<=6 AND T308.C010>=3 AND "
                + ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' {1}", Session.RentInfo.Token, StrQ);
            else
                sql = string.Format("SELECT COUNT(0) FROM T_31_022_{0} T322 LEFT JOIN T_31_020_{0} T320 ON T322.C001=T320.C001 LEFT JOIN T_31_008_{0} T308 ON T308.C002=T322.C002 LEFT JOIN T_31_021_{0} T321 ON T322.C001=T321.C001 WHERE T320.C005='N' AND T321.C004='Q' AND T322.C003='N' AND T308.C010<=6 AND T308.C010>=3 AND "
                    + ColName + " >= TO_DATE('" + ADT.BeginDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t] + "','YYYY-MM-DD HH24:MI:SS') {1}", Session.RentInfo.Token, StrQ);

            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} T322.C001,T322.C002,T322.C012,T321.C002 C005,T320.C018,T321.C003,T320.C006,T320.C017,T320.C010,T308.C016,T320.C021  FROM T_31_022_{0} T322 LEFT JOIN T_31_020_{0} T320 ON T322.C001=T320.C001 LEFT JOIN T_31_008_{0} T308 ON T308.C002=T322.C002 LEFT JOIN T_31_021_{0} T321 ON T322.C001=T321.C001 WHERE T320.C005='N' AND T321.C004='Q' AND T322.C003='N' AND T308.C010<=6 AND T308.C010>=3 AND "
                                + ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "' {2} ORDER BY T322.C001," + ColName
                                 , Session.RentInfo.Token, GetDataNum, " AND T322.C001 >" + AfterRowNum + StrQ);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C012,C021,C010,C017,C018,C005,C003,C006,C016 FROM (SELECT T322.C001,T322.C002,T322.C012,T321.C002 C005,T320.C018,T321.C003,T320.C006,T320.C017,T320.C010,T308.C016,T320.C021 FROM T_31_022_{0} T322 LEFT JOIN T_31_020_{0} T320 ON T322.C001=T320.C001 LEFT JOIN T_31_008_{0} T308 ON T308.C002=T322.C002 LEFT JOIN T_31_021_{0} T321 ON T322.C001=T321.C001 WHERE T320.C005='N' AND T321.C004='Q' AND T322.C003='N' AND T308.C010<=6 AND T308.C010>=3 AND {1} >= TO_DATE('"
                        + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND {1} <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') "
                        + "{2} ORDER BY T322.C001,{1})WHERE ROWNUM<=" + GetDataNum + " AND T322.C001>" + AfterRowNum + " ORDER BY T322.C001,{1} DESC ", Session.RentInfo.Token, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(14):{0}", sql));
            if (CiShuSum != 0)
            {
                #region 统计数据
                List<string> inspectorname = new List<string>();
                List<string> inspector = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    string name = dr["C005"].ToString();
                    string findname = inspectorname.FirstOrDefault(p => p == name);
                    //string findinspect = inspector.FirstOrDefault(p => p == dr["C003"].ToString());
                    if (findname == null)
                    {
                        inspectorname.Add(name);
                        inspector.Add(dr["C003"].ToString());
                    }
                }
                TransData(inspectorname, dt, "P1400", 1); TransData(inspector, dt, "P1401", 2);

                foreach (DataRow row in dt.Rows)
                {
                    DataRow[] RowList = _ds.Tables[0].Select(string.Format("C005={0}", row["P1400"].ToString())); int rec = 0;
                    List<string> TaskNum = new List<string>(); int inspecttime = 0; int finish = 0;
                    List<string> RecordList = new List<string>(); decimal score = 0; decimal max = 0; decimal min = 1000;
                    for (int i = 0; i < RowList.Count(); i++)
                    {
                        string findTask = TaskNum.FirstOrDefault(p => p == RowList[i]["C001"].ToString());
                        string findRecord = RecordList.FirstOrDefault(p => p == RowList[i]["C002"].ToString());
                        if (findTask == null)
                        {
                            rec += int.Parse(RowList[i]["C021"].ToString());//records time 1405
                            TaskNum.Add(RowList[i]["C001"].ToString());//task numeber1402
                            if (RowList[i]["C017"].ToString() == "Y")
                            {
                                finish++;
                                if (RowList[i]["C018"].ToString() != string.Empty)
                                {
                                    int day = (Convert.ToDateTime(RowList[i]["C018"].ToString()) - Convert.ToDateTime(RowList[i]["C006"].ToString())).Days;
                                    score += day;
                                    if (day > max)
                                    {
                                        max = day;
                                    }
                                    if (day < min)
                                    {
                                        min = day;
                                    }
                                }
                            }
                        }
                        if (findRecord == null)
                        {
                            RecordList.Add(RowList[i]["C002"].ToString());
                            inspecttime += int.Parse(RowList[i]["C016"].ToString());//inspect time 1406
                        }
                        string time = string.Empty;
                        time = Convert.ToDateTime(RowList[i]["C006"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        row["P107"] = AbsoluteDate(time);
                        row["P1402"] = TaskNum.Count; row["P1404"] = RecordList.Count; row["P1403"] = finish; row["P1405"] = TransTimeForm(rec);
                        row["P1408"] = max; row["P1406"] = TransTimeForm(inspecttime);
                        if (rec != 0)
                        {
                            row["P1407"] = decimal.Round(Decimal.Parse(inspecttime.ToString()) / Decimal.Parse(rec.ToString()), NumAfterPoint);
                        }
                        else
                            row["P1407"] = 0;
                        if (min == 1000)
                            row["P1409"] = 0;
                        else
                            row["P1409"] = min;
                        if (finish != 0)
                        {
                            row["P1410"] = decimal.Round(score / finish, NumAfterPoint);
                        }
                        else
                        {
                            row["P1410"] = 0;
                        }
                        foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                        {
                            if (row["P1400"].ToString() == dr_ap["C001"].ToString())
                            {
                                row["P1400"] = DecryptString(dr_ap["C002"].ToString());
                                row["P1401"] = DecryptString(dr_ap["C003"].ToString());
                            }
                        }
                    }
                }
                #endregion
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR15DataSet(string StrQ, int t, string ColName)
        {
            DataSet ds = new DataSet(); DataTable dt = new DataTable(); string sql = string.Empty;
            DataColumn col_inspectorname = new DataColumn("P1501", typeof(string)); dt.Columns.Add(col_inspectorname);
            DataColumn col_inspectornum = new DataColumn("P1500", typeof(string)); dt.Columns.Add(col_inspectornum);
            DataColumn col_finish = new DataColumn("P1502", typeof(int)); dt.Columns.Add(col_finish);
            DataColumn col_rectime = new DataColumn("P1503", typeof(string)); dt.Columns.Add(col_rectime);
            DataColumn col_inspectime = new DataColumn("P1504", typeof(string)); dt.Columns.Add(col_inspectime);
            DataColumn col_timeratio = new DataColumn("P1505", typeof(decimal)); dt.Columns.Add(col_timeratio);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; bool first_time = true; string TimeQuery = string.Empty; COUNT = 0;
            DataSet _ds = new DataSet(); int PreRowNum = GetDataNum; string AfterRowNum = "0"; DataSet DS_temp = new DataSet();
            string BT = ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"); string ET = ADT.EndDateTime[t].ToString("yyyyMMddHHmmss");
            if (ColName == "C006 ")
            {
                if (Session.DBType == 2)
                    TimeQuery = ColName + " >= '" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "' AND " + ColName + " <= '" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "'";
                else
                    TimeQuery = ColName + " >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND " + ColName + " <= TO_DATE('" + ADT.EndDateTime[t].ToString("yyyyMMdd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')";
            }
            else
            {
                string recorder = RecorderIDList(ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"),
                    "T_31_008_" + Session.RentInfo.Token, "C002");
                if (recorder != null)
                    TimeQuery = string.Format(" {0} IN (SELECT C011 FROM T_00_901 WHERE C001={1})", ColName, recorder);
                else
                    return ds;
            }
            sql = string.Format("SELECT COUNT(0) FROM T_31_008_{0} WHERE C009='Y' AND " + TimeQuery + StrQ, Session.RentInfo.Token);
            DataSet ds_temp = GetDataSetFromDB(sql, 100);
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu;
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} C001,C005,C002,C016,C006 P107 FROM T_31_008_{0} WHERE {2} AND (C010<=2 OR C010>=7) ORDER BY C001,P107"
                            , Session.RentInfo.Token, GetDataNum, TimeQuery + StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT C001,C005,C002,C016,P107 FROM (SELECT C001,C005,C002,C016,C006 P107 FROM T_31_008_{0} WHERE "
                        + TimeQuery + StrQ + " AND (C010<=2 OR C010>=7) ORDER BY C001,P107)WHERE ROWNUM<={1} AND C001>" + AfterRowNum + " ORDER BY C001,P107 DESC ", Session.RentInfo.Token, GetDataNum);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(15):{0}", sql));
            if (CiShuSum != 0)
            {
                ListMarkInfo.Add("T_31_008");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                DataTable dtM = _ds.Tables[0].Clone();
                #region 统计数据
                List<string> inspector = new List<string>();
                foreach (DataRow dr in _ds.Tables[0].Rows)
                {
                    dtM.ImportRow(dr);
                    bool flag = true; string name = dr["C005"].ToString();
                    for (int i = 0; i < inspector.Count; i++)
                    {
                        if (name == inspector[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        inspector.Add(name);
                    }
                }
                DataSetMark.Tables.Add(dtM);
                TransData(inspector, dt, "P1500", 1);
                foreach (DataRow row in dt.Rows)
                {
                    List<string> RecorderID = new List<string>(); DataTable DT_Final = _ds.Tables[0].Clone();
                    foreach (DataRow dr in _ds.Tables[0].Rows)
                    {
                        if (row["P1500"].ToString() == dr["C005"].ToString())
                        {
                            bool Flag = true;
                            for (int j = 0; j < RecorderID.Count; j++)
                            {
                                if (RecorderID[j] == dr["C002"].ToString())
                                {
                                    Flag = false;
                                    break;
                                }
                            }
                            if (Flag)
                            {
                                RecorderID.Add(dr["C002"].ToString());
                            }
                        }
                    }
                    for (int k = 0; k < RecorderID.Count; k++)
                    {
                        List<DataRow> RecorderRows = _ds.Tables[0].Select(string.Format("C002={0} AND C005={1}", RecorderID[k], row["P1500"].ToString())).ToList();
                        DateTime DTTemp = DateTime.MinValue; int Mark = -1;
                        for (int q = 0; q < RecorderRows.Count; q++)
                        {
                            DateTime DTRow = Convert.ToDateTime(RecorderRows[q]["P107"].ToString());
                            TimeSpan TS = DTRow - DTTemp;
                            if (TS.TotalSeconds > 0)
                            {
                                DTTemp = DTRow;
                                Mark = q;
                            }
                        }
                        if (Mark != -1)
                            DT_Final.ImportRow(RecorderRows[Mark]);
                    }

                    int finish = 0; decimal rec = 0; decimal inspecttime = 0;
                    foreach (DataRow dr in DT_Final.Rows)
                    {
                        if (dr["C005"].ToString() == row["P1500"].ToString())
                        {
                            rec += Convert.ToInt32(GetDataFromRecorderTable(BT, ET, dr["C002"].ToString(), "C012"));
                            //inspecttime += decimal.Parse(dr["C016"].ToString());
                            finish++;
                            string time = string.Empty;
                            DateTime DTTEMP = Convert.ToDateTime(dr["P107"].ToString()).ToLocalTime();
                            time = DTTEMP.ToString("yyyyMMddHHmmss");
                            row["P107"] = AbsoluteDate(time);
                        }
                    }
                    foreach (DataRow dr in _ds.Tables[0].Rows)
                    {
                        if (dr["C005"].ToString() == row["P1500"].ToString())
                        {
                            inspecttime += decimal.Parse(dr["C016"].ToString());
                        }
                    }
                    row["P1502"] = finish; row["P1503"] = Converter.Second2Time((double)rec); row["P1504"] = TransTimeForm(Convert.ToInt32(inspecttime));
                    if (rec != 0)
                    {
                        row["P1505"] = decimal.Round(inspecttime / rec, NumAfterPoint);
                    }
                    else
                    {
                        row["P1505"] = 0;
                    }
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (row["P1500"].ToString() == dr_ap["C001"].ToString())
                        {
                            row["P1500"] = DecryptString(dr_ap["C002"].ToString());
                            row["P1501"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                }
                #endregion
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR16DataSet(string StrQ, int t, string ColName)
        {
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet(); bool first_time = true; COUNT = 0;
            string sql = "SELECT COUNT(0) FROM T_31_041_" + Session.RentInfo.Token + " WHERE " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                + " AND C005 = '1' AND  " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100); DataSet ds = new DataSet();
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} * FROM T_31_041_{0} WHERE C005 = '1' AND  " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                        + " AND " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " {2} ORDER BY C001"
                                      , Session.RentInfo.Token, GetDataNum, StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT * FROM (SELECT A.* FROM T_31_041_{0} A WHERE C005 = '1' AND  {2} >="
                        + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND {2} <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ + " ORDER BY C001,{2}) WHERE ROWNUM<={1} AND C001>" + AfterRowNum
                        + " ORDER BY C001 DESC ", Session.RentInfo.Token, GetDataNum, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables.Count != 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(16):{0}", sql));
            if (CiShuSum != 0)
            {
                #region 统计数据
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable(); DataTable dtM = _dt.Clone();
                DataColumn col_agentname = new DataColumn("C017", typeof(string)); dt.Columns.Add(col_agentname);
                DataColumn col_comment = new DataColumn("P1602", typeof(string)); dt.Columns.Add(col_comment);
                DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);
                DataColumn col_agentid = new DataColumn("C016", typeof(string)); dt.Columns.Add(col_agentid);
                DataColumn col_part = new DataColumn("C015", typeof(string)); dt.Columns.Add(col_part);
                DataColumn col_score = new DataColumn("C100", typeof(decimal)); dt.Columns.Add(col_score);
                DataColumn col_inspecttime = new DataColumn("C003", typeof(string)); dt.Columns.Add(col_inspecttime);
                DataColumn col_inspect = new DataColumn("C004", typeof(string)); dt.Columns.Add(col_inspect);
                DataColumn col_recordid = new DataColumn("C002", typeof(string)); dt.Columns.Add(col_recordid);
                DataColumn col_scorename = new DataColumn("P1601", typeof(string)); dt.Columns.Add(col_scorename);
                DataColumn col_mark = new DataColumn("C000", typeof(string)); dt.Columns.Add(col_mark);
                DataColumn col_scoreid = new DataColumn("C001", typeof(string)); dt.Columns.Add(col_scoreid);
                #region 200Params
                DataColumn col_Param1 = new DataColumn("C101", typeof(string)); dt.Columns.Add(col_Param1);
                DataColumn col_Param2 = new DataColumn("C102", typeof(string)); dt.Columns.Add(col_Param2);
                DataColumn col_Param3 = new DataColumn("C103", typeof(string)); dt.Columns.Add(col_Param3);
                DataColumn col_Param4 = new DataColumn("C104", typeof(string)); dt.Columns.Add(col_Param4);
                DataColumn col_Param5 = new DataColumn("C105", typeof(string)); dt.Columns.Add(col_Param5);
                DataColumn col_Param6 = new DataColumn("C106", typeof(string)); dt.Columns.Add(col_Param6);
                DataColumn col_Param7 = new DataColumn("C107", typeof(string)); dt.Columns.Add(col_Param7);
                DataColumn col_Param8 = new DataColumn("C108", typeof(string)); dt.Columns.Add(col_Param8);
                DataColumn col_Param9 = new DataColumn("C109", typeof(string)); dt.Columns.Add(col_Param9);
                DataColumn col_Param10 = new DataColumn("C110", typeof(string)); dt.Columns.Add(col_Param10);
                DataColumn col_Param11 = new DataColumn("C111", typeof(string)); dt.Columns.Add(col_Param11);
                DataColumn col_Param12 = new DataColumn("C112", typeof(string)); dt.Columns.Add(col_Param12);
                DataColumn col_Param13 = new DataColumn("C113", typeof(string)); dt.Columns.Add(col_Param13);
                DataColumn col_Param14 = new DataColumn("C114", typeof(string)); dt.Columns.Add(col_Param14);
                DataColumn col_Param15 = new DataColumn("C115", typeof(string)); dt.Columns.Add(col_Param15);
                DataColumn col_Param16 = new DataColumn("C116", typeof(string)); dt.Columns.Add(col_Param16);
                DataColumn col_Param17 = new DataColumn("C117", typeof(string)); dt.Columns.Add(col_Param17);
                DataColumn col_Param18 = new DataColumn("C118", typeof(string)); dt.Columns.Add(col_Param18);
                DataColumn col_Param19 = new DataColumn("C119", typeof(string)); dt.Columns.Add(col_Param19);
                DataColumn col_Param20 = new DataColumn("C120", typeof(string)); dt.Columns.Add(col_Param20);
                DataColumn col_Param21 = new DataColumn("C121", typeof(string)); dt.Columns.Add(col_Param21);
                DataColumn col_Param22 = new DataColumn("C122", typeof(string)); dt.Columns.Add(col_Param22);
                DataColumn col_Param23 = new DataColumn("C123", typeof(string)); dt.Columns.Add(col_Param23);
                DataColumn col_Param24 = new DataColumn("C124", typeof(string)); dt.Columns.Add(col_Param24);
                DataColumn col_Param25 = new DataColumn("C125", typeof(string)); dt.Columns.Add(col_Param25);
                DataColumn col_Param26 = new DataColumn("C126", typeof(string)); dt.Columns.Add(col_Param26);
                DataColumn col_Param27 = new DataColumn("C127", typeof(string)); dt.Columns.Add(col_Param27);
                DataColumn col_Param28 = new DataColumn("C128", typeof(string)); dt.Columns.Add(col_Param28);
                DataColumn col_Param29 = new DataColumn("C129", typeof(string)); dt.Columns.Add(col_Param29);
                DataColumn col_Param30 = new DataColumn("C130", typeof(string)); dt.Columns.Add(col_Param30);
                DataColumn col_Param31 = new DataColumn("C131", typeof(string)); dt.Columns.Add(col_Param31);
                DataColumn col_Param32 = new DataColumn("C132", typeof(string)); dt.Columns.Add(col_Param32);
                DataColumn col_Param33 = new DataColumn("C133", typeof(string)); dt.Columns.Add(col_Param33);
                DataColumn col_Param34 = new DataColumn("C134", typeof(string)); dt.Columns.Add(col_Param34);
                DataColumn col_Param35 = new DataColumn("C135", typeof(string)); dt.Columns.Add(col_Param35);
                DataColumn col_Param36 = new DataColumn("C136", typeof(string)); dt.Columns.Add(col_Param36);
                DataColumn col_Param37 = new DataColumn("C137", typeof(string)); dt.Columns.Add(col_Param37);
                DataColumn col_Param38 = new DataColumn("C138", typeof(string)); dt.Columns.Add(col_Param38);
                DataColumn col_Param39 = new DataColumn("C139", typeof(string)); dt.Columns.Add(col_Param39);
                DataColumn col_Param40 = new DataColumn("C140", typeof(string)); dt.Columns.Add(col_Param40);
                DataColumn col_Param41 = new DataColumn("C141", typeof(string)); dt.Columns.Add(col_Param41);
                DataColumn col_Param42 = new DataColumn("C142", typeof(string)); dt.Columns.Add(col_Param42);
                DataColumn col_Param43 = new DataColumn("C143", typeof(string)); dt.Columns.Add(col_Param43);
                DataColumn col_Param44 = new DataColumn("C144", typeof(string)); dt.Columns.Add(col_Param44);
                DataColumn col_Param45 = new DataColumn("C145", typeof(string)); dt.Columns.Add(col_Param45);
                DataColumn col_Param46 = new DataColumn("C146", typeof(string)); dt.Columns.Add(col_Param46);
                DataColumn col_Param47 = new DataColumn("C147", typeof(string)); dt.Columns.Add(col_Param47);
                DataColumn col_Param48 = new DataColumn("C148", typeof(string)); dt.Columns.Add(col_Param48);
                DataColumn col_Param49 = new DataColumn("C149", typeof(string)); dt.Columns.Add(col_Param49);
                DataColumn col_Param50 = new DataColumn("C150", typeof(string)); dt.Columns.Add(col_Param50);
                DataColumn col_Param51 = new DataColumn("C151", typeof(string)); dt.Columns.Add(col_Param51);
                DataColumn col_Param52 = new DataColumn("C152", typeof(string)); dt.Columns.Add(col_Param52);
                DataColumn col_Param53 = new DataColumn("C153", typeof(string)); dt.Columns.Add(col_Param53);
                DataColumn col_Param54 = new DataColumn("C154", typeof(string)); dt.Columns.Add(col_Param54);
                DataColumn col_Param55 = new DataColumn("C155", typeof(string)); dt.Columns.Add(col_Param55);
                DataColumn col_Param56 = new DataColumn("C156", typeof(string)); dt.Columns.Add(col_Param56);
                DataColumn col_Param57 = new DataColumn("C157", typeof(string)); dt.Columns.Add(col_Param57);
                DataColumn col_Param58 = new DataColumn("C158", typeof(string)); dt.Columns.Add(col_Param58);
                DataColumn col_Param59 = new DataColumn("C159", typeof(string)); dt.Columns.Add(col_Param59);
                DataColumn col_Param60 = new DataColumn("C160", typeof(string)); dt.Columns.Add(col_Param60);
                DataColumn col_Param61 = new DataColumn("C161", typeof(string)); dt.Columns.Add(col_Param61);
                DataColumn col_Param62 = new DataColumn("C162", typeof(string)); dt.Columns.Add(col_Param62);
                DataColumn col_Param63 = new DataColumn("C163", typeof(string)); dt.Columns.Add(col_Param63);
                DataColumn col_Param64 = new DataColumn("C164", typeof(string)); dt.Columns.Add(col_Param64);
                DataColumn col_Param65 = new DataColumn("C165", typeof(string)); dt.Columns.Add(col_Param65);
                DataColumn col_Param66 = new DataColumn("C166", typeof(string)); dt.Columns.Add(col_Param66);
                DataColumn col_Param67 = new DataColumn("C167", typeof(string)); dt.Columns.Add(col_Param67);
                DataColumn col_Param68 = new DataColumn("C168", typeof(string)); dt.Columns.Add(col_Param68);
                DataColumn col_Param69 = new DataColumn("C169", typeof(string)); dt.Columns.Add(col_Param69);
                DataColumn col_Param70 = new DataColumn("C170", typeof(string)); dt.Columns.Add(col_Param70);
                DataColumn col_Param71 = new DataColumn("C171", typeof(string)); dt.Columns.Add(col_Param71);
                DataColumn col_Param72 = new DataColumn("C172", typeof(string)); dt.Columns.Add(col_Param72);
                DataColumn col_Param73 = new DataColumn("C173", typeof(string)); dt.Columns.Add(col_Param73);
                DataColumn col_Param74 = new DataColumn("C174", typeof(string)); dt.Columns.Add(col_Param74);
                DataColumn col_Param75 = new DataColumn("C175", typeof(string)); dt.Columns.Add(col_Param75);
                DataColumn col_Param76 = new DataColumn("C176", typeof(string)); dt.Columns.Add(col_Param76);
                DataColumn col_Param77 = new DataColumn("C177", typeof(string)); dt.Columns.Add(col_Param77);
                DataColumn col_Param78 = new DataColumn("C178", typeof(string)); dt.Columns.Add(col_Param78);
                DataColumn col_Param79 = new DataColumn("C179", typeof(string)); dt.Columns.Add(col_Param79);
                DataColumn col_Param80 = new DataColumn("C180", typeof(string)); dt.Columns.Add(col_Param80);
                DataColumn col_Param81 = new DataColumn("C181", typeof(string)); dt.Columns.Add(col_Param81);
                DataColumn col_Param82 = new DataColumn("C182", typeof(string)); dt.Columns.Add(col_Param82);
                DataColumn col_Param83 = new DataColumn("C183", typeof(string)); dt.Columns.Add(col_Param83);
                DataColumn col_Param84 = new DataColumn("C184", typeof(string)); dt.Columns.Add(col_Param84);
                DataColumn col_Param85 = new DataColumn("C185", typeof(string)); dt.Columns.Add(col_Param85);
                DataColumn col_Param86 = new DataColumn("C186", typeof(string)); dt.Columns.Add(col_Param86);
                DataColumn col_Param87 = new DataColumn("C187", typeof(string)); dt.Columns.Add(col_Param87);
                DataColumn col_Param88 = new DataColumn("C188", typeof(string)); dt.Columns.Add(col_Param88);
                DataColumn col_Param89 = new DataColumn("C189", typeof(string)); dt.Columns.Add(col_Param89);
                DataColumn col_Param90 = new DataColumn("C190", typeof(string)); dt.Columns.Add(col_Param90);
                DataColumn col_Param91 = new DataColumn("C191", typeof(string)); dt.Columns.Add(col_Param91);
                DataColumn col_Param92 = new DataColumn("C192", typeof(string)); dt.Columns.Add(col_Param92);
                DataColumn col_Param93 = new DataColumn("C193", typeof(string)); dt.Columns.Add(col_Param93);
                DataColumn col_Param94 = new DataColumn("C194", typeof(string)); dt.Columns.Add(col_Param94);
                DataColumn col_Param95 = new DataColumn("C195", typeof(string)); dt.Columns.Add(col_Param95);
                DataColumn col_Param96 = new DataColumn("C196", typeof(string)); dt.Columns.Add(col_Param96);
                DataColumn col_Param97 = new DataColumn("C197", typeof(string)); dt.Columns.Add(col_Param97);
                DataColumn col_Param98 = new DataColumn("C198", typeof(string)); dt.Columns.Add(col_Param98);
                DataColumn col_Param99 = new DataColumn("C199", typeof(string)); dt.Columns.Add(col_Param99);
                DataColumn col_Param100 = new DataColumn("C200", typeof(string)); dt.Columns.Add(col_Param100);
                DataColumn col_Param101 = new DataColumn("C201", typeof(string)); dt.Columns.Add(col_Param101);
                DataColumn col_Param102 = new DataColumn("C202", typeof(string)); dt.Columns.Add(col_Param102);
                DataColumn col_Param103 = new DataColumn("C203", typeof(string)); dt.Columns.Add(col_Param103);
                DataColumn col_Param104 = new DataColumn("C204", typeof(string)); dt.Columns.Add(col_Param104);
                DataColumn col_Param105 = new DataColumn("C205", typeof(string)); dt.Columns.Add(col_Param105);
                DataColumn col_Param106 = new DataColumn("C206", typeof(string)); dt.Columns.Add(col_Param106);
                DataColumn col_Param107 = new DataColumn("C207", typeof(string)); dt.Columns.Add(col_Param107);
                DataColumn col_Param108 = new DataColumn("C208", typeof(string)); dt.Columns.Add(col_Param108);
                DataColumn col_Param109 = new DataColumn("C209", typeof(string)); dt.Columns.Add(col_Param109);
                DataColumn col_Param110 = new DataColumn("C210", typeof(string)); dt.Columns.Add(col_Param110);
                DataColumn col_Param111 = new DataColumn("C211", typeof(string)); dt.Columns.Add(col_Param111);
                DataColumn col_Param112 = new DataColumn("C212", typeof(string)); dt.Columns.Add(col_Param112);
                DataColumn col_Param113 = new DataColumn("C213", typeof(string)); dt.Columns.Add(col_Param113);
                DataColumn col_Param114 = new DataColumn("C214", typeof(string)); dt.Columns.Add(col_Param114);
                DataColumn col_Param115 = new DataColumn("C215", typeof(string)); dt.Columns.Add(col_Param115);
                DataColumn col_Param116 = new DataColumn("C216", typeof(string)); dt.Columns.Add(col_Param116);
                DataColumn col_Param117 = new DataColumn("C217", typeof(string)); dt.Columns.Add(col_Param117);
                DataColumn col_Param118 = new DataColumn("C218", typeof(string)); dt.Columns.Add(col_Param118);
                DataColumn col_Param119 = new DataColumn("C219", typeof(string)); dt.Columns.Add(col_Param119);
                DataColumn col_Param120 = new DataColumn("C220", typeof(string)); dt.Columns.Add(col_Param120);
                DataColumn col_Param121 = new DataColumn("C221", typeof(string)); dt.Columns.Add(col_Param121);
                DataColumn col_Param122 = new DataColumn("C222", typeof(string)); dt.Columns.Add(col_Param122);
                DataColumn col_Param123 = new DataColumn("C223", typeof(string)); dt.Columns.Add(col_Param123);
                DataColumn col_Param124 = new DataColumn("C224", typeof(string)); dt.Columns.Add(col_Param124);
                DataColumn col_Param125 = new DataColumn("C225", typeof(string)); dt.Columns.Add(col_Param125);
                DataColumn col_Param126 = new DataColumn("C226", typeof(string)); dt.Columns.Add(col_Param126);
                DataColumn col_Param127 = new DataColumn("C227", typeof(string)); dt.Columns.Add(col_Param127);
                DataColumn col_Param128 = new DataColumn("C228", typeof(string)); dt.Columns.Add(col_Param128);
                DataColumn col_Param129 = new DataColumn("C229", typeof(string)); dt.Columns.Add(col_Param129);
                DataColumn col_Param130 = new DataColumn("C230", typeof(string)); dt.Columns.Add(col_Param130);
                DataColumn col_Param131 = new DataColumn("C231", typeof(string)); dt.Columns.Add(col_Param131);
                DataColumn col_Param132 = new DataColumn("C232", typeof(string)); dt.Columns.Add(col_Param132);
                DataColumn col_Param133 = new DataColumn("C233", typeof(string)); dt.Columns.Add(col_Param133);
                DataColumn col_Param134 = new DataColumn("C234", typeof(string)); dt.Columns.Add(col_Param134);
                DataColumn col_Param135 = new DataColumn("C235", typeof(string)); dt.Columns.Add(col_Param135);
                DataColumn col_Param136 = new DataColumn("C236", typeof(string)); dt.Columns.Add(col_Param136);
                DataColumn col_Param137 = new DataColumn("C237", typeof(string)); dt.Columns.Add(col_Param137);
                DataColumn col_Param138 = new DataColumn("C238", typeof(string)); dt.Columns.Add(col_Param138);
                DataColumn col_Param139 = new DataColumn("C239", typeof(string)); dt.Columns.Add(col_Param139);
                DataColumn col_Param140 = new DataColumn("C240", typeof(string)); dt.Columns.Add(col_Param140);
                DataColumn col_Param141 = new DataColumn("C241", typeof(string)); dt.Columns.Add(col_Param141);
                DataColumn col_Param142 = new DataColumn("C242", typeof(string)); dt.Columns.Add(col_Param142);
                DataColumn col_Param143 = new DataColumn("C243", typeof(string)); dt.Columns.Add(col_Param143);
                DataColumn col_Param144 = new DataColumn("C244", typeof(string)); dt.Columns.Add(col_Param144);
                DataColumn col_Param145 = new DataColumn("C245", typeof(string)); dt.Columns.Add(col_Param145);
                DataColumn col_Param146 = new DataColumn("C246", typeof(string)); dt.Columns.Add(col_Param146);
                DataColumn col_Param147 = new DataColumn("C247", typeof(string)); dt.Columns.Add(col_Param147);
                DataColumn col_Param148 = new DataColumn("C248", typeof(string)); dt.Columns.Add(col_Param148);
                DataColumn col_Param149 = new DataColumn("C249", typeof(string)); dt.Columns.Add(col_Param149);
                DataColumn col_Param150 = new DataColumn("C250", typeof(string)); dt.Columns.Add(col_Param150);
                DataColumn col_Param151 = new DataColumn("C251", typeof(string)); dt.Columns.Add(col_Param151);
                DataColumn col_Param152 = new DataColumn("C252", typeof(string)); dt.Columns.Add(col_Param152);
                DataColumn col_Param153 = new DataColumn("C253", typeof(string)); dt.Columns.Add(col_Param153);
                DataColumn col_Param154 = new DataColumn("C254", typeof(string)); dt.Columns.Add(col_Param154);
                DataColumn col_Param155 = new DataColumn("C255", typeof(string)); dt.Columns.Add(col_Param155);
                DataColumn col_Param156 = new DataColumn("C256", typeof(string)); dt.Columns.Add(col_Param156);
                DataColumn col_Param157 = new DataColumn("C257", typeof(string)); dt.Columns.Add(col_Param157);
                DataColumn col_Param158 = new DataColumn("C258", typeof(string)); dt.Columns.Add(col_Param158);
                DataColumn col_Param159 = new DataColumn("C259", typeof(string)); dt.Columns.Add(col_Param159);
                DataColumn col_Param160 = new DataColumn("C260", typeof(string)); dt.Columns.Add(col_Param160);
                DataColumn col_Param161 = new DataColumn("C261", typeof(string)); dt.Columns.Add(col_Param161);
                DataColumn col_Param162 = new DataColumn("C262", typeof(string)); dt.Columns.Add(col_Param162);
                DataColumn col_Param163 = new DataColumn("C263", typeof(string)); dt.Columns.Add(col_Param163);
                DataColumn col_Param164 = new DataColumn("C264", typeof(string)); dt.Columns.Add(col_Param164);
                DataColumn col_Param165 = new DataColumn("C265", typeof(string)); dt.Columns.Add(col_Param165);
                DataColumn col_Param166 = new DataColumn("C266", typeof(string)); dt.Columns.Add(col_Param166);
                DataColumn col_Param167 = new DataColumn("C267", typeof(string)); dt.Columns.Add(col_Param167);
                DataColumn col_Param168 = new DataColumn("C268", typeof(string)); dt.Columns.Add(col_Param168);
                DataColumn col_Param169 = new DataColumn("C269", typeof(string)); dt.Columns.Add(col_Param169);
                DataColumn col_Param170 = new DataColumn("C270", typeof(string)); dt.Columns.Add(col_Param170);
                DataColumn col_Param171 = new DataColumn("C271", typeof(string)); dt.Columns.Add(col_Param171);
                DataColumn col_Param172 = new DataColumn("C272", typeof(string)); dt.Columns.Add(col_Param172);
                DataColumn col_Param173 = new DataColumn("C273", typeof(string)); dt.Columns.Add(col_Param173);
                DataColumn col_Param174 = new DataColumn("C274", typeof(string)); dt.Columns.Add(col_Param174);
                DataColumn col_Param175 = new DataColumn("C275", typeof(string)); dt.Columns.Add(col_Param175);
                DataColumn col_Param176 = new DataColumn("C276", typeof(string)); dt.Columns.Add(col_Param176);
                DataColumn col_Param177 = new DataColumn("C277", typeof(string)); dt.Columns.Add(col_Param177);
                DataColumn col_Param178 = new DataColumn("C278", typeof(string)); dt.Columns.Add(col_Param178);
                DataColumn col_Param179 = new DataColumn("C279", typeof(string)); dt.Columns.Add(col_Param179);
                DataColumn col_Param180 = new DataColumn("C280", typeof(string)); dt.Columns.Add(col_Param180);
                DataColumn col_Param181 = new DataColumn("C281", typeof(string)); dt.Columns.Add(col_Param181);
                DataColumn col_Param182 = new DataColumn("C282", typeof(string)); dt.Columns.Add(col_Param182);
                DataColumn col_Param183 = new DataColumn("C283", typeof(string)); dt.Columns.Add(col_Param183);
                DataColumn col_Param184 = new DataColumn("C284", typeof(string)); dt.Columns.Add(col_Param184);
                DataColumn col_Param185 = new DataColumn("C285", typeof(string)); dt.Columns.Add(col_Param185);
                DataColumn col_Param186 = new DataColumn("C286", typeof(string)); dt.Columns.Add(col_Param186);
                DataColumn col_Param187 = new DataColumn("C287", typeof(string)); dt.Columns.Add(col_Param187);
                DataColumn col_Param188 = new DataColumn("C288", typeof(string)); dt.Columns.Add(col_Param188);
                DataColumn col_Param189 = new DataColumn("C289", typeof(string)); dt.Columns.Add(col_Param189);
                DataColumn col_Param190 = new DataColumn("C290", typeof(string)); dt.Columns.Add(col_Param190);
                DataColumn col_Param191 = new DataColumn("C291", typeof(string)); dt.Columns.Add(col_Param191);
                DataColumn col_Param192 = new DataColumn("C292", typeof(string)); dt.Columns.Add(col_Param192);
                DataColumn col_Param193 = new DataColumn("C293", typeof(string)); dt.Columns.Add(col_Param193);
                DataColumn col_Param194 = new DataColumn("C294", typeof(string)); dt.Columns.Add(col_Param194);
                DataColumn col_Param195 = new DataColumn("C295", typeof(string)); dt.Columns.Add(col_Param195);
                DataColumn col_Param196 = new DataColumn("C296", typeof(string)); dt.Columns.Add(col_Param196);
                DataColumn col_Param197 = new DataColumn("C297", typeof(string)); dt.Columns.Add(col_Param197);
                DataColumn col_Param198 = new DataColumn("C298", typeof(string)); dt.Columns.Add(col_Param198);
                DataColumn col_Param199 = new DataColumn("C299", typeof(string)); dt.Columns.Add(col_Param199);
                DataColumn col_Param200 = new DataColumn("C300", typeof(string)); dt.Columns.Add(col_Param200);
                #endregion

                foreach (DataRow row in _dt.Rows)
                {
                    dt.ImportRow(row); dtM.ImportRow(row);
                }
                ListMarkInfo.Add("T_31_041");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region //动态加列
                string ScoreID = dt.Rows[0]["C000"].ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 15;
                webRequest.Session = Session;
                webRequest.ListData.Add(ScoreID);
                webRequest.ListData.Add("0");
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R16 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return new DataSet();
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R16XmlSS Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return new DataSet();
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tScoreSheet is null"));
                    return new DataSet();
                }
                scoreSheet.Init();
                //拿所有的评分标准
                List<ScoreObject> ListScoreObject = new List<ScoreObject>();
                List<Standard> ListScoreStandard = new List<Standard>();
                List<Comment> ListScoreComment = new List<Comment>();
                //scoreSheet.GetAllStandards(ref ListScoreStandard);
                scoreSheet.GetAllScoreObject(ref ListScoreObject);
                foreach (ScoreObject Sobject in ListScoreObject)
                {
                    Standard TempStandard = Sobject as Standard;
                    if (TempStandard != null)
                    {
                        ListScoreStandard.Add(TempStandard);
                    }
                    Comment TempComment = Sobject as Comment;
                    if (TempComment != null)
                    {
                        ListScoreComment.Add(TempComment);
                    }
                }

                if (ListScoreStandard.Count == 0)
                {
                    return new DataSet();
                }
                List<string> ListColumnName = new List<string>();
                XmlDocument LXmlDoc = new XmlDocument();
                //string LStrSiteBaseDirectory = string.Format(@"Report16.rdlc");
                //MessageBox.Show(LStrSiteBaseDirectory);
                //string LStrSiteBaseDirectory = App.GetIISBaseDirectory();
                //LXmlDoc.Load(LStrSiteBaseDirectory);
                var byteData = UMPS6101.Properties.Resources.Report16;
                MemoryStream ms = new MemoryStream(byteData);
                LXmlDoc.Load(ms);
                Path16 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}\\Report16.rdlc", AppName));
                foreach (Standard a in ListScoreStandard)
                {
                    string nameCol = (a.ItemID + 100).ToString();
                    ListColumnName.Clear();
                    //动态加列
                    ListColumnName.Add(string.Format("Textbox{0}", nameCol));
                    ListColumnName.Add(a.Title);
                    ListColumnName.Add("C" + nameCol);
                    ListColumnName.Add(string.Format("=Fields!{0}.Value", "C" + nameCol));
                    try
                    {
                        WriteTablixColumnToXml(LXmlDoc);
                        WriteFieldToXml(LXmlDoc);
                        WriteTablixCellToXml(LXmlDoc, ListColumnName);
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex.Message);
                    }
                }
                //加总分列C100
                ListColumnName.Clear();
                //动态加列
                ListColumnName.Add(string.Format("Textbox100"));
                ListColumnName.Add(GetLanguageInfo("6101120501", "总分"));
                ListColumnName.Add("C100");
                ListColumnName.Add(string.Format("=Fields!{0}.Value", "C100"));
                try
                {
                    WriteTablixColumnToXml(LXmlDoc);
                    WriteFieldToXml(LXmlDoc);
                    WriteTablixCellToXml(LXmlDoc, ListColumnName);
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex.Message);
                }
                //保存
                LXmlDoc.Save(Path16);
                #endregion
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataRow dr_ap in ds_userid.Tables[0].Rows)
                    {
                        if (dr["C004"].ToString() == dr_ap["C001"].ToString())
                        {
                            dr["C004"] = DecryptString(dr_ap["C003"].ToString());
                        }
                    }
                    dr["C100"] = decimal.Round(decimal.Parse(dr["C100"].ToString()) / Multiple, NumAfterPoint);
                    DateTime TempTime = Converter.NumberToDatetime(dr["C003"].ToString());
                    dr["C003"] = TempTime.ToLocalTime();
                    //dr["C003"] = dr["C003"].ToString().Substring(0, 4) + "/" + dr["C003"].ToString().Substring(4, 2) + "/" + dr["C003"].ToString().Substring(6, 2);
                    for (int k = 0; k < ListScoreStandard.Count; k++)
                    {
                        string coln = (ListScoreStandard[k].ItemID + 100).ToString();
                        coln = string.Format("C{0}", coln);
                        string ColContent = string.Empty;
                        ColContent = dr[coln].ToString();
                        //MessageBox.Show(coln + "   " + ColContent);
                        if (ColContent != null && ColContent != string.Empty)
                            dr[coln] = decimal.Round((Convert.ToDecimal(ColContent) / Multiple), NumAfterPoint);
                    }
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (dr["C017"].ToString() == dr_ap["C001"].ToString())
                        {
                            dr["C016"] = dr_ap["C017"].ToString();
                            dr["C017"] = dr_ap["C018"].ToString();
                            dr["C015"] = dr_ap["C012"];
                            break;
                        }
                    }

                    string AllComment = string.Empty;
                    DataRow[] CommRow = ds_comm.Tables[0].Select(string.Format("C001={0}", dr["C001"].ToString()));
                    ListScoreComment = (from a in ListScoreComment orderby a.ScoreItem.ItemID, a.OrderID select a).ToList();
                    foreach (Comment Tcomm in ListScoreComment)
                    {
                        string TempAllComment = string.Format("{0}-{1}:", Tcomm.ScoreItem.ItemID, Tcomm.OrderID);
                        bool IsHaveComment = false;
                        foreach (DataRow dr_comm in CommRow)
                        {
                            if (dr_comm["C002"].ToString() == Tcomm.ID.ToString())
                            {
                                TempAllComment += string.Format("{0}\n", dr_comm["C003"].ToString());
                                IsHaveComment = true;
                            }
                        }
                        if (IsHaveComment)
                            AllComment += TempAllComment;
                    }
                    dr["P1602"] = AllComment;

                    foreach (DataRow dr_gn in ds_gn.Tables[0].Rows)
                    {
                        if (dr["C000"].ToString() == dr_gn["C001"].ToString())
                        {
                            dr["P1601"] = dr_gn["C002"].ToString();
                        }
                    }
                }
                ds.Tables.Add(dt);
                #endregion
            }
            return ds;
        }

        public DataSet GetR17DataSet(string StrQ, int t, string ColName)
        {
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet(); bool first_time = true; COUNT = 0;
            string sql = "SELECT COUNT(0) FROM T_31_041_" + Session.RentInfo.Token + " WHERE " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                + " AND C005 = '1' AND  " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100); DataSet ds = new DataSet();
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} * FROM T_31_041_{0} WHERE C005 = '1' AND  " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                        + " AND " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " {2} ORDER BY C001"
                                      , Session.RentInfo.Token, GetDataNum, StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT * FROM (SELECT A.* FROM T_31_041_{0} A WHERE C005 = '1' AND  {2} >="
                        + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND {2} <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ + " ORDER BY C001,{2}) WHERE ROWNUM<={1} AND C001>" + AfterRowNum
                        + " ORDER BY C001 DESC ", Session.RentInfo.Token, GetDataNum, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables.Count != 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(17):{0}", sql));
            if (CiShuSum != 0)
            {
                ListMarkInfo.Add("T_31_041");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable();
                DataColumn col_name = new DataColumn("P1701", typeof(string)); dt.Columns.Add(col_name);
                DataColumn col_score = new DataColumn("P1702", typeof(string)); dt.Columns.Add(col_score);
                DataColumn col_avescore = new DataColumn("P1703", typeof(string)); dt.Columns.Add(col_avescore);
                DataColumn col_sd = new DataColumn("P1704", typeof(string)); dt.Columns.Add(col_sd);
                DataColumn col_sdnum = new DataColumn("P1705", typeof(string)); dt.Columns.Add(col_sdnum);
                DataTable dtM = _dt.Clone();
                foreach (DataRow drM in _dt.Rows)
                {
                    dtM.ImportRow(drM);
                }
                DataSetMark.Tables.Add(dtM);
                #region
                string ScoreID = _dt.Rows[0]["C000"].ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 15;
                webRequest.Session = Session;
                webRequest.ListData.Add(ScoreID);
                webRequest.ListData.Add("0");
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R17 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return new DataSet();
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R17XmlSS Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return new DataSet();
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tScoreSheet is null"));
                    return new DataSet();
                }
                scoreSheet.Init();
                //拿所有的评分标准
                List<Standard> ListScoreStandard = new List<Standard>();
                scoreSheet.GetAllStandards(ref ListScoreStandard);
                if (ListScoreStandard.Count == 0)
                {
                    return new DataSet();
                }

                for (int k = 0; k < ListScoreStandard.Count; k++)
                {
                    DataRow row = dt.NewRow(); List<double> NumList = new List<double>();
                    row["P1701"] = ListScoreStandard[k].Title;
                    row["P1702"] = ListScoreStandard[k].TotalScore.ToString();
                    string colname = "C" + (ListScoreStandard[k].ItemID + 100).ToString();
                    double ScoreTotal = 0; double ScoreNum = 0; double Ave = 0; double sd = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        double FenShu = 0;
                        if (double.TryParse(dr[colname].ToString(), out FenShu))
                        {
                            ScoreTotal += (FenShu) / 10000;
                            NumList.Add(FenShu / 10000);
                            ScoreNum++;
                        }
                    }
                    Ave = ScoreTotal / ScoreNum;
                    decimal AveD = decimal.Round(Convert.ToDecimal(Ave.ToString()), NumAfterPoint);
                    row["P1703"] = AveD.ToString();
                    foreach (double num in NumList)
                    {
                        double Tempsd = num - Ave;
                        Tempsd *= Tempsd;
                        sd += Tempsd;
                    }
                    sd = sd / ScoreNum; sd = Math.Sqrt(sd);
                    row["P1704"] = sd;
                    row["P1705"] = decimal.Round(Convert.ToDecimal(sd * 100), NumAfterPoint).ToString() + "%";
                    dt.Rows.Add(row);
                }
                #endregion
                ds.Tables.Add(dt);
                #endregion
            }
            return ds;
        }

        public DataSet GetR18DataSet(string StrQ, int t, string ColName, List<string> ListEX)
        {
            List<string> TableName = new List<string>(); DataSet DS_temp = new DataSet();
            string sql = string.Empty; bool first_time = true; COUNT = 0; DataSet ds = new DataSet();
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;
            TableName = TableSec("T_21_001", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")); DataSet _ds = new DataSet();
            for (int s = 0; s < TableName.Count; s++)
            {
                int PreRowNum = GetDataNum; int AfterRowNum = 0; bool IsNull = true;
                do
                {
                    IsNull = false;

                    if (Session.DBType == 2)
                    {
                        sql = string.Format("SELECT TOP {1} A.C001,A.C042,A.C066,A.C012,A.C005 FROM {0} A WHERE A.C001>{5} AND A.C005 >= '{2}' AND A.C005 <= '{3}' {4}  ORDER BY A.C001,A.C005"
                        , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")
                        , ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), StrQ, AfterRowNum);
                    }
                    else
                    {
                        sql = string.Format("SELECT C001,C042,C066,C012,C005 FROM (SELECT A.* FROM {0} A WHERE "
                                + "A.C005 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND A.C005 <= TO_DATE('"
                                + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')" + StrQ + " ORDER BY A.C001,A.C005) WHERE C001>" + AfterRowNum + " AND ROWNUM<={1} ORDER BY C001,C005 DESC ", TableName[s], GetDataNum);
                    }
                    if (first_time)
                    {
                        _ds = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (_ds == null)
                        {
                            continue;
                        }
                        else if (_ds.Tables != null)
                        {
                            if (_ds.Tables.Count != 0 && _ds.Tables[0].Rows.Count != 0)
                            {
                                IsNull = true; CiShuSum++; first_time = false;
                            }
                        }
                    }
                    else
                    {
                        DS_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (DS_temp == null)
                        {
                            continue;
                        }
                        if (DS_temp.Tables == null)
                        {
                            continue;
                        }
                        else if (DS_temp.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++;

                            if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                    _ds.Tables[0].ImportRow(dr);
                        }
                    }
                    if (IsNull)
                    {
                        COUNT = _ds.Tables[0].Rows.Count - 1;
                        AfterRowNum = Convert.ToInt32(_ds.Tables[0].Rows[COUNT]["C001"]);
                    }
                }
                while (IsNull);
            }
            WriteLog(string.Format("(18):{0}", sql));
            if (CiShuSum != 0)
            {
                #region 统计数据
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable();
                DataColumn col_extname = new DataColumn("C042", typeof(string)); dt.Columns.Add(col_extname);
                #region 100Params
                DataColumn col_Param1 = new DataColumn("C101", typeof(string)); dt.Columns.Add(col_Param1);
                DataColumn col_Param2 = new DataColumn("C102", typeof(string)); dt.Columns.Add(col_Param2);
                DataColumn col_Param3 = new DataColumn("C103", typeof(string)); dt.Columns.Add(col_Param3);
                DataColumn col_Param4 = new DataColumn("C104", typeof(string)); dt.Columns.Add(col_Param4);
                DataColumn col_Param5 = new DataColumn("C105", typeof(string)); dt.Columns.Add(col_Param5);
                DataColumn col_Param6 = new DataColumn("C106", typeof(string)); dt.Columns.Add(col_Param6);
                DataColumn col_Param7 = new DataColumn("C107", typeof(string)); dt.Columns.Add(col_Param7);
                DataColumn col_Param8 = new DataColumn("C108", typeof(string)); dt.Columns.Add(col_Param8);
                DataColumn col_Param9 = new DataColumn("C109", typeof(string)); dt.Columns.Add(col_Param9);
                DataColumn col_Param10 = new DataColumn("C110", typeof(string)); dt.Columns.Add(col_Param10);
                DataColumn col_Param11 = new DataColumn("C111", typeof(string)); dt.Columns.Add(col_Param11);
                DataColumn col_Param12 = new DataColumn("C112", typeof(string)); dt.Columns.Add(col_Param12);
                DataColumn col_Param13 = new DataColumn("C113", typeof(string)); dt.Columns.Add(col_Param13);
                DataColumn col_Param14 = new DataColumn("C114", typeof(string)); dt.Columns.Add(col_Param14);
                DataColumn col_Param15 = new DataColumn("C115", typeof(string)); dt.Columns.Add(col_Param15);
                DataColumn col_Param16 = new DataColumn("C116", typeof(string)); dt.Columns.Add(col_Param16);
                DataColumn col_Param17 = new DataColumn("C117", typeof(string)); dt.Columns.Add(col_Param17);
                DataColumn col_Param18 = new DataColumn("C118", typeof(string)); dt.Columns.Add(col_Param18);
                DataColumn col_Param19 = new DataColumn("C119", typeof(string)); dt.Columns.Add(col_Param19);
                DataColumn col_Param20 = new DataColumn("C120", typeof(string)); dt.Columns.Add(col_Param20);
                DataColumn col_Param21 = new DataColumn("C121", typeof(string)); dt.Columns.Add(col_Param21);
                DataColumn col_Param22 = new DataColumn("C122", typeof(string)); dt.Columns.Add(col_Param22);
                DataColumn col_Param23 = new DataColumn("C123", typeof(string)); dt.Columns.Add(col_Param23);
                DataColumn col_Param24 = new DataColumn("C124", typeof(string)); dt.Columns.Add(col_Param24);
                DataColumn col_Param25 = new DataColumn("C125", typeof(string)); dt.Columns.Add(col_Param25);
                DataColumn col_Param26 = new DataColumn("C126", typeof(string)); dt.Columns.Add(col_Param26);
                DataColumn col_Param27 = new DataColumn("C127", typeof(string)); dt.Columns.Add(col_Param27);
                DataColumn col_Param28 = new DataColumn("C128", typeof(string)); dt.Columns.Add(col_Param28);
                DataColumn col_Param29 = new DataColumn("C129", typeof(string)); dt.Columns.Add(col_Param29);
                DataColumn col_Param30 = new DataColumn("C130", typeof(string)); dt.Columns.Add(col_Param30);
                DataColumn col_Param31 = new DataColumn("C131", typeof(string)); dt.Columns.Add(col_Param31);
                DataColumn col_Param32 = new DataColumn("C132", typeof(string)); dt.Columns.Add(col_Param32);
                DataColumn col_Param33 = new DataColumn("C133", typeof(string)); dt.Columns.Add(col_Param33);
                DataColumn col_Param34 = new DataColumn("C134", typeof(string)); dt.Columns.Add(col_Param34);
                DataColumn col_Param35 = new DataColumn("C135", typeof(string)); dt.Columns.Add(col_Param35);
                DataColumn col_Param36 = new DataColumn("C136", typeof(string)); dt.Columns.Add(col_Param36);
                DataColumn col_Param37 = new DataColumn("C137", typeof(string)); dt.Columns.Add(col_Param37);
                DataColumn col_Param38 = new DataColumn("C138", typeof(string)); dt.Columns.Add(col_Param38);
                DataColumn col_Param39 = new DataColumn("C139", typeof(string)); dt.Columns.Add(col_Param39);
                DataColumn col_Param40 = new DataColumn("C140", typeof(string)); dt.Columns.Add(col_Param40);
                DataColumn col_Param41 = new DataColumn("C141", typeof(string)); dt.Columns.Add(col_Param41);
                DataColumn col_Param42 = new DataColumn("C142", typeof(string)); dt.Columns.Add(col_Param42);
                DataColumn col_Param43 = new DataColumn("C143", typeof(string)); dt.Columns.Add(col_Param43);
                DataColumn col_Param44 = new DataColumn("C144", typeof(string)); dt.Columns.Add(col_Param44);
                DataColumn col_Param45 = new DataColumn("C145", typeof(string)); dt.Columns.Add(col_Param45);
                DataColumn col_Param46 = new DataColumn("C146", typeof(string)); dt.Columns.Add(col_Param46);
                DataColumn col_Param47 = new DataColumn("C147", typeof(string)); dt.Columns.Add(col_Param47);
                DataColumn col_Param48 = new DataColumn("C148", typeof(string)); dt.Columns.Add(col_Param48);
                DataColumn col_Param49 = new DataColumn("C149", typeof(string)); dt.Columns.Add(col_Param49);
                DataColumn col_Param50 = new DataColumn("C150", typeof(string)); dt.Columns.Add(col_Param50);
                DataColumn col_Param51 = new DataColumn("C151", typeof(string)); dt.Columns.Add(col_Param51);
                DataColumn col_Param52 = new DataColumn("C152", typeof(string)); dt.Columns.Add(col_Param52);
                DataColumn col_Param53 = new DataColumn("C153", typeof(string)); dt.Columns.Add(col_Param53);
                DataColumn col_Param54 = new DataColumn("C154", typeof(string)); dt.Columns.Add(col_Param54);
                DataColumn col_Param55 = new DataColumn("C155", typeof(string)); dt.Columns.Add(col_Param55);
                DataColumn col_Param56 = new DataColumn("C156", typeof(string)); dt.Columns.Add(col_Param56);
                DataColumn col_Param57 = new DataColumn("C157", typeof(string)); dt.Columns.Add(col_Param57);
                DataColumn col_Param58 = new DataColumn("C158", typeof(string)); dt.Columns.Add(col_Param58);
                DataColumn col_Param59 = new DataColumn("C159", typeof(string)); dt.Columns.Add(col_Param59);
                DataColumn col_Param60 = new DataColumn("C160", typeof(string)); dt.Columns.Add(col_Param60);
                DataColumn col_Param61 = new DataColumn("C161", typeof(string)); dt.Columns.Add(col_Param61);
                DataColumn col_Param62 = new DataColumn("C162", typeof(string)); dt.Columns.Add(col_Param62);
                DataColumn col_Param63 = new DataColumn("C163", typeof(string)); dt.Columns.Add(col_Param63);
                DataColumn col_Param64 = new DataColumn("C164", typeof(string)); dt.Columns.Add(col_Param64);
                DataColumn col_Param65 = new DataColumn("C165", typeof(string)); dt.Columns.Add(col_Param65);
                DataColumn col_Param66 = new DataColumn("C166", typeof(string)); dt.Columns.Add(col_Param66);
                DataColumn col_Param67 = new DataColumn("C167", typeof(string)); dt.Columns.Add(col_Param67);
                DataColumn col_Param68 = new DataColumn("C168", typeof(string)); dt.Columns.Add(col_Param68);
                DataColumn col_Param69 = new DataColumn("C169", typeof(string)); dt.Columns.Add(col_Param69);
                DataColumn col_Param70 = new DataColumn("C170", typeof(string)); dt.Columns.Add(col_Param70);
                DataColumn col_Param71 = new DataColumn("C171", typeof(string)); dt.Columns.Add(col_Param71);
                DataColumn col_Param72 = new DataColumn("C172", typeof(string)); dt.Columns.Add(col_Param72);
                DataColumn col_Param73 = new DataColumn("C173", typeof(string)); dt.Columns.Add(col_Param73);
                DataColumn col_Param74 = new DataColumn("C174", typeof(string)); dt.Columns.Add(col_Param74);
                DataColumn col_Param75 = new DataColumn("C175", typeof(string)); dt.Columns.Add(col_Param75);
                DataColumn col_Param76 = new DataColumn("C176", typeof(string)); dt.Columns.Add(col_Param76);
                DataColumn col_Param77 = new DataColumn("C177", typeof(string)); dt.Columns.Add(col_Param77);
                DataColumn col_Param78 = new DataColumn("C178", typeof(string)); dt.Columns.Add(col_Param78);
                DataColumn col_Param79 = new DataColumn("C179", typeof(string)); dt.Columns.Add(col_Param79);
                DataColumn col_Param80 = new DataColumn("C180", typeof(string)); dt.Columns.Add(col_Param80);
                DataColumn col_Param81 = new DataColumn("C181", typeof(string)); dt.Columns.Add(col_Param81);
                DataColumn col_Param82 = new DataColumn("C182", typeof(string)); dt.Columns.Add(col_Param82);
                DataColumn col_Param83 = new DataColumn("C183", typeof(string)); dt.Columns.Add(col_Param83);
                DataColumn col_Param84 = new DataColumn("C184", typeof(string)); dt.Columns.Add(col_Param84);
                DataColumn col_Param85 = new DataColumn("C185", typeof(string)); dt.Columns.Add(col_Param85);
                DataColumn col_Param86 = new DataColumn("C186", typeof(string)); dt.Columns.Add(col_Param86);
                DataColumn col_Param87 = new DataColumn("C187", typeof(string)); dt.Columns.Add(col_Param87);
                DataColumn col_Param88 = new DataColumn("C188", typeof(string)); dt.Columns.Add(col_Param88);
                DataColumn col_Param89 = new DataColumn("C189", typeof(string)); dt.Columns.Add(col_Param89);
                DataColumn col_Param90 = new DataColumn("C190", typeof(string)); dt.Columns.Add(col_Param90);
                DataColumn col_Param91 = new DataColumn("C191", typeof(string)); dt.Columns.Add(col_Param91);
                DataColumn col_Param92 = new DataColumn("C192", typeof(string)); dt.Columns.Add(col_Param92);
                DataColumn col_Param93 = new DataColumn("C193", typeof(string)); dt.Columns.Add(col_Param93);
                DataColumn col_Param94 = new DataColumn("C194", typeof(string)); dt.Columns.Add(col_Param94);
                DataColumn col_Param95 = new DataColumn("C195", typeof(string)); dt.Columns.Add(col_Param95);
                DataColumn col_Param96 = new DataColumn("C196", typeof(string)); dt.Columns.Add(col_Param96);
                DataColumn col_Param97 = new DataColumn("C197", typeof(string)); dt.Columns.Add(col_Param97);
                DataColumn col_Param98 = new DataColumn("C198", typeof(string)); dt.Columns.Add(col_Param98);
                DataColumn col_Param99 = new DataColumn("C199", typeof(string)); dt.Columns.Add(col_Param99);
                DataColumn col_Param100 = new DataColumn("C200", typeof(string)); dt.Columns.Add(col_Param100);
                #endregion
                #region //动态加列
                List<string> ListColumnName = new List<string>();
                XmlDocument LXmlDoc = new XmlDocument();
                if (Flag18)
                {
                    var byteData = UMPS6101.Properties.Resources.Report18;
                    MemoryStream ms = new MemoryStream(byteData);
                    LXmlDoc.Load(ms);
                    Path18 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}\\Report18.rdlc", AppName));
                    Flag18 = false;
                }
                else
                {
                    LXmlDoc.Load(Path18);
                }
                int numCol = t + 101;
                string nameCol = (numCol).ToString();
                string columnName = "C" + nameCol;
                foreach (string extension in ListEX)
                {
                    DataRow dr_new = dt.NewRow();
                    dr_new["C042"] = extension;
                    dr_new[columnName] = "0";
                    dt.Rows.Add(dr_new);
                }
                string ColTitle = string.Format("{0}/{1}/{2}", ADT.BeginDateTime[t].Year, ADT.BeginDateTime[t].Month, ADT.BeginDateTime[t].Day);
                ListColumnName.Clear();
                //动态加列
                ListColumnName.Add(string.Format("Textbox{0}", nameCol));
                ListColumnName.Add(ColTitle);
                ListColumnName.Add(columnName);
                ListColumnName.Add(string.Format("=Fields!{0}.Value", columnName));
                try
                {
                    WriteTablixColumnToXml(LXmlDoc);
                    WriteFieldToXml(LXmlDoc);
                    WriteTablixCellToXml(LXmlDoc, ListColumnName);
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex.Message);
                }

                //保存
                LXmlDoc.Save(Path18);
                #endregion
                //=============px统计

                List<RecordInfo> ListRecordInfo = new List<RecordInfo>();
                foreach (DataRow dr in _dt.Rows)
                {
                    RecordInfo RI = new RecordInfo();
                    RI.ExtName = dr["C042"].ToString();
                    RI.Trans = Convert.ToInt32(dr["C066"].ToString());
                    RI.TimeLength = Convert.ToInt32(dr["C012"].ToString());
                    ListRecordInfo.Add(RI);
                }
                var VarTemp = ListRecordInfo.GroupBy(p => p.ExtName).ToList();

                switch (ColName)
                {
                    case "0"://次数

                        for (int LCount = 0; LCount < VarTemp.Count; LCount++)
                        {
                            string Exname = VarTemp[LCount].Key;
                            int position;
                            for (position = 0; position < dt.Rows.Count; position++)
                            {
                                if (dt.Rows[position]["C042"].ToString() == Exname)
                                {
                                    break;
                                }
                            }
                            dt.Rows[position][columnName] = VarTemp[LCount].Count().ToString();
                        }

                        break;
                    case "1"://时长
                        for (int LCount = 0; LCount < VarTemp.Count; LCount++)
                        {
                            string Exname = VarTemp[LCount].Key;
                            int position;
                            for (position = 0; position < dt.Rows.Count; position++)
                            {
                                if (dt.Rows[position]["C042"].ToString() == Exname)
                                {
                                    break;
                                }
                            }
                            double TimeL = 0;
                            for (int LLpos = 0; LLpos < VarTemp[LCount].Count(); LLpos++)
                            {
                                List<RecordInfo> GroupList = VarTemp[LCount].ToList();
                                TimeL += GroupList[LLpos].TimeLength;
                            }
                            dt.Rows[position][columnName] = Converter.Second2Time(TimeL);
                        }
                        break;
                    case "2"://转接
                        for (int LCount = 0; LCount < VarTemp.Count; LCount++)
                        {
                            string Exname = VarTemp[LCount].Key;
                            int position;
                            for (position = 0; position < dt.Rows.Count; position++)
                            {
                                if (dt.Rows[position]["C042"].ToString() == Exname)
                                {
                                    break;
                                }
                            }
                            double TransTimes = 0;
                            for (int LLpos = 0; LLpos < VarTemp[LCount].Count(); LLpos++)
                            {
                                List<RecordInfo> GroupList = VarTemp[LCount].ToList();
                                if (GroupList[LLpos].Trans > 0)
                                {
                                    TransTimes++;
                                }
                            }
                            dt.Rows[position][columnName] = TransTimes;
                        }
                        break;
                }
                ds.Tables.Add(dt);
                #endregion
            }
            return ds;
        }

        public DataSet GetR19DataSet(string StrQ, int t, string ColName, List<string> ListGS)
        {
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month);
            int CiShuSum = 0; int count_num = 0; DataSet _ds = new DataSet(); DataSet DS_temp = new DataSet(); bool first_time = true; COUNT = 0;
            string sql = "SELECT COUNT(0) FROM T_31_041_" + Session.RentInfo.Token + " WHERE " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                + " AND C005 = '1' AND  " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ;
            DataSet ds_temp = GetDataSetFromDB(sql, 100); DataSet ds = new DataSet();
            if (ds_temp.Tables.Count != 0)
            {
                count_num = int.Parse(ds_temp.Tables[0].Rows[0][0].ToString());
            } int CiShu = count_num % GetDataNum;
            if (CiShu != 0)
                CiShu = count_num / GetDataNum + 1;
            else
                CiShu = count_num / GetDataNum; CiShuSum += CiShu; int PreRowNum = GetDataNum; string AfterRowNum = "0";
            for (int j = 1; j <= CiShu; j++)
            {
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {1} * FROM T_31_041_{0} WHERE C005 = '1' AND  " + ColName + " >=" + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss")
                        + " AND " + ColName + " <= " + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " {2} ORDER BY C001"
                                      , Session.RentInfo.Token, GetDataNum, StrQ + " AND C001 >" + AfterRowNum);
                }
                else
                {
                    sql = string.Format("SELECT * FROM (SELECT A.* FROM T_31_041_{0} A WHERE C005 = '1' AND  {2} >="
                        + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND {2} <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ + " ORDER BY C001,{2}) WHERE ROWNUM<={1} AND C001>" + AfterRowNum
                        + " ORDER BY C001 DESC ", Session.RentInfo.Token, GetDataNum, ColName);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100); first_time = false;
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (_ds.Tables.Count != 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToString(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            }
            WriteLog(string.Format("(19):{0}", sql));
            if (CiShuSum != 0)
            {
                ListMarkInfo.Add("T_31_041");
                ListMarkInfo.Add("C001");
                ListMarkInfo.Add("C001");
                #region 统计数据
                DataTable _dt = _ds.Tables[0]; DataTable dt = new DataTable(); DataTable dtM = _dt.Clone();
                DataColumn col_agentname = new DataColumn("C017", typeof(string)); dt.Columns.Add(col_agentname);
                DataColumn col_agentid = new DataColumn("C056", typeof(string)); dt.Columns.Add(col_agentid);
                DataColumn col_part = new DataColumn("C015", typeof(string)); dt.Columns.Add(col_part);
                DataColumn col_scorename = new DataColumn("C000", typeof(string)); dt.Columns.Add(col_scorename);
                DataColumn col_sumscore = new DataColumn("C002", typeof(decimal)); dt.Columns.Add(col_sumscore);
                DataColumn col_num = new DataColumn("C001", typeof(string)); dt.Columns.Add(col_num);
                DataColumn col_sum = new DataColumn("C100", typeof(decimal)); dt.Columns.Add(col_sum);
                DataTable dt2 = new DataTable();
                DataColumn col2_Score = new DataColumn("C098", typeof(string)); dt2.Columns.Add(col2_Score);
                DataColumn col2_AgentID = new DataColumn("C099", typeof(string)); dt2.Columns.Add(col2_AgentID);

                #region 200Params
                DataColumn col_Param1 = new DataColumn("C101", typeof(string)); dt.Columns.Add(col_Param1);
                DataColumn col_Param2 = new DataColumn("C102", typeof(string)); dt.Columns.Add(col_Param2);
                DataColumn col_Param3 = new DataColumn("C103", typeof(string)); dt.Columns.Add(col_Param3);
                DataColumn col_Param4 = new DataColumn("C104", typeof(string)); dt.Columns.Add(col_Param4);
                DataColumn col_Param5 = new DataColumn("C105", typeof(string)); dt.Columns.Add(col_Param5);
                DataColumn col_Param6 = new DataColumn("C106", typeof(string)); dt.Columns.Add(col_Param6);
                DataColumn col_Param7 = new DataColumn("C107", typeof(string)); dt.Columns.Add(col_Param7);
                DataColumn col_Param8 = new DataColumn("C108", typeof(string)); dt.Columns.Add(col_Param8);
                DataColumn col_Param9 = new DataColumn("C109", typeof(string)); dt.Columns.Add(col_Param9);
                DataColumn col_Param10 = new DataColumn("C110", typeof(string)); dt.Columns.Add(col_Param10);
                DataColumn col_Param11 = new DataColumn("C111", typeof(string)); dt.Columns.Add(col_Param11);
                DataColumn col_Param12 = new DataColumn("C112", typeof(string)); dt.Columns.Add(col_Param12);
                DataColumn col_Param13 = new DataColumn("C113", typeof(string)); dt.Columns.Add(col_Param13);
                DataColumn col_Param14 = new DataColumn("C114", typeof(string)); dt.Columns.Add(col_Param14);
                DataColumn col_Param15 = new DataColumn("C115", typeof(string)); dt.Columns.Add(col_Param15);
                DataColumn col_Param16 = new DataColumn("C116", typeof(string)); dt.Columns.Add(col_Param16);
                DataColumn col_Param17 = new DataColumn("C117", typeof(string)); dt.Columns.Add(col_Param17);
                DataColumn col_Param18 = new DataColumn("C118", typeof(string)); dt.Columns.Add(col_Param18);
                DataColumn col_Param19 = new DataColumn("C119", typeof(string)); dt.Columns.Add(col_Param19);
                DataColumn col_Param20 = new DataColumn("C120", typeof(string)); dt.Columns.Add(col_Param20);
                DataColumn col_Param21 = new DataColumn("C121", typeof(string)); dt.Columns.Add(col_Param21);
                DataColumn col_Param22 = new DataColumn("C122", typeof(string)); dt.Columns.Add(col_Param22);
                DataColumn col_Param23 = new DataColumn("C123", typeof(string)); dt.Columns.Add(col_Param23);
                DataColumn col_Param24 = new DataColumn("C124", typeof(string)); dt.Columns.Add(col_Param24);
                DataColumn col_Param25 = new DataColumn("C125", typeof(string)); dt.Columns.Add(col_Param25);
                DataColumn col_Param26 = new DataColumn("C126", typeof(string)); dt.Columns.Add(col_Param26);
                DataColumn col_Param27 = new DataColumn("C127", typeof(string)); dt.Columns.Add(col_Param27);
                DataColumn col_Param28 = new DataColumn("C128", typeof(string)); dt.Columns.Add(col_Param28);
                DataColumn col_Param29 = new DataColumn("C129", typeof(string)); dt.Columns.Add(col_Param29);
                DataColumn col_Param30 = new DataColumn("C130", typeof(string)); dt.Columns.Add(col_Param30);
                DataColumn col_Param31 = new DataColumn("C131", typeof(string)); dt.Columns.Add(col_Param31);
                DataColumn col_Param32 = new DataColumn("C132", typeof(string)); dt.Columns.Add(col_Param32);
                DataColumn col_Param33 = new DataColumn("C133", typeof(string)); dt.Columns.Add(col_Param33);
                DataColumn col_Param34 = new DataColumn("C134", typeof(string)); dt.Columns.Add(col_Param34);
                DataColumn col_Param35 = new DataColumn("C135", typeof(string)); dt.Columns.Add(col_Param35);
                DataColumn col_Param36 = new DataColumn("C136", typeof(string)); dt.Columns.Add(col_Param36);
                DataColumn col_Param37 = new DataColumn("C137", typeof(string)); dt.Columns.Add(col_Param37);
                DataColumn col_Param38 = new DataColumn("C138", typeof(string)); dt.Columns.Add(col_Param38);
                DataColumn col_Param39 = new DataColumn("C139", typeof(string)); dt.Columns.Add(col_Param39);
                DataColumn col_Param40 = new DataColumn("C140", typeof(string)); dt.Columns.Add(col_Param40);
                DataColumn col_Param41 = new DataColumn("C141", typeof(string)); dt.Columns.Add(col_Param41);
                DataColumn col_Param42 = new DataColumn("C142", typeof(string)); dt.Columns.Add(col_Param42);
                DataColumn col_Param43 = new DataColumn("C143", typeof(string)); dt.Columns.Add(col_Param43);
                DataColumn col_Param44 = new DataColumn("C144", typeof(string)); dt.Columns.Add(col_Param44);
                DataColumn col_Param45 = new DataColumn("C145", typeof(string)); dt.Columns.Add(col_Param45);
                DataColumn col_Param46 = new DataColumn("C146", typeof(string)); dt.Columns.Add(col_Param46);
                DataColumn col_Param47 = new DataColumn("C147", typeof(string)); dt.Columns.Add(col_Param47);
                DataColumn col_Param48 = new DataColumn("C148", typeof(string)); dt.Columns.Add(col_Param48);
                DataColumn col_Param49 = new DataColumn("C149", typeof(string)); dt.Columns.Add(col_Param49);
                DataColumn col_Param50 = new DataColumn("C150", typeof(string)); dt.Columns.Add(col_Param50);
                DataColumn col_Param51 = new DataColumn("C151", typeof(string)); dt.Columns.Add(col_Param51);
                DataColumn col_Param52 = new DataColumn("C152", typeof(string)); dt.Columns.Add(col_Param52);
                DataColumn col_Param53 = new DataColumn("C153", typeof(string)); dt.Columns.Add(col_Param53);
                DataColumn col_Param54 = new DataColumn("C154", typeof(string)); dt.Columns.Add(col_Param54);
                DataColumn col_Param55 = new DataColumn("C155", typeof(string)); dt.Columns.Add(col_Param55);
                DataColumn col_Param56 = new DataColumn("C156", typeof(string)); dt.Columns.Add(col_Param56);
                DataColumn col_Param57 = new DataColumn("C157", typeof(string)); dt.Columns.Add(col_Param57);
                DataColumn col_Param58 = new DataColumn("C158", typeof(string)); dt.Columns.Add(col_Param58);
                DataColumn col_Param59 = new DataColumn("C159", typeof(string)); dt.Columns.Add(col_Param59);
                DataColumn col_Param60 = new DataColumn("C160", typeof(string)); dt.Columns.Add(col_Param60);
                DataColumn col_Param61 = new DataColumn("C161", typeof(string)); dt.Columns.Add(col_Param61);
                DataColumn col_Param62 = new DataColumn("C162", typeof(string)); dt.Columns.Add(col_Param62);
                DataColumn col_Param63 = new DataColumn("C163", typeof(string)); dt.Columns.Add(col_Param63);
                DataColumn col_Param64 = new DataColumn("C164", typeof(string)); dt.Columns.Add(col_Param64);
                DataColumn col_Param65 = new DataColumn("C165", typeof(string)); dt.Columns.Add(col_Param65);
                DataColumn col_Param66 = new DataColumn("C166", typeof(string)); dt.Columns.Add(col_Param66);
                DataColumn col_Param67 = new DataColumn("C167", typeof(string)); dt.Columns.Add(col_Param67);
                DataColumn col_Param68 = new DataColumn("C168", typeof(string)); dt.Columns.Add(col_Param68);
                DataColumn col_Param69 = new DataColumn("C169", typeof(string)); dt.Columns.Add(col_Param69);
                DataColumn col_Param70 = new DataColumn("C170", typeof(string)); dt.Columns.Add(col_Param70);
                DataColumn col_Param71 = new DataColumn("C171", typeof(string)); dt.Columns.Add(col_Param71);
                DataColumn col_Param72 = new DataColumn("C172", typeof(string)); dt.Columns.Add(col_Param72);
                DataColumn col_Param73 = new DataColumn("C173", typeof(string)); dt.Columns.Add(col_Param73);
                DataColumn col_Param74 = new DataColumn("C174", typeof(string)); dt.Columns.Add(col_Param74);
                DataColumn col_Param75 = new DataColumn("C175", typeof(string)); dt.Columns.Add(col_Param75);
                DataColumn col_Param76 = new DataColumn("C176", typeof(string)); dt.Columns.Add(col_Param76);
                DataColumn col_Param77 = new DataColumn("C177", typeof(string)); dt.Columns.Add(col_Param77);
                DataColumn col_Param78 = new DataColumn("C178", typeof(string)); dt.Columns.Add(col_Param78);
                DataColumn col_Param79 = new DataColumn("C179", typeof(string)); dt.Columns.Add(col_Param79);
                DataColumn col_Param80 = new DataColumn("C180", typeof(string)); dt.Columns.Add(col_Param80);
                DataColumn col_Param81 = new DataColumn("C181", typeof(string)); dt.Columns.Add(col_Param81);
                DataColumn col_Param82 = new DataColumn("C182", typeof(string)); dt.Columns.Add(col_Param82);
                DataColumn col_Param83 = new DataColumn("C183", typeof(string)); dt.Columns.Add(col_Param83);
                DataColumn col_Param84 = new DataColumn("C184", typeof(string)); dt.Columns.Add(col_Param84);
                DataColumn col_Param85 = new DataColumn("C185", typeof(string)); dt.Columns.Add(col_Param85);
                DataColumn col_Param86 = new DataColumn("C186", typeof(string)); dt.Columns.Add(col_Param86);
                DataColumn col_Param87 = new DataColumn("C187", typeof(string)); dt.Columns.Add(col_Param87);
                DataColumn col_Param88 = new DataColumn("C188", typeof(string)); dt.Columns.Add(col_Param88);
                DataColumn col_Param89 = new DataColumn("C189", typeof(string)); dt.Columns.Add(col_Param89);
                DataColumn col_Param90 = new DataColumn("C190", typeof(string)); dt.Columns.Add(col_Param90);
                DataColumn col_Param91 = new DataColumn("C191", typeof(string)); dt.Columns.Add(col_Param91);
                DataColumn col_Param92 = new DataColumn("C192", typeof(string)); dt.Columns.Add(col_Param92);
                DataColumn col_Param93 = new DataColumn("C193", typeof(string)); dt.Columns.Add(col_Param93);
                DataColumn col_Param94 = new DataColumn("C194", typeof(string)); dt.Columns.Add(col_Param94);
                DataColumn col_Param95 = new DataColumn("C195", typeof(string)); dt.Columns.Add(col_Param95);
                DataColumn col_Param96 = new DataColumn("C196", typeof(string)); dt.Columns.Add(col_Param96);
                DataColumn col_Param97 = new DataColumn("C197", typeof(string)); dt.Columns.Add(col_Param97);
                DataColumn col_Param98 = new DataColumn("C198", typeof(string)); dt.Columns.Add(col_Param98);
                DataColumn col_Param99 = new DataColumn("C199", typeof(string)); dt.Columns.Add(col_Param99);
                DataColumn col_Param100 = new DataColumn("C200", typeof(string)); dt.Columns.Add(col_Param100);
                DataColumn col_Param101 = new DataColumn("C201", typeof(string)); dt.Columns.Add(col_Param101);
                DataColumn col_Param102 = new DataColumn("C202", typeof(string)); dt.Columns.Add(col_Param102);
                DataColumn col_Param103 = new DataColumn("C203", typeof(string)); dt.Columns.Add(col_Param103);
                DataColumn col_Param104 = new DataColumn("C204", typeof(string)); dt.Columns.Add(col_Param104);
                DataColumn col_Param105 = new DataColumn("C205", typeof(string)); dt.Columns.Add(col_Param105);
                DataColumn col_Param106 = new DataColumn("C206", typeof(string)); dt.Columns.Add(col_Param106);
                DataColumn col_Param107 = new DataColumn("C207", typeof(string)); dt.Columns.Add(col_Param107);
                DataColumn col_Param108 = new DataColumn("C208", typeof(string)); dt.Columns.Add(col_Param108);
                DataColumn col_Param109 = new DataColumn("C209", typeof(string)); dt.Columns.Add(col_Param109);
                DataColumn col_Param110 = new DataColumn("C210", typeof(string)); dt.Columns.Add(col_Param110);
                DataColumn col_Param111 = new DataColumn("C211", typeof(string)); dt.Columns.Add(col_Param111);
                DataColumn col_Param112 = new DataColumn("C212", typeof(string)); dt.Columns.Add(col_Param112);
                DataColumn col_Param113 = new DataColumn("C213", typeof(string)); dt.Columns.Add(col_Param113);
                DataColumn col_Param114 = new DataColumn("C214", typeof(string)); dt.Columns.Add(col_Param114);
                DataColumn col_Param115 = new DataColumn("C215", typeof(string)); dt.Columns.Add(col_Param115);
                DataColumn col_Param116 = new DataColumn("C216", typeof(string)); dt.Columns.Add(col_Param116);
                DataColumn col_Param117 = new DataColumn("C217", typeof(string)); dt.Columns.Add(col_Param117);
                DataColumn col_Param118 = new DataColumn("C218", typeof(string)); dt.Columns.Add(col_Param118);
                DataColumn col_Param119 = new DataColumn("C219", typeof(string)); dt.Columns.Add(col_Param119);
                DataColumn col_Param120 = new DataColumn("C220", typeof(string)); dt.Columns.Add(col_Param120);
                DataColumn col_Param121 = new DataColumn("C221", typeof(string)); dt.Columns.Add(col_Param121);
                DataColumn col_Param122 = new DataColumn("C222", typeof(string)); dt.Columns.Add(col_Param122);
                DataColumn col_Param123 = new DataColumn("C223", typeof(string)); dt.Columns.Add(col_Param123);
                DataColumn col_Param124 = new DataColumn("C224", typeof(string)); dt.Columns.Add(col_Param124);
                DataColumn col_Param125 = new DataColumn("C225", typeof(string)); dt.Columns.Add(col_Param125);
                DataColumn col_Param126 = new DataColumn("C226", typeof(string)); dt.Columns.Add(col_Param126);
                DataColumn col_Param127 = new DataColumn("C227", typeof(string)); dt.Columns.Add(col_Param127);
                DataColumn col_Param128 = new DataColumn("C228", typeof(string)); dt.Columns.Add(col_Param128);
                DataColumn col_Param129 = new DataColumn("C229", typeof(string)); dt.Columns.Add(col_Param129);
                DataColumn col_Param130 = new DataColumn("C230", typeof(string)); dt.Columns.Add(col_Param130);
                DataColumn col_Param131 = new DataColumn("C231", typeof(string)); dt.Columns.Add(col_Param131);
                DataColumn col_Param132 = new DataColumn("C232", typeof(string)); dt.Columns.Add(col_Param132);
                DataColumn col_Param133 = new DataColumn("C233", typeof(string)); dt.Columns.Add(col_Param133);
                DataColumn col_Param134 = new DataColumn("C234", typeof(string)); dt.Columns.Add(col_Param134);
                DataColumn col_Param135 = new DataColumn("C235", typeof(string)); dt.Columns.Add(col_Param135);
                DataColumn col_Param136 = new DataColumn("C236", typeof(string)); dt.Columns.Add(col_Param136);
                DataColumn col_Param137 = new DataColumn("C237", typeof(string)); dt.Columns.Add(col_Param137);
                DataColumn col_Param138 = new DataColumn("C238", typeof(string)); dt.Columns.Add(col_Param138);
                DataColumn col_Param139 = new DataColumn("C239", typeof(string)); dt.Columns.Add(col_Param139);
                DataColumn col_Param140 = new DataColumn("C240", typeof(string)); dt.Columns.Add(col_Param140);
                DataColumn col_Param141 = new DataColumn("C241", typeof(string)); dt.Columns.Add(col_Param141);
                DataColumn col_Param142 = new DataColumn("C242", typeof(string)); dt.Columns.Add(col_Param142);
                DataColumn col_Param143 = new DataColumn("C243", typeof(string)); dt.Columns.Add(col_Param143);
                DataColumn col_Param144 = new DataColumn("C244", typeof(string)); dt.Columns.Add(col_Param144);
                DataColumn col_Param145 = new DataColumn("C245", typeof(string)); dt.Columns.Add(col_Param145);
                DataColumn col_Param146 = new DataColumn("C246", typeof(string)); dt.Columns.Add(col_Param146);
                DataColumn col_Param147 = new DataColumn("C247", typeof(string)); dt.Columns.Add(col_Param147);
                DataColumn col_Param148 = new DataColumn("C248", typeof(string)); dt.Columns.Add(col_Param148);
                DataColumn col_Param149 = new DataColumn("C249", typeof(string)); dt.Columns.Add(col_Param149);
                DataColumn col_Param150 = new DataColumn("C250", typeof(string)); dt.Columns.Add(col_Param150);
                DataColumn col_Param151 = new DataColumn("C251", typeof(string)); dt.Columns.Add(col_Param151);
                DataColumn col_Param152 = new DataColumn("C252", typeof(string)); dt.Columns.Add(col_Param152);
                DataColumn col_Param153 = new DataColumn("C253", typeof(string)); dt.Columns.Add(col_Param153);
                DataColumn col_Param154 = new DataColumn("C254", typeof(string)); dt.Columns.Add(col_Param154);
                DataColumn col_Param155 = new DataColumn("C255", typeof(string)); dt.Columns.Add(col_Param155);
                DataColumn col_Param156 = new DataColumn("C256", typeof(string)); dt.Columns.Add(col_Param156);
                DataColumn col_Param157 = new DataColumn("C257", typeof(string)); dt.Columns.Add(col_Param157);
                DataColumn col_Param158 = new DataColumn("C258", typeof(string)); dt.Columns.Add(col_Param158);
                DataColumn col_Param159 = new DataColumn("C259", typeof(string)); dt.Columns.Add(col_Param159);
                DataColumn col_Param160 = new DataColumn("C260", typeof(string)); dt.Columns.Add(col_Param160);
                DataColumn col_Param161 = new DataColumn("C261", typeof(string)); dt.Columns.Add(col_Param161);
                DataColumn col_Param162 = new DataColumn("C262", typeof(string)); dt.Columns.Add(col_Param162);
                DataColumn col_Param163 = new DataColumn("C263", typeof(string)); dt.Columns.Add(col_Param163);
                DataColumn col_Param164 = new DataColumn("C264", typeof(string)); dt.Columns.Add(col_Param164);
                DataColumn col_Param165 = new DataColumn("C265", typeof(string)); dt.Columns.Add(col_Param165);
                DataColumn col_Param166 = new DataColumn("C266", typeof(string)); dt.Columns.Add(col_Param166);
                DataColumn col_Param167 = new DataColumn("C267", typeof(string)); dt.Columns.Add(col_Param167);
                DataColumn col_Param168 = new DataColumn("C268", typeof(string)); dt.Columns.Add(col_Param168);
                DataColumn col_Param169 = new DataColumn("C269", typeof(string)); dt.Columns.Add(col_Param169);
                DataColumn col_Param170 = new DataColumn("C270", typeof(string)); dt.Columns.Add(col_Param170);
                DataColumn col_Param171 = new DataColumn("C271", typeof(string)); dt.Columns.Add(col_Param171);
                DataColumn col_Param172 = new DataColumn("C272", typeof(string)); dt.Columns.Add(col_Param172);
                DataColumn col_Param173 = new DataColumn("C273", typeof(string)); dt.Columns.Add(col_Param173);
                DataColumn col_Param174 = new DataColumn("C274", typeof(string)); dt.Columns.Add(col_Param174);
                DataColumn col_Param175 = new DataColumn("C275", typeof(string)); dt.Columns.Add(col_Param175);
                DataColumn col_Param176 = new DataColumn("C276", typeof(string)); dt.Columns.Add(col_Param176);
                DataColumn col_Param177 = new DataColumn("C277", typeof(string)); dt.Columns.Add(col_Param177);
                DataColumn col_Param178 = new DataColumn("C278", typeof(string)); dt.Columns.Add(col_Param178);
                DataColumn col_Param179 = new DataColumn("C279", typeof(string)); dt.Columns.Add(col_Param179);
                DataColumn col_Param180 = new DataColumn("C280", typeof(string)); dt.Columns.Add(col_Param180);
                DataColumn col_Param181 = new DataColumn("C281", typeof(string)); dt.Columns.Add(col_Param181);
                DataColumn col_Param182 = new DataColumn("C282", typeof(string)); dt.Columns.Add(col_Param182);
                DataColumn col_Param183 = new DataColumn("C283", typeof(string)); dt.Columns.Add(col_Param183);
                DataColumn col_Param184 = new DataColumn("C284", typeof(string)); dt.Columns.Add(col_Param184);
                DataColumn col_Param185 = new DataColumn("C285", typeof(string)); dt.Columns.Add(col_Param185);
                DataColumn col_Param186 = new DataColumn("C286", typeof(string)); dt.Columns.Add(col_Param186);
                DataColumn col_Param187 = new DataColumn("C287", typeof(string)); dt.Columns.Add(col_Param187);
                DataColumn col_Param188 = new DataColumn("C288", typeof(string)); dt.Columns.Add(col_Param188);
                DataColumn col_Param189 = new DataColumn("C289", typeof(string)); dt.Columns.Add(col_Param189);
                DataColumn col_Param190 = new DataColumn("C290", typeof(string)); dt.Columns.Add(col_Param190);
                DataColumn col_Param191 = new DataColumn("C291", typeof(string)); dt.Columns.Add(col_Param191);
                DataColumn col_Param192 = new DataColumn("C292", typeof(string)); dt.Columns.Add(col_Param192);
                DataColumn col_Param193 = new DataColumn("C293", typeof(string)); dt.Columns.Add(col_Param193);
                DataColumn col_Param194 = new DataColumn("C294", typeof(string)); dt.Columns.Add(col_Param194);
                DataColumn col_Param195 = new DataColumn("C295", typeof(string)); dt.Columns.Add(col_Param195);
                DataColumn col_Param196 = new DataColumn("C296", typeof(string)); dt.Columns.Add(col_Param196);
                DataColumn col_Param197 = new DataColumn("C297", typeof(string)); dt.Columns.Add(col_Param197);
                DataColumn col_Param198 = new DataColumn("C298", typeof(string)); dt.Columns.Add(col_Param198);
                DataColumn col_Param199 = new DataColumn("C299", typeof(string)); dt.Columns.Add(col_Param199);
                DataColumn col_Param200 = new DataColumn("C300", typeof(string)); dt.Columns.Add(col_Param200);
                #endregion
                List<string> ListAgentID = new List<string>();
                string ScoreSheetID = string.Empty;
                foreach (DataRow dr_gn in ds_gn.Tables[0].Rows)
                {
                    if (_dt.Rows[0]["C000"].ToString() == dr_gn["C001"].ToString())
                    {
                        ScoreSheetID = dr_gn["C002"].ToString();
                    }
                }
                foreach (DataRow row in _dt.Rows)
                {
                    dtM.ImportRow(row);
                    bool IsAgentContain = false;
                    for (int i = 0; i < ListAgentID.Count; i++)
                    {
                        if (ListAgentID[i] == row["C017"].ToString())
                        {
                            IsAgentContain = true;
                        }
                    }
                    if (!IsAgentContain)
                    {
                        ListAgentID.Add(row["C017"].ToString());
                    }
                    //dt.ImportRow(row);
                }
                DataSetMark.Tables.Add(dtM);
                List<string> ListColName = new List<string>();
                List<AgentScoreSheetCompare> ListScoreCompare = new List<AgentScoreSheetCompare>();
                #region //动态加列
                string ScoreID = _dt.Rows[0]["C000"].ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = 15;
                webRequest.Session = Session;
                webRequest.ListData.Add(ScoreID);
                webRequest.ListData.Add("0");
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R19 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return new DataSet();
                }
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("R19XmlSS Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return new DataSet();
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tScoreSheet is null"));
                    return new DataSet();
                }
                scoreSheet.Init();
                //拿所有的评分group
                List<ScoreGroup> ListScoreGroup = new List<ScoreGroup>();
                List<ScoreObject> ListScoreObject = new List<ScoreObject>();
                scoreSheet.GetAllScoreObject(ref ListScoreObject);
                foreach (ScoreObject Sobject in ListScoreObject)
                {
                    ScoreGroup TempScoreGroup = Sobject as ScoreGroup;
                    if (TempScoreGroup != null)
                    {
                        ListScoreGroup.Add(TempScoreGroup);
                    }

                }
                if (ListScoreSheet != null && ListScoreSheet.Count != 0)
                {
                    ListScoreGroup.Clear();
                    foreach (ScoreGroup sg in ListScoreSheet)
                    {
                        ListScoreGroup.Add(sg);
                    }
                    ListScoreSheet.Clear();
                }
                //else
                //{
                //    ListScoreSheet = ListScoreGroup;
                //}
                if (ListScoreGroup.Count == 0)
                {
                    return new DataSet();
                }
                List<string> ListColumnName = new List<string>(); List<string> ListStandardsName = new List<string>();
                XmlDocument LXmlDoc = new XmlDocument();
                var byteData = UMPS6101.Properties.Resources.Report19;
                MemoryStream ms = new MemoryStream(byteData);
                LXmlDoc.Load(ms);
                Path19 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}\\Report19.rdlc", AppName));
                foreach (ScoreGroup a in ListScoreGroup)
                {
                    string nameCol = (a.ItemID + 100).ToString();
                    ListColumnName.Clear();
                    ListColName.Add("C" + nameCol);
                    ListStandardsName.Add(a.Title);
                    //动态加列
                    ListColumnName.Add(string.Format("Textbox{0}", nameCol));
                    ListColumnName.Add(a.Title);
                    ListColumnName.Add("C" + nameCol);
                    ListColumnName.Add(string.Format("=Fields!{0}.Value", "C" + nameCol));
                    try
                    {
                        WriteTablixColumnToXml(LXmlDoc);
                        WriteFieldToXml(LXmlDoc);
                        WriteTablixCellToXml(LXmlDoc, ListColumnName);
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex.Message);
                    }
                }
                //加总分列C002
                ListColumnName.Clear();
                //ListColName.Add("C02");
                //ListStandardsName.Add(CurrentApp.GetLanguageInfo("6101121903",""));
                //动态加列
                ListColumnName.Add(string.Format("Textbox002"));
                ListColumnName.Add(GetLanguageInfo("6101121903", "总平均分"));
                ListColumnName.Add("C002");
                ListColumnName.Add(string.Format("=Fields!{0}.Value", "C002"));
                try
                {
                    WriteTablixColumnToXml(LXmlDoc);
                    WriteFieldToXml(LXmlDoc);
                    WriteTablixCellToXml(LXmlDoc, ListColumnName);
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex.Message);
                }
                //保存
                LXmlDoc.Save(Path19);
                #endregion
                for (int agentid = 0; agentid < ListAgentID.Count; agentid++)
                {
                    DataRow newRowdt = dt.NewRow();
                    foreach (DataRow dr_ap in ds_agent.Tables[0].Rows)
                    {
                        if (ListAgentID[agentid] == dr_ap["C001"].ToString())
                        {
                            newRowdt["C056"] = dr_ap["C017"].ToString();
                            newRowdt["C017"] = dr_ap["C018"].ToString();
                            newRowdt["C015"] = dr_ap["C012"].ToString();
                            break;
                        }
                    }
                    newRowdt["C000"] = ScoreSheetID;
                    double ScoreCount = 0;
                    double ScoreSum = 0; double tempscore = 0;
                    for (int scoreid = 0; scoreid < ListColName.Count; scoreid++)
                    {
                        string ColScoreName = ListColName[scoreid];
                        AgentScoreSheetCompare agentScoresheet = new AgentScoreSheetCompare();
                        agentScoresheet.StrAgent = newRowdt["C056"].ToString();//得到坐席名称
                        agentScoresheet.StrStandards = ListStandardsName[scoreid];//得到评分表项

                        double ListScores = 0; ScoreCount = 0;
                        foreach (DataRow dr_dt in _dt.Rows)
                        {
                            string Strscore = string.Empty;
                            Strscore = dr_dt[ColScoreName].ToString();
                            if (double.TryParse(Strscore, out tempscore))
                            {
                                if (dr_dt["C017"].ToString() == ListAgentID[agentid])
                                {
                                    ScoreCount++;
                                    if (scoreid == 0)
                                    {
                                        ScoreSum += Convert.ToDouble(dr_dt["C100"].ToString()) / Multiple;
                                    }
                                    ListScores += tempscore / Multiple;
                                }
                            }
                        }
                        double tempAvge = ListScores / ScoreCount;
                        agentScoresheet.DoubleScore = tempAvge;//得到该项平均分
                        ListScoreCompare.Add(agentScoresheet);//保存到集合里面

                        newRowdt[ColScoreName] = decimal.Round(decimal.Parse((tempAvge).ToString()), NumAfterPoint);
                        //ScoreSum += tempAvge;
                    }

                    newRowdt["C001"] = ScoreCount;
                    newRowdt["C002"] = decimal.Round(decimal.Parse((ScoreSum / (ScoreCount)).ToString()), NumAfterPoint);
                    dt.Rows.Add(newRowdt);
                }
                List<DataRow> dtRows = dt.Select("", "[C002] DESC").ToList();
                DataTable dt1 = dt.Clone();
                dt1.Rows.Clear();
                foreach (DataRow drR in dtRows)
                {
                    dt1.ImportRow(drR);
                }
                //做小表。所有数据在list  ListScoreCompare 里
                dt2.Clear();
                for (int scoreid = 0; scoreid < ListStandardsName.Count; scoreid++)
                {
                    for (int num = 0; num < scoreSheet.Items.Count; num++)
                    {
                        if (scoreSheet.Items[num].Title == ListStandardsName[scoreid])
                        {
                            DataRow newrow2 = dt2.NewRow();
                            newrow2["C098"] = ListStandardsName[scoreid]; newrow2["C099"] = GetLanguageInfo("", "AgentID");
                            dt2.Rows.Add(newrow2);
                            List<AgentScoreSheetCompare> ListTemp = ListScoreCompare.Where(p => p.StrStandards == ListStandardsName[scoreid]).ToList();
                            ListTemp = ListTemp.OrderByDescending(p => p.DoubleScore).ToList(); int countnumber = TopNum;
                            if (ListTemp.Count < countnumber)
                            {
                                countnumber = ListTemp.Count;
                            }
                            for (int i = 0; i < countnumber; i++)
                            {
                                DataRow dr = dt2.NewRow();
                                dr["C098"] = decimal.Round(decimal.Parse((ListTemp[i].DoubleScore).ToString()), NumAfterPoint);
                                dr["C099"] = ListTemp[i].StrAgent;
                                dt2.Rows.Add(dr);
                            }
                        }
                    }
                }
                ds.Tables.Add(dt1);
                ds.Tables.Add(dt2);
                #endregion
            }
            return ds;
        }

        public DataSet GetR20DataSet(string StrQ, int t, int DepartBasic)
        {
            DataTable dt = new DataTable(); DataSet ds = new DataSet(); DataTable DT = new DataTable(); DataTable DT_Chart = new DataTable();
            DataColumn col_agentid = new DataColumn("C039", typeof(string)); dt.Columns.Add(col_agentid);
            DataColumn col_ext = new DataColumn("C042", typeof(string)); dt.Columns.Add(col_ext);
            DataColumn col_part = new DataColumn("P2001", typeof(string)); dt.Columns.Add(col_part);
            DataColumn col_cin = new DataColumn("P2002", typeof(int)); dt.Columns.Add(col_cin);
            DataColumn col_cout = new DataColumn("P2003", typeof(int)); dt.Columns.Add(col_cout);
            DataColumn col_cintime = new DataColumn("P2004", typeof(string)); dt.Columns.Add(col_cintime);
            DataColumn col_couttime = new DataColumn("P2005", typeof(string)); dt.Columns.Add(col_couttime);
            DataColumn col_avrragetime = new DataColumn("P2008", typeof(string)); dt.Columns.Add(col_avrragetime);
            DataColumn col_alltime = new DataColumn("P2007", typeof(string)); dt.Columns.Add(col_alltime);
            DataColumn col_allc = new DataColumn("P2006", typeof(int)); dt.Columns.Add(col_allc);
            DataColumn col_absdate = new DataColumn("P107", typeof(string)); dt.Columns.Add(col_absdate);

            DT_Chart = dt.Clone();
            DataTable _dt1 = ds_agent.Tables[0]; COUNT = 0;
            DT = dt.Clone(); List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;
            TableName = TableSec("T_21_001", ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss")); DataSet _ds = new DataSet();
            for (int s = 0; s < TableName.Count; s++)
            {
                int AfterRowNum = 0; bool IsNull = true;
                do
                {
                    IsNull = false;

                    if (Session.DBType == 2)
                    {
                        sql = string.Format("SELECT TOP {1} C001,C002,C039,C042,C012,C005,C045,C110 FROM {0} A WHERE C001>{5} AND C005 >= '{2}' AND C005 <= '{3}' {4}  ORDER BY C001,C005"
                            , TableName[s], GetDataNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), StrQ, AfterRowNum);
                    }
                    else
                    {
                        sql = string.Format("SELECT C001,C002,C039,C042,C012,C005,C045,C110 FROM (SELECT A.* FROM {0} A WHERE "
                            + "C005 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND C005 <= TO_DATE('"
                            + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')" + StrQ + " ORDER BY C001,C005) WHERE C001 >" + AfterRowNum + " AND ROWNUM<={1} ORDER BY C001,C005 DESC ", TableName[s], GetDataNum);
                    }
                    if (first_time)
                    {
                        _ds = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (_ds == null)
                        {
                            continue;
                        }
                        if (_ds.Tables != null && _ds.Tables.Count != 0)
                            if (_ds.Tables[0].Rows.Count != 0)
                            {
                                IsNull = true; CiShuSum++; first_time = false;
                            }
                    }
                    else
                    {
                        DS_temp = GetDataSetFromDB(sql, 100, TableName[s]);
                        if (DS_temp == null)
                        {
                            continue;
                        }
                        if (DS_temp.Tables == null)
                        {
                            continue;
                        }
                        else if (DS_temp.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++;
                        }
                        if (DS_temp != null)
                        {
                            if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                                foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                    _ds.Tables[0].ImportRow(dr);
                        }
                    }
                    if (IsNull)
                    {
                        COUNT = _ds.Tables[0].Rows.Count - 1;
                        AfterRowNum = Convert.ToInt32(_ds.Tables[0].Rows[COUNT]["C001"]);
                    }
                } while (IsNull);
            }

            WriteLog(string.Format("(20):{0}", sql));
            if (CiShuSum != 0)
            {
                DataTable _dt = _ds.Tables[0];
                DataColumn col_partmant = new DataColumn("P2001", typeof(string)); _dt.Columns.Add(col_partmant);
                #region 统计数据
                if (DepartBasic == 0)//agent
                {
                    foreach (DataRow drs in _dt.Rows)
                    {
                        for (int rowc = 0; rowc < ds_agent.Tables[0].Rows.Count; rowc++)
                        {
                            if (ds_agent.Tables[0].Rows[rowc]["C017"].ToString() == drs["C039"].ToString())
                            {
                                drs["P2001"] = ds_agent.Tables[0].Rows[rowc]["C012"].ToString(); break;
                            }
                        }
                    }
                }
                else//ext
                {
                    foreach (DataRow drs in _dt.Rows)
                    {
                        for (int rowc = 0; rowc < ds_ext.Tables[0].Rows.Count; rowc++)
                        {
                            string[] arrInfo = ds_ext.Tables[0].Rows[rowc]["C017"].ToString().Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrInfo.Length < 2) { continue; }
                            string strExtension = arrInfo[0];
                            string strIP = arrInfo[1];
                            if (strExtension == drs["C042"].ToString() && strIP == drs["C110"].ToString())
                            {
                                drs["P2001"] = ds_ext.Tables[0].Rows[rowc]["C012"].ToString(); break;
                            }
                        }
                    }
                }

                List<string> DepartmentList = new List<string>();
                foreach (DataRow depart in _dt.Rows)
                {
                    bool flag = true;
                    for (int i = 0; i < DepartmentList.Count; i++)
                    {
                        if (depart["P2001"].ToString() == DepartmentList[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        DepartmentList.Add(depart["P2001"].ToString());
                    }
                }
                TransData(DepartmentList, dt, "P2001", 1);
                for (int i = 0; i < DepartmentList.Count; i++)
                {
                    int cin = 0; int cout = 0; int cintime = 0; int couttime = 0;
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (DepartmentList[i] == dr["P2001"].ToString())
                        {
                            string StrTemp = Convert.ToDateTime(dr["C005"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            dt.Rows[i]["P107"] = AbsoluteDate(StrTemp.Substring(0, StrTemp.Length - 8));
                            if (dr["C045"].ToString() == "1")
                            {
                                cin += 1;
                                cintime += Convert.ToInt32(dr["C012"]);
                            }
                            else
                            {
                                cout += 1;
                                couttime += Convert.ToInt32(dr["C012"]);
                            }
                        }
                    }
                    dt.Rows[i]["P2006"] = cin + cout; dt.Rows[i]["P2002"] = cin; dt.Rows[i]["P2003"] = cout; dt.Rows[i]["P2004"] = TransTimeForm(cintime); dt.Rows[i]["P2005"] = TransTimeForm(couttime);
                    dt.Rows[i]["P2007"] = TransTimeForm(cintime + couttime);
                    if (cin + cout == 0)
                    {
                        dt.Rows[i]["P2008"] = "00:00:00";
                    }
                    else
                        dt.Rows[i]["P2008"] = TransTimeForm((cintime + couttime) / (cin + cout));
                }
                #endregion
                foreach (DataRow dr in dt.Rows)
                    DT.ImportRow(dr);
                ds.Tables.Add(DT);
                DataRow[] DR = dt.Select("P2001<>''");
                for (int i = 0; i < DR.Count(); i++)
                {
                    DT_Chart.ImportRow(DR[i]);
                }
                ds.Tables.Add(DT_Chart);
            }
            return ds;
        }

        public DataSet GetR21DataSet(string Deal, int t)
        {
            DataTable _dt1 = ds_userid.Tables[0]; COUNT = 0; DataSet _ds = new DataSet();
            List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;

            long AfterRowNum = 0; bool IsNull = true;
            do
            {
                IsNull = false;
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {0} A.C023,A.C024,A.C006,A.C007,A.C014,A.C015,A.C019,A.C001,B.C002 P302,C.C002 P502,D.C002 P902 " +
                        "FROM T_25_001 A LEFT JOIN T_25_003 B ON A.C001=B.C001 LEFT JOIN T_25_005 C ON B.C001=C.C001 LEFT JOIN T_25_009 D ON C.C001=D.C001 " +
                    "WHERE A.C001>{1} AND A.C019 >= '{2}' AND A.C019 <= '{3}' AND A.C002=0 AND A.C012=3  ORDER BY A.C001,A.C019"
                        , GetDataNum, AfterRowNum, ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"), ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    sql = string.Format("SELECT A.C023,A.C024,A.C006,A.C007,A.C014,A.C015,A.C019,A.C001,B.C002 P302,C.C002 P502,D.C002 P902 FROM (SELECT  A.C023,A.C024,A.C006,A.C007,A.C014,A.C015,A.C001,B.C002 P302,C.C002 P502,D.C002 P902 "
                        + " FROM T_25_001 A LEFT JOIN T_25_003 B ON A.C001=B.C001 LEFT JOIN T_25_005 C ON B.C001=C.C001 LEFT JOIN T_25_009 D ON C.C001=D.C001 WHERE "
                        + " A.C019 >= TO_DATE('" + ADT.BeginDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') AND A.C019 <= TO_DATE('"
                        + ADT.EndDateTime[t].ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') ORDER BY A.C001,A.C019) WHERE A.C001 >" + AfterRowNum + " AND ROWNUM<={0} ORDER BY A.C001,A.C019 DESC ", GetDataNum);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100);
                    if (_ds == null)
                    {
                        continue;
                    }
                    if (_ds.Tables != null && _ds.Tables.Count != 0)
                        if (_ds.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++; first_time = false;
                        }
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp == null)
                    {
                        continue;
                    }
                    if (DS_temp.Tables == null)
                    {
                        continue;
                    }
                    else if (DS_temp.Tables[0].Rows.Count != 0)
                    {
                        IsNull = true; CiShuSum++;
                    }
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (IsNull)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToInt64(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            } while (IsNull);

            WriteLog(string.Format("(21):{0}", sql));
            DataTable dt = new DataTable(); DataSet ds = new DataSet();
            if (CiShuSum != 0)
            {
                DataColumn col_alarmid = new DataColumn("C023", typeof(string)); dt.Columns.Add(col_alarmid);
                DataColumn col_content = new DataColumn("P2101", typeof(string)); dt.Columns.Add(col_content);
                DataColumn col_alarmtime = new DataColumn("C019", typeof(string)); dt.Columns.Add(col_alarmtime);
                DataColumn col_dealtime = new DataColumn("P2102", typeof(string)); dt.Columns.Add(col_dealtime);
                DataColumn col_mail = new DataColumn("P2103", typeof(string)); dt.Columns.Add(col_mail);
                DataColumn col_client = new DataColumn("P2104", typeof(string)); dt.Columns.Add(col_client);
                DataColumn col_app = new DataColumn("P2105", typeof(string)); dt.Columns.Add(col_app);
                DataColumn col_deal = new DataColumn("C024", typeof(string)); dt.Columns.Add(col_deal);
                DataColumn col_6 = new DataColumn("C006", typeof(string)); dt.Columns.Add(col_6);
                DataColumn col_7 = new DataColumn("C007", typeof(string)); dt.Columns.Add(col_7);
                DataColumn col_14 = new DataColumn("C014", typeof(string)); dt.Columns.Add(col_14);
                DataColumn col_15 = new DataColumn("C015", typeof(string)); dt.Columns.Add(col_15);
                DataTable ds_dt = dt.Clone();
                DataTable _dt = _ds.Tables[0]; List<string> TListAlarmID = new List<string>();
                foreach (DataRow dr in _dt.Rows)
                {
                    if (dr["C024"].ToString() == "1") { continue; }
                    string AlarmID = dr["C023"].ToString();
                    string IsFind = TListAlarmID.FirstOrDefault(p => p == AlarmID);
                    if (IsFind == null)
                    {
                        TListAlarmID.Add(AlarmID);
                        DataRow newRow = dt.NewRow();
                        newRow["C023"] = AlarmID;
                        newRow["C019"] = Convert.ToDateTime(dr["C019"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        newRow["C006"] = dr["C006"].ToString();
                        newRow["C007"] = dr["C007"].ToString();
                        newRow["C014"] = dr["C014"].ToString();
                        newRow["C015"] = dr["C015"].ToString();
                        newRow["P2101"] = dr["C006"].ToString() + dr["C007"].ToString() + Convert.ToInt32(dr["C014"].ToString()).ToString("000") + Convert.ToInt32(dr["C015"].ToString()).ToString("000");
                        DataRow[] DealRows = _dt.Select("C023='" + AlarmID + "' AND C024=1");
                        if (DealRows.Count() != 0)
                        {
                            newRow["C024"] = "1";
                            newRow["P2102"] = Convert.ToDateTime(DealRows[0]["C019"].ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            newRow["C024"] = "0";
                        }
                        string Mails = string.Empty; string Client = string.Empty; string APP = string.Empty;
                        for (int Count = 0; Count < DealRows.Count(); Count++)
                        {
                            Mails += GetUserNameByUserID(DealRows[Count]["P302"].ToString()) + ",";
                            Client += GetUserNameByUserID(DealRows[Count]["P502"].ToString()) + ",";
                            APP += GetUserNameByUserID(DealRows[Count]["P902"].ToString()) + ",";
                        }
                        if (Mails != null) { Mails = Mails.TrimEnd(','); }
                        if (Client != null) { Client = Client.TrimEnd(','); }
                        if (APP != null) { APP = APP.TrimEnd(','); }
                        newRow["P2103"] = Mails;
                        newRow["P2104"] = Client;
                        newRow["P2105"] = APP;
                        dt.Rows.Add(newRow);
                    }
                }
                DataRow[] finalRows;
                if (Deal == "1")
                { finalRows = dt.Select("C024='1'"); }
                else if (Deal == "0")
                { finalRows = dt.Select("C024='0'"); }
                else
                { finalRows = dt.Select(); }

                foreach (DataRow frow in finalRows)
                {
                    string contentID = frow["P2101"].ToString();
                    frow["P2101"] = GetLanguageInfo(string.Format("6101Alarm{0}", contentID), contentID);
                    ds_dt.ImportRow(frow);
                }
                ds.Tables.Add(ds_dt);
            }
            return ds;
        }

        public DataSet GetR22DataSet(string StrQ, int t)
        {
            DataTable _dt1 = ds_userid.Tables[0]; COUNT = 0; DataSet _ds = new DataSet();
            List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;

            long AfterRowNum = 0; bool IsNull = true;
            do
            {
                IsNull = false;
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {0} * " +
                        "FROM T_51_009_{1}  " +
                    "WHERE C001>{2} AND C011 >={3} AND C011 <={4} AND C004='{1}' {5} ORDER BY C001,C011"
                        , GetDataNum, Session.RentInfo.Token, AfterRowNum, ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"), StrQ);
                }
                else
                {
                    sql = string.Format("SELECT * FROM (SELECT  * "
                        + "FROM T_51_009_{1} WHERE C004='{1}' AND C011 >= " + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND C011 <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + StrQ + " ORDER BY C001,C011 DESC) WHERE C001 >" + AfterRowNum + " AND ROWNUM<={0} ORDER BY C001,C011 DESC ", GetDataNum, Session.RentInfo.Token);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100);
                    if (_ds == null)
                    {
                        continue;
                    }
                    if (_ds.Tables != null && _ds.Tables.Count != 0)
                        if (_ds.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++; first_time = false;
                        }
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp == null)
                    {
                        continue;
                    }
                    if (DS_temp.Tables == null)
                    {
                        continue;
                    }
                    else if (DS_temp.Tables[0].Rows.Count != 0)
                    {
                        IsNull = true; CiShuSum++;
                    }
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (IsNull)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToInt64(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            } while (IsNull);

            WriteLog(string.Format("(22):{0}", sql));
            DataTable dt = new DataTable(); DataSet ds = new DataSet();
            if (CiShuSum != 0)
            {
                DataColumn col_keywordid = new DataColumn("C001", typeof(string)); dt.Columns.Add(col_keywordid);
                DataColumn col_keyword = new DataColumn("C006", typeof(string)); dt.Columns.Add(col_keyword);
                DataColumn col_count = new DataColumn("P2201", typeof(string)); dt.Columns.Add(col_count);
                DataColumn col_recordnum = new DataColumn("P2202", typeof(string)); dt.Columns.Add(col_recordnum);
                DataColumn col_coutnum = new DataColumn("P2203", typeof(string)); dt.Columns.Add(col_coutnum);

                //DataTable ds_dt = dt.Clone();
                DataTable _dt = _ds.Tables[0]; List<string> TListKeyWord = new List<string>();
                foreach (DataRow dr in _dt.Rows)
                {
                    string KeyWordID = dr["C010"].ToString();
                    string IsFind = TListKeyWord.FirstOrDefault(p => p == KeyWordID);
                    if (IsFind == null)
                    {
                        TListKeyWord.Add(KeyWordID);
                        DataRow newRow = dt.NewRow();
                        newRow["C001"] = KeyWordID;
                        newRow["C006"] = dr["C008"].ToString();
                        DataRow[] CountRows = _dt.Select("C010='" + KeyWordID+"'");
                        double countR = CountRows.Count();
                        newRow["P2201"] = countR.ToString();
                        List<string> ListRecord = new List<string>();
                        foreach (DataRow drow in CountRows)
                        {
                            string RecID = ListRecord.FirstOrDefault(p => p == drow["C003"].ToString());
                            if (RecID == null)
                            {
                                ListRecord.Add(drow["C003"].ToString());
                            }
                        }
                        newRow["P2202"] = ListRecord.Count;
                        if (ListRecord.Count != 0)
                            newRow["P2203"] = decimal.Round(Convert.ToDecimal(countR / ListRecord.Count), NumAfterPoint);
                        else
                            newRow["P2203"] = 0;
                        dt.Rows.Add(newRow);
                    }
                }
                //foreach (DataRow drow in dt.Rows)
                //{
                //    ds_dt.ImportRow(drow);
                //}
                ds.Tables.Add(dt);
                //ds.Tables.Add(ds_dt);
            }
            return ds;
        }

        public DataSet GetR23DataSet(string StrQ, int t, string ColName)
        {
            DataTable _dt1 = ds_userid.Tables[0]; COUNT = 0; DataSet _ds = new DataSet();
            List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;

            long AfterRowNum = 0; bool IsNull = true;
            do
            {
                IsNull = false;
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {0} C001,C002,C007,C008,C009,C010,C014 C103,C015 C104,C016 C105,C017 " +
                        "FROM T_51_009_{1}  " +
                    "WHERE C001>{2} AND C004='{1}' AND C011 >={3} AND C011 <={4} {5} ORDER BY C001,C011"
                        , GetDataNum, Session.RentInfo.Token, AfterRowNum, ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"), StrQ);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C007,C008,C009,C010,C014 C103,C015 C104,C016 C105,C017 FROM (SELECT  * "
                        + "FROM T_51_009 _{1} WHERE C004='{1}' " + StrQ
                        + "AND C011 >= " + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND C011 <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " ORDER BY C001,C011 DESC) WHERE C001 >" + AfterRowNum + " AND ROWNUM<={0} ORDER BY C001,C011 DESC ", GetDataNum, Session.RentInfo.Token);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100);
                    if (_ds == null)
                    {
                        continue;
                    }
                    if (_ds.Tables != null && _ds.Tables.Count != 0)
                        if (_ds.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++; first_time = false;
                        }
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp == null)
                    {
                        continue;
                    }
                    if (DS_temp.Tables == null)
                    {
                        continue;
                    }
                    else if (DS_temp.Tables[0].Rows.Count != 0)
                    {
                        IsNull = true; CiShuSum++;
                    }
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (IsNull)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToInt64(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            } while (IsNull);

            WriteLog(string.Format("(23-{1}):{0}", sql, ColName));
            DataTable dt = new DataTable(); DataSet ds = new DataSet();
            if (CiShuSum != 0)
            {
                DataColumn col_agentname = new DataColumn("P2301", typeof(string)); dt.Columns.Add(col_agentname);
                DataColumn col_time = new DataColumn("C011", typeof(string)); dt.Columns.Add(col_time);
                DataColumn col_agentid = new DataColumn("C103", typeof(string)); dt.Columns.Add(col_agentid);
                DataColumn col_extid = new DataColumn("C104", typeof(string)); dt.Columns.Add(col_extid);
                DataColumn col_realextid = new DataColumn("C105", typeof(string)); dt.Columns.Add(col_realextid);

                //DataTable ds_dt = dt.Clone();
                DataTable _dt = _ds.Tables[0]; List<string> TListKeyWord = new List<string>(); List<string> ListGroup = new List<string>();
                List<string> ListColName = new List<string>();
                foreach (DataRow dr in _dt.Rows)
                {
                    string KeyWordID = dr["C010"].ToString();
                    string IsFind = TListKeyWord.FirstOrDefault(p => p == KeyWordID);
                    if (IsFind == null)
                    {
                        TListKeyWord.Add(KeyWordID);
                    }
                    string Groupname = dr[ColName].ToString();
                    string findname = ListGroup.FirstOrDefault(p => p == Groupname);
                    if (findname == null && Groupname != string.Empty)
                    {
                        ListGroup.Add(Groupname);
                    }
                }
                //加列动态
                List<string> ListColumnName = new List<string>();
                XmlDocument LXmlDoc = new XmlDocument();
                var byteData = UMPS6101.Properties.Resources.Report23;
                MemoryStream ms = new MemoryStream(byteData);
                LXmlDoc.Load(ms);
                Path23 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}\\Report23.rdlc", AppName));
                for (int i = 0; i < TListKeyWord.Count; i++)
                {
                    string nameCol = (i + 23001).ToString();
                    ListColumnName.Clear();
                    //动态加列
                    ListColumnName.Add(string.Format("Textbox{0}", nameCol));
                    ListColumnName.Add(TListKeyWord[i]);
                    ListColumnName.Add("P" + nameCol);
                    ListColumnName.Add(string.Format("=Fields!{0}.Value", "P" + nameCol));
                    ListColName.Add("P" + nameCol);
                    DataColumn col_colnum = new DataColumn("P" + nameCol, typeof(string)); dt.Columns.Add(col_colnum);
                    try
                    {
                        WriteTablixColumnToXml(LXmlDoc);
                        WriteFieldToXml(LXmlDoc);
                        WriteTablixCellToXml(LXmlDoc, ListColumnName);
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex.Message);
                    }
                }
                //保存
                LXmlDoc.Save(Path23);
                for (int j = 0; j < ListGroup.Count; j++)
                {
                    DataRow newRow = dt.NewRow();
                    bool Isfind = false;
                    if (ColName == "C104")
                    {
                        for (int rowc = 0; rowc < ds_ext.Tables[0].Rows.Count; rowc++)
                        {
                            if (ds_ext.Tables[0].Rows[rowc].ToString() == ListGroup[j])
                            {
                                Isfind = true;
                                newRow["P2301"] = ds_ext.Tables[0].Rows[rowc]["C012"].ToString(); break;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < ds_agent.Tables[0].Rows.Count; k++)
                        {
                            string tempoc_table = ds_agent.Tables[0].Rows[k]["C017"].ToString();
                            if (ListGroup[j] == tempoc_table)
                            {
                                Isfind = true;
                                newRow["P2301"] = ds_agent.Tables[0].Rows[k]["C018"].ToString();
                                break;
                            }
                        }
                    }
                    if (!Isfind)
                    {
                        newRow["P2301"] = ListGroup[j];
                    }
                    DataRow[] ListData = _dt.Select(string.Format("{0}='{1}'", ColName, ListGroup[j]));
                    for (int q = 0; q < ListColName.Count(); q++)
                    {
                        int KeyWcount = 0;
                        for (int rum = 0; rum < ListData.Count(); rum++)
                        {
                            if (ListData[rum]["C010"].ToString() == TListKeyWord[q])
                            {
                                KeyWcount++;
                            }
                        }
                        newRow[ListColName[q]] = KeyWcount.ToString();
                    }
                    dt.Rows.Add(newRow);
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet GetR25DataSet(string StrQ, int t, string ColName)
        {
            DataTable _dt1 = ds_userid.Tables[0]; COUNT = 0; DataSet _ds = new DataSet();
            List<string> TableName = new List<string>(); string sql = string.Empty; DataSet DS_temp = new DataSet(); bool first_time = true;
            int monthDiff = (ADT.EndDateTime[t].Year - ADT.BeginDateTime[t].Year) * 12 + (ADT.EndDateTime[t].Month - ADT.BeginDateTime[t].Month); int CiShuSum = 0; int count_num = 0;

            long AfterRowNum = 0; bool IsNull = true;
            do
            {
                IsNull = false;
                if (Session.DBType == 2)
                {
                    sql = string.Format("SELECT TOP {0} C001,C002,C003,C005,C007,C008,C009,C010,C011,C014 C103,C015 C104,C016 C105 " +
                        "FROM T_51_009_{1}  " +
                    "WHERE C001>{2} AND C004='{1}' AND C011 >={3} AND C011 <={4} {5} ORDER BY C001,C011"
                        , GetDataNum, Session.RentInfo.Token, AfterRowNum, ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss"), ADT.EndDateTime[t].ToString("yyyyMMddHHmmss"), StrQ);
                }
                else
                {
                    sql = string.Format("SELECT C001,C002,C003,C005,C007,C008,C009,C010,C011,C014 C103,C015 C104,C016 C105 FROM (SELECT  * "
                        + "FROM T_51_009_{1} WHERE C004='{1}' " + StrQ
                        + "AND C011 >= " + ADT.BeginDateTime[t].ToString("yyyyMMddHHmmss") + " AND C011 <= "
                        + ADT.EndDateTime[t].ToString("yyyyMMddHHmmss") + " ORDER BY C001,C011 DESC) WHERE C001 >" + AfterRowNum + " AND ROWNUM<={0} ORDER BY C001,C011 DESC ", GetDataNum, Session.RentInfo.Token);
                }
                if (first_time)
                {
                    _ds = GetDataSetFromDB(sql, 100);
                    if (_ds == null)
                    {
                        continue;
                    }
                    if (_ds.Tables != null && _ds.Tables.Count != 0)
                        if (_ds.Tables[0].Rows.Count != 0)
                        {
                            IsNull = true; CiShuSum++; first_time = false;
                        }
                }
                else
                {
                    DS_temp = GetDataSetFromDB(sql, 100);
                    if (DS_temp == null)
                    {
                        continue;
                    }
                    if (DS_temp.Tables == null)
                    {
                        continue;
                    }
                    else if (DS_temp.Tables[0].Rows.Count != 0)
                    {
                        IsNull = true; CiShuSum++;
                    }
                    if (DS_temp != null)
                    {
                        if (DS_temp.Tables != null && DS_temp.Tables.Count != 0)
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                _ds.Tables[0].ImportRow(dr);
                    }
                }
                if (IsNull)
                {
                    COUNT = _ds.Tables[0].Rows.Count - 1;
                    AfterRowNum = Convert.ToInt64(_ds.Tables[0].Rows[COUNT]["C001"]);
                }
            } while (IsNull);

            WriteLog(string.Format("(25-{1}):{0}", sql, ColName));
            DataTable dt = new DataTable(); DataSet ds = new DataSet();
            if (CiShuSum != 0)
            {
                //dt= _ds.Tables[0];
                DataTable _dt = _ds.Tables[0];
                DataColumn col_ID = new DataColumn("P2501", typeof(string)); dt.Columns.Add(col_ID);
                DataColumn col_name = new DataColumn("P2502", typeof(string)); dt.Columns.Add(col_name);
                DataColumn col_time = new DataColumn("C011", typeof(string)); dt.Columns.Add(col_time);
                DataColumn col_agentid = new DataColumn("C103", typeof(string)); dt.Columns.Add(col_agentid);
                DataColumn col_extid = new DataColumn("C104", typeof(string)); dt.Columns.Add(col_extid);
                DataColumn col_realextid = new DataColumn("C105", typeof(string)); dt.Columns.Add(col_realextid);
                DataColumn col_Recordid = new DataColumn("C003", typeof(string)); dt.Columns.Add(col_Recordid);
                DataColumn col_kws = new DataColumn("C006", typeof(string)); dt.Columns.Add(col_kws);
                DataColumn col_Starttime = new DataColumn("C004", typeof(string)); dt.Columns.Add(col_Starttime);
                DataColumn col_kwsID = new DataColumn("C001", typeof(string)); dt.Columns.Add(col_kwsID);

                for (int j = 0; j < _dt.Rows.Count; j++)
                {
                    DataRow _dr = _dt.Rows[j];
                    DataRow newRow = dt.NewRow();
                    newRow["C104"] = _dr["C104"];
                    newRow["C103"] = _dr["C103"];
                    newRow["C105"] = _dr["C105"];
                    newRow["C003"] = _dr["C003"];
                    newRow["C006"] = _dr["C010"];
                    newRow["C011"] = _dr["C011"];
                    double TimeNum = 0;
                    if (double.TryParse(_dr["C005"].ToString(), out TimeNum))
                    {
                        newRow["C004"] = Converter.MilliSecond2Time(TimeNum);
                    }
                    else
                    {
                        newRow["C004"] = _dr["C005"].ToString();
                    }
                    newRow["C001"] = _dr["C001"];
                    bool Isfind = false;
                    if (ColName == "C104")
                    {
                        for (int rowc = 0; rowc < ds_ext.Tables[0].Rows.Count; rowc++)
                        {
                            if (ds_ext.Tables[0].Rows[rowc]["C001"].ToString() == newRow[ColName].ToString())
                            {
                                Isfind = true;
                                newRow["P2501"] = ds_ext.Tables[0].Rows[rowc]["C017"].ToString();
                                newRow["P2502"] = ds_ext.Tables[0].Rows[rowc]["C018"].ToString(); break;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < ds_agent.Tables[0].Rows.Count; k++)
                        {
                            string tempoc_table = ds_agent.Tables[0].Rows[k]["C017"].ToString();
                            if (newRow[ColName].ToString() == tempoc_table)
                            {
                                Isfind = true;
                                newRow["P2501"] = ds_agent.Tables[0].Rows[k]["C017"].ToString();
                                newRow["P2502"] = ds_agent.Tables[0].Rows[k]["C018"].ToString();
                                break;
                            }
                        }
                    }
                    if (!Isfind)
                    {
                        newRow["P2501"] = newRow[ColName];
                        newRow["P2502"] = newRow[ColName];
                    }
                    dt.Rows.Add(newRow);
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }
        #endregion

        #region Tools
        //21张用，获取用户的名称通过用户的19位编号
        private string GetUserNameByUserID(string UserID)
        {
            if (UserID == string.Empty) { return UserID; }
            DataRow[] Users = ds_userid.Tables[0].Select("C001=" + UserID);
            if (Users.Count() != 0)
            {
                return DecryptString(Users[0]["C003"].ToString());
            }
            else
            {
                return UserID;
            }
        }
        //第三张要用的吧
        private static string DecodeContent(string strContent)
        {
            string strReg = string.Format("{0}<{0}\\w+{0}>{0}", (char)30);
            Regex regex = new Regex(strReg);
            var match = regex.Match(strContent);
            int index = 0;
            string strReplace = string.Empty;
            while (match.Success)
            {
                string strKey = match.ToString().Substring(3, match.ToString().Length - 6);
                //var langInfo = mListLanguageInfos.FirstOrDefault(l => l.LangType == mLangType && l.Name == strKey);
                for (int i = 0; i < dslangdata.Tables[0].Rows.Count; i++)
                {
                    if (dslangdata.Tables[0].Rows[i]["C002"].ToString() == strKey)
                    {
                        strReplace = dslangdata.Tables[0].Rows[i]["C005"].ToString();
                    }
                }
                if (strReplace == string.Empty)
                {
                    strReplace = strKey;
                }
                //if (langInfo == null)
                //{
                //    strReplace = strKey;
                //}
                //else
                //{
                //    strReplace = langInfo.Display;
                //}           
                strContent = regex.Replace(strContent, strReplace, 1);
                index++;
                match = match.NextMatch();
            }
            return strContent;
        }

        //如果传入参数最后是1，表示是空表插入数据条。如果不为1，则表示不是第一次插入数据。
        private void TransData(List<string> ListStr, DataTable dt, string ColName, int num)
        {
            int count = ListStr.Count;
            for (int i = 0; i < count; i++)
            {
                if (num == 1)
                {
                    DataRow dr = dt.NewRow();
                    dr[ColName] = ListStr[i];
                    dt.Rows.Add(dr);
                }
                else
                {
                    dt.Rows[i][ColName] = ListStr[i];
                }
            }
        }

        public static string TransTimeForm(int Num)
        {
            string TimeStr; int hour = 0; int min = 0; int sec = 0;
            sec = Num % 60; min = Num / 60; hour = min / 60; min = min % 60;
            string H = hour.ToString(); string M = min.ToString(); string S = sec.ToString();
            if (H.Length == 1)
            {
                H = string.Format("0{0}", H);
            }
            if (M.Length == 1)
            {
                M = string.Format("0{0}", M);
            }
            if (S.Length == 1)
            {
                S = string.Format("0{0}", S);
            }
            TimeStr = H + ":" + M + ":" + S;
            return TimeStr;
        }  //numtostring
        //从数据库里通过查询获取一个DataSet
        public DataSet GetDataSetFromDB(string sql, int number)
        {
            // App.TimeTest += "开始连接数据库:" + DateTime.Now.ToString("hh:mm:ss:fff");
            DataSet data_set = new DataSet();
            try
            {
                WebRequest result = new WebRequest();
                result.Session = Session;
                result.Code = number;
                result.Data = sql;
                Service61011Client ServiceClient = new Service61011Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service61011"));
                WebHelper.SetServiceClient(ServiceClient);
                WebReturn WR = ServiceClient.DoOperation(result); ServiceClient.Close();
                if (WR.Result)
                    data_set = WR.DataSetData;
                else
                {
                    data_set = new DataSet();
                }
                //App.TimeTest += "获取数据完毕:" + DateTime.Now.ToString("hh:mm:ss:fff");           
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Data Get Fail.\n\t" + sql);
                WriteLog("GetDataSetFromDB"+ex.Message + ":" + ex.ToString());
            }
            return data_set;
        }

        public DataSet GetDataSetFromDB(string sql, int number, string TableName)
        {
            DataSet data_set = new DataSet();
            string TableTest = string.Empty;
            if (Session.DBType == 2)
                TableTest = string.Format("SELECT COUNT(1) FROM SYSOBJECTS WHERE NAME='{0}' AND TYPE='U'", TableName);
            else
                TableTest = string.Format("SELECT COUNT(1) FROM ALL_TABLES WHERE TABLE_NAME = '{0}' AND OWNER='{1}'", TableName, Session.DatabaseInfo.LoginName.ToUpper());
            data_set = GetDataSetFromDB(TableTest, 100);
            int IsExit = Convert.ToInt32(data_set.Tables[0].Rows[0][0].ToString());
            if (IsExit != 0)
            {
                data_set = GetDataSetFromDB(sql, number);
            }
            else
                data_set = null;
            return data_set;
        }

        public List<string> TableSec(string TableName, string st, string et)
        {
            List<string> TS = new List<string>();

            var tableInfo =
                    Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == TableName && t.PartType == TablePartType.DatetimeRange);
            if (tableInfo == null)
            {
                tableInfo =
                Session.ListPartitionTables.FirstOrDefault(
                    t => t.TableName == TableName && t.PartType == TablePartType.VoiceID);
                if (tableInfo == null)
                {
                    TS.Add(TableName + "_" + Session.RentInfo.Token);
                    //TS.Add(TableName);
                }
                else
                {
                    //按录音服务器查询,没有实现，暂时还是按普通方式来
                    TS.Add(TableName + "_" + Session.RentInfo.Token);
                    TS.Add(TableName);
                }
            }
            else
            {
                //按月查询
                TS = DateSec(st, et);
                for (int i = 0; i < TS.Count; i++)
                {
                    TS[i] = TableName + "_" + Session.RentInfo.Token + "_" + TS[i];
                }
            }
            return TS;
        }//分表

        public string AbsoluteDate(string date)
        {
            if (date.Length == 14)
                date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2) + " " + date.Substring(8, 2)
                    + ":" + date.Substring(10, 2) + ":" + date.Substring(12);
            DateTime DT = Convert.ToDateTime(date);
            switch (ADT.Sign)
            {
                case "W":
                    DateTime BY = Convert.ToDateTime(DT.Year.ToString() + "/1/1");
                    TimeSpan bts = DT - BY; int days = bts.Days; int quotient = 0; int Remainder = 0;
                    int no = (int)DT.DayOfWeek; quotient = days / 7; Remainder = days % 7;
                    if (no > Remainder)
                        quotient += 1;
                    if (no < Remainder)
                        quotient += 2;
                    date = DT.Year.ToString() + GetLanguageInfo("61011005", "年") + GetLanguageInfo("61011000", "第") + quotient + GetLanguageInfo("61011002", "周");
                    break;
                case "M":
                    date = DT.Year.ToString() + GetLanguageInfo("61011005", "年") + DT.Month + GetLanguageInfo("61011003", "月");
                    break;
                case "Y":
                    date = DT.Year.ToString() + GetLanguageInfo("61011005", "年");
                    break;
                case "D":
                    date = DT.ToString("yyyy/MM/dd").Substring(0, 10);
                    break;
                default:
                    date = string.Empty;
                    break;
            }
            return date;
        }

        public static List<string> DateSec(string st, string et)
        {
            string Sy = st.Substring(0, 4); string Sm = st.Substring(5, 2);
            string Ey = et.Substring(0, 4); string Em = et.Substring(5, 2); List<string> TableName = new List<string>();
            if (Sy != Ey)
            {
                int Dy = int.Parse(Ey) - int.Parse(Sy);
                for (int i = int.Parse(Sm); i < 13; i++)
                {
                    string mon = i.ToString();
                    if (mon.Length == 1)
                        mon = "0" + mon;
                    TableName.Add(Sy.Substring(2, 2) + mon);
                }
                for (int j = 1; j < Dy; j++)
                {
                    for (int k = 1; k < 13; k++)
                    {
                        string mon = k.ToString();
                        if (mon.Length == 1)
                            mon = "0" + mon;
                        TableName.Add((int.Parse(Sy.Substring(2, 2)) + j).ToString() + mon);
                    }
                }
                for (int l = 1; l <= int.Parse(Em); l++)
                {
                    string mon = l.ToString();
                    if (mon.Length == 1)
                        mon = "0" + mon;
                    TableName.Add(Ey.Substring(2, 2) + mon);
                }
            }
            else
            {
                if (Sm == Em)
                    TableName.Add(Sy.Substring(2, 2) + Sm);
                else
                {
                    for (int i = int.Parse(Sm); i <= int.Parse(Em); i++)
                    {
                        string mon = i.ToString();
                        if (mon.Length == 1)
                            mon = "0" + mon;
                        TableName.Add(Sy.Substring(2, 2) + mon);
                    }
                }
            }
            return TableName;
        }//按月分表

        public void GetUserPermissions()
        {
            try
            {
                //获取可管理的坐席
                string SqlAgent = string.Format("SELECT C001,C017,C018 FROM T_11_101_{0} WHERE C002=1 AND C001 LIKE '103%' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1})", Session.RentInfo.Token, Session.UserID.ToString());
                DataSet AgentDS = GetDataSetFromDB(SqlAgent, 102);
                //获取可管理的质检员
                string SqlInpecter = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C001 IN (SELECT C005 FROM T_31_008_{0}) AND C001 IN (SELECT C004 FROM T_11_201_00000 WHERE C003 = {1})", Session.RentInfo.Token, Session.UserID.ToString());
                DataSet InspecterDS = GetDataSetFromDB(SqlInpecter, 100);
                //获取可管理的分机
                //string SqlExten = string.Format("");
                //DataSet ExtDS = GetDataSetFromDB(SqlExten, 100);

                foreach (DataRow dr in AgentDS.Tables[0].Rows)
                {
                    AgentList.Add(dr["C001"].ToString()); UserList.Add(dr["C001"].ToString());
                    if (dr["C017"].ToString() != "")
                        AgentNameList.Add(DecryptString(dr["C017"].ToString()));
                }
                foreach (DataRow dr in InspecterDS.Tables[0].Rows)
                {
                    InspecterList.Add(dr["C001"].ToString());
                }
                GetUserList(Session.UserID.ToString());
            }
            catch (Exception ex)
            {
                ShowInfoMessage("Get Users Info Fail.");
                WriteLog("GetUserPermissions_"+ex.Message + ":" + ex.ToString());
            }
        }

        //通过时间筛选21-001表中符合的录音流水号list写入临时表返回编号
        public string RecorderIDList(string st, string et, string tableName, string Colname)
        {
            string StarT = st; string EndT = et;
            if (st.Length <= 14)
            {
                StarT = st.Substring(0, 4) + "-" + st.Substring(4, 2);
                EndT = et.Substring(0, 4) + "-" + et.Substring(4, 2);
            }
            List<string> TableName = TableSec("T_21_001", StarT, EndT);
            List<string> RecorderID = new List<string>(); string sql; DataSet DS = new DataSet();
            for (int i = 0; i < TableName.Count; i++)
            {
                sql = string.Format("SELECT COUNT(C001) FROM {3} WHERE {4} IN (SELECT C002 FROM {0} WHERE C006>'{1}' AND C006<'{2}')",
                    TableName[i], st, et, tableName, Colname);
                DS = GetDataSetFromDB(sql, 100, TableName[i]);
                int cishu = Convert.ToInt32(DS.Tables[0].Rows[0][0]);
                int yushu = cishu % GetDataNum; cishu = cishu / GetDataNum;
                if (yushu != 0)
                {
                    cishu = cishu + 1;
                }
                string Temp = "0";
                for (int j = 0; j < cishu; j++)
                {
                    switch (Session.DBType)
                    {
                        case 2:
                            sql = string.Format("SELECT TOP {5} C001,{4} FROM {3} WHERE {4} IN (SELECT C002 FROM {0} WHERE C006>'{1}' AND C006<'{2}' AND C001>{6})ORDER BY C001",
                                TableName[i], st, et, tableName, Colname, GetDataNum, Temp);
                            break;
                        case 3:
                            sql = string.Format("SELECT C001,{4} FROM(SELECT {4} FROM {3} WHERE {4} IN (SELECT C002 FROM {0} WHERE C006>'{1}' AND C006<'{2}' AND C001>{6})ORDER BY C001)WHERE ROWNUM<={5}",
                                TableName[i], st, et, tableName, Colname, GetDataNum, Temp);
                            break;
                    }
                    DS = GetDataSetFromDB(sql, 100, TableName[i]);
                    if (DS != null)
                        if (DS.Tables != null)
                            if (DS.Tables[0].Rows.Count != 0)
                                foreach (DataRow dr in DS.Tables[0].Rows)
                                {
                                    RecorderID.Add(dr[0].ToString());
                                    Temp = dr["C001"].ToString();
                                }
                }
            }
            //装到临时表  返回编码
            string Return = PutTempData(RecorderID);
            return Return;
        }

        //根据录音流水号 获取某些字段
        public string GetDataFromRecorderTable(string st, string et, string recorder, string Colname)
        {
            string RecorderID = null;
            try
            {
                string StarT = st; string EndT = et;
                if (st.Length <= 14)
                {
                    StarT = st.Substring(0, 4) + "-" + st.Substring(4, 2);
                    EndT = et.Substring(0, 4) + "-" + et.Substring(4, 2);
                }
                DateTime dt = Convert.ToDateTime(StarT);
                for (int j = 1; j <= 3; j++)
                {
                    dt = dt.AddMonths(-j); StarT = dt.ToString("yyyy-MM-dd HH:mm:ss");
                }
                dt = Convert.ToDateTime(EndT);
                for (int k = 1; k <= 3; k++)
                {
                    dt = dt.AddMonths(k); EndT = dt.ToString("yyyy-MM-dd HH:mm:ss");
                }
                List<string> TableName = TableSec("T_21_001", StarT, EndT);

                string sql; DataSet DS = new DataSet();
                for (int i = 0; i < TableName.Count; i++)
                {
                    sql = string.Format("SELECT {4} FROM {0} WHERE C002='{3}'ORDER BY C001", TableName[i], st, et, recorder, Colname);
                    DS = GetDataSetFromDB(sql, 100, TableName[i]);
                    if (DS != null)
                        if (DS.Tables != null && DS.Tables.Count != 0)
                            if (DS.Tables[0].Rows != null && DS.Tables[0].Rows.Count != 0)
                                RecorderID = DS.Tables[0].Rows[0][0].ToString();
                }
                return RecorderID;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
                return RecorderID;
            }
        }

        public string PutTempData(List<string> Str)
        {
            try
            {
                Service11012Client ServiceClient = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(ServiceClient);
                WebRequest WReq = new WebRequest();
                WReq.Session = Session;
                WReq.Code = (int)RequestCode.WSInsertTempData;
                WReq.ListData.Add(string.Empty);
                WReq.ListData.Add(Str.Count.ToString());
                foreach (string str in Str)
                    WReq.ListData.Add(str);
                WebReturn WRet = ServiceClient.DoOperation(WReq);
                return WRet.Data;
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        #region
        public void GetUserList(string UserId)
        {
            //获取可以管理的用户
            string SqlUser = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1})",
                Session.RentInfo.Token, UserId);
            DataSet UserDS = GetDataSetFromDB(SqlUser, 100);//如若需要用户的账号C002和全名C003，可以改成103
            if (UserDS.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in UserDS.Tables[0].Rows)
                {
                    UserList.Add(dr["C001"].ToString());
                }
            }
        }
        #endregion

        public void GetParameters()
        {
            try
            {
                //获取全局参数
                //string sql = string.Format("SELECT C003,C006 FROM T_11_001_{0} WHERE C001=1000000000000000000 AND C003 IN ({2})", Session.RentInfo.Token, Session.RentID, "12010401,12010101,12010102");
                //string sql = string.Format("SELECT C003,C006 FROM T_11_001_{0} WHERE C001={1} AND C003 IN ({2})", Session.RentInfo.Token, Session.RentID, "12010401,12010101,12010102");

                string sql = string.Format("SELECT C003,C006 FROM T_11_001_{0} WHERE C003 IN ({1})", Session.RentInfo.Token, "12010401,12010101,12010102");
                DataSet ds = GetDataSetFromDB(sql, 0);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string id = dr["C003"].ToString(); string TempleData = dr["C006"].ToString();
                    TempleData = DecryptString(TempleData);
                    TempleData = TempleData.Substring(8, TempleData.Length - 8);
                    if (id == "12010101")
                    {
                        WeekStart = Convert.ToInt32(TempleData);
                    }
                    if (id == "12010102")
                    {
                        MonthStart = Convert.ToInt32(TempleData);
                    }
                    if (id == "12010401")
                    {
                        PacketMode = TempleData;//没有处理完
                    }
                }
            }
            catch (Exception ex)
            { WriteLog("GetParameters_" + ex.Message + ":" + ex.ToString()); }
        }
        public static void TreeItemNoSelect(ObjectItem RootItem)
        {
            if (RootItem != null)
            {
                RootItem.IsChecked = false;
            }
            for (int i = 0; i < RootItem.Children.Count; i++)
            {
                ObjectItem ChildObjItem = RootItem.Children[i] as ObjectItem;
                TreeItemNoSelect(ChildObjItem);
            }
        }
        #endregion

        #region 密码

        public void DecryptAgentPart()
        {
            try
            {
                string sqllangdata = "SELECT C002,C005 FROM T_00_005 WHERE C001='" + Session.LangTypeInfo.LangID + "'";
                dslangdata = GetDataSetFromDB(sqllangdata, 100);

                dslangFO = dslangdata.Clone();
                DataRow[] DR = dslangdata.Tables[0].Select("C002 like 'FO%'");
                for (int i = 0; i < DR.Count(); i++)
                {
                    dslangFO.Tables[0].ImportRow(DR[i]);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void GetDataSet()
        {
            try
            {
                ds_31019 = GetDataSetFromDB(string.Format("SELECT C008,C009,C002 FROM T_31_019_{0}", Session.RentInfo.Token), 100);

                DataSet ds_agentpart = GetDataSetFromDB(string.Format("SELECT C001,C002 FROM T_11_006_{0}", Session.RentInfo.Token), 200);
                foreach (DataRow dr in ds_agentpart.Tables[0].Rows)
                {
                    dr["C002"] = DecryptString(dr["C002"].ToString());
                }
                ds_agent = GetDataSetFromDB(string.Format("SELECT C001,C017,C018,C011 FROM T_11_101_{0} WHERE C001>1030000000000000000 AND C001<1040000000000000000 AND C002=1 ", Session.RentInfo.Token), 102);
                DataColumn col = new DataColumn("C012", typeof(string));
                DataTable dt = ds_agent.Tables[0]; dt.Columns.Add(col);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["C017"] = DecryptString(dr["C017"].ToString());
                    dr["C018"] = DecryptString(dr["C018"].ToString());
                    foreach (DataRow dr_part in ds_agentpart.Tables[0].Rows)
                    {
                        if (dr["C011"].ToString() == dr_part["C001"].ToString())
                        {
                            dr["C012"] = dr_part["C002"].ToString();
                            break;
                        }
                    }
                }
                //ext
                ds_ext = GetDataSetFromDB(string.Format("SELECT C001,C017,C018,C011 FROM T_11_101_{0} WHERE C001>1040000000000000000 AND C001<1050000000000000000 AND C002=1 ", Session.RentInfo.Token), 102);
                DataColumn col_ext = new DataColumn("C012", typeof(string));
                DataTable dt_ext = ds_ext.Tables[0]; dt_ext.Columns.Add(col_ext);
                foreach (DataRow dr in dt_ext.Rows)
                {
                    dr["C017"] = DecryptString(dr["C017"].ToString());
                    dr["C018"] = DecryptString(dr["C018"].ToString());
                    foreach (DataRow dr_part in ds_agentpart.Tables[0].Rows)
                    {
                        if (dr["C011"].ToString() == dr_part["C001"].ToString())
                        {
                            dr["C012"] = dr_part["C002"].ToString();
                            break;
                        }
                    }
                }
                //realext
                ds_realext = GetDataSetFromDB(string.Format("SELECT C001,C017,C018,C011 FROM T_11_101_{0} WHERE C001>1050000000000000000 AND C001<1060000000000000000 AND C002=1 ", Session.RentInfo.Token), 102);
                DataColumn col_realext = new DataColumn("C012", typeof(string));
                DataTable dt_realext = ds_realext.Tables[0]; dt_realext.Columns.Add(col_realext);
                foreach (DataRow dr in dt_realext.Rows)
                {
                    dr["C017"] = DecryptString(dr["C017"].ToString());
                    dr["C018"] = DecryptString(dr["C018"].ToString());
                    foreach (DataRow dr_part in ds_agentpart.Tables[0].Rows)
                    {
                        if (dr["C011"].ToString() == dr_part["C001"].ToString())
                        {
                            dr["C012"] = dr_part["C002"].ToString();
                            break;
                        }
                    }
                }
                string sqluserid = "SELECT C001,C002,C003 FROM T_11_005_" + Session.RentInfo.Token;
                ds_userid = GetDataSetFromDB(sqluserid, 103);

                string sql1 = string.Format("SELECT * FROM T_31_011_{0} ORDER BY C002", Session.RentInfo.Token);
                ds_comm = GetDataSetFromDB(sql1, 100);

                string sql2 = string.Format("SELECT * FROM T_31_046_{0} WHERE C006='1' ", Session.RentInfo.Token);
                ds_comm46 = GetDataSetFromDB(sql1, 100);

                ds_ap = GetDataSetFromDB("SELECT C001,C002 FROM T_11_006_" + Session.RentInfo.Token, 200);
                ds_gn = GetDataSetFromDB("SELECT C001,C002 FROM T_31_001_" + Session.RentInfo.Token, 100);
                //获取所有关键词11-009
                //string sqlKeyWord = string.Format("SELECT * FROM T_11_009_{0} WHERE C000=4 AND C004='1' AND C010 <> 1", Session.RentInfo.Token);
                string sqlKeyWord = string.Format("SELECT A.C001 A001,A.C002 A002,A.C003 A003,A.C004 A004,C.C001,C.C002 FROM T_51_006_{0} A LEFT JOIN T_51_008_{0} B ON A.C001=B.C002 LEFT JOIN T_51_007_{0} C ON B.C001=C.C001 WHERE B.C003='1' AND A.C005='1' AND A.C010='0' AND C.C008='0'", Session.RentInfo.Token);
                ds_keywords = GetDataSetFromDB(sqlKeyWord, 100);
                //获取坐席的Tenure信息（等级）22
                string SqlAgentTenure = string.Format("SELECT C001,C012,C017,C018 FROM T_11_101_{0} WHERE C002=3 AND C001 LIKE '103%' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1})", Session.RentInfo.Token, Session.UserID.ToString());
                ds_agenttenure = GetDataSetFromDB(SqlAgentTenure, 100);
                foreach (DataRow dr in ds_agenttenure.Tables[0].Rows)
                {
                    DataRow[] TempDataR = ds_agent.Tables[0].Select(string.Format("C001='{0}'", dr["C001"].ToString()));
                    if (TempDataR.Count() != 0)
                    {
                        dr["C017"] = DecryptString(TempDataR[0]["C017"].ToString());
                        dr["C018"] = DecryptString(TempDataR[0]["C018"].ToString());
                    }
                }
            }
            catch (Exception ex)
            { }
        }//获取相关要用的一些信息

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

        #endregion

        #region writeXML

        /// <summary>
        /// 获取当前UMP安装的目录
        /// </summary>
        /// <returns></returns>
        public static string GetIISBaseDirectory()
        {
            string LStrBaseDirectory = string.Empty;

            try
            {
                LStrBaseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string[] LStrArrayDirectory = LStrBaseDirectory.Split(@"\".ToCharArray());
                LStrBaseDirectory = string.Empty;
                foreach (string LStrSingleDirectory in LStrArrayDirectory)
                {
                    LStrBaseDirectory += LStrSingleDirectory + @"\";
                    if (System.IO.Directory.Exists(LStrBaseDirectory + "Report"))
                    { LStrBaseDirectory += @"Report\Report16.rdlc"; break; }
                }
            }
            catch { LStrBaseDirectory = string.Empty; }

            return LStrBaseDirectory;
        }

        private static void WriteTablixColumnToXml(XmlDocument XmlDoc)
        {
            //try
            {
                //找到节点TablixColumns

                var xmlManager = new XmlNamespaceManager(XmlDoc.NameTable);
                xmlManager.AddNamespace("ab", Const6101.XmlNamespace);
                var LXMLNodeSection = XmlDoc.SelectSingleNode(Const6101.WriteTablixColumn, xmlManager);

                //添加需要的内容
                //<TablixColumn>
                //  <Width>2.5cm<>
                //</TablixColumn>
                XmlElement xmlTablixColumn1 = XmlDoc.CreateElement("TablixColumn", Const6101.XmlNamespace);
                XmlElement xmlTCWidth = XmlDoc.CreateElement("Width", Const6101.XmlNamespace);
                xmlTCWidth.InnerText = "2.5cm";
                LXMLNodeSection.AppendChild(xmlTablixColumn1);
                xmlTablixColumn1.AppendChild(xmlTCWidth);
            }
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //最后统一另存为修改
        }

        private static void WriteTablixCellToXml(XmlDocument XmlDoc, List<string> ListName)
        {
            //找到节点TablixCells
            var xmlManager = new XmlNamespaceManager(XmlDoc.NameTable);
            xmlManager.AddNamespace("ab", Const6101.XmlNamespace);
            //xmlManager.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            XmlNode LXMLNodeSection = XmlDoc.SelectSingleNode(Const6101.WriteTablixCell, xmlManager);
            XmlNodeList LXmlNodes = LXMLNodeSection.ChildNodes;
            for (int i = 0; i < LXmlNodes.Count; i++)
            {
                string Name = ""; string Value = "";
                if (i == 0)
                {
                    Name = ListName[0];
                    Value = ListName[1];
                }
                else
                {
                    Name = ListName[2];
                    Value = ListName[3];
                }
                //添加需要的内容：相当多
                XmlNode xmlNode = LXmlNodes[i].SelectSingleNode("ab:TablixCells", xmlManager);
                XmlElement xmlTablixCell = XmlDoc.CreateElement("TablixCell", Const6101.XmlNamespace);
                xmlNode.AppendChild(xmlTablixCell);

                XmlElement xmlCellContents = XmlDoc.CreateElement("CellContents", Const6101.XmlNamespace);
                xmlTablixCell.AppendChild(xmlCellContents);

                XmlElement xmlTextbox = XmlDoc.CreateElement("Textbox", Const6101.XmlNamespace);
                xmlTextbox.SetAttribute("Name", Name);//要改
                xmlCellContents.AppendChild(xmlTextbox);


                XmlElement xmlCanGrow = XmlDoc.CreateElement("CanGrow", Const6101.XmlNamespace);
                xmlCanGrow.InnerText = "true";
                xmlTextbox.AppendChild(xmlCanGrow);
                XmlElement xmlKeepTogether = XmlDoc.CreateElement("KeepTogether", Const6101.XmlNamespace);
                xmlKeepTogether.InnerText = "true";
                xmlTextbox.AppendChild(xmlKeepTogether);
                XmlElement xmlParagraphs = XmlDoc.CreateElement("Paragraphs", Const6101.XmlNamespace);
                xmlTextbox.AppendChild(xmlParagraphs);

                XmlElement xmlParagraph = XmlDoc.CreateElement("Paragraph", Const6101.XmlNamespace);
                xmlParagraphs.AppendChild(xmlParagraph);

                XmlElement xmlTextRuns = XmlDoc.CreateElement("TextRuns", Const6101.XmlNamespace);
                xmlParagraph.AppendChild(xmlTextRuns);

                XmlElement xmlTextRun = XmlDoc.CreateElement("TextRun", Const6101.XmlNamespace);
                xmlTextRuns.AppendChild(xmlTextRun);

                XmlElement xmlValue = XmlDoc.CreateElement("Value", Const6101.XmlNamespace);
                xmlValue.InnerText = string.Format(Value);//要改值
                xmlTextRun.AppendChild(xmlValue);

                if (i == 0)
                {
                    //-根据不同行，写入内容不一样------------------
                    XmlElement xmlStyle = XmlDoc.CreateElement("Style", Const6101.XmlNamespace);
                    xmlTextRun.AppendChild(xmlStyle);

                    XmlElement xmlFontSize = XmlDoc.CreateElement("FontSize", Const6101.XmlNamespace);
                    xmlFontSize.InnerText = "12pt";
                    xmlStyle.AppendChild(xmlFontSize);
                    XmlElement xmlFontWeight = XmlDoc.CreateElement("FontWeight", Const6101.XmlNamespace);
                    xmlFontWeight.InnerText = "Bold";
                    xmlStyle.AppendChild(xmlFontWeight);
                    //----------------------------------------------
                }
                XmlElement xmlStyle1 = XmlDoc.CreateElement("Style", Const6101.XmlNamespace);
                xmlParagraph.AppendChild(xmlStyle1);

                XmlElement xmlTextAlign = XmlDoc.CreateElement("TextAlign", Const6101.XmlNamespace);
                xmlTextAlign.InnerText = "Center";
                xmlStyle1.AppendChild(xmlTextAlign);

                XmlElement xmlrd = XmlDoc.CreateElement("rd:DefaultName", Const6101.XmlNamespaceRD);
                xmlrd.InnerText = Name;//要改

                XmlElement xmlStyle2 = XmlDoc.CreateElement("Style", Const6101.XmlNamespace);
                xmlTextbox.AppendChild(xmlStyle2);

                XmlElement xmlBorder = XmlDoc.CreateElement("Border", Const6101.XmlNamespace);
                xmlStyle2.AppendChild(xmlBorder);

                XmlElement xmlStyle3 = XmlDoc.CreateElement("Style", Const6101.XmlNamespace);
                xmlStyle3.InnerText = "Solid";
                xmlBorder.AppendChild(xmlStyle3);

                //--------------------------------------
                string EName = string.Empty;
                for (int j = 0; j < 4; j++)
                {
                    switch (j)
                    {
                        case 0:
                            EName = "TopBorder";
                            break;
                        case 1:
                            EName = "BottomBorder";
                            break;
                        case 2:
                            EName = "LeftBorder";
                            break;
                        case 3:
                            EName = "RightBorder";
                            break;
                    }
                    XmlElement xmlTBLRBorder = XmlDoc.CreateElement(EName, Const6101.XmlNamespace);
                    WriteTBLRBorder(XmlDoc, xmlTBLRBorder);
                    xmlStyle2.AppendChild(xmlTBLRBorder);
                }
                //--------------------------------------

                //XmlElement xmlVerticalAlign = XmlDoc.CreateElement("VerticalAlign", Const6101.XmlNamespace);
                //xmlVerticalAlign.InnerText = "Middle";
                //xmlStyle2.AppendChild(xmlVerticalAlign);

                XmlElement xmlPaddingLeft = XmlDoc.CreateElement("PaddingLeft", Const6101.XmlNamespace);
                xmlPaddingLeft.InnerText = "2pt";
                xmlStyle2.AppendChild(xmlPaddingLeft);

                XmlElement xmlPaddingRight = XmlDoc.CreateElement("PaddingRight", Const6101.XmlNamespace);
                xmlPaddingRight.InnerText = "2pt";
                xmlStyle2.AppendChild(xmlPaddingRight);

                XmlElement xmlPaddingTop = XmlDoc.CreateElement("PaddingTop", Const6101.XmlNamespace);
                xmlPaddingTop.InnerText = "2pt";
                xmlStyle2.AppendChild(xmlPaddingTop);

                XmlElement xmlPaddingBottom = XmlDoc.CreateElement("PaddingBottom", Const6101.XmlNamespace);
                xmlPaddingBottom.InnerText = "2pt";
                xmlStyle2.AppendChild(xmlPaddingBottom);
            }
            //最后统一另存为修改
        }

        private static void WriteFieldToXml(XmlDocument XmlDoc)
        {
            //找到节点Fields
            var xmlManager = new XmlNamespaceManager(XmlDoc.NameTable);
            xmlManager.AddNamespace("ab", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XmlNode LXMLNodeSection = XmlDoc.SelectSingleNode(Const6101.WriteField, xmlManager);

            //添加需要的内容
            //<TablixMember/>
            XmlElement xmlTCWidth = XmlDoc.CreateElement("TablixMember", Const6101.XmlNamespace);
            LXMLNodeSection.AppendChild(xmlTCWidth);
            //最后统一另存为修改
        }

        private static void WriteTBLRBorder(XmlDocument XmlDoc, XmlElement xmlE)
        {
            XmlElement xmlColor = XmlDoc.CreateElement("Color", Const6101.XmlNamespace);
            xmlColor.InnerText = "Black";
            XmlElement xmlStyle = XmlDoc.CreateElement("Style", Const6101.XmlNamespace);
            xmlStyle.InnerText = "Solid";
            XmlElement xmlWidth = XmlDoc.CreateElement("Width", Const6101.XmlNamespace);
            xmlWidth.InnerText = "1pt";
            xmlE.AppendChild(xmlColor);
            xmlE.AppendChild(xmlStyle);
            xmlE.AppendChild(xmlWidth);
        }
        #endregion


    }
}
