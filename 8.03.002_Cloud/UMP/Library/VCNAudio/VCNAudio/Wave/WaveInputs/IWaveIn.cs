//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    34bc1b34-e3ed-4aff-8d50-2d9eabdee14e
//        CLR Version:              4.0.30319.18444
//        Name:                     IWaveIn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                IWaveIn
//
//        created by Charley at 2014/12/8 16:03:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Generic interface for wave recording
    /// </summary>
    public interface IWaveIn : IDisposable
    {
        /// <summary>
        /// Recording WaveFormat
        /// </summary>
        WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// Start Recording
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stop Recording
        /// </summary>
        void StopRecording();

        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        event EventHandler<StoppedEventArgs> RecordingStopped;
    }
}
