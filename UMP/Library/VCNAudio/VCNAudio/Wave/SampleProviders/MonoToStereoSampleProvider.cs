//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9a62a7f1-06ea-4140-af63-828cdd510015
//        CLR Version:              4.0.30319.34003
//        Name:                     MonoToStereoSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                MonoToStereoSampleProvider
//
//        created by Charley at 2013/12/1 13:38:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{

    /// <summary>
    /// No nonsense mono to stereo provider, no volume adjustment,
    /// just copies input to left and right. 
    /// </summary>
    public class MonoToStereoSampleProvider : ISampleProvider
    {
        private ISampleProvider source;
        private WaveFormat waveFormat;
        private float[] sourceBuffer;

        /// <summary>
        /// Initializes a new instance of MonoToStereoSampleProvider
        /// </summary>
        /// <param name="source">Source sample provider</param>
        public MonoToStereoSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source must be mono");
            }
            this.source = source;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
        }

        /// <summary>
        /// WaveFormat of this provider
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        /// <summary>
        /// Reads samples from this provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int sourceSamplesRequired = count / 2;
            int outIndex = offset;
            EnsureSourceBuffer(sourceSamplesRequired);
            int sourceSamplesRead = source.Read(sourceBuffer, 0, sourceSamplesRequired);
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n];
                buffer[outIndex++] = sourceBuffer[n];
            }
            return sourceSamplesRead * 2;
        }

        private void EnsureSourceBuffer(int count)
        {
            if (sourceBuffer == null || sourceBuffer.Length < count)
            {
                sourceBuffer = new float[count];
            }
        }
    }
}
