//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    543906e9-d6c9-4f25-b541-246c0e471394
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCRegionModify
//
//        created by Charley at 2016/5/11 10:18:23
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using UMPS4412.Models;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{
    /// <summary>
    /// UCRegionModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionModify
    {

        #region Members

        public RegionManageMainView PageParent;
        public bool IsModify;
        public ObjItem RegionItem;

        private bool mIsInited;
        private ObservableCollection<RegionTypeItem> mListRegionTypes;

        #endregion


        public UCRegionModify()
        {
            InitializeComponent();

            mListRegionTypes = new ObservableCollection<RegionTypeItem>();

            Loaded += UCRegionModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCRegionModify_Loaded(object sender, RoutedEventArgs e)
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
                ComboRegionTypes.ItemsSource = mListRegionTypes;
                InitRegionTypes();

                if (RegionItem == null) { return; }
                if (IsModify)
                {
                    var info = RegionItem.Data as RegionInfo;
                    if (info == null) { return; }
                    TxtRegionName.Text = info.Name;
                    ComboRegionTypes.SelectedItem = mListRegionTypes.FirstOrDefault(r => r.Value == info.Type);
                    RadioStateEnable.IsChecked = info.State == 1;
                    RadioStateDisable.IsChecked = info.State == 0;
                    RadioDefaultYes.IsChecked = info.IsDefault;
                    RadioDefaultNo.IsChecked = !info.IsDefault;
                    TxtRegionWidth.Value = info.Width;
                    TxtRegionHeight.Value = info.Height;
                    TxtBgColor.SelectedColor = GetColorFromString(info.BgColor);
                    TxtBgImage.Text = info.BgImage;
                }
                else
                {
                    TxtRegionName.Text = string.Empty;
                    ComboRegionTypes.SelectedItem = mListRegionTypes.FirstOrDefault(r => r.Value == 0);
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    RadioDefaultYes.IsChecked = true;
                    RadioDefaultNo.IsChecked = false;
                    TxtRegionWidth.Value = 0;
                    TxtRegionHeight.Value = 0;
                    TxtBgColor.SelectedColor = Brushes.Transparent.Color;
                    TxtBgImage.Text = string.Empty;
                }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRegionTypes()
        {
            try
            {
                mListRegionTypes.Clear();
                RegionTypeItem item = new RegionTypeItem();
                item.Name = "WorkRegion";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", 0.ToString("000")), "Work Region");
                item.Value = 0;
                mListRegionTypes.Add(item);
                item = new RegionTypeItem();
                item.Name = "PlaceRegion";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", 1.ToString("000")), "Place Region");
                item.Value = 1;
                mListRegionTypes.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddRegion()
        {
            try
            {
                RegionInfo regionInfo = new RegionInfo();
                if (RegionItem == null) { return; }
                var parentRegion = RegionItem.Data as RegionInfo;
                if (parentRegion == null) { return; }
                regionInfo.ParentObjID = parentRegion.ObjID;
                if (string.IsNullOrEmpty(TxtRegionName.Text))
                {
                    ShowException(string.Format("Region name can not be empty."));
                    return;
                }
                regionInfo.Name = TxtRegionName.Text;
                var regionType = ComboRegionTypes.SelectedItem as RegionTypeItem;
                if (regionType == null)
                {
                    ShowException(string.Format("Region type invalid."));
                    return;
                }
                regionInfo.Type = regionType.Value;
                regionInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                regionInfo.IsDefault = RadioDefaultYes.IsChecked == true;
                int intValue = 0;
                if (TxtRegionWidth.Value != null && TxtRegionWidth.Value > 0)
                {
                    intValue = (int)TxtRegionWidth.Value;
                }
                regionInfo.Width = intValue;
                intValue = 0;
                if (TxtRegionHeight.Value != null && TxtRegionHeight.Value > 0)
                {
                    intValue = (int)TxtRegionHeight.Value;
                }
                regionInfo.Height = intValue;
                regionInfo.BgColor = TxtBgColor.SelectedColor.ToString();
                regionInfo.BgImage = TxtBgImage.Text;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(regionInfo);
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
                        webRequest.Code = (int)S4410Codes.SaveRegionInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
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
                        ShowInformation(string.Format("Add region end"));
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
                                RegionItem.IsExpanded = true;
                                PageParent.ReloadRegionInfo(RegionItem);
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

        private void ModifyRegion()
        {
            try
            {
                RegionInfo regionInfo = RegionItem.Data as RegionInfo;
                if (regionInfo == null) { return; }
                if (string.IsNullOrEmpty(TxtRegionName.Text))
                {
                    ShowException(string.Format("Region name can not be empty."));
                    return;
                }
                regionInfo.Name = TxtRegionName.Text;
                var regionType = ComboRegionTypes.SelectedItem as RegionTypeItem;
                if (regionType == null)
                {
                    ShowException(string.Format("Region type invalid."));
                    return;
                }
                regionInfo.Type = regionType.Value;
                regionInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                regionInfo.IsDefault = RadioDefaultYes.IsChecked == true;
                int intValue = 0;
                if (TxtRegionWidth.Value != null && TxtRegionWidth.Value > 0)
                {
                    intValue = (int)TxtRegionWidth.Value;
                }
                regionInfo.Width = intValue;
                intValue = 0;
                if (TxtRegionHeight.Value != null && TxtRegionHeight.Value > 0)
                {
                    intValue = (int)TxtRegionHeight.Value;
                }
                regionInfo.Height = intValue;
                regionInfo.BgColor = TxtBgColor.SelectedColor.ToString();
                regionInfo.BgImage = TxtBgImage.Text;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(regionInfo);
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
                        webRequest.Code = (int)S4410Codes.SaveRegionInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add(regionInfo.ObjID.ToString());
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
                        ShowInformation(string.Format("Modify region end"));
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
                                var parentItem = RegionItem.Parent as ObjItem;
                                if (parentItem != null)
                                {
                                    parentItem.IsExpanded = true;
                                    PageParent.ReloadRegionInfo(parentItem);
                                }
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
                ModifyRegion();
            }
            else
            {
                AddRegion();
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
                        parent.Title = CurrentApp.GetLanguageInfo("4412003", "Modify Region");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbRegionName.Text = CurrentApp.GetLanguageInfo("4412005", "Region Name");
                LbRegionType.Text = CurrentApp.GetLanguageInfo("4412006", "Region Type");
                LbRegionState.Text = CurrentApp.GetLanguageInfo("4412007", "State");
                LbIsDefault.Text = CurrentApp.GetLanguageInfo("4412008", "Is Default");
                LbRegionWidth.Text = CurrentApp.GetLanguageInfo("4412009", "Width");
                LbRegionHeight.Text = CurrentApp.GetLanguageInfo("4412010", "Height");
                LbBgColor.Text = CurrentApp.GetLanguageInfo("4412011", "Background Color");
                LbBgImage.Text = CurrentApp.GetLanguageInfo("4412012", "Background Image");

                for (int i = 0; i < mListRegionTypes.Count; i++)
                {
                    var item = mListRegionTypes[i];
                    int intValue = item.Value;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", intValue.ToString("000")),
                        intValue.ToString());
                }

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4412014001", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4412014000", "Disable");
                RadioDefaultYes.Content = CurrentApp.GetLanguageInfo("4412015001", "Yes");
                RadioDefaultNo.Content = CurrentApp.GetLanguageInfo("4412015000", "No");

            }
            catch { }
        }

        #endregion

    }
}
