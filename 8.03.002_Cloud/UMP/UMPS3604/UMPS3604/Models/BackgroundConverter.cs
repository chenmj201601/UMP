using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UMPS3604.Models
{
    public class BackgroundConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                ListViewItem item = (ListViewItem)value;
                ListView listview = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                int index = listview.ItemContainerGenerator.IndexFromContainer(item);
                if (index % 2 == 1)
                {
                    return Brushes.LightGray;
                }
                else
                {
                    return Brushes.Transparent;
                }
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
