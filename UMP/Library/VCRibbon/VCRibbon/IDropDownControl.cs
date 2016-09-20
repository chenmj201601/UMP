//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7727c126-fe6f-4b6f-bac0-ad7de60d88e4
//        CLR Version:              4.0.30319.18444
//        Name:                     IDropDownControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                IDropDownControl
//
//        created by Charley at 2014/5/27 17:35:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents control that have drop down popup
    /// </summary>
    public interface IDropDownControl
    {
        /// <summary>
        /// Gets drop down popup
        /// </summary>
        Popup DropDownPopup { get; }

        /// <summary>
        /// Gets a value indicating whether control context menu is opened
        /// </summary>
        bool IsContextMenuOpened { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether drop down is opened
        /// </summary>
        bool IsDropDownOpen { get; set; }
    }
}
