//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6dd286df-7f5d-40b9-8943-2614d6e4ab60
//        CLR Version:              4.0.30319.18408
//        Name:                     SeatManageMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4413
//        File Name:                SeatManageMainView
//
//        created by Charley at 2016/6/6 16:10:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS4413.Models;
using UMPS4413.Wcf11012;
using UMPS4413.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;

namespace UMPS4413
{
    /// <summary>
    /// SeatManageMainView.xaml 的交互逻辑
    /// </summary>
    public partial class SeatManageMainView
    {

        #region Members

        private List<OperationInfo> mListUserOperations;
        private ObservableCollection<ViewColumnInfo> mListViewColumns;
        private List<SeatInfo> mListSeatInfos;
        private ObservableCollection<ObjItem> mListSeatItems;

        #endregion

        public SeatManageMainView()
        {
            InitializeComponent();

            mListUserOperations = new List<OperationInfo>();
            mListViewColumns = new ObservableCollection<ViewColumnInfo>();
            mListSeatInfos = new List<SeatInfo>();
            mListSeatItems = new ObservableCollection<ObjItem>();

            LvSeatList.SelectionChanged += LvSeatList_SelectionChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "SeatManageMainView";
                StylePath = "UMPS4413/SeatManageMainView.xaml";

                LvSeatList.ItemsSource = mListSeatItems;

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                if (CurrentApp != null)
                {
                    SetBusy(true, CurrentApp.GetLanguageInfo("COMN005", string.Format("Loading basic data...")));
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserOperations();
                    LoadViewColumns();
                    LoadSeatInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateSeatColumns();
                    CreateBasicOperations();
                    CreateSeatItems();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserOperations()
        {
            try
            {
                mListUserOperations.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("44");
                webRequest.ListData.Add("4413");
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        mListUserOperations.Add(optInfo);
                    }
                }

                CurrentApp.WriteLog("LoadOperations", string.Format("Load end.\t{0}", mListUserOperations.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadViewColumns()
        {
            try
            {
                mListViewColumns.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("4413001");
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
                int count = webReturn.ListData.Count;
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo info = optReturn.Data as ViewColumnInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tViewColumnInfo is null"));
                        return;
                    }
                    mListViewColumns.Add(info);
                }

                CurrentApp.WriteLog("LoadViewColumns", string.Format("Load end.\t{0}", mListViewColumns.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadSeatInfos()
        {
            try
            {
                mListSeatInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetSeatInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\t ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<SeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    SeatInfo seatInfo = optReturn.Data as SeatInfo;
                    if (seatInfo == null)
                    {
                        ShowException(string.Format("Fail.\t SeatInfo is null"));
                        return;
                    }
                    mListSeatInfos.Add(seatInfo);
                }

                CurrentApp.WriteLog("LoadSeatInfos", string.Format("Load end.\t{0}", mListSeatInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateSeatColumns()
        {
            try
            {
                GridView gv = new GridView();

                var listColumns = mListViewColumns.OrderBy(c => c.SortID).ToList();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = listColumns[i];

                    GridViewColumn gvc = new GridViewColumn();
                    GridViewColumnHeader gvch = new GridViewColumnHeader();
                    gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4413001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4413001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    string strName = columnInfo.ColumnName;
                    if (strName == "State")
                    {
                        strName = "StrState";
                    }
                    if (strName == "Level")
                    {
                        strName = "StrLevel";
                    }
                    gvc.DisplayMemberBinding = new Binding(strName);

                    gv.Columns.Add(gvc);
                }

                LvSeatList.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateBasicOperations()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                OperationInfo item;
                Button btn;

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    item = mListUserOperations[i];

                    //同步座位和导入座位暂时屏蔽
                    if (item.ID == S4410Consts.OPT_SYNCSEAT
                        || item.ID == S4410Consts.OPT_IMPORTSEAT) { return; }

                    //基本操作按钮
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateSeatItems()
        {
            try
            {
                mListSeatItems.Clear();
                for (int i = 0; i < mListSeatInfos.Count; i++)
                {
                    SeatInfo seatInfo = mListSeatInfos[i];

                    ObjItem item = new ObjItem();
                    item.Data = seatInfo;
                    item.ObjType = S4410Consts.RESOURCE_SEAT;
                    item.ObjID = seatInfo.ObjID;
                    item.Name = seatInfo.Name;
                    item.State = seatInfo.State;
                    item.Level = seatInfo.Level;
                    item.Extension = seatInfo.Extension;
                    item.Description = seatInfo.Description;

                    item.StrState =
                        CurrentApp.GetLanguageInfo(string.Format("4413010{0}", seatInfo.State.ToString("000")),
                            seatInfo.State.ToString());
                    item.StrLevel = item.Level.ToString();

                    mListSeatItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddSeat()
        {
            try
            {
                UCSeatModify uc = new UCSeatModify();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.IsAdd = true;
                uc.ListAllSeatInfo = mListSeatInfos;
                PopupPanel.Title = string.Format("Add Seat");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteSeat()
        {
            try
            {
                var items = LvSeatList.SelectedItems;
                string strMsg = string.Empty;
                List<SeatInfo> listSeatInfos = new List<SeatInfo>();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as ObjItem;
                    if (item != null)
                    {
                        var seatInfo = item.Data as SeatInfo;
                        if (seatInfo != null)
                        {
                            listSeatInfos.Add(seatInfo);
                        }
                    }
                }
                int count = listSeatInfos.Count;
                int tempCount = Math.Min(count, 5);
                for (int i = 0; i < tempCount; i++)
                {
                    strMsg += string.Format("{0}\r\n", listSeatInfos[i].Name);
                }
                if (tempCount < count)
                {
                    strMsg += string.Format("...\r\n");
                }
                strMsg = string.Format("{0}\r\n{1}", CurrentApp.GetLanguageInfo("4413N001", "Confirm delete seats?"), strMsg);
                var result = MessageBox.Show(strMsg, CurrentApp.AppTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                OperationReturn optReturn;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveSeatInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("2");//删除
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listSeatInfos[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                bool isSuccessful = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            ShowException(string.Format("Fail.\t ListData is null"));
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            CurrentApp.WriteLog("DeleteSeat", string.Format("{0}", webReturn.ListData[i]));
                        }
                        isSuccessful = true;
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    try
                    {
                        if (isSuccessful)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("4413N002", string.Format("Delete seat successful.")));

                            Reload();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifySeat()
        {
            try
            {
                var item = LvSeatList.SelectedItem as ObjItem;
                if (item == null) { return; }
                var seatInfo = item.Data as SeatInfo;
                if (seatInfo == null) { return; }

                UCSeatModify uc = new UCSeatModify();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.IsAdd = false;
                uc.SeatItem = item;
                uc.ListAllSeatInfo = mListSeatInfos;
                PopupPanel.Title = string.Format("Modify Seat");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Reload()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadSeatInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateSeatItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var optItem = btn.DataContext as OperationInfo;
                    if (optItem == null) { return; }
                    switch (optItem.ID)
                    {
                        case S4410Consts.OPT_ADDSEAT:
                            AddSeat();
                            break;
                        case S4410Consts.OPT_DELETESEAT:
                            DeleteSeat();
                            break;
                        case S4410Consts.OPT_MODIFYSEAT:
                            ModifySeat();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void LvSeatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS4413;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO4413", "Seat Management");

                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("4413001", "Basic Operations");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("4413002", "Other Positions");

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    var optInfo = mListUserOperations[i];
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                }
                CreateBasicOperations();
                CreateSeatColumns();

                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];
                    var info = item.Data as SeatInfo;
                    if (info == null) { continue; }

                    item.StrState =
                      CurrentApp.GetLanguageInfo(string.Format("4413010{0}", info.State.ToString("000")),
                          info.State.ToString());
                }

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
