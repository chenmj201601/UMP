//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dd454d5b-a481-4917-815e-4876b6c292dc
//        CLR Version:              4.0.30319.18444
//        Name:                     IWavePlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                IWavePlayer
//
//        created by Charley at 2014/12/8 16:07:27
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Represents the interface to a device that can play a WaveFile
    /// </summary>
    public interface IWavePlayer : IDisposable
    {
        /// <summary>
        /// Begin playback
        /// </summary>
        void Play();

        /// <summary>
        /// Stop playback
        /// </summary>
        void Stop();

        /// <summary>
        /// Pause Playback
        /// </summary>
        void Pause();

        /// <summary>
        /// Initialise playback
        /// </summary>
        /// <param name="waveProvider">The waveprovider to be played</param>
        void Init(IWaveProvider waveProvider);

        /// <summary>
        /// Current playback state
        /// </summary>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// The volume 1.0 is full scale
        /// </summary>
        [Obsolete("Not intending to keep supporting this going forward: set the volume on your input WaveProvider instead")]
        float Volume { get; set; }

        /// <summary>
        /// Indicates that playback has gone into a stopped state due to 
        /// reaching the end of the input stream or an error has been encountered during playback
        /// </summary>
        event EventHandler<StoppedEventArgs> PlaybackStopped;
    }
}
