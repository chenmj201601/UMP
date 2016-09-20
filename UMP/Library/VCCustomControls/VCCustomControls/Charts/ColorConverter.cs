using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Random r;
            int index;

            UIElement c = value as UIElement;
            Panel p = (Panel)VisualTreeHelper.GetParent(c);
            if (p == null)
            {
                r = new Random((int)DateTime.Now.Ticks);
            }
            else
            {
                ItemsControl _parent = ((ItemsControl)((FrameworkElement)VisualTreeHelper.GetParent(p)).TemplatedParent);
                index = _parent.ItemContainerGenerator.IndexFromContainer(c);
                r = new Random(index);
            }
            Color color = Color.FromScRgb(1.0f, (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            return new SolidColorBrush(color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
