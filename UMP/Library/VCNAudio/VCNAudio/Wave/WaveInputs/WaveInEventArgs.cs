//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    84458971-e212-49ab-b1cd-6fa3da897c16
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveInEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveInEventArgs
//
//        created by Charley at 2014/12/8 16:03:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Event Args for WaveInStream event
    /// </summary>
    public class WaveInEventArgs : EventArgs
    {
        private byte[] buffer;
        private int bytes;

        /// <summary>
        /// Creates new WaveInEventArgs
        /// </summary>
        public WaveInEventArgs(byte[] buffer, int bytes)
        {
            this.buffer = buffer;
            this.bytes = bytes;
        }

        /// <summary>
        /// Buffer containing recorded data. Note that it might not be completely
        /// full. <seealso cref="BytesRecorded"/>
        /// </summary>
        public byte[] Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// The number of recorded bytes in Buffer. <seealso cref="Buffer"/>
        /// </summary>
        public int BytesRecorded
        {
            get { return bytes; }
        }
    }
}
