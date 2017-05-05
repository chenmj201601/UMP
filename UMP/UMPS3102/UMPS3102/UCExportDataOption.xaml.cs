//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a8c7b5a0-4019-48cd-9a13-e6e3292b2de0
//        CLR Version:              4.0.30319.18444
//        Name:                     UCExportDataOption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCExportDataOption
//
//        created by Charley at 2014/11/20 10:18:03
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// UCExportDataOption.xaml 的交互逻辑
    /// </summary>
    public partial class UCExportDataOption
    {
        public QMMainView PageParent;

        private List<SettingInfo> mListSettingInfos;

        public UCExportDataOption()
        {
            InitializeComponent();

            mListSettingInfos = new List<SettingInfo>();

            Loaded += UCExportDataOption_Loaded;
        }

        void UCExportDataOption_Loaded(object sender, RoutedEventArgs e)
        {
            BtnApply.Click += BtnApply_Click;
            BtnClose.Click += BtnClose_Click;
            CbRemember.Click += CbRemember_Click;

            LoadUserSettingInfos();

            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            try
            {
                bool isRemember = false;
                SettingInfo settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_REMEMBER);
                if (settingInfo != null && settingInfo.StringValue == "1")
                {
                    isRemember = true;
                }
                CbRemember.IsChecked = isRemember;
                string strType = "1";
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_TYPE);
                if (settingInfo != null && isRemember)
                {
                    strType = settingInfo.StringValue;
                }
                RadioCurrentPage.IsChecked = strType == "2";
                RadioAllPage.IsChecked = strType == "3";
                RadioCurrentSelected.IsChecked = strType == "1";
                settingInfo =
                   mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_NOTSHOW);
                if (settingInfo != null && settingInfo.StringValue == "1" && isRemember)
                {
                    CbNotShow.IsChecked = true;
                }
                else
                {
                    CbNotShow.IsChecked = false;
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

        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SettingInfo> listSettingInfos = new List<SettingInfo>();
                SettingInfo settingInfo;
                string strType = "1";
                if (RadioCurrentSelected.IsChecked == true)
                {
                    strType = "1";
                }
                if (RadioCurrentPage.IsChecked == true)
                {
                    strType = "2";
                }
                if (RadioAllPage.IsChecked == true)
                {
                    strType = "3";
                }
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_TYPE;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 0;
                settingInfo.StringValue = strType;
                settingInfo.DataType = 2;
                listSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_REMEMBER;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 1;
                settingInfo.StringValue = CbRemember.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                listSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_NOTSHOW;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 2;
                settingInfo.StringValue = CbNotShow.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                listSettingInfos.Add(settingInfo);

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveUserSettingInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(listSettingInfos.Count.ToString());
                for (int i = 0; i < listSettingInfos.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(listSettingInfos[i]);
                    if (!optReturn.Result)  
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (PageParent != null)
                {
                    PageParent.ReloadUserSettings();
                    PageParent.ExportData();
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

        void CbRemember_Click(object sender, RoutedEventArgs e)
        {
            CbNotShow.IsEnabled = CbRemember.IsChecked == true;
            if (CbRemember.IsChecked == false)
            {
                CbNotShow.IsChecked = false;
            }
        }

        private void LoadUserSettingInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetUserSettingList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("310202");
                webRequest.ListData.Add(string.Empty);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SettingInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SettingInfo settingInfo = optReturn.Data as SettingInfo;
                    if (settingInfo == null)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\tSettingInfo is null"));
                        continue;
                    }
                    var temp =
                        mListSettingInfos.FirstOrDefault(
                            s => s.ParamID == settingInfo.ParamID && s.UserID == settingInfo.UserID);
                    if (temp == null)
                    {
                        mListSettingInfos.Add(settingInfo);
                    }
                    else
                    {
                        mListSettingInfos.Remove(temp);
                        mListSettingInfos.Add(settingInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                BtnApply.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("31021800", "Export data option");
                }
                RadioCurrentSelected.Content = CurrentApp.GetLanguageInfo("31021500", "Current selected records");
                RadioCurrentPage.Content = CurrentApp.GetLanguageInfo("31021501", "Current page");
                RadioAllPage.Content = CurrentApp.GetLanguageInfo("31021502", "All page");
                CbRemember.Content = CurrentApp.GetLanguageInfo("31021503", "Remember current settings");
                CbNotShow.Content = CurrentApp.GetLanguageInfo("31021504", "Not show next time");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
