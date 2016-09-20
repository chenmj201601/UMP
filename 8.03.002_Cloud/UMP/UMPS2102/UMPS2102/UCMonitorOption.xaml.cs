//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b2c23c8e-d779-41cf-b2bf-dc95f6830620
//        CLR Version:              4.0.30319.18063
//        Name:                     UCMonitorOption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                UCMonitorOption
//
//        created by Charley at 2015/7/7 16:04:01
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UMPS2102.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2102
{
    /// <summary>
    /// UCMonitorOption.xaml 的交互逻辑
    /// </summary>
    public partial class UCMonitorOption
    {
        public List<UserParamInfo> ListUserParams;
        public ChanMonitorMainView PageParent;

        private bool mIsOptSuccess;
        private BackgroundWorker mWorker;

        public UCMonitorOption()
        {
            InitializeComponent();

            Loaded += UCMonitorOption_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCMonitorOption_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            try
            {
                if (ListUserParams == null) { return; }
                var userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCLOGINSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorVocLoginState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorVocLoginState.SelectedColor = Brushes.Wheat.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRLOGINSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorScrLoginState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorScrLoginState.SelectedColor = Brushes.Wheat.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRLOGINSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorVocScrLoginState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorVocScrLoginState.SelectedColor = Brushes.Wheat.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCRECORDSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorVocRecordState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorVocRecordState.SelectedColor = Brushes.LightCoral.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRRECORDSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorScrRecordState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorScrRecordState.SelectedColor = Brushes.LightCoral.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRRECORDSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorVocScrRecordState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorVocScrRecordState.SelectedColor = Brushes.LightCoral.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLINSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorCallinState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorCallinState.SelectedColor = Brushes.DarkBlue.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLOUTSTATE);
                if (userParam != null)
                {
                    string strColor = userParam.ParamValue;
                    try
                    {
                        Color color = Utils.GetColorFromRgbString(strColor);
                        ColorCalloutState.SelectedColor = color;
                    }
                    catch { }
                }
                else
                {
                    ColorCalloutState.SelectedColor = Brushes.DarkGreen.Color;
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_TOPMOST);
                if (userParam != null)
                {
                    CbScreenTopMost.IsChecked = userParam.ParamValue == "1";
                }
                userParam = ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_SCALE);
                if (userParam != null)
                {
                    for (int i = 0; i < ComboScreenScale.Items.Count; i++)
                    {
                        var item = ComboScreenScale.Items[i] as ComboBoxItem;
                        if (item != null)
                        {
                            if (item.Tag.ToString() == userParam.ParamValue)
                            {
                                ComboScreenScale.SelectedItem = item;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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
            mIsOptSuccess = true;
            SaveConfig();
            if (mIsOptSuccess)
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
        }

        private void SaveConfig()
        {
            try
            {
                if (ListUserParams == null)
                {
                    ListUserParams = new List<UserParamInfo>();
                }
                UserParamInfo userParam =
                    ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCLOGINSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_VOCLOGINSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                var color = ColorVocLoginState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                    ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRLOGINSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_SCRLOGINSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorScrLoginState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                   ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRLOGINSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_VOCSCRLOGINSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorVocScrLoginState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                    ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCRECORDSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_VOCRECORDSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorVocRecordState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                   ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_SCRRECORDSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_SCRRECORDSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorScrRecordState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                  ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_VOCSCRRECORDSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_VOCSCRRECORDSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorVocScrRecordState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                    ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLINSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_CALLINSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorCallinState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                    ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_COLOR_CALLOUTSTATE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_COLOR_CALLOUTSTATE;
                    userParam.GroupID = S2102Consts.UP_GROUP_COLOR;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                color = ColorCalloutState.SelectedColor;
                userParam.ParamValue = color.ToString().Substring(3);
                userParam =
                   ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_TOPMOST);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_PLAYSCREEN_TOPMOST;
                    userParam.GroupID = S2102Consts.UP_GROUP_SCREEN;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                userParam.ParamValue = CbScreenTopMost.IsChecked == true ? "1" : "0";
                userParam =
                  ListUserParams.FirstOrDefault(p => p.ParamID == S2102Consts.UP_PLAYSCREEN_SCALE);
                if (userParam == null)
                {
                    userParam = new UserParamInfo();
                    userParam.UserID = CurrentApp.Session.UserID;
                    userParam.ParamID = S2102Consts.UP_PLAYSCREEN_SCALE;
                    userParam.GroupID = S2102Consts.UP_GROUP_SCREEN;
                    userParam.DataType = DBDataType.NVarchar;
                    userParam.SortID = 0;
                    ListUserParams.Add(userParam);
                }
                userParam.ParamValue = "0";
                var item = ComboScreenScale.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    userParam.ParamValue = item.Tag.ToString();
                }
                if (PageParent != null)
                {
                    PageParent.SetMyWaiterVisibility(true);
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => DoSaveConfig();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    if (PageParent != null)
                    {
                        PageParent.SetMyWaiterVisibility(false);
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoSaveConfig()
        {
            try
            {
                OperationReturn optReturn;
                if (ListUserParams == null || ListUserParams.Count <= 0) { return; }
                int count = ListUserParams.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSSaveUserParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(ListUserParams[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        mIsOptSuccess = false;
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    mIsOptSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                mIsOptSuccess = false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("2102100", "Monitor Option");
                }

                TabStateColor.Header = string.Format(" {0} ", CurrentApp.GetLanguageInfo("2102H001", "State Color"));
                TabScreenPlay.Header = string.Format(" {0} ", CurrentApp.GetLanguageInfo("2102H002", "Screen Play"));
                LbColorVocLoginState.Content = CurrentApp.GetLanguageInfo("2102102", "Voice Login State");
                LbColorVocRecordState.Content = CurrentApp.GetLanguageInfo("2102103", "Voice Record State");
                LbColorScrLoginState.Content = CurrentApp.GetLanguageInfo("2102106", "Screen Login State");
                LbColorScrRecordState.Content = CurrentApp.GetLanguageInfo("2102107", "Screen Record State");
                LbColorVocScrLoginState.Content = CurrentApp.GetLanguageInfo("2102108", "Voice and Screen Login State");
                LbColorVocScrRecordState.Content = CurrentApp.GetLanguageInfo("2102109", "Voice and Screen Record State");
                LbColorCallinState.Content = CurrentApp.GetLanguageInfo("2102104", "Callin State");
                LbColorCalloutState.Content = CurrentApp.GetLanguageInfo("2102105", "Callout State");
                CbScreenTopMost.Content = CurrentApp.GetLanguageInfo("2102111", "Screen player top most");
                LbScreenScale.Content = CurrentApp.GetLanguageInfo("2102112", "Screen player scale");
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");
            }catch{}
        }
    }
}
