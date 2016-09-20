//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    582294d3-0119-49e4-8ce6-24fe18e26649
//        CLR Version:              4.0.30319.18444
//        Name:                     UCPropertyEditor410
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Editors
//        File Name:                UCPropertyEditor410
//
//        created by Charley at 2015/3/13 11:21:24
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
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;

namespace UMPS1110.Editors
{
    /// <summary>
    /// CTI服务名称下拉选择配置器
    /// 展开下拉列表的时候向指定的Service00发出请求获取服务名
    /// </summary>
    public partial class UCPropertyEditor410 : IResourcePropertyEditor
    {

        #region DependencyProperty

        public static readonly DependencyProperty MainPageProperty =
            DependencyProperty.Register("MainPage", typeof(ResourceMainView), typeof(UCPropertyEditor410), new PropertyMetadata(default(ResourceMainView)));

        public ResourceMainView MainPage
        {
            get { return (ResourceMainView)GetValue(MainPageProperty); }
            set { SetValue(MainPageProperty, value); }
        }

        public static readonly DependencyProperty PropertyInfoItemProperty =
            DependencyProperty.Register("PropertyInfoItem", typeof(ResourcePropertyInfoItem), typeof(UCPropertyEditor410), new PropertyMetadata(default(ResourcePropertyInfoItem), OnPropertyInfoItemChanged));

        public ResourcePropertyInfoItem PropertyInfoItem
        {
            get { return (ResourcePropertyInfoItem)GetValue(PropertyInfoItemProperty); }
            set { SetValue(PropertyInfoItemProperty, value); }
        }

        private static void OnPropertyInfoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as UCPropertyEditor410;
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

        private ObservableCollection<PropertyValueEnumItem> mListCTIServiceNameItems;
        private ConfigObject mConfigObject;
        private ObjectPropertyInfo mPropertyInfo;
        private ResourceProperty mPropertyValue;
        private BackgroundWorker mWorker;

        #endregion

        public UCPropertyEditor410()
        {
            InitializeComponent();

            mListCTIServiceNameItems = new ObservableCollection<PropertyValueEnumItem>();

            Loaded += UCPropertyEditor410_Loaded;
            ComboServiceNames.DropDownOpened += ComboServiceNames_DropDownOpened;
            ComboServiceNames.SelectionChanged += ComboServiceNames_SelectionChanged;
            ComboServiceNames.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(ComboServiceNames_TextChanged));
        }

        void UCPropertyEditor410_Loaded(object sender, RoutedEventArgs e)
        {
            ComboServiceNames.ItemsSource = mListCTIServiceNameItems;
            Init();
        }


        #region Init and Load

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
                    mListCTIServiceNameItems.Clear();
                    CTIServiceNameInfo info = new CTIServiceNameInfo();
                    info.Name = mPropertyValue.Value;
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Value = info.Name;
                    item.Display = info.Name;
                    item.Description = info.Name;
                    item.Info = info;
                    mListCTIServiceNameItems.Add(item);
                    for (int i = 0; i < ComboServiceNames.Items.Count; i++)
                    {
                        var temp = ComboServiceNames.Items[i] as PropertyValueEnumItem;
                        if (temp != null)
                        {
                            if (temp.Value == item.Value)
                            {
                                ComboServiceNames.SelectedItem = temp;
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

        #endregion


        #region Others

        private void LoadCTIServiceNames()
        {
            try
            {
                if (PropertyInfoItem == null || PropertyInfoItem.ListConfigObjects == null) { return; }
                if (mConfigObject == null) { return; }
                var parentObj =
                    PropertyInfoItem.ListConfigObjects.FirstOrDefault(
                        o => o.ObjectID == mConfigObject.ParentID);
                if (parentObj == null) { return; }
                //Property 11 is CTI Type
                ResourceProperty propertyValue = parentObj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                if (propertyValue == null) { return; }
                //AES, CVCT 才有效
                if (propertyValue.Value != "4" && propertyValue.Value != "5")
                {
                    return;
                }
                string strAddress = GetService00Address();
                string strPBXAddress = GetPBXAddress();
                if (string.IsNullOrEmpty(strAddress)
                    || string.IsNullOrEmpty(strPBXAddress))
                {
                    ShowException(string.Format("Server address or PBX address is empty"));
                    return;
                }
                ServerRequestInfo requestInfo = new ServerRequestInfo();
                requestInfo.ServerHost = strAddress;
                requestInfo.ServerPort = 8009;
                requestInfo.Command = (int)ServerRequestCommand.GetCTIServiceName;
                requestInfo.ListData.Add(strPBXAddress);
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
                    MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("011", "Getting PBX serviceName..."));
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
                        MainPage.SetBusy(false, string.Empty);
                    }
                    if (webReturn == null) { return; }
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
                        string name = webReturn.ListData[i];
                        CTIServiceNameInfo info = new CTIServiceNameInfo();
                        info.Name = name;
                        var temp = mListCTIServiceNameItems.FirstOrDefault(t => t.Value == info.Name);
                        if (temp == null)
                        {
                            temp = new PropertyValueEnumItem();
                            temp.Value = info.Name;
                            temp.Display = info.Name;
                            temp.Description = info.Name;
                            temp.Info = info;
                            mListCTIServiceNameItems.Add(temp);
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

        private string GetService00Address()
        {
            string strReturn = string.Empty;
            if (PropertyInfoItem == null
                || PropertyInfoItem.ListConfigObjects == null
                || PropertyInfoItem.ListConfigObjects.Count <= 0)
            {
                return strReturn;
            }
            //获取一个CTIHub
            var ctiHub =
                PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectType == S1110Consts.RESOURCE_CTIHUBSERVER)
                    as ServiceObject;
            if (ctiHub == null)
            {
                return strReturn;
            }
            //Service00的地址就是CTIHub的地址
            strReturn = ctiHub.HostAddress;
            return strReturn;
        }

        private string GetPBXAddress()
        {
            string strReturn = string.Empty;
            if (mConfigObject == null) { return strReturn; }
            if (mConfigObject.ObjectType != S1110Consts.RESOURCE_CTICONNECTION)
            {
                return strReturn;
            }
            //Property 11 is CTIHost Address
            ResourceProperty propertyValue = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
            if (propertyValue == null
                || string.IsNullOrEmpty(propertyValue.Value)
                || propertyValue.Value == "0.0.0.0")
            {
                return strReturn;
            }
            strReturn = propertyValue.Value;
            return strReturn;
        }

        #endregion


        #region EventHandlers

        void ComboServiceNames_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                LoadCTIServiceNames();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ComboServiceNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ComboServiceNames.SelectedItem as PropertyValueEnumItem;
                if (item == null)
                {
                    string strName = ComboServiceNames.Text;
                    if (string.IsNullOrEmpty(strName)) { return; }
                    CTIServiceNameInfo info = new CTIServiceNameInfo();
                    info.Name = strName;
                    item = new PropertyValueEnumItem();
                    item.Value = info.Name;
                    item.Display = info.Name;
                    item.Description = info.Name;
                    item.Info = info;
                    //mListCTIServiceNameItems.Add(item);
                }
                if (mPropertyValue != null)
                {
                    mPropertyValue.Value = item.Value;
                }
                OnPropertyValueChanged();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ComboServiceNames_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var item = ComboServiceNames.SelectedItem as PropertyValueEnumItem;
                if (item == null)
                {
                    string strName = ComboServiceNames.Text;
                    if (string.IsNullOrEmpty(strName)) { return; }
                    CTIServiceNameInfo info = new CTIServiceNameInfo();
                    info.Name = strName;
                    item = new PropertyValueEnumItem();
                    item.Value = info.Name;
                    item.Display = info.Name;
                    item.Description = info.Name;
                    item.Info = info;
                    //mListCTIServiceNameItems.Add(item);
                }
                if (mPropertyValue != null)
                {
                    mPropertyValue.Value = item.Value;
                }
                OnPropertyValueChanged();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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
            var item = ComboServiceNames.SelectedItem as PropertyValueEnumItem;
            args.ValueItem = item;
            OnPropertyValueChanged(args);
        }

        #endregion

    }
}
