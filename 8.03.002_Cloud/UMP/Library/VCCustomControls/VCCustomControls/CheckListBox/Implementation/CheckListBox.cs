//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    97c64c7b-dac4-4c76-b89a-ad608c345f58
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckListBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.CheckListBox.Implementation
//        File Name:                CheckListBox
//
//        created by Charley at 2014/7/18 16:22:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.CustomControls.Primitives;

namespace VoiceCyber.Wpf.CustomControls
{
    public class CheckListBox : Selector
    {
        #region Constructors

        static CheckListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckListBox), new FrameworkPropertyMetadata(typeof(CheckListBox)));
        }

        #endregion //Constructors
    }
}
