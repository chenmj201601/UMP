//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    370f83c4-809e-43c5-85ba-3af309e2fa63
//        CLR Version:              4.0.30319.18444
//        Name:                     TopMostPopup
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Popups.Implementaion
//        File Name:                TopMostPopup
//
//        created by Charley at 2014/7/17 14:23:57
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// Popup 解决普通Popup遮挡输入法的问题，需要将Topmost属性设为False
    /// </summary>
    [SecuritySafeCritical]
    public class TopMostPopup : Popup
    {
        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(
        typeof(TopMostPopup),
        new FrameworkPropertyMetadata(false, OnTopmostChanged));

        public bool Topmost
        {
            get { return (bool)GetValue(TopmostProperty); }
            set { SetValue(TopmostProperty, value); }
        }

        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as TopMostPopup).UpdateWindow();
        }

        protected override void OnOpened(EventArgs e)
        {
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            RECT rect;

            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion

    }
}
