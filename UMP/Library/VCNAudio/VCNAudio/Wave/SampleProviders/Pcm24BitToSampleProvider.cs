//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7ee268d6-b4da-4c12-9782-e56f49f1dfe9
//        CLR Version:              4.0.30319.34003
//        Name:                     Pcm24BitToSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                Pcm24BitToSampleProvider
//
//        created by Charley at 2013/12/1 13:36:10
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts an IWaveProvider containing 24 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm24BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm24BitToSampleProvider
        /// </summary>
        /// <param name="source">Source Wave Provider</param>
        public Pcm24BitToSampleProvider(IWaveProvider source)
            : base(source)
        {

        }

        /// <summary>
        /// Reads floating point samples from this sample provider
        /// </summary>
        /// <param name="buffer">sample buffer</param>
        /// <param name="offset">offset within sample buffer to write to</param>
        /// <param name="count">number of samples required</param>
        /// <returns>number of samples provided</returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 3;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n += 3)
            {
                buffer[outIndex++] = (((sbyte)sourceBuffer[n + 2] << 16) | (sourceBuffer[n + 1] << 8) | sourceBuffer[n]) / 8388608f;
            }
            return bytesRead / 3;
        }
    }
}
