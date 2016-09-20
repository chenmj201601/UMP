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
            DependencyProperty.Register("MainPage", typeof (ResourceMainView), typeof (UCPropertyEditor420), new PropertyMetadata(default(ResourceMainView)));

        public ResourceMainView MainPage
        {
            get { return (ResourceMainView) GetValue(MainPageProperty); }
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
        private BackgroundWorker mWorker;

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
            try
            {
                string address = GetService00Address();
                if (string.IsNullOrEmpty(address))
                {
                    ShowException(string.Format("Server address is empty"));
                    return;
                }
                ServerRequestInfo requestInfo = new ServerRequestInfo();
                requestInfo.ServerHost = address;
                requestInfo.ServerPort = 8009;
                requestInfo.Command = (int)ServerRequestCommand.GetNetworkCardList;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(requestInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1110Codes.GetServerInfo;
                webRequest.Data = optReturn.Data.ToString();
                Service11102Client client = new Service11102Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11102"));
                WebReturn webReturn = null;
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("005", "Getting server networkcard information"));
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        webReturn = client.DoOperation(webRequest);
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false,string.Empty);
                    }
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
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        string info = webReturn.ListData[i];
                        string[] arrInfo = info.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length < 2) { continue; }
                        NetworkCardInfo card = new NetworkCardInfo();
                        card.ID = arrInfo[1];
                        card.Name = arrInfo[0];
                        card.Description = arrInfo[0];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = card.ID;
                        item.Display = card.Name;
                        item.Description = string.Format("{0}({1})", card.Name, card.ID);
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
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private string GetService00Address()
        {
            string strReturn = string.Empty;
            try
            {
                if (mConfigObject == null)
                {
                    return strReturn;
                }
                if (mPropertyValue == null)
                {
                    return strReturn;
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
                                    strReturn = voice.HostAddress;
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
                                        strReturn = voice.HostAddress;
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
                                    strReturn = cmServer.HostAddress;
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
                                        strReturn = cmServer.HostAddress;
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
                strReturn = string.Empty;
            }
            return strReturn;
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
