//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cafc634c-6133-4a26-8501-7c1b2a51bd0e
//        CLR Version:              4.0.30319.18444
//        Name:                     UnActivePopup
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Popups.Implementaion
//        File Name:                UnActivePopup
//
//        created by Charley at 2014/10/21 11:19:11
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace VoiceCyber.Wpf.CustomControls
{
    public class UnActivePopup:Popup
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);

        static UnActivePopup()
        {
            EventManager.RegisterClassHandler(typeof(UnActivePopup), 
                PreviewGotKeyboardFocusEvent, 
                new KeyboardFocusChangedEventHandler(OnPreviewGotKeyboardFocus), true);
        }

        private static void OnPreviewGotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.NewFocus as TextBoxBase;
            if (textBox != null)
            {
                var hwndSource = PresentationSource.FromVisual(textBox) as HwndSource;
                if (hwndSource != null)
                {
                    SetActiveWindow(hwndSource.Handle);
                }
            }
        }
    }
}
