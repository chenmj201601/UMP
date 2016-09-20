//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a1ac7fa5-bcaf-4e54-86c9-20725c862e30
//        CLR Version:              4.0.30319.18063
//        Name:                     UCScorePropertyLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                UCScorePropertyLister
//
//        created by Charley at 2015/11/5 17:57:05
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
using UMPS3101.Models;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101
{
    /// <summary>
    /// UCScorePropertyLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCScorePropertyLister
    {

        static UCScorePropertyLister()
        {
            PropertyListerEventEvent = EventManager.RegisterRoutedEvent("PropertyListerEvent", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<PropertyListerEventEventArgs>), typeof(UCScorePropertyLister));
        }

        public UCScorePropertyLister()
        {
            InitializeComponent();

            mListScorePropertyInfoItems = new ObservableCollection<ScorePropertyInfoItem>();

            Loaded += UCScorePropertyLister_Loaded;
            SizeChanged += UCScorePropertyLister_SizeChanged;
            ListBoxPropertyList.SelectionChanged += ListBoxPropertyList_SelectionChanged;
        }

        void UCScorePropertyLister_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxPropertyList.ItemsSource = mListScorePropertyInfoItems;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBoxPropertyList.ItemsSource);
            if (view != null && view.GroupDescriptions != null)
            {
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            }

            Init();
            ChangeLanguage();
        }


        #region NameWidthProperty

        public static readonly DependencyProperty NameWidthProperty =
            DependencyProperty.Register("NameWidth", typeof(double), typeof(UCScorePropertyLister), new PropertyMetadata(default(double)));

        public double NameWidth
        {
            get { return (double)GetValue(NameWidthProperty); }
            set { SetValue(NameWidthProperty, value); }
        }

        #endregion


        #region ListScorePropertyInfoItems

        public ObservableCollection<ScorePropertyInfoItem> ListScorePropertyInfoItems
        {
            get { return mListScorePropertyInfoItems; }
        }

        #endregion


        #region Members

        public ScoreObject ScoreObject { get; set; }

        private ObservableCollection<ScorePropertyInfoItem> mListScorePropertyInfoItems;

        #endregion


        #region Init and Load

        private void Init()
        {
            //设置名称默认宽度
            double width = ActualWidth;
            if (!double.IsNaN(width))
            {
                NameWidth = width / 2;
            }

            mListScorePropertyInfoItems.Clear();
            if (ScoreObject == null) { return; }
            List<ScoreProperty> listProperties = new List<ScoreProperty>();
            ScoreObject.GetPropertyList(ref listProperties);
            for (int i = 0; i < listProperties.Count; i++)
            {
                var scoreProperty = listProperties[i];
                if ((scoreProperty.Flag & ScorePropertyFlag.Visible) == 0) { continue; }
                ScorePropertyInfoItem item = new ScorePropertyInfoItem();
                item.CurrentApp = CurrentApp;
                item.IsEnabled = true;
                if (ScoreObject is YesNoStandard)
                {
                    var yesNoStandard = ScoreObject as YesNoStandard;
                    if (yesNoStandard.IsAutoStandard)
                    {
                        //自动评分项，除Title,TotalScore外其他属性都不能配置
                        if (scoreProperty.ID != ScoreProperty.PRO_ISALLOWNA
                            && scoreProperty.ID != ScoreProperty.PRO_TOTALSCORE
                            && scoreProperty.ID != ScoreProperty.PRO_DESCRIPTION)
                        {
                            item.IsEnabled = false;
                        }
                    }
                }
                item.ScoreObject = ScoreObject;
                item.PropertyID = scoreProperty.ID;
                item.StrPropertyName = CurrentApp.GetLanguageInfo(string.Format("PRO301{0}", item.PropertyID.ToString("000")),
                    item.PropertyID.ToString());
                item.StrPropertyDescription =
                    CurrentApp.GetLanguageInfo(string.Format("PROD301{0}", item.PropertyID.ToString("000")),
                        item.PropertyID.ToString());
                item.GroupID = scoreProperty.Category;
                item.GroupName = CurrentApp.GetLanguageInfo(string.Format("3101GRP301{0}", item.GroupID.ToString("000")),
                    item.GroupID.ToString());
                item.ObjType = (int)ScoreObject.Type;
                item.ScoreProperty = scoreProperty;
                mListScorePropertyInfoItems.Add(item);
            }

        }

        #endregion


        #region EventHandlers

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                NameWidth = NameWidth + e.HorizontalChange;
            }
            catch { }
        }

        void UCScorePropertyLister_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = ActualWidth;
            if (!double.IsNaN(width))
            {
                NameWidth = width / 2;
            }
        }

        void ListBoxPropertyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ListBoxPropertyList.SelectedItem as ScorePropertyInfoItem;
                if (item != null)
                {
                    PropertyItemChangedEventArgs a = new PropertyItemChangedEventArgs();
                    a.ScoreObject = ScoreObject;
                    a.PropertyItem = item;
                    PropertyListerEventEventArgs args = new PropertyListerEventEventArgs();
                    args.Code = PropertyListerEventEventArgs.CODE_PRO_ITEM_CHANGED;
                    args.ScoreObject = ScoreObject;
                    args.Data = a;
                    OnPropertyListerEvent(args);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void UCScorePropertyEditor_OnPropertyValueChanged(object sender, RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> e)
        {
            try
            {
                var a = e.NewValue;
                if (a == null) { return; }
                PropertyListerEventEventArgs args = new PropertyListerEventEventArgs();
                args.Code = PropertyListerEventEventArgs.CODE_PRO_VALUE_CHANGED;
                args.ScoreObject = ScoreObject;
                args.Data = a;
                OnPropertyListerEvent(args);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region PropertyListerEvent Event

        public static readonly RoutedEvent PropertyListerEventEvent;

        public event RoutedPropertyChangedEventHandler<PropertyListerEventEventArgs> PropertyListerEvent
        {
            add { AddHandler(PropertyListerEventEvent, value); }
            remove { RemoveHandler(PropertyListerEventEvent, value); }
        }

        private void OnPropertyListerEvent(PropertyListerEventEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs> p =
                new RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs>(null, args);
            p.RoutedEvent = PropertyListerEventEvent;
            RaiseEvent(p);
        }

        #endregion


        #region Others

        public void RefreshProperty()
        {
            Init();
        }

        public void RefershProperty(int propertyID)
        {
            var pro = mListScorePropertyInfoItems.FirstOrDefault(p => p.PropertyID == propertyID);
            if (pro != null)
            {
                var editor = pro.Editor;
                if (editor != null)
                {
                    editor.RefreshValue();
                }
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListScorePropertyInfoItems.Count; i++)
                {
                    var item = mListScorePropertyInfoItems[i];
                    item.StrPropertyName =
                        CurrentApp.GetLanguageInfo(string.Format("PRO301{0}", item.PropertyID.ToString("000")),
                            item.PropertyID.ToString());
                    item.StrPropertyDescription =
                        CurrentApp.GetLanguageInfo(string.Format("PROD301{0}", item.PropertyID.ToString("000")),
                            item.PropertyID.ToString());
                    item.GroupName = CurrentApp.GetLanguageInfo(string.Format("3101GRP301{0}", item.GroupID.ToString("000")),
                        item.GroupID.ToString());
                }

                //此操作将触发每个ListBoxItem重新Load，当然ListBoxItem也会切换语言
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBoxPropertyList.ItemsSource);
                if (view != null && view.GroupDescriptions != null)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion


    }
}
