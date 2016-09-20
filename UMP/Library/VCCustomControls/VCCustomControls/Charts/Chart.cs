using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 表示一个图标
    /// </summary>
    public class Chart : ItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ChartItem;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ChartItem();
        }
        /// <summary>
        /// 
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(Chart), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TitleTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(Chart), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector TitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TitleTemplateSelectorProperty); }
            set { SetValue(TitleTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TitleTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleTemplateSelectorProperty =
            DependencyProperty.Register("TitleTemplateSelector", typeof(DataTemplateSelector), typeof(Chart), new UIPropertyMetadata(null));
    }
}
