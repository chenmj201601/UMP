//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29e5d96b-9405-4dab-9a99-8f3d678446b6
//        CLR Version:              4.0.30319.18063
//        Name:                     SSMMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                SSMMainPage
//
//        created by Charley at 2015/11/4 17:34:32
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UMPS3101.Commands;
using UMPS3101.Models;
using UMPS3101.Wcf11012;
using UMPS3101.Wcf31011;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101
{
    /// <summary>
    /// SSMMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class SSMMainPage
    {

        #region Members

        /// <summary>
        /// 当前用户操作权限
        /// </summary>
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private BackgroundWorker mWorker;
        private ObservableCollection<OperationInfo> mListBasicOperations;
        private ObservableCollection<ViewColumnInfo> mListGridColumns;
        private ObservableCollection<ScoreSheetItem> mListScoreSheets;
        public ScoreSheetItem mCurrentScoreSheetItem;

        #endregion


        public SSMMainPage()
        {
            InitializeComponent();

            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListGridColumns = new ObservableCollection<ViewColumnInfo>();
            mListScoreSheets = new ObservableCollection<ScoreSheetItem>();

            LvScoreSheets.SelectionChanged+=LvScoreSheets_SelectionChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "SSMMainPage";
                StylePath = "UMPS3101/SSMMainPage.xaml";

                base.Init();

                LvScoreSheets.ItemsSource = mListScoreSheets;
                BindCommands();

                MyWaiter.Visibility = Visibility.Visible;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    SendLoadedMessage();

                    InitOperations();
                    InitColumnData();
                    LoadScoreSheets();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;

                    InitColumns();
                    InitBasicOperations();
                    CreateOptButtons();

                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBindings.Add(
                new CommandBinding(SSMMainPageCommands.CreateScoreSheetCommand,
                    CreateScoreSheetCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
                new CommandBinding(SSMMainPageCommands.ModifyScoreSheetCommand,
                     ModifyScoreSheetCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
                new CommandBinding(SSMMainPageCommands.DeleteScoreSheetCommand,
                    DeleteScoreSheetCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
              new CommandBinding(SSMMainPageCommands.SetManageUserCommand,
                  SetManageUserCommand_Executed,
                  (s, e) => e.CanExecute = true));
            CommandBindings.Add(
                new CommandBinding(SSMMainPageCommands.ImportScoreSheetCommand,
                    ImportScoreSheetCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
                new CommandBinding(SSMMainPageCommands.ExportScoreSheetCommand,
                    ExportScoreSheetCommand_Executed,
                    (s, e) => e.CanExecute = true));
        }

        private void InitOperations()
        {
            try
            {
                ListOperations.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3101");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                App.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitColumnData()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3101001");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                App.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("No columns"));
                    return;
                }
                mListGridColumns.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo column = optReturn.Data as ViewColumnInfo;
                    if (column != null) { mListGridColumns.Add(column); }
                }

                App.WriteLog("PageLoad", string.Format("Load ViewColumn"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadScoreSheets()
        {
            try
            {
                ClearChildItem();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3101Codes.GetScoreSheetList;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<string> listScoreSheet = webReturn.ListData;
                OperationReturn optReturn;
                for (int i = 0; i < listScoreSheet.Count; i++)
                {
                    string strScoreSheetInfo = listScoreSheet[i];
                    optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strScoreSheetInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo scoreSheetInfo = optReturn.Data as BasicScoreSheetInfo;
                    if (scoreSheetInfo == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tScoreSheetInfo is null"));
                        return;
                    }
                    ScoreSheetItem scoreSheetItem = new ScoreSheetItem();
                    scoreSheetItem.ID = scoreSheetInfo.ID;
                    scoreSheetItem.Name = scoreSheetInfo.Name;
                    scoreSheetItem.State = scoreSheetInfo.State;
                    scoreSheetItem.TotalScore = scoreSheetInfo.TotalScore;
                    scoreSheetItem.ItemCount = scoreSheetInfo.ItemCount;
                    scoreSheetItem.ViewClassic = scoreSheetInfo.ViewClassic;
                    scoreSheetItem.ScoreType = scoreSheetInfo.ScoreType;
                    scoreSheetItem.UseFlag = scoreSheetInfo.UseFlag;
                    scoreSheetItem.Description = scoreSheetInfo.Description;
                    scoreSheetItem.TipState = "State:" + scoreSheetItem.State;
                    scoreSheetItem.TipViewClassic = "ViewClassic" + scoreSheetItem.ViewClassic;
                    scoreSheetItem.TipScoreType = "ScoreType:" + scoreSheetItem.ScoreType;
                    scoreSheetItem.Data = scoreSheetInfo;
                    AddChildItem(scoreSheetItem);
                }

                App.WriteLog("PageLoad", string.Format("Load ScoreSheet"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListGridColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListGridColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        if (columnInfo.ColumnName == "Operation")
                        {
                            gvch.Content = string.Empty;
                        }
                        else
                        {
                            gvch.Content = App.GetLanguageInfo(string.Format("COL3101001{0}", columnInfo.ColumnName), columnInfo.ColumnName);
                        }
                        gvch.ToolTip = App.GetLanguageInfo(string.Format("COL3101001{0}", columnInfo.ColumnName), columnInfo.ColumnName);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        if (columnInfo.ColumnName == "State"
                            || columnInfo.ColumnName == "ViewClassic"
                            || columnInfo.ColumnName == "ScoreType"
                            || columnInfo.ColumnName == "Operation")
                        {
                            DataTemplate dt;
                            if (columnInfo.ColumnName == "State")
                            {
                                dt = Resources["CellStateTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                            if (columnInfo.ColumnName == "ViewClassic")
                            {
                                dt = Resources["CellViewClassicTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                            if (columnInfo.ColumnName == "ScoreType")
                            {
                                dt = Resources["CellScoreTypeTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                            if (columnInfo.ColumnName == "Operation")
                            {
                                dt = Resources["CellOperationTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvScoreSheets.View = gv;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitBasicOperations()
        {
            mListBasicOperations.Clear();
            //if (mCurrentScoreSheetItem == null) { return; }
            for (int i = 0; i < ListOperations.Count; i++)
            {
                mListBasicOperations.Add(ListOperations[i]);
            }
        }

        #endregion


        #region EventHandlers

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            try
            {
                switch (e.Code)
                {
                    //切换主题
                    case 100:
                        ThemeInfo themeInfo = e.Data as ThemeInfo;
                        if (themeInfo != null)
                        {
                            ThemeInfo = themeInfo;
                            App.Session.ThemeInfo = themeInfo;
                            App.Session.ThemeName = themeInfo.Name;
                            ChangeTheme();
                            SendThemeChangeMessage();
                        }
                        break;
                    //切换语言
                    case 110:
                        LangTypeInfo langType = e.Data as LangTypeInfo;
                        if (langType != null)
                        {
                            LangTypeInfo = langType;
                            App.Session.LangTypeInfo = langType;
                            App.Session.LangTypeID = langType.LangID;
                            MyWaiter.Visibility = Visibility.Visible;
                            mWorker = new BackgroundWorker();
                            mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                            mWorker.RunWorkerCompleted += (s, re) =>
                            {
                                mWorker.Dispose();
                                MyWaiter.Visibility = Visibility.Hidden;
                                ChangeLanguage();
                                SendLanguageChangeMessage();
                            };
                            mWorker.RunWorkerAsync();
                        }
                        break;
                    case PageHeadEventArgs.EVT_LEFT_PANEL:
                        if (GridLeft.ActualWidth > 0)
                        {
                            GridLeft.Width = new GridLength(0);
                        }
                        else
                        {
                            GridLeft.Width = new GridLength(200);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                MyWaiter.Visibility = Visibility.Visible;
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    MyWaiter.Visibility = Visibility.Hidden;
                                    ChangeLanguage();
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        void LvScoreSheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var scoreSheetItem = LvScoreSheets.SelectedItem as ScoreSheetItem;
            mCurrentScoreSheetItem = scoreSheetItem;
            CreateOptButtons();
            ShowObjectDetail();
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                var scoreSheetItem = mCurrentScoreSheetItem;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case S3101Consts.OPT_CREATESCORESHEET:
                            CreateScoreSheet();
                            break;
                        case S3101Consts.OPT_MODIFYSCORESHEET:
                            ModifyScoreSheet(scoreSheetItem);
                            break;
                        case S3101Consts.OPT_DELETESCORESHEET:
                            DeleteScoreSheet(scoreSheetItem);
                            break;
                        case S3101Consts.OPT_SETMANAGEUSER:
                            SetManageUser(scoreSheetItem);
                            break;
                        case S3101Consts.OPT_IMPORTSCORESHEET:
                            ImportScoreSheet();
                            break;
                        case S3101Consts.OPT_EXPORTSCORESHEET:
                            ExportScoreSheet(scoreSheetItem);
                            break;
                    }
                }
            }
        }

        #endregion


        #region CommandHandlers

        private void CreateScoreSheetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateScoreSheet();
        }

        private void ModifyScoreSheetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var scoreSheetItem = e.Parameter as ScoreSheetItem;
            if (scoreSheetItem != null)
            {
                ModifyScoreSheet(scoreSheetItem);
            }
        }

        private void DeleteScoreSheetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var scoreSheetItem = e.Parameter as ScoreSheetItem;
            if (scoreSheetItem != null)
            {
                DeleteScoreSheet(scoreSheetItem);
            }
        }

        private void SetManageUserCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var scoreSheetItem = e.Parameter as ScoreSheetItem;
            if (scoreSheetItem != null)
            {
                SetManageUser(scoreSheetItem);
            }
        }

        private void ImportScoreSheetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ExportScoreSheetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var scoreSheetItem = e.Parameter as ScoreSheetItem;
            if (scoreSheetItem != null)
            {
                ExportScoreSheet(scoreSheetItem);
            }
        }

        #endregion


        #region Operations

        private void CreateScoreSheet()
        {
            //App.ShowInfoMessage(string.Format("Create ScoreSheet"));
            try
            {
                App.IsModifyScoreSheet = false;
                if (NavigationService != null)
                    NavigationService.Navigate(new Uri("SSDMainPage.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ModifyScoreSheet(ScoreSheetItem item)
        {
            if (item != null)
            {
                //App.ShowInfoMessage(string.Format("Modify scoresheet:{0}", item.Name));
                var basicScoreSheetInfo = item.Data as BasicScoreSheetInfo;
                if (basicScoreSheetInfo != null)
                {
                    App.CurrentScoreSheetInfo = basicScoreSheetInfo;
                    App.IsModifyScoreSheet = true;

                    if (NavigationService != null)
                        NavigationService.Navigate(new Uri("SSDMainPage.xaml", UriKind.Relative));
                }
            }
        }

        private void DeleteScoreSheet(ScoreSheetItem item)
        {
            if (item != null)
            {
                if (item.UseFlag > 0)
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("3101N006", "Score Sheet Was Used, Can Not Be Delete"));
                    return;
                }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                    App.GetMessageLanguageInfo("001", "Confirm delete scoresheet?"),
                    item.Name),
                    App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MyWaiter.Visibility = Visibility.Visible;
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        try
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Session = App.Session;
                            webRequest.Code = (int)S3101Codes.RemoveScoreSheetInfo;
                            webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                            webRequest.ListData.Add(item.ID.ToString());
                            Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                                WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31011"));
                            WebReturn webReturn = client.DoOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                if (webReturn.Message.Equals("NoExist"))
                                {
                                    App.ShowInfoMessage(App.GetLanguageInfo("3101N005", "Score Sheet Is Not Exist"));
                                    return;
                                }
                                if (webReturn.Message.Equals("BeUsed"))
                                {
                                    App.ShowInfoMessage(App.GetLanguageInfo("3101N006", "Score Sheet Was Used, Can Not Be Delete"));
                                    return;
                                }
                                App.ShowExceptionMessage(string.Format("Fail.{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                            LoadScoreSheets();


                            #region 写操作日志

                            string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001Name"), item.Name);
                            App.WriteOperationLog(S3101Consts.OPT_DELETESCORESHEET.ToString(),
                                ConstValue.OPT_RESULT_SUCCESS, strLog);

                            #endregion

                        }
                        catch (Exception ex)
                        {
                            App.ShowExceptionMessage(ex.Message);
                        }
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        MyWaiter.Visibility = Visibility.Collapsed;
                    };
                    mWorker.RunWorkerAsync();
                }
            }
        }

        private void SetManageUser(ScoreSheetItem item)
        {
            if (item != null)
            {
                PopupPanel.Title = "ScoreSheet User Management";
                ScoreUserManagement scoreUserManagement = new ScoreUserManagement();
                scoreUserManagement.PageParent = this;
                scoreUserManagement.ScoreSheetItem = item;
                PopupPanel.Content = scoreUserManagement;
                PopupPanel.IsOpen = true;
            }
        }

        private void ExportScoreSheet(ScoreSheetItem item)
        {
            if (item != null)
            {
                PopupPanel.Title = App.GetLanguageInfo("FO3101006", "Export ScoreSheet");

                ExportScoreSheet exportScoreSheet = new ExportScoreSheet();

                exportScoreSheet.PageParent = this;
                exportScoreSheet.ScoreSheetItem = item;
                PopupPanel.Content = exportScoreSheet;
                PopupPanel.IsOpen = true;
            }
        }

        private void ImportScoreSheet()
        {
            try
            {
                //读取zip文件位置
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Multiselect = true;
                //fileDialog.Title = "请选择文件";
                //fileDialog.Filter = "所有文件(*.*)|*.*";
                fileDialog.Title = App.GetLanguageInfo("3101N014", "Please Select");
                fileDialog.Filter = @"Zip file(*.zip)|*.zip";
                string file;
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    file = fileDialog.FileName;
                    //System.Windows.Forms.MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    return;
                }

                //这个是解压的位置
                string x = Path.GetDirectoryName(file);

                //解压zip文件获取xml 并获得xml文件的位置
                string temp = UnZipFile(file, x);

                OperationReturn optReturn = XMLHelper.DeserializeFile<ScoreSheet>(temp);

                //删除解压的xml文件
                File.Delete(temp);

                ScoreSheet tempScoreSheet = optReturn.Data as ScoreSheet;
                if (tempScoreSheet == null)
                {
                    return;
                }
                tempScoreSheet.ScoreSheet = tempScoreSheet;
                tempScoreSheet.Init();

                List<ScoreObject> listScoreObjectTemp = new List<ScoreObject>();
                tempScoreSheet.GetAllScoreObject(ref listScoreObjectTemp);
                for (int i = 0; i < listScoreObjectTemp.Count; i++)
                {
                    listScoreObjectTemp[i].ID = GetSerialID();
                }
                tempScoreSheet.UseTag = 0;
                tempScoreSheet.ID = GetSerialID();
                tempScoreSheet.InitUseItemID();
                SaveScoreSheetData(tempScoreSheet);

                //刷新列表
                LoadScoreSheets();
                App.ShowInfoMessage(App.GetLanguageInfo("3101N007", "SECCESS"));
                string msg = string.Format("{0}{1}{2}", App.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3101005")), tempScoreSheet.Display);
                App.WriteOperationLog(S3101Consts.OPT_IMPORTSCORESHEET.ToString(),
                    ConstValue.OPT_RESULT_SUCCESS, msg);
            }
            catch (Exception ex)
            {
                App.ShowInfoMessage(App.GetLanguageInfo("3101N008", "Fail"));
            }
        }

        public BasicScoreSheetInfo InitExportScoreSheet(ScoreSheetItem item)
        {
            try
            {
                BasicScoreSheetInfo scoreSheetItemInfo = new BasicScoreSheetInfo();

                scoreSheetItemInfo.ID = item.ID;
                scoreSheetItemInfo.Name = item.Name;
                scoreSheetItemInfo.State = item.State;
                scoreSheetItemInfo.TotalScore = item.TotalScore;
                scoreSheetItemInfo.ViewClassic = item.ViewClassic;
                scoreSheetItemInfo.ScoreType = item.ScoreType;
                scoreSheetItemInfo.UseFlag = item.UseFlag;
                scoreSheetItemInfo.ItemCount = item.ItemCount;
                scoreSheetItemInfo.Description = item.Description;
                //OperationReturn temp = XMLHelper.SeriallizeObject<BasicScoreSheetInfo>(scoreSheetItemInfo);

                return scoreSheetItemInfo;
            }
            catch (Exception ex)
            {
                App.ShowInfoMessage(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="infile">压缩包</param>
        /// <param name="outFilePath">解压地址</param>
        /// 返回值是   解压后的文件的完整位置
        public static string UnZipFile(string infile, string outFilePath)   //解压缩
        {
            string mFileName = string.Empty;//这个是用来表示解压后的文件的完整路径
            if (!string.IsNullOrEmpty(infile))
            {
                if (!Directory.Exists(outFilePath)) Directory.CreateDirectory(outFilePath);
                using (var inFileStream = new FileStream(infile, FileMode.Open, FileAccess.Read, FileShare.Read))//创建读文件流
                {
                    using (var zipInputfs = new ZipInputStream(inFileStream))//创建zip读文件流
                    {
                        while (true)
                        {
                            var zp = zipInputfs.GetNextEntry(); //获取下一个对象
                            if (zp == null) break;
                            if (!zp.IsDirectory && zp.Crc != 00000000L)//验证是不是为目录
                            {
                                int i = 2048;
                                byte[] b = new byte[i];
                                using (var outFileStream = File.Create(Path.Combine(outFilePath, zp.Name)))//创建写文件流
                                {
                                    while (true)
                                    {
                                        i = zipInputfs.Read(b, 0, b.Length);//读流
                                        if (i <= 0) break;
                                        outFileStream.Write(b, 0, i);//写文件
                                    }
                                    outFileStream.Close();
                                    mFileName = Path.Combine(outFilePath, zp.Name);
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(Path.Combine(outFilePath, zp.Name));//创建目录 
                                mFileName = Path.Combine(outFilePath, zp.Name);
                            }
                        }
                        zipInputfs.Close();
                    }
                    inFileStream.Close();
                }
            }
            return mFileName;
        }

        private void SaveScoreSheetData(ScoreSheet scoreSheet)
        {
            try
            {
                OperationReturn optReturn = XMLHelper.SeriallizeObject(scoreSheet);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3101Codes.SaveScoreSheetInfo;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                App.WriteLog("SaveScoreSheet", webReturn.Data);

                #region 写操作日志

                //string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001Name"),
                //    scoreSheet.Title);
                //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001TotalScore"),
                //    scoreSheet.TotalScore);
                //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ViewClassic"),
                // Utils.FormatOptLogString(string.Format("3101Tip002{0}", (int)scoreSheet.ViewClassic)));
                //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ScoreType"),
                // Utils.FormatOptLogString(string.Format("3101Tip003{0}", (int)scoreSheet.ScoreType)));
                //App.WriteOperationLog(App.IsModifyScoreSheet ?
                //    S3101Consts.OPT_MODIFYSCORESHEET.ToString() : S3101Consts.OPT_CREATESCORESHEET.ToString(),
                //    ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                //App.ShowInfoMessage(App.GetMessageLanguageInfo("002", "Save ScoreSheet end"));
                //mIsChanged = false;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void ShowObjectDetail()
        {
            try
            {
                ScoreSheetItem item = mCurrentScoreSheetItem;
                if (item == null) { return; }
                ObjectDetail.Title = item.Name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(string.Format("/Themes/Default/UMPS3101/Images/template.ico"), UriKind.Relative);
                image.EndInit();
                ObjectDetail.Icon = image;
                List<PropertyItem> listProperties = new List<PropertyItem>();
                PropertyItem property;
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T002", "Total Score");
                property.ToolTip = property.Name;
                property.Value = item.TotalScore.ToString();
                listProperties.Add(property);
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T003", "Item Count");
                property.ToolTip = property.Name;
                property.Value = item.ItemCount.ToString();
                listProperties.Add(property);
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T004", "View Classic");
                property.ToolTip = property.Name;
                property.Value = App.GetLanguageInfo(string.Format("3101Tip002{0}", item.ViewClassic),
                    string.Format("ViewClassic:{0}", item.ViewClassic));
                listProperties.Add(property);
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T005", "Score Type");
                property.ToolTip = property.Name;
                property.Value = App.GetLanguageInfo(string.Format("3101Tip003{0}", item.ScoreType),
                    string.Format("ScoreType:{0}", item.ScoreType));
                listProperties.Add(property);
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T006", "Use Flag");
                property.ToolTip = property.Name;
                property.Value = item.UseFlag.ToString();
                listProperties.Add(property);
                property = new PropertyItem();
                property.Name = App.GetLanguageInfo("3101T007", "Description");
                property.ToolTip = property.Name;
                property.Value = item.Description;
                listProperties.Add(property);
                ObjectDetail.ItemsSource = listProperties;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddChildItem(ScoreSheetItem item)
        {
            Dispatcher.Invoke(new Action(() => mListScoreSheets.Add(item)));
        }

        private void ClearChildItem()
        {
            Dispatcher.Invoke(new Action(() => mListScoreSheets.Clear()));
        }

        private long GetSerialID()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("301");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return -1;
            }
        }

        public void SetBusy(bool isBusying)
        {
            MyWaiter.Visibility = isBusying ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                if (PageHead != null)
                {
                    PageHead.AppName = App.GetLanguageInfo("3101001", "UMP ScoreSheet Management");
                }

                //Operation
                string tipModify = string.Empty, tipDelete = string.Empty, tipSetUser = string.Empty, tipExport = string.Empty;
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = App.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());

                    if (ListOperations[i].ID == 3101002)
                    {
                        tipModify = ListOperations[i].Display;
                    }
                    if (ListOperations[i].ID == 3101003)
                    {
                        tipDelete = ListOperations[i].Display;
                    }
                    if (ListOperations[i].ID == 3101004)
                    {
                        tipSetUser = ListOperations[i].Display;
                    }
                    if (ListOperations[i].ID == 3101005)
                    {
                    }
                    if (ListOperations[i].ID == 3101006)
                    {
                        tipExport = ListOperations[i].Display;
                    }
                }
                CreateOptButtons();

                //Other
                ExpBasicOpt.Header = App.GetLanguageInfo("31011000", "Basic Operations");
                ExpOtherPos.Header = App.GetLanguageInfo("31011001", "Other Position");

                //列名
                InitColumns();

                //ScoreSheetItem
                for (int i = 0; i < mListScoreSheets.Count; i++)
                {
                    ScoreSheetItem item = mListScoreSheets[i];
                    item.TipState = App.GetLanguageInfo(string.Format("3101Tip001{0}", item.State),
                        string.Format("State:{0}", item.State));
                    item.TipViewClassic = App.GetLanguageInfo(string.Format("3101Tip002{0}", item.ViewClassic),
                        string.Format("ViewClassic:{0}", item.ViewClassic));
                    item.TipScoreType = App.GetLanguageInfo(string.Format("3101Tip003{0}", item.ScoreType),
                        string.Format("ScoreType:{0}", item.ScoreType));

                    item.TipOptModify = tipModify;
                    item.TipOptDelete = tipDelete;
                    item.TipOptSetUser = tipSetUser;
                    item.TipOptExport = tipExport;

                }

                //详细信息
                ShowObjectDetail();

                //弹出面板
                var popup = PopupPanel;
                if (popup != null)
                {
                    popup.ChangeLanguage();
                }
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region ChangeTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/Themes/Default/UMPS3101/SSMStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }

            var popup = PopupPanel;
            if (popup != null)
            {
                popup.ChangeTheme();
            }
        }

        #endregion
    }
}
