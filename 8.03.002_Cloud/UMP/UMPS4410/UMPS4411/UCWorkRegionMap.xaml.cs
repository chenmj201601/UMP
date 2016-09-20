//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2dca0c3b-e06f-45fc-9127-d24b1ef8b67c
//        CLR Version:              4.0.30319.18408
//        Name:                     UCWorkRegionMap
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCWorkRegionMap
//
//        created by Charley at 2016/6/17 11:09:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UMPS4411.Models;
using UMPS4411.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4411
{
    /// <summary>
    /// UCWorkRegionMap.xaml 的交互逻辑
    /// </summary>
    public partial class UCWorkRegionMap : IOnsiteView
    {

        #region Members

        public OnsiteMonitorMainView PageParent;
        public ObjItem RegionItem;
        public List<SeatInfo> ListAllSeatInfos;
        public List<AgentStateInfo> ListAllStateInfos;

        private List<RegionSeatInfo> mListRegionSeats;
        private List<MonitorObject> mListMonitorObjects;
        private StateSetting mStateSetting;
        private ObservableCollection<RegionSeatItem> mListSeatItems;
        private List<StateDurationInfo> mListStateDurationInfos;
        private List<AlarmMessageInfo> mListAlarmMessageInfos;

        private bool mIsInited;
        private RegionInfo mRegionInfo;
        private NetClient mMonClient;
        private double mTimeDeviation;      //与监视服务器的时间差
        private Timer mRecordLengthTimer;

        #endregion


        public UCWorkRegionMap()
        {
            InitializeComponent();

            mListRegionSeats = new List<RegionSeatInfo>();
            mListSeatItems = new ObservableCollection<RegionSeatItem>();
            mListMonitorObjects = new List<MonitorObject>();
            mListStateDurationInfos = new List<StateDurationInfo>();
            mListAlarmMessageInfos = new List<AlarmMessageInfo>();

            Loaded += UCWorkRegionMap_Loaded;
            Unloaded += UCWorkRegionMap_Unloaded;
            PopupPanel.Closing += PopupPanel_Closing;

            mTimeDeviation = 0;
            mRecordLengthTimer = new Timer(1000);
        }

        void UCWorkRegionMap_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void UCWorkRegionMap_Unloaded(object sender, RoutedEventArgs e)
        {
            mRecordLengthTimer.Stop();
            if (mMonClient != null)
            {
                mMonClient.Stop();
                mMonClient = null;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                CommandBindings.Add(new CommandBinding(UCSeatStatusViewer.ItemClickCommand,
                    SeatItemClickCommand_Executed, (s, e) => e.CanExecute = true));
                ListBoxRegionSeats.ItemsSource = mListSeatItems;
                mRecordLengthTimer.Elapsed += RecordLengthTimer_Elapsed;

                InitStateSetting();
                if (RegionItem == null) { return; }
                var regionInfo = RegionItem.Data as RegionInfo;
                mRegionInfo = regionInfo;
                if (mRegionInfo != null)
                {
                    if (mRegionInfo.Width > 0)
                    {
                        BorderRegionMap.Width = mRegionInfo.Width;
                    }
                    if (mRegionInfo.Height > 0)
                    {
                        BorderRegionMap.Height = mRegionInfo.Height;
                    }
                    if (!string.IsNullOrEmpty(mRegionInfo.BgColor))
                    {
                        try
                        {
                            string strColor = mRegionInfo.BgColor;
                            if (strColor.StartsWith("#"))
                            {
                                strColor = strColor.Substring(1);
                            }
                            if (strColor.Length == 8)
                            {
                                strColor = strColor.Substring(2);
                            }
                            BorderRegionMap.Background = new SolidColorBrush(Utils.GetColorFromRgbString(strColor));
                        }
                        catch { }
                    }
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAlarmMessageInfos();
                    LoadRegionSeatInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateSeatItems();
                    CreateMonClient();

                    mRecordLengthTimer.Start();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStateSetting()
        {
            try
            {
                StateSetting setting = new StateSetting();
                setting.Name = "Default";

                StateSettingItem item = new StateSettingItem();       //logon
                item.Type = 1;
                item.Number = 1;
                item.Value = 1;
                item.Description = "logon";
                setting.ListItemBorderStates.Add(item);

                item = new StateSettingItem();      //logon
                item.Type = 1;
                item.Number = 1;
                item.Value = 1;
                item.Description = "logon";
                setting.ListHeadBackgroundStates.Add(item);

                item = new StateSettingItem();      //blank
                item.Type = 5;
                item.Number = 9;
                item.Value = 0;
                item.Description = "blank";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //notready
                item.Type = 5;
                item.Number = 11;
                item.Value = 3;
                item.Description = "notready";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //ready
                item.Type = 5;
                item.Number = 10;
                item.Value = 2;
                item.Description = "ready";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //ACW
                item.Type = 5;
                item.Number = 12;
                item.Value = 4;
                item.Description = "aftercallwork";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //inbound
                item.Type = 4;
                item.Number = 7;
                item.Value = 1;
                item.Description = "inbound";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //outbound
                item.Type = 4;
                item.Number = 8;
                item.Value = 2;
                item.Description = "outbound";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //dialing
                item.Type = 2;
                item.Number = 3;
                item.Value = 1;
                item.Description = "dialing";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //Ringing
                item.Type = 2;
                item.Number = 4;
                item.Value = 2;
                item.Description = "Ringing";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //Talking
                item.Type = 2;
                item.Number = 5;
                item.Value = 3;
                item.Description = "Talking";
                setting.ListContentBackgroundStates.Add(item);
                item = new StateSettingItem();      //Recording
                item.Type = 3;
                item.Number = 6;
                item.Value = 1;
                item.Description = "Recording";
                setting.ListContentBackgroundStates.Add(item);

                item = new StateSettingItem();      //blank
                item.Type = 5;
                item.Number = 9;
                item.Value = 0;
                item.Description = "blank";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //notready
                item.Type = 5;
                item.Number = 11;
                item.Value = 3;
                item.Description = "notready";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //ready
                item.Type = 5;
                item.Number = 10;
                item.Value = 2;
                item.Description = "ready";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //ACW
                item.Type = 5;
                item.Number = 12;
                item.Value = 4;
                item.Description = "aftercallwork";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //inbound
                item.Type = 4;
                item.Number = 7;
                item.Value = 1;
                item.Description = "inbound";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //outbound
                item.Type = 4;
                item.Number = 8;
                item.Value = 2;
                item.Description = "outbound";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //dialing
                item.Type = 2;
                item.Number = 3;
                item.Value = 1;
                item.Description = "dialing";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //Ringing
                item.Type = 2;
                item.Number = 4;
                item.Value = 2;
                item.Description = "Ringing";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //Talking
                item.Type = 2;
                item.Number = 5;
                item.Value = 3;
                item.Description = "Talking";
                setting.ListLabelMainStates.Add(item);
                item = new StateSettingItem();      //Recording
                item.Type = 3;
                item.Number = 6;
                item.Value = 1;
                item.Description = "Recording";
                setting.ListLabelMainStates.Add(item);


                mStateSetting = setting;

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("{0}\\{1}", ConstValue.UMP_PRODUCTER_SHORTNAME, CurrentApp.AppName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string file = Path.Combine(path, StateSetting.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(setting, file);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("InitStateSetting", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
                else
                {
                    CurrentApp.WriteLog("InitStateSetting", string.Format("End.\t{0}", file));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionSeatInfos()
        {
            try
            {
                mListRegionSeats.Clear();
                if (RegionItem == null) { return; }
                long regionID = RegionItem.ObjID;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetRegionSeatList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(regionID.ToString());
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
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RegionSeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RegionSeatInfo info = optReturn.Data as RegionSeatInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tRegionSeatInfo is null"));
                        return;
                    }
                    mListRegionSeats.Add(info);
                }

                CurrentApp.WriteLog("LoadRegionSeatInfo", string.Format("Load end.\t{0}", mListRegionSeats.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAlarmMessageInfos()
        {
            try
            {
                mListAlarmMessageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAlarmMessageList;
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
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                int count = webReturn.ListData.Count;
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<AlarmMessageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmMessageInfo info = optReturn.Data as AlarmMessageInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tAlarmMessage is null"));
                        return;
                    }
                    mListAlarmMessageInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAlarmMessage", string.Format("Load end.\t{0}", mListAlarmMessageInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateSeatItems()
        {
            try
            {
                mListSeatItems.Clear();
                if (mRegionInfo == null) { return; }
                long regionID = mRegionInfo.ObjID;
                for (int i = 0; i < mListRegionSeats.Count; i++)
                {
                    var regionSeat = mListRegionSeats[i];
                    RegionSeatItem item = new RegionSeatItem();
                    item.CurrentApp = CurrentApp;
                    item.Info = regionSeat;
                    item.RegionID = regionID;
                    item.SeatID = regionSeat.SeatID;
                    item.SeatName = regionSeat.SeatID.ToString();
                    if (ListAllSeatInfos != null)
                    {
                        var temp = ListAllSeatInfos.FirstOrDefault(s => s.ObjID == regionSeat.SeatID);
                        if (temp != null)
                        {
                            item.SeatName = temp.Name;
                            item.Extension = temp.Extension;
                            item.SeatInfo = temp;
                        }
                    }
                    item.Left = regionSeat.Left;
                    item.Top = regionSeat.Top;
                    item.BorderBrush = Brushes.DarkGray;
                    item.Background = Brushes.LightGray;
                    mListSeatItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetMapScale(double viewScale)
        {
            try
            {
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = viewScale;
                tran.ScaleY = viewScale;
                BorderRegionMap.LayoutTransform = tran;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateMonClient()
        {
            try
            {
                if (mMonClient != null)
                {
                    mMonClient.Stop();
                    mMonClient = null;
                }
                mMonClient = new NetClient();
                mMonClient.Debug +=
                    (mode, cat, msg) => CurrentApp.WriteLog("MonClient", string.Format("{0}\t{1}", cat, msg));
                mMonClient.ConnectionEvent += MonClient_ConnectionEvent;
                mMonClient.ReturnMessageReceived += MonClient_ReturnMessageReceived;
                mMonClient.NotifyMessageReceived += MonClient_NotifyMessageReceived;
                mMonClient.IsSSL = true;
                mMonClient.Host = CurrentApp.Session.AppServerInfo.Address;
                mMonClient.Port = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 11
                    : CurrentApp.Session.AppServerInfo.Port - 10;
                mMonClient.Connect();

                CurrentApp.WriteLog("CreateMonClient", string.Format("End.\t{0}:{1}", mMonClient.Host, mMonClient.Port));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CreateMonClient", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public void Close()
        {
            mRecordLengthTimer.Stop();
            if (mMonClient != null)
            {
                mMonClient.Stop();
                mMonClient = null;
            }
        }

        #endregion


        #region MonClient Operations

        private void SetMonType()
        {
            try
            {
                if (mMonClient == null || !mMonClient.IsConnected) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mMonClient.SessionID;
                request.Command = (int)Service10Command.ReqSetMonType;
                request.ListData.Add(((int)MonType.State).ToString());
                mMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SetMonType", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void AddMonObjects()
        {
            try
            {
                if (mMonClient == null || !mMonClient.IsConnected) { return; }
                List<string> listExts = new List<string>();
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];
                    var ext = item.Extension;
                    if (string.IsNullOrEmpty(ext)) { continue; }
                    listExts.Add(ext);
                }
                int count = listExts.Count;
                RequestMessage request = new RequestMessage();
                request.SessionID = mMonClient.SessionID;
                request.Command = (int)Service10Command.ReqAddMonObj;
                request.ListData.Add("0");
                request.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    request.ListData.Add(listExts[i]);
                }
                mMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("AddMonObjects", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void QueryExtState(List<string> listMonIDs)
        {
            try
            {
                if (mMonClient == null || !mMonClient.IsConnected) { return; }
                int count = listMonIDs.Count;
                RequestMessage request = new RequestMessage();
                request.SessionID = mMonClient.SessionID;
                request.Command = (int)Service10Command.ReqQueryExtState;
                request.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    string strMonID = listMonIDs[i];
                    request.ListData.Add(strMonID);
                }
                mMonClient.SendMessage(request);
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

        private void DealAddMonObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealAddObj", string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                CurrentApp.WriteLog("DealAddObj", string.Format("AddMonObjResponse \tValid:{0};Total:{1}", strValid, strTotal));
                int valid;
                if (!int.TryParse(strValid, out valid)
                    || valid < 0)
                {
                    CurrentApp.WriteLog("DealAddObj", string.Format("Valid count invalid.\t{0}", strValid));
                    return;
                }
                if (retMessage.ListData.Count < 2 + valid)
                {
                    CurrentApp.WriteLog("DealAddObj", string.Format("MonObject count invalid."));
                    return;
                }
                List<string> listMonIDs = new List<string>();
                OperationReturn optReturn;
                for (int i = 0; i < valid; i++)
                {
                    string strInfo = retMessage.ListData[i + 2];
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
                    var temp = mListMonitorObjects.FirstOrDefault(m => m.MonID == monObj.MonID);
                    if (temp == null)
                    {
                        mListMonitorObjects.Add(monObj);
                    }
                    var item = mListSeatItems.FirstOrDefault(e => e.Extension == monObj.Name);
                    if (item != null)
                    {
                        item.MonID = monObj.MonID;
                        item.MonObject = monObj;
                    }
                    listMonIDs.Add(monObj.MonID);
                }
                QueryExtState(listMonIDs);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealAddObj", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealQueryExtStateMessage(ReturnMessage retMessage)
        {
            try
            {
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
                var item = mListSeatItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtensionInfo = extInfo;
                    item.TimeDeviation = mTimeDeviation;
                    UpdateExtItemState(item);
                }
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
                var item = mListSeatItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtensionInfo = extInfo;
                    UpdateExtItemState(item);
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealExtState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void UpdateExtItemState(RegionSeatItem item)
        {
            try
            {

                ExtensionInfo extInfo = item.ExtensionInfo;
                if (extInfo == null) { return; }
                if (ListAllStateInfos == null) { return; }
                var viewer = item.Viewer;
                var detail = item.DetailViewer;
                ExtStateInfo stateInfo;


                #region 设置登录状态及坐席

                stateInfo =
                   extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_LOGIN);
                if (stateInfo != null)
                {
                    string strColor = S4410Consts.COLOR_DEFAULT_LOGON;
                    var agentStateInfo =
                        ListAllStateInfos.FirstOrDefault(s => s.Number == S4410Consts.STATE_NUMBER_LOGON);
                    if (agentStateInfo != null)
                    {
                        strColor = agentStateInfo.Color;
                    }
                    string strAgentID;
                    bool isLogined;
                    if (stateInfo.State > 0)
                    {
                        strAgentID = extInfo.AgentID;
                        isLogined = true;
                    }
                    else
                    {
                        strColor = S4410Consts.COLOR_DEFAULT_LOGOFF;
                        strAgentID = string.Empty;
                        isLogined = false;
                    }
                    if (viewer != null)
                    {
                        viewer.SetItemBorderBrush(strColor);
                        viewer.SetHeadBackground(strColor);
                        viewer.SetLabelAgent(strAgentID);
                        viewer.SetIsLogined(isLogined);
                    }
                }

                #endregion


                #region 设置呼叫方向

                stateInfo =
                  extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                if (stateInfo != null)
                {
                    if (viewer != null)
                    {
                        viewer.SetIsCalling(stateInfo.State > 0, stateInfo.State);
                    }
                }

                #endregion


                #region 设置背景颜色

                if (mStateSetting != null)
                {
                    string strColor = string.Empty;
                    var stateSettings = mStateSetting.ListContentBackgroundStates;
                    for (int i = stateSettings.Count - 1; i >= 0; i--)
                    {
                        var setting = stateSettings[i];
                        int stateType = setting.Type;
                        if (stateType == S4410Consts.STATE_TYPE_LOGIN)
                        {
                            var agentState =
                               extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_LOGIN);
                            if (agentState != null)
                            {
                                if (agentState.State >= setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strColor = state.Color;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_CALL)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_CALL);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strColor = state.Color;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_RECORD)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                            if (agentState != null)
                            {
                                if (agentState.State >= setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strColor = state.Color;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_DIRECTION)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strColor = state.Color;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_AGNET)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_AGNET);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strColor = state.Color;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (viewer != null)
                    {
                        viewer.SetContentBackground(strColor);
                    }
                }

                #endregion


                #region 设置LabelMain

                if (mStateSetting != null)
                {
                    string strContent = string.Empty;
                    var stateSettings = mStateSetting.ListLabelMainStates;
                    for (int i = stateSettings.Count - 1; i >= 0; i--)
                    {
                        var setting = stateSettings[i];
                        int stateType = setting.Type;
                        if (stateType == S4410Consts.STATE_TYPE_LOGIN)
                        {
                            var agentState =
                               extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_LOGIN);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strContent = state.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_CALL)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_CALL);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strContent = state.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_RECORD)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strContent = state.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_DIRECTION)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strContent = state.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        if (stateType == S4410Consts.STATE_TYPE_AGNET)
                        {
                            var agentState =
                                extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_AGNET);
                            if (agentState != null)
                            {
                                if (agentState.State == setting.Value)
                                {
                                    var state =
                                        ListAllStateInfos.FirstOrDefault(s => s.Number == setting.Number);
                                    if (state != null)
                                    {
                                        strContent = state.Name;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (viewer != null)
                    {
                        viewer.SetLabelMain(strContent);
                    }
                }

                #endregion


                #region 设置LabelSub

                stateInfo =
                 extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_RECORD);
                if (stateInfo != null)
                {
                    string strContent = string.Empty;
                    if (stateInfo.State > 0)
                    {
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
                            strContent = string.Format("V:{0} S:{1}", strVoice, strScreen);
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
                            strContent = strVoice;
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
                            strContent = strScreen;
                        }
                        else
                        {
                            strContent = string.Empty;
                        }
                    }
                    if (viewer != null)
                    {
                        viewer.SetLabelSub(strContent);
                    }
                }

                #endregion


                #region 详细信息页状态更新

                if (detail != null)
                {
                    detail.SetStateChanged();
                }

                #endregion


                #region 设置状态时长

                SetStateDuration(item);

                #endregion

            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("UpdateExtItem", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public void RefreshState()
        {
            try
            {
                CreateSeatItems();
                CreateMonClient();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void OpenSeatPlayback(RegionSeatItem item)
        {
            try
            {
                UCSeatPlayback uc = new UCSeatPlayback();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.RegionSeatItem = item;

                string strName = item.SeatName;
                PopupPanel.Title = string.Format("Monitor and Playback -- {0}", strName);
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region MonClient Event

        void MonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("NotifyEvent", string.Format("{0}", ParseCommand(e.NotifyMessage.Command)));
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
                CurrentApp.WriteLog("NotifyEvent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void MonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                if (!e.ReturnMessage.Result)
                {
                    CurrentApp.WriteLog("ReturnEvent", string.Format("ErrorMessage \t{0}\t{1}", e.ReturnMessage.Code,
                        e.ReturnMessage.Message));
                }
                else
                {
                    CurrentApp.WriteLog("ReturnEvent", string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                    int command = e.ReturnMessage.Command;
                    switch (command)
                    {
                        case (int)RequestCode.NCWelcome:
                            CurrentApp.WriteLog("ReturnEvent", string.Format("Welcome message"));
                            DealWelcomeMessage(e.ReturnMessage);
                            SetMonType();
                            break;
                        case (int)Service10Command.ResSetMonType:
                            AddMonObjects();
                            break;
                        case (int)Service10Command.ResAddMonObj:
                            DealAddMonObjectMessage(e.ReturnMessage);
                            break;
                        case (int)Service10Command.ResQueryExtState:
                            DealQueryExtStateMessage(e.ReturnMessage);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("ReturnEvent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void MonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                CurrentApp.WriteLog("ConnEvent", string.Format("{0}\t{1}", e.Code, e.Message));
                if (e.Code == (Defines.EVT_NET_CONNECTED))
                {
                    //SetMonType();
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("ConnEvent", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void RecordLengthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];
                    item.TimeDeviation = mTimeDeviation;
                    UpdateExtItemState(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PopupPanel_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                var uc = PopupPanel.Content as UCSeatPlayback;
                if (uc == null) { return; }
                uc.Close();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region SeatItemClickCommand

        private void SeatItemClickCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var viewer = e.Parameter as UCSeatStatusViewer;
                if (viewer == null) { return; }
                var item = viewer.RegionSeatItem;
                if (item == null) { return; }
                OpenSeatPlayback(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Alarm Operations

        private void CreateAlarmPanel(StateDurationInfo stateDuration,StateAlarmInfo stateAlarm)
        {
            try
            {
                UCAlarmPanel uc = new UCAlarmPanel();
                uc.PageParent = this;
                uc.CurrentApp = CurrentApp;
                uc.StateDuration = stateDuration;
                uc.StateAlarmInfo = stateAlarm;
                GridAlarmPanel.Children.Add(uc);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void RemoveAlarmPanel(UCAlarmPanel panel)
        {
            try
            {
                GridAlarmPanel.Children.Remove(panel);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetStateDuration(RegionSeatItem item)
        {
            try
            {
                var extInfo = item.ExtensionInfo;
                if (extInfo == null) { return; }
                string strExt = extInfo.Extension;
                ExtStateInfo extState;
                StateDurationInfo stateDuration;
                extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_LOGIN);
                if (extState != null)
                {
                    stateDuration =
                        mListStateDurationInfos.FirstOrDefault(
                            d => d.Extension == strExt
                                 && d.StateType == S4410Consts.STATE_TYPE_LOGIN);
                    DateTime now = DateTime.Now;
                    if (stateDuration == null)
                    {
                        stateDuration = new StateDurationInfo();
                        stateDuration.Extension = strExt;
                        stateDuration.StateType = S4410Consts.STATE_TYPE_LOGIN;
                        stateDuration.StateValue = extState.State;
                        stateDuration.BeginTime = now;
                        stateDuration.SeatItem = item;
                        mListStateDurationInfos.Add(stateDuration);
                    }
                    else
                    {
                        if (extState.State != stateDuration.StateValue)
                        {
                            stateDuration.BeginTime = now;
                            stateDuration.StateValue = extState.State;
                            stateDuration.ListStateAlarms.Clear();
                        }
                    }
                    stateDuration.Duration = (now - stateDuration.BeginTime).TotalSeconds;
                    CheckAlarm(stateDuration);
                }
                extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_CALL);
                if (extState != null)
                {
                    stateDuration =
                        mListStateDurationInfos.FirstOrDefault(
                            d => d.Extension == strExt
                                 && d.StateType == S4410Consts.STATE_TYPE_CALL);
                    DateTime now = DateTime.Now;
                    if (stateDuration == null)
                    {
                        stateDuration = new StateDurationInfo();
                        stateDuration.Extension = strExt;
                        stateDuration.StateType = S4410Consts.STATE_TYPE_CALL;
                        stateDuration.StateValue = extState.State;
                        stateDuration.BeginTime = now;
                        stateDuration.SeatItem = item;
                        mListStateDurationInfos.Add(stateDuration);
                    }
                    else
                    {
                        if (extState.State != stateDuration.StateValue)
                        {
                            stateDuration.BeginTime = now;
                            stateDuration.StateValue = extState.State;
                            stateDuration.ListStateAlarms.Clear();
                        }
                    }
                    stateDuration.Duration = (now - stateDuration.BeginTime).TotalSeconds;
                    CheckAlarm(stateDuration);
                }
                extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                if (extState != null)
                {
                    stateDuration =
                         mListStateDurationInfos.FirstOrDefault(
                             d => d.Extension == strExt
                                  && d.StateType == S4410Consts.STATE_TYPE_RECORD);
                    DateTime now = DateTime.Now;
                    if (stateDuration == null)
                    {
                        stateDuration = new StateDurationInfo();
                        stateDuration.Extension = strExt;
                        stateDuration.StateType = S4410Consts.STATE_TYPE_RECORD;
                        stateDuration.StateValue = extState.State;
                        stateDuration.BeginTime = now;
                        stateDuration.SeatItem = item;
                        mListStateDurationInfos.Add(stateDuration);
                    }
                    else
                    {
                        if (extState.State != stateDuration.StateValue)
                        {
                            stateDuration.BeginTime = now;
                            stateDuration.StateValue = extState.State;
                            stateDuration.ListStateAlarms.Clear();
                        }
                    }
                    stateDuration.Duration = (now - stateDuration.BeginTime).TotalSeconds;
                    CheckAlarm(stateDuration);
                }
                extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                if (extState != null)
                {
                    stateDuration =
                        mListStateDurationInfos.FirstOrDefault(
                            d => d.Extension == strExt
                                 && d.StateType == S4410Consts.STATE_TYPE_DIRECTION);
                    DateTime now = DateTime.Now;
                    if (stateDuration == null)
                    {
                        stateDuration = new StateDurationInfo();
                        stateDuration.Extension = strExt;
                        stateDuration.StateType = S4410Consts.STATE_TYPE_DIRECTION;
                        stateDuration.StateValue = extState.State;
                        stateDuration.BeginTime = now;
                        stateDuration.SeatItem = item;
                        mListStateDurationInfos.Add(stateDuration);
                    }
                    else
                    {
                        if (extState.State != stateDuration.StateValue)
                        {
                            stateDuration.BeginTime = now;
                            stateDuration.StateValue = extState.State;
                            stateDuration.ListStateAlarms.Clear();
                        }
                    }
                    stateDuration.Duration = (now - stateDuration.BeginTime).TotalSeconds;
                    CheckAlarm(stateDuration);
                }
                extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_AGNET);
                if (extState != null)
                {
                    stateDuration =
                        mListStateDurationInfos.FirstOrDefault(
                            d => d.Extension == strExt
                                 && d.StateType == S4410Consts.STATE_TYPE_AGNET);
                    DateTime now = DateTime.Now;
                    if (stateDuration == null)
                    {
                        stateDuration = new StateDurationInfo();
                        stateDuration.Extension = strExt;
                        stateDuration.StateType = S4410Consts.STATE_TYPE_AGNET;
                        stateDuration.StateValue = extState.State;
                        stateDuration.BeginTime = now;
                        stateDuration.SeatItem = item;
                        mListStateDurationInfos.Add(stateDuration);
                    }
                    else
                    {
                        if (extState.State != stateDuration.StateValue)
                        {
                            stateDuration.BeginTime = now;
                            stateDuration.StateValue = extState.State;
                            stateDuration.ListStateAlarms.Clear();
                        }
                    }
                    stateDuration.Duration = (now - stateDuration.BeginTime).TotalSeconds;
                    CheckAlarm(stateDuration);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CheckAlarm(StateDurationInfo stateDuration)
        {
            try
            {
                RegionSeatItem seatItem = stateDuration.SeatItem;
                if (seatItem == null) { return; }
                if (ListAllStateInfos == null) { return; }
                int stateType = stateDuration.StateType;
                int stateValue = stateDuration.StateValue;
                AgentStateInfo stateInfo;
                stateInfo = ListAllStateInfos.FirstOrDefault(s => s.Type == stateType && (s.Value & stateValue) > 0);
                if (stateInfo == null) { return; }
                var alarmMsgs = mListAlarmMessageInfos.Where(a => a.Type == 1 && a.StateID == stateInfo.ObjID);
                foreach (var alarmMsg in alarmMsgs)
                {
                    long alarmID = alarmMsg.SerialID;
                    var stateAlarm = stateDuration.ListStateAlarms.FirstOrDefault(a => a.AlarmID == alarmID);
                    if (stateAlarm == null)
                    {
                        string strValue = alarmMsg.Value;
                        double seconds;
                        if (!double.TryParse(strValue, out seconds)) { return; }
                        if (seconds > 0 && seconds <= stateDuration.Duration)
                        {
                            stateAlarm=new StateAlarmInfo();
                            stateAlarm.AlarmID = alarmID;
                            stateAlarm.AlarmTime = DateTime.Now;
                            stateAlarm.AlarmMessage = alarmMsg;
                            stateDuration.ListStateAlarms.Add(stateAlarm);
                            Dispatcher.Invoke(new Action(() => CreateAlarmPanel(stateDuration, stateAlarm)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private string ParseCommand(int command)
        {
            string str = string.Empty;
            str = ((RequestCode)command).ToString();
            if (command >= 1000 && command < 10000)
            {
                str = ((Service10Command)command).ToString();
            }
            return str;
        }

        #endregion


        #region Test



        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];
                    var viewer = item.Viewer;
                    if (viewer != null)
                    {
                        viewer.ChangeLanguage();
                    }
                }
                PopupPanel.ChangeLanguage();
                for (int i = 0; i < GridAlarmPanel.Children.Count; i++)
                {
                    var panel = GridAlarmPanel.Children[i] as UMPUserControl;
                    if (panel != null)
                    {
                        panel.ChangeLanguage();
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
