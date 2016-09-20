//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dfe09bae-8844-4c67-a991-61218136f303
//        CLR Version:              4.0.30319.18052
//        Name:                     SampleToWaveProvider16
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                SampleToWaveProvider16
//
//        created by Charley at 2013/12/2 11:03:20
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.NAudio.Utils;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts a sample provider to 16 bit PCM, optionally clipping and adjusting volume along the way
    /// </summary>
    public class SampleToWaveProvider16 : IWaveProvider
    {
        private ISampleProvider sourceProvider;
        private readonly WaveFormat mWaveFormat;
        private volatile float mVolume;
        private float[] mSourceBuffer;

        /// <summary>
        /// Creates a new SampleToWaveProvider16
        /// </summary>
        /// <param name="sourceProvider">the source provider</param>
        public SampleToWaveProvider16(ISampleProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                throw new ApplicationException("Only PCM supported");
            if (sourceProvider.WaveFormat.BitsPerSample != 32)
                throw new ApplicationException("Only 32 bit audio supported");

            mWaveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);

            this.sourceProvider = sourceProvider;
            mVolume = 1.0f;
        }

        /// <summary>
        /// Reads bytes from this wave stream
        /// </summary>
        /// <param name="destBuffer">The destination buffer</param>
        /// <param name="offset">Offset into the destination buffer</param>
        /// <param name="numBytes">Number of bytes read</param>
        /// <returns>Number of bytes read.</returns>
        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int samplesRequired = numBytes / 2;
            mSourceBuffer = BufferHelpers.Ensure(mSourceBuffer, samplesRequired);
            int sourceSamples = sourceProvider.Read(mSourceBuffer, 0, samplesRequired);
            WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

            int destOffset = offset / 2;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                // adjust volume
                float sample32 = mSourceBuffer[sample] * mVolume;
                // clip
                if (sample32 > 1.0f)
                    sample32 = 1.0f;
                if (sample32 < -1.0f)
                    sample32 = -1.0f;
                destWaveBuffer.ShortBuffer[destOffset++] = (short)(sample32 * 32767);
            }

            return sourceSamples * 2;
        }

        /// <summary>
        /// <see cref="IWaveProvider.WaveFormat"/>
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return mWaveFormat; }
        }

        /// <summary>
        /// Volume of this channel. 1.0 = full scale
        /// </summary>
        public float Volume
        {
            get { return mVolume; }
            set { mVolume = value; }
        }
    }
}
