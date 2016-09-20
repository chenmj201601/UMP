//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c293a8cf-5fb9-47f5-bd83-08d2ae54e0d4
//        CLR Version:              4.0.30319.18444
//        Name:                     IWaveProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                IWaveProvider
//
//        created by Charley at 2014/12/8 15:11:09
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Generic interface for all WaveProviders.
    /// </summary>
    public interface IWaveProvider
    {
        /// <summary>
        /// Gets the WaveFormat of this WaveProvider.
        /// </summary>
        /// <value>The wave format.</value>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Fill the specified buffer with wave data.
        /// </summary>
        /// <param name="buffer">The buffer to fill of wave data.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>the number of bytes written to the buffer.</returns>
        int Read(byte[] buffer, int offset, int count);
    }
}
