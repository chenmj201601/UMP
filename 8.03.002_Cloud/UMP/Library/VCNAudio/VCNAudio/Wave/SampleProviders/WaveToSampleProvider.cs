//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    426568fd-2291-4150-96da-df3fd051096e
//        CLR Version:              4.0.30319.34003
//        Name:                     WaveToSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                WaveToSampleProvider
//
//        created by Charley at 2013/12/1 13:36:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Helper class turning an already 32 bit floating point IWaveProvider
    /// into an ISampleProvider - hopefully not needed for most applications
    /// </summary>
    public class WaveToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the WaveToSampleProvider class
        /// </summary>
        /// <param name="source">Source wave provider, must be IEEE float</param>
        public WaveToSampleProvider(IWaveProvider source)
            : base(source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
        }

        /// <summary>
        /// Reads from this provider
        /// </summary>
        public override int Read(float[] buffer, int offset, int count)
        {
            int bytesNeeded = count * 4;
            EnsureSourceBuffer(bytesNeeded);
            int bytesRead = source.Read(sourceBuffer, 0, bytesNeeded);
            int samplesRead = bytesRead / 4;
            int outputIndex = offset;
            for (int n = 0; n < bytesRead; n += 4)
            {
                buffer[outputIndex++] = BitConverter.ToSingle(sourceBuffer, n);
            }
            return samplesRead;
        }
    }
}
