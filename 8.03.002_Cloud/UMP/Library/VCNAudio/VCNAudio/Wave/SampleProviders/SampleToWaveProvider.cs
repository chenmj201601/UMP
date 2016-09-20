//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cd2d225a-3b64-4007-b862-fe5237dee575
//        CLR Version:              4.0.30319.34003
//        Name:                     SampleToWaveProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                SampleToWaveProvider
//
//        created by Charley at 2013/12/1 13:56:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Helper class for when you need to convert back to an IWaveProvider from
    /// an ISampleProvider. Keeps it as IEEE float
    /// </summary>
    public class SampleToWaveProvider : IWaveProvider
    {
        private ISampleProvider source;

        /// <summary>
        /// Initializes a new instance of the WaveProviderFloatToWaveProvider class
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public SampleToWaveProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
            this.source = source;
        }

        /// <summary>
        /// Reads from this provider
        /// </summary>
        public int Read(byte[] buffer, int offset, int count)
        {
            int samplesNeeded = count / 4;
            WaveBuffer wb = new WaveBuffer(buffer);
            int samplesRead = source.Read(wb.FloatBuffer, offset / 4, samplesNeeded);
            return samplesRead * 4;
        }

        /// <summary>
        /// The waveformat of this WaveProvider (same as the source)
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }
    }
}
