//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e40f51f-aacc-4f05-9017-559a3d839cd9
//        CLR Version:              4.0.30319.34003
//        Name:                     MeteringSampleProvider
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                MeteringSampleProvider
//
//        created by Charley at 2013/12/1 13:32:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Simple SampleProvider that passes through audio unchanged and raises
    /// an event every n samples with the maximum sample value from the period
    /// for metering purposes
    /// </summary>
    public class MeteringSampleProvider : ISampleProvider
    {
        private ISampleProvider source;

        private float[] maxSamples;
        private int sampleCount;
        private int channels;
        private StreamVolumeEventArgs args;

        /// <summary>
        /// Number of Samples per notification
        /// </summary>
        public int SamplesPerNotification { get; set; }

        /// <summary>
        /// Raised periodically to inform the user of the max volume
        /// </summary>
        public event EventHandler<StreamVolumeEventArgs> StreamVolume;

        /// <summary>
        /// Initialises a new instance of MeteringSampleProvider that raises 10 stream volume
        /// events per second
        /// </summary>
        /// <param name="source">Source sample provider</param>
        public MeteringSampleProvider(ISampleProvider source) :
            this(source, source.WaveFormat.SampleRate / 10)
        {
        }

        /// <summary>
        /// Initialises a new instance of MeteringSampleProvider 
        /// </summary>
        /// <param name="source">source sampler provider</param>
        /// <param name="samplesPerNotification">Number of samples between notifications</param>
        public MeteringSampleProvider(ISampleProvider source, int samplesPerNotification)
        {
            this.source = source;
            channels = source.WaveFormat.Channels;
            maxSamples = new float[channels];
            SamplesPerNotification = samplesPerNotification;
            args = new StreamVolumeEventArgs { MaxSampleValues = maxSamples }; // create objects up front giving GC little to do
        }

        /// <summary>
        /// The WaveFormat of this sample provider
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        /// <summary>
        /// Reads samples from this Sample Provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            // only bother if there is an event listener
            if (StreamVolume != null)
            {
                for (int index = 0; index < samplesRead; index += channels)
                {
                    for (int channel = 0; channel < channels; channel++)
                    {
                        float sampleValue = Math.Abs(buffer[offset + index + channel]);
                        maxSamples[channel] = Math.Max(maxSamples[channel], sampleValue);
                    }
                    sampleCount++;
                    if (sampleCount >= SamplesPerNotification)
                    {
                        StreamVolume(this, args);
                        sampleCount = 0;
                        // n.b. we avoid creating new instances of anything here
                        Array.Clear(maxSamples, 0, channels);
                    }
                }
            }
            return samplesRead;
        }
    }

    /// <summary>
    /// Event args for aggregated stream volume
    /// </summary>
    public class StreamVolumeEventArgs : EventArgs
    {
        /// <summary>
        /// Max sample values array (one for each channel)
        /// </summary>
        public float[] MaxSampleValues { get; set; }
    }
}
