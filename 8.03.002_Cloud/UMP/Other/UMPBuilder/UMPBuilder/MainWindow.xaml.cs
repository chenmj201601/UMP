using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UMPBuilder.Models;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;

namespace UMPBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private string mAppName = "UMPBuilder";
        private List<ProjectInfo> mListProjectInfos;
        private List<UMPFileInfo> mListFileInfos;
        private List<OptButtonItem> mListOptButtons;
        private List<OptObjectItem> mListOptObjectItems;
        private List<GlobalSetting> mListSystemSettings;
        private SystemConfig mSystemConfig;
        private StatisticalItem mStatisticalItem;
        private LogOperator mLogOperator;
        private BackgroundWorker mWorker;
        private bool mIsContinue;
        private bool mIsOptFail;
        private long mPackagedSize;
        private int mPackageCount;
        private long mPackageTotalSize;
        private bool mBuildUpdater;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListOptButtons = new List<OptButtonItem>();
            mListProjectInfos = new List<ProjectInfo>();
            mListFileInfos = new List<UMPFileInfo>();
            mListOptObjectItems = new List<OptObjectItem>();
            mListSystemSettings = new List<GlobalSetting>();
            mSystemConfig = new SystemConfig();
            mStatisticalItem = new StatisticalItem();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLogOperator();
            Init();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mIsContinue)
            {
                var result = MessageBox.Show(string.Format("UMP Builder is Building, Do you confirm exit?"),
                    "UMP Builder", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                mIsContinue = false;
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                LoadSystemConfig();
                SetMyWaiter(true);
                SetStatusMessage(string.Format("Loading basic data..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    InitOperationButtons();
                    LoadSystemSettings();
                    LoadProjectInfos();
                    LoadUMPFileInfos();

                    InitOptObjectItems();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetMyWaiter(false);
                    SetStatusMessage(string.Empty);

                    CreateOptButtons();
                    CreateStartPage();
                    CreateSvnUpdatePage();
                    CreateComilePage();
                    CreateFileCopyPage();
                    CreateFilePackagePage();
                    CreateStatisticalPage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadSystemConfig()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SystemConfig.FILE_NAME);
                if (!File.Exists(path))
                {
                    WriteLog("LoadConfig", string.Format("SystemConfig file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<SystemConfig>(path);
                if (!optReturn.Result)
                {
                    WriteLog("LoadConfig",
                        string.Format("LoadSystemConfig fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                SystemConfig config = optReturn.Data as SystemConfig;
                if (config == null)
                {
                    WriteLog("LoadConfig", string.Format("SystemConfig is null."));
                    return;
                }
                mSystemConfig = config;

                WriteLog("LoadConfig", string.Format("RootDir:{0}", mSystemConfig.RootDir));
                WriteLog("LoadConfig", string.Format("CompilerPath:{0}", mSystemConfig.CompilerPath));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadSystemSettings()
        {
            try
            {
                mListSystemSettings.Clear();
                if (mSystemConfig == null) { return; }
                for (int i = 0; i < mSystemConfig.ListSettings.Count; i++)
                {
                    var setting = mSystemConfig.ListSettings[i];
                    if (setting != null)
                    {
                        mListSystemSettings.Add(setting);
                    }
                }

                WriteLog("LoadConfig", string.Format("LoadSystemSettings end.\tCount:{0}", mListSystemSettings.Count));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadProjectInfos()
        {
            try
            {
                mListProjectInfos.Clear();
                if (mSystemConfig == null) { return; }
                for (int i = 0; i < mSystemConfig.ListProjects.Count; i++)
                {
                    var project = mSystemConfig.ListProjects[i];
                    if (project != null)
                    {
                        mListProjectInfos.Add(project);
                    }
                }

                WriteLog("LoadConfig", string.Format("LoadProjectInfos end.\tCount:{0}", mListProjectInfos.Count));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadUMPFileInfos()
        {
            try
            {
                mListFileInfos.Clear();
                if (mSystemConfig == null) { return; }
                for (int i = 0; i < mSystemConfig.ListUMPFileInfos.Count; i++)
                {
                    var project = mSystemConfig.ListUMPFileInfos[i];
                    if (project != null)
                    {
                        mListFileInfos.Add(project);
                    }
                }

                WriteLog("LoadConfig", string.Format("LoadUMPFileInfos end.\tCount:{0}", mListFileInfos.Count));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitOperationButtons()
        {
            try
            {
                mListOptButtons.Clear();
                OptButtonItem item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_RESTART;
                item.Display = UMPBuilderConsts.OPT_DESC_RESTART;
                item.Tip = item.Display;
                item.Icon = "Images/00007.png";
                mListOptButtons.Add(item);
                item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_START;
                item.Display = UMPBuilderConsts.OPT_DESC_START;
                item.Tip = item.Display;
                item.Icon = "Images/00004.png";
                mListOptButtons.Add(item);
                //item = new OptButtonItem();
                //item.Name = UMPBuilderConsts.OPT_NAME_PAUSE;
                //item.Display = UMPBuilderConsts.OPT_DESC_PAUSE;
                //item.Tip = item.Display;
                //item.Icon = "Images/00003.png";
                //mListOptButtons.Add(item);
                item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_STOP;
                item.Display = UMPBuilderConsts.OPT_DESC_STOP;
                item.Tip = item.Display;
                item.Icon = "Images/00005.png";
                mListOptButtons.Add(item);
                item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_PRE;
                item.Display = UMPBuilderConsts.OPT_DESC_PRE;
                item.Tip = item.Display;
                item.Icon = "Images/00001.png";
                mListOptButtons.Add(item);
                item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_NEXT;
                item.Display = UMPBuilderConsts.OPT_DESC_NEXT;
                item.Tip = item.Display;
                item.Icon = "Images/00002.png";
                mListOptButtons.Add(item);
                item = new OptButtonItem();
                item.Name = UMPBuilderConsts.OPT_NAME_SETTING;
                item.Display = UMPBuilderConsts.OPT_DESC_SETTING;
                item.Tip = item.Display;
                item.Icon = "Images/00006.png";
                mListOptButtons.Add(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitOptObjectItems()
        {
            try
            {
                mListOptObjectItems.Clear();

                OptObjectItem item;
                for (int i = 0; i < mListProjectInfos.Count; i++)
                {
                    var project = mListProjectInfos[i];
                    item = new OptObjectItem();
                    item.Operation = UMPBuilderConsts.OPTOBJ_SVNUPDATE;
                    item.IsChecked = true;
                    item.Name = project.ProjectName;
                    item.Description = project.ProjectDir;
                    item.Category = project.ProjectType;
                    item.StrCategory = GetStrCategory(item.Category, 1);
                    item.Status = UMPBuilderConsts.STA_DEFAULT;
                    item.StrStatus = string.Empty;
                    item.StrMessage = string.Empty;
                    item.Info = project;
                    mListOptObjectItems.Add(item);
                }
                for (int i = 0; i < mListProjectInfos.Count; i++)
                {
                    var project = mListProjectInfos[i];
                    item = new OptObjectItem();
                    item.Operation = UMPBuilderConsts.OPTOBJ_COMPILE;
                    item.IsChecked = true;
                    item.Name = project.ProjectName;
                    item.Description = project.ProjectDir;
                    item.Category = project.ProjectType;
                    item.StrCategory = GetStrCategory(item.Category, 1);
                    item.Status = UMPBuilderConsts.STA_DEFAULT;
                    item.StrStatus = string.Empty;
                    item.StrMessage = string.Empty;
                    item.Info = project;
                    mListOptObjectItems.Add(item);
                }
                for (int i = 0; i < mListFileInfos.Count; i++)
                {
                    var file = mListFileInfos[i];
                    item = new OptObjectItem();
                    item.Operation = UMPBuilderConsts.OPTOBJ_COPYFILE;
                    item.IsChecked = true;
                    item.Category = file.Category;
                    item.StrCategory = GetStrCategory(item.Category, 2);
                    item.Name = file.FileName;
                    item.Description = file.SourcePath;
                    item.Status = UMPBuilderConsts.STA_DEFAULT;
                    item.StrStatus = string.Empty;
                    item.StrMessage = string.Empty;
                    item.Info = file;
                    mListOptObjectItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void RestartBuild()
        {
            try
            {
                if (mSystemConfig == null) { return; }
                double percentage;
                int count;
                mBuildUpdater = false;
                mIsContinue = true;
                mIsOptFail = false;
                SetMyWaiter(true);
                SetBuildProgress(0);
                BorderProgressBuild.Visibility = Visibility.Visible;
                DateTime beginTime = DateTime.Now;
                mStatisticalItem.ClearValue();
                mStatisticalItem.BeginTime = beginTime.ToString("yyyy-MM-dd HH:mm:ss");
                mStatisticalItem.ProjectCount = mListProjectInfos.Count.ToString();
                mStatisticalItem.FileCount = mListFileInfos.Count.ToString();
                SetStatusMessage(string.Format("Working..."));
                WriteLog("Building", string.Format("Begin build..."));
                var setting = mSystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                if (setting != null
                    && setting.Value == "1")
                {
                    mBuildUpdater = true;
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        OperationReturn optReturn;
                        List<OptObjectItem> listTemp;


                        #region 重置状态

                        //重置状态
                        for (int i = 0; i < mListOptObjectItems.Count; i++)
                        {
                            if (!mIsContinue) { return; }
                            var item = mListOptObjectItems[i];
                            SetOptObjItemStatus(item, UMPBuilderConsts.STA_DEFAULT, string.Empty);
                        }

                        #endregion


                        #region SVN更新

                        //Svn更新
                        WriteLog("Building", string.Format("Begin SvnUpdate..."));
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 1;
                        }));
                        setting =
                            mSystemConfig.ListSettings.FirstOrDefault(p => p.Key == UMPBuilderConsts.GS_SVNUPDATEALL);
                        if (setting != null && setting.Value == "1")
                        {
                            if (!mIsContinue) { return; }
                            SetStatusMessage(string.Format("SvnUpdating {0} ...", mSystemConfig.RootDir));
                            optReturn = DoSvnUpdateRootDirectory();
                            if (!optReturn.Result)
                            {
                                ShowErrorMessage(string.Format("SvnUpdate RootDir fail.\t{0}",
                                    string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message))));
                            }

                            percentage = 1 * UMPBuilderConsts.PG_UPDATESVN +
                                         UMPBuilderConsts.PG_BASE_UPDATESVN;

                            SetBuildProgress(percentage);
                        }
                        else
                        {
                            listTemp = new List<OptObjectItem>();
                            for (int i = 0; i < mListOptObjectItems.Count; i++)
                            {
                                var item = mListOptObjectItems[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }
                                if (item.Operation != UMPBuilderConsts.OPTOBJ_SVNUPDATE) { continue; }
                                listTemp.Add(item);
                            }
                            count = listTemp.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (!mIsContinue) { return; }
                                var item = listTemp[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }

                                SetStatusMessage(string.Format("SvnUpdating {0} ...", project.ProjectName));
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                                NotifyWorkingItem(item);
                                optReturn = DoSvnUpdateSingleProject(project);
                                if (!optReturn.Result)
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                                }
                                else
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                                }

                                percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_UPDATESVN +
                                             UMPBuilderConsts.PG_BASE_UPDATESVN;
                                SetBuildProgress(percentage);
                            }
                        }
                        WriteLog("Building", string.Format("Svn update end"));

                        #endregion


                        #region 编译项目

                        //编译项目
                        WriteLog("Building", string.Format("Begin compile..."));
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 2;
                        }));
                        int duration = 60;
                        setting =
                            mSystemConfig.ListSettings.FirstOrDefault(
                                p => p.Key == UMPBuilderConsts.GS_DURATION_DEVENV);
                        if (setting != null)
                        {
                            string strValue = setting.Value;
                            int intValue;
                            if (int.TryParse(strValue, out intValue)
                                 && intValue > 0)
                            {
                                duration = intValue;
                            }
                        }
                        listTemp = new List<OptObjectItem>();
                        for (int i = 0; i < mListOptObjectItems.Count; i++)
                        {
                            var item = mListOptObjectItems[i];
                            var project = item.Info as ProjectInfo;
                            if (project == null) { continue; }
                            if (item.Operation != UMPBuilderConsts.OPTOBJ_COMPILE) { continue; }
                            listTemp.Add(item);
                        }
                        count = listTemp.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!mIsContinue) { return; }
                            var item = listTemp[i];
                            var project = item.Info as ProjectInfo;
                            if (project == null) { continue; }
                            SetStatusMessage(string.Format("Compiling {0} ...", project.ProjectName));
                            SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                            NotifyWorkingItem(item);
                            optReturn = DoCompileSingleProject(project, duration);
                            if (!optReturn.Result)
                            {
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                            }
                            else
                            {
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                            }

                            percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_PROJCOMPILE +
                                       UMPBuilderConsts.PG_BASE_PROJCOMPILE;
                            SetBuildProgress(percentage);
                        }
                        WriteLog("Building", string.Format("Compile project end."));

                        #endregion


                        #region 复制文件

                        //复制文件
                        WriteLog("Building", string.Format("Begin copy file..."));
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 3;
                        }));
                        listTemp = new List<OptObjectItem>();
                        for (int i = 0; i < mListOptObjectItems.Count; i++)
                        {
                            var item = mListOptObjectItems[i];
                            var file = item.Info as UMPFileInfo;
                            if (file == null) { continue; }
                            if (item.Operation != UMPBuilderConsts.OPTOBJ_COPYFILE) { continue; }
                            listTemp.Add(item);
                        }
                        count = listTemp.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!mIsContinue) { return; }
                            var item = listTemp[i];
                            var file = item.Info as UMPFileInfo;
                            if (file == null) { continue; }
                            SetStatusMessage(string.Format("Copying {0} ...", file.FileName));
                            SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                            NotifyWorkingItem(item);
                            optReturn = DoCopySingleUMPFile(file);
                            if (!optReturn.Result)
                            {
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                            }
                            else
                            {
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                            }

                            percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_COPYFILE +
                                      UMPBuilderConsts.PG_BASE_COPYFILE;
                            SetBuildProgress(percentage);
                        }
                        WriteLog("Building", string.Format("Copy file end."));

                        #endregion


                        #region 打包文件

                        //打包文件
                        WriteLog("Building", string.Format("Begin package file..."));
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 4;
                        }));
                        optReturn = DoPackageUMPFile();
                        if (!optReturn.Result)
                        {
                            //ShowErrorMessage(string.Format("PackageFile fail.\t{0}\t{1}", optReturn.Code,
                            //    optReturn.Message));
                            mIsOptFail = true;
                            WriteLog("Packaging", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        WriteLog("Building", string.Format("Package file end."));

                        #endregion


                        #region 生成UMPSetup 或 UMPUpdater

                        WriteLog("Building", string.Format("Begin generate UMPSetup or UMPUpdater..."));
                        if (mBuildUpdater)
                        {
                            optReturn = DoGenerateUMPUpdater();
                        }
                        else
                        {
                            optReturn = DoGenerateUMPSetup();
                        }
                        if (!optReturn.Result)
                        {
                            mIsOptFail = true;
                            WriteLog("Other", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        WriteLog("Building", string.Format("Generate UMPSetup or UMPUpdater end."));

                        #endregion


                        #region Statistical

                        //统计
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 5;
                        }));

                        #endregion

                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }

                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetMyWaiter(false);
                    SetStatusMessage(string.Empty);

                    mIsContinue = false;
                    DateTime endTime = DateTime.Now;
                    mStatisticalItem.EndTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                    TimeSpan ts = endTime - beginTime;
                    mStatisticalItem.Duration = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes.ToString("00"),
                        ts.Seconds.ToString("00"));
                    if (mIsOptFail)
                    {
                        ShowErrorMessage(string.Format("End with some errors."));
                    }
                    else
                    {
                        ShowInfoMessage(string.Format("Build End"));
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void StartBuild()
        {
            try
            {
                if (mSystemConfig == null) { return; }
                OptObjectItem optObjItem = null;
                var tabIndex = TabControlMain.SelectedIndex;
                if (tabIndex < 1)
                {
                    tabIndex = 1;
                }
                var tab = TabControlMain.Items[tabIndex] as TabItem;
                if (tab != null)
                {
                    var lister = tab.Content as IOptObjectLister;
                    if (lister != null)
                    {
                        optObjItem = lister.GetSelectedItem();
                    }
                }
                if (optObjItem == null)
                {
                    optObjItem = mListOptObjectItems.FirstOrDefault();
                }
                if (optObjItem == null) { return; }
                int optItemIndex = mListOptObjectItems.IndexOf(optObjItem);
                if (optItemIndex < 0) { return; }

                mBuildUpdater = false;
                mIsContinue = true;
                double percentage;
                int count;
                SetMyWaiter(true);
                SetBuildProgress(0);
                BorderProgressBuild.Visibility = Visibility.Visible;
                DateTime beginTime = DateTime.Now;
                mStatisticalItem.ClearValue();
                mStatisticalItem.BeginTime = beginTime.ToString("yyyy-MM-dd HH:mm:ss");
                mStatisticalItem.ProjectCount = mListProjectInfos.Count.ToString();
                mStatisticalItem.FileCount = mListFileInfos.Count.ToString();
                SetStatusMessage(string.Format("Working..."));
                WriteLog("Building", string.Format("Begin build..."));
                var setting = mSystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                if (setting != null
                    && setting.Value == "1")
                {
                    mBuildUpdater = true;
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        OperationReturn optReturn;


                        #region 重置状态

                        //重置状态
                        for (int i = 0; i < mListOptObjectItems.Count; i++)
                        {
                            if (!mIsContinue) { return; }
                            var item = mListOptObjectItems[i];
                            SetOptObjItemStatus(item, UMPBuilderConsts.STA_DEFAULT, string.Empty);
                        }

                        #endregion


                        #region SVN更新

                        if (tabIndex <= 1)
                        {
                            //Svn更新
                            WriteLog("Building", string.Format("Begin SvnUpdate..."));
                            Dispatcher.Invoke(new Action(() =>
                            {
                                TabControlMain.SelectedIndex = 1;
                            }));
                            List<OptObjectItem> listTemp = new List<OptObjectItem>();
                            for (int i = 0; i < mListOptObjectItems.Count; i++)
                            {
                                if (!mIsContinue) { return; }
                                if (i < optItemIndex) { continue; }
                                var item = mListOptObjectItems[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }
                                if (item.Operation != UMPBuilderConsts.OPTOBJ_SVNUPDATE) { continue; }
                                listTemp.Add(item);
                            }
                            count = listTemp.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (!mIsContinue) { return; }
                                var item = listTemp[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }

                                SetStatusMessage(string.Format("SvnUpdating {0} ...", project.ProjectName));
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                                NotifyWorkingItem(item);
                                optReturn = DoSvnUpdateSingleProject(project);
                                if (!optReturn.Result)
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                                }
                                else
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                                }

                                percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_UPDATESVN +
                                             UMPBuilderConsts.PG_BASE_UPDATESVN;
                                SetBuildProgress(percentage);
                            }
                            WriteLog("Building", string.Format("Svn update end"));
                        }

                        #endregion


                        #region 编译项目

                        if (tabIndex <= 2)
                        {
                            //编译项目
                            WriteLog("Building", string.Format("Begin compile..."));
                            Dispatcher.Invoke(new Action(() =>
                            {
                                TabControlMain.SelectedIndex = 2;
                            }));
                            int duration = 60;
                            setting =
                                 mSystemConfig.ListSettings.FirstOrDefault(
                                     p => p.Key == UMPBuilderConsts.GS_DURATION_DEVENV);
                            if (setting != null)
                            {
                                string strValue = setting.Value;
                                int intValue;
                                if (int.TryParse(strValue, out intValue)
                                     && intValue > 0)
                                {
                                    duration = intValue;
                                }
                            }
                            List<OptObjectItem> listTemp = new List<OptObjectItem>();
                            for (int i = 0; i < mListOptObjectItems.Count; i++)
                            {
                                if (i < optItemIndex) { continue; }
                                var item = mListOptObjectItems[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }
                                if (item.Operation != UMPBuilderConsts.OPTOBJ_COMPILE) { continue; }
                                listTemp.Add(item);
                            }
                            count = listTemp.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (!mIsContinue) { return; }
                                var item = listTemp[i];
                                var project = item.Info as ProjectInfo;
                                if (project == null) { continue; }
                                SetStatusMessage(string.Format("Compiling {0} ...", project.ProjectName));
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                                NotifyWorkingItem(item);
                                optReturn = DoCompileSingleProject(project, duration);
                                if (!optReturn.Result)
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                                }
                                else
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                                }

                                percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_PROJCOMPILE +
                                           UMPBuilderConsts.PG_BASE_PROJCOMPILE;
                                SetBuildProgress(percentage);
                            }
                            WriteLog("Building", string.Format("Compile project end."));
                        }

                        #endregion


                        #region 复制文件

                        if (tabIndex <= 3)
                        {
                            //复制文件
                            WriteLog("Building", string.Format("Begin copy file..."));
                            Dispatcher.Invoke(new Action(() =>
                            {
                                TabControlMain.SelectedIndex = 3;
                            }));
                            List<OptObjectItem> listTemp = new List<OptObjectItem>();
                            for (int i = 0; i < mListOptObjectItems.Count; i++)
                            {
                                if (i < optItemIndex) { continue; }
                                var item = mListOptObjectItems[i];
                                var file = item.Info as UMPFileInfo;
                                if (file == null) { continue; }
                                if (item.Operation != UMPBuilderConsts.OPTOBJ_COPYFILE) { continue; }
                                listTemp.Add(item);
                            }
                            count = listTemp.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (!mIsContinue) { return; }
                                var item = listTemp[i];
                                var file = item.Info as UMPFileInfo;
                                if (file == null) { continue; }
                                SetStatusMessage(string.Format("Copying {0} ...", file.FileName));
                                SetOptObjItemStatus(item, UMPBuilderConsts.STA_WORKING, string.Empty);
                                NotifyWorkingItem(item);
                                optReturn = DoCopySingleUMPFile(file);
                                if (!optReturn.Result)
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_FAIL, string.Format("{0}\t{1}", optReturn.Code, GetErrorString(optReturn.Code, optReturn.Message)));
                                }
                                else
                                {
                                    SetOptObjItemStatus(item, UMPBuilderConsts.STA_SUCCESS, string.Empty);
                                }

                                percentage = ((i + 1) / (count * 1.0)) * UMPBuilderConsts.PG_COPYFILE +
                                          UMPBuilderConsts.PG_BASE_COPYFILE;
                                SetBuildProgress(percentage);
                            }
                            WriteLog("Building", string.Format("Copy file end."));
                        }

                        #endregion


                        #region 打包文件

                        if (tabIndex <= 4)
                        {
                            //打包文件
                            WriteLog("Building", string.Format("Begin package file..."));
                            Dispatcher.Invoke(new Action(() =>
                            {
                                TabControlMain.SelectedIndex = 4;
                            }));
                            optReturn = DoPackageUMPFile();
                            if (!optReturn.Result)
                            {
                                //ShowErrorMessage(string.Format("PackageFile fail.\t{0}\t{1}", optReturn.Code,
                                //    optReturn.Message));
                                mIsOptFail = true;
                                WriteLog("Packaging", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            WriteLog("Building", string.Format("Package file end."));
                        }

                        #endregion


                        #region 生成UMPSetup 或 UMPUpdater

                        WriteLog("Building", string.Format("Begin generate UMPSetup or UMPUpdater..."));
                        if (mBuildUpdater)
                        {
                            optReturn = DoGenerateUMPUpdater();
                        }
                        else
                        {
                            optReturn = DoGenerateUMPSetup();
                        }
                        if (!optReturn.Result)
                        {
                            mIsOptFail = true;
                            WriteLog("Other", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        WriteLog("Building", string.Format("Generate UMPSetup or UMPUpdater end."));

                        #endregion


                        #region Statistical

                        //统计
                        Dispatcher.Invoke(new Action(() =>
                        {
                            TabControlMain.SelectedIndex = 5;
                        }));

                        #endregion

                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetMyWaiter(false);
                    SetStatusMessage(string.Empty);

                    mIsContinue = false;
                    DateTime endTime = DateTime.Now;
                    mStatisticalItem.EndTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                    TimeSpan ts = endTime - beginTime;
                    mStatisticalItem.Duration = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes.ToString("00"),
                        ts.Seconds.ToString("00"));
                    if (mIsOptFail)
                    {
                        ShowErrorMessage(string.Format("End with some errors."));
                    }
                    else
                    {
                        ShowInfoMessage(string.Format("Build End"));
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void PauseBuild()
        {
            mIsContinue = false;
        }

        private void StopBuild()
        {
            mIsContinue = false;
        }

        private void PreOperation()
        {
            try
            {
                var index = TabControlMain.SelectedIndex;
                if (index >= 1)
                {
                    index--;
                }
                TabControlMain.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NextOperation()
        {
            try
            {
                var index = TabControlMain.SelectedIndex;
                if (index < TabControlMain.Items.Count - 1)
                {
                    index++;
                }
                TabControlMain.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        public void ShowSettingsWindow()
        {
            try
            {
                SettingsWindow win = new SettingsWindow();
                win.PageParent = this;
                win.SystemConfig = mSystemConfig;
                var result = win.ShowDialog();
                if (result == true)
                {
                    CreateStartPage();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private OperationReturn DoSvnUpdateRootDirectory()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!mIsContinue)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                    optReturn.Message = string.Format("User cancel");
                    return optReturn;
                }
                string strRootDir = mSystemConfig.RootDir;
                string strArgs =
                    string.Format("/command:update /path:\"{0}\" /closeonend:1",
                        strRootDir);
                string strExeFile = mSystemConfig.SvnProcPath;
                Process process = new Process();
                process.StartInfo.FileName = strExeFile;
                process.StartInfo.Arguments = strArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                int duration = 20 * 60;
                var setting =
                    mSystemConfig.ListSettings.FirstOrDefault(
                        s => s.Key == UMPBuilderConsts.GS_DURATION_SVNUPDATEROOTDIR);
                if (setting != null)
                {
                    string strValue = setting.Value;
                    int intValue;
                    if (int.TryParse(strValue, out intValue)
                         && intValue > 0)
                    {
                        duration = intValue;
                    }
                }
                process.WaitForExit(duration * 1000);
                if (!process.HasExited)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_FAIL;
                    optReturn.Message = string.Format("Exit fail");
                    return optReturn;
                }
                int intRet = process.ExitCode;
                if (intRet > 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_SVNPROC_FAIL + intRet;
                    optReturn.Message = string.Format("SvnUpdate fail");
                    return optReturn;
                }
                process.Dispose();
                optReturn.Message = string.Format("SvnUpdate successful");
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoSvnUpdateSingleProject(ProjectInfo projectItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!mIsContinue)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                    optReturn.Message = string.Format("User cancel");
                    return optReturn;
                }
                string strRootDir = mSystemConfig.RootDir;
                string strProjectDir = Path.Combine(strRootDir, projectItem.ProjectDir);
                string strArgs =
                    string.Format("/command:update /path:\"{0}\" /closeonend:1",
                        strProjectDir);
                string strExeFile = mSystemConfig.SvnProcPath;
                Process process = new Process();
                process.StartInfo.FileName = strExeFile;
                process.StartInfo.Arguments = strArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                int duration = 60;
                var setting =
                    mSystemConfig.ListSettings.FirstOrDefault(
                        s => s.Key == UMPBuilderConsts.GS_DURATION_SVNUPDATE);
                if (setting != null)
                {
                    string strValue = setting.Value;
                    int intValue;
                    if (int.TryParse(strValue, out intValue)
                        && intValue > 0)
                    {
                        duration = intValue;
                    }
                }
                process.WaitForExit(duration * 1000);
                if (!process.HasExited)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_FAIL;
                    optReturn.Message = string.Format("Exit fail");
                    return optReturn;
                }
                int intRet = process.ExitCode;
                if (intRet > 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_SVNPROC_FAIL + intRet;
                    optReturn.Message = string.Format("SvnUpdate fail");
                    return optReturn;
                }
                process.Dispose();
                optReturn.Message = string.Format("SvnUpdate successful");
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoCompileSingleProject(ProjectInfo projectItem, int duration)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!mIsContinue)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                    optReturn.Message = string.Format("User cancel");
                    return optReturn;
                }
                string strRootDir = mSystemConfig.RootDir;
                string strProjectName = projectItem.ProjectName;
                string strProjectDir = Path.Combine(strRootDir, projectItem.ProjectDir);
                string strProjectFile = Path.Combine(strProjectDir, projectItem.ProjectFile);
                string strBuildInfoPath = Path.Combine(strProjectDir, "BuildInfo.txt");
                if (File.Exists(strBuildInfoPath))
                {
                    File.Delete(strBuildInfoPath);
                }
                string strExeFile = mSystemConfig.CompilerPath;
                string strArgs =
                    string.Format("\"{0}\" /rebuild \"Release|AnyCPU\" /project {1} /out \"{2}\"",
                        strProjectFile, strProjectName, strBuildInfoPath);
                Process process = new Process();
                process.StartInfo.FileName = strExeFile;
                process.StartInfo.Arguments = strArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                //int duration = 60;
                //var setting =
                //    mSystemConfig.ListSettings.FirstOrDefault(
                //        s => s.Key == UMPBuilderConsts.GS_DURATION_DEVENV);
                //if (setting != null)
                //{
                //    string strValue = setting.Value;
                //    int intValue;
                //    if (int.TryParse(strValue, out intValue)
                //         && intValue > 0)
                //    {
                //        duration = intValue;
                //    }
                //}
                process.WaitForExit(duration * 1000);
                if (!process.HasExited)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_FAIL;
                    optReturn.Message = string.Format("Exit fail");
                    return optReturn;
                }
                int intRet = process.ExitCode;
                if (intRet > 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_DEVENV_FAIL + intRet;
                    optReturn.Message = "Compile fail. Refer to BuildInfo.";
                    if (File.Exists(strBuildInfoPath))
                    {
                        string content = File.ReadAllText(strBuildInfoPath, Encoding.Default);
                        optReturn.Message = content;
                    }
                    return optReturn;
                }
                process.Dispose();
                optReturn.Message = string.Format("Compile successful");
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoCopySingleUMPFile(UMPFileInfo fileItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!mIsContinue)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                    optReturn.Message = string.Format("User cancel");
                    return optReturn;
                }
                string strRootDir = mSystemConfig.RootDir;
                string sourcePath = Path.Combine(strRootDir, fileItem.SourcePath);
                string targetPath = mSystemConfig.CopyDir;
                var setting = mSystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                if (setting != null && setting.Value == "1")
                {
                    targetPath = mSystemConfig.UpdateDir;
                    targetPath = Path.Combine(targetPath, "UMP");
                }
                switch (fileItem.Category)
                {
                    case 1:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCFSERVICES,
                            UMPBuilderConsts.DIR_BIN);
                        break;
                    case 2:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCF2CLIENT,
                            UMPBuilderConsts.DIR_BIN);
                        break;
                    case 3:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WINSERVICES);
                        break;
                    case 4:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCFSERVICES);
                        break;
                    case 5:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCF2CLIENT);
                        break;
                    case 6:
                        
                        break;
                    case 7:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_MANAGEMENT);
                        break;
                    case 8:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCF1600,
                            UMPBuilderConsts.DIR_BIN);
                        break;
                    case 9:
                        targetPath = Path.Combine(targetPath, UMPBuilderConsts.DIR_WCF1600);
                        break;
                }
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                string strExeFile = "xcopy";
                string strArgs = string.Format("/y /f \"{0}\" \"{1}\"",
                    sourcePath,
                    targetPath);
                Process process = new Process();
                process.StartInfo.FileName = strExeFile;
                process.StartInfo.Arguments = strArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                int duration = 10;
                setting =
                    mSystemConfig.ListSettings.FirstOrDefault(
                        s => s.Key == UMPBuilderConsts.GS_DURATION_XCOPYDURATION);
                if (setting != null)
                {
                    string strValue = setting.Value;
                    int intValue;
                    if (int.TryParse(strValue, out intValue)
                         && intValue > 0)
                    {
                        duration = intValue;
                    }
                }
                process.WaitForExit(duration * 1000);
                if (!process.HasExited)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_EXIT_FAIL;
                    optReturn.Message = string.Format("Exit fail");
                    return optReturn;
                }
                int intRet = process.ExitCode;
                if (intRet > 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = UMPBuilderConsts.RET_XCOPY_FAIL + intRet;
                    optReturn.Message = string.Format("Exe fail");
                    return optReturn;
                }
                process.Dispose();
                optReturn.Message = "Copy file successful";
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoPackageUMPFile()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strPackageDir = mSystemConfig.RootDir;
                if (mBuildUpdater)
                {
                    strPackageDir = Path.Combine(strPackageDir, "Other\\UMPSetup\\UMPUpdater\\Resources");
                }
                else
                {
                    strPackageDir = Path.Combine(strPackageDir, "Other\\UMPSetup\\UMPSetup\\Resources");
                }
                if (!Directory.Exists(strPackageDir))
                {
                    Directory.CreateDirectory(strPackageDir);
                }
                string strPackageFile = Path.Combine(strPackageDir, UMPBuilderConsts.FILE_NAME_PACKAGEFILE);
                ZipOutputStream stream = new ZipOutputStream(File.Create(strPackageFile));
                stream.SetLevel(9);
                string strDir;
                if (mBuildUpdater)
                {
                    strDir = mSystemConfig.UpdateDir;
                }
                else
                {
                    strDir = mSystemConfig.CopyDir;
                }
                mPackagedSize = 0;
                mPackageCount = 0;
                mPackageTotalSize = 0;
                if (Directory.Exists(strDir))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(strDir);
                    long totalSize = GetDirSize(dirInfo);
                    mPackageTotalSize = totalSize;
                }
                optReturn = DoPackageUMPFile(strDir, string.Empty, stream);
                stream.Flush();
                stream.Close();

                //复制UpdateInfo.xml到UMPUpdater编译目录
                if (mBuildUpdater)
                {
                    string strUpdateInfoFile = Path.Combine(strDir, UMPBuilderConsts.FILE_NAME_UPDATEINFO);
                    if (!File.Exists(strUpdateInfoFile))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                        optReturn.Message = string.Format("UpdateInfo.xml not exist.\t{0}", strUpdateInfoFile);
                        return optReturn;
                    }
                    string strTargetDir = mSystemConfig.RootDir;
                    strTargetDir = Path.Combine(strTargetDir, "Other\\UMPSetup\\UMPUpdater");
                    if (!Directory.Exists(strTargetDir))
                    {
                        Directory.CreateDirectory(strTargetDir);
                    }
                    string strTargetFile = Path.Combine(strTargetDir, UMPBuilderConsts.FILE_NAME_UPDATEINFO);
                    File.Copy(strUpdateInfoFile, strTargetFile, true);

                    WriteLog("Building", string.Format("Copy UpdateInfo.xml end.\t{0}", strTargetFile));
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoPackageUMPFile(string strDir, string strName, ZipOutputStream stream)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!Directory.Exists(strDir))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Directory not exist.\t{0}", strDir);
                    return optReturn;
                }
                DirectoryInfo dirInfo = new DirectoryInfo(strDir);
                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (!mIsContinue)
                    {
                        optReturn.Result = false;
                        optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                        optReturn.Message = string.Format("User cancel");
                        return optReturn;
                    }
                    string name = string.Format("{0}{1}", string.IsNullOrEmpty(strName) ? string.Empty : strName + "\\",
                         dirs[i].Name);
                    optReturn = DoPackageUMPFile(dirs[i].FullName, name, stream);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                double percentage;
                FileInfo[] files = dirInfo.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    if (!mIsContinue)
                    {
                        optReturn.Result = false;
                        optReturn.Code = UMPBuilderConsts.RET_EXIT_USERCANCEL;
                        optReturn.Message = string.Format("User cancel");
                        return optReturn;
                    }
                    FileInfo file = files[i];
                    SetStatusMessage(string.Format("Packaging {0} ...", file.Name));
                    FileStream fs = File.OpenRead(file.FullName);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string name = string.Format("{0}{1}", string.IsNullOrEmpty(strName) ? string.Empty : strName + "\\",
                        file.Name);
                    ZipEntry entry = new ZipEntry(name);
                    entry.DateTime = file.LastWriteTime;
                    stream.PutNextEntry(entry);
                    stream.Write(buffer, 0, buffer.Length);
                    mPackagedSize += file.Length;
                    mPackageCount++;
                    NotifyProgress(file);

                    percentage = (mPackagedSize / (mPackageTotalSize * 1.0)) * UMPBuilderConsts.PG_PACKAGE +
                                 UMPBuilderConsts.PG_BASE_PACKAGE;
                    SetBuildProgress(percentage);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoGenerateUMPSetup()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //编译UMPSetup
                ProjectInfo info = new ProjectInfo();
                info.ProjectName = "UMPSetup";
                info.ProjectType = (int)ProjectType.WpfApp;
                info.ProjectDir = "Other\\UMPSetup\\UMPSetup";
                info.ProjectFile = "UMPSetup.csproj";
                SetStatusMessage(string.Format("Compiling UMPSetup..."));
                optReturn = DoCompileSingleProject(info, 60 * 10);  //编译UMPSetup耗时比较久，这里等待10分钟
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                double percentage = 0.8 * UMPBuilderConsts.PG_OTHER + UMPBuilderConsts.PG_BASE_OTHER;
                SetBuildProgress(percentage);

                //复制UMPSetup.exe到打包目录
                string strSourceDir = mSystemConfig.RootDir;
                string strSourceFile = Path.Combine(strSourceDir, "Other\\UMPSetup\\UMPSetup\\bin\\Release\\UMPSetup.exe");
                if (!File.Exists(strSourceFile))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMPSetup.exe not exist.\t{0}", strSourceFile);
                    return optReturn;
                }
                string strTargetDir = mSystemConfig.PackageDir;
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetDir);
                }
                string strTargetFile = Path.Combine(strTargetDir, "UMPSetup.exe");
                SetStatusMessage(string.Format("Copying UMPSetup.exe..."));
                File.Copy(strSourceFile, strTargetFile, true);

                percentage = 1.0 * UMPBuilderConsts.PG_OTHER + UMPBuilderConsts.PG_BASE_OTHER;
                SetBuildProgress(percentage);

                Dispatcher.Invoke(new Action(() =>
                {
                    mStatisticalItem.PackageFile = strTargetFile;
                }));
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn DoGenerateUMPUpdater()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //编译UMPUpdater
                ProjectInfo info = new ProjectInfo();
                info.ProjectName = "UMPUpdater";
                info.ProjectType = (int)ProjectType.WpfApp;
                info.ProjectDir = "Other\\UMPSetup\\UMPUpdater";
                info.ProjectFile = "UMPUpdater.csproj";
                SetStatusMessage(string.Format("Compiling UMPUpdater..."));
                optReturn = DoCompileSingleProject(info, 60 * 10);  //编译UMPUpdater耗时比较久，这里等待10分钟
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                double percentage = 0.8 * UMPBuilderConsts.PG_OTHER + UMPBuilderConsts.PG_BASE_OTHER;
                SetBuildProgress(percentage);

                //复制UMPUpdater.exe到打包目录
                string strSourceDir = mSystemConfig.RootDir;
                string strSourceFile = Path.Combine(strSourceDir, "Other\\UMPSetup\\UMPUpdater\\bin\\Release\\UMPUpdater.exe");
                if (!File.Exists(strSourceFile))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMPUpdater.exe not exist.\t{0}", strSourceFile);
                    return optReturn;
                }
                string strTargetDir = mSystemConfig.PackageDir;
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetDir);
                }
                string strTargetFile = Path.Combine(strTargetDir, "UMPUpdater.exe");
                SetStatusMessage(string.Format("Copying UMPUpdater.exe..."));
                File.Copy(strSourceFile, strTargetFile, true);

                percentage = 1.0 * UMPBuilderConsts.PG_OTHER + UMPBuilderConsts.PG_BASE_OTHER;
                SetBuildProgress(percentage);

                Dispatcher.Invoke(new Action(() =>
                {
                    mStatisticalItem.PackageFile = strTargetFile;
                }));
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private void NotifyProgress(FileInfo file)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                mStatisticalItem.PackageSize = mPackagedSize.ToString();
                mStatisticalItem.PackageCount = mPackageCount.ToString();
                if (TabControlMain.SelectedIndex == 4)
                {
                    var tab = TabControlMain.Items[4] as TabItem;
                    if (tab != null)
                    {
                        var uc = tab.Content as UCFilePackage;
                        if (uc != null)
                        {
                            string msg = string.Format("Packaging file... \t{0}", file.FullName);
                            uc.SetProgress(mPackagedSize, msg);
                        }
                    }
                }
            }));
        }

        private void NotifyWorkingItem(OptObjectItem item)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var tab = TabControlMain.SelectedItem as TabItem;
                if (tab != null)
                {
                    IOptObjectLister lister = tab.Content as IOptObjectLister;
                    if (lister != null)
                    {
                        lister.SetSelectedItem(item);
                    }
                }
            }));
        }

        private long GetDirSize(DirectoryInfo dirInfo)
        {
            long size = 0;
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                size += GetDirSize(dir);
            }
            FileInfo[] files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                size += file.Length;
            }
            return size;
        }

        #endregion


        #region Others

        private void CreateStartPage()
        {
            try
            {
                UCStartPage uc = new UCStartPage();
                uc.PageParent = this;
                uc.SystemConfig = mSystemConfig;
                TabStartPage.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateSvnUpdatePage()
        {
            try
            {
                UCSvnUpdate uc = new UCSvnUpdate();
                uc.PageParent = this;
                uc.ListOptObjects = mListOptObjectItems;
                TabSvnUpdate.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateComilePage()
        {
            try
            {
                UCProjectCompile uc = new UCProjectCompile();
                uc.PageParent = this;
                uc.ListOptObjects = mListOptObjectItems;
                TabCompile.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateFileCopyPage()
        {
            try
            {
                UCFileCopy uc = new UCFileCopy();
                uc.PageParent = this;
                uc.ListOptObjects = mListOptObjectItems;
                TabCopyFile.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateFilePackagePage()
        {
            try
            {
                UCFilePackage uc = new UCFilePackage();
                uc.PageParent = this;
                uc.SystemConfig = mSystemConfig;
                TabPackageFile.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateStatisticalPage()
        {
            try
            {
                UCStatisticalInfo uc = new UCStatisticalInfo();
                uc.PageParent = this;
                uc.StatistialItem = mStatisticalItem;
                TabStatisticalPage.Content = uc;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateOptButtons()
        {
            try
            {
                PanelOperationButton.Children.Clear();
                Button btn;
                OptButtonItem item;
                for (int i = 0; i < mListOptButtons.Count; i++)
                {
                    item = mListOptButtons[i];
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelOperationButton.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        public void SetMyWaiter(bool isShow)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MyWaiter.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
            }));
        }

        public void SetStatusMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtStatus.Text = msg;
            }));
        }

        private void SetOptObjItemStatus(OptObjectItem item, int status, string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                item.Status = status;
                item.StrStatus = GetStrStatus(item.Status);
                item.StrMessage = msg;
            }));
        }

        public void ShowErrorMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, mAppName, MessageBoxButton.OK, MessageBoxImage.Error)));
        }

        public void ShowInfoMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, mAppName, MessageBoxButton.OK, MessageBoxImage.Information)));
        }

        private string GetStrStatus(int status)
        {
            string strReturn = string.Empty;
            switch (status)
            {
                case UMPBuilderConsts.STA_DEFAULT:
                    strReturn = string.Empty;
                    break;
                case UMPBuilderConsts.STA_WORKING:
                    strReturn = "Working...";
                    break;
                case UMPBuilderConsts.STA_SUCCESS:
                    strReturn = "Success";
                    break;
                case UMPBuilderConsts.STA_FAIL:
                    strReturn = "Fail";
                    break;
                case UMPBuilderConsts.STA_WAITING:
                    strReturn = "Waiting...";
                    break;
            }
            return strReturn;
        }

        private string GetErrorString(int code, string msg)
        {
            string strReturn = msg;
            switch (code)
            {
                case UMPBuilderConsts.RET_EXIT_FAIL:
                    strReturn = string.Format("Exe exit fail.");
                    break;
                case UMPBuilderConsts.RET_EXIT_USERCANCEL:
                    strReturn = string.Format("User cancel.");
                    break;
                case UMPBuilderConsts.RET_DEVENV_COMMON:
                    strReturn = string.Format("Compile fail.\t{0}", msg);
                    break;
                case UMPBuilderConsts.RET_XCOPY_SOURCEFILENOTEXIST:
                    strReturn = string.Format("Copy fail. Source file not exist");
                    break;
            }
            return strReturn;
        }

        private string GetStrCategory(int category, int type)
        {
            string strReturn = string.Empty;
            if (type == 1)
            {
                switch (category)
                {
                    case (int)ProjectType.Library:
                        strReturn = "Library";
                        break;
                    case (int)ProjectType.WcfService:
                        strReturn = "WcfService";
                        break;
                    case (int)ProjectType.WinService:
                        strReturn = "WinService";
                        break;
                    case (int)ProjectType.WpfApp:
                        strReturn = "WpfApp";
                        break;
                }
            }
            if (type == 2)
            {
                switch (category)
                {
                    case 0:
                        strReturn = string.Empty;
                        break;
                    case 1:
                        strReturn = "WcfServices\\bin";
                        break;
                    case 2:
                        strReturn = "Wcf2Client\\bin";
                        break;
                    case 3:
                        strReturn = "WinServices";
                        break;
                    case 4:
                        strReturn = "WcfServices";
                        break;
                    case 5:
                        strReturn = "Wcf2Client";
                        break;
                    case 6:
                        strReturn = "Root";
                        break;
                    case 7:
                        strReturn = "ManagementMaintenance";
                        break;
                    case 8:
                        strReturn = "WCF1600\\bin";
                        break;
                    case 9:
                        strReturn = "WCF1600";
                        break;
                }
            }
            return strReturn;
        }

        private void SetBuildProgress(double value)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressBuild.Value = value;
                string txt = string.Format("{0} %", value.ToString("0.00"));
                TxtProgress.Text = txt;
            }));
        }

        #endregion


        #region EventHandlers

        void OptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn == null) { return; }
                var opt = btn.DataContext as OptButtonItem;
                if (opt == null) { return; }
                switch (opt.Name)
                {
                    case UMPBuilderConsts.OPT_NAME_RESTART:
                        RestartBuild();
                        break;
                    case UMPBuilderConsts.OPT_NAME_START:
                        StartBuild();
                        break;
                    case UMPBuilderConsts.OPT_NAME_PAUSE:
                        PauseBuild();
                        break;
                    case UMPBuilderConsts.OPT_NAME_STOP:
                        StopBuild();
                        break;
                    case UMPBuilderConsts.OPT_NAME_PRE:
                        PreOperation();
                        break;
                    case UMPBuilderConsts.OPT_NAME_NEXT:
                        NextOperation();
                        break;
                    case UMPBuilderConsts.OPT_NAME_SETTING:
                        ShowSettingsWindow();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region LogOperator

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("AppInfo\r\n");
                strInfo += string.Format("\tLogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog("AppLoad", strInfo);
            }
            catch { }
        }

        private void WriteLog(string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }

        public void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, mAppName, msg);
            }
        }

        #endregion

    }
}
