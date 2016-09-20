//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b566a373-d68b-435e-8b77-245b3648581e
//        CLR Version:              4.0.30319.18444
//        Name:                     IKeyTipedControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                IKeyTipedControl
//
//        created by Charley at 2014/5/27 17:20:56
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
    /// Base interface for controls supports key tips
    /// </summary>
    public interface IKeyTipedControl
    {
        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        void OnKeyTipPressed();

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        void OnKeyTipBack();
    }
}
