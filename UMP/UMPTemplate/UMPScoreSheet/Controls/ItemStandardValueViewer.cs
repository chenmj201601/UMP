//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d45dac19-6431-48ec-826a-61b01647cce5
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ItemStandardValueViewer
//
//        created by Charley at 2014/8/12 11:19:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value, Type = typeof(ComboBox))]
    public class ItemStandardValueViewer : ScoreObjectViewer
    {
        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Border mBorderPanel;
        private ComboBox mValue;

        public static readonly DependencyProperty ItemStandardProperty =
            DependencyProperty.Register("ItemStandard", typeof(ItemStandard), typeof(ItemStandardValueViewer), new PropertyMetadata(default(ItemStandard)));

        public ItemStandard ItemStandard
        {
            get { return (ItemStandard)GetValue(ItemStandardProperty); }
            set { SetValue(ItemStandardProperty, value); }
        }

        private ObservableCollection<StandardItem> mListStandardItems;

        static ItemStandardValueViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemStandardValueViewer),
                new FrameworkPropertyMetadata(typeof(ItemStandardValueViewer)));
        }

        public ItemStandardValueViewer()
        {
            mListStandardItems = new ObservableCollection<StandardItem>();
        }

        public override void Init()
        {
            base.Init();
            InitStandardItems();
            SetCurrentItem();
        }

        private void SetCurrentItem()
        {
            if (ItemStandard != null && mValue != null)
            {
                StandardItem item;
                ScoreSheet scoreSheet = ItemStandard.ScoreSheet;
                if (scoreSheet != null)
                {
                    //如果是修改成绩或查看成绩，显示实际成绩，否则显示默认值
                    if (ViewMode == 1 || ViewMode == 2)
                    {
                        for (int i = 0; i < ItemStandard.ValueItems.Count; i++)
                        {
                            item = ItemStandard.ValueItems[i];
                            var temp = mListStandardItems.FirstOrDefault(s => s.ID == item.ID && s.Value == ItemStandard.Score);
                            if (temp != null)
                            {
                                mValue.SelectedItem = temp;
                            }
                        }
                        //查看模式，控件不可用
                        if (ViewMode == 2)
                        {
                            mValue.IsEnabled = false;
                        }
                        return;
                    }
                }
                if (ItemStandard.DefaultValue == null) { return; }
                item = mListStandardItems.FirstOrDefault(i => i.ID == ItemStandard.DefaultValue.ID);
                mValue.SelectedItem = item;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mValue = GetTemplateChild(PART_Value) as ComboBox;
            if (mValue != null)
            {
                mValue.ItemsSource = mListStandardItems;
                mValue.SelectionChanged += mValue_SelectionChanged;
            }
        }

        void mValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mValue != null)
            {
                var selectItem = mValue.SelectedItem as StandardItem;
                if (selectItem != null)
                {
                    ItemStandard.SelectValue = selectItem;
                    ItemStandard.Score = selectItem.Value;

                    PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                    args.ScoreObject = ItemStandard;
                    args.PropertyName = "Score";
                    ItemStandard.PropertyChanged(ItemStandard, args);
                }
            }
        }

        private void InitStandardItems()
        {
            if (ItemStandard != null)
            {
                mListStandardItems.Clear();
                for (int i = 0; i < ItemStandard.ValueItems.Count; i++)
                {
                    mListStandardItems.Add(ItemStandard.ValueItems[i]);
                }
            }
        }
    }
}
