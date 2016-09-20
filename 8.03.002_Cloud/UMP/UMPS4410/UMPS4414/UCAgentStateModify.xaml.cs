//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e0876bb-6de9-4157-ab5d-944cbe81f8fe
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAgentStateModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414
//        File Name:                UCAgentStateModify
//
//        created by Charley at 2016/6/23 09:48:32
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
using UMPS4414.Models;
using UMPS4414.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4414
{
    /// <summary>
    /// UCAgentStateModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentStateModify
    {
        public StateSettingMainView PageParent;
        public bool IsModify;
        public ObjItem StateItem;
        public List<AgentStateInfo> ListAllStateInfos;

        private bool mIsInited;
        private AgentStateInfo mStateInfo;
        private ObservableCollection<StateTypeItem> mListStateTypeItems;

        public UCAgentStateModify()
        {
            InitializeComponent();

            mListStateTypeItems = new ObservableCollection<StateTypeItem>();

            Loaded += UCAgentStateModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCAgentStateModify_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                ComboStateTypes.ItemsSource = mListStateTypeItems;
                InitStateTypeItems();
                if (IsModify)
                {
                    if (StateItem == null) { return; }
                    var stateInfo = StateItem.Data as AgentStateInfo;
                    mStateInfo = stateInfo;
                    if (mStateInfo == null) { return; }
                    TxtName.Text = mStateInfo.Name;
                    var typeItem = mListStateTypeItems.FirstOrDefault(t => t.Value == mStateInfo.Type);
                    ComboStateTypes.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = mStateInfo.State == 1;
                    RadioStateDisable.IsChecked = mStateInfo.State != 1;
                    TxtColor.SelectedColor = GetColorFromString(mStateInfo.Color);
                    TxtIcon.Text = mStateInfo.Icon;
                    RadioIsWorkTimeYes.IsChecked = mStateInfo.IsWorkTime;
                    TxtValue.Text = mStateInfo.Value.ToString();
                    RadioIsWorkTimeNo.IsChecked = !mStateInfo.IsWorkTime;
                    TxtDescription.Text = mStateInfo.Description;
                }
                else
                {
                    TxtName.Text = string.Empty;
                    var typeItem = mListStateTypeItems.FirstOrDefault();
                    ComboStateTypes.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    TxtColor.SelectedColor = Brushes.Transparent.Color;
                    TxtIcon.Text = string.Empty;
                    RadioIsWorkTimeYes.IsChecked = true;
                    TxtValue.Text = "0";
                    RadioIsWorkTimeNo.IsChecked = false;
                    TxtDescription.Text = string.Empty;
                }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStateTypeItems()
        {
            try
            {
                mListStateTypeItems.Clear();
                StateTypeItem item = new StateTypeItem();
                item.Name = "None";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 0.ToString("000")), "None");
                item.Value = 0;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Login";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 1.ToString("000")), "Login");
                item.Value = 1;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Call";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 2.ToString("000")), "Call");
                item.Value = 2;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Record";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 3.ToString("000")), "Record");
                item.Value = 3;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Direction";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 4.ToString("000")), "Direction");
                item.Value = 4;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Agent";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 5.ToString("000")), "Agent");
                item.Value = 5;
                mListStateTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        private void AddAgentState()
        {
            try
            {
                AgentStateInfo stateInfo = new AgentStateInfo();
                if (ListAllStateInfos == null) { return; }
                int number = 10;        //自定义的状态的编码从10开始累加
                for (int i = 0; i < ListAllStateInfos.Count; i++)
                {
                    var info = ListAllStateInfos[i];
                    number = Math.Max(info.Number, number);
                }
                number++;
                stateInfo.Number = number;
                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(string.Format("Name can not be empty."));
                    return;
                }
                stateInfo.Name = strName;
                var typeItem = ComboStateTypes.SelectedItem as StateTypeItem;
                if (typeItem != null)
                {
                    stateInfo.Type = typeItem.Value;
                }
                else
                {
                    stateInfo.Type = S4410Consts.STATE_TYPE_UNKOWN;
                }
                string strValue = TxtValue.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(string.Format("Value invalid"));
                    return;
                }
                stateInfo.Value = intValue;
                stateInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                stateInfo.Color = TxtColor.SelectedColor.ToString();
                stateInfo.Icon = TxtIcon.Text;
                stateInfo.IsWorkTime = RadioIsWorkTimeYes.IsChecked == true;
                stateInfo.Description = TxtDescription.Text;
                stateInfo.Type = 0;
                stateInfo.Creator = CurrentApp.Session.UserID;
                stateInfo.CreateTime = DateTime.Now.ToUniversalTime();
                stateInfo.Modifier = CurrentApp.Session.UserID;
                stateInfo.ModifyTime = DateTime.Now.ToUniversalTime();

                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(stateInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strInfo = optReturn.Data.ToString();

                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.SaveAgentStateInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add("1");
                        webRequest.ListData.Add(strInfo);
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
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                CurrentApp.WriteLog("DeleteAgentState", string.Format("{0}", webReturn.ListData[i]));
                            }
                        }
                        ShowInformation(string.Format("Add AgentState end"));
                        isSuccess = true;
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
                        if (isSuccess)
                        {
                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }

                            if (PageParent != null)
                            {
                                PageParent.ReloadData();
                            }
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

        private void ModifyAgentState()
        {
            try
            {
                if (StateItem == null) { return; }
                var stateInfo = StateItem.Data as AgentStateInfo;
                if (stateInfo == null) { return; }

                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(string.Format("Name can not be empty."));
                    return;
                }
                stateInfo.Name = strName;
                var typeItem = ComboStateTypes.SelectedItem as StateTypeItem;
                if (typeItem != null)
                {
                    stateInfo.Type = typeItem.Value;
                }
                else
                {
                    stateInfo.Type = S4410Consts.STATE_TYPE_UNKOWN;
                }
                string strValue = TxtValue.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(string.Format("Value invalid"));
                    return;
                }
                stateInfo.Value = intValue;
                stateInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                stateInfo.Color = TxtColor.SelectedColor.ToString();
                stateInfo.Icon = TxtIcon.Text;
                stateInfo.IsWorkTime = RadioIsWorkTimeYes.IsChecked == true;
                stateInfo.Description = TxtDescription.Text;
                stateInfo.Modifier = CurrentApp.Session.UserID;
                stateInfo.ModifyTime = DateTime.Now.ToUniversalTime();

                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(stateInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strInfo = optReturn.Data.ToString();

                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.SaveAgentStateInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add("1");
                        webRequest.ListData.Add(strInfo);
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
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                CurrentApp.WriteLog("DeleteAgentState", string.Format("{0}", webReturn.ListData[i]));
                            }
                        }
                        ShowInformation(string.Format("Modify AgentState end"));
                        isSuccess = true;
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
                        if (isSuccess)
                        {
                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }

                            if (PageParent != null)
                            {
                                PageParent.ReloadData();
                            }
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


        #region Event Handler

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (IsModify)
            {
                ModifyAgentState();
            }
            else
            {
                AddAgentState();
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
                        parent.Title = CurrentApp.GetLanguageInfo("4414004", "Modify State");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4414003", "Add State");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbName.Text = CurrentApp.GetLanguageInfo("4414005", "State Name");
                LbType.Text = CurrentApp.GetLanguageInfo("4414006", "Type");
                LbState.Text = CurrentApp.GetLanguageInfo("4414007", "State");
                LbColor.Text = CurrentApp.GetLanguageInfo("4414008", "Color");
                LbIcon.Text = CurrentApp.GetLanguageInfo("4414009", "Icon");
                LbIsWorkTime.Text = CurrentApp.GetLanguageInfo("4414010", "Is work time");
                LbValue.Text = CurrentApp.GetLanguageInfo("4414011", "Value");
                LbDescription.Text = CurrentApp.GetLanguageInfo("4414012", "Description");

                for (int i = 0; i < mListStateTypeItems.Count; i++)
                {
                    var item = mListStateTypeItems[i];
                    int intValue = item.Value;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", intValue.ToString("000")),
                        intValue.ToString());
                }

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4414014001", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4414014000", "Disable");
                RadioIsWorkTimeYes.Content = CurrentApp.GetLanguageInfo("4414015001", "Yes");
                RadioIsWorkTimeNo.Content = CurrentApp.GetLanguageInfo("4414015000", "No");
            }
            catch { }
        }

        #endregion
    }
}
