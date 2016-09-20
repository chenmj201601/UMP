//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    55b62f83-b4b6-41a7-a228-ddf1a45e4fd2
//        CLR Version:              4.0.30319.18408
//        Name:                     UCWidgetPropertyEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                UCWidgetPropertyEditor
//
//        created by Charley at 2016/5/4 11:29:03
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UMPS1206.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1206
{
    /// <summary>
    /// UCWidgetPropertyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class UCWidgetPropertyEditor
    {

        #region Members

        private bool mIsInited;
        private WidgetItem mWidgetItem;
        private WidgetPropertyInfo mWidgetPropertyInfo;
        private UserWidgetPropertyValue mWidgetPropertyValue;
        private ObservableCollection<PropertyValueEnumItem> mListPropertyValueEnumItems;

        #endregion


        public UCWidgetPropertyEditor()
        {
            InitializeComponent();

            mListPropertyValueEnumItems = new ObservableCollection<PropertyValueEnumItem>();

            Loaded += UCWidgetPropertyEditor_Loaded;
        }

        void UCWidgetPropertyEditor_Loaded(object sender, RoutedEventArgs e)
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
                if (PropertyItem == null) { return; }
                CurrentApp = PropertyItem.CurrentApp;
                PropertyItem.Editor = this;
                if (CurrentApp == null) { return; }
                mWidgetItem = PropertyItem.WidgetItem;
                mWidgetPropertyInfo = PropertyItem.PropertyInfo;
                if (mWidgetPropertyInfo != null)
                {
                    ConvertFormat = mWidgetPropertyInfo.ConvertFormat;
                }
                mWidgetPropertyValue = PropertyItem.PropertyValue;
                if (mWidgetPropertyValue != null)
                {
                    Value = mWidgetPropertyValue.Value01;
                    Text = mWidgetPropertyValue.Value01;
                }
                InitEnumItems();
                ShowValue();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitEnumItems()
        {
            mListPropertyValueEnumItems.Clear();
            if (mWidgetPropertyInfo != null)
            {
                switch (mWidgetPropertyInfo.ConvertFormat)
                {
                    case WidgetPropertyConvertFormat.YesNo:
                        InitYesNoEnumItems();
                        break;
                    case WidgetPropertyConvertFormat.BasicInfoSingleSelect:
                        InitBasicInfoEnumItems();
                        break;
                }
            }
        }

        private void InitYesNoEnumItems()
        {
            PropertyValueEnumItem item = new PropertyValueEnumItem();
            item.Value = "1";
            item.Display = CurrentApp.GetLanguageInfo(string.Format("COM004"), "Yes");
            item.IsSelected = true;
            item.Info = "COM004";
            mListPropertyValueEnumItems.Add(item);
            item = new PropertyValueEnumItem();
            item.Value = "0";
            item.Info = "COM005";
            item.Display = CurrentApp.GetLanguageInfo(string.Format("COM005"), "No");
            item.IsSelected = false;
            mListPropertyValueEnumItems.Add(item);
        }

        private void InitBasicInfoEnumItems()
        {
            try
            {
                if (mWidgetItem == null) { return; }
                var listAllBasicInfos = mWidgetItem.ListBasicDataInfos;
                if (listAllBasicInfos == null) { return; }
                if (mWidgetPropertyInfo == null) { return; }
                long sourceID = mWidgetPropertyInfo.SourceID;
                var listBasicInfos = listAllBasicInfos.Where(b => b.InfoID == sourceID).OrderBy(b => b.SortID).ToList();
                for (int i = 0; i < listBasicInfos.Count; i++)
                {
                    var info = listBasicInfos[i];

                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Display =
                        CurrentApp.GetLanguageInfo(
                            string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                            info.Icon);
                    item.Description = CurrentApp.GetLanguageInfo(
                            string.Format("BIDD{0}{1}", info.InfoID, info.SortID.ToString("000")),
                            info.Icon);
                    item.SortID = info.SortID;
                    item.Value = info.Value;
                    item.Info = info;
                    mListPropertyValueEnumItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowValue()
        {
            try
            {
                if (mWidgetPropertyInfo != null)
                {
                    switch (mWidgetPropertyInfo.ConvertFormat)
                    {
                        case WidgetPropertyConvertFormat.YesNo:
                            for (int i = 0; i < mListPropertyValueEnumItems.Count; i++)
                            {
                                if (mListPropertyValueEnumItems[i].Value == Value)
                                {
                                    mListPropertyValueEnumItems[i].IsSelected = true;
                                }
                            }
                            break;
                        case WidgetPropertyConvertFormat.BasicInfoSingleSelect:
                            for (int i = 0; i < mListPropertyValueEnumItems.Count; i++)
                            {
                                if (mListPropertyValueEnumItems[i].Value == Value)
                                {
                                    if (mItemsSelectControlValue != null)
                                    {
                                        mItemsSelectControlValue.SelectedIndex = i;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Template

        private const string PART_Panel = "PART_Panel";
        private const string PART_TextBlock = "PART_TextBlock";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_IntTextBox = "PART_IntTextBox";
        private const string PART_ItemsSelectControl = "PART_ItemsSelectControl";

        private Border mBorderPanel;
        private TextBlock mTextBlockValue;
        private AutoSelectTextBox mTextBoxValue;
        private IntegerUpDown mIntTextBoxValue;
        private Selector mItemsSelectControlValue;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {

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
                mItemsSelectControlValue.ItemsSource = mListPropertyValueEnumItems;
                ShowValue();
                //SetDefaultValue();
            }
        }

        #endregion


        #region Event Handler

        void mTextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mWidgetPropertyValue != null
                && mTextBoxValue != null)
            {
                mWidgetPropertyValue.Value01 = mTextBoxValue.Text;
            }
        }

        void mIntTextBoxValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (mWidgetPropertyValue != null
               && mIntTextBoxValue != null)
            {
                string value = mIntTextBoxValue.Value == null ? string.Empty : mIntTextBoxValue.Value.ToString();
                mWidgetPropertyValue.Value01 = value;
            }
        }

        void mItemsSelectControlValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isInit = e.RemovedItems.Count == 0;        //此处判断是否第一次触发事件不太合理
            if (mWidgetPropertyValue == null) { return; }
            if (mItemsSelectControlValue != null)
            {
                var item = mItemsSelectControlValue.SelectedItem as PropertyValueEnumItem;
                if (item != null)
                {
                    mWidgetPropertyValue.Value01 = item.Value;
                }
                else
                {
                    mWidgetPropertyValue.Value01 = string.Empty;
                }
            }
        }

        void EnumItem_IsCheckedChanged()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowException("EnumItem_IsCheckedChanged:" + ex.Message);
            }
        }

        void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowException("ComboBox_DropDownOpened:" + ex.Message);
            }
        }

        void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowException("ComboBox_TextChanged:" + ex.Message);
            }
        }

        #endregion


        #region PropertyItem Property

        public static readonly DependencyProperty PropertyItemProperty =
          DependencyProperty.Register("PropertyItem", typeof(WidgetPropertyItem), typeof(UCWidgetPropertyEditor), new PropertyMetadata(default(WidgetPropertyItem)));

        public WidgetPropertyItem PropertyItem
        {
            get { return (WidgetPropertyItem)GetValue(PropertyItemProperty); }
            set { SetValue(PropertyItemProperty, value); }
        }

        #endregion


        #region ConvertFormat Property

        public static readonly DependencyProperty ConvertFormatProperty =
            DependencyProperty.Register("ConvertFormat", typeof(WidgetPropertyConvertFormat), typeof(UCWidgetPropertyEditor), new PropertyMetadata(default(WidgetPropertyConvertFormat)));

        public WidgetPropertyConvertFormat ConvertFormat
        {
            get { return (WidgetPropertyConvertFormat)GetValue(ConvertFormatProperty); }
            set { SetValue(ConvertFormatProperty, value); }
        }

        #endregion


        #region Value Property

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(UCWidgetPropertyEditor), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion


        #region Text Property

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UCWidgetPropertyEditor), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListPropertyValueEnumItems.Count; i++)
                {
                    var item = mListPropertyValueEnumItems[i];
                    var basicDataInfo = item.Info as BasicDataInfo;
                    if (basicDataInfo != null)
                    {
                        item.Display =
                            CurrentApp.GetLanguageInfo(
                                string.Format("BID{0}{1}", basicDataInfo.InfoID, basicDataInfo.SortID.ToString("000")),
                                basicDataInfo.Icon);
                        item.Description =
                            CurrentApp.GetLanguageInfo(
                                string.Format("BIDD{0}{1}", basicDataInfo.InfoID, basicDataInfo.SortID.ToString("000")),
                                basicDataInfo.Icon);
                    }
                    if (item.Info != null)
                    {
                        string strCom = item.Info.ToString();
                        if (strCom.StartsWith("COM"))
                        {
                            item.Display = CurrentApp.GetLanguageInfo(strCom, strCom);
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
