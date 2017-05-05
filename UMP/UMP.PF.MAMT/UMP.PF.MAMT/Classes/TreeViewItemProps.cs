using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UMP.PF.MAMT.Classes
{
    public class TreeViewItemProps
    {
        public static readonly DependencyProperty ItemImageNameProperty;

        static TreeViewItemProps()
        {
            ItemImageNameProperty = DependencyProperty.RegisterAttached("ItemImageName", typeof(string), typeof(TreeViewItemProps), new UIPropertyMetadata(string.Empty));
        }
        public static void SetItemImageName(DependencyObject obj, string value)
        {
            obj.SetValue(ItemImageNameProperty, value);
        }
    }
}
