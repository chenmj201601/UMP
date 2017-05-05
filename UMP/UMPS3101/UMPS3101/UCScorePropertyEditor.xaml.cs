//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5e6d41bf-2a37-4db1-aec2-cd45b8714e7a
//        CLR Version:              4.0.30319.18063
//        Name:                     UCScorePropertyEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                UCScorePropertyEditor
//
//        created by Charley at 2015/11/5 18:02:22
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
using System.Windows.Media;
using UMPS3101.Models;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3101
{
    /// <summary>
    /// UCScorePropertyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class UCScorePropertyEditor
    {

        static UCScorePropertyEditor()
        {
            PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs>),
                typeof(UCScorePropertyEditor));
        }

        public UCScorePropertyEditor()
        {
            InitializeComponent();

            mListEnumValueItems = new ObservableCollection<PropertyValueEnumItem>();

            Loaded += UCScorePropertyEditor_Loaded;
        }

        void UCScorePropertyEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            ChangeLanguage();
        }


        #region ScorePropertyInfoItem

        public static readonly DependencyProperty ScorePropertyInfoItemProperty =
          DependencyProperty.Register("ScorePropertyInfoItem", typeof(ScorePropertyInfoItem), typeof(UCScorePropertyEditor), new PropertyMetadata(default(ScorePropertyInfoItem), OnScorePropertyInfoItemChanged));

        public ScorePropertyInfoItem ScorePropertyInfoItem
        {
            get { return (ScorePropertyInfoItem)GetValue(ScorePropertyInfoItemProperty); }
            set { SetValue(ScorePropertyInfoItemProperty, value); }
        }

        private static void OnScorePropertyInfoItemChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var editor = dp as UCScorePropertyEditor;
            if (editor != null)
            {
                editor.OnScorePropertyInfoItemChanged(e.OldValue, e.NewValue);
            }
        }

        public void OnScorePropertyInfoItemChanged(object oldItem, object newItem)
        {
            if (newItem != null)
            {
                //Init();
            }
        }

        #endregion


        #region ConvertFormat

        public static readonly DependencyProperty ConvertFormatProperty =
            DependencyProperty.Register("ConvertFormat", typeof(ScorePropertyDataType), typeof(UCScorePropertyEditor), new PropertyMetadata(default(ScorePropertyDataType)));

        public ScorePropertyDataType ConvertFormat
        {
            get { return (ScorePropertyDataType)GetValue(ConvertFormatProperty); }
            set { SetValue(ConvertFormatProperty, value); }
        }

        #endregion


        #region ValueProperty

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(UCScorePropertyEditor), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion


        #region TextProperty

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UCScorePropertyEditor), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion


        #region BoolValueProperty

        public static readonly DependencyProperty BoolValueProperty =
            DependencyProperty.Register("BoolValue", typeof(bool), typeof(UCScorePropertyEditor), new PropertyMetadata(false));

        public bool BoolValue
        {
            get { return (bool)GetValue(BoolValueProperty); }
            set { SetValue(BoolValueProperty, value); }
        }

        #endregion


        #region ColorValueProperty

        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register("ColorValue", typeof(Color), typeof(UCScorePropertyEditor), new PropertyMetadata(default(Color)));

        public Color ColorValue
        {
            get { return (Color)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }

        #endregion


        #region Members

        private ScoreObject mScoreObject;
        private ScoreProperty mScoreProperty;
        private ObservableCollection<PropertyValueEnumItem> mListEnumValueItems;

        #endregion


        #region Template

        private const string PART_PANEL = "PART_Panel";
        private const string PART_TEXTBOX = "PART_TextBox";
        private const string PART_INTTEXTBOX = "PART_IntTextBox";
        private const string PART_DOUBLETEXTBOX = "PART_DoubleTextBox";
        private const string PART_BOOLCHECKBOX = "PART_BoolCheckBox";
        private const string PART_DATETIMETEXTBOX = "PART_DateTimeTextBox";
        private const string PART_ITEMSSELECTCONTROL = "PART_ItemsSelectControl";
        private const string PART_COLORTEXTBOX = "PART_ColorTextBox";

        private Border mBorderPanel;
        private AutoSelectTextBox mTxtTextBox;
        private IntegerUpDown mTxtIntTextBox;
        private DoubleUpDown mTxtDoubleTextBox;
        private CheckBox mBoolCheckBox;
        private DateTimePicker mDatetimeTextBox;
        private Selector mItemSelectControl;
        private ColorPicker mColorPickerTextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_PANEL) as Border;
            if (mBorderPanel != null)
            {

            }
            mTxtTextBox = GetTemplateChild(PART_TEXTBOX) as AutoSelectTextBox;
            if (mTxtTextBox != null)
            {
                mTxtTextBox.TextChanged += TxtTextBox_TextChanged;
            }
            mTxtIntTextBox = GetTemplateChild(PART_INTTEXTBOX) as IntegerUpDown;
            if (mTxtIntTextBox != null)
            {
                mTxtIntTextBox.ValueChanged += TxtIntTextBox_ValueChanged;
            }
            mTxtDoubleTextBox = GetTemplateChild(PART_DOUBLETEXTBOX) as DoubleUpDown;
            if (mTxtDoubleTextBox != null)
            {
                mTxtDoubleTextBox.ValueChanged += mTxtDoubleTextBox_ValueChanged;
            }
            mBoolCheckBox = GetTemplateChild(PART_BOOLCHECKBOX) as CheckBox;
            if (mBoolCheckBox != null)
            {
                mBoolCheckBox.Click += BoolCheckBox_Click;
            }
            mDatetimeTextBox = GetTemplateChild(PART_DATETIMETEXTBOX) as DateTimePicker;
            if (mDatetimeTextBox != null)
            {
                mDatetimeTextBox.ValueChanged += mDatetimeTextBox_ValueChanged;
            }
            mItemSelectControl = GetTemplateChild(PART_ITEMSSELECTCONTROL) as Selector;
            if (mItemSelectControl != null)
            {
                mItemSelectControl.ItemsSource = mListEnumValueItems;
                mItemSelectControl.SelectionChanged += ItemSelectControl_SelectionChanged;
                ShowValue();
            }
            mColorPickerTextBox = GetTemplateChild(PART_COLORTEXTBOX) as ColorPicker;
            if (mColorPickerTextBox != null)
            {
                mColorPickerTextBox.SelectedColorChanged += mColorPickerTextBox_SelectedColorChanged;
            }
        }

        #endregion


        #region EventHandlers

        void BoolCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mBoolCheckBox != null)
                {
                    bool boolValue = mBoolCheckBox.IsChecked == true;
                    OnScorePropertyValueChanged(boolValue);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtIntTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (mTxtIntTextBox != null)
                {
                    if (mTxtIntTextBox.Value != null)
                    {
                        OnScorePropertyValueChanged(mTxtIntTextBox.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void mTxtDoubleTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (mTxtDoubleTextBox != null)
                {
                    if (mTxtDoubleTextBox.Value != null && mTxtDoubleTextBox.Value > 0)
                    {
                        OnScorePropertyValueChanged(mTxtDoubleTextBox.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void mDatetimeTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (mDatetimeTextBox != null)
                {
                    if (mDatetimeTextBox.Value != null)
                    {
                        OnScorePropertyValueChanged(mDatetimeTextBox.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (mTxtTextBox != null)
                {
                    string strValue = mTxtTextBox.Text;
                    OnScorePropertyValueChanged(strValue);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ItemSelectControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bool isInit = e.RemovedItems.Count == 0;        //此处判断是否第一次触发事件,不太合理
                if (mItemSelectControl != null)
                {
                    var item = mItemSelectControl.SelectedItem as PropertyValueEnumItem;
                    if (item != null)
                    {
                        var value = item.Info;
                        OnScorePropertyValueChanged(value);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void mColorPickerTextBox_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            try
            {
                if (mColorPickerTextBox != null)
                {
                    var color = mColorPickerTextBox.SelectedColor;
                    OnScorePropertyValueChanged(color);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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
            PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
            args.ScoreObject = mScoreObject;
            args.PropertyItem = ScorePropertyInfoItem;
            args.ScoreProperty = mScoreProperty;
            OnPropertyValueChanged(args);
        }

        private void OnScorePropertyValueChanged(object value)
        {
            if (mScoreProperty == null) { return; }
            if (mScoreObject == null) { return; }
            try
            {
                mScoreProperty.SetPropertyValue(mScoreObject, value);

                PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                args.NewValue = value;
                args.ScoreObject = mScoreObject;
                args.PropertyName = mScoreProperty.PropertyName;
                args.NewValue = value;
                mScoreObject.PropertyChanged(this, args);

                RaisePropertyValueChangedEvent();
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
                if (ScorePropertyInfoItem == null) { return; }
                CurrentApp = ScorePropertyInfoItem.CurrentApp;
                ScorePropertyInfoItem.Editor = this;
                mScoreObject = ScorePropertyInfoItem.ScoreObject;
                ScoreProperty scoreProperty = ScorePropertyInfoItem.ScoreProperty;
                mScoreProperty = scoreProperty;
                if (scoreProperty != null)
                {
                    ConvertFormat = scoreProperty.DataType;
                }
                if (mScoreObject != null && scoreProperty != null)
                {
                    var objValue = scoreProperty.GetPropertyValue(mScoreObject);
                    if (objValue != null)
                    {
                        Value = objValue.ToString();
                        if (objValue is Color)
                        {
                            ColorValue = (Color)objValue;
                        }
                        if (objValue is Boolean)
                        {
                            BoolValue = (bool) objValue;
                        }
                    }
                }
                InitSelectValueItems();
                ShowValue();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSelectValueItems()
        {
            try
            {
                mListEnumValueItems.Clear();
                if (mScoreObject == null
                    || mScoreProperty == null) { return; }
                switch (ConvertFormat)
                {
                    case ScorePropertyDataType.Enum:
                        InitEnumValueItems();
                        break;
                    case ScorePropertyDataType.FontFamily:
                        InitFontFamilyValueItems();
                        break;
                    case ScorePropertyDataType.FontWeight:
                        InitFontWeightValueItems();
                        break;
                    case ScorePropertyDataType.ScoreItem:
                        InitScoreItemValueItems();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitEnumValueItems()
        {
            try
            {
                if (mScoreObject == null
                   || mScoreProperty == null) { return; }
                var type = mScoreProperty.ValueType;
                string[] enums = Enum.GetNames(type);
                for (int i = 0; i < enums.Length; i++)
                {
                    var strName = enums[i];
                    var temp = Enum.Parse(type, strName);
                    string strDisplay =
                        CurrentApp.GetLanguageInfo(string.Format("3101ENUM{0}{1}", type.Name, ((int) temp).ToString("000")),
                            temp.ToString());
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Display = strDisplay;
                    item.Description = strDisplay;
                    item.SortID = i;
                    item.Info = temp;
                    mListEnumValueItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitFontFamilyValueItems()
        {
            try
            {
                var fonts = Fonts.SystemFontFamilies;
                int count = 0;
                var listTemp = new List<PropertyValueEnumItem>();
                foreach (var fontFamily in fonts)
                {
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Display = fontFamily.ToString();
                    item.Display = fontFamily.ToString();
                    item.SortID = count;
                    item.Info = fontFamily;
                    count++;
                    listTemp.Add(item);
                }
                listTemp = listTemp.OrderBy(f => f.Display).ToList();
                for (int i = 0; i < listTemp.Count; i++)
                {
                    mListEnumValueItems.Add(listTemp[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitFontWeightValueItems()
        {
            try
            {
                var fontWeights = GetFontWeights();
                int count = 0;
                var listTemp = new List<PropertyValueEnumItem>();
                foreach (var fontWeight in fontWeights)
                {
                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                    item.Display = fontWeight.ToString();
                    item.Display = fontWeight.ToString();
                    item.SortID = count;
                    item.Info = fontWeight;
                    count++;
                    listTemp.Add(item);
                }
                listTemp = listTemp.OrderBy(f => f.Display).ToList();
                for (int i = 0; i < listTemp.Count; i++)
                {
                    mListEnumValueItems.Add(listTemp[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitScoreItemValueItems()
        {
            try
            {
                if (mScoreObject == null
                  || mScoreProperty == null) { return; }
                switch (mScoreProperty.ID)
                {
                    case ScoreProperty.PRO_CTL_SOURCE:
                    case ScoreProperty.PRO_CTL_TARGET:
                        var ctlItem = mScoreObject as ControlItem;
                        if (ctlItem != null)
                        {
                            var scoreSheet = ctlItem.ScoreSheet;
                            if (scoreSheet != null)
                            {
                                List<ScoreItem> listItems = new List<ScoreItem>();
                                scoreSheet.GetAllScoreItem(ref listItems);
                                if (mScoreProperty.ID == ScoreProperty.PRO_CTL_TARGET)
                                {
                                    listItems.Add(scoreSheet);
                                }
                                for (int i = 0; i < listItems.Count; i++)
                                {
                                    var scoreItem = listItems[i];
                                    PropertyValueEnumItem item = new PropertyValueEnumItem();
                                    item.Display = scoreItem.Title;
                                    item.Description = scoreItem.Description;
                                    item.SortID = i;
                                    item.Info = scoreItem;
                                    mListEnumValueItems.Add(item);
                                }
                            }
                        }
                        break;
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
                if (mScoreObject == null
                    || mScoreProperty == null) { return; }
                switch (ConvertFormat)
                {
                    case ScorePropertyDataType.Enum:
                    case ScorePropertyDataType.FontFamily:
                    case ScorePropertyDataType.FontWeight:
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            var temp = mListEnumValueItems[i];
                            var info = temp.Info;
                            if (info == null) { continue; }
                            var value = mScoreProperty.GetPropertyValue(mScoreObject);
                            if (value == null) { continue; }
                            if (info.ToString() == value.ToString())
                            {
                                if (mItemSelectControl != null)
                                {
                                    mItemSelectControl.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case ScorePropertyDataType.ScoreItem:
                        for (int i = 0; i < mListEnumValueItems.Count; i++)
                        {
                            var temp = mListEnumValueItems[i];
                            var info = temp.Info;
                            if (info == null) { continue; }
                            var scoreItem = info as ScoreItem;
                            if (scoreItem == null) { continue; }
                            var value = mScoreProperty.GetPropertyValue(mScoreObject) as ScoreItem;
                            if (value == null) { continue; }
                            if (scoreItem.ID == value.ID)
                            {
                                if (mItemSelectControl != null)
                                {
                                    mItemSelectControl.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private IEnumerable<FontWeight> GetFontWeights()
        {
            yield return FontWeights.Black;
            yield return FontWeights.Bold;
            yield return FontWeights.ExtraBlack;
            yield return FontWeights.ExtraBold;
            yield return FontWeights.ExtraLight;
            yield return FontWeights.Light;
            yield return FontWeights.Medium;
            yield return FontWeights.Normal;
            yield return FontWeights.SemiBold;
            yield return FontWeights.Thin;
        }

        #endregion


        #region Others

        public void RefreshValue()
        {
            Init();
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {

            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion

    }
}
