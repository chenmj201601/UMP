//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bb6ffb33-f1a9-4d01-9f0c-db357922a569
//        CLR Version:              4.0.30319.42000
//        Name:                     BarChart
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Charts
//        File Name:                BarChart
//
//        created by Charley at 2016/3/24 15:47:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 表示一个条状图表
    /// </summary>
    public class BarChart:Chart
    {
        /// <summary>
        /// 
        /// </summary>
        static BarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BarChart), new FrameworkPropertyMetadata(typeof (BarChart)));
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BarPenProperty =
            DependencyProperty.Register("BarPen", typeof(Pen), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public Pen BarPen
        {
            get { return (Pen) GetValue(BarPenProperty); }
            set { SetValue(BarPenProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BarBrushProperty =
            DependencyProperty.Register("BarBrush", typeof(Brush), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public Brush BarBrush
        {
            get { return (Brush) GetValue(BarBrushProperty); }
            set { SetValue(BarBrushProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueAxisItemTemplateProperty =
            DependencyProperty.Register("ValueAxisItemTemplate", typeof(DataTemplate), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ValueAxisItemTemplate
        {
            get { return (DataTemplate) GetValue(ValueAxisItemTemplateProperty); }
            set { SetValue(ValueAxisItemTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("ValueAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarChart), new UIPropertyMetadata(default(DataTemplateSelector)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ValueAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ValueAxisItemTemplateSelectorProperty); }
            set { SetValue(ValueAxisItemTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAxisItemTemplateProperty =
            DependencyProperty.Register("LabelAxisItemTemplate", typeof(DataTemplate), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LabelAxisItemTemplate
        {
            get { return (DataTemplate) GetValue(LabelAxisItemTemplateProperty); }
            set { SetValue(LabelAxisItemTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("LabelAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarChart), new UIPropertyMetadata(default(DataTemplateSelector)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LabelAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(LabelAxisItemTemplateSelectorProperty); }
            set { SetValue(LabelAxisItemTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleProperty =
            DependencyProperty.Register("ValueAxisTitle", typeof(object), typeof(BarChart), new UIPropertyMetadata(default(object)));
        /// <summary>
        /// 
        /// </summary>
        public object ValueAxisTitle
        {
            get { return (object) GetValue(ValueAxisTitleProperty); }
            set { SetValue(ValueAxisTitleProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleTemplateProperty =
            DependencyProperty.Register("ValueAxisTitleTemplate", typeof(DataTemplate), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ValueAxisTitleTemplate
        {
            get { return (DataTemplate) GetValue(ValueAxisTitleTemplateProperty); }
            set { SetValue(ValueAxisTitleTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("ValueAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(BarChart), new UIPropertyMetadata(default(DataTemplateSelector)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ValueAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ValueAxisTitleTemplateSelectorProperty); }
            set { SetValue(ValueAxisTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleProperty =
            DependencyProperty.Register("LabelAxisTitle", typeof(object), typeof(BarChart), new UIPropertyMetadata(default(object)));
        /// <summary>
        /// 
        /// </summary>
        public object LabelAxisTitle
        {
            get { return (object) GetValue(LabelAxisTitleProperty); }
            set { SetValue(LabelAxisTitleProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleTemplateProperty =
            DependencyProperty.Register("LabelAxisTitleTemplate", typeof(DataTemplate), typeof(BarChart), new UIPropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LabelAxisTitleTemplate
        {
            get { return (DataTemplate) GetValue(LabelAxisTitleTemplateProperty); }
            set { SetValue(LabelAxisTitleTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("LabelAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(BarChart), new UIPropertyMetadata(default(DataTemplateSelector)));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LabelAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(LabelAxisTitleTemplateSelectorProperty); }
            set { SetValue(LabelAxisTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(PropertyPath), typeof(BarChart), new UIPropertyMetadata(default(PropertyPath)));
        /// <summary>
        /// 
        /// </summary>
        public PropertyPath ValuePath
        {
            get { return (PropertyPath) GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(PropertyPath), typeof(BarChart), new UIPropertyMetadata(default(PropertyPath)));
        /// <summary>
        /// 
        /// </summary>
        public PropertyPath LabelPath
        {
            get { return (PropertyPath) GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowValueAxisTicksProperty =
            DependencyProperty.Register("ShowValueAxisTicks", typeof(bool), typeof(BarChart), new UIPropertyMetadata(default(bool)));
        /// <summary>
        /// 
        /// </summary>
        public bool ShowValueAxisTicks
        {
            get { return (bool) GetValue(ShowValueAxisTicksProperty); }
            set { SetValue(ShowValueAxisTicksProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowLabelAxisTicksProperty =
            DependencyProperty.Register("ShowLabelAxisTicks", typeof(bool), typeof(BarChart), new UIPropertyMetadata(default(bool)));
        /// <summary>
        /// 
        /// </summary>
        public bool ShowLabelAxisTicks
        {
            get { return (bool) GetValue(ShowLabelAxisTicksProperty); }
            set { SetValue(ShowLabelAxisTicksProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowValueAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowValueAxisReferenceLines", typeof(bool), typeof(BarChart), new UIPropertyMetadata(default(bool)));
        /// <summary>
        /// 
        /// </summary>
        public bool ShowValueAxisReferenceLines
        {
            get { return (bool) GetValue(ShowValueAxisReferenceLinesProperty); }
            set { SetValue(ShowValueAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowLabelAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowLabelAxisReferenceLines", typeof(bool), typeof(BarChart), new UIPropertyMetadata(default(bool)));
        /// <summary>
        /// 
        /// </summary>
        public bool ShowLabelAxisReferenceLines
        {
            get { return (bool) GetValue(ShowLabelAxisReferenceLinesProperty); }
            set { SetValue(ShowLabelAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ReferenceLinePenProperty =
            DependencyProperty.Register("ReferenceLinePen", typeof(Pen), typeof(BarChart), new UIPropertyMetadata(default(Pen)));
        /// <summary>
        /// 
        /// </summary>
        public Pen ReferenceLinePen
        {
            get { return (Pen) GetValue(ReferenceLinePenProperty); }
            set { SetValue(ReferenceLinePenProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(BarChart), new UIPropertyMetadata(default(double)));
        /// <summary>
        /// 
        /// </summary>
        public double TickLength
        {
            get { return (double) GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }
    }
}
