//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    49bb1a0d-6028-4a74-8f88-a14933d518e4
//        CLR Version:              4.0.30319.18444
//        Name:                     RibbonToolBarControlGroupDefinition
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                RibbonToolBarControlGroupDefinition
//
//        created by Charley at 2014/5/27 22:24:03
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroupDefinition : DependencyObject
    {
        #region Events

        /// <summary>
        /// Occures when children has been changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        #endregion

        #region Fields

        // User defined rows
        readonly ObservableCollection<RibbonToolBarControlDefinition> children = new ObservableCollection<RibbonToolBarControlDefinition>();

        #endregion

        #region Children Property

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarControlDefinition> Children
        {
            get { return children; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarControlGroupDefinition()
        {
            children.CollectionChanged += OnChildrenCollectionChanged;
        }

        void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ChildrenChanged != null) ChildrenChanged(sender, e);
        }

        #endregion
    }
}
