//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8e078b84-6a44-4b92-9a9e-accd7f1b7df1
//        CLR Version:              4.0.30319.18444
//        Name:                     IRibbonControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                IRibbonControl
//
//        created by Charley at 2014/5/27 17:20:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents logical sizes of a ribbon control 
    /// </summary>
    public enum RibbonControlSize
    {
        /// <summary>
        /// Large size of a control
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle size of a control
        /// </summary>
        Middle,
        /// <summary>
        /// Small size of a control
        /// </summary>
        Small
    }

    /// <summary>
    /// Base interface for Fluent controls
    /// </summary>
    public interface IRibbonControl : IKeyTipedControl
    {
        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        object Icon { get; set; }

    }
}
