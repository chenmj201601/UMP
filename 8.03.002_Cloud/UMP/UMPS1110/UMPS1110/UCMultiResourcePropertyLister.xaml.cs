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
using UMPS1110.Models;
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

        public MultiSelectedItem MultiSelectedItem { get; set; }
        public List<ObjectPropertyInfo> ListObjectPropertyInfos { get; set; }
        public List<ResourceGroupParam> ListResourceGroupParams { get; set; }
        public List<ConfigObject> ListAllConfigObjects { get; set; } 

        private ObservableCollection<ResourcePropertyInfoItem> mListResourcePropertyInfoItems;
        private List<ConfigObject> mListConfigObjects;
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

        }

        #endregion


        #region Init and Load

        private void Init()
        {
            double width = ActualWidth;
            if (!double.IsNaN(width))
            {
                NameWidth = width * 2 / 5;
            }
            InitPropertyInfoItems();
        }

        private void InitPropertyInfoItems()
        {
            if (MultiSelectedItem == null) { return; }
            mObjType = MultiSelectedItem.ObjType;
            ConfigObject baseConfigObject = null;
            for (int i = 0; i < MultiSelectedItem.ListObjectItems.Count; i++)
            {
                var item = MultiSelectedItem.ListObjectItems[i];
                var configObject = item.Data as ConfigObject;
                if (configObject == null) { continue; }
                baseConfigObject = configObject;
                mListConfigObjects.Add(configObject);
            }
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
                ResourceProperty propertyValue =
                    baseConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                if (propertyValue == null) { continue; }
                ResourcePropertyInfoItem item = new ResourcePropertyInfoItem();
                item.IsEnabled = true;
                item.IsProp6Enabled = true;
                item.StrPropertyName =
                    CurrentApp.GetLanguageInfo(
                        string.Format("PRO{0}{1}", propertyInfo.ObjType.ToString("000"),
                            propertyInfo.PropertyID.ToString("000")), propertyInfo.Description);
                item.GroupID = propertyInfo.GroupID;
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
                item.ResourceProperty = propertyValue;
                item.PropertyInfo = propertyInfo;
                mListResourcePropertyInfoItems.Add(item);
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
            //var item = ListBoxPropertyList.SelectedItem as ResourcePropertyInfoItem;
            //if (item != null)
            //{
            //    PropertyItemChangedEventArgs args = new PropertyItemChangedEventArgs();
            //    args.ConfigObject = ConfigObject;
            //    args.PropertyItem = item;
            //    OnPropertyItemChannged(args);
            //    //触发PropertyItemChanged事件
            //    PropertyListerEventEventArgs listerEventArgs = new PropertyListerEventEventArgs();
            //    listerEventArgs.Code = 1;
            //    listerEventArgs.Data = args;
            //    OnPropertyListerEvent(listerEventArgs);
            //}
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
                    //PropertyValueChangedOperation(args);
                    //ObjectPropertyInfo propertyInfo = args.PropertyInfo;
                    ////特殊的处理操作
                    //switch (propertyInfo.ObjType)
                    //{
                    //    case S1110Consts.RESOURCE_VOICESERVER:
                    //        PropertyValueChangedOperation221(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_VOIPPROTOCAL:
                    //        PropertyValueChangedOperation223(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_CHANNEL:
                    //        PropertyValueChangedOperation225(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_LICENSESERVER:
                    //        PropertyValueChangedOperation211(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_STORAGEDEVICE:
                    //        PropertyValueChangedOperation214(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_ALARMSERVER:
                    //        PropertyValueChangedOperation218(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_SCREENSERVER:
                    //        PropertyValueChangedOperation231(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_CMSERVER:
                    //        PropertyValueChangedOperation236(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_FILEOPERATOR:
                    //        PropertyValueChangedOperation251(args);
                    //        break;
                    //    case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:
                    //        PropertyValueChangedOperation281(args);
                    //        break;
                    //}

                    //PropertyListerEventEventArgs listerEventArgs = new PropertyListerEventEventArgs();
                    //listerEventArgs.Code = 2;
                    //listerEventArgs.Data = args;
                    //OnPropertyListerEvent(listerEventArgs);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region PropertyValueChangedOperations

        #endregion
    }
}
