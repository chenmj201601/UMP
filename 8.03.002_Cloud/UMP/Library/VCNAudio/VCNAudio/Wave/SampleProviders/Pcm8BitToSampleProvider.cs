//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    15739f88-eec5-446d-93ce-d76bdc4e69c7
//        CLR Version:              4.0.30319.34003
//        Name:                     Pcm8BitToSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.SampleProviders
//        File Name:                Pcm8BitToSampleProvider
//
//        created by Charley at 2013/12/1 13:33:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts an IWaveProvider containing 8 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm8BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm8BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm8BitToSampleProvider(IWaveProvider source) :
            base(source)
        {
        }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples to read</param>
        /// <returns>Number of samples read</returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n] / 128f - 1.0f;
            }
            return bytesRead;
        }
    }
}
