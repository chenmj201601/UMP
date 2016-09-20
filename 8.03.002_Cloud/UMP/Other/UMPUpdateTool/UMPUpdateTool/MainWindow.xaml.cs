using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMPUpdateTool.Models;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdateTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private DatabaseInfo mDatabaseInfo;
        private string mDBConnectionString;
        private ObservableCollection<BugItem> mListBugItems;
        private List<UpdateModule> mListBugInfos;
        private ObservableCollection<ComboItem> mListVersionsItems;
        private ObservableCollection<ComboItem> mListBugTypeItems;
        private ObservableCollection<ComboItem> mListModuleItems;
        private ObservableCollection<ComboItem> mListDBTypeItems;
        private ComboItem mCurrentVersion;
        private BugItem mCurrentBugItem;
        private UpdateConfig mUpdateConfig;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListBugItems = new ObservableCollection<BugItem>();
            mListBugInfos = new List<UpdateModule>();
            mListVersionsItems = new ObservableCollection<ComboItem>();
            mListBugTypeItems = new ObservableCollection<ComboItem>();
            mListModuleItems = new ObservableCollection<ComboItem>();
            mListDBTypeItems = new ObservableCollection<ComboItem>();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnReload.Click += BtnReload_Click;
            BtnAdd.Click += BtnAdd_Click;
            BtnModify.Click += BtnModify_Click;
            LvBugList.SelectionChanged += LvBugList_SelectionChanged;
            CbToday.Click += CbToday_Click;
            BtnSetting.Click += BtnSetting_Click;
            ComboVersions.SelectionChanged += ComboVersions_SelectionChanged;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboDBType.ItemsSource = mListDBTypeItems;
                ComboVersions.ItemsSource = mListVersionsItems;
                LvBugList.ItemsSource = mListBugItems;
                ComboBugTypes.ItemsSource = mListBugTypeItems;
                ComboModules.ItemsSource = mListModuleItems;

                InitDBTypeItems();
                InitVersionItems();
                InitBugTypeItems();
                LoadUpdateConfig();
                InitModuleItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadBugInfoList();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    ComboVersions.SelectedItem = mCurrentVersion;
                    CreateBugItems();
                    ShowDatabaseInfos();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Init and Load

        private void LoadUpdateConfig()
        {
            try
            {
                string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateConfig.FILE_NAME);
                if (!File.Exists(strPath))
                {
                    ShowException(string.Format("UpdateConfig file not exist.\t{0}", strPath));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<UpdateConfig>(strPath);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                var config = optReturn.Data as UpdateConfig;
                if (config == null)
                {
                    ShowException(string.Format("UpdateConfig is null"));
                    return;
                }
                mUpdateConfig = config;
                mDatabaseInfo = mUpdateConfig.DatabaseInfo;
                if (mDatabaseInfo != null)
                {
                    mDatabaseInfo.RealPassword = mDatabaseInfo.Password;
                    mDBConnectionString = mDatabaseInfo.GetConnectionString();
                }
                mCurrentVersion = mListVersionsItems.FirstOrDefault(v => v.StrValue == mUpdateConfig.CurrentVersion);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitVersionItems()
        {
            try
            {
                mListVersionsItems.Clear();

                ComboItem item = new ComboItem();
                item.Name = "803001P02000";
                item.Display = "8.03.001P02.000";
                item.StrValue = "803001P02000";
                mListVersionsItems.Add(item);
                item = new ComboItem();
                item.Name = "803001P03000";
                item.Display = "8.03.001P03.000";
                item.StrValue = "803001P03000";
                mListVersionsItems.Add(item);
                item = new ComboItem();
                item.Name = "803001P04000";
                item.Display = "8.03.001P04.000";
                item.StrValue = "803001P04000";
                mListVersionsItems.Add(item);
                item = new ComboItem();
                item.Name = "803001P05000";
                item.Display = "8.03.001P05.000";
                item.StrValue = "803001P05000";
                mListVersionsItems.Add(item);
                item = new ComboItem();
                item.Name = "803002P00000";
                item.Display = "8.03.002P00.000";
                item.StrValue = "803002P00000";
                mListVersionsItems.Add(item);

                AppendMessage(string.Format("InitVersionItems end.\t{0}", mListVersionsItems.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitDBTypeItems()
        {
            try
            {
                mListDBTypeItems.Clear();

                ComboItem item = new ComboItem();
                item.Name = "MSSQL";
                item.Display = "SQL Server";
                item.IntValue = 2;
                mListDBTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "ORCL";
                item.Display = "Oracle";
                item.IntValue = 3;
                mListDBTypeItems.Add(item);

                AppendMessage(string.Format("InitDBTypeItems end.\t{0}", mListDBTypeItems));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBugTypeItems()
        {
            try
            {
                mListBugTypeItems.Clear();

                ComboItem item = new ComboItem();
                item.Name = "Increase";
                item.Display = "Increase";
                item.IntValue = 1;
                mListBugTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Improvement";
                item.Display = "Improvement";
                item.IntValue = 2;
                mListBugTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Bug";
                item.Display = "Bug";
                item.IntValue = 3;
                mListBugTypeItems.Add(item);

                AppendMessage(string.Format("InitBugTypeItems end.\t{0}", mListBugTypeItems.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitModuleItems()
        {
            try
            {
                mListModuleItems.Clear();

                //ComboItem item = new ComboItem();
                //item.Name = "Basic";
                //item.Display = "Basic";
                //item.IntValue = 0;
                //mListModuleItems.Add(item);
                //item = new ComboItem();
                //item.Name = "Query";
                //item.Display = "Query";
                //item.IntValue = 3102;
                //mListModuleItems.Add(item);
                //item = new ComboItem();
                //item.Name = "Resource";
                //item.Display = "Resource";
                //item.IntValue = 1110;
                //mListModuleItems.Add(item);

                if (mUpdateConfig == null) { return; }
                for (int i = 0; i < mUpdateConfig.ListModuleInfos.Count; i++)
                {
                    var module = mUpdateConfig.ListModuleInfos[i];
                    ComboItem item = new ComboItem();
                    item.Name = module.ModuleName;
                    item.Display = module.Display;
                    item.IntValue = module.ModuleID;
                    item.Data = item;
                    mListModuleItems.Add(item);
                }

                AppendMessage(string.Format("InitModuleItems end.\t{0}", mListModuleItems.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadBugInfoList()
        {
            try
            {
                mListBugInfos.Clear();
                if (mDatabaseInfo == null) { return; }
                if (mCurrentVersion == null) { return; }
                string strVersion = mCurrentVersion.StrValue;
                string strSql;
                OperationReturn optReturn;
                switch (mDatabaseInfo.TypeID)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE C001 LIKE '{0}%' ORDER BY C001",
                            strVersion);
                        optReturn = MssqlOperation.GetDataSet(mDBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE C001 LIKE '{0}%' ORDER BY C001",
                            strVersion);
                        optReturn = OracleOperation.GetDataSet(mDBConnectionString, strSql);
                        break;
                    default:
                        ShowException(string.Format("DBType invalid."));
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
                    ShowException(string.Format("Fail. DataSet is null"));
                    return;
                }
                int count = objDataSet.Tables[0].Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    UpdateModule info = new UpdateModule();
                    info.SerialNo = dr["C001"].ToString();
                    info.Type = Convert.ToInt32(dr["C002"]);
                    info.ModuleID = Convert.ToInt32(dr["C003"]);
                    info.ModuleName = dr["C004"].ToString();
                    info.OptDate = dr["C005"].ToString();
                    info.Level = Convert.ToInt32(dr["C006"]);
                    info.Content = dr["C007"].ToString();
                    info.LangID = dr["C008"].ToString();
                    info.ModuleLangID = dr["C009"].ToString();
                    mListBugInfos.Add(info);
                }

                AppendMessage(string.Format("LoadBugInfoList end.\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateBugItems()
        {
            try
            {
                mListBugItems.Clear();
                int count = 0;
                for (int i = 0; i < mListBugInfos.Count; i++)
                {
                    var info = mListBugInfos[i];

                    BugItem item = new BugItem();
                    item.SerialNo = info.SerialNo;
                    item.Type = info.Type;
                    item.StrType = info.Type == 1 ? "Increase" : info.Type == 2 ? "Improvement" : "Bug";
                    item.ModuleID = info.ModuleID;
                    item.ModuleName = info.ModuleName;
                    item.UpdateDate = GetUpdateDate(info.OptDate);
                    item.StrUpdateDate = item.UpdateDate.ToString("yyyy-MM-dd");
                    item.Level = info.Level;
                    item.Content = info.Content;
                    item.ContentLangID = info.LangID;
                    item.ModuleLangID = info.ModuleLangID;
                    item.Info = info;

                    count++;
                    mListBugItems.Add(item);
                }

                AppendMessage(string.Format("CreateBugItems end.\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Show Information

        private void ShowDatabaseInfos()
        {
            try
            {
                if (mDatabaseInfo == null) { return; }
                var dbTypeItem = mListDBTypeItems.FirstOrDefault(d => d.IntValue == mDatabaseInfo.TypeID);
                ComboDBType.SelectedItem = dbTypeItem;
                TxtDBHost.Text = mDatabaseInfo.Host;
                TxtDBPort.Text = mDatabaseInfo.Port.ToString();
                TxtDBName.Text = mDatabaseInfo.DBName;
                TxtDBUser.Text = mDatabaseInfo.LoginName;
                TxtDBPassword.Password = mDatabaseInfo.Password;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowBugItemInfo()
        {
            try
            {
                if (mCurrentBugItem == null) { return; }
                var comboItem = mListBugTypeItems.FirstOrDefault(m => m.IntValue == mCurrentBugItem.Type);
                ComboBugTypes.SelectedItem = comboItem;
                comboItem = mListModuleItems.FirstOrDefault(m => m.IntValue == mCurrentBugItem.ModuleID);
                ComboModules.SelectedItem = comboItem;
                TxtUpdateDate.Text = mCurrentBugItem.UpdateDate.ToString("yyyy-MM-dd");
                CbToday.IsChecked = false;
                TxtLevel.Text = mCurrentBugItem.Level.ToString();
                TxtContent.Text = mCurrentBugItem.Content;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private int GetBugSerialNoIndex()
        {
            int index = 0;
            try
            {
                var infos = mListBugInfos.OrderBy(b => b.SerialNo).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    string strSerialNo = info.SerialNo;
                    if (strSerialNo.Length >= 12)
                    {
                        string strIndex = strSerialNo.Substring(12);
                        int temp;
                        if (int.TryParse(strIndex, out temp))
                        {
                            index = Math.Max(index, temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return index;
        }

        private int GetBugLangIDIndex(int moduleID)
        {
            int index = 0;
            try
            {
                var infos = mListBugInfos.Where(b => b.ModuleID == moduleID).OrderBy(b => b.LangID).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    string strLangID = info.LangID;
                    if (strLangID.Length >= 6)
                    {
                        string strIndex = strLangID.Substring(6);
                        int temp;
                        if (int.TryParse(strIndex, out temp))
                        {
                            index = Math.Max(index, temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return index;
        }

        private DateTime GetUpdateDate(string strDate)
        {
            DateTime dtValue = DateTime.MinValue;
            try
            {
                if (strDate.Length >= 8)
                {
                    string year = strDate.Substring(0, 4);
                    string month = strDate.Substring(4, 2);
                    string day = strDate.Substring(6, 2);
                    dtValue = DateTime.Parse(string.Format("{0}-{1}-{2} 00:00:00", year, month, day));
                }
            }
            catch { }
            return dtValue;
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, "UMPUpdateTool", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion


        #region Operations

        private void AddBugInfos()
        {
            try
            {
                if (mCurrentVersion == null) { return; }
                UpdateModule info = new UpdateModule();
                int serialNoIndex = GetBugSerialNoIndex();
                serialNoIndex++;
                info.SerialNo = string.Format("{0}{1}", mCurrentVersion.StrValue, serialNoIndex.ToString("0000"));
                var bugType = ComboBugTypes.SelectedItem as ComboItem;
                if (bugType == null)
                {
                    ShowException(string.Format("Bug Type invalid!"));
                    return;
                }
                info.Type = bugType.IntValue;
                var module = ComboModules.SelectedItem as ComboItem;
                if (module == null)
                {
                    ShowException(string.Format("Module invalid!"));
                    return;
                }
                info.ModuleID = module.IntValue;
                info.ModuleName = module.Name;
                DateTime dtValue;
                if (!DateTime.TryParse(TxtUpdateDate.Text + " 00:00:00", out dtValue))
                {
                    ShowException(string.Format("UpdateDate invalid!"));
                    return;
                }
                info.OptDate = dtValue.ToString("yyyyMMdd");
                int intValue;
                if (!int.TryParse(TxtLevel.Text, out intValue)
                    || intValue < 0
                    || intValue > 10)
                {
                    ShowException(string.Format("Level invalid!"));
                    return;
                }
                info.Level = intValue;
                info.Content = TxtContent.Text;
                int langIDIndex = GetBugLangIDIndex(info.ModuleID);
                langIDIndex++;
                info.LangID = string.Format("MC{0}{1}", info.ModuleID.ToString("0000"), langIDIndex.ToString("0000"));
                info.ModuleLangID = string.Format("M{0}", info.ModuleID.ToString("0000"));

                if (mDatabaseInfo == null) { return; }
                bool bIsSucess = false;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        string strConn = mDatabaseInfo.GetConnectionString();
                        int dbType = mDatabaseInfo.TypeID;
                        string strSql;
                        IDbConnection objConn;
                        IDbDataAdapter objAdapter;
                        DbCommandBuilder objCmdBuilder;
                        switch (dbType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE 1 = 2");
                                objConn = MssqlOperation.GetConnection(strConn);
                                objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE 1 = 2");
                                objConn = OracleOperation.GetConnection(strConn);
                                objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                break;
                            default:
                                ShowException(string.Format("DatabaseType invalid!"));
                                bIsSucess = false;
                                return;
                        }
                        if (objConn == null || objAdapter == null || objCmdBuilder == null)
                        {
                            ShowException(string.Format("DBObject is null"));
                            bIsSucess = false;
                            return;
                        }
                        objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        objCmdBuilder.SetAllValues = false;
                        try
                        {
                            DataSet objDataSet = new DataSet();
                            objAdapter.Fill(objDataSet);

                            DataRow dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = info.SerialNo;
                            dr["C002"] = info.Type;
                            dr["C003"] = info.ModuleID;
                            dr["C004"] = info.ModuleName;
                            dr["C005"] = info.OptDate;
                            dr["C006"] = info.Level;
                            dr["C007"] = info.Content;
                            dr["C008"] = info.LangID;
                            dr["C009"] = info.ModuleLangID;
                            dr["C010"] = mCurrentVersion.Display;

                            objDataSet.Tables[0].Rows.Add(dr);

                            objAdapter.Update(objDataSet);
                            objDataSet.AcceptChanges();

                            bIsSucess = true;
                        }
                        catch (Exception ex)
                        {
                            ShowException(string.Format("Fail.\t{0}", ex.Message));
                            bIsSucess = false;
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
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (bIsSucess)
                    {
                        AppendMessage(string.Format("Add end.\t{0}", info.SerialNo));

                        ReloaddBugInfos();
                    }
                };
                worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyBugInfos()
        {
            try
            {
                var item = LvBugList.SelectedItem as BugItem;
                if (item == null) { return; }
                var info = item.Info;
                if (info == null) { return; }
                var bugType = ComboBugTypes.SelectedItem as ComboItem;
                if (bugType == null)
                {
                    ShowException(string.Format("Bug Type invalid!"));
                    return;
                }
                info.Type = bugType.IntValue;
                DateTime dtValue;
                if (!DateTime.TryParse(TxtUpdateDate.Text + " 00:00:00", out dtValue))
                {
                    ShowException(string.Format("UpdateDate invalid!"));
                    return;
                }
                info.OptDate = dtValue.ToString("yyyyMMdd");
                int intValue;
                if (!int.TryParse(TxtLevel.Text, out intValue)
                    || intValue < 0
                    || intValue > 10)
                {
                    ShowException(string.Format("Level invalid!"));
                    return;
                }
                info.Level = intValue;
                info.Content = TxtContent.Text;

                if (mDatabaseInfo == null) { return; }
                bool bIsSucess = false;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        string strConn = mDatabaseInfo.GetConnectionString();
                        int dbType = mDatabaseInfo.TypeID;
                        string strSql;
                        IDbConnection objConn;
                        IDbDataAdapter objAdapter;
                        DbCommandBuilder objCmdBuilder;
                        switch (dbType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE C001 = '{0}'", info.SerialNo);
                                objConn = MssqlOperation.GetConnection(strConn);
                                objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_UPDATE_LIST WHERE C001 = '{0}'", info.SerialNo);
                                objConn = OracleOperation.GetConnection(strConn);
                                objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                break;
                            default:
                                ShowException(string.Format("DatabaseType invalid!"));
                                bIsSucess = false;
                                return;
                        }
                        if (objConn == null || objAdapter == null || objCmdBuilder == null)
                        {
                            ShowException(string.Format("DBObject is null"));
                            bIsSucess = false;
                            return;
                        }
                        objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        objCmdBuilder.SetAllValues = false;
                        try
                        {
                            DataSet objDataSet = new DataSet();
                            objAdapter.Fill(objDataSet);

                            if (objDataSet.Tables[0].Rows.Count <= 0)
                            {
                                ShowException(string.Format("Fail.\tDataRow not exist.\t{0}", info.SerialNo));
                                return;
                            }
                            DataRow dr = objDataSet.Tables[0].Rows[0];
                            dr["C001"] = info.SerialNo;
                            dr["C002"] = info.Type;
                            dr["C003"] = info.ModuleID;
                            dr["C004"] = info.ModuleName;
                            dr["C005"] = info.OptDate;
                            dr["C006"] = info.Level;
                            dr["C007"] = info.Content;
                            dr["C008"] = info.LangID;
                            dr["C009"] = info.ModuleLangID;
                            dr["C010"] = mCurrentVersion.Display;

                            objAdapter.Update(objDataSet);
                            objDataSet.AcceptChanges();

                            bIsSucess = true;
                        }
                        catch (Exception ex)
                        {
                            ShowException(string.Format("Fail.\t{0}", ex.Message));
                            bIsSucess = false;
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
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (bIsSucess)
                    {
                        AppendMessage(string.Format("Modify end.\t{0}", info.SerialNo));

                        ReloaddBugInfos();
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ReloaddBugInfos()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, de) =>
            {
                LoadBugInfoList();
            };
            worker.RunWorkerCompleted += (s, re) =>
            {
                worker.Dispose();

                CreateBugItems();
            };
            worker.RunWorkerAsync();
        }

        private void SaveSettings()
        {
            try
            {
                DatabaseInfo dbInfo = new DatabaseInfo();
                var dbTypeItem = ComboDBType.SelectedItem as ComboItem;
                if (dbTypeItem == null)
                {
                    ShowException(string.Format("DBType invalid!"));
                    return;
                }
                dbInfo.TypeID = dbTypeItem.IntValue;
                if (string.IsNullOrEmpty(TxtDBHost.Text))
                {
                    ShowException(string.Format("DBHost invalid!"));
                    return;
                }
                dbInfo.Host = TxtDBHost.Text;
                int intValue;
                if (string.IsNullOrEmpty(TxtDBPort.Text)
                    || !int.TryParse(TxtDBPort.Text, out intValue))
                {
                    ShowException(string.Format("DBPort invalid!"));
                    return;
                }
                dbInfo.Port = intValue;
                if (string.IsNullOrEmpty(TxtDBName.Text))
                {
                    ShowException(string.Format("DBName invalid!"));
                    return;
                }
                dbInfo.DBName = TxtDBName.Text;
                if (string.IsNullOrEmpty(TxtDBUser.Text))
                {
                    ShowException(string.Format("DBUser Invalid!"));
                    return;
                }
                dbInfo.LoginName = TxtDBUser.Text;
                if (string.IsNullOrEmpty(TxtDBPassword.Password))
                {
                    ShowException(string.Format("Password invalid!"));
                    return;
                }
                dbInfo.Password = TxtDBPassword.Password;
                dbInfo.RealPassword = dbInfo.Password;


                mDatabaseInfo = dbInfo;
                mDBConnectionString = dbInfo.GetConnectionString();
                if (mUpdateConfig == null) { return; }
                mUpdateConfig.DatabaseInfo = mDatabaseInfo;
                mUpdateConfig.CurrentVersion = mCurrentVersion.StrValue;

                string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateConfig.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mUpdateConfig, strPath);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                AppendMessage(string.Format("End.\t{0}", strPath));
                ReloaddBugInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region 生成配置文件

                //UpdateConfig config = new UpdateConfig();
                //config.CurrentVersion = "803001P02000";
                //ModuleInfo module = new ModuleInfo();
                //module.ModuleID = 3102;
                //module.MasterID = 31;
                //module.ModuleName = "Query";
                //module.Display = "查询";
                //config.ListModuleInfos.Add(module);

                //string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateConfig.FILE_NAME);
                //OperationReturn optReturn = XMLHelper.SerializeFile(config, strPath);
                //if (!optReturn.Result)
                //{
                //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}

                //AppendMessage(string.Format("End.\t{0}", strPath));

                #endregion


                #region 导出语言包

                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.TypeID = 3;
                dbInfo.Host = "192.168.4.182";
                dbInfo.Port = 1521;
                dbInfo.DBName = "PFOrcl";
                dbInfo.LoginName = "PFDEV832";
                dbInfo.Password = "pfdev832";
                dbInfo.RealPassword = dbInfo.Password;

                List<int> listLangIDs = new List<int>();
                listLangIDs.Add(1033);
                listLangIDs.Add(1028);
                listLangIDs.Add(1041);
                listLangIDs.Add(2052);

                for (int i = 0; i < listLangIDs.Count; i++)
                {
                    int langTypeID = listLangIDs[i];
                    string strLangName = langTypeID.ToString();
                    string strConn = dbInfo.GetConnectionString();
                    string strSql;
                    OperationReturn optReturn;
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002", langTypeID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002", langTypeID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            break;
                        default:
                            AppendMessage(string.Format("Fail.\t DBType invalid"));
                            continue;
                    }
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    DataSet objDataSet = optReturn.Data as DataSet;
                    if (objDataSet == null
                        || objDataSet.Tables.Count <= 0)
                    {
                        AppendMessage(string.Format("DataSet is null or DataTable not exist."));
                        continue;
                    }
                    LangLister lister = new LangLister();
                    lister.LangID = langTypeID;
                    lister.LangName = strLangName;
                    lister.Path = string.Format("{0}.XML", langTypeID);
                    int count = objDataSet.Tables[0].Rows.Count;
                    for (int j = 0; j < count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        LanguageInfo langInfo = new LanguageInfo();
                        langInfo.LangID = langTypeID;
                        langInfo.Name = dr["C002"].ToString();
                        langInfo.Module = Convert.ToInt32(dr["C009"]);
                        langInfo.SubModule = Convert.ToInt32(dr["C010"]);
                        langInfo.Page = dr["C011"].ToString();
                        langInfo.ObjName = dr["C012"].ToString();
                        string display = string.Empty;
                        display += dr["C005"].ToString();
                        display += dr["C006"].ToString();
                        display += dr["C007"].ToString();
                        display += dr["C008"].ToString();
                        langInfo.Display = display;

                        lister.ListLangInfos.Add(langInfo);
                    }

                    string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, lister.Path);
                    optReturn = XMLHelper.SerializeFile(lister, strPath);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    AppendMessage(string.Format("End.\t{0}\t{1}", strPath, count));
                }
                AppendMessage(string.Format("End."));

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddBugInfos();
        }

        void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            ReloaddBugInfos();
        }

        void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            ModifyBugInfos();
        }

        void LvBugList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = LvBugList.SelectedItem as BugItem;
                if (item == null) { return; }
                mCurrentBugItem = item;
                ShowBugItemInfo();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void CbToday_Click(object sender, RoutedEventArgs e)
        {
            if (CbToday.IsChecked == true)
            {
                TxtUpdateDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        void ComboVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count <= 0) { return; }
            var item = ComboVersions.SelectedItem as ComboItem;
            if (item == null) { return; }
            mCurrentVersion = item;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, de) =>
            {
                LoadBugInfoList();
            };
            worker.RunWorkerCompleted += (s, re) =>
            {
                worker.Dispose();

                CreateBugItems();
                ShowDatabaseInfos();
            };
            worker.RunWorkerAsync();
        }

        #endregion

    }
}
