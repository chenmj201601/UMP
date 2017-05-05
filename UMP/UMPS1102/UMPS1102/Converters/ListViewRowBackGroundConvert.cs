using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace UMPS1102.Converters
{
    public class ListViewRowBackGroundConvert:IValueConverter
    {

        public object Convert(object value,Type targetType,object paramter,System.Globalization.CultureInfo culture) 
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
            catch (Exception)
            {

                return Brushes.Transparent;
            }
        }


        public object ConvertBack(object value,Type targetType,object parameter,System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
