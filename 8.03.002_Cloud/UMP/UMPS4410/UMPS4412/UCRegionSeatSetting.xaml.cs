//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3066b27b-5e21-4c7a-be9a-dbe3d6611d8e
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionSeatSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCRegionSeatSetting
//
//        created by Charley at 2016/6/13 16:51:10
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS4412.Models;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS4412
{
    /// <summary>
    /// UCRegionSeatSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionSeatSetting
    {

        #region Members

        public RegionManageMainView PageParent;
        public ObjItem RegionItem;

        private bool mIsInited;

        private List<RegionSeatInfo> mListRegionSeats;
        private List<ImageButtonItem> mListToolBtnItems;
        private List<SeatInfo> mListAllSeatInfos;
        private ObservableCollection<RegionSeatItem> mListSeatItems;
        private RegionInfo mRegionInfo;
        private DragHelper mDragHelper;
        private RegionSeatItem mCurrentSeatItem;
        private double mViewerScale;

        #endregion


        public UCRegionSeatSetting()
        {
            InitializeComponent();

            mListRegionSeats = new List<RegionSeatInfo>();
            mListToolBtnItems = new List<ImageButtonItem>();
            mListSeatItems = new ObservableCollection<RegionSeatItem>();
            mListAllSeatInfos = new List<SeatInfo>();

            mDragHelper = new DragHelper();

            Loaded += UCRegionSeatSetting_Loaded;
            TxtSeatLeft.KeyDown += TxtSeatLeft_KeyDown;
            TxtSeatTop.KeyDown += TxtSeatTop_KeyDown;
            SliderMapScale.ValueChanged += SliderMapScale_ValueChanged;
        }

        void UCRegionSeatSetting_Loaded(object sender, RoutedEventArgs e)
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
                mDragHelper.Init(GridCurrentSeat, PanelCurrentSeat);
                mViewerScale = 1.0;
                if (RegionItem == null) { return; }
                mRegionInfo = RegionItem.Data as RegionInfo;
                if (mRegionInfo == null) { return; }
                TxtRegionName.Text = mRegionInfo.Name;
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

                InitToolBtnItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAllSeatInfos();
                    LoadRegionSeatInfo();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateToolBtnItems();
                    InitRegion();
                    CreateSeatItem();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAllSeatInfos()
        {
            try
            {
                mListAllSeatInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetSeatInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
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
                    optReturn = XMLHelper.DeserializeObject<SeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    SeatInfo info = optReturn.Data as SeatInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tSeatInfo is null"));
                        return;
                    }
                    mListAllSeatInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAllSeatInfos", string.Format("Load end.\t{0}", mListAllSeatInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionSeatInfo()
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

        private void InitToolBtnItems()
        {
            try
            {
                mListToolBtnItems.Clear();
                ImageButtonItem item = new ImageButtonItem();
                item.Name = "BtnAddSeat";
                item.Display = "AddSeat";
                item.ToolTip = "Add Seat";
                item.Icon = "Images/00007.png";
                mListToolBtnItems.Add(item);
                item = new ImageButtonItem();
                item.Name = "BtnArrangeSeat";
                item.Display = "ArrangeSeat";
                item.ToolTip = "Arrange Seat";
                item.Icon = "Images/00011.png";
                mListToolBtnItems.Add(item);
                item = new ImageButtonItem();
                item.Name = "BtnSaveMap";
                item.Display = "SaveMap";
                item.ToolTip = "Save Region Map";
                item.Icon = "Images/00010.png";
                mListToolBtnItems.Add(item);
                item = new ImageButtonItem();
                item.Name = "BtnClose";
                item.Display = "BtnClose";
                item.ToolTip = "Close";
                item.Icon = "Images/00009.png";
                mListToolBtnItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateToolBtnItems()
        {
            try
            {
                PanelToolBtn.Children.Clear();
                for (int i = 0; i < mListToolBtnItems.Count; i++)
                {
                    var item = mListToolBtnItems[i];
                    Button btn = new Button();
                    btn.Click += ToolBtn_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "ToolBtnStyle");
                    PanelToolBtn.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRegion()
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
                    item.PageParent = this;
                    item.CurrentApp = CurrentApp;
                    item.Info = regionSeat;
                    item.RegionID = regionID;
                    item.SeatID = regionSeat.SeatID;
                    var temp = mListAllSeatInfos.FirstOrDefault(s => s.ObjID == regionSeat.SeatID);
                    if (temp != null)
                    {
                        item.SeatName = temp.Name;
                        item.SeatInfo = temp;
                    }
                    item.Left = regionSeat.Left;
                    item.Top = regionSeat.Top;
                    mListSeatItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSeatInfo()
        {
            try
            {
                if (mCurrentSeatItem == null) { return; }
                TxtSeatName.Text = mCurrentSeatItem.SeatName;
                TxtSeatLeft.Text = mCurrentSeatItem.Left.ToString();
                TxtSeatTop.Text = mCurrentSeatItem.Top.ToString();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateSeatItem()
        {
            try
            {
                PanelRegionMap.Children.Clear();
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];

                    UCDragableSeat uc = new UCDragableSeat();
                    uc.CurrentApp = CurrentApp;
                    uc.RegionSeatItem = item;
                    PanelRegionMap.Children.Add(uc);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddSeat()
        {
            try
            {
                UCSelectSeat uc = new UCSelectSeat();
                uc.PageParent = this;
                uc.RegionInfo = mRegionInfo;
                uc.ListRegionSeats = mListRegionSeats;
                uc.ListAllSeatInfos = mListAllSeatInfos;
                uc.CurrentApp = CurrentApp;
                string strTitle = string.Format("Select Seat");
                if (PageParent != null)
                {
                    PageParent.SelectSeat(uc, strTitle);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveRegionMap()
        {
            try
            {
                if (mRegionInfo == null) { return; }
                List<RegionSeatInfo> listRegionSeats = new List<RegionSeatInfo>();
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var item = mListSeatItems[i];
                    var info = item.Info;
                    if (info == null) { continue; }
                    info.Modifier = CurrentApp.Session.UserID;
                    info.ModifyTime = DateTime.Now.ToUniversalTime();
                    listRegionSeats.Add(info);
                }
                int count = listRegionSeats.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveRegionSeatInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(mRegionInfo.ObjID.ToString());
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listRegionSeats[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    CurrentApp.WriteLog("SaveRegionMap", string.Format("{0}", webReturn.ListData[i]));
                }

                ShowInformation(string.Format("Save end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ArrangeSeat()
        {
            try
            {
                double totalWidth = BorderRegionMap.ActualWidth;
                double totalHeight = BorderRegionMap.ActualHeight;
                double x = 0, y = 0;
                int count = mListSeatItems.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = mListSeatItems[i];
                    item.Left = (int)Math.Round(x);
                    item.Top = (int)Math.Round(y);
                    var info = item.Info;
                    if (info != null)
                    {
                        info.Left = item.Left;
                        info.Top = item.Top;
                    }
                    var panel = item.SeatPanel;
                    if (panel != null)
                    {
                        panel.SetPosition();
                    }
                    //基本的排列策略，更为复杂的排列策略以后实现
                    //每个座位是 80 * 80 ，座位与座位之间间隔5，从左向右，从上到下顺序排列
                    x += 80 + 5;
                    if (x + 80 + 5 >= totalWidth)
                    {
                        x = 0;
                        y += 80 + 5;
                    }
                    if (y + 80 + 5 >= totalHeight)
                    {
                        //空间不足
                        ShowException(string.Format("No enough area, some seat can not arrange!"));
                        return;
                    }
                }
                ShowInformation("Arrange seat end.");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Reload()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAllSeatInfos();
                    LoadRegionSeatInfo();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateToolBtnItems();
                    InitRegion();
                    CreateSeatItem();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Close()
        {
            try
            {
                if (PageParent != null)
                {
                    PageParent.CloseSeatSetting();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnItemFocused(RegionSeatItem item)
        {
            try
            {
                var current = mListSeatItems.FirstOrDefault(s => s.SeatID == item.SeatID);
                if (current == null) { return; }
                mCurrentSeatItem = current;
                for (int i = 0; i < mListSeatItems.Count; i++)
                {
                    var temp = mListSeatItems[i];
                    var panel = temp.SeatPanel;
                    if (panel != null)
                    {
                        if (temp.SeatID == current.SeatID)
                        {
                            panel.SetValue(Grid.ZIndexProperty, 10);
                        }
                        else
                        {
                            panel.SetValue(Grid.ZIndexProperty, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void OnItemMoved(RegionSeatItem item)
        {
            try
            {
                mCurrentSeatItem = item;
                InitSeatInfo();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        private void ToolBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null) { return; }
            var imageBtn = btn.DataContext as ImageButtonItem;
            if (imageBtn == null) { return; }
            string strName = imageBtn.Name;
            switch (strName)
            {
                case "BtnAddSeat":
                    AddSeat();
                    break;
                case "BtnClose":
                    Close();
                    break;
                case "BtnSaveMap":
                    SaveRegionMap();
                    break;
                case "BtnArrangeSeat":
                    ArrangeSeat();
                    break;
            }
        }

        void TxtSeatTop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (mCurrentSeatItem == null) { return; }
                string strValue = TxtSeatTop.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(string.Format("Input invalid!"));
                    return;
                }
                var panel = mCurrentSeatItem.SeatPanel;
                if (panel == null) { return; }
                mCurrentSeatItem.Top = intValue;
                var info = mCurrentSeatItem.Info;
                if (info != null)
                {
                    info.Top = mCurrentSeatItem.Top;
                }
                panel.SetPosition();
            }
        }

        void TxtSeatLeft_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (mCurrentSeatItem == null) { return; }
                string strValue = TxtSeatLeft.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(string.Format("Input invalid!"));
                    return;
                }
                var panel = mCurrentSeatItem.SeatPanel;
                if (panel == null) { return; }
                mCurrentSeatItem.Left = intValue;
                var info = mCurrentSeatItem.Info;
                if (info != null)
                {
                    info.Left = mCurrentSeatItem.Left;
                }
                panel.SetPosition();
            }
        }

        void SliderMapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int intValue;
                if (int.TryParse(SliderMapScale.Value.ToString(), out intValue))
                {
                    switch (intValue)
                    {
                        case 5:
                            mViewerScale = 0.1;
                            break;
                        case 10:
                            mViewerScale = 0.2;
                            break;
                        case 15:
                            mViewerScale = 0.3;
                            break;
                        case 20:
                            mViewerScale = 0.4;
                            break;
                        case 25:
                            mViewerScale = 0.5;
                            break;
                        case 30:
                            mViewerScale = 0.6;
                            break;
                        case 35:
                            mViewerScale = 0.7;
                            break;
                        case 40:
                            mViewerScale = 0.8;
                            break;
                        case 45:
                            mViewerScale = 0.9;
                            break;
                        case 50:
                            mViewerScale = 1.0;
                            break;
                        case 55:
                            mViewerScale = 1.5;
                            break;
                        case 60:
                            mViewerScale = 2.0;
                            break;
                        case 65:
                            mViewerScale = 2.5;
                            break;
                        case 70:
                            mViewerScale = 3.0;
                            break;
                        case 75:
                            mViewerScale = 3.5;
                            break;
                        case 80:
                            mViewerScale = 4.0;
                            break;
                        case 85:
                            mViewerScale = 4.5;
                            break;
                        case 90:
                            mViewerScale = 5.0;
                            break;
                        case 95:
                            mViewerScale = 5.5;
                            break;
                    }
                }
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = mViewerScale;
                tran.ScaleY = mViewerScale;
                BorderRegionMap.LayoutTransform = tran;
                SliderMapScale.Tag = mViewerScale;
                BindingExpression be = SliderMapScale.GetBindingExpression(ToolTipProperty);
                if (be != null)
                {
                    be.UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Changelanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                LbSeatLeft.Text = CurrentApp.GetLanguageInfo("4412017", "Left");
                LbSeatTop.Text = CurrentApp.GetLanguageInfo("4412018", "Top");
            }
            catch { }
        }

        #endregion

    }
}
