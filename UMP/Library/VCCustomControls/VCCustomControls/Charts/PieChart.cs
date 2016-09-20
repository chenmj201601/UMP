using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 表示一个饼状图表
    /// </summary>
    public class PieChart : Chart
    {
        static PieChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(typeof(PieChart)));
        }
        /// <summary>
        /// 
        /// </summary>
        public object ValueTitle
        {
            get { return GetValue(ValueTitleProperty); }
            set { SetValue(ValueTitleProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueTitleProperty =
            DependencyProperty.Register("ValueTitle", typeof(object), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ValueTitleTemplate
        {
            get { return (DataTemplate)GetValue(ValueTitleTemplateProperty); }
            set { SetValue(ValueTitleTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueTitleTemplateProperty =
            DependencyProperty.Register("ValueTitleTemplate", typeof(DataTemplate), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ValueTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ValueTitleTemplateSelectorProperty); }
            set { SetValue(ValueTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueTitleTemplateSelectorProperty =
            DependencyProperty.Register("ValueTitleTemplateSelector", typeof(DataTemplateSelector), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public object LabelTitle
        {
            get { return GetValue(LabelTitleProperty); }
            set { SetValue(LabelTitleProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelTitleProperty =
            DependencyProperty.Register("LabelTitle", typeof(object), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LabelTitleTemplate
        {
            get { return (DataTemplate)GetValue(LabelTitleTemplateProperty); }
            set { SetValue(LabelTitleTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelTitleTemplateProperty =
            DependencyProperty.Register("LabelTitleTemplate", typeof(DataTemplate), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LabelTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LabelTitleTemplateSelectorProperty); }
            set { SetValue(LabelTitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelTitleTemplateSelectorProperty =
            DependencyProperty.Register("LabelTitleTemplateSelector", typeof(DataTemplateSelector), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath ValuePath
        {
            get { return (PropertyPath)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(PropertyPath), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath LabelPath
        {
            get { return (PropertyPath)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(PropertyPath), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LegendItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
            set { SetValue(LegendItemTemplateProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LegendItemTemplateProperty =
            DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(PieChart), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector LegendItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LegendItemTemplateSelectorProperty); }
            set { SetValue(LegendItemTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LegendItemTemplateSelectorProperty =
            DependencyProperty.Register("LegendItemTemplateSelector", typeof(DataTemplate), typeof(PieChart), new UIPropertyMetadata(null));


    }
}
