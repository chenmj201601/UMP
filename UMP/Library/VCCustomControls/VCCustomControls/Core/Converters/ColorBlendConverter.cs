//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9691fdfe-e5be-4e96-aeab-11635b47ac33
//        CLR Version:              4.0.30319.18444
//        Name:                     ColorBlendConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                ColorBlendConverter
//
//        created by Charley at 2014/7/18 10:07:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    /// <summary>
    /// This converter allow to blend two colors into one based on a specified ratio
    /// </summary>
    public class ColorBlendConverter : IValueConverter
    {
        private double _blendedColorRatio = 0;

        /// <summary>
        /// The ratio of the blended color. Must be between 0 and 1.
        /// </summary>
        public double BlendedColorRatio
        {
            get { return _blendedColorRatio; }

            set
            {
                if (value < 0d || value > 1d)
                    throw new ArgumentException("BlendedColorRatio must greater than or equal to 0 and lower than or equal to 1 ");

                _blendedColorRatio = value;
            }
        }

        /// <summary>
        /// The color to blend with the source color
        /// </summary>
        public Color BlendedColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Color))
                return null;

            Color color = (Color)value;
            return new Color()
            {
                A = this.BlendValue(color.A, this.BlendedColor.A),
                R = this.BlendValue(color.R, this.BlendedColor.R),
                G = this.BlendValue(color.G, this.BlendedColor.G),
                B = this.BlendValue(color.B, this.BlendedColor.B)
            };
        }

        private byte BlendValue(byte original, byte blend)
        {
            double blendRatio = this.BlendedColorRatio;
            double sourceRatio = 1 - blendRatio;

            double result = (((double)original) * sourceRatio) + (((double)blend) * blendRatio);
            result = Math.Round(result);
            result = Math.Min(255d, Math.Max(0d, result));
            return System.Convert.ToByte(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
