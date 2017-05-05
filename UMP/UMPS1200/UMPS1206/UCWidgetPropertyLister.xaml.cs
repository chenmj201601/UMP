//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    addc015d-202a-47a7-97f0-11cab3fa1161
//        CLR Version:              4.0.30319.18408
//        Name:                     UCWidgetPropertyLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                UCWidgetPropertyLister
//
//        created by Charley at 2016/5/3 17:35:26
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1206.Models;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Communications;

namespace UMPS1206
{
    /// <summary>
    /// UCWidgetPropertyLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCWidgetPropertyLister
    {
        public WidgetItem WidgetItem;

        private bool mIsInited;
        private List<WidgetPropertyInfo> mListWidgetPropertyInfos;
        private List<UserWidgetPropertyValue> mListWidgetPropertyValues;
        private ObservableCollection<WidgetPropertyItem> mListWidgetPropertyItems;


        public UCWidgetPropertyLister()
        {
            InitializeComponent();

            mListWidgetPropertyInfos = new List<WidgetPropertyInfo>();
            mListWidgetPropertyValues = new List<UserWidgetPropertyValue>();
            mListWidgetPropertyItems = new ObservableCollection<WidgetPropertyItem>();

            Loaded += UCWidgetPropertyLister_Loaded;
        }

        void UCWidgetPropertyLister_Loaded(object sender, RoutedEventArgs e)
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
                ListBoxWidgetProperty.ItemsSource = mListWidgetPropertyItems;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadWidgetPropertyInfos();
                    LoadUserWidgetPropertyValues();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitPropertyItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadWidgetPropertyInfos()
        {
            try
            {
                mListWidgetPropertyInfos.Clear();
                if (WidgetItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetWidgetPropertyInfoList;
                webRequest.ListData.Add(WidgetItem.WidgetID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
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
                    optReturn = XMLHelper.DeserializeObject<WidgetPropertyInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WidgetPropertyInfo info = optReturn.Data as WidgetPropertyInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("WidgetPropertyInfo is null"));
                        return;
                    }
                    mListWidgetPropertyInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadPropertyInfos", string.Format("End.\t{0}", mListWidgetPropertyInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserWidgetPropertyValues()
        {
            try
            {
                mListWidgetPropertyValues.Clear();
                if (WidgetItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserWidgetPropertyValueList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(WidgetItem.WidgetID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
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
                    optReturn = XMLHelper.DeserializeObject<UserWidgetPropertyValue>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserWidgetPropertyValue info = optReturn.Data as UserWidgetPropertyValue;
                    if (info == null)
                    {
                        ShowException(string.Format("WidgetPropertyInfo is null"));
                        return;
                    }
                    mListWidgetPropertyValues.Add(info);
                }

                CurrentApp.WriteLog("LoadPropertyValues", string.Format("End.\t{0}", mListWidgetPropertyValues.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPropertyItems()
        {
            try
            {
                mListWidgetPropertyItems.Clear();
                for (int i = 0; i < mListWidgetPropertyInfos.Count; i++)
                {
                    var info = mListWidgetPropertyInfos[i];
                    WidgetPropertyItem item = new WidgetPropertyItem();
                    item.WidgetID = info.WidgetID;
                    item.PropertyID = info.PropertyID;
                    item.Name = info.Name;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206WP{0}{1}", info.WidgetID, info.PropertyID.ToString("000")), info.Name);
                    item.Description = item.Display;
                    item.PropertyInfo = info;
                    GetItemDescription(item);

                    var propertyValue =
                        mListWidgetPropertyValues.FirstOrDefault(
                            p =>
                                p.WidgetID == item.WidgetID && p.UserID == CurrentApp.Session.UserID &&
                                p.PropertyID == item.PropertyID);
                    if (propertyValue == null)
                    {
                        propertyValue = new UserWidgetPropertyValue();
                        propertyValue.WidgetID = item.WidgetID;
                        propertyValue.UserID = CurrentApp.Session.UserID;
                        propertyValue.PropertyID = item.PropertyID;
                        propertyValue.Value01 = info.DefaultValue;
                        mListWidgetPropertyValues.Add(propertyValue);
                    }
                    item.PropertyValue = propertyValue;

                    item.CurrentApp = CurrentApp;
                    item.WidgetItem = WidgetItem;
                    item.ListAllPropertyItems = mListWidgetPropertyItems;
                    item.ListAllPropertyValues = mListWidgetPropertyValues;

                    mListWidgetPropertyItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void GetItemDescription(WidgetPropertyItem item)
        {
            try
            {
                var info = item.PropertyInfo;
                if (info == null) { return;}
                string str = CurrentApp.GetLanguageInfo(string.Format("1206WPD{0}{1}",info.WidgetID,info.PropertyID.ToString("000")), string.Empty);
                item.ShowDescription = !string.IsNullOrEmpty(str);
                item.Description = str;
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("GetItemDescription", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations

        public OperationReturn SaveWidgetPropertyValues()
        {
            OperationReturn optReturn=new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                int count = mListWidgetPropertyValues.Count;
                if (count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1200Codes.SaveUserWidgetPropertyValues;
                    webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    webRequest.ListData.Add(count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(mListWidgetPropertyValues[i]);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service12001Client client = new Service12001Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        optReturn.Data = webReturn.ListData;
                        return optReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListWidgetPropertyItems.Count; i++)
                {
                    var item = mListWidgetPropertyItems[i];
                    var info = item.PropertyInfo;
                    if (info == null) { continue;}
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206WP{0}{1}", info.WidgetID, info.PropertyID.ToString("000")), info.Name);
                    item.Description = CurrentApp.GetLanguageInfo(string.Format("1206WPD{0}{1}", info.WidgetID, info.PropertyID.ToString("000")), info.Name);
                    var editor = item.Editor;
                    if (editor != null)
                    {
                        editor.ChangeLanguage();
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
