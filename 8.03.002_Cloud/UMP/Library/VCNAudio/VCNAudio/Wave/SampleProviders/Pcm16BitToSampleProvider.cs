//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c54e5c90-9b20-4035-aaac-82a6b33c7214
//        CLR Version:              4.0.30319.34003
//        Name:                     Pcm16BitToSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                Pcm16BitToSampleProvider
//
//        created by Charley at 2013/12/1 13:35:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts an IWaveProvider containing 16 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm16BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm16BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm16BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Samples required</param>
        /// <returns>Number of samples read</returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 2;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n += 2)
            {
                buffer[outIndex++] = BitConverter.ToInt16(sourceBuffer, n) / 32768f;
            }
            return bytesRead / 2;
        }
    }
}
