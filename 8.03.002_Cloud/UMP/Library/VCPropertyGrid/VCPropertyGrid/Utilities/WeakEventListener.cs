//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a5bcc6a4-a232-4780-adeb-39ad7abae336
//        CLR Version:              4.0.30319.18444
//        Name:                     WeakEventListener
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Utilities
//        File Name:                WeakEventListener
//
//        created by Charley at 2014/7/23 15:36:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids.Utilities
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
