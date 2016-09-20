//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4cbd36cf-0eb5-49ef-b27f-11e38790fb59
//        CLR Version:              4.0.30319.34003
//        Name:                     VolumeSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                VolumeSampleProvider
//
//        created by Charley at 2013/12/1 13:31:10
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Very simple sample provider supporting adjustable gain
    /// </summary>
    public class VolumeSampleProvider : ISampleProvider
    {
        private ISampleProvider source;
        private float volume;

        /// <summary>
        /// Initializes a new instance of VolumeSampleProvider
        /// </summary>
        /// <param name="source">Source Sample Provider</param>
        public VolumeSampleProvider(ISampleProvider source)
        {
            this.source = source;
            volume = 1.0f;
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="sampleCount">Number of samples desired</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            if (volume != 1f)
            {
                for (int n = 0; n < sampleCount; n++)
                {
                    buffer[offset + n] *= volume;
                }
            }
            return samplesRead;
        }

        /// <summary>
        /// Allows adjusting the volume, 1.0f = full volume
        /// </summary>
        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }
}
