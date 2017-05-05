//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    56cf99d5-ec00-423b-854e-2149e5fc606e
//        CLR Version:              4.0.30319.42000
//        Name:                     UCOptLogView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.WidgetViews
//        File Name:                UCOptLogView
//
//        created by Charley at 2016/3/14 16:51:12
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS1206.Models;
using UMPS1206.Wcf11012;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Communications;

namespace UMPS1206.WidgetViews
{
    /// <summary>
    /// UCOptLogView.xaml 的交互逻辑
    /// </summary>
    public partial class UCOptLogView : IWidgetView
    {

        #region Members

        public WidgetItem WidgetItem { get; set; }
        public IList<BasicDataInfo> ListBasicDataInfos { get; set; }
        public bool IsCenter { get; set; }
        public bool IsFull { get; set; }

        private bool mIsInited;
        private List<OperationLogInfo> mListOptLogInfos;
        private ObservableCollection<OperationLogItem> mListOptLogItems;
        private List<ViewColumnInfo> mListOptLogColumns;
        private List<UserWidgetPropertyValue> mListPropertyValues;
        private List<LanguageInfo> mListOptLanguages;
        private List<string> mListOptContentLangIDs;
        private List<LanguageInfo> mListOptContentLanguages;
        private List<ResourceObject> mListUserInfoList;
        private int mDefaultSize = 20;
        private int mNextSize = 10;
        private bool mOnlyMyself = true;
        private int mRecordCount;
        private int mLoadCount;

        #endregion


        #region 配置参数的属性编号

        public const int WIDGET_PROPERTY_ID_DEFAULTSIZE = 1;
        public const int WIDGET_PROPERTY_ID_NEXTSIZE = 2;
        public const int WIDGET_PROPERTY_ID_ONLYMYSELF = 3;

        #endregion


        public UCOptLogView()
        {
            InitializeComponent();

            mListOptLogInfos = new List<OperationLogInfo>();
            mListOptLogItems = new ObservableCollection<OperationLogItem>();
            mListOptLogColumns = new List<ViewColumnInfo>();
            mListOptLanguages = new List<LanguageInfo>();
            mListOptContentLangIDs = new List<string>();
            mListOptContentLanguages = new List<LanguageInfo>();
            mListPropertyValues = new List<UserWidgetPropertyValue>();
            mListUserInfoList = new List<ResourceObject>();

            Loaded += UCOptLogView_Loaded;
            BtnMore.Click += BtnMore_Click;
        }

        void UCOptLogView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ListViewOptLogs.ItemsSource = mListOptLogItems;
                mListOptLogInfos.Clear();
                mListOptLogItems.Clear();
                mRecordCount = 0;
                mLoadCount = 0;
                if (WidgetItem != null && WidgetItem.MainView != null)
                {
                    WidgetItem.MainView.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    InitOptLogColumns();
                    LoadOptLanguages();
                    LoadUserInfoList();
                    LoadWidgetPropertyValues();
                    LoadOptLogInfos(mDefaultSize, 0);
                    LoadOptContentLanguages();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (WidgetItem != null && WidgetItem.MainView != null)
                    {
                        WidgetItem.MainView.SetBusy(false, string.Empty);
                    }

                    CreateOptLogColumns();
                    CreateOptLogItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitOptLogColumns()
        {
            try
            {
                //mListOptLogColumns.Clear();
                //WebRequest webRequest = new WebRequest();
                //webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                //webRequest.Session = CurrentApp.Session;
                //webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                //webRequest.ListData.Add("1206001");
                //Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                //    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                //WebReturn webReturn = client.DoOperation(webRequest);
                //client.Close();
                //if (!webReturn.Result)
                //{
                //    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                //    return;
                //}
                //List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                //for (int i = 0; i < webReturn.ListData.Count; i++)
                //{
                //    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                //    if (!optReturn.Result)
                //    {
                //        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //        return;
                //    }
                //    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                //    if (columnInfo != null)
                //    {
                //        columnInfo.Display = columnInfo.ColumnName;
                //        listColumns.Add(columnInfo);
                //    }
                //}
                //listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                //mListOptLogColumns.Clear();
                //for (int i = 0; i < listColumns.Count; i++)
                //{
                //    mListOptLogColumns.Add(listColumns[i]);
                //}

                mListOptLogColumns.Clear();
                ViewColumnInfo info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrTime";
                info.Display = info.ColumnName;
                info.Description = info.Display;
                info.Visibility = "1";
                info.Width = 150;
                mListOptLogColumns.Add(info);
                info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrResult";
                info.Display = "";
                info.Description = "";
                info.Visibility = "1";
                info.Width = 30;
                mListOptLogColumns.Add(info);
                info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrOperation";
                info.Display = info.ColumnName;
                info.Description = info.Display;
                info.Visibility = "1";
                info.Width = 120;
                mListOptLogColumns.Add(info);
                info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrUser";
                info.Display = info.ColumnName;
                info.Description = info.Display;
                info.Visibility = "1";
                info.Width = 120;
                mListOptLogColumns.Add(info);
                info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrHost";
                info.Display = info.ColumnName;
                info.Description = info.Display;
                info.Visibility = "1";
                info.Width = 180;
                mListOptLogColumns.Add(info);
                info = new ViewColumnInfo();
                info.ViewID = 1206001;
                info.ColumnName = "StrContent";
                info.Display = info.ColumnName;
                info.Description = info.Display;
                info.Visibility = "1";
                info.Width = 500;
                mListOptLogColumns.Add(info);

                CurrentApp.WriteLog("InitColumns", string.Format("Load OptLog Columns end."));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOptLogInfos(int size, int skip)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetOptLogInfoList;
                if (mOnlyMyself)
                {
                    webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                }
                else
                {
                    webRequest.ListData.Add("0");
                }
                webRequest.ListData.Add(size.ToString());
                webRequest.ListData.Add(skip.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationLogInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationLogInfo info = optReturn.Data as OperationLogInfo;
                    if (info == null) { continue; }
                    info.LogArgs = S1206App.DecryptString(info.LogArgs);
                    mListOptContentLangIDs.Add(info.LangID);
                    GetOptContentLangID(info);
                    mListOptLogInfos.Add(info);
                    mRecordCount++;
                }
                //去除重复
                mListOptContentLangIDs =
                    mListOptContentLangIDs.Where((l, i) => mListOptContentLangIDs.FindIndex(z => z == l) == i).ToList();

                CurrentApp.WriteLog("LoadOptLogInfo", string.Format("Load end.\t{0}", mListOptLogInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOptLanguages()
        {
            try
            {
                mListOptLanguages.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add("FO");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowException(string.Format("LanguageInfo is null"));
                        return;
                    }
                    mListOptLanguages.Add(langInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOptContentLanguages()
        {
            try
            {
                mListOptContentLanguages.Clear();
                int count = mListOptContentLangIDs.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1200Codes.GetOptContentLangList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    webRequest.ListData.Add(mListOptContentLangIDs[i]);
                }
                CurrentApp.WriteLog("LoadOptContentLangs", string.Format("ContentLangID count.\t{0}", count));
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<LanguageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo lang = optReturn.Data as LanguageInfo;
                    if (lang == null)
                    {
                        ShowException(string.Format("LanguageInfo is null"));
                        return;
                    }
                    mListOptContentLanguages.Add(lang);
                }
                CurrentApp.WriteLog("LoadOptContentLangs", string.Format("End.\t{0}", mListOptContentLanguages.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadWidgetPropertyValues()
        {
            try
            {
                mListPropertyValues.Clear();
                if (WidgetItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserWidgetPropertyValueList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(WidgetItem.WidgetID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserWidgetPropertyValue>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserWidgetPropertyValue info = optReturn.Data as UserWidgetPropertyValue;
                    if (info == null) { continue; }
                    mListPropertyValues.Add(info);
                }

                int intValue;
                for (int i = 0; i < mListPropertyValues.Count; i++)
                {
                    var propertyValue = mListPropertyValues[i];
                    string strValue = propertyValue.Value01;
                    switch (propertyValue.PropertyID)
                    {
                        case WIDGET_PROPERTY_ID_DEFAULTSIZE:
                            if (int.TryParse(strValue, out intValue)
                                && intValue > 0)
                            {
                                mDefaultSize = intValue;
                            }
                            break;
                        case WIDGET_PROPERTY_ID_NEXTSIZE:
                            if (int.TryParse(strValue, out intValue)
                                && intValue > 0)
                            {
                                mNextSize = intValue;
                            }
                            break;
                        case WIDGET_PROPERTY_ID_ONLYMYSELF:
                            mOnlyMyself = strValue == "1";
                            break;
                    }
                }

                CurrentApp.WriteLog("LoadPropertyValues", string.Format("Load end.\t{0}", mListPropertyValues.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserInfoList()
        {
            try
            {
                mListUserInfoList.Clear();

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("2");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    mListUserInfoList.Add(info);
                }

                CurrentApp.WriteLog("LoadUserInfoList", string.Format("Load end.\t{0}", mListUserInfoList.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create Operation

        private void CreateOptLogItems()
        {
            try
            {
                int count = mListOptLogItems.Count;
                for (int i = 0; i < mListOptLogInfos.Count; i++)
                {
                    if (i < count) { continue; }
                    var info = mListOptLogInfos[i];
                    string strUser = string.Empty;
                    var userInfo = mListUserInfoList.FirstOrDefault(u => u.ObjID == info.UserID);
                    if (userInfo != null)
                    {
                        strUser = userInfo.Name;
                    }
                    OperationLogItem item = new OperationLogItem();
                    item.LogID = info.ID;
                    item.StrUser = strUser;
                    item.Info = info;
                    ParseOptLogItemInfo(item);
                    mListOptLogItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ParseOptLogItemInfo(OperationLogItem item)
        {
            try
            {
                var info = item.Info;
                if (info == null) { return; }
                long optID = info.OptID;
                string strOptID = optID.ToString();
                if (strOptID.Length > 4)
                {
                    string strModule = strOptID.Substring(0, 4);
                    if (strModule != "1100")
                    {
                        strModule = GetOptLanguage(string.Format("FO{0}", strModule), strModule);
                        string strOpt = GetOptLanguage(string.Format("FO{0}", optID), optID.ToString());
                        item.StrOperation = string.Format("[{0}]{1}", strModule, strOpt);
                    }
                    else
                    {
                        item.StrOperation = GetOptLanguage(string.Format("FO{0}", optID), optID.ToString());
                    }
                }
                else
                {
                    item.StrOperation = GetOptLanguage(string.Format("FO{0}", optID), optID.ToString());
                }
                item.StrTime = info.LogTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                item.StrHost = string.Format("{0}[{1}]", info.MachineName, info.MachineIP);
                item.StrContent = GetOptContentDisplay(info);
                item.StrResult = info.LogResult == "R0"
                    ? "Images/00006.png"
                    : info.LogResult == "R1"
                        ? "Images/00005.png"
                        : info.LogResult == "R3"
                            ? "Images/00007.png"
                            : string.Empty;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateOptLogColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListOptLogColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListOptLogColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL1206001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL1206001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "StrResult")
                        {
                            dt = (DataTemplate)Resources["CellResultTemplate"];
                        }
                        if (columnInfo.ColumnName == "StrContent")
                        {
                            dt = (DataTemplate)Resources["CellContentTemplate"];
                        }
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                ListViewOptLogs.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mLoadCount++;
                int size = mLoadCount * mNextSize + mDefaultSize;
                int skip = mRecordCount;
                if (WidgetItem != null && WidgetItem.MainView != null)
                {
                    WidgetItem.MainView.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadOptLogInfos(size, skip);
                    LoadOptContentLanguages();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (WidgetItem != null && WidgetItem.MainView != null)
                    {
                        WidgetItem.MainView.SetBusy(false, string.Empty);
                    }

                    CreateOptLogItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private string GetOptLanguage(string code, string display)
        {
            var lang = mListOptLanguages.FirstOrDefault(l => l.Name == code);
            if (lang != null)
            {
                return lang.Display;
            }
            return display;
        }

        private void GetOptContentLangID(OperationLogInfo optLogInfo)
        {
            try
            {
                string strRegex = string.Format("{0}<{0}\\w+{0}>{0}", ConstValue.SPLITER_CHAR_2);
                Regex regex = new Regex(strRegex);
                var collections = regex.Matches(optLogInfo.LogArgs);
                for (int i = 0; i < collections.Count; i++)
                {
                    var item = collections[i];
                    int length = item.Length;
                    if (length < 6) { continue; }
                    string strLangID = item.Value.Substring(3, item.Value.Length - 6);
                    if (string.IsNullOrEmpty(strLangID)) { continue; }
                    mListOptContentLangIDs.Add(strLangID);
                }
            }
            catch { }
        }

        private string GetOptContentDisplay(OperationLogInfo optLogInfo)
        {
            string strReturn = optLogInfo.LogArgs;
            try
            {
                var lang = mListOptContentLanguages.FirstOrDefault(l => l.Name == optLogInfo.LangID);
                if (lang != null)
                {
                    strReturn = lang.Display;
                }
                string strRegex1 = string.Format("{{\\d+}}");
                Regex regex1 = new Regex(strRegex1);
                var collection1 = regex1.Matches(strReturn);
                int count1 = collection1.Count;
                if (count1 > 0)
                {
                    string[] arrArgs =
                        optLogInfo.LogArgs.Split(new[] {string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR_2)},
                            StringSplitOptions.None);
                    for (int i = 0; i < count1; i++)
                    {
                        if (arrArgs.Length > i)
                        {
                            strReturn = strReturn.Replace(collection1[i].Value, arrArgs[i]);
                        }
                    }
                }
                string strRegex2 = string.Format("{0}<{0}\\w+{0}>{0}", ConstValue.SPLITER_CHAR_2);
                Regex regex2 = new Regex(strRegex2);
                var collection2 = regex2.Matches(strReturn);
                int count2 = collection2.Count;
                if (count2 > 0)
                {
                    for (int i = 0; i < count2; i++)
                    {
                        string str = collection2[i].Value;
                        if (str.Length > 6)
                        {
                            string strID = str.Substring(3, str.Length - 6);
                            lang = mListOptContentLanguages.FirstOrDefault(l => l.Name == strID);
                            if (lang != null)
                            {
                                strReturn =
                                    strReturn.Replace(
                                        string.Format("{0}<{0}{1}{0}>{0}", ConstValue.SPLITER_CHAR_2, strID),
                                        lang.Display);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("GetOptContentDisplay", string.Format("Fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        #endregion


        #region Refresh

        public void Refresh()
        {
            try
            {
                mListOptLogInfos.Clear();
                mListOptLogItems.Clear();
                mRecordCount = 0;
                mLoadCount = 0;
                if (WidgetItem != null && WidgetItem.MainView != null)
                {
                    WidgetItem.MainView.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserInfoList();
                    LoadWidgetPropertyValues();
                    LoadOptLogInfos(mDefaultSize, 0);
                    LoadOptContentLanguages();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (WidgetItem != null && WidgetItem.MainView != null)
                    {
                        WidgetItem.MainView.SetBusy(false, string.Empty);
                    }

                    CreateOptLogItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                LoadOptLanguages();
                LoadOptContentLanguages();

                CreateOptLogColumns();

                for (int i = 0; i < mListOptLogItems.Count; i++)
                {
                    var item = mListOptLogItems[i];
                    ParseOptLogItemInfo(item);
                }
            }
            catch { }
        }

        #endregion

    }
}
