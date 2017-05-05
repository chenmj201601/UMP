//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9275336d-c43c-4e89-a426-8d9f7c3c5b8a
//        CLR Version:              4.0.30319.18408
//        Name:                     UCSeatPlayback
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCSeatPlayback
//
//        created by Charley at 2016/7/6 11:14:52
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using UMPS4411.Models;
using UMPS4411.Wcf000A1;
using UMPS4411.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.SDKs.NMon;
using VoiceCyber.SDKs.ScreenMP;
using VoiceCyber.SDKs.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common000A1;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.CustomControls.ListItem.Implementation;
using Timer = System.Timers.Timer;

namespace UMPS4411
{
    /// <summary>
    /// UCSeatPlayback.xaml 的交互逻辑
    /// </summary>
    public partial class UCSeatPlayback
    {

        #region Members

        public RegionSeatItem RegionSeatItem;
        public UCWorkRegionMap PageParent;

        private bool mIsInited;
        private ObservableCollection<RecordInfoItem> mListRecordItems;
        private ObservableCollection<ViewColumnInfo> mListViewColumns;
        private MonPlaybackItem mNMonItem;
        private MonPlaybackItem mStateMonItem;
        private MonPlaybackItem mSMonItem;
        private NetClient mNMonClient;
        private NetClient mStateMonClient;
        private NetClient mSMonClient;
        private double mTimeDeviation;
        private NMonCore mNMonCore;
        private bool mIsNMon;
        private bool mIsSMon;
        private Timer mRecordLengthTimer;
        private string mAgentID;
        private bool mIsLogined;
        private RecordInfoItem mCurrentItem;
        private bool mCanCloseScreenWindow;

        #endregion


        public UCSeatPlayback()
        {
            InitializeComponent();

            mListRecordItems = new ObservableCollection<RecordInfoItem>();
            mListViewColumns = new ObservableCollection<ViewColumnInfo>();

            Loaded += UCSeatPlayback_Loaded;
            Unloaded += UCSeatPlayback_Unloaded;
            BtnNMon.Click += BtnNMon_Click;
            BtnSMon.Click += BtnSMon_Click;
            TabControlRecordList.SelectionChanged += TabControlRecordList_SelectionChanged;
            ListViewRecordList.AddHandler(ListItemPanel.ItemMouseDoubleClickEvent, new RoutedPropertyChangedEventHandler<ListItemEventArgs>(ListViewRecordList_MouseDoubleClicked));

            mTimeDeviation = 0;
            mRecordLengthTimer = new Timer(1000);
        }


        void UCSeatPlayback_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void UCSeatPlayback_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AudioPlayer.Close();
            }
            catch { }
            try
            {
                if (mCanCloseScreenWindow)
                {
                    ScreenMPInterop.VLSMonCloseWnd();
                }
            }
            catch { }
            if (mNMonCore != null)
            {
                mNMonCore.StopMon();
                mNMonCore = null;
            }
            if (mNMonClient != null)
            {
                mNMonClient.Stop();
                mNMonClient = null;
            }
            if (mSMonClient != null)
            {
                mSMonClient.Stop();
                mSMonClient = null;
            }
            if (mStateMonClient != null)
            {
                mStateMonClient.Stop();
                mStateMonClient = null;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                mRecordLengthTimer.Elapsed += RecordLengthTimer_Elapsed;
                ListViewRecordList.ItemsSource = mListRecordItems;
                if (RegionSeatItem == null) { return; }
                LoadViewColumns();
                mIsLogined = false;
                mAgentID = string.Empty;
                mNMonItem = new MonPlaybackItem();
                mNMonItem.SeatID = RegionSeatItem.SeatID;
                mNMonItem.SeatName = RegionSeatItem.SeatName;
                mNMonItem.Extension = RegionSeatItem.Extension;
                mStateMonItem = new MonPlaybackItem();
                mStateMonItem.SeatID = RegionSeatItem.SeatID;
                mStateMonItem.SeatName = RegionSeatItem.SeatName;
                mStateMonItem.Extension = RegionSeatItem.Extension;
                mSMonItem = new MonPlaybackItem();
                mSMonItem.SeatID = RegionSeatItem.SeatID;
                mSMonItem.SeatName = RegionSeatItem.SeatName;
                mSMonItem.Extension = RegionSeatItem.Extension;
                CreateRecordColumns();
                InitInfo();
                CreateStateMonClient();
                mRecordLengthTimer.Start();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInfo()
        {
            try
            {
                if (mStateMonItem == null) { return; }
                string strExtension = mStateMonItem.Extension;
                TxtExtension.Text = strExtension;
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (mIsLogined)
                    {
                        parent.Title = string.Format("{0} --- {1}",
                            CurrentApp.GetLanguageInfo("4411010", "Playback and Monitor"), mAgentID);
                    }
                    else
                    {
                        parent.Title = string.Format("{0} --- {1}",
                            CurrentApp.GetLanguageInfo("4411010", "Playback and Monitor"), strExtension);
                    }
                }
                TabRecordList.IsEnabled = mIsLogined;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRecordInfos()
        {
            try
            {
                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetLogRecordData;
                request.ListData.Add(CurrentApp.Session.UserID.ToString());
                request.ListData.Add("1");
                request.ListData.Add("0");
                request.ListData.Add(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add("10");
                request.ListData.Add("2");//按开始时间倒序
                JsonObject json = new JsonObject();
                json[UMPRecordInfo.PRO_AGENT] = new JsonProperty(string.Format("\"{0}\"", mAgentID));
                request.ListData.Add(json.ToString());
                Service000A1Client client = new Service000A1Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service000A1"));
                SDKReturn sdkReturn = client.DoOperation(request);
                if (!sdkReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    string strInfo = sdkReturn.ListData[i];
                    JsonObject jsonRecord = new JsonObject(strInfo);
                    optReturn = GetRecordInfoFromJsonObject(jsonRecord);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UMPRecordInfo recordInfo = optReturn.Data as UMPRecordInfo;
                    if (recordInfo == null)
                    {
                        ShowException(string.Format("Fail.\tRecordInfo is null"));
                        return;
                    }
                    RecordInfoItem item = new RecordInfoItem();
                    item.SerialNo = recordInfo.SerialID;
                    item.RecordReference = recordInfo.RecordReference;
                    item.StartRecordTime = recordInfo.StartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    item.Extension = recordInfo.Extension;
                    item.Agent = recordInfo.Agent;
                    item.Direction = jsonRecord[UMPRecordInfo.PRO_DIRECTION].Value;
                    item.CallerID = jsonRecord[UMPRecordInfo.PRO_CALLERID].Value;
                    item.CalledID = jsonRecord[UMPRecordInfo.PRO_CALLEDID].Value;

                    var ts = recordInfo.StopRecordTime - recordInfo.StartRecordTime;
                    item.RecordLength = string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"),
                        ts.Seconds.ToString("00"));

                    item.Info = recordInfo;
                    Dispatcher.Invoke(new Action(() => mListRecordItems.Add(item)));
                }
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
                webRequest.ListData.Add("4411001");
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

        #endregion


        #region Operations

        private void PlayRecord()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                var recordInfo = mCurrentItem.Info;
                if (recordInfo == null) { return; }
                bool isFail = true;
                string strFileName = string.Empty;
                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetLogRecordUrl;
                request.ListData.Add(CurrentApp.Session.UserID.ToString());
                request.ListData.Add("10");
                request.ListData.Add(recordInfo.StringInfo);
                request.ListData.Add(string.Empty);
                request.ListData.Add(S4411App.EncryptString(CurrentApp.Session.UserInfo.Password));
                request.ListData.Add(string.Empty);
                Service000A1Client client = new Service000A1Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service000A1"));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        SDKReturn sdkReturn = client.DoOperation(request);
                        if (!sdkReturn.Result)
                        {
                            ShowException(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                            return;
                        }
                        if (sdkReturn.ListData == null)
                        {
                            ShowException(string.Format("Fail.\tListData is null"));
                            return;
                        }
                        if (sdkReturn.ListData.Count < 1)
                        {
                            ShowException(string.Format("Fail.\tFileName not exist"));
                            return;
                        }
                        strFileName = sdkReturn.ListData[0];
                        isFail = false;
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
                        if (isFail) { return; }

                        CurrentApp.WriteLog("DownloadRecord", string.Format("FileName:{0}", strFileName));
                        string strUrl = string.Format("{0}://{1}:{2}/MediaData/{3}",
                            CurrentApp.Session.AppServerInfo.Protocol, CurrentApp.Session.AppServerInfo.Address,
                            CurrentApp.Session.AppServerInfo.Port, strFileName);
                        CurrentApp.WriteLog("DownloadRecord", string.Format("Url:{0}", strUrl));
                        AudioPlayer.Url = strUrl;
                        AudioPlayer.Play();
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

        #endregion


        #region Create

        private void CreateRecordColumns()
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
                    gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4411001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4411001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    string strName = columnInfo.ColumnName;
                    gvc.DisplayMemberBinding = new Binding(strName);

                    gv.Columns.Add(gvc);
                }

                ListViewRecordList.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnNMon_Click(object sender, RoutedEventArgs e)
        {
            if (!mIsNMon)
            {
                CreateNMonClient();
                ImageNMon.SetResourceReference(StyleProperty, "SeatPlaybackImageCloseNMonStyle");
            }
            else
            {
                StopNMon();
                ImageNMon.SetResourceReference(StyleProperty, "SeatPlaybackImageNMonStyle");
            }
            mIsNMon = !mIsNMon;
        }

        void BtnSMon_Click(object sender, RoutedEventArgs e)
        {
            if (!mIsSMon)
            {
                CreateSMonClient();
                ImageSMon.SetResourceReference(StyleProperty, "SeatPlaybackImageCloseSMonStyle");
            }
            else
            {
                StopSMon();
                ImageSMon.SetResourceReference(StyleProperty, "SeatPlaybackImageSMonStyle");
            }
            mIsSMon = !mIsSMon;
        }

        void RecordLengthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (mStateMonItem == null) { return; }
                mStateMonItem.TimeDeviation = mTimeDeviation;
                Dispatcher.Invoke(new Action(UpdateExtItemState));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TabControlRecordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var tab = e.Source as TabControl;
                if (tab == null) { return; }
                var index = tab.SelectedIndex;
                if (index != 1) { return; }
                if (!mIsLogined || string.IsNullOrEmpty(mAgentID)) { return; }
                mListRecordItems.Clear();
                LoadRecordInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListViewRecordList_MouseDoubleClicked(object sender, RoutedPropertyChangedEventArgs<ListItemEventArgs> e)
        {
            try
            {
                var item = ListViewRecordList.SelectedItem as RecordInfoItem;
                if (item == null) { return; }
                mCurrentItem = item;
                PlayRecord();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region StateMon Operations

        private void CreateStateMonClient()
        {
            try
            {
                if (mStateMonItem == null) { return; }
                if (mStateMonClient != null)
                {
                    mStateMonClient.Stop();
                    mStateMonClient = null;
                }
                mStateMonClient = new NetClient();
                mStateMonClient.Debug += (mode, cat, msg) => CurrentApp.WriteLog("StateMonClient", string.Format("{0}\t{1}", cat, msg));
                mStateMonClient.ConnectionEvent += StateMonClient_ConnectionEvent;
                mStateMonClient.ReturnMessageReceived += StateMonClient_ReturnMessageReceived;
                mStateMonClient.NotifyMessageReceived += StateMonClient_NotifyMessageReceived;
                mStateMonClient.IsSSL = true;
                mStateMonClient.Host = CurrentApp.Session.AppServerInfo.Address;
                mStateMonClient.Port = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 10 - 1
                    : CurrentApp.Session.AppServerInfo.Port - 10;
                mStateMonClient.Connect();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CreateStateMonClient", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SetStateMonType()
        {
            try
            {
                if (mStateMonClient == null || !mStateMonClient.IsConnected) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mStateMonClient.SessionID;
                request.Command = (int)Service10Command.ReqSetMonType;
                request.ListData.Add(((int)MonType.State).ToString());
                mStateMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SetStateMonType", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void AddStateMonObjects()
        {
            try
            {
                if (mStateMonClient == null || !mStateMonClient.IsConnected) { return; }
                if (mStateMonClient == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mStateMonClient.SessionID;
                request.Command = (int)Service10Command.ReqAddMonObj;
                request.ListData.Add("0");
                request.ListData.Add("1");
                request.ListData.Add(mStateMonItem.Extension);
                mStateMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("AddStateMonObjects", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void QueryExtState()
        {
            try
            {
                if (mStateMonClient == null || !mStateMonClient.IsConnected) { return; }
                if (mStateMonClient == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mStateMonClient.SessionID;
                request.Command = (int)Service10Command.ReqQueryExtState;
                request.ListData.Add("1");
                request.ListData.Add(mStateMonItem.MonID);
                mStateMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("QueryExtState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealWelcomeMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData == null || retMessage.ListData.Count < 4)
                {
                    CurrentApp.WriteLog("DealWelcome", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strServerTime = retMessage.ListData[3];
                DateTime dtServerTime;
                if (DateTime.TryParse(strServerTime, out dtServerTime))
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    double timeDeviation = (now - dtServerTime).TotalSeconds;
                    mTimeDeviation = timeDeviation;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealWelcome", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealAddStateObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (mStateMonItem == null) { return; }
                if (retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealAddStateObj", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                CurrentApp.WriteLog("DealAddStateObj", string.Format("AddMonObjResponse \tValid:{0};Total:{1}", strValid, strTotal));
                if (retMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("DealAddStateObj", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strInfo = retMessage.ListData[2];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealAddObj", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                MonitorObject monObj = optReturn.Data as MonitorObject;
                if (monObj == null)
                {
                    CurrentApp.WriteLog("DealAddObj", string.Format("Fail.\tMonObject is null"));
                    return;
                }
                mStateMonItem.MonID = monObj.MonID;
                mStateMonItem.MonObject = monObj;
                QueryExtState();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealAddStateObj", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealQueryExtStateMessage(ReturnMessage retMessage)
        {
            try
            {
                if (mStateMonItem == null) { return; }
                if (retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealQueryExt", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strExtInfo = retMessage.ListData[1];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<ExtensionInfo>(strExtInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealQueryExt", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ExtensionInfo extInfo = optReturn.Data as ExtensionInfo;
                if (extInfo == null)
                {
                    CurrentApp.WriteLog("DealQueryExt", string.Format("Fail.\tExtInfo is null"));
                    return;
                }
                if (!strMonID.Equals(mStateMonItem.MonID))
                {
                    CurrentApp.WriteLog("DealQueryExt", string.Format("Fail.\tMonID invalid"));
                    return;
                }
                mStateMonItem.ExtensionInfo = extInfo;
                mStateMonItem.TimeDeviation = mTimeDeviation;
                Dispatcher.Invoke(new Action(UpdateExtItemState));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealQueryExt", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealExtStateChangedMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealExtState", string.Format("NotifyMessage param count invalid"));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strExtInfo = notMessage.ListData[1];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<ExtensionInfo>(strExtInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealExtState", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ExtensionInfo extInfo = optReturn.Data as ExtensionInfo;
                if (extInfo == null)
                {
                    CurrentApp.WriteLog("DealExtState", string.Format("ExtInfo is null"));
                    return;
                }
                if (!strMonID.Equals(mStateMonItem.MonID))
                {
                    CurrentApp.WriteLog("DealQueryExt", string.Format("Fail.\tMonID invalid"));
                    return;
                }
                mStateMonItem.ExtensionInfo = extInfo;
                mStateMonItem.TimeDeviation = mTimeDeviation;
                Dispatcher.Invoke(new Action(UpdateExtItemState));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealExtState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void UpdateExtItemState()
        {
            try
            {
                if (mStateMonItem == null) { return; }
                ExtensionInfo extInfo = mStateMonItem.ExtensionInfo;
                if (extInfo == null) { return; }

                string strAgentID = string.Empty;
                string strStatus = string.Empty;
                string strStartTime = string.Empty;
                string strDirection = string.Empty;
                string strCallerID = string.Empty;
                string strCalledID = string.Empty;
                string strRecordLength = string.Empty;
                string strValue;
                ExtStateInfo stateInfo;

                stateInfo =
                   extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_LOGIN);
                if (stateInfo != null)
                {
                    if (stateInfo.State > 0)
                    {
                        strValue =
                            CurrentApp.GetLanguageInfo(string.Format("4411022{0}", stateInfo.State.ToString("000")),
                                ((LoginState)stateInfo.State).ToString());

                        strStatus += string.Format("{0}; ", strValue);
                        strAgentID = extInfo.AgentID;
                        mIsLogined = true;
                    }
                    else
                    {
                        strAgentID = string.Empty;
                        mIsLogined = false;
                    }
                }

                stateInfo =
                    extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_CALL);
                if (stateInfo != null)
                {
                    if (stateInfo.State > 0)
                    {
                        strValue =
                          CurrentApp.GetLanguageInfo(string.Format("4411021{0}", stateInfo.State.ToString("000")),
                              ((CallState)stateInfo.State).ToString());

                        strStatus += string.Format("{0}; ", strValue);

                        if (stateInfo.State == (int)CallState.Ringing
                            || stateInfo.State == (int)CallState.Talking)
                        {
                            strStartTime = extInfo.VocStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            strCallerID = extInfo.CallerID;
                            strCalledID = extInfo.CalledID;
                        }
                    }
                }
                stateInfo =
                   extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                if (stateInfo != null)
                {
                    if (stateInfo.State > 0)
                    {
                        strValue =
                         CurrentApp.GetLanguageInfo(string.Format("4411024{0}", stateInfo.State.ToString("000")),
                             ((RecordState)stateInfo.State).ToString());

                        strStatus += string.Format("{0}; ", strValue);

                        if (stateInfo.State == (int)RecordState.All)
                        {
                            string strVoice = string.Empty;
                            string strScreen = string.Empty;
                            double serverTimeDeviation = extInfo.VocTimeDeviation;
                            double thisTimeDeviation = mTimeDeviation;
                            double timeDeviation = serverTimeDeviation + thisTimeDeviation;
                            DateTime now = DateTime.Now.ToUniversalTime();
                            DateTime start = extInfo.VocStartRecordTime;
                            if (start > DateTime.Parse("2014/1/1"))
                            {
                                TimeSpan ts = now - start;
                                int voice = (int)(ts.TotalSeconds - timeDeviation);
                                try
                                {
                                    strVoice = Converter.Second2Time(voice);
                                }
                                catch { }
                            }
                            serverTimeDeviation = extInfo.ScrTimeDiviation;
                            thisTimeDeviation = mTimeDeviation;
                            timeDeviation = serverTimeDeviation + thisTimeDeviation;
                            now = DateTime.Now.ToUniversalTime();
                            start = extInfo.ScrStartRecordTime;
                            if (start > DateTime.Parse("2014/1/1"))
                            {
                                TimeSpan ts = now - start;
                                int screen = (int)(ts.TotalSeconds - timeDeviation);
                                try
                                {
                                    strScreen = Converter.Second2Time(screen);
                                }
                                catch { }
                            }
                            strRecordLength = string.Format("V:{0} S:{1}", strVoice, strScreen);
                        }
                        else if (stateInfo.State == (int)RecordState.Voice)
                        {
                            string strVoice = string.Empty;
                            double serverTimeDeviation = extInfo.VocTimeDeviation;
                            double thisTimeDeviation = mTimeDeviation;
                            double timeDeviation = serverTimeDeviation + thisTimeDeviation;
                            DateTime now = DateTime.Now.ToUniversalTime();
                            DateTime start = extInfo.VocStartRecordTime;
                            if (start > DateTime.Parse("2014/1/1"))
                            {
                                TimeSpan ts = now - start;
                                int voice = (int)(ts.TotalSeconds - timeDeviation);
                                try
                                {
                                    strVoice = Converter.Second2Time(voice);
                                }
                                catch { }
                            }
                            strRecordLength = strVoice;
                        }
                        else if (stateInfo.State == (int)RecordState.Screen)
                        {
                            string strScreen = string.Empty;
                            double serverTimeDeviation = extInfo.ScrTimeDiviation;
                            double thisTimeDeviation = mTimeDeviation;
                            double timeDeviation = serverTimeDeviation + thisTimeDeviation;
                            DateTime now = DateTime.Now.ToUniversalTime();
                            DateTime start = extInfo.ScrStartRecordTime;
                            if (start > DateTime.Parse("2014/1/1"))
                            {
                                TimeSpan ts = now - start;
                                int screen = (int)(ts.TotalSeconds - timeDeviation);
                                try
                                {
                                    strScreen = Converter.Second2Time(screen);
                                }
                                catch { }
                            }
                            strRecordLength = strScreen;
                        }
                        else
                        {
                            strRecordLength = string.Empty;
                        }
                    }
                }
                stateInfo =
                  extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                if (stateInfo != null)
                {
                    if (stateInfo.State > 0)
                    {
                        strValue =
                        CurrentApp.GetLanguageInfo(string.Format("4411020{0}", stateInfo.State.ToString("000")),
                            ((DirectionState)stateInfo.State).ToString());

                        strStatus += string.Format("{0}; ", strValue);
                        strDirection = string.Format("{0}", strValue);
                    }
                }
                stateInfo =
                 extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_AGNET);
                if (stateInfo != null)
                {
                    if (stateInfo.State > 0)
                    {
                        strValue =
                      CurrentApp.GetLanguageInfo(string.Format("4411023{0}", stateInfo.State.ToString("000")),
                          ((AgentState)stateInfo.State).ToString());

                        strStatus += string.Format("{0}; ", strValue);
                    }
                }
                TxtAgentID.Text = strAgentID;
                TxtStatus.Text = strStatus;
                TxtStartTime.Text = strStartTime;
                TxtDirection.Text = strDirection;
                TxtCallerID.Text = strCallerID;
                TxtCalledID.Text = strCalledID;
                TxtRecordLength.Text = strRecordLength;
                mAgentID = strAgentID;

                InitInfo();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("UpdateExtState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region StateMon EventHandlers

        void StateMonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("ConnectionEvent", string.Format("{0}\t{1}", e.Code, e.Message));
                if (e.Code == Defines.EVT_NET_CONNECTED)
                {

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void StateMonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                if (!e.ReturnMessage.Result)
                {
                    CurrentApp.WriteLog("ErrorMessage",
                        string.Format("{0}\t{1}", e.ReturnMessage.Code, e.ReturnMessage.Message));
                }
                else
                {
                    CurrentApp.WriteLog("ReturnMessage", string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                    int command = e.ReturnMessage.Command;
                    switch (command)
                    {
                        case (int)RequestCode.NCWelcome:
                            CurrentApp.WriteLog("ReturnMessage", string.Format("Welcome message"));
                            DealWelcomeMessage(e.ReturnMessage);
                            SetStateMonType();
                            break;
                        case (int)Service10Command.ResSetMonType:
                            AddStateMonObjects();
                            break;
                        case (int)Service10Command.ResAddMonObj:
                            DealAddStateObjectMessage(e.ReturnMessage);
                            break;
                        case (int)Service10Command.ResQueryExtState:
                            DealQueryExtStateMessage(e.ReturnMessage);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void StateMonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("NotifyMessage", string.Format("{0}", ParseCommand(e.NotifyMessage.Command)));
                int command = e.NotifyMessage.Command;
                switch (command)
                {
                    case (int)Service10Command.NotExtStateChanged:
                        DealExtStateChangedMessage(e.NotifyMessage);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region NMonClient EventHandlers

        void NMonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("ConnectionEvent", string.Format("{0}\t{1}", e.Code, e.Message));
                if (e.Code == Defines.EVT_NET_CONNECTED)
                {

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void NMonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                if (!e.ReturnMessage.Result)
                {
                    CurrentApp.WriteLog("ErrorMessage",
                        string.Format("{0}\t{1}", e.ReturnMessage.Code, e.ReturnMessage.Message));
                }
                else
                {
                    CurrentApp.WriteLog("ReturnMessage", string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                    int command = e.ReturnMessage.Command;
                    switch (command)
                    {
                        case (int)RequestCode.NCWelcome:
                            CurrentApp.WriteLog("ReturnMessage", string.Format("Welcome message"));
                            ThreadPool.QueueUserWorkItem(a => SetNMonType());
                            break;
                        case (int)Service10Command.ResSetMonType:
                            ThreadPool.QueueUserWorkItem(a => AddNMonObjects());
                            break;
                        case (int)Service10Command.ResAddMonObj:
                            ThreadPool.QueueUserWorkItem(a => DealAddNMonObjectMessage(e.ReturnMessage));
                            break;
                        case (int)Service10Command.ResStartNMon:
                            ThreadPool.QueueUserWorkItem(a => DealStartNMonMessage(e.ReturnMessage));
                            break;
                        case (int)Service10Command.ResStopNMon:
                            CurrentApp.WriteLog("ReturnMessage", string.Format("ResStopNMon message"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void NMonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("NotifyMessage", string.Format("{0}", ParseCommand(e.NotifyMessage.Command)));
                int command = e.NotifyMessage.Command;
                switch (command)
                {
                    case (int)Service10Command.NotNMonHeadReceived:
                        ThreadPool.QueueUserWorkItem(a => DealNMonHeadMessage(e.NotifyMessage));
                        break;
                    case (int)Service10Command.NotNMonDataReceived:
                        ThreadPool.QueueUserWorkItem(a => DealNMonDataMessage(e.NotifyMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region NMon Operations

        private void CreateNMonClient()
        {
            try
            {
                if (mNMonItem == null) { return; }
                if (mNMonClient != null)
                {
                    mNMonClient.Stop();
                    mNMonClient = null;
                }
                mNMonClient = new NetClient();
                mNMonClient.Debug += (mode, cat, msg) => CurrentApp.WriteLog("NMonClient", string.Format("{0}\t{1}", cat, msg));
                mNMonClient.ConnectionEvent += NMonClient_ConnectionEvent;
                mNMonClient.ReturnMessageReceived += NMonClient_ReturnMessageReceived;
                mNMonClient.NotifyMessageReceived += NMonClient_NotifyMessageReceived;
                mNMonClient.IsSSL = true;
                mNMonClient.Host = CurrentApp.Session.AppServerInfo.Address;
                mNMonClient.Port = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 10 - 1
                    : CurrentApp.Session.AppServerInfo.Port - 10;
                mNMonClient.Connect();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CreateNMonClient", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SetNMonType()
        {
            try
            {
                if (mNMonClient == null || !mNMonClient.IsConnected) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mNMonClient.SessionID;
                request.Command = (int)Service10Command.ReqSetMonType;
                request.ListData.Add(((int)MonType.NMon).ToString());
                mNMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("NMonSetMonType", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void AddNMonObjects()
        {
            try
            {
                if (mNMonClient == null || !mNMonClient.IsConnected) { return; }
                if (mNMonItem == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mNMonClient.SessionID;
                request.Command = (int)Service10Command.ReqAddMonObj;
                request.ListData.Add("0");
                request.ListData.Add("1");
                request.ListData.Add(mNMonItem.Extension);
                mNMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("AddNMonObjects", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void StartNMon()
        {
            try
            {
                if (mNMonClient == null || !mNMonClient.IsConnected) { return; }
                if (mNMonItem == null) { return; }
                string strMonID = mNMonItem.MonID;
                RequestMessage request = new RequestMessage();
                request.SessionID = mNMonClient.SessionID;
                request.Command = (int)Service10Command.ReqStartNMon;
                request.ListData.Add(strMonID);
                mNMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StartNMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void StopNMon()
        {
            try
            {
                if (mNMonItem == null) { return; }
                string strMonID = mNMonItem.MonID;
                RequestMessage request = new RequestMessage();
                request.SessionID = mNMonClient.SessionID;
                request.Command = (int)Service10Command.ReqStopNMon;
                request.ListData.Add(strMonID);
                mNMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StopNMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealAddNMonObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealAddNMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                CurrentApp.WriteLog("DealAddNMon", string.Format("Valid:{0};Total:{1}", strValid, strTotal));
                if (retMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("DealAddNMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strInfo = retMessage.ListData[2];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealAddNMon", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                MonitorObject monObj = optReturn.Data as MonitorObject;
                if (monObj == null)
                {
                    CurrentApp.WriteLog("DealAddNMon", string.Format("Fail.\tMonObject is null"));
                    return;
                }
                if (mNMonItem == null) { return; }
                mNMonItem.MonID = monObj.MonID;
                mNMonItem.MonObject = monObj;
                StartNMon();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealAddNMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealStartNMonMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 5)
                {
                    CurrentApp.WriteLog("DealStartNMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strMonPort = retMessage.ListData[1];
                string strTransAudioData = retMessage.ListData[2];
                string strAddress = retMessage.ListData[3];
                string strChanID = retMessage.ListData[4];
                CurrentApp.WriteLog("DealStartNMon", string.Format("MonID:{0};MonPort:{1};TransAudioData:{2};Address:{3};ChanID:{4}", strMonID,
                    strMonPort, strTransAudioData, strAddress, strChanID));
                if (strTransAudioData == "0")
                {
                    int intMonPort;
                    int intChan;
                    if (!int.TryParse(strMonPort, out intMonPort)
                        || !int.TryParse(strChanID, out intChan))
                    {
                        CurrentApp.WriteLog("DealStartNMon", string.Format("MonPort or ChanID invalid."));
                        return;
                    }
                    if (mNMonCore != null)
                    {
                        mNMonCore.StopMon();
                        mNMonCore = null;
                    }
                    mNMonCore = new NMonCore(mNMonItem.MonObject);
                    mNMonCore.Debug += (s, msg) => CurrentApp.WriteLog("DealStartNMon", string.Format("[NMonCore]\t{0}", msg));
                    mNMonCore.IsConnectServer = true;
                    mNMonCore.IsDecodeData = true;
                    mNMonCore.IsPlayWave = true;
                    NETMON_PARAM param = new NETMON_PARAM();
                    param.Host = strAddress;
                    param.Port = intMonPort;
                    param.Channel = intChan;
                    CurrentApp.WriteLog("DealStartNMon", string.Format("Monitor param \tHost:{0};Port:{1};Chan:{2}", param.Host, param.Port,
                        param.Channel));
                    mNMonCore.StartMon(param);
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealStartNMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealNMonHeadMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData == null || notMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealNMonHead", string.Format("NotifyMessage param count invalid."));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strHead = notMessage.ListData[1];
                if (mNMonItem == null) { return; }
                if (mNMonItem.MonID != strMonID) { return; }
                MonitorObject monObj = mNMonItem.MonObject;
                if (monObj == null) { return; }
                byte[] data = Converter.Hex2Byte(strHead);
                SNM_RESPONSE head = (SNM_RESPONSE)Converter.Bytes2Struct(data, typeof(SNM_RESPONSE));
                if (mNMonCore != null)
                {
                    mNMonCore.StopMon();
                    mNMonCore = null;
                }
                mNMonCore = new NMonCore(monObj);
                mNMonCore.IsConnectServer = false;
                mNMonCore.IsDecodeData = false;
                mNMonCore.Debug += (s, msg) => CurrentApp.WriteLog("NMonCore", string.Format("{0}", msg));
                mNMonCore.ReceiveHead(head);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealNMonHead", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealNMonDataMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData == null || notMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("DealNMonData", string.Format("NotifyMessage param count invalid."));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strLength = notMessage.ListData[1];
                string strData = notMessage.ListData[2];
                if (mNMonItem == null) { return; }
                if (mNMonItem.MonID != strMonID) { return; }
                MonitorObject monObj = mNMonItem.MonObject;
                if (monObj == null) { return; }
                int intLength;
                if (!int.TryParse(strLength, out intLength))
                {
                    CurrentApp.WriteLog("DealNMonData", string.Format("DataLength invalid."));
                    return;
                }
                byte[] tempData = new byte[intLength];
                byte[] data = Converter.Hex2Byte(strData);
                Array.Copy(data, 0, tempData, 0, intLength);
                if (mNMonCore == null
                    || mNMonCore.User == null)
                {
                    CurrentApp.WriteLog("DealNMonData", string.Format("NMonCore or its user is null."));
                    return;
                }
                var temp = mNMonCore.User as MonitorObject;
                if (temp == null || temp.MonID != monObj.MonID)
                {
                    CurrentApp.WriteLog("DealNMonData", string.Format("MonObject not exist."));
                    return;
                }
                mNMonCore.ReceiveData(tempData, intLength);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealNMonData", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region SMon EventHandlers

        void SMonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("ConnectionEvent", string.Format("{0}\t{1}", e.Code, e.Message));
                if (e.Code == Defines.EVT_NET_CONNECTED)
                {

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SMonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                if (!e.ReturnMessage.Result)
                {
                    CurrentApp.WriteLog("ErrorMessage",
                        string.Format("{0}\t{1}", e.ReturnMessage.Code, e.ReturnMessage.Message));
                }
                else
                {
                    CurrentApp.WriteLog("ReturnMessage", string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                    int command = e.ReturnMessage.Command;
                    switch (command)
                    {
                        case (int)RequestCode.NCWelcome:
                            CurrentApp.WriteLog("ReturnMessage", string.Format("Welcome message"));
                            ThreadPool.QueueUserWorkItem(a => SetSMonType());
                            break;
                        case (int)Service10Command.ResSetMonType:
                            ThreadPool.QueueUserWorkItem(a => AddSMonObjects());
                            break;
                        case (int)Service10Command.ResAddMonObj:
                            ThreadPool.QueueUserWorkItem(a => DealAddSMonObjectMessage(e.ReturnMessage));
                            break;
                        case (int)Service10Command.ResStartSMon:
                            //ThreadPool.QueueUserWorkItem(a => DealStartSMonMessage(e.ReturnMessage));
                            Dispatcher.Invoke(new Action(() => DealStartSMonMessage(e.ReturnMessage)));
                            break;
                        case (int)Service10Command.ResStopSMon:
                            CurrentApp.WriteLog("ReturnMessage", string.Format("ResStopSMon message"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SMonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("NotifyMessage", string.Format("{0}", ParseCommand(e.NotifyMessage.Command)));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region SMon Operations

        private void CreateSMonClient()
        {
            try
            {
                if (mSMonItem == null) { return; }
                if (mSMonClient != null)
                {
                    mSMonClient.Stop();
                    mSMonClient = null;
                }
                mSMonClient = new NetClient();
                mSMonClient.Debug += (mode, cat, msg) => CurrentApp.WriteLog("SMonClient", string.Format("{0}\t{1}", cat, msg));
                mSMonClient.ConnectionEvent += SMonClient_ConnectionEvent;
                mSMonClient.ReturnMessageReceived += SMonClient_ReturnMessageReceived;
                mSMonClient.NotifyMessageReceived += SMonClient_NotifyMessageReceived;
                mSMonClient.IsSSL = true;
                mSMonClient.Host = CurrentApp.Session.AppServerInfo.Address;
                mSMonClient.Port = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 10 - 1
                    : CurrentApp.Session.AppServerInfo.Port - 10;
                mSMonClient.Connect();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CreateSMonClient", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SetSMonType()
        {
            try
            {
                if (mSMonClient == null || !mSMonClient.IsConnected) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mSMonClient.SessionID;
                request.Command = (int)Service10Command.ReqSetMonType;
                request.ListData.Add(((int)MonType.MonScr).ToString());
                mSMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SMonSetMonType", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void AddSMonObjects()
        {
            try
            {
                if (mSMonClient == null || !mSMonClient.IsConnected) { return; }
                if (mSMonItem == null) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mSMonClient.SessionID;
                request.Command = (int)Service10Command.ReqAddMonObj;
                request.ListData.Add("0");
                request.ListData.Add("1");
                request.ListData.Add(mSMonItem.Extension);
                mSMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("AddSMonObjects", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void StartSMon()
        {
            try
            {
                if (mSMonClient == null || !mSMonClient.IsConnected) { return; }
                if (mSMonItem == null) { return; }
                string strMonID = mSMonItem.MonID;
                RequestMessage request = new RequestMessage();
                request.SessionID = mSMonClient.SessionID;
                request.Command = (int)Service10Command.ReqStartSMon;
                request.ListData.Add(strMonID);
                mSMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StartSMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void StopSMon()
        {
            try
            {
                if (mSMonItem == null) { return; }
                string strMonID = mSMonItem.MonID;
                RequestMessage request = new RequestMessage();
                request.SessionID = mSMonClient.SessionID;
                request.Command = (int)Service10Command.ReqStopSMon;
                request.ListData.Add(strMonID);
                mSMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StopSMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealAddSMonObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealAddSMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                CurrentApp.WriteLog("DealAddNMon", string.Format("Valid:{0};Total:{1}", strValid, strTotal));
                if (retMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("DealAddSMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strInfo = retMessage.ListData[2];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealAddSMon", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                MonitorObject monObj = optReturn.Data as MonitorObject;
                if (monObj == null)
                {
                    CurrentApp.WriteLog("DealAddSMon", string.Format("Fail.\tMonObject is null"));
                    return;
                }
                if (mSMonItem == null) { return; }
                mSMonItem.MonID = monObj.MonID;
                mSMonItem.MonObject = monObj;
                StartSMon();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealAddSMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealStartSMonMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 4)
                {
                    CurrentApp.WriteLog("DealStartSMon", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strMonPort = retMessage.ListData[1];
                string strAddress = retMessage.ListData[2];
                string strChanID = retMessage.ListData[3];
                CurrentApp.WriteLog("DealStartSMon", string.Format("MonID:{0};MonPort:{1};Address:{2};ChanID:{3}", strMonID,
                    strMonPort, strAddress, strChanID));
                int intMonPort;
                int intChan;
                if (!int.TryParse(strMonPort, out intMonPort)
                    || !int.TryParse(strChanID, out intChan))
                {
                    CurrentApp.WriteLog("DealStartSMon", string.Format("MonPort or ChanID invalid."));
                    return;
                }

                OperationReturn optReturn = Utils.DownloadMediaUtils(CurrentApp.Session);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealStartSMon",
                        string.Format("Download MediaUtil fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                //获取本窗口的句柄
                int handle = 0;
                HwndSource hwndSource = HwndSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    //hwndSource.AddHook(WndProc);
                    handle = hwndSource.Handle.ToInt32();
                }
                int intReturn;
                //设置反馈进度的频率
                intReturn = ScreenMPInterop.VLSMonSetWaterMark(1);
                CurrentApp.WriteLog("DealStartSMon", string.Format("VLSMonSetWaterMarkReturn:{0}", intReturn));
                //设置窗口尺寸
                int intScale = 100;
                intReturn = ScreenMPInterop.VLSMonSetScale(intScale);
                CurrentApp.WriteLog("DealStartSMon", string.Format("VLSMonSetScaleReturn:{0}", intReturn));
                SRCMON_PARAM param = new SRCMON_PARAM();
                param.sVocIp = Converter.String2ByteArray(strAddress, ScreenMPDefines.SIZE_IPADDRESS);
                param.nPort = (ushort)intMonPort;
                param.nChannel = (ushort)intChan;
                intReturn = ScreenMPInterop.VLSMonStart(ref param, null, handle);
                CurrentApp.WriteLog("DealStartSMon", string.Format("VLSMonStartReturn:{0}", intReturn));
                if (intReturn == 0)
                {
                    mCanCloseScreenWindow = true;
                    //获取播放窗口句柄
                    IntPtr hwdPlayer = ScreenMPInterop.VLSMonGetPlayerHwnd();
                    intReturn = User32Interop.SetWindowPos(hwdPlayer, -1, 0, 0, 0, 0, 3);
                    CurrentApp.WriteLog("DealStartSMon", string.Format("SetWindowPos:{0}", intReturn));
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealStartSMon", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private string ParseCommand(int command)
        {
            string str;
            str = ((RequestCode)command).ToString();
            if (command >= 1000 && command < 10000)
            {
                str = ((Service10Command)command).ToString();
            }
            return str;
        }

        public void Close()
        {
            try
            {
                AudioPlayer.Close();
            }
            catch { }
            try
            {
                if (mCanCloseScreenWindow)
                {
                    ScreenMPInterop.VLSMonCloseWnd();
                }
            }
            catch { }
            if (mNMonCore != null)
            {
                mNMonCore.StopMon();
                mNMonCore = null;
            }
            if (mNMonClient != null)
            {
                mNMonClient.Stop();
                mNMonClient = null;
            }
            if (mSMonClient != null)
            {
                mSMonClient.Stop();
                mSMonClient = null;
            }
            if (mStateMonClient != null)
            {
                mStateMonClient.Stop();
                mStateMonClient = null;
            }
        }

        private OperationReturn GetRecordInfoFromJsonObject(JsonObject json)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                UMPRecordInfo recordInfo = new UMPRecordInfo();
                recordInfo.RowID = Convert.ToInt64(json[UMPRecordInfo.PRO_ROWID].Number);
                recordInfo.SerialID = json[UMPRecordInfo.PRO_SERIALID].Value;
                recordInfo.RecordReference = json[UMPRecordInfo.PRO_RECORDREFERENCE].Value;
                recordInfo.StartRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STARTRECORDTIME].Value);
                recordInfo.StopRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STOPRECORDTIME].Value);
                recordInfo.Extension = json[UMPRecordInfo.PRO_EXTENSION].Value;
                recordInfo.Agent = json[UMPRecordInfo.PRO_AGENT].Value;
                recordInfo.MediaType = Convert.ToInt32(json[UMPRecordInfo.PRO_MEDIATYPE].Number);
                recordInfo.EncryptFlag = json[UMPRecordInfo.PRO_ENCRYPTFLAG].Value;
                recordInfo.ServerID = Convert.ToInt32(json[UMPRecordInfo.PRO_SERVERID].Number);
                recordInfo.ServerIP = json[UMPRecordInfo.PRO_SERVERIP].Value;
                recordInfo.ChannelID = Convert.ToInt32(json[UMPRecordInfo.PRO_CHANNELID].Number);
                recordInfo.StringInfo = json.ToString();

                optReturn.Data = recordInfo;
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

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                string strExt = string.Empty;
                if (RegionSeatItem != null)
                {
                    strExt = RegionSeatItem.Extension;
                }
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (mIsLogined)
                    {
                        parent.Title = string.Format("{0} --- {1}",
                            CurrentApp.GetLanguageInfo("4411010", "Playback and Monitor"), mAgentID);
                    }
                    else
                    {
                        parent.Title = string.Format("{0} --- {1}",
                            CurrentApp.GetLanguageInfo("4411010", "Playback and Monitor"), strExt);
                    }
                }

                TabDetail.Header = CurrentApp.GetLanguageInfo("4411011", "Detail Information");
                TabRecordList.Header = CurrentApp.GetLanguageInfo("4411012", "Record List");

                LbAgent.Text = CurrentApp.GetLanguageInfo("4411002", "Agent");
                LbExtension.Text = CurrentApp.GetLanguageInfo("4411003", "Extension");
                LbStatus.Text = CurrentApp.GetLanguageInfo("4411004", "Status");
                LbStartTime.Text = CurrentApp.GetLanguageInfo("4411005", "Start Time");
                LbDirection.Text = CurrentApp.GetLanguageInfo("4411006", "Direction");
                LbCallerID.Text = CurrentApp.GetLanguageInfo("4411007", "Caller");
                LbCalledID.Text = CurrentApp.GetLanguageInfo("4411008", "Called");
                LbRecordLength.Text = CurrentApp.GetLanguageInfo("4411009", "Record Length");

                CreateRecordColumns();
            }
            catch { }
        }

        #endregion

    }
}
