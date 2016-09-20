//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    428aa7af-449e-4ae3-bac8-6716060667d0
//        CLR Version:              4.0.30319.18444
//        Name:                     StoppedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                StoppedEventArgs
//
//        created by Charley at 2014/12/8 16:04:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Stopped Event Args
    /// </summary>
    public class StoppedEventArgs : EventArgs
    {
        private readonly Exception mException;

        /// <summary>
        /// Initializes a new instance of StoppedEventArgs
        /// </summary>
        /// <param name="exception">An exception to report (null if no exception)</param>
        public StoppedEventArgs(Exception exception = null)
        {
            mException = exception;
        }

        /// <summary>
        /// An exception. Will be null if the playback or record operation stopped
        /// </summary>
        public Exception Exception { get { return mException; } }
    }
}
