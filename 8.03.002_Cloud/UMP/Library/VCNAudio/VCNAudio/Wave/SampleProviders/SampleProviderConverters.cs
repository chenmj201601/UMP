//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d3967765-a7f2-4cc1-8614-0e79d7f13a8d
//        CLR Version:              4.0.30319.34003
//        Name:                     SampleProviderConverters
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                SampleProviderConverters
//
//        created by Charley at 2013/12/1 13:33:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Utility class for converting to SampleProvider
    /// </summary>
    static class SampleProviderConverters
    {
        /// <summary>
        /// Helper function to go from IWaveProvider to a SampleProvider
        /// Must already be PCM or IEEE float
        /// </summary>
        /// <param name="waveProvider">The WaveProvider to convert</param>
        /// <returns>A sample provider</returns>
        public static ISampleProvider ConvertWaveProviderIntoSampleProvider(IWaveProvider waveProvider)
        {
            ISampleProvider sampleProvider;
            if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                // go to float
                if (waveProvider.WaveFormat.BitsPerSample == 8)
                {
                    sampleProvider = new Pcm8BitToSampleProvider(waveProvider);
                }
                else if (waveProvider.WaveFormat.BitsPerSample == 16)
                {
                    sampleProvider = new Pcm16BitToSampleProvider(waveProvider);
                }
                else if (waveProvider.WaveFormat.BitsPerSample == 24)
                {
                    sampleProvider = new Pcm24BitToSampleProvider(waveProvider);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported operation");
                }
            }
            else if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                sampleProvider = new WaveToSampleProvider(waveProvider);
            }
            else
            {
                throw new ArgumentException("Unsupported source encoding");
            }
            return sampleProvider;
        }
    }
}
