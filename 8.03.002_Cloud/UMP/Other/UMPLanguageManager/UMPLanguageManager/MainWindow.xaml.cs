using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using UMPLanguageManager.Commands;
using UMPLanguageManager.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;

namespace UMPLanguageManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private ObservableCollection<LangTypeInfo> mListLangTypeItems;
        private ObservableCollection<LanguageItem> mListLanguageItems;
        private List<FilterItem> mListFilterItems;
        private LangConfigInfo mConfigInfo;
        private FilterItem mRootFilterModule;
        private FilterItem mRootFilterPrefix;
        private FilterItem mRootFilterCategory;
        private DatabaseInfo mDatabaseInfo;
        private BackgroundWorker mWorker;
        private ConditionInfo mConditionInfo;
        private LangFileInfo mLangFilgeInfo;
        private List<LanguageItem> mListSearchedItems;
        private List<LanguageInfo> mListOptLangInfos;   //待处理（同步，导出等）的语言列表
        private bool mIsLangChanged;
        private bool mIsOptSuccess;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListLangTypeItems = new ObservableCollection<LangTypeInfo>();
            mListLanguageItems = new ObservableCollection<LanguageItem>();
            mListSearchedItems = new List<LanguageItem>();
            mListOptLangInfos = new List<LanguageInfo>();
            mListFilterItems = new List<FilterItem>();
            mRootFilterModule = new FilterItem();
            mRootFilterPrefix = new FilterItem();
            mRootFilterCategory = new FilterItem();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            InRibbonLanuguageType.SelectionChanged += InRibbonLanuguageType_SelectionChanged;
            TvModuleList.SelectedItemChanged += TvModuleList_SelectedItemChanged;
            TvPrefixList.SelectedItemChanged += TvPrefixList_SelectedItemChanged;
            TvCategoryList.SelectedItemChanged += TvCategoryList_SelectedItemChanged;
            TxtSearch.KeyDown += TxtSearch_KeyDown;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InRibbonLanuguageType.ItemsSource = mListLangTypeItems;
            TvModuleList.ItemsSource = mRootFilterModule.Children;
            TvPrefixList.ItemsSource = mRootFilterPrefix.Children;
            TvCategoryList.ItemsSource = mRootFilterCategory.Children;
            LvLanguageItems.ItemsSource = mListLanguageItems;

            BindCommands();
            LoadSettingInfos();
            LoadFilterInfos();
            LoadLayoutInfos();
            InitLangTypeItems();
            InitFilterItems();
            InitDatabaseInfo();
            SetViewStatus();

            mIsLangChanged = false;
            mIsOptSuccess = true;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {

        }


        #region Init and load

        private void InitLangTypeItems()
        {
            try
            {
                mListLangTypeItems.Clear();
                LangTypeInfo info = new LangTypeInfo();
                info.LangID = 1033;
                info.LangName = "en-us";
                info.Display = "English";
                mListLangTypeItems.Add(info);
                info = new LangTypeInfo();
                info.LangID = 2052;
                info.LangName = "zh-cn";
                info.Display = "简体中文";
                mListLangTypeItems.Add(info);
                info = new LangTypeInfo();
                info.LangID = 1028;
                info.LangName = "zh-tw";
                info.Display = "繁体中文";
                mListLangTypeItems.Add(info);
                info = new LangTypeInfo();
                info.LangID = 1041;
                info.LangName = "jap";
                info.Display = "日本语";
                mListLangTypeItems.Add(info);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadSettingInfos()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigInfo.xml");
                if (!File.Exists(path))
                {
                    ShowErrorMessage(string.Format("Config file not exist"));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<LangConfigInfo>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                LangConfigInfo configInfo = optReturn.Data as LangConfigInfo;
                if (configInfo == null)
                {
                    ShowErrorMessage(string.Format("Fail.\tConfigInfo is null"));
                    return;
                }
                mConfigInfo = configInfo;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadFilterInfos()
        {
            try
            {
                if (mConfigInfo == null) { return; }
                if (mConfigInfo.ListFilterInfos != null)
                {
                    mListFilterItems.Clear();
                    for (int i = 0; i < mConfigInfo.ListFilterInfos.Count; i++)
                    {
                        FilterInfo info = mConfigInfo.ListFilterInfos[i];
                        FilterItem item = FilterItem.CreateItem(info);
                        mListFilterItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadLayoutInfos()
        {
            try
            {
                var serializer = new XmlLayoutSerializer(DockingMain);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layout.xml");
                if (File.Exists(path))
                {
                    using (var stream = new StreamReader(path))
                    {
                        serializer.Deserialize(stream);
                    }
                }
                var panel = GetPanlByContentID("PanelModuleList");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Closing += PanelDocument_Closing;
                }
                panel = GetPanlByContentID("PanelPrefixList");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Closing += PanelDocument_Closing;
                }
                panel = GetPanlByContentID("PanelCategoryList");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Closing += PanelDocument_Closing;
                }
                panel = GetPanlByContentID("PanelCopyLang");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Closing += PanelDocument_Closing;
                }
                panel = GetPanlByContentID("PanelAddLang");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Closing += PanelDocument_Closing;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitFilterItems()
        {
            try
            {
                InitModuleItems();
                InitPrifixItems();
                InitCategoryItems();
                mRootFilterModule.IsExpanded = true;
                mRootFilterPrefix.IsExpanded = true;
                mRootFilterCategory.IsExpanded = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitModuleItems()
        {
            try
            {
                var parents = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_ALLMODULE).ToList();
                for (int i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    var childs = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_MODULE).ToList();
                    for (int j = 0; j < childs.Count; j++)
                    {
                        var child = childs[j];
                        var subChilds =
                            mListFilterItems.Where(
                                f => f.Type == FilterInfo.TYPE_SUBMODULE && f.Other01 == child.Value).ToList();
                        for (int k = 0; k < subChilds.Count; k++)
                        {
                            var subChild = subChilds[k];
                            child.AddChild(subChild);
                        }
                        parent.AddChild(child);
                    }
                    parent.IsExpanded = true;
                    mRootFilterModule.AddChild(parent);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitPrifixItems()
        {
            try
            {
                var parents = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_ALLPRIFIX).ToList();
                for (int i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    var childs = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_PREFIX).ToList();
                    for (int j = 0; j < childs.Count; j++)
                    {
                        var child = childs[j];
                        parent.AddChild(child);
                    }
                    parent.IsExpanded = true;
                    mRootFilterPrefix.AddChild(parent);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitCategoryItems()
        {
            try
            {
                var parents = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_ALLCATEGORY).ToList();
                for (int i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    var childs = mListFilterItems.Where(f => f.Type == FilterInfo.TYPE_CATEGORY).ToList();
                    for (int j = 0; j < childs.Count; j++)
                    {
                        var child = childs[j];
                        parent.AddChild(child);
                    }
                    parent.IsExpanded = true;
                    mRootFilterCategory.AddChild(parent);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitDatabaseInfo()
        {
            try
            {
                //mDatabaseInfo = new DatabaseInfo();
                //mDatabaseInfo.TypeID = 3;
                //mDatabaseInfo.TypeName = "Oracle";
                //mDatabaseInfo.Host = "192.168.4.182";
                //mDatabaseInfo.Port = 1521;
                //mDatabaseInfo.DBName = "PFOrcl";
                //mDatabaseInfo.LoginName = "PFDEV";
                //mDatabaseInfo.Password = "PF,123";

                if (mConfigInfo == null) { return; }
                mDatabaseInfo = mConfigInfo.DataBaseInfo;

                SetDatabaseInfo();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Commands

        private void BindCommands()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, (s, e) => { },
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SettingCommand, SettingCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommand_Executed,
                (s, e) => e.CanExecute = mIsLangChanged));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.ExportCommand, ExportCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SearchCommand, SearchCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SearchPreCommand, SearchPreCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SearchNextCommand, SearchNextCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SearchClearCommand, SearchClearCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SaveLayoutCommand, SaveLayoutCommand_Executed,
               (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.ResetLayoutCommand, ResetLayoutCommand_Executed,
               (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.CopyLangCommand, CopyLangCommand_Executed,
               (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.AddLangCommand, AddLangCommand_Executed,
               (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SetViewCommand, SetViewCommand_Executed,
             (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(MainWindowCommands.SynchronCommand, SyncLangCommand_Executed,
             (s, e) => e.CanExecute = true));

        }

        #endregion


        #region Command Handlers

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void SettingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                PopupWindow popup = new PopupWindow();
                UCSystemSetting uc = new UCSystemSetting();
                uc.ConfigInfo = mConfigInfo;
                popup.Content = uc;
                var result = popup.ShowDialog();
                if (result == true)
                {
                    InitDatabaseInfo();
                    SaveSettingInfos();
                    LoadLanguageItems();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveLanguageItems();
        }

        private void ExportCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExportLanguageItems();
        }

        private void SearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchLanguageItems();
        }

        private void SearchPreCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchPreLanguageItems();
        }

        private void SearchNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchNextLanguageItems();
        }

        private void SearchClearCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSearchState();
        }

        private void SaveLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(DockingMain);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layout.xml");
                using (var stream = new StreamWriter(path))
                {
                    serializer.Serialize(stream);
                }
                ShowInfoMessage(string.Format("Save layout end"));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ResetLayoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LoadLayoutInfos();
                SetViewStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CopyLangCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddLangCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SetViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SetPanelVisibility();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SyncLangCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SyncLanguageItems();
        }

        #endregion


        #region EventHandlers

        void InRibbonLanuguageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems;
            if (items == null || items.Count <= 0) { return; }
            var item = items[0] as LangTypeInfo;
            if (item == null) { return; }
            LoadLanguageItems(item);
        }

        void TvCategoryList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadLanguageItems();
        }

        void TvPrefixList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadLanguageItems();
        }

        void TvModuleList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadLanguageItems();
        }

        void LangItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as LanguageItem;
            if (item == null) { return; }
            if (e.PropertyName == "State")
            {
                mIsLangChanged = mIsLangChanged | ((item.State & LangItemState.ValueChanged) > 0);
            }
        }

        void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    SearchLanguageItems();
                }
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
            ThreadPool.QueueUserWorkItem(
                a =>
                    Dispatcher.Invoke(
                        new Action(
                            () =>
                                MessageBox.Show(msg, "UMP Language Manager", MessageBoxButton.OK, MessageBoxImage.Error))));

        }

        private void ShowInfoMessage(string msg)
        {
            ThreadPool.QueueUserWorkItem(
               a =>
                   Dispatcher.Invoke(
                       new Action(
                           () =>
                               MessageBox.Show(msg, "UMP Language Manager", MessageBoxButton.OK, MessageBoxImage.Information))));
        }

        #endregion


        #region Operations

        private void LoadLanguageItems()
        {
            var langItem = InRibbonLanuguageType.SelectedItem as LangTypeInfo;
            LoadLanguageItems(langItem);
        }

        private void LoadLanguageItems(LangTypeInfo langItem)
        {
            try
            {
                if (mIsLangChanged)
                {
                    var result = MessageBox.Show(string.Format("Language has changed and not saved, Save change now?"),
                        "UMPLanguageManager", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        SaveLanguageItems();
                    }
                    else if (result == MessageBoxResult.Cancel) { return; }
                }
                mListLanguageItems.Clear();
                string strStatistic = string.Format("Count:{0}", 0);
                SetStatistic(strStatistic);
                if (mConditionInfo == null)
                {
                    mConditionInfo = new ConditionInfo();
                }
                if (mDatabaseInfo == null)
                {
                    return;
                }
                mConditionInfo.Reset();
                mConditionInfo.DBType = mDatabaseInfo.TypeID;
                mConditionInfo.ConnectionString = mDatabaseInfo.GetConnectionString();

                if (langItem == null) { return; }
                mConditionInfo.LangID = langItem.LangID;
                var moduleItem = TvModuleList.SelectedItem as FilterItem;
                var prefixItem = TvPrefixList.SelectedItem as FilterItem;
                var categoryItem = TvCategoryList.SelectedItem as FilterItem;
                if (moduleItem == null
                    || moduleItem.Type == FilterInfo.TYPE_ALLMODULE)
                {
                    return;
                }
                int module;
                if (moduleItem.Type == FilterInfo.TYPE_MODULE)
                {
                    if (!int.TryParse(moduleItem.Value, out module))
                    {
                        ShowErrorMessage(string.Format("Module invalid.\t{0}", moduleItem.Value));
                        return;
                    }
                    mConditionInfo.ModuleID = module;
                    mConditionInfo.SubModuleID = -1;
                }
                if (moduleItem.Type == FilterInfo.TYPE_SUBMODULE)
                {
                    int submodule;
                    if (!int.TryParse(moduleItem.Value, out submodule))
                    {
                        ShowErrorMessage(string.Format("SubModule invalid.\t{0}", moduleItem.Value));
                        return;
                    }
                    mConditionInfo.SubModuleID = submodule;
                    if (!int.TryParse(moduleItem.Other01, out module))
                    {
                        ShowErrorMessage(string.Format("Module invalid.\t{0}", moduleItem.Other01));
                        return;
                    }
                    mConditionInfo.ModuleID = module;
                }
                if (prefixItem != null)
                {
                    if (prefixItem.Type != FilterInfo.TYPE_ALLPRIFIX)
                    {
                        string prefix = prefixItem.Value;
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            mConditionInfo.Prefix = prefix;
                        }
                    }
                }
                if (categoryItem != null)
                {
                    if (categoryItem.Type != FilterInfo.TYPE_ALLCATEGORY)
                    {
                        string category = categoryItem.Value;
                        if (!string.IsNullOrEmpty(category))
                        {
                            mConditionInfo.Category = category;
                        }
                    }
                }
                MyWaiter.Visibility = Visibility.Visible;
                SetStatusMessage(string.Format("Loading langguage infos..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => DoLoadLanguageItems();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatusMessage(string.Empty);

                    mIsLangChanged = false;
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void DoLoadLanguageItems()
        {
            try
            {
                if (mConditionInfo == null) { return; }
                mConditionInfo.GetQueryString();
                string strConn = mConditionInfo.ConnectionString;
                string strSql = mConditionInfo.QueryString;
                OperationReturn optReturn;
                DataSet objDataSet = new DataSet();
                switch (mConditionInfo.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    ShowErrorMessage(string.Format("DataSet is null or DataTable not exist"));
                    return;
                }
                int total = 0;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    LanguageInfo info = new LanguageInfo();
                    info.LangID = Convert.ToInt32(dr["C001"]);
                    info.Name = dr["C002"].ToString();
                    info.Module = Convert.ToInt32(dr["C009"]);
                    info.SubModule = Convert.ToInt32(dr["C010"]);
                    string str = string.Empty;
                    str += dr["C005"].ToString();
                    str += dr["C006"].ToString();
                    str += dr["C007"].ToString();
                    str += dr["C008"].ToString();
                    info.Display = str;
                    info.Module = Convert.ToInt32(dr["C009"]);
                    info.SubModule = Convert.ToInt32(dr["C010"]);
                    var item = LanguageItem.CreateItem(info);
                    if (item == null) { continue; }
                    item.PropertyChanged += LangItem_PropertyChanged;
                    total++;
                    string strStatistic = string.Format("Count:{0}", total);
                    SetStatistic(strStatistic);
                    AddLanguageItem(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveLanguageItems()
        {
            try
            {
                if (mConditionInfo == null) { return; }
                MyWaiter.Visibility = Visibility.Visible;
                SetStatusMessage(string.Format("Saving langguage infos..."));
                mIsOptSuccess = true;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => DoSaveLanguageItems();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatusMessage(string.Empty);

                    if (mIsOptSuccess)
                    {
                        ShowInfoMessage(string.Format("Save language infos end"));

                        for (int i = 0; i < mListLanguageItems.Count; i++)
                        {
                            var item = mListLanguageItems[i];
                            item.State = LangItemState.None;
                        }
                        mIsLangChanged = false;
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void DoSaveLanguageItems()
        {
            try
            {
                if (mConditionInfo == null)
                {
                    mIsOptSuccess = false;
                    return;
                }
                if (!mIsOptSuccess) { return; }
                string strConn = mConditionInfo.ConnectionString;
                string strSql = mConditionInfo.QueryString;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (mConditionInfo.DBType)
                {
                    case 2:
                        objConn = MssqlOperation.GetConnection(strConn);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        objConn = OracleOperation.GetConnection(strConn);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        ShowErrorMessage(string.Format("DBType not support"));
                        mIsOptSuccess = false;
                        return;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    ShowErrorMessage(string.Format("Db object is null"));
                    mIsOptSuccess = false;
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < mListLanguageItems.Count; i++)
                    {
                        var item = mListLanguageItems[i];
                        if (!((item.State & LangItemState.ValueChanged) > 0))
                        {
                            continue;
                        }
                        string strName = item.Name;
                        string strDisplay = item.Display;
                        DataRow dr;
                        if (mConditionInfo.SubModuleID == -1)
                        {
                            dr = objDataSet.Tables[0].Select(
                                string.Format("C001 = {0} and C002 = '{1}' and C009 = {2}",
                                    mConditionInfo.LangID,
                                    strName,
                                    mConditionInfo.ModuleID)).FirstOrDefault();
                        }
                        else
                        {
                            dr = objDataSet.Tables[0].Select(
                                string.Format("C001 = {0} and C002 = '{1}' and C010 = {2}",
                                    mConditionInfo.LangID,
                                    strName,
                                    mConditionInfo.SubModuleID)).FirstOrDefault();
                        }
                        if (dr != null)
                        {
                            string strTemp = strDisplay;
                            int intBlock = 512;
                            for (int j = 0; j < 4; j++)
                            {
                                if (strTemp.Length > intBlock)
                                {
                                    dr[string.Format("C00{0}", 5 + j)] = strTemp.Substring(0, intBlock);
                                }
                                else
                                {
                                    dr[string.Format("C00{0}", 5 + j)] = strTemp;
                                    break;
                                }
                                strTemp = strTemp.Substring(intBlock);
                            }
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                    mIsOptSuccess = false;
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
                ShowErrorMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        private void SearchLanguageItems()
        {
            try
            {
                mListSearchedItems.Clear();
                int searchCount = 0;
                int searchCurrent = 0;
                string strContent = TxtSearch.Text;
                if (string.IsNullOrEmpty(strContent)) { return; }
                for (int i = 0; i < mListLanguageItems.Count; i++)
                {
                    var item = mListLanguageItems[i];
                    if (item.Display.ToLower().Contains(strContent.ToLower()))
                    {
                        item.State = item.State | LangItemState.Searched;
                        mListSearchedItems.Add(item);
                        searchCount++;
                    }
                    else
                    {
                        item.State = item.State & (~LangItemState.Searched);
                    }
                }
                if (mListSearchedItems.Count > 0)
                {
                    var first = mListSearchedItems[0];
                    first.IsSelected = true;
                    LvLanguageItems.ScrollIntoView(first);
                    searchCurrent = 0;
                }
                SetStatistic(string.Format("{0} record searched, currrent:{1}", searchCount, searchCurrent));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ClearSearchState()
        {
            try
            {
                mListSearchedItems.Clear();
                TxtSearch.Text = string.Empty;
                for (int i = 0; i < mListLanguageItems.Count; i++)
                {
                    var item = mListLanguageItems[i];
                    item.State = item.State & (~LangItemState.Searched);
                }
                SetStatistic(string.Empty);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SearchNextLanguageItems()
        {
            try
            {
                int searchCount = mListSearchedItems.Count;
                int searchCurrent = 0;
                var item = LvLanguageItems.SelectedItem as LanguageItem;
                if (item == null) { return; }
                int index = mListSearchedItems.IndexOf(item);
                if (index < mListSearchedItems.Count - 1)
                {
                    int next = index + 1;
                    mListSearchedItems[next].IsSelected = true;
                    LvLanguageItems.ScrollIntoView(mListSearchedItems[next]);
                    searchCurrent = next;
                }
                SetStatistic(string.Format("{0} record searched, currrent:{1}", searchCount, searchCurrent));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SearchPreLanguageItems()
        {
            try
            {
                int searchCount = mListSearchedItems.Count;
                int searchCurrent = 0;
                var item = LvLanguageItems.SelectedItem as LanguageItem;
                if (item == null) { return; }
                int index = mListSearchedItems.IndexOf(item);
                if (index > 0)
                {
                    int pre = index - 1;
                    mListSearchedItems[pre].IsSelected = true;
                    LvLanguageItems.ScrollIntoView(mListSearchedItems[pre]);
                    searchCurrent = pre;
                }
                SetStatistic(string.Format("{0} record searched, currrent:{1}", searchCount, searchCurrent));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveSettingInfos()
        {
            try
            {
                if (mConfigInfo == null) { return; }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigInfo.xml");
                OperationReturn optReturn = XMLHelper.SerializeFile(mConfigInfo, path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    //return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SyncLanguageItems()
        {
            try
            {
                if (mConfigInfo == null)
                {
                    ShowErrorMessage(string.Format("ConfigInfo is null"));
                    return;
                }
                DatabaseInfo sourceDBInfo = mConfigInfo.DataBaseInfo;
                DatabaseInfo syncDBInfo = mConfigInfo.SyncDBInfo;
                if (sourceDBInfo == null || syncDBInfo == null)
                {
                    ShowErrorMessage(string.Format("SourceDBInfo or SyncDBInfo is null"));
                    return;
                }
                int syncMethod = 0;     //0:This page   1:This module   2:All
                if (RbSyncPage.IsChecked == true)
                {
                    syncMethod = 0;
                }
                if (RbSyncModule.IsChecked == true)
                {
                    syncMethod = 1;
                }
                if (RbSyncAll.IsChecked == true)
                {
                    syncMethod = 2;
                }
                bool isAllLangType = CbSyncAllLangType.IsChecked == true;
                bool isOverWrite = CbSyncOverWrite.IsChecked == true;
                if (mConditionInfo == null) { return; }
                ConditionInfo syncCondition = new ConditionInfo();
                syncCondition.DBType = syncDBInfo.TypeID;
                syncCondition.ConnectionString = syncDBInfo.GetConnectionString();
                mListOptLangInfos.Clear();
                if (syncMethod == 0 && !isAllLangType)
                {
                    //直接使用当前列表作为源
                    syncCondition.LangID = mConditionInfo.LangID;
                    syncCondition.ModuleID = mConditionInfo.ModuleID;
                    syncCondition.SubModuleID = mConditionInfo.SubModuleID;
                    syncCondition.Prefix = mConditionInfo.Prefix;
                    syncCondition.Category = mConditionInfo.Category;
                    for (int i = 0; i < mListLanguageItems.Count; i++)
                    {
                        var item = mListLanguageItems[i];
                        item.UpdateLangInfo();
                        var info = item.Info;
                        if (info != null)
                        {
                            mListOptLangInfos.Add(info);
                        }
                    }
                    mIsOptSuccess = true;
                    MyWaiter.Visibility = Visibility.Visible;
                    SetStatusMessage(string.Format("Synchronizin language infos..."));
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) => SyncLanguageItems(syncCondition, isOverWrite);
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        MyWaiter.Visibility = Visibility.Collapsed;
                        SetStatusMessage(string.Empty);

                        if (mIsOptSuccess)
                        {
                            ShowInfoMessage(string.Format("Synchronizin end"));
                        }
                    };
                    mWorker.RunWorkerAsync();
                    return;
                }
                ConditionInfo optCondition = new ConditionInfo();
                optCondition.Reset();
                optCondition.DBType = mDatabaseInfo.TypeID;
                optCondition.ConnectionString = mDatabaseInfo.GetConnectionString();
                if (isAllLangType)
                {
                    optCondition.LangID = 0;
                }
                else
                {
                    optCondition.LangID = mConditionInfo.LangID;
                }
                switch (syncMethod)
                {
                    case 0:     //This Page
                        optCondition.ModuleID = mConditionInfo.ModuleID;
                        optCondition.SubModuleID = mConditionInfo.SubModuleID;
                        optCondition.Prefix = mConditionInfo.Prefix;
                        optCondition.Category = mConditionInfo.Category;
                        break;
                    case 1:
                        optCondition.ModuleID = mConditionInfo.ModuleID;
                        optCondition.SubModuleID = mConditionInfo.SubModuleID;
                        optCondition.Prefix = string.Empty;
                        optCondition.Category = string.Empty;
                        break;
                    case 2:
                        optCondition.ModuleID = -1;
                        optCondition.SubModuleID = -1;
                        optCondition.Prefix = string.Empty;
                        optCondition.Category = string.Empty;
                        break;
                }
                syncCondition.LangID = optCondition.LangID;
                syncCondition.ModuleID = optCondition.ModuleID;
                syncCondition.SubModuleID = optCondition.SubModuleID;
                syncCondition.Prefix = optCondition.Prefix;
                syncCondition.Category = optCondition.Category;

                mIsOptSuccess = true;
                MyWaiter.Visibility = Visibility.Visible;
                SetStatusMessage(string.Format("Loading LanguageInfos..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadOptLanguageInfos(optCondition);
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;
                    SetStatusMessage(string.Empty);

                    if (mIsOptSuccess)
                    {
                        MyWaiter.Visibility = Visibility.Visible;
                        SetStatusMessage("Synchronizin language infos...");
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (ss, dde) => SyncLanguageItems(syncCondition, isOverWrite);
                        mWorker.RunWorkerCompleted += (ss, rre) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Collapsed;
                            SetStatusMessage(string.Empty);

                            ShowInfoMessage(string.Format("Synchronizin end"));
                        };
                        mWorker.RunWorkerAsync();
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadOptLanguageInfos(ConditionInfo optConditionInfo)
        {
            try
            {
                if (optConditionInfo == null) { return; }
                optConditionInfo.GetQueryString();
                string strConn = optConditionInfo.ConnectionString;
                string strSql = optConditionInfo.QueryString;
                OperationReturn optReturn;
                DataSet objDataSet = new DataSet();
                switch (optConditionInfo.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mIsOptSuccess = false;
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mIsOptSuccess = false;
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    ShowErrorMessage(string.Format("DataSet is null or DataTable not exist"));
                    mIsOptSuccess = false;
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    LanguageInfo info = new LanguageInfo();
                    info.LangID = Convert.ToInt32(dr["C001"]);
                    info.Name = dr["C002"].ToString();
                    info.Module = Convert.ToInt32(dr["C009"]);
                    info.SubModule = Convert.ToInt32(dr["C010"]);
                    string str = string.Empty;
                    str += dr["C005"].ToString();
                    str += dr["C006"].ToString();
                    str += dr["C007"].ToString();
                    str += dr["C008"].ToString();
                    info.Display = str;
                    info.Module = Convert.ToInt32(dr["C009"]);
                    info.SubModule = Convert.ToInt32(dr["C010"]);
                    mListOptLangInfos.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        private void SyncLanguageItems(ConditionInfo syncConditionInfo, bool isOverWrite)
        {
            try
            {
                syncConditionInfo.GetQueryString();
                string strConn = syncConditionInfo.ConnectionString;
                string strSql = syncConditionInfo.QueryString;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (syncConditionInfo.DBType)
                {
                    case 2:
                        objConn = MssqlOperation.GetConnection(strConn);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        objConn = OracleOperation.GetConnection(strConn);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        ShowErrorMessage(string.Format("DBType not support"));
                        mIsOptSuccess = false;
                        return;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    ShowErrorMessage(string.Format("Db object is null"));
                    mIsOptSuccess = false;
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    int totalCount = 0;
                    int updateCount = 0;
                    int addCount = 0;
                    int skipCount = 0;
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < mListOptLangInfos.Count; i++)
                    {
                        var info = mListOptLangInfos[i];
                        bool isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = '{0}'", info.Name)).FirstOrDefault();
                        if (dr != null)
                        {
                            if (isOverWrite)
                            {
                                string strTemp = info.Display;
                                int intBlock = 512;
                                for (int j = 0; j < 4; j++)
                                {
                                    if (strTemp.Length > intBlock)
                                    {
                                        dr[string.Format("C00{0}", 5 + j)] = strTemp.Substring(0, intBlock);
                                    }
                                    else
                                    {
                                        dr[string.Format("C00{0}", 5 + j)] = strTemp;
                                        break;
                                    }
                                    strTemp = strTemp.Substring(intBlock);
                                }
                                updateCount++;
                            }
                            else
                            {
                                skipCount++;
                            }
                        }
                        else
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = info.LangID;
                            dr["C002"] = info.Name;
                            dr["C003"] = 0;
                            dr["C004"] = 0;
                            string strTemp = info.Display;
                            int intBlock = 512;
                            for (int j = 0; j < 4; j++)
                            {
                                if (strTemp.Length > intBlock)
                                {
                                    dr[string.Format("C00{0}", 5 + j)] = strTemp.Substring(0, intBlock);
                                }
                                else
                                {
                                    dr[string.Format("C00{0}", 5 + j)] = strTemp;
                                    break;
                                }
                                strTemp = strTemp.Substring(intBlock);
                            }
                            dr["C009"] = info.Module;
                            dr["C010"] = info.SubModule;
                            dr["C011"] = info.Page;
                            isAdd = true;
                        }
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            addCount++;
                        }
                        totalCount++;

                        SetSyncStatistic(totalCount, updateCount, addCount, skipCount);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                    mIsOptSuccess = false;
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
                ShowErrorMessage(ex.Message);
            }
        }

        private void ExportLanguageItems()
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Xml Document(*.xml)|*.xml";
                dialog.FileName = "UMP Languages.xml";
                var result = dialog.ShowDialog();
                if (result != true) { return; }
                var path = dialog.FileName;
                if (mConfigInfo == null)
                {
                    ShowErrorMessage(string.Format("ConfigInfo is null"));
                    return;
                }
                if (mConditionInfo == null)
                {
                    ShowErrorMessage(string.Format("ConditonInfo is null"));
                    return;
                }
                int exportMethod = 0;     //0:This page   1:This module   2:All
                if (RbExportPage.IsChecked == true)
                {
                    exportMethod = 0;
                }
                if (RbExportModule.IsChecked == true)
                {
                    exportMethod = 1;
                }
                if (RbExportAll.IsChecked == true)
                {
                    exportMethod = 2;
                }
                bool isAllLangType = CbExportAllLangType.IsChecked == true;
                ConditionInfo optCondition = new ConditionInfo();
                optCondition.Reset();
                optCondition.DBType = mConditionInfo.DBType;
                optCondition.ConnectionString = mConditionInfo.ConnectionString;
                mListOptLangInfos.Clear();
                if (exportMethod == 0 && !isAllLangType)
                {
                    optCondition.LangID = mConditionInfo.LangID;
                    optCondition.ModuleID = mConditionInfo.ModuleID;
                    optCondition.SubModuleID = mConditionInfo.SubModuleID;
                    optCondition.Prefix = mConditionInfo.Prefix;
                    optCondition.Category = mConditionInfo.Category;
                    for (int i = 0; i < mListLanguageItems.Count; i++)
                    {
                        var item = mListLanguageItems[i];
                        item.UpdateLangInfo();
                        var info = item.Info;
                        if (info != null)
                        {
                            mListOptLangInfos.Add(info);
                        }
                    }
                    DoExportLanguageInfos(optCondition, path);
                    if (mIsOptSuccess)
                    {
                        ShowInfoMessage(string.Format("Export end.\t{0}", path));
                    }
                    return;
                }
                if (isAllLangType)
                {
                    optCondition.LangID = 0;
                }
                else
                {
                    optCondition.LangID = mConditionInfo.LangID;
                }
                switch (exportMethod)
                {
                    case 0:     //This Page
                        optCondition.ModuleID = mConditionInfo.ModuleID;
                        optCondition.SubModuleID = mConditionInfo.SubModuleID;
                        optCondition.Prefix = mConditionInfo.Prefix;
                        optCondition.Category = mConditionInfo.Category;
                        break;
                    case 1:
                        optCondition.ModuleID = mConditionInfo.ModuleID;
                        optCondition.SubModuleID = mConditionInfo.SubModuleID;
                        optCondition.Prefix = string.Empty;
                        optCondition.Category = string.Empty;
                        break;
                    case 2:
                        optCondition.ModuleID = -1;
                        optCondition.SubModuleID = -1;
                        optCondition.Prefix = string.Empty;
                        optCondition.Category = string.Empty;
                        break;
                }
                mIsOptSuccess = true;
                MyWaiter.Visibility = Visibility.Visible;
                SetStatusMessage(string.Format("Loading LanguageInfos..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadOptLanguageInfos(optCondition);
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;
                    SetStatusMessage(string.Empty);

                    DoExportLanguageInfos(optCondition, path);
                    if (mIsOptSuccess)
                    {
                        ShowInfoMessage(string.Format("Export end.\t{0}", path));
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void DoExportLanguageInfos(ConditionInfo exportCondition, string path)
        {
            try
            {
                if (mLangFilgeInfo == null)
                {
                    mLangFilgeInfo = new LangFileInfo();
                }
                mLangFilgeInfo.GenerateTime = DateTime.Now;
                if (mConditionInfo != null)
                {
                    mLangFilgeInfo.LangType = exportCondition.LangID;
                    mLangFilgeInfo.ModuleID = exportCondition.ModuleID;
                    mLangFilgeInfo.SubModuleID = exportCondition.SubModuleID;
                    mLangFilgeInfo.Prefix = exportCondition.Prefix;
                    mLangFilgeInfo.Category = exportCondition.Category;
                }
                mLangFilgeInfo.TotalCount = mListOptLangInfos.Count;
                OperationReturn optReturn;
                mLangFilgeInfo.ListLangInfos.Clear();
                for (int i = 0; i < mListOptLangInfos.Count; i++)
                {
                    var info = mListOptLangInfos[i];
                    mLangFilgeInfo.ListLangInfos.Add(info);
                }
                optReturn = XMLHelper.SerializeFile(mLangFilgeInfo, path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    mIsOptSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        #endregion


        #region Others

        private void SetStatistic(string str)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LbCount.Content = str;
            }));
        }

        private void SetSyncStatistic(int total, int update, int add, int skip)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LbCount.Content = string.Format("Total:{0} Update:{1} Add:{2} Skip:{3}", total, update, add, skip);
            }));
        }

        private void AddLanguageItem(LanguageItem item)
        {
            Dispatcher.Invoke(new Action(() => mListLanguageItems.Add(item)));
        }

        private void SetDatabaseInfo()
        {
            ThreadPool.QueueUserWorkItem(a => Dispatcher.Invoke(new Action(() =>
            {
                if (mDatabaseInfo == null) { return; }
                LbDatabaseInfo.Content = string.Format("{0}-{1}:{2}-{3}-{4}",
                    mDatabaseInfo.TypeID == 3 ? "Oracle" : "MSSQL",
                    mDatabaseInfo.Host,
                    mDatabaseInfo.Port,
                    mDatabaseInfo.DBName,
                    mDatabaseInfo.LoginName);
            })));
        }

        private void SetStatusMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LbStatus.Content = msg;
            }));
        }

        private LayoutAnchorable GetPanlByContentID(string contentID)
        {
            var panel =
                DockingMain.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        private void SetViewStatus()
        {
            var panel = GetPanlByContentID("PanelModuleList");
            if (panel != null)
            {
                CbModuleList.IsChecked = panel.IsVisible;
            }
            panel = GetPanlByContentID("PanelPrefixList");
            if (panel != null)
            {
                CbPrefix.IsChecked = panel.IsVisible;
            }
            panel = GetPanlByContentID("PanelCategoryList");
            if (panel != null)
            {
                CbCategory.IsChecked = panel.IsVisible;
            }
            //panel = GetPanlByContentID("PanelCopyLang");
            //if (panel != null)
            //{
            //    CbCopyLang.IsChecked = panel.IsVisible;
            //}
            //panel = GetPanlByContentID("PanelAddLang");
            //if (panel != null)
            //{
            //    CbAddLang.IsChecked = panel.IsVisible;
            //}
        }

        private void SetPanelVisibility()
        {
            var visible = CbModuleList.IsChecked == true;
            var panel = GetPanlByContentID("PanelModuleList");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            visible = CbPrefix.IsChecked == true;
            panel = GetPanlByContentID("PanelPrefixList");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            visible = CbCategory.IsChecked == true;
            panel = GetPanlByContentID("PanelCategoryList");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            //visible = CbCopyLang.IsChecked == true;
            //panel = GetPanlByContentID("PanelCopyLang");
            //if (panel != null)
            //{
            //    if (visible) { panel.Show(); }
            //    else { panel.Hide(); }
            //}
            //visible = CbAddLang.IsChecked == true;
            //panel = GetPanlByContentID("PanelAddLang");
            //if (panel != null)
            //{
            //    if (visible) { panel.Show(); }
            //    else { panel.Hide(); }
            //}
        }

        private void PanelDocument_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                var panel = sender as LayoutAnchorable;
                if (panel != null)
                {
                    panel.Hide();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion

    }
}
