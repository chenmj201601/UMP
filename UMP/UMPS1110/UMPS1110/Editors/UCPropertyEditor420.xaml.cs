//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    37baba7a-fc09-42b8-b8fa-9478f26bbe04
//        CLR Version:              4.0.30319.18444
//        Name:                     UCPropertyEditor420
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Editors
//        File Name:                UCPropertyEditor420
//
//        created by Charley at 2015/4/15 14:33:44
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
using UMPS1110.Models;
using UMPS1110.Models.ConfigObjects;
using UMPS1110.Wcf11102;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;

namespace UMPS1110.Editors
{
    /// <summary>
    /// 网卡下拉选择配置器
    /// 组合框下拉的时候向服务器请求获取网卡信息
    /// </summary>
    public partial class UCPropertyEditor420 : IResourcePropertyEditor
    {

        #region MainPageProperty

        public static readonly DependencyProperty MainPageProperty =
            DependencyProperty.Register("MainPage", typeof(ResourceMainView), typeof(UCPropertyEditor420), new PropertyMetadata(default(ResourceMainView)));

        public ResourceMainView MainPage
        {
            get { return (ResourceMainView)GetValue(MainPageProperty); }
            set { SetValue(MainPageProperty, value); }
        }

        #endregion


        #region PropertyInfoItemProperty

        //注意，经验告诉我，当PropertyInfoItem变化，应该重新加载Editor，否则Editor可能获取不到PropertyInfoItem

        public static readonly DependencyProperty PropertyInfoItemProperty =
            DependencyProperty.Register("PropertyInfoItem", typeof(ResourcePropertyInfoItem), typeof(UCPropertyEditor420), new PropertyMetadata(default(ResourcePropertyInfoItem), OnPropertyInfoItemChanged));

        public ResourcePropertyInfoItem PropertyInfoItem
        {
            get { return (ResourcePropertyInfoItem)GetValue(PropertyInfoItemProperty); }
            set { SetValue(PropertyInfoItemProperty, value); }
        }

        private static void OnPropertyInfoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as UCPropertyEditor420;
            if (editor != null)
            {
                editor.OnPropertyInfoItemChanged((ResourcePropertyInfoItem)e.OldValue,
                    (ResourcePropertyInfoItem)e.NewValue);
            }
        }

        public void OnPropertyInfoItemChanged(ResourcePropertyInfoItem oldValue, ResourcePropertyInfoItem newValue)
        {
            RefreshValue();
        }

        #endregion


        #region Members

        private ObservableCollection<PropertyValueEnumItem> mListNetworkCardItems;
        private ConfigObject mConfigObject;
        private ObjectPropertyInfo mPropertyInfo;
        private ResourceProperty mPropertyValue;

        #endregion


        public UCPropertyEditor420()
        {
            InitializeComponent();

            mListNetworkCardItems = new ObservableCollection<PropertyValueEnumItem>();

            Loaded += UCPropertyEditor420_Loaded;
            ComboNetworkCards.DropDownOpened += ComboNetworkCards_DropDownOpened;
            ComboNetworkCards.SelectionChanged += ComboNetworkCards_SelectionChanged;
        }

        void UCPropertyEditor420_Loaded(object sender, RoutedEventArgs e)
        {
            ComboNetworkCards.ItemsSource = mListNetworkCardItems;
            Init();
        }


        #region Init and Loaded

        private void Init()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                CurrentApp = PropertyInfoItem.CurrentApp;
                ConfigObject configObject = PropertyInfoItem.ConfigObject;
                if (configObject != null)
                {
                    mConfigObject = configObject;
                }
                ObjectPropertyInfo propertyInfo = PropertyInfoItem.PropertyInfo;
                if (propertyInfo != null)
                {
                    mPropertyInfo = propertyInfo;
                }
                ResourceProperty propertyValue = PropertyInfoItem.ResourceProperty;
                if (propertyValue != null)
                {
                    mPropertyValue = propertyValue;
                }
                if (mPropertyValue == null) { return; }
                if (!string.IsNullOrEmpty(mPropertyValue.Value))
                {
                    string cardID = mPropertyValue.Value;
                    string cardName = cardID;
                    if (mPropertyValue.ListOtherValues != null && mPropertyValue.ListOtherValues.Count > 0)
                    {
                        cardName = mPropertyValue.ListOtherValues[0];
                    }
                    NetworkCardInfo info = new NetworkCardInfo();
                    info.ID = cardID;
                    info.Name = cardName;
                    info.Description = string.Format("{0}({1})", cardName, cardID);
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Value = info.ID;
                    item.Display = info.Name;
                    item.Description = info.Description;
                    item.Info = info;
                    var temp = mListNetworkCardItems.FirstOrDefault(c => c.Value == cardID);
                    if (temp == null)
                    {
                        temp = item;
                        mListNetworkCardItems.Add(temp);
                    }
                    ComboNetworkCards.SelectedItem = temp;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadNetworkCardInfos()
        {
            List<string> listAddresses = GetService00Address();
            LoadNetworkCardInfos(listAddresses);
        }

        private void LoadNetworkCardInfos(List<string> listHostAddresses)
        {
            List<NetworkCardInfo> listCardInfo = new List<NetworkCardInfo>();
            WebRequest webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)S1110Codes.GetServerInfo;
            Service11102Client client = new Service11102Client(
                WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                WebHelper.CreateEndpointAddress(
                    CurrentApp.Session.AppServerInfo,
                    "Service11102"));
            bool isFail = true;
            string strMsg = string.Empty;
            if (MainPage != null)
            {
                MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("005", "Getting server networkcard information"));
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, de) =>
            {
                try
                {
                    for (int i = 0; i < listHostAddresses.Count; i++)
                    {
                        string address = listHostAddresses[i];
                        if (string.IsNullOrEmpty(address))
                        {
                            strMsg += string.Format("Server address is empty!;");
                            continue;
                        }
                        ServerRequestInfo requestInfo = new ServerRequestInfo();
                        requestInfo.ServerHost = address;
                        requestInfo.ServerPort = 8009;
                        requestInfo.Command = (int)ServerRequestCommand.GetNetworkCardList;
                        OperationReturn optReturn = XMLHelper.SeriallizeObject(requestInfo);
                        if (!optReturn.Result)
                        {
                            strMsg += string.Format("Fail.\t{0}\t{1}\t{2};", address, optReturn.Code, optReturn.Message);
                            continue;
                        }
                        webRequest.Data = optReturn.Data.ToString();
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg += string.Format("WSFail.\t{0}\t{1}\t{2};", address, webReturn.Code, webReturn.Message);
                            continue;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg += string.Format("ListData is null;");
                            continue;
                        }
                        for (int j = 0; j < webReturn.ListData.Count; j++)
                        {
                            string info = webReturn.ListData[j];
                            string[] arrInfo = info.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrInfo.Length < 2) { continue; }
                            NetworkCardInfo card = new NetworkCardInfo();
                            card.ID = arrInfo[1];
                            card.Name = string.Format("{0}[{1}]", arrInfo[0], address);
                            card.Description = string.Format("{0}[{1}]({2})", arrInfo[0],address, arrInfo[1]);
                            listCardInfo.Add(card);
                        }
                    }
                    isFail = false;
                }
                catch (Exception ex)
                {
                    isFail = true;
                    strMsg = ex.Message;
                }
            };
            worker.RunWorkerCompleted += (s, re) =>
            {
                worker.Dispose();

                if (MainPage != null)
                {
                    MainPage.SetBusy(false, string.Empty);
                }

                try
                {
                    if (isFail)
                    {
                        ShowException(strMsg);
                        return;
                    }
                    if (!string.IsNullOrEmpty(strMsg))
                    {
                        ShowException(strMsg);
                    }
                    for (int i = 0; i < listCardInfo.Count; i++)
                    {
                        var card = listCardInfo[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = card.ID;
                        item.Display = card.Name;
                        item.Description = card.Description;
                        item.Info = card;
                        var temp = mListNetworkCardItems.FirstOrDefault(e => e.Value == item.Value);
                        if (temp == null)
                        {
                            mListNetworkCardItems.Add(item);
                        }
                        else
                        {
                            temp.Display = item.Display;
                            temp.Description = item.Description;
                            temp.Info = item.Info;
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

        #endregion


        #region Others

        private List<string> GetService00Address()
        {
            List<string> listAddresses = new List<string>();
            try
            {
                if (mConfigObject == null)
                {
                    return listAddresses;
                }
                if (mPropertyValue == null)
                {
                    return listAddresses;
                }
                switch (mPropertyValue.ObjType)
                {
                    case S1110Consts.RESOURCE_VOICESERVER:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 143:
                            case 162:
                            case 163:
                                var voice = mConfigObject as ServiceObject;
                                if (voice != null)
                                {
                                    listAddresses.Add(voice.HostAddress);
                                }
                                break;
                        }
                        break;
                    case S1110Consts.RESOURCE_NETWORKCARD:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 12:
                                if (PropertyInfoItem.ListConfigObjects != null)
                                {
                                    var voice =
                                        PropertyInfoItem.ListConfigObjects.FirstOrDefault(
                                            o => o.ObjectID == mConfigObject.ParentID) as ServiceObject;
                                    if (voice != null)
                                    {
                                        listAddresses.Add(voice.HostAddress);
                                    }
                                    var cmServerObjs =
                                        PropertyInfoItem.ListConfigObjects.Where(
                                            o => o.ObjectType == S1110Consts.RESOURCE_CMSERVER).ToList();
                                    for (int i = 0; i < cmServerObjs.Count; i++)
                                    {
                                        var cmServer = cmServerObjs[i] as ServiceObject;
                                        if (cmServer == null) { continue;}
                                        string address = cmServer.HostAddress;
                                        if (!listAddresses.Contains(address))
                                        {
                                            listAddresses.Add(address);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    case S1110Consts.RESOURCE_CMSERVER:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 13:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                            case 28:
                            case 29:
                                var cmServer = mConfigObject as ServiceObject;
                                if (cmServer != null)
                                {
                                   listAddresses.Add(cmServer.HostAddress);
                                }
                                break;
                        }
                        break;
                    case S1110Consts.RESOURCE_CMSERVERVOICE:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 16:
                                if (PropertyInfoItem.ListConfigObjects != null)
                                {
                                    var cmServer =
                                        PropertyInfoItem.ListConfigObjects.FirstOrDefault(
                                            o => o.ObjectID == mConfigObject.ParentID) as ServiceObject;
                                    if (cmServer != null)
                                    {
                                        listAddresses.Add(cmServer.HostAddress);
                                    }
                                }
                                break;
                        }
                        break;
                    case S1110Consts.RESOURCE_CAPTURENETWORKCARD:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 11:
                                if (PropertyInfoItem.ListConfigObjects != null)
                                {
                                    var voice =
                                        PropertyInfoItem.ListConfigObjects.FirstOrDefault(
                                            o => o.ObjectID == mConfigObject.ParentID) as ServiceObject;
                                    if (voice != null)
                                    {
                                        listAddresses.Add(voice.HostAddress);
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return listAddresses;
        }

        #endregion


        #region EventHandlers

        void ComboNetworkCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems == null || e.AddedItems.Count <= 0) { return; }
                var item = e.AddedItems[0] as PropertyValueEnumItem;
                if (item == null) { return; }
                if (mPropertyValue == null) { return; }
                mPropertyValue.Value = item.Value;
                mPropertyValue.ListOtherValues.Clear();
                mPropertyValue.ListOtherValues.Add(item.Display);
                if (mConfigObject != null)
                {
                    mConfigObject.GetBasicPropertyValues();
                    mConfigObject.RefreshObjectItem();
                }
                OnPropertyValueChanged();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ComboNetworkCards_DropDownOpened(object sender, EventArgs e)
        {
            LoadNetworkCardInfos();
        }

        #endregion


        #region RefreshValue

        public void RefreshValue()
        {
            Init();
        }

        #endregion


        #region PropertyValueChanged

        public event RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;

        private void OnPropertyValueChanged(PropertyValueChangedEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> p =
               new RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs>(null, args);
            if (PropertyValueChanged != null)
            {
                PropertyValueChanged(this, p);
            }
        }

        private void OnPropertyValueChanged()
        {
            if (mConfigObject != null)
            {
                mConfigObject.GetBasicPropertyValues();
            }
            PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
            args.PropertyItem = PropertyInfoItem;
            args.ConfigObject = mConfigObject;
            args.PropertyInfo = mPropertyInfo;
            args.PropetyValue = mPropertyValue;
            if (mPropertyValue != null)
            {
                args.Value = mPropertyValue.Value;
            }
            var item = ComboNetworkCards.SelectedItem as PropertyValueEnumItem;
            args.ValueItem = item;
            OnPropertyValueChanged(args);
        }

        #endregion
    }
}
