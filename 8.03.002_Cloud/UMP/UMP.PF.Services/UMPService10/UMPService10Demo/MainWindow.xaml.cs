using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.SDKs.NMon;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;

namespace UMPService10Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        private ObservableCollection<ExtStateItem> mListExtStateItems;
        private List<MonitorObject> mListMonitorObjects;

        private NetClient mMonClient;
        private NetClient mNMonClient;
        private ExtStateItem mNMonItem;
        private NMonCore mNMonCore;


        public MainWindow()
        {
            InitializeComponent();

            mListExtStateItems = new ObservableCollection<ExtStateItem>();
            mListMonitorObjects = new List<MonitorObject>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnStartNMon.Click += BtnStartNMon_Click;
            BtnStopNMon.Click += BtnStopNMon_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ListViewExtState.ItemsSource = mListExtStateItems;
                InitExtStateItems();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mNMonCore != null)
            {
                mNMonCore.StopMon();
                mNMonCore = null;
            }
            if (mMonClient != null)
            {
                mMonClient.Stop();
                mMonClient = null;
            }
            if (mNMonClient != null)
            {
                mNMonClient.Stop();
                mNMonClient = null;
            }
        }


        #region Init and Load

        private void InitExtStateItems()
        {
            try
            {
                mListExtStateItems.Clear();

                ExtStateItem item = new ExtStateItem();
                item.Extension = "1130";
                mListExtStateItems.Add(item);
                item = new ExtStateItem();
                item.Extension = "3002";
                mListExtStateItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mMonClient != null)
                {
                    mMonClient.Stop();
                    mMonClient = null;
                }
                mMonClient = new NetClient();
                mMonClient.Debug += (mode, cat, msg) => AppendMessage(string.Format("[StateMonClient]{0}\t{1}", cat, msg));
                mMonClient.ConnectionEvent += MonClient_ConnectionEvent;
                mMonClient.ReturnMessageReceived += MonClient_ReturnMessageReceived;
                mMonClient.NotifyMessageReceived += MonClient_NotifyMessageReceived;
                mMonClient.IsSSL = true;
                mMonClient.Host = "192.168.6.63";
                mMonClient.Port = 8081 - 10;
                mMonClient.Connect();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnStopNMon_Click(object sender, RoutedEventArgs e)
        {
            StopNMon();
            if (mNMonCore != null)
            {
                mNMonCore.StopMon();
                mNMonCore = null;
            }
        }

        void BtnStartNMon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = ListViewExtState.SelectedItem as ExtStateItem;
                if (item == null) { return; }
                mNMonItem = item;

                if (mNMonClient != null)
                {
                    mNMonClient.Stop();
                    mNMonClient = null;
                }
                mNMonClient = new NetClient();
                mNMonClient.Debug += (mode, cat, msg) => AppendMessage(string.Format("[NMonClient]{0}\t{1}", cat, msg));
                mNMonClient.ConnectionEvent += NMonClient_ConnectionEvent;
                mNMonClient.ReturnMessageReceived += NMonClient_ReturnMessageReceived;
                mNMonClient.NotifyMessageReceived += NMonClient_NotifyMessageReceived;
                mNMonClient.IsSSL = true;
                mNMonClient.Host = "192.168.6.63";
                mNMonClient.Port = 8081 - 10;
                mNMonClient.Connect();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #endregion


        #region NetClient Handlers

        void MonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            AppendMessage(string.Format("NotifyMessage\t{0}", ParseCommand(e.NotifyMessage.Command)));
            int command = e.NotifyMessage.Command;
            switch (command)
            {
                case (int)Service10Command.NotExtStateChanged:
                    DealExtStateChangedMessage(e.NotifyMessage);
                    break;
            }
        }

        void MonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            if (!e.ReturnMessage.Result)
            {
                AppendMessage(string.Format("ErrorMessage \t{0}\t{1}", e.ReturnMessage.Code,
                    e.ReturnMessage.Message));
            }
            else
            {
                AppendMessage(string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                int command = e.ReturnMessage.Command;
                switch (command)
                {
                    case (int)RequestCode.NCWelcome:
                        AppendMessage(string.Format("Welcome message"));
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

        void MonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            AppendMessage(string.Format("ConnectionEvent\t{0}\t{1}", e.Code, e.Message));
            if (e.Code == (Defines.EVT_NET_CONNECTED))
            {
                //SetMonType();
            }
        }

        void NMonClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            AppendMessage(string.Format("ConnectionEvent\t{0}\t{1}", e.Code, e.Message));
            if (e.Code == (Defines.EVT_NET_CONNECTED))
            {
                //SetNMonType();
            }
        }

        void NMonClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            if (!e.ReturnMessage.Result)
            {
                AppendMessage(string.Format("ErrorMessage \t{0}\t{1}", e.ReturnMessage.Code,
                    e.ReturnMessage.Message));
            }
            else
            {
                AppendMessage(string.Format("{0}", ParseCommand(e.ReturnMessage.Command)));
                int command = e.ReturnMessage.Command;
                switch (command)
                {
                    case (int)RequestCode.NCWelcome:
                        AppendMessage(string.Format("Welcome message"));
                        ThreadPool.QueueUserWorkItem(a => SetNMonType());
                        break;
                    case (int)Service10Command.ResSetMonType:
                        ThreadPool.QueueUserWorkItem(a => AddNMonObjects());
                        break;
                    case (int)Service10Command.ResAddMonObj:
                        ThreadPool.QueueUserWorkItem(a => DealNMonAddMonObjectMessage(e.ReturnMessage));
                        break;
                    case (int)Service10Command.ResStartNMon:
                        ThreadPool.QueueUserWorkItem(a => DealNMonStartNMonMessage(e.ReturnMessage));
                        break;
                    case (int)Service10Command.ResStopNMon:
                        AppendMessage(string.Format("ResStopNMon message"));
                        break;
                }
            }
        }

        void NMonClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            AppendMessage(string.Format("NotifyMessage\t{0}", ParseCommand(e.NotifyMessage.Command)));
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

        #endregion


        #region StateMon Operations

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
                AppendMessage(ex.Message);
            }
        }

        private void AddMonObjects()
        {
            try
            {
                if (mMonClient == null || !mMonClient.IsConnected) { return; }
                RequestMessage request = new RequestMessage();
                request.SessionID = mMonClient.SessionID;
                request.Command = (int)Service10Command.ReqAddMonObj;
                request.ListData.Add("0");
                int count = mListExtStateItems.Count;
                request.ListData.Add(count.ToString());
                for (int i = 0; i < mListExtStateItems.Count; i++)
                {
                    var item = mListExtStateItems[i];
                    request.ListData.Add(item.Extension);
                }
                mMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
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
                AppendMessage(ex.Message);
            }
        }

        private void DealAddMonObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    AppendMessage(string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                AppendMessage(string.Format("AddMonObjResponse \tValid:{0};Total:{1}", strValid, strTotal));
                int valid;
                if (!int.TryParse(strValid, out valid)
                    || valid < 0)
                {
                    AppendMessage(string.Format("Valid count invalid.\t{0}", strValid));
                    return;
                }
                if (retMessage.ListData.Count < 2 + valid)
                {
                    AppendMessage(string.Format("MonObject count invalid."));
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
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject monObj = optReturn.Data as MonitorObject;
                    if (monObj == null)
                    {
                        AppendMessage(string.Format("Fail.\tMonObject is null"));
                        return;
                    }
                    var temp = mListMonitorObjects.FirstOrDefault(m => m.MonID == monObj.MonID);
                    if (temp == null)
                    {
                        mListMonitorObjects.Add(monObj);
                    }
                    var item = mListExtStateItems.FirstOrDefault(e => e.Extension == monObj.Name);
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
                AppendMessage(ex.Message);
            }
        }

        private void DealQueryExtStateMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    AppendMessage(string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strExtInfo = retMessage.ListData[1];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<ExtensionInfo>(strExtInfo);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ExtensionInfo extInfo = optReturn.Data as ExtensionInfo;
                if (extInfo == null)
                {
                    AppendMessage(string.Format("Fail.\tExtInfo is null"));
                    return;
                }
                var item = mListExtStateItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtensionInfo = extInfo;
                    UpdateExtItemState(item);
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void DealExtStateChangedMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData.Count < 2)
                {
                    AppendMessage(string.Format("NotifyMessage param count invalid"));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strExtInfo = notMessage.ListData[1];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<ExtensionInfo>(strExtInfo);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ExtensionInfo extInfo = optReturn.Data as ExtensionInfo;
                if (extInfo == null)
                {
                    AppendMessage(string.Format("ExtInfo is null"));
                    return;
                }
                var item = mListExtStateItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtensionInfo = extInfo;
                    UpdateExtItemState(item);
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void UpdateExtItemState(ExtStateItem item)
        {
            try
            {
                ExtensionInfo extInfo = item.ExtensionInfo;
                if (extInfo == null) { return; }
                var stateInfo =
                    extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_LOGIN);
                if (stateInfo != null)
                {
                    item.StrLoginState = ((LoginState)stateInfo.State).ToString();

                    item.AgentID = extInfo.AgentID;
                }
                stateInfo =
                    extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_CALL);
                if (stateInfo != null)
                {
                    item.StrCallState = ((CallState)stateInfo.State).ToString();
                }
                stateInfo =
                  extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_RECORD);
                if (stateInfo != null)
                {
                    item.StrRecordState = ((RecordState)stateInfo.State).ToString();

                    if (stateInfo.State == (int)RecordState.All)
                    {
                        item.RecordReference = string.Format("V:{0} S:{1}", extInfo.VocRecordReference,
                            extInfo.ScrRecordReference);
                        item.StartRecordTime = string.Format("V:{0} S:{1}", extInfo.VocStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                            extInfo.ScrStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                        item.StopRecordTime = string.Empty;
                        item.RecordLength = string.Format("V:{0} S:{1}", extInfo.VocRecordLength,
                            extInfo.ScrRecordLength);
                    }
                    else if (stateInfo.State == (int)RecordState.Voice)
                    {
                        item.RecordReference = extInfo.VocRecordReference;
                        item.StartRecordTime = extInfo.VocStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        item.StopRecordTime = string.Empty;
                        item.RecordLength = extInfo.VocRecordLength.ToString();
                    }
                    else if (stateInfo.State == (int)RecordState.Screen)
                    {
                        item.RecordReference = extInfo.ScrRecordReference;
                        item.StartRecordTime = extInfo.ScrStartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        item.StopRecordTime = string.Empty;
                        item.RecordLength = extInfo.ScrRecordLength.ToString();
                    }
                    else
                    {
                        item.RecordReference = string.Empty;
                        item.StartRecordTime = string.Empty;
                        if (extInfo.VocStopRecordTime > DateTime.Parse("2014/1/1")
                            && extInfo.ScrStopRecordTime > DateTime.Parse("2014/1/1"))
                        {
                            item.StopRecordTime = string.Format("V:{0} S:{1}",
                                extInfo.VocStopRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                extInfo.ScrStopRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else if (extInfo.VocStopRecordTime > DateTime.Parse("2014/1/1"))
                        {
                            item.StopRecordTime = extInfo.VocStopRecordTime.ToLocalTime()
                                .ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else if (extInfo.ScrStopRecordTime > DateTime.Parse("2014/1/1"))
                        {
                            item.StopRecordTime = extInfo.ScrStopRecordTime.ToLocalTime()
                                .ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            item.StopRecordTime = string.Empty;
                        }

                        item.RecordLength = string.Empty;
                    }
                }
                stateInfo =
                    extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                if (stateInfo != null)
                {
                    item.StrDirectionState = ((DirectionState)stateInfo.State).ToString();

                    item.CallerID = extInfo.CallerID;
                    item.CalledID = extInfo.CalledID;
                }
                stateInfo =
                   extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == Service10Consts.STATE_TYPE_AGENT);
                if (stateInfo != null)
                {
                    item.StrAgentState = ((AgentState)stateInfo.State).ToString();
                }

            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        #endregion


        #region NMon Operations

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
                AppendMessage(ex.Message);
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
                int count = 1;
                request.ListData.Add(count.ToString());
                request.ListData.Add(mNMonItem.Extension);
                mNMonClient.SendMessage(request);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void StartNMon()
        {
            try
            {
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
                AppendMessage(ex.Message);
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
                AppendMessage(ex.Message);
            }
        }

        private void DealNMonAddMonObjectMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 2)
                {
                    AppendMessage(string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strValid = retMessage.ListData[0];
                string strTotal = retMessage.ListData[1];
                AppendMessage(string.Format("AddMonObjResponse \tValid:{0};Total:{1}", strValid, strTotal));
                if (retMessage.ListData.Count < 3)
                {
                    AppendMessage(string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strInfo = retMessage.ListData[2];
                OperationReturn optReturn;
                optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                MonitorObject monObj = optReturn.Data as MonitorObject;
                if (monObj == null)
                {
                    AppendMessage(string.Format("Fail.\tMonObject is null"));
                    return;
                }
                if (mNMonItem == null) { return; }
                mNMonItem.MonID = monObj.MonID;
                mNMonItem.MonObject = monObj;
                StartNMon();
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void DealNMonStartNMonMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData.Count < 5)
                {
                    AppendMessage(string.Format("ReturnMessage param count invalid."));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strMonPort = retMessage.ListData[1];
                string strTransAudioData = retMessage.ListData[2];
                string strAddress = retMessage.ListData[3];
                string strChanID = retMessage.ListData[4];
                AppendMessage(string.Format("MonID:{0};MonPort:{1};TransAudioData:{2};Address:{3};ChanID:{4}", strMonID,
                    strMonPort, strTransAudioData, strAddress, strChanID));
                //strTransAudioData = "0";
                if (strTransAudioData == "0")
                {
                    int intMonPort;
                    int intChan;
                    if (!int.TryParse(strMonPort, out intMonPort)
                        || !int.TryParse(strChanID, out intChan))
                    {
                        AppendMessage(string.Format("MonPort or ChanID invalid."));
                        return;
                    }
                    if (mNMonCore != null)
                    {
                        mNMonCore.StopMon();
                        mNMonCore = null;
                    }
                    mNMonCore = new NMonCore(mNMonItem.MonObject);
                    mNMonCore.Debug += (s, msg) => AppendMessage(string.Format("[NMonCore]\t{0}", msg));
                    mNMonCore.IsConnectServer = true;
                    mNMonCore.IsDecodeData = true;
                    mNMonCore.IsPlayWave = true;
                    NETMON_PARAM param = new NETMON_PARAM();
                    param.Host = strAddress;
                    param.Port = intMonPort;
                    param.Channel = intChan;
                    AppendMessage(string.Format("Monitor param \tHost:{0};Port:{1};Chan:{2}", param.Host, param.Port,
                        param.Channel));
                    mNMonCore.StartMon(param);
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void DealNMonHeadMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData == null || notMessage.ListData.Count < 2)
                {
                    AppendMessage(string.Format("NotifyMessage param count invalid."));
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
                mNMonCore.Debug += (s, msg) => AppendMessage(string.Format("NMonCore:{0}", msg));
                mNMonCore.ReceiveHead(head);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void DealNMonDataMessage(NotifyMessage notMessage)
        {
            try
            {
                if (notMessage.ListData == null || notMessage.ListData.Count < 3)
                {
                    AppendMessage(string.Format("NotifyMessage param count invalid."));
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
                    AppendMessage(string.Format("DataLength invalid."));
                    return;
                }
                byte[] tempData = new byte[intLength];
                byte[] data = Converter.Hex2Byte(strData);
                Array.Copy(data, 0, tempData, 0, intLength);
                if (mNMonCore == null
                    || mNMonCore.User == null)
                {
                    AppendMessage(string.Format("NMonCore or its user is null."));
                    return;
                }
                var temp = mNMonCore.User as MonitorObject;
                if (temp == null || temp.MonID != monObj.MonID)
                {
                    AppendMessage(string.Format("MonObject not exist."));
                    return;
                }
                mNMonCore.ReceiveData(tempData, intLength);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
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
            MessageBox.Show(msg, "Service10Demo", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string msg)
        {
            MessageBox.Show(msg, "Service10Demo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


    }
}
