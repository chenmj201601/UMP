//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    97159522-6ea0-4875-be28-f1d2fc81643c
//        CLR Version:              4.0.30319.18063
//        Name:                     UCDragElement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                UCDragElement
//
//        created by Charley at 2014/4/4 12:19:59
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Windows.Input;
using VoiceCyber.Wpf.CustomControls;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// UCDragElement.xaml 的交互逻辑
    /// </summary>
    public partial class UCDragElement : IDragElement
    {
        public UCDragElement()
        {
            InitializeComponent();
        }

        private void Border_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SubMoveStarted();
        }

        private void Border_OnMouseMove(object sender, MouseEventArgs e)
        {
            SubMoved();
        }

        private void Border_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SubMoveStopped();
        }

        private void UCDragElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            SubMoveStopped();
        }

        private void SubMoveStarted()
        {
            if (MoveStarted != null)
            {
                MoveStarted();
            }
        }

        private void SubMoveStopped()
        {
            if (MoveStopped != null)
            {
                MoveStopped();
            }
        }

        private void SubMoved()
        {
            if (Moved != null)
            {
                Moved();
            }
        }

        #region IDragElement 成员

        public event Action MoveStarted;

        public event Action Moved;

        public event Action MoveStopped;

        #endregion

     
    }
}
