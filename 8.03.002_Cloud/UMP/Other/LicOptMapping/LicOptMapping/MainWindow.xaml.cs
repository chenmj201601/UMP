using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;

namespace LicOptMapping
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow 
    {
        private SessionInfo mSession;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitSessionInfo();
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mSession == null) { return; }
                var dbInfo = mSession.DatabaseInfo;
                if (dbInfo == null) { return; }
                int dbType = dbInfo.TypeID;
                OperationReturn optReturn;
                List<LicModel> listLicModels = new List<LicModel>();
                string strSql;
                string strConn = dbInfo.GetConnectionString();
                switch (dbType)
                {
                    case 2:
                        //strSql = string.Format("SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, A.C004 AS AC004, A.C016 AS AC016, B.C005 AS BC005 FROM T_11_003_00000 A, T_00_005 B WHERE 'FO'+ A.C002 = B.C002 AND A.C016 > 0  AND B.C001 = '1033' ORDER BY A.C001,A.C002");
                        strSql = string.Format("SELECT * FROM T_11_003_00000 WHERE C016 > 0 ORDER BY C002");
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        //strSql = string.Format("SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, A.C004 AS AC004, A.C016 AS AC016, B.C005 AS BC005 FROM T_11_003_00000 A, T_00_005 B WHERE 'FO'|| A.C002 = B.C002 AND A.C016 > 0  AND B.C001 = '1033' ORDER BY A.C001,A.C002");
                        strSql = string.Format("SELECT * FROM T_11_003_00000 WHERE C016 > 0  ORDER BY C002");
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        break;
                    default:
                        ShowException(string.Format("DBType invalid"));
                        return;
                }
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    ShowException(string.Format("DataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    LicModel info = new LicModel();
                    //info.MasterID = Convert.ToInt32(dr["AC001"]);
                    //info.OptID = Convert.ToInt64(dr["AC002"]);
                    //info.ParentID = Convert.ToInt64(dr["AC003"]);
                    //info.SortID = Convert.ToInt32(dr["AC004"]);
                    //info.LicNo = Convert.ToInt32(dr["AC016"]);
                    //info.LicID = 1000000000 + info.OptID;
                    //info.OptName = dr["BC005"].ToString();
                    info.MasterID = Convert.ToInt32(dr["C001"]);
                    info.OptID = Convert.ToInt64(dr["C002"]);
                    info.ParentID = Convert.ToInt64(dr["C003"]);
                    long parentID = info.ParentID;
                    if (parentID > 9999)
                    {
                        parentID = int.Parse(parentID.ToString().Substring(0, 4));
                        info.ParentID = parentID;
                    }
                    info.SortID = Convert.ToInt32(dr["C004"]);
                    info.LicNo = Convert.ToInt32(dr["C016"]);
                    info.LicID = 1000000000 + info.OptID;
                    info.OptName = dr["C017"].ToString();
                    listLicModels.Add(info);
                }
                LicDefineFile licFile = new LicDefineFile();
                licFile.LicID = 1100101;
                for (int i = 0; i < listLicModels.Count; i++)
                {
                    licFile.ListLics.Add(listLicModels[i]);
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicDefineFile.FILE_NAME);
                optReturn = XMLHelper.SerializeFile(licFile, path);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                MessageBox.Show("End", "Demo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSessionInfo()
        {
            mSession = SessionInfo.CreateSessionInfo("Demo", 0, (int)AppType.UMPClient);
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

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            mSession.ThemeInfo = themeInfo;
            mSession.ThemeName = "Style01";

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

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.63";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            mSession.AppServerInfo = appServerInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0304";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //mSession.DatabaseInfo = dbInfo;
            //mSession.DBType = dbInfo.TypeID;
            //mSession.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 3;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1521;
            dbInfo.DBName = "PFOrcl";
            dbInfo.LoginName = "pfdev832";
            dbInfo.Password = "pfdev832";
            dbInfo.RealPassword = "pfdev832";
            mSession.DatabaseInfo = dbInfo;
            mSession.DBType = dbInfo.TypeID;
            mSession.DBConnectionString = dbInfo.GetConnectionString();
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, "Demo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [XmlRoot(ElementName = "LicDefine")]
    public class LicModel
    {
        [XmlAttribute]
        public int MasterID { get; set; }
        [XmlAttribute]
        public long OptID { get; set; }
        [XmlAttribute]
        public long ParentID { get; set; }
        [XmlAttribute]
        public int SortID { get; set; }
        [XmlAttribute]
        public int LicNo { get; set; }
        [XmlAttribute]
        public long LicID { get; set; }
        [XmlAttribute]
        public string OptName { get; set; }
    }

    [XmlRoot(ElementName = "LicCollection")]
    public class LicDefineFile
    {
        public const string FILE_NAME = "LicDefine_1100101.xml";

        [XmlAttribute]
        public long LicID { get; set; }

        private List<LicModel> mListLics;
        [XmlArray(ElementName = "Lics")]
        public List<LicModel> ListLics
        {
            get { return mListLics; }
        }

        public LicDefineFile()
        {
            mListLics = new List<LicModel>();
        }
    }
}
