using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using UMPTools.Models;
using UMPTools.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private ObservableCollection<DBTypeItem> mListDBTypeItems;
        private ObservableCollection<GlobalSettingItem> mListGlobalSettings;
        private ObservableCollection<LangEntityItem> mListLangEntities;
        private ObservableCollection<LangModuleItem> mListLangModules;
        private ObservableCollection<ModuleInfo> mListColumnModules;
        private ObservableCollection<ModuleInfo> mListColumnSubModules;
        private ObservableCollection<ViewColumnInfo> mListColumnColumns;
        private ObservableCollection<ViewColumnItem> mListColumnItems;
        private ObservableCollection<ConfigNameItem> mListConfigNameItems;
        private SessionInfo mSession;
        private SystemConfig mSystemConfig;
        private ConfigInfo mConfigInfo;
        private string mAppName = "UMPTools";
        private BackgroundWorker mWorker;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListDBTypeItems = new ObservableCollection<DBTypeItem>();
            mListGlobalSettings = new ObservableCollection<GlobalSettingItem>();
            mListLangEntities = new ObservableCollection<LangEntityItem>();
            mListLangModules = new ObservableCollection<LangModuleItem>();
            mListColumnModules = new ObservableCollection<ModuleInfo>();
            mListColumnSubModules = new ObservableCollection<ModuleInfo>();
            mListColumnColumns = new ObservableCollection<ViewColumnInfo>();
            mListColumnItems = new ObservableCollection<ViewColumnItem>();
            mListConfigNameItems = new ObservableCollection<ConfigNameItem>();

            Loaded += MainWindow_Loaded;
            BtnGenerate.Click += BtnGenerate_Click;
            BtnGetAppSession.Click += BtnGetAppSession_Click;
            BtnDealCommand.Click += BtnDealCommand_Click;
            BtnLangInit.Click += BtnLangInit_Click;
            ComboEntities.SelectionChanged += ComboEntities_SelectionChanged;
            BtnNetPipeTest.Click += BtnNetPipeTest_Click;
            BtnNetPipeSend.Click += BtnNetPipeSend_Click;
            BtnWcfTest.Click += BtnWcfTest_Click;
            ComboColumnModule.SelectionChanged += ComboColumnModule_SelectionChanged;
            ComboColumnSubModule.SelectionChanged += ComboColumnSubModule_SelectionChanged;
            BtnConfigInfoGenerate.Click += BtnConfigInfoGenerate_Click;
            BtnConfigInfoApply.Click += BtnConfigInfoApply_Click;
            BtnConfigInfoRemove.Click += BtnConfigInfoRemove_Click;
            LvConfigInfoList.SelectionChanged += LvConfigInfoList_SelectionChanged;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ComboDBType.ItemsSource = mListDBTypeItems;
            ListBoxGlobalSettings.ItemsSource = mListGlobalSettings;
            ComboEntities.ItemsSource = mListLangEntities;
            ComboModules.ItemsSource = mListLangModules;
            ComboColumnModule.ItemsSource = mListColumnModules;
            ComboColumnSubModule.ItemsSource = mListColumnSubModules;
            LvColumnData.ItemsSource = mListColumnItems;
            LvConfigInfoList.ItemsSource = mListGlobalSettings;
            ComboConfigName.ItemsSource = mListConfigNameItems;

            InitSystemConfig();
            InitDBTypeItems();
            InitDescription();
            InitLangEntities();
            InitColumnColumns();
            InitColumnModuleList();
            InitConfigNameInfos();
            Init();
            InitSessionInfo();
            InitValues();

            CreateColumnColumns();
        }


        #region Init and Load

        private void Init()
        {
            mSession = new SessionInfo();
            mSession.SessionID = Guid.NewGuid().ToString();
            mSession.AppName = mAppName;
            mSession.LastActiveTime = DateTime.Now;

            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            mSession.RentInfo = rentInfo;
            mSession.RentID = ConstValue.RENT_DEFAULT;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            userInfo.Password = "voicecyber";
            mSession.UserInfo = userInfo;
            mSession.UserID = ConstValue.USER_ADMIN;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            mSession.RoleInfo = roleInfo;
            mSession.RoleID = ConstValue.ROLE_SYSTEMADMIN;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.6.55";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;
            //Session.AppServerInfo = serverInfo;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "https";
            serverInfo.Address = "192.168.6.27";
            serverInfo.Port = 8082;
            serverInfo.SupportHttps = true;
            mSession.AppServerInfo = serverInfo;

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            mSession.ThemeInfo = themeInfo;
            mSession.ThemeName = "Default";

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            mSession.SupportThemes.Add(themeInfo);

            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            mSession.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            mSession.SupportLangTypes.Add(langType);
            mSession.LangTypeInfo = langType;
            mSession.LangTypeID = langType.LangID;

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "zh-cn";
            langType.Display = "繁体中文";
            mSession.SupportLangTypes.Add(langType);

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
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB1424";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            mSession.DatabaseInfo = dbInfo;
            mSession.DBType = dbInfo.TypeID;
            mSession.DBConnectionString = dbInfo.GetConnectionString();

            mSession.InstallPath = @"C:\UMPRelease";
        }

        private void InitDBTypeItems()
        {
            try
            {
                mListDBTypeItems.Clear();
                DBTypeItem item = new DBTypeItem();
                item.TypeID = 2;
                item.Name = "MSSQL";
                mListDBTypeItems.Add(item);
                item = new DBTypeItem();
                item.TypeID = 3;
                item.Name = "Oracle";
                mListDBTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Commands:\r\n"));
            sb.Append(string.Format("0\tGetMonitorObject\r\n"));
            sb.Append(string.Format("1\tGetSessionInfo\r\n"));
            sb.Append(string.Format("\r\n"));
            sb.Append(string.Format("DataType:\r\n"));
            sb.Append(string.Format("1\tSessionInfo\r\n"));
            sb.Append(string.Format("10\tWebRequest\r\n"));
            sb.Append(string.Format("11\tWebReturn\r\n"));
            TxtDescription.Text = sb.ToString();

            sb = new StringBuilder();
            sb.Append(string.Format("Commands:\r\n"));
            sb.Append(string.Format("20002\tSCChangePassword\r\n"));
            sb.Append(string.Format("20003\tSCLogout\r\n"));
            sb.Append(string.Format("21010\tSCOperation\r\n"));
            sb.Append(string.Format("22001\tSCThemeChange\r\n"));
            sb.Append(string.Format("22002\tSCLanguageChange\r\n"));
            sb.Append(string.Format("22003\tSCRoleChange\r\n"));
            TxtNetPipeDescription.Text = sb.ToString();
        }

        private void InitSessionInfo()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "umpsession.xml");
                if (!File.Exists(path)) { return; }
                OperationReturn optReturn = XMLHelper.DeserializeFile<SessionInfo>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                SessionInfo session = optReturn.Data as SessionInfo;
                if (session == null)
                {
                    ShowErrorMessage(string.Format("SessionInfo is null"));
                    return;
                }
                mSession.SetSessionInfo(session);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitValues()
        {
            try
            {
                if (mSession == null) { return; }
                TxtSessionID.Text = mSession.SessionID;
                TxtAppName.Text = mSession.AppName;
                TxtLastActiveTime.Text = mSession.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss");
                CbIsMonitor.IsChecked = mSession.IsMonitor;

                AppServerInfo appServer = mSession.AppServerInfo;
                TxtProtocol.Text = appServer.Protocol;
                TxtAppHost.Text = appServer.Address;
                TxtAppPort.Text = appServer.Port.ToString();
                CbSupportHttps.IsChecked = appServer.SupportHttps;

                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                var item = mListDBTypeItems.FirstOrDefault(d => d.TypeID == dbInfo.TypeID);
                if (item != null)
                {
                    ComboDBType.SelectedItem = item;
                }
                TxtDBHost.Text = dbInfo.Host;
                TxtDBPort.Text = dbInfo.Port.ToString();
                TxtDBName.Text = dbInfo.DBName;
                TxtDBLoginUser.Text = dbInfo.LoginName;
                TxtDBPassword.Text = dbInfo.Password;

                TxtWcfServiceID.Text = "11012";
                TxtWcfCode.Text = "30011";

                InitGlobalSettings();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitConfigNameInfos()
        {
            try
            {
                mListConfigNameItems.Clear();

                ConfigNameItem item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_LOG_MODE;
                item.Description = "Log Mode (31 for all log, 30 for default log)";
                mListConfigNameItems.Add(item);

                item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_TIMEOUT_SESSION;
                item.Description = "Session timeout (over this time server will remove current session)";
                mListConfigNameItems.Add(item);

                item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_INTERVAL_REREADPARAM;
                item.Description = "ReRead param interval (Service will reread param per this time)";
                mListConfigNameItems.Add(item);

                item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_INTERVAL_RECONNECTDB;
                item.Description = "ReConnect database interval (Service will reconnect to database per this time when connect fail)";
                mListConfigNameItems.Add(item);

                item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_INTERVAL_CHECKSESSION;
                item.Description = "CheckSession interval (Server will check session per this time and remove timeout session)";
                mListConfigNameItems.Add(item);

                item = new ConfigNameItem();
                item.Name = ConstValue.GS_KEY_C021_MODE;
                item.Description = "Resource management mode ( A for agent, E for Extension and R for RealExtension)";
                mListConfigNameItems.Add(item);

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitGlobalSettings()
        {
            try
            {
                mListGlobalSettings.Clear();
                if (mSession == null) { return; }
                if (mSession.ListGlobalSettings != null)
                {
                    for (int i = 0; i < mSession.ListGlobalSettings.Count; i++)
                    {
                        var item = GlobalSettingItem.CreateItem(mSession.ListGlobalSettings[i]);
                        mListGlobalSettings.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitColumnColumns()
        {
            try
            {
                mListColumnColumns.Clear();
                ViewColumnInfo column = new ViewColumnInfo();
                column.ViewID = 0;
                column.ColumnName = "ViewID";
                column.Display = "View ID";
                column.Description = "View ID";
                column.SortID = 1;
                column.Width = 80;
                column.Visibility = "1";
                mListColumnColumns.Add(column);
                column = new ViewColumnInfo();
                column.ViewID = 0;
                column.ColumnName = "ColumnName";
                column.Display = "ColumnName";
                column.Description = "ColumnName";
                column.SortID = 2;
                column.Width = 120;
                column.Visibility = "1";
                mListColumnColumns.Add(column);
                column = new ViewColumnInfo();
                column.ViewID = 0;
                column.ColumnName = "Visibility";
                column.Display = "Visibility";
                column.Description = "Visibility";
                column.SortID = 3;
                column.Width = 50;
                column.Visibility = "1";
                mListColumnColumns.Add(column);
                column = new ViewColumnInfo();
                column.ViewID = 0;
                column.ColumnName = "Width";
                column.Display = "Width";
                column.Description = "Width";
                column.SortID = 4;
                column.Width = 80;
                column.Visibility = "1";
                mListColumnColumns.Add(column);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitLangEntities()
        {
            try
            {
                mListLangEntities.Clear();
                LangEntityItem item = new LangEntityItem();
                item.Entity = LangEntityItem.BASICINFODATA;
                item.Name = "BID";
                item.Display = "Basic Info Data";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.BASICINFODATA_DESC;
                item.Name = "BIDD";
                item.Display = "Basic Info Data Description";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.FEATURE_OPERATION;
                item.Name = "FO";
                item.Display = "Feature Operation";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.VIEW_COLUMN;
                item.Name = "COL";
                item.Display = "View Column";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.RESOURCE_OBJECT;
                item.Name = "OBJ";
                item.Display = "Resource Object";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.RESOURCE_OBJECT_DESC;
                item.Name = "OBJD";
                item.Display = "Resource Object Description";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.RESOURCE_PROPERTY;
                item.Name = "PRO";
                item.Display = "Resource Property";
                mListLangEntities.Add(item);
                item = new LangEntityItem();
                item.Entity = LangEntityItem.RESOURCE_PROPERTY_DESC;
                item.Name = "PROD";
                item.Display = "Resource Property Description";
                mListLangEntities.Add(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitLangModules()
        {
            try
            {
                mListLangModules.Clear();
                var item = ComboEntities.SelectedItem as LangEntityItem;
                if (item == null) { return; }
                switch (item.Entity)
                {
                    case LangEntityItem.BASICINFODATA:
                    case LangEntityItem.BASICINFODATA_DESC:
                    case LangEntityItem.FEATURE_OPERATION:
                    case LangEntityItem.VIEW_COLUMN:
                        if (mSystemConfig == null
                            || mSystemConfig.ListModuleInfos == null)
                        {
                            return;
                        }
                        for (int i = 0; i < mSystemConfig.ListModuleInfos.Count; i++)
                        {
                            var module = mSystemConfig.ListModuleInfos[i];
                            if (module.Module > 1000 && module.Module < 10000)
                            {
                                var info = new LangModuleItem();
                                info.Name = module.Name;
                                info.Display = module.Display;
                                info.Module = module.Module;
                                info.Main = module.Main;
                                mListLangModules.Add(info);
                            }
                        }
                        break;
                    case LangEntityItem.RESOURCE_OBJECT:
                    case LangEntityItem.RESOURCE_OBJECT_DESC:
                    case LangEntityItem.RESOURCE_PROPERTY:
                    case LangEntityItem.RESOURCE_PROPERTY_DESC:
                        if (mSystemConfig == null
                            || mSystemConfig.ListModuleInfos == null)
                        {
                            return;
                        }
                        for (int i = 0; i < mSystemConfig.ListModuleInfos.Count; i++)
                        {
                            var module = mSystemConfig.ListModuleInfos[i];
                            if (module.Module > 100 && module.Module < 1000)
                            {
                                var info = new LangModuleItem();
                                info.Name = module.Name;
                                info.Display = module.Display;
                                info.Module = module.Module;
                                info.Main = module.Main;
                                mListLangModules.Add(info);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitSystemConfig()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(path, SystemConfig.CONFIG_NAME);
                if (!File.Exists(path))
                {
                    ShowErrorMessage(string.Format("SystemConfig file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<SystemConfig>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                var sysConfig = optReturn.Data as SystemConfig;
                if (sysConfig == null)
                {
                    ShowErrorMessage(string.Format("System Config is null"));
                    return;
                }
                mSystemConfig = sysConfig;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitColumnModuleList()
        {
            try
            {
                mListColumnModules.Clear();
                if (mSystemConfig == null) { return; }
                var listModuleInfos = mSystemConfig.ListModuleInfos;
                if (listModuleInfos == null) { return; }
                for (int i = 0; i < listModuleInfos.Count; i++)
                {
                    var info = listModuleInfos[i];
                    if (info.Module >= 10 && info.Module < 100)
                    {
                        var temp = mListColumnModules.FirstOrDefault(t => t.Module == info.Module);
                        if (temp == null)
                        {
                            mListColumnModules.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitColumnSubModuleList()
        {
            try
            {
                mListColumnSubModules.Clear();
                var item = ComboColumnModule.SelectedItem as ModuleInfo;
                if (item == null) { return; }
                int module = item.Module;
                if (mSystemConfig == null) { return; }
                var listModules = mSystemConfig.ListModuleInfos;
                if (listModules == null) { return; }
                var temp = listModules.Where(t => t.Main == module).ToList();
                for (int i = 0; i < temp.Count; i++)
                {
                    var sub = temp[i];
                    if (sub.Module >= 1000 && sub.Module < 10000)
                    {
                        mListColumnSubModules.Add(sub);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitColumnItemList()
        {
            try
            {
                mListColumnItems.Clear();
                var moduleItem = ComboColumnSubModule.SelectedItem as ModuleInfo;
                if (moduleItem == null) { return; }
                int moduleID = moduleItem.Module;
                if (!(moduleID > 1000 && moduleID < 10000)) { return; }
                string strViewID = TxtColumnViewID.Text;
                int viewID;
                if (!int.TryParse(strViewID, out viewID)
                    || viewID <= 0
                    || viewID >= 10000)
                {
                    ShowErrorMessage(string.Format("ViewID invalid"));
                    return;
                }
                viewID = moduleID * 1000 + viewID;
                if (mSession == null) { return; }
                string strUserID = TxtColumnUserID.Text;
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    ShowErrorMessage(string.Format("User ID invalid"));
                    return;
                }
                string rentToken = mSession.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                OperationReturn optReturn;
                switch (mSession.DBType)
                {
                    case 2:
                        if (userID > 0)
                        {
                            strSql = string.Empty;
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_00_102 WHERE C001 = {0} ORDER BY C001,C012", viewID);
                        }
                        optReturn = MssqlOperation.GetDataSet(mSession.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (userID > 0)
                        {
                            strSql = string.Empty;
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_00_102 WHERE C001 = {0} ORDER BY C001,C012", viewID);
                        }
                        optReturn = OracleOperation.GetDataSet(mSession.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        ShowErrorMessage(string.Format("DBType invalid"));
                        return;
                }
                if (objDataSet == null)
                {
                    ShowErrorMessage(string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ViewColumnInfo column = new ViewColumnInfo();
                    column.ViewID = Convert.ToInt64(dr["C001"]);
                    string strName = dr["C002"].ToString();
                    column.ColumnName = strName;
                    column.Display = strName;
                    column.Description = dr["C017"].ToString();
                    column.DataType = Convert.ToInt32(dr["C003"]);
                    column.Width = Convert.ToInt32(dr["C007"]);
                    column.Visibility = dr["C011"].ToString();
                    column.SortID = Convert.ToInt32(dr["C012"]);
                    ViewColumnItem item = ViewColumnItem.CreateItem(column);
                    mListColumnItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!GetSessionInfoSettings()) { return; }

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "umpsession.xml");
                OperationReturn optReturn = XMLHelper.SerializeFile(mSession, path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ShowInfoMessage(string.Format("End.\t{0}", path));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnGetAppSession_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appSessionID = TxtAppSessionID.Text;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)RequestCode.CSMonitor;
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(ConstValue.MONITOR_COMMAND_GETSESSIONINFO.ToString());
                WebReturn webReturn = SendNetPipeMessage(webRequest, appSessionID);
                if (!webReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string time = webReturn.Data;
                ShowInfoMessage(string.Format("{0}", time));
                SessionInfo session = webReturn.Session;
                mSession.SetSessionInfo(session);
                mSession.SessionID = session.SessionID;
                mSession.AppName = session.AppName;
                mSession.LastActiveTime = session.LastActiveTime;
                mSession.IsMonitor = session.IsMonitor;
                InitValues();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnDealCommand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appSessionID = TxtAppSessionID.Text;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)RequestCode.CSMonitor;

                string strCommand = TxtCommand.Text;
                int intCommand;
                if (!int.TryParse(strCommand, out intCommand))
                {
                    ShowErrorMessage(string.Format("Command invalid"));
                    return;
                }
                //开启
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(intCommand.ToString());
                if (intCommand == 0)
                {
                    //Command 0 表示获取监控列表中的对象
                    string strType = TxtType.Text;  //对象类型
                    int intType;
                    if (!int.TryParse(strType, out intType))
                    {
                        ShowErrorMessage(string.Format("Type invalid"));
                        return;
                    }
                    webRequest.ListData.Add(intType.ToString());
                    string strName = TxtName.Text;  //对象名称
                    webRequest.ListData.Add(strName);
                    int index;
                    if (!int.TryParse(TxtIndex.Text, out index)  //索引顺序
                        || index < 0)
                    {
                        ShowErrorMessage(string.Format("Index invalid"));
                        return;
                    }
                    webRequest.ListData.Add(index.ToString());
                }

                WebReturn webReturn = SendNetPipeMessage(webRequest, appSessionID);
                if (!webReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string data = webReturn.Data;
                AppendMessage(data);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnLangInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if(mSystemConfig==null){return;}
                //string path = AppDomain.CurrentDomain.BaseDirectory;
                //path = Path.Combine(path, SystemConfig.CONFIG_NAME);
                //OperationReturn optReturn = XMLHelper.SerializeFile(mSystemConfig, path);
                //if (!optReturn.Result)
                //{
                //    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //ShowInfoMessage(string.Format("Save end"));

                LangGenerate();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void ComboEntities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitLangModules();
        }

        void BtnNetPipeSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sessionID = TxtNetPipeAppSession.Text;
                if (string.IsNullOrEmpty(sessionID))
                {
                    ShowErrorMessage(string.Format("SessionID is empty"));
                    return;
                }
                int command;
                if (!int.TryParse(TxtNetPipeCommand.Text, out command))
                {
                    ShowErrorMessage(string.Format("Command invalid"));
                    return;
                }
                string strData = TxtNetPipeData.Text;
                string strListData1 = TxtNetPipeData1.Text;
                string strListData2 = TxtNetPipeData2.Text;
                string strListData3 = TxtNetPipeData3.Text;
                string strListData4 = TxtNetPipeData4.Text;
                string strListData5 = TxtNetPipeData5.Text;
                WebRequest request = new WebRequest();
                request.Session = mSession;
                request.Code = command;
                request.Data = strData;
                request.ListData.Add(strListData1);
                request.ListData.Add(strListData2);
                request.ListData.Add(strListData3);
                request.ListData.Add(strListData4);
                request.ListData.Add(strListData5);
                WebReturn webReturn = SendNetPipeMessage(request, sessionID);
                if (!webReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strReturn = webReturn.LogInfo();
                AppendMessage(strReturn);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnNetPipeTest_Click(object sender, RoutedEventArgs e)
        {

        }

        void ComboColumnModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitColumnSubModuleList();
            InitColumnItemList();
        }

        void ComboColumnSubModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitColumnItemList();
        }

        void BtnConfigInfoRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = LvConfigInfoList.SelectedItem as GlobalSettingItem;
                if (item != null)
                {
                    mListGlobalSettings.Remove(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnConfigInfoApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strName = ComboConfigName.Text;
                var name = ComboConfigName.SelectedItem as ConfigNameItem;
                if (name != null)
                {
                    strName = name.Name;
                }
                string strValue = TxtConfigValue.Text;
                if (string.IsNullOrEmpty(strName)
                    || string.IsNullOrEmpty(strValue))
                {
                    ShowErrorMessage(string.Format("Name or value empty!"));
                    return;
                }
                var item = mListGlobalSettings.FirstOrDefault(s => s.Key == strName);
                if (item == null)
                {
                    GlobalSetting settiing = new GlobalSetting();
                    settiing.Key = strName;
                    settiing.Value = strValue;
                    item = new GlobalSettingItem();
                    item.Key = settiing.Key;
                    item.Value = settiing.Value;
                    item.Info = settiing;
                    mListGlobalSettings.Add(item);
                }
                else
                {
                    var info = item.Info;
                    if (info != null)
                    {
                        info.Value = strValue;
                    }
                    else
                    {
                        info = new GlobalSetting();
                        info.Key = strName;
                        info.Value = strValue;
                    }
                    item.Value = info.Value;
                }
                LvConfigInfoList.SelectedItem = item;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnConfigInfoGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mConfigInfo == null)
                {
                    mConfigInfo = new ConfigInfo();
                    mConfigInfo.FileName = ConstValue.TEMP_FILE_CONFIGINFO;
                }
                mConfigInfo.ListSettings.Clear();
                for (int i = 0; i < mListGlobalSettings.Count; i++)
                {
                    var item = mListGlobalSettings[i];
                    var setting = item.Info;
                    if (setting != null)
                    {
                        mConfigInfo.ListSettings.Add(setting);
                    }
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mConfigInfo.FileName);
                OperationReturn optReturn = XMLHelper.SerializeFile(mConfigInfo, path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ShowInfoMessage(string.Format("Generate end.\t{0}", path));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void LvConfigInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = LvConfigInfoList.SelectedItem as GlobalSettingItem;
                if (item != null)
                {
                    var name = mListConfigNameItems.FirstOrDefault(n => n.Name == item.Key);
                    if (name != null)
                    {
                        ComboConfigName.SelectedItem = name;
                    }
                    else
                    {
                        ComboConfigName.Text = item.Key;
                    }
                    TxtConfigValue.Text = item.Value;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnWcfTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strServiceID = TxtWcfServiceID.Text;
                int serviceID;
                if (!int.TryParse(strServiceID, out serviceID)
                    || serviceID < 10000
                    || serviceID > 99999)
                {
                    ShowErrorMessage(string.Format("Service ID invalid.\t{0}", strServiceID));
                    return;
                }
                string strCode = TxtWcfCode.Text;
                int intCode;
                if (!int.TryParse(strCode, out intCode)
                    || intCode < 0)
                {
                    ShowErrorMessage(string.Format("Code invalid.\t{0}", strCode));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = intCode;
                webRequest.Data = TxtWcfData.Text;
                webRequest.ListData.Add(TxtWcfListData1.Text);
                webRequest.ListData.Add(TxtWcfListData2.Text);
                webRequest.ListData.Add(TxtWcfListData3.Text);
                webRequest.ListData.Add(TxtWcfListData4.Text);
                webRequest.ListData.Add(TxtWcfListData5.Text);

                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(mSession.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowErrorMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                AppendMessage(string.Format("Return Message:{0}", webReturn.Message));
                AppendMessage(string.Format("Return Data:{0}", webReturn.Data));
                if (webReturn.ListData != null)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        string strInfo = webReturn.ListData[i];
                        AppendMessage(string.Format("Return ListData{0}:{1}", i + 1, strInfo));
                    }
                }
                AppendMessage(string.Format("End"));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void LangGenerate()
        {
            try
            {
                LangGenConfig config = new LangGenConfig();
                var entity = ComboEntities.SelectedItem as LangEntityItem;
                if (entity == null) { return; }
                config.Entity = entity;
                var module = ComboModules.SelectedItem as LangModuleItem;
                if (module == null) { return; }
                config.Module = module;
                bool isResplace = CbReqplace.IsChecked == true;
                config.IsReplace = isResplace;
                bool is2052Only = Cb2052Only.IsChecked == true;
                config.Is2052Only = is2052Only;
                GetSessionInfoSettings();
                if (mSession == null || mSession.DatabaseInfo == null) { return; }
                var dbInfo = mSession.DatabaseInfo;
                config.DBInfo = dbInfo;
                MyWaiter.Visibility = Visibility.Visible;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    DoLangGenerate(config);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void DoLangGenerate(LangGenConfig config)
        {
            try
            {
                var dbInfo = config.DBInfo;
                var entity = config.Entity;
                var module = config.Module;
                var isReplace = config.IsReplace;
                var is2052Only = config.Is2052Only;
                if (dbInfo == null) { return; }
                string strCon = dbInfo.GetConnectionString();
                string strSqlSource;
                string strSqlTarget;
                DataSet objDataSetSource;
                DataSet objDataSetTarget;
                OperationReturn optReturn = null;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DbCommandBuilder objCmdBuilder = null;
                switch (entity.Entity)
                {

                    #region BID

                    case LangEntityItem.BASICINFODATA:
                    case LangEntityItem.BASICINFODATA_DESC:
                        if (module.Module > 1000 && module.Module < 10000)
                        {
                            strSqlSource = string.Format("select * from t_00_003 where c001 >= {0} and c001 < {1}",
                                module.Module * 100000,
                                (module.Module + 1) * 100000);
                            string pre = "BID";
                            if (entity.Entity == LangEntityItem.BASICINFODATA)
                            {
                                pre = "BID";
                            }
                            if (entity.Entity == LangEntityItem.BASICINFODATA_DESC)
                            {
                                pre = "BIDD";
                            }
                            strSqlTarget =
                                string.Format(
                                    "select * from t_00_005 where {0} c002 like '{1}{2}%' and c010 = {2}",
                                    is2052Only ? string.Format("c001 = 2052 and") : string.Empty,
                                    pre,
                                    module.Module);
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    optReturn = MssqlOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = MssqlOperation.GetConnection(strCon);
                                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    break;
                                case 3:
                                    optReturn = OracleOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = OracleOperation.GetConnection(strCon);
                                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    break;
                            }
                        }
                        break;

                    #endregion


                    #region COL

                    case LangEntityItem.VIEW_COLUMN:
                        if (module.Module > 1000 && module.Module < 10000)
                        {
                            strSqlSource = string.Format("select * from t_00_102 where c001 >= {0} and c001 < {1}",
                                module.Module * 1000,
                                (module.Module + 1) * 1000);
                            string pre = "COL";
                            strSqlTarget =
                                string.Format(
                                    "select * from t_00_005 where {0} c002 like '{1}{2}%' and c010 = {2}",
                                    is2052Only ? string.Format("c001 = 2052 and") : string.Empty,
                                    pre,
                                    module.Module);
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    optReturn = MssqlOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = MssqlOperation.GetConnection(strCon);
                                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    break;
                                case 3:
                                    optReturn = OracleOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = OracleOperation.GetConnection(strCon);
                                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    break;
                            }
                        }
                        break;

                    #endregion


                    #region FO

                    case LangEntityItem.FEATURE_OPERATION:
                        if (module.Module > 1000 && module.Module < 10000)
                        {
                            strSqlSource = string.Format("select * from t_11_003_00000 where c002 like '{0}%'",
                                module.Module);
                            string pre = "FO";
                            strSqlTarget =
                                string.Format(
                                    "select * from t_00_005 where {0} c002 like '{1}{2}%' and c010 = {2}",
                                    is2052Only ? string.Format("c001 = 2052 and") : string.Empty,
                                    pre,
                                    module.Module);
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    optReturn = MssqlOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = MssqlOperation.GetConnection(strCon);
                                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    break;
                                case 3:
                                    optReturn = OracleOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = OracleOperation.GetConnection(strCon);
                                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    break;
                            }
                        }
                        break;

                    #endregion


                    #region OBJ

                    case LangEntityItem.RESOURCE_OBJECT:
                    case LangEntityItem.RESOURCE_OBJECT_DESC:
                        if (module.Module > 100 && module.Module < 1000)
                        {
                            strSqlSource = string.Format("select * from t_00_010 where c001 = {0}",
                                module.Module);
                            string pre = "OBJ";
                            if (entity.Entity == LangEntityItem.RESOURCE_OBJECT)
                            {
                                pre = "OBJ";
                            }
                            if (entity.Entity == LangEntityItem.RESOURCE_OBJECT_DESC)
                            {
                                pre = "OBJD";
                            }
                            strSqlTarget =
                                string.Format(
                                    "select * from t_00_005 where {0} c002 like '{1}{2}' and c009 = 0 and c010 = 0",
                                    is2052Only ? string.Format("c001 = 2052 and") : string.Empty,
                                    pre,
                                    module.Module);
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    optReturn = MssqlOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = MssqlOperation.GetConnection(strCon);
                                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    break;
                                case 3:
                                    optReturn = OracleOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = OracleOperation.GetConnection(strCon);
                                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    break;
                            }
                        }
                        break;

                    #endregion


                    #region PRO

                    case LangEntityItem.RESOURCE_PROPERTY:
                    case LangEntityItem.RESOURCE_PROPERTY_DESC:
                        if (module.Module > 100 && module.Module < 1000)
                        {
                            strSqlSource = string.Format("select * from t_00_009 where c001 = {0}",
                                module.Module);
                            string pre = "PRO";
                            if (entity.Entity == LangEntityItem.RESOURCE_PROPERTY)
                            {
                                pre = "PRO";
                            }
                            if (entity.Entity == LangEntityItem.RESOURCE_PROPERTY_DESC)
                            {
                                pre = "PROD";
                            }
                            strSqlTarget =
                                string.Format(
                                    "select * from t_00_005 where {0} c002 like '{1}{2}%' and c009 = 0 and c010 = 0",
                                    is2052Only ? string.Format("c001 = 2052 and") : string.Empty,
                                    pre,
                                    module.Module);
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    optReturn = MssqlOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = MssqlOperation.GetConnection(strCon);
                                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    break;
                                case 3:
                                    optReturn = OracleOperation.GetDataSet(strCon, strSqlSource);
                                    objConn = OracleOperation.GetConnection(strCon);
                                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSqlTarget);
                                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    break;
                            }
                        }
                        break;

                    #endregion

                }
                if (optReturn == null)
                {
                    ShowErrorMessage(string.Format("Fail.\tOperationReturn is null"));
                    return;
                }
                objDataSetSource = optReturn.Data as DataSet;
                if (objDataSetSource == null)
                {
                    ShowErrorMessage(string.Format("Fail.\tDataSet is null"));
                    return;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    ShowErrorMessage(string.Format("Fail.\tDataAdapter is null"));
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    objDataSetTarget = new DataSet();
                    objAdapter.Fill(objDataSetTarget);
                    int addCount = 0;
                    int modifyCount = 0;
                    int skipCount = 0;
                    int acceptCount = 0;
                    string pre;
                    int objType;
                    int proID;
                    for (int i = 0; i < objDataSetSource.Tables[0].Rows.Count; i++)
                    {
                        var row = objDataSetSource.Tables[0].Rows[i];
                        DataRow[] rows;
                        DataRow temp;
                        string strName;
                        string strDisplay;
                        switch (entity.Entity)
                        {

                            #region BID

                            case LangEntityItem.BASICINFODATA:
                            case LangEntityItem.BASICINFODATA_DESC:
                                long infoID = Convert.ToInt64(row["C001"]);
                                int sortID = Convert.ToInt32(row["C002"]);
                                pre = "BID";
                                if (entity.Entity == LangEntityItem.BASICINFODATA)
                                {
                                    pre = "BID";
                                }
                                if (entity.Entity == LangEntityItem.BASICINFODATA_DESC)
                                {
                                    pre = "BIDD";
                                }
                                strName = string.Format("{0}{1}{2}", pre, infoID, sortID.ToString("000"));
                                strDisplay = row["C007"].ToString();
                                if (is2052Only)
                                {
                                    rows =
                                        objDataSetTarget.Tables[0].Select(string.Format("C001 = 2052 and C002 = '{0}'",
                                            strName));
                                }
                                else
                                {
                                    rows = objDataSetTarget.Tables[0].Select(string.Format("C002 = '{0}'", strName));
                                }
                                if (rows.Length <= 0)
                                {
                                    temp = objDataSetTarget.Tables[0].NewRow();
                                    temp["C001"] = 2052;
                                    temp["C002"] = strName;
                                    temp["C003"] = 0;
                                    temp["C004"] = 0;
                                    temp["C005"] = strDisplay;
                                    temp["C009"] = module.Main;
                                    temp["C010"] = module.Module;
                                    objDataSetTarget.Tables[0].Rows.Add(temp);
                                    addCount++;
                                    acceptCount++;
                                    AppendMessage(string.Format("Add:{0};{1};{2};{3}", 2052, strName, strDisplay,
                                        module.Module));
                                }
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    temp = rows[j];
                                    if (isReplace)
                                    {
                                        temp["C005"] = strDisplay;
                                        modifyCount++;
                                        acceptCount++;
                                        AppendMessage(string.Format("Modify:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                            module.Module));
                                    }
                                    else
                                    {
                                        skipCount++;
                                        AppendMessage(string.Format("Skip:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                           module.Module));
                                    }
                                }
                                break;


                            #endregion


                            #region COL

                            case LangEntityItem.VIEW_COLUMN:
                                int viewID = Convert.ToInt32(row["C001"]);
                                string colName = row["C002"].ToString();
                                strName = string.Format("COL{0}{1}", viewID, colName);
                                strDisplay = row["C017"].ToString();
                                if (is2052Only)
                                {
                                    rows =
                                        objDataSetTarget.Tables[0].Select(string.Format("C001 = 2052 and C002 = '{0}'",
                                            strName));
                                }
                                else
                                {
                                    rows = objDataSetTarget.Tables[0].Select(string.Format("C002 = '{0}'", strName));
                                }
                                if (rows.Length <= 0)
                                {
                                    temp = objDataSetTarget.Tables[0].NewRow();
                                    temp["C001"] = 2052;
                                    temp["C002"] = strName;
                                    temp["C003"] = 0;
                                    temp["C004"] = 0;
                                    temp["C005"] = strDisplay;
                                    temp["C009"] = module.Main;
                                    temp["C010"] = module.Module;
                                    objDataSetTarget.Tables[0].Rows.Add(temp);
                                    addCount++;
                                    acceptCount++;
                                    AppendMessage(string.Format("Add:{0};{1};{2};{3}", 2052, strName, strDisplay,
                                        module.Module));
                                }
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    temp = rows[j];
                                    if (isReplace)
                                    {
                                        temp["C005"] = strDisplay;
                                        modifyCount++;
                                        acceptCount++;
                                        AppendMessage(string.Format("Modify:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                            module.Module));
                                    }
                                    else
                                    {
                                        skipCount++;
                                        AppendMessage(string.Format("Skip:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                           module.Module));
                                    }
                                }
                                break;

                            #endregion


                            #region FO

                            case LangEntityItem.FEATURE_OPERATION:
                                long optID = Convert.ToInt64(row["C002"]);
                                strName = string.Format("FO{0}", optID);
                                strDisplay = row["C015"].ToString();
                                if (string.IsNullOrEmpty(strDisplay))
                                {
                                    strDisplay = optID.ToString();
                                }
                                if (is2052Only)
                                {
                                    rows =
                                        objDataSetTarget.Tables[0].Select(string.Format("C001 = 2052 and C002 = '{0}'",
                                            strName));
                                }
                                else
                                {
                                    rows = objDataSetTarget.Tables[0].Select(string.Format("C002 = '{0}'", strName));
                                }
                                if (rows.Length <= 0)
                                {
                                    temp = objDataSetTarget.Tables[0].NewRow();
                                    temp["C001"] = 2052;
                                    temp["C002"] = strName;
                                    temp["C003"] = 0;
                                    temp["C004"] = 0;
                                    temp["C005"] = strDisplay;
                                    temp["C009"] = module.Main;
                                    temp["C010"] = module.Module;
                                    objDataSetTarget.Tables[0].Rows.Add(temp);
                                    addCount++;
                                    acceptCount++;
                                    AppendMessage(string.Format("Add:{0};{1};{2};{3}", 2052, strName, strDisplay,
                                        module.Module));
                                }
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    temp = rows[j];
                                    if (isReplace)
                                    {
                                        temp["C005"] = strDisplay;
                                        modifyCount++;
                                        acceptCount++;
                                        AppendMessage(string.Format("Modify:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                            module.Module));
                                    }
                                    else
                                    {
                                        skipCount++;
                                        AppendMessage(string.Format("Skip:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                           module.Module));
                                    }
                                }
                                break;

                            #endregion


                            #region OBJ

                            case LangEntityItem.RESOURCE_OBJECT:
                            case LangEntityItem.RESOURCE_OBJECT_DESC:
                                pre = "OBJ";
                                if (entity.Entity == LangEntityItem.RESOURCE_OBJECT)
                                {
                                    pre = "OBJ";
                                }
                                if (entity.Entity == LangEntityItem.RESOURCE_OBJECT_DESC)
                                {
                                    pre = "OBJD";
                                }
                                objType = Convert.ToInt32(row["C001"]);
                                strName = string.Format("{0}{1}", pre, objType);
                                strDisplay = row["C010"].ToString();
                                if (is2052Only)
                                {
                                    rows =
                                        objDataSetTarget.Tables[0].Select(string.Format("C001 = 2052 and C002 = '{0}'",
                                            strName));
                                }
                                else
                                {
                                    rows = objDataSetTarget.Tables[0].Select(string.Format("C002 = '{0}'", strName));
                                }
                                if (rows.Length <= 0)
                                {
                                    temp = objDataSetTarget.Tables[0].NewRow();
                                    temp["C001"] = 2052;
                                    temp["C002"] = strName;
                                    temp["C003"] = 0;
                                    temp["C004"] = 0;
                                    temp["C005"] = strDisplay;
                                    temp["C009"] = 0;
                                    temp["C010"] = 0;
                                    objDataSetTarget.Tables[0].Rows.Add(temp);
                                    addCount++;
                                    acceptCount++;
                                    AppendMessage(string.Format("Add:{0};{1};{2};{3}", 2052, strName, strDisplay,
                                        module.Module));
                                }
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    temp = rows[j];
                                    if (isReplace)
                                    {
                                        temp["C005"] = strDisplay;
                                        modifyCount++;
                                        acceptCount++;
                                        AppendMessage(string.Format("Modify:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                            module.Module));
                                    }
                                    else
                                    {
                                        skipCount++;
                                        AppendMessage(string.Format("Skip:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                           module.Module));
                                    }
                                }
                                break;

                            #endregion


                            #region PRO

                            case LangEntityItem.RESOURCE_PROPERTY:
                            case LangEntityItem.RESOURCE_PROPERTY_DESC:
                                pre = "PRO";
                                if (entity.Entity == LangEntityItem.RESOURCE_PROPERTY)
                                {
                                    pre = "PRO";
                                }
                                if (entity.Entity == LangEntityItem.RESOURCE_PROPERTY_DESC)
                                {
                                    pre = "PROD";
                                }
                                objType = Convert.ToInt32(row["C001"]);
                                proID = Convert.ToInt32(row["C002"]);
                                strName = string.Format("{0}{1}{2}", pre, objType, proID.ToString("000"));
                                strDisplay = row["C018"].ToString();
                                if (is2052Only)
                                {
                                    rows =
                                        objDataSetTarget.Tables[0].Select(string.Format("C001 = 2052 and C002 = '{0}'",
                                            strName));
                                }
                                else
                                {
                                    rows = objDataSetTarget.Tables[0].Select(string.Format("C002 = '{0}'", strName));
                                }
                                if (rows.Length <= 0)
                                {
                                    temp = objDataSetTarget.Tables[0].NewRow();
                                    temp["C001"] = 2052;
                                    temp["C002"] = strName;
                                    temp["C003"] = 0;
                                    temp["C004"] = 0;
                                    temp["C005"] = strDisplay;
                                    temp["C009"] = 0;
                                    temp["C010"] = 0;
                                    objDataSetTarget.Tables[0].Rows.Add(temp);
                                    addCount++;
                                    acceptCount++;
                                    AppendMessage(string.Format("Add:{0};{1};{2};{3}", 2052, strName, strDisplay,
                                        module.Module));
                                }
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    temp = rows[j];
                                    if (isReplace)
                                    {
                                        temp["C005"] = strDisplay;
                                        modifyCount++;
                                        acceptCount++;
                                        AppendMessage(string.Format("Modify:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                            module.Module));
                                    }
                                    else
                                    {
                                        skipCount++;
                                        AppendMessage(string.Format("Skip:{0};{1};{2};{3}", temp["C001"], strName, strDisplay,
                                           module.Module));
                                    }
                                }
                                break;

                            #endregion
                        }

                        SetStateMessage(string.Format("LangGenerate\t Add:{0};Modify:{1};Skip:{2}", addCount,
                            modifyCount, skipCount));

                        //没20条记录提交一次
                        if (acceptCount > 50)
                        {
                            objAdapter.Update(objDataSetTarget);
                            objDataSetTarget.AcceptChanges();

                            acceptCount = 0;
                        }
                    }

                    objAdapter.Update(objDataSetTarget);
                    objDataSetTarget.AcceptChanges();

                    AppendMessage(string.Format("End\tAdd:{0};Modify:{1};Skip:{2}", addCount, modifyCount,
                        skipCount));
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}", ex.Message));
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private bool GetSessionInfoSettings()
        {
            try
            {
                if (mSession == null)
                {
                    mSession = new SessionInfo();
                }
                mSession.SessionID = TxtSessionID.Text;
                mSession.AppName = TxtAppName.Text;
                mSession.LastActiveTime = DateTime.Parse(TxtLastActiveTime.Text);
                mSession.IsMonitor = CbIsMonitor.IsChecked == true;

                AppServerInfo appServer = new AppServerInfo();
                appServer.Protocol = TxtProtocol.Text;
                appServer.Address = TxtAppHost.Text;
                appServer.Port = int.Parse(TxtAppPort.Text);
                appServer.SupportHttps = CbSupportHttps.IsChecked == true;
                mSession.AppServerInfo = appServer;

                DatabaseInfo dbInfo = new DatabaseInfo();
                var dbTypeItem = ComboDBType.SelectedItem as DBTypeItem;
                if (dbTypeItem != null)
                    dbInfo.TypeID = dbTypeItem.TypeID;
                dbInfo.Host = TxtDBHost.Text;
                dbInfo.Port = int.Parse(TxtDBPort.Text);
                dbInfo.DBName = TxtDBName.Text;
                dbInfo.LoginName = TxtDBLoginUser.Text;
                dbInfo.Password = TxtDBPassword.Text;
                mSession.DatabaseInfo = dbInfo;
                mSession.DBConnectionString = dbInfo.GetConnectionString();

                for (int i = 0; i < mListGlobalSettings.Count; i++)
                {
                    var item = mListGlobalSettings[i];
                    mSession[item.Key] = item.Value;
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return false;
            }
            return true;
        }

        private void CreateColumnColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;

                for (int i = 0; i < mListColumnColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListColumnColumns[i];
                    if (columnInfo.Visibility != "1") { continue; }
                    gvc = new GridViewColumn();
                    gvch = new GridViewColumnHeader();
                    gvch.Content = columnInfo.Display;
                    gvch.ToolTip = columnInfo.Description;
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                    gv.Columns.Add(gvc);
                }

                LvColumnData.View = gv;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Basic

        private void ShowErrorMessage(string msg)
        {
            Dispatcher.Invoke(
                new Action(() => MessageBox.Show(msg, "UMP Tools", MessageBoxButton.OK, MessageBoxImage.Error)));
        }

        private void ShowInfoMessage(string msg)
        {
            Dispatcher.Invoke(
                new Action(() => MessageBox.Show(msg, "UMP Tools", MessageBoxButton.OK, MessageBoxImage.Information)));
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        private void SetStateMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LbMsg.Content = msg;
            }));
        }

        #endregion


        #region NetPipe

        public WebReturn SendNetPipeMessage(WebRequest request, string appSessionID)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                string url = string.Format("net.pipe://localhost/Client_{0}", appSessionID);
                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
                quotas.MaxArrayLength = int.MaxValue;
                quotas.MaxStringContentLength = int.MaxValue;
                binding.ReaderQuotas = quotas;
                ChannelFactory<IMessageHandler> factory = new ChannelFactory<IMessageHandler>(binding,
                    new EndpointAddress(url));
                IMessageHandler handler = factory.CreateChannel();
                return handler.DealMessage(request);
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = 1;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        #endregion
    }
}
