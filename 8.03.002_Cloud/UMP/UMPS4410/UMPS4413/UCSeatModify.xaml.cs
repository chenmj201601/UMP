//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    94713975-5832-4542-b475-80679e379b5c
//        CLR Version:              4.0.30319.18408
//        Name:                     UCSeatModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4413
//        File Name:                UCSeatModify
//
//        created by Charley at 2016/6/8 11:16:28
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS4413.Models;
using UMPS4413.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4413
{
    /// <summary>
    /// UCSeatModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCSeatModify
    {

        public SeatManageMainView PageParent;
        public ObjItem SeatItem;
        public bool IsAdd;

        private bool mIsInited;

        public UCSeatModify()
        {
            InitializeComponent();

            Loaded += UCSeatModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            CbEqualName.Click += CbEqualName_Click;
        }


        void UCSeatModify_Loaded(object sender, RoutedEventArgs e)
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
                if (IsAdd)
                {
                    GridCount.Height = new GridLength();
                    TxtCount.Value = 1;
                    TxtName.Text = string.Empty;
                    TxtExtension.Text = string.Empty;
                    CbEqualName.IsChecked = false;
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    TxtLevel.Value = 0;
                    TxtDescription.Text = string.Empty;
                }
                else
                {
                    GridCount.Height = new GridLength(0);
                    if (SeatItem == null) { return; }
                    var seatInfo = SeatItem.Data as SeatInfo;
                    if (seatInfo == null) { return; }
                    TxtName.Text = seatInfo.Name;
                    TxtExtension.Text = seatInfo.Extension;
                    CbEqualName.IsChecked = false;
                    RadioStateEnable.IsChecked = seatInfo.State == 1;
                    RadioStateDisable.IsChecked = seatInfo.State != 1;
                    TxtLevel.Value = seatInfo.Level;
                    TxtDescription.Text = seatInfo.Description;
                }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void AddSeat()
        {
            try
            {
                if (TxtCount.Value == null)
                {
                    ShowException(string.Format("Count invalid"));
                    return;
                }
                int intCount = (int)TxtCount.Value;
                if (intCount <= 0 || intCount >= 10000)
                {
                    ShowException(string.Format("Count invalid"));
                    return;
                }
                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(string.Format("Name invalid"));
                    return;
                }
                string strExtension = TxtExtension.Text;
                if (string.IsNullOrEmpty(strExtension))
                {
                    ShowException(string.Format("Extension invalid"));
                    return;
                }
                bool isEnable = RadioStateEnable.IsChecked == true;
                if (TxtLevel.Value == null)
                {
                    ShowException(string.Format("Level invalid"));
                    return;
                }
                int intLevel = (int)TxtLevel.Value;
                if (intLevel < 0 || intLevel >= 10)
                {
                    ShowException(string.Format("Level invalid"));
                    return;
                }
                string strDescription = TxtDescription.Text;
                List<SeatInfo> listSeatInfos = new List<SeatInfo>();
                if (intCount == 1)
                {
                    SeatInfo seatInfo = new SeatInfo();
                    seatInfo.Name = strName;
                    seatInfo.Extension = strExtension;
                    seatInfo.State = isEnable ? 1 : 0;
                    seatInfo.Level = intLevel;
                    seatInfo.Description = strDescription;
                    seatInfo.Creator = CurrentApp.Session.UserID;
                    seatInfo.CreateTime = DateTime.Now.ToUniversalTime();
                    seatInfo.Modifier = CurrentApp.Session.UserID;
                    seatInfo.ModifyTime = DateTime.Now.ToUniversalTime();
                    listSeatInfos.Add(seatInfo);
                }
                else
                {
                    string strTemp = strName;
                    string strPreName = string.Empty;
                    int intLengthName = strTemp.Length;
                    int intNumberName;
                    while (!int.TryParse(strTemp, out intNumberName)
                        && !string.IsNullOrEmpty(strTemp))
                    {
                        string str = strTemp.Substring(0, 1);
                        strPreName += str;
                        strTemp = strTemp.Substring(1);
                        intLengthName--;
                    }
                    strTemp = strExtension;
                    string strPreExtension = string.Empty;
                    int intLengthExtension = strTemp.Length;
                    int intNumberExtension;
                    while (!int.TryParse(strTemp, out intNumberExtension)
                        && !string.IsNullOrEmpty(strTemp))
                    {
                        string str = strTemp.Substring(0, 1);
                        strPreExtension += str;
                        strTemp = strTemp.Substring(1);
                        intLengthExtension--;
                    }
                    for (int i = 0; i < intCount; i++)
                    {
                        string strNumber = (i + intNumberName).ToString();
                        strName = strPreName + strNumber.PadLeft(intLengthName, '0');
                        strNumber = (i + intNumberExtension).ToString();
                        strExtension = strPreExtension + strNumber.PadLeft(intLengthExtension, '0');
                        SeatInfo seatInfo = new SeatInfo();
                        seatInfo.Name = strName;
                        seatInfo.Extension = strExtension;
                        seatInfo.State = isEnable ? 1 : 0;
                        seatInfo.Level = intLevel;
                        seatInfo.Description = strDescription;
                        seatInfo.Creator = CurrentApp.Session.UserID;
                        seatInfo.CreateTime = DateTime.Now.ToUniversalTime();
                        seatInfo.Modifier = CurrentApp.Session.UserID;
                        seatInfo.ModifyTime = DateTime.Now.ToUniversalTime();
                        listSeatInfos.Add(seatInfo);
                    }
                }
                int count = listSeatInfos.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveSeatInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listSeatInfos[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}"));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
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
                        if (webReturn.ListData == null)
                        {
                            ShowException(string.Format("Fail. \tListData is null"));
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            CurrentApp.WriteLog("AddSeat", string.Format("{0}", webReturn.ListData[i]));
                        }
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
                        if (!isSuccess) { return; }
                        ShowInformation(string.Format("Add seats successful!"));

                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                        if (PageParent != null)
                        {
                            PageParent.Reload();
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

        private void ModifySeat()
        {
            try
            {
                if (SeatItem == null) { return;}
                long objID = SeatItem.ObjID;
                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(string.Format("Name invalid"));
                    return;
                }
                string strExtension = TxtExtension.Text;
                if (string.IsNullOrEmpty(strExtension))
                {
                    ShowException(string.Format("Extension invalid"));
                    return;
                }
                bool isEnable = RadioStateEnable.IsChecked == true;
                if (TxtLevel.Value == null)
                {
                    ShowException(string.Format("Level invalid"));
                    return;
                }
                int intLevel = (int)TxtLevel.Value;
                if (intLevel < 0 || intLevel >= 10)
                {
                    ShowException(string.Format("Level invalid"));
                    return;
                }
                string strDescription = TxtDescription.Text;
                SeatInfo seatInfo=new SeatInfo();
                seatInfo.ObjID = objID;
                seatInfo.Name = strName;
                seatInfo.Extension = strExtension;
                seatInfo.State = isEnable ? 1 : 0;
                seatInfo.Level = intLevel;
                seatInfo.Description = strDescription;
                seatInfo.Modifier = CurrentApp.Session.UserID;
                seatInfo.ModifyTime = DateTime.Now.ToUniversalTime();

                List<SeatInfo> listSeatInfos = new List<SeatInfo>();
                listSeatInfos.Add(seatInfo);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveSeatInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(seatInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
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
                        if (webReturn.ListData == null)
                        {
                            ShowException(string.Format("Fail. \tListData is null"));
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            CurrentApp.WriteLog("ModifySeat", string.Format("{0}", webReturn.ListData[i]));
                        }
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
                        if (!isSuccess) { return; }
                        ShowInformation(string.Format("Modify seat successful!"));

                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                        if (PageParent != null)
                        {
                            PageParent.Reload();
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


        #region EventHandlers

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
            if (IsAdd)
            {
                AddSeat();
            }
            else
            {
                ModifySeat();
            }
        }

        void CbEqualName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbEqualName.IsChecked == true)
                {
                    TxtExtension.Text = TxtName.Text;
                    TxtExtension.IsEnabled = false;
                }
                else
                {
                    TxtExtension.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguages

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (IsAdd)
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4413003", "Add Seat");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4413004", "Modify Seat");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbCount.Text = CurrentApp.GetLanguageInfo("4413021", "Count");
                LbName.Text = CurrentApp.GetLanguageInfo("4413005", "Seat Name");
                LbExtension.Text = CurrentApp.GetLanguageInfo("4413006", "Extension");
                LbState.Text = CurrentApp.GetLanguageInfo("4413007", "State");
                LbLevel.Text = CurrentApp.GetLanguageInfo("4413008", "Level");
                LbDescription.Text = CurrentApp.GetLanguageInfo("4413009", "Description");

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4413010001", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4413010000", "Disable");
                CbEqualName.Content = CurrentApp.GetLanguageInfo("4413011", "Equal Name");
            }
            catch { }
        }

        #endregion

    }
}
