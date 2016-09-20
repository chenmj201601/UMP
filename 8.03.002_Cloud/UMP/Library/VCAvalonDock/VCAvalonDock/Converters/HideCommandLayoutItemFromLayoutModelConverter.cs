//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6f69c8e1-cc48-4495-bc8f-79be9d3d0fe1
//        CLR Version:              4.0.30319.18444
//        Name:                     HideCommandLayoutItemFromLayoutModelConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                HideCommandLayoutItemFromLayoutModelConverter
//
//        created by Charley at 2014/7/22 9:31:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using VoiceCyber.Wpf.AvalonDock.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    public class HideCommandLayoutItemFromLayoutModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //when this converter is called layout could be constructing so many properties here are potentially not valid
            var layoutModel = value as LayoutContent;
            if (layoutModel == null)
                return null;
            if (layoutModel.Root == null)
                return null;
            if (layoutModel.Root.Manager == null)
                return null;

            var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel(layoutModel) as LayoutAnchorableItem;
            if (layoutItemModel == null)
                return Binding.DoNothing;

            return layoutItemModel.HideCommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
