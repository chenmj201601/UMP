//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c72ae87a-d2ba-4fe8-a2cf-bae9b57fd2a1
//        CLR Version:              4.0.30319.18444
//        Name:                     GalleryGroupIcon
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                GalleryGroupIcon
//
//        created by Charley at 2014/5/27 22:53:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents gallery group icon definition
    /// </summary>
    public class GalleryGroupIcon : DependencyObject
    {
        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string),
            typeof(GalleryGroupIcon), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets group icon
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(GalleryGroupIcon),
                                        new UIPropertyMetadata(null));
    }
}
