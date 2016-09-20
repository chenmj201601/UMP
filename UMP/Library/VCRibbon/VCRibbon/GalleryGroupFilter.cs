//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    86db9c73-d7a7-47c3-96ff-51af9072cbe0
//        CLR Version:              4.0.30319.18444
//        Name:                     GalleryGroupFilter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                GalleryGroupFilter
//
//        created by Charley at 2014/5/27 17:52:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents gallery group filter definition
    /// </summary>
    public class GalleryGroupFilter : DependencyObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets title of filter
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(GalleryGroupFilter), new UIPropertyMetadata("GalleryGroupFilter"));

        /// <summary>
        /// Gets or sets list pf groups splitted by comma
        /// </summary>
        public string Groups
        {
            get { return (string)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContextualGroups.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("ContextualGroups", typeof(string),
            typeof(GalleryGroupFilter), new UIPropertyMetadata(""));

        #endregion
    }
}
