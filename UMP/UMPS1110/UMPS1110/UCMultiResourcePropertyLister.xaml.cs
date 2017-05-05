//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    45d86ba3-756a-4e4d-82e3-2bdc602b56b7
//        CLR Version:              4.0.30319.18444
//        Name:                     UCMultiResourcePropertyLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                UCMultiResourcePropertyLister
//
//        created by Charley at 2015/4/6 13:12:22
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
using System.Windows.Data;
using System.Windows.Shell;
using UMPS1110.Models;
using UMPS1110.Models.ConfigObjects;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110
{
    /// <summary>
    /// UCMultiResourcePropertyLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCMultiResourcePropertyLister
    {
        static UCMultiResourcePropertyLister()
        {
            PropertyListerEventEvent = EventManager.RegisterRoutedEvent("PropertyListerEvent", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<PropertyListerEventEventArgs>),
                typeof(UCMultiResourcePropertyLister));
        }

        public UCMultiResourcePropertyLister()
        {
            InitializeComponent();

            mListResourcePropertyInfoItems = new ObservableCollection<ResourcePropertyInfoItem>();
            mListConfigObjects = new List<ConfigObject>();

            Loaded += UCMultiResourcePropertyLister_Loaded;
            Unloaded += UCMultiResourcePropertyLister_Unloaded;
            ListBoxPropertyList.SelectionChanged += ListBoxPropertyList_SelectionChanged;
            SizeChanged += UCResourcePropertyLister_SizeChanged;
        }


        #region NameWidthProperty

        public static readonly DependencyProperty NameWidthProperty =
            DependencyProperty.Register("NameWidth", typeof(double), typeof(UCMultiResourcePropertyLister), new PropertyMetadata(default(double)));

        public double NameWidth
        {
            get { return (double)GetValue(NameWidthProperty); }
            set { SetValue(NameWidthProperty, value); }
        }

        #endregion


        #region Members

        public ResourceMainView PageParent { get; set; }
        public MultiSelectedItem MultiSelectedItem { get; set; }
        public List<ObjectPropertyInfo> ListObjectPropertyInfos { get; set; }
        public List<ResourceProperty> ListPropertyValues { get; set; }
        public List<ResourceGroupParam> ListResourceGroupParams { get; set; }
        public List<ConfigObject> ListConfigObjects { get; set; }
        public List<BasicUserInfo> ListSftpUsers { get; set; }
        public List<BasicInfoData> ListBasicInfoDatas { get; set; }

        private ObservableCollection<ResourcePropertyInfoItem> mListResourcePropertyInfoItems;
        private List<ConfigObject> mListConfigObjects;
        private ConfigObject mBaseConfigObject;     //基准对象，通常是ConfigObject列表中的第一个对象
        private int mObjType;

        #endregion


        #region Loaded and Unloaded

        void UCMultiResourcePropertyLister_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxPropertyList.ItemsSource = mListResourcePropertyInfoItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBoxPropertyList.ItemsSource);
            if (view != null && view.GroupDescriptions != null)
            {
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            }
            Init();
        }

        void UCMultiResourcePropertyLister_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //卸载页面的时候检查一下参数的有效性
                var result = CheckConfig();
                if (!result.Result)
                {
                    string strMsg = string.Format("{0}", CurrentApp.GetMessageLanguageInfo("014", "Check config fail."));
                    ConfigObject configObject = result.ConfigObject;
                    if (configObject != null)
                    {
                        strMsg += string.Format("\r\n\r\n[{0}]{1}",
                            CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", configObject.ObjectType.ToString("000")),
                                configObject.ObjectType.ToString()), configObject.Name);

                        int propertyID = result.PropertyID;
                        if (propertyID > 0)
                        {
                            strMsg += string.Format("\r\n\r\n{0}",
                               CurrentApp.GetLanguageInfo(
                                   string.Format("PRO{0}{1}", configObject.ObjectType.ToString("000"),
                                       propertyID.ToString("000")), propertyID.ToString()));
                        }
                    }
                    strMsg += string.Format("\r\n\r\n{0}",
                        CurrentApp.GetMessageLanguageInfo(string.Format("014{0}", result.Code.ToString("000")),
                            string.Format("[{0}]{1}", result.Code, result.Message)));
                    ShowException(strMsg);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Init and Load

        private void Init()
        {
            try
            {
                double width = ActualWidth;
                if (!double.IsNaN(width))
                {
                    NameWidth = width * 2 / 5;
                }
                InitPropertyInfoItems();
                //InitPropertyValues();     //如果不注释掉，那么只要一多选对象就会把所有多选的对象赋为相同的属性值；如果必要，可将此行取消注释
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPropertyInfoItems()
        {
            try
            {
                if (MultiSelectedItem == null) { return; }
                mObjType = MultiSelectedItem.ObjType;

                ConfigObject baseConfigObject = null;
                for (int i = 0; i < MultiSelectedItem.ListObjectItems.Count; i++)
                {
                    var item = MultiSelectedItem.ListObjectItems[i];
                    var configObject = item.Data as ConfigObject;
                    if (configObject == null) { continue; }
                    if (i == 0)
                    {
                        baseConfigObject = configObject;
                    }
                    mListConfigObjects.Add(configObject);
                }
                mBaseConfigObject = baseConfigObject;

                mListResourcePropertyInfoItems.Clear();
                if (baseConfigObject == null) { return; }
                if (ListObjectPropertyInfos == null) { return; }
                if (ListResourceGroupParams == null) { return; }
                List<ResourceGroupParam> listGroups =
                    ListResourceGroupParams.Where(g => g.TypeID == mObjType && g.ParentGroup == 0).ToList();
                List<ObjectPropertyInfo> listPropertyInfos =
                  ListObjectPropertyInfos.Where(p => p.ObjType == mObjType).ToList();

                for (int i = 0; i < listPropertyInfos.Count; i++)
                {
                    ObjectPropertyInfo propertyInfo = listPropertyInfos[i];
                    if (!propertyInfo.IsParam) { continue; }
                    if (propertyInfo.BatchModify <= 0) { continue; }
                    ResourceProperty propertyValue =
                        baseConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                    if (propertyValue == null) { continue; }
                    ResourcePropertyInfoItem item = new ResourcePropertyInfoItem();
                    item.CurrentApp = CurrentApp;
                    item.IsEnabled = true;
                    item.IsProp6Enabled = true;
                    item.StrPropertyName =
                        CurrentApp.GetLanguageInfo(
                            string.Format("PRO{0}{1}", propertyInfo.ObjType.ToString("000"),
                                propertyInfo.PropertyID.ToString("000")), propertyInfo.Description);
                    item.GroupID = propertyInfo.GroupID;
                    item.SortID = propertyInfo.SortID;
                    ResourceGroupParam groupParam = listGroups.FirstOrDefault(g => g.GroupID == propertyInfo.GroupID);
                    if (groupParam == null)
                    {
                        if (propertyInfo.GroupID == 0)
                        {
                            item.GroupName = CurrentApp.GetLanguageInfo("1110GRP000000", "Basic");
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        item.GroupName =
                            CurrentApp.GetLanguageInfo(
                                string.Format("1110GRP{0}{1}", groupParam.TypeID.ToString("000"),
                                    groupParam.GroupID.ToString("000")), groupParam.Description);
                    }
                    item.ObjType = propertyInfo.ObjType;
                    item.PropertyID = propertyInfo.PropertyID;
                    item.IsKeyProperty = propertyInfo.IsKeyProperty;
                    item.ResourceProperty = propertyValue;
                    item.PropertyInfo = propertyInfo;
                    item.ListPropertyValues = ListPropertyValues;
                    item.ListConfigObjects = ListConfigObjects;
                    item.ListSftpUsers = ListSftpUsers;
                    item.ListBasicInfoDatas = ListBasicInfoDatas;
                    item.ConfigObject = baseConfigObject;
                    item.MainPage = PageParent;
                    mListResourcePropertyInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPropertyValues()
        {
            try
            {
                if (MultiSelectedItem == null) { return; }
                var baseConfigObject = mBaseConfigObject;
                if (baseConfigObject == null) { return; }
                if (ListObjectPropertyInfos == null) { return; }
                List<ObjectPropertyInfo> listPropertyInfos =
                    ListObjectPropertyInfos.Where(p => p.ObjType == mObjType).ToList();
                for (int i = 0; i < listPropertyInfos.Count; i++)
                {
                    ObjectPropertyInfo propertyInfo = listPropertyInfos[i];
                    if (!propertyInfo.IsParam) { continue; }
                    if (propertyInfo.BatchModify <= 0) { continue; }
                    ResourceProperty propertyValue =
                       baseConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                    if (propertyValue == null) { continue; }
                    for (int j = 0; j < MultiSelectedItem.ListObjectItems.Count; j++)
                    {
                        var item = MultiSelectedItem.ListObjectItems[j];
                        var thisConfigObject = item.Data as ConfigObject;
                        if (thisConfigObject == null) { continue; }
                        var temp =
                            thisConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                        if (temp != null)
                        {
                            //把BaseConfigObject的属性值赋给其他配置对象
                            temp.Value = propertyValue.Value;
                            temp.OriginalValue = propertyValue.OriginalValue;
                            for (int k = 0; k < propertyValue.ListOtherValues.Count; k++)
                            {
                                string strValue = propertyValue.ListOtherValues[k];
                                if (temp.ListOtherValues.Count <= k)
                                {
                                    temp.ListOtherValues.Add(strValue);
                                }
                                else
                                {
                                    temp.ListOtherValues[k] = strValue;
                                }
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


        #region EventHandler

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                NameWidth = NameWidth + e.HorizontalChange;
            }
            catch { }
        }

        void ListBoxPropertyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ListBoxPropertyList.SelectedItem as ResourcePropertyInfoItem;
            if (item != null)
            {
                if (MultiSelectedItem == null) { return; }
                PropertyItemChangedEventArgs args = new PropertyItemChangedEventArgs();
                args.ListConfigObjects.Clear();
                for (int i = 0; i < MultiSelectedItem.ListObjectItems.Count; i++)
                {
                    var objItem = MultiSelectedItem.ListObjectItems[i];
                    var obj = objItem.Data as ConfigObject;
                    if (obj == null) { continue; }
                    args.ListConfigObjects.Add(obj);
                }
                args.PropertyItem = item;
                //触发PropertyItemChanged事件
                PropertyListerEventEventArgs listerEventArgs = new PropertyListerEventEventArgs();
                listerEventArgs.Code = 1;
                listerEventArgs.Data = args;
                OnPropertyListerEvent(listerEventArgs);
            }
        }

        void UCResourcePropertyLister_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = ActualWidth;
            if (!double.IsNaN(width))
            {
                NameWidth = width * 2 / 5;
            }
        }

        private void UCResourcePropertyEditor_OnPropertyValueChanged(object sender, RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> e)
        {
            try
            {
                var args = e.NewValue;
                if (args != null)
                {
                    //共有的处理操作
                    PropertyValueChangedOperation(args);
                    ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                    //特殊的处理操作
                    switch (propertyInfo.ObjType)
                    {
                        case S1110Consts.RESOURCE_PBXDEVICE:
                            PropertyValueChangedOperation220(args);
                            break;
                        case S1110Consts.RESOURCE_VOICESERVER:
                            PropertyValueChangedOperation221(args);
                            break;
                        case S1110Consts.RESOURCE_VOIPPROTOCAL:
                            PropertyValueChangedOperation223(args);
                            break;
                        case S1110Consts.RESOURCE_CHANNEL:
                            PropertyValueChangedOperation225(args);
                            break;
                        case S1110Consts.RESOURCE_LICENSESERVER:
                            PropertyValueChangedOperation211(args);
                            break;
                        case S1110Consts.RESOURCE_STORAGEDEVICE:
                            PropertyValueChangedOperation214(args);
                            break;
                        case S1110Consts.RESOURCE_ALARMSERVER:
                            PropertyValueChangedOperation218(args);
                            break;
                        case S1110Consts.RESOURCE_SCREENSERVER:
                            PropertyValueChangedOperation231(args);
                            break;
                        case S1110Consts.RESOURCE_CMSERVER:
                            PropertyValueChangedOperation236(args);
                            break;
                        case S1110Consts.RESOURCE_FILEOPERATOR:
                            PropertyValueChangedOperation251(args);
                            break;
                        case S1110Consts.RESOURCE_ARCHIVESTRATEGY:
                            PropertyValueChangedOperation256(args);
                            break;
                        case S1110Consts.RESOURCE_BACKUPSTRATEGY:
                            PropertyValueChangedOperation258(args);
                            break;
                        case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:
                            PropertyValueChangedOperation281(args);
                            break;
                        case S1110Consts.RESOURCE_CTICONNECTION:
                            PropertyValueChangedOperation241(args);
                            break;
                        case S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION:
                            PropertyValueChangedOperation243(args);
                            break;
                        case S1110Consts.RESOURCE_ALARMEMAILSERVER:
                            PropertyValueChangedOperation271(args);
                            break;
                        case S1110Consts.RESOURCE_ALARMRECORDNUMBERDEVICE:
                            PropertyValueChangedOperation276(args);
                            break;
                        case S1110Consts.RESOURCE_ALARMNORECORDCHECKDEVICE:
                            PropertyValueChangedOperation279(args);
                            break;
                    }

                    //把其他对象的属性值同步修改
                    InitPropertyValues();

                    PropertyListerEventEventArgs listerEventArgs = new PropertyListerEventEventArgs();
                    listerEventArgs.Code = 2;
                    listerEventArgs.ListConfigObjects.Clear();
                    for (int i = 0; i < MultiSelectedItem.ListObjectItems.Count; i++)
                    {
                        var objItem = MultiSelectedItem.ListObjectItems[i];
                        var obj = objItem.Data as ConfigObject;
                        if (obj == null) { continue; }
                        listerEventArgs.ListConfigObjects.Add(obj);
                    }
                    listerEventArgs.Data = args;
                    OnPropertyListerEvent(listerEventArgs);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region PropertyValueChangedOperations

        private void PropertyValueChangedOperation(PropertyValueChangedEventArgs args)
        {
            //资源属性改变应做的统一操作
            ConfigObject configObject = args.ConfigObject;
            ObjectPropertyInfo propertyInfo = args.PropertyInfo;
            ResourceProperty propertyValue = args.PropetyValue;
            string strValue = args.Value;
            if (configObject == null) { return; }
            if (propertyValue == null) { return; }
            if (propertyInfo == null) { return; }
            ResourcePropertyInfoItem item;
            List<ResourcePropertyInfoItem> listItems;
            switch (propertyValue.PropertyID)
            {
                //启用，禁用
                case S1110Consts.PROPERTYID_ENABLEDISABLE:
                    for (int i = S1110Consts.PROPERTYID_ENABLEDISABLE + 1; i < 1000; i++)
                    {
                        item = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == i);
                        if (item != null)
                        {
                            item.IsProp6Enabled = strValue == "1";
                        }
                    }
                    break;
                //洲，国家联动
                case S1110Consts.PROPERTYID_CONTINENT:
                    item =
                        mListResourcePropertyInfoItems.FirstOrDefault(
                            p => p.PropertyID == S1110Consts.PROPERTYID_COUNTRY);
                    if (item != null)
                    {
                        var editor = item.Editor;
                        if (editor != null)
                        {
                            //editor.ReloadItemData(args.ValueItem);
                            editor.RefreshValue();
                        }
                    }
                    break;
                //所在机器
                case S1110Consts.PROPERTYID_MACHINE:
                    if (ListConfigObjects == null) { return; }
                    MachineObject machine = ListConfigObjects.FirstOrDefault(o => o.ObjectID.ToString() == strValue) as MachineObject;
                    if (machine == null) { return; }
                    ServiceObject service = configObject as ServiceObject;
                    if (service == null) { return; }
                    service.SetPropertyValue(S1110Consts.PROPERTYID_MACHINE, machine.ObjectID.ToString());
                    //刷新一次设计器
                    listItems =
                        mListResourcePropertyInfoItems.Where(
                            p =>
                                p.PropertyID != S1110Consts.PROPERTYID_MACHINE).ToList();
                    for (int i = 0; i < listItems.Count; i++)
                    {
                        var editor = listItems[i].Editor;
                        if (editor != null)
                        {
                            editor.RefreshValue();
                        }
                    }
                    break;
            }
        }

        private void PropertyValueChangedOperation220(PropertyValueChangedEventArgs args)
        {
            try
            {
                ResourceProperty propertyValue = args.PropetyValue;
                if (propertyValue.PropertyID != 12) { return; }
                if (args == null || args.ValueItem == null) { return; }
                ConfigObject configVoice = args.ValueItem.Info as ConfigObject;
                ResourcePropertyInfoItem item;
                if (configVoice == null) { return; }
                if (ListConfigObjects == null) { return; }
                item = mListResourcePropertyInfoItems.FirstOrDefault(
                                        p => p.PropertyID == 13);
                if (item != null && item.Editor != null)
                {
                    item.Editor.RefreshValue();
                }
                item = mListResourcePropertyInfoItems.FirstOrDefault(
                                       p => p.PropertyID == 14);
                if (item != null && item.Editor != null)
                {
                    item.Editor.RefreshValue();
                }
            }
            catch (Exception ex)
            {
                ShowException("PropertyValueChangedOperation220:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation221(PropertyValueChangedEventArgs args)
        {
            ConfigObject configObject = args.ConfigObject;
            ObjectPropertyInfo propertyInfo = args.PropertyInfo;
            ResourceProperty propertyValue = args.PropetyValue;
            string strValue = args.Value;
            ResourcePropertyInfoItem item1, item2;
            if (configObject == null) { return; }
            if (propertyValue == null) { return; }
            if (propertyInfo == null) { return; }
            switch (propertyValue.PropertyID)
            {
                //服务器角色
                case VoiceServiceObject.PRO_STANDBYROLE:
                    //主机地址
                    item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == VoiceServiceObject.PRO_MASTERADDRESS);
                    if (item1 != null)
                    {
                        item1.IsEnabled = strValue == "2";
                    }
                    //主机端口
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == VoiceServiceObject.PRO_MASTERPORT);
                    if (item2 != null)
                    {
                        item2.IsEnabled = strValue == "1" || strValue == "2";
                    }
                    break;
                //是否启用混合录音
                case VoiceServiceObject.PRO_ISMIXRECORD:
                    //MainNtidrvPath
                    item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == VoiceServiceObject.PRO_MAINNTIDRVPATH);
                    if (item1 != null)
                    {
                        item1.IsEnabled = strValue == "1";

                        var propertyPath = item1.ResourceProperty;
                        if (propertyPath != null)
                        {
                            if (strValue == "1" && string.IsNullOrEmpty(propertyPath.Value))
                            {
                                configObject.SetPropertyValue(VoiceServiceObject.PRO_MAINNTIDRVPATH, @".\Ntidrv_mix.dll");
                            }
                            if (strValue == "0")
                            {
                                configObject.SetPropertyValue(VoiceServiceObject.PRO_MAINNTIDRVPATH, string.Empty);
                            }
                        }
                        var editor = item1.Editor;
                        if (editor != null)
                        {
                            editor.RefreshValue();
                        }
                    }
                    break;
                //是否启用按键登录
                case VoiceServiceObject.PRO_ISDTMFLOGIN:
                    for (int i = 1; i <= 5; i++)
                    {
                        item1 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == VoiceServiceObject.PRO_ISDTMFLOGIN + i);
                        if (item1 != null)
                        {
                            item1.IsEnabled = strValue == "1";
                        }
                    }
                    break;
            }

        }

        private void PropertyValueChangedOperation223(PropertyValueChangedEventArgs args)
        {
            try
            {
                ConfigObject configObject = args.ConfigObject;
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                ResourcePropertyInfoItem item1;
                if (configObject == null) { return; }
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyInfo.PropertyID)
                {
                    //Voip协议类型
                    case VoipProtocalObject.PRO_CODE:
                        if (args.IsInit) { return; }        //如果是第一次触发事件，不做处理
                        int intValue;
                        //根据协议类型自动设置端口
                        if (int.TryParse(strValue, out intValue))
                        {
                            item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == VoipProtocalObject.PRO_PBXMAINPORT);
                            if (item1 != null)
                            {
                                var propertyPort = item1.ResourceProperty;
                                if (propertyPort != null)
                                {
                                    switch (intValue)
                                    {
                                        case 0:
                                        case 3:
                                        case 20:
                                        case 21:
                                        case 22:
                                        case 23:
                                        case 24:
                                        case 25:
                                        case 26:
                                        case 31:
                                        case 37:
                                        case 38:
                                        case 101:
                                        case 102:
                                        case 103:
                                        case 143:
                                            //propertyPort.Value = "0";
                                            break;
                                        case 1:
                                        case 27:
                                            propertyPort.Value = "5060";
                                            break;
                                        case 2:
                                        case 5:
                                        case 18:
                                        case 33:
                                            propertyPort.Value = "1720";
                                            break;
                                        case 4:
                                            propertyPort.Value = "32640";
                                            break;
                                        case 6:
                                            propertyPort.Value = "2748";
                                            break;
                                        case 8:
                                            propertyPort.Value = "4060";
                                            break;
                                        case 19:
                                            propertyPort.Value = "2000";
                                            break;
                                        case 28:
                                        case 29:
                                            propertyPort.Value = "2427";
                                            break;
                                        case 30:
                                        case 35:
                                        case 36:
                                            propertyPort.Value = "2727";
                                            break;
                                        case 32:
                                            propertyPort.Value = "2944";
                                            break;
                                        case 34:
                                            propertyPort.Value = "30000";
                                            break;
                                    }
                                }
                                var editor = item1.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("PropertyValueChangedOperation223:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation225(PropertyValueChangedEventArgs args)
        {
            ConfigObject configObject = args.ConfigObject;
            ObjectPropertyInfo propertyInfo = args.PropertyInfo;
            ResourceProperty propertyValue = args.PropetyValue;
            string strValue = args.Value;
            ResourcePropertyInfoItem item1, item2;
            if (configObject == null) { return; }
            if (propertyValue == null) { return; }
            if (propertyInfo == null) { return; }
            switch (propertyValue.PropertyID)
            {

                #region 启动方式

                //启动方式
                case 21:
                    //是否Voip启动方式，Voip启动方式时可以让Voip相关的配置项可用
                    bool isVoipStartType = strValue == "1101"
                                           || strValue == "1102"
                                           || strValue == "1103"
                                           || strValue == "1107"
                                           || strValue == "1108"
                                           || strValue == "1110"
                                           || strValue == "1113";
                    //是否Voip通道
                    configObject.SetPropertyValue(26, isVoipStartType ? "1" : "0");
                    item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 26);
                    if (item1 != null && item1.Editor != null)
                    {
                        item1.Editor.RefreshValue();
                    }
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 13);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isVoipStartType;
                    }
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 14);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isVoipStartType;
                    }
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 15);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isVoipStartType;
                    }
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 16);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isVoipStartType;
                    }
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 17);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isVoipStartType;
                    }

                    //是否CTI启动方式，CTI启动方式可以选择启动方式扩展
                    bool isCtiStartType = strValue == "1005"
                                          || strValue == "1011"
                                          || strValue == "1014"
                                          || strValue == "1103"
                                          || strValue == "1104"
                                          || strValue == "1105"
                                          || strValue == "1106"
                                          || strValue == "1106"
                                          || strValue == "1107"
                                          || strValue == "1113";
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 22);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isCtiStartType;
                    }

                    //是否Voip，CTI会议启动方式，如果是，启用密码配置项
                    bool isIPConference = strValue == "1113";
                    item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 18);
                    if (item2 != null)
                    {
                        item2.IsEnabled = isIPConference;
                    }
                    break;

                #endregion


                #region 提示音

                case VoiceChannelObject.PRO_ALERTTYPE:
                    item1 =
                        mListResourcePropertyInfoItems.FirstOrDefault(
                            p => p.PropertyID == VoiceChannelObject.PRO_RECORDDELAY);
                    if (item1 != null)
                    {
                        item1.IsEnabled = (strValue == "2" || strValue == "4");
                    }
                    break;

                #endregion


                #region 能量启动参数

                //使用板卡默认的能量启动参数
                case VoiceChannelObject.PRO_ISBOARDDEFAULTACTIVITY:
                    for (int i = 1; i <= 5; i++)
                    {
                        item1 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == VoiceChannelObject.PRO_ISBOARDDEFAULTACTIVITY + i);
                        if (item1 != null)
                        {
                            item1.IsEnabled = strValue == "0";
                        }
                    }
                    break;

                #endregion


                #region AGC

                //启用AGC
                case VoiceChannelObject.PRO_ISAGCENABLE:
                    item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == VoiceChannelObject.PRO_ISBOARDDEFAULTAGC);
                    bool isEnable = false;
                    if (item1 != null)
                    {
                        item1.IsEnabled = strValue == "1";

                        if (strValue == "1")
                        {
                            if (item1.ResourceProperty != null && item1.ResourceProperty.Value == "0")
                            {
                                isEnable = true;
                            }
                        }
                    }
                    for (int i = 2; i <= 5; i++)
                    {
                        item2 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == VoiceChannelObject.PRO_ISAGCENABLE + i);
                        if (item2 != null)
                        {
                            item2.IsEnabled = isEnable;
                        }
                    }
                    break;
                //使用板卡默认的AGC参数
                case VoiceChannelObject.PRO_ISBOARDDEFAULTAGC:
                    for (int i = 2; i <= 5; i++)
                    {
                        item2 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == VoiceChannelObject.PRO_ISAGCENABLE + i);
                        if (item2 != null)
                        {
                            item2.IsEnabled = strValue == "0";
                        }
                    }
                    break;

                #endregion

            }
        }

        private void PropertyValueChangedOperation211(PropertyValueChangedEventArgs args)
        {
            ObjectPropertyInfo propertyInfo = args.PropertyInfo;
            ResourceProperty propertyValue = args.PropetyValue;
            string strValue = args.Value;
            if (propertyValue == null) { return; }
            if (propertyInfo == null) { return; }
            switch (propertyInfo.PropertyID)
            {
                //主备状态
                case S1110Consts.PROPERTYID_MASTERSLAVER:
                    if (ListConfigObjects != null)
                    {
                        var listObjs =
                            ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_LICENSESERVER).ToList();
                        for (int i = 0; i < listObjs.Count; i++)
                        {
                            var obj = listObjs[i];
                            var prop =
                                obj.ListProperties.FirstOrDefault(
                                    p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                            if (strValue == "1")
                            {
                                if (obj.ObjectID != propertyValue.ObjID)
                                {
                                    obj.SetPropertyValue(S1110Consts.PROPERTYID_MASTERSLAVER, "2");
                                }
                            }
                            if (strValue == "2")
                            {
                                if (obj.ObjectID != propertyValue.ObjID)
                                {
                                    obj.SetPropertyValue(S1110Consts.PROPERTYID_MASTERSLAVER, "1");
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void PropertyValueChangedOperation218(PropertyValueChangedEventArgs args)
        {
            ObjectPropertyInfo propertyInfo = args.PropertyInfo;
            ResourceProperty propertyValue = args.PropetyValue;
            string strValue = args.Value;
            if (propertyValue == null) { return; }
            if (propertyInfo == null) { return; }
            switch (propertyInfo.PropertyID)
            {
                //主备状态
                case 5:
                    if (ListConfigObjects != null)
                    {
                        var listObjs =
                            ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_ALARMSERVER).ToList();
                        for (int i = 0; i < listObjs.Count; i++)
                        {
                            var obj = listObjs[i];
                            var prop =
                                obj.ListProperties.FirstOrDefault(
                                    p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                            if (strValue == "1")
                            {
                                if (obj.ObjectID != propertyValue.ObjID)
                                {
                                    obj.SetPropertyValue(S1110Consts.PROPERTYID_MASTERSLAVER, "2");
                                }
                            }
                            if (strValue == "2")
                            {
                                if (obj.ObjectID != propertyValue.ObjID)
                                {
                                    obj.SetPropertyValue(S1110Consts.PROPERTYID_MASTERSLAVER, "1");
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void PropertyValueChangedOperation214(PropertyValueChangedEventArgs args)
        {
            try
            {
                ConfigObject configObject = args.ConfigObject;
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                PropertyValueEnumItem itemValue = args.ValueItem;
                string strValue = args.Value;
                ResourcePropertyInfoItem item2, item3, item4, item5;
                if (configObject == null) { return; }
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyInfo.PropertyID)
                {
                    //设备类型
                    case StorageDeviceObject.PRO_TYPE:
                        item2 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == StorageDeviceObject.PRO_ROOTDIR);
                        item3 =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == StorageDeviceObject.PRO_SERVERID);
                        item4 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_AUTHUSERNAME);
                        item5 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_AUTHPASSWORD);
                        if (item2 != null)
                        {
                            var prop2 = item2.ResourceProperty;
                            if (prop2 != null)
                            {
                                var prop2Value = prop2.Value;
                                if (strValue == "0")
                                {
                                    if (prop2Value.StartsWith("\\\\"))
                                    {
                                        prop2.Value = string.Empty;
                                    }
                                }
                                if (strValue == "1")
                                {
                                    if (!prop2Value.StartsWith("\\\\"))
                                    {
                                        prop2.Value = "\\\\";
                                    }
                                }
                            }
                            var editor = item2.Editor;
                            if (editor != null)
                            {
                                editor.RefreshValue();
                            }
                        }
                        if (item3 != null)
                        {
                            item3.IsEnabled = strValue == "0";
                            if (strValue == "1")
                            {
                                var prop3 = item3.ResourceProperty;
                                if (prop3 != null)
                                {
                                    prop3.Value = "-1";
                                }
                            }
                            var editor = item3.Editor;
                            if (editor != null)
                            {
                                editor.RefreshValue();
                            }
                        }
                        if (item4 != null)
                        {
                            item4.IsEnabled = strValue == "1" || strValue == "2";
                        }
                        if (item5 != null)
                        {
                            item5.IsEnabled = strValue == "1" || strValue == "2";
                        }
                        break;
                    //所在机器
                    case StorageDeviceObject.PRO_SERVERID:
                        if (itemValue == null) { return; }
                        var machine = itemValue.Info as MachineObject;
                        if (machine == null) { return; }
                        configObject.SetPropertyValue(StorageDeviceObject.PRO_HOSTADDRESS, machine.HostAddress);
                        item4 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == StorageDeviceObject.PRO_HOSTADDRESS);
                        if (item4 != null)
                        {
                            var editor = item4.Editor;
                            if (editor != null)
                            {
                                editor.RefreshValue();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("214:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation231(PropertyValueChangedEventArgs args)
        {
            ConfigObject configObject = args.ConfigObject;
            ResourceProperty propertyValue = args.PropetyValue;
            PropertyValueEnumItem itemValue = args.ValueItem;
            if (configObject == null) { return; }
            if (propertyValue == null) { return; }
            switch (propertyValue.PropertyID)
            {
                //Sftp登录名,修改了登录名要同时修改登录密码
                case ScreenServiceObject.PRO_SFTPLOGINNAME:
                    if (itemValue == null) { return; }
                    BasicUserInfo userInfo = itemValue.Info as BasicUserInfo;
                    if (userInfo == null) { return; }
                    //Sftp登录密码
                    var temp = configObject.ListProperties.FirstOrDefault(p => p.PropertyID == ScreenServiceObject.PRO_SFTPPASSWORD);
                    if (temp != null)
                    {
                        temp.Value = userInfo.Password;
                    }
                    break;
            }
        }

        private void PropertyValueChangedOperation236(PropertyValueChangedEventArgs args)
        {
            try
            {
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                ResourcePropertyInfoItem item1;
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyValue.PropertyID)
                {
                    //主备状态
                    case CMServerServiceObject.PRO_STANDBYROLE:
                        item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == CMServerServiceObject.PRO_REMOTECMADDRESS);
                        if (item1 != null)
                        {
                            item1.IsEnabled = strValue == "2" || strValue == "1";
                        }
                        item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == CMServerServiceObject.PRO_DETECTNICNAME);
                        if (item1 != null)
                        {
                            item1.IsEnabled = strValue == "2" || strValue == "1";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("236:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation251(PropertyValueChangedEventArgs args)
        {
            try
            {
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyValue.PropertyID)
                {
                    //ManageNet,只能有一个FileOperator的ManageNet为1
                    case 12:
                        var temp = from pvs in ListPropertyValues
                                   where pvs.ObjType == S1110Consts.RESOURCE_FILEOPERATOR
                                   group pvs by pvs.ObjID;
                        foreach (var fileOperator in temp)
                        {
                            if (strValue == "1")
                            {
                                if (fileOperator.Key != propertyValue.ObjID)
                                {
                                    var prop = fileOperator.FirstOrDefault(p => p.PropertyID == 12);
                                    if (prop != null)
                                    {
                                        //prop.Value = "0";
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("251:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation256(PropertyValueChangedEventArgs args)
        {
            try
            {
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                ResourcePropertyInfoItem item1, item2;
                switch (propertyValue.PropertyID)
                {
                    //是否指定启动时间
                    case 22:
                        item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 23);
                        item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 24);
                        if (strValue == "1")
                        {
                            if (item1 != null)
                            {
                                item1.IsEnabled = true;
                            }
                            if (item2 != null)
                            {
                                item2.IsEnabled = true;
                            }
                        }
                        if (strValue == "0")
                        {
                            if (item1 != null)
                            {
                                item1.IsEnabled = false;
                                var value = item1.ResourceProperty;
                                if (value != null)
                                {
                                    value.Value = "00:00";
                                }
                                var editor = item1.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                            if (item2 != null)
                            {
                                item2.IsEnabled = false;
                                var value = item2.ResourceProperty;
                                if (value != null)
                                {
                                    value.Value = "23:59";
                                }
                                var editor = item2.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("256:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation258(PropertyValueChangedEventArgs args)
        {
            try
            {
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                ResourcePropertyInfoItem item1, item2;
                switch (propertyValue.PropertyID)
                {
                    //是否指定启动时间
                    case 22:
                        item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 23);
                        item2 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 24);
                        if (strValue == "1")
                        {
                            if (item1 != null)
                            {
                                item1.IsEnabled = true;
                            }
                            if (item2 != null)
                            {
                                item2.IsEnabled = true;
                            }
                        }
                        if (strValue == "0")
                        {
                            if (item1 != null)
                            {
                                item1.IsEnabled = false;
                                var value = item1.ResourceProperty;
                                if (value != null)
                                {
                                    value.Value = "00:00";
                                }
                                var editor = item1.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                            if (item2 != null)
                            {
                                item2.IsEnabled = false;
                                var value = item2.ResourceProperty;
                                if (value != null)
                                {
                                    value.Value = "23:59";
                                }
                                var editor = item2.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("258:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation271(PropertyValueChangedEventArgs args)
        {
            try
            {
                ConfigObject configObject = args.ConfigObject;
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (configObject == null) { return; }
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                ResourcePropertyInfoItem item;
                ResourceProperty propertyValue1;
                switch (propertyValue.PropertyID)
                {
                    //类型
                    case 11:
                        item =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == S1110Consts.PROPERTYID_HOSTPORT);
                        if (item != null)
                        {
                            if (!args.IsInit)
                            {
                                propertyValue1 = item.ResourceProperty;
                                if (propertyValue1 != null)
                                {
                                    if (strValue == "1")
                                    {
                                        //发送服务器默认端口25
                                        configObject.SetPropertyValue(S1110Consts.PROPERTYID_HOSTPORT, "25");
                                    }
                                    if (strValue == "2")
                                    {
                                        //接收服务器默认端口110；
                                        configObject.SetPropertyValue(S1110Consts.PROPERTYID_HOSTPORT, "110");
                                    }
                                }
                            }
                            var editor = item.Editor;
                            if (editor != null)
                            {
                                editor.RefreshValue();
                            }
                        }
                        item =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == 13);
                        if (item != null)
                        {
                            item.IsEnabled = strValue == "1" || strValue == "3";
                        }
                        item =
                            mListResourcePropertyInfoItems.FirstOrDefault(
                                p => p.PropertyID == 14);
                        if (item != null)
                        {
                            item.IsEnabled = strValue == "1" || strValue == "3";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("271:" + ex.Message);
            }
        }

        private void PropertyValueChangedOperation276(PropertyValueChangedEventArgs args)
        {
            try
            {
                ResourceProperty propertyValue = args.PropetyValue;
                if (propertyValue.PropertyID != 14) { return; }
                if (args == null || args.ValueItem == null) { return; }
                ConfigObject configVoice = args.ValueItem.Info as ConfigObject;
                ResourcePropertyInfoItem item;
                if (configVoice == null) { return; }
                if (ListConfigObjects == null) { return; }
                List<ConfigObject> mListChannels = ListConfigObjects.Where(p => p.ObjectID == configVoice.ObjectID).ToList();
                item = mListResourcePropertyInfoItems.FirstOrDefault(
                                        p => p.PropertyID == 13);
                if (item != null && item.Editor != null)
                {
                    item.Editor.RefreshValue();
                }
            }
            catch (Exception ex)
            {
                ShowException("276:" + ex.ToString());
            }
        }

        private void PropertyValueChangedOperation279(PropertyValueChangedEventArgs args)
        {
            try
            {
                ResourceProperty propertyValue = args.PropetyValue;
                if (propertyValue.PropertyID != 12) { return; }
                ConfigObject configVoice = args.ValueItem.Info as ConfigObject;
                ResourcePropertyInfoItem item;
                if (configVoice == null) { return; }
                if (ListConfigObjects == null) { return; }
                List<ConfigObject> mListChannels = ListConfigObjects.Where(p => p.ObjectID == configVoice.ObjectID).ToList();
                item = mListResourcePropertyInfoItems.FirstOrDefault(
                                        p => p.PropertyID == 11);
                if (item != null && item.Editor != null)
                {
                    item.Editor.RefreshValue();
                }
            }
            catch (Exception ex)
            {
                ShowException("279:" + ex.ToString());
            }
        }

        private void PropertyValueChangedOperation281(PropertyValueChangedEventArgs args)
        {
            try
            {
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                ResourcePropertyInfoItem item1;
                int proid;
                switch (propertyValue.PropertyID)
                {
                    //是否启用
                    case SpeechAnalysisServiceObject.PRO_ANALYSISENABLE:
                        for (int i = 1; i < 10; i++)
                        {
                            proid = SpeechAnalysisServiceObject.PRO_ANALYSISENABLE + i;
                            item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == proid);
                            if (item1 != null)
                            {
                                item1.IsEnabled = strValue == "1";
                            }
                        }
                        break;
                    case SpeechAnalysisServiceObject.PRO_KEYWORDENABLE:
                        for (int i = 1; i < 10; i++)
                        {
                            proid = SpeechAnalysisServiceObject.PRO_KEYWORDENABLE + i;
                            item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == proid);
                            if (item1 != null)
                            {
                                item1.IsEnabled = strValue == "1";
                            }
                        }
                        break;
                    case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTENABLE:
                        for (int i = 1; i < 20; i++)
                        {
                            proid = SpeechAnalysisServiceObject.PRO_SPEECHTOTXTENABLE + i;
                            item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == proid);
                            if (item1 != null)
                            {
                                item1.IsEnabled = strValue == "1";
                            }
                        }
                        break;
                    case SpeechAnalysisServiceObject.PRO_ANALYSISSOURCEACCESS:
                    case SpeechAnalysisServiceObject.PRO_KEYWORDSOURCEACCESS:
                    case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTSOURCEACCESS:
                        //case SpeechAnalysisServiceObject.PRO_SPEECHTOTXTTARGETACCESS:
                        proid = propertyValue.PropertyID + 1;
                        item1 = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == proid);
                        if (item1 != null)
                        {
                            item1.IsEnabled = strValue != "0";
                            //item1.Editor.RefreshValue();
                        }
                        break;
                    //case SpeechAnalysisServiceObject.PRO_SPEECHTOSAVEFILE://px：用下面的list找到item设置内容
                    //    if (strValue == "0")//后面三个属性全部变灰
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 52 && p.PropertyID <= 54).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsEnabled = false;
                    //        }
                    //    }
                    //    break;
                    //case SpeechAnalysisServiceObject.PRO_SPEECHTOENGINETYPE1://px：用下面的list找到item设置内容
                    //    if (strValue != "2")//后面三个属性全部变灰
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 61 && p.PropertyID <= 64).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = true;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 22);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 61 && p.PropertyID <= 64).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = false;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 22);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = false;
                    //        }

                    //    }
                    //    break;
                    //case SpeechAnalysisServiceObject.PRO_SPEECHTOENGINETYPE2://px：用下面的list找到item设置内容
                    //    if (strValue != "2")//后面三个属性全部变灰
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 65 && p.PropertyID <= 68).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = true;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 32);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 65 && p.PropertyID <= 68).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = false;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 32);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = false;
                    //        }

                    //    }
                    //    break;
                    //case SpeechAnalysisServiceObject.PRO_SPEECHTOENGINETYPE3://px：用下面的list找到item设置内容
                    //    if (strValue != "2")//后面三个属性全部变灰
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 69 && p.PropertyID <= 72).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = true;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 52);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = true;
                    //        }
                    //        TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 42);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        List<ResourcePropertyInfoItem> TempList = mListResourcePropertyInfoItems.Where(p => p.PropertyID >= 69 && p.PropertyID <= 72).ToList();
                    //        foreach (ResourcePropertyInfoItem r in TempList)
                    //        {
                    //            r.IsHidden = false;
                    //        }
                    //        ResourcePropertyInfoItem TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 52);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = false;
                    //        }
                    //        TempInfoItem = mListResourcePropertyInfoItems.FirstOrDefault(p => p.PropertyID == 42);
                    //        if (TempInfoItem != null)
                    //        {
                    //            if (TempInfoItem.ResourceProperty != null)
                    //                TempInfoItem.ResourceProperty.Value = "0";
                    //            if (TempInfoItem.Editor != null)
                    //                TempInfoItem.Editor.RefreshValue();
                    //            TempInfoItem.IsEnabled = false;
                    //        }
                    //    }
                    //    break;
                }
            }
            catch (Exception ex)
            {
                ShowException("281:" + ex.ToString());
            }
        }

        private void PropertyValueChangedOperation241(PropertyValueChangedEventArgs args)
        {
            try
            {
                ConfigObject configObject = args.ConfigObject;
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                ResourcePropertyInfoItem item1;
                if (configObject == null) { return; }
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyInfo.PropertyID)
                {
                    //Use DMCC
                    case CTIConnectionObject.PRO_USERDMCC:
                        for (int i = 1; i <= 4; i++)
                        {
                            item1 =
                                mListResourcePropertyInfoItems.FirstOrDefault(
                                    p => p.PropertyID == CTIConnectionObject.PRO_USERDMCC + i);
                            if (item1 != null)
                            {
                                item1.IsEnabled = strValue == "1";
                            }
                        }
                        break;
                    //版本协议
                    case CTIConnectionObject.PRO_VERSIONPROTOCOL:
                        if (args.IsInit) { return; }
                        if (ListBasicInfoDatas != null)
                        {
                            var info =
                                ListBasicInfoDatas.FirstOrDefault(
                                    b =>
                                        b.InfoID == S1110Consts.SOURCEID_CTICONNECTION_VERSIONPROTOCOL &&
                                        b.Value == strValue);
                            if (info != null)
                            {
                                var proValue =
                                    configObject.ListProperties.FirstOrDefault(
                                        p => p.PropertyID == CTIConnectionObject.PRO_VERSION);
                                if (proValue != null)
                                {
                                    proValue.Value = info.Icon;
                                }
                                proValue =
                                    configObject.ListProperties.FirstOrDefault(
                                        p => p.PropertyID == CTIConnectionObject.PRO_PROTOCOL);
                                if (proValue != null)
                                {
                                    proValue.Value = info.Value;
                                }
                            }
                            item1 =
                                mListResourcePropertyInfoItems.FirstOrDefault(
                                    p => p.PropertyID == CTIConnectionObject.PRO_VERSION);
                            if (item1 != null)
                            {
                                var editor = item1.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                            item1 =
                                mListResourcePropertyInfoItems.FirstOrDefault(
                                    p => p.PropertyID == CTIConnectionObject.PRO_PROTOCOL);
                            if (item1 != null)
                            {
                                var editor = item1.Editor;
                                if (editor != null)
                                {
                                    editor.RefreshValue();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("241:" + ex.ToString());
            }
        }

        private void PropertyValueChangedOperation243(PropertyValueChangedEventArgs args)
        {
            try
            {
                ConfigObject configObject = args.ConfigObject;
                ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                ResourceProperty propertyValue = args.PropetyValue;
                string strValue = args.Value;
                if (configObject == null) { return; }
                if (propertyValue == null) { return; }
                if (propertyInfo == null) { return; }
                switch (propertyInfo.PropertyID)
                {
                    //CTI类型,自动修改子资源的CTI类型
                    case CTIConnectionGroupCollectionObject.PRO_CTITYPE:
                        if (ListConfigObjects == null) { return; }
                        List<ConfigObject> childs =
                            ListConfigObjects.Where(c => c.ParentID == configObject.ObjectID).ToList();
                        for (int i = 0; i < childs.Count; i++)
                        {
                            ConfigObject temp = childs[i];
                            ResourceProperty prop = temp.ListProperties.FirstOrDefault(p => p.PropertyID == CTIConnectionGroupObject.PRO_CTITYPE);
                            if (prop != null)
                            {
                                prop.Value = strValue;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException("243:" + ex.ToString());
            }
        }

        #endregion


        #region PropertyListerEvent Event

        public static readonly RoutedEvent PropertyListerEventEvent;

        public event RoutedPropertyChangedEventHandler<PropertyListerEventEventArgs> PropertyListerEvent
        {
            add { AddHandler(PropertyListerEventEvent, value); }
            remove { RemoveHandler(PropertyListerEventEvent, value); }
        }

        private void OnPropertyListerEvent(PropertyListerEventEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs> p =
                new RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs>(null, args);
            p.RoutedEvent = PropertyListerEventEvent;
            RaiseEvent(p);
        }

        #endregion


        #region Check and Set Config
        /// <summary>
        /// 检查参数有效性
        /// </summary>
        /// <returns></returns>
        public CheckResult CheckConfig()
        {
            CheckResult result = new CheckResult();
            result.Result = true;
            result.Code = 0;
            //if (ConfigObject == null)
            //{
            //    result.Result = false;
            //    result.Code = 1;
            //    result.Message = string.Format("ConfigObject is null");
            //    return result;
            //}
            ////检查配置参数前先设置一下依赖属性的值
            //ConfigObject.SetBasicPropertyValues();
            //result = ConfigObject.CheckConfig();
            return result;
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                if (ListResourceGroupParams == null || ListObjectPropertyInfos == null) { return; }
                for (int i = 0; i < mListResourcePropertyInfoItems.Count; i++)
                {
                    var item = mListResourcePropertyInfoItems[i];
                    var propertyInfo =
                        ListObjectPropertyInfos.FirstOrDefault(
                            p => p.ObjType == item.ObjType && p.PropertyID == item.PropertyID);
                    if (propertyInfo == null) { continue; }
                    item.StrPropertyName =
                           CurrentApp.GetLanguageInfo(
                               string.Format("PRO{0}{1}", propertyInfo.ObjType.ToString("000"),
                                   propertyInfo.PropertyID.ToString("000")), propertyInfo.Description);
                    var group =
                        ListResourceGroupParams.FirstOrDefault(g => g.TypeID == item.ObjType && g.GroupID == item.GroupID);
                    if (group == null)
                    {
                        if (item.GroupID == 0)
                        {
                            item.GroupName = CurrentApp.GetLanguageInfo("1110GRP000000", "Basic");
                        }
                    }
                    else
                    {
                        item.GroupName =
                       CurrentApp.GetLanguageInfo(
                           string.Format("1110GRP{0}{1}", group.TypeID.ToString("000"), group.GroupID.ToString("000")),
                           group.Description);
                    }

                    if (item.Editor != null)
                    {
                        item.Editor.ChangeLanguage();
                    }
                }
                //此操作将触发每个ListBoxItem重新Load，当然ListBoxItem也会切换语言
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBoxPropertyList.ItemsSource);
                if (view != null && view.GroupDescriptions != null)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
                }
            }
            catch { }

        }

        #endregion

    }
}
