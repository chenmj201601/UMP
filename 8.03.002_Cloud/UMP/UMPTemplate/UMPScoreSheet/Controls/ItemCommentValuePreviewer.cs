//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f1b40c88-6c34-4d9b-a8ce-d30141fdda97
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemCommentValuePreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ItemCommentValuePreviewer
//
//        created by Charley at 2014/8/6 15:02:41
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
    public class ItemCommentValuePreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty ItemCommentProperty =
            DependencyProperty.Register("ItemComment", typeof(ItemComment), typeof(ItemCommentValuePreviewer), new PropertyMetadata(default(ItemComment)));

        public ItemComment ItemComment
        {
            get { return (ItemComment)GetValue(ItemCommentProperty); }
            set { SetValue(ItemCommentProperty, value); }
        }

        static ItemCommentValuePreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemCommentValuePreviewer),
                new FrameworkPropertyMetadata(typeof(ItemCommentValuePreviewer)));
        }

        private ObservableCollection<CommentItem> mListCommentItems;

        public ItemCommentValuePreviewer()
        {
            mListCommentItems = new ObservableCollection<CommentItem>();
        }

        public override void Init()
        {
            base.Init();

            InitCommentItems();

            if (ItemComment.SelectValue != null && mValue != null)
            {
                CommentItem temp = ItemComment.ValueItems.FirstOrDefault(i => i.ID == ItemComment.SelectValue.ID);
                if (temp != null)
                {
                    mValue.SelectedItem = temp;
                }
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
                    ItemComment.SelectValue = selectItem;
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
