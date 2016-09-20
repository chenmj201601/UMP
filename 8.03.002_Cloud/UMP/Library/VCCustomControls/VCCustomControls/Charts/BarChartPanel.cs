//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    87fd0d90-aa01-40f1-8998-cca7d49a59ee
//        CLR Version:              4.0.30319.42000
//        Name:                     BarChartPanel
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Charts
//        File Name:                BarChartPanel
//
//        created by Charley at 2016/3/24 16:24:04
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class BarChartPanel : Panel
    {
        private ObservableCollection<Point> _childrenPositions;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _childrenPositions = new ObservableCollection<Point>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BarChartPanel v = sender as BarChartPanel;
            if (v == null)
                return;
            ItemCollection oldItems = args.OldValue as ItemCollection;
            ItemCollection newItems = args.NewValue as ItemCollection;
            if (oldItems != null)
                ((INotifyCollectionChanged)oldItems).CollectionChanged -= v.BarChartPanel_CollectionChanged;

            if (args.Property == XValuesProperty)
            {
                if (GetXValues(v) != null)
                    GetXValues(v).CollectionChanged += v.BarChartPanel_CollectionChanged;
            }
            else if (args.Property == YValuesProperty)
            {
                if (GetYValues(v) != null)
                    GetYValues(v).CollectionChanged += v.BarChartPanel_CollectionChanged;
            }
            v.InvalidateArrange();
            v.InvalidateVisual();
        }

        private void BarChartPanel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateArrange();
            InvalidateVisual();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            base.OnRender(dc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = Math.Min(XValues.Count, YValues.Count);
            _childrenPositions.Clear();
            for (int i = 0; i < count; i++)
            {
                double x = (i + 1 >= XValues.Count) ? XValues[i] : XValues[i] + XValues[i + 1];
                Point position = new Point(x / 2, YValues[i]);
                _childrenPositions.Add(position);
                Rect r = new Rect(position.X - InternalChildren[i].DesiredSize.Width / 2,
                  position.Y
                    , InternalChildren[i].DesiredSize.Width, GetHorizontalAxis(this) - position.Y);
                InternalChildren[i].Arrange(r);
            }
            return finalSize;
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BarPenProperty =
            DependencyProperty.Register("BarPen", typeof(Pen), typeof(BarChartPanel), new UIPropertyMetadata(default(Pen)));
        /// <summary>
        /// 
        /// </summary>
        public Pen BarPen
        {
            get { return (Pen)GetValue(BarPenProperty); }
            set { SetValue(BarPenProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BarBrushProperty =
            DependencyProperty.Register("BarBrush", typeof(Brush), typeof(BarChartPanel), new UIPropertyMetadata(default(Brush)));
        /// <summary>
        /// 
        /// </summary>
        public Brush BarBrush
        {
            get { return (Brush)GetValue(BarBrushProperty); }
            set { SetValue(BarBrushProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.Register("XValues", typeof(ObservableCollection<double>), typeof(BarChartPanel), new PropertyMetadata(null, OnValuesChanged));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> XValues
        {
            get { return GetXValues(this); }
            set { SetXValues(this, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ObservableCollection<double> GetXValues(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (ObservableCollection<double>)obj.GetValue(XValuesProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetXValues(DependencyObject obj, ObservableCollection<double> value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(XValuesProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty YValuesProperty =
            DependencyProperty.Register("YValues", typeof(ObservableCollection<double>), typeof(BarChartPanel), new PropertyMetadata(null, OnValuesChanged));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> YValues
        {
            get { return GetYValues(this); }
            set { SetYValues(this, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ObservableCollection<double> GetYValues(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (ObservableCollection<double>)obj.GetValue(YValuesProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetYValues(DependencyObject obj, ObservableCollection<double> value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(YValuesProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register("HorizontalAxis", typeof(double), typeof(BarChartPanel), new PropertyMetadata(0.0));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetHorizontalAxis(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(HorizontalAxisProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHorizontalAxis(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(HorizontalAxisProperty, value);
        }
    }
}
