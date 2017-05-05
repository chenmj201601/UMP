//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bd3f3f16-8118-4046-942a-c8c1c77dfb01
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemCommentValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ItemCommentValueViewer
//
//        created by Charley at 2014/8/12 11:50:20
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
    public class ItemCommentValueViewer : ScoreObjectViewer
    {
        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Border mBorderPanel;
        private ComboBox mValue;

        public static readonly DependencyProperty ItemCommentProperty =
            DependencyProperty.Register("ItemComment", typeof (ItemComment), typeof (ItemCommentValueViewer), new PropertyMetadata(default(ItemComment)));

        public ItemComment ItemComment
        {
            get { return (ItemComment) GetValue(ItemCommentProperty); }
            set { SetValue(ItemCommentProperty, value); }
        }

        private ObservableCollection<CommentItem> mListCommentItems;

        static ItemCommentValueViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ItemCommentValueViewer),
                new FrameworkPropertyMetadata(typeof (ItemCommentValueViewer)));
        }

        public ItemCommentValueViewer()
        {
            mListCommentItems = new ObservableCollection<CommentItem>();
        }

        public override void Init()
        {
            base.Init();

            InitCommentItems();

            if (ItemComment.SelectItem != null && mValue != null)
            {
                CommentItem temp = ItemComment.ValueItems.FirstOrDefault(i => i.ID == ItemComment.SelectItem.ID);
                if (temp != null)
                {
                    mValue.SelectedItem = temp;
                }
            }
            if (ViewMode == 2)
            {
                IsEnabled = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mValue = GetTemplateChild(PART_Value) as ComboBox;
            if (mValue != null)
            {
                mValue.ItemsSource = mListCommentItems;
                mValue.SelectionChanged += mValue_SelectionChanged;
            }
        }

        void mValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mValue != null)
            {
                var selectItem = mValue.SelectedItem as CommentItem;
                if (selectItem != null)
                {
                    ItemComment.SelectItem = selectItem;
                }
            }
        }

        private void InitCommentItems()
        {
            if (ItemComment != null)
            {
                mListCommentItems.Clear();
                for (int i = 0; i < ItemComment.ValueItems.Count; i++)
                {
                    mListCommentItems.Add(ItemComment.ValueItems[i]);
                }
            }
        }
    }
}
