//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    db3ce7bf-0386-48c0-a06e-196e468645da
//        CLR Version:              4.0.30319.18063
//        Name:                     SessionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                SessionInfo
//
//        created by Charley at 2014/8/20 15:25:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 会话信息，用于保存一次会话需要的信息，通常包含一些全局信息
    /// </summary>
    [DataContract]
    public class SessionInfo
    {
        /// <summary>
        /// 会话ID，一个GUID，创建SessionInfo的自动生成一个GUID
        /// </summary>
        [DataMember]
        public string SessionID { get; set; }
        /// <summary>
        /// 客户端或子系统的名称，使用简称，不要太长
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 所属模块号，4位小模块号
        /// 这个模块号通常与小模块号对应，但有时候一个Application可能有多个模块，这时通常使用两位的大模块号+00表示
        /// 这个模块号会影响操作日志，文本日志，语言包等功能
        /// </summary>
        [DataMember]
        public int ModuleID { get; set; }
        /// <summary>
        /// 应用终端的类型，参考AppType中的定义
        /// </summary>
        [DataMember]
        public int AppType { get; set; }
        /// <summary>
        /// 最后一次与服务端通信的时间，每次请求wcf后应将此时间更新（UTC时间）
        /// 序列化中使用标准全日期时间的格式，即 yyyy-MM-dd HH:mm:ss
        /// </summary>
        [DataMember]
        public DateTime LastActiveTime { get; set; }
        /// <summary>
        /// 数据库类型，与DatabaseInfo的TypeID字段对应
        /// 0   Unkown
        /// 1   MySQL
        /// 2   Microsoft SQL Server
        /// 3   Oracle
        /// ......
        /// </summary>
        [DataMember]
        public int DBType { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [DataMember]
        public string DBConnectionString { get; set; }
        /// <summary>
        /// 数据库信息
        /// </summary>
        [DataMember]
        public DatabaseInfo DatabaseInfo { get; set; }
        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        [DataMember]
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 当前登录角色ID
        /// </summary>
        [DataMember]
        public long RoleID { get; set; }
        /// <summary>
        /// 角色信息
        /// </summary>
        [DataMember]
        public RoleInfo RoleInfo { get; set; }
        /// <summary>
        /// 登录流水号
        /// </summary>
        [DataMember]
        public long LoginID { get; set; }
        /// <summary>
        /// 登录信息
        /// </summary>
        [DataMember]
        public LoginInfo LoginInfo { get; set; }
        /// <summary>
        /// 域编码
        /// </summary>
        [DataMember]
        public long DomainID { get; set; }
        /// <summary>
        /// 域信息
        /// </summary>
        [DataMember]
        public DomainInfo DomainInfo { get; set; }
        /// <summary>
        /// 所在机构的ID
        /// </summary>
        [DataMember]
        public long OrgID { get; set; }
        /// <summary>
        /// 所在机构信息
        /// </summary>
        [DataMember]
        public OrgInfo OrgInfo { get; set; }
        /// <summary>
        /// 当前主题名称
        /// </summary>
        [DataMember]
        public string ThemeName { get; set; }
        /// <summary>
        /// 主题样式信息
        /// </summary>
        [DataMember]
        public ThemeInfo ThemeInfo { get; set; }
        /// <summary>
        /// 当前语言ID
        /// </summary>
        [DataMember]
        public int LangTypeID { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        [DataMember]
        public LangTypeInfo LangTypeInfo { get; set; }
        /// <summary>
        /// 服务器信息
        /// </summary>
        [DataMember]
        public AppServerInfo AppServerInfo { get; set; }
        /// <summary>
        /// 本机信息
        /// </summary>
        [DataMember]
        public LocalHostInfo LocalMachineInfo { get; set; }
        /// <summary>
        /// 安装目录
        /// </summary>
        [DataMember]
        public string InstallPath { get; set; }
        /// <summary>
        /// 应用程序版本（如：8.03.002）
        /// </summary>
        [DataMember]
        public string AppVersion { get; set; }
        /// <summary>
        /// 是否启用本地监视，
        /// 如果启用，则会把需要监视的对象放入一个特定的列表中
        /// 监视端可以请求App获取列表中的监视对象的内容
        /// 默认关闭
        /// </summary>
        [DataMember]
        public bool IsMonitor { get; set; }
        /// <summary>
        /// 当前域编码
        /// </summary>
        [DataMember]
        public long RentID { get; set; }
        /// <summary>
        /// 租户信息
        /// </summary>
        [DataMember]
        public RentInfo RentInfo { get; set; }

        private List<LangTypeInfo> mSupportLangTypes;
        /// <summary>
        /// 支持的语言种类
        /// </summary>
        [DataMember]
        public List<LangTypeInfo> SupportLangTypes
        {
            get { return mSupportLangTypes; }
            set { mSupportLangTypes = value; }
        }

        private List<ThemeInfo> mSupportThemes;
        /// <summary>
        /// 支持的主题种类
        /// </summary>
        [DataMember]
        public List<ThemeInfo> SupportThemes
        {
            get { return mSupportThemes; }
            set { mSupportThemes = value; }
        }

        private List<PartitionTableInfo> mListPartitionTables;
        /// <summary>
        /// 分表信息
        /// </summary>
        [DataMember]
        public List<PartitionTableInfo> ListPartitionTables
        {
            get { return mListPartitionTables; }
            set { mListPartitionTables = value; }
        }

        private List<GlobalSetting> mListGlobalSettings;
        /// <summary>
        /// 全局设定参数值列表
        /// </summary>
        public List<GlobalSetting> ListGlobalSettings
        {
            get { return mListGlobalSettings; }
            set { mListGlobalSettings = value; }
        }

        /// <summary>
        /// 创建SessionInfo，并初始化默认值
        /// </summary>
        public SessionInfo()
        {
            mSupportLangTypes = new List<LangTypeInfo>();
            mSupportThemes = new List<ThemeInfo>();
            mListPartitionTables = new List<PartitionTableInfo>();
            mListGlobalSettings = new List<GlobalSetting>();

            DatabaseInfo = new DatabaseInfo();
            UserInfo = new UserInfo();
            RoleInfo = new RoleInfo();
            LoginInfo = new LoginInfo();
            DomainInfo = new DomainInfo();
            OrgInfo = new OrgInfo();
            ThemeInfo = new ThemeInfo();
            LangTypeInfo = new LangTypeInfo();
            AppServerInfo = new AppServerInfo();
            RentInfo = new RentInfo();
            LocalMachineInfo = new LocalHostInfo();
        }

        /// <summary>
        /// 根据新的SessionInfo初始化SessionInfo（通常在SendLoadingMessage之后，调用此方法更新SessionInfo）
        /// </summary>
        /// <param name="newSession"></param>
        public void SetSessionInfo(SessionInfo newSession)
        {
            if (newSession == null) { return; }
            if (newSession.DatabaseInfo != null)
            {
                DatabaseInfo = newSession.DatabaseInfo;
                DBType = DatabaseInfo.TypeID;
                DBConnectionString = newSession.DBConnectionString;
            }
            if (newSession.UserInfo != null)
            {
                UserInfo = newSession.UserInfo;
                UserID = UserInfo.UserID;
            }
            if (newSession.RoleInfo != null)
            {
                RoleInfo = newSession.RoleInfo;
                RoleID = RoleInfo.ID;
            }
            if (newSession.LoginInfo != null)
            {
                LoginInfo = newSession.LoginInfo;
                LoginID = LoginInfo.LoginID;
            }
            if (newSession.DomainInfo != null)
            {
                DomainInfo = newSession.DomainInfo;
                DomainID = DomainInfo.DomainID;
            }
            if (newSession.OrgInfo != null)
            {
                OrgInfo = newSession.OrgInfo;
                OrgID = OrgInfo.OrgID;
            }
            if (newSession.ThemeInfo != null)
            {
                ThemeInfo = newSession.ThemeInfo;
                ThemeName = ThemeInfo.Name;
            }
            if (newSession.LangTypeInfo != null)
            {
                LangTypeInfo = newSession.LangTypeInfo;
                LangTypeID = LangTypeInfo.LangID;
            }
            if (newSession.AppServerInfo != null)
            {
                AppServerInfo = newSession.AppServerInfo;
            }
            if (newSession.LocalMachineInfo != null)
            {
                LocalMachineInfo = newSession.LocalMachineInfo;
            }
            if (newSession.SupportLangTypes != null)
            {
                SupportLangTypes = newSession.SupportLangTypes;
            }
            if (newSession.SupportThemes != null)
            {
                SupportThemes = newSession.SupportThemes;
            }
            if (!string.IsNullOrEmpty(newSession.InstallPath))
            {
                InstallPath = newSession.InstallPath;
            }
            if (newSession.RentInfo != null)
            {
                RentInfo = newSession.RentInfo;
                RentID = RentInfo.ID;
            }
            if (newSession.ListPartitionTables != null)
            {
                ListPartitionTables = newSession.ListPartitionTables;
            }
            if (newSession.ListGlobalSettings != null)
            {
                mListGlobalSettings = newSession.ListGlobalSettings;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]({2})[{3},{4}]",
                AppName,
                SessionID,
                IsMonitor,
                AppServerInfo,
                DatabaseInfo);
        }

        /// <summary>
        /// 创建一个默认SessionInfo对象
        /// </summary>
        /// <returns></returns>
        public static SessionInfo CreateSessionInfo()
        {
            return CreateSessionInfo(string.Empty, 0);
        }

        /// <summary>
        /// 指定AppName和ModuleID创建一个SessionInfo对象
        /// </summary>
        /// <param name="appName">AppName</param>
        /// <param name="moduleID">ModuleID</param>
        /// <returns></returns>
        public static SessionInfo CreateSessionInfo(string appName, int moduleID)
        {
            return CreateSessionInfo(appName, moduleID, (int)Common.AppType.Unkown);
        }

        /// <summary>
        /// 指定AppName,ModuleID和AppType创建一个SessionInfo对象
        /// </summary>
        /// <param name="appName">AppName</param>
        /// <param name="moduleID">ModuleID</param>
        /// <param name="appType">AppType</param>
        /// <returns></returns>
        public static SessionInfo CreateSessionInfo(string appName, int moduleID, int appType)
        {
            SessionInfo session = new SessionInfo();
            session.SessionID = Guid.NewGuid().ToString();
            session.AppName = appName;
            session.ModuleID = moduleID;
            session.AppType = appType;
            session.LastActiveTime = DateTime.Now.ToUniversalTime();
            return session;
        }

        /// <summary>
        /// 记录SessionInfo的详细信息
        /// </summary>
        /// <returns></returns>
        public string LogInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("\tSessionID:{0}\r\n", SessionID));
            sb.Append(string.Format("\tAppName:{0}\r\n", AppName));
            sb.Append(string.Format("\tModuleID:{0}\r\n", ModuleID));
            sb.Append(string.Format("\tAppType:{0}\r\n", (AppType)AppType));
            sb.Append(string.Format("\tLastActiveTime:{0}\r\n", LastActiveTime));
            sb.Append(string.Format("\tIsMonitor:{0}\r\n", IsMonitor));
            sb.Append(string.Format("\tInstallPath:{0}\r\n", InstallPath));
            sb.Append(string.Format("\tAppVersion:{0}\r\n", AppVersion));
            sb.Append(string.Format("\tDatabase:{0}\r\n", DatabaseInfo));
            sb.Append(string.Format("\tAppServerInfo:{0}\r\n", AppServerInfo));
            sb.Append(string.Format("\tRentInfo:{0}\r\n", RentInfo));
            sb.Append(string.Format("\tRoleInfo:{0}\r\n", RoleInfo));
            sb.Append(string.Format("\tUserInfo:{0}\r\n", UserInfo));
            sb.Append(string.Format("\tLoignInfo:{0}\r\n", LoginInfo));
            sb.Append(string.Format("\tDomainInfo:{0}\r\n", DomainInfo));
            sb.Append(string.Format("\tOrgInfo:{0}\r\n", OrgInfo));
            sb.Append(string.Format("\tThemeInfo:{0}\r\n", ThemeInfo));
            sb.Append(string.Format("\tLanguageTypeInfo:{0}\r\n", LangTypeInfo));
            StringBuilder sbGlobal = new StringBuilder();
            for (int i = 0; i < mListGlobalSettings.Count; i++)
            {
                sbGlobal.Append(string.Format("\t\t{0}:{1}\r\n", mListGlobalSettings[i].Key,
                    mListGlobalSettings[i].Value));
            }
            string strGlobal = sbGlobal.ToString().TrimEnd('\r', '\n');
            sb.Append(string.Format("\tGlobalSettingInfo:\r\n"));
            sb.Append(strGlobal);
            return sb.ToString();
        }

        /// <summary>
        /// 通过键名获取或设置全局设定参数值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                //从列表中查找指定名称的参数
                var setting = mListGlobalSettings.FirstOrDefault(s => s.Key == key);
                if (setting == null)
                {
                    return string.Empty;
                }
                return setting.Value;
            }
            set
            {
                //从列表中查找指定名称的参数，如果不存在就创建一个新的全局参数
                var setting = mListGlobalSettings.FirstOrDefault(s => s.Key == key);
                if (setting == null)
                {
                    setting = new GlobalSetting();
                    setting.Key = key;
                    mListGlobalSettings.Add(setting);
                }
                setting.Value = value;
            }
        }

    }
}
