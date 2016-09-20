//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    504297ec-8724-4f60-88bb-83dbc97518a9
//        CLR Version:              4.0.30319.18444
//        Name:                     InputValidationErrorEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Input
//        File Name:                InputValidationErrorEventArgs
//
//        created by Charley at 2014/7/17 16:12:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core.Input
{
    public delegate void InputValidationErrorEventHandler(object sender, InputValidationErrorEventArgs e);

    public class InputValidationErrorEventArgs : EventArgs
    {
        #region Constructors

        public InputValidationErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        #endregion

        #region Exception Property

        public Exception Exception
        {
            get
            {
                return exception;
            }
            private set
            {
                exception = value;
            }
        }

        private Exception exception;

        #endregion

        #region ThrowException Property

        public bool ThrowException
        {
            get
            {
                return _throwException;
            }
            set
            {
                _throwException = value;
            }
        }

        private bool _throwException;

        #endregion
    }
}
