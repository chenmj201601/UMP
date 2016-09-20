//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b19c95d9-251f-4f7f-a5c6-736be6eaae16
//        CLR Version:              4.0.30319.18444
//        Name:                     RibbonToolBarRow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                RibbonToolBarRow
//
//        created by Charley at 2014/5/27 22:21:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents size definition for group box
    /// </summary>
    [ContentProperty("Children")]
    [SuppressMessage("Microsoft.Naming", "CA1702", Justification = "We mean here 'bar row' instead of 'barrow'")]
    public class RibbonToolBarRow : DependencyObject
    {
        #region Fields

        // User defined rows
        readonly ObservableCollection<DependencyObject> children = new ObservableCollection<DependencyObject>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DependencyObject> Children
        {
            get { return children; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarRow() { }

        #endregion
    }
}
