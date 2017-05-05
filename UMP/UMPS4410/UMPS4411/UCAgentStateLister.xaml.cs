//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    45e9feea-9571-4263-8e6f-e5159dc68289
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAgentStateLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCAgentStateLister
//
//        created by Charley at 2016/7/4 15:38:41
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using UMPS4411.Models;
using UMPS4411.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;

namespace UMPS4411
{
    /// <summary>
    /// UCAgentStateLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentStateLister : IOnsiteView
    {

        #region Members

        public OnsiteMonitorMainView PageParent;
        public ObjItem RegionItem;
        public List<AgentStateInfo> ListAllStateInfos;
        public List<SeatInfo> ListAllSeatInfos;

        private bool mIsInited;
        private ObservableCollection<AgentStateItem> mListAgentStateItems;
        private List<RegionSeatInfo> mListRegionSeats;
        private List<MonitorObject> mListMonitorObjects;
        private List<StateSeatItem> mListStateSeatItems;
        private NetClient mMonClient;
        private double mTimeDeviation;      //与监视服务器的时间差
        private Timer mRecordLengthTimer;

        #endregion


        public UCAgentStateLister()
        {
            InitializeComponent();

            mListRegionSeats = new List<RegionSeatInfo>();
            mListMonitorObjects = new List<MonitorObject>();
            mListStateSeatItems = new List<StateSeatItem>();
            mListAgentStateItems = new ObservableCollection<AgentStateItem>();

            mTimeDeviation = 0;
            mRecordLengthTimer = new Timer(1000);

            Loaded += UCAgentStateLister_Loaded;
            Unloaded += UCAgentStateLister_Unloaded;
        }

        void UCAgentStateLister_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void UCAgentStateLister_Unloaded(object sender, RoutedEventArgs e)
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
                ListBoxStateItems.ItemsSource = mListAgentStateItems;
                mRecordLengthTimer.Elapsed += RecordLengthTimer_Elapsed;
                CreateStateItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadRegionSeatInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitStateSeatItems();
                    CreateMonClient();
                    mRecordLengthTimer.Start();
                };
                worker.RunWorkerAsync();
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

        private void InitStateSeatItems()
        {
            try
            {
                mListStateSeatItems.Clear();
                if (ListAllSeatInfos == null) { return; }
                if (ListAllStateInfos == null) { return; }

                for (int i = 0; i < mListRegionSeats.Count; i++)
                {
                    var regionSeat = mListRegionSeats[i];

                    SeatInfo seatInfo = ListAllSeatInfos.FirstOrDefault(s => s.ObjID == regionSeat.SeatID);
                    if (seatInfo == null) { continue; }

                    for (int j = 0; j < ListAllStateInfos.Count; j++)
                    {
                        AgentStateInfo stateInfo = ListAllStateInfos[j];
                        int stateNumber = stateInfo.Number;
                        AgentStateItem stateItem = mListAgentStateItems.FirstOrDefault(s => s.Number == stateNumber);

                        StateSeatItem item = new StateSeatItem();
                        item.Info = regionSeat;
                        item.SeatInfo = seatInfo;
                        item.StateItem = stateItem;
                        item.ObjID = regionSeat.SeatID;
                        item.SeatName = seatInfo.Name;
                        item.Extension = seatInfo.Extension;
                        item.Number = stateNumber;
                        item.Description = string.Empty;
                        mListStateSeatItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateStateItems()
        {
            try
            {
                mListAgentStateItems.Clear();
                if (ListAllStateInfos == null) { return; }
                for (int i = 0; i < ListAllStateInfos.Count; i++)
                {
                    var info = ListAllStateInfos[i];

                    AgentStateItem item = new AgentStateItem();
                    item.Info = info;
                    item.ObjID = info.ObjID;
                    item.Number = info.Number;
                    item.Name = info.Name;
                    item.StateType = info.Type;
                    item.StateValue = info.Value;
                    Brush brushHead = Brushes.LightGray;
                    try
                    {
                        brushHead = new SolidColorBrush(GetColorFromString(info.Color));
                    }
                    catch { }
                    item.BrushHead = brushHead;
                    mListAgentStateItems.Add(item);
                }
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
                for (int i = 0; i < mListStateSeatItems.Count; i++)
                {
                    var stateSeat = mListStateSeatItems[i];
                    string strExt = stateSeat.Extension;
                    if (!string.IsNullOrEmpty(strExt)
                        && !listExts.Contains(strExt))
                    {
                        listExts.Add(strExt);
                    }
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
                CurrentApp.WriteLog("SetMonType", string.Format("Fail.\t{0}", ex.Message));
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
                    var items = mListStateSeatItems.Where(e => e.Extension == monObj.Name);
                    foreach (var item in items)
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
                //var item = mListStateSeatItems.FirstOrDefault(e => e.MonID == strMonID);
                //if (item != null)
                //{
                //    item.ExtensionInfo = extInfo;
                //    item.TimeDeviation = mTimeDeviation;
                //    Dispatcher.Invoke(new Action(() => UpdateStateSeatItem(item)));
                //}
                var items = mListStateSeatItems.Where(e => e.MonID == strMonID);
                foreach (var item in items)
                {
                    item.ExtensionInfo = extInfo;
                    item.TimeDeviation = mTimeDeviation;
                    StateSeatItem item1 = item;
                    Dispatcher.Invoke(new Action(() => UpdateStateSeatItem(item1)));
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
                //var item = mListStateSeatItems.FirstOrDefault(e => e.MonID == strMonID);
                //if (item != null)
                //{
                //    item.ExtensionInfo = extInfo;
                //    Dispatcher.Invoke(new Action(() => UpdateStateSeatItem(item)));
                //}
                var items = mListStateSeatItems.Where(e => e.MonID == strMonID);
                foreach (var item in items)
                {
                    item.ExtensionInfo = extInfo;
                    item.TimeDeviation = mTimeDeviation;
                    StateSeatItem item1 = item;
                    Dispatcher.Invoke(new Action(() => UpdateStateSeatItem(item1)));
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealExtState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void UpdateStateSeatItem(StateSeatItem item)
        {
            try
            {
                ExtStateInfo extStateInfo;
                AgentStateItem agentStateItem;
                ExtensionInfo extInfo;

                if (ListAllStateInfos == null) { return; }
                extInfo = item.ExtensionInfo;
                if (extInfo == null) { return; }
                string strExt = extInfo.Extension;

                agentStateItem = item.StateItem;
                if (agentStateItem == null) { return; }
                int stateNumber = agentStateItem.Number;

                var stateInfo = agentStateItem.Info;
                if (stateInfo == null) { return; }

                int stateType = stateInfo.Type;
                int stateValue = stateInfo.Value;

                switch (stateType)
                {
                    case S4410Consts.STATE_TYPE_LOGIN:
                        extStateInfo = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                        if (extStateInfo != null)
                        {
                            if ((stateValue & extStateInfo.State) > 0)
                            {
                                var temp =
                                    agentStateItem.ListSeatItems.FirstOrDefault(
                                        s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp == null)
                                {
                                    agentStateItem.ListSeatItems.Add(item);
                                }
                            }
                            else
                            {
                                var temp =
                                   agentStateItem.ListSeatItems.FirstOrDefault(
                                       s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp != null)
                                {
                                    agentStateItem.ListSeatItems.Remove(temp);
                                }
                            }
                            item.Description = extInfo.AgentID;
                        }
                        break;
                    case S4410Consts.STATE_TYPE_CALL:
                        extStateInfo = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                        if (extStateInfo != null)
                        {
                            if (stateValue == extStateInfo.State)
                            {
                                var temp =
                                    agentStateItem.ListSeatItems.FirstOrDefault(
                                        s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp == null)
                                {
                                    agentStateItem.ListSeatItems.Add(item);
                                }
                            }
                            else
                            {
                                var temp =
                                   agentStateItem.ListSeatItems.FirstOrDefault(
                                       s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp != null)
                                {
                                    agentStateItem.ListSeatItems.Remove(temp);
                                }
                            }
                            item.Description = string.Format("{0}-->{1}", extInfo.CallerID, extInfo.CalledID);
                        }
                        break;
                    case S4410Consts.STATE_TYPE_RECORD:
                        extStateInfo = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                        if (extStateInfo != null)
                        {
                            if ((stateValue & extStateInfo.State) > 0)
                            {
                                var temp =
                                    agentStateItem.ListSeatItems.FirstOrDefault(
                                        s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp == null)
                                {
                                    agentStateItem.ListSeatItems.Add(item);
                                }
                            }
                            else
                            {
                                var temp =
                                   agentStateItem.ListSeatItems.FirstOrDefault(
                                       s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp != null)
                                {
                                    agentStateItem.ListSeatItems.Remove(temp);
                                }
                            }
                            item.Description = GetRecordLength(item);
                        }
                        break;
                    case S4410Consts.STATE_TYPE_DIRECTION:
                        extStateInfo = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                        if (extStateInfo != null)
                        {
                            if (stateValue == extStateInfo.State)
                            {
                                var temp =
                                    agentStateItem.ListSeatItems.FirstOrDefault(
                                        s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp == null)
                                {
                                    agentStateItem.ListSeatItems.Add(item);
                                }
                            }
                            else
                            {
                                var temp =
                                   agentStateItem.ListSeatItems.FirstOrDefault(
                                       s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp != null)
                                {
                                    agentStateItem.ListSeatItems.Remove(temp);
                                }
                            }
                            item.Description = GetRecordLength(item);
                        }
                        break;
                    case S4410Consts.STATE_TYPE_AGNET:
                        extStateInfo = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                        if (extStateInfo != null)
                        {
                            if (stateValue == extStateInfo.State)
                            {
                                var temp =
                                    agentStateItem.ListSeatItems.FirstOrDefault(
                                        s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp == null)
                                {
                                    agentStateItem.ListSeatItems.Add(item);
                                }
                            }
                            else
                            {
                                var temp =
                                   agentStateItem.ListSeatItems.FirstOrDefault(
                                       s => s.Extension == strExt && s.Number == stateNumber);
                                if (temp != null)
                                {
                                    agentStateItem.ListSeatItems.Remove(temp);
                                }
                            }
                            item.Description = string.Empty;
                        }
                        break;
                }

                agentStateItem.SeatCount = agentStateItem.ListSeatItems.Count;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void RefreshState()
        {
            try
            {
                CreateStateItems();
                InitStateSeatItems();
                CreateMonClient();
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
                for (int i = 0; i < mListStateSeatItems.Count; i++)
                {
                    var item = mListStateSeatItems[i];
                    item.TimeDeviation = mTimeDeviation;
                    UpdateStateSeatItem(item);
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

        private Color GetColorFromString(string strColor)
        {
            Color color = Brushes.Transparent.Color;
            try
            {
                string strA = strColor.Substring(1, 2);
                string strR = strColor.Substring(3, 2);
                string strG = strColor.Substring(5, 2);
                string strB = strColor.Substring(7, 2);
                color = Color.FromArgb((byte)Convert.ToInt32(strA, 16), (byte)Convert.ToInt32(strR, 16), (byte)Convert.ToInt32(strG, 16),
                    (byte)Convert.ToInt32(strB, 16));
            }
            catch { }
            return color;
        }

        private string GetRecordLength(StateSeatItem item)
        {
            string strReturn = string.Empty;
            try
            {
                var extInfo = item.ExtensionInfo;
                if (extInfo == null)
                {
                    return strReturn;
                }
                var extStateInfo =
                    extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == S4410Consts.STATE_TYPE_RECORD);
                if (extStateInfo != null)
                {
                    if (extStateInfo.State > 0)
                    {
                        if (extStateInfo.State == (int)RecordState.All)
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
                            strReturn = string.Format("V:{0} S:{1}", strVoice, strScreen);
                        }
                        else if (extStateInfo.State == (int)RecordState.Voice)
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
                            strReturn = strVoice;
                        }
                        else if (extStateInfo.State == (int)RecordState.Screen)
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
                            strReturn = strScreen;
                        }
                    }
                }
            }
            catch { }
            return strReturn;
        }

        #endregion

    }
}
