//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d93860b7-47aa-44ff-8467-e81e4f9b3774
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionStateViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCRegionStateViewer
//
//        created by Charley at 2016/7/17 13:12:16
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
    /// UCRegionStateViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionStateViewer:IOnsiteView
    {

        #region Members

        public OnsiteMonitorMainView PageParent;
        public ObjItem RegionItem;
        public List<SeatInfo> ListAllSeatInfos;
        public List<AgentStateInfo> ListAllStateInfos;

        private ObservableCollection<ChildRegionItem> mListChildRegionItems;
        private List<RegionExtensionItem> mListAllRegionExtItems;
        private List<RegionStateItem> mListAllRegionStateItems;
        private List<MonitorObject> mListMonitorObjects;
        private bool mIsInited;
        private NetClient mMonClient;
        private Thread mThreadCaculate;
        private int mCaculateWait = 5000;  //统计等待时间，也就是刷新频率，默认10s

        #endregion


        public UCRegionStateViewer()
        {
            InitializeComponent();

            mListChildRegionItems = new ObservableCollection<ChildRegionItem>();
            mListAllRegionExtItems = new List<RegionExtensionItem>();
            mListAllRegionStateItems = new List<RegionStateItem>();
            mListMonitorObjects = new List<MonitorObject>();

            Loaded += UCRegionStateViewer_Loaded;
            Unloaded += UCRegionStateViewer_Unloaded;
        }

        void UCRegionStateViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void UCRegionStateViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCaculateThread();
            if (mMonClient != null)
            {
                mMonClient.Stop();
            }
            mMonClient = null;
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ListBoxRegionList.ItemsSource = mListChildRegionItems;

                CreateRegionItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadRegionSeatInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateMonClient();
                    CreateCaculateThread();
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
                mListAllRegionExtItems.Clear();
                for (int i = 0; i < mListChildRegionItems.Count; i++)
                {
                    var item = mListChildRegionItems[i];
                    LoadRegionSeatInfos(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionSeatInfos(ChildRegionItem childRegionItem)
        {
            try
            {
                childRegionItem.ListRegionExtItems.Clear();
                var regionItem = childRegionItem.RegionItem;
                if (regionItem == null) { return; }
                var regionInfo = childRegionItem.RegionInfo;
                if (regionInfo == null) { return; }
                if (regionInfo.Type == 0)
                {
                    LoadRegionExtItems(childRegionItem, regionItem);
                }
                else
                {
                    for (int i = 0; i < regionItem.Children.Count; i++)
                    {
                        var child = regionItem.Children[i] as ObjItem;
                        if (child == null) { continue; }
                        LoadRegionExtItems(childRegionItem, child);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionExtItems(ChildRegionItem childRegionItem, ObjItem regionItem)
        {
            try
            {
                long regionID = regionItem.ObjID;
                string strRegionName = regionItem.Name;
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
                    if (ListAllSeatInfos == null) { continue; }
                    var seatInfo = ListAllSeatInfos.FirstOrDefault(s => s.ObjID == info.SeatID);
                    if (seatInfo == null) { continue; }
                    RegionExtensionItem extItem = new RegionExtensionItem();
                    extItem.SeatInfo = seatInfo;
                    extItem.ChildRegionID = childRegionItem.RegionID;
                    extItem.RegionID = regionID;
                    extItem.SeatID = seatInfo.ObjID;
                    extItem.SeatName = seatInfo.Name;
                    extItem.Extension = seatInfo.Extension;
                    childRegionItem.ListRegionExtItems.Add(extItem);
                    mListAllRegionExtItems.Add(extItem);
                }

                CurrentApp.WriteLog("LoadRegionExtItems", string.Format("Load end.\t{0}\t{1}", strRegionName, childRegionItem.ListRegionExtItems.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateRegionItems()
        {
            try
            {
                mListChildRegionItems.Clear();
                mListAllRegionStateItems.Clear();
                if (RegionItem == null) { return; }
                for (int i = 0; i < RegionItem.Children.Count; i++)
                {
                    var child = RegionItem.Children[i] as ObjItem;
                    if (child == null) { continue; }
                    var regionInfo = child.Data as RegionInfo;
                    if (regionInfo == null) { continue; }
                    ChildRegionItem item = new ChildRegionItem();
                    item.RegionInfo = regionInfo;
                    item.RegionItem = child;
                    item.RegionID = regionInfo.ObjID;
                    item.Name = regionInfo.Name;
                    item.CurrentApp = CurrentApp;
                    mListChildRegionItems.Add(item);
                    CreateRegionStateItems(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRegionStateItems(ChildRegionItem regionItem)
        {
            try
            {
                regionItem.ListRegionStateItems.Clear();
                if (ListAllStateInfos == null) { return; }
                for (int i = 0; i < ListAllStateInfos.Count; i++)
                {
                    var stateInfo = ListAllStateInfos[i];

                    RegionStateItem item = new RegionStateItem();
                    item.StateInfo = stateInfo;
                    item.StateID = stateInfo.ObjID;
                    item.StateNumber = stateInfo.Number;
                    item.StateName = stateInfo.Name;
                    item.SeatNum = 0;
                    item.Color = new SolidColorBrush(GetColorFromString(stateInfo.Color));
                    regionItem.ListRegionStateItems.Add(item);
                    mListAllRegionStateItems.Add(item);
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

        #endregion


        #region Operations

        public void RefreshState()
        {
            try
            {
                CreateMonClient();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Caculate SeatNum

        private void CreateCaculateThread()
        {
            try
            {
                if (mThreadCaculate != null)
                {
                    try
                    {
                        mThreadCaculate.Abort();
                    }
                    catch { }
                    mThreadCaculate = null;
                }
                mThreadCaculate = new Thread(CaculateSeatWorker);
                mThreadCaculate.Start();

                CurrentApp.WriteLog("CreateCaculateThread", string.Format("End.\t{0}", mThreadCaculate.ManagedThreadId));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CreateCaculateThread", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void StopCaculateThread()
        {
            try
            {
                if (mThreadCaculate != null)
                {
                    mThreadCaculate.Abort();
                }
                mThreadCaculate = null;

                CurrentApp.WriteLog("StopCaculateThread", string.Format("Thread stopped."));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StopCaculateThread", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void CaculateSeatWorker()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mCaculateWait);

                    CaculateSeatNum();
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    CurrentApp.WriteLog("CaculateSeatWorker", string.Format("Fail.\t{0}", ex.Message));
                }
            }
        }

        private void CaculateSeatNum()
        {
            try
            {
                for (int i = 0; i < mListChildRegionItems.Count; i++)
                {
                    var childRegionItem = mListChildRegionItems[i];

                    for (int j = 0; j < childRegionItem.ListRegionStateItems.Count; j++)
                    {
                        var regionStateItem = childRegionItem.ListRegionStateItems[j];

                        CaculateSeatNum(childRegionItem, regionStateItem);
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CaculateSeatNum", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void CaculateSeatNum(ChildRegionItem childRegionItem, RegionStateItem regionStateItem)
        {
            try
            {
                var stateInfo = regionStateItem.StateInfo;
                if (stateInfo == null) { return; }
                int stateType = stateInfo.Type;
                int stateValue = stateInfo.Value;
                int num = 0;
                for (int i = 0; i < childRegionItem.ListRegionExtItems.Count; i++)
                {
                    var extItem = childRegionItem.ListRegionExtItems[i];

                    var extInfo = extItem.ExtInfo;
                    if (extInfo == null) { continue; }
                    ExtStateInfo extState = extInfo.ListStateInfos.FirstOrDefault(s => s.StateType == stateType);
                    if (extState == null) { continue; }
                    if (stateType == S4410Consts.STATE_TYPE_LOGIN
                        || stateType == S4410Consts.STATE_TYPE_RECORD)
                    {
                        if ((extState.State & stateValue) > 0)
                        {
                            num++;
                        }
                    }
                    else
                    {
                        if (extState.State == stateValue)
                        {
                            num++;
                        }
                    }
                }
                //regionStateItem.SeatNum = num;
                Dispatcher.Invoke(new Action(() =>
                {
                    regionStateItem.SeatNum = num;
                    var viewer = childRegionItem.Viewer;
                    if (viewer != null)
                    {
                        viewer.Refresh();
                    }
                }));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("CaculateSeatNum", string.Format("Fail.\t{0}", ex.Message));
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
                for (int i = 0; i < mListAllRegionExtItems.Count; i++)
                {
                    var item = mListAllRegionExtItems[i];
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
                //string strServerTime = retMessage.ListData[3];
                //DateTime dtServerTime;
                //if (DateTime.TryParse(strServerTime, out dtServerTime))
                //{
                //    DateTime now = DateTime.Now.ToUniversalTime();
                //    double timeDeviation = (now - dtServerTime).TotalSeconds;
                //}
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
                    var item = mListAllRegionExtItems.FirstOrDefault(e => e.Extension == monObj.Name);
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
                var item = mListAllRegionExtItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtInfo = extInfo;
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
                var item = mListAllRegionExtItems.FirstOrDefault(e => e.MonID == strMonID);
                if (item != null)
                {
                    item.ExtInfo = extInfo;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DealExtState", string.Format("Fail.\t{0}", ex.Message));
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

        public void Close()
        {
            StopCaculateThread();
            if (mMonClient != null)
            {
                mMonClient.Stop();
            }
            mMonClient = null;
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListChildRegionItems.Count; i++)
                {
                    var item = mListChildRegionItems[i];

                    var viewer = item.Viewer;
                    if (viewer != null)
                    {
                        viewer.ChangeLanguage();
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
