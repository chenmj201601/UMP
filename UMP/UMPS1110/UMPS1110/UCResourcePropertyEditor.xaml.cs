//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5084acaa-a12e-4b2e-835d-5da2008f44d9
//        CLR Version:              4.0.30319.18444
//        Name:                     UCResourcePropertyEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                UCResourcePropertyEditor
//
//        created by Charley at 2015/1/16 11:49:25
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UMPS1110.Editors;
using UMPS1110.Models;
using UMPS1110.Models.ConfigObjects;
using UMPS1110.Wcf11101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;
using DatabaseInfo = UMPS1110.Models.DatabaseInfo;

namespace UMPS1110
{
    /// <summary>
    /// UCResourcePropertyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class UCResourcePropertyEditor
    {
        static UCResourcePropertyEditor()
        {
            PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs>), typeof(UCResourcePropertyEditor));
        }

        public UCResourcePropertyEditor()
        {
            InitializeComponent();

            mListEnumValueItems = new ObservableCollection<PropertyValueEnumItem>();

            Loaded += UCResourcePropertyEditor_Loaded;
        }

        void UCResourcePropertyEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }


        #region PropertyInfoItemProperty

        public static readonly DependencyProperty PropertyInfoItemProperty =
            DependencyProperty.Register("PropertyInfoItem", typeof(ResourcePropertyInfoItem), typeof(UCResourcePropertyEditor), new PropertyMetadata(default(UCResourcePropertyEditor)));

        public ResourcePropertyInfoItem PropertyInfoItem
        {
            get { return (ResourcePropertyInfoItem)GetValue(PropertyInfoItemProperty); }
            set { SetValue(PropertyInfoItemProperty, value); }
        }

        #endregion


        #region ConvertFormatProperty

        public static readonly DependencyProperty ConvertFormatProperty =
            DependencyProperty.Register("ConvertFormat", typeof(ObjectPropertyConvertFormat), typeof(UCResourcePropertyEditor), new PropertyMetadata(default(ObjectPropertyConvertFormat)));

        public ObjectPropertyConvertFormat ConvertFormat
        {
            get { return (ObjectPropertyConvertFormat)GetValue(ConvertFormatProperty); }
            set { SetValue(ConvertFormatProperty, value); }
        }

        #endregion


        #region ValueProperty

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(UCResourcePropertyEditor), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion


        #region TextProperty

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UCResourcePropertyEditor), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion


        #region IsLockedProperty

        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(UCResourcePropertyEditor), new PropertyMetadata(default(bool)));

        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        #endregion


        #region Members

        public ResourceMainView MainPage;

        private ConfigObject mConfigObject;
        private ObjectPropertyInfo mPropertyInfo;
        private ResourceProperty mPropertyValue;
        private ObservableCollection<PropertyValueEnumItem> mListEnumValueItems;

        #endregion


        #region Init and Load

        private void Init()
        {
            try
            {
                if (PropertyInfoItem != null)
                {
                    CurrentApp = PropertyInfoItem.CurrentApp;
                    PropertyInfoItem.Editor = this;
                    ObjectPropertyInfo propertyInfo = PropertyInfoItem.PropertyInfo;
                    mPropertyInfo = propertyInfo;
                    ResourceProperty propertyValue = PropertyInfoItem.ResourceProperty;
                    mPropertyValue = propertyValue;
                    ConfigObject configObject = PropertyInfoItem.ConfigObject;
                    mConfigObject = configObject;
                    if (propertyInfo != null)
                    {
                        ConvertFormat = propertyInfo.ConvertFormat;
                        IsLocked = propertyInfo.IsLocked;
                    }
                    if (propertyValue != null)
                    {
                        Value = propertyValue.Value;
                        if (mIPTextBox != null)
                        {
                            mIPTextBox.SetIP(Value);
                        }
                        if (mBorderPanel != null)
                        {
                            var editor = mBorderPanel.Child as IResourcePropertyEditor;
                            if (editor != null)
                            {
                                editor.RefreshValue();
                            }
                        }
                    }
                    ResourceMainView mainPage = PropertyInfoItem.MainPage;
                    if (mainPage != null)
                    {
                        MainPage = mainPage;
                    }
                }
                InitEnumValueItems();
                ShowValue();
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                ShowException("Editor Init:" + ex.Message);
            }
        }

        private void InitEnumValueItems()
        {
            mListEnumValueItems.Clear();
            switch (ConvertFormat)
            {
                case ObjectPropertyConvertFormat.YesNo:
                    InitYesNoEnumValueItems();
                    break;
                case ObjectPropertyConvertFormat.EnableDisable:
                    InitEnableDisableEnumValueItems();
                    break;
                case ObjectPropertyConvertFormat.MasterSlaver:
                    InitMasterSlaverEnumValueItems();
                    break;
                case ObjectPropertyConvertFormat.DisableMasterSlaver:
                    InitDisableMasterSlaverEnumValueItems();
                    break;
                case ObjectPropertyConvertFormat.BasicInfoSingleRadio:
                case ObjectPropertyConvertFormat.BasicInfoSingleSelect:
                case ObjectPropertyConvertFormat.BasicInfoMultiSelect:
                case ObjectPropertyConvertFormat.BasicInfoSingleEditSelect:
                    if (mPropertyInfo != null)
                    {
                        InitBasicInfoDataItems(mPropertyInfo.SourceID);
                    }
                    break;
                case ObjectPropertyConvertFormat.ComboSingleSelect:
                case ObjectPropertyConvertFormat.ComboMultiSelect:
                    //case ObjectPropertyConvertFormat.ComboSingleEditSelect:
                    InitComboSingleSelectDataItems();
                    break;
            }
        }

        private void InitYesNoEnumValueItems()
        {
            BasicInfoData info = new BasicInfoData();
            info.InfoID = 111000001;
            info.SortID = 0;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "1";
            PropertyValueEnumItem item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Yes");
            item.IsSelected = true;
            item.Info = info;
            mListEnumValueItems.Add(item);
            info = new BasicInfoData();
            info.InfoID = 111000001;
            info.SortID = 1;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "0";
            item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "No");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
        }

        private void InitEnableDisableEnumValueItems()
        {
            BasicInfoData info = new BasicInfoData();
            info.InfoID = 111000002;
            info.SortID = 0;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "1";
            PropertyValueEnumItem item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Enable");
            item.IsSelected = true;
            item.Info = info;
            mListEnumValueItems.Add(item);
            info = new BasicInfoData();
            info.InfoID = 111000002;
            info.SortID = 1;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "0";
            item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Disable");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
        }

        private void InitMasterSlaverEnumValueItems()
        {
            BasicInfoData info = new BasicInfoData();
            info.InfoID = 111000003;
            info.SortID = 0;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "1";
            PropertyValueEnumItem item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Master");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
            info = new BasicInfoData();
            info.InfoID = 111000003;
            info.SortID = 1;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "2";
            item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Slaver");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
        }

        private void InitDisableMasterSlaverEnumValueItems()
        {

            BasicInfoData info = new BasicInfoData();
            info.InfoID = 111000004;
            info.SortID = 0;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "0";
            PropertyValueEnumItem item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Disable");
            item.IsSelected = true;
            item.Info = info;
            mListEnumValueItems.Add(item);
            info = new BasicInfoData();
            info.InfoID = 111000004;
            info.SortID = 1;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "1";
            item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Master");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
            info = new BasicInfoData();
            info.InfoID = 111000004;
            info.SortID = 2;
            info.ParentID = 0;
            info.IsEnable = true;
            info.EncryptVersion = 0;
            info.Value = "2";
            item = new PropertyValueEnumItem();
            item.Value = info.Value;
            item.Display = CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                "Slaver");
            item.IsSelected = false;
            item.Info = info;
            mListEnumValueItems.Add(item);
        }

        private void InitBasicInfoDataItems(int infoID)
        {
            try
            {
                if (PropertyInfoItem == null || PropertyInfoItem.ListBasicInfoDatas == null) { return; }
                List<BasicInfoData> listItems = new List<BasicInfoData>();
                if (mPropertyInfo == null) { return; }

                #region 设置洲和国家

                if (infoID == S1110Consts.SOURCEID_COUNTRY)
                {
                    //特殊处理
                    //如果是获取国家列表，使用当前洲作为父级
                    if (mConfigObject != null)
                    {
                        ResourceProperty propertyValue =
                            mConfigObject.ListProperties.FirstOrDefault(
                                p => p.PropertyID == S1110Consts.PROPERTYID_CONTINENT);
                        if (propertyValue != null)
                        {
                            var infoItem =
                                PropertyInfoItem.ListBasicInfoDatas.FirstOrDefault(
                                    i => i.ParentID == S1110Consts.SOURCEID_CONTINENT && i.Value == propertyValue.Value);
                            if (infoItem != null)
                            {
                                listItems =
                                    PropertyInfoItem.ListBasicInfoDatas.Where(i => i.ParentID == infoItem.InfoID)
                                        .ToList();
                            }
                        }
                    }
                }
                else if (infoID == S1110Consts.SOURCEID_CONTINENT)
                {
                    //特殊处理
                    //如果是获取洲列表，使用111010000作为父级
                    listItems =
                               PropertyInfoItem.ListBasicInfoDatas.Where(
                                   i => i.ParentID == infoID).ToList();
                }

                #endregion

                else if (mPropertyInfo.ObjType == S1110Consts.RESOURCE_PBXDEVICE && mPropertyInfo.PropertyID == 14)
                {
                    listItems = PropertyInfoItem.ListBasicInfoDatas.Where(i => i.InfoID == infoID).ToList();
                    if (mConfigObject != null)
                    {
                        var pro12 = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                        if (pro12 != null)
                        {
                            if (pro12.Value == "12")
                            {
                                listItems = listItems.Where(i => i.SortID == 1 || i.SortID == 2).ToList();
                            }
                        }
                    }
                }
                else
                {
                    listItems = PropertyInfoItem.ListBasicInfoDatas.Where(i => i.InfoID == infoID).ToList();
                }
                for (int i = 0; i < listItems.Count; i++)
                {
                    BasicInfoData info = listItems[i];
                    if (!info.IsEnable) { continue; }
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                    item.Value = info.Value;
                    item.Display =
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                            info.Icon);
                    item.Info = info;
                    mListEnumValueItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException("InitBasicInfoDataItems:" + ex.Message);
            }
        }

        private void InitComboSingleSelectDataItems()
        {
            if (mPropertyInfo == null) { return; }
            InitMachineDataItems();
            switch (mPropertyInfo.ObjType)
            {
                case S1110Consts.RESOURCE_VOICESERVER:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //VoiceServer的DBBridgeIndex
                        case 16:
                            InitVoiceServerDBBridgeItems();
                            break;
                        case 190:
                        case 192:
                            InitVoiceServerStorageDeviceItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_DBBRIDGENAME:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //DBBridgeName的Index
                        case 11:
                            InitDBBridgeNameIndexDataItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_STORAGEDEVICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //StorageDevice的VoiceServer ID
                        case 12:
                            InitStorageDeviceMachineItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_RECOVERSERVER:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 11:
                            InitScreenServerSFTPUserItems();
                            break;
                        case 21:
                            InitVoiceServerDBBridgeItems();
                            break;
                        case 31:
                        case 41:
                            InitVoiceServerStorageDeviceItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_CHANNEL:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //协议序号
                        case 17:
                            InitChannelVoipProtocalItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_SCREENSERVER:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //连接的VoiceServer
                        case 11:
                            InitScreenServerVoiceServerItems();
                            break;
                        //DBBridge的ID
                        case 15:
                            InitScreenServerDBBridgeItems();
                            break;
                        //Sftp服务器
                        case 16:
                            InitScreenServerSFTPItems();
                            break;
                        //Sftp登录名
                        case 17:
                            InitScreenServerSFTPUserItems();
                            break;
                        case 21:
                        case 26:
                            InitScreenServerStorageDeviceItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_CMSERVERVOICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //CMServer VoiceConfig的VoiceID
                        case 11:
                            InitCMServerVoiceVoiceServerItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_ARCHIVESTRATEGY:
                     switch (mPropertyInfo.PropertyID)
                    {
                        //ArchiveStrategy的BindDevice
                        case 16:
                            InitArchiveStrategyStorageDeviceItems();
                            break;
                        //ArchiveStrategy的FilterCondition
                        case 21:
                            InitArchiveStrategryFilterConditionItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_BACKUPSTRATEGY:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //ArchiveStrategy的BindDevice
                        case 16:
                            InitArchiveStrategyStorageDeviceItems();
                            break;
                        //ArchiveStrategy的FilterCondition
                        case 21:
                            InitBackupStrategryFilterConditionItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_PBXDEVICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        //CTIType
                        case 12:
                            InitPBXDeviceCTITypeItems();
                            break;
                            //DeviceType
                        //case 13:
                        //    InitPBXDeviceDeviceType();
                        //    break;
                        //    //MonitorMode
                        //case 14:
                        //    InitPBXDeviceMonitorMode();
                        //    break;
                        case 31:
                            InitPBXDeviceCTIHubServerModuleNumber();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_DOWNLOADPARAM:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 12:
                            InitDownloadParamVoiceServerIDItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_ALARMMONITORPARAM:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 11:
                            InitAlarmMonitorParamServiceItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case SpeechAnalysisServiceObject.PRO_PCMDEVICE:
                        //case SpeechAnalysisServiceObject.PRO_ANALYSISSOURCEDEVICE:
                        //case SpeechAnalysisServiceObject.PRO_KEYWORDSOURCEDEVICE:
                        //case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTSOURCEDEVICE:
                        case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTTARGETDEVICE:
                            InitSpeechAnalysisStorageDevices();
                            break;
                    }
                    break;
                //px+
                case S1110Consts.RESOURCE_ALARMRECORDNUMBERDEVICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 14:
                            InitAlarmVoiceServerItems();
                            break;
                        case 13:
                            InitAlarmChannelItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_ALARMNORECORDCHECKDEVICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 11:
                            InitAlarmNoRecordChannelItems();
                            break;
                        case 12:
                            InitAlarmVoiceServerItems();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_ALARMHEALTHCHECK:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 11:
                            InitAlarmHealthCheckTime();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_CTICONNECTIONGROUP:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 31:
                            InitCTIGroupCTIHubServerModuleNumber();
                            break;
                    }
                    break;
                case S1110Consts.RESOURCE_ALARMSCREENNUMBERDEVICE:
                    switch (mPropertyInfo.PropertyID)
                    {
                        case 14:
                            InitAlarmScreenServerItems();
                            break;
                        case 13:
                            InitAlarmScreenChannelItems();
                            break;
                    }
                    break;
            }
        }

        #region 特殊处理

        private void InitMachineDataItems()
        {
            try
            {
                if (mPropertyValue == null) { return; }
                if (mPropertyValue.PropertyID == S1110Consts.PROPERTYID_MACHINE)
                {
                    if (PropertyInfoItem == null) { return; }
                    if (PropertyInfoItem.ListConfigObjects == null) { return; }
                    List<ConfigObject> listMachines =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_MACHINE).ToList();
                    for (int i = 0; i < listMachines.Count; i++)
                    {
                        ConfigObject machine = listMachines[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = machine.ObjectID.ToString();
                        item.Display = machine.Name;
                        item.Info = machine;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitMachineDataItems:" + ex.Message);
            }
        }

        private void InitDBBridgeNameIndexDataItems()
        {
            try
            {
                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.DBType = CurrentApp.Session.DatabaseInfo.TypeID;
                dbInfo.Host = CurrentApp.Session.DatabaseInfo.Host;
                dbInfo.Port = CurrentApp.Session.DatabaseInfo.Port;
                dbInfo.DBName = CurrentApp.Session.DatabaseInfo.DBName;
                PropertyValueEnumItem item = new PropertyValueEnumItem();
                item.Value = dbInfo.Token;
                item.Display = dbInfo.Token;
                item.Info = dbInfo;
                mListEnumValueItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException("InitDBBridgeNameIndexDataItems:" + ex.Message);
            }
        }

        private void InitVoiceServerDBBridgeItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_DBBRIDGE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitVoiceServerDBBridgeItems:" + ex.Message);
            }
        }

        private void InitVoiceServerStorageDeviceItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (mConfigObject == null) { return; }
                ServiceObject voice = mConfigObject as ServiceObject;
                if (voice == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_STORAGEDEVICE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        StorageDeviceObject storage = obj as StorageDeviceObject;
                        if (storage == null) { continue; }
                        if (storage.DeviceType != 0) { continue; }
                        if (storage.HostAddress != voice.HostAddress) { continue; }
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitVoiceServerStorageDeviceItems:" + ex.Message);
            }
        }

        private void InitStorageDeviceMachineItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_MACHINE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitStorageDeviceMachineItems:" + ex.Message);
            }
        }

        private void InitChannelVoipProtocalItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    if (mConfigObject == null) { return; }
                    var voice =
                        PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectID == mConfigObject.ParentID);
                    if (voice == null) { return; }
                    var objs =
                        voice.ListChildObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOIPPROTOCAL).ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitChannelVoipProtocalItems:" + ex.Message);
            }
        }

        private void InitScreenServerVoiceServerItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitScreenServerVoiceServerItems:" + ex.Message);
            }
        }

        private void InitScreenServerDBBridgeItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_DBBRIDGE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitScreenServerDBBridgeItems:" + ex.Message);
            }
        }

        private void InitScreenServerStorageDeviceItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (mConfigObject == null) { return; }
                ServiceObject screen = mConfigObject as ServiceObject;
                if (screen == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_STORAGEDEVICE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        StorageDeviceObject storage = obj as StorageDeviceObject;
                        if (storage == null) { continue; }
                        if (storage.DeviceType != 0) { continue; }
                        if (storage.HostAddress != screen.HostAddress) { continue; }
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitScreenServerStorageDeviceItems:" + ex.Message);
            }
        }

        private void InitScreenServerSFTPItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_SFTP)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitScreenServerSFTPItems:" + ex.Message);
            }
        }

        private void InitScreenServerSFTPUserItems()
        {
            try
            {
                if (PropertyInfoItem != null && PropertyInfoItem.ListSftpUsers != null)
                {
                    for (int i = 0; i < PropertyInfoItem.ListSftpUsers.Count; i++)
                    {
                        var user = PropertyInfoItem.ListSftpUsers[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Display = user.Account;
                        item.Description = user.FullName;
                        item.Value = string.Format("{0}|{1}", user.UserID, CurrentApp.Session.RentInfo.Token);
                        item.Info = user;
                        var temp = mListEnumValueItems.FirstOrDefault(e => e.Value == item.Value);
                        if (temp == null)
                        {
                            mListEnumValueItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitScreenServerSFTPUserItems:" + ex.Message);
            }
        }

        private void InitCMServerVoiceVoiceServerItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitCMServerVoiceVoiceServerItems:" + ex.Message);
            }
        }

        private void InitArchiveStrategyStorageDeviceItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_STORAGEDEVICE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitArchiveStrategyStorageDeviceItems:" + ex.Message);
            }
        }

        private void InitArchiveStrategryFilterConditionItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1110Codes.GetResouceObjectList;
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(S1110Consts.RESOURCE_FILTERCONDITION.ToString());
                webRequest.ListData.Add("0");
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
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
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    //只绑定归档策略的筛选条件
                    if (info.Other02 != "1") { continue; }
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                    item.Value = info.ObjID.ToString();
                    item.Display = info.Name;
                    item.Info = info;
                    mListEnumValueItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException("InitArchiveStrategryFilterConditionItems:" + ex.Message);
            }
        }

        private void InitBackupStrategryFilterConditionItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1110Codes.GetResouceObjectList;
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(S1110Consts.RESOURCE_FILTERCONDITION.ToString());
                webRequest.ListData.Add("0");
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
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
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    //只绑定回删策略的筛选条件
                    if (info.Other02 != "3") { continue; }
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                    item.Value = info.ObjID.ToString();
                    item.Display = info.Name;
                    item.Info = info;
                    mListEnumValueItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException("InitArchiveStrategryFilterConditionItems:" + ex.Message);
            }
        }

        private void InitPBXDeviceCTITypeItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        ResourceProperty propertyValue = obj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                        if (propertyValue != null)
                        {
                            string ctiType = propertyValue.Value;
                            PropertyValueEnumItem item = new PropertyValueEnumItem();
                            item.Value = ctiType;
                            item.Display = obj.Name;
                            item.Description = obj.Name;
                            item.Info = obj;
                            if (PropertyInfoItem.ListBasicInfoDatas != null)
                            {
                                var basicInfo =
                                    PropertyInfoItem.ListBasicInfoDatas.FirstOrDefault(
                                        b => b.InfoID == 111000300 && b.Value == ctiType);
                                if (basicInfo != null)
                                {
                                    item.Display = basicInfo.Icon;
                                }
                            }
                            mListEnumValueItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitPBXDeviceCTITypeItems:" + ex.Message);
            }
        }

        private void InitDownloadParamVoiceServerIDItems()
        {
            try
            {
                //固定项
                PropertyValueEnumItem defaultItem = new PropertyValueEnumItem();
                defaultItem.Value = "-1";
                defaultItem.Display = CurrentApp.GetLanguageInfo("1110105", "All Servers"); ;
                mListEnumValueItems.Add(defaultItem);

                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitDownloadParamVoiceServerIDItems:" + ex.Message);
            }
        }

        private void InitAlarmMonitorParamServiceItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_ALARMMONITOR)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmMonitorParamServiceItems:" + ex.Message);
            }
        }

        private void InitSpeechAnalysisStorageDevices()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (mConfigObject == null) { return; }
                ServiceObject service = mConfigObject as ServiceObject;
                if (service == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_STORAGEDEVICE)
                            .ToList();
                    int StrStorageDevType = 0;
                    //switch (mPropertyInfo.PropertyID)
                    //{
                    //    case SpeechAnalysisServiceObject.PRO_ANALYSISSOURCEDEVICE:
                    //        if (mConfigObject != null)
                    //        {
                    //            var TempProprty = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == SpeechAnalysisServiceObject.PRO_ANALYSISSOURCEACCESS);
                    //            if (TempProprty != null)
                    //            {
                    //                StrStorageDevType = Convert.ToInt32(TempProprty.Value);
                    //                MessageBox.Show(StrStorageDevType.ToString());
                    //            }
                    //        }
                    //        break;
                    //    case SpeechAnalysisServiceObject.PRO_KEYWORDSOURCEDEVICE:
                    //        if (mConfigObject != null)
                    //        {
                    //            var TempProprty = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == SpeechAnalysisServiceObject.PRO_KEYWORDSOURCEACCESS);
                    //            if (TempProprty != null)
                    //            {
                    //                StrStorageDevType = Convert.ToInt32(TempProprty.Value);
                    //            }
                    //        }
                    //        break;
                    //    case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTSOURCEDEVICE:
                    //        if (mConfigObject != null)
                    //        {
                    //            var TempProprty = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == SpeechAnalysisServiceObject.PRO_SPEECHTOTXTSOURCEACCESS);
                    //            if (TempProprty != null)
                    //            {
                    //                StrStorageDevType = Convert.ToInt32(TempProprty.Value);
                    //            }
                    //        }
                    //        break;
                    //    case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTTARGETDEVICE:
                    //        if (mConfigObject != null)
                    //        {
                    //            var TempProprty = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == SpeechAnalysisServiceObject.PRO_SPEECHTOTXTTARGETACCESS);
                    //            if (TempProprty != null)
                    //            {
                    //                StrStorageDevType = Convert.ToInt32(TempProprty.Value);
                    //            }
                    //        }
                    //        break;
                    //}
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        StorageDeviceObject storage = obj as StorageDeviceObject;
                        if (storage == null) { continue; }

                        if (storage.DeviceType == StrStorageDevType)
                        {
                            if (PropertyInfoItem.PropertyID != SpeechAnalysisServiceObject.PRO_PCMDEVICE
                                && PropertyInfoItem.PropertyID != SpeechAnalysisServiceObject.PRO_SPEECHTOTXTTARGETDEVICE) { continue; }
                        }
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitSpeechAnalysisStorageDevices:" + ex.Message);
            }
        }
        //px+
        private void InitAlarmVoiceServerItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmVoiceServerItems:" + ex.Message);
            }
        }
        private void InitAlarmScreenServerItems()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_SCREENSERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmScreenServerItems:" + ex.Message);
            }
        }
        private void InitAlarmScreenChannelItems()
        {
            //根据选中项，加载相应的分机信息
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    if (mConfigObject == null) { return; }
                    long ObjID;
                    var propertyTemp = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 14);
                    if (propertyTemp != null)
                    {
                        int IntValue = Convert.ToInt32(propertyTemp.Value);
                        var objsVoice =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_SCREENSERVER)
                            .ToList();
                        if (objsVoice.Count > IntValue)
                        {
                            ObjID = objsVoice[IntValue].ObjectID;
                        }
                        else
                        {
                            return;
                        }
                        var voice =
                            PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectID == ObjID);

                        if (voice == null) { return; }
                        var objs =
                            voice.ListChildObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_SCREENCHANNEL).ToList();
                        for (int i = 0; i < objs.Count; i++)
                        {
                            ConfigObject obj = objs[i];
                            ChannelObject ChanObj = obj as ChannelObject;
                            PropertyValueEnumItem item = new PropertyValueEnumItem();
                            if (ChanObj == null) { continue; }
                            item.Value = ChanObj.Extension;
                            item.Display = obj.Name;
                            item.Info = obj;
                            mListEnumValueItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmScreenChannelItems:" + ex.Message);
            }
        }
        private void InitAlarmChannelItems()
        {
            //根据选中项，加载相应的分机信息
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    if (mConfigObject == null) { return; }
                    long ObjID;
                    var propertyTemp = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 14);
                    if (propertyTemp != null)
                    {
                        int IntValue = Convert.ToInt32(propertyTemp.Value);
                        var objsVoice =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                        if (objsVoice.Count > IntValue)
                        {
                            ObjID = objsVoice[IntValue].ObjectID;
                        }
                        else
                        {
                            return;
                        }
                        var voice =
                            PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectID == ObjID);

                        if (voice == null) { return; }
                        var objs =
                            voice.ListChildObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CHANNEL).ToList();
                        for (int i = 0; i < objs.Count; i++)
                        {
                            ConfigObject obj = objs[i];
                            ChannelObject Chanobj = obj as ChannelObject;
                            PropertyValueEnumItem item = new PropertyValueEnumItem();
                            if (Chanobj == null) { continue; }
                            item.Value = Chanobj.Extension;
                            item.Display = obj.Name;
                            item.Info = obj;
                            mListEnumValueItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmChannelItems:" + ex.Message);
            }
        }

        private void InitAlarmNoRecordChannelItems()
        {
            //根据选中项，加载相应的分机信息
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    if (mConfigObject == null) { return; }
                    long ObjID;
                    var propertyTemp = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                    if (propertyTemp != null)
                    {
                        int IntValue = Convert.ToInt32(propertyTemp.Value);
                        var objsVoice =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER)
                            .ToList();
                        if (objsVoice.Count > IntValue)
                        {
                            ObjID = objsVoice[IntValue].ObjectID;
                        }
                        else
                        {
                            return;
                        }
                        var voice =
                            PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectID == ObjID);

                        if (voice == null) { return; }
                        var objs =
                            voice.ListChildObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CHANNEL).ToList();
                        for (int i = 0; i < objs.Count; i++)
                        {
                            ConfigObject obj = objs[i];
                            PropertyValueEnumItem item = new PropertyValueEnumItem();
                            item.Value = obj.ID.ToString();
                            item.Display = obj.Name;
                            item.Info = obj;
                            mListEnumValueItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmNoRecordChannelItems:" + ex.Message);
            }
        }

        private void InitAlarmHealthCheckTime()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    double TimeSec = 0;
                    for (int i = 0; i < 48; i++)
                    {
                        string StrTime = Converter.Second2Time(TimeSec);
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = StrTime;
                        item.Display = StrTime;
                        item.Info = TimeSec;
                        item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                        mListEnumValueItems.Add(item);
                        TimeSec += 1800;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("InitAlarmHealthCheckTime:" + ex.Message);
            }
        }
        //px-end
        private void InitPBXDeviceCTIHubServerModuleNumber()
        {
            try
            {
                PropertyValueEnumItem item0 = new PropertyValueEnumItem();
                item0.Value = "-1";
                item0.Display = CurrentApp.GetLanguageInfo("", "Default");
                item0.Info = null;
                mListEnumValueItems.Add(item0);
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CTIHUBSERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                ShowException("InitPBXDeviceCTIHubServerModuleNumber:" + ex.Message);
            }
        }

        private void InitCTIGroupCTIHubServerModuleNumber()
        {
            try
            {
                PropertyValueEnumItem item0 = new PropertyValueEnumItem();
                item0.Value = "-1";
                item0.Display = CurrentApp.GetLanguageInfo("", "Default");
                item0.Info = null;
                mListEnumValueItems.Add(item0);
                if (PropertyInfoItem == null) { return; }
                if (PropertyInfoItem.ListConfigObjects != null)
                {
                    var objs =
                        PropertyInfoItem.ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CTIHUBSERVER)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        PropertyValueEnumItem item = new PropertyValueEnumItem();
                        item.Value = obj.ID.ToString();
                        item.Display = obj.Name;
                        item.Info = obj;
                        mListEnumValueItems.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                ShowException("InitCTIGroupCTIHubServerModuleNumber:" + ex.Message);
            }
        }
        #endregion


        #endregion


        #region Others

        private void ShowValue()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                string strText;
                switch (ConvertFormat)
                {
                    case ObjectPropertyConvertFormat.YesNo:
                    case ObjectPropertyConvertFormat.EnableDisable:
                    case ObjectPropertyConvertFormat.MasterSlaver:
                    case ObjectPropertyConvertFormat.DisableMasterSlaver:
                    case ObjectPropertyConvertFormat.BasicInfoSingleRadio:
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].Value == Value)
                            {
                                mListEnumValueItems[i].IsSelected = true;
                            }
                        }
                        break;
                    case ObjectPropertyConvertFormat.BasicInfoSingleSelect:
                    case ObjectPropertyConvertFormat.ComboSingleSelect:
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].Value == Value)
                            {
                                if (mItemsSelectControlValue != null)
                                {
                                    mItemsSelectControlValue.SelectedIndex = i;
                                }
                            }
                        }
                        break;
                    case ObjectPropertyConvertFormat.BasicInfoSingleEditSelect:
                    case ObjectPropertyConvertFormat.ComboSingleEditSelect:
                        bool isFound = false;
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].Value == Value)
                            {
                                isFound = true;
                                if (mItemsSelectControlValue != null)
                                {
                                    mItemsSelectControlValue.SelectedIndex = i;
                                }
                            }
                        }
                        if (!isFound)
                        {
                            var combox = mItemsSelectControlValue as ComboBox;
                            if (combox != null)
                                combox.Text = Value;
                        }
                        break;
                    case ObjectPropertyConvertFormat.BasicInfoMultiSelect:
                        int intValue1, intValue2;
                        if (!int.TryParse(Value, out intValue1))
                        {
                            ShowException(string.Format("Not IntValue1"));
                            return;
                        }
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            var item = mListEnumValueItems[i];
                            if (!int.TryParse(item.Value, out intValue2))
                            {
                                ShowException(string.Format("Not IntValue2"));
                                return;
                            }
                            item.IsChecked = (intValue1 & intValue2) == intValue2;
                        }
                        strText = string.Empty;
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].IsChecked)
                            {
                                strText += string.Format("{0},", mListEnumValueItems[i].Display);
                            }
                        }
                        strText = strText.TrimEnd(new[] { ',' });
                        Text = strText;
                        break;
                    case ObjectPropertyConvertFormat.ComboMultiSelect:
                        string strValue = Value;
                        string[] arrValues = strValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (arrValues.Contains(mListEnumValueItems[i].Value))
                            {
                                mListEnumValueItems[i].IsChecked = true;
                            }
                        }
                        strText = string.Empty;
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].IsChecked)
                            {
                                strText += string.Format("{0},", mListEnumValueItems[i].Display);
                            }
                        }
                        strText = strText.TrimEnd(new[] { ',' });
                        Text = strText;
                        break;
                    case ObjectPropertyConvertFormat.Numeric2:
                        if (PropertyInfoItem == null) { return; }
                        if (mConfigObject == null) { return; }
                        if (mPropertyInfo == null) { return; }
                        int TempNumber = Convert.ToInt32(Value);
                        Text = (TempNumber / 3600).ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("ShowValue:" + ex.Message);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                if (mPropertyInfo == null) { return; }
                switch (mPropertyInfo.ObjType)
                {
                    case S1110Consts.RESOURCE_LICENSESERVER:
                        SetDefaultValue211();
                        break;
                    case S1110Consts.RESOURCE_ALARMSERVER:
                        SetDefaultValue218();
                        break;
                    case S1110Consts.RESOURCE_VOICESERVER:
                        SetDefaultValue221();
                        break;
                    case S1110Consts.RESOURCE_NETWORKCARD:
                        SetDefaultValue222();
                        break;
                    case S1110Consts.RESOURCE_CMSERVER:
                        SetDefaultValue236();
                        break;
                    case S1110Consts.RESOURCE_CMSERVERVOICE:
                        SetDefaultValue237();
                        break;
                    case S1110Consts.RESOURCE_CTICONNECTION:
                        SetDefaultValue241();
                        break;
                    case S1110Consts.RESOURCE_PBXDEVICE:
                        SetDefaultValue220();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("SetDefaultValue:" + ex.Message);
            }
        }

        public void RefreshValue()
        {
            Init();
        }

        public void ReloadItemData(PropertyValueEnumItem item)
        {
            try
            {
                if (item == null) { return; }
                if (mPropertyInfo != null)
                {
                    switch (mPropertyInfo.PropertyID)
                    {
                        //根据洲筛选国家
                        case S1110Consts.PROPERTYID_COUNTRY:
                            var parentItem = item.Info as BasicInfoData;
                            if (parentItem == null) { return; }
                            int parentID = parentItem.InfoID;
                            ReloadItemData(parentID);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("ReloadItemData:" + ex.Message);
            }
        }

        public void ReloadItemData(int infoID)
        {
            try
            {
                if (PropertyInfoItem == null || PropertyInfoItem.ListBasicInfoDatas == null) { return; }
                List<BasicInfoData> listItems;
                if (infoID >= 111010000 && infoID <= 111019999)
                {
                    //根据洲获取国家列表
                    listItems =
                               PropertyInfoItem.ListBasicInfoDatas.Where(
                                   i => i.ParentID == infoID).ToList();
                }
                else
                {
                    listItems =
                               PropertyInfoItem.ListBasicInfoDatas.Where(
                                   i => i.InfoID == infoID).ToList();
                }
                mListEnumValueItems.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    BasicInfoData info = listItems[i];
                    if (!info.IsEnable) { continue; }
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.IsCheckedChanged += EnumItem_IsCheckedChanged;
                    item.Value = info.Value;
                    item.Display =
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                            info.Icon);
                    item.Info = info;
                    mListEnumValueItems.Add(item);
                }
                if (mListEnumValueItems.Count > 0)
                {
                    if (mItemsSelectControlValue != null)
                    {
                        mItemsSelectControlValue.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (mPropertyValue != null)
                    {
                        mPropertyValue.Value = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("ReloadItemData" + ex.Message);
            }
        }


        #region 特殊处理

        private void SetDefaultValue211()
        {
            try
            {
                if (mPropertyInfo == null) { return; }
                switch (mPropertyInfo.PropertyID)
                {
                    //是否主服务器
                    case S1110Consts.PROPERTYID_MASTERSLAVER:
                        if (PropertyInfoItem.ListPropertyValues == null) { return; }
                        var temp = (from pvs in PropertyInfoItem.ListPropertyValues
                                    where pvs.ObjType == S1110Consts.RESOURCE_LICENSESERVER
                                    group pvs by pvs.ObjID).ToList();
                        int count = temp.Count;
                        //如果只有一台AlarmServer，那么这个服务器必须是主服务器,不能切换为备服务器
                        if (count == 1)
                        {
                            PropertyInfoItem.IsEnabled = false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("SetDefaultValue211:" + ex.Message);
            }
        }

        private void SetDefaultValue218()
        {
            try
            {
                if (PropertyInfoItem == null) { return; }
                switch (mPropertyInfo.PropertyID)
                {
                    //是否主服务器
                    case S1110Consts.PROPERTYID_MASTERSLAVER:
                        if (PropertyInfoItem.ListPropertyValues == null) { return; }
                        var temp = (from pvs in PropertyInfoItem.ListPropertyValues
                                    where pvs.ObjType == S1110Consts.RESOURCE_ALARMSERVER
                                    group pvs by pvs.ObjID).ToList();
                        int count = temp.Count;
                        //如果只有一台AlarmServer，那么这个服务器必须是主服务器,不能切换为备服务器
                        if (count == 1)
                        {
                            PropertyInfoItem.IsEnabled = false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("T218:" + ex.Message);
            }
        }

        private void SetDefaultValue222()
        {
            try
            {
                if (mPropertyValue != null)
                {
                    switch (mPropertyValue.PropertyID)
                    {
                        case 12:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("T222:" + ex.Message);
            }
        }

        private void SetDefaultValue221()
        {
            try
            {
                if (mPropertyValue != null)
                {
                    switch (mPropertyValue.PropertyID)
                    {
                        case 162:
                        case 163:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("221T:" + ex.Message);
            }
        }

        private void SetDefaultValue236()
        {
            try
            {
                if (mPropertyValue != null)
                {
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

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("236T:" + ex.Message);
            }
        }

        private void SetDefaultValue237()
        {
            try
            {
                if (mPropertyValue != null)
                {
                    switch (mPropertyValue.PropertyID)
                    {
                        case 16:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("237T:" + ex.Message);
            }
        }

        private void SetDefaultValue241()
        {
            /// <summary>
            /// CTIType
            /// 1:CTC
            /// 2:Genesys
            /// 3:Alcatel
            /// 4:CVCT
            /// 5:AES
            /// 6:AIC
            /// 7:Tadiran
            /// 8:CTI OS
            /// 9:NES OAI
            /// 10:Gerneral CSTA
            /// 11:CnCall
            /// 12:Gerneral Tapi
            /// 13:HuaweiICD
            /// 14:GeneralCSTA
            /// </summary>
            try
            {
                if (PropertyInfoItem == null) { return; }
                if (mConfigObject == null) { return; }
                if (mPropertyInfo == null) { return; }

                //获取所属的CTIConnectionGroup
                if (PropertyInfoItem.ListConfigObjects == null) { return; }
                var group = PropertyInfoItem.ListConfigObjects.FirstOrDefault(o => o.ObjectID == mConfigObject.ParentID) as CTIConnectionGroupObject;
                if (group == null) { return; }
                int ctiType = group.CTIType;
                switch (mPropertyInfo.PropertyID)
                {
                    case 11:
                        PropertyInfoItem.IsHidden =
                            !(ctiType == 1
                            || ctiType == 2
                            || ctiType == 5
                            || ctiType == 7
                            || ctiType == 9
                            || ctiType == 10
                            || ctiType == 11
                            || ctiType == 13
                            || ctiType == 14);
                        break;
                    case 12:
                        PropertyInfoItem.IsHidden =
                           !(ctiType == 2
                           || ctiType == 5
                           || ctiType == 7
                           || ctiType == 9
                           || ctiType == 10 
                           || ctiType == 13
                           || ctiType == 14);
                        break;
                    case 13:
                        PropertyInfoItem.IsHidden = !(ctiType == 5);
                        break;
                    case 14:
                        PropertyInfoItem.IsHidden = !(ctiType == 4 || ctiType == 5);
                        break;
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                        PropertyInfoItem.IsHidden = !(ctiType == 5);
                        break;
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                        PropertyInfoItem.IsHidden = !(ctiType == 6);
                        break;
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                        PropertyInfoItem.IsHidden = !(ctiType == 8);
                        break;
                    case 41:
                        PropertyInfoItem.IsHidden = !(ctiType == 1 || ctiType == 11);
                        break;
                    case 911:
                    case 912:
                        PropertyInfoItem.IsHidden =
                            !(ctiType == 1
                            || ctiType == 2
                            || ctiType == 4
                            || ctiType == 5
                            || ctiType == 6);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("241T:" + ex.Message);
            }
        }

        private void SetDefaultValue220()
        {
            try
            {
                if (mConfigObject == null) { return; }
                var pro12 = mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (pro12 == null) { return; }
                if (mPropertyValue != null)
                {
                    switch (mPropertyValue.PropertyID)
                    {
                        case 13:
                            if (pro12.Value == "12")
                            {
                                mPropertyValue.Value = "1";
                                Value = mPropertyValue.Value;
                                for (int i = 0; i < mListEnumValueItems.Count; i++)
                                {
                                    if (mListEnumValueItems[i].Value == Value)
                                    {
                                        if (mItemsSelectControlValue != null)
                                        {
                                            mItemsSelectControlValue.SelectedIndex = i;
                                        }
                                    }
                                }
                                IsEnabled = false;
                            }
                            else
                            {
                                IsEnabled = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("237T:" + ex.Message);
            }
        }

        #endregion

        #endregion

        #region Templates

        private const string PART_Panel = "PART_Panel";
        private const string PART_TextBlock = "PART_TextBlock";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_IntTextBox = "PART_IntTextBox";
        private const string PART_IPTextBox = "PART_IPTextBox";
        private const string PART_PasswordBox = "PART_PasswordBox";
        private const string PART_TimeTextBox = "PART_TimeTextBox";
        private const string PART_DateTimeTextBox = "PART_DateTimeTextBox";
        private const string PART_ItemsSelectControl = "PART_ItemsSelectControl";

        private Border mBorderPanel;
        private TextBlock mTextBlockValue;
        private AutoSelectTextBox mTextBoxValue;
        private IntegerUpDown mIntTextBoxValue;
        private IPv4AddressTextBox mIPTextBox;
        private PasswordBox mPasswordBoxValue;
        private TimePicker mTimeTextBox;
        private DateTimePicker mDateTimeTextBox;
        private Selector mItemsSelectControlValue;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {
                var editor = mBorderPanel.Child as IResourcePropertyEditor;
                if (editor != null)
                {
                    editor.PropertyValueChanged += Editor_PropertyValueChanged;
                }
            }
            mTextBlockValue = GetTemplateChild(PART_TextBlock) as TextBlock;
            if (mTextBlockValue != null)
            {

            }
            mTextBoxValue = GetTemplateChild(PART_TextBox) as AutoSelectTextBox;
            if (mTextBoxValue != null)
            {
                mTextBoxValue.TextChanged += mTextBoxValue_TextChanged;
            }
            mIntTextBoxValue = GetTemplateChild(PART_IntTextBox) as IntegerUpDown;
            if (mIntTextBoxValue != null)
            {
                mIntTextBoxValue.ValueChanged += mIntTextBoxValue_ValueChanged;
            }
            mIPTextBox = GetTemplateChild(PART_IPTextBox) as IPv4AddressTextBox;
            if (mIPTextBox != null)
            {
                mIPTextBox.ValueChanged += mIPTextBox_ValueChanged;
            }
            mPasswordBoxValue = GetTemplateChild(PART_PasswordBox) as PasswordBox;
            if (mPasswordBoxValue != null)
            {
                mPasswordBoxValue.Password = Value;
                mPasswordBoxValue.PasswordChanged += mPasswordBoxValue_PasswordChanged;
            }
            mTimeTextBox = GetTemplateChild(PART_TimeTextBox) as TimePicker;
            if (mTimeTextBox != null)
            {
                mTimeTextBox.ValueChanged += mTimeTextBox_ValueChanged;
            }
            mDateTimeTextBox = GetTemplateChild(PART_DateTimeTextBox) as DateTimePicker;
            if (mDateTimeTextBox != null)
            {
                mDateTimeTextBox.ValueChanged += mDateTimeTextBox_ValueChanged;
            }
            mItemsSelectControlValue = GetTemplateChild(PART_ItemsSelectControl) as Selector;
            if (mItemsSelectControlValue != null)
            {
                mItemsSelectControlValue.SelectionChanged += mItemsSelectControlValue_SelectionChanged;
                var combo = mItemsSelectControlValue as ComboBox;
                if (combo != null)
                {
                    combo.DropDownOpened += ComboBox_DropDownOpened;
                    combo.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(ComboBox_TextChanged));
                }
                mItemsSelectControlValue.ItemsSource = mListEnumValueItems;
                ShowValue();
                SetDefaultValue();
            }
        }

        #endregion


        #region EventHandlers

        void mTextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mPropertyValue != null
                && mTextBoxValue != null)
            {
                mPropertyValue.Value = mTextBoxValue.Text;

                RaisePropertyValueChangedEvent();
            }
        }

        void mIntTextBoxValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mPropertyValue != null
                && mIntTextBoxValue != null)
            {
                string value = mIntTextBoxValue.Value == null ? string.Empty : mIntTextBoxValue.Value.ToString();
                switch (mPropertyValue.ObjType)
                {
                    case S1110Consts.RESOURCE_ALARMSERVERPARAM:
                        switch (mPropertyValue.PropertyID)
                        {
                            case 23:
                            case 33:
                                int intValue;
                                if (int.TryParse(value, out intValue))
                                {
                                    value = (intValue * 3600).ToString(); ;
                                }
                                break;
                        }
                        break;
                }
                mPropertyValue.Value = value;

                RaisePropertyValueChangedEvent();
            }
        }

        void mIPTextBox_ValueChanged(string obj)
        {
            if (mPropertyValue != null
                && mIPTextBox != null)
            {
                mPropertyValue.Value = mIPTextBox.GetIP();

                RaisePropertyValueChangedEvent();
            }
        }

        void mPasswordBoxValue_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (mPropertyValue != null
                && mPasswordBoxValue != null)
            {
                mPropertyValue.Value = mPasswordBoxValue.Password;

                RaisePropertyValueChangedEvent();
            }
        }

        void mTimeTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mPropertyValue != null
                && mTimeTextBox != null)
            {
                mPropertyValue.Value = mTimeTextBox.Value == null
                    ? string.Empty : (DateTime.Parse(mTimeTextBox.Value.ToString())).ToString("HH:mm:ss");

                RaisePropertyValueChangedEvent();
            }
        }

        void mDateTimeTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mPropertyValue != null
                && mDateTimeTextBox != null)
            {
                mPropertyValue.Value = mTimeTextBox.Value == null
                    ? string.Empty : (DateTime.Parse(mTimeTextBox.Value.ToString())).ToString("yyyy-MM-dd HH:mm:ss");

                RaisePropertyValueChangedEvent();
            }
        }

        void mItemsSelectControlValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isInit = e.RemovedItems.Count == 0;        //此处判断是否第一次触发事件不太合理
            if (mItemsSelectControlValue != null)
            {
                var item = mItemsSelectControlValue.SelectedItem as PropertyValueEnumItem;
                if (item != null)
                {
                    mPropertyValue.Value = item.Value;
                }
                else
                {
                    mPropertyValue.Value = string.Empty;
                }
                if (mConfigObject != null)
                {
                    mConfigObject.SetPropertyValue(mPropertyValue.PropertyID, mPropertyValue.Value);
                }
                PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
                args.ConfigObject = mConfigObject;
                args.PropertyItem = PropertyInfoItem;
                args.PropertyInfo = mPropertyInfo;
                args.PropetyValue = mPropertyValue;
                args.Value = mPropertyValue.Value;
                args.ValueItem = item;
                args.IsInit = isInit;
                OnPropertyValueChanged(args);
            }
        }

        void EnumItem_IsCheckedChanged()
        {
            try
            {
                if (mPropertyInfo == null) { return; }
                string strText = string.Empty;
                switch (mPropertyInfo.ConvertFormat)
                {
                    case ObjectPropertyConvertFormat.BasicInfoMultiSelect:
                        int intValue = 0;
                        int intTemp;
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].IsChecked)
                            {
                                strText += string.Format("{0},", mListEnumValueItems[i].Display);

                                if (!int.TryParse(mListEnumValueItems[i].Value, out intTemp))
                                {
                                    continue;
                                }
                                intValue = (intValue | intTemp);
                            }
                        }
                        strText = strText.TrimEnd(new[] { ',' });
                        if (mPropertyValue != null)
                        {
                            mPropertyValue.Value = intValue.ToString();

                            RaisePropertyValueChangedEvent();
                        }
                        Text = strText;
                        break;
                    case ObjectPropertyConvertFormat.ComboMultiSelect:
                        string strValue = string.Empty;
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            if (mListEnumValueItems[i].IsChecked)
                            {
                                strText += string.Format("{0},", mListEnumValueItems[i].Display);
                                strValue += string.Format("{0};", mListEnumValueItems[i].Value);
                            }
                        }
                        strText = strText.TrimEnd(new[] { ',' });
                        strValue = strValue.TrimEnd(new[] { ';' });
                        if (mPropertyValue != null)
                        {
                            mPropertyValue.Value = strValue;

                            RaisePropertyValueChangedEvent();
                        }
                        Text = strText;
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("EnumItem_IsCheckedChanged:" + ex.Message);
            }
        }

        void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (mConfigObject != null)
            {
                switch (mConfigObject.ObjectType)
                {
                    case S1110Consts.RESOURCE_VOICESERVER:
                        if (mPropertyInfo != null)
                        {
                            switch (mPropertyInfo.PropertyID)
                            {
                                case 162:
                                case 163:
                                    //InitVoiceServerTellConfigNicItems();
                                    break;
                            }
                        }
                        break;
                    case S1110Consts.RESOURCE_NETWORKCARD:
                        if (mPropertyInfo != null)
                        {
                            switch (mPropertyInfo.PropertyID)
                            {
                                case 12:
                                    //InitNetworkCardNameItems();
                                    break;
                            }
                        }
                        break;
                    case S1110Consts.RESOURCE_CMSERVER:
                        if (mPropertyInfo != null)
                        {
                            switch (mPropertyInfo.PropertyID)
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
                                    //InitCMServerNicItems();
                                    break;
                            }
                        }
                        break;
                    case S1110Consts.RESOURCE_CMSERVERVOICE:
                        if (mPropertyInfo != null)
                        {
                            switch (mPropertyInfo.PropertyID)
                            {
                                case 16:
                                    //InitCMServerVoiceNicItems();
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (mItemsSelectControlValue != null)
                {
                    var item = mItemsSelectControlValue.SelectedItem as PropertyValueEnumItem;
                    if (item != null)
                    {
                        mPropertyValue.Value = item.Value;
                    }
                    else
                    {
                        var combox = mItemsSelectControlValue as ComboBox;
                        if (combox != null)
                            mPropertyValue.Value = combox.Text;
                    }
                    if (mConfigObject != null)
                    {
                        mConfigObject.SetPropertyValue(mPropertyValue.PropertyID, mPropertyValue.Value);
                    }
                    PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
                    args.ConfigObject = mConfigObject;
                    args.PropertyItem = PropertyInfoItem;
                    args.PropertyInfo = mPropertyInfo;
                    args.PropetyValue = mPropertyValue;
                    args.Value = mPropertyValue.Value;
                    args.ValueItem = item;
                    //args.IsInit = true;
                    OnPropertyValueChanged(args);
                }
            }
            catch (Exception ex)
            {
                ShowException("ComboBox_TextChanged:" + ex.Message);
            }
        }


        void Editor_PropertyValueChanged(object sender, RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> e)
        {
            OnPropertyValueChanged(e.NewValue);
        }

        #endregion


        #region PropertyValueChangedEvent

        public static readonly RoutedEvent PropertyValueChangedEvent;

        public event RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs> PropertyValueChanged
        {
            add { AddHandler(PropertyValueChangedEvent, value); }
            remove { RemoveHandler(PropertyValueChangedEvent, value); }
        }

        private void OnPropertyValueChanged(PropertyValueChangedEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> p =
                new RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs>(null, args);
            p.RoutedEvent = PropertyValueChangedEvent;
            RaiseEvent(p);
        }

        private void RaisePropertyValueChangedEvent()
        {
            if (mConfigObject != null)
            {
                if (mPropertyValue != null)
                {
                    mConfigObject.SetPropertyValue(mPropertyValue.PropertyID, mPropertyValue.Value);
                }
            }
            PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
            args.ConfigObject = mConfigObject;
            args.PropertyItem = PropertyInfoItem;
            args.PropertyInfo = mPropertyInfo;
            args.PropetyValue = mPropertyValue;
            if (mPropertyValue != null)
            {
                args.Value = mPropertyValue.Value;
            }
            OnPropertyValueChanged(args);
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            for (int i = 0; i < mListEnumValueItems.Count; i++)
            {
                PropertyValueEnumItem item = mListEnumValueItems[i];
                BasicInfoData basicInfoData = item.Info as BasicInfoData;
                if (basicInfoData != null)
                {
                    item.Display =
                        CurrentApp.GetLanguageInfo(
                            string.Format("BID{0}{1}", basicInfoData.InfoID, basicInfoData.SortID.ToString("000")),
                            basicInfoData.Icon);
                }
            }
        }

        #endregion

    }
}
