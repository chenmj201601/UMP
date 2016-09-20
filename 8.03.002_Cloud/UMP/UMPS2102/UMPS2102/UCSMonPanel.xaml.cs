//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22acbe6c-907e-4d92-8ec7-85dd13d7f83e
//        CLR Version:              4.0.30319.18063
//        Name:                     UCSMonPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                UCSMonPanel
//
//        created by Charley at 2015/8/1 13:45:24
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using UMPS2102.Models;
using VoiceCyber.Common;
using VoiceCyber.SDKs.ScreenMP;
using VoiceCyber.SDKs.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21021;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;
using Timer = System.Timers.Timer;

namespace UMPS2102
{
    /// <summary>
    /// UCSMonPanel.xaml 的交互逻辑
    /// </summary>
    public partial class UCSMonPanel
    {

        #region Members

        public ChanMonitorMainView PageParent;
        public MonitorData MonitorData;
        public ObservableCollection<OperationInfo> ListOperations;
        public List<UserParamInfo> ListUserParams;

        private NetClient mMonitorClient;
        private MonitorItem mMonitorItem;
        private MonitorObject mMonitorObject;
        private Timer mRecordLengthTimer;
        private double mTimeDeviation;  //与Service04服务器的时间偏差
        private bool mIsScrWinOpened;   //定义一个标识，表示是否成功打开了录屏播放窗口，只有成功打开了播放窗口，在关闭面板的时候才调用关闭录屏的接口

        #endregion


        public UCSMonPanel()
        {
            InitializeComponent();

            Loaded += UCSMonPanel_Loaded;
            Unloaded += UCSMonPanel_Unloaded;

            mIsScrWinOpened = false;
            mRecordLengthTimer = new Timer(1000);
            mTimeDeviation = 0;
            mRecordLengthTimer.Elapsed += RecordLengthTimer_Elapsed;
        }

        void UCSMonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Init();
                CreateOperationButtons();
                mRecordLengthTimer.Start();
                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void UCSMonPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mMonitorClient != null)
            {
                mMonitorClient.Stop();
                mMonitorClient = null;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                string strTitle = string.Format("{0} —— ", CurrentApp.GetLanguageInfo("2102016", "Screen Monitor"));
                if (MonitorData != null)
                {
                    mMonitorItem = MonitorItem.CreateItem(MonitorData);
                    mMonitorItem.CurrentApp = CurrentApp;
                    mMonitorItem.ListUserParams = ListUserParams;
                    mMonitorObject = new MonitorObject();
                    mMonitorObject.MonType = MonitorType.MonScr;
                    mMonitorObject.ObjID = MonitorData.ObjID;
                    mMonitorObject.ObjType = MonitorData.ObjType;
                    mMonitorObject.ObjValue = MonitorData.Name;
                    mMonitorObject.Role = 2;
                    string strOther03 = MonitorData.Other03;
                    if (!string.IsNullOrEmpty(strOther03))
                    {
                        string[] arrOther03 = strOther03.Split(new[] { ';' }, StringSplitOptions.None);
                        if (arrOther03.Length > 1)
                        {
                            mMonitorObject.Other03 = arrOther03[1];
                        }
                        else if (arrOther03.Length > 0)
                        {
                            mMonitorObject.Other03 = arrOther03[0];
                        }
                    }
                    mMonitorItem.ScreenChanMonObject = mMonitorObject;
                    Dispatcher.Invoke(new Action(mMonitorItem.UpdateState));
                    DataContext = mMonitorItem;
                    strTitle += mMonitorItem.Name;

                    ImageIcon.SetResourceReference(StyleProperty,
                        string.Format("NMonImageIcon{0}Style", mMonitorItem.ObjType));

                    InitMonitorClient();
                }
                TxtObjListTitle.Text = strTitle;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitMonitorClient()
        {
            try
            {
                if (mMonitorClient != null)
                {
                    mMonitorClient.Stop();
                    mMonitorClient = null;
                }

                string strAddress = CurrentApp.Session.AppServerInfo.Address;
                int intPort = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 5
                    : CurrentApp.Session.AppServerInfo.Port - 4;

                //string strAddress = "192.168.5.31";
                //int intPort = 8081 - 4;

                mMonitorClient = new NetClient();
                mMonitorClient.Debug += (mode, cat, msg) => CurrentApp.WriteLog("MonitorClient", string.Format("{0}\t{1}", cat, msg));
                mMonitorClient.ReturnMessageReceived += NMonMonitorClient_ReturnMessageReceived;
                mMonitorClient.NotifyMessageReceived += NMonMonitorClient_NotifyMessageReceived;
                mMonitorClient.IsSSL = true;
                mMonitorClient.Connect(strAddress, intPort);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations


        #region 网络监听消息处理

        private void SetMonType()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqSetMonType;
                request.SessionID = mMonitorClient.SessionID;
                request.ListData.Add(((int)MonitorType.MonScr).ToString());
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                CurrentApp.WriteLog("MonitorClient", string.Format("Send SetMonType message end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddQueryChanMonObject()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                if (mMonitorObject == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqAddQueryChan;
                request.ListData.Add("1");
                optReturn = XMLHelper.SeriallizeObject(mMonitorObject);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                request.ListData.Add(optReturn.Data.ToString());
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void QueryChanInfo(MonitorObject obj)
        {
            try
            {
                OperationReturn optReturn;
                if (obj != null)
                {
                    string strMonID = obj.MonID;
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service04Command.ReqQueryChan;
                    request.ListData.Add("1");
                    request.ListData.Add(strMonID);
                    CurrentApp.WriteLog("QueryChan", string.Format("MonObj:{0}", obj.ObjValue));
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("QueryChan",
                           string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RemoveMonObject()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                if (mMonitorObject == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqRemoveMonObj;
                request.ListData.Add("1");
                request.ListData.Add(mMonitorObject.MonID);
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void QueryChanState()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                if (mMonitorObject == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqQueryState;
                request.ListData.Add(mMonitorObject.MonID);
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void StartSMonObject()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                if (mMonitorObject == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqStartSMon;
                request.ListData.Add(mMonitorObject.MonID);
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {
                    #region 写操作日志

                    CurrentApp.WriteOperationLog(S2102Consts.OPT_SCRMON.ToString(), ConstValue.OPT_RESULT_EXCEPTION,
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #endregion

                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_SCRMON.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                    string.Format("{0}[{1}]", mMonitorObject.ObjValue, Utils.FormatOptLogString(string.Format("OBJ{0}", mMonitorObject.ObjType))));

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void StopSMonObject(bool isClose)
        {
            try
            {
                string strOpt = S2102Consts.OPT_STOPSCRMON.ToString();
                if (isClose)
                {
                    strOpt = S2102Consts.OPT_CLOSESCRMON.ToString();
                }
                if (mMonitorClient == null) { return; }
                if (mMonitorObject == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqStopSMon;
                request.ListData.Add(mMonitorObject.MonID);
                optReturn = mMonitorClient.SendMessage(request);
                if (!optReturn.Result)
                {

                    #region 写操作日志

                    CurrentApp.WriteOperationLog(strOpt, ConstValue.OPT_RESULT_EXCEPTION,
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #endregion

                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }

                #region 写操作日志

                CurrentApp.WriteOperationLog(strOpt, ConstValue.OPT_RESULT_SUCCESS,
                    string.Format("{0}[{1}]", mMonitorObject.ObjValue, Utils.FormatOptLogString(string.Format("OBJ{0}", mMonitorObject.ObjType))));

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealWelcomeMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData == null || retMessage.ListData.Count < 4)
                {
                    CurrentApp.WriteLog("WelcomeMessage", string.Format("ListData is null or count invalid"));
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
                ShowException(ex.Message);
            }
        }

        private void DealAddQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    CurrentApp.WriteLog("DealAddQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    CurrentApp.WriteLog("DealAddQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count > 1)
                {
                    var strInfo = retMessage.ListData[1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DealAddQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        CurrentApp.WriteLog("DealAddQueryChan", string.Format("MonitorObject is null"));
                        return;
                    }
                    if (mMonitorObject.ObjID == obj.ObjID && mMonitorObject.Role == obj.Role)
                    {
                        mMonitorObject.MonID = obj.MonID;
                        mMonitorObject.UpdateInfo(obj);
                        if (mMonitorItem != null)
                        {
                            Dispatcher.Invoke(new Action(mMonitorItem.UpdateState));
                        }

                        //查询通道状态
                        QueryChanState();

                        //开始监屏
                        StartSMonObject();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count < intCount + 1)
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData count invalid"));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    var strInfo = retMessage.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DealQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        CurrentApp.WriteLog("DealQueryChan", string.Format("MonitorObject is null"));
                        return;
                    }
                    if (mMonitorObject.ObjID == obj.ObjID && mMonitorObject.Role == obj.Role)
                    {
                        mMonitorObject.UpdateInfo(obj);
                        if (mMonitorItem != null)
                        {
                            Dispatcher.Invoke(new Action(mMonitorItem.UpdateState));
                        }

                        //查询通道状态
                        QueryChanState();

                        //开始监屏
                        StartSMonObject();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealQueryStateResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealQueryState", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strState = retMessage.ListData[1];
                if (strMonID == mMonitorObject.MonID)
                {
                    MonitorObject obj = mMonitorObject;
                    optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DealQueryState", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ChanState chanState = optReturn.Data as ChanState;
                    if (chanState == null)
                    {
                        CurrentApp.WriteLog("DealQueryState", string.Format("ChannelState object is null"));
                        return;
                    }
                    if (mMonitorItem != null)
                    {
                        if (obj.Role == 1)
                        {
                            mMonitorItem.VoiceChanState = chanState;
                        }
                        if (obj.Role == 2)
                        {
                            mMonitorItem.ScreenChanState = chanState;
                        }
                        mMonitorItem.TimeDeviation = mTimeDeviation;
                        Dispatcher.Invoke(new Action(mMonitorItem.UpdateState));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealChanStateChanged(NotifyMessage notMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (notMessage.ListData == null || notMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("ChanStateChanged", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strState = notMessage.ListData[1];
                string strNewChanObjID = notMessage.ListData[2];
                long newChanObjID;
                if (strMonID == mMonitorObject.MonID)
                {
                    MonitorObject obj = mMonitorObject;
                    if (!string.IsNullOrEmpty(strNewChanObjID)
                        && long.TryParse(strNewChanObjID, out newChanObjID)
                        && newChanObjID > 0)
                    {
                        //关联的通道变了，需要重新查询通道信息
                        QueryChanInfo(obj);
                        return;
                    }
                    optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("ChanStateChanged", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ChanState chanState = optReturn.Data as ChanState;
                    if (chanState == null)
                    {
                        CurrentApp.WriteLog("ChanStateChanged", string.Format("ChannelState object is null"));
                        return;
                    }
                    if (mMonitorItem != null)
                    {
                        if (obj.Role == 1)
                        {
                            mMonitorItem.VoiceChanState = chanState;
                        }
                        if (obj.Role == 2)
                        {
                            mMonitorItem.ScreenChanState = chanState;
                        }
                        mMonitorItem.TimeDeviation = mTimeDeviation;
                        Dispatcher.Invoke(new Action(mMonitorItem.UpdateState));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealStartSMonResponse(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData == null || retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("StartSMonResponse", string.Format("ListData count invalid"));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strMonPort = retMessage.ListData[1];
                if (strMonID == mMonitorObject.MonID)
                {
                    MonitorObject monObj = mMonitorObject;
                    int intMonPort;
                    if (!int.TryParse(strMonPort, out intMonPort))
                    {
                        CurrentApp.WriteLog("StartSMonResponse", string.Format("Monitor port invalid"));
                        return;
                    }
                    string strHost = monObj.Other03;    //录屏服务器的地址
                    string strChanID = monObj.Other01;
                    int intChanID;
                    if (!int.TryParse(strChanID, out intChanID))
                    {
                        CurrentApp.WriteLog("StartSMonResponse", string.Format("Channel ID invalid"));
                        return;
                    }

                    OperationReturn optReturn = Utils.DownloadMediaUtils(CurrentApp.Session);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("StartSMonResponse",
                            string.Format("DownloadMediaUtil fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                    CurrentApp.WriteLog("StartSMonResponse", string.Format("VLSMonSetWaterMarkReturn:{0}", intReturn));
                    //设置窗口尺寸
                    int intScale = 100;
                    if (ListUserParams != null)
                    {
                        var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_SCALE);
                        if (userParam != null)
                        {
                            int temp;
                            if (int.TryParse(userParam.ParamValue, out temp))
                            {
                                if (temp > 0 && temp <= 100)
                                {
                                    intScale = temp;
                                }
                            }
                        }
                    }
                    intReturn = ScreenMPInterop.VLSMonSetScale(intScale);
                    CurrentApp.WriteLog("StartSMonResponse", string.Format("VLSMonSetScaleReturn:{0}", intReturn));
                    SRCMON_PARAM param = new SRCMON_PARAM();
                    param.sVocIp = Converter.String2ByteArray(strHost, ScreenMPDefines.SIZE_IPADDRESS);
                    param.nPort = (ushort)intMonPort;
                    param.nChannel = (ushort)intChanID;
                    intReturn = ScreenMPInterop.VLSMonStart(ref param, null, handle);
                    CurrentApp.WriteLog("StartSMonResponse", string.Format("VLSMonStartReturn:{0}", intReturn));
                    if (intReturn == 0)
                    {
                        mIsScrWinOpened = true;
                        IntPtr hwdPlayer = ScreenMPInterop.VLSMonGetPlayerHwnd();

                        //窗口置顶
                        if (ListUserParams != null)
                        {
                            var userParam =
                                ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_TOPMOST);
                            if (userParam != null
                                && userParam.ParamValue == "1"
                                && hwdPlayer != IntPtr.Zero)
                            {
                                intReturn = User32Interop.SetWindowPos(hwdPlayer, -1, 0, 0, 0, 0, 3);
                                CurrentApp.WriteLog("StartSMonResponse", string.Format("SetWindowPos:{0}", intReturn));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("StartSMonResponse", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealServerErrorMessage(ReturnMessage retMessage)
        {
            try
            {
                string strMsg = retMessage.Message;
                if (retMessage.ListData != null)
                {
                    if (retMessage.ListData.Count > 0)
                    {
                        strMsg = string.Format("{0};\t[{1}]", strMsg, retMessage.ListData[0]);
                    }
                    if (retMessage.ListData.Count > 1)
                    {
                        strMsg = string.Format("{0};\t[{1}]", strMsg, retMessage.ListData[1]);
                    }
                }
                CurrentApp.WriteLog("DealServerError", strMsg);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        private void StartSMon()
        {
            try
            {
                StartSMonObject();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void StopSMon()
        {
            try
            {
                StopSMonObject(false);

                if (mIsScrWinOpened)
                {
                    int intReturn;
                    intReturn = ScreenMPInterop.VLSMonStop();
                    CurrentApp.WriteLog("StoSMon", string.Format("MonStopReturn:{0}", intReturn));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void CloseSMonPanel()
        {
            try
            {
                StopSMonObject(true);

                if (mIsScrWinOpened)
                {
                    int intReturn;
                    intReturn = ScreenMPInterop.VLSMonStop();
                    CurrentApp.WriteLog("StoSMon", string.Format("MonStopReturn:{0}", intReturn));
                }

                RemoveMonObject();
                if (PageParent != null)
                {
                    PageParent.ClosePanel(2);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateOperationButtons()
        {
            try
            {
                OperationInfo optInfo;
                Button btn;

                PanelSMonOpts.Children.Clear();
                //开始监屏
                if (ListOperations != null)
                {
                    optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_SCRMON);
                    if (optInfo != null)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = optInfo;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelSMonOpts.Children.Add(btn);
                    }
                }
                //暂停（停止）监屏
                optInfo = new OperationInfo();
                optInfo.ID = S2102Consts.OPT_STOPSCRMON;
                optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "PauseScrMon");
                optInfo.Description = optInfo.Display;
                optInfo.Icon = "Images/00011.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelSMonOpts.Children.Add(btn);
                //关闭监屏面板（同时停止监屏）
                optInfo = new OperationInfo();
                optInfo.ID = S2102Consts.OPT_CLOSESCRMON;
                optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "CloseScrMon");
                optInfo.Description = optInfo.Display;
                optInfo.Icon = "Images/00012.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelSMonOpts.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void NMonMonitorClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                var notMessage = e.NotifyMessage;
                if (notMessage == null) { return; }
                switch (notMessage.Command)
                {
                    case (int)Service04Command.NotStateChanged:
                        ThreadPool.QueueUserWorkItem(a => DealChanStateChanged(notMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void NMonMonitorClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                var retMessage = e.ReturnMessage;
                if (retMessage == null) { return; }
                if (!retMessage.Result)
                {
                    CurrentApp.WriteLog("DealMessage", string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                    return;
                }
                switch (retMessage.Command)
                {
                    case (int)RequestCode.NCWelcome:
                        CurrentApp.WriteLog("DealMessage", string.Format("SetMonType response"));
                        DealWelcomeMessage(retMessage);
                        //连接成功，设定监视方式
                        ThreadPool.QueueUserWorkItem(a => SetMonType());
                        break;
                    case (int)Service04Command.ResSetMonType:
                        if (retMessage.ListData != null && retMessage.ListData.Count > 1)
                        {
                            string newType = retMessage.ListData[0];
                            string oldType = retMessage.ListData[1];
                            CurrentApp.WriteLog("DealMessage", string.Format("MonType\tNew:{0}\tOld:{1}", newType, oldType));
                        }
                        //设定监视方式后，添加监视对象
                        ThreadPool.QueueUserWorkItem(a => AddQueryChanMonObject());
                        break;
                    case (int)Service04Command.ResRemoveMonObj:
                        CurrentApp.WriteLog("DealMessage", string.Format("RemoveMonObj response"));
                        break;
                    case (int)Service04Command.ResAddQueryChan:
                        CurrentApp.WriteLog("DealMessage", string.Format("AddQueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealAddQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResQueryChan:
                        CurrentApp.WriteLog("DealMessage", string.Format("QueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResQueryState:
                        CurrentApp.WriteLog("DealMessage", string.Format("QueryState response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryStateResponse(retMessage));
                        break;
                    case (int)Service04Command.ResStartSMon:
                        CurrentApp.WriteLog("DealMessage", string.Format("StartSMon response"));
                        ThreadPool.QueueUserWorkItem(a => Dispatcher.Invoke(new Action(() => DealStartSMonResponse(retMessage))));
                        break;
                    case (int)Service04Command.ResStopSMon:
                        CurrentApp.WriteLog("DealMessage", string.Format("StopSMon response"));
                        break;
                    case (int)RequestCode.NCError:
                        CurrentApp.WriteLog("DealMessage", string.Format("ServerError message"));
                        ThreadPool.QueueUserWorkItem(a => DealServerErrorMessage(retMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null) { return; }
            var optItem = btn.DataContext as OperationInfo;
            if (optItem == null) { return; }
            switch (optItem.ID)
            {
                case S2102Consts.OPT_SCRMON:
                    StartSMon();
                    break;
                case S2102Consts.OPT_STOPSCRMON:
                    StopSMon();
                    break;
                case S2102Consts.OPT_CLOSESCRMON:
                    CloseSMonPanel();
                    break;
            }
        }

        void RecordLengthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (mMonitorItem != null)
                {
                    mMonitorItem.TimeDeviation = mTimeDeviation;
                    mMonitorItem.UpdateState();
                }
            }
            catch { }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                string strTitle = string.Format("{0} —— ", CurrentApp.GetLanguageInfo("2102016", "Screen Monitor"));
                if (mMonitorItem != null)
                {
                    strTitle += mMonitorItem.Name;
                }
                TxtObjListTitle.Text = strTitle;

                TxtLoginState.Text = CurrentApp.GetLanguageInfo("COL2102001LoginState", "Login State");
                TxtRecordState.Text = CurrentApp.GetLanguageInfo("COL2102001RecordState", "Record State");
                TxtReference.Text = CurrentApp.GetLanguageInfo("COL2102001RecordReference", "Record Reference");
                TxtStartTime.Text = CurrentApp.GetLanguageInfo("COL2102001StartRecordTime", "Start Time");

                CreateOperationButtons();

            }
            catch { }
        }

        #endregion
    }
}
