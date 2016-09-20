//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b31aa6ba-eb79-4e55-b010-cbbf53b2b26c
//        CLR Version:              4.0.30319.18444
//        Name:                     ScorePropertyGrid
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScorePropertyGrid
//
//        created by Charley at 2014/7/28 10:14:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class ScorePropertyGrid : Control
    {
        private ObservableCollection<ScoreProperty> mListProperties;
        private Thumb mThumbNameWidth;
        private ListBox mListBoxGrid;

        static ScorePropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScorePropertyGrid), new FrameworkPropertyMetadata(typeof(ScorePropertyGrid)));
        }

        public ScorePropertyGrid()
        {
            mListProperties = new ObservableCollection<ScoreProperty>();

            this.Loaded += ScorePropertyGrid_Loaded;
        }

        void ScorePropertyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (mListBoxGrid != null)
            {
                mListBoxGrid.ItemsSource = mListProperties;
            }
            var view = (CollectionView)CollectionViewSource.GetDefaultView(mListProperties);
            if (view != null)
            {
                if (view.GroupDescriptions != null && view.GroupDescriptions.Count == 0)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                }
                else
                {
                    if (view.GroupDescriptions != null)
                    {
                        view.GroupDescriptions.Clear();
                    }
                }
            }
            NameColumnWidth = 100;
            if (SelectObject != null)
            {
                mListProperties.Clear();
                List<ScoreProperty> listProperties = new List<ScoreProperty>();
                SelectObject.GetPropertyList(ref listProperties);
                for (int i = 0; i < listProperties.Count; i++)
                {
                    mListProperties.Add(listProperties[i]);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (mThumbNameWidth != null)
            {
                mThumbNameWidth.DragDelta -= ThumbNameWidth_OnDragDelta;
            }
            mThumbNameWidth = GetTemplateChild("PART_Thumb") as Thumb;
            if (mThumbNameWidth != null)
            {
                mThumbNameWidth.DragDelta += ThumbNameWidth_OnDragDelta;
            }
            mListBoxGrid = GetTemplateChild("PART_ListBox") as ListBox;
            if (mListBoxGrid != null)
            {
                
            }
        }

        private void ThumbNameWidth_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            NameColumnWidth = Math.Max(0, NameColumnWidth + e.HorizontalChange);
        }

        public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof(double), typeof(ScorePropertyGrid), new UIPropertyMetadata(150.0, OnNameColumnWidthChanged));
        public double NameColumnWidth
        {
            get
            {
                return (double)GetValue(NameColumnWidthProperty);
            }
            set
            {
                SetValue(NameColumnWidthProperty, value);
            }
        }

        public static readonly DependencyProperty SelectObjectProperty =
            DependencyProperty.Register("SelectObject", typeof(ScoreObject), typeof(ScorePropertyGrid), new PropertyMetadata(default(ScoreObject),OnSelectObjectChanged));

        public ScoreObject SelectObject
        {
            get { return (ScoreObject)GetValue(SelectObjectProperty); }
            set { SetValue(SelectObjectProperty, value); }
        }


        private static void OnNameColumnWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScorePropertyGrid propertyGrid = o as ScorePropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnNameColumnWidthChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnNameColumnWidthChanged(double oldValue, double newValue)
        {
            if (mThumbNameWidth != null)
            {
                ((TranslateTransform)mThumbNameWidth.RenderTransform).X = newValue;
            }
        }

        private static void OnSelectObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScorePropertyGrid propertyGrid = o as ScorePropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectObjectChanged((ScoreObject)e.OldValue, (ScoreObject)e.NewValue);
            }
        }

        private void OnSelectObjectChanged(ScoreObject oldValue, ScoreObject newValue)
        {
            if (newValue != null)
            {
                mListProperties.Clear();
                List<ScoreProperty> listProperties = new List<ScoreProperty>();
                SelectObject.GetPropertyList(ref listProperties);
                for (int i = 0; i < listProperties.Count; i++)
                {
                    mListProperties.Add(listProperties[i]);
                }
            }
        }
    }
}
