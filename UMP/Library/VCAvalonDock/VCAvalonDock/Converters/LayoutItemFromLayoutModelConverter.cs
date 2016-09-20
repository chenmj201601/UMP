//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6779b3b8-8438-4427-b65c-42d157bd99fc
//        CLR Version:              4.0.30319.18444
//        Name:                     LayoutItemFromLayoutModelConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                LayoutItemFromLayoutModelConverter
//
//        created by Charley at 2014/7/22 9:32:35
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    public class LayoutItemFromLayoutModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var layoutModel = value as LayoutContent;
            if (layoutModel == null)
                return null;
            if (layoutModel.Root == null)
                return null;
            if (layoutModel.Root.Manager == null)
                return null;

            var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel(layoutModel);
            if (layoutItemModel == null)
                return Binding.DoNothing;

            return layoutItemModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
