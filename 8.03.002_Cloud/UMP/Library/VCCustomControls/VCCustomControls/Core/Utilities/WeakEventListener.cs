//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8d24cede-17c6-4c9c-8bbb-cb7e0564b0f5
//        CLR Version:              4.0.30319.18444
//        Name:                     WeakEventListener
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Utilities
//        File Name:                WeakEventListener
//
//        created by Charley at 2014/7/21 11:06:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls.Core.Utilities
{
    internal class WeakEventListener<TArgs> : IWeakEventListener where TArgs : EventArgs
    {
        private Action<object, TArgs> _callback;

        public WeakEventListener(Action<object, TArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _callback = callback;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            _callback(sender, (TArgs)e);
            return true;
        }
    }
}
