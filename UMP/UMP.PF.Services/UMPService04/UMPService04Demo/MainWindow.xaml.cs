using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;

namespace UMPService04Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private NetClient mMonitorClient;
        private ObservableCollection<VoiceChanStateItem> mListVoiceChanStateItems;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListVoiceChanStateItems = new ObservableCollection<VoiceChanStateItem>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LvChanStates.ItemsSource = mListVoiceChanStateItems;

            InitChannelInfos();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mMonitorClient != null)
            {
                mMonitorClient.Stop();
                mMonitorClient = null;
            }
        }


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string strAddress = "192.168.5.31";
                //int intPort = 8081 - 4;
                //mMonitorClient = new MonitorClient();
                //mMonitorClient.Debug += (mode, msg) => AppendMessage(string.Format("MonitorClient\t{0}", msg));
                //mMonitorClient.ReturnMessageReceived += MonitorClient_ReturnMessageReceived;
                //mMonitorClient.NotifyMessageReceived += MonitorClient_NotifyMessageReceived;
                //mMonitorClient.Connect(strAddress, intPort);

                GlobalSetting setting=new GlobalSetting();
                setting.Key = ConstValue.GS_KEY_LOG_MODE;
                setting.Value = ((int) LogMode.All).ToString();
                ConfigInfo configInfo = new ConfigInfo();
                configInfo.ListSettings.Add(setting);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstValue.TEMP_FILE_CONFIGINFO);
                OperationReturn optReturn = XMLHelper.SerializeFile(configInfo, path);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SetMonType()
        {
            try
            {
                OperationReturn optReturn;

                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqSetMonType;
                request.SessionID = mMonitorClient.SessionID;
                request.ListData.Add(((int)MonitorType.State).ToString());
                optReturn = mMonitorClient.SendMessage((int)Service04Command.ReqAddMonObj, request);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("AddMonObj fail.\t{0}", ex.Message));
            }
        }

        private void AddMonObj()
        {
            try
            {
                OperationReturn optReturn;

                List<MonitorObject> listObj = new List<MonitorObject>();
                for (int i = 0; i < mListVoiceChanStateItems.Count; i++)
                {
                    var item = mListVoiceChanStateItems[i];
                    var obj = item.MonitorObject;
                    if (obj != null)
                    {
                        listObj.Add(obj);
                    }
                }

                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqAddMonObj;
                request.SessionID = mMonitorClient.SessionID;
                int count = listObj.Count;
                request.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    var obj = listObj[i];
                    optReturn = XMLHelper.SeriallizeObject(obj);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    request.ListData.Add(optReturn.Data.ToString());
                }
                optReturn = mMonitorClient.SendMessage((int)Service04Command.ReqAddMonObj, request);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("AddMonObj fail.\t{0}", ex.Message));
            }
        }

        private void QueryChanInfo()
        {
            try
            {
                OperationReturn optReturn;

                List<MonitorObject> listObj = new List<MonitorObject>();
                for (int i = 0; i < mListVoiceChanStateItems.Count; i++)
                {
                    var item = mListVoiceChanStateItems[i];
                    var obj = item.MonitorObject;
                    if (obj != null)
                    {
                        listObj.Add(obj);
                    }
                }

                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqQueryChan;
                request.SessionID = mMonitorClient.SessionID;
                int count = listObj.Count;
                request.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    var obj = listObj[i];
                    optReturn = XMLHelper.SeriallizeObject(obj);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    request.ListData.Add(optReturn.Data.ToString());
                }
                optReturn = mMonitorClient.SendMessage((int)Service04Command.ReqQueryChan, request);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("QueryChanInfo fail.\t{0}", ex.Message));
            }
        }

        private void QueryChanState()
        {
            try
            {
                OperationReturn optReturn;

                for (int i = 0; i < mListVoiceChanStateItems.Count; i++)
                {
                    var item = mListVoiceChanStateItems[i];
                    var obj = item.MonitorObject;
                    if (obj != null)
                    {
                        RequestMessage request = new RequestMessage();
                        request.Command = (int)Service04Command.ReqQueryState;
                        request.SessionID = mMonitorClient.SessionID;
                        optReturn = XMLHelper.SeriallizeObject(obj);
                        if (!optReturn.Result)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        request.ListData.Add(optReturn.Data.ToString());
                        optReturn = mMonitorClient.SendMessage((int)Service04Command.ReqQueryState, request);
                        if (!optReturn.Result)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("QueryChanInfo fail.\t{0}", ex.Message));
            }
        }

        private void UpdateChanState(MonitorObject monObj, ChanState chanState)
        {
            //try
            //{
            //    var item =
            //        mListVoiceChanStateItems.FirstOrDefault(
            //            o => o.VoiceID.ToString() == monObj.Other01 && o.ChanID.ToString() == monObj.Name);
            //    if (item != null)
            //    {
            //        item.UpdateInfo(chanState);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    AppendMessage(string.Format("UpdateChanState fail.\t{0}", ex.Message));
            //}
        }

        #endregion


        #region DealMessageReceived

        void MonitorClient_ReturnMessageReceived(ReturnMessage retMessage)
        {
            try
            {
                if (!retMessage.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}\t{2}",
                        retMessage.Command,
                        retMessage.Code,
                        retMessage.Message));
                    return;
                }
                switch (retMessage.Command)
                {
                    case (int)RequestCode.NCWelcome:
                        SetMonType();
                        break;
                    case (int)Service04Command.ResSetMonType:
                        AppendMessage(string.Format("End.\t{0}\t{1}",
                            retMessage.Command,
                            retMessage.ListData[0]));
                        AddMonObj();
                        break;
                    case (int)Service04Command.ResAddMonObj:
                        AppendMessage(string.Format("End.\t{0}\t{1}\t{2}",
                            retMessage.Command,
                            retMessage.ListData[0],
                            retMessage.ListData[1]));
                        QueryChanInfo();
                        break;
                    case (int)Service04Command.ResQueryChan:
                        AppendMessage(string.Format("End.\t{0}\t{1}",
                           retMessage.Command,
                           retMessage.ListData[0]));
                        int count;
                        if (int.TryParse(retMessage.ListData[0], out count))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                string str = retMessage.ListData[i + 1];
                                OperationReturn optReturn = XMLHelper.DeserializeObject<MonitorObject>(str);
                                if (!optReturn.Result)
                                {
                                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    return;
                                }
                                MonitorObject obj = optReturn.Data as MonitorObject;
                                if (obj == null)
                                {
                                    AppendMessage(string.Format("MonitorObject is null"));
                                    return;
                                }
                                var temp =
                                    mListVoiceChanStateItems.FirstOrDefault(c => c.MonitorObject.ObjID == obj.ObjID);
                                if (temp != null)
                                {
                                    temp.MonitorObject.ChanObjID = obj.ChanObjID;
                                    temp.ChanObjID = obj.ChanObjID;
                                }
                            }
                        }
                        QueryChanState();
                        break;
                    case (int)Service04Command.ResQueryState:
                        AppendMessage(string.Format("End.\t{0}\t{1}\t{2}",
                            retMessage.Command,
                            retMessage.ListData[0],
                            retMessage.ListData[1]));
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("DealMessage fail.\t{0}", ex.Message));
            }
        }

        void MonitorClient_NotifyMessageReceived(NotifyMessage norMessage)
        {
            try
            {
                MonitorObject monObj;
                ChanState chanState;
                string strInfo;
                OperationReturn optReturn;

                switch (norMessage.Command)
                {
                    case (int)Service04Command.NotStateChanged:
                        if (norMessage.ListData == null || norMessage.ListData.Count < 2)
                        {
                            AppendMessage(string.Format("ListData is null or count invalid"));
                            return;
                        }
                        strInfo = norMessage.ListData[0];
                        optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                        if (!optReturn.Result)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        monObj = optReturn.Data as MonitorObject;
                        if (monObj == null)
                        {
                            AppendMessage(string.Format("MonitorObject is null"));
                            return;
                        }
                        strInfo = norMessage.ListData[1];
                        optReturn = XMLHelper.DeserializeObject<ChanState>(strInfo);
                        if (!optReturn.Result)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        chanState = optReturn.Data as ChanState;
                        if (chanState == null)
                        {
                            AppendMessage(string.Format("VoiceChanState is null"));
                            return;
                        }
                        UpdateChanState(monObj, chanState);
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("DealNotifyMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Init and Load

        private void InitChannelInfos()
        {
            mListVoiceChanStateItems.Clear();
            for (int i = 0; i < 10; i++)
            {
                MonitorObject obj = new MonitorObject();
                obj.MonType = MonitorType.State;
                obj.ObjID = i + 2250000000000000001;
                obj.ObjType = ConstValue.RESOURCE_VOICECHANNEL;
                VoiceChanStateItem item = new VoiceChanStateItem();
                item.ChanObjID = obj.ObjID;
                item.MonitorObject = obj;
                mListVoiceChanStateItems.Add(item);
            }
        }

        #endregion


        #region Others

        private void AppendMessage(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            })));
        }

        #endregion


    }
}
