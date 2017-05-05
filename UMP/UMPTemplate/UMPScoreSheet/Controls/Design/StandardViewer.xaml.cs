//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    72e0859a-5016-4b4b-ab15-7649bb38db57
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                StandardViewer
//
//        created by Charley at 2014/6/18 16:36:02
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// StandardViewer.xaml 的交互逻辑
    /// </summary>
    public partial class StandardViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// Standard
        /// </summary>
        public Standard Standard;
        /// <summary>
        /// ViewClassic
        /// </summary>
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// 设置信息
        /// </summary>
        public List<ScoreSetting> Settings { get; set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public List<ScoreLangauge> Languages { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        public int LangID { get; set; }
        /// <summary>
        /// New StandardViewer
        /// </summary>
        public StandardViewer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            CreateTitle();
            CreateTip();
            CreateScoreItemValue();
            SetPanelStyle(Standard.PanelStyle);
            SetNAAvaliable();
            SetStandardValueWidth();
        }

        private void StandardViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Standard;
            ScoreObject socreObject = Standard;
            if (socreObject != null)
            {
                socreObject.OnPropertyChanged += ScoreObject_OnPropertyChanged;
            }
            CbNATable.Click += CheckBoxNA_Click;
            CbNATree.Click += CheckBoxNA_Click;
            Init();
        }

        private void CheckBoxNA_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;
            if (checkBox != null)
            {
                if (checkBox.IsChecked == true)
                {
                    CbNATable.IsChecked = true;
                    CbNATree.IsChecked = true;
                    BorderValueTable.IsEnabled = false;
                    BorderValueTree.IsEnabled = false;
                    Standard.IsNA = true;
                }
                else
                {
                    CbNATable.IsChecked = false;
                    CbNATree.IsChecked = false;
                    BorderValueTable.IsEnabled = true;
                    BorderValueTree.IsEnabled = true;
                    Standard.IsNA = false;
                }
            }
        }

        void ScoreObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetPanelStyle(Standard.PanelStyle);
            if (e.PropertyName == "ViewClassic")
            {
                ViewClassic = (ScoreItemClassic)e.NewValue;
                CreateTitle();
                CreateTip();
                CreateScoreItemValue();
                SetNAAvaliable();
                SetStandardValueWidth();
            }
        }

        private void SetNAAvaliable()
        {
            if (Standard == null)
            {
                return;
            }
            if (Standard.IsAllowNA)
            {
                CbNATable.Visibility = Visibility.Visible;
                CbNATree.Visibility = Visibility.Visible;
            }
            else
            {
                CbNATable.Visibility = Visibility.Collapsed;
                CbNATree.Visibility = Visibility.Collapsed;
            }
            if (Standard.IsNA)
            {
                CbNATable.IsChecked = true;
                CbNATree.IsChecked = true;
            }
            else
            {
                CbNATable.IsChecked = false;
                CbNATree.IsChecked = false;
            }
        }

        private void SetPanelStyle(VisualStyle style)
        {
            if (style != null)
            {
                if (style.Width != 0)
                {
                    BorderPanel.Width = style.Width;
                }
                else
                {
                    BorderPanel.ClearValue(WidthProperty);
                }
                if (style.Height != 0)
                {
                    BorderPanel.Height = style.Height;
                }
                else
                {
                    BorderPanel.ClearValue(HeightProperty);
                }
                BorderPanel.Background = new SolidColorBrush(style.BackColor);
            }
        }

        private void SetStandardValueWidth()
        {
            if(Settings==null){return;}
            ScoreSetting setting = Settings.FirstOrDefault(s => s.Category == "S" && s.Code == "V_WIDTH");
            if (setting != null)
            {
                int intValue;
                if (int.TryParse(setting.Value, out intValue))
                {
                    GridValueTree.Width = new GridLength(intValue);
                }
            }
        }

        private void CreateTitle()
        {
            if (Standard != null)
            {
                ScoreItemTitleViewer titleViewer = new ScoreItemTitleViewer();
                titleViewer.ScoreItem = Standard;
                titleViewer.ViewClassic = ViewClassic;
                titleViewer.Settings = Settings;
                titleViewer.LangID = LangID;
                titleViewer.Languages = Languages;
                BorderTitleTable.Child = titleViewer;
                titleViewer = new ScoreItemTitleViewer();
                titleViewer.ScoreItem = Standard;
                titleViewer.ViewClassic = ViewClassic;
                titleViewer.Settings = Settings;
                titleViewer.LangID = LangID;
                titleViewer.Languages = Languages;
                BorderTitleTree.Child = titleViewer;
            }
        }

        private void CreateScoreItemValue()
        {
            if (Standard == null) { return; }
            if (ViewClassic == ScoreItemClassic.Table)
            {
                GridTable.Visibility = Visibility.Visible;
                GridTree.Visibility = Visibility.Collapsed;
                switch (Standard.StandardType)
                {
                    case StandardType.Numeric:
                        NumericStandard numericStandard = Standard as NumericStandard;
                        if (numericStandard == null)
                        {
                            return;
                        }
                        TextBoxStandardValueViewer textBoxStandardValue = new TextBoxStandardValueViewer();
                        textBoxStandardValue.ViewClassic = Standard.ViewClassic;
                        textBoxStandardValue.Settings = Settings;
                        textBoxStandardValue.LangID = LangID;
                        textBoxStandardValue.Languages = Languages;
                        textBoxStandardValue.NumericStandard = numericStandard;
                        BorderValueTable.Child = textBoxStandardValue;
                        break;
                    case StandardType.YesNo:
                        YesNoStandard yesNoStandard = Standard as YesNoStandard;
                        if (yesNoStandard == null)
                        {
                            return;
                        }
                        RadioYesNoStandardValueViewer yesNoStandardValue = new RadioYesNoStandardValueViewer();
                        yesNoStandardValue.ViewClassic = Standard.ViewClassic;
                        yesNoStandardValue.Settings = Settings;
                        yesNoStandardValue.LangID = LangID;
                        yesNoStandardValue.Languages = Languages;
                        yesNoStandardValue.YesNoStandard = yesNoStandard;
                        BorderValueTable.Child = yesNoStandardValue;
                        break;
                    case StandardType.Item:
                        ItemStandard itemStandard = Standard as ItemStandard;
                        if (itemStandard == null)
                        {
                            return;
                        }
                        DropDownItemStandardValueViewer dropDownItemStandardValue =
                            new DropDownItemStandardValueViewer();
                        dropDownItemStandardValue.ViewClassic = Standard.ViewClassic;
                        dropDownItemStandardValue.Settings = Settings;
                        dropDownItemStandardValue.LangID = LangID;
                        dropDownItemStandardValue.Languages = Languages;
                        dropDownItemStandardValue.ItemStandard = itemStandard;
                        BorderValueTable.Child = dropDownItemStandardValue;
                        break;
                    case StandardType.Slider:
                        SliderStandard sliderStandard = Standard as SliderStandard;
                        if (sliderStandard == null)
                        {
                            return;
                        }
                        SliderStandardValueViewer sliderStandardValue = new SliderStandardValueViewer();
                        sliderStandardValue.ViewClassic = Standard.ViewClassic;
                        sliderStandardValue.Settings = Settings;
                        sliderStandardValue.LangID = LangID;
                        sliderStandardValue.Languages = Languages;
                        sliderStandardValue.SliderStandard = sliderStandard;
                        BorderValueTable.Child = sliderStandardValue;
                        break;
                }
            }
            else
            {
                GridTable.Visibility = Visibility.Collapsed;
                GridTree.Visibility = Visibility.Visible;
                switch (Standard.StandardType)
                {
                    case StandardType.Numeric:
                        NumericStandard numericStandard = Standard as NumericStandard;
                        if (numericStandard == null) { return; }
                        TextBoxStandardValueViewer textBoxStandardValue = new TextBoxStandardValueViewer();
                        textBoxStandardValue.ViewClassic = Standard.ViewClassic;
                        textBoxStandardValue.Settings = Settings;
                        textBoxStandardValue.LangID = LangID;
                        textBoxStandardValue.Languages = Languages;
                        textBoxStandardValue.NumericStandard = numericStandard;
                        BorderValueTree.Child = textBoxStandardValue;
                        break;
                    case StandardType.YesNo:
                        YesNoStandard yesNoStandard = Standard as YesNoStandard;
                        if (yesNoStandard == null) { return; }
                        RadioYesNoStandardValueViewer yesNoStandardValue = new RadioYesNoStandardValueViewer();
                        yesNoStandardValue.ViewClassic = Standard.ViewClassic;
                        yesNoStandardValue.Settings = Settings;
                        yesNoStandardValue.LangID = LangID;
                        yesNoStandardValue.Languages = Languages;
                        yesNoStandardValue.YesNoStandard = yesNoStandard;
                        BorderValueTree.Child = yesNoStandardValue;
                        break;
                    case StandardType.Item:
                        ItemStandard itemStandard = Standard as ItemStandard;
                        if (itemStandard == null) { return; }
                        DropDownItemStandardValueViewer dropDownItemStandardValue = new DropDownItemStandardValueViewer();
                        dropDownItemStandardValue.ViewClassic = Standard.ViewClassic;
                        dropDownItemStandardValue.Settings = Settings;
                        dropDownItemStandardValue.LangID = LangID;
                        dropDownItemStandardValue.Languages = Languages;
                        dropDownItemStandardValue.ItemStandard = itemStandard;
                        BorderValueTree.Child = dropDownItemStandardValue;
                        break;
                    case StandardType.Slider:
                        SliderStandard sliderStandard = Standard as SliderStandard;
                        if (sliderStandard == null) { return; }
                        SliderStandardValueViewer sliderStandardValue = new SliderStandardValueViewer();
                        sliderStandard.ViewClassic = Standard.ViewClassic;
                        sliderStandardValue.Settings = Settings;
                        sliderStandardValue.LangID = LangID;
                        sliderStandardValue.Languages = Languages;
                        sliderStandardValue.SliderStandard = sliderStandard;
                        BorderValueTree.Child = sliderStandardValue;
                        break;
                }
            }
        }

        private void CreateTip()
        {
            if (Standard != null)
            {
                ScoreItemTipViewer tipViewer = new ScoreItemTipViewer();
                tipViewer.ScoreItem = Standard;
                tipViewer.ViewClassic = ViewClassic;
                tipViewer.Settings = Settings;
                tipViewer.LangID = LangID;
                tipViewer.Languages = Languages;
                tipViewer.HorizontalAlignment = HorizontalAlignment.Right;
                BorderTipTable.Child = tipViewer;
                tipViewer = new ScoreItemTipViewer();
                tipViewer.ScoreItem = Standard;
                tipViewer.ViewClassic = ViewClassic;
                tipViewer.Settings = Settings;
                tipViewer.LangID = LangID;
                tipViewer.Languages = Languages;
                tipViewer.HorizontalAlignment = HorizontalAlignment.Center;
                BorderTipTree.Child = tipViewer;
            }
        }

        private void SubViewerLoaded()
        {
            if (ViewerLoaded != null)
            {
                ViewerLoaded();
            }
        }
    }
}
