//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    94516508-690f-41ec-b7ec-dd3059cc9b67
//        CLR Version:              4.0.30319.18063
//        Name:                     UCPropertyChildList
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                UCPropertyChildList
//
//        created by Charley at 2015/11/10 10:04:56
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UMPS3101.Commands;
using UMPS3101.Models;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101
{
    /// <summary>
    /// UCPropertyChildList.xaml 的交互逻辑
    /// </summary>
    public partial class UCPropertyChildList
    {
        public UCPropertyChildList()
        {
            InitializeComponent();

            mListAddRemoveButtons = new List<ButtonItem>();
            mListChildItems = new ObservableCollection<ScoreChildInfoItem>();

            Loaded += UCPropertyChildList_Loaded;
            ListChild.SelectionChanged += ListChild_SelectionChanged;
        }

        void UCPropertyChildList_Loaded(object sender, RoutedEventArgs e)
        {
            ListChild.ItemsSource = mListChildItems;

            InitAddRemoveButtons();
            CreateAddRemoveButtons();
            Init();
        }


        #region ScoreObjectProperty

        public static readonly DependencyProperty ScoreObjectProperty =
            DependencyProperty.Register("ScoreObject", typeof(ScoreObject), typeof(UCPropertyChildList), new PropertyMetadata(default(ScoreObject)));

        public ScoreObject ScoreObject
        {
            get { return (ScoreObject)GetValue(ScoreObjectProperty); }
            set { SetValue(ScoreObjectProperty, value); }
        }

        #endregion


        #region ChildNameProperty

        public static readonly DependencyProperty ChildNameProperty =
            DependencyProperty.Register("ChildName", typeof(string), typeof(UCPropertyChildList), new PropertyMetadata(default(string)));

        public string ChildName
        {
            get { return (string)GetValue(ChildNameProperty); }
            set { SetValue(ChildNameProperty, value); }
        }

        #endregion


        #region SelectedItemChangedEvent

        public static readonly RoutedEvent SelectedItemChangedEvent =
           EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<ScoreChildInfoItem>), typeof(UCPropertyChildList));

        public event RoutedPropertyChangedEventHandler<ScoreChildInfoItem> SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
        }

        #endregion


        #region Members

        public SSDMainView PageParent;

        private List<ButtonItem> mListAddRemoveButtons;
        private ObservableCollection<ScoreChildInfoItem> mListChildItems;

        #endregion


        #region Init and Load

        private void Init()
        {
            try
            {
                InitChildItems();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitChildItems()
        {
            mListChildItems.Clear();
            var itemStandard = ScoreObject as ItemStandard;
            if (itemStandard != null)
            {
                ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_I_VALUEITEMS.ToString("000")), "Standard Item");
                for (int i = 0; i < itemStandard.ValueItems.Count; i++)
                {
                    var child = itemStandard.ValueItems[i];
                    ScoreChildInfoItem item = new ScoreChildInfoItem();
                    item.ParentObject = itemStandard;
                    item.SortID = i;
                    item.Display = child.Display;
                    item.Data = child;
                    mListChildItems.Add(item);
                }
            }
            var itemComment = ScoreObject as ItemComment;
            if (itemComment != null)
            {
                ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_C_VALUEITEMS.ToString("000")), "Comment Item");
                for (int i = 0; i < itemComment.ValueItems.Count; i++)
                {
                    var child = itemComment.ValueItems[i];
                    ScoreChildInfoItem item = new ScoreChildInfoItem();
                    item.ParentObject = itemComment;
                    item.SortID = i;
                    item.Display = child.Display;
                    item.Data = child;
                    mListChildItems.Add(item);
                }
            }
            var scoreSheet = ScoreObject as ScoreSheet;
            if (scoreSheet != null)
            {
                ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_CONTROLITEMS.ToString("000")), "Control Item");
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    var child = scoreSheet.ControlItems[i];
                    ScoreChildInfoItem item = new ScoreChildInfoItem();
                    item.ParentObject = scoreSheet;
                    item.SortID = i;
                    item.Display = child.Display;
                    item.Data = child;
                    mListChildItems.Add(item);
                }
            }
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
                btn.Command = SSDPageCommands.ChildListCommand;
                btn.CommandParameter = args;
                PanelAddRemoveButton.Children.Add(btn);
            }
        }

        #endregion


        #region Operations

        public void MoveScoreObject(string method)
        {
            ScoreChildInfoItem selectedItem = GetSelectedItem();
            if (selectedItem == null) { return; }
            int index;
            index = mListChildItems.IndexOf(selectedItem);
            if (index > 0 && method == "Up")
            {
                var standardItem = selectedItem.Data as StandardItem;
                var itemStandard = ScoreObject as ItemStandard;
                if (standardItem != null && itemStandard != null)
                {
                    itemStandard.ValueItems.Remove(standardItem);
                    itemStandard.ValueItems.Insert(index - 1, standardItem);
                    ResetScoreObjectOrder(selectedItem, itemStandard);
                }
                var commentItem = selectedItem.Data as CommentItem;
                var itemComment = ScoreObject as ItemComment;
                if (commentItem != null && itemComment != null)
                {
                    itemComment.ValueItems.Remove(commentItem);
                    itemComment.ValueItems.Insert(index - 1, commentItem);
                    ResetScoreObjectOrder(selectedItem, itemComment);
                }
                var controlItem = selectedItem.Data as ControlItem;
                var scoreSheet = ScoreObject as ScoreSheet;
                if (controlItem != null && scoreSheet != null)
                {
                    scoreSheet.ControlItems.Remove(controlItem);
                    scoreSheet.ControlItems.Insert(index - 1, controlItem);
                    ResetScoreObjectOrder(selectedItem, scoreSheet);
                }
                selectedItem.SortID--;
                mListChildItems.Remove(selectedItem);
                mListChildItems.Insert(index - 1, selectedItem);
                ListChild.SelectedItem = selectedItem;
            }
            else if (index < mListChildItems.Count - 1 && method == "Down")
            {
                var standardItem = selectedItem.Data as StandardItem;
                var itemStandard = ScoreObject as ItemStandard;
                if (standardItem != null && itemStandard != null)
                {
                    itemStandard.ValueItems.Remove(standardItem);
                    itemStandard.ValueItems.Insert(index + 1, standardItem);
                    ResetScoreObjectOrder(selectedItem, itemStandard);
                }
                var commentItem = selectedItem.Data as CommentItem;
                var itemComment = ScoreObject as ItemComment;
                if (commentItem != null && itemComment != null)
                {
                    itemComment.ValueItems.Remove(commentItem);
                    itemComment.ValueItems.Insert(index + 1, commentItem);
                    ResetScoreObjectOrder(selectedItem, itemComment);
                }
                var controlItem = selectedItem.Data as ControlItem;
                var scoreSheet = ScoreObject as ScoreSheet;
                if (controlItem != null && scoreSheet != null)
                {
                    scoreSheet.ControlItems.Remove(controlItem);
                    scoreSheet.ControlItems.Insert(index + 1, controlItem);
                    ResetScoreObjectOrder(selectedItem, scoreSheet);
                }
                selectedItem.SortID++;
                mListChildItems.Remove(selectedItem);
                mListChildItems.Insert(index + 1, selectedItem);
                ListChild.SelectedItem = selectedItem;
            }
        }

        public void RefreshItemDisplay()
        {
            try
            {
                for (int i = 0; i < mListChildItems.Count; i++)
                {
                    var item = mListChildItems[i];
                    var data = item.Data;
                    if (data != null)
                    {
                        var itemStandard = ScoreObject as ItemStandard;
                        if (itemStandard != null)
                        {
                            ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_I_VALUEITEMS.ToString("000")), "Standard Item");
                            var child = data as StandardItem;
                            if (child != null)
                            {
                                item.Display = child.Display;
                            }
                        }
                        var itemComment = ScoreObject as ItemComment;
                        if (itemComment != null)
                        {
                            ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_C_VALUEITEMS.ToString("000")), "Comment Item");
                            var child = data as CommentItem;
                            if (child != null)
                            {
                                item.Display = child.Display;
                            }
                        }
                        var scoreSheet = ScoreObject as ScoreSheet;
                        if (scoreSheet != null)
                        {
                            ChildName = GetLanguage("PropertyViewer", string.Format("PRO301{0}", ScoreProperty.PRO_CONTROLITEMS.ToString("000")), "Control Item");
                            var child = data as ControlItem;
                            if (child != null)
                            {
                                item.Display = child.Display;
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

        private void ResetScoreObjectOrder(ScoreChildInfoItem item, ScoreObject scoreObject)
        {
            //重新设置OrderID
            var scoreSheet = scoreObject as ScoreSheet;
            ScoreObject itemObject = item.Data;
            if (itemObject == null) { return; }
            var controlItem = itemObject as ControlItem;
            if (scoreSheet != null && controlItem != null)
            {
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    scoreSheet.ControlItems[i].OrderID = i;
                }
            }
            var itemComment = scoreObject as ItemComment;
            var commentItem = itemObject as CommentItem;
            if (itemComment != null && commentItem != null)
            {
                for (int i = 0; i < itemComment.ValueItems.Count; i++)
                {
                    itemComment.ValueItems[i].OrderID = i;
                }
            }
            var itemStandard = scoreObject as ItemStandard;
            var standardItem = itemObject as StandardItem;
            if (itemStandard != null && standardItem != null)
            {
                for (int i = 0; i < itemStandard.ValueItems.Count; i++)
                {
                    itemStandard.ValueItems[i].OrderID = i;
                }
            }
        }

        #endregion


        #region EventHandlers

        void ListChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScoreChildInfoItem oldValue = null;
            ScoreChildInfoItem newValue = null;
            if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                oldValue = e.RemovedItems[0] as ScoreChildInfoItem;
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                newValue = e.AddedItems[0] as ScoreChildInfoItem;
            }
            RoutedPropertyChangedEventArgs<ScoreChildInfoItem> args = new RoutedPropertyChangedEventArgs<ScoreChildInfoItem>(
                oldValue, newValue);
            args.RoutedEvent = SelectedItemChangedEvent;
            RaiseEvent(args);
        }

        #endregion


        #region Others

        public ScoreChildInfoItem GetSelectedItem()
        {
            var selectedItem = ListChild.SelectedItem as ScoreChildInfoItem;
            if (selectedItem != null)
            {
                return selectedItem;
            }
            return null;
        }

        public ObservableCollection<ScoreChildInfoItem> GetChildList()
        {
            return mListChildItems;
        }

        private string GetLanguage(string category, string code, string display)
        {
            if (PageParent != null)
            {
                return PageParent.GetLanguage(category, code, display);
            }
            return display;
        }

        #endregion

    }
}
