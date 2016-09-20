//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9a9316ae-ef0d-4d1d-8ca2-8e512bad71c9
//        CLR Version:              4.0.30319.18408
//        Name:                     UCSelectSeat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCSelectSeat
//
//        created by Charley at 2016/6/14 16:07:08
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMPS4412.Models;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{
    /// <summary>
    /// UCSelectSeat.xaml 的交互逻辑
    /// </summary>
    public partial class UCSelectSeat
    {
        public UCRegionSeatSetting PageParent;
        public RegionInfo RegionInfo;
        public List<RegionSeatInfo> ListRegionSeats;
        public List<SeatInfo> ListAllSeatInfos;

        private ObservableCollection<CheckableSeatItem> mListCheckableSeatItems;

        private bool mIsInited;

        public UCSelectSeat()
        {
            InitializeComponent();

            mListCheckableSeatItems = new ObservableCollection<CheckableSeatItem>();

            Loaded += UCSelectSeat_Loaded;
            CbAll.Click += CbAll_Click;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            ListBoxSeatList.SelectionChanged += ListBoxSeatList_SelectionChanged;
        }

        void UCSelectSeat_Loaded(object sender, RoutedEventArgs e)
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
                ListBoxSeatList.ItemsSource = mListCheckableSeatItems;
                if (RegionInfo != null)
                {
                    TxtRegionName.Text = RegionInfo.Name;
                }
                CreateCheckabelSeatItems();
                InitSeatItemCheckState();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateCheckabelSeatItems()
        {
            try
            {
                mListCheckableSeatItems.Clear();
                if (ListAllSeatInfos == null) { return; }
                for (int i = 0; i < ListAllSeatInfos.Count; i++)
                {
                    var seatInfo = ListAllSeatInfos[i];

                    CheckableSeatItem item = new CheckableSeatItem();
                    item.SeatID = seatInfo.ObjID;
                    item.Name = seatInfo.Name;
                    item.IsChecked = false;
                    item.Info = seatInfo;
                    mListCheckableSeatItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSeatItemCheckState()
        {
            try
            {
                if (ListRegionSeats == null) { return; }
                for (int i = 0; i < mListCheckableSeatItems.Count; i++)
                {
                    var item = mListCheckableSeatItems[i];

                    var temp = ListRegionSeats.FirstOrDefault(s => s.SeatID == item.SeatID);
                    if (temp != null)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        private void SaveRegionSeats()
        {
            try
            {
                if (RegionInfo == null) { return; }
                List<RegionSeatInfo> listCheckedSeats = new List<RegionSeatInfo>();
                for (int i = 0; i < mListCheckableSeatItems.Count; i++)
                {
                    var item = mListCheckableSeatItems[i];
                    if (item.IsChecked)
                    {
                        var regionSeat = ListRegionSeats.FirstOrDefault(s => s.SeatID == item.SeatID);
                        if (regionSeat == null)
                        {
                            regionSeat = new RegionSeatInfo();
                            regionSeat.RegionID = RegionInfo.ObjID;
                            regionSeat.SeatID = item.SeatID;
                            regionSeat.Creator = CurrentApp.Session.UserID;
                            regionSeat.CreateTime = DateTime.Now.ToUniversalTime();
                        }
                        regionSeat.Modifier = CurrentApp.Session.UserID;
                        regionSeat.ModifyTime = DateTime.Now.ToUniversalTime();
                        listCheckedSeats.Add(regionSeat);
                    }
                }
                int count = listCheckedSeats.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveRegionSeatInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(RegionInfo.ObjID.ToString());
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listCheckedSeats[i]);
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
                    CurrentApp.WriteLog("SaveRegionSeats", string.Format("{0}", webReturn.ListData[i]));
                }

                CurrentApp.WriteLog("SaveRegionSeats", "Save end.");
                ShowInformation(string.Format("Save region seat end."));

                if (PageParent != null)
                {
                    PageParent.Reload();
                }

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

        #endregion


        #region EventHandlers

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
            SaveRegionSeats();
        }

        void CbAll_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = CbAll.IsChecked == true;
            var items = ListBoxSeatList.SelectedItems;
            if (items.Count <= 0)
            {
                for (int i = 0; i < mListCheckableSeatItems.Count; i++)
                {
                    var item = mListCheckableSeatItems[i];
                    if (item != null)
                    {
                        item.IsChecked = isChecked;
                    }
                }
            }
            else
            {

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as CheckableSeatItem;
                    if (item != null)
                    {
                        item.IsChecked = isChecked;
                    }
                }
            }
        }

        void ListBoxSeatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = ListBoxSeatList.SelectedItems;
            bool tag = true;
            for (int i = 0; i < items.Count; i++)
            {
                var seatItem = items[i] as CheckableSeatItem;
                if (seatItem != null)
                {
                    if (seatItem.IsChecked != true)
                    {
                        tag = false;
                        break;
                    }
                }
            }
            CbAll.IsChecked = tag;
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
                    parent.Title = CurrentApp.GetLanguageInfo("4412019", "Selet Seat");
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                CbAll.Content = CurrentApp.GetLanguageInfo("4412020", "All");
            }
            catch { }
        }

        #endregion

    }
}
