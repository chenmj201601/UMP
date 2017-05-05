//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    750799ac-e851-43d8-8a3f-edf26df3d432
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemStandardValuePreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ItemStandardValuePreviewer
//
//        created by Charley at 2014/8/6 14:04:51
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
    public class ItemStandardValuePreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty ItemStandardProperty =
            DependencyProperty.Register("ItemStandard", typeof(ItemStandard), typeof(ItemStandardValuePreviewer), new PropertyMetadata(default(ItemStandard)));

        public ItemStandard ItemStandard
        {
            get { return (ItemStandard)GetValue(ItemStandardProperty); }
            set { SetValue(ItemStandardProperty, value); }
        }
        private ObservableCollection<StandardItem> mListStandardItems;

        static ItemStandardValuePreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemStandardValuePreviewer),
                new FrameworkPropertyMetadata(typeof(ItemStandardValuePreviewer)));
        }

        public ItemStandardValuePreviewer()
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
                        if (ItemStandard.SelectValue == null) { return; }
                        item = mListStandardItems.FirstOrDefault(i => i.ID == ItemStandard.SelectValue.ID);
                        if (item != null)
                        {
                            mValue.SelectedItem = item;
                        }
                        return;
                    }
                }
                if (ItemStandard.DefaultValue == null) { return; }
                item = mListStandardItems.FirstOrDefault(i => i.ID == ItemStandard.DefaultValue.ID);
                mValue.SelectedItem = item;
            }
        }

        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Border mBorderPanel;
        private ComboBox mValue;

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
