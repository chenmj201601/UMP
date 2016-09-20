using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 表示一个线状图标
    /// </summary>
    public class LineChart : Chart
    {
        static LineChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineChart), new FrameworkPropertyMetadata(typeof(LineChart)));
        }
        /// <summary>
        /// 
        /// </summary>
        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public Brush AreaBrush
        {
            get { return (Brush)GetValue(AreaBrushProperty); }
            set { SetValue(AreaBrushProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AreaBrushProperty =
            DependencyProperty.Register("AreaBrush", typeof(Brush), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public bool IsSmoothOutline
        {
            get { return (bool)GetValue(IsSmoothOutlineProperty); }
            set { SetValue(IsSmoothOutlineProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSmoothOutline.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSmoothOutlineProperty =
            DependencyProperty.Register("IsSmoothOutline", typeof(bool), typeof(LineChart), new UIPropertyMetadata(false));


        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ValueAxisItemTemplate
        {
            get { return (DataTemplate)GetValue(ValueAxisItemTemplateProperty); }
            set { SetValue(ValueAxisItemTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueAxisItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueAxisItemTemplateProperty =
            DependencyProperty.Register("ValueAxisItemTemplate", typeof(DataTemplate), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ValueAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ValueAxisItemTemplateSelectorProperty); }
            set { SetValue(ValueAxisItemTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueAxisItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("ValueAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LabelAxisItemTemplate
        {
            get { return (DataTemplate)GetValue(LabelAxisItemTemplateProperty); }
            set { SetValue(LabelAxisItemTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAxisItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAxisItemTemplateProperty =
            DependencyProperty.Register("LabelAxisItemTemplate", typeof(DataTemplate), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LabelAxisItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LabelAxisItemTemplateSelectorProperty); }
            set { SetValue(LabelAxisItemTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAxisItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAxisItemTemplateSelectorProperty =
            DependencyProperty.Register("LabelAxisItemTemplateSelector", typeof(DataTemplateSelector), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public object ValueAxisTitle
        {
            get { return GetValue(ValueAxisTitleProperty); }
            set { SetValue(ValueAxisTitleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueAxisTitle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleProperty =
            DependencyProperty.Register("ValueAxisTitle", typeof(object), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ValueAxisTitleTemplate
        {
            get { return (DataTemplate)GetValue(ValueAxisTitleTemplateProperty); }
            set { SetValue(ValueAxisTitleTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueAxisTitleTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleTemplateProperty =
            DependencyProperty.Register("ValueAxisTitleTemplate", typeof(DataTemplate), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ValueAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ValueAxisTitleTemplateSelectorProperty); }
            set { SetValue(ValueAxisTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueAxisTitleTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("ValueAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public object LabelAxisTitle
        {
            get { return GetValue(LabelAxisTitleProperty); }
            set { SetValue(LabelAxisTitleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAxisTitle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleProperty =
            DependencyProperty.Register("LabelAxisTitle", typeof(object), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LabelAxisTitleTemplate
        {
            get { return (DataTemplate)GetValue(LabelAxisTitleTemplateProperty); }
            set { SetValue(LabelAxisTitleTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAxisTitleTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleTemplateProperty =
            DependencyProperty.Register("LabelAxisTitleTemplate", typeof(DataTemplate), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LabelAxisTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LabelAxisTitleTemplateSelectorProperty); }
            set { SetValue(LabelAxisTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAxisTitleTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAxisTitleTemplateSelectorProperty =
            DependencyProperty.Register("LabelAxisTitleTemplateSelector", typeof(DataTemplateSelector), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath ValuePath
        {
            get { return (PropertyPath)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ValuePath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(PropertyPath), typeof(LineChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath LabelPath
        {
            get { return (PropertyPath)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(PropertyPath), typeof(LineChart), new UIPropertyMetadata(null));



        /// <summary>
        /// 
        /// </summary>
        public bool ShowValueAxisTicks
        {
            get { return (bool)GetValue(ShowValueAxisTicksProperty); }
            set { SetValue(ShowValueAxisTicksProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowValueAxisTicks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowValueAxisTicksProperty =
            DependencyProperty.Register("ShowValueAxisTicks", typeof(bool), typeof(LineChart), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public bool ShowLabelAxisTicks
        {
            get { return (bool)GetValue(ShowLabelAxisTicksProperty); }
            set { SetValue(ShowLabelAxisTicksProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowLabelAxisTicks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowLabelAxisTicksProperty =
            DependencyProperty.Register("ShowLabelAxisTicks", typeof(bool), typeof(LineChart), new UIPropertyMetadata(null));



        /// <summary>
        /// 
        /// </summary>
        public bool ShowValueAxisReferenceLines
        {
            get { return (bool)GetValue(ShowValueAxisReferenceLinesProperty); }
            set { SetValue(ShowValueAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowValueAxisReferenceLines.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowValueAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowValueAxisReferenceLines", typeof(bool), typeof(LineChart), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public bool ShowLabelAxisReferenceLines
        {
            get { return (bool)GetValue(ShowLabelAxisReferenceLinesProperty); }
            set { SetValue(ShowLabelAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowLabelAxisReferenceLines.  This enables animation, styling, binding, etc...
        /// </summary>

        public static readonly DependencyProperty ShowLabelAxisReferenceLinesProperty =
            DependencyProperty.Register("ShowLabelAxisReferenceLines", typeof(bool), typeof(LineChart), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public Pen ReferenceLinePen
        {
            get { return (Pen)GetValue(ReferenceLinePenProperty); }
            set { SetValue(ReferenceLinePenProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ReferenceLinePen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReferenceLinePenProperty =
            DependencyProperty.Register("ReferenceLinePen", typeof(Pen), typeof(LineChart), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TickLength.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(LineChart), new UIPropertyMetadata(null));



    }
}
