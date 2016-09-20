//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4e22a0e3-2c74-4bf7-b6b6-7fe86665bdce
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAlarmInfoModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415
//        File Name:                UCAlarmInfoModify
//
//        created by Charley at 2016/7/12 18:31:15
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using UMPS4415.Models;
using UMPS4415.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4415
{
    /// <summary>
    /// UCAlarmInfoModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmInfoModify
    {

        #region Members

        public AlarmSettingMainView PageParent;
        public AlarmMessageItem AlarmItem;
        public bool IsModify;

        private List<AgentStateInfo> mListAgentStateInfos;
        private ObservableCollection<ComboItem> mListTypeItems;
        private ObservableCollection<ComboItem> mListAlarmTypeItems;
        private ObservableCollection<ComboItem> mListRelativeStateItems;
        private bool mIsInited;

        #endregion


        public UCAlarmInfoModify()
        {
            InitializeComponent();

            mListAgentStateInfos = new List<AgentStateInfo>();
            mListTypeItems = new ObservableCollection<ComboItem>();
            mListAlarmTypeItems = new ObservableCollection<ComboItem>();
            mListRelativeStateItems = new ObservableCollection<ComboItem>();

            Loaded += UCAlarmInfoModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnIcon.Click += BtnIcon_Click;
        }

        void UCAlarmInfoModify_Loaded(object sender, RoutedEventArgs e)
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
                ComboType.ItemsSource = mListTypeItems;
                ComboAlarmType.ItemsSource = mListAlarmTypeItems;
                ComboRelativeState.ItemsSource = mListRelativeStateItems;

                InitTypeItems();
                InitAlarmTypeItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAgentStateInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateRelativeStateItems();
                    InitInfo();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
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
                if (IsModify)
                {
                    if (AlarmItem == null) { return; }
                    var alarmInfo = AlarmItem.Info;
                    if (alarmInfo == null) { return; }
                    TxtName.Text = alarmInfo.Name;
                    var typeItem = mListTypeItems.FirstOrDefault(t => t.IntValue == alarmInfo.Type);
                    ComboType.SelectedItem = typeItem;
                    typeItem = mListAlarmTypeItems.FirstOrDefault(t => t.IntValue == alarmInfo.AlarmType);
                    ComboAlarmType.SelectedItem = typeItem;
                    TxtRank.Value = alarmInfo.Rank;
                    TxtColor.SelectedColor = GetColorFromString(alarmInfo.Color);
                    TxtIcon.Text = alarmInfo.Icon;
                    TxtHoldTime.Value = alarmInfo.HoldTime;
                    typeItem = mListRelativeStateItems.FirstOrDefault(s => s.LongValue == alarmInfo.StateID);
                    ComboRelativeState.SelectedItem = typeItem;
                    TxtValue.Text = alarmInfo.Value;
                    TxtContent.Text = alarmInfo.Content;
                }
                else
                {
                    TxtName.Text = string.Empty;
                    var typeItem = mListTypeItems.FirstOrDefault(t => t.IntValue == 0);
                    ComboType.SelectedItem = typeItem;
                    typeItem = mListAlarmTypeItems.FirstOrDefault(t => t.IntValue == 0);
                    ComboAlarmType.SelectedItem = typeItem;
                    TxtRank.Value = 0;
                    TxtColor.SelectedColor = Brushes.Transparent.Color;
                    TxtIcon.Text = string.Empty;
                    TxtHoldTime.Value = 0;
                    typeItem = mListRelativeStateItems.FirstOrDefault(s => s.LongValue == 0);
                    ComboRelativeState.SelectedItem = typeItem;
                    TxtValue.Text = string.Empty;
                    TxtContent.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitTypeItems()
        {
            try
            {
                mListTypeItems.Clear();
                ComboItem item = new ComboItem();
                item.Name = "Traditional";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 0.ToString("000")), "Traditional");
                item.IntValue = 0;
                mListTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "State Timeout";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 1.ToString("000")), "State Timeout");
                item.IntValue = 1;
                mListTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Keyword";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 2.ToString("000")), "Keyword");
                item.IntValue = 1;
                mListTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAlarmTypeItems()
        {
            try
            {
                mListAlarmTypeItems.Clear();
                ComboItem item = new ComboItem();
                item.Name = "None";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415016{0}", 0.ToString("000")), "None");
                item.IntValue = 0;
                mListAlarmTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Alarm";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415016{0}", 1.ToString("000")), "Alarm");
                item.IntValue = 1;
                mListAlarmTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Alert";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415016{0}", 2.ToString("000")), "Alert");
                item.IntValue = 2;
                mListAlarmTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAgentStateInfos()
        {
            try
            {
                mListAgentStateInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAgentStateList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<AgentStateInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AgentStateInfo info = optReturn.Data as AgentStateInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("AgentStateInfo is null"));
                        return;
                    }
                    mListAgentStateInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAgentStateInfos", string.Format("End.\t{0}", mListAgentStateInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRelativeStateItems()
        {
            try
            {
                mListRelativeStateItems.Clear();
                for (int i = 0; i < mListAgentStateInfos.Count; i++)
                {
                    var info = mListAgentStateInfos[i];
                    ComboItem item = new ComboItem();
                    item.Data = info;
                    item.Name = info.Name;
                    item.Display = info.Name;
                    item.LongValue = info.ObjID;
                    mListRelativeStateItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnIcon_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (IsModify)
            {
                ModifyAlarmMessage();
            }
            else
            {
                AddAlarmMessage();
            }
        }

        #endregion


        #region Others

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

        #endregion


        #region Operations

        private void AddAlarmMessage()
        {
            try
            {
                AlarmMessageInfo alarmInfo = new AlarmMessageInfo();
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    ShowException(string.Format("Name empty"));
                    return;
                }
                alarmInfo.Name = TxtName.Text;
                var typeItem = ComboType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(string.Format("Type invalid"));
                    return;
                }
                alarmInfo.Type = typeItem.IntValue;
                typeItem = ComboAlarmType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(string.Format("AlarmType invalid"));
                    return;
                }
                alarmInfo.AlarmType = typeItem.IntValue;
                if (TxtRank.Value == null)
                {
                    ShowException(string.Format("Rank invalid"));
                    return;
                }
                int intValue = (int)TxtRank.Value;
                if (intValue < 0 || intValue > 10)
                {
                    ShowException(string.Format("Rank invalid"));
                    return;
                }
                alarmInfo.Rank = intValue;
                alarmInfo.Color = TxtColor.SelectedColor.ToString();
                alarmInfo.Icon = TxtIcon.Text;
                if (TxtHoldTime.Value == null)
                {
                    ShowException(string.Format("HoldTime invalid"));
                    return;
                }
                intValue = (int)TxtHoldTime.Value;
                if (intValue < 0)
                {
                    ShowException(string.Format("HoldTime invalid"));
                    return;
                }
                alarmInfo.HoldTime = intValue;
                typeItem = ComboRelativeState.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    alarmInfo.StateID = 0;
                }
                else
                {
                    alarmInfo.StateID = typeItem.LongValue;
                }
                alarmInfo.Value = TxtValue.Text;
                alarmInfo.Content = TxtContent.Text;

                bool isFail = true;
                string strMsg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmMessage;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(alarmInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.ListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string str = webReturn.ListData[i];

                            CurrentApp.WriteLog("AddAlarmMessage", string.Format("{0}", str));
                        }
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

                    if (!isFail)
                    {
                        ShowInformation(string.Format("Add successful"));

                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                    else
                    {
                        ShowException(strMsg);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAlarmMessage()
        {
            try
            {
                if (AlarmItem == null) { return; }
                var alarmInfo = AlarmItem.Info;
                if (alarmInfo == null) { return; }
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    ShowException(string.Format("Name empty"));
                    return;
                }
                alarmInfo.Name = TxtName.Text;
                var typeItem = ComboType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(string.Format("Type invalid"));
                    return;
                }
                alarmInfo.Type = typeItem.IntValue;
                typeItem = ComboAlarmType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(string.Format("AlarmType invalid"));
                    return;
                }
                alarmInfo.AlarmType = typeItem.IntValue;
                if (TxtRank.Value == null)
                {
                    ShowException(string.Format("Rank invalid"));
                    return;
                }
                int intValue = (int)TxtRank.Value;
                if (intValue < 0 || intValue > 10)
                {
                    ShowException(string.Format("Rank invalid"));
                    return;
                }
                alarmInfo.Rank = intValue;
                alarmInfo.Color = TxtColor.SelectedColor.ToString();
                alarmInfo.Icon = TxtIcon.Text;
                if (TxtHoldTime.Value == null)
                {
                    ShowException(string.Format("HoldTime invalid"));
                    return;
                }
                intValue = (int)TxtHoldTime.Value;
                if (intValue < 0)
                {
                    ShowException(string.Format("HoldTime invalid"));
                    return;
                }
                alarmInfo.HoldTime = intValue;
                typeItem = ComboRelativeState.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    alarmInfo.StateID = 0;
                }
                else
                {
                    alarmInfo.StateID = typeItem.LongValue;
                }
                alarmInfo.Value = TxtValue.Text;
                alarmInfo.Content = TxtContent.Text;

                bool isFail = true;
                string strMsg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmMessage;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(alarmInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                       new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                           WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.ListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string str = webReturn.ListData[i];

                            CurrentApp.WriteLog("ModifyAlarmMessage", string.Format("{0}", str));
                        }
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

                    if (!isFail)
                    {
                        ShowInformation(string.Format("Modify successful"));

                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                    else
                    {
                        ShowException(strMsg);
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


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (IsModify)
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4415004", "Modify Alarm");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4415003", "Add Alarm");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbName.Text = CurrentApp.GetLanguageInfo("4415005", "Alarm Name");
                LbType.Text = CurrentApp.GetLanguageInfo("4415006", "Type");
                LbAlarmType.Text = CurrentApp.GetLanguageInfo("4415007", "Alarm Type");
                LbRank.Text = CurrentApp.GetLanguageInfo("4415008", "Rank");
                LbColor.Text = CurrentApp.GetLanguageInfo("4415009", "Color");
                LbIcon.Text = CurrentApp.GetLanguageInfo("4415010", "Icon");
                LbHoldTime.Text = CurrentApp.GetLanguageInfo("4415011", "Hold Time");
                LbRelativeState.Text = CurrentApp.GetLanguageInfo("4415012", "Relative State");
                LbValue.Text = CurrentApp.GetLanguageInfo("4415013", "Value");
                LbContent.Text = CurrentApp.GetLanguageInfo("4415014", "Content");

                for (int i = 0; i < mListTypeItems.Count; i++)
                {
                    var item = mListTypeItems[i];
                    int intValue = item.IntValue;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", intValue.ToString("000")),
                        intValue.ToString());
                }
                for (int i = 0; i < mListAlarmTypeItems.Count; i++)
                {
                    var item = mListAlarmTypeItems[i];
                    int intValue = item.IntValue;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4415016{0}", intValue.ToString("000")),
                        intValue.ToString());
                }
            }
            catch (Exception ex){}
        }

        #endregion

    }
}
