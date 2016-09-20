//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7fdc57ab-1882-480d-a1aa-c319d9f63a32
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyChildList
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner
//        File Name:                PropertyChildList
//
//        created by Charley at 2014/7/29 14:23:29
//        http://www.voicecyber.com
//
//======================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UMPScoreSheetDesigner.Commands;
using UMPScoreSheetDesigner.Models;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPScoreSheetDesigner
{
    /// <summary>
    /// PropertyChildList.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyChildList
    {
        public static readonly DependencyProperty ScoreObjectProperty =
            DependencyProperty.Register("ScoreObject", typeof(ScoreObject), typeof(PropertyChildList), new PropertyMetadata(default(ScoreObject)));

        public ScoreObject ScoreObject
        {
            get { return (ScoreObject)GetValue(ScoreObjectProperty); }
            set { SetValue(ScoreObjectProperty, value); }
        }

        public static readonly DependencyProperty ChildNameProperty =
            DependencyProperty.Register("ChildName", typeof(string), typeof(PropertyChildList), new PropertyMetadata(default(string)));

        public string ChildName
        {
            get { return (string)GetValue(ChildNameProperty); }
            set { SetValue(ChildNameProperty, value); }
        }

        public static readonly RoutedEvent SelectedItemChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<ScoreObject>), typeof(PropertyChildList));

        public event RoutedPropertyChangedEventHandler<ScoreObject> SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
        }

        private List<ButtonItem> mListAddRemoveButtons;
        private ObservableCollection<ScoreObject> mListChildItems;

        public PropertyChildList()
        {
            InitializeComponent();

            mListAddRemoveButtons = new List<ButtonItem>();
            mListChildItems = new ObservableCollection<ScoreObject>();
        }

        private void PropertyChildList_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitAddRemoveButtons();
            CreateAddRemoveButtons();
            InitChildItems();
            ListChild.SelectionChanged += ListChild_SelectionChanged;
            ListChild.ItemsSource = mListChildItems;
        }

        public void InitChildItems()
        {
            mListChildItems.Clear();
            var itemStandard = ScoreObject as ItemStandard;
            if (itemStandard != null)
            {
                for (int i = 0; i < itemStandard.ValueItems.Count; i++)
                {
                    mListChildItems.Add(itemStandard.ValueItems[i]);
                }
            }
            var itemComment = ScoreObject as ItemComment;
            if (itemComment != null)
            {
                for (int i = 0; i < itemComment.ValueItems.Count; i++)
                {
                    mListChildItems.Add(itemComment.ValueItems[i]);
                }
            }
            var scoreSheet = ScoreObject as ScoreSheet;
            if (scoreSheet != null)
            {
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    mListChildItems.Add(scoreSheet.ControlItems[i]);
                }
            }
        }

        public ScoreObject GetSelectedItem()
        {
            var selectedItem = ListChild.SelectedItem as ScoreObject;
            if (selectedItem != null)
            {
                return selectedItem;
            }
            return null;
        }

        public void MoveScoreObject(string method)
        {
            ScoreObject selectedItem = GetSelectedItem();
            if (selectedItem == null) { return; }
            int index;
            index = mListChildItems.IndexOf(selectedItem);
            if (index > 0 && method == "Up")
            {
                var standardItem = selectedItem as StandardItem;
                var itemStandard = ScoreObject as ItemStandard;
                if (standardItem != null && itemStandard != null)
                {
                    itemStandard.ValueItems.Remove(standardItem);
                    itemStandard.ValueItems.Insert(index - 1, standardItem);
                }
                var commentItem = selectedItem as CommentItem;
                var itemComment = ScoreObject as ItemComment;
                if (commentItem != null && itemComment != null)
                {
                    itemComment.ValueItems.Remove(commentItem);
                    itemComment.ValueItems.Insert(index - 1, commentItem);
                }
                var controlItem = selectedItem as ControlItem;
                var scoreSheet = ScoreObject as ScoreSheet;
                if (controlItem != null && scoreSheet != null)
                {
                    scoreSheet.ControlItems.Remove(controlItem);
                    scoreSheet.ControlItems.Insert(index - 1, controlItem);
                }
                mListChildItems.Remove(selectedItem);
                mListChildItems.Insert(index - 1, selectedItem);
                ListChild.SelectedItem = selectedItem;
            }
            else if (index < mListChildItems.Count - 1 && method == "Down")
            {
                var standardItem = selectedItem as StandardItem;
                var itemStandard = ScoreObject as ItemStandard;
                if (standardItem != null && itemStandard != null)
                {
                    itemStandard.ValueItems.Remove(standardItem);
                    itemStandard.ValueItems.Insert(index + 1, standardItem);
                }
                var commentItem = selectedItem as CommentItem;
                var itemComment = ScoreObject as ItemComment;
                if (commentItem != null && itemComment != null)
                {
                    itemComment.ValueItems.Remove(commentItem);
                    itemComment.ValueItems.Insert(index + 1, commentItem);
                }
                var controlItem = selectedItem as ControlItem;
                var scoreSheet = ScoreObject as ScoreSheet;
                if (controlItem != null && scoreSheet != null)
                {
                    scoreSheet.ControlItems.Remove(controlItem);
                    scoreSheet.ControlItems.Insert(index + 1, controlItem);
                }
                mListChildItems.Remove(selectedItem);
                mListChildItems.Insert(index + 1, selectedItem);
                ListChild.SelectedItem = selectedItem;
            }
        }

        public ObservableCollection<ScoreObject> GetChildList()
        {
            return mListChildItems;
        }

        void ListChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScoreObject oldValue = null;
            ScoreObject newValue = null;
            if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                oldValue = e.RemovedItems[0] as ScoreObject;
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                newValue = e.AddedItems[0] as ScoreObject;
            }
            RoutedPropertyChangedEventArgs<ScoreObject> args = new RoutedPropertyChangedEventArgs<ScoreObject>(
                oldValue, newValue);
            args.RoutedEvent = SelectedItemChangedEvent;
            RaiseEvent(args);
        }

        private void InitAddRemoveButtons()
        {
            mListAddRemoveButtons.Clear();
            mListAddRemoveButtons.Add(new ButtonItem
            {
                Name = "Add",
                Header = "Add",
                Icon = "Images/add.png",
            });
            mListAddRemoveButtons.Add(new ButtonItem
            {
                Name = "Remove",
                Header = "Remove",
                Icon = "Images/remove.png",
            });
            mListAddRemoveButtons.Add(new ButtonItem
            {
                Name = "Up",
                Header = "Up",
                Icon = "Images/up.png",
            });
            mListAddRemoveButtons.Add(new ButtonItem
            {
                Name = "Down",
                Header = "Down",
                Icon = "Images/down.png",
            });
        }

        private void CreateAddRemoveButtons()
        {
            PanelAddRemoveButton.Children.Clear();
            for (int i = 0; i < mListAddRemoveButtons.Count; i++)
            {
                ButtonItem item = mListAddRemoveButtons[i];
                Button btn = new Button();
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "AddRemoveButtonStyle");
                ChildListCommandArgs args = new ChildListCommandArgs();
                args.Name = item.Name;
                args.Data = this;
                btn.Command = MainWindowCommands.ChildListCommand;
                btn.CommandParameter = args;
                PanelAddRemoveButton.Children.Add(btn);
            }
        }
    }
}
