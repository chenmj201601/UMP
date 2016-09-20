//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    df4cae87-6d14-4279-af23-01e7d9132e2b
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryMoveFocusEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.AutoSelectTextBox.Implementation
//        File Name:                QueryMoveFocusEventArgs
//
//        created by Charley at 2014/7/17 15:09:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace VoiceCyber.Wpf.CustomControls
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void QueryMoveFocusEventHandler(object sender, QueryMoveFocusEventArgs e);

    public class QueryMoveFocusEventArgs : RoutedEventArgs
    {
        //default CTOR private to prevent its usage.
        private QueryMoveFocusEventArgs()
        {
        }

        //internal to prevent anybody from building this type of event.
        internal QueryMoveFocusEventArgs(FocusNavigationDirection direction, bool reachedMaxLength)
            : base(AutoSelectTextBox.QueryMoveFocusEvent)
        {
            m_navigationDirection = direction;
            m_reachedMaxLength = reachedMaxLength;
        }

        public FocusNavigationDirection FocusNavigationDirection
        {
            get
            {
                return m_navigationDirection;
            }
        }

        public bool ReachedMaxLength
        {
            get
            {
                return m_reachedMaxLength;
            }
        }

        public bool CanMoveFocus
        {
            get
            {
                return m_canMove;
            }
            set
            {
                m_canMove = value;
            }
        }

        private FocusNavigationDirection m_navigationDirection;
        private bool m_reachedMaxLength;
        private bool m_canMove = true; //defaults to true... if nobody does nothing, then its capable of moving focus.

    }
}
