//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d96c5ffd-0257-4cc6-906a-ee012005f35a
//        CLR Version:              4.0.30319.18444
//        Name:                     GrayscaleEffect
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                GrayscaleEffect
//
//        created by Charley at 2014/5/28 11:29:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// An effect that turns the input into shades of a single color.
    /// </summary>
    public class GrayscaleEffect : ShaderEffect
    {
        /// <summary>
        /// Dependency property for Input
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);

        /// <summary>
        /// Dependency property for FilterColor
        /// </summary>
        public static readonly DependencyProperty FilterColorProperty =
            DependencyProperty.Register("FilterColor", typeof(Color), typeof(GrayscaleEffect),
            new UIPropertyMetadata(Color.FromArgb(255, 255, 255, 255), PixelShaderConstantCallback(0)));

        /// <summary>
        /// Default constructor
        /// </summary>
        public GrayscaleEffect()
        {
            PixelShader pixelShader = new PixelShader();
            var prop = DesignerProperties.IsInDesignModeProperty;

            bool isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            if (!isInDesignMode) pixelShader.UriSource = new Uri("/VCRibbon;Component/Themes/Office2010/Effects/Grayscale.ps", UriKind.Relative);
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FilterColorProperty);
        }

        /// <summary>
        /// Impicit input
        /// </summary>
        public Brush Input
        {
            get
            {
                return ((Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        /// <summary>
        /// The color used to tint the input.
        /// </summary>
        public Color FilterColor
        {
            get
            {
                return ((Color)(GetValue(FilterColorProperty)));
            }
            set
            {
                SetValue(FilterColorProperty, value);
            }
        }
    }
}
