//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0be4d371-f09e-4287-8a28-75b0dbe86aa4
//        CLR Version:              4.0.30319.18444
//        Name:                     WindowControlBackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Core.Converters
//        File Name:                WindowControlBackgroundConverter
//
//        created by Charley at 2014/10/8 11:05:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class WindowControlBackgroundConverter : IMultiValueConverter
  {
    /// <summary>
    /// Used in the WindowContainer Template to calculate the resulting background brush
    /// from the WindowBackground (values[0]) and WindowOpacity (values[1]) propreties.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      Brush backgroundColor = ( Brush )values[ 0 ];
      double opacity = ( double )values[ 1 ];

      if( backgroundColor != null )
      {
        // Do not override any possible opacity value specifically set by the user.
        // Only use WindowOpacity value if the user did not set an opacity first.
        if( backgroundColor.ReadLocalValue( Brush.OpacityProperty ) == System.Windows.DependencyProperty.UnsetValue )
        {
          backgroundColor = backgroundColor.Clone();
          backgroundColor.Opacity = opacity;
        }
      }
      return backgroundColor;
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
